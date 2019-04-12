using Dflat.Business;
using Dflat.Business.Factories;
using Dflat.EF6.DataAccess;
using Dflat.ViewModels;
using Dflat.ViewModels.Dialogs;
using Dflat.ViewModels.DialogViewModels;
using System.Windows;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace DflatWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            

            var container = new UnityContainer();

            container.RegisterType<IUnitOfWorkFactory, UnitOfWorkFactory>();
            container.RegisterType<IViewModelFactory, ViewModelFactory>();
            container.RegisterType<IUowLifetimeManagerFactory, UowLifetimeManagerFactory>();
            container.RegisterType<IUnitOfWorkLifetimeManager, UnitOfWorkLifetimeManager>();

            container.RegisterType<IViewService, ViewService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDialogService, DialogService>();

            
            container.RegisterType<IView, FileSourceManager>(nameof(FileSourceManagerViewModel));

            container.RegisterType<IDialogView, ConfirmDialog>(nameof(ConfirmDialogViewModel));
            container.RegisterType<IDialogView, FileSourceFolderEditor>(nameof(FileSourceFolderEditorViewModel));
            container.RegisterType<IFolderChooserDialog, FolderChooserDialog>();
            
            var viewService = container.Resolve<IViewService>();
            
            var viewModel = container.Resolve<MainWindowViewModel>();

            var view = new MainWindow();

            view.DataContext = viewModel;

            view.Show();
        }


    }


}
