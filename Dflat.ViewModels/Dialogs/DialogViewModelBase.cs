using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dflat.ViewModels
{
    public abstract class DialogViewModelBase : ViewModelBase, IDialogRequestClose
    {
        public DialogViewModelBase(string title, string message)
        {
            Title = title;
            Message = message;
            OkCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true)));
            CancelCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false)));
        }
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        public string Title { get; }
        public string Message { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
    }
}
