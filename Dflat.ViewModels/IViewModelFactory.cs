using Dflat.Business;
using GalaSoft.MvvmLight;

namespace Dflat.ViewModels
{
    public interface IViewModelFactory
    {
        T Create<T>() where T : ViewModelBase;

        FileSourceManagerViewModel CreateFileSourceManagerViewModel(IUnitOfWorkLifetimeManager uowLifetimeManager);
    }
}