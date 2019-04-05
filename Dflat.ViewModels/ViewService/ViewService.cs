using Dflat.Business.Factories;
using System;
using System.Collections.Generic;
namespace Dflat.ViewModels
{
    public class ViewService : IViewService
    {
        /// <summary>
        /// Maps a ViewModel type to a Window View
        /// </summary>
        private readonly Dictionary<Type, Type> viewModelToWindowMappings;

        /// <summary>
        /// Maps a ViewModel type to a Dialog View
        /// </summary>
        private readonly Dictionary<Type, Type> viewModelToDialogMappings;

        /// <summary>
        /// Maps a ViewModel type to an existing instance of a window view
        /// </summary>
        private readonly Dictionary<Type, IView> currentWindows;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public ViewService(IUnitOfWorkFactory unitOfWorkFactory, object viewParent)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            viewModelToWindowMappings = new Dictionary<Type, Type>();
            currentWindows = new Dictionary<Type, IView>();

            viewModelToDialogMappings = new Dictionary<Type, Type>();
        }

        public void Register<TViewModel, TView>() where TViewModel : ViewModelBase where TView : IView
        {
            if (viewModelToWindowMappings.ContainsKey(typeof(TViewModel)))
            {
                throw new ArgumentException($"Type {typeof(TViewModel)} is already mapped");
            }

            viewModelToWindowMappings.Add(typeof(TViewModel), typeof(TView));
        }


        /// <summary>
        /// Registers a ViewModel and DialogView combination.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel Type to register</typeparam>
        /// <typeparam name="TDialogView">Corresponding DialogView type to register</typeparam>
        public void RegisterDialog<TViewModel, TDialogView>() where TViewModel : DialogViewModelBase
                                                              where TDialogView : IDialogView
        {
            if (viewModelToDialogMappings.ContainsKey(typeof(TViewModel)))
            {
                throw new ArgumentException($"Type {typeof(TViewModel)} is already mapped");
            }

            viewModelToDialogMappings.Add(typeof(TViewModel), typeof(TDialogView));
        }


        /// <summary>
        /// Shows a View/ViewModel that corresponds to an already registered TViewModel.  DataContext of the new view is set to the view model.
        /// </summary>
        /// <typeparam name="TViewModel">Type of the ViewModel for which this view is being created.</typeparam>
        /// <param name="viewModel">viewModel for which this is being created.</param>
        public void ShowWindow<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
        {
            IView view;

            if (currentWindows.TryGetValue(typeof(TViewModel), out view) == false)
            {
                Type viewType = viewModelToWindowMappings[typeof(TViewModel)];

                view = (IView)Activator.CreateInstance(viewType);

                view.DataContext = viewModel;

                currentWindows.Add(typeof(TViewModel), view);

                viewModel.OnClose += delegate (object o, EventArgs args) {
                    currentWindows.Remove(typeof(TViewModel));
                };
            }
            
            view.Show();
            view.Activate();
        }

        
        public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : DialogViewModelBase
        {
            Type viewType = viewModelToDialogMappings[typeof(TViewModel)];

            IDialogView dialog = (IDialogView)Activator.CreateInstance(viewType);

            EventHandler<DialogCloseRequestedEventArgs> handler = null;

            handler = (sender, e) =>
            {
                viewModel.CloseRequested -= handler;

                if (e.DialogResult.HasValue)
                {
                    dialog.DialogResult = e.DialogResult;
                }
                else
                {
                    dialog.Close();
                }
            };

            viewModel.CloseRequested += handler;

            dialog.DataContext = viewModel;

            return dialog.ShowDialog();
        }
    }
}
