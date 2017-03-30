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
        bool chemAdding = true;

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

            dataGridViewElements.DataSource = elements;
            dataGridViewElements.Columns[3].Visible = false;
            dataGridViewElements.Columns[0].Width = 125;
            dataGridViewElements.Columns[1].Width = 125;
            dataGridViewElements.Columns[2].Width = 247;
            dataGridViewElements.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewElements.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewElements.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            
            
            // base types:
            // 0 -> fixed, FA1, FA2, FA3, FA4, FA1 + FA2, FA1 + FA3, FA1 + FA4, FA2 + FA3, FA2 + FA4, FA3 + FA4, FA1 + FA2 + FA3, FA1 + FA2 + FA4, FA1 + FA3 + FA4, PRE
            // 1 -> fixed, FA1, FA2, FA3, FA1 + FA2, FA1 + FA3, FA2 + FA3, PRE
            // 2 -> fixed, FA1, FA2, PRE
            // 3 -> fixed, FA, PRE
            // 4 -> fixed, LCB, FA, HG, LCB + FA, LCB + HG, FA + HG, PRE
            // 5 -> fixed, LCB, HG, PRE
            // 6 -> fixed, FA1, FA2, HG, FA1 + FA2, FA1 + HG, FA2 + HG, PRE
            // 7 -> fixed, FA1, HG, PRE
            String lipidClass = ((TabPage)ms2form.tabPages[ms2form.tabControlFragments.SelectedIndex]).Text;
            int bbType = ms2form.creatorGUI.lipidCreatorForm.buildingBlockTypes[lipidClass];
            
            selectBaseCombobox.Items.Add("fixed");
            switch(bbType)
            {
                case 0:
                    buildingBlocks = new string[]{"", "FA1", "FA2", "FA3", "FA4", "FA1;FA2", "FA1;FA3", "FA1;FA4", "FA2;FA3", "FA2;FA4", "FA3;FA4", "FA1;FA2;FA3", "FA1;FA2;FA4", "FA1;FA3;FA4", "FA2;FA3;FA4", "PRE"};
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
                    break;
                    
                case 1:
                    buildingBlocks = new string[]{"", "FA1", "FA2", "FA3", "FA1;FA2", "FA1;FA3", "FA2;FA3", "PRE"};
                    selectBaseCombobox.Items.Add("FA1");
                    selectBaseCombobox.Items.Add("FA2");
                    selectBaseCombobox.Items.Add("FA3");
                    selectBaseCombobox.Items.Add("FA1 + FA2");
                    selectBaseCombobox.Items.Add("FA1 + FA3");
                    selectBaseCombobox.Items.Add("FA2 + FA3");
                    break;
                    
                case 2:
                    buildingBlocks = new string[]{"", "FA1", "FA2", "PRE"};
                    selectBaseCombobox.Items.Add("FA1");
                    selectBaseCombobox.Items.Add("FA2");
                    break;
                    
                case 3:
                    buildingBlocks = new string[]{"", "FA", "PRE"};
                    selectBaseCombobox.Items.Add("FA");
                    break;
                    
                case 4:
                    buildingBlocks = new string[]{"", "LCB", "FA", "HG", "LCB;FA", "LCB;HG", "FA;HG", "PRE"};
                    selectBaseCombobox.Items.Add("LCB");
                    selectBaseCombobox.Items.Add("FA");
                    selectBaseCombobox.Items.Add("HG");
                    selectBaseCombobox.Items.Add("LCB + FA");
                    selectBaseCombobox.Items.Add("LCB + HG");
                    selectBaseCombobox.Items.Add("FA + HG");
                    break;
                    
                case 5:
                    buildingBlocks = new string[]{"", "LCB", "HG", "PRE"};
                    selectBaseCombobox.Items.Add("LCB");
                    selectBaseCombobox.Items.Add("HG");
                    break;
                    
                case 6:
                    buildingBlocks = new string[]{"", "FA1", "FA2", "HG", "FA1;FA2", "FA1;HG", "FA2;HG", "PRE"};
                    selectBaseCombobox.Items.Add("FA1");
                    selectBaseCombobox.Items.Add("FA2");
                    selectBaseCombobox.Items.Add("HG");
                    selectBaseCombobox.Items.Add("FA1 + FA2");
                    selectBaseCombobox.Items.Add("FA1 + HG");
                    selectBaseCombobox.Items.Add("FA2 + HG");
                    break;
                    
                case 7:
                    buildingBlocks = new string[]{"", "FA", "HG", "PRE"};
                    selectBaseCombobox.Items.Add("FA");
                    selectBaseCombobox.Items.Add("HG");
                    break;
            }
            selectBaseCombobox.Items.Add("Precursor");
            selectBaseCombobox.SelectedIndex = 0;
        }

        private void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        

        private void addClick(object sender, EventArgs e)
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
                foreach (DataRow row in elements.Rows)
                {
                    row["Count"] = -Convert.ToInt32(row["Count"]);
                }
            }
            
            String lipidClass = ((TabPage)ms2form.tabPages[ms2form.tabControlFragments.SelectedIndex]).Text;
            ((ArrayList)ms2form.currentLipid.MS2Fragments[lipidClass]).Add(new MS2Fragment(textBoxFragmentName.Text, Convert.ToInt32(numericUpDownCharge.Value), null, true, elements, buildingBlocks[selectBaseCombobox.SelectedIndex], ""));
            this.Close();
        }
        
        
        
        private void dataGridView1CellValueChanged(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            int n;
            if (string.IsNullOrEmpty(Convert.ToString(elements.Rows[e.RowIndex][0])) || !int.TryParse(Convert.ToString(elements.Rows[e.RowIndex][0]), out n)) elements.Rows[e.RowIndex][0] = 0;
            updateInfo();
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
            
            foreach (DataRow row in elements.Rows)
            {
                //mass += Convert.ToDouble(row["Count"]) * Convert.ToDouble(row["mass"]);
                if (Convert.ToInt32(row["Count"]) > 0)
                {
                    chemForm += Convert.ToString(row["Shortcut"]) + Convert.ToString(row["Count"]);
                }
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
            //label1.Text = baseName + string.Format("{0:0.0000} Da", mass) + ", " + chemForm;
            labelMass.Text = lBracket + baseName + connector + chemForm + rBracket + chrg;
        }

        private void numericUpDown1TextChanged(object sender, EventArgs e)
        {
            updateInfo();
        }
    }
}
