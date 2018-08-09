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
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        public ArrayList parsedLipids;
    
        public const string GRAMMER_FILENAME = "data/lipidmaps.grammer";
        public const char QUOTE = '"';
            
        public LipidMapsParserEventHandler lipidMapsParserEventHandler;
        public Parser parser;
        public const string FIRST_HEADER = "Old lipid name";
        public const string SECOND_HEADER = "Current lipid name";

    
        public TranslatorDialog()
        {
            lipidNamesList = new DataTable("lipidNamesList");
            lipidNamesList.Columns.Add(new DataColumn(FIRST_HEADER));
            lipidNamesList.Columns[0].DataType = typeof(string);
            lipidNamesList.Columns.Add(new DataColumn(SECOND_HEADER));
            lipidNamesList.Columns[1].DataType = typeof(string);
            lipidNamesList.Columns[1].ReadOnly = true;
            DataRow row = lipidNamesList.NewRow();
            row[FIRST_HEADER] = "";
            lipidNamesList.Rows.Add(row);
            InitializeComponent();
            lipidNamesGridView.Refresh();
            lipidNamesGridView.AllowUserToAddRows = false;
        }
        
        
        private void lipidNamesGridViewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
        
            lipidNamesGridView.Columns[0].Width = lipidNamesGridView.Width >> 1;
            lipidNamesGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            lipidNamesGridView.AllowUserToResizeColumns = false;
        }

        
        
        
        private void lipidNamesGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == lipidNamesList.Rows.Count - 1 && (string)lipidNamesList.Rows[e.RowIndex][FIRST_HEADER] != "")
            {
                lipidNamesGridView.AllowUserToAddRows = true;
                DataRow row = lipidNamesList.NewRow();
                row[FIRST_HEADER] = "";
                lipidNamesList.Rows.Add(row);
                lipidNamesGridView.AllowUserToAddRows = false;
                lipidNamesGridView.Refresh();
            }
            Console.WriteLine("vc: " + lipidNamesList.Rows[lipidNamesGridView.CurrentCell.RowIndex][FIRST_HEADER]);
        }
        
        
        public void lipidNamesGridViewEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyDown += new KeyEventHandler(lipidNamesGridViewKeyDown);
            Console.WriteLine("ed: " + lipidNamesList.Rows[lipidNamesGridView.CurrentCell.RowIndex][FIRST_HEADER]);
        }

        
        
        
        public void lipidNamesGridViewKeyDown(object sender, KeyEventArgs e)
        {
            lipidNamesGridView.AllowUserToAddRows = true;
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
            {
                int currentCell = lipidNamesGridView.CurrentCell.RowIndex; 
                string[] insertText = Clipboard.GetText().Split('\n');
                foreach (string insert in insertText)
                {
                    if (insert.Length == 0) continue;
                    if (currentCell < lipidNamesList.Rows.Count)
                    {
                        lipidNamesList.Rows[currentCell][FIRST_HEADER] = insert;
                    }
                    else 
                    {
                        lipidNamesList.Rows.Add(new string[]{insert, ""});
                    }
                    ++currentCell;
                }
                if (currentCell == lipidNamesList.Rows.Count)
                {
                    lipidNamesList.Rows.Add(new string[]{"", ""});
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete && lipidNamesGridView.CurrentCell.RowIndex != lipidNamesList.Rows.Count - 1)
            {
                lipidNamesList.Rows.RemoveAt(lipidNamesGridView.CurrentCell.RowIndex);
            }
            lipidNamesGridView.AllowUserToAddRows = false;
            lipidNamesGridView.Refresh();
            Console.WriteLine("kd: " + lipidNamesList.Rows[lipidNamesGridView.CurrentCell.RowIndex][FIRST_HEADER]);
        }

        
        
        // cancel
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        
        
        
        // translate
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        
        
        
        // import
        private void button3_Click(object sender, EventArgs e)
        {
            Close();

        }
    }
}
