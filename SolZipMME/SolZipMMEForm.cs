using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SolZipBasis;

namespace SolZipMME
{
    public partial class SolZipMMEForm : Form
    {
        public SolZipMMEForm(string fileToZip)
        {
            InitializeComponent();
            Height = 202;
            FileToZipTextBox.Text = fileToZip;
            ZipFileTextBox.Text = SolZipHelper.GetZipFileName(fileToZip);
        }

        private void FileToZipButton_Click(object sender, EventArgs e)
        {
            FileToZipBrowser.FileName = FileToZipTextBox.Text;
            FileToZipBrowser.ShowDialog(this);
            FileToZipTextBox.Text = FileToZipBrowser.FileName;
        }

        private void ZipFileButton_Click(object sender, EventArgs e)
        {
            ZipFileNameDialog.FileName = ZipFileTextBox.Text;
            ZipFileNameDialog.ShowDialog(this);
            ZipFileTextBox.Text = ZipFileNameDialog.FileName;
        }

        private void ZipItButton_Click(object sender, EventArgs e)
        {
            Cursor cursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                SolZipHelper.Zip(
                    ZipFileTextBox.Text, FileToZipTextBox.Text,
                    ShowCheckBox.Checked, ClipboardCheckBox.Checked, 
                    ExcludeCheckBox.Checked, RemoveSourceControlCheckBox.Checked);
                Close();
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show("Solution Zipper", "Exception: " + ex.ToString());
#else
                MessageBox.Show("Solution Zipper", "There was a problem: " + ex.Message);
#endif
            }
            finally
            {
                Cursor.Current = cursor;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ViewItemsButton_Click(object sender, EventArgs e)
        {
            Height = 582;
        }
    }
}
