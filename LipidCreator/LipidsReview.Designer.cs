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

using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System;

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
        
            int windowWidth = 1100;
            int windowHeight = 580;
        
            this.dataGridViewTransitions = new System.Windows.Forms.DataGridView();
            this.buttonSendToSkyline = new System.Windows.Forms.Button();
            this.buttonStoreTransitionList = new System.Windows.Forms.Button();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCheckValues = new System.Windows.Forms.Button();
            this.checkBoxHideReplicates = new System.Windows.Forms.CheckBox();
            this.checkBoxEditMode = new System.Windows.Forms.CheckBox();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
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
            this.dataGridViewTransitions.AllowUserToResizeColumns = true;
            this.dataGridViewTransitions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewTransitions.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.None;
            this.dataGridViewTransitions.AllowUserToResizeRows = false;
            this.dataGridViewTransitions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTransitions.Location = new System.Drawing.Point(12, 58);
            this.dataGridViewTransitions.Name = "dataGridView1";
            this.dataGridViewTransitions.ReadOnly = true;
            this.dataGridViewTransitions.RowHeadersVisible = false;
            this.dataGridViewTransitions.RowTemplate.Height = 34;
            this.dataGridViewTransitions.Size = new System.Drawing.Size(1055, 409);
            this.dataGridViewTransitions.TabIndex = 0;
            this.dataGridViewTransitions.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(gridviewDataBindingComplete);
            this.dataGridViewTransitions.Sorted += new EventHandler(gridviewDataSorted);
            this.dataGridViewTransitions.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing; 
            this.dataGridViewTransitions.RowsAdded += new DataGridViewRowsAddedEventHandler(gridviewDataRowAdded);
            this.dataGridViewTransitions.RowsRemoved += new DataGridViewRowsRemovedEventHandler(gridviewDataRowRemoved);
            this.dataGridViewTransitions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            // 
            // button1
            // 
            this.buttonSendToSkyline.Location = new System.Drawing.Point(410, 494);
            this.buttonSendToSkyline.Name = "button1";
            this.buttonSendToSkyline.Size = new System.Drawing.Size(258, 34);
            this.buttonSendToSkyline.TabIndex = 1;
            this.buttonSendToSkyline.Text = "Send to Skyline";
            this.buttonSendToSkyline.UseVisualStyleBackColor = true;
            this.buttonSendToSkyline.Click += new System.EventHandler(this.buttonSendToSkylineClick);
            // 
            // button2
            // 
            this.buttonStoreTransitionList.Location = new System.Drawing.Point(12, 494);
            this.buttonStoreTransitionList.Name = "button2";
            this.buttonStoreTransitionList.Size = new System.Drawing.Size(258, 34);
            this.buttonStoreTransitionList.TabIndex = 2;
            this.buttonStoreTransitionList.Text = "Store transition list";
            this.buttonStoreTransitionList.UseVisualStyleBackColor = true;
            this.buttonStoreTransitionList.Click += new System.EventHandler(this.buttonStoreTransitionListClick);
            this.buttonStoreTransitionList.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Location = new System.Drawing.Point(120, 6);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(580, 40);
            this.groupBoxOptions.TabIndex = 4;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "Options";
            groupBoxOptions.Controls.Add(this.checkBoxEditMode);
            groupBoxOptions.Controls.Add(this.checkBoxHideReplicates);
            groupBoxOptions.Controls.Add(this.checkBoxCreateSpectralLibrary);
            // 
            // button3
            // 
            this.buttonBack.Location = new System.Drawing.Point(12, 12);
            this.buttonBack.Name = "button3";
            this.buttonBack.Size = new System.Drawing.Size(88, 34);
            this.buttonBack.TabIndex = 0;
            this.buttonBack.Text = "Back";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBackClick);
            // 
            // button4
            // 
            this.buttonCheckValues.Location = new System.Drawing.Point(900, 12);
            this.buttonCheckValues.Name = "button3";
            this.buttonCheckValues.Size = new System.Drawing.Size(168, 34);
            this.buttonCheckValues.TabIndex = 0;
            this.buttonCheckValues.Text = "Check transition list";
            this.buttonCheckValues.UseVisualStyleBackColor = true;
            this.buttonCheckValues.Click += new System.EventHandler(this.buttonCheckValuesClick);
            this.buttonCheckValues.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            // 
            // checkBox1
            // 
            this.checkBoxEditMode.AutoSize = true;
            this.checkBoxEditMode.Location = new System.Drawing.Point(12, 16);
            this.checkBoxEditMode.Name = "checkBox1";
            this.checkBoxEditMode.Size = new System.Drawing.Size(96, 17);
            this.checkBoxEditMode.TabIndex = 3;
            this.checkBoxEditMode.Text = "Edit mode";
            this.checkBoxEditMode.UseVisualStyleBackColor = true;
            this.checkBoxEditMode.CheckedChanged += new System.EventHandler(this.checkBoxEditModeChanged);
            // 
            // checkBox3
            // 
            this.checkBoxHideReplicates.AutoSize = true;
            //this.checkBoxHideReplicates.Location = new System.Drawing.Point(12, 472);
            this.checkBoxHideReplicates.Location = new System.Drawing.Point(130, 16);
            this.checkBoxHideReplicates.Name = "checkBox3";
            this.checkBoxHideReplicates.Size = new System.Drawing.Size(96, 17);
            this.checkBoxHideReplicates.TabIndex = 4;
            this.checkBoxHideReplicates.Text = "Only show unique transtions";
            this.checkBoxHideReplicates.UseVisualStyleBackColor = true;
            this.checkBoxHideReplicates.CheckedChanged += new System.EventHandler(this.checkBoxCheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBoxCreateSpectralLibrary.AutoSize = true;
            //this.checkBoxCreateSpectralLibrary.Location = new System.Drawing.Point(500, 472);
            this.checkBoxCreateSpectralLibrary.Location = new System.Drawing.Point(360, 16);
            this.checkBoxCreateSpectralLibrary.Name = "checkBox2";
            this.checkBoxCreateSpectralLibrary.Size = new System.Drawing.Size(96, 17);
            this.checkBoxCreateSpectralLibrary.TabIndex = 3;
            this.checkBoxCreateSpectralLibrary.Text = "Send spectral library to Skyline";
            this.checkBoxCreateSpectralLibrary.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.buttonStoreSpectralLibrary.Location = new System.Drawing.Point(809, 494);
            this.buttonStoreSpectralLibrary.Name = "button3";
            this.buttonStoreSpectralLibrary.Size = new System.Drawing.Size(258, 34);
            this.buttonStoreSpectralLibrary.TabIndex = 4;
            this.buttonStoreSpectralLibrary.Text = "Store spectral library";
            this.buttonStoreSpectralLibrary.UseVisualStyleBackColor = true;
            this.buttonStoreSpectralLibrary.Click += new System.EventHandler(this.buttonStoreSpectralLibraryClick);
            this.buttonStoreSpectralLibrary.Enabled = false;
            this.buttonStoreSpectralLibrary.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            // 
            // label1
            // 
            this.labelNumberOfTransitions.AutoSize = true;
            //this.labelNumberOfTransitions.Location = new System.Drawing.Point(246, 472);
            this.labelNumberOfTransitions.Location = new System.Drawing.Point(12, 472);
            this.labelNumberOfTransitions.Name = "label1";
            this.labelNumberOfTransitions.Size = new System.Drawing.Size(109, 13);
            this.labelNumberOfTransitions.TabIndex = 5;
            this.labelNumberOfTransitions.Text = "Number of transitions:";
            this.labelNumberOfTransitions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelNumberOfTransitions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            // 
            // LipidsReview
            //
            this.FormClosing += new FormClosingEventHandler(closingInteraction);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Size = new System.Drawing.Size(windowWidth, windowHeight);
            this.Resize += new EventHandler(formResize);
            this.MinimumSize = new System.Drawing.Size(windowWidth, windowHeight);
            this.Controls.Add(this.labelNumberOfTransitions);
            this.Controls.Add(this.buttonStoreSpectralLibrary);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCheckValues);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.buttonStoreTransitionList);
            this.Controls.Add(this.buttonSendToSkyline);
            this.Controls.Add(this.dataGridViewTransitions);
            this.Name = "LipidsReview";
            this.Text = "Lipid Transitions Review";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTransitions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            controlElements = new ArrayList(){buttonSendToSkyline, buttonBack, dataGridViewTransitions, buttonStoreTransitionList, checkBoxHideReplicates, checkBoxEditMode, checkBoxCreateSpectralLibrary, buttonStoreSpectralLibrary, buttonCheckValues};
            
            formResize(null, null);
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
        public System.Windows.Forms.DataGridView dataGridViewTransitions;
        [NonSerialized]
        public System.Windows.Forms.Button buttonSendToSkyline;
        [NonSerialized]
        public Button buttonStoreTransitionList;
        [NonSerialized]
        public Button buttonBack;
        [NonSerialized]
        public Button buttonCheckValues;
        [NonSerialized]
        public CheckBox checkBoxHideReplicates;
        [NonSerialized]
        public CheckBox checkBoxEditMode;
        [NonSerialized]
        public CheckBox checkBoxCreateSpectralLibrary;
        [NonSerialized]
        public Button buttonStoreSpectralLibrary;
        [NonSerialized]
        public Label labelNumberOfTransitions;
        public GroupBox groupBoxOptions;
        public ArrayList controlElements;
    }
}