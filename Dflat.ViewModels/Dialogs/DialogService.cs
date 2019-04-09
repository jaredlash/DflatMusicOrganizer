using System;
using System.Collections.Generic;
using Unity;

namespace Dflat.ViewModels.Dialogs
{
    public class DialogService : IDialogService
    {
        private readonly IUnityContainer iocContainer;
        private readonly IViewModelFactory viewModelFactory;

        public DialogService(IViewModelFactory viewModelFactory, IUnityContainer iocContainer)
        {
            this.viewModelFactory = viewModelFactory;
            this.iocContainer = iocContainer;
        }



        public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : DialogViewModelBase
        {
            Type vmType = typeof(TViewModel);

            IDialogView dialog = iocContainer.Resolve<IDialogView>(vmType.Name);

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

        public bool? ConfirmDialog(string title, string message, string confirmButtonText, string denyButtonText)
        {
            //var viewModel = viewModelFactory.CreateConfirmDialogViewModel(title, message, confirmButtonText, denyButtonText);

            return false; // ShowDialog<ConfirmDialogViewModel>(viewModel);
        }
    }
}
