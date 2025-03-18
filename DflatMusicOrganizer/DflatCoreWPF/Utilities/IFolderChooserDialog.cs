using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DflatCoreWPF.Utilities
{
    public interface IFolderChooserDialog
    {
        string Title { get; set; }
        string InitialFolder { get; set; }

        bool? ShowDialog();
        string ResultFolder { get; set; }
    }
}
