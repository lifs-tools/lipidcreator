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
    
        public TranslatorDialog(CreatorGUI _creatorGUI)
        {
            lipidNamesList = new DataTable("lipidNamesList");
            lipidNamesList.Columns.Add(new DataColumn(FIRST_HEADER));
            lipidNamesList.Columns[0].DataType = typeof(string);
            lipidNamesList.Columns.Add(new DataColumn(SECOND_HEADER));
            lipidNamesList.Columns[1].DataType = typeof(string);
            lipidNamesList.Columns[1].ReadOnly = true;
            creatorGUI = _creatorGUI;
            parsedLipids = new ArrayList();
            
            lipidMapsParserEventHandler = new LipidMapsParserEventHandler(creatorGUI.lipidCreator);
            parser = new Parser(lipidMapsParserEventHandler, GRAMMER_FILENAME, QUOTE);
            
            InitializeComponent();
        }
        
        public void lipidNamesGridViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                int currentCell = lipidNamesGridView.CurrentCell.RowIndex; 
                string[] insertText = Clipboard.GetText().Split('\n');
                if (insertText.Length > 1){
                    foreach (string insert in insertText)
                    {
                        if (currentCell < lipidNamesList.Rows.Count)
                        {
                            lipidNamesList.Rows[currentCell][FIRST_HEADER] = insert;
                        }
                        else 
                        {
                            DataRow row = lipidNamesList.NewRow();
                            row[FIRST_HEADER] = insert;
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
            lipidNamesList.Columns[1].ReadOnly = false;
            parsedLipids.Clear();
            foreach (DataRow row in lipidNamesList.Rows)
            {
                Lipid lipid = null;
                if (row[FIRST_HEADER] is string)
                {
                    string oldLipidName = (string)row[FIRST_HEADER];
                    if (oldLipidName.Length > 0)
                    {
                        parser.parse(oldLipidName);
                        if (parser.wordInGrammer)
                        {
                            parser.raiseEvents();
                            if (lipidMapsParserEventHandler.lipid != null)
                            {
                                lipid = lipidMapsParserEventHandler.lipid;
                            }
                        }
                    }
                }
                Console.WriteLine(lipid != null);
                parsedLipids.Add(lipid);
            }
            
            
            
            Console.WriteLine(parsedLipids.Count);
            
            
            
            HashSet<String> usedKeys = new HashSet<String>();
            ArrayList precursorDataList = new ArrayList();
            int i = 0;
            foreach (Lipid currentLipid in parsedLipids)
            {
                if (currentLipid != null)
                {
                    currentLipid.computePrecursorData(creatorGUI.lipidCreator.headgroups, usedKeys, precursorDataList);
                    lipidNamesList.Rows[i][SECOND_HEADER] = ((PrecursorData)precursorDataList[precursorDataList.Count - 1]).precursorName;
                    usedKeys.Clear();
                }
                else
                {
                    lipidNamesList.Rows[i][SECOND_HEADER] = "Unrecognized lipid";
                }
                ++i;
            }
            lipidNamesList.Columns[1].ReadOnly = true;
            lipidNamesGridView.Refresh();
        }
        
        
        
        
        // import
        private void button3_Click(object sender, EventArgs e)
        {
            Close();

        }
    }
}
