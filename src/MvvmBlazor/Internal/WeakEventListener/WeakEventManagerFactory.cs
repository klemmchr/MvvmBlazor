using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmBlazor.Internal.WeakEventListener
{
    internal interface IWeakEventManagerFactory
    {
        IWeakEventManager Create();
    }

    internal class WeakEventManagerFactory : IWeakEventManagerFactory
    {
        public IWeakEventManager Create()
        {
            return new WeakEventManager();
        }
    }
}