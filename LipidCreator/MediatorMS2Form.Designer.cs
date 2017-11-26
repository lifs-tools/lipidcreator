/*
MIT License

Copyright (c) 2017 Dominik Kopczynski   -   dominik.kopczynski {at} isas.de
                   Bing Peng   -   bing.peng {at} isas.de

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

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace LipidCreator
{
    partial class MediatorMS2Form
    {
        public PictureBox pictureBoxFragments;
        public CheckedListBox checkedListBoxMonoIsotopicFragments;
        public CheckedListBox checkedListBoxDeuteratedFragments;
        public Label labelMediators;
        public Label labelMonoIsotopicFragments;
        public Label labelDeuteratedFragments;
        
        public Label labelFragmentDescriptionBlack;
        public Label labelFragmentDescriptionRed;
        public Label labelFragmentDescriptionBlue;
        public Label labelPositiveSelectAll;
        public Label labelPositiveDeselectAll;
        public Label labelNegativeSelectAll;
        public Label labelNegativeDeselectAll;
        public Label labelSlashPositive;
        public Label labelSlashNegative;
        public Button buttonCancel;
        public Button buttonOK;
        public Button buttonAddFragment;
        public TabControl tabControlFragments;
        public ArrayList tabPages;
        public ToolTip toolTip1;
        public ListBox medHgListbox;
        
        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(1168, 447);
            
            medHgListbox = new ListBox();
            checkedListBoxMonoIsotopicFragments = new System.Windows.Forms.CheckedListBox();
            checkedListBoxDeuteratedFragments = new System.Windows.Forms.CheckedListBox();
            labelMediators = new System.Windows.Forms.Label();
            labelMonoIsotopicFragments = new System.Windows.Forms.Label();
            labelDeuteratedFragments = new System.Windows.Forms.Label();
            
            medHgListbox.Location = new System.Drawing.Point(12, 22);
            medHgListbox.Size = new System.Drawing.Size(150, 294);
            medHgListbox.BringToFront();
            medHgListbox.BorderStyle = BorderStyle.Fixed3D;
            //medHgListbox.SelectionMode = SelectionMode.SingleSimple;
            //medHgListbox.SelectedValueChanged += new System.EventHandler(medHGListboxSelectedValueChanged);
            //medHgListbox.MouseLeave += new System.EventHandler(medHGListboxMouseLeave);
            //medHgListbox.MouseMove += new System.Windows.Forms.MouseEventHandler(medHGListboxMouseHover);
            this.Controls.Add(medHgListbox);
        
            /*
            this.pictureBoxFragments = new System.Windows.Forms.PictureBox();
            this.labelPositiveFragments = new System.Windows.Forms.Label();
            this.labelNegativeFragments = new System.Windows.Forms.Label();
            this.labelFragmentDescriptionBlack = new System.Windows.Forms.Label();
            this.labelFragmentDescriptionRed = new System.Windows.Forms.Label();
            this.labelFragmentDescriptionBlue = new System.Windows.Forms.Label();
            this.labelPositiveSelectAll = new System.Windows.Forms.Label();
            this.labelPositiveDeselectAll = new System.Windows.Forms.Label();
            this.labelNegativeSelectAll = new System.Windows.Forms.Label();
            this.labelNegativeDeselectAll = new System.Windows.Forms.Label();
            this.labelSlashPositive = new System.Windows.Forms.Label();
            this.labelSlashNegative = new System.Windows.Forms.Label();
            this.checkedListBoxNegativeFragments = new System.Windows.Forms.CheckedListBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonAddFragment = new System.Windows.Forms.Button();
            this.tabControlFragments = new System.Windows.Forms.TabControl();
            toolTip1 = new System.Windows.Forms.ToolTip();
            this.tabPages = new ArrayList();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFragments)).BeginInit();
            this.tabControlFragments.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBoxFragments.Location = new System.Drawing.Point(340, 2);
            this.pictureBoxFragments.Name = "pictureBox1";
            this.pictureBoxFragments.Size = new System.Drawing.Size(736, 358);
            this.pictureBoxFragments.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxFragments.TabIndex = 1;
            this.pictureBoxFragments.TabStop = false;
            */
            // 
            // checkedListBox1 - monoisotopic fragments
            // 
            checkedListBoxMonoIsotopicFragments.CheckOnClick = true;
            checkedListBoxMonoIsotopicFragments.FormattingEnabled = true;
            checkedListBoxMonoIsotopicFragments.Location = new System.Drawing.Point(200, 22);
            checkedListBoxMonoIsotopicFragments.Name = "checkedListBox1";
            checkedListBoxMonoIsotopicFragments.ScrollAlwaysVisible = true;
            checkedListBoxMonoIsotopicFragments.Size = new System.Drawing.Size(150, 294);
            checkedListBoxMonoIsotopicFragments.TabIndex = 2;
            checkedListBoxMonoIsotopicFragments.ThreeDCheckBoxes = true;
            //checkedListBoxMonoIsotopicFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxPositiveItemCheck);
            //checkedListBoxMonoIsotopicFragments.MouseLeave += new System.EventHandler(checkedListBoxMonoIsotopicFragments.checkedListBoxMouseLeave);
            //checkedListBoxPositiveFragments.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkedListBoxPositiveMouseHover);
            this.Controls.Add(checkedListBoxMonoIsotopicFragments);
            // 
            // checkedListBox2 - monoisotopic fragments
            // 
            checkedListBoxDeuteratedFragments.CheckOnClick = true;
            checkedListBoxDeuteratedFragments.FormattingEnabled = true;
            checkedListBoxDeuteratedFragments.Location = new System.Drawing.Point(360, 22);
            checkedListBoxDeuteratedFragments.Name = "checkedListBox2";
            checkedListBoxDeuteratedFragments.ScrollAlwaysVisible = true;
            checkedListBoxDeuteratedFragments.Size = new System.Drawing.Size(150, 294);
            checkedListBoxDeuteratedFragments.TabIndex = 3;
            checkedListBoxDeuteratedFragments.ThreeDCheckBoxes = true;
            //checkedListBoxMonoIsotopicFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxPositiveItemCheck);
            //checkedListBoxMonoIsotopicFragments.MouseLeave += new System.EventHandler(checkedListBoxMonoIsotopicFragments.checkedListBoxMouseLeave);
            //checkedListBoxPositiveFragments.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkedListBoxPositiveMouseHover);
            this.Controls.Add(checkedListBoxDeuteratedFragments);
            
            
            // 
            // label1
            // 
            labelMediators.AutoSize = true;
            labelMediators.Location = new System.Drawing.Point(12, 6);
            labelMediators.Name = "label1";
            labelMediators.Text = "Mediator";
            Controls.Add(labelMediators);
            // 
            // label2
            // 
            labelMonoIsotopicFragments.AutoSize = true;
            labelMonoIsotopicFragments.Location = new System.Drawing.Point(200, 6);
            labelMonoIsotopicFragments.Name = "label2";
            labelMonoIsotopicFragments.Text = "Monoisotopic Fragments";
            Controls.Add(labelMonoIsotopicFragments);
            // 
            // label3
            // 
            labelDeuteratedFragments.AutoSize = true;
            labelDeuteratedFragments.Location = new System.Drawing.Point(360, 6);
            labelDeuteratedFragments.Name = "label3";
            labelDeuteratedFragments.Text = "Deuterated Fragments";
            this.Controls.Add(labelDeuteratedFragments);
            /*
            // 
            // label4
            // 
            this.labelFragmentDescriptionRed.Size = new Size(200, 13);
            this.labelFragmentDescriptionRed.Location = new System.Drawing.Point(1110 - labelFragmentDescriptionRed.Width, 295);
            this.labelFragmentDescriptionRed.Name = "label4";
            this.labelFragmentDescriptionRed.TabIndex = 5;
            this.labelFragmentDescriptionRed.Text = "red: specific for lipid category";
            this.labelFragmentDescriptionRed.ForeColor = Color.FromArgb(227, 5, 19);
            this.labelFragmentDescriptionRed.TextAlign = ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.labelFragmentDescriptionBlue.Size = new Size(200, 13);
            this.labelFragmentDescriptionBlue.Location = new System.Drawing.Point(1110 - labelFragmentDescriptionBlue.Width, 310);
            this.labelFragmentDescriptionBlue.Name = "label5";
            this.labelFragmentDescriptionBlue.TabIndex = 5;
            this.labelFragmentDescriptionBlue.Text = "blue: specific for lipid class";
            this.labelFragmentDescriptionBlue.ForeColor = Color.FromArgb(48, 38, 131);
            this.labelFragmentDescriptionBlue.TextAlign = ContentAlignment.TopRight;
            // 
            // label6 - positive fragments
            // 
            this.labelPositiveSelectAll.AutoSize = true;
            this.labelPositiveSelectAll.Location = new System.Drawing.Point(12, 316);
            this.labelPositiveSelectAll.Name = "label6";
            this.labelPositiveSelectAll.TabIndex = 6;
            this.labelPositiveSelectAll.Text = "select all";
            this.labelPositiveSelectAll.ForeColor = Color.FromArgb(0, 0, 255);
            this.labelPositiveSelectAll.Click += new System.EventHandler(checkedListBoxPositiveSelectAll);
            // 
            // label7 - positive fragments
            // 
            this.labelPositiveDeselectAll.AutoSize = true;
            this.labelPositiveDeselectAll.Location = new System.Drawing.Point(74, 316);
            this.labelPositiveDeselectAll.Name = "label7";
            this.labelPositiveDeselectAll.TabIndex = 6;
            this.labelPositiveDeselectAll.Text = "deselect all";
            this.labelPositiveDeselectAll.ForeColor = Color.FromArgb(0, 0, 255);
            this.labelPositiveDeselectAll.Click += new System.EventHandler(checkedListBoxPositiveDeselectAll);
            // 
            // label8 - positive fragments
            // 
            this.labelNegativeSelectAll.AutoSize = true;
            this.labelNegativeSelectAll.Location = new System.Drawing.Point(170, 316);
            this.labelNegativeSelectAll.Name = "label8";
            this.labelNegativeSelectAll.TabIndex = 6;
            this.labelNegativeSelectAll.Text = "select all";
            this.labelNegativeSelectAll.ForeColor = Color.FromArgb(0, 0, 255);
            this.labelNegativeSelectAll.Click += new System.EventHandler(checkedListBoxNegativeSelectAll);
            // 
            // label9 - positive fragments
            // 
            this.labelNegativeDeselectAll.AutoSize = true;
            this.labelNegativeDeselectAll.Location = new System.Drawing.Point(232, 316);
            this.labelNegativeDeselectAll.Name = "label9";
            this.labelNegativeDeselectAll.TabIndex = 6;
            this.labelNegativeDeselectAll.Text = "deselect all";
            this.labelNegativeDeselectAll.ForeColor = Color.FromArgb(0, 0, 255);
            this.labelNegativeDeselectAll.Click += new System.EventHandler(checkedListBoxNegativeDeselectAll);
            // 
            // label10
            // 
            this.labelSlashPositive.AutoSize = true;
            this.labelSlashPositive.Location = new System.Drawing.Point(67, 316);
            this.labelSlashPositive.Name = "label10";
            this.labelSlashPositive.Text = "/";
            // 
            // label11
            // 
            this.labelSlashNegative.AutoSize = true;
            this.labelSlashNegative.Location = new System.Drawing.Point(225, 316);
            this.labelSlashNegative.Name = "label11";
            this.labelSlashNegative.Text = "/";
            
            // 
            // checkedListBox2 - negative fragments
            // 
            this.checkedListBoxNegativeFragments.CheckOnClick = true;
            this.checkedListBoxNegativeFragments.FormattingEnabled = true;
            this.checkedListBoxNegativeFragments.Location = new System.Drawing.Point(170, 22);
            this.checkedListBoxNegativeFragments.Name = "checkedListBox2";
            this.checkedListBoxNegativeFragments.ScrollAlwaysVisible = true;
            this.checkedListBoxNegativeFragments.Size = new System.Drawing.Size(150, 294);
            this.checkedListBoxNegativeFragments.TabIndex = 5;
            this.checkedListBoxNegativeFragments.ThreeDCheckBoxes = true;
            this.checkedListBoxNegativeFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxNegativeItemCheck);
            this.checkedListBoxNegativeFragments.MouseLeave += new System.EventHandler(this.checkedListBoxMouseLeave);
            this.checkedListBoxNegativeFragments.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkedListBoxNegativeMouseHover);
            // 
            // button2
            // 
            this.buttonCancel.Location = new System.Drawing.Point(1058, 374);
            this.buttonCancel.Name = "button2";
            this.buttonCancel.Size = new System.Drawing.Size(98, 30);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.cancelClick);
            // 
            // button3
            // 
            this.buttonOK.Location = new System.Drawing.Point(954, 374);
            this.buttonOK.Name = "button3";
            this.buttonOK.Size = new System.Drawing.Size(98, 30);
            this.buttonOK.TabIndex = 8;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.okClick);
            // 
            // button4
            // 
            this.buttonAddFragment.Location = new System.Drawing.Point(12, 374);
            this.buttonAddFragment.Name = "button4";
            this.buttonAddFragment.Size = new System.Drawing.Size(98, 30);
            this.buttonAddFragment.TabIndex = 9;
            this.buttonAddFragment.Text = "Add fragment";
            this.buttonAddFragment.UseVisualStyleBackColor = true;
            this.buttonAddFragment.Click += new System.EventHandler(this.addFragmentClick);
            // 
            // tabControl1
            // 
            this.tabControlFragments.Location = new System.Drawing.Point(12, 6);
            this.tabControlFragments.Name = "tabControl1";
            this.tabControlFragments.SelectedIndex = 0;
            this.tabControlFragments.Size = new System.Drawing.Size(1144, 360);
            this.tabControlFragments.TabIndex = 11;
            this.tabControlFragments.SelectedIndexChanged += new System.EventHandler(tabIndexChanged);
            */
            
            // 
            // MediatorMS2Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1168, 420);
            this.Controls.Add(this.tabControlFragments);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonAddFragment);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            
            /*
            foreach (KeyValuePair<String, ArrayList> item in MS2Fragments)
            {
                TabPage tp = new TabPage();
                tp.Location = new System.Drawing.Point(4, 22);
                tp.Name = item.Key;
                tp.Padding = new System.Windows.Forms.Padding(3);
                tp.Size = new System.Drawing.Size(766, 372);
                tp.TabIndex = 0;
                tp.Text = item.Key;
                tp.UseVisualStyleBackColor = true;
                this.tabControlFragments.Controls.Add(tp);
                this.tabPages.Add(tp);

            }*/

            this.Name = "MediatorMS2Form";
            this.Text = "Mediator MS2 Fragments";
            /*
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFragments)).EndInit();
            this.tabControlFragments.ResumeLayout(false);
            */
        }
    }
}