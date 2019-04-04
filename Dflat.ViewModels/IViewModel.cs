using System;
using System.Windows.Input;

namespace Dflat.ViewModels
{
    public interface IViewModel
    {
        ICommand CloseCommand { get; }

        void ViewModelClose();
        
        event EventHandler OnClose;
    }
}