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

namespace UoFiddler.Controls.Forms
{
    partial class MultiImportForm
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
            importTypeComboBox = new System.Windows.Forms.ComboBox();
            filenameTextBox = new System.Windows.Forms.TextBox();
            button1 = new System.Windows.Forms.Button();
            importButton = new System.Windows.Forms.Button();
            MassImportCheckBox = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // importTypeComboBox
            // 
            importTypeComboBox.FormattingEnabled = true;
            importTypeComboBox.Items.AddRange(new object[] { "Txt file", "UOA file", "UOA Binary file", "WSC file", "CSV (punt's multi tool)" });
            importTypeComboBox.Location = new System.Drawing.Point(13, 12);
            importTypeComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            importTypeComboBox.Name = "importTypeComboBox";
            importTypeComboBox.Size = new System.Drawing.Size(178, 23);
            importTypeComboBox.TabIndex = 0;
            // 
            // filenameTextBox
            // 
            filenameTextBox.Location = new System.Drawing.Point(13, 42);
            filenameTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            filenameTextBox.Name = "filenameTextBox";
            filenameTextBox.Size = new System.Drawing.Size(275, 23);
            filenameTextBox.TabIndex = 2;
            // 
            // button1
            // 
            button1.AutoSize = true;
            button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            button1.Location = new System.Drawing.Point(296, 42);
            button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(26, 25);
            button1.TabIndex = 3;
            button1.Text = "...";
            button1.UseVisualStyleBackColor = true;
            button1.Click += OnClickBrowse;
            // 
            // importButton
            // 
            importButton.Location = new System.Drawing.Point(234, 72);
            importButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            importButton.Name = "importButton";
            importButton.Size = new System.Drawing.Size(88, 27);
            importButton.TabIndex = 4;
            importButton.Text = "Import";
            importButton.UseVisualStyleBackColor = true;
            importButton.Click += OnClickImport;
            // 
            // MassImportCheckBox
            // 
            MassImportCheckBox.AutoSize = true;
            MassImportCheckBox.Location = new System.Drawing.Point(198, 14);
            MassImportCheckBox.Name = "MassImportCheckBox";
            MassImportCheckBox.Size = new System.Drawing.Size(133, 19);
            MassImportCheckBox.TabIndex = 5;
            MassImportCheckBox.Text = "Multiple files import";
            MassImportCheckBox.UseVisualStyleBackColor = true;
            MassImportCheckBox.CheckedChanged += MassImportCheckBox_CheckedChanged;
            // 
            // MultiImportForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(337, 109);
            Controls.Add(MassImportCheckBox);
            Controls.Add(importButton);
            Controls.Add(button1);
            Controls.Add(filenameTextBox);
            Controls.Add(importTypeComboBox);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MultiImportForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Multi Import";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.ComboBox importTypeComboBox;
        private System.Windows.Forms.TextBox filenameTextBox;
        private System.Windows.Forms.CheckBox MassImportCheckBox;
    }
}