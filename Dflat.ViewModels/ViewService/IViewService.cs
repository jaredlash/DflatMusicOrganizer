using GalaSoft.MvvmLight;

namespace Dflat.ViewModels
{
    public interface IViewService
    {
        
        void ShowWindow<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase;
    }
}