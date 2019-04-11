using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.ViewModels.Dialogs
{
    public interface IFolderChooserDialog : IDialogView
    {
        string Title { get; set; }
        string InitialFolder { get; set; }

        string ResultFolder { get; set; }
    }
}
