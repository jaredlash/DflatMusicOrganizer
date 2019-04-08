using GalaSoft.MvvmLight;

namespace Dflat.ViewModels
{
    public interface IViewService
    {
        void Register<TViewModel, TView>()
            where TViewModel : ViewModelBase
            where TView : IView;
        void ShowWindow<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase;
    }
}