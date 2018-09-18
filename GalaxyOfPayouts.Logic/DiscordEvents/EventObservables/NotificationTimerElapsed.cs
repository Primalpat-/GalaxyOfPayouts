using System;
using System.Collections.Generic;
using Discord.WebSocket;
using GalaxyOfPayouts.Logic.Observable;

namespace GalaxyOfPayouts.Logic.DiscordEvents.EventObservables
{
    public class NotificationTimerElapsed : IObservable<DiscordSocketClient>
    {
        private readonly List<IObserver<DiscordSocketClient>> _observers;

        public NotificationTimerElapsed()
        {
            _observers = new List<IObserver<DiscordSocketClient>>();
        }

        public IDisposable Subscribe(IObserver<DiscordSocketClient> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);

            return new Unsubscriber<DiscordSocketClient>(_observers, observer);
        }

        public void HandleNotification(DiscordSocketClient client)
        {
            foreach (var observer in _observers)
                observer.OnNext(client);
        }
    }
}