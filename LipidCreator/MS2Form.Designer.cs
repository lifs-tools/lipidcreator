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
    partial class MS2Form
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
        private void InitializeComponent(Dictionary<String, ArrayList> MS2Fragments)
        {
            this.Size = new System.Drawing.Size(1168, 447);
        
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPages = new ArrayList();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(340, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(736, 358);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // checkedListBox1 - positive fragments
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(12, 22);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.ScrollAlwaysVisible = true;
            this.checkedListBox1.Size = new System.Drawing.Size(150, 294);
            this.checkedListBox1.TabIndex = 2;
            this.checkedListBox1.ThreeDCheckBoxes = true;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox1_ItemCheck);
            this.checkedListBox1.MouseLeave += new System.EventHandler(this.checkedListBox_MouseLeave);
            this.checkedListBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkedListBox1_MouseHover);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 2);
            this.label1.Name = "label1";
            this.label1.TabIndex = 3;
            this.label1.Text = "Positive Fragments";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(170, 2);
            this.label2.Name = "label2";
            this.label2.TabIndex = 4;
            this.label2.Text = "Negative Fragments";
            // 
            // label3
            // 
            this.label3.Size = new Size(200, 13);
            this.label3.Location = new System.Drawing.Point(1110 - label3.Width, 280);
            this.label3.Name = "label3";
            this.label3.TabIndex = 5;
            this.label3.Text = "black: unspecific";
            this.label3.TextAlign = ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.Size = new Size(200, 13);
            this.label4.Location = new System.Drawing.Point(1110 - label4.Width, 295);
            this.label4.Name = "label4";
            this.label4.TabIndex = 5;
            this.label4.Text = "red: specific for lipid category";
            this.label4.ForeColor = Color.FromArgb(227, 5, 19);
            this.label4.TextAlign = ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.Size = new Size(200, 13);
            this.label5.Location = new System.Drawing.Point(1110 - label5.Width, 310);
            this.label5.Name = "label5";
            this.label5.TabIndex = 5;
            this.label5.Text = "blue: specific for lipid class";
            this.label5.ForeColor = Color.FromArgb(48, 38, 131);
            this.label5.TextAlign = ContentAlignment.TopRight;
            // 
            // label6 - positive fragments
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 316);
            this.label6.Name = "label6";
            this.label6.TabIndex = 6;
            this.label6.Text = "select all";
            this.label6.ForeColor = Color.FromArgb(0, 0, 255);
            this.label6.Click += new System.EventHandler(checkedListBoxPositiveSelectAll);
            // 
            // label7 - positive fragments
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(74, 316);
            this.label7.Name = "label7";
            this.label7.TabIndex = 6;
            this.label7.Text = "deselect all";
            this.label7.ForeColor = Color.FromArgb(0, 0, 255);
            this.label7.Click += new System.EventHandler(checkedListBoxPositiveDeselectAll);
            // 
            // label8 - positive fragments
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(170, 316);
            this.label8.Name = "label8";
            this.label8.TabIndex = 6;
            this.label8.Text = "select all";
            this.label8.ForeColor = Color.FromArgb(0, 0, 255);
            this.label8.Click += new System.EventHandler(checkedListBoxNegativeSelectAll);
            // 
            // label9 - positive fragments
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(232, 316);
            this.label9.Name = "label9";
            this.label9.TabIndex = 6;
            this.label9.Text = "deselect all";
            this.label9.ForeColor = Color.FromArgb(0, 0, 255);
            this.label9.Click += new System.EventHandler(checkedListBoxNegativeDeselectAll);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(67, 316);
            this.label10.Name = "label10";
            this.label10.Text = "/";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(225, 316);
            this.label11.Name = "label11";
            this.label11.Text = "/";
            
            // 
            // checkedListBox2 - negative fragments
            // 
            this.checkedListBox2.CheckOnClick = true;
            this.checkedListBox2.FormattingEnabled = true;
            this.checkedListBox2.Location = new System.Drawing.Point(170, 22);
            this.checkedListBox2.Name = "checkedListBox2";
            this.checkedListBox2.ScrollAlwaysVisible = true;
            this.checkedListBox2.Size = new System.Drawing.Size(150, 294);
            this.checkedListBox2.TabIndex = 5;
            this.checkedListBox2.ThreeDCheckBoxes = true;
            this.checkedListBox2.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox2_ItemCheck);
            this.checkedListBox2.MouseLeave += new System.EventHandler(this.checkedListBox_MouseLeave);
            this.checkedListBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkedListBox2_MouseHover);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1058, 374);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(98, 30);
            this.button2.TabIndex = 7;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.cancel_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(954, 374);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(98, 30);
            this.button3.TabIndex = 8;
            this.button3.Text = "OK";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.ok_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(12, 374);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(98, 30);
            this.button4.TabIndex = 9;
            this.button4.Text = "Add fragment";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.add_fragment_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Location = new System.Drawing.Point(12, 6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1144, 360);
            this.tabControl1.TabIndex = 11;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(tabIndexChanged);
            // 
            // MS2Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1168, 420);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button4);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            
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
                this.tabControl1.Controls.Add(tp);
                this.tabPages.Add(tp);

            }

            this.Name = "MS2Form";
            this.Text = "MS2 Fragments";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            //this.ResumeLayout(false);
            //this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.CheckedListBox checkedListBox1;
        public Label label1;
        public Label label2;
        public Label label3;
        public Label label4;
        public Label label5;
        public Label label6;
        public Label label7;
        public Label label8;
        public Label label9;
        public Label label10;
        public Label label11;
        public CheckedListBox checkedListBox2;
        public Button button2;
        public Button button3;
        public Button button4;
        public TabControl tabControl1;
        public ArrayList tabPages;
    }
}