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
    public partial class CEInspector : Form
    {
        public CreatorGUI creatorGUI;
        public double[] xValCoords;
        public double[][] yValCoords;
        public string[] fragmentNames;

        
        public CEInspector(CreatorGUI _creatorGUI)
        {
            creatorGUI = _creatorGUI;
            InitializeComponent();
            var collisionEnergyHandler = creatorGUI.lipidCreator.collisionEnergyHandler;
            
            xValCoords = new double[cartesean.innerWidthPx + 1];
            int n = collisionEnergyHandler.instrumentParameters["MS:1002523"]["12-HETE/d8"].Count;
            fragmentNames = new string[n];
            yValCoords = new double[n][];
            
            
            for (int i = 0; i <= cartesean.innerWidthPx; ++i)
            {
                xValCoords[i] = ((double)i) / cartesean.innerWidthPx * cartesean.maxXVal;
            }
            
            int k = 0;
            foreach(string fragmentName in collisionEnergyHandler.instrumentParameters["MS:1002523"]["12-HETE/d8"].Keys)
            {
                yValCoords[k] = new double[cartesean.innerWidthPx + 1];
                fragmentNames[k] = fragmentName;
                int j = 0;
                foreach (double valX in xValCoords)
                {
                    yValCoords[k][j] = 10000 * collisionEnergyHandler.getIntensity("MS:1002523", "12-HETE/d8", fragmentName, "[M8H2-H]1-", valX);
                    ++j;
                }
                ++k;
            }
            
        }
        
        public void instrumentComboboxChanged(Object sender, EventArgs e)
        {
        
        }
        
        public void classComboboxChanged(Object sender, EventArgs e)
        {
        
        }
        
        public void adductComboboxChanged(Object sender, EventArgs e)
        {
        
        }
        
        
        
        public void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        public void applyClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        public void mouseMove(object sender, MouseEventArgs e)
        {
            if (cartesean.marginLeft <= e.X && e.X <= cartesean.Width - cartesean.marginRight)
            {
                PointF vals = cartesean.pxToValue(e.X, e.Y);
                int pos = Math.Max(0, e.X - cartesean.marginLeft);
                pos = Math.Min(pos, cartesean.innerWidthPx);
                int highlight = -1;
                for (int k = 0; k < fragmentNames.Length; ++k)
                {
                    if (vals.Y - cartesean.offset <= yValCoords[k][pos] && yValCoords[k][pos] <= vals.Y + cartesean.offset)
                    {
                        highlight = k;
                        break;
                    }
                }
                if (cartesean.highlight != highlight)
                {
                    cartesean.highlight = highlight;
                    cartesean.Refresh();
                }
                if (highlight > -1)
                {
                    ToolTip1.SetToolTip(cartesean, fragmentNames[highlight]);
                }
                else
                {
                    ToolTip1.SetToolTip(cartesean, null);
                }
            }
        }
    }
}
