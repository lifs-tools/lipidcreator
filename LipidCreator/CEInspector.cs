using System;
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
        public Dictionary<string, double> norming;
        public Dictionary<string, double> fragmentApex;
        public Dictionary<string, Dictionary<string, Dictionary<string, double>>> collisionEnergies;  // for instrument / class / adduct
        public string selectedInstrument;
        public string selectedClass;
        public string selectedAdduct;
        public DataTable fragmentsList;
        public bool initialCall = true;
        public HashSet<string> selectedFragments;
        
        public CEInspector(CreatorGUI _creatorGUI)
        {
            creatorGUI = _creatorGUI;
            collisionEnergies = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
            fragmentApex = new Dictionary<string, double>();
            selectedFragments = new HashSet<string>();
            
            fragmentsList = new DataTable("fragmentsList");
            fragmentsList.Columns.Add(new DataColumn("View"));
            fragmentsList.Columns[0].DataType = typeof(bool);
            fragmentsList.Columns.Add(new DataColumn("Fragment name"));
            fragmentsList.Columns[1].DataType = typeof(string);
            
            
            
            InitializeComponent();
            textBoxCurrentCE.Text = String.Format("{0:0.00}",cartesean.CEval);
            
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
                        p2.Add(kvp3.Key, 0.0);
                    }
                }
            }
            
            
            foreach (string instrumentName in collisionEnergies.Keys)
            {
                instrumentCombobox.Items.Add(instrumentName);
                instrumentCombobox.SelectedIndex = 0;
            }
        }
        
        public void changeSmooth(object sender, System.Timers.ElapsedEventArgs e)
        {
            cartesean.smooth = true;
            cartesean.Refresh();
            timerSmooth.Enabled = false;
        }
        
        
        
        
        public Tuple<double, double, double> productLogNormal(double m1, double s1, double sft1, double m2, double s2, double sft2)
        {
            double s1sq = sq(s1);
            double s2sq = sq(s2);
            double s = Math.Sqrt(s1sq * s2sq / (s1sq + s2sq));
            double m = (s2sq * m1 + s1sq * m2) / (s1sq + s2sq) - sq(s);
            
            double x = (Math.Exp(m1 - s1sq) - sft1 + Math.Exp(m2 - s2sq) - sft2) / 2.0;
            
            // actually this is a newton method x^+ = x - f(x) / f'(x)
            // with f(x) = d/dx log(L(x | m1, s1, sft1) * L(x | m2, s2, sft2))
            // where L is shifted lognormal pdf and
            // the three parameters m, s, sft (shift) for each
            for (int ii = 0; ii < 20; ++ii)
            {
                double numerator = (-s1sq * (sft1 + x) * Math.Log(sft2 + x) - s2sq * (sft2 + x) * Math.Log(sft1 + x) + sft1 * m2 * s1sq - sft1 * s1sq * s2sq + sft2 * m1 * s2sq - sft2 * s1sq * s2sq + m1 * s2sq * x + m2 * s1sq * x - 2 * s1sq * s2sq * x)/(s1sq * s2sq * (sft1 + x) * (sft2 + x));
            
                double denominator = -((sq(sft1) * s1sq + sq(sft1) * m2 * s1sq + sq(sft2) * s2sq + sq(sft2) * m1 * s2sq - sq(sft1) * s1sq * s2sq - sq(sft2) * s1sq * s2sq + 2 * sft1 * s1sq * x + 2 * sft1 * m2 * s1sq * x + 2 * sft2 * s2sq * x + 2 * sft2 * m1 * s2sq * x - 2 * sft1 * s1sq * s2sq * x - 2 * sft2 * s1sq * s2sq * x + s1sq * sq(x) + m2 * s1sq * sq(x) + s2sq * sq(x) + m1 * s2sq * sq(x) - 2 * s1sq * s2sq * sq(x) - s2sq * sq(sft2 + x) * Math.Log(sft1 + x) - s1sq *sq (sft1 + x) * Math.Log(sft2 + x))/(s1sq * s2sq * sq(sft1 + x) * sq(sft2 + x)));
                x -= numerator / denominator;
            }
            
            // x stores now the apex (mode) of the product distribution
            // to get the shift, it has to be subtracted from the unshifted mode
            double sft = Math.Exp(m - sq(s)) - x;
            
            return new Tuple<double, double, double>(m, s, sft); 
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
            
            norming = new Dictionary<string, double>();
            norming["productProfile"] = 0;
            
            
            // compute x values (a.k.a. collision energies)
            for (int i = 0; i <= cartesean.innerWidthPx; ++i)
            {
                xValCoords[i] = cartesean.minXVal + ((double)i) / cartesean.innerWidthPx * (cartesean.maxXVal - cartesean.minXVal);
            }
            
            
            // precompute y values for all fragment model curves
            int k = 0;
            foreach(DataRow row in fragmentsList.Rows)
            {
                string fragmentName = (string)row["Fragment name"];
                           
                yValCoords[fragmentName] = new double[cartesean.innerWidthPx + 1];
                norming[fragmentName] = 0;
                int j = 0;
                
                fragmentApex[fragmentName] = creatorGUI.lipidCreator.collisionEnergyHandler.getCollisionEnergy(selectedInstrument, selectedClass, selectedAdduct, fragmentName);
                
                foreach (double valX in xValCoords)
                {
                    double intens = 10000 * creatorGUI.lipidCreator.collisionEnergyHandler.getIntensity(selectedInstrument, selectedClass, selectedAdduct, fragmentName, valX);
                    yValCoords[fragmentName][j] = intens;
                    norming[fragmentName] += intens;
                    ++j;
                }
                ++k;
            }
            cartesean.Refresh();
            fragmentSelectionChanged();
        }
        
        
        
        
        
        
        
        public void fragmentSelectionChanged()
        {
            norming["productProfile"] = 0;
            selectedFragments.Clear();
            
            for(int i = 0; i < yValCoords["productProfile"].Length; ++i) yValCoords["productProfile"][i] = 0;
            
            double m = -1, sd = -1, sft = 0;
            foreach(DataRow row in fragmentsList.Rows)
            {
                if ((bool)row["View"])
                {
                    string fragmentName = (string)row["Fragment Name"];
                    selectedFragments.Add(fragmentName);
                    Dictionary<string, string> pars =  creatorGUI.lipidCreator.collisionEnergyHandler.instrumentParameters[selectedInstrument][selectedClass][selectedAdduct][fragmentName];
                    if (sd < 0)
                    {
                        m = Convert.ToDouble(pars["meanlog"], CultureInfo.InvariantCulture);
                        sd = Convert.ToDouble(pars["sdlog"], CultureInfo.InvariantCulture);
                        sft = Convert.ToDouble(pars["shift"], CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Tuple<double, double, double> t = productLogNormal(m,
                        sd,
                        sft,
                        Convert.ToDouble(pars["meanlog"], CultureInfo.InvariantCulture),
                        Convert.ToDouble(pars["sdlog"], CultureInfo.InvariantCulture),
                        Convert.ToDouble(pars["shift"], CultureInfo.InvariantCulture));
                        
                        m = t.Item1;
                        sd = t.Item2;
                        sft = t.Item3;
                    }
                }
            }
            
            
            for (int i = 0; i < xValCoords.Length; ++i)
            {
                double collisionEnergy = xValCoords[i] + sft;
                yValCoords["productProfile"][i] = 1000 / (collisionEnergy * sd * Math.Sqrt(2 * Math.PI)) * Math.Exp(-sq(Math.Log(collisionEnergy) - m) / (2 * sq(sd)));
            }
            
            /*
            foreach(DataRow row in fragmentsList.Rows)
            {
                
                if ((bool)row["View"])
                {
                    string fragmentName = (string)row["Fragment Name"];
                    
                    for (int j = 0; j < yValCoords[fragmentName].Length; ++j)
                    {
                        yValCoords["productProfile"][j] += Math.Log10(yValCoords[fragmentName][j] / norming[fragmentName]);
                    }
                }
            }
            
            foreach(double intens in yValCoords["productProfile"]) norming["productProfile"] += Math.Pow(10, intens);
            if (norming["productProfile"] > 0)
            {
                for(int i = 0; i < yValCoords["productProfile"].Length; ++i)
                {
                    yValCoords["productProfile"][i] = Math.Pow(10, yValCoords["productProfile"][i]) * 10000.0 / norming["productProfile"];
                }
            }
            */
            
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
                cartesean.CEval = Convert.ToDouble(textBoxCurrentCE.Text);
            }
            catch (Exception ee)
            {
                
            }
            if (cartesean.CEval < cartesean.minXVal || cartesean.maxXVal < cartesean.CEval)
            {
                cartesean.CEval = oldCE;
                textBoxCurrentCE.Text = String.Format("{0:0.00}",cartesean.CEval);
            }
            cartesean.Refresh();
        }
        
        
        public void textBoxCurrentCE_Keydown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) checkValidTextBoxtCurrentCE();
        }
        
        
        public void instrumentComboboxChanged(Object sender, EventArgs e)
        {
            selectedInstrument = (string)instrumentCombobox.Items[instrumentCombobox.SelectedIndex];
            classCombobox.Items.Clear();
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
                textBoxCurrentCE.Text = String.Format("{0:0.00}",cartesean.CEval);
                cartesean.Refresh();
            }
        
            else if (cartesean.marginLeft <= e.X && e.X <= cartesean.Width - cartesean.marginRight)
            {
                cartesean.Focus();
                PointF vals = cartesean.pxToValue(e.X, e.Y);
                int pos = Math.Max(0, e.X - cartesean.marginLeft);
                pos = Math.Min(pos, cartesean.innerWidthPx);
                string highlightName = "";
            
                foreach(DataRow row in fragmentsList.Rows)
                {
                    if ((bool)row["View"])
                    {
                        string fragmentName = (string)row["Fragment name"];
                        if (vals.Y - cartesean.offset <= yValCoords[fragmentName][pos] && yValCoords[fragmentName][pos] <= vals.Y + cartesean.offset)
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
                    ToolTip1.SetToolTip(cartesean, highlightName);
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
        
        
        
        public PointF productDistribution(double mu1, double sigma1, double mu2, double sigma2)
        {
            double sigma1sq = sq(sigma1);
            double sigma2sq = sq(sigma2);
            PointF p = new PointF();
            p.X = (float)((sigma2sq * mu1 + sigma1sq * mu2) / (sigma1sq + sigma2sq));
            p.Y = (float)Math.Sqrt(sigma1sq * sigma2sq / (sigma1sq + sigma2sq));
            return p;
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
