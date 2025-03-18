using System;
using System.Windows.Forms;

namespace DflatCoreWPF.Utilities
{
    public class FolderChooserDialog : IFolderChooserDialog
    {

        public string InitialFolder { get; set; }

        public string ResultFolder { get; set; }

        public string Title { get; set; }



        public bool? ShowDialog()
        {
            var dialog = new FolderBrowserDialog
            {
                Description = Title,
                UseDescriptionForTitle = true,
                SelectedPath = InitialFolder,

                ShowNewFolderButton = false
            };


            // Process open file dialog box results 
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ResultFolder = dialog.SelectedPath;

                return true;
            }

            return false;
        }

    }
}
