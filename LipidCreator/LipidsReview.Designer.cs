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

namespace LipidCreator
{
    partial class LipidsReview
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
            this.dataGridViewTransitions = new System.Windows.Forms.DataGridView();
            this.buttonSendToSkyline = new System.Windows.Forms.Button();
            this.buttonStoreTransitionList = new System.Windows.Forms.Button();
            this.checkBoxHideReplicates = new System.Windows.Forms.CheckBox();
            this.checkBoxCreateSpectralLibrary = new System.Windows.Forms.CheckBox();
            this.buttonStoreSpectralLibrary = new System.Windows.Forms.Button();
            this.labelNumberOfTransitions = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTransitions)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridViewTransitions.AllowUserToAddRows = false;
            this.dataGridViewTransitions.AllowUserToDeleteRows = false;
            //this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridViewTransitions.AllowUserToResizeRows = false;
            this.dataGridViewTransitions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewTransitions.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewTransitions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTransitions.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewTransitions.Name = "dataGridView1";
            this.dataGridViewTransitions.ReadOnly = true;
            this.dataGridViewTransitions.RowHeadersVisible = false;
            this.dataGridViewTransitions.RowTemplate.Height = 34;
            this.dataGridViewTransitions.Size = new System.Drawing.Size(955, 409);
            this.dataGridViewTransitions.TabIndex = 0;
            // 
            // button1
            // 
            this.buttonSendToSkyline.Location = new System.Drawing.Point(360, 448);
            this.buttonSendToSkyline.Name = "button1";
            this.buttonSendToSkyline.Size = new System.Drawing.Size(258, 34);
            this.buttonSendToSkyline.TabIndex = 1;
            this.buttonSendToSkyline.Text = "Send to Skyline";
            this.buttonSendToSkyline.UseVisualStyleBackColor = true;
            this.buttonSendToSkyline.Click += new System.EventHandler(this.buttonSendToSkylineClick);
            // 
            // button2
            // 
            this.buttonStoreTransitionList.Location = new System.Drawing.Point(12, 448);
            this.buttonStoreTransitionList.Name = "button2";
            this.buttonStoreTransitionList.Size = new System.Drawing.Size(258, 34);
            this.buttonStoreTransitionList.TabIndex = 2;
            this.buttonStoreTransitionList.Text = "Store transition list";
            this.buttonStoreTransitionList.UseVisualStyleBackColor = true;
            this.buttonStoreTransitionList.Click += new System.EventHandler(this.buttonStoreTransitionListClick);
            // 
            // checkBox1
            // 
            this.checkBoxHideReplicates.AutoSize = true;
            this.checkBoxHideReplicates.Location = new System.Drawing.Point(12, 426);
            this.checkBoxHideReplicates.Name = "checkBox1";
            this.checkBoxHideReplicates.Size = new System.Drawing.Size(96, 17);
            this.checkBoxHideReplicates.TabIndex = 3;
            this.checkBoxHideReplicates.Text = "Show unique products only";
            this.checkBoxHideReplicates.UseVisualStyleBackColor = true;
            this.checkBoxHideReplicates.CheckedChanged += new System.EventHandler(this.checkBoxCheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBoxCreateSpectralLibrary.AutoSize = true;
            this.checkBoxCreateSpectralLibrary.Location = new System.Drawing.Point(450, 426);
            this.checkBoxCreateSpectralLibrary.Name = "checkBox2";
            this.checkBoxCreateSpectralLibrary.Size = new System.Drawing.Size(96, 17);
            this.checkBoxCreateSpectralLibrary.TabIndex = 3;
            this.checkBoxCreateSpectralLibrary.Text = "Create Spectral library";
            this.checkBoxCreateSpectralLibrary.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.buttonStoreSpectralLibrary.Location = new System.Drawing.Point(709, 448);
            this.buttonStoreSpectralLibrary.Name = "button3";
            this.buttonStoreSpectralLibrary.Size = new System.Drawing.Size(258, 34);
            this.buttonStoreSpectralLibrary.TabIndex = 4;
            this.buttonStoreSpectralLibrary.Text = "Store spectral library";
            this.buttonStoreSpectralLibrary.UseVisualStyleBackColor = true;
            this.buttonStoreSpectralLibrary.Click += new System.EventHandler(this.buttonStoreSpectralLibraryClick);
            // 
            // label1
            // 
            this.labelNumberOfTransitions.AutoSize = true;
            this.labelNumberOfTransitions.Location = new System.Drawing.Point(166, 426);
            this.labelNumberOfTransitions.Name = "label1";
            this.labelNumberOfTransitions.Size = new System.Drawing.Size(109, 13);
            this.labelNumberOfTransitions.TabIndex = 5;
            this.labelNumberOfTransitions.Text = "Number of transitions:";
            this.labelNumberOfTransitions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LipidsReview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(979, 494);
            this.Controls.Add(this.labelNumberOfTransitions);
            this.Controls.Add(this.buttonStoreSpectralLibrary);
            this.Controls.Add(this.checkBoxHideReplicates);
            this.Controls.Add(this.checkBoxCreateSpectralLibrary);
            this.Controls.Add(this.buttonStoreTransitionList);
            this.Controls.Add(this.buttonSendToSkyline);
            this.Controls.Add(this.dataGridViewTransitions);
            this.Name = "LipidsReview";
            this.Text = "LipidsReview";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTransitions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewTransitions;
        private System.Windows.Forms.Button buttonSendToSkyline;
        private Button buttonStoreTransitionList;
        private CheckBox checkBoxHideReplicates;
        private CheckBox checkBoxCreateSpectralLibrary;
        private Button buttonStoreSpectralLibrary;
        private Label labelNumberOfTransitions;
    }
}