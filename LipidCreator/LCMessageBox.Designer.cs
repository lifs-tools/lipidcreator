using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace LipidCreator
{   
    partial class LCMessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LCMessageBox));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.AutoSize = true;
            this.richTextBox1.Location = new System.Drawing.Point(13, 13);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Size = new System.Drawing.Size(484, 60);
            this.richTextBox1.BackColor = SystemColors.Control;
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Text = "123456";
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.GotFocus += new System.EventHandler(richTextBox1_Focus);
            this.richTextBox1.MouseEnter += new System.EventHandler(richTextBox1_MouseEnter);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(270, 60);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Merge";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.mergeClick);
            
            this.timerText = new System.Timers.Timer(150);
            this.timerText.Elapsed += this.timer_Elapsed;
            this.timerText.Enabled = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(390, 60);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Replace";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.replaceClick);
            // 
            // LCMessageBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(510, 93);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LCMessageBox";
            this.Text = "Merge or Replace?";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        public System.Timers.Timer timerText;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}