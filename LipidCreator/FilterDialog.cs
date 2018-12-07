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
using System.Windows.Forms;

namespace LipidCreator
{
    public partial class FilterDialog : Form
    {
        Lipid lipid = null;
        int[] parameters = null;
        public FilterDialog(Lipid _lipid)
        {
            lipid = _lipid;
            
            InitializeComponent();
            
            switch (lipid.onlyPrecursors){
                case 0: radioButton1.Checked = true; break;
                case 1: radioButton2.Checked = true; break;
                case 2: radioButton3.Checked = true; break;
            }
            
            switch (lipid.onlyHeavyLabeled){
                case 0: radioButton4.Checked = true; break;
                case 1: radioButton5.Checked = true; break;
                case 2: radioButton6.Checked = true; break;
            }
        }
        
        
        public FilterDialog(int[] _parameters)
        {
            parameters = _parameters;
            InitializeComponent();
            button1.Enabled = false;
            radioButton3.Checked = true;
            radioButton6.Checked = true;
        }
        
        
        private void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void applyClick(object sender, EventArgs e)
        {
            if (lipid != null){
                if (radioButton1.Checked) lipid.onlyPrecursors = 0;
                else if (radioButton2.Checked) lipid.onlyPrecursors = 1;
                else if (radioButton3.Checked) lipid.onlyPrecursors = 2;
                
                if (radioButton4.Checked) lipid.onlyHeavyLabeled = 0;
                else if (radioButton5.Checked) lipid.onlyHeavyLabeled = 1;
                else if (radioButton6.Checked) lipid.onlyHeavyLabeled = 2;
            }
            else {
                if (radioButton1.Checked) parameters[0] = 0;
                else if (radioButton2.Checked) parameters[0] = 1;
                else if (radioButton3.Checked) parameters[0] = 2;
                
                if (radioButton4.Checked) parameters[1] = 0;
                else if (radioButton5.Checked) parameters[1] = 1;
                else if (radioButton6.Checked) parameters[1] = 2;
            }
            this.Close();
        }
    }
}
