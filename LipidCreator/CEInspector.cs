/*
MIT License

Copyright (c) 2017 Dominik Kopczynski   -   dominik.kopczynski {at} isas.de
                   Bing Peng   -   bing.peng {at} isas.de

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace LipidCreator
{
    public partial class CEInspector : Form
    {
        public CreatorGUI creatorGUI;
        public double[] xValCoords;
        public Dictionary<string, double[]> yValCoords;
        public Dictionary<string, double> fragmentApex;
        public Dictionary<string, Dictionary<string, Dictionary<string, double>>> collisionEnergies;  // for instrument / class / adduct
        public string selectedInstrument;
        public string selectedClass;
        public string selectedAdduct;
        public DataTable fragmentsList;
        public bool initialCall = true;
        public HashSet<string> selectedFragments;
        public ArrayList indexToInstrument;
        
        public CEInspector(CreatorGUI _creatorGUI, string _currentInstrument)
        {
            creatorGUI = _creatorGUI;
            selectedInstrument = _currentInstrument;
            collisionEnergies = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
            fragmentApex = new Dictionary<string, double>();
            selectedFragments = new HashSet<string>();
            indexToInstrument = new ArrayList();
            
            fragmentsList = new DataTable("fragmentsList");
            fragmentsList.Columns.Add(new DataColumn("View"));
            fragmentsList.Columns[0].DataType = typeof(bool);
            fragmentsList.Columns.Add(new DataColumn("Fragment name"));
            fragmentsList.Columns[1].DataType = typeof(string);
            
            
            
            InitializeComponent();
            textBoxCurrentCE.Text = String.Format(new CultureInfo("en-US"), "{0:0.00}",cartesean.CEval);
            
            // foreach instrument
            foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> kvp1 in creatorGUI.lipidCreator.collisionEnergyHandler.instrumentParameters)
            {
                Dictionary<string, Dictionary<string, double>> p1 = new Dictionary<string, Dictionary<string, double>>();
                collisionEnergies.Add(kvp1.Key, p1);
                
                // foreach class
                foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> kvp2 in kvp1.Value)
                {
                    Dictionary<string, double> p2 = new Dictionary<string, double>();
                    p1.Add(kvp2.Key, p2);
                    
                    // foreach adduct
                    foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, string>>> kvp3 in kvp2.Value)
                    {
                        var e = kvp3.Value.Keys.GetEnumerator();
                        e.MoveNext();
                        string firstFragment = e.Current;
                        p2.Add(kvp3.Key, Convert.ToDouble(kvp3.Value[firstFragment]["CE"], CultureInfo.InvariantCulture));
                    }
                }
            }
            
            
            int ii = 0;
            foreach (string instrumentName in collisionEnergies.Keys)
            {
                indexToInstrument.Add(instrumentName);
                instrumentCombobox.Items.Add(creatorGUI.lipidCreator.msInstruments[instrumentName][0]);
                if (instrumentName == selectedInstrument) instrumentCombobox.SelectedIndex = ii;
                ++ii;
            }
            
        }
        
        
        
        public void changeSmooth(object sender, System.Timers.ElapsedEventArgs e)
        {
            cartesean.smooth = true;
            cartesean.Refresh();
            timerSmooth.Enabled = false;
        }
        
        
        
        
        private void fragmentsGridViewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            fragmentsGridView.Columns[0].Width = 50;
            fragmentsGridView.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            fragmentsGridView.Columns[1].ReadOnly = true;
            fragmentsGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        
        
        public void computeCurves()
        {
            xValCoords = new double[cartesean.innerWidthPx + 1];
            int n = fragmentsList.Rows.Count;
            cartesean.setFragmentColors();
            
            yValCoords = new Dictionary<string, double[]>();
            yValCoords["productProfile"] = new double[cartesean.innerWidthPx + 1];
            
            fragmentApex.Clear();
            fragmentApex["productProfile"] = 0;
            
            double highestIntensity = 0;
            
            
            // compute x values (a.k.a. collision energies)
            for (int i = 0; i <= cartesean.innerWidthPx; ++i)
            {
                xValCoords[i] = cartesean.minXVal + ((double)i) / cartesean.innerWidthPx * (cartesean.maxXVal - cartesean.minXVal);
            }
            
            
            // precompute y values for all fragment model curves
            foreach(DataRow row in fragmentsList.Rows)
            {
                string fragmentName = (string)row["Fragment name"];
                           
                yValCoords[fragmentName] = new double[cartesean.innerWidthPx + 1];
                fragmentApex[fragmentName] = creatorGUI.lipidCreator.collisionEnergyHandler.getApex(selectedInstrument, selectedClass, selectedAdduct, fragmentName);
                double apexIntensity = creatorGUI.lipidCreator.collisionEnergyHandler.getIntensity(selectedInstrument, selectedClass, selectedAdduct, fragmentName, fragmentApex[fragmentName]);
                if (highestIntensity < apexIntensity) highestIntensity = apexIntensity;
            }
            
            double scale = 100.0 / highestIntensity;
                
            foreach(DataRow row in fragmentsList.Rows)
            {
                string fragmentName = (string)row["Fragment name"];
                yValCoords[fragmentName] = creatorGUI.lipidCreator.collisionEnergyHandler.getIntensityCurve(selectedInstrument, selectedClass, selectedAdduct, fragmentName, xValCoords, scale);
            }
            cartesean.Refresh();
            fragmentSelectionChanged();
        }
        
        
        
        
        
        
        
        public void fragmentSelectionChanged()
        {
            selectedFragments.Clear();
            selectedFragments.Add("productProfile");
            
            for (int i = 0; i < yValCoords["productProfile"].Length; ++i) yValCoords["productProfile"][i] = 1;
            
            //int ii = 0;
            foreach(DataRow row in fragmentsList.Rows)
            {
                if ((bool)row["View"])
                {
                    string fragmentName = (string)row["Fragment Name"];
                    selectedFragments.Add(fragmentName);
                    
                    yValCoords["productProfile"] = CollisionEnergy.productTwoDistributions(yValCoords["productProfile"], yValCoords[fragmentName]);
                }
            }
            
            double argMaxY = 0;
            double maxY = 0;
            for (int i = 0; i < yValCoords["productProfile"].Length; ++i)
            {
                yValCoords["productProfile"][i] *= 4000;
                if (maxY < yValCoords["productProfile"][i])
                {
                    argMaxY = xValCoords[i];
                    maxY = yValCoords["productProfile"][i];
                }
            }
            
            fragmentApex["productProfile"] = argMaxY;
            cartesean.Refresh();
        }
        
        
        
        
        public void textBoxCurrentCE_ValueChanged(Object sender, EventArgs e)
        {
            checkValidTextBoxtCurrentCE();
        }
        
        
        
        public void checkValidTextBoxtCurrentCE()
        {
            double oldCE = cartesean.CEval;
            try
            {
                cartesean.CEval = Convert.ToDouble(textBoxCurrentCE.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception ee)
            {
                
            }
            if (cartesean.CEval < cartesean.minXVal || cartesean.maxXVal < cartesean.CEval)
            {
                cartesean.CEval = oldCE;
                textBoxCurrentCE.Text = String.Format(new CultureInfo("en-US"), "{0:0.00}",cartesean.CEval);
            }
            collisionEnergies[selectedInstrument][selectedClass][selectedAdduct] = cartesean.CEval;
            cartesean.Refresh();
        }
        
        
        public void textBoxCurrentCE_Keydown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) checkValidTextBoxtCurrentCE();
        }
        
        
        
        
        public void instrumentComboboxChanged(Object sender, EventArgs e)
        {
            selectedInstrument = (string)indexToInstrument[instrumentCombobox.SelectedIndex];
            classCombobox.Items.Clear();
            cartesean.updateXBoundaries((double)creatorGUI.lipidCreator.msInstruments[selectedInstrument][1], (double)creatorGUI.lipidCreator.msInstruments[selectedInstrument][2]);
            foreach(string lipidClass in collisionEnergies[selectedInstrument].Keys)
            {
                classCombobox.Items.Add(lipidClass);
            }
            classCombobox.SelectedIndex = 0;
        }
        
        
        
        
        
        public void classComboboxChanged(Object sender, EventArgs e)
        {
            selectedClass = (string)classCombobox.Items[classCombobox.SelectedIndex];
            adductCombobox.Items.Clear();
            foreach(string adduct in collisionEnergies[selectedInstrument][selectedClass].Keys)
            {
                adductCombobox.Items.Add(adduct);
            }
            adductCombobox.SelectedIndex = 0;
        }
        
        
        
        
        
        public void adductComboboxChanged(Object sender, EventArgs e)
        {
            selectedAdduct = (string)adductCombobox.Items[adductCombobox.SelectedIndex];
            fragmentsList.Rows.Clear();
            
            
            foreach(string fragmentName in creatorGUI.lipidCreator.collisionEnergyHandler.instrumentParameters[selectedInstrument][selectedClass][selectedAdduct].Keys.OrderBy(fragmentName => Convert.ToInt32(creatorGUI.lipidCreator.collisionEnergyHandler.instrumentParameters[selectedInstrument][selectedClass][selectedAdduct][fragmentName]["rank"])))
            {
                DataRow row = fragmentsList.NewRow();
                row["View"] = true;
                row["Fragment name"] = fragmentName;
                fragmentsList.Rows.Add(row);
            }
            
            
            cartesean.CEval = collisionEnergies[selectedInstrument][selectedClass][selectedAdduct];
            textBoxCurrentCE.Text = String.Format(new CultureInfo("en-US"), "{0:0.00}",cartesean.CEval);
         
            fragmentsGridView.Update();
            fragmentsGridView.Refresh();
            computeCurves();
        }
        
        
        
        
        
        
        public void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        
        
        
        public void applyClick(object sender, EventArgs e)
        {
            foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, double>>> kvp1 in collisionEnergies)
            {
                // foreach class
                foreach(KeyValuePair<string, Dictionary<string, double>> kvp2 in kvp1.Value)
                {
                    // foreach adduct
                    foreach(KeyValuePair<string, double> kvp3 in kvp2.Value)
                    {
                        string stringCE = String.Format(new CultureInfo("en-US"), "{0:0.00}", kvp3.Value);
                        
                        // foreach fragment
                        foreach(KeyValuePair<string, Dictionary<string, string>> kvp4 in creatorGUI.lipidCreator.collisionEnergyHandler.instrumentParameters[kvp1.Key][kvp2.Key][kvp3.Key])
                        {
                            kvp4.Value["CE"] = stringCE;
                        }
                    }
                }
            }
        
            this.Close();
        }
        
        
        
        
        public void cartesean_mouseUp(object sender, MouseEventArgs e)
        {
            if (cartesean.CELineShift)
            {
                cartesean.CELineShift = false;
                cartesean.smooth = true;
                cartesean.Refresh();
            }
        }
        
        
        public void cartesean_mouseDown(object sender, MouseEventArgs e)
        {
            cartesean.CELineShift = cartesean.mouseOverCELine(e);
            if (cartesean.CELineShift) cartesean.smooth = false;
        }
        
        
        public void cartesean_mouseMove(object sender, MouseEventArgs e)
        {
            if (cartesean.mouseOverCELine(e))
            {
                cartesean.Cursor = Cursors.Hand;
            }
            else
            {
                cartesean.Cursor = Cursors.Default;
            }
            
            
            
            if (cartesean.CELineShift)
            {
                PointF vals = cartesean.pxToValue(e.X, e.Y);
                
                string highlightName = "";
                foreach (KeyValuePair<string, double> kvp in fragmentApex.Where(x => selectedFragments.Contains(x.Key)).OrderBy(x => Math.Abs(e.X - x.Value)))
                {
                    if (kvp.Value - Cartesean.CE_GRAB_MARGIN <= vals.X && vals.X <= kvp.Value + Cartesean.CE_GRAB_MARGIN)
                    {
                        vals.X = (float)kvp.Value;
                        highlightName = kvp.Key;
                        break;
                    }
                }
                if (cartesean.highlightName != highlightName) cartesean.highlightName = highlightName;
                cartesean.CEval = vals.X;
                textBoxCurrentCE.Text = String.Format(new CultureInfo("en-US"), "{0:0.00}",cartesean.CEval);
                collisionEnergies[selectedInstrument][selectedClass][selectedAdduct] = cartesean.CEval;
                cartesean.Refresh();
            }
        
            else if (cartesean.marginLeft <= e.X && e.X <= cartesean.Width - cartesean.marginRight)
            {
                cartesean.Focus();
                PointF vals = cartesean.pxToValue(e.X, e.Y);
                int pos = Math.Max(0, e.X - cartesean.marginLeft);
                pos = Math.Min(pos, cartesean.innerWidthPx);
                string highlightName = "";
                
                double offset = (cartesean.pxToValue(0, 100)).Y - (cartesean.pxToValue(0, 100 + Cartesean.offsetPX)).Y;
                
                foreach(DataRow row in fragmentsList.Rows)
                {
                    if ((bool)row["View"])
                    {
                        string fragmentName = (string)row["Fragment name"];
                        if (vals.Y - offset <= yValCoords[fragmentName][pos] && yValCoords[fragmentName][pos] <= vals.Y + offset)
                        {
                            highlightName = fragmentName;
                            break;
                        }
                    }
                }
                if (cartesean.highlightName != highlightName)
                {
                    cartesean.highlightName = highlightName;
                    cartesean.Refresh();
                }
                if (highlightName.Length > 0)
                {
                    ToolTip1.SetToolTip(cartesean, highlightName + ", opt CE at " + String.Format(new CultureInfo("en-US"), "{0:0.00}", fragmentApex[highlightName]));
                }
                else
                {
                    ToolTip1.SetToolTip(cartesean, null);
                    ToolTip1.Hide(cartesean);
                }
            }
        }
        
        
        private void fragmentsGridView_CellContentClick(object sender, EventArgs e)
        {
            fragmentsGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        
        
        
        private void fragmentsGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            fragmentSelectionChanged();
        }
        
        public double sq(double x)
        {
            return x * x;
        }
        
        public double muToE(double mu, double sigma)
        {
            return Math.Exp(mu + sq(sigma) / 2.0);
        }
        
        public double sigmaToS(double mu, double sigma)
        {
            return Math.Exp(mu + sq(sigma) / 2.0) * Math.Sqrt(Math.Exp(sq(sigma)) - 1.0);
        }
        
        
        private void fragmentsGridView_MouseMove(object sender, MouseEventArgs e)
        {
            int rowIndexFromMouseDown = fragmentsGridView.HitTest(e.X, e.Y).RowIndex;
            string highlightName = "";
            if (rowIndexFromMouseDown != -1)
            {
                highlightName = (string)fragmentsList.Rows[rowIndexFromMouseDown]["Fragment name"];
            }
            
            if (cartesean.highlightName != highlightName)
            {
                cartesean.highlightName = highlightName;
                cartesean.Refresh();
            }
        }
        
    }
}
