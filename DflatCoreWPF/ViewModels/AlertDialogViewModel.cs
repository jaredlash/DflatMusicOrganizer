using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace DflatCoreWPF.ViewModels
{
    public class AlertDialogViewModel : Screen
    {
        public string Title { get; set; }
        public string Message { get; set; }
        
        public void Okay()
        {
            TryCloseAsync(true);
        }
    }
}
