using Dflat.Business.Factories;
using Dflat.EF6.DataAccess;
using Dflat.ViewModels;
using System.Windows;
using Unity;
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
            container.RegisterType<IViewService, ViewService>(new ContainerControlledLifetimeManager());

            var viewService = container.Resolve<IViewService>();

            viewService.Register<FileSourceManagerViewModel, FileSourceManager>();

            var viewModel = container.Resolve<MainWindowViewModel>();
            var view = new MainWindow();
            view.DataContext = viewModel;

            view.Show();
        }


    }


}
