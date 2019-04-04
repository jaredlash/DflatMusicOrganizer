namespace Dflat.ViewModels
{
    public interface IViewService
    {
        void Register<TViewModel, TView>()
            where TViewModel : ViewModelBase
            where TView : IView;
        void ShowViewModel<TViewModel>() where TViewModel : ViewModelBase;
    }
}