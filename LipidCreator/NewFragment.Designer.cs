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

namespace LipidCreator
{

    public class MyNumericUpDown : System.Windows.Forms.NumericUpDown
    {
        protected override void UpdateEditText()
        {
            base.UpdateEditText();
            ChangingText = true;
            if (Value > 0)
            {
                Text = "+" + Text;
            }
        }
    }

    partial class NewFragment
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
            this.addButton = new System.Windows.Forms.Button();
            this.labelMass = new System.Windows.Forms.Label();
            this.numericUpDownCharge = new MyNumericUpDown();
            this.textBoxFragmentName = new System.Windows.Forms.TextBox();
            this.labelCharge = new System.Windows.Forms.Label();
            this.labelFragmentName = new System.Windows.Forms.Label();
            this.labelSelectBase = new System.Windows.Forms.Label();
            this.groupboxAddingSubtracting = new System.Windows.Forms.GroupBox();
            this.radioButtonAdding = new System.Windows.Forms.RadioButton();
            this.radioButtonSubtracting = new System.Windows.Forms.RadioButton();
            selectBaseCombobox = new System.Windows.Forms.ComboBox();
            this.dataGridViewElements = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCharge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewElements)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.cancelButton.Location = new System.Drawing.Point(413, 239);
            this.cancelButton.Name = "cancel";
            this.cancelButton.Size = new System.Drawing.Size(101, 32);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelClick);
            // 
            // button2
            // 
            this.addButton.Location = new System.Drawing.Point(306, 239);
            this.addButton.Name = "add";
            this.addButton.Size = new System.Drawing.Size(101, 32);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addClick);
            // 
            // label1
            // 
            this.labelMass.AutoSize = true;
            this.labelMass.Location = new System.Drawing.Point(9, 228);
            this.labelMass.Name = "label1";
            this.labelMass.Size = new System.Drawing.Size(39, 10);
            this.labelMass.TabIndex = 3;
            this.labelMass.Text = "0.0 Da";
            // 
            // numericUpDown1
            // 
            this.numericUpDownCharge.Location = new System.Drawing.Point(453, 24);
            this.numericUpDownCharge.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownCharge.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            -2147483648});
            this.numericUpDownCharge.Name = "numericUpDown1";
            this.numericUpDownCharge.Size = new System.Drawing.Size(61, 20);
            this.numericUpDownCharge.TabIndex = 4;
            this.numericUpDownCharge.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownCharge.TextChanged += new System.EventHandler(numericUpDown1TextChanged);
            this.numericUpDownCharge.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // textBox1
            // 
            this.textBoxFragmentName.Location = new System.Drawing.Point(9, 24);
            this.textBoxFragmentName.Name = "textBox1";
            this.textBoxFragmentName.Size = new System.Drawing.Size(117, 20);
            this.textBoxFragmentName.TabIndex = 5;
            // 
            // label2
            // 
            this.labelCharge.AutoSize = true;
            this.labelCharge.Location = new System.Drawing.Point(453, 8);
            this.labelCharge.Name = "label2";
            this.labelCharge.Size = new System.Drawing.Size(41, 10);
            this.labelCharge.TabIndex = 6;
            this.labelCharge.Text = "Charge";
            // 
            // label3
            // 
            this.labelFragmentName.AutoSize = true;
            this.labelFragmentName.Location = new System.Drawing.Point(9, 8);
            this.labelFragmentName.Name = "label3";
            this.labelFragmentName.Size = new System.Drawing.Size(80, 10);
            this.labelFragmentName.TabIndex = 7;
            this.labelFragmentName.Text = "Fragment name";
            // 
            // label4
            // 
            this.labelSelectBase.AutoSize = true;
            this.labelSelectBase.Location = new System.Drawing.Point(140, 8);
            this.labelSelectBase.Name = "label4";
            this.labelSelectBase.Size = new System.Drawing.Size(80, 10);
            this.labelSelectBase.TabIndex = 7;
            this.labelSelectBase.Text = "Select base";
            // 
            // selectBaseCombobox
            // 
            this.selectBaseCombobox.Location = new System.Drawing.Point(140, 24);
            this.selectBaseCombobox.Size = new System.Drawing.Size(130, 20);
            selectBaseCombobox.SelectedIndexChanged += new System.EventHandler(this.selectBaseComboboxValueChanged);
            //
            groupboxAddingSubtracting.Text = "... chemical formula";
            groupboxAddingSubtracting.Location = new System.Drawing.Point(300,8);
            groupboxAddingSubtracting.Size = new System.Drawing.Size(120,36);
            groupboxAddingSubtracting.Enabled = false;
            //
            //
            //
            radioButtonAdding.Size = new System.Drawing.Size(40,13);
            radioButtonAdding.Location = new System.Drawing.Point(10,18);
            radioButtonAdding.Click += new System.EventHandler(addingClicked);
            radioButtonAdding.Text = "Add";
            radioButtonAdding.Checked = true;
            groupboxAddingSubtracting.Controls.Add(radioButtonAdding);
            //
            //
            //
            radioButtonSubtracting.Size = new System.Drawing.Size(60,13);
            radioButtonSubtracting.Location = new System.Drawing.Point(50,18);
            radioButtonSubtracting.Click += new System.EventHandler(subtractingClicked);
            radioButtonSubtracting.Text = "Subtract";
            groupboxAddingSubtracting.Controls.Add(radioButtonSubtracting);

            // 
            // dataGridView1
            // 
            this.dataGridViewElements.AllowUserToAddRows = false;
            this.dataGridViewElements.AllowUserToDeleteRows = false;
            this.dataGridViewElements.AllowUserToResizeColumns = false;
            this.dataGridViewElements.AllowUserToResizeRows = false;
            this.dataGridViewElements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewElements.Location = new System.Drawing.Point(9, 50);
            this.dataGridViewElements.MultiSelect = false;
            this.dataGridViewElements.Name = "dataGridView1";
            this.dataGridViewElements.RowHeadersVisible = false;
            this.dataGridViewElements.Size = new System.Drawing.Size(503, 178);
            this.dataGridViewElements.TabIndex = 9;
            this.dataGridViewElements.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1CellValueChanged);
            // 
            // NewFragment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 283);
            this.Controls.Add(this.dataGridViewElements);
            this.Controls.Add(this.labelFragmentName);
            this.Controls.Add(this.labelSelectBase);
            this.Controls.Add(this.labelCharge);
            this.Controls.Add(this.selectBaseCombobox);
            this.Controls.Add(this.textBoxFragmentName);
            this.Controls.Add(this.numericUpDownCharge);
            this.Controls.Add(this.labelMass);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(groupboxAddingSubtracting);
            this.Name = "NewFragment";
            this.Text = "New Fragment";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCharge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewElements)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Label labelMass;
        private MyNumericUpDown numericUpDownCharge;
        private System.Windows.Forms.TextBox textBoxFragmentName;
        private System.Windows.Forms.Label labelCharge;
        private System.Windows.Forms.Label labelFragmentName;
        private System.Windows.Forms.Label labelSelectBase;
        private System.Windows.Forms.DataGridView dataGridViewElements;
        private System.Windows.Forms.ComboBox selectBaseCombobox;
        private System.Windows.Forms.GroupBox groupboxAddingSubtracting;
        private System.Windows.Forms.RadioButton radioButtonAdding;
        private System.Windows.Forms.RadioButton radioButtonSubtracting;
    }
}