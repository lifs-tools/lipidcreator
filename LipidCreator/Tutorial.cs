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
                {(int)Tutorials.TutorialPRM, 20},
                {(int)Tutorials.TutorialMRM, 0},
                {(int)Tutorials.TutorialHeavyLabeled, 0}
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
        }
        
        
        
        public void startTutorial(Tutorials t)
        {
            tutorial = t;
            tutorialStep = 13;
            
            
            
            
            // TODO: remove these lines
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
            
            
            
            
            
            nextEnabled = true;
            currentTab = LipidCategory.NoLipid;
            ((TabPage)creatorGUI.tabList[(int)currentTab]).Controls.Add(tutorialArrow);
            ((TabPage)creatorGUI.tabList[(int)currentTab]).Controls.Add(tutorialWindow);
            tutorialArrow.BringToFront();
            tutorialWindow.BringToFront();
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
            foreach (Object element in creatorGUI.controlElements)
            {
                if (element is MenuItem) 
                {
                    ((MenuItem)element).Enabled = false;
                }
                else
                {
                    ((Control)element).Enabled = false;
                }
            }
            tutorialArrow.Visible = false;
            tutorialWindow.Visible = false;
            creatorGUI.Refresh();
            if (creatorGUI.ms2fragmentsForm != null)
            {
                foreach (Control control in creatorGUI.ms2fragmentsForm.controlElements)
                {
                    control.Enabled = false;
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
        }
        
        
        
        public void changeTab(LipidCategory lip)
        {
            if (currentTab != lip)
            {
                currentTab = lip;
                creatorGUI.changeTab((int)currentTab);
                ((TabPage)creatorGUI.tabList[(int)currentTab]).Controls.Add(tutorialArrow);
                ((TabPage)creatorGUI.tabList[(int)currentTab]).Controls.Add(tutorialWindow);
                tutorialArrow.BringToFront();
                tutorialWindow.BringToFront();
            }
            ((TabPage)creatorGUI.tabList[(int)currentTab]).Enabled = true;
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
        }
        
        
        public void listBoxInteraction(object sender, System.EventArgs e)
        {
            ListBox box = (ListBox)sender;
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 3 && box.SelectedItems.Count == 1 && box.SelectedItems[0].ToString().Equals("PG")) nextEnabled = true;
            else nextEnabled = false;
            tutorialWindow.Refresh();
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
                nextEnabled = (creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName.Text != "") && (creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.SelectedIndex == 1);
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
                nextEnabled = (creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName.Text != "") && (creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.SelectedIndex == 1);
            }
            tutorialWindow.Refresh();
        }
        
        
        public void buttonInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && (tutorialStep == 10 || tutorialStep == 14))
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
            creatorGUI.ms2fragmentsForm.Refresh();
        }
        
        
        
        private void closingInteraction(Object sender, FormClosingEventArgs e)
        {
            quitTutorial();
        }
        
        
        
        public void TutorialPRMStep()
        {
            disableEverything();
            nextEnabled = true;
            creatorGUI.Enabled = true;
            tutorialWindow.Visible = false;
            tutorialArrow.Visible = false;
            creatorGUI.Refresh();
            if (creatorGUI.ms2fragmentsForm != null) creatorGUI.ms2fragmentsForm.Refresh();
            switch(tutorialStep)
            {   
                case 1:
                    changeTab(LipidCategory.NoLipid);
                    
                    tutorialWindow.update(new Size(540, 200), new Point(140, 200), "Click on continue", "Welcome to the first tutorial of LipidCreator. It will guide you interactively through this tool by showing you all necessary steps to create both a transition list and a spectral library for targeted lipidomics.");
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
                    pgIndex = 0;
                    TabControl ms2tc = creatorGUI.ms2fragmentsForm.tabControlFragments;
                    for (; pgIndex < ms2tc.TabPages.Count; ++pgIndex)
                    {
                        if (ms2tc.TabPages[pgIndex].Text.Equals("PG")) break;
                    } 
                    changeMS2Tab(0, (TabPage)creatorGUI.ms2fragmentsForm.tabPages[0]);
                    
                    creatorGUI.Enabled = false;
                    creatorGUI.ms2fragmentsForm.FormClosing += new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
                    ms2tc.SelectedIndexChanged += new System.EventHandler(tabInteraction);
                    creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(checkedListBoxInteraction);
                    creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(checkedListBoxInteraction);
                    creatorGUI.ms2fragmentsForm.buttonAddFragment.Click += buttonInteraction;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Continue", "In the MS2 fragments dialog you can see all predefined positive and negative fragments for all lipid classes of the according category.");
                    
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
                    
                    creatorGUI.ms2fragmentsForm.newFragment.FormClosing += new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Continue", "This form enables to define own fragments. In the current version of this tool, the definition is descriptive. Name, dependent building blocks, polarity and constant elements can be added. Please Continue.");
                    break;
                    
                    
                case 16:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, creatorGUI.ms2fragmentsForm);
                    
                    creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName.Enabled = true;
                    creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.Enabled = true;
                    
                    creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName.TextChanged += new EventHandler(textBoxInteraction);
                    creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.SelectedIndexChanged += new EventHandler(comboBoxInteraction);
                    
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Define a name, select a fragment base", "Give the new fragment an arbitrary name. Tha fragment can either be fixed or dependent on building blocks as head groups of fatty acids. In this example, the new fragment contains a fatty acid, please select FA1.");
                    nextEnabled = false;
                    break;
                    
                    
                case 17:
                    changeTab(LipidCategory.PhosphoLipid);
                    changeMS2Tab(pgIndex, creatorGUI.ms2fragmentsForm);
                    
                    creatorGUI.ms2fragmentsForm.newFragment.numericUpDownCharge.Enabled = true;
                    
                    creatorGUI.ms2fragmentsForm.newFragment.numericUpDownCharge.ValueChanged += new EventHandler(numericInteraction);
                    
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Set the charge", "Accordingly, the constant elements can be either added or subtracted from the fragment. We keep adding the fragments. Please set the charge to -1.");
                    nextEnabled = false;
                    break;
                    
                    
                default:
                    quitTutorial();
                    break;
            }
        }
        
        
        public void TutorialMRMStep()
        {
        
        }
        
        
        public void TutorialHeavyLabeledStep()
        {
        
        }
    }    
}