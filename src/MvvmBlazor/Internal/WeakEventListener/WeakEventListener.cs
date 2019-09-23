using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace MvvmBlazor.Internal.WeakEventListener
{
    internal interface IWeakEventListener
    {
        bool IsAlive { get; }
        object? Source { get; }
        Delegate? Handler { get; }
        void StopListening();
    }

    internal abstract class WeakEventListenerBase<T, TArgs> : IWeakEventListener
        where T : class
        where TArgs : EventArgs
    {
        private readonly WeakReference<Action<T, TArgs>> _handler;
        private readonly WeakReference<T> _source;

        protected WeakEventListenerBase(T source, Action<T, TArgs> handler)
        {
            _source = new WeakReference<T>(source ?? throw new ArgumentNullException(nameof(source)));
            _handler = new WeakReference<Action<T, TArgs>>(handler ?? throw new ArgumentNullException(nameof(handler)));
        }

        public bool IsAlive => _handler.TryGetTarget(out _) && _source.TryGetTarget(out _);

        public object? Source
        {
            get
            {
                if (_source.TryGetTarget(out var source)) return source;
                return null;
            }
        }

        public Delegate? Handler
        {
            get
            {
                if (_handler.TryGetTarget(out var handler)) return handler;
                return null;
            }
        }

        public void StopListening()
        {
            if (_source.TryGetTarget(out var source)) StopListening(source);
        }

        protected void HandleEvent(object sender, TArgs e)
        {
            if (_handler.TryGetTarget(out var handler))
                handler((T) sender, e);
            else
                StopListening();
        }

        protected abstract void StopListening(T source);
    }

    internal class TypedWeakEventListener<T, TArgs> : WeakEventListenerBase<T, TArgs>
        where T : class
        where TArgs : EventArgs
    {
        private readonly Action<T, EventHandler<TArgs>> _unregister;

        public TypedWeakEventListener(T source, Action<T, EventHandler<TArgs>> register,
            Action<T, EventHandler<TArgs>> unregister, Action<T, TArgs> handler)
            : base(source, handler)
        {
            if (register == null) throw new ArgumentNullException(nameof(register));
            _unregister = unregister ?? throw new ArgumentNullException(nameof(unregister));
            register(source, HandleEvent);
        }

        protected override void StopListening(T source)
        {
            _unregister(source, HandleEvent);
        }
    }

    internal class PropertyChangedWeakEventListener<T> : WeakEventListenerBase<T, PropertyChangedEventArgs>
        where T : class, INotifyPropertyChanged
    {
        public PropertyChangedWeakEventListener(T source, Action<T, PropertyChangedEventArgs> handler)
            : base(source, handler)
        {
            source.PropertyChanged += HandleEvent;
        }

        protected override void StopListening(T source)
        {
            source.PropertyChanged -= HandleEvent;
        }
    }

    internal class CollectionChangedWeakEventListener<T> : WeakEventListenerBase<T, NotifyCollectionChangedEventArgs>
        where T : class, INotifyCollectionChanged
    {
        public CollectionChangedWeakEventListener(T source, Action<T, NotifyCollectionChangedEventArgs> handler)
            : base(source, handler)
        {
            source.CollectionChanged += HandleEvent;
        }

        protected override void StopListening(T source)
        {
            source.CollectionChanged -= HandleEvent;
        }
    }

    internal class WeakEventListener<T, TArgs> : WeakEventListenerBase<T, TArgs>
        where T : class
        where TArgs : EventArgs
    {
        private readonly EventInfo _eventInfo;

        public WeakEventListener(T source, string eventName, Action<T, TArgs> handler)
            : base(source, handler)
        {
            _eventInfo = source.GetType().GetEvent(eventName) ??
                         throw new ArgumentException("Unknown Event Name", nameof(eventName));
            if (_eventInfo.EventHandlerType == typeof(EventHandler<TArgs>))
                _eventInfo.AddEventHandler(source, new EventHandler<TArgs>(HandleEvent));
            else //the event type isn't just an EventHandler<> so we have to create the delegate using reflection
                _eventInfo.AddEventHandler(source,
                    Delegate.CreateDelegate(_eventInfo.EventHandlerType, this, nameof(HandleEvent)));
        }

        protected override void StopListening(T source)
        {
            if (_eventInfo.EventHandlerType == typeof(EventHandler<TArgs>))
                _eventInfo.RemoveEventHandler(source, new EventHandler<TArgs>(HandleEvent));
            else
                _eventInfo.RemoveEventHandler(source,
                    Delegate.CreateDelegate(_eventInfo.EventHandlerType, this, nameof(HandleEvent)));
        }
    }
}