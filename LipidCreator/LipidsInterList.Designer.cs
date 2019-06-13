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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.continueReviewButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPrecursors)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridViewPrecursors.Size = new System.Drawing.Size(705, 609);
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
            this.cancelButton.Location = new System.Drawing.Point(508, 688);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Location = new System.Drawing.Point(12, 648);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(488, 75);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(13, 15);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(37, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Display precursors on subspecies level";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(13, 35);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(42, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "Display precursors on species level (only containing class specific fragments)";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(13, 55);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(42, 17);
            this.radioButton3.TabIndex = 1;
            this.radioButton3.Text = "Display precursors on species level";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
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
            this.labelSelected.Location = new System.Drawing.Point(200, 628);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.TabIndex = 7;
            this.labelSelected.Text = "Selected precursors: -";
            // 
            // button2
            // 
            this.continueReviewButton.Location = new System.Drawing.Point(609, 688);
            this.continueReviewButton.Name = "button2";
            this.continueReviewButton.Size = new System.Drawing.Size(108, 34);
            this.continueReviewButton.TabIndex = 1;
            this.continueReviewButton.Text = "Continue";
            this.continueReviewButton.UseVisualStyleBackColor = true;
            this.continueReviewButton.Click += new System.EventHandler(this.continueReviewButtonClick);
            
            this.Font = new Font(Font.Name, CreatorGUI.REGULAR_FONT_SIZE * CreatorGUI.FONT_SIZE_FACTOR, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(729, 734);
            this.Controls.Add(this.continueReviewButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.dataGridViewPrecursors);
            this.Controls.Add(this.labelSlash);
            this.Controls.Add(this.labelSelectAll);
            this.Controls.Add(this.labelSelected);
            this.Controls.Add(this.labelDeselectAll);
            this.Controls.Add(this.groupBox1);
            this.Name = "LipidsInterList";
            this.Text = "Lipid Precursor Review";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPrecursors)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            controlElements = new ArrayList(){cancelButton, dataGridViewPrecursors, continueReviewButton, labelSelectAll, labelDeselectAll, groupBox1, radioButton1, radioButton2, radioButton3};
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        public ArrayList controlElements;
    }
}
