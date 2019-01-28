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

        public LipidsInterList (CreatorGUI _creatorGUI)
        {
            creatorGUI = _creatorGUI;
            
            precursorDataTable = new DataTable("precursorDataTable");
            precursorDataTable.Columns.Add(new DataColumn("Keep"));
            precursorDataTable.Columns[0].DataType = typeof(bool);
            precursorDataTable.Columns.Add(new DataColumn("Precursor name"));
            precursorDataTable.Columns[1].DataType = typeof(string);
            precursorDataTable.Columns.Add(new DataColumn("Category"));
            precursorDataTable.Columns[2].DataType = typeof(string);
            precursorDataTable.Columns.Add(new DataColumn("reference"));
            precursorDataTable.Columns[3].DataType = typeof(PrecursorData);
            
            InitializeComponent ();
            
            
            foreach(PrecursorData precursorData in creatorGUI.lipidCreator.precursorDataList)
            {
                DataRow row = precursorDataTable.NewRow();
                row["Keep"] = precursorData.precursorSelected;
                row["Precursor name"] = precursorData.precursorName;
                row["Category"] = precursorData.lipidCategory.ToString();
                row["reference"] = precursorData;
                precursorDataTable.Rows.Add(row);
            }
            
            dataGridViewPrecursors.Update();
            refreshDataGridViewPrecursors();
        }
        
        
        
        
        
        private void precursorGridViewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridViewPrecursors.Columns[0].Width = 50;
            dataGridViewPrecursors.Columns[1].ReadOnly = true;
            dataGridViewPrecursors.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewPrecursors.Columns[2].ReadOnly = true;
            dataGridViewPrecursors.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewPrecursors.Columns[3].Visible = false;
            
        }
        
        
        
        
        private void refreshDataGridViewPrecursors()
        {
            if(this.dataGridViewPrecursors.InvokeRequired) {
                this.dataGridViewPrecursors.Invoke(new Action(() => {this.dataGridViewPrecursors.Refresh();}));
            } else {
                this.dataGridViewPrecursors.Refresh();
            }
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
        }
        
        
        
        
        public void cancelButtonClick (Object sender, EventArgs e)
        {
            Close();
        }
        
        
        
        
        
        public void continueReviewButtonClick (Object sender, EventArgs e)
        {
            if (LipidCreator.LC_OS == PlatformID.Unix)
            {
                Close();
            }
            else
            {
                Dispose();
            }
            creatorGUI.continueReviewForm();
        }
    }
}
