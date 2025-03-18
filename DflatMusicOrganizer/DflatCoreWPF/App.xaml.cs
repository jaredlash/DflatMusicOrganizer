using AutoMapper;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.Application.Services;
using Dflat.Application.Services.JobServices;
using Dflat.Application.Wrappers;
using Dflat.Data.Dapper.Repositories;
using DflatCoreWPF.Utilities;
using DflatCoreWPF.ViewModels;
using DflatCoreWPF.Views;
using DflatCoreWPF.WindowService;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.IO;
using System.Windows;
using Unity;
using Unity.Injection;

namespace DflatCoreWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private UnityContainer container;

        protected override void OnStartup(StartupEventArgs e)
        {

            container = new UnityContainer();

            Configure();
            

            var mainWindowViewModel = container.Resolve<MainWindowViewModel>();
            var windowService = container.Resolve<IWindowService>();

            windowService.ShowWindow(mainWindowViewModel);
        }


        private void Configure()
        {
            var automapconfig = new MapperConfiguration(cfg =>
            {

                cfg.CreateMap<Dflat.Application.Models.FileSourceFolder, FileSourceFolderEditorViewModel>()
                    .ForMember(dest => dest.SelectedExcludePath, opt => opt.Ignore())
                    .ReverseMap()
                    .DisableCtorValidation();

                cfg.CreateMap<FileResult, Dflat.Application.Models.File>()
                    .ForMember(dest => dest.FileID, opt => opt.Ignore())
                    .ForMember(dest => dest.MD5Sum, opt => opt.MapFrom((src) => string.Empty));

            });

            //automapconfig.AssertConfigurationIsValid();


            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();

            //string connectionString = config.GetConnectionString("DflatMusicOrganizerDB");
            string connectionString = config.GetConnectionString("DflatDB");


            var mapper = automapconfig.CreateMapper();
            container.RegisterInstance(mapper);

            container.RegisterSingleton<IWindowService, WindowService.WindowService>()
                .RegisterSingleton<IJobService<FileSourceFolderScanJob>, FileSourceFolderScanService>()
                .RegisterSingleton<IJobService<MD5Job>, MD5Service>()
                .RegisterSingleton<JobMonitor>()
                .RegisterType<IFolderChooserDialog, FolderChooserDialog>()
                .RegisterType<IFolderSearchService, FolderSearchService>()
                .RegisterType<IFileCollectionCompare, FileCollectionCompare>()
                .RegisterType<IBackgroundJobRunner<FileSourceFolderScanJob>, BackgroundJobRunner<FileSourceFolderScanJob>>()
                .RegisterType<IBackgroundJobRunner<MD5Job>, BackgroundJobRunner<MD5Job>>()
                .RegisterType<ISystemIOWrapper, SystemIOWrapper>();

            container.RegisterInstance("connectionString", connectionString);


            container.RegisterType<IJobRepository, JobRepository>(new InjectionConstructor(new ResolvedParameter<string>("connectionString")));
            container.RegisterType<IFileSourceFolderRepository, FileSourceFolderRepository>(new InjectionConstructor(new ResolvedParameter<string>("connectionString")));
            container.RegisterType<IFileRepository, FileRepository>(new InjectionConstructor(new ResolvedParameter<string>("connectionString")));

            // Register View and ViewModel mappings
            container.RegisterType<Window, AlertDialogView>(nameof(AlertDialogViewModel));
            container.RegisterType<Window, ConfirmDialogView>(nameof(ConfirmDialogViewModel));
            container.RegisterType<Window, ConfirmShutdownView>(nameof(ConfirmShutdownViewModel));
            container.RegisterType<Window, FileSourceFolderEditorView>(nameof(FileSourceFolderEditorViewModel));
            container.RegisterType<Window, FileSourceManagerView>(nameof(FileSourceManagerViewModel));
            container.RegisterType<Window, JobMonitorView>(nameof(JobMonitorViewModel));
            container.RegisterType<Window, JobDetailView>(nameof(JobDetailViewModel));
            container.RegisterType<Window, MainWindowView>(nameof(MainWindowViewModel));

        }
    }
}
