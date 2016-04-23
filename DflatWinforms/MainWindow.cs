using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DflatApplication.Presentation;

namespace DflatWinforms
{
    public partial class MainWindow : Form, IOrganizerView
    {
        private JobsWindow jobsWindow = null;
        private ArtistReleaseViewFilterDialog artistReleaseViewFilterDialog = null;
        private FolderViewFilterDialog folderViewFilterDialog = null;

        private List<LibraryViewChoice> _libraryViewChoices = null;



        public MainWindow()
        {
            InitializeComponent();

            _libraryViewChoices = new List<LibraryViewChoice> { new LibraryViewChoice("Folders", "folderView"), new LibraryViewChoice("Artists / Releases", "releaseView") };
            cbLibraryView.DataSource = _libraryViewChoices;
            cbLibraryView.DisplayMember = "DisplayName";
            cbLibraryView.ValueMember = "Choice";

            cbLibraryView_SelectionChangeCommitted(null, null);

            
        }

        private void addMusicSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AddMusicSource != null)
                AddMusicSource(this, e);
        }

        private void currentJobsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (jobsWindow == null || jobsWindow.IsDisposed)
            {
                jobsWindow = new JobsWindow();
            }

            jobsWindow.Show();
            jobsWindow.BringToFront();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (cbLibraryView.SelectedValue.ToString() == "releaseView")
            {
                if (artistReleaseViewFilterDialog == null || artistReleaseViewFilterDialog.IsDisposed)
                {
                    artistReleaseViewFilterDialog = new ArtistReleaseViewFilterDialog();
                }

                artistReleaseViewFilterDialog.ShowDialog();
            }

            if (cbLibraryView.SelectedValue.ToString() == "folderView")
            {
                if (folderViewFilterDialog == null || folderViewFilterDialog.IsDisposed)
                {
                    folderViewFilterDialog = new FolderViewFilterDialog();
                }

                folderViewFilterDialog.ShowDialog();
            }

        }

        private void cbLibraryView_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbLibraryView.SelectedValue.ToString() == "releaseView")
            {
                btnFilter.Text = "Filter Artists / Releases";
                tcChildrenViews.SelectTab(0);
            }
            else if (cbLibraryView.SelectedValue.ToString() == "folderView")
            {
                btnFilter.Text = "Filter Folders";
                tcChildrenViews.SelectTab(1);
            }
        }

        public event EventHandler<EventArgs> AddMusicSource;
        
    }
}
