/*
MIT License

Copyright (c) 2018 Dominik Kopczynski   -   dominik.kopczynski {at} isas.de
                   Bing Peng   -   bing.peng {at} isas.de
                   Nils Hoffmann  -  nils.hoffmann {at} isas.de

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System;

namespace LipidCreator
{
    partial class LipidsInterList
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
            this.dataGridViewPrecursors = new System.Windows.Forms.DataGridView();
            this.cancelButton = new System.Windows.Forms.Button();
            this.labelSelectAll = new System.Windows.Forms.Label();
            this.labelDeselectAll = new System.Windows.Forms.Label();
            this.labelSelected = new System.Windows.Forms.Label();
            this.labelSlash = new System.Windows.Forms.Label();
            this.continueReviewButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPrecursors)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridViewPrecursors.Size = new System.Drawing.Size(555, 609);
            this.dataGridViewPrecursors.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewPrecursors.DataSource = precursorDataTable;
            this.dataGridViewPrecursors.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dataGridViewPrecursors.AllowUserToResizeColumns = false;
            this.dataGridViewPrecursors.AllowUserToAddRows = false;
            this.dataGridViewPrecursors.AllowUserToResizeRows = false;
            this.dataGridViewPrecursors.MultiSelect = false;
            this.dataGridViewPrecursors.RowTemplate.Height = 34;
            this.dataGridViewPrecursors.AllowDrop = true;
            this.dataGridViewPrecursors.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPrecursors.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(precursorGridViewDataBindingComplete);
            this.dataGridViewPrecursors.CellContentClick += new DataGridViewCellEventHandler(precursorGridView_CellClicked);
            this.dataGridViewPrecursors.CellValueChanged += new DataGridViewCellEventHandler(precursorGridView_CellValueChanged);
            this.dataGridViewPrecursors.RowHeadersVisible = false;
            this.dataGridViewPrecursors.ScrollBars = ScrollBars.Vertical;
            // 
            // button1
            // 
            this.cancelButton.Location = new System.Drawing.Point(356, 628);
            this.cancelButton.Name = "button1";
            this.cancelButton.Size = new System.Drawing.Size(88, 34);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButtonClick);
            //
            // label
            //
            this.labelSlash.AutoSize = true;
            this.labelSlash.Location = new System.Drawing.Point(67, 628);
            this.labelSlash.Name = "labelSlash";
            this.labelSlash.Text = "/";
            
            // 
            // labelSelectAll
            // 
            this.labelSelectAll.AutoSize = true;
            this.labelSelectAll.Location = new System.Drawing.Point(12, 628);
            this.labelSelectAll.Name = "labelSelectAll";
            this.labelSelectAll.TabIndex = 6;
            this.labelSelectAll.Text = "select all";
            this.labelSelectAll.ForeColor = Color.FromArgb(0, 0, 255);
            this.labelSelectAll.Click += new System.EventHandler(precursorSelectAll);
            // 
            // labelDeselectAll
            // 
            this.labelDeselectAll.AutoSize = true;
            this.labelDeselectAll.Location = new System.Drawing.Point(74, 628);
            this.labelDeselectAll.Name = "labelDeselectAll";
            this.labelDeselectAll.TabIndex = 6;
            this.labelDeselectAll.Text = "deselect all";
            this.labelDeselectAll.ForeColor = Color.FromArgb(0, 0, 255);
            this.labelDeselectAll.Click += new System.EventHandler(precursorDeselectAll);
            // 
            // labelSelected
            // 
            this.labelSelected.AutoSize = true;
            this.labelSelected.Location = new System.Drawing.Point(12, 648);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.TabIndex = 7;
            this.labelSelected.Text = "Selected precursors: -";
            // 
            // button2
            // 
            this.continueReviewButton.Location = new System.Drawing.Point(459, 628);
            this.continueReviewButton.Name = "button2";
            this.continueReviewButton.Size = new System.Drawing.Size(108, 34);
            this.continueReviewButton.TabIndex = 1;
            this.continueReviewButton.Text = "Continue";
            this.continueReviewButton.UseVisualStyleBackColor = true;
            this.continueReviewButton.Click += new System.EventHandler(this.continueReviewButtonClick);
            this.Font = new Font(Font.Name, CreatorGUI.REGULAR_FONT_SIZE * CreatorGUI.FONT_SIZE_FACTOR, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(579, 674);
            this.Controls.Add(this.continueReviewButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.dataGridViewPrecursors);
            this.Controls.Add(this.labelSlash);
            this.Controls.Add(this.labelSelectAll);
            this.Controls.Add(this.labelSelected);
            this.Controls.Add(this.labelDeselectAll);
            this.Name = "LipidsInterList";
            this.Text = "Lipid Precursor Review";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPrecursors)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            controlElements = new ArrayList(){cancelButton, dataGridViewPrecursors, continueReviewButton, labelSelectAll, labelDeselectAll};
        }

        #endregion

        [NonSerialized]
        public Label labelSelectAll;
        [NonSerialized]
        public Label labelDeselectAll;
        [NonSerialized]
        public Label labelSelected;
        [NonSerialized]
        public Label labelSlash;
        [NonSerialized]
        public System.Windows.Forms.DataGridView dataGridViewPrecursors;
        [NonSerialized]
        public System.Windows.Forms.Button cancelButton;
        [NonSerialized]
        public Button continueReviewButton;
        public ArrayList controlElements;
    }
}
