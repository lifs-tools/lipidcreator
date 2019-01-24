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
        public ArrayList precursorList;
        public DataTable currentView;
        public CreatorGUI creatorGUI;
        public DataTable precursorDataList = null;

        public LipidsInterList (CreatorGUI _creatorGUI)
        {
            creatorGUI = _creatorGUI;
            precursorList = creatorGUI.lipidCreator.precursorDataList;
            precursorDataList = new DataTable("precursorDataList");
            precursorDataList.Columns.Add(new DataColumn("Keep"));
            precursorDataList.Columns[0].DataType = typeof(bool);
            precursorDataList.Columns.Add(new DataColumn("Precursor name"));
            precursorDataList.Columns[1].DataType = typeof(string);
            
            InitializeComponent ();
            
            foreach(PrecursorData precursorData in precursorList)
            {
                DataRow row = precursorDataList.NewRow();
                row["Keep"] = (bool)true;
                row["Precursor name"] = precursorData.precursorName;
                precursorDataList.Rows.Add(row);
            }
            dataGridViewPrecursors.Update();
            
        }
        
        private void precursorGridViewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridViewPrecursors.Columns[0].Width = 50;
            dataGridViewPrecursors.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewPrecursors.Columns[1].ReadOnly = true;
            dataGridViewPrecursors.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        
        
        public void cancelButtonClick (Object sender, EventArgs e)
        {
            Close();
        }
        
        
        
        public void continueReviewButtonClick (Object sender, EventArgs e)
        {
            for (int i = precursorDataList.Rows.Count - 1; i >= 0; i--)
            {
                if (!((bool)precursorDataList.Rows[i]["Keep"]))
                {
                    creatorGUI.lipidCreator.precursorDataList.RemoveAt(i);
                }
            }
        
            Close();
            creatorGUI.continueReviewForm();
        }
    }
}
