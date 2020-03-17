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
            
            this.SuspendLayout();
            // 
            // button1
            // 
            this.cancelButton.Location = new System.Drawing.Point(306, 239);
            this.cancelButton.Name = "cancel";
            this.cancelButton.Size = new System.Drawing.Size(101, 32);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelClick);
            // 
            // button2
            // 
            this.continueButton.Location = new System.Drawing.Point(413, 239);
            this.continueButton.Name = "add";
            this.continueButton.Size = new System.Drawing.Size(101, 32);
            this.continueButton.TabIndex = 1;
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
            this.Name = "MagicLipidWizard";
            this.Text = "Magic Lipid Wizard";
            this.ResumeLayout(false);
            this.PerformLayout();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            controlElements = new ArrayList(){};
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
        
        public ArrayList controlElements;
    }
}