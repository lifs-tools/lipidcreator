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
            foreach (DataGridViewColumn dgvc in dataGridView1.Columns)
            {
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGridView1.Update();
            dataGridView1.Refresh();
        }
    
        public void send_to_Skyline(Object sender, EventArgs e)
        {
            lipidCreatorForm.send_to_Skyline(allFragments);
            this.Close();
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
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
