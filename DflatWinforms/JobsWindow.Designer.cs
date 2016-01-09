namespace DflatWinforms
{
    partial class JobsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listView1 = new System.Windows.Forms.ListView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.displayOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showRunningJobsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showErroredJobsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showWaitingJobsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Location = new System.Drawing.Point(0, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(443, 349);
            this.panel1.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(443, 349);
            this.splitContainer1.SplitterDistance = 213;
            this.splitContainer1.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Location = new System.Drawing.Point(1, 43);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(206, 294);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayOptionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(96, 16);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(108, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // displayOptionsToolStripMenuItem
            // 
            this.displayOptionsToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.displayOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showRunningJobsToolStripMenuItem,
            this.showErroredJobsToolStripMenuItem,
            this.showWaitingJobsToolStripMenuItem});
            this.displayOptionsToolStripMenuItem.Name = "displayOptionsToolStripMenuItem";
            this.displayOptionsToolStripMenuItem.Size = new System.Drawing.Size(100, 20);
            this.displayOptionsToolStripMenuItem.Text = "Display options";
            // 
            // showRunningJobsToolStripMenuItem
            // 
            this.showRunningJobsToolStripMenuItem.Checked = true;
            this.showRunningJobsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showRunningJobsToolStripMenuItem.Name = "showRunningJobsToolStripMenuItem";
            this.showRunningJobsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.showRunningJobsToolStripMenuItem.Text = "Show running jobs";
            // 
            // showErroredJobsToolStripMenuItem
            // 
            this.showErroredJobsToolStripMenuItem.Checked = true;
            this.showErroredJobsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showErroredJobsToolStripMenuItem.Name = "showErroredJobsToolStripMenuItem";
            this.showErroredJobsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.showErroredJobsToolStripMenuItem.Text = "Show errored jobs";
            // 
            // showWaitingJobsToolStripMenuItem
            // 
            this.showWaitingJobsToolStripMenuItem.Checked = true;
            this.showWaitingJobsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showWaitingJobsToolStripMenuItem.Name = "showWaitingJobsToolStripMenuItem";
            this.showWaitingJobsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.showWaitingJobsToolStripMenuItem.Text = "Show waiting jobs";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Controls.Add(this.menuStrip1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(207, 343);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current Jobs";
            // 
            // JobsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 399);
            this.Controls.Add(this.panel1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "JobsWindow";
            this.Text = "Jobs";
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem displayOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showRunningJobsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showErroredJobsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showWaitingJobsToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}