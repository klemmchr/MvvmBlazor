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