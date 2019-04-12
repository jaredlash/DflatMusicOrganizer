using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.ViewModels
{
    public interface IDialogView
    {
        object DataContext { get; set; }
        bool? DialogResult { get; set; }
        bool Activate();
        void Close();
        bool? ShowDialog();
    }
}
