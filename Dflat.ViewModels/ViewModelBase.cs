

using System;
using System.Windows.Input;

namespace Dflat.ViewModels
{
    
    public abstract class ViewModelBase : ObservableObject, IViewModel
    {
        public ICommand CloseCommand {
            get
            {
                return new RelayCommand(p => ViewModelClose());
            }
        }

        public event EventHandler OnClose;

        public virtual void ViewModelClose()
        {
            if (OnClose != null) OnClose.Invoke(this, null);
        }

        public ICommand InitializeCommand
        {
            get
            {
                return new RelayCommand(p => ViewModelInitialize());
            }
        }

        public virtual void ViewModelInitialize()
        {

        }
        
    }
}
