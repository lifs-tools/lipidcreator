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
            
            /*
            
            dataGridViewTransitions.DataSource = currentView;
            buttonSendToSkyline.Enabled = creatorGUI.lipidCreator.openedAsExternal;
            labelNumberOfTransitions.Text = "Number of transitions: " + currentView.Rows.Count;
            foreach (DataGridViewColumn dgvc in dataGridViewTransitions.Columns) {
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            
            dataGridViewTransitions.Update ();
            dataGridViewTransitions.Refresh ();
            
            if (creatorGUI.lipidCreator.selectedInstrumentForCE.Length > 0)
            {
                InstrumentData instrumentData = creatorGUI.lipidCreator.msInstruments[creatorGUI.lipidCreator.selectedInstrumentForCE];
                buttonStoreSpectralLibrary.Enabled = instrumentData.minCE > 0 && instrumentData.maxCE > 0 && instrumentData.minCE < instrumentData.maxCE;
            }
            
            checkBoxCreateSpectralLibrary.Enabled = creatorGUI.lipidCreator.openedAsExternal && buttonStoreSpectralLibrary.Enabled;
            */
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
            Close();
            creatorGUI.continueReviewForm();
        }
        
        
        /*
        private void gridviewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (currentView == transitionList)
            {
                foreach (int i in replicates) dataGridViewTransitions.Rows[i].DefaultCellStyle.BackColor = Color.Beige;
            }
            else dataGridViewTransitions.DefaultCellStyle.BackColor = Color.Empty;
            foreach (DataGridViewColumn d in dataGridViewTransitions.Columns) d.SortMode = DataGridViewColumnSortMode.NotSortable;
        }
        
        

        public void buttonSendToSkylineClick (Object sender, EventArgs e)
        {
            this.Enabled = false;
            
            if (checkBoxCreateSpectralLibrary.Checked) {
                String[] specName = new String[]{""};
                SpectralName spectralName = new SpectralName (specName);
                spectralName.Owner = this;
                spectralName.ShowInTaskbar = false;
                spectralName.ShowDialog ();
                spectralName.Dispose ();
                if (specName [0].Length > 0) {
                    string blibPath = Application.StartupPath + "\\..\\Temp\\" + specName[0] + ".blib";
                    creatorGUI.lipidCreator.createBlib (blibPath);
                    creatorGUI.lipidCreator.sendToSkyline (currentView, specName[0], blibPath);
                    MessageBox.Show ("Sending transition list and spectral library to Skyline is complete.", "Sending complete");
                }
            } else {
                creatorGUI.lipidCreator.sendToSkyline (currentView, "", "");
                MessageBox.Show ("Sending transition list to Skyline is complete.", "Sending complete");
            }
            this.Enabled = true;
        }

        
        
        
        private void checkBoxCheckedChanged (object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {            
                currentView = this.transitionListUnique;
            }
            else
            {
                currentView = this.transitionList;
            }
            
            labelNumberOfTransitions.Text = "Number of transitions: " + currentView.Rows.Count;
            dataGridViewTransitions.DataSource = currentView;
            dataGridViewTransitions.Update();
            dataGridViewTransitions.Refresh();
        }
        
        
        
        

        private void buttonStoreTransitionListClick (object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog ();
            
            saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv|tsv files (*.tsv)|*.tsv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog () == DialogResult.OK) {
                string mode = ".csv";
                string separator = ",";
                if(saveFileDialog1.FilterIndex==2) {
                    mode = ".tsv";
                    separator = "\t";
                }
                DialogResult mbr = MessageBox.Show ("Split polarities into two separate files?", "Storing mode", MessageBoxButtons.YesNo);
                
                this.Enabled = false;
                creatorGUI.lipidCreator.storeTransitionList(separator, (mbr == DialogResult.Yes), Path.GetFullPath (saveFileDialog1.FileName), currentView, mode);
                this.Enabled = true;
                MessageBox.Show ("Storing of transition list is complete.", "Storing complete");
            }
        }
        
        
        
        

        private void buttonStoreSpectralLibraryClick (object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog ();
            
            saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.Filter = "blib files (*.blib)|*.blib|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog () == DialogResult.OK) {
                this.Enabled = false;
                try
                {
                    creatorGUI.lipidCreator.createBlib(Path.GetFullPath(saveFileDialog1.FileName));
                    MessageBox.Show("Storing of spectral library is complete.", "Storing complete");
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Problem storing spectral library: " + exception.Message, "Problem storing spectral library");
                }
                this.Enabled = true;
            }
        }
        */
    }
}
