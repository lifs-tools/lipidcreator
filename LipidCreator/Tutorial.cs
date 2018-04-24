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
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace LipidCreator
{
    
    public enum Tutorials {NoTutorial = -1, TutorialPRM = 0, TutorialMRM = 1, TutorialHeavyLabeled = 2};
    

    [Serializable]
    public class Tutorial
    {
        public Tutorials tutorial;
        public int tutorialStep;
        public CreatorGUI creatorGUI;
        public ArrayList elementsEnabledState;
        public LipidCategory currentTab;
        public Dictionary<int, int> maxSteps;
        public bool nextEnabled;
        public int pgIndex;
        public int currentMS2TabIndex;
        public Overlay tutorialArrow;
        public TutorialWindow tutorialWindow;
        
        public Tutorial(CreatorGUI creatorGUI)
        {
            tutorial = Tutorials.NoTutorial;
            tutorialStep = 0;
            nextEnabled = true;
            this.creatorGUI = creatorGUI;
            currentTab = LipidCategory.NoLipid;
            maxSteps = new Dictionary<int, int>(){
                {(int)Tutorials.NoTutorial, 0},
                {(int)Tutorials.TutorialPRM, 80},
                {(int)Tutorials.TutorialMRM, 20},
                {(int)Tutorials.TutorialHeavyLabeled, 20}
            };
            tutorialArrow = new Overlay(creatorGUI.lipidCreator.prefixPath);
            tutorialWindow = new TutorialWindow(creatorGUI, creatorGUI.lipidCreator.prefixPath);
            tutorialArrow.Visible = false;
            tutorialWindow.Visible = false;
            creatorGUI.plHgListbox.SelectedValueChanged += new System.EventHandler(listBoxInteraction);
            creatorGUI.tabControl.SelectedIndexChanged += new System.EventHandler(tabInteraction);
            creatorGUI.plFA1Textbox.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.plDB1Textbox.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.plFA2Textbox.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.plDB2Textbox.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.plPosAdductCheckbox1.CheckedChanged += new EventHandler(checkBoxInteraction);
            creatorGUI.plPosAdductCheckbox3.CheckedChanged += new EventHandler(checkBoxInteraction);
            creatorGUI.MS2fragmentsLipidButton.Click += buttonInteraction;
            creatorGUI.addLipidButton.Click += buttonInteraction;
            creatorGUI.addHeavyIsotopeButton.Click += buttonInteraction;
        }
        
        
        
        public void startTutorial(Tutorials t)
        {
        
            // if (!creatorGUI.resetLipidCreator()) return;
        
        
            tutorial = t;
            tutorialStep = 31;
            currentTab = LipidCategory.NoLipid;
            
            
            
            
            // TODO: remove these lines
            /*
            creatorGUI.changeTab((int)LipidCategory.PhosphoLipid);
            creatorGUI.ms2fragmentsForm = new MS2Form(creatorGUI);
            creatorGUI.ms2fragmentsForm.Owner = creatorGUI;
            creatorGUI.ms2fragmentsForm.ShowInTaskbar = false;
            creatorGUI.ms2fragmentsForm.Show();
            TabControl ms2tc = creatorGUI.ms2fragmentsForm.tabControlFragments;
            for (; pgIndex < ms2tc.TabPages.Count; ++pgIndex)
            {
                if (ms2tc.TabPages[pgIndex].Text.Equals("PG")) break;
            } 
            creatorGUI.ms2fragmentsForm.FormClosing += new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
            ms2tc.SelectedIndexChanged += new System.EventHandler(tabInteraction);
            creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(checkedListBoxInteraction);
            creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(checkedListBoxInteraction);
            creatorGUI.ms2fragmentsForm.buttonAddFragment.Click += buttonInteraction;
            creatorGUI.ms2fragmentsForm.buttonOK.Click += buttonInteraction;
            Dictionary<int, int> newElements = MS2Fragment.createEmptyElementDict();
            newElements[(int)Molecules.H] = 3;
            newElements[(int)Molecules.O] = 2;
            MS2Fragment newFragment = new MS2Fragment("testFrag", 1, null, newElements, "FA1");
            newFragment.userDefined = true;
            creatorGUI.lipidCreator.allFragments["PG"][true].Add("testFrag", newFragment);
            creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.Items.Add("testFrag");
            creatorGUI.ms2fragmentsForm.currentLipid.negativeFragments["PG"].Clear();
            creatorGUI.ms2fragmentsForm.currentLipid.negativeFragments["PG"].Add("FA1");
            creatorGUI.ms2fragmentsForm.currentLipid.negativeFragments["PG"].Add("HG(PG)");
            creatorGUI.ms2fragmentsForm.Refresh();
            creatorGUI.lipidTabList[(int)currentTab] = new PLLipid((PLLipid)creatorGUI.ms2fragmentsForm.currentLipid);
            creatorGUI.currentLipid = (Lipid)creatorGUI.lipidTabList[(int)currentTab];
            creatorGUI.ms2fragmentsForm.Close();
            
            creatorGUI.changeTab((int)LipidCategory.PhosphoLipid);
            creatorGUI.addHeavyPrecursor = new AddHeavyPrecursor(creatorGUI, creatorGUI.currentIndex);
            creatorGUI.addHeavyPrecursor.Owner = creatorGUI;
            creatorGUI.addHeavyPrecursor.ShowInTaskbar = false;
            creatorGUI.addHeavyPrecursor.Show();
            initHeavyLabeled();
            creatorGUI.addHeavyPrecursor.comboBox1.SelectedIndex = 24;
            creatorGUI.addHeavyPrecursor.textBox1.Text = "13C6d30";
            */
            
            XDocument doc;
            try 
            {
                doc = XDocument.Load("tutorial.lcXML");
                creatorGUI.lipidCreator.import(doc);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not read file, " + ex.Message, "Error while reading", MessageBoxButtons.OK);
                Console.WriteLine(ex.StackTrace);
            }
            
            
            
            
            elementsEnabledState = new ArrayList();
            ((TabPage)creatorGUI.tabList[(int)LipidCategory.NoLipid]).Controls.Add(tutorialArrow);
            foreach (Object element in creatorGUI.controlElements)
            {
                if (element is MenuItem) 
                {
                    elementsEnabledState.Add(((MenuItem)element).Enabled);
                    ((MenuItem)element).Enabled = false;
                }
                else
                {
                    elementsEnabledState.Add(((Control)element).Enabled);
                    ((Control)element).Enabled = false;
                }
            }
            nextTutorialStep(true);
        }
        
        
        
        public void initMS2Form()
        {
            pgIndex = 0;
            TabControl ms2tc = creatorGUI.ms2fragmentsForm.tabControlFragments;
            for (; pgIndex < ms2tc.TabPages.Count; ++pgIndex)
            {
                if (ms2tc.TabPages[pgIndex].Text.Equals("PG")) break;
            } 
            changeMS2Tab(0, (TabPage)creatorGUI.ms2fragmentsForm.tabPages[0]);
        
            creatorGUI.ms2fragmentsForm.FormClosing += new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
            ms2tc.SelectedIndexChanged += new System.EventHandler(tabInteraction);
            creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(checkedListBoxInteraction);
            creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(checkedListBoxInteraction);
            creatorGUI.ms2fragmentsForm.buttonAddFragment.Click += buttonInteraction;
            creatorGUI.ms2fragmentsForm.buttonOK.Click += buttonInteraction;
            creatorGUI.ms2fragmentsForm.isotopeList.SelectedIndexChanged += new EventHandler(comboBoxInteraction);
            creatorGUI.ms2fragmentsForm.contextMenuFragment.Popup += new System.EventHandler(contextMenuPopupInteraction);
            creatorGUI.ms2fragmentsForm.menuFragmentItem1.Click += new System.EventHandler(buttonInteraction);
            creatorGUI.ms2fragmentsForm.menuFragmentItem2.Click += new System.EventHandler(buttonInteraction);
        }
        
        
        
        public void initAddFragmentForm()
        {
            creatorGUI.ms2fragmentsForm.newFragment.FormClosing += new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
            creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.SelectedIndexChanged += new EventHandler(comboBoxInteraction);
            creatorGUI.ms2fragmentsForm.newFragment.addButton.Click += buttonInteraction;
            creatorGUI.ms2fragmentsForm.newFragment.numericUpDownCharge.ValueChanged += new EventHandler(numericInteraction);
            creatorGUI.ms2fragmentsForm.newFragment.dataGridViewElements.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(tableCellChanged);
        }
        
        
        
        public void initHeavyLabeled()
        {
            creatorGUI.addHeavyPrecursor.FormClosing += new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
            creatorGUI.addHeavyPrecursor.comboBox1.SelectedIndexChanged += new EventHandler(comboBoxInteraction);
            creatorGUI.addHeavyPrecursor.comboBox2.SelectedIndexChanged += new EventHandler(comboBoxInteraction);
            creatorGUI.addHeavyPrecursor.textBox1.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.addHeavyPrecursor.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(tableCellChanged);
            creatorGUI.addHeavyPrecursor.button1.Click += buttonInteraction;
            creatorGUI.addHeavyPrecursor.button2.Click += buttonInteraction;
        }
        
        
        
        public void nextTutorialStep(bool forward)
        {
            tutorialStep = forward ? (tutorialStep + 1) : (Math.Max(tutorialStep - 1, 1));
            if (tutorial == Tutorials.TutorialPRM) TutorialPRMStep();
            else if (tutorial == Tutorials.TutorialMRM) TutorialMRMStep();
            else if (tutorial == Tutorials.TutorialHeavyLabeled) TutorialHeavyLabeledStep();
            else quitTutorial();
        }
        
        
        
        public void quitTutorial()
        {
            tutorial = Tutorials.NoTutorial;
            tutorialStep = 0;
            tutorialArrow.Visible = false;
            tutorialWindow.Visible = false;
            
            for (int i = 0; i < elementsEnabledState.Count; ++i)
            {
                Object element = creatorGUI.controlElements[i];
                if (element is MenuItem) 
                {
                    ((MenuItem)element).Enabled = (bool)elementsEnabledState[i];
                }
                else
                {
                    ((Control)element).Enabled = (bool)elementsEnabledState[i];
                }
            }
            if (creatorGUI.ms2fragmentsForm != null)
            {
                if (creatorGUI.ms2fragmentsForm.newFragment != null) creatorGUI.ms2fragmentsForm.newFragment.Close();
                creatorGUI.ms2fragmentsForm.Close();
            }
            creatorGUI.Enabled = true;
            creatorGUI.changeTab((int)currentTab);
        }
        
        
        
        public void disableEverything()
        {
            if (tutorialArrow.Parent != null) tutorialArrow.Parent.Controls.Remove(tutorialArrow);
            if (tutorialWindow.Parent != null) tutorialWindow.Parent.Controls.Remove(tutorialWindow);
            
            foreach (Object element in creatorGUI.controlElements)
            {
                if (element is MenuItem)  ((MenuItem)element).Enabled = false;
                else ((Control)element).Enabled = false;
            }
            tutorialArrow.Visible = false;
            tutorialWindow.Visible = false;
            creatorGUI.Refresh();
            if (creatorGUI.ms2fragmentsForm != null)
            {
            
            
                foreach (Object element in creatorGUI.ms2fragmentsForm.controlElements)
                {
                    if (element is MenuItem){
                        ((MenuItem)element).Enabled = false;
                    }
                    else ((Control)element).Enabled = false;
                }
                creatorGUI.ms2fragmentsForm.Refresh();
                
                
                if (creatorGUI.ms2fragmentsForm.newFragment != null)
                {
                    NewFragment newFrag = creatorGUI.ms2fragmentsForm.newFragment;
                    foreach (Control control in newFrag.controlElements)
                    {
                        control.Enabled = false;
                    }
                    creatorGUI.ms2fragmentsForm.Refresh();
                }
            }
            if (creatorGUI.addHeavyPrecursor != null)
            {
                foreach (Control control in creatorGUI.addHeavyPrecursor.controlElements)
                {
                    control.Enabled = false;
                }
                creatorGUI.addHeavyPrecursor.Refresh();
            }
        }
        
        
        
        public void changeTab(LipidCategory lip)
        {
            if (currentTab != lip)
            {
                currentTab = lip;
                creatorGUI.changeTab((int)currentTab);
            }
            
            Control tab = (TabPage)creatorGUI.tabList[(int)currentTab];
            tab.Controls.Add(tutorialArrow);
            tab.Controls.Add(tutorialWindow);
            tutorialArrow.BringToFront();
            tutorialWindow.BringToFront();
            
            tab.Enabled = true;
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
            creatorGUI.Refresh();
        }
        
        
        
        public void changeMS2Tab(int index, Control control)
        {
            if (currentMS2TabIndex != index)
            {
                currentMS2TabIndex = index;
                creatorGUI.ms2fragmentsForm.tabControlFragments.SelectedIndex = index;
            }
            
            control.Controls.Add(tutorialArrow);
            creatorGUI.ms2fragmentsForm.Controls.Add(tutorialWindow);
            tutorialArrow.BringToFront();
            tutorialWindow.BringToFront();
            tutorialWindow.Refresh();
            creatorGUI.ms2fragmentsForm.Refresh();
        }
        
        
        
        
        public void listBoxInteraction(object sender, System.EventArgs e)
        {
            ListBox box = (ListBox)sender;
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 3 && box.SelectedItems.Count == 1 && box.SelectedItems[0].ToString().Equals("PG")) nextEnabled = true;
            else nextEnabled = false;
            tutorialWindow.Refresh();
        }
        
        
        
        
        
        public void contextMenuPopupInteraction(Object sender, EventArgs e)
        {
            creatorGUI.ms2fragmentsForm.menuFragmentItem1.Enabled = false;
            creatorGUI.ms2fragmentsForm.menuFragmentItem2.Enabled = false;
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 37 && creatorGUI.ms2fragmentsForm.editDeleteIndex == 0)
            {
                creatorGUI.ms2fragmentsForm.menuFragmentItem1.Enabled = true;
            }
            creatorGUI.ms2fragmentsForm.Refresh();
        }
        
        
        
        
        
        
        public void numericInteraction(object sender, System.EventArgs e)
        {
            MyNumericUpDown numericUpDown = (MyNumericUpDown)sender;
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 17)
            {
                nextEnabled = numericUpDown.Value == 1;
            }
            tutorialWindow.Refresh();
        }
        
        
        
        
        
        public void tabInteraction(Object sender,  EventArgs e)
        {
            if (creatorGUI.changingTabForced) return;
            if (tutorial == Tutorials.NoTutorial) return;
            
            if (creatorGUI.currentTabIndex == (int)LipidCategory.PhosphoLipid && tutorial == Tutorials.TutorialPRM && tutorialStep == 2)
            {
                nextTutorialStep(true);
                return;
            }
            else if (creatorGUI.ms2fragmentsForm.tabControlFragments.SelectedIndex == pgIndex && tutorial == Tutorials.TutorialPRM && tutorialStep == 12)
            {
                nextTutorialStep(true);
                return;
            }
            else if (creatorGUI.ms2fragmentsForm.tabControlFragments.SelectedIndex == pgIndex && tutorial == Tutorials.TutorialPRM && tutorialStep == 33)
            {
                nextTutorialStep(true);
                return;
            }
            creatorGUI.changeTab((int)currentTab);
            if (creatorGUI.ms2fragmentsForm != null) creatorGUI.ms2fragmentsForm.tabControlFragments.SelectedIndex = currentMS2TabIndex;
            
        }
        
        
        
        
        
        public void textBoxInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 4)
            {
                HashSet<int> expected = new HashSet<int>(){14, 15, 16, 17, 18, 20};
                HashSet<int> carbonCounts = ((PLLipid)creatorGUI.lipidTabList[(int)LipidCategory.PhosphoLipid]).fag1.carbonCounts;
                nextEnabled = carbonCounts != null && carbonCounts.Intersect(expected).Count() == 6;
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == 5)
            {
                HashSet<int> expected = new HashSet<int>(){0, 1};
                HashSet<int> doubleBondCounts = ((PLLipid)creatorGUI.lipidTabList[(int)LipidCategory.PhosphoLipid]).fag1.doubleBondCounts;
                nextEnabled = doubleBondCounts != null && doubleBondCounts.Intersect(expected).Count() == 2;
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == 8)
            {
                nextEnabled = true;
                
                HashSet<int> expectedFA = new HashSet<int>(){8, 9, 10};
                HashSet<int> carbonCounts = ((PLLipid)creatorGUI.lipidTabList[(int)LipidCategory.PhosphoLipid]).fag2.carbonCounts;
                nextEnabled = carbonCounts != null && carbonCounts.Intersect(expectedFA).Count() == 3;
                
                HashSet<int> expectedDB = new HashSet<int>(){2};
                HashSet<int> doubleBondCounts = ((PLLipid)creatorGUI.lipidTabList[(int)LipidCategory.PhosphoLipid]).fag2.doubleBondCounts;
                nextEnabled = nextEnabled && doubleBondCounts != null && doubleBondCounts.Intersect(expectedDB).Count() == 1;
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == 16)
            {
                nextEnabled = (creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName.Text == "testFrag") && (creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.SelectedIndex == 1);
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == 24)
            {
                string lipidClass = (string)creatorGUI.addHeavyPrecursor.comboBox1.Items[creatorGUI.addHeavyPrecursor.comboBox1.SelectedIndex];
                nextEnabled = (creatorGUI.addHeavyPrecursor.textBox1.Text == "13C6d30") && (lipidClass == "PG");
            }
            tutorialWindow.Refresh();
        }
        
        
        
        
        
        
        public void checkBoxInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 9)
            {
                nextEnabled = creatorGUI.plPosAdductCheckbox1.Checked && !creatorGUI.plPosAdductCheckbox3.Checked;
            }
            tutorialWindow.Refresh();
        }
        
        
        
        
        
        public void comboBoxInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 16)
            {
                nextEnabled = (creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName.Text == "testFrag") && (creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.SelectedIndex == 1);
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == 24)
            {
                string lipidClass = (string)creatorGUI.addHeavyPrecursor.comboBox1.Items[creatorGUI.addHeavyPrecursor.comboBox1.SelectedIndex];
                nextEnabled = (creatorGUI.addHeavyPrecursor.textBox1.Text == "13C6d30") && (lipidClass == "PG");
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == 27)
            {
                nextEnabled = creatorGUI.addHeavyPrecursor.comboBox2.SelectedIndex == 1;
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == 34)
            {
                nextEnabled = creatorGUI.ms2fragmentsForm.isotopeList.SelectedIndex == 1;
            }
            tutorialWindow.Refresh();
        }
        
        
        
        
        
        public void tableCellChanged(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            nextEnabled = true;
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 18)
            {
                DataGridView dgv = creatorGUI.ms2fragmentsForm.newFragment.dataGridViewElements;
                Dictionary<string, object[]> elements = creatorGUI.ms2fragmentsForm.newFragment.elements;
                for (int i = 0; i < dgv.Rows.Count; ++i){
                    string key = dgv.Rows[i].Cells[0].Value.ToString();
                    int val = 0;
                    if (key == "H") val = 3;
                    else if (key == "O") val = 2;
                    
                    if ((int)elements[key][0] != val ||(int)elements[key][1] != 0)
                    {
                        nextEnabled = false;
                        break;
                    }
                }
                creatorGUI.ms2fragmentsForm.Refresh();
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == 26)
            {
                DataGridView dgv = creatorGUI.addHeavyPrecursor.dataGridView1;
                for (int i = 0; i < dgv.Rows.Count; ++i){
                    string key = dgv.Rows[i].Cells[0].Value.ToString();
                    int val = 0;
                    if (key == "C") val = 6;
                    
                    if ((int)creatorGUI.addHeavyPrecursor.currentDict[key][1] != val)
                    {
                        nextEnabled = false;
                        break;
                    }
                }
                creatorGUI.addHeavyPrecursor.Refresh();
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == 28)
            {
                DataGridView dgv = creatorGUI.ms2fragmentsForm.newFragment.dataGridViewElements;;
                for (int i = 0; i < dgv.Rows.Count; ++i){
                    string key = dgv.Rows[i].Cells[0].Value.ToString();
                    int val = 0;
                    if (key == "H") val = 12;
                    
                    if ((int)creatorGUI.addHeavyPrecursor.currentDict[key][1] != val)
                    {
                        nextEnabled = false;
                        break;
                    }
                }
                creatorGUI.addHeavyPrecursor.Refresh();
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == 38)
            {
                DataGridView dgv = creatorGUI.ms2fragmentsForm.newFragment.dataGridViewElements;
                Dictionary<string, object[]> elements = creatorGUI.ms2fragmentsForm.newFragment.elements;
                for (int i = 0; i < dgv.Rows.Count; ++i){
                    string key = dgv.Rows[i].Cells[0].Value.ToString();
                    int val_mono = 0;
                    int val_heavy = 0;
                    if (key == "H"){
                        val_mono = 0;
                        val_heavy = 1;
                    }
                    else if (key == "O"){
                        val_mono = 1;
                        val_heavy = 0;
                    }
                        
                    if (((int)elements[key][0] != val_mono) || ((int)elements[key][1] != val_heavy))
                    {
                        nextEnabled = false;
                        break;
                    }
                }
                creatorGUI.ms2fragmentsForm.Refresh();
            }
            tutorialWindow.Refresh();
        }
        
        
        
        
        
        
        public void buttonInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && (new HashSet<int>(new int[]{10, 14, 19, 21, 22, 29, 31, 32, 37, 39, 40}).Contains(tutorialStep)))
            {
                nextTutorialStep(true);
            }
        }
        
        
        
        
        
        public void checkedListBoxInteraction(Object sender, ItemCheckEventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 13)
            {
                 HashSet<string> posFrag = creatorGUI.ms2fragmentsForm.currentLipid.positiveFragments["PG"];
                 HashSet<string> negFrag = creatorGUI.ms2fragmentsForm.currentLipid.negativeFragments["PG"];
                 
                 nextEnabled = (posFrag.Count == 1 && posFrag.Contains("NL(PG)") && negFrag.Count == 2 && negFrag.Contains("FA1") && negFrag.Contains("HG(PG)"));
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == 20)
            {
                 HashSet<string> posFrag = creatorGUI.ms2fragmentsForm.currentLipid.positiveFragments["PG"];
                 HashSet<string> negFrag = creatorGUI.ms2fragmentsForm.currentLipid.negativeFragments["PG"];
                 
                 nextEnabled = (posFrag.Count == 2 && posFrag.Contains("NL(PG)")&& posFrag.Contains("testFrag") && negFrag.Count == 2 && negFrag.Contains("FA1") && negFrag.Contains("HG(PG)"));
            }
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 35)
            {
                 HashSet<string> posFrag = creatorGUI.ms2fragmentsForm.currentLipid.positiveFragments["PG/13C6d30"];
                 HashSet<string> negFrag = creatorGUI.ms2fragmentsForm.currentLipid.negativeFragments["PG/13C6d30"];
                 
                 nextEnabled = (posFrag.Count == 1 && posFrag.Contains("NL(PG)") && negFrag.Count == 2 && negFrag.Contains("FA1") && negFrag.Contains("HG(PG)"));
            }
            creatorGUI.ms2fragmentsForm.Refresh();
        }
        
        
        
        
        
        
        private void closingInteraction(Object sender, FormClosingEventArgs e)
        {
            if (tutorialArrow.Parent != null) tutorialArrow.Parent.Controls.Remove(tutorialArrow);
            if (tutorialWindow.Parent != null) tutorialWindow.Parent.Controls.Remove(tutorialWindow);
            if(e.CloseReason != CloseReason.UserClosing) quitTutorial();
        }
        
        
        public void prepareStep()
        {
            disableEverything();
            nextEnabled = true;
            creatorGUI.Enabled = true;
            tutorialWindow.Visible = false;
            tutorialArrow.Visible = false;
            creatorGUI.Refresh();
            if (creatorGUI.ms2fragmentsForm != null) creatorGUI.ms2fragmentsForm.Refresh();
            if (creatorGUI.addHeavyPrecursor != null) creatorGUI.addHeavyPrecursor.Refresh();
        }
        
        
        public void TutorialPRMStep()
        {
            prepareStep();
            switch(tutorialStep)
            {   
                case 1:
                    changeTab(LipidCategory.NoLipid);
                    
                    tutorialWindow.update(new Size(540, 200), new Point(140, 200), "Click on continue", "Welcome to the first tutorial of LipidCreator. It will guide you interactively through this tool by showing you all necessary steps to create both a transition list and a spectral library for targeted lipidomics.", false);
                    nextEnabled = true;
                    break;
                    
                    
                case 2:
                    changeTab(LipidCategory.NoLipid);
                    TabPage p = (TabPage)creatorGUI.tabList[(int)LipidCategory.PhosphoLipid];
                    tutorialArrow.update(new Point((int)(creatorGUI.tabControl.ItemSize.Width * 2.5), 0), "lt");
                    
                    tutorialWindow.update(new Size(540, 200), new Point(140, 200), "click on 'Phosholipids' tab", "Let's start. LipidCreator offers computation for five lipid categories, namely glycerolipids, phopholipids, sphingolipids, cholesterols and mediators. To go on the lipid assembly form for phopholipids, please click at the 'Phospholipids' tab.");
                    
                    nextEnabled = false;
                    break;
                    
                    
                case 3:
                    changeTab(LipidCategory.PhosphoLipid);
                    
                    ListBox plHG = creatorGUI.plHgListbox;
                    int plHGpg = 0;
                    for (; plHGpg < plHG.Items.Count; ++plHGpg) if (plHG.Items[plHGpg].ToString().Equals("PG")) break;
                    tutorialArrow.update(new Point(plHG.Location.X + plHG.Size.Width, plHG.Location.Y + (int)((plHGpg + 0.5) * plHG.ItemHeight)), "tl");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(460, 200), "Select solely 'PG' headgroup", "Great, phospholipids have multiple headgroups. The user can multiply select them. Notice that when hovering above the headgroups, the according adducts are highlighted. We are interested in phosphatidylglycerol (PG). Please select only PG as headgroup and continue.");
                    
                    creatorGUI.plHgListbox.SelectedItems.Clear();
                    creatorGUI.plHgListbox.Enabled = true;
                    nextEnabled = false;
                    break;
                    
                    
                case 4:
                    changeTab(LipidCategory.PhosphoLipid);
                    TextBox plFA1 = creatorGUI.plFA1Textbox;
                    tutorialArrow.update(new Point(plFA1.Location.X, plFA1.Location.Y + (plFA1.Size.Height >> 1)), "tr");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(460, 200), "Set first fatty acid carbon lengths to '14-18, 20'", "LipidCreator was designed to describe a set of fatty acids (FAs) instead of FA separately. PG contains two FAs. We want to create a transition list of PGs with carbon length of first FA between 14 and 18 and additionally 20. Please type in first FA carbon field '14-18, 20'.");
                                      
                    
                    plFA1.Text = "12 - 15";
                    plFA1.Enabled = true;
                    nextEnabled = false;
                    break;
                    
                    
                case 5:
                    changeTab(LipidCategory.PhosphoLipid);
                    
                    TextBox plDB1 = creatorGUI.plDB1Textbox;
                    tutorialArrow.update(new Point(plDB1.Location.X + plDB1.Size.Width, plDB1.Location.Y + (plDB1.Size.Height >> 1)), "tl");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(60, 200), "Set first double bond occurrences to '0-1'", "Here, one can specify the number of double bonds (DBs) for first FA. We are for example interested zero and one DBs. Please type in first FA double bond field '0-1' or '0,1'.");
                                      
                    
                    plDB1.Text = "0";
                    plDB1.Enabled = true;
                    nextEnabled = false;
                    break;
                    
                    
                case 6:
                    changeTab(LipidCategory.PhosphoLipid);
                    TextBox plHyd1 = creatorGUI.plHydroxyl1Textbox;
                    tutorialArrow.update(new Point(plHyd1.Location.X + (plHyd1.Size.Width >> 1), plHyd1.Location.Y + plHyd1.Size.Height), "lt");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(60, 200), "Continue", "Even more parameters can be set for fatty acids. For instance, up to ten hydroxyl groups can be adjusted to FAs. In this tutorial, we stick to zero hydroxyls.");
                    
                    break;
                    
                    
                case 7:
                    changeTab(LipidCategory.PhosphoLipid);
                    CheckBox plFACheck1 = creatorGUI.plFA1Checkbox1;
                    tutorialArrow.update(new Point(plFACheck1.Location.X, plFACheck1.Location.Y + (plFACheck1.Size.Height >> 1)), "tr");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(460, 200), "Continue", "Additionally, fatty acids with ether bond or with ester bond (plasmenyl and plasmanyl) can be created.");
                    
                    break;
                    
                    
                case 8:
                    changeTab(LipidCategory.PhosphoLipid);
                    TextBox plFA2 = creatorGUI.plFA2Textbox;
                    tutorialArrow.update(new Point(plFA2.Location.X, plFA2.Location.Y + (plFA2.Size.Height >> 1)), "tr");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(460, 200), "Set second FA carbon lengths to '8-10' and DB to '2'", "For the second fatty acid, we are interested in carbon length 8-10 and exactly 2 double bonds. Please make the following adjustments.");
                    
                    plFA2.Text = "12 - 15";
                    plFA2.Enabled = true;
                    creatorGUI.plDB2Textbox.Text = "0";
                    creatorGUI.plDB2Textbox.Enabled = true;
                    nextEnabled = false;
                    break;
                    
                    
                case 9:
                    changeTab(LipidCategory.PhosphoLipid);
                    CheckBox adductP1 = creatorGUI.plPosAdductCheckbox1;
                    GroupBox P1 = creatorGUI.plPositiveAdduct;
                    tutorialArrow.update(new Point(P1.Location.X, P1.Location.Y + (P1.Size.Height >> 1)), "tr");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(60, 200), "Select +H(+) adduct", "Several adducts are possible for selection. By default, for PG only the negative adduct -H(-) is selected. Please select the positive adduct +H(+) and proceed.");
                    
                    
                    adductP1.Checked = false;
                    P1.Enabled = true;
                    adductP1.Enabled = true;
                    break;
                    
                    
                case 10:
                    changeTab(LipidCategory.PhosphoLipid);
                    if (creatorGUI.ms2fragmentsForm != null) creatorGUI.ms2fragmentsForm.Close();
                    
                    Button ms2 = creatorGUI.MS2fragmentsLipidButton;
                    tutorialArrow.update(new Point(ms2.Location.X + (ms2.Size.Width >> 1), ms2.Location.Y), "lb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(460, 200), "Open MS2 fragments dialog", "As next, we will have a deeper look into all MS2 fragments of our selected lipids. Please open the MS2 fragments dialog.");
                    
                    ms2.Enabled = true;
                    nextEnabled = false;
                    break;
                    
                    
                case 11:
                    changeTab(LipidCategory.PhosphoLipid);
                    currentMS2TabIndex = 0;
                    initMS2Form();
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Continue", "In the MS2 fragments dialog you can see all predefined positive and negative fragments for all lipid classes of the according category.", false);
                    
                    break;
                    
                     
                case 12:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(0, (TabPage)creatorGUI.ms2fragmentsForm.tabPages[0]);
                    
                    TabControl ms2tc2 = creatorGUI.ms2fragmentsForm.tabControlFragments;
                    tutorialArrow.update(new Point((int)(ms2tc2.ItemSize.Width * ((pgIndex % 16) + 0.5)), 0), "lt");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Select 'PG' tab", "We want to manually select fragments for PG. Please select the 'PG' tab.");
                    
                    nextEnabled = false;
                    break;
                    
                     
                case 13:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, (TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex]);
                    
                    CheckedListBox negCLB = creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments;
                    tutorialArrow.update(new Point(negCLB.Location.X + negCLB.Size.Width, negCLB.Location.Y + (negCLB.Size.Height >> 1)), "tl");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Select only NL(GP)+, FA1- and HG(PG)- fragments", "A positive and negative list are indicating all predefined fragments for PG. Please select NL(GP) in positive mode and FA1, HG(PG) in negative mode. When hovering over the fragments, a structure of the fragment is displayed.");
                    
                    creatorGUI.ms2fragmentsForm.labelPositiveDeselectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.labelPositiveSelectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.labelNegativeDeselectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.labelNegativeSelectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.Enabled = true;
                    creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.Enabled = true;
                    nextEnabled = false;
                    break;
                    
                
                case 14:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, creatorGUI.ms2fragmentsForm);
                    if (creatorGUI.ms2fragmentsForm.newFragment != null)
                    {
                        creatorGUI.ms2fragmentsForm.newFragment.Close();
                        creatorGUI.ms2fragmentsForm.newFragment = null;
                    }
                    
                    Button ms2fragButton = creatorGUI.ms2fragmentsForm.buttonAddFragment;
                    
                    tutorialArrow.update(new Point(ms2fragButton.Location.X + (ms2fragButton.Size.Width >> 1), ms2fragButton.Location.Y), "lb");
                    ms2fragButton.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Open 'Add fragment' dialog", "It is possible to define new fragments Please click on 'Add fragment' button to open the according dialog.");
                    
                    nextEnabled = false;
                    break;
                    
                    
                case 15:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, creatorGUI.ms2fragmentsForm);
                    
                    initAddFragmentForm();
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Continue", "This form enables to define own fragments. In the current version of this tool, the definition is descriptive. Name, dependent building blocks, polarity and constant elements can be added. Please Continue.", false);
                    break;
                    
                    
                case 16:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, creatorGUI.ms2fragmentsForm);
                    
                    creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName.Enabled = true;
                    creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.Enabled = true;
                    
                    
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Define a name, select a fragment base", "Give the new fragment the name 'testFrag'. Tha fragment can either be fixed or dependent on building blocks as head groups of fatty acids. In this example, the new fragment contains a fatty acid, please select FA1.");
                    nextEnabled = false;
                    break;
                    
                    
                case 17:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, creatorGUI.ms2fragmentsForm);
                    
                    creatorGUI.ms2fragmentsForm.newFragment.numericUpDownCharge.Enabled = true;
                    
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Set the charge", "In the right most upper field, the charge can be set. Please set to +1.");
                    nextEnabled = false;
                    break;
                    
                    
                case 18:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, creatorGUI.ms2fragmentsForm);
                    
                    creatorGUI.ms2fragmentsForm.newFragment.dataGridViewElements.Enabled = true;
                    
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Set hydrogen to 3 and oxygen to 2", "Finally, a constant set of elements can be defined which will be added to the fragment. When fixed base is selected, element numbers can only be positive, otherwise negative counts are also allowed. Please set hydrogen to 3 and oxygen to 2.");
                    nextEnabled = false;
                    break;
                    
                    
                case 19:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, creatorGUI.ms2fragmentsForm);
                    
                    creatorGUI.ms2fragmentsForm.newFragment.addButton.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Add fragment", "To store the new fragment, click on the 'Add' button.");
                    nextEnabled = false;
                    break;
                    
                    
                case 20:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, creatorGUI.ms2fragmentsForm);
                    
                    creatorGUI.ms2fragmentsForm.newFragment = null;
                    CheckedListBox posCLB = creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments;
                    posCLB.Enabled = true;
                    tutorialArrow.update(new Point(posCLB.Location.X + posCLB.Size.Width, posCLB.Location.Y + (posCLB.Size.Height >> 1)), "tl");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 34), "Select new fragment", "Please select the new fragment. By right click you can either edit the fragment or delete it. Only user defined fragments are allowed te be updated or deleted.", false);
                    nextEnabled = false;
                    break;
                    
                    
                case 21:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, creatorGUI.ms2fragmentsForm);
                    
                    Button b = creatorGUI.ms2fragmentsForm.buttonOK;
                    b.Enabled = true;
                    tutorialArrow.update(new Point(b.Location.X + (b.Size.Width >> 1), b.Location.Y), "rb");
                    
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 34), "Click OK", "Please confirm the fragment selection by clicking on the 'OK' button.");
                    nextEnabled = false;
                    break;
                    
                    
                case 22:
                    changeTab(LipidCategory.PhosphoLipid);
                    creatorGUI.Enabled = true;
                    
                    creatorGUI.ms2fragmentsForm = null;
                    Button hli = creatorGUI.addHeavyIsotopeButton;
                    hli.Enabled = true;
                    tutorialArrow.update(new Point(hli.Location.X + (hli.Size.Width >> 1), hli.Location.Y), "lb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Open heavy labeled dialog", "In the next step, we are going to create a heavy labeled isotope of our selected lipid class. Therefore, please open the heavy labeled dialog.", false);
                    
                    nextEnabled = false;
                    break;
                    
                    
                case 23:
                    changeTab(LipidCategory.PhosphoLipid);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Continue", "In the upper part, the mode can be selected either adding a new heavy labeled isotope or edit existing user defined heavy isotopes. Let's create one.", false);
                    
                    break;
                    
                    
                case 24:
                    changeTab(LipidCategory.PhosphoLipid);
                    
                    creatorGUI.addHeavyPrecursor.comboBox1.Enabled = true;
                    creatorGUI.addHeavyPrecursor.textBox1.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Select PG and name it '13C6d30'", "Please select the current lipid class PG and name it with the suffix '13C6d30'.");
                    
                    nextEnabled = false;
                    break;
                    
                    
                case 25:
                    changeTab(LipidCategory.PhosphoLipid);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Continue", "PG has three building blocks. That is the head group and two variable fatty acids. We will edit the head group and the first fatty acid. Please continue for editing the head group.");
                    
                    break;
                    
                    
                case 26:
                    changeTab(LipidCategory.PhosphoLipid);
                    
                    creatorGUI.addHeavyPrecursor.dataGridView1.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Set 13C to 6", "In head group mode, the number of mono isotopic elements is fixed and will be updated, when heavy labeled elements are added. For head group, we want to change the carbons. Please set 13C to 6.");
                    
                    nextEnabled = false;
                    break;
                    
                    
                case 27:
                    changeTab(LipidCategory.PhosphoLipid);
                    creatorGUI.addHeavyPrecursor.comboBox2.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Change building block to 'Fatty Acid 1'", "To continue with the modification, please change the building block to 'Fatty Acid 1'.");
                    
                    nextEnabled = false;
                    break;
                    
                    
                case 28:
                    changeTab(LipidCategory.PhosphoLipid);
                    creatorGUI.addHeavyPrecursor.dataGridView1.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Set 2H to 12", "Since the fatty acid building block has a variable number of elements depending e.g. on the carbon chain length, no fixed mono isotopic element numbers are provided. The heavy labeled elements numbers act as an upper limit for the element. Please set 2H to 12.");
                    
                    nextEnabled = false;
                    break;
                    
                    
                case 29:
                    changeTab(LipidCategory.PhosphoLipid);
                    creatorGUI.addHeavyPrecursor.button2.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Add isotope", "You are adding the heavy isotope by clicking on 'Add isotope'. All fragments of the mono isotopic parent will be copied and are enabled to be updated or deleted.");
                    
                    nextEnabled = false;
                    break;
                    
                    
                case 30:
                    changeTab(LipidCategory.PhosphoLipid);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Continue", "All user defined heavy isotopes can be modified by changing the window mode in the upper part. This function will be not explained in detail.");
                    
                    break;
                    
                    
                case 31:
                    changeTab(LipidCategory.PhosphoLipid);
                    creatorGUI.addHeavyPrecursor.button1.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Close window", "For updating the fragments of the freshly created heavey isotope, please close the window ...", false);
                    
                    nextEnabled = false;
                    break;
                    
                    
                case 32:
                    changeTab(LipidCategory.PhosphoLipid);
                    
                    Button ms2_2 = creatorGUI.MS2fragmentsLipidButton;
                    tutorialArrow.update(new Point(ms2_2.Location.X + (ms2_2.Size.Width >> 1), ms2_2.Location.Y), "lb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Open MS2 fragments dialog", "... and open the MS2 fragments dialog, again.", false);
                    
                    ms2_2.Enabled = true;
                    nextEnabled = false;
                    
                    break;
                    
                    
                case 33:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(0, (TabPage)creatorGUI.ms2fragmentsForm.tabPages[0]);
                    initMS2Form();
                    
                    TabControl ms2tc2_2 = creatorGUI.ms2fragmentsForm.tabControlFragments;
                    tutorialArrow.update(new Point((int)(ms2tc2_2.ItemSize.Width * ((pgIndex % 16) + 0.5)), 0), "lt");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Select 'PG' tab", "Select 'PG' tab, again.");
                    
                    nextEnabled = false;
                    break;
                    
                    
                case 34:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, (TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex]);
                    
                    ComboBox il1 = creatorGUI.ms2fragmentsForm.isotopeList;
                    tutorialArrow.update(new Point(il1.Location.X + (il1.Width >> 1), il1.Location.Y + il1.Height), "lt");
                    il1.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Select the '13C6d30' isotope", "");
                    
                    nextEnabled = false;
                    break;
                    
                    
                case 35:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, (TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex]);
                    
                    CheckedListBox negCLB_2 = creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments;
                    tutorialArrow.update(new Point(negCLB_2.Location.X + negCLB_2.Size.Width, negCLB_2.Location.Y + (negCLB_2.Size.Height >> 1)), "tl");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Select only NL(GP)+, FA1- and HG(PG)- fragments", "");
                    
                    creatorGUI.ms2fragmentsForm.labelPositiveDeselectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.labelPositiveSelectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.labelNegativeDeselectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.labelNegativeSelectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.Enabled = true;
                    creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.Enabled = true;
                    
                    nextEnabled = false;
                    break;
                    
                    
                    
                case 36:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, (TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex]);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Continue", "Since all fragments have a list of constant elements, you have to check for all effected fragments, if your precursor modifications satisfy the fragment moditifactions.");
                    
                    break;
                    
                    
                    
                case 37:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, (TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex]);
                    
                    
                    CheckedListBox negCLB_3 = creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments;
                    tutorialArrow.update(new Point(negCLB_3.Location.X + negCLB_3.Size.Width, negCLB_3.Location.Y + (negCLB_3.Size.Height >> 1)), "tl");
                    creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Right click on FA1- and 'Edit fragment'", "In our case, FA1- has a constant hydrogen which is yet uneffected from being deuterated. To change, please right click on FA1- and click on 'Edit fragment'.");
                    
                    
                    nextEnabled = false;
                    break;
                    
                    
                    
                case 38:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, (TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex]);
                    initAddFragmentForm();
                    
                    creatorGUI.ms2fragmentsForm.newFragment.dataGridViewElements.Enabled = true;
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Set 2H to 1 and H to 0", "", false);
                    
                    nextEnabled = false;
                    break;
                    
                    
                    
                case 39:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, (TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex]);
                    
                    creatorGUI.ms2fragmentsForm.newFragment.addButton.Enabled = true;
                    tutorialWindow.update(new Size(500, 200), new Point(620, 34), "Click 'OK'", "To confirm the update, click 'OK' ...");
                    
                    nextEnabled = false;
                    break;
                    
                    
                    
                case 40:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, creatorGUI.ms2fragmentsForm);
                    
                    Button b_2 = creatorGUI.ms2fragmentsForm.buttonOK;
                    b_2.Enabled = true;
                    tutorialArrow.update(new Point(b_2.Location.X + (b_2.Size.Width >> 1), b_2.Location.Y), "rb");
                    tutorialWindow.update(new Size(500, 200), new Point(620, 34), "Click 'OK'", "... and again 'OK'.", false);
                    
                    nextEnabled = false;
                    break;
                    
                    
                    
                case 41:
                    changeTab(LipidCategory.PhosphoLipid);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(34, 34), "Continue", "continue", false);
                    
                    break;
                    
                    
                case 100:
                    changeTab(LipidCategory.PhosphoLipid);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Dummy", "Dummy");
                    
                    nextEnabled = false;
                    break;
                    
                    
                default:
                    quitTutorial();
                    break;
            }
        }
        
        
        public void TutorialMRMStep()
        {
            prepareStep();
            switch(tutorialStep)
            {   
                case 1:
                    changeTab(LipidCategory.NoLipid);
                    
                    tutorialWindow.update(new Size(540, 200), new Point(140, 200), "Click on continue", "Welcome to the first tutorial of LipidCreator. It will guide you interactively through this tool by showing you all necessary steps to create both a transition list and a spectral library for targeted lipidomics.", false);
                    nextEnabled = true;
                    break;
                    
                default:
                    quitTutorial();
                    break;
            }
        }
        
        
        public void TutorialHeavyLabeledStep()
        {
        
            prepareStep();
            switch(tutorialStep)
            {   
                case 1:
                    changeTab(LipidCategory.NoLipid);
                    
                    tutorialWindow.update(new Size(540, 200), new Point(140, 200), "Click on continue", "Welcome to the first tutorial of LipidCreator. It will guide you interactively through this tool by showing you all necessary steps to create both a transition list and a spectral library for targeted lipidomics.", false);
                    nextEnabled = true;
                    break;
                    
                default:
                    quitTutorial();
                    break;
            }
        }
    }    
}