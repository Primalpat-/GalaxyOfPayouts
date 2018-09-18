using System;
using System.Collections.Generic;
using System.Text;
using Z.Core.Extensions;

namespace GalaxyOfPayouts.Logic.Observable
{
    public class Unsubscriber<T> : IDisposable
    {
        private readonly List<IObserver<T>> _observers;
        private readonly IObserver<T> _observer;

        public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observer.IsNotNull() && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
