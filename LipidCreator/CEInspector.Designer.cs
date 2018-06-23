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
        public int marginLeft = 30;
        public int marginRight = 10;
        public int marginTop = 10;
        public int marginBottom = 20;
        public const int LABEL_EXTENSION = 5;
        public const double VAL_DENOMINATOR = 100.0;
        
        public int innerWidthPx;
        public int innerHeightPx;
        public int maxXVal = 50;
        public int maxYVal = 200;
        
        public Point valueToPx(double valX, double valY)
        {
            return new Point((int)(marginLeft + valX * innerWidthPx / maxXVal), (int)(Height - marginBottom - valY * innerHeightPx / maxYVal));
        }
    
        protected override void OnPaint(PaintEventArgs e)
        {
            innerWidthPx = Width - marginLeft - marginRight;
            innerHeightPx = Height - marginBottom - marginTop;
        
            // draw border
            Graphics g = e.Graphics;
            Pen blackPen = new Pen(Color.Black, 1);
            g.DrawRectangle(blackPen, 0, 0, this.Size.Width - 1, this.Size.Height - 1);
            
            
            Pen redPen = new Pen(Color.Red, 1);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            
            double lastX = 0;
            double lastY = 0;
            for (int i = 0; i <= VAL_DENOMINATOR; ++i)
            {
                double valX = ((double)i) / VAL_DENOMINATOR * maxXVal;
                double valY = 120.0 / (valX * valX / 100.0 + 1.0);
                if (i > 0) g.DrawLine(redPen, valueToPx(lastX, lastY), valueToPx(valX, valY));
                lastX = valX;
                lastY = valY;
            }
            
            
            Font labelFont = new Font("Arial", 8);
            
            
            
            // drawing the axes
            blackPen = new Pen(Color.Black, 2);
            g.DrawLine(blackPen, new Point(marginLeft - LABEL_EXTENSION, Height - marginBottom), new Point(Width, Height - marginBottom));
            g.DrawLine(blackPen, new Point(marginLeft, Height - marginBottom + LABEL_EXTENSION), new Point(marginLeft, 0));
            
            // labels at x-axis
            for (int i = 0, j = 0; i < Width; i += innerWidthPx / 5, j += maxXVal / 5)
            {
                g.DrawLine(blackPen, new Point(marginLeft + i, Height - marginBottom - LABEL_EXTENSION), new Point(marginLeft + i, Height - marginBottom + LABEL_EXTENSION));
                
                
                int stringSize = (int)g.MeasureString(Convert.ToString(j), labelFont, 20).Width;
                RectangleF instructionRect = new RectangleF(marginLeft + i - (stringSize >> 1), Height - marginBottom + 5, stringSize, 20);
                g.DrawString(Convert.ToString(j), labelFont, drawBrush, instructionRect);
            }
            
            
            
            // labels at y-axis
            for (int i = 0, j = 0; i < Height; i += innerHeightPx / 5, j += maxYVal / 5)
            {
                g.DrawLine(blackPen, new Point(marginLeft - LABEL_EXTENSION, Height - marginBottom - i), new Point(marginLeft + LABEL_EXTENSION, Height - marginBottom -i ));
                
                SizeF stringSize = g.MeasureString(Convert.ToString(j), labelFont, 30);
                int stringSizeW = (int)stringSize.Width;
                int stringSizeH = (int)stringSize.Height;
                RectangleF instructionRect = new RectangleF(marginLeft - stringSizeW - (LABEL_EXTENSION << 1), Height - marginBottom - i - (stringSizeH >> 1), stringSizeW, 16);
                g.DrawString(Convert.ToString(j), labelFont, drawBrush, instructionRect);
            }
            
            
            
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
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(800, 500);
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
            
            
            
            Cartesean myParentCanvas = new Cartesean();
            myParentCanvas.Width = 700;
            myParentCanvas.Height = 350;
            myParentCanvas.Location = new Point(10, 10);
            
            // 
            // CEInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(myParentCanvas);
            this.Name = "CEInspector";
            this.Text = "Collision energy optimization";
            this.ResumeLayout(false);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}