using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using GalaxyOfPayouts.Logic.Observable;

namespace GalaxyOfPayouts.Logic.DiscordEvents.EventObservables
{
    public class MessageReceived : IObservable<SocketMessage>
    {
        private readonly List<IObserver<SocketMessage>> _observers;

        public MessageReceived()
        {
            _observers = new List<IObserver<SocketMessage>>();
        }

        public IDisposable Subscribe(IObserver<SocketMessage> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);

            return new Unsubscriber<SocketMessage>(_observers, observer);
        }

        public void ReceiveMessage(SocketMessage message)
        {
            foreach (var observer in _observers)
                observer.OnNext(message);
        }
    }
}
