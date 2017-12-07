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

namespace LipidCreator
{
    public partial class AddHeavyPrecursor : Form
    {
        public CreatorGUI creatorGUI;
        ArrayList buildingBlockDataTables;
        public bool updating;
        public Dictionary<string, object[]> currentDict = null;
        
        
        public AddHeavyPrecursor(CreatorGUI creatorGUI, LipidCategory category)
        {
            this.creatorGUI = creatorGUI;
            buildingBlockDataTables = new ArrayList();
        
            InitializeComponent();
            
            updating = true;
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Element";
            dataGridView1.Columns[1].Name = "Count";
            dataGridView1.Columns[2].Name = "Isotope count";
            DataGridViewComboBoxColumn combo1 = new DataGridViewComboBoxColumn();
            dataGridView1.Columns.Add(combo1);
            combo1.Name = "Isotope type";
            
            for (int k = 0; k < MS2Fragment.HEAVY_DERIVATIVE.Count; ++k) dataGridView1.Rows.Add(new object[] {"-", 0, 0, new DataGridViewComboBoxCell()});
            
            foreach (KeyValuePair<int, ArrayList> row in MS2Fragment.HEAVY_DERIVATIVE)
            {
                int l = MS2Fragment.MONOISOTOPE_POSITIONS[row.Key];
                dataGridView1.Rows[l].Cells[0].Value = MS2Fragment.ELEMENT_SHORTCUTS[row.Key];
                dataGridView1.Rows[l].Cells[1].Value = 0;
                dataGridView1.Rows[l].Cells[2].Value = 0;
                
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridView1.Rows[l].Cells[3];
                int j = 0;
                foreach (int element in row.Value)
                {
                    if (j++ == 0) cell.Value = MS2Fragment.HEAVY_SHORTCUTS[element];
                    cell.Items.Add(MS2Fragment.HEAVY_SHORTCUTS[element]);
                }
            }
            dataGridView1.Columns[0].Width = (dataGridView1.Width - 2) / 4;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].Width = (dataGridView1.Width - 2) / 4;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].Width = (dataGridView1.Width - 2) / 4;
            dataGridView1.Columns[2].Width = (dataGridView1.Width - 2) / 4;
            dataGridView1.AllowUserToAddRows = false;
            updating = false;
            
            foreach (string lipidClass in creatorGUI.lipidCreator.categoryToClass[(int)category])
            {
                if (!creatorGUI.lipidCreator.headgroups.ContainsKey(lipidClass)) Console.WriteLine(lipidClass);
                if (!creatorGUI.lipidCreator.headgroups[lipidClass].heavyLabeled) comboBox1.Items.Add(lipidClass);
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            
            
        }
        
        
        
        
        public void changeDataGridContent(Dictionary<string, object[]> data)
        {
            currentDict = data;
            updating = true;
            foreach (KeyValuePair<string, object[]> row in data)
            {
                int l = MS2Fragment.MONOISOTOPE_POSITIONS[(int)MS2Fragment.ELEMENT_POSITIONS[row.Key]];
                
                dataGridView1.Rows[l].Cells[1].Value = row.Value[0];
                dataGridView1.Rows[l].Cells[2].Value = row.Value[1];
                dataGridView1.Rows[l].Cells[3].Value = row.Value[2];
            }
            updating = false;
        }
        
        
        
        public Dictionary<string, object[]> createGridData(Dictionary<int, int> input)
        {
            Dictionary<string, object[]> data = new Dictionary<string, object[]>();
            foreach (KeyValuePair<int, int> row in input)
            {
                if (MS2Fragment.MONOISOTOPE_POSITIONS.ContainsKey(row.Key))
                {
                    data.Add(MS2Fragment.ELEMENT_SHORTCUTS[row.Key], new object[]{row.Value, 0, MS2Fragment.HEAVY_SHORTCUTS[(int)MS2Fragment.HEAVY_DERIVATIVE[(int)row.Key][0]]});
                }
            }
            return data;
        }
        
        

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // base types:
            // 0 -> fixed, FA1, FA2, FA3, FA4, FA1 + FA2, FA1 + FA3, FA1 + FA4, FA2 + FA3, FA2 + FA4, FA3 + FA4, FA1 + FA2 + FA3, FA1 + FA2 + FA4, FA1 + FA3 + FA4, PRE
            // 1 -> fixed, FA1, FA2, FA3, FA1 + FA2, FA1 + FA3, FA2 + FA3, PRE
            // 2 -> fixed, FA1, FA2, PRE
            // 3 -> fixed, FA, PRE
            // 4 -> fixed, LCB, FA, HG, LCB + FA, LCB + HG, FA + HG, PRE
            // 5 -> fixed, LCB, HG, PRE
            // 6 -> fixed, FA1, FA2, HG, FA1 + FA2, FA1 + HG, FA2 + HG, PRE
            // 7 -> fixed, FA1, HG, PRE
            // 8 -> fixed, PRE
            string headgroup = (string)comboBox1.Items[comboBox1.SelectedIndex];
            Precursor precursor = creatorGUI.lipidCreator.headgroups[headgroup];
            comboBox2.Items.Clear();
            buildingBlockDataTables.Clear();
            
            
            comboBox2.Items.Add("Head group");
            buildingBlockDataTables.Add(createGridData(MS2Fragment.createFilledElementDict(precursor.elements)));
                    
            switch(precursor.buildingBlockType)
            {
                case 0:
                    comboBox2.Items.Add("Fatty acid 1");
                    comboBox2.Items.Add("Fatty acid 2");
                    comboBox2.Items.Add("Fatty acid 3");
                    comboBox2.Items.Add("Fatty acid 4");
                    buildingBlockDataTables.Add(createGridData(MS2Fragment.createEmptyElementDict()));
                    buildingBlockDataTables.Add(createGridData(MS2Fragment.createEmptyElementDict()));
                    buildingBlockDataTables.Add(createGridData(MS2Fragment.createEmptyElementDict()));
                    buildingBlockDataTables.Add(createGridData(MS2Fragment.createEmptyElementDict()));
                    break;
                    
                case 1:
                    comboBox2.Items.Add("Fatty acid 1");
                    comboBox2.Items.Add("Fatty acid 2");
                    comboBox2.Items.Add("Fatty acid 3");
                    buildingBlockDataTables.Add(createGridData(MS2Fragment.createEmptyElementDict()));
                    buildingBlockDataTables.Add(createGridData(MS2Fragment.createEmptyElementDict()));
                    buildingBlockDataTables.Add(createGridData(MS2Fragment.createEmptyElementDict()));
                    break;
                    
                case 2:
                case 6:
                    comboBox2.Items.Add("Fatty acid 1");
                    comboBox2.Items.Add("Fatty acid 2");
                    buildingBlockDataTables.Add(createGridData(MS2Fragment.createEmptyElementDict()));
                    buildingBlockDataTables.Add(createGridData(MS2Fragment.createEmptyElementDict()));
                    break;
                    
                case 3:
                case 7:
                    comboBox2.Items.Add("Fatty acid");
                    buildingBlockDataTables.Add(createGridData(MS2Fragment.createEmptyElementDict()));
                    break;
                    
                case 4:
                    comboBox2.Items.Add("Long chain base");
                    comboBox2.Items.Add("Fatty acid");
                    buildingBlockDataTables.Add(createGridData(MS2Fragment.createEmptyElementDict()));
                    buildingBlockDataTables.Add(createGridData(MS2Fragment.createEmptyElementDict()));
                    break;
                    
                case 5:
                    comboBox2.Items.Add("Long chain base");
                    buildingBlockDataTables.Add(createGridData(MS2Fragment.createEmptyElementDict()));
                    break;
                    
                case 8:
                    break;
                    
                default:
                    break;
            }
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
                changeDataGridContent((Dictionary<string, object[]>)buildingBlockDataTables[0]);
            }
            
        }
        

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Items.Count > 0)
            {
                changeDataGridContent((Dictionary<string, object[]>)buildingBlockDataTables[comboBox2.SelectedIndex]);
            }
        }
        
        
        private void dataGridView1CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (currentDict == null) return;
            string key = (string)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
            currentDict[key][e.ColumnIndex - 1] = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            
            if(updating) return;
            
            if (e.ColumnIndex != 2) return;
            updating = true;
            if (comboBox2.SelectedIndex == 0)
            {
                string headgroup = (string)comboBox1.Items[comboBox1.SelectedIndex];
                Precursor precursor = creatorGUI.lipidCreator.headgroups[headgroup];
                
                int orig = precursor.elements[MS2Fragment.LIGHT_ORIGIN[MS2Fragment.HEAVY_POSITIONS[(string)dataGridView1.Rows[e.RowIndex].Cells[3].Value]]];
                int n = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[2].Value);
                if (n < 0) n = 0;
                if (n > orig) n = orig;
                dataGridView1.Rows[e.RowIndex].Cells[2].Value = n;
                dataGridView1.Rows[e.RowIndex].Cells[1].Value = orig - n;
            }
            else
            {
                int n = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[2].Value);
                if (n < 0) n = 0;
                dataGridView1.Rows[e.RowIndex].Cells[2].Value = n;
                dataGridView1.Rows[e.RowIndex].Cells[1].Value = -n;
            }
            updating = false;
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            string headgroup = (string)comboBox1.Items[comboBox1.SelectedIndex];
            string name = headgroup + "/" + textBox1.Text;
            
            Console.WriteLine(dataGridView1.Rows[0].Cells[0].Value);
            
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("Please set a suffix!", "Not registrable");
            }
            else if(creatorGUI.lipidCreator.headgroups.ContainsKey(name))
            {
                MessageBox.Show("The lipid class '" + headgroup + "' with heavy label suffix '" + textBox1.Text + "' is already registered!", "Not registrable");
            }   
            else
            {
                // create and set precursor properties
                Precursor precursor = creatorGUI.lipidCreator.headgroups[headgroup];
                
                
                Precursor heavyPrecursor = new Precursor();
                heavyPrecursor.elements = MS2Fragment.createFilledElementDict((DataTable)buildingBlockDataTables[0]);
                heavyPrecursor.name = name;
                heavyPrecursor.category = precursor.category;
                heavyPrecursor.pathToImage = precursor.pathToImage;
                heavyPrecursor.buildingBlockType = precursor.buildingBlockType;
                foreach (KeyValuePair<string, bool> kvp in precursor.adductRestrictions)
                {
                    heavyPrecursor.adductRestrictions.Add(kvp.Key, kvp.Value);
                }
                heavyPrecursor.derivative = precursor.derivative;
                heavyPrecursor.heavyLabeled = true;
                heavyPrecursor.userDefined = true;
                heavyPrecursor.userDefinedFattyAcids = new ArrayList();
                for (int i = 1; i < buildingBlockDataTables.Count; ++i)
                {
                    heavyPrecursor.userDefinedFattyAcids.Add(MS2Fragment.createFilledElementDict((DataTable)buildingBlockDataTables[i]));
                }
                creatorGUI.lipidCreator.headgroups.Add(name, heavyPrecursor);
                precursor.heavyLabeledPrecursors.Add(heavyPrecursor);
                
                creatorGUI.lipidCreator.categoryToClass[(int)heavyPrecursor.category].Add(name);
                
                // copy all MS2Fragments
                creatorGUI.lipidCreator.allFragments.Add(name, new Dictionary<bool, Dictionary<string, MS2Fragment>>());
                creatorGUI.lipidCreator.allFragments[name].Add(true, new Dictionary<string, MS2Fragment>());
                creatorGUI.lipidCreator.allFragments[name].Add(false, new Dictionary<string, MS2Fragment>());
                foreach (KeyValuePair<string, MS2Fragment> ms2Fragment in creatorGUI.lipidCreator.allFragments[precursor.name][true])
                {
                    creatorGUI.lipidCreator.allFragments[name][true].Add(ms2Fragment.Key, new MS2Fragment(ms2Fragment.Value));
                }
                foreach (KeyValuePair<string, MS2Fragment> ms2Fragment in creatorGUI.lipidCreator.allFragments[precursor.name][false])
                {
                    creatorGUI.lipidCreator.allFragments[name][false].Add(ms2Fragment.Key, new MS2Fragment(ms2Fragment.Value));
                }
                
                this.Close();
            }
        }
    }
}
