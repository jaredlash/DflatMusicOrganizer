using Dflat.Business;
using Dflat.Business.Factories;
using Unity;
using Unity.Resolution;
using Dflat.Business.Models;
using Dflat.ViewModels.DialogViewModels;

namespace Dflat.ViewModels
{
    public class ViewModelFactory : IViewModelFactory
    {
        
        private readonly IUnityContainer iocContainer;

        public ViewModelFactory(IUnitOfWorkFactory unitOfWorkFactory, IUnityContainer iocContainer)
        {
            this.iocContainer = iocContainer;
        }

        public FileSourceManagerViewModel CreateFileSourceManagerViewModel()
        {
            return iocContainer.Resolve<FileSourceManagerViewModel>();
        }

        public ConfirmDialogViewModel CreateConfirmDialogViewModel(string title, string message, string confirmButtonText, string denyButtonText)
        {
            return iocContainer.Resolve<ConfirmDialogViewModel>(new ResolverOverride[] {
                new ParameterOverride("title", title),
                new ParameterOverride("message", message),
                new ParameterOverride("confirmButtonText", confirmButtonText),
                new ParameterOverride("denyButtonText", denyButtonText) });
        }

        public FileSourceFolderEditorViewModel CreateFileSourceFolderEditorViewModel(IUnitOfWorkLifetimeManager uowLifetimeManager, FileSourceFolder fileSourceFolder, FileSourceFolderEditorMode mode)
        {
            return iocContainer.Resolve<FileSourceFolderEditorViewModel>(new ResolverOverride[] {
                new ParameterOverride("uowLifetimeManager", uowLifetimeManager),
                new ParameterOverride("fileSourceFolder", fileSourceFolder),
                new ParameterOverride("mode", mode) });
        }
    }
}
