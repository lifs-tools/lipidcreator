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
    public partial class FilterDialog : Form
    {
        CreatorGUI creatorGUI;
        Lipid lipid;
        public FilterDialog(CreatorGUI _creatorGUI, Lipid _lipid)
        {
            creatorGUI = _creatorGUI;
            lipid = _lipid;
            
            InitializeComponent();
            
            checkBox1.Checked = lipid.onlyPrecursors;
            checkBox2.Checked = lipid.onlyHeavyLabeled;
        }
        
        private void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void applyClick(object sender, EventArgs e)
        {
            lipid.onlyPrecursors = checkBox1.Checked;
            lipid.onlyHeavyLabeled = checkBox2.Checked;
            this.Close();
        }
    }
}
