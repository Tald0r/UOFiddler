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
        private bool _isBatchRunning = false;

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

        private async void OnClickImport(object sender, EventArgs e)
        {
            if (_isBatchRunning) return;

            // Normalize selection
            if (_selectedFiles == null || _selectedFiles.Length == 0)
            {
                if (!File.Exists(filenameTextBox.Text)) return;
                _selectedFiles = new[] { filenameTextBox.Text };
            }

            var type = (Multis.ImportType)importTypeComboBox.SelectedIndex;

            // Single-file behaves synchronously as before
            if (!MassImportCheckBox.Checked || _selectedFiles.Length == 1)
            {
                string path = _selectedFiles[0];
                if (!File.Exists(path)) return;

                try
                {
                    var multi = Ultima.Multis.ImportFromFile(_id, path, type);
                    _changeMultiAction(_id, multi);
                    Options.ChangedUltimaClass["Multis"] = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Import failed:\n{ex.Message}", "Multi import",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            // Batch mode (inside OnClickImport)
            _isBatchRunning = true;
            ToggleUi(false);

            int startId = _id;
            string[] files = _selectedFiles; // snapshot
            int total = files.Length;
            int imported = 0, skipped = 0, errors = 0;

            // Only progress text updates while running
            var progress = new Progress<int>(done =>
            {
                filenameTextBox.Text = $"Importing {done}/{total}...";
            });

            try
            {
                // Run the heavy work off the UI thread
                var results = await System.Threading.Tasks.Task.Run(() =>
                {
                    var list = new System.Collections.Generic.List<(int id, Ultima.MultiComponentList multi)>(total);
                    int idCursor = startId;

                    for (int i = 0; i < total; i++, idCursor++)
                    {
                        string path = files[i];
                        if (!File.Exists(path))
                        {
                            skipped++;
                            ((IProgress<int>)progress).Report(i + 1);
                            continue;
                        }

                        try
                        {
                            // Parse/build multi off the UI thread
                            var multi = Ultima.Multis.ImportFromFile(idCursor, path, type);
                            list.Add((idCursor, multi));
                            imported++;
                        }
                        catch
                        {
                            errors++;
                        }

                        ((IProgress<int>)progress).Report(i + 1);
                    }

                    return list;
                });

                // Apply all changes ONCE on the UI thread, with rendering still disabled in the main app
                // (no per-item events during the run)
                BeginInvoke(new Action(() =>
                {
                    // Commit every imported entry now
                    foreach (var (id, multi) in results)
                        _changeMultiAction(id, multi);

                    // Single flag + one final refresh handled by owning UI
                    Options.ChangedUltimaClass["Multis"] = true;

                    // Close the dialog; main UI will repaint after this returns
                    Close();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Batch import failed:\n{ex.Message}", "Multi import",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isBatchRunning = false;
                // Keep UI disabled until the final BeginInvoke above executes Close();
                // If you prefer to re-enable here, move ToggleUi(true) into the BeginInvoke after Close()
            }
        }

        private void ToggleUi(bool enabled)
        {
            try
            {
                //browseButton.Enabled = enabled;
                importButton.Enabled = enabled;
                importTypeComboBox.Enabled = enabled;
                MassImportCheckBox.Enabled = enabled;
                filenameTextBox.ReadOnly = !enabled;
                Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
            }
            catch { /* tolerate missing controls by name */ }
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
