namespace Dflat.Business
{
    public interface IUnitOfWorkLifetimeManager
    {
        IUnitOfWork UnitOfWork { get; }

        void Dispose();
    }
}