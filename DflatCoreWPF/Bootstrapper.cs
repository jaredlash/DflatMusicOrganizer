using AutoMapper;
using Caliburn.Micro;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.EFCore.DB;
using Dflat.EFCore.DB.Models;
using Dflat.EFCore.DB.Repositories;
using DflatCoreWPF.Utilities;
using DflatCoreWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace DflatCoreWPF
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            var automapconfig = new MapperConfiguration(cfg =>
            {

                cfg.CreateMap<Dflat.Application.Models.FileSourceFolder, FileSourceFolderEditorViewModel>()
                    .ForMember(dest => dest.SelectedExcludePath, opt => opt.Ignore())
                    .ForMember(dest => dest.IsInitialized, opt => opt.Ignore())
                    .ForMember(dest => dest.Parent, opt => opt.Ignore())
                    .ForMember(dest => dest.DisplayName, opt => opt.Ignore())
                    .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                    .ForMember(dest => dest.IsNotifying, opt => opt.Ignore())
                    .ReverseMap()
                    .DisableCtorValidation();

                //cfg.CreateMap<FileSourceFolderEditorViewModel, FileSourceFolder>();

                cfg.UseEntityFrameworkCoreModel<DataContext>();
                cfg.CreateMap<Dflat.EFCore.DB.Models.FileSourceFolder, Dflat.Application.Models.FileSourceFolder>()
                    .ForMember(dest => dest.IsChanged, opt => opt.Ignore())
                    .ReverseMap();
                cfg.CreateMap<Dflat.EFCore.DB.Models.ExcludePath, Dflat.Application.Models.ExcludePath>().ReverseMap();
            });

            //automapconfig.AssertConfigurationIsValid();

            var mapper = automapconfig.CreateMapper();

            container.Instance(mapper);

            container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .PerRequest<IFolderChooserDialog, FolderChooserDialog>()
                .PerRequest<IFileSourceFolderRepository, FileSourceFolderRepository>();

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => container.RegisterPerRequest(viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
    }
}
