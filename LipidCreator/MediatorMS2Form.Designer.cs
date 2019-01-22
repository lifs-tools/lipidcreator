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

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace LipidCreator
{
    partial class MediatorMS2Form
    {
        [NonSerialized]
        public PictureBox pictureBoxFragments;
        [NonSerialized]
        public CheckedListBox checkedListBoxMonoIsotopicFragments;
        [NonSerialized]
        public CheckedListBox checkedListBoxDeuteratedFragments;
        [NonSerialized]
        public Label labelMediators;
        [NonSerialized]
        public Label labelMonoIsotopicFragments;
        [NonSerialized]
        public Label labelDeuteratedFragments;
        [NonSerialized]
        public Label labelSlashMonoisotope;
        [NonSerialized]
        public Label labelSlashDeuterated;
        [NonSerialized]
        public Button buttonCancel;
        [NonSerialized]
        public Button buttonOK;
        [NonSerialized]
        public Button buttonAddFragment;
        [NonSerialized]
        public MenuItem menuFragmentItem1;
        [NonSerialized]
        public ContextMenu contextMenuFragment;

        [NonSerialized]
        public Label labelFragmentDescriptionBlack;
        [NonSerialized]
        public Label labelFragmentDescriptionRed;
        [NonSerialized]
        public Label labelFragmentDescriptionBlue;
        [NonSerialized]
        public Label labelMonoisotopeSelectAll;
        [NonSerialized]
        public Label labelMonoisotopeDeselectAll;
        [NonSerialized]
        public Label labelDeuteratedSelectAll;
        [NonSerialized]
        public Label labelDeuteratedDeselectAll;
        [NonSerialized]
        public Label labelSlashPositive;
        [NonSerialized]
        public Label labelSlashNegative;
        [NonSerialized]
        public TabControl tabControlFragments;
        public ArrayList tabPages;
        [NonSerialized]
        public ToolTip toolTip1;
        [NonSerialized]
        public ListBox medHgListbox;
        [NonSerialized]
        public ComboBox deuteratedMediatorHeadgroups;
        
        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(1168, 447);
            
            medHgListbox = new ListBox();
            checkedListBoxMonoIsotopicFragments = new System.Windows.Forms.CheckedListBox();
            checkedListBoxDeuteratedFragments = new System.Windows.Forms.CheckedListBox();
            labelMediators = new System.Windows.Forms.Label();
            labelMonoIsotopicFragments = new System.Windows.Forms.Label();
            labelDeuteratedFragments = new System.Windows.Forms.Label();
            labelMonoisotopeSelectAll = new System.Windows.Forms.Label();
            labelMonoisotopeDeselectAll = new System.Windows.Forms.Label();
            labelDeuteratedSelectAll = new System.Windows.Forms.Label();
            labelDeuteratedDeselectAll = new System.Windows.Forms.Label();
            labelSlashMonoisotope = new System.Windows.Forms.Label();
            labelSlashDeuterated = new System.Windows.Forms.Label();
            pictureBoxFragments = new System.Windows.Forms.PictureBox();
            buttonCancel = new System.Windows.Forms.Button();
            buttonOK = new System.Windows.Forms.Button();
            buttonAddFragment = new System.Windows.Forms.Button();
            deuteratedMediatorHeadgroups = new System.Windows.Forms.ComboBox();
            this.menuFragmentItem1 = new System.Windows.Forms.MenuItem();
            this.contextMenuFragment = new System.Windows.Forms.ContextMenu();
            this.pictureBoxFragments = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            
            medHgListbox.Location = new System.Drawing.Point(12, 22);
            medHgListbox.Size = new System.Drawing.Size(150, 294);
            medHgListbox.BringToFront();
            medHgListbox.BorderStyle = BorderStyle.Fixed3D;
            medHgListbox.SelectedValueChanged += new System.EventHandler(medHGListboxSelectedValueChanged);
            this.Controls.Add(medHgListbox);
        
            /*
            toolTip1 = new System.Windows.Forms.ToolTip();
            */
            
            // pictureBox1
            pictureBoxFragments.Location = new System.Drawing.Point(520, 2);
            pictureBoxFragments.Name = "pictureBox1";
            pictureBoxFragments.Size = new System.Drawing.Size(620, 358);
            pictureBoxFragments.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            pictureBoxFragments.TabStop = false;
            this.Controls.Add(pictureBoxFragments);
            
            // checkedListBox1 - monoisotopic fragments
            checkedListBoxMonoIsotopicFragments.CheckOnClick = true;
            checkedListBoxMonoIsotopicFragments.FormattingEnabled = true;
            checkedListBoxMonoIsotopicFragments.Location = new System.Drawing.Point(200, 47);
            checkedListBoxMonoIsotopicFragments.Name = "checkedListBox1";
            checkedListBoxMonoIsotopicFragments.ScrollAlwaysVisible = true;
            checkedListBoxMonoIsotopicFragments.Size = new System.Drawing.Size(150, 269);
            checkedListBoxMonoIsotopicFragments.TabIndex = 2;
            checkedListBoxMonoIsotopicFragments.ThreeDCheckBoxes = true;
            checkedListBoxMonoIsotopicFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(checkedListBoxMonoIsotopicValueChanged);
            checkedListBoxMonoIsotopicFragments.MouseMove += new System.Windows.Forms.MouseEventHandler(checkedListBoxMonoIsotopicMouseHover);
            this.Controls.Add(checkedListBoxMonoIsotopicFragments);
            
            
            deuteratedMediatorHeadgroups.Location = new System.Drawing.Point(360, 22);
            deuteratedMediatorHeadgroups.Width = 150;
            deuteratedMediatorHeadgroups.DropDownStyle = ComboBoxStyle.DropDownList;
            deuteratedMediatorHeadgroups.SelectedIndexChanged += new EventHandler(deuteratedCheckBoxValueChanged);
            this.Controls.Add(deuteratedMediatorHeadgroups);
            
            
            
            // checkedListBox2 - deuterated fragments
            checkedListBoxDeuteratedFragments.CheckOnClick = true;
            checkedListBoxDeuteratedFragments.FormattingEnabled = true;
            checkedListBoxDeuteratedFragments.Location = new System.Drawing.Point(360, 47);
            checkedListBoxDeuteratedFragments.Name = "checkedListBox2";
            checkedListBoxDeuteratedFragments.ScrollAlwaysVisible = true;
            checkedListBoxDeuteratedFragments.Size = new System.Drawing.Size(150, 269);
            checkedListBoxDeuteratedFragments.TabIndex = 3;
            checkedListBoxDeuteratedFragments.ThreeDCheckBoxes = true;
            checkedListBoxDeuteratedFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(checkedListBoxDeuteratedValueChanged);
            checkedListBoxDeuteratedFragments.MouseMove += new System.Windows.Forms.MouseEventHandler(checkedListBoxDeuteratedeMouseHover);
            this.Controls.Add(checkedListBoxDeuteratedFragments);
            
            
            this.contextMenuFragment.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {this.menuFragmentItem1});
            this.contextMenuFragment.Popup += new System.EventHandler(contextMenuFragmentPopup);
            // 
            // menuItem1
            // 
            this.menuFragmentItem1.Index = 0;
            this.menuFragmentItem1.Text = "Delete fragment";
            this.menuFragmentItem1.Click += new System.EventHandler(deleteFragment);
            
            // label1
            labelMediators.AutoSize = true;
            labelMediators.Location = new System.Drawing.Point(12, 6);
            labelMediators.Name = "label1";
            labelMediators.Text = "Mediator";
            Controls.Add(labelMediators);
            
            
            // label2
            labelMonoIsotopicFragments.AutoSize = true;
            labelMonoIsotopicFragments.Location = new System.Drawing.Point(200, 6);
            labelMonoIsotopicFragments.Name = "label2";
            labelMonoIsotopicFragments.Text = "Monoisotopic Fragments";
            Controls.Add(labelMonoIsotopicFragments);
            
            
            // label3
            labelDeuteratedFragments.AutoSize = true;
            labelDeuteratedFragments.Location = new System.Drawing.Point(360, 6);
            labelDeuteratedFragments.Name = "label3";
            labelDeuteratedFragments.Text = "Isotopic Fragments";
            this.Controls.Add(labelDeuteratedFragments);
            
            
            // label6 - positive fragments
            labelMonoisotopeSelectAll.AutoSize = true;
            labelMonoisotopeSelectAll.Location = new System.Drawing.Point(200, 316);
            labelMonoisotopeSelectAll.Name = "label6";
            labelMonoisotopeSelectAll.TabIndex = 6;
            labelMonoisotopeSelectAll.Text = "select all";
            labelMonoisotopeSelectAll.ForeColor = Color.FromArgb(0, 0, 255);
            labelMonoisotopeSelectAll.Click += new System.EventHandler(checkedListBoxMonoisotopicSelectAll);
            this.Controls.Add(labelMonoisotopeSelectAll);
            
            
            // label7 - positive fragments
            labelMonoisotopeDeselectAll.AutoSize = true;
            labelMonoisotopeDeselectAll.Location = new System.Drawing.Point(262, 316);
            labelMonoisotopeDeselectAll.Name = "label7";
            labelMonoisotopeDeselectAll.TabIndex = 6;
            labelMonoisotopeDeselectAll.Text = "deselect all";
            labelMonoisotopeDeselectAll.ForeColor = Color.FromArgb(0, 0, 255);
            labelMonoisotopeDeselectAll.Click += new System.EventHandler(checkedListBoxMonoisotopicDeselectAll);
            this.Controls.Add(labelMonoisotopeDeselectAll);
            
            
            // label8 - positive fragments
            labelDeuteratedSelectAll.AutoSize = true;
            labelDeuteratedSelectAll.Location = new System.Drawing.Point(360, 316);
            labelDeuteratedSelectAll.Name = "label8";
            labelDeuteratedSelectAll.TabIndex = 6;
            labelDeuteratedSelectAll.Text = "select all";
            labelDeuteratedSelectAll.ForeColor = Color.FromArgb(0, 0, 255);
            labelDeuteratedSelectAll.Click += new System.EventHandler(checkedListBoxDeuteratedSelectAll);
            this.Controls.Add(labelDeuteratedSelectAll);
            
            
            // label9 - positive fragments
            labelDeuteratedDeselectAll.AutoSize = true;
            labelDeuteratedDeselectAll.Location = new System.Drawing.Point(422, 316);
            labelDeuteratedDeselectAll.Name = "label9";
            labelDeuteratedDeselectAll.TabIndex = 6;
            labelDeuteratedDeselectAll.Text = "deselect all";
            labelDeuteratedDeselectAll.ForeColor = Color.FromArgb(0, 0, 255);
            labelDeuteratedDeselectAll.Click += new System.EventHandler(checkedListBoxDeuteratedDeselectAll);
            this.Controls.Add(labelDeuteratedDeselectAll);
            
            
            // label10
            labelSlashMonoisotope.AutoSize = true;
            labelSlashMonoisotope.Location = new System.Drawing.Point(255, 316);
            labelSlashMonoisotope.Name = "label10";
            labelSlashMonoisotope.Text = "/";
            this.Controls.Add(labelSlashMonoisotope);
            
            
            // label11
            labelSlashDeuterated.AutoSize = true;
            labelSlashDeuterated.Location = new System.Drawing.Point(415, 316);
            labelSlashDeuterated.Name = "label11";
            labelSlashDeuterated.Text = "/";
            this.Controls.Add(labelSlashDeuterated);
            
            
            // button2
            buttonCancel.Location = new System.Drawing.Point(1058, 374);
            buttonCancel.Name = "button2";
            buttonCancel.Size = new System.Drawing.Size(98, 30);
            buttonCancel.TabIndex = 7;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += new System.EventHandler(cancelClick);
            this.Controls.Add(buttonCancel);
            
            
            // button3
            buttonOK.Location = new System.Drawing.Point(954, 374);
            buttonOK.Name = "button3";
            buttonOK.Size = new System.Drawing.Size(98, 30);
            buttonOK.TabIndex = 8;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += new System.EventHandler(okClick);
            this.Controls.Add(buttonOK);
           
           
            // button4
            buttonAddFragment.Location = new System.Drawing.Point(12, 374);
            buttonAddFragment.Name = "button4";
            buttonAddFragment.Size = new System.Drawing.Size(98, 30);
            buttonAddFragment.TabIndex = 9;
            buttonAddFragment.Text = "Add fragment";
            buttonAddFragment.UseVisualStyleBackColor = true;
            buttonAddFragment.Click += new System.EventHandler(this.addFragmentClick);
            this.Controls.Add(buttonAddFragment);
            
            // 
            // MediatorMS2Form
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1168, 420);
            this.Controls.Add(this.tabControlFragments);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonAddFragment);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Name = "MediatorMS2Form";
            this.Text = "Mediator MS2 Fragments";
            
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFragments)).EndInit();
        }
    }
}