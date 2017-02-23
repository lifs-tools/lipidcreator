using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace LipidCreator
{
    [Serializable]
    public partial class LipidsReview : Form
    {
        public DataTable allFragments;
        public DataTable allFragmentsUnique;
        public LipidCreatorForm lipidCreatorForm;
        public LipidsReview(LipidCreatorForm lipidCreatorForm, DataTable allFragments, DataTable allFragmentsUnique)
        {
            this.lipidCreatorForm = lipidCreatorForm;
            this.allFragments = allFragments;
            this.allFragmentsUnique = allFragmentsUnique;
            InitializeComponent();
            dataGridView1.DataSource = this.allFragments;
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
                dataGridView1.DataSource = this.allFragmentsUnique;
            }
            else
            {
                dataGridView1.DataSource = this.allFragments;
            }
            dataGridView1.Update();
            dataGridView1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
