/***************************************************************************
 *
 * $Author: Turley
 * 
 * "THE BEER-WARE LICENSE"
 * As long as you retain this notice you can do whatever you want with 
 * this stuff. If we meet some day, and you think this stuff is worth it,
 * you can buy me a beer in return.
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Windows.Forms;
using Ultima;
using UoFiddler.Controls.Classes;

namespace UoFiddler.Controls.Forms
{
    public partial class MultiImportForm : Form
    {
        private readonly int _id;
        private readonly Action<int, MultiComponentList> _changeMultiAction;
        private string[] _selectedFiles = Array.Empty<string>(); // filled by Browse when MassImport is enabled

        public MultiImportForm(int id, Action<int, MultiComponentList> changeMultiAction)
        {
            InitializeComponent();
            Icon = Options.GetFiddlerIcon();

            _id = id;
            _changeMultiAction = changeMultiAction;

            importTypeComboBox.SelectedIndex = 0;
        }

        private void OnClickBrowse(object sender, EventArgs e)
        {
            // Multiselect mirrors the checkbox state
            using (OpenFileDialog dialog = new OpenFileDialog { Multiselect = MassImportCheckBox.Checked })
            {
                string type = "txt";

                switch (importTypeComboBox.SelectedIndex)
                {
                    case 0:
                        type = "txt";
                        break;
                    case 1:
                        type = "txt";
                        break;
                    case 2:
                        type = "uoab";
                        break;
                    case 3:
                        type = "wsc";
                        break;
                    case 4:
                        type = "csv";
                        break;
                }

                dialog.Title = $"Choose {type} file to import";
                dialog.CheckFileExists = true;
                dialog.Filter = type == "uoab" ? "{0} file (*.uoa)|*.uoa" : string.Format("{0} file (*.{0})|*.{0}", type);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (dialog.Multiselect)
                    {
                        _selectedFiles = dialog.FileNames;
                        // Show first file and a count hint
                        filenameTextBox.Text = _selectedFiles.Length > 0
                            ? $"{_selectedFiles[0]} (+{_selectedFiles.Length - 1} more)"
                            : string.Empty;
                    }
                    else
                    {
                        _selectedFiles = new[] { dialog.FileName };
                        filenameTextBox.Text = dialog.FileName;
                    }
                }
            }
        }

        private void OnClickImport(object sender, EventArgs e)
        {
            // Fallback for legacy behavior if user bypassed Browse
            if (_selectedFiles == null || _selectedFiles.Length == 0)
            {
                if (!File.Exists(filenameTextBox.Text))
                    return;

                _selectedFiles = new[] { filenameTextBox.Text };
            }

            Multis.ImportType type = (Multis.ImportType)importTypeComboBox.SelectedIndex;

            if (MassImportCheckBox.Checked)
            {
                int idCursor = _id;
                foreach (var path in _selectedFiles)
                {
                    if (!File.Exists(path))
                    {
                        idCursor++;
                        continue;
                    }

                    MultiComponentList multi = Multis.ImportFromFile(idCursor, path, type);
                    _changeMultiAction(idCursor, multi);
                    idCursor++;
                }

                Options.ChangedUltimaClass["Multis"] = true;
                Close();
                return;
            }
            else
            {
                // Single import path (original behavior)
                string path = _selectedFiles[0];
                if (!File.Exists(path))
                    return;

                MultiComponentList multi = Multis.ImportFromFile(_id, path, type);
                _changeMultiAction(_id, multi);
                Options.ChangedUltimaClass["Multis"] = true;
                Close();
            }
        }

        private void MassImportCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Clear any previously selected files to avoid accidental reuse when the mode changes
            _selectedFiles = Array.Empty<string>();
            filenameTextBox.Clear();
            // The OpenFileDialog.Multiselect is set at Browse time from the checkbox, so no further action needed here
        }

    }
}
