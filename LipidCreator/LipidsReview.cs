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
        public LipidCreatorForm lipidCreatorForm;
        public LipidsReview(LipidCreatorForm lipidCreatorForm, DataTable allFragments)
        {
            this.lipidCreatorForm = lipidCreatorForm;
            this.allFragments = allFragments;
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
    }
}
