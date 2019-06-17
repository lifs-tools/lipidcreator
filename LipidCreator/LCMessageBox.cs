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
    public partial class LCMessageBox : Form
    {
        public int[] returnMessage = null;
        public int type = 0;
        public CreatorGUI creatorGUI;
        
        public LCMessageBox(int[] _returnMessage, int _type, LipidException lipidException = null)
        {
            returnMessage = _returnMessage;
            type = _type;
            InitializeComponent();
            
            switch (type)
            {
                case 0:
                    richTextBox1.Text = "Do you want to merge or replace the imported file?";
                    button1.Text = "Merge";
                    button2.Text = "Replace";
                    Text = "Merge or Replace?";
                    break;
            
                case 1:
                
                    string lipidName = lipidException.precursorData.precursorName;
                    string fragmentName = lipidException.fragment.fragmentName;
                    string elementName = MS2Fragment.ALL_ELEMENTS[lipidException.molecule].shortcut;
                    creatorGUI = lipidException.creatorGUI;
                    int counts = lipidException.counts;
                    string heavyIsotope = lipidException.heavyIsotope.Length > 0 ? " the heavy isotope '{" + lipidException.heavyIsotope + "}' of" : "";
                    string infoText = "A problem occurred during the computation of fragment '" + fragmentName + "' for" + heavyIsotope + " lipid '" + lipidName + "'. The element '" + elementName + "' contains " + counts + " counts. Please update the fragment with regard on the element counts.";
                
                    richTextBox1.Text = infoText;
                    button1.Text = "Go to fragment";
                    button2.Text = "Close";
                    button1.Location = new System.Drawing.Point(button1.Location.X, button1.Location.Y + 30);
                    button2.Location = new System.Drawing.Point(button2.Location.X, button2.Location.Y + 30);
                    ClientSize = new System.Drawing.Size(ClientSize.Width, ClientSize.Height + 30);
                    Text = "Problem in fragment computation";
                    break;
            }
        }
        
        protected void timer_Elapsed(object sender, System.EventArgs e)
        {
            richTextBox1.Select(0,2);
        }
        
        protected void richTextBox1_Focus(object sender, System.EventArgs e)
        {
            button1.Focus();
        }
        
        protected void richTextBox1_MouseEnter(object sender, System.EventArgs e)
        {
            Cursor = Cursors.Default;
        }
        
        protected void mergeClick(object sender, System.EventArgs e)
        {
            returnMessage[0] = 0; // merge / go to fragment
            
            switch (type)
            {
                case 0:
                    Close();
                    break;
                
                case 1:
                    
                    break;
            }
        }
        
        protected void replaceClick(object sender, System.EventArgs e)
        {
            returnMessage[0] = 1; // replace / close
            Close();
        }
    }
}
