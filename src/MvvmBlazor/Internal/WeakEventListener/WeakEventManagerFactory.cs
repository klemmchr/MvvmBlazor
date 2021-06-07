namespace MvvmBlazor.Internal.WeakEventListener
{
    public interface IWeakEventManagerFactory
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