/*
MIT License

Copyright (c) 2018 Dominik Kopczynski   -   dominik.kopczynski {at} isas.de
                   Bing Peng   -   bing.peng {at} isas.de
                   Nils Hoffmann  -  nils.hoffmann {at} isas.de

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
        public CreatorGUI creatorGUI = null;
        public double[] xValCoords = null;
        public IDictionary<string, double[]> yValCoords = null;
        public IDictionary<string, double> fragmentApex = null;
        public IDictionary<string, IDictionary<string, IDictionary<string, double>>> collisionEnergies = null;  // for instrument / class / adduct
        public IDictionary<string, IDictionary<string, IDictionary<string, IDictionary<string, bool>>>> fragmentSelections = null;  // for instrument / class / adduct / fragment
        public string selectedInstrument = "";
        public string selectedClass = "";
        public string selectedAdduct = "";
        public DataTable fragmentsList = null;
        public bool initialCall = true;
        public HashSet<string> selectedFragments = null;
        public IDictionary<string, string> indexToInstrument = null;
        public PRMTypes PRMMode;
        
        public CEInspector(CreatorGUI _creatorGUI, string _currentInstrument)
        {
            creatorGUI = _creatorGUI;
            selectedInstrument = _currentInstrument;
            collisionEnergies = new Dictionary<string, IDictionary<string, IDictionary<string, double>>>();
            fragmentSelections = new Dictionary<string, IDictionary<string, IDictionary<string, IDictionary<string, bool>>>>();
            fragmentApex = new Dictionary<string, double>();
            selectedFragments = new HashSet<string>();
            indexToInstrument = new Dictionary<string, string>();
            PRMMode = creatorGUI.lipidCreator.PRMMode;
            
            fragmentsList = new DataTable("fragmentsList");
            fragmentsList.Columns.Add(new DataColumn("View"));
            fragmentsList.Columns[0].DataType = typeof(bool);
            fragmentsList.Columns.Add(new DataColumn("Fragment name"));
            fragmentsList.Columns[1].DataType = typeof(string);
            
            
            
            InitializeComponent();
            numericalUpDownCurrentCE.Value = (decimal)cartesean.CEval;
            
            // foreach instrument
            foreach(KeyValuePair<string, IDictionary<string, IDictionary<string, IDictionary<string, IDictionary<string, string>>>>> kvp1 in creatorGUI.lipidCreator.collisionEnergyHandler.instrumentParameters)
            {
                IDictionary<string, IDictionary<string, double>> ce1 = new SortedList<string, IDictionary<string, double>>();
                collisionEnergies.Add(kvp1.Key, ce1);
                IDictionary<string, IDictionary<string, IDictionary<string, bool>>> fs1 = new Dictionary<string, IDictionary<string, IDictionary<string, bool>>>();
                fragmentSelections.Add(kvp1.Key, fs1);
                
                // foreach class
                foreach(KeyValuePair<string, IDictionary<string, IDictionary<string, IDictionary<string, string>>>> kvp2 in kvp1.Value)
                {
                    Dictionary<string, double> ce2 = new Dictionary<string, double>();
                    ce1.Add(kvp2.Key, ce2);
                    Dictionary<string, IDictionary<string, bool>> fs2 = new Dictionary<string, IDictionary<string, bool>>();
                    fs1.Add(kvp2.Key, fs2);
                    
                    // foreach adduct
                    foreach(KeyValuePair<string, IDictionary<string, IDictionary<string, string>>> kvp3 in kvp2.Value)
                    {
                    
                        Dictionary<string, bool> fs3 = new Dictionary<string, bool>();
                        fs2.Add(kvp3.Key, fs3);
                        
                        
                        ce2.Add(kvp3.Key, creatorGUI.lipidCreator.collisionEnergyHandler.collisionEnergies[kvp1.Key][kvp2.Key][kvp3.Key]);
                        
                        // foreach adduct
                        foreach(KeyValuePair<string, IDictionary<string, string>> kvp4 in kvp3.Value)
                        {
                            fs3.Add(kvp4.Key, kvp4.Value["selected"] == "1");
                        }
                    }
                }
            }
            
            

            indexToInstrument.Clear();
            indexToInstrument.Add(creatorGUI.lipidCreator.msInstruments[selectedInstrument].CVTerm, creatorGUI.lipidCreator.msInstruments[selectedInstrument].model+" ["+creatorGUI.lipidCreator.msInstruments[selectedInstrument].CVTerm+"]");
            instrumentCombobox.DataSource = new BindingSource(indexToInstrument, null);
            instrumentCombobox.DisplayMember = "Value";
            instrumentCombobox.ValueMember = "Key";
            instrumentCombobox.SelectedValue = selectedInstrument; // select by CVTerm string value, e.g.: MS:1002791
            if (PRMMode == PRMTypes.PRMAutomatically)
            {
                radioButtonPRMFragments.Checked = true;
            }
            else
            {
                radioButtonPRMArbitrary.Checked = true;
            }
            PRMModeChanged();
        }

        
        public void changeSmooth(object sender, System.Timers.ElapsedEventArgs e)
        {
            cartesean.smooth = true;
            refreshCartesan();
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
            if(this.cartesean.InvokeRequired) {
            } else {
                refreshCartesan();
            }
            fragmentSelectionChanged();
        }
        
        
        
        
        
        
        
        public void fragmentSelectionChanged()
        {
            selectedFragments.Clear();
            selectedFragments.Add("productProfile");
            
            for (int i = 0; i < yValCoords["productProfile"].Length; ++i) yValCoords["productProfile"][i] = 1;
            
            double maxYCurves = 0;
            //int ii = 0;
            foreach(DataRow row in fragmentsList.Rows)
            {
                string fragmentName = (string)row["Fragment Name"];
                if ((bool)row["View"])
                {
                    selectedFragments.Add(fragmentName);
                    
                    yValCoords["productProfile"] = CollisionEnergy.productTwoDistributions(yValCoords["productProfile"], yValCoords[fragmentName]);
                    maxYCurves = Math.Max(maxYCurves, yValCoords[fragmentName].Max());
                }
                
                fragmentSelections[selectedInstrument][selectedClass][selectedAdduct][fragmentName] = ((bool)row["View"]);
            }
            
            double argMaxY = 0;
            double maxYProductProfile = yValCoords["productProfile"].Max();
            double maxY = 0;
            for (int i = 0; i < yValCoords["productProfile"].Length; ++i)
            {
                yValCoords["productProfile"][i] /= maxYProductProfile;
                yValCoords["productProfile"][i] *= maxYCurves;
                if (maxY < yValCoords["productProfile"][i])
                {
                    argMaxY = xValCoords[i];
                    maxY = yValCoords["productProfile"][i];
                }
            }
            
            if (collisionEnergies[selectedInstrument][selectedClass][selectedAdduct] < 0)
            {
                collisionEnergies[selectedInstrument][selectedClass][selectedAdduct] = argMaxY;
            }
            
            fragmentApex["productProfile"] = argMaxY;
            if (PRMMode == PRMTypes.PRMAutomatically)
            {
                cartesean.CEval = argMaxY;
                numericalUpDownCurrentCE.Value = (decimal)Math.Max(cartesean.minCEVal, cartesean.CEval);
                collisionEnergies[selectedInstrument][selectedClass][selectedAdduct] = cartesean.CEval;
                refreshCartesan();
            }
            refreshCartesan();
        }
        
        
        
        
        public void textBoxCurrentCE_ValueChanged(Object sender, EventArgs e)
        {
            checkValidTextBoxtCurrentCE();
        }
        
        
        
        public void checkValidTextBoxtCurrentCE()
        {
            if (selectedInstrument == "" || selectedClass == "" || selectedAdduct == "") return;
            
            double oldCE = cartesean.CEval;
            try
            {
                cartesean.CEval = (double)numericalUpDownCurrentCE.Value;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
            if (cartesean.CEval < cartesean.minXVal || cartesean.maxXVal < cartesean.CEval)
            {
                cartesean.CEval = oldCE;
                numericalUpDownCurrentCE.Value = (decimal)cartesean.CEval;
            }
            collisionEnergies[selectedInstrument][selectedClass][selectedAdduct] = cartesean.CEval;
            refreshCartesan();
        }
        
        
        public void textBoxCurrentCE_Keydown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) checkValidTextBoxtCurrentCE();
        }
        
        /**
         * Refreshes the cartesean grid of the curve plot control.
         * 
         * Refresh should check whether it needs to be performed in the thread that created the form element.
         * See https://docs.microsoft.com/de-de/dotnet/framework/winforms/controls/how-to-make-thread-safe-calls-to-windows-forms-controls for reference.
         */
        private void refreshCartesan() {
            if(this.cartesean.InvokeRequired) {
                this.cartesean.Invoke(new Action(() => {this.cartesean.Refresh();}));
            } else {
                this.cartesean.Refresh();
            }
        }
        
        
        public void instrumentComboboxChanged(Object sender, EventArgs e)
        {
            if(instrumentCombobox.SelectedItem!=null) {
                string key = (string)((System.Collections.Generic.KeyValuePair<string,string>)instrumentCombobox.SelectedItem).Key;
                //KeyValuePair<string,string> kvp = indexToInstrument[key];
                selectedInstrument = key;//(string)indexToInstrument[instrumentCombobox.SelectedValue];
                if(classCombobox.SelectedIndex >-1) {
                    selectedClass = (string)classCombobox.Items[classCombobox.SelectedIndex];
                }
                classCombobox.Items.Clear();
                double minVal = (double)creatorGUI.lipidCreator.msInstruments[selectedInstrument].minCE;
                double maxVal = (double)creatorGUI.lipidCreator.msInstruments[selectedInstrument].maxCE;
                numericalUpDownCurrentCE.Minimum = (decimal)minVal;
                numericalUpDownCurrentCE.Maximum = (decimal)maxVal;
                cartesean.xAxisLabel = (string)creatorGUI.lipidCreator.msInstruments[selectedInstrument].xAxisLabel;
            
                cartesean.updateXBoundaries(0, maxVal, minVal, maxVal);
                foreach(string lipidClass in collisionEnergies[selectedInstrument].Keys)
                {
                    classCombobox.Items.Add(lipidClass);
                }
                //
                if(selectedClass != null) {
                    classCombobox.SelectedIndex = classCombobox.FindString(selectedClass);
                } else {
                    classCombobox.SelectedIndex = 0;
                }
            }
        }
        
        
        
        public void PRMModeChanged()
        {
            if (PRMMode == PRMTypes.PRMAutomatically)
            {
                numericalUpDownCurrentCE.Enabled = false;
                fragmentSelectionChanged();
            }
            else
            {
                numericalUpDownCurrentCE.Enabled = true;
            }
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
            
            
            foreach(KeyValuePair<string, bool> fragmentPar in fragmentSelections[selectedInstrument][selectedClass][selectedAdduct])
            {
                DataRow row = fragmentsList.NewRow();
                row["View"] = fragmentPar.Value;
                row["Fragment name"] = fragmentPar.Key;
                fragmentsList.Rows.Add(row);
            }
            
            
         
            fragmentsGridView.Update();
            refreshFragmentsGridView();
            computeCurves();
            
            cartesean.CEval = collisionEnergies[selectedInstrument][selectedClass][selectedAdduct];
            numericalUpDownCurrentCE.Value = (decimal)cartesean.CEval;
        }
        
        /**
         * Refreshes the fragment grid view of the curve plot control.
         * 
         * Refresh should check whether it needs to be performed in the thread that created the form element.
         * See https://docs.microsoft.com/de-de/dotnet/framework/winforms/controls/how-to-make-thread-safe-calls-to-windows-forms-controls for reference.
         */
        private void refreshFragmentsGridView()
        {
            if(this.fragmentsGridView.InvokeRequired) {
                this.fragmentsGridView.Invoke(new Action(() => {this.fragmentsGridView.Refresh();}));
            } else {
                this.fragmentsGridView.Refresh();
            }
        }
        
        
        
        public void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        
        
        
        public void applyClick(object sender, EventArgs e)
        {
            foreach(KeyValuePair<string, IDictionary<string, IDictionary<string, double>>> kvp1 in collisionEnergies)
            {
                // foreach class
                foreach(KeyValuePair<string, IDictionary<string, double>> kvp2 in kvp1.Value)
                {
                    // foreach adduct
                    foreach(KeyValuePair<string, double> kvp3 in kvp2.Value)
                    {
                        creatorGUI.lipidCreator.collisionEnergyHandler.collisionEnergies[kvp1.Key][kvp2.Key][kvp3.Key] = kvp3.Value;
                        
                        // foreach fragment
                        foreach(KeyValuePair<string, IDictionary<string, string>> kvp4 in creatorGUI.lipidCreator.collisionEnergyHandler.instrumentParameters[kvp1.Key][kvp2.Key][kvp3.Key])
                        {
                            kvp4.Value["selected"] = fragmentSelections[kvp1.Key][kvp2.Key][kvp3.Key][kvp4.Key] ? "1" : "0";
                        }
                    }
                }
            }
            creatorGUI.lipidCreator.PRMMode = PRMMode;
            this.Close();
        }
        
        
        
        public void PRMModeCheckedChanged(Object sender, EventArgs e)
        {
            PRMMode = radioButtonPRMFragments.Checked ? PRMTypes.PRMAutomatically : PRMMode = PRMTypes.PRMManually;
            PRMModeChanged();
        }
        
        
        
        
        public void cartesean_mouseUp(object sender, MouseEventArgs e)
        {
            if (cartesean.CELineShift)
            {
                cartesean.CELineShift = false;
                cartesean.smooth = true;
                refreshCartesan();
            }
        }
        
        
        public void cartesean_mouseDown(object sender, MouseEventArgs e)
        {
            if (PRMMode == PRMTypes.PRMManually)
            {
                cartesean.CELineShift = cartesean.mouseOverCELine(e);
                if (cartesean.CELineShift) cartesean.smooth = false;
            }
        }
        
        
        public void cartesean_mouseMove(object sender, MouseEventArgs e)
        {
            if (PRMMode == PRMTypes.PRMManually && cartesean.mouseOverCELine(e))
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
                
                if (vals.X < cartesean.minXVal) vals.X = (float)cartesean.minXVal;
                if (vals.X > cartesean.maxXVal) vals.X = (float)cartesean.maxXVal;

                cartesean.CEval = Math.Max(cartesean.minCEVal, vals.X);
                numericalUpDownCurrentCE.Value = (decimal)Math.Max(cartesean.minCEVal, cartesean.CEval);
                collisionEnergies[selectedInstrument][selectedClass][selectedAdduct] = cartesean.CEval;
                refreshCartesan();
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
                
                if (highlightName.Length == 0)
                {
                    string fragmentName = "productProfile";
                    if (vals.Y - offset <= yValCoords[fragmentName][pos] && yValCoords[fragmentName][pos] <= vals.Y + offset)
                    {
                        highlightName = fragmentName;
                    }
                }
                
                if (cartesean.highlightName != highlightName)
                {
                    cartesean.highlightName = highlightName;
                    refreshCartesan();
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
                refreshCartesan();
            }
        }
        
        
        
        
        void checkedListBoxSelectAll(object sender, EventArgs e)
        {
            foreach(DataRow row in fragmentsList.Rows)
            {
                row["View"] = true;
            }
            fragmentSelectionChanged();
        }
        
        
        
        
        void checkedListBoxDeselectAll(object sender, EventArgs e)
        {
            foreach(DataRow row in fragmentsList.Rows)
            {
                row["View"] = false;
            }
            fragmentSelectionChanged();
        }
        
    }
}
