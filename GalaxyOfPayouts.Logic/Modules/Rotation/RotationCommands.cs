//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Discord;
//using Discord.Commands;
//using GalaxyOfPayouts.Logic.Utilities;

//namespace GalaxyOfPayouts.Logic.Modules.Rotation
//{
//    public class RotationCommands
//    {
//        private readonly ICommandContext Context;
        
//        public string AddPlayers(string timezoneText, string players)
//        {
//            var response = FindRotation(timezoneText);
//            if (response.Rotation == null)
//                return response.ErrorMessage;

//            if (players == string.Empty)
//                return $"You must supply a player or list of players to add.";

//            var playerList = players.Split(',').ToList();
//            var usersResult = GetDiscordUsers(playerList);
//            if (usersResult.Failure)
//                return usersResult.Message;

//            UpdateStoredUsers(usersResult.Value);
//            AddUsersToRotation(response.Rotation, usersResult.Value);

//            return GetRotation(response.Rotation.Id).ToFormattedMessage();
//        }

//        public string RemovePlayers(string timezoneText, string players)
//        {
//            var response = FindRotation(timezoneText);
//            if (response.Rotation == null)
//                return response.ErrorMessage;

//            if (players == string.Empty)
//                return $"You must supply a player or list of players to remove.";

//            if (response.Rotation.RotationUsers.Count == 0)
//                return $"The rotation for {timezoneText} does not have any players to remove.";

//            var playerList = players.Split(',').ToList();
//            var usersResult = GetDiscordUsers(playerList);
//            if (usersResult.Failure)
//                return usersResult.Message;

//            RemovePlayersFromRotation(response.Rotation, usersResult.Value);

//            return GetRotation(response.Rotation.Id).ToFormattedMessage();
//        }

//        public string ArrangePlayers(string timezoneText, string players)
//        {
//            var response = FindRotation(timezoneText);
//            if (response.Rotation == null)
//                return response.ErrorMessage;

//            if (response.Rotation.RotationUsers.Count == 0)
//                return $"You must add players to this rotation before you can arrange them.";

//            if (players == string.Empty)
//                return $"You must supply a player or list of players in the order they should appear in the ranks.";

//            var playerList = players.Split(',').ToList();
//            var usersResult = GetDiscordUsers(playerList);
//            if (usersResult.Failure)
//                return usersResult.Message;

//            ArrangeUsersInRotation(response.Rotation, usersResult.Value);

//            return GetRotation(response.Rotation.Id).ToFormattedMessage();
//        }

//        #region Helpers

//        private Rotations GetRotation(int rotationId)
//        {
//            using (var db = new GOPContext())
//            {
//                return db.Rotations
//                         .Include(r => r.RotationUsers).ThenInclude(ru => ru.User)
//                         .FirstOrDefault(r => r.Id == rotationId);
//            }
//        }

//        private RotationResponse FindRotation(string timezoneText)
//        {
//            if (timezoneText == string.Empty)
//                return new RotationResponse
//                {
//                    TimezoneValid = false,
//                    ErrorMessage = Responses.InvalidTimeZoneResponse(timezoneText)
//                };

//            var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timezoneText);
//            if (zone == null)
//                return new RotationResponse
//                {
//                    TimezoneValid = false,
//                    ErrorMessage = Responses.InvalidTimeZoneResponse(timezoneText)
//                };

//            Rotations rotation;
//            using (var db = new GOPContext())
//            {
//                rotation = db.Rotations
//                             .Include(r => r.RotationUsers)
//                             .ThenInclude(ru => ru.User)
//                             .FirstOrDefault(r => (ulong)r.ChannelId == Context.Channel.Id &&
//                                                  r.Timezone == timezoneText);
//            }

//            if (rotation == null)
//                return new RotationResponse
//                {
//                    TimezoneValid = true,
//                    Timezone = zone,
//                    ErrorMessage = Responses.UnknownRotationResponse(timezoneText)
//                };

//            return new RotationResponse
//            {
//                TimezoneValid = true,
//                Timezone = zone,
//                Rotation = rotation
//            };
//        }

//        private string GetAll()
//        {
//            var result = string.Empty;

//            using (var db = new GOPContext())
//            {
//                result = db.Rotations
//                           .Include(r => r.RotationUsers).ThenInclude(ru => ru.User)
//                           .Where(r => r.ChannelId == Context.Channel.Id)
//                           .ToList()
//                           .Aggregate(string.Empty,
//                                      (c, r) => c + Environment.NewLine + Environment.NewLine + r.ToFormattedMessage());
//            }

//            if (string.IsNullOrEmpty(result))
//                return $"No rotations saved yet. Use **!rotation help** for instructions on setting them up!";

//            return result;
//        }

//        private UsersResponse GetDiscordUsers(List<string> names)
//        {
//            var result = new UsersResponse();
//            var discordUsers = Context.Guild.GetUsersAsync().Result;

//            foreach (var name in names)
//            {
//                var user = GetDiscordUser(discordUsers, name);
//                if (user == null)
//                {
//                    result = new UsersResponse();
//                    result.Failure = true;
//                    result.Message = Responses.UserNotFoundResponse(name);
//                }
//                result.Value.Add(user);
//            }

//            return result;
//        }

//        private IGuildUser GetDiscordUser(IReadOnlyCollection<IGuildUser> users, string playerName)
//        {
//            playerName = playerName.Trim();

//            foreach (var user in users)
//            {
//                var nickName = user.Nickname?.Trim();
//                if (string.Equals(nickName, playerName, StringComparison.OrdinalIgnoreCase))
//                    return user;

//                var userName = user.Username?.Trim();
//                if (string.Equals(userName, playerName, StringComparison.OrdinalIgnoreCase))
//                    return user;
//            }

//            return null;
//        }

//        private void UpdateStoredUsers(List<IGuildUser> users)
//        {
//            using (var db = new GOPContext())
//            {
//                foreach (var user in users)
//                {
//                    var storedUser = db.Users.FirstOrDefault(u => u.Id == user.Id);
//                    if (storedUser == null)
//                        db.Users.Add(new Users
//                        {
//                            Id = user.Id,
//                            Nickname = user.Nickname,
//                            Username = user.Username,
//                            Mention = user.Mention
//                        });
//                }
//                db.SaveChanges();
//            }
//        }

//        private void AddUsersToRotation(Rotations rotation, List<IGuildUser> users)
//        {
//            var targetRank = rotation.RotationUsers.Count + 1;
//            using (var db = new GOPContext())
//            {
//                foreach (var user in users)
//                {
//                    var savedRank = rotation.RotationUsers.FirstOrDefault(ru => ru.UserId == user.Id);
//                    if (savedRank != null)
//                        continue;

//                    db.RotationUsers.Add(new RotationUsers
//                    {
//                        NextRank = targetRank,
//                        RotationId = rotation.Id,
//                        UserId = user.Id
//                    });
//                    targetRank++;
//                }
//                db.SaveChanges();
//            }
//        }

//        private void ArrangeUsersInRotation(Rotations rotation, List<IGuildUser> users)
//        {
//            using (var db = new GOPContext())
//            {
//                var oldRanks = rotation.RotationUsers.ToList();
//                var newRanks = new List<RotationUsers>();

//                db.RotationUsers.RemoveRange(oldRanks);
//                db.SaveChanges();

//                var incrementor = 0;
//                foreach (var rank in oldRanks)
//                {
//                    var nextRank = users.FindIndex(u => u.Id == rank.UserId);
//                    if (nextRank == -1)
//                    {
//                        nextRank = users.Count + incrementor;
//                        incrementor++;
//                    }

//                    rank.NextRank = nextRank + 1;
//                    newRanks.Add(rank);
//                }

//                db.RotationUsers.AddRange(newRanks);
//                db.SaveChanges();
//            }
//        }

//        private void RemovePlayersFromRotation(Rotations rotation, List<IGuildUser> users)
//        {
//            using (var db = new GOPContext())
//            {
//                var ranksToShift = 0;
//                foreach (var rank in rotation.RotationUsers)
//                {
//                    var remove = users.FindIndex(u => u.Id == rank.UserId) > -1;
//                    if (remove)
//                    {
//                        db.RotationUsers.Remove(rank);
//                        ranksToShift++;
//                        continue;
//                    }
//                    rank.NextRank = rank.NextRank - ranksToShift;
//                }
//                db.SaveChanges();
//            }
//        }

//        #endregion

//        public RotationCommands(ICommandContext context)
//        {
//            Context = context;
//        }
//    }
//}