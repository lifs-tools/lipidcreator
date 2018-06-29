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
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace LipidCreator
{

    public class Cartesean : Control
    {
        public int marginLeft = 50;
        public int marginRight = 10;
        public int marginTop = 10;
        public int marginBottom = 20;
        public const int LABEL_EXTENSION = 5;
        public const int CE_GRAB_MARGIN = 1;
        public const double VAL_DENOMINATOR = 100.0;
        public const int offsetPX = 5;
        public CEInspector ceInspector;
        public Dictionary<string, string> fragmentToColor;
        public string [] colors = new string[]{"#e6194b", "#3cb44b", "#ffe119", "#0082c8", "#f58231", "#911eb4", "#46f0f0", "#f032e6", "#d2f53c", "#fabebe", "#008080", "#e6beff", "#aa6e28", "#fffac8", "#800000", "#aaffc3", "#808000", "#ffd8b1", "#000080", "#808080"};
        
        public int innerWidthPx;
        public int innerHeightPx;
        public double maxXVal = 60;
        public double minXVal = 10;
        public double maxYVal = 100;
        public string highlightName = "";
        public double CEval;
        public bool CELineShift = false;
        public bool smooth = true;
        
        public Cartesean(CEInspector _ceInspector, int width, int height)
        {
            Width = width;
            Height = height;
            CEval = (maxXVal + minXVal) / 2.0;
            ceInspector = _ceInspector;
            innerWidthPx = Width - marginLeft - marginRight;
            innerHeightPx = Height - marginBottom - marginTop;
            DoubleBuffered = true;
            fragmentToColor = new Dictionary<string, string>();
        }
        
        
        
        
        public Point valueToPx(double valX, double valY)
        {
            return new Point((int)(marginLeft + (valX - minXVal) * innerWidthPx / (maxXVal - minXVal)), (int)(Height - marginBottom - valY * innerHeightPx / maxYVal));
        }
        
        
        
        
        public PointF pxToValue(int pxX, int pxY)
        {
            return new PointF((float)((pxX - marginLeft) * (maxXVal - minXVal) / innerWidthPx + minXVal), (float)((Height - marginBottom - pxY) * maxYVal / innerHeightPx));
        }
        
        
        
        public bool mouseOverCELine(MouseEventArgs e)
        {
            PointF vals = pxToValue(e.X, e.Y);
            return (CEval - CE_GRAB_MARGIN <= vals.X && vals.X <= CEval + CE_GRAB_MARGIN);
        }
        
        
        
        
        public void setFragmentColors()
        {
            int k = 0;
            fragmentToColor.Clear();
            foreach(DataRow row in ceInspector.fragmentsList.Rows)
            {
                fragmentToColor[(string)row["Fragment name"]] = colors[k++ % colors.Length];
            }
        }
        
        
        
        
        public void mouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // if mouse curves hovers over control
            if (ClientRectangle.Contains(PointToClient(Control.MousePosition)))
            {
                double newYVal = maxYVal + 10 * ((e.Delta > 0) ? -1.0 : 1.0);
                smooth = false;
                ceInspector.timerSmooth.Enabled = true;
                if (10 <= newYVal)
                {
                    maxYVal = newYVal;
                    ceInspector.cartesean_mouseMove(sender, e);
                    Refresh();
                }
            }
        }
        
        
        
        
    
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            float[] dashValues = { 5, 10 };
            if (smooth) g.SmoothingMode = SmoothingMode.AntiAlias;
            
            
            // draw white background
            SolidBrush whiteBrush = new SolidBrush(Color.White);
            Rectangle rectBG = new Rectangle(0, 0, Width, Height);
            e.Graphics.FillRectangle(whiteBrush, rectBG);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            
            
            // draw grid in the background
            Pen grayPen = new Pen(ColorTranslator.FromHtml("#dddddd"));
            grayPen.DashPattern = dashValues;
            for (int i = 0; i < Width; i += innerWidthPx / 5)
            {
                g.DrawLine(grayPen, new Point(marginLeft + i, Height - marginBottom), new Point(marginLeft + i, 0));
            }
            for (int i = 0; i < Height; i += innerHeightPx / 5)
            {
                g.DrawLine(grayPen, new Point(marginLeft, Height - marginBottom - i), new Point(Width, Height - marginBottom -i ));
            }
            
            
            
            
            // drawing the product profile
            double lastX = 0;
            double lastY = 0;
            Pen profilePen = new Pen(ColorTranslator.FromHtml("#aaaaaa"));
            for (int ii = 0; ii < ceInspector.yValCoords["productProfile"].Length; ++ii)
            {
                double valX = ceInspector.xValCoords[ii];
                double valY = ceInspector.yValCoords["productProfile"][ii];
                
                if (ii > 0) g.DrawLine(profilePen, valueToPx(lastX, lastY), valueToPx(valX, valY));
                lastX = valX;
                lastY = valY;
            }
            
            
            
            
            // draw all curves
            foreach(DataRow row in ceInspector.fragmentsList.Rows)
            {
                if (!(bool)row["View"]) continue;
                string fragmentName = (string)row["Fragment name"];
                lastX = 0;
                lastY = 0;
                Pen colorPen = new Pen(ColorTranslator.FromHtml(fragmentToColor[fragmentName]), (fragmentName == highlightName ? 4 : 2));
                for (int i = 0; i < ceInspector.yValCoords[fragmentName].Length; ++i)
                {
                    double valX = ceInspector.xValCoords[i];
                    double valY = ceInspector.yValCoords[fragmentName][i];
                    
                    if (i > 0) g.DrawLine(colorPen, valueToPx(lastX, lastY), valueToPx(valX, valY));
                    lastX = valX;
                    lastY = valY;
                }
            }
            
            // drawing the axes
            Font labelFont = new Font("Arial", 8);
            Pen blackPen = new Pen(Color.Black, 2);
            g.DrawLine(blackPen, new Point(marginLeft - LABEL_EXTENSION, Height - marginBottom), new Point(Width, Height - marginBottom));
            g.DrawLine(blackPen, new Point(marginLeft, Height - marginBottom + LABEL_EXTENSION), new Point(marginLeft, 0));
            
            // labels at x-axis
            double jj = minXVal;
            for (int i = 0; i < Width; i += innerWidthPx / 5, jj += (maxXVal - minXVal) / 5.0)
            {
                g.DrawLine(blackPen, new Point(marginLeft + i, Height - marginBottom - LABEL_EXTENSION), new Point(marginLeft + i, Height - marginBottom + LABEL_EXTENSION));
                
                
                int stringSize = (int)g.MeasureString(Convert.ToString(jj), labelFont, 20).Width;
                RectangleF instructionRect = new RectangleF(marginLeft + i - (stringSize >> 1), Height - marginBottom + 5, stringSize, 20);
                g.DrawString(Convert.ToString(jj), labelFont, drawBrush, instructionRect);
            }
            
            
            
            // labels at y-axis
            jj = 0;
            for (int i = 0; i < Height; i += innerHeightPx / 5, jj += maxYVal / 5.0)
            {
                g.DrawLine(blackPen, new Point(marginLeft - LABEL_EXTENSION, Height - marginBottom - i), new Point(marginLeft + LABEL_EXTENSION, Height - marginBottom -i ));
                
                SizeF stringSize = g.MeasureString(Convert.ToString(jj), labelFont, 30);
                int stringSizeW = (int)stringSize.Width;
                int stringSizeH = (int)stringSize.Height;
                RectangleF instructionRect = new RectangleF(marginLeft - stringSizeW - (LABEL_EXTENSION << 1), Height - marginBottom - i - (stringSizeH >> 1), stringSizeW, 16);
                g.DrawString(Convert.ToString(jj), labelFont, drawBrush, instructionRect);
            }
            
            // draw dashed collision energy line
            if (CEval > 0)
            {
                Pen bPen = new Pen(Color.Black, 1);
                bPen.DashPattern = dashValues;
                int ceX = valueToPx(CEval, 0).X;
                g.DrawLine(bPen, new Point(ceX, Height - marginBottom), new Point(ceX, 0));
            }
            
            // draw border
            g.DrawRectangle(blackPen, 0, 0, this.Size.Width - 1, this.Size.Height - 1);
            
            
            base.OnPaint(e);
        }
    }



    partial class CEInspector
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
            this.ToolTip1 = new System.Windows.Forms.ToolTip();
            this.labelInstrument = new Label();
            this.labelClass = new Label();
            this.labelAdduct = new Label();
            this.labelFragment = new Label();
            this.labelCurrentCE = new Label();
            this.textBoxCurrentCE = new TextBox();
            this.instrumentCombobox = new ComboBox();
            this.classCombobox = new ComboBox();
            this.adductCombobox = new ComboBox();
            fragmentsGridView = new DataGridView();
            this.timerSmooth = new System.Timers.Timer(250);
            this.timerSmooth.Elapsed += this.changeSmooth;
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(1050, 562);
            
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(ClientSize.Width - 173, ClientSize.Height - 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.cancelClick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(ClientSize.Width - 88, ClientSize.Height - 35);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.applyClick);
            
            
            
            cartesean = new Cartesean(this, 800, 500);
            cartesean.Location = new Point(10, 50);
            cartesean.MouseMove += new System.Windows.Forms.MouseEventHandler(cartesean_mouseMove);
            cartesean.MouseDown += new MouseEventHandler(cartesean_mouseDown);
            cartesean.MouseUp += new MouseEventHandler(cartesean_mouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(cartesean.mouseWheel);

            
            
            instrumentCombobox.Location = new Point(10, 20);
            instrumentCombobox.Width = 200;
            instrumentCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            instrumentCombobox.SelectedIndexChanged += new EventHandler(instrumentComboboxChanged);
            
            
            classCombobox.Location = new Point(210, 20);
            classCombobox.Width = 200;
            classCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            classCombobox.SelectedIndexChanged += new EventHandler(classComboboxChanged);
            
            adductCombobox.Location = new Point(420, 20);
            adductCombobox.Width = 200;
            adductCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            adductCombobox.SelectedIndexChanged += new EventHandler(adductComboboxChanged);
            
            labelInstrument.Text = "Instruments:";
            labelInstrument.Width = 140;
            labelInstrument.Location = new Point(10, 4);
            
            labelClass.Text = "Lipid classes:";
            labelClass.Width = 140;
            labelClass.Location = new Point(210, 4);
            
            labelAdduct.Text = "Adducts:";
            labelAdduct.Width = 140;
            labelAdduct.Location = new Point(420, 4);
            
            labelFragment.Text = "Fragments:";
            labelFragment.Width = 140;
            labelFragment.Height = 16;
            labelFragment.Location = new Point(720, 34);
            
            labelCurrentCE.Text = "Current collision energy:";
            labelCurrentCE.Width = 140;
            labelCurrentCE.Height = 16;
            labelCurrentCE.Location = new Point(820, 454);
            
            
            textBoxCurrentCE.Location = new Point(820, 470);
            textBoxCurrentCE.Width = 140;
            textBoxCurrentCE.Leave += new EventHandler(textBoxCurrentCE_ValueChanged);
            textBoxCurrentCE.KeyDown += new KeyEventHandler(textBoxCurrentCE_Keydown);
            
            
            
            fragmentsGridView.Location = new Point(820, 50);
            fragmentsGridView.Size = new Size(216, 390);
            fragmentsGridView.DataSource = fragmentsList;
            fragmentsGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            fragmentsGridView.AllowUserToResizeColumns = false;
            fragmentsGridView.AllowUserToAddRows = false;
            fragmentsGridView.AllowUserToResizeRows = false;
            //fragmentsGridView.ReadOnly = true;
            fragmentsGridView.MultiSelect = false;
            fragmentsGridView.RowTemplate.Height = 34;
            fragmentsGridView.AllowDrop = true;
            fragmentsGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            fragmentsGridView.MouseMove += new System.Windows.Forms.MouseEventHandler(fragmentsGridView_MouseMove);
            fragmentsGridView.CellValueChanged += new DataGridViewCellEventHandler(fragmentsGridView_CellValueChanged);
            fragmentsGridView.CellContentClick += new DataGridViewCellEventHandler(fragmentsGridView_CellContentClick);
            fragmentsGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(fragmentsGridViewDataBindingComplete);
            fragmentsGridView.RowHeadersVisible = false;
            fragmentsGridView.ScrollBars = ScrollBars.Vertical;
            
            
            
            // 
            // CEInspector
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cartesean);
            this.Controls.Add(this.instrumentCombobox);
            this.Controls.Add(this.classCombobox);
            this.Controls.Add(this.adductCombobox);
            this.Controls.Add(this.labelInstrument);
            this.Controls.Add(this.labelClass);
            this.Controls.Add(this.labelAdduct);
            this.Controls.Add(this.labelFragment);
            this.Controls.Add(this.labelCurrentCE);
            this.Controls.Add(this.textBoxCurrentCE);
            this.Controls.Add(this.fragmentsGridView);
            this.Name = "CEInspector";
            this.Text = "Collision energy optimization";
            this.ResumeLayout(false);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
        }
        
        

        #endregion
        public Cartesean cartesean;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.ToolTip ToolTip1;
        public ComboBox instrumentCombobox;
        public ComboBox classCombobox;
        public ComboBox adductCombobox;
        public Label labelInstrument;
        public Label labelClass;
        public Label labelAdduct;
        public Label labelFragment;
        public Label labelCurrentCE;
        public TextBox textBoxCurrentCE;
        public DataGridView fragmentsGridView;
        public System.Timers.Timer timerSmooth;
    }
}