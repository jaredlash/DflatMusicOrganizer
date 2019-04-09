using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using Unity;

namespace Dflat.ViewModels
{
    public class ViewService : IViewService
    {


        /// <summary>
        /// Maps a ViewModel type to an existing instance of a window view
        /// </summary>
        private readonly Dictionary<Type, IView> currentWindows;

        private readonly IUnityContainer iocContainer;


        public ViewService(IUnityContainer iocContainer)
        {
            this.iocContainer = iocContainer;
            
            currentWindows = new Dictionary<Type, IView>();
        }



        /// <summary>
        /// Shows a View/ViewModel that corresponds to an already registered TViewModel.  DataContext of the new view is set to the view model.
        /// </summary>
        /// <typeparam name="TViewModel">Type of the ViewModel for which this view is being created.</typeparam>
        /// <param name="viewModel">viewModel for which this is being created.</param>
        public void ShowWindow<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
        {
            IView view;
            Type vmType = typeof(TViewModel);

            if (currentWindows.TryGetValue(vmType, out view) == false)
            {
                view = iocContainer.Resolve<IView>(vmType.Name);

                view.DataContext = viewModel;

                currentWindows.Add(vmType, view);

                view.Closed += delegate (object o, EventArgs args) {
                    currentWindows.Remove(vmType);
                };
            }
            
            view.Show();
            view.Activate();
        }
    }
}
