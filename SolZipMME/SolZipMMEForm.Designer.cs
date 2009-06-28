namespace SolZipMME
{
    partial class SolZipMMEForm
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
            this.FileToZipLabel = new System.Windows.Forms.Label();
            this.FileToZipTextBox = new System.Windows.Forms.TextBox();
            this.FileToZipButton = new System.Windows.Forms.Button();
            this.ZipFileLabel = new System.Windows.Forms.Label();
            this.ZipFileTextBox = new System.Windows.Forms.TextBox();
            this.ZipFileButton = new System.Windows.Forms.Button();
            this.ShowCheckBox = new System.Windows.Forms.CheckBox();
            this.ClipboardCheckBox = new System.Windows.Forms.CheckBox();
            this.ExcludeCheckBox = new System.Windows.Forms.CheckBox();
            this.ZipItButton = new System.Windows.Forms.Button();
            this.CancelItButton = new System.Windows.Forms.Button();
            this.ZipFileFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.FileToZipBrowser = new System.Windows.Forms.OpenFileDialog();
            this.RemoveSourceControlCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // FileToZipLabel
            // 
            this.FileToZipLabel.AutoSize = true;
            this.FileToZipLabel.Location = new System.Drawing.Point(13, 13);
            this.FileToZipLabel.Name = "FileToZipLabel";
            this.FileToZipLabel.Size = new System.Drawing.Size(56, 13);
            this.FileToZipLabel.TabIndex = 0;
            this.FileToZipLabel.Text = "File to Zip:";
            // 
            // FileToZipTextBox
            // 
            this.FileToZipTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.FileToZipTextBox.Location = new System.Drawing.Point(100, 10);
            this.FileToZipTextBox.Name = "FileToZipTextBox";
            this.FileToZipTextBox.Size = new System.Drawing.Size(339, 20);
            this.FileToZipTextBox.TabIndex = 1;
            // 
            // FileToZipButton
            // 
            this.FileToZipButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FileToZipButton.Location = new System.Drawing.Point(445, 8);
            this.FileToZipButton.Name = "FileToZipButton";
            this.FileToZipButton.Size = new System.Drawing.Size(34, 23);
            this.FileToZipButton.TabIndex = 2;
            this.FileToZipButton.Text = "...";
            this.FileToZipButton.UseVisualStyleBackColor = true;
            this.FileToZipButton.Click += new System.EventHandler(this.FileToZipButton_Click);
            // 
            // ZipFileLabel
            // 
            this.ZipFileLabel.AutoSize = true;
            this.ZipFileLabel.Location = new System.Drawing.Point(16, 40);
            this.ZipFileLabel.Name = "ZipFileLabel";
            this.ZipFileLabel.Size = new System.Drawing.Size(81, 13);
            this.ZipFileLabel.TabIndex = 3;
            this.ZipFileLabel.Text = "Name of Zipfile:";
            // 
            // ZipFileTextBox
            // 
            this.ZipFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ZipFileTextBox.Location = new System.Drawing.Point(100, 37);
            this.ZipFileTextBox.Name = "ZipFileTextBox";
            this.ZipFileTextBox.Size = new System.Drawing.Size(339, 20);
            this.ZipFileTextBox.TabIndex = 4;
            // 
            // ZipFileButton
            // 
            this.ZipFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ZipFileButton.Location = new System.Drawing.Point(445, 35);
            this.ZipFileButton.Name = "ZipFileButton";
            this.ZipFileButton.Size = new System.Drawing.Size(34, 23);
            this.ZipFileButton.TabIndex = 5;
            this.ZipFileButton.Text = "...";
            this.ZipFileButton.UseVisualStyleBackColor = true;
            this.ZipFileButton.Click += new System.EventHandler(this.ZipFileButton_Click);
            // 
            // ShowCheckBox
            // 
            this.ShowCheckBox.AutoSize = true;
            this.ShowCheckBox.Checked = true;
            this.ShowCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowCheckBox.Location = new System.Drawing.Point(19, 73);
            this.ShowCheckBox.Name = "ShowCheckBox";
            this.ShowCheckBox.Size = new System.Drawing.Size(111, 17);
            this.ShowCheckBox.TabIndex = 6;
            this.ShowCheckBox.Text = "Show resulting file";
            this.ShowCheckBox.UseVisualStyleBackColor = true;
            // 
            // ClipboardCheckBox
            // 
            this.ClipboardCheckBox.AutoSize = true;
            this.ClipboardCheckBox.Checked = true;
            this.ClipboardCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ClipboardCheckBox.Location = new System.Drawing.Point(19, 96);
            this.ClipboardCheckBox.Name = "ClipboardCheckBox";
            this.ClipboardCheckBox.Size = new System.Drawing.Size(133, 17);
            this.ClipboardCheckBox.TabIndex = 7;
            this.ClipboardCheckBox.Text = "Copy path to Clipboard";
            this.ClipboardCheckBox.UseVisualStyleBackColor = true;
            // 
            // ExcludeCheckBox
            // 
            this.ExcludeCheckBox.AutoSize = true;
            this.ExcludeCheckBox.Location = new System.Drawing.Point(19, 120);
            this.ExcludeCheckBox.Name = "ExcludeCheckBox";
            this.ExcludeCheckBox.Size = new System.Drawing.Size(151, 17);
            this.ExcludeCheckBox.TabIndex = 8;
            this.ExcludeCheckBox.Text = "Exclude SolZipReadme.txt";
            this.ExcludeCheckBox.UseVisualStyleBackColor = true;
            // 
            // ZipItButton
            // 
            this.ZipItButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ZipItButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ZipItButton.Location = new System.Drawing.Point(323, 141);
            this.ZipItButton.Name = "ZipItButton";
            this.ZipItButton.Size = new System.Drawing.Size(75, 23);
            this.ZipItButton.TabIndex = 10;
            this.ZipItButton.Text = "&Zip it";
            this.ZipItButton.UseVisualStyleBackColor = true;
            this.ZipItButton.Click += new System.EventHandler(this.ZipItButton_Click);
            // 
            // CancelItButton
            // 
            this.CancelItButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelItButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelItButton.Location = new System.Drawing.Point(404, 141);
            this.CancelItButton.Name = "CancelItButton";
            this.CancelItButton.Size = new System.Drawing.Size(75, 23);
            this.CancelItButton.TabIndex = 11;
            this.CancelItButton.Text = "&Cancel";
            this.CancelItButton.UseVisualStyleBackColor = true;
            this.CancelItButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // FileToZipBrowser
            // 
            this.FileToZipBrowser.FileName = "openFileDialog1";
            // 
            // RemoveSourceControlCheckBox
            // 
            this.RemoveSourceControlCheckBox.AutoSize = true;
            this.RemoveSourceControlCheckBox.Checked = true;
            this.RemoveSourceControlCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.RemoveSourceControlCheckBox.Location = new System.Drawing.Point(19, 143);
            this.RemoveSourceControlCheckBox.Name = "RemoveSourceControlCheckBox";
            this.RemoveSourceControlCheckBox.Size = new System.Drawing.Size(207, 17);
            this.RemoveSourceControlCheckBox.TabIndex = 9;
            this.RemoveSourceControlCheckBox.Text = "Do not include source control bindings";
            this.RemoveSourceControlCheckBox.UseVisualStyleBackColor = true;
            // 
            // SolZipMMEForm
            // 
            this.AcceptButton = this.ZipItButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelItButton;
            this.ClientSize = new System.Drawing.Size(488, 174);
            this.Controls.Add(this.RemoveSourceControlCheckBox);
            this.Controls.Add(this.CancelItButton);
            this.Controls.Add(this.ZipItButton);
            this.Controls.Add(this.ExcludeCheckBox);
            this.Controls.Add(this.ClipboardCheckBox);
            this.Controls.Add(this.ShowCheckBox);
            this.Controls.Add(this.ZipFileButton);
            this.Controls.Add(this.ZipFileTextBox);
            this.Controls.Add(this.ZipFileLabel);
            this.Controls.Add(this.FileToZipButton);
            this.Controls.Add(this.FileToZipTextBox);
            this.Controls.Add(this.FileToZipLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SolZipMMEForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SolZip";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label FileToZipLabel;
        private System.Windows.Forms.TextBox FileToZipTextBox;
        private System.Windows.Forms.Button FileToZipButton;
        private System.Windows.Forms.Label ZipFileLabel;
        private System.Windows.Forms.TextBox ZipFileTextBox;
        private System.Windows.Forms.Button ZipFileButton;
        private System.Windows.Forms.CheckBox ShowCheckBox;
        private System.Windows.Forms.CheckBox ClipboardCheckBox;
        private System.Windows.Forms.CheckBox ExcludeCheckBox;
        private System.Windows.Forms.Button ZipItButton;
        private System.Windows.Forms.Button CancelItButton;
        private System.Windows.Forms.FolderBrowserDialog ZipFileFolderBrowser;
        private System.Windows.Forms.OpenFileDialog FileToZipBrowser;
        private System.Windows.Forms.CheckBox RemoveSourceControlCheckBox;
    }
}