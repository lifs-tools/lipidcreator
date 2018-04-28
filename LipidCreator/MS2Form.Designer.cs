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


    public class CustomPictureBox : PictureBox
    {
        public event EventHandler ImageChanged;
        public Image Image
        {
            get
            {
                return base.Image;
            }
            set
            {
                if (base.Image != value)
                {
                    base.Image = value;
                    if (this.ImageChanged != null)
                        this.ImageChanged(this, new EventArgs());
                }
            }
        }
    }


    partial class MS2Form
    {

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        
            this.pictureBoxFragments = new CustomPictureBox();
            this.checkedListBoxPositiveFragments = new System.Windows.Forms.CheckedListBox();
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
            this.isotopeList = new System.Windows.Forms.ComboBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonAddFragment = new System.Windows.Forms.Button();
            this.tabControlFragments = new System.Windows.Forms.TabControl();
            this.contextMenuFragment = new System.Windows.Forms.ContextMenu();
            this.menuFragmentItem1 = new System.Windows.Forms.MenuItem();
            this.menuFragmentItem2 = new System.Windows.Forms.MenuItem();
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
            //
            // checkBox1 - isotope list
            //
            isotopeList.Location = new System.Drawing.Point(12, 12);
            isotopeList.Width = 150;
            isotopeList.DropDownStyle = ComboBoxStyle.DropDownList;
            isotopeList.SelectedIndexChanged += new EventHandler(isotopeListComboBoxValueChanged);
            // 
            // checkedListBox1 - positive fragments
            // 
            this.checkedListBoxPositiveFragments.CheckOnClick = true;
            this.checkedListBoxPositiveFragments.FormattingEnabled = true;
            this.checkedListBoxPositiveFragments.Location = new System.Drawing.Point(12, 62);
            this.checkedListBoxPositiveFragments.Name = "checkedListBoxPositive";
            this.checkedListBoxPositiveFragments.ScrollAlwaysVisible = true;
            this.checkedListBoxPositiveFragments.Size = new System.Drawing.Size(150, 254);
            this.checkedListBoxPositiveFragments.TabIndex = 2;
            this.checkedListBoxPositiveFragments.ThreeDCheckBoxes = true;
            this.checkedListBoxPositiveFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxPositiveItemCheck);
            this.checkedListBoxPositiveFragments.MouseLeave += new System.EventHandler(this.checkedListBoxMouseLeave);
            this.checkedListBoxPositiveFragments.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkedListBoxPositiveMouseHover);
            // 
            // label1
            // 
            this.labelPositiveFragments.AutoSize = true;
            this.labelPositiveFragments.Location = new System.Drawing.Point(12, 42);
            this.labelPositiveFragments.Name = "label1";
            this.labelPositiveFragments.TabIndex = 3;
            this.labelPositiveFragments.Text = "Positive Fragments";
            // 
            // label2
            // 
            this.labelNegativeFragments.AutoSize = true;
            this.labelNegativeFragments.Location = new System.Drawing.Point(170, 42);
            this.labelNegativeFragments.Name = "label2";
            this.labelNegativeFragments.TabIndex = 4;
            this.labelNegativeFragments.Text = "Negative Fragments";
            // 
            // label3
            // 
            this.labelFragmentDescriptionBlack.Size = new Size(200, 13);
            this.labelFragmentDescriptionBlack.Location = new System.Drawing.Point(1110 - labelFragmentDescriptionBlack.Width, 280);
            this.labelFragmentDescriptionBlack.Name = "label3";
            this.labelFragmentDescriptionBlack.TabIndex = 5;
            this.labelFragmentDescriptionBlack.Text = "black: unspecific";
            this.labelFragmentDescriptionBlack.TextAlign = ContentAlignment.TopRight;
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
            this.checkedListBoxNegativeFragments.Location = new System.Drawing.Point(170, 62);
            this.checkedListBoxNegativeFragments.Name = "checkedListBoxNegative";
            this.checkedListBoxNegativeFragments.ScrollAlwaysVisible = true;
            this.checkedListBoxNegativeFragments.Size = new System.Drawing.Size(150, 254);
            this.checkedListBoxNegativeFragments.TabIndex = 5;
            this.checkedListBoxNegativeFragments.ThreeDCheckBoxes = true;
            this.checkedListBoxNegativeFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxNegativeItemCheck);
            this.checkedListBoxNegativeFragments.MouseLeave += new System.EventHandler(this.checkedListBoxMouseLeave);
            this.checkedListBoxNegativeFragments.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkedListBoxNegativeMouseHover);
            // 
            // button2
            // 
            this.buttonCancel.Location = new System.Drawing.Point(1058, 394);
            this.buttonCancel.Name = "button2";
            this.buttonCancel.Size = new System.Drawing.Size(98, 30);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.cancelClick);
            // 
            // button3
            // 
            this.buttonOK.Location = new System.Drawing.Point(954, 394);
            this.buttonOK.Name = "button3";
            this.buttonOK.Size = new System.Drawing.Size(98, 30);
            this.buttonOK.TabIndex = 8;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.okClick);
            // 
            // button4
            // 
            this.buttonAddFragment.Location = new System.Drawing.Point(12, 394);
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
            this.tabControlFragments.Size = new System.Drawing.Size(1154, 380);
            this.tabControlFragments.TabIndex = 11;
            this.tabControlFragments.SelectedIndexChanged += new System.EventHandler(tabIndexChanged);
            
            
            
            this.contextMenuFragment.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFragmentItem1,
            this.menuFragmentItem2});
            this.contextMenuFragment.Popup += new System.EventHandler(contextMenuFragmentPopup);
            // 
            // menuItem1
            // 
            this.menuFragmentItem1.Index = 0;
            this.menuFragmentItem1.Text = "Edit fragment";
            this.menuFragmentItem1.Click += new System.EventHandler(editFragment);
            // 
            // menuItem2
            // 
            this.menuFragmentItem2.Index = 1;
            this.menuFragmentItem2.Text = "Delete fragment";
            this.menuFragmentItem2.Click += new System.EventHandler(deleteFragment);
            
            
            // 
            // MS2Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 440);
            this.Controls.Add(this.tabControlFragments);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonAddFragment);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Name = "MS2Form";
            this.Text = "MS2 Fragments";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFragments)).EndInit();
            this.tabControlFragments.ResumeLayout(false);
            
            controlElements = new ArrayList(){checkedListBoxPositiveFragments, checkedListBoxNegativeFragments, buttonCancel, buttonOK, buttonAddFragment, isotopeList, labelPositiveDeselectAll, labelPositiveSelectAll, labelNegativeDeselectAll, labelNegativeSelectAll, menuFragmentItem1, menuFragmentItem2};
        }

        #endregion

        public CustomPictureBox pictureBoxFragments;
        public CheckedListBox checkedListBoxPositiveFragments;
        public CheckedListBox checkedListBoxNegativeFragments;
        public Label labelPositiveFragments;
        public Label labelNegativeFragments;
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
        public ComboBox isotopeList;
        public ArrayList controlElements;
        
        public ContextMenu contextMenuFragment;
        public MenuItem menuFragmentItem1;
        public MenuItem menuFragmentItem2;
    }
}