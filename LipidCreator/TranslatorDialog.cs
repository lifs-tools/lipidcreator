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
    public partial class TranslatorDialog : Form
    {
        public CreatorGUI creatorGUI;
        public DataTable lipidNamesList;
    
    
    
        public TranslatorDialog(CreatorGUI _creatorGUI)
        {
            lipidNamesList = new DataTable("lipidNamesList");
            lipidNamesList.Columns.Add(new DataColumn("Old lipid name"));
            lipidNamesList.Columns[0].DataType = typeof(string);
            lipidNamesList.Columns.Add(new DataColumn("Current lipid name"));
            lipidNamesList.Columns[1].DataType = typeof(string);
            lipidNamesList.Columns[1].ReadOnly = true;
            creatorGUI = _creatorGUI;
            
            InitializeComponent();
        }
        
        public void lipidNamesGridViewPasteGridCellContent(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                int currentCell = lipidNamesGridView.CurrentCell.RowIndex; 
                //lipidNamesGridView.DataSource = null;
                string[] insertText = Clipboard.GetText().Split('\n');
                if (insertText.Length > 1){
                    foreach (string insert in insertText)
                    {
                        if (currentCell < lipidNamesList.Rows.Count)
                        {
                            lipidNamesList.Rows[currentCell]["Old lipid name"] = insert;
                        }
                        else 
                        {
                            DataRow row = lipidNamesList.NewRow();
                            row["Old lipid name"] = insert;
                            lipidNamesList.Rows.Add(row);
                        }
                        ++currentCell;
                    }
                }
                //lipidNamesGridView.DataSource = lipidNamesList;
                lipidNamesGridView.Refresh();
            }
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();

        }
    }
}
