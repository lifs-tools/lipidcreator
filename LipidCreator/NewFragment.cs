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

        DataTable elements;
        MS2Form ms2form;
        string[] buildingBlocks;

        public NewFragment(MS2Form ms2form)
        {
            this.ms2form = ms2form;
            elements = new DataTable();
            elements.Clear();
            String count = "Count";
            String shortcut = "Shortcut";
            String element = "Element";
            String monoMass = "mass";

            DataColumn columnCount = elements.Columns.Add(count);
            DataColumn columnShortcut = elements.Columns.Add(shortcut);
            DataColumn columnElement = elements.Columns.Add(element);
            elements.Columns.Add(monoMass);

            columnCount.DataType = System.Type.GetType("System.Int32");
            columnShortcut.ReadOnly = true;
            columnElement.ReadOnly = true;

            DataRow carbon = elements.NewRow();
            carbon[count] = "0";
            carbon[shortcut] = "C";
            carbon[element] = "carbon";
            carbon[monoMass] = 12;
            elements.Rows.Add(carbon);

            DataRow hydrogen = elements.NewRow();
            hydrogen[count] = "0";
            hydrogen[shortcut] = "H";
            hydrogen[element] = "hydrogen";
            hydrogen[monoMass] = 1.007825035;
            elements.Rows.Add(hydrogen);

            DataRow oxygen = elements.NewRow();
            oxygen[count] = "0";
            oxygen[shortcut] = "O";
            oxygen[element] = "oxygen";
            oxygen[monoMass] = 15.99491463;
            elements.Rows.Add(oxygen);

            DataRow nitrogen = elements.NewRow();
            nitrogen[count] = "0";
            nitrogen[shortcut] = "N";
            nitrogen[element] = "nitrogen";
            nitrogen[monoMass] = 14.003074;
            elements.Rows.Add(nitrogen);

            DataRow phosphor = elements.NewRow();
            phosphor[count] = "0";
            phosphor[shortcut] = "P";
            phosphor[element] = "phosphor";
            phosphor[monoMass] = 30.973762;
            elements.Rows.Add(phosphor);

            DataRow sulfur = elements.NewRow();
            sulfur[count] = "0";
            sulfur[shortcut] = "S";
            sulfur[element] = "sulfur";
            sulfur[monoMass] =  31.9720707;
            elements.Rows.Add(sulfur);

            DataRow sodium = elements.NewRow();
            sodium[count] = "0";
            sodium[shortcut] = "Na";
            sodium[element] = "sodium";
            sodium[monoMass] = 22.9897677;
            elements.Rows.Add(sodium);


            InitializeComponent();

            dataGridView1.DataSource = elements;
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[0].Width = 125;
            dataGridView1.Columns[1].Width = 125;
            dataGridView1.Columns[2].Width = 247;
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            
            
            // base types:
            // 0 -> fixed, FA1, FA2, FA3, FA4, FA1 + FA2, FA1 + FA3, FA1 + FA4, FA2 + FA3, FA2 + FA4, FA3 + FA4, FA1 + FA2 + FA3, FA1 + FA2 + FA4, FA1 + FA3 + FA4, PRE
            // 1 -> fixed, FA1, FA2, FA3, FA1 + FA2, FA1 + FA3, FA2 + FA3, PRE
            // 2 -> fixed, FA1, FA2, PRE
            // 3 -> fixed, FA, PRE
            // 4 -> fixed, LCB, FA, HG, LCB + FA, LCB + HG, FA + HG, PRE
            // 5 -> fixed, LCB, HG, PRE
            // 6 -> fixed, FA1, FA2, HG, FA1 + FA2, FA1 + HG, FA2 + HG, PRE
            // 7 -> fixed, FA1, HG, PRE
            String lipidClass = ((TabPage)ms2form.tabPages[ms2form.tabControl1.SelectedIndex]).Text;
            int bbType = ms2form.creatorGUI.lipidCreatorForm.buildingBlockTypes[lipidClass];
            
            select_base_combobox.Items.Add("fixed");
            switch(bbType)
            {
                case 0:
                    buildingBlocks = new string[]{"", "FA1", "FA2", "FA3", "FA4", "FA1;FA2", "FA1;FA3", "FA1;FA4", "FA2;FA3", "FA2;FA4", "FA3;FA4", "FA1;FA2;FA3", "FA1;FA2;FA4", "FA1;FA3;FA4", "FA2;FA3;FA4", "PRE"};
                    select_base_combobox.Items.Add("FA1");
                    select_base_combobox.Items.Add("FA2");
                    select_base_combobox.Items.Add("FA3");
                    select_base_combobox.Items.Add("FA4");
                    select_base_combobox.Items.Add("FA1 + FA2");
                    select_base_combobox.Items.Add("FA1 + FA3");
                    select_base_combobox.Items.Add("FA1 + FA4");
                    select_base_combobox.Items.Add("FA2 + FA3");
                    select_base_combobox.Items.Add("FA2 + FA4");
                    select_base_combobox.Items.Add("FA3 + FA4");
                    select_base_combobox.Items.Add("FA1 + FA2 + FA3");
                    select_base_combobox.Items.Add("FA1 + FA2 + FA4");
                    select_base_combobox.Items.Add("FA1 + FA3 + FA4");
                    select_base_combobox.Items.Add("FA2 + FA3 + FA4");
                    break;
                    
                case 1:
                    buildingBlocks = new string[]{"", "FA1", "FA2", "FA3", "FA1;FA2", "FA1;FA3", "FA2;FA3", "PRE"};
                    select_base_combobox.Items.Add("FA1");
                    select_base_combobox.Items.Add("FA2");
                    select_base_combobox.Items.Add("FA3");
                    select_base_combobox.Items.Add("FA1 + FA2");
                    select_base_combobox.Items.Add("FA1 + FA3");
                    select_base_combobox.Items.Add("FA2 + FA3");
                    break;
                    
                case 2:
                    buildingBlocks = new string[]{"", "FA1", "FA2", "PRE"};
                    select_base_combobox.Items.Add("FA1");
                    select_base_combobox.Items.Add("FA2");
                    break;
                    
                case 3:
                    buildingBlocks = new string[]{"", "FA", "PRE"};
                    select_base_combobox.Items.Add("FA");
                    break;
                    
                case 4:
                    buildingBlocks = new string[]{"", "LCB", "FA", "HG", "LCB;FA", "LCB;HG", "FA;HG", "PRE"};
                    select_base_combobox.Items.Add("LCB");
                    select_base_combobox.Items.Add("FA");
                    select_base_combobox.Items.Add("HG");
                    select_base_combobox.Items.Add("LCB + FA");
                    select_base_combobox.Items.Add("LCB + HG");
                    select_base_combobox.Items.Add("FA + HG");
                    break;
                    
                case 5:
                    buildingBlocks = new string[]{"", "LCB", "HG", "PRE"};
                    select_base_combobox.Items.Add("LCB");
                    select_base_combobox.Items.Add("HG");
                    break;
                    
                case 6:
                    buildingBlocks = new string[]{"", "FA1", "FA2", "HG", "FA1;FA2", "FA1;HG", "FA2;HG", "PRE"};
                    select_base_combobox.Items.Add("FA1");
                    select_base_combobox.Items.Add("FA2");
                    select_base_combobox.Items.Add("HG");
                    select_base_combobox.Items.Add("FA1 + FA2");
                    select_base_combobox.Items.Add("FA1 + HG");
                    select_base_combobox.Items.Add("FA2 + HG");
                    break;
                    
                case 7:
                    buildingBlocks = new string[]{"", "FA", "HG", "PRE"};
                    select_base_combobox.Items.Add("FA");
                    select_base_combobox.Items.Add("HG");
                    break;
            }
            select_base_combobox.Items.Add("Precursor");
            select_base_combobox.SelectedIndex = 0;
        }

        private void tableView_KeyPress(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Check for the flag being set in the KeyDown event.
            if (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9)
            {
                // Stop the character from being entered into the control since it is non-numerical.
                e.Handled = true;
            }
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        

        private void add_Click(object sender, EventArgs e)
        {
            int elementsSelected = 0;
            foreach (DataRow row in elements.Rows)
            {
                int cnt = Convert.ToInt32(row["Count"]);
                if (cnt < 0)
                {
                    MessageBox.Show("Invalid count for element " + row["Element"]);
                    return;
                }
                if (cnt > 0) elementsSelected += 1;
            }
            if (elementsSelected == 0)
            {
                MessageBox.Show("No element selected");
                return;
            }
            if (textBox1.Text == "")
            {
                MessageBox.Show("No name defined");
                return;
            }
            
            String lipidClass = ((TabPage)ms2form.tabPages[ms2form.tabControl1.SelectedIndex]).Text;
            ((ArrayList)ms2form.currentLipid.MS2Fragments[lipidClass]).Add(new MS2Fragment(textBox1.Text, Convert.ToInt32(numericUpDown1.Value), null, true, elements, buildingBlocks[select_base_combobox.SelectedIndex], ""));
            this.Close();
        }
        private void dataGridView1_CellValueChanged(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (string.IsNullOrEmpty(Convert.ToString(elements.Rows[e.RowIndex][0]))) elements.Rows[e.RowIndex][0] = 0;
            updateInfo();
        }

        private void updateInfo()
        {
            double mass = 0;
            String chemForm = "";
            foreach (DataRow row in elements.Rows)
            {
                mass += Convert.ToDouble(row["Count"]) * Convert.ToDouble(row["mass"]);
                if (Convert.ToInt32(row["Count"]) > 0)
                {
                    chemForm += Convert.ToString(row["Shortcut"]) + Convert.ToString(row["Count"]);
                }
            }
            if (chemForm != "" && numericUpDown1.Value > 0)
            {
                chemForm += "+";
            }
            else if (chemForm != "" && numericUpDown1.Value < 0)
            {
                chemForm += "-";
            }
            label1.Text = string.Format("{0:0.0000} Da", mass) + ", " + chemForm;
        }

        private void numericUpDown1_TextChanged(object sender, EventArgs e)
        {
            updateInfo();
        }
    }
}
