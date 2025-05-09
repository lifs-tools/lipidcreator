﻿/*
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
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace LipidCreator
{
    public partial class TranslatorDialog : Form
    {
        public CreatorGUI creatorGUI = null;
        public DataTable lipidNamesList = null;
        public ArrayList parsedLipids = null;
        public ArrayList parsedLipidList = null;
        public Image whiteImage = null;
        public bool tableInitialized = false;
            
        public const string FIRST_HEADER = "Old lipid name";
        public const string SECOND_HEADER = "Current lipid name";
        public const string DELETE_HEADER = "Delete";

    
        public TranslatorDialog(CreatorGUI _creatorGUI)
        {
            parsedLipids = new ArrayList();
            creatorGUI = _creatorGUI;
            
            whiteImage = Image.FromFile(Path.Combine(creatorGUI.lipidCreator.prefixPath, "images", "white.png"));

            lipidNamesList = new DataTable("lipidNamesList");
            lipidNamesList.Columns.Add(new DataColumn(FIRST_HEADER));
            lipidNamesList.Columns[0].DataType = typeof(string);
            lipidNamesList.Columns.Add(new DataColumn(SECOND_HEADER));
            lipidNamesList.Columns[1].DataType = typeof(string);
            lipidNamesList.Columns[1].ReadOnly = true;
            
            InitializeComponent();
            InitializeCustom();
            disableImport();
        }
        
        
        private void lipidNamesGridViewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (tableInitialized) return;
            tableInitialized = true;
            DataGridViewImageColumn deleteColumn = new DataGridViewImageColumn();  
            deleteColumn.Name = "Delete";  
            deleteColumn.HeaderText = "Delete";  
            deleteColumn.ValuesAreIcons = false;
            deleteColumn.Width = 40;
            lipidNamesGridView.Columns.Add(deleteColumn);
            lipidNamesGridView.Columns[0].Width = lipidNamesGridView.Width >> 1;
            lipidNamesGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            lipidNamesGridView.AllowUserToResizeColumns = false;
            
            
            DataRow row = lipidNamesList.NewRow();
            row[FIRST_HEADER] = "";
            row[SECOND_HEADER] = "";
            lipidNamesList.Rows.Add(row);
            
            lipidNamesGridView.Rows[0].Cells[DELETE_HEADER].Value = whiteImage;
            
            foreach (DataGridViewColumn dgvc in lipidNamesGridView.Columns) {
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            
            lipidNamesGridView.Update();
            lipidNamesGridView.Refresh();
            lipidNamesGridView.AllowUserToAddRows = false;
        }

        
        
        
        private void lipidNamesGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == lipidNamesList.Rows.Count - 1 && (string)lipidNamesList.Rows[e.RowIndex][FIRST_HEADER] != "")
            {
                DataRow row = lipidNamesList.NewRow();
                row[FIRST_HEADER] = "";
                row[SECOND_HEADER] = "";
                lipidNamesList.Rows.Add(row);
                for (int i = 0; i < lipidNamesList.Rows.Count - 1; ++i)
                {
                    lipidNamesGridView.Rows[i].Cells[DELETE_HEADER].Value = creatorGUI.deleteImage;
                }
                lipidNamesGridView.Rows[lipidNamesList.Rows.Count - 1].Cells[DELETE_HEADER].Value = whiteImage;
                lipidNamesGridView.Update();
                lipidNamesGridView.Refresh();
            }
            disableImport();
        }
        
        
        
        public void disableImport()
        {
            button3.Enabled = false;
        }
        
        
        
        public void lipidNamesGridViewEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyDown += new KeyEventHandler(lipidNamesGridViewKeyDown);
        }

        
        
        
        public void lipidNamesGridViewKeyDown(object sender, KeyEventArgs e)
        {
            lipidNamesGridView.AllowUserToAddRows = true;
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
            {
                int currentCell = lipidNamesGridView.CurrentCell.RowIndex; 
                string[] insertText = Clipboard.GetText().Split(new char[]{'\n'});
                foreach (string ins in insertText)
                {
                    string insert = Parser.strip(ins, (char)13);
                    insert = Parser.strip(insert, (char)10);
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
            for (int i = 0; i < lipidNamesList.Rows.Count - 1; ++i)
            {
                lipidNamesGridView.Rows[i].Cells[DELETE_HEADER].Value = creatorGUI.deleteImage;
            }
            lipidNamesGridView.Rows[lipidNamesList.Rows.Count - 1].Cells[DELETE_HEADER].Value = whiteImage;
            lipidNamesGridView.Update();
            lipidNamesGridView.Refresh();
            lipidNamesGridView.AllowUserToAddRows = false;
            
            disableImport();
        }
        
        
        
        
        public void lipidsGridviewDoubleClick(Object sender, EventArgs e)
        {
            int rowIndex = ((DataGridView)sender).CurrentCell.RowIndex;
            int colIndex = ((DataGridView)sender).CurrentCell.ColumnIndex;
            if (((DataGridView)sender).Columns[colIndex].Name == "Delete")
            {
                lipidNamesList.Rows.RemoveAt(rowIndex);
                for (int i = 0; i < lipidNamesList.Rows.Count - 1; ++i)
                {
                    lipidNamesGridView.Rows[i].Cells[DELETE_HEADER].Value = creatorGUI.deleteImage;
                }
                lipidNamesGridView.Rows[lipidNamesList.Rows.Count - 1].Cells[DELETE_HEADER].Value = whiteImage;
                lipidNamesGridView.Update();
                lipidNamesGridView.Refresh();
            }
        }
        

        
        
        // cancel
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        
        
        
        // translate
        private void button2_Click(object sender, EventArgs e)
        {
            ArrayList lipidNames = new ArrayList();
            int i = 0;
            foreach (DataRow row in lipidNamesList.Rows)
            {
                if (i == lipidNamesList.Rows.Count - 1) break;
                lipidNamesGridView.Rows[i++].DefaultCellStyle.BackColor = Color.Empty;
                try
                {
                    lipidNames.Add((string)row[FIRST_HEADER]);
                }
                catch
                {
                    lipidNames.Add("");
                }
            }
            
            parsedLipids = creatorGUI.lipidCreator.translate(lipidNames);
            parsedLipidList = new ArrayList();
            
            lipidNamesList.Columns[1].ReadOnly = false;
            HashSet<String> usedKeys = new HashSet<String>();
            ArrayList precursorDataList = new ArrayList();
            i = 0;
            int correctlyParsed = 0;
            foreach (object[] currentLipidRow in parsedLipids)
            {
                Lipid currentLipid = (Lipid)currentLipidRow[0];
                string currentHeavyName = (string)currentLipidRow[1];
                
                string newLipidName = "";
                if (currentLipid != null)
                {
                    currentLipid.computePrecursorData(creatorGUI.lipidCreator.headgroups, usedKeys, precursorDataList);
                    if (!(currentLipid is UnsupportedLipid))
                    {
                        if (precursorDataList.Count == 0)
                        {
                            newLipidName = "Unrecognized molecule";
                            lipidNamesGridView.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                        }
                        else
                        {
                            int pdc = 0;
                            if  (precursorDataList.Count > 1 && currentLipid.onlyHeavyLabeled == 1)
                            {
                                for (int j = 0; j < precursorDataList.Count; ++j)
                                {
                                    string fullMoleculeListName = ((PrecursorData)precursorDataList[j]).fullMoleculeListName;
                                    if (LipidCreator.precursorNameSplit(fullMoleculeListName)[1] == currentHeavyName)
                                    {
                                        pdc = j;
                                        break;
                                    }
                                }
                            }
                        
                            newLipidName = ((PrecursorData)precursorDataList[pdc]).precursorName;
                            string adductName = "";
                            foreach (string addct in currentLipid.adducts.Keys)
                            {
                                if (currentLipid.adducts[addct])
                                {
                                    adductName = addct;
                                    break;
                                }
                            }
                            Adduct adduct = Lipid.ALL_ADDUCTS[Lipid.ADDUCT_POSITIONS[adductName]];
                            ElementDictionary precursorElements = creatorGUI.lipidCreator.headgroups[((PrecursorData)precursorDataList[pdc]).moleculeListName].elements;
                            newLipidName += LipidCreator.computeAdductFormula(precursorElements, adduct);
                            ++correctlyParsed;
                        }
                    }
                    else
                    {
                        newLipidName = "Unsupported molecule";
                        lipidNamesGridView.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    }
                    usedKeys.Clear();
                    precursorDataList.Clear();
                    
                }
                else
                {
                    newLipidName = "Unrecognized molecule";
                    lipidNamesGridView.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
                
                lipidNamesList.Rows[i][SECOND_HEADER] = newLipidName;
                parsedLipidList.Add(new object[]{newLipidName, currentLipid});
                ++i;
            }
            lipidNamesList.Columns[1].ReadOnly = true;
            lipidNamesGridView.Refresh();
            
            if (lipidNamesList.Rows.Count > 1 && correctlyParsed == lipidNamesList.Rows.Count - 1) button3.Enabled = true;
        }

        
        
        
        
        // import
        private void button3_Click(object sender, EventArgs e)
        {
            int[] filterParameters = {2, 2};
            Dictionary<string, Lipid> parsedLipidsDict = new Dictionary<string, Lipid>();
            FilterDialog importFilterDialog = new FilterDialog(filterParameters);
            importFilterDialog.Owner = this;
            importFilterDialog.ShowInTaskbar = false;
            importFilterDialog.ShowDialog();
            importFilterDialog.Dispose();
            
            int[] returnMessage = new int[]{0};
            LCMessageBox lcmb = new LCMessageBox(returnMessage, 0);
            lcmb.Owner = this;
            lcmb.StartPosition = FormStartPosition.CenterParent;
            lcmb.ShowInTaskbar = false;
            lcmb.ShowDialog();
            lcmb.Dispose();
            if (returnMessage[0] == 1)
            {
                creatorGUI.lipidCreator.registeredLipidDictionary.Clear(); // replace
                creatorGUI.lipidCreator.registeredLipids.Clear(); // replace
            }
            
            // merge lipids if Precursor names match
            ArrayList lipidListForInsertion = new ArrayList();
            foreach(object[] lipidRow in parsedLipidList)
            {
                string lipidName = (string)lipidRow[0];
                Lipid currentLipid = (Lipid)lipidRow[1];
                if (!parsedLipidsDict.ContainsKey(lipidName))
                {
                    parsedLipidsDict.Add(lipidName, currentLipid);
                    lipidListForInsertion.Add(currentLipid);
                }
                else
                {
                    Lipid lipidForMerging = parsedLipidsDict[lipidName];
                    foreach (string lipidClass in lipidForMerging.positiveFragments.Keys)
                    {
                        lipidForMerging.positiveFragments[lipidClass].UnionWith(currentLipid.positiveFragments[lipidClass]);
                    }
                    foreach (string lipidClass in lipidForMerging.negativeFragments.Keys)
                    {
                        lipidForMerging.negativeFragments[lipidClass].UnionWith(currentLipid.negativeFragments[lipidClass]);
                    }
                }
            }
            
        
            foreach(Lipid currentLipid in lipidListForInsertion)
            {
                ulong lipidHash = 0;
                if (currentLipid is Glycerolipid) lipidHash = ((Glycerolipid)currentLipid).getHashCode();
                else if (currentLipid is Phospholipid) lipidHash = ((Phospholipid)currentLipid).getHashCode();
                else if (currentLipid is Sphingolipid) lipidHash = ((Sphingolipid)currentLipid).getHashCode();
                else if (currentLipid is Sterol) lipidHash = ((Sterol)currentLipid).getHashCode();
                else if (currentLipid is Mediator) lipidHash = ((Mediator)currentLipid).getHashCode();
                else if (currentLipid is UnsupportedLipid) lipidHash = ((UnsupportedLipid)currentLipid).getHashCode();
            
                if (!creatorGUI.lipidCreator.registeredLipidDictionary.ContainsKey(lipidHash))
                {
                    currentLipid.onlyPrecursors = filterParameters[0];
                    currentLipid.onlyHeavyLabeled = filterParameters[1];
                    creatorGUI.lipidCreator.registeredLipidDictionary.Add(lipidHash, currentLipid);
                    creatorGUI.lipidCreator.registeredLipids.Add(lipidHash);
                }
            }
            creatorGUI.refreshRegisteredLipidsTable();
            Close();
        }
    }
}
