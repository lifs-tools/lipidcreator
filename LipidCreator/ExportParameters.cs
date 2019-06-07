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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LipidCreator
{
    public partial class ExportParameters : Form
    {
        public int[] parameterValues = null;
        
        public ExportParameters(int[] _parameterValues)
        {
            parameterValues = _parameterValues;
            InitializeComponent();
        }
        
        
        public void rb1CheckedChanged(Object sender, EventArgs e)
        {
            parameterValues[0] = 0;
        }
        
        
        public void rb2CheckedChanged(Object sender, EventArgs e)
        {
            parameterValues[0] = 1;
        }
        
        
        public void rb3CheckedChanged(Object sender, EventArgs e)
        {
            parameterValues[1] = 1;
        }
        
        
        public void rb4CheckedChanged(Object sender, EventArgs e)
        {
            parameterValues[1] = 0;
        }
        
        
        protected void okClick(object sender, System.EventArgs e)
        {
            Close();        
        }
        
        protected void cancelClick(object sender, System.EventArgs e)
        {
            parameterValues[2] = 1;
            Close();
        }
    }
}
