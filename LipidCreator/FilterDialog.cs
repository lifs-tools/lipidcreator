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
            
            switch (lipid.onlyPrecursors){
                case 0: radioButton1.Checked = true; break;
                case 1: radioButton2.Checked = true; break;
                case 2: radioButton3.Checked = true; break;
            }
            
            switch (lipid.onlyHeavyLabeled){
                case 0: radioButton4.Checked = true; break;
                case 1: radioButton5.Checked = true; break;
                case 2: radioButton6.Checked = true; break;
            }
        }
        
        private void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void applyClick(object sender, EventArgs e)
        {
            if (radioButton1.Checked) lipid.onlyPrecursors = 0;
            else if (radioButton2.Checked) lipid.onlyPrecursors = 1;
            else if (radioButton3.Checked) lipid.onlyPrecursors = 2;
            
            if (radioButton4.Checked) lipid.onlyHeavyLabeled = 0;
            else if (radioButton5.Checked) lipid.onlyHeavyLabeled = 1;
            else if (radioButton6.Checked) lipid.onlyHeavyLabeled = 2;
            
            this.Close();
        }
    }
}
