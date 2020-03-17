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
            this.labelInformation = new System.Windows.Forms.Label();
            this.categoryCombobox = new System.Windows.Forms.ComboBox();
            this.hgListbox = new System.Windows.Forms.ListBox();
            this.faCombobox = new System.Windows.Forms.ComboBox();
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
            
            
            String dbText = "No. DB";
            String hydroxylText = "No. Hydroxy";
            int dbLength = 70;
            int sep = 15;
            int sepText = 20;
            int faLength = 150;
            string formattingFA = "Comma seperated single entries or intervals. Example formatting: 2, 3, 5-6, 13-20";
            string formattingDB = "Comma seperated single entries or intervals. Example formatting: 2, 3-4, 6";
            string formattingHydroxyl = "Comma seperated single entries or intervals. Example formatting: 2-4, 10, 12";
            string FApInformation = "Plasmenyl fatty acids need at least one double bond";
            
            this.SuspendLayout();
            // 
            // button1
            // 
            this.cancelButton.Location = new System.Drawing.Point(306, 239);
            this.cancelButton.Name = "cancel";
            this.cancelButton.Size = new System.Drawing.Size(101, 32);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelClick);
            // 
            // button2
            // 
            this.continueButton.Location = new System.Drawing.Point(413, 239);
            this.continueButton.Name = "add";
            this.continueButton.Size = new System.Drawing.Size(101, 32);
            this.continueButton.TabIndex = 0;
            this.continueButton.Text = "Continue";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueClick);
            // 
            // label1
            // 
            this.labelInformation.AutoSize = true;
            this.labelInformation.Location = new System.Drawing.Point(12, 12);
            this.labelInformation.Name = "label1";
            this.labelInformation.Size = new System.Drawing.Size(500, 14);
            this.labelInformation.TabIndex = 3;
            
            // 
            // Wizard
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(526, 283);
            this.Controls.Add(this.labelInformation);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.categoryCombobox);
            this.Controls.Add(this.hgListbox);
            this.Controls.Add(this.faCombobox);
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
            
            this.Name = "MagicLipidWizard";
            this.Text = "Magic Lipid Wizard";
            this.ResumeLayout(false);
            this.PerformLayout();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            
            hgListbox.Size = new Size(180, 160);
            hgListbox.Location = new Point((ClientSize.Width - hgListbox.Width) >> 1, 40);
            hgListbox.BringToFront();
            hgListbox.BorderStyle = BorderStyle.Fixed3D;
            hgListbox.SelectionMode = SelectionMode.One;
            hgListbox.SelectedValueChanged += new System.EventHandler(hgListboxSelectedValueChanged);
            
            
            categoryCombobox.Items.Add("Glycero lipid");
            categoryCombobox.Items.Add("Glycerophosho lipid");
            categoryCombobox.Items.Add("Sphingo lipid");
            categoryCombobox.Items.Add("Sterol lipid");
            categoryCombobox.Items.Add("Lipid mediator");
            
            categoryCombobox.Size = new System.Drawing.Size(180, 20);
            categoryCombobox.Location = new Point((ClientSize.Width - categoryCombobox.Width) >> 1, 100);
            categoryCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            categoryCombobox.SelectedIndex = 0;
            
            
            toolTip = new ToolTip();
            
            faCombobox.Items.Add("Fatty acyl chain");
            faCombobox.Items.Add("Fatty acyl chain - odd");
            faCombobox.Items.Add("Fatty acyl chain - even");
            
            
            faTextbox.Location = new Point(100, 80);
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
            
            
            
            positiveAdduct.Width = 120;
            positiveAdduct.Location = new Point(120, 60);
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
            negativeAdduct.Location = new Point(280, 60);
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
            
            
            
            

            controlElements = new ArrayList(){categoryCombobox, hgListbox, faCombobox, faCheckbox1, faCheckbox2, faCheckbox3, faTextbox, dbTextbox, dbLabel, hydroxylTextbox, hydroxylLabel, positiveAdduct, negativeAdduct};
        }
        
        
        
        private void InitializeCustom()
        {
            this.SuspendLayout();
            this.Font = new Font(Font.Name, CreatorGUI.REGULAR_FONT_SIZE * CreatorGUI.FONT_SIZE_FACTOR, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        [NonSerialized]
        public System.Windows.Forms.Button cancelButton;
        [NonSerialized]
        public System.Windows.Forms.Button continueButton;
        [NonSerialized]
        public System.Windows.Forms.Label labelInformation;
        [NonSerialized]
        public System.Windows.Forms.ComboBox categoryCombobox;
        [NonSerialized]
        public System.Windows.Forms.ListBox hgListbox;
        [NonSerialized]
        public System.Windows.Forms.CheckBox faCheckbox1;
        [NonSerialized]
        public System.Windows.Forms.CheckBox faCheckbox2;
        [NonSerialized]
        public System.Windows.Forms.CheckBox faCheckbox3;
        [NonSerialized]
        public System.Windows.Forms.ComboBox faCombobox;
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
        
        public ArrayList controlElements;
    }
}