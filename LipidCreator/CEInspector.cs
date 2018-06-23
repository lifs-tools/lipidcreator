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
        public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> instrumentParameters;
        public string selectedInstrument;
        public string selectedClass;
        public string selectedAdduct;
        
        public CEInspector(CreatorGUI _creatorGUI)
        {
            creatorGUI = _creatorGUI;
            instrumentParameters = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>();
            InitializeComponent();
            
            // foreach instrument
            foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> kvp1 in creatorGUI.lipidCreator.collisionEnergyHandler.instrumentParameters)
            {
                Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> p1 = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();
                instrumentParameters.Add(kvp1.Key, p1);
                
                // foreach class
                foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> kvp2 in kvp1.Value)
                {
                    Dictionary<string, Dictionary<string, Dictionary<string, string>>> p2 = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
                    p1.Add(kvp2.Key, p2);
                    
                    // foreach adduct
                    foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, string>>> kvp3 in kvp2.Value)
                    {
                        Dictionary<string, Dictionary<string, string>> p3 = new Dictionary<string, Dictionary<string, string>>();
                        p2.Add(kvp3.Key, p3);
                        
                        
                        // foreach fragment
                        foreach(KeyValuePair<string, Dictionary<string, string>> kvp4 in kvp3.Value)
                        {
                            Dictionary<string, string> p4 = new Dictionary<string, string>();
                            p3.Add(kvp4.Key, p4);
                            
                            // foreach parameter
                            foreach(KeyValuePair<string, string> p5 in kvp4.Value)
                            {
                                p4.Add(p5.Key, p5.Value);
                            }
                        }
                    }
                }
            }
            
            
            foreach (string instrumentName in instrumentParameters.Keys)
            {
                instrumentCombobox.Items.Add(instrumentName);
                instrumentCombobox.SelectedIndex = 0;
            }
            
        }
        
        public void computeCurves()
        {
            cartesean.CEval = 0;
            xValCoords = new double[cartesean.innerWidthPx + 1];
            int n = instrumentParameters[selectedInstrument][selectedClass][selectedAdduct].Count;
            fragmentNames = new string[n];
            yValCoords = new double[n][];
            
            
            for (int i = 0; i <= cartesean.innerWidthPx; ++i)
            {
                xValCoords[i] = ((double)i) / cartesean.innerWidthPx * cartesean.maxXVal;
            }
            
            int k = 0;
            int topRank = 100000;
            string topFragment = "";
            foreach(string fragmentName in instrumentParameters[selectedInstrument][selectedClass][selectedAdduct].Keys)
            {
                yValCoords[k] = new double[cartesean.innerWidthPx + 1];
                fragmentNames[k] = fragmentName;
                int j = 0;
                
                int rank = creatorGUI.lipidCreator.collisionEnergyHandler.getRank(selectedInstrument, selectedClass, selectedAdduct, fragmentName);
                if (topRank > rank)
                {
                    topRank = rank;
                    topFragment = fragmentName;
                }
                
                
                foreach (double valX in xValCoords)
                {
                    yValCoords[k][j] = 10000 * creatorGUI.lipidCreator.collisionEnergyHandler.getIntensity(selectedInstrument, selectedClass, selectedAdduct, fragmentName, valX);
                    ++j;
                }
                ++k;
            }
            
            cartesean.CEval = creatorGUI.lipidCreator.collisionEnergyHandler.getCollisionEnergy(selectedInstrument, selectedClass, selectedAdduct, topFragment);
            
            cartesean.Refresh();
        }
        
        public void instrumentComboboxChanged(Object sender, EventArgs e)
        {
            selectedInstrument = (string)instrumentCombobox.Items[instrumentCombobox.SelectedIndex];
            classCombobox.Items.Clear();
            foreach(string lipidClass in instrumentParameters[selectedInstrument].Keys)
            {
                classCombobox.Items.Add(lipidClass);
            }
            classCombobox.SelectedIndex = 0;
        }
        
        public void classComboboxChanged(Object sender, EventArgs e)
        {
            selectedClass = (string)classCombobox.Items[classCombobox.SelectedIndex];
            adductCombobox.Items.Clear();
            foreach(string adduct in instrumentParameters[selectedInstrument][selectedClass].Keys)
            {
                adductCombobox.Items.Add(adduct);
            }
            adductCombobox.SelectedIndex = 0;
        }
        
        
        public void adductComboboxChanged(Object sender, EventArgs e)
        {
            selectedAdduct = (string)adductCombobox.Items[adductCombobox.SelectedIndex];
            computeCurves();
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
                    ToolTip1.Hide(cartesean);
                }
            }
        }
    }
}
