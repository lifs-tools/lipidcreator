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
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace LipidCreator
{
    [Serializable]
    public partial class LipidsInterList : Form
    {
        public CreatorGUI creatorGUI;
        public DataTable precursorDataTable = null;
        public ArrayList returnValues;

        public LipidsInterList (CreatorGUI _creatorGUI, ArrayList _returnValues)
        {
            creatorGUI = _creatorGUI;
            returnValues = _returnValues;
            
            precursorDataTable = new DataTable("precursorDataTable");
            precursorDataTable.Columns.Add(new DataColumn("Keep"));
            precursorDataTable.Columns[0].DataType = typeof(bool);
            precursorDataTable.Columns.Add(new DataColumn("Precursor name"));
            precursorDataTable.Columns[1].DataType = typeof(string);
            precursorDataTable.Columns.Add(new DataColumn("Adduct"));
            precursorDataTable.Columns[2].DataType = typeof(string);
            precursorDataTable.Columns.Add(new DataColumn("Category"));
            precursorDataTable.Columns[3].DataType = typeof(string);
            precursorDataTable.Columns.Add(new DataColumn("reference"));
            precursorDataTable.Columns[4].DataType = typeof(PrecursorData);
            
            InitializeComponent ();
            
            foreach(PrecursorData precursorData in creatorGUI.lipidCreator.precursorDataList)
            {
                DataRow row = precursorDataTable.NewRow();
                row["Keep"] = precursorData.precursorSelected;
                row["Precursor name"] = precursorData.precursorName;
                row["Adduct"] = precursorData.precursorAdduct.ToString();
                row["Category"] = precursorData.lipidCategory.ToString();
                row["reference"] = precursorData;
                precursorDataTable.Rows.Add(row);
            }
            
            dataGridViewPrecursors.Update();
            updateSelectedLabel();
            refreshDataGridViewPrecursors();
        }
        
        
        
        
        
        private void precursorGridViewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridViewPrecursors.Columns[0].Width = 50;
            dataGridViewPrecursors.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridViewPrecursors.Columns[1].Width = (int)Math.Floor(dataGridViewPrecursors.Size.Width * 0.4);
            dataGridViewPrecursors.Columns[1].ReadOnly = true;
            dataGridViewPrecursors.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewPrecursors.Columns[2].Width = (int)Math.Floor(dataGridViewPrecursors.Size.Width * 0.2);
            dataGridViewPrecursors.Columns[2].ReadOnly = true;
            dataGridViewPrecursors.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewPrecursors.Columns[3].ReadOnly = true;
            dataGridViewPrecursors.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewPrecursors.Columns[4].Visible = false;
            
        }
        
        
        
        
        
        public void updateSelectedLabel()
        {
            int count = 0;
            foreach (DataRow dataRow in precursorDataTable.Rows)
            {
                count += (bool)dataRow[0] ? 1 : 0;
            }
            labelSelected.Text = "Selected precursors: " + count.ToString();
            continueReviewButton.Enabled = count > 0;
        }
        
        
        
        
        private void refreshDataGridViewPrecursors()
        {
            if(this.dataGridViewPrecursors.InvokeRequired) {
                this.dataGridViewPrecursors.Invoke(new Action(() => {this.dataGridViewPrecursors.Refresh();}));
            } else {
                this.dataGridViewPrecursors.Refresh();
            }
        }
        
        
        
        private void precursorSelectAll(object sender, EventArgs e)
        {
            foreach (DataRow dataRow in precursorDataTable.Rows)
            {
                dataRow[0] = true;
                ((PrecursorData)dataRow[4]).precursorSelected = true;
            }
            refreshDataGridViewPrecursors();
            updateSelectedLabel();
        }
        
        
        
        private void precursorDeselectAll(object sender, EventArgs e)
        {
            foreach (DataRow dataRow in precursorDataTable.Rows)
            {
                dataRow[0] = false;
                ((PrecursorData)dataRow[4]).precursorSelected = false;
            }
            refreshDataGridViewPrecursors();
            updateSelectedLabel();
        }
        
        
        
        private void precursorGridView_CellClicked(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewPrecursors.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        
        
        private void precursorGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = ((DataGridView)sender).CurrentCell.RowIndex;
            PrecursorData precursorData = (PrecursorData)(((DataGridView)sender).Rows[rowIndex].Cells["reference"]).Value;
            precursorData.precursorSelected = (bool)((DataGridView)sender).Rows[rowIndex].Cells["Keep"].Value;
            updateSelectedLabel();
        }
        
        
        
        
        public void cancelButtonClick (Object sender, EventArgs e)
        {
            returnValues[0] = false;
            Close();
        }
        
        
        
        
        
        public void continueReviewButtonClick (Object sender, EventArgs e)
        {
            returnValues[1] = radioButton1.Checked ? 0 : (radioButton2.Checked ? 1 : 2);
            returnValues[0] = true;
            Close();
        }
    }
}
