using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.ViewModels
{
    public interface IView
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        object DataContext { get; set; }

        /// <summary>
        /// Shows the view if currently hidden.
        /// </summary>
        void Show();

        /// <summary>
        /// Closes the view
        /// </summary>
        void Close();

        event EventHandler Closed;

        /// <summary>
        /// Used to bring a window to the foreground.
        /// </summary>
        /// <returns>True if successful.</returns>
        bool Activate();
    }
}
