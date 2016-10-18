using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LipidCreator
{
    public partial class LipidsReview : Form
    {
        DataTable allFragments;
        public LipidsReview(DataTable allFragments)
        {
            this.allFragments = allFragments;
            InitializeComponent();
            dataGridView1.DataSource = this.allFragments;
            Console.WriteLine(dataGridView1.Columns.Count);
            foreach (DataGridViewColumn dgvc in dataGridView1.Columns)
            {
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGridView1.Update();
            dataGridView1.Refresh();
        }
    }
}
