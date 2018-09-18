using System;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Logic.DiscordResponders;
using GalaxyOfPayouts.Logic.DiscordResponders.Behaviors;
using Z.Core.Extensions;

namespace GalaxyOfPayouts.Logic.DiscordEvents.EventObservers
{
    public class NotificationObserver : IObserver<DiscordSocketClient>
    {
        private readonly GOPContext _db;
        private readonly IDisposable _unsubscriber;

        public NotificationObserver(GOPContext db, IObservable<DiscordSocketClient> provider)
        {
            _db = db;

            if (provider.IsNotNull())
                _unsubscriber = provider.Subscribe(this);
        }

        public void OnCompleted()
        {
            //TODO
            Console.WriteLine("Additional messages will not be processed.");
        }

        public void OnError(Exception error)
        {
            //TODO
            throw error;
        }

        public void OnNext(DiscordSocketClient client)
        {
            var responder = new DiscordResponder();
            responder.SetResponseBehavior(new NotificationBehavior(_db, client));
            responder.Respond();
        }

        public virtual void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }
    }
}
