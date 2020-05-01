using AutoMapper;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.Application.Services;
using Dflat.Application.Services.JobServices;
using Dflat.Application.Wrappers;
using Dflat.EFCore.DB;
using Dflat.EFCore.DB.Models;
using Dflat.EFCore.DB.Repositories;
using DflatCoreWPF.Utilities;
using DflatCoreWPF.ViewModels;
using DflatCoreWPF.Views;
using DflatCoreWPF.WindowService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Unity;

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
            

            var viewModel = container.Resolve<MainWindowViewModel>();

            var view = new MainWindowView();

            view.DataContext = viewModel;

            view.Show();
        }


        private void Configure()
        {
            var automapconfig = new MapperConfiguration(cfg =>
            {

                cfg.CreateMap<Dflat.Application.Models.FileSourceFolder, FileSourceFolderEditorViewModel>()
                    .ForMember(dest => dest.SelectedExcludePath, opt => opt.Ignore())
                    .ReverseMap()
                    .DisableCtorValidation();

                //cfg.CreateMap<FileSourceFolderEditorViewModel, FileSourceFolder>();

                cfg.UseEntityFrameworkCoreModel<DataContext>();
                cfg.CreateMap<Dflat.EFCore.DB.Models.FileSourceFolder, Dflat.Application.Models.FileSourceFolder>()
                    .ForMember(dest => dest.IsChanged, opt => opt.Ignore())
                    .ReverseMap();
                cfg.CreateMap<Dflat.EFCore.DB.Models.ExcludePath, Dflat.Application.Models.ExcludePath>().ReverseMap();
                cfg.CreateMap<Dflat.EFCore.DB.Models.FileSourceFolderScanJob, Dflat.Application.Models.FileSourceFolderScanJob>().ReverseMap();
                cfg.CreateMap<FileResult, Dflat.Application.Models.File>()
                    .ForMember(dest => dest.FileID, opt => opt.Ignore())
                    .ForMember(dest => dest.Chromaprint, opt => opt.Ignore())
                    .ForMember(dest => dest.MD5Sum, opt => opt.Ignore());

            });

            //automapconfig.AssertConfigurationIsValid();

            var mapper = automapconfig.CreateMapper();
            container.RegisterInstance(mapper);

            container.RegisterSingleton<IWindowService, WindowService.WindowService>()
                .RegisterSingleton<IJobService<Dflat.Application.Models.FileSourceFolderScanJob>, FileSourceFolderScanService>()
                .RegisterSingleton<JobMonitor>()
                .RegisterType<IFolderChooserDialog, FolderChooserDialog>()
                .RegisterType<IFileSourceFolderRepository, FileSourceFolderRepository>()
                .RegisterType<IFileRepository, FileRepository>()
                .RegisterType<IFolderSearchService, FolderSearchService>()
                .RegisterType<IFileCollectionCompare, FileCollectionCompare>()
                .RegisterType<IJobRepository, JobRepository>()
                .RegisterType<IBackgroundJobRunner<Dflat.Application.Models.FileSourceFolderScanJob>, BackgroundJobRunner<Dflat.Application.Models.FileSourceFolderScanJob>>()
                .RegisterType<ISystemIOWrapper, SystemIOWrapper>();

            // Register View and ViewModel mappings
            container.RegisterType<Window, AlertDialogView>(nameof(AlertDialogViewModel));
            container.RegisterType<Window, ConfirmDialogView>(nameof(ConfirmDialogViewModel));
            container.RegisterType<Window, FileSourceFolderEditorView>(nameof(FileSourceFolderEditorViewModel));
            container.RegisterType<Window, FileSourceManagerView>(nameof(FileSourceManagerViewModel));
            container.RegisterType<Window, JobMonitorView>(nameof(JobMonitorViewModel));
            container.RegisterType<Window, JobDetailView>(nameof(JobDetailViewModel));
            container.RegisterType<Window, MainWindowView>(nameof(MainWindowViewModel));

        }
    }
}
