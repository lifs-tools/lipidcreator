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
    public partial class ExportParameters : Form
    {
        public string[] returnMessage = null;
        
        public ExportParameters(string[] _returnMessage)
        {
            returnMessage = _returnMessage;
            InitializeComponent();
        }
        
        protected void mergeClick(object sender, System.EventArgs e)
        {
            returnMessage[0] = "merge";
            Close();        
        }
        
        protected void replaceClick(object sender, System.EventArgs e)
        {
            returnMessage[0] = "replace";
            Close();
        }
    }
}
