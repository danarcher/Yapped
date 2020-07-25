namespace Yapped
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.ofdRegulation = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exploreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripComboBoxGame = new System.Windows.Forms.ToolStripComboBox();
            this.hideUnusedParamsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gotoRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.verifyDeletionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.fbdExport = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ofdRegulation
            // 
            this.ofdRegulation.FileName = "gameparam.parambnd.dcx";
            this.ofdRegulation.Filter = "Regulation or parambnd|*";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.updateToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(760, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.restoreToolStripMenuItem,
            this.exploreToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.toolStripSeparator2,
            this.toolStripComboBoxGame,
            this.hideUnusedParamsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.ToolTipText = "Browse for a regulation file to edit.";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.ToolTipText = "Save changes to the regulation file.";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.ToolTipText = "Restore the regulation file backup.";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.RestoreToolStripMenuItem_Click);
            // 
            // exploreToolStripMenuItem
            // 
            this.exploreToolStripMenuItem.Name = "exploreToolStripMenuItem";
            this.exploreToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exploreToolStripMenuItem.Text = "Explore";
            this.exploreToolStripMenuItem.ToolTipText = "Open the regulation file directory in Explorer.";
            this.exploreToolStripMenuItem.Click += new System.EventHandler(this.ExploreToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.ToolTipText = "Export an encrypted Data0.bdt to decrypted parambnds.";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.ExportToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(181, 6);
            // 
            // toolStripComboBoxGame
            // 
            this.toolStripComboBoxGame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxGame.Name = "toolStripComboBoxGame";
            this.toolStripComboBoxGame.Size = new System.Drawing.Size(121, 23);
            // 
            // hideUnusedParamsToolStripMenuItem
            // 
            this.hideUnusedParamsToolStripMenuItem.Checked = true;
            this.hideUnusedParamsToolStripMenuItem.CheckOnClick = true;
            this.hideUnusedParamsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideUnusedParamsToolStripMenuItem.Name = "hideUnusedParamsToolStripMenuItem";
            this.hideUnusedParamsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.hideUnusedParamsToolStripMenuItem.Text = "Hide Unused Params";
            this.hideUnusedParamsToolStripMenuItem.ToolTipText = "When checked, unused params will not be loaded";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addRowToolStripMenuItem,
            this.duplicateRowToolStripMenuItem,
            this.deleteRowToolStripMenuItem,
            this.findRowToolStripMenuItem,
            this.findNextRowToolStripMenuItem,
            this.gotoRowToolStripMenuItem,
            this.findFieldToolStripMenuItem,
            this.findNextFieldToolStripMenuItem,
            this.importNamesToolStripMenuItem,
            this.exportNamesToolStripMenuItem,
            this.toolStripSeparator1,
            this.verifyDeletionsToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // addRowToolStripMenuItem
            // 
            this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
            this.addRowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.addRowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.addRowToolStripMenuItem.Text = "Add Row";
            this.addRowToolStripMenuItem.ToolTipText = "Add a new row to the active param.";
            this.addRowToolStripMenuItem.Click += new System.EventHandler(this.AddRowToolStripMenuItem_Click);
            // 
            // duplicateRowToolStripMenuItem
            // 
            this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
            this.duplicateRowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.N)));
            this.duplicateRowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.duplicateRowToolStripMenuItem.Text = "Duplicate Row";
            this.duplicateRowToolStripMenuItem.ToolTipText = "Create a new row with values identical to the selected one";
            this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.DuplicateRowToolStripMenuItem_Click);
            // 
            // deleteRowToolStripMenuItem
            // 
            this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
            this.deleteRowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.deleteRowToolStripMenuItem.Text = "Delete Row";
            this.deleteRowToolStripMenuItem.ToolTipText = "Delete the currently selected row";
            this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.DeleteRowToolStripMenuItem_Click);
            // 
            // findRowToolStripMenuItem
            // 
            this.findRowToolStripMenuItem.Name = "findRowToolStripMenuItem";
            this.findRowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findRowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.findRowToolStripMenuItem.Text = "&Find Row";
            this.findRowToolStripMenuItem.ToolTipText = "Search for a row with a matching name";
            this.findRowToolStripMenuItem.Click += new System.EventHandler(this.FindRowToolStripMenuItem_Click);
            // 
            // findNextRowToolStripMenuItem
            // 
            this.findNextRowToolStripMenuItem.Name = "findNextRowToolStripMenuItem";
            this.findNextRowToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextRowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.findNextRowToolStripMenuItem.Text = "Find Next Row";
            this.findNextRowToolStripMenuItem.ToolTipText = "Search again with the previous pattern";
            this.findNextRowToolStripMenuItem.Click += new System.EventHandler(this.FindNextRowToolStripMenuItem_Click);
            // 
            // gotoRowToolStripMenuItem
            // 
            this.gotoRowToolStripMenuItem.Name = "gotoRowToolStripMenuItem";
            this.gotoRowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.gotoRowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.gotoRowToolStripMenuItem.Text = "&Goto Row";
            this.gotoRowToolStripMenuItem.ToolTipText = "Go to a row with a certain ID";
            this.gotoRowToolStripMenuItem.Click += new System.EventHandler(this.GotoRowToolStripMenuItem_Click);
            // 
            // findFieldToolStripMenuItem
            // 
            this.findFieldToolStripMenuItem.Name = "findFieldToolStripMenuItem";
            this.findFieldToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
            this.findFieldToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.findFieldToolStripMenuItem.Text = "Find Field";
            this.findFieldToolStripMenuItem.ToolTipText = "Search for a field with a matching name";
            this.findFieldToolStripMenuItem.Click += new System.EventHandler(this.FindFieldToolStripMenuItem_Click);
            // 
            // findNextFieldToolStripMenuItem
            // 
            this.findNextFieldToolStripMenuItem.Name = "findNextFieldToolStripMenuItem";
            this.findNextFieldToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.findNextFieldToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.findNextFieldToolStripMenuItem.Text = "Find Next Field";
            this.findNextFieldToolStripMenuItem.ToolTipText = "Search again with the previous pattern";
            this.findNextFieldToolStripMenuItem.Click += new System.EventHandler(this.FindNextFieldToolStripMenuItem_Click);
            // 
            // importNamesToolStripMenuItem
            // 
            this.importNamesToolStripMenuItem.Name = "importNamesToolStripMenuItem";
            this.importNamesToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.importNamesToolStripMenuItem.Text = "Import Names";
            this.importNamesToolStripMenuItem.ToolTipText = "Insert known row names to their corresponding IDs";
            this.importNamesToolStripMenuItem.Click += new System.EventHandler(this.ImportNamesToolStripMenuItem_Click);
            // 
            // exportNamesToolStripMenuItem
            // 
            this.exportNamesToolStripMenuItem.Name = "exportNamesToolStripMenuItem";
            this.exportNamesToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.exportNamesToolStripMenuItem.Text = "Export Names";
            this.exportNamesToolStripMenuItem.ToolTipText = "Export row names from the params to text files";
            this.exportNamesToolStripMenuItem.Click += new System.EventHandler(this.ExportNamesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(222, 6);
            // 
            // verifyDeletionsToolStripMenuItem
            // 
            this.verifyDeletionsToolStripMenuItem.Checked = true;
            this.verifyDeletionsToolStripMenuItem.CheckOnClick = true;
            this.verifyDeletionsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.verifyDeletionsToolStripMenuItem.Name = "verifyDeletionsToolStripMenuItem";
            this.verifyDeletionsToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.verifyDeletionsToolStripMenuItem.Text = "Verify Row Deletion";
            this.verifyDeletionsToolStripMenuItem.ToolTipText = "If checked, always ask before deleting a row";
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.updateToolStripMenuItem.BackColor = System.Drawing.Color.Yellow;
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.updateToolStripMenuItem.Text = "Update";
            this.updateToolStripMenuItem.ToolTipText = "An update is available! Click to download it.";
            this.updateToolStripMenuItem.Visible = false;
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.UpdateToolStripMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 24);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(760, 500);
            this.splitContainer2.SplitterDistance = 249;
            this.splitContainer2.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Size = new System.Drawing.Size(507, 500);
            this.splitContainer1.SplitterDistance = 250;
            this.splitContainer1.TabIndex = 7;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 524);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(760, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(81, 17);
            this.toolStripStatusLabel1.Text = "No file loaded";
            // 
            // fbdExport
            // 
            this.fbdExport.Description = "Choose the folder to export parambnds to";
            this.fbdExport.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 546);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "Yapped <version>";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog ofdRegulation;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exploreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importNamesToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportNamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem verifyDeletionsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem hideUnusedParamsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gotoRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findNextRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findFieldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findNextFieldToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem duplicateRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog fbdExport;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxGame;
    }
}

