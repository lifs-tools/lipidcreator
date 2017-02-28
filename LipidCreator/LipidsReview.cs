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
            dataGridView1.DataSource = currentView;
            button1.Enabled = lipidCreatorForm.opened_as_external;
            checkBox2.Enabled = lipidCreatorForm.opened_as_external;
            label1.Text = "Number of transitions: " + currentView.Rows.Count;
            foreach (DataGridViewColumn dgvc in dataGridView1.Columns)
            {
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGridView1.Update();
            dataGridView1.Refresh();
        }
    
        public void button1_Click(Object sender, EventArgs e)
        {
            this.Enabled = false;
            
            if (checkBox2.Checked)
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
                    lipidCreatorForm.send_to_Skyline(allFragments, specName[0], blibPath);
                    MessageBox.Show("Sending transition list and spectral library to Skyline is complete.", "Sending complete");
                }
            }
            else {
                lipidCreatorForm.send_to_Skyline(allFragments, "", "");
                MessageBox.Show("Sending transition list to Skyline is complete.", "Sending complete");
            }
            this.Enabled = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                currentView = this.allFragmentsUnique;
            }
            else
            {
                currentView = this.allFragments;
            }
            label1.Text = "Number of transitions: " + currentView.Rows.Count;
            dataGridView1.DataSource = currentView;
            dataGridView1.Update();
            dataGridView1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
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
                                (String)row["Pruduct Name"] + "," +
                                (String)row["Pruduct Ion Formula"] + "," +
                                ((String)row["Pruduct m/z"]).Replace(",", ".") + "," +
                                (String)row["Pruduct Charge"]);
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
                                (String)row["Pruduct Name"] + "," +
                                (String)row["Pruduct Ion Formula"] + "," +
                                ((String)row["Pruduct m/z"]).Replace(",", ".") + "," +
                                (String)row["Pruduct Charge"]);
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
                            (String)row["Pruduct Name"] + "," +
                            (String)row["Pruduct Ion Formula"] + "," +
                            ((String)row["Pruduct m/z"]).Replace(",", ".") + "," +
                            (String)row["Pruduct Charge"]);
                        }
                        writer.Dispose();
                        writer.Close();
                    }
                    MessageBox.Show("Storing of transition list is complete.", "Storing complete");
                    this.Enabled = true;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
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
