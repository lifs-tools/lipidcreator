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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;


namespace LipidCreator
{
    [Serializable]
    public partial class LipidsReview : Form
    {
        public DataTable transitionList;
        public DataTable transitionListUnique;
        public DataTable currentView;
        public LipidCreator lipidCreatorForm;
        public string[] dataColumns = { };

        public LipidsReview (LipidCreator lipidCreatorForm)
        {
            this.lipidCreatorForm = lipidCreatorForm;
            transitionList = lipidCreatorForm.transitionList;
            transitionListUnique = null;
            
            InitializeComponent ();
            currentView = this.transitionList;
            dataGridViewTransitions.DataSource = currentView;
            buttonSendToSkyline.Enabled = lipidCreatorForm.openedAsExternal;
            checkBoxCreateSpectralLibrary.Enabled = lipidCreatorForm.openedAsExternal;
            labelNumberOfTransitions.Text = "Number of transitions: " + currentView.Rows.Count;
            foreach (DataGridViewColumn dgvc in dataGridViewTransitions.Columns) {
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGridViewTransitions.Update ();
            dataGridViewTransitions.Refresh ();
        }

        public void buttonSendToSkylineClick (Object sender, EventArgs e)
        {
            this.Enabled = false;
            
            if (checkBoxCreateSpectralLibrary.Checked) {
                String[] specName = new String[]{ "" };
                SpectralName spectralName = new SpectralName (specName);
                spectralName.Owner = this;
                spectralName.ShowInTaskbar = false;
                spectralName.ShowDialog ();
                spectralName.Dispose ();
                if (specName [0].Length > 0) {
                    string blibPath = Application.StartupPath + "\\" + specName [0] + ".blib";
                    lipidCreatorForm.createBlib (blibPath);
                    lipidCreatorForm.sendToSkyline (transitionList, specName [0], blibPath);
                    MessageBox.Show ("Sending transition list and spectral library to Skyline is complete.", "Sending complete");
                }
            } else {
                lipidCreatorForm.sendToSkyline (transitionList, "", "");
                MessageBox.Show ("Sending transition list to Skyline is complete.", "Sending complete");
            }
            this.Enabled = true;
        }

        private void checkBoxCheckedChanged (object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) {
                if (transitionListUnique == null) {
                    transitionListUnique = lipidCreatorForm.addDataColumns (new DataTable ());
                    HashSet<String> replicates = new HashSet<String> ();
                    
                    foreach (DataRow row in currentView.Rows) {
                        string replicateKey = (String)row [LipidCreator.PRECURSOR_NEUTRAL_FORMULA] + "/" + (((String)row [LipidCreator.PRODUCT_NEUTRAL_FORMULA]) != "" ? (String)row [LipidCreator.PRODUCT_NEUTRAL_FORMULA] : (String)row [LipidCreator.PRODUCT_NAME]);
                        if (!replicates.Contains (replicateKey)) {
                            replicates.Add (replicateKey);
                            transitionListUnique.ImportRow (row);
                        }
                    }
                    
                }
            
                currentView = this.transitionList;
            } else {
                currentView = this.transitionList;
            }
            labelNumberOfTransitions.Text = "Number of transitions: " + currentView.Rows.Count;
            dataGridViewTransitions.DataSource = currentView;
            dataGridViewTransitions.Update ();
            dataGridViewTransitions.Refresh ();
        }

        private void buttonStoreTransitionListClick (object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog ();
            
            saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog () == DialogResult.OK) {
                DialogResult mbr = MessageBox.Show ("Split polarities into two separate files?", "Storing mode", MessageBoxButtons.YesNo);
                
                if (mbr == DialogResult.Yes) {
                    this.Enabled = false;
                    using (StreamWriter outputFile = new StreamWriter (Path.GetFullPath (saveFileDialog1.FileName).Replace (".csv", "_positive.csv"))) {
                        outputFile.WriteLine (String.Join (",", LipidCreator.DATA_COLUMN_KEYS));
                        foreach (DataRow row in currentView.Rows) {
                            if (((String)row [LipidCreator.PRECURSOR_CHARGE]) == "+1" || ((String)row [LipidCreator.PRECURSOR_CHARGE]) == "+2") {
                                outputFile.WriteLine (toLine (row, LipidCreator.DATA_COLUMN_KEYS));
                            }
                        }
                        outputFile.Dispose ();
                        outputFile.Close ();
                    }
                    using (StreamWriter outputFile = new StreamWriter (Path.GetFullPath (saveFileDialog1.FileName).Replace (".csv", "_negative.csv"))) {
                        outputFile.WriteLine (String.Join (",", LipidCreator.DATA_COLUMN_KEYS));
                        foreach (DataRow row in currentView.Rows) {
                            if (((String)row [LipidCreator.PRECURSOR_CHARGE]) == "-1" || ((String)row [LipidCreator.PRECURSOR_CHARGE]) == "-2") {
                                outputFile.WriteLine (toLine (row, LipidCreator.DATA_COLUMN_KEYS));
                            }
                        }
                        outputFile.Dispose ();
                        outputFile.Close ();
                    }
                    MessageBox.Show ("Storing of transition list is complete.", "Storing complete");
                    this.Enabled = true;
                } else {
                    this.Enabled = false;
                    StreamWriter writer;
                    if ((writer = new StreamWriter (saveFileDialog1.OpenFile ())) != null) {
                        writer.WriteLine (String.Join (",", LipidCreator.DATA_COLUMN_KEYS));
                        foreach (DataRow row in currentView.Rows) {
                            writer.WriteLine (toLine (row, LipidCreator.DATA_COLUMN_KEYS));
                        }
                        writer.Dispose ();
                        writer.Close ();
                    }
                    MessageBox.Show ("Storing of transition list is complete.", "Storing complete");
                    this.Enabled = true;
                }
            }
        }

        private string toLine (DataRow row, string[] columnKeys)
        {
            List<string> line = new List<string> ();
            foreach (String columnKey in LipidCreator.DATA_COLUMN_KEYS) {
                if (columnKey == LipidCreator.PRODUCT_MZ || columnKey == LipidCreator.PRECURSOR_MZ) {
                    line.Add (((String)row [columnKey]).Replace (",", "."));
                } else {
                    line.Add (((String)row [columnKey]));
                }
            }
            return String.Join (",", line.ToArray ());
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
                lipidCreatorForm.createBlib (Path.GetFullPath (saveFileDialog1.FileName));
                MessageBox.Show ("Storing of spectral library is complete.", "Storing complete");
                this.Enabled = true;
            }
        }
    }
}
