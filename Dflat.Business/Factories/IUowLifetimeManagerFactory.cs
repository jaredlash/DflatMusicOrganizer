namespace Dflat.Business.Factories
{
    public interface IUowLifetimeManagerFactory
    {
        IUnitOfWorkLifetimeManager Create();

        IUnitOfWorkLifetimeManager Create(IUnitOfWork unitOfWork);
    }
}