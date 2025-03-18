using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;

namespace DflatCoreWPF.ViewModels
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        public Action<bool?> CloseAction { get; set; }


        public void TryClose()
        {
            CloseAction?.Invoke(true);
        }

        public void TryClose(bool? result)
        {
            CloseAction?.Invoke(result);
        }

        public virtual void OnClose()
        {

        }
    }
}
