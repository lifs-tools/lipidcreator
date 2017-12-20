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
using System.Windows.Forms;

namespace LipidCreator
{
    [Serializable]
    public partial class NewFragment : Form
    {

        Dictionary<string, object[]> elements;
        LipidMS2Form ms2form;
        string[] buildingBlocks;
        bool chemAdding = true;
        bool updating = false;

        public NewFragment(LipidMS2Form ms2form)
        {
            this.ms2form = ms2form;
            elements = createGridData(MS2Fragment.createEmptyElementDict());
            InitializeComponent();
            
            updating = true;
            dataGridViewElements.ColumnCount = 3;
            dataGridViewElements.Columns[0].Name = "Element";
            dataGridViewElements.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewElements.Columns[1].Name = "Count";
            dataGridViewElements.Columns[2].Name = "Isotope count";
            DataGridViewComboBoxColumn combo1 = new DataGridViewComboBoxColumn();
            dataGridViewElements.Columns.Add(combo1);
            dataGridViewElements.Columns[0].Width = (dataGridViewElements.Width - 2) / 4;
            dataGridViewElements.Columns[0].ReadOnly = true;
            dataGridViewElements.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewElements.Columns[1].Width = (dataGridViewElements.Width - 2) / 4;
            dataGridViewElements.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewElements.Columns[2].Width = (dataGridViewElements.Width - 2) / 4;
            dataGridViewElements.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewElements.Columns[3].Width = (dataGridViewElements.Width - 2) / 4;
            dataGridViewElements.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewElements.AllowUserToAddRows = false;
            combo1.Name = "Isotope type";
            
            
            for (int k = 0; k < MS2Fragment.HEAVY_DERIVATIVE.Count; ++k) dataGridViewElements.Rows.Add(new object[] {"-", 0, 0, new DataGridViewComboBoxCell()});
            foreach (KeyValuePair<int, ArrayList> row in MS2Fragment.HEAVY_DERIVATIVE)
            {
                int l = MS2Fragment.MONOISOTOPE_POSITIONS[row.Key];
                dataGridViewElements.Rows[l].Cells[0].Value = MS2Fragment.ELEMENT_SHORTCUTS[row.Key];
                dataGridViewElements.Rows[l].Cells[1].Value = 0;
                dataGridViewElements.Rows[l].Cells[2].Value = 0;
                
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridViewElements.Rows[l].Cells[3];
                int j = 0;
                foreach (int element in row.Value)
                {
                    if (j++ == 0) cell.Value = MS2Fragment.HEAVY_SHORTCUTS[element];
                    cell.Items.Add(MS2Fragment.HEAVY_SHORTCUTS[element]);
                }
            }
            updating = false;
            
            // base types:
            // 0 -> fixed, FA1, FA2, FA3, FA4, FA1 + FA2, FA1 + FA3, FA1 + FA4, FA2 + FA3, FA2 + FA4, FA3 + FA4, FA1 + FA2 + FA3, FA1 + FA2 + FA4, FA1 + FA3 + FA4, HG
            // 1 -> fixed, FA1, FA2, FA3, FA1 + FA2, FA1 + FA3, FA2 + FA3, HG
            // 2 -> fixed, FA1, FA2, HG
            // 3 -> fixed, FA, HG
            // 4 -> fixed, LCB, FA, HG, LCB + FA, LCB + HG, FA + HG, HG
            // 5 -> fixed, LCB, HG, HG
            // 6 -> fixed, FA1, FA2, HG, FA1 + FA2, FA1 + HG, FA2 + HG, HG
            // 7 -> fixed, FA1, HG, HG
            // 8 -> fixed, HG
            String lipidClass = ((TabPage)ms2form.tabPages[ms2form.tabControlFragments.SelectedIndex]).Text;
            int bbType = ms2form.creatorGUI.lipidCreator.headgroups[lipidClass].buildingBlockType;
            
            selectBaseCombobox.Items.Add("fixed");
            ArrayList buildingBlocksArray = new ArrayList();
            buildingBlocksArray.Add("");
            switch(bbType)
            {
                case 0:
                    createCombinations(new String[]{"FA1", "FA2", "FA3", "FA4", "HG"}, -1, "", selectBaseCombobox, buildingBlocksArray);
                    /*
                    buildingBlocks = new string[]{"", "FA1", "FA2", "FA3", "FA4", "FA1;FA2", "FA1;FA3", "FA1;FA4", "FA2;FA3", "FA2;FA4", "FA3;FA4", "FA1;FA2;FA3", "FA1;FA2;FA4", "FA1;FA3;FA4", "FA2;FA3;FA4", "HG"};
                    selectBaseCombobox.Items.Add("FA1");
                    selectBaseCombobox.Items.Add("FA2");
                    selectBaseCombobox.Items.Add("FA3");
                    selectBaseCombobox.Items.Add("FA4");
                    selectBaseCombobox.Items.Add("FA1 + FA2");
                    selectBaseCombobox.Items.Add("FA1 + FA3");
                    selectBaseCombobox.Items.Add("FA1 + FA4");
                    selectBaseCombobox.Items.Add("FA2 + FA3");
                    selectBaseCombobox.Items.Add("FA2 + FA4");
                    selectBaseCombobox.Items.Add("FA3 + FA4");
                    selectBaseCombobox.Items.Add("FA1 + FA2 + FA3");
                    selectBaseCombobox.Items.Add("FA1 + FA2 + FA4");
                    selectBaseCombobox.Items.Add("FA1 + FA3 + FA4");
                    selectBaseCombobox.Items.Add("FA2 + FA3 + FA4");
                    */
                    break;
                    
                case 1:
                    createCombinations(new String[]{"FA1", "FA2", "FA3", "HG"}, -1, "", selectBaseCombobox, buildingBlocksArray);
                    /*
                    buildingBlocks = new string[]{"", "FA1", "FA2", "FA3", "FA1;FA2", "FA1;FA3", "FA2;FA3", "HG"};
                    selectBaseCombobox.Items.Add("FA1");
                    selectBaseCombobox.Items.Add("FA2");
                    selectBaseCombobox.Items.Add("FA3");
                    selectBaseCombobox.Items.Add("FA1 + FA2");
                    selectBaseCombobox.Items.Add("FA1 + FA3");
                    selectBaseCombobox.Items.Add("FA2 + FA3");
                    */
                    break;
                    
                case 2:
                    createCombinations(new String[]{"FA1", "FA2", "HG"}, -1, "", selectBaseCombobox, buildingBlocksArray);
                    /*
                    buildingBlocks = new string[]{"", "FA1", "FA2", "HG"};
                    selectBaseCombobox.Items.Add("FA1");
                    selectBaseCombobox.Items.Add("FA2");
                    */
                    break;
                    
                case 3:
                    createCombinations(new String[]{"FA", "HG"}, -1, "", selectBaseCombobox, buildingBlocksArray);
                    /*
                    buildingBlocks = new string[]{"", "FA", "HG"};
                    selectBaseCombobox.Items.Add("FA");
                    */
                    break;
                    
                case 4:
                    createCombinations(new String[]{"LCB", "FA", "HG"}, -1, "", selectBaseCombobox, buildingBlocksArray);
                    /*
                    buildingBlocks = new string[]{"", "LCB", "FA", "HG", "LCB;FA", "LCB;HG", "FA;HG", "HG"};
                    selectBaseCombobox.Items.Add("LCB");
                    selectBaseCombobox.Items.Add("FA");
                    selectBaseCombobox.Items.Add("HG");
                    selectBaseCombobox.Items.Add("LCB + FA");
                    selectBaseCombobox.Items.Add("LCB + HG");
                    selectBaseCombobox.Items.Add("FA + HG");
                    */
                    break;
                    
                case 5:
                    createCombinations(new String[]{"LCB", "HG"}, -1, "", selectBaseCombobox, buildingBlocksArray);
                    /*
                    buildingBlocks = new string[]{"", "LCB", "HG", "HG"};
                    selectBaseCombobox.Items.Add("LCB");
                    selectBaseCombobox.Items.Add("HG");
                    */
                    break;
                    
                case 6:
                    createCombinations(new String[]{"FA1", "FA2", "HG"}, -1, "", selectBaseCombobox, buildingBlocksArray);
                    /*
                    buildingBlocks = new string[]{"", "FA1", "FA2", "HG", "FA1;FA2", "FA1;HG", "FA2;HG", "HG"};
                    selectBaseCombobox.Items.Add("FA1");
                    selectBaseCombobox.Items.Add("FA2");
                    selectBaseCombobox.Items.Add("HG");
                    selectBaseCombobox.Items.Add("FA1 + FA2");
                    selectBaseCombobox.Items.Add("FA1 + HG");
                    selectBaseCombobox.Items.Add("FA2 + HG");
                    */
                    break;
                    
                case 7:
                    createCombinations(new String[]{"FA", "HG"}, -1, "", selectBaseCombobox, buildingBlocksArray);
                    /*
                    buildingBlocks = new string[]{"", "FA", "HG", "HG"};
                    selectBaseCombobox.Items.Add("FA");
                    selectBaseCombobox.Items.Add("HG");
                    */
                    break;
                    
                case 8:
                    createCombinations(new String[]{"HG"}, -1, "", selectBaseCombobox, buildingBlocksArray);
                    /*
                    buildingBlocks = new string[]{"", "HG"};
                    */
                    break;
            }
            buildingBlocks = (string[])buildingBlocksArray.ToArray(typeof(string));
            selectBaseCombobox.SelectedIndex = 0;
        }
        
        
        public void createCombinations(string[] tokens, int pos, string baseCombination, ComboBox combobox, ArrayList buildingBlocksArray)
        {
            if (pos == tokens.Length) return;
            for (int i = pos + 1; i < tokens.Length; ++i)
            {
                string newBaseCombination = baseCombination + (baseCombination.Length > 0 ? ";" : "") +tokens[i];
                buildingBlocksArray.Add(newBaseCombination);
                combobox.Items.Add(newBaseCombination.Replace(";", " + "));
                createCombinations(tokens, i, newBaseCombination, combobox, buildingBlocksArray);
            }
        }
        

        private void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        
        
        public Dictionary<string, object[]> createGridData(Dictionary<int, int> input)
        {
            Dictionary<string, object[]> data = new Dictionary<string, object[]>();
            
            foreach (KeyValuePair<int, int> row in input)
            {
                if (MS2Fragment.MONOISOTOPE_POSITIONS.ContainsKey(row.Key))
                {
                    // check for heavy isotopes
                    int heavyElementIndex = MS2Fragment.HEAVY_DERIVATIVE[row.Key].Count - 1;
                    int heavyElementCount = 0;
                    string heavyShortcut = "";
                    for (; heavyElementIndex >= 0; --heavyElementIndex)
                    {
                        heavyElementCount = input[(int)MS2Fragment.HEAVY_DERIVATIVE[row.Key][heavyElementIndex]];
                        heavyShortcut = MS2Fragment.HEAVY_SHORTCUTS[(int)MS2Fragment.HEAVY_DERIVATIVE[(int)row.Key][heavyElementIndex]];
                        if (input[(int)MS2Fragment.HEAVY_DERIVATIVE[row.Key][heavyElementIndex]] > 0)
                        {
                            break;
                        }
                    }
            
                    data.Add(MS2Fragment.ELEMENT_SHORTCUTS[row.Key], new object[]{row.Value, heavyElementCount, heavyShortcut});
                }
            }
            return data;
        }
        
        
        
        
        public Dictionary<int, int> createElementData(Dictionary<string, object[]> input)
        {
            Dictionary<int, int> elements = MS2Fragment.createEmptyElementDict();
            foreach (KeyValuePair<string, object[]> row in input)
            {
                int elementIndex = MS2Fragment.ELEMENT_POSITIONS[row.Key];
                int heavyIndex = MS2Fragment.HEAVY_POSITIONS[(string)row.Value[2]];
                
                elements[elementIndex] = (int)row.Value[0];
                elements[heavyIndex] = (int)row.Value[1];
                
            }
            return elements;
        }
        

        private void addClick(object sender, EventArgs e)
        {
            int elementsSelected = 0;
            foreach (KeyValuePair<string, object[]> row in elements)
            {
                elementsSelected += (int)row.Value[0];
                elementsSelected += (int)row.Value[1];
            }
            if (elementsSelected == 0)
            {
                MessageBox.Show("No element selected");
                return;
            }
            if (textBoxFragmentName.Text == "")
            {
                MessageBox.Show("No name defined");
                return;
            }
            if (numericUpDownCharge.Value == 0)
            {
                MessageBox.Show("Fragment must have an either positive or negative charge");
                return;
            }
            if (!chemAdding)
            {
                foreach (KeyValuePair<string, object[]> row in elements)
                {
                    row.Value[0] = -(int)row.Value[0];
                    row.Value[1] = -(int)row.Value[1];
                }
            }
            
            string lipidClass = ms2form.getHeadgroup();
            int charge = Convert.ToInt32(numericUpDownCharge.Value);
            if (ms2form.creatorGUI.lipidCreator.allFragments[lipidClass][charge >= 0].ContainsKey(textBoxFragmentName.Text))
            {
                MessageBox.Show((charge >= 0 ? "Positive" : "Negative") + " fragment '" + textBoxFragmentName.Text + "' already registered for lipid class '" + lipidClass + "'");
                return;
            }
        
            
            Dictionary<int, int> newElements = createElementData(elements);
            MS2Fragment newFragment = new MS2Fragment(textBoxFragmentName.Text, charge, null, newElements, buildingBlocks[selectBaseCombobox.SelectedIndex]);
            newFragment.userDefined = true;
            ms2form.creatorGUI.lipidCreator.allFragments[lipidClass][charge >= 0].Add(textBoxFragmentName.Text, newFragment);
            if (Convert.ToInt32(numericUpDownCharge.Value) > 0)
            {
                ms2form.checkedListBoxPositiveFragments.Items.Add(textBoxFragmentName.Text);
            }
            else
            {
                ms2form.checkedListBoxNegativeFragments.Items.Add(textBoxFragmentName.Text);
            }
            this.Close();
        }
        
        
        
        private void dataGridView1CellValueChanged(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if(updating) return;
            updating = true;
            string key = dataGridViewElements.Rows[e.RowIndex].Cells[0].Value.ToString();
            string val = dataGridViewElements.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            if (e.ColumnIndex != 3)
            {
                int n;
                try {
                    n = Convert.ToInt32(val);
                }
                catch (Exception ee){
                    n = 0;
                }
                n = Math.Max(n, 0);
                elements[key][e.ColumnIndex - 1] = n;
                dataGridViewElements.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = n;
            }
            else
            {
                elements[key][e.ColumnIndex - 1] = val;
            }
            updateInfo();
            updating = false;
        }
        
        
        public void selectBaseComboboxValueChanged(Object sender, EventArgs e)
        {
            if (selectBaseCombobox.SelectedIndex > 0)
            {
                groupboxAddingSubtracting.Enabled = true;
            }
            else
            {
                groupboxAddingSubtracting.Enabled = false;
                chemAdding = true;
                radioButtonAdding.Checked = true;
            }
            updateInfo();
        }
        
        void addingClicked(Object sender,EventArgs e)
        {
            chemAdding = true;
            updateInfo();
        }
        
        void subtractingClicked(Object sender,EventArgs e)
        {
            chemAdding = false;
            updateInfo();
        }

        private void updateInfo()
        {
            //double mass = 0;
            string chemForm = "";
            string baseName = "";
            string connector = "";
            string lBracket = "";
            string rBracket = "";
            string chrg = "";
            
            if (selectBaseCombobox.SelectedIndex > 0)
            {
                baseName = (string)selectBaseCombobox.SelectedItem;
            }
            
            foreach (KeyValuePair<int, int> row in MS2Fragment.MONOISOTOPE_POSITIONS)
            {
                string element = MS2Fragment.ELEMENT_SHORTCUTS[row.Key];
                int elementCount = (int)elements[element][0];
                int heavyElementCount = (int)elements[element][1];
                string heavyElement = MS2Fragment.ELEMENT_SHORTCUTS[MS2Fragment.HEAVY_POSITIONS[(string)elements[element][2]]];
                
                if (elementCount > 0) chemForm += element + Convert.ToString(elementCount);
                if (heavyElementCount > 0) chemForm += heavyElement + Convert.ToString(heavyElementCount);
            }
            if (chemForm != "" && numericUpDownCharge.Value > 0)
            {
                chrg = "+";
            }
            else if (chemForm != "" && numericUpDownCharge.Value < 0)
            {
                chrg = "-";
            }
            if (baseName.Length > 0 && chemForm.Length > 0)
            {
                connector = chemAdding ? " + " : " - ";
                lBracket = "(";
                rBracket = ")";
            }
            labelMass.Text = lBracket + baseName + connector + chemForm + rBracket + chrg;
        }

        private void numericUpDown1TextChanged(object sender, EventArgs e)
        {
            updateInfo();
        }
    }
}
