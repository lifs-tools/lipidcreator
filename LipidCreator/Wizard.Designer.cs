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

using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Data;
using System;

namespace LipidCreator
{


    partial class Wizard
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.continueButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.labelInformation = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.categoryCombobox = new System.Windows.Forms.ComboBox();
            this.hgCombobox = new System.Windows.Forms.ComboBox();
            this.faCombobox = new System.Windows.Forms.ComboBox();
            this.faHydroxyCombobox = new System.Windows.Forms.ComboBox();
            this.lcbHydroxyCombobox = new System.Windows.Forms.ComboBox();
            this.faCheckbox1 = new System.Windows.Forms.CheckBox();
            this.faCheckbox2 = new System.Windows.Forms.CheckBox();
            this.faCheckbox3 = new System.Windows.Forms.CheckBox();
            this.faTextbox = new System.Windows.Forms.TextBox();
            this.dbTextbox = new System.Windows.Forms.TextBox();
            this.dbLabel = new System.Windows.Forms.Label();
            this.hydroxylTextbox = new System.Windows.Forms.TextBox();
            this.hydroxylLabel = new System.Windows.Forms.Label();
            this.posAdductCheckbox1 = new System.Windows.Forms.CheckBox();
            this.posAdductCheckbox2 = new System.Windows.Forms.CheckBox();
            this.posAdductCheckbox3 = new System.Windows.Forms.CheckBox();
            this.negAdductCheckbox1 = new System.Windows.Forms.CheckBox();
            this.negAdductCheckbox2 = new System.Windows.Forms.CheckBox();
            this.negAdductCheckbox3 = new System.Windows.Forms.CheckBox();
            this.negAdductCheckbox4 = new System.Windows.Forms.CheckBox();
            this.positiveAdduct = new System.Windows.Forms.GroupBox();
            this.negativeAdduct = new System.Windows.Forms.GroupBox();
            this.filterGroupbox = new System.Windows.Forms.GroupBox();
            this.checkedListBoxPositiveFragments = new System.Windows.Forms.CheckedListBox();
            this.checkedListBoxNegativeFragments = new System.Windows.Forms.CheckedListBox();
            this.labelPositiveFragments = new System.Windows.Forms.Label();
            this.labelNegativeFragments = new System.Windows.Forms.Label();
            this.labelPositiveSelectAll = new System.Windows.Forms.Label();
            this.labelPositiveDeselectAll = new System.Windows.Forms.Label();
            this.labelNegativeSelectAll = new System.Windows.Forms.Label();
            this.labelNegativeDeselectAll = new System.Windows.Forms.Label();
            this.labelSlashPositive = new System.Windows.Forms.Label();
            this.labelSlashNegative = new System.Windows.Forms.Label();
            this.wizardPictureBox = new System.Windows.Forms.PictureBox();
            this.labelLine = new System.Windows.Forms.Label();
            this.faRepresentative = new System.Windows.Forms.CheckBox();
            this.lipidPreview = new System.Windows.Forms.DataGridView();
            this.lipidDataTable = new DataTable();
            
            this.noPrecursorRadiobutton = new System.Windows.Forms.RadioButton();
            this.onlyPrecursorRadiobutton = new System.Windows.Forms.RadioButton();
            this.withPrecursorRadiobutton = new System.Windows.Forms.RadioButton();
            
            
            lcbHydroxyCombobox.Items.Add("2");
            lcbHydroxyCombobox.Items.Add("3");
            
            faHydroxyCombobox.Items.Add("0");
            faHydroxyCombobox.Items.Add("1");
            faHydroxyCombobox.Items.Add("2");
            faHydroxyCombobox.Items.Add("3");
            
            
            
            wizardPictureBox.Image = Image.FromFile(Path.Combine(creatorGUI.lipidCreator.prefixPath, "images", "wizard_banner.png"));
            wizardPictureBox.Location = new Point(0, 0);
            wizardPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            wizardPictureBox.BackColor = Color.White;
            
            String dbText = "No. DB";
            String hydroxylText = "No. Hydroxy";
            int dbLength = 70;
            int sep = 20;
            int sepText = 20;
            int faLength = 150;
            string formattingFA = "Comma seperated single entries or intervals. Example formatting: 2, 3, 5-6, 13-20";
            string formattingDB = "Comma seperated single entries or intervals. Example formatting: 2, 3-4, 6";
            string formattingHydroxyl = "Comma seperated single entries or intervals. Example formatting: 2-4, 10, 12";
            string FApInformation = "Plasmenyl fatty acids need at least one double bond";
            string repFAText = "All fatty acyl parameters will be copied from the first FA to all remaining FAs";

            
            this.SuspendLayout();
            // 
            // button1
            // 
            this.cancelButton.Location = new System.Drawing.Point(12, 323);
            this.cancelButton.Name = "cancel";
            this.cancelButton.Size = new System.Drawing.Size(101, 32);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelClick);
            // 
            // button2
            // 
            this.continueButton.Location = new System.Drawing.Point(413, 323);
            this.continueButton.Name = "continue";
            this.continueButton.Size = new System.Drawing.Size(101, 32);
            this.continueButton.TabIndex = 0;
            this.continueButton.Text = "Continue";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueClick);
            
            this.backButton.Location = new System.Drawing.Point(306, 323);
            this.backButton.Name = "back";
            this.backButton.Size = new System.Drawing.Size(101, 32);
            this.backButton.TabIndex = 2;
            this.backButton.Text = "Back";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backClick);
            
            this.labelInformation.AutoSize = true;
            this.labelInformation.Location = new System.Drawing.Point(12, 90);
            this.labelInformation.Name = "labelInformation";
            this.labelInformation.Size = new System.Drawing.Size(500, 14);
            this.labelInformation.TabIndex = 3;
            
            
            Font titleFont = new Font(labelTitle.Font.FontFamily, (CreatorGUI.REGULAR_FONT_SIZE + 3) * CreatorGUI.FONT_SIZE_FACTOR, FontStyle.Bold);
            
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = titleFont;
            this.labelTitle.Location = new System.Drawing.Point(12, 70);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(500, 14);
            this.labelTitle.TabIndex = 4;
            
            // 
            // Wizard
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(526, 367);
            this.Controls.Add(this.labelInformation);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.categoryCombobox);
            this.Controls.Add(this.hgCombobox);
            this.Controls.Add(this.faCombobox);
            this.Controls.Add(this.faHydroxyCombobox);
            this.Controls.Add(this.lcbHydroxyCombobox);
            this.Controls.Add(this.faCheckbox1);
            this.Controls.Add(this.faCheckbox2);
            this.Controls.Add(this.faCheckbox3);
            this.Controls.Add(this.faTextbox);
            this.Controls.Add(this.dbTextbox);
            this.Controls.Add(this.dbLabel);
            this.Controls.Add(this.hydroxylTextbox);
            this.Controls.Add(this.hydroxylLabel);
            this.Controls.Add(this.posAdductCheckbox1);
            this.Controls.Add(this.posAdductCheckbox2);
            this.Controls.Add(this.posAdductCheckbox3);
            this.Controls.Add(this.negAdductCheckbox1);
            this.Controls.Add(this.negAdductCheckbox2);
            this.Controls.Add(this.negAdductCheckbox3);
            this.Controls.Add(this.negAdductCheckbox4);
            this.Controls.Add(this.positiveAdduct);
            this.Controls.Add(this.negativeAdduct);
            this.Controls.Add(this.filterGroupbox);
            this.Controls.Add(this.checkedListBoxPositiveFragments);
            this.Controls.Add(this.checkedListBoxNegativeFragments);
            this.Controls.Add(this.labelPositiveFragments);
            this.Controls.Add(this.labelNegativeFragments);
            this.Controls.Add(this.labelPositiveSelectAll);
            this.Controls.Add(this.labelPositiveDeselectAll);
            this.Controls.Add(this.labelNegativeSelectAll);
            this.Controls.Add(this.labelNegativeDeselectAll);
            this.Controls.Add(this.labelSlashPositive);
            this.Controls.Add(this.labelSlashNegative);
            this.Controls.Add(this.wizardPictureBox);
            this.Controls.Add(this.labelLine);
            this.Controls.Add(this.faRepresentative);
            this.Controls.Add(this.lipidPreview);
            
            this.Name = "MagicLipidWizard";
            this.Text = "Magic Lipid Wizard";
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Font = new Font(Font.Name, CreatorGUI.REGULAR_FONT_SIZE * CreatorGUI.FONT_SIZE_FACTOR, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.FormClosing += new FormClosingEventHandler(closing);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            
            hgCombobox.Size = new Size(180, 20);
            hgCombobox.Location = new Point((ClientSize.Width - hgCombobox.Width) >> 1, 184);
            hgCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            hgCombobox.SelectedIndexChanged += new System.EventHandler(hgComboboxSelectedValueChanged);
            
            labelLine.AutoSize = false;
            labelLine.Height = 2;
            labelLine.BorderStyle = BorderStyle.Fixed3D;
            labelLine.Location = new System.Drawing.Point(12, 312);
            labelLine.Width = 504;
            
            
            categoryCombobox.Items.Add("Glycerolipids");
            categoryCombobox.Items.Add("Glycerophospholipids");
            categoryCombobox.Items.Add("Sphingolipids");
            categoryCombobox.Items.Add("Sterol lipids");
            categoryCombobox.Items.Add("Lipid Mediators");
            
            categoryCombobox.Size = new System.Drawing.Size(180, 20);
            categoryCombobox.Location = new Point((ClientSize.Width - categoryCombobox.Width) >> 1, 184);
            categoryCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            categoryCombobox.SelectedIndex = 0;
            
            
            toolTip = new ToolTip();
            
            
            
            faTextbox.Location = new Point(100, 164);
            faTextbox.Width = faLength;
            faTextbox.Text = "12-15";
            faTextbox.TextChanged += delegate(object s, EventArgs e){ creatorGUI.updateCarbon(s, new FattyAcidEventArgs(fag, "" )); };
            toolTip.SetToolTip(faTextbox, formattingFA);
            faCombobox.Location = new Point(faTextbox.Left, faTextbox.Top - sepText);
            faCombobox.Width = faLength;
            faCombobox.SelectedItem = "Fatty acyl chain";
            faCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            faCombobox.SelectedIndexChanged += delegate(object s, EventArgs e){ creatorGUI.updateOddEven(s, new FattyAcidEventArgs(fag, faTextbox )); };
            dbTextbox.Location = new Point(faTextbox.Left + faTextbox.Width + sep, faTextbox.Top);
            dbTextbox.Width = dbLength;
            dbTextbox.Text = "0";
            dbTextbox.TextChanged += delegate(object s, EventArgs e){ creatorGUI.updateDB(s, new FattyAcidEventArgs(fag, "" )); };
            toolTip.SetToolTip(dbTextbox, formattingDB);
            dbLabel.Location = new Point(dbTextbox.Left, dbTextbox.Top - sep);
            dbLabel.Width = dbLength;
            dbLabel.Text = dbText;
            hydroxylTextbox.Width = dbLength;
            hydroxylTextbox.Location = new Point(dbTextbox.Left + dbTextbox.Width + sep, dbTextbox.Top);
            hydroxylTextbox.Text = "0";
            hydroxylTextbox.TextChanged += delegate(object s, EventArgs e){ creatorGUI.updateHydroxyl(s, new FattyAcidEventArgs(fag, "" )); };
            toolTip.SetToolTip(hydroxylTextbox, formattingHydroxyl);
            hydroxylLabel.Width = dbLength;
            hydroxylLabel.Location = new Point(hydroxylTextbox.Left, hydroxylTextbox.Top - sep);
            hydroxylLabel.Text = hydroxylText;

            faCheckbox3.Location = new Point(faTextbox.Left + 100, faTextbox.Top + faTextbox.Height);
            faCheckbox3.Text = "FAa";
            faCheckbox3.Width = 45;
            faCheckbox3.CheckedChanged += delegate(object s, EventArgs e){ creatorGUI.FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs(fag, "FAa" )); };
            faCheckbox2.Location = new Point(faTextbox.Left + 50, faTextbox.Top + faTextbox.Height);
            faCheckbox2.Text = "FAp";
            faCheckbox2.Width = 45;
            toolTip.SetToolTip(faCheckbox2, FApInformation);
            faCheckbox2.CheckedChanged += delegate(object s, EventArgs e){ creatorGUI.FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs(fag, "FAp" )); };
            faCheckbox1.Location = new Point(faTextbox.Left, faTextbox.Top + faTextbox.Height);
            faCheckbox1.Text = "FA";
            faCheckbox1.Width = 45;
            faCheckbox1.CheckedChanged += delegate(object s, EventArgs e){ creatorGUI.FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs(fag, "FA" )); };
            faCheckbox1.Checked = true;
            
            faHydroxyCombobox.Location = new Point(dbTextbox.Left + dbTextbox.Width + sep, dbTextbox.Top);
            faHydroxyCombobox.SelectedItem = "2";
            faHydroxyCombobox.Width = dbLength;
            faHydroxyCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            faHydroxyCombobox.SelectedIndexChanged += new EventHandler(faHydroxyComboboxValueChanged);
            
            lcbHydroxyCombobox.Location = new Point(dbTextbox.Left + dbTextbox.Width + sep, dbTextbox.Top);
            lcbHydroxyCombobox.SelectedItem = "2";
            lcbHydroxyCombobox.Width = dbLength;
            lcbHydroxyCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            lcbHydroxyCombobox.SelectedIndexChanged += new EventHandler(lcbHydroxyComboboxValueChanged);
            
            
            positiveAdduct.Width = 120;
            positiveAdduct.Location = new Point(120, 144);
            positiveAdduct.Height = 120;
            positiveAdduct.Text = "Positive adducts";
            posAdductCheckbox1.Parent = positiveAdduct;
            posAdductCheckbox1.Location = new Point(10, 15);
            posAdductCheckbox1.Text = "+H⁺";
            posAdductCheckbox1.CheckedChanged += delegate(object s, EventArgs e){creatorGUI.AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+H", lipid));};
            posAdductCheckbox2.Parent = positiveAdduct;
            posAdductCheckbox2.Location = new Point(10, 35);
            posAdductCheckbox2.Text = "+2H⁺⁺";
            posAdductCheckbox2.CheckedChanged += delegate(object s, EventArgs e){creatorGUI.AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+2H", lipid));};
            posAdductCheckbox3.Parent = positiveAdduct;
            posAdductCheckbox3.Location = new Point(10, 55);
            posAdductCheckbox3.Text = "+NH4⁺";
            posAdductCheckbox3.CheckedChanged += delegate(object s, EventArgs e){creatorGUI.AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+NH4", lipid));};
            negativeAdduct.Width = 120;
            negativeAdduct.Location = new Point(280, 144);
            negativeAdduct.Height = 120;
            negativeAdduct.Text = "Negative adducts";
            negAdductCheckbox1.Parent = negativeAdduct;
            negAdductCheckbox1.Location = new Point(10, 15);
            negAdductCheckbox1.Text = "-H⁻";
            negAdductCheckbox1.CheckedChanged += delegate(object s, EventArgs e){creatorGUI.AdductCheckBoxChecked(s, new AdductCheckedEventArgs("-H", lipid));};
            negAdductCheckbox2.Parent = negativeAdduct;
            negAdductCheckbox2.Location = new Point(10, 35);
            negAdductCheckbox2.Text = "-2H⁻ ⁻";
            negAdductCheckbox2.CheckedChanged += delegate(object s, EventArgs e){creatorGUI.AdductCheckBoxChecked(s, new AdductCheckedEventArgs("-2H", lipid));};
            negAdductCheckbox3.Parent = negativeAdduct;
            negAdductCheckbox3.Location = new Point(10, 55);
            negAdductCheckbox3.Text = "+HCOO⁻";
            negAdductCheckbox3.CheckedChanged += delegate(object s, EventArgs e){creatorGUI.AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+HCOO", lipid));};
            negAdductCheckbox4.Parent = negativeAdduct;
            negAdductCheckbox4.Location = new Point(10, 75);
            negAdductCheckbox4.Text = "+CH3COO⁻";
            negAdductCheckbox4.CheckedChanged += delegate(object s, EventArgs e){creatorGUI.AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+CH3COO", lipid));};
            
            
            
            // 
            // filterGroupbox
            // 
            this.filterGroupbox.Controls.Add(this.withPrecursorRadiobutton);
            this.filterGroupbox.Controls.Add(this.onlyPrecursorRadiobutton);
            this.filterGroupbox.Controls.Add(this.noPrecursorRadiobutton);
            this.filterGroupbox.Name = "filterGroupbox";
            this.filterGroupbox.Size = new System.Drawing.Size(260, 93);
            this.filterGroupbox.Location = new System.Drawing.Point((ClientSize.Width - filterGroupbox.Width) >> 1, 144);
            this.filterGroupbox.TabIndex = 4;
            this.filterGroupbox.TabStop = false;
            this.filterGroupbox.Text = "Precursor filter";
            // 
            // withPrecursorRadiobutton
            // 
            this.withPrecursorRadiobutton.AutoSize = true;
            this.withPrecursorRadiobutton.Location = new System.Drawing.Point(6, 65);
            this.withPrecursorRadiobutton.Name = "withPrecursorRadiobutton";
            this.withPrecursorRadiobutton.Size = new System.Drawing.Size(186, 17);
            this.withPrecursorRadiobutton.TabIndex = 2;
            this.withPrecursorRadiobutton.TabStop = true;
            this.withPrecursorRadiobutton.Text = "Compute with precursor transitions";
            this.withPrecursorRadiobutton.UseVisualStyleBackColor = true;
            this.withPrecursorRadiobutton.CheckedChanged += new EventHandler(filterChanged);
            this.withPrecursorRadiobutton.Checked = true;
            // 
            // onlyPrecursorRadiobutton
            // 
            this.onlyPrecursorRadiobutton.AutoSize = true;
            this.onlyPrecursorRadiobutton.Location = new System.Drawing.Point(6, 42);
            this.onlyPrecursorRadiobutton.Name = "onlyPrecursorRadiobutton";
            this.onlyPrecursorRadiobutton.Size = new System.Drawing.Size(186, 17);
            this.onlyPrecursorRadiobutton.TabIndex = 1;
            this.onlyPrecursorRadiobutton.TabStop = true;
            this.onlyPrecursorRadiobutton.Text = "Compute only precursor transitions";
            this.onlyPrecursorRadiobutton.UseVisualStyleBackColor = true;
            this.onlyPrecursorRadiobutton.CheckedChanged += new EventHandler(filterChanged);
            // 
            // noPrecursorRadiobutton
            // 
            this.noPrecursorRadiobutton.AutoSize = true;
            this.noPrecursorRadiobutton.Location = new System.Drawing.Point(6, 19);
            this.noPrecursorRadiobutton.Name = "noPrecursorRadiobutton";
            this.noPrecursorRadiobutton.Size = new System.Drawing.Size(179, 17);
            this.noPrecursorRadiobutton.TabIndex = 0;
            this.noPrecursorRadiobutton.TabStop = true;
            this.noPrecursorRadiobutton.Text = "Compute no precursor transitions";
            this.noPrecursorRadiobutton.UseVisualStyleBackColor = true;
            this.noPrecursorRadiobutton.CheckedChanged += new EventHandler(filterChanged);
            
            
            
            this.checkedListBoxPositiveFragments.CheckOnClick = true;
            this.checkedListBoxPositiveFragments.FormattingEnabled = true;
            this.checkedListBoxPositiveFragments.Location = new System.Drawing.Point(12, 132);
            this.checkedListBoxPositiveFragments.Name = "checkedListBoxPositive";
            this.checkedListBoxPositiveFragments.ScrollAlwaysVisible = true;
            this.checkedListBoxPositiveFragments.Size = new System.Drawing.Size(240, 160);
            this.checkedListBoxPositiveFragments.TabIndex = 2;
            this.checkedListBoxPositiveFragments.ThreeDCheckBoxes = true;
            this.checkedListBoxPositiveFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxPositiveItemCheck);
            
            
            this.checkedListBoxNegativeFragments.CheckOnClick = true;
            this.checkedListBoxNegativeFragments.FormattingEnabled = true;
            this.checkedListBoxNegativeFragments.Location = new System.Drawing.Point(280, 132);
            this.checkedListBoxNegativeFragments.Name = "checkedListBoxNegative";
            this.checkedListBoxNegativeFragments.ScrollAlwaysVisible = true;
            this.checkedListBoxNegativeFragments.Size = new System.Drawing.Size(240, 160);
            this.checkedListBoxNegativeFragments.TabIndex = 5;
            this.checkedListBoxNegativeFragments.ThreeDCheckBoxes = true;
            this.checkedListBoxNegativeFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxNegativeItemCheck);
            
            
            this.labelPositiveFragments.AutoSize = true;
            this.labelPositiveFragments.Location = new System.Drawing.Point(12, 116);
            this.labelPositiveFragments.Name = "label1";
            this.labelPositiveFragments.TabIndex = 3;
            this.labelPositiveFragments.Text = "Positive Fragments";
            
            this.labelNegativeFragments.AutoSize = true;
            this.labelNegativeFragments.Location = new System.Drawing.Point(280, 116);
            this.labelNegativeFragments.Name = "label2";
            this.labelNegativeFragments.TabIndex = 4;
            this.labelNegativeFragments.Text = "Negative Fragments";
            
            
            
            
            this.labelPositiveSelectAll.AutoSize = true;
            this.labelPositiveSelectAll.Location = new System.Drawing.Point(12, 294);
            this.labelPositiveSelectAll.Name = "label6";
            this.labelPositiveSelectAll.TabIndex = 6;
            this.labelPositiveSelectAll.Text = "select all";
            this.labelPositiveSelectAll.ForeColor = Color.FromArgb(0, 0, 255);
            this.labelPositiveSelectAll.Click += new System.EventHandler(checkedListBoxPositiveSelectAll);
            
            this.labelNegativeSelectAll.AutoSize = true;
            this.labelNegativeSelectAll.Location = new System.Drawing.Point(280, 294);
            this.labelNegativeSelectAll.Name = "label8";
            this.labelNegativeSelectAll.TabIndex = 6;
            this.labelNegativeSelectAll.Text = "select all";
            this.labelNegativeSelectAll.ForeColor = Color.FromArgb(0, 0, 255);
            this.labelNegativeSelectAll.Click += new System.EventHandler(checkedListBoxNegativeSelectAll);
            
            this.labelSlashPositive.AutoSize = true;
            this.labelSlashPositive.Location = new System.Drawing.Point(labelPositiveSelectAll.Left + 55, labelPositiveSelectAll.Top);
            this.labelSlashPositive.Name = "label10";
            this.labelSlashPositive.Text = "/";
            
            this.labelSlashNegative.AutoSize = true;
            this.labelSlashNegative.Location = new System.Drawing.Point(labelNegativeSelectAll.Left + 55, labelNegativeSelectAll.Top);
            this.labelSlashNegative.Name = "label11";
            this.labelSlashNegative.Text = "/";
            
            this.labelPositiveDeselectAll.AutoSize = true;
            this.labelPositiveDeselectAll.Location = new System.Drawing.Point(labelSlashPositive.Left + 7, labelPositiveSelectAll.Top);
            this.labelPositiveDeselectAll.Name = "label7";
            this.labelPositiveDeselectAll.TabIndex = 6;
            this.labelPositiveDeselectAll.Text = "deselect all";
            this.labelPositiveDeselectAll.ForeColor = Color.FromArgb(0, 0, 255);
            this.labelPositiveDeselectAll.Click += new System.EventHandler(checkedListBoxPositiveDeselectAll);
            
            
            this.labelNegativeDeselectAll.AutoSize = true;
            this.labelNegativeDeselectAll.Location = new System.Drawing.Point(labelSlashNegative.Left + 7, labelNegativeSelectAll.Top);
            this.labelNegativeDeselectAll.Name = "label9";
            this.labelNegativeDeselectAll.TabIndex = 6;
            this.labelNegativeDeselectAll.Text = "deselect all";
            this.labelNegativeDeselectAll.ForeColor = Color.FromArgb(0, 0, 255);
            this.labelNegativeDeselectAll.Click += new System.EventHandler(checkedListBoxNegativeDeselectAll);
            
            this.lipidPreview.AllowUserToAddRows = false;
            this.lipidPreview.AllowUserToDeleteRows = false;
            this.lipidPreview.AllowUserToResizeColumns = false;
            this.lipidPreview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.lipidPreview.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.None;
            this.lipidPreview.AllowUserToResizeRows = false;
            this.lipidPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lipidPreview.Size = new System.Drawing.Size(350, 180);
            this.lipidPreview.Location = new System.Drawing.Point((ClientSize.Width - lipidPreview.Width) >> 1, 122);
            this.lipidPreview.Name = "dataGridView1";
            this.lipidPreview.ReadOnly = true;
            this.lipidPreview.RowHeadersVisible = false;
            this.lipidPreview.ColumnHeadersVisible = false;
            this.lipidPreview.TabIndex = 0;
            this.lipidPreview.DataSource = lipidDataTable;
            
            lipidDataTable.Columns.Add(new DataColumn("Key"));
            lipidDataTable.Columns.Add(new DataColumn("Value"));
            
            
            this.faRepresentative.Location = new Point(faCheckbox1.Left, faCheckbox1.Top + 40);
            this.faRepresentative.Width = 150;
            this.faRepresentative.Text = "First FA representative";
            toolTip.SetToolTip(this.faRepresentative, repFAText);
            

            controlElements = new ArrayList(){categoryCombobox, hgCombobox, faCombobox, faCheckbox1, faCheckbox2, faCheckbox3, faTextbox, dbTextbox, dbLabel, hydroxylTextbox, hydroxylLabel, positiveAdduct, negativeAdduct, filterGroupbox, faHydroxyCombobox, lcbHydroxyCombobox, checkedListBoxPositiveFragments, checkedListBoxNegativeFragments, labelPositiveFragments, labelNegativeFragments, labelPositiveSelectAll, labelPositiveDeselectAll, labelNegativeSelectAll, labelNegativeDeselectAll, labelSlashPositive, labelSlashNegative, faRepresentative, lipidPreview};
        }
        
        
        
        private void InitializeCustom()
        {
            this.SuspendLayout();
            this.Font = new Font(Font.Name, CreatorGUI.REGULAR_FONT_SIZE * CreatorGUI.FONT_SIZE_FACTOR, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        public DataTable lipidDataTable;
        [NonSerialized]
        public System.Windows.Forms.DataGridView lipidPreview;
        [NonSerialized]
        public System.Windows.Forms.Button cancelButton;
        [NonSerialized]
        public System.Windows.Forms.Button continueButton;
        [NonSerialized]
        public System.Windows.Forms.Button backButton;
        [NonSerialized]
        public System.Windows.Forms.Label labelInformation;
        [NonSerialized]
        public System.Windows.Forms.Label labelTitle;
        [NonSerialized]
        public System.Windows.Forms.Label labelLine;
        [NonSerialized]
        public System.Windows.Forms.ComboBox categoryCombobox;
        [NonSerialized]
        public System.Windows.Forms.ComboBox hgCombobox;
        [NonSerialized]
        public System.Windows.Forms.CheckBox faCheckbox1;
        [NonSerialized]
        public System.Windows.Forms.CheckBox faCheckbox2;
        [NonSerialized]
        public System.Windows.Forms.CheckBox faCheckbox3;
        [NonSerialized]
        public System.Windows.Forms.CheckBox faRepresentative;
        [NonSerialized]
        public System.Windows.Forms.ComboBox faCombobox;
        [NonSerialized]
        public System.Windows.Forms.ComboBox faHydroxyCombobox;
        [NonSerialized]
        public System.Windows.Forms.ComboBox lcbHydroxyCombobox;
        [NonSerialized]
        public System.Windows.Forms.TextBox faTextbox;
        [NonSerialized]
        public System.Windows.Forms.TextBox dbTextbox;
        [NonSerialized]
        public System.Windows.Forms.Label dbLabel;
        [NonSerialized]
        public System.Windows.Forms.TextBox hydroxylTextbox;
        [NonSerialized]
        public System.Windows.Forms.Label hydroxylLabel;
        [NonSerialized]
        ToolTip toolTip;
        
        [NonSerialized]
        public CheckBox posAdductCheckbox1;
        [NonSerialized]
        public CheckBox posAdductCheckbox2;
        [NonSerialized]
        public CheckBox posAdductCheckbox3;
        [NonSerialized]
        public CheckBox negAdductCheckbox1;
        [NonSerialized]
        public CheckBox negAdductCheckbox2;
        [NonSerialized]
        public CheckBox negAdductCheckbox3;
        [NonSerialized]
        public CheckBox negAdductCheckbox4;
        [NonSerialized]
        public GroupBox positiveAdduct;
        [NonSerialized]
        public GroupBox negativeAdduct;
        [NonSerialized]
        public GroupBox filterGroupbox;
        [NonSerialized]
        public RadioButton noPrecursorRadiobutton;
        [NonSerialized]
        public RadioButton onlyPrecursorRadiobutton;
        [NonSerialized]
        public RadioButton withPrecursorRadiobutton;
        [NonSerialized]
        public CheckedListBox checkedListBoxPositiveFragments;
        [NonSerialized]
        public CheckedListBox checkedListBoxNegativeFragments;
        [NonSerialized]
        public Label labelPositiveFragments;
        [NonSerialized]
        public Label labelNegativeFragments;
        [NonSerialized]
        public Label labelPositiveSelectAll;
        [NonSerialized]
        public Label labelPositiveDeselectAll;
        [NonSerialized]
        public Label labelNegativeSelectAll;
        [NonSerialized]
        public Label labelNegativeDeselectAll;
        [NonSerialized]
        public Label labelSlashPositive;
        [NonSerialized]
        public Label labelSlashNegative;
        [NonSerialized]
        public PictureBox wizardPictureBox;
        
        public ArrayList controlElements;
    }
}
