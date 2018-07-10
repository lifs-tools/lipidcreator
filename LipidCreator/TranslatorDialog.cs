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
    
        public const string GRAMMER_FILENAME = "data/lipidmaps.grammer";
        public const char QUOTE = '"';
            
        public LipidMapsParserEventHandler lipidMapsParserEventHandler;
        public Parser parser;
    
        public TranslatorDialog(CreatorGUI _creatorGUI)
        {
            lipidNamesList = new DataTable("lipidNamesList");
            lipidNamesList.Columns.Add(new DataColumn("Old lipid name"));
            lipidNamesList.Columns[0].DataType = typeof(string);
            lipidNamesList.Columns.Add(new DataColumn("Current lipid name"));
            lipidNamesList.Columns[1].DataType = typeof(string);
            lipidNamesList.Columns[1].ReadOnly = true;
            creatorGUI = _creatorGUI;
            
            lipidMapsParserEventHandler = new LipidMapsParserEventHandler(creatorGUI.lipidCreator);
            parser = new Parser(lipidMapsParserEventHandler, GRAMMER_FILENAME, QUOTE);
            
            InitializeComponent();
        }
        
        public void lipidNamesGridViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                int currentCell = lipidNamesGridView.CurrentCell.RowIndex; 
                Console.WriteLine(currentCell);
                string[] insertText = Clipboard.GetText().Split('\n');
                if (insertText.Length > 1){
                    foreach (string insert in insertText)
                    {
                        Console.WriteLine("'" + insert + "'");
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
                lipidNamesGridView.Refresh();
            }
        }
        
        
        
        public void lipidNamesGridViewCellValueChanged(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyDown += new KeyEventHandler(lipidNamesGridViewKeyDown);
        }
        
        
        
        private void lipidNamesGridViewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
        
            lipidNamesGridView.Columns[0].Width = lipidNamesGridView.Width >> 1;
            lipidNamesGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            lipidNamesGridView.AllowUserToResizeColumns = false;
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
