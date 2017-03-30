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
using System.IO;
using System.Windows.Forms;


namespace LipidCreator
{
    [Serializable]
    public partial class LipidsReview : Form
    {
        public DataTable allFragments;
        public DataTable allFragmentsUnique;
        public DataTable currentView;
        public LipidCreatorForm lipidCreatorForm;
        public LipidsReview(LipidCreatorForm lipidCreatorForm, DataTable allFragments, DataTable allFragmentsUnique)
        {
            this.lipidCreatorForm = lipidCreatorForm;
            this.allFragments = allFragments;
            this.allFragmentsUnique = allFragmentsUnique;
            InitializeComponent();
            currentView = this.allFragments;
            dataGridViewTransitions.DataSource = currentView;
            buttonSendToSkyline.Enabled = lipidCreatorForm.openedAsExternal;
            checkBoxCreateSpectralLibrary.Enabled = lipidCreatorForm.openedAsExternal;
            labelNumberOfTransitions.Text = "Number of transitions: " + currentView.Rows.Count;
            foreach (DataGridViewColumn dgvc in dataGridViewTransitions.Columns)
            {
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGridViewTransitions.Update();
            dataGridViewTransitions.Refresh();
        }
    
        public void buttonSendToSkylineClick(Object sender, EventArgs e)
        {
            this.Enabled = false;
            
            if (checkBoxCreateSpectralLibrary.Checked)
            {
                String[] specName = new String[]{""};
                SpectralName spectralName = new SpectralName(specName);
                spectralName.Owner = this;
                spectralName.ShowInTaskbar = false;
                spectralName.ShowDialog();
                spectralName.Dispose();
                if (specName[0].Length > 0)
                {
                    string blibPath = Application.StartupPath + "\\" + specName[0] + ".blib";
                    lipidCreatorForm.createBlib(blibPath);
                    lipidCreatorForm.sendToSkyline(allFragments, specName[0], blibPath);
                    MessageBox.Show("Sending transition list and spectral library to Skyline is complete.", "Sending complete");
                }
            }
            else {
                lipidCreatorForm.sendToSkyline(allFragments, "", "");
                MessageBox.Show("Sending transition list to Skyline is complete.", "Sending complete");
            }
            this.Enabled = true;
        }

        private void checkBoxCheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                currentView = this.allFragmentsUnique;
            }
            else
            {
                currentView = this.allFragments;
            }
            labelNumberOfTransitions.Text = "Number of transitions: " + currentView.Rows.Count;
            dataGridViewTransitions.DataSource = currentView;
            dataGridViewTransitions.Update();
            dataGridViewTransitions.Refresh();
        }

        private void buttonStoreTransitionListClick(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            
            saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;

            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DialogResult mbr = MessageBox.Show("Split polarities into two separate files?", "Storing mode", MessageBoxButtons.YesNo);
                
                if (mbr == DialogResult.Yes)
                {
                    this.Enabled = false;
                    using (StreamWriter outputFile = new StreamWriter(Path.GetFullPath(saveFileDialog1.FileName).Replace(".csv", "_positive.csv"))) {
                        foreach (DataRow row in currentView.Rows)
                        {
                            if (((String)row["Precursor Charge"]) == "+1" || ((String)row["Precursor Charge"]) == "+2")
                            {
                                outputFile.WriteLine((String)row["Molecule List Name"] + "," +
                                (String)row["Precursor Name"] + "," +
                                (String)row["Precursor Ion Formula"] + "," +
                                (String)row["Precursor Adduct"] + "," +
                                ((String)row["Precursor m/z"]).Replace(",", ".") + "," +
                                (String)row["Precursor Charge"] + "," +
                                (String)row["Product Name"] + "," +
                                (String)row["Product Ion Formula"] + "," +
                                ((String)row["Product m/z"]).Replace(",", ".") + "," +
                                (String)row["Product Charge"]);
                            }
                        }
                    }
                    using (StreamWriter outputFile = new StreamWriter(Path.GetFullPath(saveFileDialog1.FileName).Replace(".csv", "_negative.csv"))) {
                        foreach (DataRow row in currentView.Rows)
                        {
                            if (((String)row["Precursor Charge"]) == "-1" || ((String)row["Precursor Charge"]) == "-2")
                            {
                                outputFile.WriteLine((String)row["Molecule List Name"] + "," +
                                (String)row["Precursor Name"] + "," +
                                (String)row["Precursor Ion Formula"] + "," +
                                (String)row["Precursor Adduct"] + "," +
                                ((String)row["Precursor m/z"]).Replace(",", ".") + "," +
                                (String)row["Precursor Charge"] + "," +
                                (String)row["Product Name"] + "," +
                                (String)row["Product Ion Formula"] + "," +
                                ((String)row["Product m/z"]).Replace(",", ".") + "," +
                                (String)row["Product Charge"]);
                            }
                        }
                    }
                    MessageBox.Show("Storing of transition list is complete.", "Storing complete");
                    this.Enabled = true;
                }
                else
                {
                    this.Enabled = false;
                    StreamWriter writer;
                    if((writer = new StreamWriter(saveFileDialog1.OpenFile())) != null)
                    {
                        foreach (DataRow row in currentView.Rows)
                        {
                            writer.WriteLine((String)row["Molecule List Name"] + "," +
                            (String)row["Precursor Name"] + "," +
                            (String)row["Precursor Ion Formula"] + "," +
                            (String)row["Precursor Adduct"] + "," +
                            ((String)row["Precursor m/z"]).Replace(",", ".") + "," +
                            (String)row["Precursor Charge"] + "," +
                            (String)row["Product Name"] + "," +
                            (String)row["Product Ion Formula"] + "," +
                            ((String)row["Product m/z"]).Replace(",", ".") + "," +
                            (String)row["Product Charge"]);
                        }
                        writer.Dispose();
                        writer.Close();
                    }
                    MessageBox.Show("Storing of transition list is complete.", "Storing complete");
                    this.Enabled = true;
                }
            }
        }

        private void buttonStoreSpectralLibraryClick(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            
            saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.Filter = "blib files (*.blib)|*.blib|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;

            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Enabled = false;
                lipidCreatorForm.createBlib(Path.GetFullPath(saveFileDialog1.FileName));
                MessageBox.Show("Storing of spectral library is complete.", "Storing complete");
                this.Enabled = true;
            }
        }
    }
}
