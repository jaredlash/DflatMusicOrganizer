using Dflat.Business.Factories;
using System;
using System.Collections.Generic;
namespace Dflat.ViewModels
{
    public class ViewService : IViewService
    {
        private readonly Dictionary<Type, Type> viewModelMappings;
        private readonly Dictionary<Type, IView> currentViewModels;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IViewModelFactory viewModelFactory;

        public ViewService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            viewModelMappings = new Dictionary<Type, Type>();
            currentViewModels = new Dictionary<Type, IView>();
        }

        public void Register<TViewModel, TView>() where TViewModel : ViewModelBase where TView : IView
        {
            if (viewModelMappings.ContainsKey(typeof(TViewModel)))
            {
                throw new ArgumentException($"Type {typeof(TViewModel)} is already mapped");
            }

            viewModelMappings.Add(typeof(TViewModel), typeof(TView));
        }


        /// <summary>
        /// Shows a View/ViewModel that corresponds to an already registered TViewModel.  DataContext of the new view is set to the view model.
        /// </summary>
        /// <typeparam name="TViewModel">Type of the ViewModel for which this view is being created.</typeparam>
        /// <param name="viewModel">viewModel for which this is being created.</param>
        public void ShowViewModel<TViewModel>() where TViewModel : ViewModelBase
        {
            IView view;

            if (currentViewModels.TryGetValue(typeof(TViewModel), out view) == false)
            {
                Type viewType = viewModelMappings[typeof(TViewModel)];

                IViewModel viewModel = (IViewModel)Activator.CreateInstance(typeof(TViewModel), unitOfWorkFactory.Create());
                view = (IView)Activator.CreateInstance(viewType);

                view.DataContext = viewModel;

                currentViewModels.Add(typeof(TViewModel), view);

                viewModel.OnClose += delegate (object o, EventArgs args) {
                    currentViewModels.Remove(typeof(TViewModel));
                };
            }
            
            view.Show();
            view.Activate();
        }

        
    }
}
