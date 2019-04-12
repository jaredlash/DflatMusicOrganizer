using Dflat.ViewModels.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
namespace DflatWPF
{
    public class FolderChooserDialog : IFolderChooserDialog
    {


        #region Unused members to implement IFolderChooserDialog interface

        public object DataContext { get; set; }

        public bool? DialogResult { get; set; }

        public bool Activate() { return true; }

        #endregion


        public string InitialFolder { get; set; }

        public string ResultFolder { get; set; }

        public string Title { get; set; }

        

        public void Close()
        {
            
        }

        public bool? ShowDialog()
        {
            var dialog = new CommonOpenFileDialog();

            dialog.IsFolderPicker = true;

            dialog.Title = Title;
            dialog.InitialDirectory = InitialFolder;

            dialog.Multiselect = false;


            // Process open file dialog box results 
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ResultFolder = dialog.FileName;

                return true;
            }

            return false;
        }
    }
}
