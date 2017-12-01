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
        
        public AddHeavyPrecursor(CreatorGUI creatorGUI, LipidCategory category)
        {
            this.creatorGUI = creatorGUI;
            buildingBlockDataTables = new ArrayList();
            updating = false;
        
            InitializeComponent();
            
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
            buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable(precursor.elements));
                    
            switch(precursor.buildingBlockType)
            {
                case 0:
                    comboBox2.Items.Add("Fatty acid 1");
                    comboBox2.Items.Add("Fatty acid 2");
                    comboBox2.Items.Add("Fatty acid 3");
                    comboBox2.Items.Add("Fatty acid 4");
                    buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable());
                    buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable());
                    buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable());
                    buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable());
                    break;
                    
                case 1:
                    comboBox2.Items.Add("Fatty acid 1");
                    comboBox2.Items.Add("Fatty acid 2");
                    comboBox2.Items.Add("Fatty acid 3");
                    buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable());
                    buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable());
                    buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable());
                    break;
                    
                case 2:
                case 6:
                    comboBox2.Items.Add("Fatty acid 1");
                    comboBox2.Items.Add("Fatty acid 2");
                    buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable());
                    buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable());
                    break;
                    
                case 3:
                case 7:
                    comboBox2.Items.Add("Fatty acid");
                    buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable());
                    break;
                    
                case 4:
                    comboBox2.Items.Add("Long chain base");
                    comboBox2.Items.Add("Fatty acid");
                    buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable());
                    buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable());
                    break;
                    
                case 5:
                    comboBox2.Items.Add("Long chain base");
                    buildingBlockDataTables.Add(MS2Fragment.createEmptyElementTable());
                    break;
                    
                case 8:
                    break;
                    
                default:
                    break;
            }
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
                dataGridView1.DataSource = buildingBlockDataTables[0];
            }
            
        }
        

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Items.Count > 0)
            {
                dataGridView1.DataSource = buildingBlockDataTables[comboBox2.SelectedIndex];
            }
        }
        
        
        public void dataGridView1DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.Columns[0].Width = (dataGridView1.Width - 2) / 3;
            dataGridView1.Columns[1].Width = (dataGridView1.Width - 2) / 3;
            dataGridView1.Columns[2].Width = (dataGridView1.Width - 2) / 3;
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Rows[(int)Molecules.C].ReadOnly = true;
            dataGridView1.Rows[(int)Molecules.H].ReadOnly = true;
            dataGridView1.Rows[(int)Molecules.O].ReadOnly = true;
            dataGridView1.Rows[(int)Molecules.N].ReadOnly = true;
            dataGridView1.Rows[(int)Molecules.P].ReadOnly = true;
            dataGridView1.Rows[(int)Molecules.S].ReadOnly = true;
            dataGridView1.Rows[(int)Molecules.Na].ReadOnly = true;
            dataGridView1.Update();
            dataGridView1.Refresh();
        }
        
        
        private void dataGridView1CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(updating) return;
            if (!dataGridView1.Rows[e.RowIndex].ReadOnly && dataGridView1.Columns[e.ColumnIndex].Name == "Count")
            {   
                updating = true;
                if (comboBox2.SelectedIndex == 0)
                {
                    string headgroup = (string)comboBox1.Items[comboBox1.SelectedIndex];
                    Precursor precursor = creatorGUI.lipidCreator.headgroups[headgroup];
                    
                    int orig = Convert.ToInt32(precursor.elements.Rows[e.RowIndex - 1][0]);
                    int n = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                    if (n < 0) n = 0;
                    if (n > orig) n = orig;
                    dataGridView1.Rows[e.RowIndex].Cells[0].Value = n;
                    dataGridView1.Rows[e.RowIndex - 1].Cells[0].Value = orig - n;
                }
                else
                {
                    int n = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                    if (n < 0) n = 0;
                    dataGridView1.Rows[e.RowIndex].Cells[0].Value = n;
                    dataGridView1.Rows[e.RowIndex - 1].Cells[0].Value = -n;
                }
                updating = false;
            }
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            string headgroup = (string)comboBox1.Items[comboBox1.SelectedIndex];
            string name = headgroup + "/" + textBox1.Text;
            
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
                heavyPrecursor.elements = MS2Fragment.createEmptyElementTable((DataTable)buildingBlockDataTables[0]);
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
                    heavyPrecursor.userDefinedFattyAcids.Add(buildingBlockDataTables[i]);
                }
                creatorGUI.lipidCreator.headgroups.Add(name, heavyPrecursor);
                precursor.heavyLabeledPrecursors.Add(heavyPrecursor);
                
                creatorGUI.lipidCreator.categoryToClass[(int)heavyPrecursor.category].Add(name);
                
                // copy all MS2Fragments
                creatorGUI.lipidCreator.allFragments.Add(name, new ArrayList());
                foreach (MS2Fragment ms2Fragment in creatorGUI.lipidCreator.allFragments[precursor.name])
                {
                    creatorGUI.lipidCreator.allFragments[name].Add(new MS2Fragment(ms2Fragment));
                }
                
                this.Close();
            }
        }
    }
}
