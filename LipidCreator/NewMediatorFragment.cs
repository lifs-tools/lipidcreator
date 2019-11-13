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
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Linq;
using log4net;


namespace LipidCreator
{
    public partial class NewMediatorFragment : Form
    {
    
        private static readonly ILog log = LogManager.GetLogger(typeof(NewMediatorFragment));
        public MediatorMS2Form mediatorMS2Form = null;
        public string headgroup = "";
        public string fragmentName = "";
        public bool allowToAdd = false;
        public bool updating = false;
        public Dictionary<string, object[]> elementDict = null;
        
        public NewMediatorFragment(MediatorMS2Form _mediatorMS2Form, string _headgroup)
        {
            mediatorMS2Form = _mediatorMS2Form;
            headgroup = _headgroup;
        
            InitializeComponent();
            InitializeCustom();

            elementDict = AddHeavyPrecursor.createGridData(MS2Fragment.createEmptyElementDict());
            
            updating = true;
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Element";
            dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.Columns[1].Name = "Count (Monoisotopic)";
            dataGridView1.Columns[2].Name = "Count (Isotopic)";
            DataGridViewComboBoxColumn combo1 = new DataGridViewComboBoxColumn();
            dataGridView1.Columns.Add(combo1);
            dataGridView1.Columns[0].Width = (dataGridView1.Width - 2) / 4;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].Width = (dataGridView1.Width - 2) / 4;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].Width = (dataGridView1.Width - 2) / 4;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].Width = (dataGridView1.Width - 2) / 4;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.AllowUserToAddRows = false;
            
            foreach (Molecule m in MS2Fragment.ALL_ELEMENTS.Keys.Where(x => !MS2Fragment.ALL_ELEMENTS[x].isHeavy))
            {
                dataGridView1.Rows.Add(new object[] {"-", 0, 0, new DataGridViewComboBoxCell()});
            }
            
            foreach (Molecule molecule in MS2Fragment.ALL_ELEMENTS.Keys.Where(x => !MS2Fragment.ALL_ELEMENTS[x].isHeavy))
            {
                int l = MS2Fragment.ALL_ELEMENTS[molecule].position;
                dataGridView1.Rows[l].Cells[0].Value = MS2Fragment.ALL_ELEMENTS[molecule].shortcut;
                dataGridView1.Rows[l].Cells[1].Value = 0;
                dataGridView1.Rows[l].Cells[2].Value = 0;
                
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridView1.Rows[l].Cells[3];
                int j = 0;
                foreach (Molecule heavyMolecule in MS2Fragment.ALL_ELEMENTS[molecule].derivatives)
                {
                    if (j++ == 0) cell.Value = MS2Fragment.ALL_ELEMENTS[heavyMolecule].shortcutNumber;
                    cell.Items.Add(MS2Fragment.ALL_ELEMENTS[heavyMolecule].shortcutNumber);
                }
            }
            updating = false;
            
            comboBox1.Items.Add("Monoisotopic");
            comboBox1.SelectedIndex = 0;
            foreach(Precursor heavyPrecursor in mediatorMS2Form.creatorGUI.lipidCreator.headgroups[headgroup].heavyLabeledPrecursors)
            {
                comboBox1.Items.Add(heavyPrecursor.name);
            }
            makePreview();
        }

        // Add
        private void button1_Click(object sender, EventArgs e)
        {
            string lipidClass = (comboBox1.SelectedIndex == 0) ? headgroup : (string)comboBox1.Items[comboBox1.SelectedIndex];
            
            if (mediatorMS2Form.creatorGUI.lipidCreator.allFragments[lipidClass][false].ContainsKey(fragmentName))
            {
                MessageBox.Show("Negative fragment '" + fragmentName + "' already registered for lipid class '" + lipidClass + "'");
                return;
            }
            
            
            ElementDictionary newElements = LipidCreator.createElementData(elementDict);
            MS2Fragment newFragment = new MS2Fragment(fragmentName, fragmentName, Lipid.chargeToAdduct[-1], "", newElements, "HG");
            newFragment.userDefined = true;
            
            mediatorMS2Form.creatorGUI.lipidCreator.allFragments[lipidClass][false].Add(fragmentName, newFragment);
            
            if (comboBox1.SelectedIndex == 0)
            {
                mediatorMS2Form.checkedListBoxMonoIsotopicFragments.Items.Add(fragmentName);
                mediatorMS2Form.checkedListBoxMonoIsotopicFragments.SetItemChecked(mediatorMS2Form.checkedListBoxMonoIsotopicFragments.Items.Count - 1, false);
            }
            else if ((string)mediatorMS2Form.deuteratedMediatorHeadgroups.Items[mediatorMS2Form.deuteratedMediatorHeadgroups.SelectedIndex] == lipidClass)
            {
                mediatorMS2Form.checkedListBoxDeuteratedFragments.Items.Add(fragmentName);
                mediatorMS2Form.checkedListBoxDeuteratedFragments.SetItemChecked(mediatorMS2Form.checkedListBoxDeuteratedFragments.Items.Count - 1, false);
            }
            this.Close();
        }

        // Cancel
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        
        public void makePreview()
        {
            fragmentName = "m/z ";
            allowToAdd = true;
            
            if (tabControl1.SelectedIndex == 0)
            {
                try {
                    double fragmentMass = double.Parse(textBox1.Text, System.Globalization.CultureInfo.InvariantCulture);
                    if (fragmentMass > MS2Fragment.ALL_ELEMENTS[Molecule.H].mass)
                    {
                        fragmentName += String.Format(new CultureInfo("en-US"), "{0:0.0000}", fragmentMass);
                    }
                    else {
                        allowToAdd = false;
                    }
                }
                catch (Exception e)
                {
                    log.Error("Caught exception while parsing string values as fragment masses.", e);
                    allowToAdd = false;
                }
            }
            else {
                double fragmentMass = LipidCreator.computeMass(LipidCreator.createElementData(elementDict), -1);
                if (fragmentMass > MS2Fragment.ALL_ELEMENTS[Molecule.H].mass)
                {
                    fragmentName += String.Format(new CultureInfo("en-US"), "{0:0.0000}", fragmentMass);
                }
                else {
                    allowToAdd = false;
                }
            }
            allowToAdd &= !mediatorMS2Form.creatorGUI.lipidCreator.allFragments[headgroup][false].ContainsKey(fragmentName);
            label4.Text = fragmentName;
            label4.ForeColor = allowToAdd ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 0, 0);
            if (label4.Text.Length > Lipid.MEDIATOR_PREFIX_LENGTH) label4.Text += "-";
            label4.Text = "Result name: " + label4.Text;
            button1.Enabled = allowToAdd;
        }
        
        
        private void dataGridView1CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(updating || elementDict == null) return;
            updating = true;
            string key = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            string val;
            try 
            {
                val = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            catch(Exception ee)
            {
                log.Error("Conversion error while updating cell value, setting value to 0.", ee);
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0";
                val = "0";
            }
            
            
            
            if (e.ColumnIndex != 3)
            {
                int n;
                try {
                    n = Convert.ToInt32(val);
                }
                catch (Exception ee){
                    log.Error("Conversion error while updating cell value to int32: " + val, ee);
                    n = 0;
                }
                n = Math.Max(n, 0);
                elementDict[key][e.ColumnIndex - 1] = n;
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = n;
            }
            else
            {
                elementDict[key][e.ColumnIndex - 1] = val;
            }
            makePreview();
            updating = false;
        }
        
        
        
        public void textBox1TextChanged(Object sender, EventArgs e)
        {
            makePreview();
        }
        
        
        
        public void tabIndexChanged(Object sender, TabControlCancelEventArgs e)
        {
            makePreview();
        }
        
        
        
        public void comboBox1ValueChanged(Object sender, EventArgs e)
        {
            int isotopicIndex = ((ComboBox)sender).SelectedIndex;
            if (isotopicIndex > 0)
            {
                dataGridView1.Columns[2].DefaultCellStyle.BackColor = Color.Empty;
                dataGridView1.Columns[2].ReadOnly = false;
                dataGridView1.Columns[3].DefaultCellStyle.BackColor = Color.Empty;
                dataGridView1.Columns[3].ReadOnly = false;
            }
            else {
                dataGridView1.Columns[2].DefaultCellStyle.BackColor = Color.LightGray;
                dataGridView1.Columns[2].ReadOnly = true;
                dataGridView1.Columns[3].DefaultCellStyle.BackColor = Color.LightGray;
                dataGridView1.Columns[3].ReadOnly = true;
                
                for (int l = 0; l < dataGridView1.Rows.Count; ++l)
                {
                    dataGridView1.Rows[l].Cells[2].Value = 0;
                }
            }
            dataGridView1.Refresh();
        }
    }
}
