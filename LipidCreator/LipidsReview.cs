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
        public ArrayList replicates;
        public DataTable currentView;
        public CreatorGUI creatorGUI;
        public string[] dataColumns = {};

        public LipidsReview (CreatorGUI _creatorGUI)
        {
            creatorGUI = _creatorGUI;
            transitionList = creatorGUI.lipidCreator.transitionList;
            currentView = this.transitionList;
            replicates = new ArrayList();
            transitionListUnique = creatorGUI.lipidCreator.addDataColumns (new DataTable ());
            Dictionary<String, String> replicateKeys = new Dictionary<String, String> ();
            
            int i = 0;
            foreach (DataRow row in currentView.Rows)
            {
            string prec_mass = string.Format("{0:N4}%", (String)row [LipidCreator.PRECURSOR_MZ]);
            string prod_mass = string.Format("{0:N4}%", (((String)row [LipidCreator.PRODUCT_NEUTRAL_FORMULA]) != "" ? (String)row [LipidCreator.PRODUCT_MZ] : (String)row [LipidCreator.PRODUCT_NAME]));
                string replicateKey = prec_mass + "/" + prod_mass;
                if (!replicateKeys.ContainsKey (replicateKey)) {
                    string note = "replicate of " + (String)row[LipidCreator.PRECURSOR_NAME] + " " + (String)row[LipidCreator.PRECURSOR_ADDUCT] + " " + (String)row[LipidCreator.PRODUCT_NAME];
                    replicateKeys.Add(replicateKey, note);
                    transitionListUnique.ImportRow (row);
                }
                else
                {
                    row[LipidCreator.NOTE] = replicateKeys[replicateKey];
                    replicates.Add(i);
                }
                ++i;
            }
            
            
            InitializeComponent ();
            dataGridViewTransitions.DataSource = currentView;
            buttonSendToSkyline.Enabled = creatorGUI.lipidCreator.openedAsExternal;
            labelNumberOfTransitions.Text = "Number of transitions: " + currentView.Rows.Count;
            foreach (DataGridViewColumn dgvc in dataGridViewTransitions.Columns) {
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            
            dataGridViewTransitions.Update ();
            dataGridViewTransitions.Refresh ();
            
            buttonStoreSpectralLibrary.Enabled = creatorGUI.selectedInstrumentForCE.Length > 0 && (bool)creatorGUI.lipidCreator.msInstruments[creatorGUI.selectedInstrumentForCE][1];
            
            checkBoxCreateSpectralLibrary.Enabled = creatorGUI.lipidCreator.openedAsExternal && buttonStoreSpectralLibrary.Enabled;
        }
        
        
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
                String[] specName = new String[]{ "" };
                SpectralName spectralName = new SpectralName (specName);
                spectralName.Owner = this;
                spectralName.ShowInTaskbar = false;
                spectralName.ShowDialog ();
                spectralName.Dispose ();
                if (specName [0].Length > 0) {
                    string blibPath = Application.StartupPath + "\\..\\Temp\\" + specName[0] + ".blib";
                    creatorGUI.lipidCreator.createBlib (blibPath, creatorGUI.selectedInstrumentForCE);
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
            if (((CheckBox)sender).Checked) {            
                currentView = this.transitionListUnique;
            } else {
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
                
                if (mbr == DialogResult.Yes) {
                    this.Enabled = false;
                    using (StreamWriter outputFile = new StreamWriter (Path.GetFullPath (saveFileDialog1.FileName).Replace (mode, "_positive"+mode))) {
                        outputFile.WriteLine (toHeaderLine (separator, LipidCreator.DATA_COLUMN_KEYS));
                        foreach (DataRow row in currentView.Rows) {
                            if (((String)row [LipidCreator.PRECURSOR_CHARGE]) == "+1" || ((String)row [LipidCreator.PRECURSOR_CHARGE]) == "+2") {
                                outputFile.WriteLine (toLine (row, LipidCreator.DATA_COLUMN_KEYS, separator));
                            }
                        }
                        outputFile.Dispose ();
                        outputFile.Close ();
                    }
                    using (StreamWriter outputFile = new StreamWriter (Path.GetFullPath (saveFileDialog1.FileName).Replace (mode, "_negative"+mode))) {
                        outputFile.WriteLine (toHeaderLine (separator, LipidCreator.DATA_COLUMN_KEYS));
                        foreach (DataRow row in currentView.Rows) {
                            if (((String)row [LipidCreator.PRECURSOR_CHARGE]) == "-1" || ((String)row [LipidCreator.PRECURSOR_CHARGE]) == "-2") {
                                outputFile.WriteLine (toLine (row, LipidCreator.DATA_COLUMN_KEYS, separator));
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
                        writer.WriteLine (toHeaderLine (separator, LipidCreator.DATA_COLUMN_KEYS));
                        foreach (DataRow row in currentView.Rows) {
                            writer.WriteLine (toLine (row, LipidCreator.DATA_COLUMN_KEYS, separator));
                        }
                        writer.Dispose ();
                        writer.Close ();
                    }
                    MessageBox.Show ("Storing of transition list is complete.", "Storing complete");
                    this.Enabled = true;
                }
            }
        }

        private string toHeaderLine(string separator, string[] columnKeys) {
            string quote = "";
            if(separator==",") {
                quote = "\"";
            }
            return String.Join(separator, columnKeys.ToList().ConvertAll<string>(key => quote+key+quote).ToArray());
        }

        private string toLine (DataRow row, string[] columnKeys, string separator)
        {
            List<string> line = new List<string> ();
            foreach (String columnKey in LipidCreator.DATA_COLUMN_KEYS) {
                if (columnKey == LipidCreator.PRODUCT_MZ || columnKey == LipidCreator.PRECURSOR_MZ) {
                    line.Add (((String)row [columnKey]).Replace (",", "."));
                } else {
                    //quote strings when we are in csv mode
                    if (separator == ",")
                    {
                        line.Add("\""+((String)row[columnKey])+"\"");
                    } else { //otherwise just add the plain string
                        line.Add(((String)row[columnKey]));
                    }
                }
            }
            return String.Join (separator, line.ToArray ());
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
                    creatorGUI.lipidCreator.createBlib(Path.GetFullPath(saveFileDialog1.FileName), creatorGUI.selectedInstrumentForCE);
                    MessageBox.Show("Storing of spectral library is complete.", "Storing complete");
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Problem storing spectral library: " + exception.Message, "Problem storing spectral library");
                }
                this.Enabled = true;
            }
        }
    }
}
