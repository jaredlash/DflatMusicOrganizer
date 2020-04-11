using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace DflatCoreWPF.ViewModels
{
    public class ConfirmDialogViewModel : Screen
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string YesText { get; set; }
        public string NoText { get; set; }

        public void Yes()
        {
            TryCloseAsync(true);
        }

        public void No()
        {
            TryCloseAsync(false);
        }
    }
}
