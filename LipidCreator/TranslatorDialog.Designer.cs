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

namespace LipidCreator
{
    partial class TranslatorDialog
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            infoText = new Label();
            this.lipidNamesGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.lipidNamesGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(549, 378);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(630, 378);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Translate";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(711, 378);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Insert";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            
            
            infoText.Text = "Unsupported lipid: lipid is not suppored in the current version" + Environment.NewLine + "Unrecognized molecule: string can not be recognized as lipid name";
            infoText.Width = 500;
            infoText.Height = 60;
            infoText.Location = new Point(12, 358);
            
            
            lipidNamesGridView.DataSource = lipidNamesList;
            lipidNamesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            lipidNamesGridView.Location = new System.Drawing.Point(12, 12);
            lipidNamesGridView.Name = "lipidNamesGridView";
            lipidNamesGridView.Size = new System.Drawing.Size(774, 340);
            lipidNamesGridView.TabIndex = 3;
            lipidNamesGridView.AllowUserToAddRows = true;
            lipidNamesGridView.AllowUserToResizeRows = false;
            lipidNamesGridView.MultiSelect = false;
            lipidNamesGridView.RowTemplate.Height = 34;
            lipidNamesGridView.KeyDown += lipidNamesGridViewKeyDown;
            lipidNamesGridView.RowHeadersVisible = false;
            lipidNamesGridView.CellValueChanged += new DataGridViewCellEventHandler(lipidNamesGridViewCellValueChanged);
            lipidNamesGridView.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(lipidNamesGridViewEditingControlShowing);
            lipidNamesGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(lipidNamesGridViewDataBindingComplete);
            lipidNamesGridView.DoubleClick += new EventHandler(lipidsGridviewDoubleClick);


            // 
            // TranslatorDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(798, 413);
            this.Controls.Add(this.lipidNamesGridView);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(infoText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TranslatorDialog";
            this.Text = "Lipid names translation";
            ((System.ComponentModel.ISupportInitialize)(this.lipidNamesGridView)).EndInit();
            this.ResumeLayout(false);

        }
        private void InitializeCustom()
        {
            this.SuspendLayout();
            this.Font = new System.Drawing.Font(Font.Name, CreatorGUI.REGULAR_FONT_SIZE * CreatorGUI.FONT_SIZE_FACTOR, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.Button button3;
        public Label infoText;
        public DataGridView lipidNamesGridView;
    }
}