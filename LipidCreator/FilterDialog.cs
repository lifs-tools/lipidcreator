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
            
            comboBox1.SelectedIndex = lipid.onlyPrecursors;
            comboBox2.SelectedIndex = lipid.onlyHeavyLabeled;
        }
        
        private void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void applyClick(object sender, EventArgs e)
        {
            lipid.onlyPrecursors = comboBox1.SelectedIndex;
            lipid.onlyHeavyLabeled = comboBox2.SelectedIndex;
            this.Close();
        }
    }
}
