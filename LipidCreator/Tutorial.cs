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
using System.Linq;
using System.Windows.Forms;

using log4net;
using log4net.Config;



namespace LipidCreator
{
    
    public enum Tutorials {NoTutorial = -1, TutorialPRM = 0, TutorialSRM = 1, TutorialHL = 2, TutorialCE = 3};
    
    public enum PRMSteps {Null, Welcome, PhosphoTab, PGheadgroup, SetFA, SetDB, MoreParameters, RepresentitativeFA, Ether, SecondFADB, SelectAdduct, OpenFilter, SelectFilter, AddLipid, OpenInterlist, ExplainInterlist, OpenReview, StoreList, Finish};
    
    public enum SRMSteps {Null, Welcome, PhosphoTab, OpenMS2, InMS2, SelectPG, SelectFragments, AddFragment, InFragment, NameFragment, SetCharge, SetElements, AddingFragment, SelectNew, ClickOK, AddLipid, OpenInterlist, OpenReview, StoreList, Finish};
    
    public enum HLSteps {Null, Welcome, OpenHeavy, HeavyPanel, NameHeavy, OptionsExplain, SetElements, ChangeBuildingBlock, SetElements2, AddIsotope, EditExplain, CloseHeavy, OpenMS2, SelectPG, SelectHeavy, SelectFragments, CheckFragment, EditFragment, SetFragElement, ConfirmEdit, CloseFragment, OpenFilter, SelectFilter, AddLipid, OpenInterlist, OpenReview, StoreList, Finish};
    
    public enum CESteps {Null, Welcome, ActivateCE, OpenCEDialog, SelectTXB2, ExplainBlackCurve, ChangeManually, CEto20, SameForD4, CloseCE, ChangeToMediators, SelectTXB2HG, AddLipid, OpenInterlist, ReviewLipids, ExplainLCasExternal, StoreBlib, Finish};
    

    [Serializable]
    public class Tutorial
    {
        public Tutorials tutorial;
        public int tutorialStep;
        public CreatorGUI creatorGUI;
        public ArrayList elementsEnabledState;
        public Dictionary<int, int> maxSteps;
        public bool nextEnabled;
        public int pgIndex = 0;
        public int currentTabIndex = 0;
        [NonSerialized]
        public Overlay tutorialArrow;
        [NonSerialized]
        public TutorialWindow tutorialWindow;
        [NonSerialized]
        public System.Timers.Timer timer;
        public ArrayList creatorGUIEventHandlers;
        public bool continueTutorial = false;
        public bool passTabChange = false;
        public bool quitting = false;
        private static readonly ILog log = LogManager.GetLogger(typeof(Tutorial));
        
        public Tutorial(CreatorGUI creatorGUI)
        {
            tutorial = Tutorials.NoTutorial;
            tutorialStep = 0;
            nextEnabled = true;
            creatorGUIEventHandlers = new ArrayList();
            this.creatorGUI = creatorGUI;
            maxSteps = new Dictionary<int, int>(){
                {(int)Tutorials.NoTutorial, 0},
                {(int)Tutorials.TutorialPRM, Enum.GetNames(typeof(PRMSteps)).Length - 1},
                {(int)Tutorials.TutorialSRM, Enum.GetNames(typeof(SRMSteps)).Length - 1},
                {(int)Tutorials.TutorialHL, Enum.GetNames(typeof(HLSteps)).Length - 1},
                {(int)Tutorials.TutorialCE, Enum.GetNames(typeof(CESteps)).Length - 1}
            };
            tutorialArrow = new Overlay(creatorGUI.lipidCreator.prefixPath);
            tutorialWindow = new TutorialWindow(this, creatorGUI.lipidCreator.prefixPath);
            tutorialArrow.Visible = false;
            tutorialWindow.Visible = false;
            
            
           
        }
        
        
        public void startTutorial(Tutorials t)
        {
            if (!creatorGUI.resetLipidCreator()) return;
            
            tutorial = t;
            tutorialStep = 0;
            quitting = false;
            
            creatorGUI.plHgListbox.SelectedValueChanged += new EventHandler(listBoxInteraction);
            creatorGUI.medHgListbox.SelectedValueChanged += new EventHandler(listBoxInteraction);
            creatorGUI.tabControl.Deselecting += new TabControlCancelEventHandler(tabDeselectingInteraction);
            creatorGUI.tabControl.MouseMove += new MouseEventHandler(dragInteraction);
            creatorGUI.tabControl.SelectedIndexChanged += new EventHandler(tabSelectedInteraction);
            creatorGUI.plFA1Textbox.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.plDB1Textbox.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.plFA2Textbox.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.plDB2Textbox.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.plPosAdductCheckbox1.CheckedChanged += new EventHandler(checkBoxInteraction);
            creatorGUI.plPosAdductCheckbox3.CheckedChanged += new EventHandler(checkBoxInteraction);
            creatorGUI.MS2fragmentsLipidButton.MouseUp += new MouseEventHandler(buttonInteraction);
            creatorGUI.medPictureBox.ImageChanged += new EventHandler(mouseHoverInteraction);
            creatorGUI.addLipidButton.Click += new EventHandler(buttonInteraction);
            creatorGUI.addHeavyIsotopeButton.Click += new EventHandler(buttonInteraction);
            creatorGUI.openReviewFormButton.Click += new EventHandler(buttonInteraction);
            creatorGUI.filtersButton.Click += new EventHandler(buttonInteraction);
            creatorGUI.menuCollisionEnergyOpt.Click += new EventHandler(buttonInteraction);
            foreach (MenuItem menuItem in creatorGUI.menuCollisionEnergy.MenuItems)
            {
                menuItem.Click += new EventHandler(buttonInteraction);
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
            ms2tc.SelectedIndex = 0;
            ms2tc.MouseMove += new MouseEventHandler(dragInteraction);
            ms2tc.Deselecting += new TabControlCancelEventHandler(tabDeselectingInteraction);
            ms2tc.SelectedIndexChanged += new EventHandler(tabSelectedInteraction);
            
            
            creatorGUI.ms2fragmentsForm.FormClosing += new FormClosingEventHandler(closingInteraction);
            creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.ItemCheck += new ItemCheckEventHandler(checkedListBoxInteraction);
            creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.ItemCheck += new ItemCheckEventHandler(checkedListBoxInteraction);
            creatorGUI.ms2fragmentsForm.buttonAddFragment.Click += buttonInteraction;
            creatorGUI.ms2fragmentsForm.buttonOK.Click += buttonInteraction;
            creatorGUI.ms2fragmentsForm.buttonOK.MouseDown += mouseDownInteraction;
            creatorGUI.ms2fragmentsForm.isotopeList.SelectedIndexChanged += new EventHandler(comboBoxInteraction);
            creatorGUI.ms2fragmentsForm.contextMenuFragment.Popup += new EventHandler(contextMenuPopupInteraction);
            creatorGUI.ms2fragmentsForm.menuFragmentItem1.Click += new EventHandler(buttonInteraction);
            creatorGUI.ms2fragmentsForm.menuFragmentItem2.Click += new EventHandler(buttonInteraction);
            creatorGUI.ms2fragmentsForm.pictureBoxFragments.ImageChanged += new EventHandler(mouseHoverInteraction);
            creatorGUI.ms2fragmentsForm.Refresh();
        }
        
        
        public void mouseHoverInteraction(object sender, EventArgs e)
        {
            //tutorialArrow.BringToFront();
            tutorialArrow.Refresh();
        }
        
        
        
        
        
        public void initAddFragmentForm()
        {
            creatorGUI.ms2fragmentsForm.newFragment.FormClosing += new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
            creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.SelectedIndexChanged += new EventHandler(comboBoxInteraction);
            creatorGUI.ms2fragmentsForm.newFragment.addButton.Click += buttonInteraction;
            creatorGUI.ms2fragmentsForm.newFragment.addButton.MouseDown += mouseDownInteraction;
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
            creatorGUI.addHeavyPrecursor.button1.MouseDown += mouseDownInteraction;
            creatorGUI.addHeavyPrecursor.button1.Click += buttonInteraction;
            creatorGUI.addHeavyPrecursor.button2.Click += buttonInteraction;
        }
        
        
        
        
        
        
        public void initInterList()
        {
            creatorGUI.lipidsInterList.FormClosing += new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
            creatorGUI.lipidsInterList.continueReviewButton.Click += buttonInteraction;
            creatorGUI.lipidsInterList.continueReviewButton.MouseDown += mouseDownInteraction;
        }
        
        
        public void initFilterDialog()
        {
            creatorGUI.filterDialog.FormClosing += new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
            creatorGUI.filterDialog.button2.MouseDown += mouseDownInteraction;
            creatorGUI.filterDialog.button2.Click += buttonInteraction;
            creatorGUI.filterDialog.radioButton1.CheckedChanged += new EventHandler(radioButtonInteraction);
            creatorGUI.filterDialog.radioButton2.CheckedChanged += new EventHandler(radioButtonInteraction);
            creatorGUI.filterDialog.radioButton3.CheckedChanged += new EventHandler(radioButtonInteraction);
            creatorGUI.filterDialog.radioButton4.CheckedChanged += new EventHandler(radioButtonInteraction);
            creatorGUI.filterDialog.radioButton5.CheckedChanged += new EventHandler(radioButtonInteraction);
            creatorGUI.filterDialog.radioButton6.CheckedChanged += new EventHandler(radioButtonInteraction);
        }
        
        
        
        public void initLipidReview()
        {
            creatorGUI.lipidsReview.buttonStoreTransitionList.Click += buttonInteraction;
            creatorGUI.lipidsReview.buttonStoreSpectralLibrary.Click += buttonInteraction;
            creatorGUI.lipidsReview.FormClosing += new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
        }
        
        
        public void initCEInspector()
        {
            creatorGUI.ceInspector.fragmentsGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(tableCellChanged);
            creatorGUI.ceInspector.radioButtonPRMArbitrary.CheckedChanged += new EventHandler(radioButtonInteraction);
            creatorGUI.ceInspector.button2.Click += buttonInteraction;
            creatorGUI.ceInspector.button2.MouseDown += mouseDownInteraction;
            creatorGUI.ceInspector.cartesean.MouseMove += new MouseEventHandler(mouseHoverInteraction);
            creatorGUI.ceInspector.numericalUpDownCurrentCE.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.ceInspector.classCombobox.SelectedIndexChanged += new EventHandler(comboBoxInteraction);
            creatorGUI.ceInspector.FormClosing += new System.Windows.Forms.FormClosingEventHandler(closingInteraction);            
        }
        
        
        
        public void nextTutorialStep(bool forward)
        {
            tutorialStep = forward ? (tutorialStep + 1) : (Math.Max(tutorialStep - 1, 1));
            if (tutorial == Tutorials.TutorialPRM) TutorialPRMStep();
            else if (tutorial == Tutorials.TutorialSRM) TutorialSRMStep();
            else if (tutorial == Tutorials.TutorialHL) TutorialHLStep();
            else if (tutorial == Tutorials.TutorialCE) TutorialCEStep();
            else quitTutorial(true);
        }
        
        
        
        public void quitTutorial(bool userDefined = false)
        {
            if (quitting) return;
            quitting = true;
            tutorial = Tutorials.NoTutorial;
            tutorialStep = 0;
            tutorialArrow.Visible = false;
            tutorialWindow.Visible = false;
            
            if (tutorialArrow.Parent != null) tutorialArrow.Parent.Controls.Remove(tutorialArrow);
            if (tutorialWindow.Parent != null) tutorialWindow.Parent.Controls.Remove(tutorialWindow);
            
            creatorGUI.plHgListbox.SelectedValueChanged -= new EventHandler(listBoxInteraction);
            creatorGUI.medHgListbox.SelectedValueChanged -= new EventHandler(listBoxInteraction);
            creatorGUI.tabControl.MouseMove -= new MouseEventHandler(dragInteraction);
            creatorGUI.tabControl.Deselecting -= new TabControlCancelEventHandler(tabDeselectingInteraction);
            creatorGUI.tabControl.SelectedIndexChanged -= new EventHandler(tabSelectedInteraction);
            creatorGUI.plFA1Textbox.TextChanged -= new EventHandler(textBoxInteraction);
            creatorGUI.plDB1Textbox.TextChanged -= new EventHandler(textBoxInteraction);
            creatorGUI.plFA2Textbox.TextChanged -= new EventHandler(textBoxInteraction);
            creatorGUI.plDB2Textbox.TextChanged -= new EventHandler(textBoxInteraction);
            creatorGUI.medPictureBox.ImageChanged -= new EventHandler(mouseHoverInteraction);
            creatorGUI.plPosAdductCheckbox1.CheckedChanged -= new EventHandler(checkBoxInteraction);
            creatorGUI.plPosAdductCheckbox3.CheckedChanged -= new EventHandler(checkBoxInteraction);
            creatorGUI.MS2fragmentsLipidButton.Click -= new EventHandler(buttonInteraction);
            creatorGUI.addLipidButton.Click -= new EventHandler(buttonInteraction);
            creatorGUI.addHeavyIsotopeButton.Click -= new EventHandler(buttonInteraction);
            creatorGUI.openReviewFormButton.Click -= new EventHandler(buttonInteraction);
            creatorGUI.filtersButton.Click -= new EventHandler(buttonInteraction);
            creatorGUI.menuCollisionEnergyOpt.Click -= new EventHandler(buttonInteraction);
            foreach (MenuItem menuItem in creatorGUI.menuCollisionEnergy.MenuItems)
            {
                menuItem.Click -= new EventHandler(buttonInteraction);
            }
            
            if (creatorGUI.lipidsReview != null)
            {
                creatorGUI.lipidsReview.buttonStoreTransitionList.Click -= buttonInteraction;
                creatorGUI.lipidsReview.buttonStoreSpectralLibrary.Click -= buttonInteraction;
                creatorGUI.lipidsReview.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
                creatorGUI.lipidsReview.Close();
            }
            
            if (creatorGUI.ms2fragmentsForm != null)
            {
                creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.ItemCheck -= new System.Windows.Forms.ItemCheckEventHandler(checkedListBoxInteraction);
                creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.ItemCheck -= new System.Windows.Forms.ItemCheckEventHandler(checkedListBoxInteraction);
                creatorGUI.ms2fragmentsForm.buttonAddFragment.Click -= buttonInteraction;
                creatorGUI.ms2fragmentsForm.buttonOK.Click -= buttonInteraction;
                creatorGUI.ms2fragmentsForm.buttonOK.MouseDown -= mouseDownInteraction;
                creatorGUI.ms2fragmentsForm.isotopeList.SelectedIndexChanged -= new EventHandler(comboBoxInteraction);
                creatorGUI.ms2fragmentsForm.contextMenuFragment.Popup -= new System.EventHandler(contextMenuPopupInteraction);
                creatorGUI.ms2fragmentsForm.menuFragmentItem1.Click -= new System.EventHandler(buttonInteraction);
                creatorGUI.ms2fragmentsForm.menuFragmentItem2.Click -= new System.EventHandler(buttonInteraction);
                
                if (creatorGUI.ms2fragmentsForm.newFragment != null)
                {
                    creatorGUI.ms2fragmentsForm.newFragment.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
                    creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName.TextChanged -= new EventHandler(textBoxInteraction);
                    creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.SelectedIndexChanged -= new EventHandler(comboBoxInteraction);
                    creatorGUI.ms2fragmentsForm.newFragment.addButton.Click -= buttonInteraction;
                    creatorGUI.ms2fragmentsForm.newFragment.numericUpDownCharge.ValueChanged -= new EventHandler(numericInteraction);
                    creatorGUI.ms2fragmentsForm.newFragment.dataGridViewElements.CellValueChanged -= new System.Windows.Forms.DataGridViewCellEventHandler(tableCellChanged);
                    creatorGUI.ms2fragmentsForm.newFragment.Close();
                }
                creatorGUI.ms2fragmentsForm.Close();
                
            }
            if (creatorGUI.lipidsInterList != null)
            {
                creatorGUI.lipidsInterList.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
                creatorGUI.lipidsInterList.continueReviewButton.Click -= buttonInteraction;
                creatorGUI.lipidsInterList.continueReviewButton.MouseDown -= mouseDownInteraction;
                creatorGUI.lipidsInterList.Close();
            }
            
            if (creatorGUI.ceInspector != null)
            {
                creatorGUI.ceInspector.fragmentsGridView.CellValueChanged -= new System.Windows.Forms.DataGridViewCellEventHandler(tableCellChanged);
                creatorGUI.ceInspector.radioButtonPRMArbitrary.CheckedChanged -= new EventHandler(radioButtonInteraction);
                creatorGUI.ceInspector.button2.Click -= buttonInteraction;
                creatorGUI.ceInspector.button2.MouseDown -= mouseDownInteraction;
                creatorGUI.ceInspector.numericalUpDownCurrentCE.TextChanged -= new EventHandler(textBoxInteraction);
                creatorGUI.ceInspector.classCombobox.SelectedIndexChanged -= new EventHandler(comboBoxInteraction);
                creatorGUI.ceInspector.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(closingInteraction); 
                creatorGUI.ceInspector.Close();
            }
            
            if (creatorGUI.filterDialog != null)
            {
                creatorGUI.filterDialog.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
                creatorGUI.filterDialog.button2.MouseDown -= mouseDownInteraction;
                creatorGUI.filterDialog.button2.Click -= buttonInteraction;
                creatorGUI.filterDialog.radioButton1.CheckedChanged -= new EventHandler(radioButtonInteraction);
                creatorGUI.filterDialog.radioButton2.CheckedChanged -= new EventHandler(radioButtonInteraction);
                creatorGUI.filterDialog.radioButton3.CheckedChanged -= new EventHandler(radioButtonInteraction);
                creatorGUI.filterDialog.radioButton4.CheckedChanged -= new EventHandler(radioButtonInteraction);
                creatorGUI.filterDialog.radioButton5.CheckedChanged -= new EventHandler(radioButtonInteraction);
                creatorGUI.filterDialog.radioButton6.CheckedChanged -= new EventHandler(radioButtonInteraction);
                creatorGUI.filterDialog.Close();
            }
            
            if (creatorGUI.addHeavyPrecursor != null)
            {
                creatorGUI.addHeavyPrecursor.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(closingInteraction);
                creatorGUI.addHeavyPrecursor.comboBox1.SelectedIndexChanged -= new EventHandler(comboBoxInteraction);
                creatorGUI.addHeavyPrecursor.comboBox2.SelectedIndexChanged -= new EventHandler(comboBoxInteraction);
                creatorGUI.addHeavyPrecursor.textBox1.TextChanged -= new EventHandler(textBoxInteraction);
                creatorGUI.addHeavyPrecursor.dataGridView1.CellValueChanged -= new System.Windows.Forms.DataGridViewCellEventHandler(tableCellChanged);
                creatorGUI.addHeavyPrecursor.button1.Click -= buttonInteraction;
                creatorGUI.addHeavyPrecursor.button2.Click -= buttonInteraction;
                creatorGUI.addHeavyPrecursor.Close();
            }
            
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
            if (creatorGUI.ms2fragmentsForm != null && !userDefined)
            {
                if (creatorGUI.ms2fragmentsForm.newFragment != null) creatorGUI.ms2fragmentsForm.newFragment.Close();
                creatorGUI.ms2fragmentsForm.Close();
            }
            if (creatorGUI.addHeavyPrecursor != null && !userDefined)
            {
                creatorGUI.addHeavyPrecursor.Close();
            }
            if (creatorGUI.lipidsReview != null && !userDefined)
            {
                creatorGUI.lipidsReview.Close();
            }
            creatorGUI.Enabled = true;
            quitting = false;
        }
        
        
        
        public void disableEverything()
        {
            if (tutorialArrow.Parent != null) tutorialArrow.Parent.Controls.Remove(tutorialArrow);
            if (tutorialWindow.Parent != null) tutorialWindow.Parent.Controls.Remove(tutorialWindow);
            continueTutorial = false;
            
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
                    foreach (Control control in newFrag.controlElements) control.Enabled = false;
                    creatorGUI.ms2fragmentsForm.Refresh();
                }
            }
            
            if (creatorGUI.addHeavyPrecursor != null)
            {
                foreach (Control control in creatorGUI.addHeavyPrecursor.controlElements) control.Enabled = false;
                creatorGUI.addHeavyPrecursor.Refresh();
            }
            
            if (creatorGUI.lipidsReview != null)
            {
                foreach (Control control in creatorGUI.lipidsReview.controlElements) control.Enabled = false;
                creatorGUI.lipidsReview.Refresh();
            }
            
            if (creatorGUI.ceInspector != null)
            {
                foreach (Control control in creatorGUI.ceInspector.controlElements) control.Enabled = false;
                creatorGUI.ceInspector.Refresh();
            }
            
            if (creatorGUI.filterDialog != null)
            {
                foreach (Control control in creatorGUI.filterDialog.controlElements) control.Enabled = false;
                creatorGUI.filterDialog.Refresh();
            }
            
            if (creatorGUI.lipidsInterList != null)
            {
                foreach (Control control in creatorGUI.lipidsInterList.controlElements) control.Enabled = false;
                creatorGUI.lipidsInterList.Refresh();
            }
        }
        
        
        public void setTutorialControls(Control controlForArrow, Control controlForWindow = null)
        {
            if (controlForWindow == null) controlForWindow = controlForArrow;
                
            controlForWindow.Controls.Add(tutorialWindow);
            controlForArrow.Controls.Add(tutorialArrow);
        }
        
        
        
        
        public void listBoxInteraction(object sender, System.EventArgs e)
        {
            ListBox box = (ListBox)sender;
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == (int)PRMSteps.PGheadgroup && box.SelectedItems.Count == 1 && box.SelectedItems[0].ToString().Equals("PG"))
            {
                nextEnabled = true;
            }
            else if (tutorial == Tutorials.TutorialCE && tutorialStep == (int)CESteps.SelectTXB2HG && box.SelectedItems.Count == 1 && box.SelectedItems[0].ToString().Equals("TXB2"))
            {
                nextEnabled = true;
            }
            else
            {
                nextEnabled = false;
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        
        
        
        public void contextMenuPopupInteraction(Object sender, EventArgs e)
        {
            creatorGUI.ms2fragmentsForm.menuFragmentItem1.Enabled = false;
            creatorGUI.ms2fragmentsForm.menuFragmentItem2.Enabled = false;
            if (tutorial == Tutorials.TutorialHL && tutorialStep == (int)HLSteps.EditFragment && creatorGUI.ms2fragmentsForm.editDeleteIndex == 0)
            {
                creatorGUI.ms2fragmentsForm.menuFragmentItem1.Enabled = true;
            }
            creatorGUI.ms2fragmentsForm.Refresh();
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        
        
        
        public void numericInteraction(object sender, System.EventArgs e)
        {
            MyNumericUpDown numericUpDown = (MyNumericUpDown)sender;
            if (tutorial == Tutorials.TutorialSRM && tutorialStep == (int)SRMSteps.SetCharge)
            {
                nextEnabled = numericUpDown.Value == 1;
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        
        public void tabDeselectingInteraction(Object sender, TabControlCancelEventArgs e)
        {
            if (tutorial != Tutorials.NoTutorial)
            {
                if (currentTabIndex == (int)LipidCategory.Glycerophospholipid && tutorial == Tutorials.TutorialPRM && tutorialStep == (int)PRMSteps.PhosphoTab)
                {
                    return;
                }
                else if (currentTabIndex == pgIndex && tutorial == Tutorials.TutorialSRM && tutorialStep == (int)SRMSteps.SelectPG)
                {
                    return;
                }
                else if (currentTabIndex == (int)LipidCategory.Glycerophospholipid && tutorial == Tutorials.TutorialSRM && tutorialStep == (int)SRMSteps.PhosphoTab)
                {
                    return;
                }
                else if (currentTabIndex == pgIndex && tutorial == Tutorials.TutorialHL && tutorialStep == (int)HLSteps.SelectPG)
                {
                    return;
                }
                else if (currentTabIndex == (int)LipidCategory.Glycerophospholipid && tutorial == Tutorials.TutorialHL && tutorialStep == (int)HLSteps.OpenHeavy)
                {
                    return;
                }
                else if (currentTabIndex == (int)LipidCategory.LipidMediator && tutorial == Tutorials.TutorialCE && tutorialStep == (int)CESteps.ChangeToMediators)
                {
                    return;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        public void dragInteraction(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < ((TabControl)sender).TabCount; ++i)
            {  
                if (((TabControl)sender).GetTabRect(i).Contains(e.Location))
                {
                    currentTabIndex = i;
                    break;
                }
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        
        public void tabSelectedInteraction(Object sender,  EventArgs e)
        {
            // these exceptions should only be executed, when the tabs are being changed by the tutorial and not by the user clicking at a certain tab
            if (
                (currentTabIndex == (int)LipidCategory.Glycerophospholipid && tutorial == Tutorials.TutorialSRM && tutorialStep == (int)SRMSteps.PhosphoTab) ||
                (currentTabIndex == (int)LipidCategory.Glycerophospholipid && tutorial == Tutorials.TutorialHL && tutorialStep == (int)HLSteps.OpenHeavy) ||
                (currentTabIndex == (int)LipidCategory.LipidMediator && tutorial == Tutorials.TutorialCE && tutorialStep == (int)CESteps.SelectTXB2HG)
                )
            {
                return;
            }
            nextTutorialStep(true);
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        
        
        public void textBoxInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == (int)PRMSteps.SetFA)
            {
                HashSet<int> expected = new HashSet<int>(){14, 15, 16, 17, 18, 20};
                HashSet<int> carbonCounts = ((Phospholipid)creatorGUI.lipidTabList[(int)LipidCategory.Glycerophospholipid]).fag1.carbonCounts;
                nextEnabled = carbonCounts != null && carbonCounts.Intersect(expected).Count() == 6;
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == (int)PRMSteps.SetDB)
            {
                HashSet<int> expected = new HashSet<int>(){0, 1};
                HashSet<int> doubleBondCounts = ((Phospholipid)creatorGUI.lipidTabList[(int)LipidCategory.Glycerophospholipid]).fag1.doubleBondCounts;
                nextEnabled = doubleBondCounts != null && doubleBondCounts.Intersect(expected).Count() == 2;
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == (int)PRMSteps.SecondFADB)
            {
                nextEnabled = true;
                
                HashSet<int> expectedFA = new HashSet<int>(){8, 9, 10};
                HashSet<int> carbonCounts = ((Phospholipid)creatorGUI.lipidTabList[(int)LipidCategory.Glycerophospholipid]).fag2.carbonCounts;
                nextEnabled = carbonCounts != null && carbonCounts.Intersect(expectedFA).Count() == 3;
                
                HashSet<int> expectedDB = new HashSet<int>(){2};
                HashSet<int> doubleBondCounts = ((Phospholipid)creatorGUI.lipidTabList[(int)LipidCategory.Glycerophospholipid]).fag2.doubleBondCounts;
                nextEnabled = nextEnabled && doubleBondCounts != null && doubleBondCounts.Intersect(expectedDB).Count() == 1;
            }
            else if (tutorial == Tutorials.TutorialSRM && tutorialStep == (int)SRMSteps.NameFragment)
            {
                nextEnabled = (creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName.Text == "testFrag") && (creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.SelectedIndex == 1);
            }
            else if (tutorial == Tutorials.TutorialHL && tutorialStep == (int)HLSteps.NameHeavy)
            {
                string lipidClass = (string)creatorGUI.addHeavyPrecursor.comboBox1.Items[creatorGUI.addHeavyPrecursor.comboBox1.SelectedIndex];
                nextEnabled = (creatorGUI.addHeavyPrecursor.textBox1.Text == "13C6d30") && (lipidClass == "PG");
            }
            else if (tutorial == Tutorials.TutorialCE && (tutorialStep == (int)CESteps.CEto20 || tutorialStep == (int)CESteps.SameForD4))
            {
                string ceValue = creatorGUI.ceInspector.numericalUpDownCurrentCE.Text;
                if (ceValue != "")
                {
                    nextEnabled = Convert.ToDouble(ceValue) == 20.0;
                }
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        
        
        
        
        public void radioButtonInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == (int)PRMSteps.SelectFilter)
            {
                creatorGUI.filterDialog.button2.Enabled = creatorGUI.filterDialog.radioButton2.Checked;
                creatorGUI.filterDialog.Refresh();
            }
            else if (tutorial == Tutorials.TutorialHL && tutorialStep == (int)HLSteps.SelectFilter)
            {
                creatorGUI.filterDialog.button2.Enabled = creatorGUI.filterDialog.radioButton5.Checked;
                creatorGUI.filterDialog.Refresh();
            }
            else if (tutorial == Tutorials.TutorialCE && tutorialStep == (int)CESteps.ChangeManually)
            {
                nextEnabled = creatorGUI.ceInspector.radioButtonPRMArbitrary.Checked;
                tutorialWindow.Refresh();
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        
        
        public void checkBoxInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == (int)PRMSteps.SelectAdduct)
            {
                nextEnabled = creatorGUI.plPosAdductCheckbox1.Checked && !creatorGUI.plPosAdductCheckbox3.Checked;
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        
        
        
        public void comboBoxInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialSRM && tutorialStep == (int)SRMSteps.NameFragment)
            {
                nextEnabled = (creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName.Text == "testFrag") && (creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.SelectedIndex == 1);
            }
            else if (tutorial == Tutorials.TutorialHL && tutorialStep == (int)HLSteps.NameHeavy)
            {
                string lipidClass = (string)creatorGUI.addHeavyPrecursor.comboBox1.Items[creatorGUI.addHeavyPrecursor.comboBox1.SelectedIndex];
                nextEnabled = (creatorGUI.addHeavyPrecursor.textBox1.Text == "13C6d30") && (lipidClass == "PG");
            }
            else if (tutorial == Tutorials.TutorialHL && tutorialStep == (int)HLSteps.ChangeBuildingBlock)
            {
                nextEnabled = creatorGUI.addHeavyPrecursor.comboBox2.SelectedIndex == 1;
            }
            else if (tutorial == Tutorials.TutorialHL && tutorialStep == (int)HLSteps.SelectHeavy)
            {
                nextEnabled = creatorGUI.ms2fragmentsForm.isotopeList.SelectedIndex == 1;
            }
            else if (tutorial == Tutorials.TutorialCE && tutorialStep == (int)CESteps.SelectTXB2)
            {
                nextEnabled = (string)creatorGUI.ceInspector.classCombobox.Items[creatorGUI.ceInspector.classCombobox.SelectedIndex] == "TXB2";
            }
            else if (tutorial == Tutorials.TutorialCE && tutorialStep == (int)CESteps.SameForD4)
            {
                creatorGUI.ceInspector.numericalUpDownCurrentCE.Enabled = (string)creatorGUI.ceInspector.classCombobox.Items[creatorGUI.ceInspector.classCombobox.SelectedIndex] == "TXB2{d4}";
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        
        
        
        public void tableCellChanged(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            nextEnabled = true;
            if (tutorial == Tutorials.TutorialSRM && tutorialStep == (int)SRMSteps.SetElements)
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
            else if (tutorial == Tutorials.TutorialHL && tutorialStep == (int)HLSteps.SetElements)
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
            else if (tutorial == Tutorials.TutorialHL && tutorialStep == (int)HLSteps.SetElements2)
            {
                DataGridView dgv = creatorGUI.addHeavyPrecursor.dataGridView1;
                for (int i = 0; i < dgv.Rows.Count; ++i){
                    string key = dgv.Rows[i].Cells[0].Value.ToString();
                    int val = 0;
                    if (key == "H") val = 30;
                    
            
                    if ((int)creatorGUI.addHeavyPrecursor.currentDict[key][1] != val)
                    {
                        nextEnabled = false;
                        break;
                    }
                }
                creatorGUI.addHeavyPrecursor.Refresh();
            }
            else if (tutorial == Tutorials.TutorialHL && tutorialStep == (int)HLSteps.SetFragElement)
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
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        
        
        
        
        public void buttonInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && (new HashSet<int>(new int[]{(int)PRMSteps.AddLipid, (int)PRMSteps.OpenFilter, (int)PRMSteps.SelectFilter, (int)PRMSteps.OpenInterlist, (int)PRMSteps.OpenReview, (int)PRMSteps.StoreList, (int)PRMSteps.Finish}).Contains(tutorialStep)))
            {
            
                if (tutorialStep == (int)PRMSteps.OpenReview) creatorGUI.lipidsReview.Show();
                nextTutorialStep(true);
            }
            
            else if (tutorial == Tutorials.TutorialSRM && (new HashSet<int>(new int[]{(int)SRMSteps.OpenMS2, (int)SRMSteps.AddFragment, (int)SRMSteps.AddingFragment, (int)SRMSteps.ClickOK, (int)SRMSteps.AddLipid, (int)SRMSteps.OpenInterlist, (int)SRMSteps.OpenReview, (int)SRMSteps.StoreList, (int)SRMSteps.Finish}).Contains(tutorialStep)))
            {
                if (tutorialStep == (int)SRMSteps.OpenReview) creatorGUI.lipidsReview.Show();
                nextTutorialStep(true);
            }
            
            else if (tutorial == Tutorials.TutorialHL && (new HashSet<int>(new int[]{(int)HLSteps.OpenHeavy, (int)HLSteps.AddIsotope, (int)HLSteps.CloseHeavy, (int)HLSteps.OpenMS2, (int)HLSteps.EditFragment, (int)HLSteps.ConfirmEdit, (int)HLSteps.CloseFragment, (int)HLSteps.OpenFilter, (int)HLSteps.SelectFilter, (int)HLSteps.AddLipid, (int)HLSteps.OpenInterlist, (int)HLSteps.OpenReview, (int)HLSteps.StoreList, (int)HLSteps.Finish}).Contains(tutorialStep)))
            {
            
                if (tutorialStep == (int)HLSteps.OpenReview) creatorGUI.lipidsReview.Show();
                nextTutorialStep(true);
            }
            
            else if (tutorial == Tutorials.TutorialCE && (int)CESteps.ActivateCE == tutorialStep)
            {
                nextEnabled = (sender is MenuItem) && ((string[])((MenuItem)sender).Tag != null) && (((string[])((MenuItem)sender).Tag)[0] == "MS:1002523");
                tutorialWindow.Refresh();
            }
            
            else if (tutorial == Tutorials.TutorialCE && (new HashSet<int>(new int[]{(int)CESteps.OpenCEDialog, (int)CESteps.CloseCE, (int)CESteps.AddLipid, (int)CESteps.OpenInterlist, (int)CESteps.ReviewLipids, (int)CESteps.StoreBlib})).Contains(tutorialStep))
            {
                if (tutorialStep == (int)CESteps.ReviewLipids) creatorGUI.lipidsReview.Show();
                nextTutorialStep(true);
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        public void mouseDownInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && (new HashSet<int>(new int[]{(int)PRMSteps.OpenReview, (int)PRMSteps.SelectFilter, (int)PRMSteps.StoreList}).Contains(tutorialStep)))
            {
                
                
                continueTutorial = true;
                if (tutorialStep == (int)PRMSteps.OpenReview) tutorialAssembleLipids();
            }
            
            else if (tutorial == Tutorials.TutorialSRM && (new HashSet<int>(new int[]{(int)SRMSteps.OpenMS2, (int)SRMSteps.OpenReview, (int)SRMSteps.AddFragment, (int)SRMSteps.AddingFragment, (int)SRMSteps.ClickOK}).Contains(tutorialStep)))
            {
                continueTutorial = true;
                if (tutorialStep == (int)SRMSteps.OpenReview) tutorialAssembleLipids();
            }
            
            else if (tutorial == Tutorials.TutorialHL && (new HashSet<int>(new int[]{(int)HLSteps.OpenHeavy, (int)HLSteps.AddIsotope, (int)HLSteps.CloseHeavy, (int)HLSteps.OpenMS2, (int)HLSteps.EditFragment, (int)HLSteps.OpenReview, (int)HLSteps.ConfirmEdit, (int)HLSteps.SelectFilter, (int)HLSteps.CloseFragment}).Contains(tutorialStep)))
            {
                if (tutorialStep == (int)HLSteps.OpenReview) tutorialAssembleLipids();
                continueTutorial = true;
            }
            
            else if (tutorial == Tutorials.TutorialCE && (tutorialStep == (int)CESteps.CloseCE || tutorialStep == (int)CESteps.ReviewLipids))
            {
                if (tutorialStep == (int)CESteps.ReviewLipids) tutorialAssembleLipids();
                continueTutorial = true;
            }
            
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        public void tutorialAssembleLipids()
        {   
            creatorGUI.lipidCreator.assembleFragments(creatorGUI.asDeveloper);
            
            creatorGUI.lipidsReview = new LipidsReview(creatorGUI, null);
            creatorGUI.lipidsReview.Owner = creatorGUI;
            creatorGUI.lipidsReview.ShowInTaskbar = false;
        }
        
        
        public void checkedListBoxInteraction(Object sender, ItemCheckEventArgs e)
        {
            if (tutorial == Tutorials.TutorialSRM && tutorialStep == (int)SRMSteps.SelectFragments)
            {
                 HashSet<string> posFrag = creatorGUI.ms2fragmentsForm.currentLipid.positiveFragments["PG"];
                 HashSet<string> negFrag = creatorGUI.ms2fragmentsForm.currentLipid.negativeFragments["PG"];
                 
                 nextEnabled = (posFrag.Count == 1 && posFrag.Contains("-HG(PG,172)") && negFrag.Count == 2 && negFrag.Contains("FA1(+O)") && negFrag.Contains("HG(PG,171)"));
            }
            else if (tutorial == Tutorials.TutorialSRM && tutorialStep == (int)SRMSteps.SelectNew)
            {
                 HashSet<string> posFrag = creatorGUI.ms2fragmentsForm.currentLipid.positiveFragments["PG"];
                 HashSet<string> negFrag = creatorGUI.ms2fragmentsForm.currentLipid.negativeFragments["PG"];
                 
                 nextEnabled = (posFrag.Count == 2 && posFrag.Contains("-HG(PG,172)")&& posFrag.Contains("testFrag") && negFrag.Count == 2 && negFrag.Contains("FA1(+O)") && negFrag.Contains("HG(PG,171)"));
            }
            if (tutorial == Tutorials.TutorialHL && tutorialStep == (int)HLSteps.SelectFragments)
            {
                 HashSet<string> posFrag = creatorGUI.ms2fragmentsForm.currentLipid.positiveFragments["PG{13C6d30}"];
                 HashSet<string> negFrag = creatorGUI.ms2fragmentsForm.currentLipid.negativeFragments["PG{13C6d30}"];
                 
                 nextEnabled = (posFrag.Count == 1 && posFrag.Contains("-HG(PG,172)") && negFrag.Count == 2 && negFrag.Contains("FA1(+O)") && negFrag.Contains("HG(PG,171)"));
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        
        
        private void closingInteraction(Object sender, FormClosingEventArgs e)
        {
        
            if (tutorialArrow.Parent != null) tutorialArrow.Parent.Controls.Remove(tutorialArrow);
            if (tutorialWindow.Parent != null) tutorialWindow.Parent.Controls.Remove(tutorialWindow);
            
            if(e.CloseReason == CloseReason.UserClosing && !continueTutorial)
            {
                quitTutorial(true);
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        public void prepareStep()
        {
            disableEverything();
            nextEnabled = false;
            tutorialWindow.Visible = false;
            tutorialArrow.Visible = false;
        }
        
        public void TutorialPRMStep()
        {
            prepareStep();
            switch(tutorialStep)
            {   
                case (int)PRMSteps.Welcome:
                    setTutorialControls(creatorGUI.homeTab);
                    
                    
                    tutorialWindow.update(new Size(540, 200), new Point(140, 200), "Click on 'Continue'", "Welcome to the PRM tutorial (transitions for precursors) of LipidCreator. It will guide you interactively through this tool by showing you all necessary steps to create a targeted assay.", false);
                    
                    nextEnabled = true;
                    break;
                    
                    
                case (int)PRMSteps.PhosphoTab:
                    setTutorialControls(creatorGUI.homeTab);
                    
                    tutorialArrow.update(new Point((int)(creatorGUI.tabControl.ItemSize.Width * 2.5), 0), "lt");
                    
                    tutorialWindow.update(new Size(540, 200), new Point(140, 200), "Click on 'Phosholipids' tab", "LipidCreator offers computation for five lipid categories, namely glycerolipids, phospholipids, sphingolipids, cholesterols and mediators.");
                    
                    break;
                    
                    
                case (int)PRMSteps.PGheadgroup:
                    setTutorialControls(creatorGUI.plStep1, creatorGUI.phospholipidsTab);
                    
                    ListBox plHG = creatorGUI.plHgListbox;
                    int plHGpg = 0;
                    for (; plHGpg < plHG.Items.Count; ++plHGpg) if (plHG.Items[plHGpg].ToString().Equals("PG")) break;
                    tutorialArrow.update(new Point(plHG.Location.X + plHG.Size.Width, plHG.Location.Y + (int)((plHGpg + 0.5) * plHG.ItemHeight)), "tl");
                    
                    tutorialWindow.update(new Size(540, 200), new Point(460, 200), "Select the 'PG' headgroup", "The user can select multiple different headgroups. The according adducts are highlighted when hovering above a headgroup.", false);
                    
                    creatorGUI.plHgListbox.SelectedItems.Clear();
                    creatorGUI.plHgListbox.Enabled = true;
                    break;
                    
                    
                case (int)PRMSteps.SetFA:
                    setTutorialControls(creatorGUI.plStep1, creatorGUI.phospholipidsTab);
                    
                    TextBox plFA1 = creatorGUI.plFA1Textbox;
                    tutorialArrow.update(new Point(plFA1.Location.X, plFA1.Location.Y + (plFA1.Size.Height >> 1)), "tr");
                    
                    tutorialWindow.update(new Size(540, 200), new Point(460, 200), "Set first fatty acyl chain lengths to '14-18, 20'", "LipidCreator allows to describe a set of different fatty acyls (FAs) concisely instead of describing each FA separately.");
                                      
                    
                    plFA1.Text = "12-15";
                    plFA1.Enabled = true;
                    
                    break;
                    
                    
                case (int)PRMSteps.SetDB:
                    setTutorialControls(creatorGUI.plStep1, creatorGUI.phospholipidsTab);
                    
                    TextBox plDB1 = creatorGUI.plDB1Textbox;
                    tutorialArrow.update(new Point(plDB1.Location.X + plDB1.Size.Width, plDB1.Location.Y + (plDB1.Size.Height >> 1)), "tl");
                    
                    tutorialWindow.update(new Size(540, 200), new Point(60, 200), "Set the number of double bonds for first FA to '0-1'", "Here, one can specify the number of double bonds (DBs) for the first FA.");
                                      
                    
                    plDB1.Text = "0";
                    plDB1.Enabled = true;
                    break;
                    
                    
                case (int)PRMSteps.MoreParameters:
                    setTutorialControls(creatorGUI.plStep1, creatorGUI.phospholipidsTab);
                    TextBox plHyd1 = creatorGUI.plHydroxyl1Textbox;
                    tutorialArrow.update(new Point(plHyd1.Location.X + (plHyd1.Size.Width >> 1), plHyd1.Location.Y + plHyd1.Size.Height), "lt");
                    
                    tutorialWindow.update(new Size(540, 200), new Point(60, 200), "Click on 'Continue'", "The number of hydroxyl groups can be adjusted for each FA specification. Here, we stick with zero.");
                    
                    nextEnabled = true;
                    break;
                    
                    
                case (int)PRMSteps.RepresentitativeFA:
                    setTutorialControls(creatorGUI.plStep1, creatorGUI.phospholipidsTab);
                    
                    CheckBox plRep = creatorGUI.plRepresentativeFA;
                    tutorialArrow.update(new Point(plRep.Location.X, plRep.Location.Y + (plRep.Size.Height >> 1)), "tr");
                    
                    tutorialWindow.update(new Size(540, 200), new Point(60, 200), "Click on 'Continue'", "When selecting this check box, all FA parameters will be copied from the first FA to all remaining FAs.");
                    
                    nextEnabled = true;
                    break;
                    
                    
                case (int)PRMSteps.Ether:
                    setTutorialControls(creatorGUI.plStep1, creatorGUI.phospholipidsTab);
                    CheckBox plFACheck1 = creatorGUI.plFA1Checkbox1;
                    tutorialArrow.update(new Point(plFACheck1.Location.X, plFACheck1.Location.Y + (plFACheck1.Size.Height >> 1)), "tr");
                    
                    tutorialWindow.update(new Size(540, 200), new Point(460, 200), "Click on 'Continue'", "Ester or ether linked fatty acyls (fatty acyl, plasmenyl or plasmanyl) can be created here.");
                    nextEnabled = true;
                    
                    break;
                    
                    
                case (int)PRMSteps.SecondFADB:
                    setTutorialControls(creatorGUI.plStep1, creatorGUI.phospholipidsTab);
                    TextBox plFA2 = creatorGUI.plFA2Textbox;
                    tutorialArrow.update(new Point(plFA2.Location.X, plFA2.Location.Y + (plFA2.Size.Height >> 1)), "tr");
                    
                    tutorialWindow.update(new Size(540, 200), new Point(460, 200), "Set the second FA carbon chain lengths to '8-10' and number of DBs to '2'", "");
                    
                    plFA2.Text = "12-15";
                    plFA2.Enabled = true;
                    creatorGUI.plDB2Textbox.Text = "0";
                    creatorGUI.plDB2Textbox.Enabled = true;
                    break;
                    
                    
                case (int)PRMSteps.SelectAdduct:
                    setTutorialControls(creatorGUI.plStep1, creatorGUI.phospholipidsTab);
                    CheckBox adductP1 = creatorGUI.plPosAdductCheckbox1;
                    GroupBox P1 = creatorGUI.plPositiveAdduct;
                    tutorialArrow.update(new Point(P1.Location.X, P1.Location.Y + (P1.Size.Height >> 1)), "tr");
                    
                    tutorialWindow.update(new Size(540, 200), new Point(60, 200), "Select +H(+) adduct", "Several adducts are possible for selection. For PG, only the negative adduct -H(-) is selected by default.");
                    
                    
                    adductP1.Checked = false;
                    P1.Enabled = true;
                    adductP1.Enabled = true;
                    break;
                    
                    
                    
                case (int)PRMSteps.OpenFilter: 
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    Button fB = creatorGUI.filtersButton;
                    fB.Enabled = true;
                    tutorialArrow.update(new Point(fB.Location.X + (fB.Size.Width >> 1) + creatorGUI.lcStep2.Location.X, fB.Location.Y + creatorGUI.lcStep2.Location.Y), "rb");
                    
                    tutorialWindow.update(new Size(440, 200), new Point(560, 200), "Click on 'Filters'", "", false);
                    break;
                
                
                
                case (int)PRMSteps.SelectFilter:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    
                    initFilterDialog();
                    
                    tutorialWindow.update(new Size(440, 200), new Point(560, 200), "Select 'Compute only precursor transitions' and click on 'Ok'", "", false);
                    
                    creatorGUI.filterDialog.groupBox1.Enabled = true;
                    break;
                    
                    
                case (int)PRMSteps.AddLipid:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    
                    
                    Button alb = creatorGUI.addLipidButton;
                    tutorialArrow.update(new Point(alb.Location.X + 20 + creatorGUI.lcStep3.Location.X, alb.Location.Y + creatorGUI.lcStep3.Location.Y), "rb");
                    alb.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(34, 34), "Click on 'Add phospholipid'", "Add the complete lipid assembly into the basket", false);
                    break;
                    
                    
                    
                    
                case (int)PRMSteps.OpenInterlist:
                    setTutorialControls(creatorGUI.lipidsGroupbox, creatorGUI);
                    
                    
                    Button orfb = creatorGUI.openReviewFormButton;
                    orfb.Enabled = true;
                    tutorialArrow.update(new Point(orfb.Location.X + (orfb.Size.Width >> 1), orfb.Location.Y), "lb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Review Lipids'", "This creates and displays the precursors and further the final transition list which including all precursors and fragment information.");
                    break;
                
                
                
                case (int)PRMSteps.ExplainInterlist:
                    initInterList();
                    setTutorialControls(creatorGUI.lipidsInterList);
                    
                    tutorialWindow.update(new Size(500, 250), new Point(40, 200), "Click on 'Continue", "The precursors can be select/deselect from this list for the generation of transition list in next step.", false);
                    nextEnabled = true;
                    
                    break;

                    
                case (int)PRMSteps.OpenReview:
                    setTutorialControls(creatorGUI.lipidsInterList);
                    
                    Button lilb = creatorGUI.lipidsInterList.continueReviewButton;
                    lilb.Enabled = true;
                    tutorialArrow.update(new Point(lilb.Location.X + (lilb.Size.Width >> 1), lilb.Location.Y), "rb");
                    
                    tutorialWindow.update(new Size(500, 250), new Point(40, 200), "Click on 'Continue' in 'Lipid Precursor Review' window", "");
                    break;
                    
                    
                case (int)PRMSteps.StoreList:
                    setTutorialControls(creatorGUI.lipidsReview);
                    initLipidReview();
                    
                    Button bstl = creatorGUI.lipidsReview.buttonStoreTransitionList;
                    bstl.Enabled = true;
                    
                    tutorialArrow.update(new Point(bstl.Location.X + (bstl.Size.Width >> 1), bstl.Location.Y), "lb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Store transition list'", "'Store transition list' stores the list in csv format. If LipidCreator is started from Skyline, the transition list can be directly transfered to Skyline by clicking on 'Send to Skyline'.", false);
                    
                    break;
                    
                    
                case (int)PRMSteps.Finish:
                    setTutorialControls(creatorGUI.lipidsReview);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(40, 34), "End", "Congratulations, you finished the first tutorial. If you need more information, please use the other tutorials. Have fun with LipidCreator!");
                    
                    nextEnabled = true;
                    tutorialWindow.Refresh();
                    break;
                    
                    
                default:
                    quitTutorial(true);
                    break;
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        public void TutorialSRMStep()
        {
            prepareStep();
            switch(tutorialStep)
            {   
                case (int)SRMSteps.Welcome:
                    setTutorialControls(creatorGUI.homeTab);
                    
                    
                    tutorialWindow.update(new Size(540, 200), new Point(140, 200), "Click on 'Continue'", "Welcome to the SRM tutorial (transitions for fragments) of LipidCreator. This tutorial builds upon the first tutorial.", false);
                    
                    nextEnabled = true;
                    tutorialWindow.Refresh();
                    break;
                    
                    
                case (int)SRMSteps.PhosphoTab:
                    // set MS1 data from tutorial one
                    ((Lipid)creatorGUI.lipidTabList[2]).headGroupNames.Add("PG");
                    ((Lipid)creatorGUI.lipidTabList[2]).adducts["+H"] = true;
                    ((Phospholipid)creatorGUI.lipidTabList[2]).fag1.lengthInfo = "14-18, 20";
                    ((Phospholipid)creatorGUI.lipidTabList[2]).fag1.dbInfo = "0, 1";
                    ((Phospholipid)creatorGUI.lipidTabList[2]).fag2.lengthInfo = "8-10";
                    ((Phospholipid)creatorGUI.lipidTabList[2]).fag2.dbInfo = "2";
                    
                    currentTabIndex = 2;
                    creatorGUI.changeTab(2);
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    
                    tutorialWindow.update(new Size(540, 200), new Point(140, 200), "Click on 'Continue'", "The selection from tutorial one is already present. We can continue immediately.", false);
                    nextEnabled = true;
                    tutorialWindow.Refresh();
                    
                    break;
                    
                    
                    
                    
                case (int)SRMSteps.OpenMS2:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    
                    Button ms2 = creatorGUI.MS2fragmentsLipidButton;
                    tutorialArrow.update(new Point(ms2.Location.X + (ms2.Size.Width >> 1), ms2.Location.Y + creatorGUI.lcStep2.Location.Y), "lb");
                    
                    tutorialWindow.update(new Size(540, 200), new Point(460, 200), "Open the MS2 fragments dialog", "");
                    
                    ms2.Enabled = true;
                    break;
                    
                    
                case (int)SRMSteps.InMS2:
                    initMS2Form();
                    setTutorialControls(creatorGUI.ms2fragmentsForm);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Click on 'Continue'", "Here you can have a deeper look into all predefined MS2 fragments for each lipid category.", false);
                    
                    nextEnabled = true;
                    tutorialWindow.Refresh();
                    break;
                    
                     
                case (int)SRMSteps.SelectPG:
                    setTutorialControls((TabPage)creatorGUI.ms2fragmentsForm.tabPages[0], creatorGUI.ms2fragmentsForm);
                    
                    TabControl ms2tc2 = creatorGUI.ms2fragmentsForm.tabControlFragments;
                    tutorialArrow.update(new Point((int)(ms2tc2.ItemSize.Width * ((pgIndex % 16) + 0.5)), 0), "lt");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Select the 'PG' tab", "", false);
                    
                    break;
                    
                     
                case (int)SRMSteps.SelectFragments:
                    setTutorialControls((TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex], creatorGUI.ms2fragmentsForm);
                    
                    CheckedListBox negCLB = creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments;
                    tutorialArrow.update(new Point(negCLB.Location.X + negCLB.Size.Width, negCLB.Location.Y + (negCLB.Size.Height >> 1)), "tl");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Select only the -HG(PG,172)+, FA1(+O)- and HG(PG,171)- fragments", "When hovering over the fragments, a structure of the fragment is displayed.");
                    
                    creatorGUI.ms2fragmentsForm.labelPositiveDeselectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.labelPositiveSelectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.labelNegativeDeselectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.labelNegativeSelectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.Enabled = true;
                    creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.Enabled = true;
                    
                    break;
                    
                
                case (int)SRMSteps.AddFragment:
                    setTutorialControls((TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex], creatorGUI.ms2fragmentsForm);
                    
                    Button ms2fragButton = creatorGUI.ms2fragmentsForm.buttonAddFragment;
                    
                    ms2fragButton.Enabled = true;
                    tutorialArrow.update(new Point(ms2fragButton.Location.X + (ms2fragButton.Size.Width >> 1) - creatorGUI.ms2fragmentsForm.tabControlFragments.Left, creatorGUI.ms2fragmentsForm.tabControlFragments.Height - 46), "lb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Open the 'Add fragment' dialog", "Here you can define new fragments.");
                    
                    
                    break;
                    
                    
                case (int)SRMSteps.InFragment:
                    setTutorialControls(creatorGUI.ms2fragmentsForm);
                    
                    initAddFragmentForm();
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Click on 'Continue'", "This form allows you to define your own fragments. The definition is descriptive. Name, dependent building blocks, polarity and constant elements can be added.", false);
                    
                    nextEnabled = true;
                    tutorialWindow.Refresh();
                    break;
                    
                    
                case (int)SRMSteps.NameFragment:
                    setTutorialControls(creatorGUI.ms2fragmentsForm);
                    
                    creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName.Enabled = true;
                    creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.Enabled = true;
                    
                    
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Enter the name 'testFrag', choose 'FA1' from Select base", "The fragment can either be fixed or dependent on its building blocks.");
                    
                    break;
                    
                    
                case (int)SRMSteps.SetCharge:
                    setTutorialControls(creatorGUI.ms2fragmentsForm);
                    
                    creatorGUI.ms2fragmentsForm.newFragment.numericUpDownCharge.Enabled = true;
                    
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Set the charge to +1", "In the right most upper field, the charge can be set.");
                    
                    break;
                    
                    
                case (int)SRMSteps.SetElements:
                    setTutorialControls(creatorGUI.ms2fragmentsForm);
                    
                    creatorGUI.ms2fragmentsForm.newFragment.dataGridViewElements.Enabled = true;
                    
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Set hydrogen count to 3 and oxygen to 2", "A constant set of elements can be defined which will be added to the fragment. When a 'fixed' base is selected, element numbers can only be positive, otherwise negative counts are also allowed.");
                    
                    break;
                    
                    
                case (int)SRMSteps.AddingFragment:
                    setTutorialControls(creatorGUI.ms2fragmentsForm);
                    
                    creatorGUI.ms2fragmentsForm.newFragment.addButton.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 234), "Click on the 'Add' button to add the fragment", "");
                    
                    break;
                    
                    
                case (int)SRMSteps.SelectNew:
                    setTutorialControls((TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex], creatorGUI.ms2fragmentsForm);
                    
                    creatorGUI.ms2fragmentsForm.newFragment = null;
                    CheckedListBox posCLB = creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments;
                    posCLB.Enabled = true;
                    tutorialArrow.update(new Point(posCLB.Location.X + posCLB.Size.Width, posCLB.Location.Y + (posCLB.Size.Height >> 1)), "tl");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 34), "Select new fragment", "By right clicking, you can either edit or delete the fragment. Only user defined fragments are allowed to be updated or deleted.", false);
                    
                    break;
                    
                    
                case (int)SRMSteps.ClickOK:
                    setTutorialControls((TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex], creatorGUI.ms2fragmentsForm);
                    
                    Button b = creatorGUI.ms2fragmentsForm.buttonOK;
                    b.Enabled = true;
                    tutorialArrow.update(new Point(b.Location.X + (b.Size.Width >> 1) - creatorGUI.ms2fragmentsForm.tabControlFragments.Left, creatorGUI.ms2fragmentsForm.tabControlFragments.Height - 46), "rb");
                    
                    
                    tutorialWindow.update(new Size(500, 200), new Point(620, 34), "Click on 'OK'", "Please confirm the fragment selection.");
                    
                    break;
                    
                    
                    
                    
                case (int)SRMSteps.AddLipid:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    
                    Button alb = creatorGUI.addLipidButton;
                    tutorialArrow.update(new Point(alb.Location.X + 20 + creatorGUI.lcStep3.Location.X, alb.Location.Y + creatorGUI.lcStep3.Location.Y), "rb");
                    alb.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(34, 34), "Click on 'Add phospholipid'", "Add the complete lipid assembly into the basket", false);
                    break;
                    
                    
                case (int)SRMSteps.OpenInterlist:
                    setTutorialControls(creatorGUI.lipidsGroupbox, creatorGUI);
                    
                    
                    Button orfb = creatorGUI.openReviewFormButton;
                    orfb.Enabled = true;
                    tutorialArrow.update(new Point(orfb.Location.X + (orfb.Size.Width >> 1), orfb.Location.Y), "lb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Review Lipids'", "This creates and displays precursors and further the final transition list, including all precursors and fragment information.");
                    
                    break;

                    
                case (int)SRMSteps.OpenReview:
                    initInterList();
                    setTutorialControls(creatorGUI.lipidsInterList);
                    
                    Button lilb = creatorGUI.lipidsInterList.continueReviewButton;
                    lilb.Enabled = true;
                    tutorialArrow.update(new Point(lilb.Location.X + (lilb.Size.Width >> 1), lilb.Location.Y), "rb");
                    
                    tutorialWindow.update(new Size(500, 250), new Point(40, 200), "Click on 'Continue' in 'Lipid Precursor Review'", "");
                    break;
                    
                    
                    
                    
                case (int)SRMSteps.StoreList:
                    setTutorialControls(creatorGUI.lipidsReview);
                    initLipidReview();
                    
                    Button bstl = creatorGUI.lipidsReview.buttonStoreTransitionList;
                    bstl.Enabled = true;
                    
                    tutorialArrow.update(new Point(bstl.Location.X + (bstl.Size.Width >> 1), bstl.Location.Y), "lb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Store transition list'", "'Store transition list' stores the list in csv format. If LipidCreator is started from Skyline, the transition list can be directly transfered to Skyline by clicking on 'Send to Skyline'.", false);
                    
                    break;
                    
                    
                case (int)SRMSteps.Finish:
                    setTutorialControls(creatorGUI.lipidsReview);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(40, 34), "End", "Congratulations, you finished this tutorial. If you need more information, please use the other tutorials. Have fun with LipidCreator!");
                    
                    nextEnabled = true;
                    tutorialWindow.Refresh();
                    break;
                    
                default:
                    quitTutorial();
                    break;
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        public void TutorialHLStep()
        {
        
            prepareStep();
            switch(tutorialStep)
            {   
                case (int)HLSteps.Welcome:
                    setTutorialControls(creatorGUI.homeTab);
                    
                    tutorialWindow.update(new Size(540, 200), new Point(140, 200), "Click on 'Continue'", "This tutorial will provide an introduction to the creation of heavy labelled lipids.", false);
                    nextEnabled = true;
                    break;
                    
                    
                    
                    
                case (int)HLSteps.OpenHeavy:
                    // set MS1 data from tutorial one
                    ((Lipid)creatorGUI.lipidTabList[2]).headGroupNames.Add("PG");
                    ((Lipid)creatorGUI.lipidTabList[2]).adducts["+H"] = true;
                    ((Phospholipid)creatorGUI.lipidTabList[2]).fag1.lengthInfo = "14-18, 20";
                    ((Phospholipid)creatorGUI.lipidTabList[2]).fag1.dbInfo = "0, 1";
                    ((Phospholipid)creatorGUI.lipidTabList[2]).fag2.lengthInfo = "8-10";
                    ((Phospholipid)creatorGUI.lipidTabList[2]).fag2.dbInfo = "2";
                    
                    // set MS2 data from tutorial two
                    Dictionary<int, int> newElements = MS2Fragment.createEmptyElementDict();
                    newElements[MS2Fragment.ELEMENT_POSITIONS["H"]] = 3;
                    newElements[MS2Fragment.ELEMENT_POSITIONS["O"]] = 2;
                    MS2Fragment newFragment = new MS2Fragment("testFrag", "testFrag", 1, null, newElements, "FA1");
                    newFragment.userDefined = true;
                    creatorGUI.lipidCreator.allFragments["PG"][true]["testFrag"] = newFragment;
                    ((Lipid)creatorGUI.lipidTabList[2]).positiveFragments["PG"].Clear();
                    ((Lipid)creatorGUI.lipidTabList[2]).positiveFragments["PG"].Add("-HG(PG,172)");
                    ((Lipid)creatorGUI.lipidTabList[2]).positiveFragments["PG"].Add("testFrag");
                    ((Lipid)creatorGUI.lipidTabList[2]).negativeFragments["PG"].Clear();
                    ((Lipid)creatorGUI.lipidTabList[2]).negativeFragments["PG"].Add("FA1(+O)");
                    ((Lipid)creatorGUI.lipidTabList[2]).negativeFragments["PG"].Add("HG(PG,171)");
                
                    currentTabIndex = 2;
                    creatorGUI.changeTab(2);
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    creatorGUI.Enabled = true;
                    
                    creatorGUI.ms2fragmentsForm = null;
                    Button hli = creatorGUI.addHeavyIsotopeButton;
                    hli.Enabled = true;
                    tutorialArrow.update(new Point(hli.Location.X + (hli.Size.Width >> 1), hli.Location.Y + creatorGUI.lcStep2.Location.Y), "lb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Manage heavy isotopes'", "", false);
                    break;
                    
                    
                case (int)HLSteps.HeavyPanel:
                    initHeavyLabeled();
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Continue'", "The mode can be selected either by adding new or editing existing user defined heavy isotopes.", false);
                    
                    nextEnabled = true;
                    tutorialWindow.Refresh();
                    break;
                    
                    
                case (int)HLSteps.NameHeavy:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    
                    creatorGUI.addHeavyPrecursor.comboBox1.Enabled = true;
                    creatorGUI.addHeavyPrecursor.textBox1.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Select PG and name it '13C6d30'", "Please select the current lipid class PG and set its isotope suffix to '13C6d30'.");
                    
                    break;
                    
                    
                case (int)HLSteps.OptionsExplain:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Continue'", "In 'Building block', the head group and two fatty acyls can be edited for PG. We will start with the head group.");
                    
                    nextEnabled = true;
                    tutorialWindow.Refresh();
                    break;
                    
                    
                case (int)HLSteps.SetElements:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    
                    creatorGUI.addHeavyPrecursor.dataGridView1.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Set the isotopic count of 13C to 6", "Press Enter after typing in the number.");
                    
                    break;
                    
                    
                case (int)HLSteps.ChangeBuildingBlock:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    creatorGUI.addHeavyPrecursor.comboBox2.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Change building block to 'Fatty acyl 1'", "");
                    
                    break;
                    
                    
                case (int)HLSteps.SetElements2:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    creatorGUI.addHeavyPrecursor.dataGridView1.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Set the isotopic count of 2H to 30", "The heavy labelled element numbers act as an upper limit for the element, since the fatty acyl building block has a variable number of elements depending e.g. on the carbon chain length.");
                    
                    break;
                    
                    
                case (int)HLSteps.AddIsotope:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    creatorGUI.addHeavyPrecursor.button2.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Add isotope'", "The user defined isotopes will be reset when restarting LipidCreator. To use them further, please export lipid settings from the 'File' menu.");
                    
                    break;
                    
                    
                case (int)HLSteps.EditExplain:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Continue'", "All user defined heavy isotopes can be modified by changing the window mode in the upper part.");
                    
                    nextEnabled = true;
                    tutorialWindow.Refresh();
                    break;
                    
                    
                case (int)HLSteps.CloseHeavy:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    creatorGUI.addHeavyPrecursor.button1.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Close Window'", "", false);
                    
                    break;
                    
                    
                case (int)HLSteps.OpenMS2:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    
                    Button ms2_2 = creatorGUI.MS2fragmentsLipidButton;
                    tutorialArrow.update(new Point(ms2_2.Location.X + (ms2_2.Size.Width >> 1), ms2_2.Location.Y + creatorGUI.lcStep2.Location.Y), "lb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Open the MS2 fragments dialog", "", false);
                    
                    ms2_2.Enabled = true;
                    
                    break;
                    
                    
                case (int)HLSteps.SelectPG:
                    initMS2Form();
                    setTutorialControls((TabPage)creatorGUI.ms2fragmentsForm.tabPages[0], creatorGUI.ms2fragmentsForm);
                    
                    TabControl ms2tc2_2 = creatorGUI.ms2fragmentsForm.tabControlFragments;
                    tutorialArrow.update(new Point((int)(ms2tc2_2.ItemSize.Width * ((pgIndex % 16) + 0.5)), 0), "lt");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Select the 'PG' tab", "", false);
                    
                    break;
                    
                    
                case (int)HLSteps.SelectHeavy:
                    setTutorialControls((TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex], creatorGUI.ms2fragmentsForm);
                    
                    ComboBox il1 = creatorGUI.ms2fragmentsForm.isotopeList;
                    tutorialArrow.update(new Point(il1.Location.X + (il1.Width >> 1), il1.Location.Y + il1.Height), "lt");
                    il1.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Select PG{13C6d30}'", "", false);
                    
                    break;
                    
                    
                case (int)HLSteps.SelectFragments:
                    setTutorialControls((TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex], creatorGUI.ms2fragmentsForm);
                    
                    CheckedListBox negCLB_2 = creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments;
                    tutorialArrow.update(new Point(negCLB_2.Location.X + negCLB_2.Size.Width, negCLB_2.Location.Y + (negCLB_2.Size.Height >> 1)), "tl");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Select only -HG(PG,172), FA1(+O) and HG(PG,171) fragments", "");
                    
                    creatorGUI.ms2fragmentsForm.labelPositiveDeselectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.labelPositiveSelectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.labelNegativeDeselectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.labelNegativeSelectAll.Enabled = true;
                    creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.Enabled = true;
                    creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.Enabled = true;
                    
                    break;
                    
                    
                    
                case (int)HLSteps.CheckFragment:
                    setTutorialControls((TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex], creatorGUI.ms2fragmentsForm);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Click on 'Continue'", "It is necessary to check if precursor modification satisfies the fragment modification.");
                    
                    nextEnabled = true;
                    tutorialWindow.Refresh();
                    break;
                    
                    
                    
                case (int)HLSteps.EditFragment:
                    setTutorialControls((TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex], creatorGUI.ms2fragmentsForm);
                    
                    
                    CheckedListBox negCLB_3 = creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments;
                    tutorialArrow.update(new Point(negCLB_3.Location.X + negCLB_3.Size.Width, negCLB_3.Location.Y + (negCLB_3.Size.Height >> 1)), "tl");
                    creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Right click on FA1(+O) and 'Edit fragment'", "In this case, FA1(+O) has a constant hydrogen which is not effected from being deuterated, it needs to be changed.");
                    
                    break;
                    
                    
                    
                case (int)HLSteps.SetFragElement:
                    setTutorialControls((TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex], creatorGUI.ms2fragmentsForm);
                    initAddFragmentForm();
                    
                    creatorGUI.ms2fragmentsForm.newFragment.dataGridViewElements.Enabled = true;
                    tutorialWindow.update(new Size(500, 200), new Point(500, 200), "Set isotopic count of 2H to 1 and count of H to 0", "", false);
                    
                    break;
                    
                    
                    
                case (int)HLSteps.ConfirmEdit:
                    setTutorialControls((TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex], creatorGUI.ms2fragmentsForm);
                    
                    creatorGUI.ms2fragmentsForm.newFragment.addButton.Enabled = true;
                    tutorialWindow.update(new Size(500, 200), new Point(620, 34), "Click on 'OK'", "");
                    
                    break;
                    
                    
                    
                case (int)HLSteps.CloseFragment:
                    setTutorialControls((TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex], creatorGUI.ms2fragmentsForm);
                    
                    Button b_2 = creatorGUI.ms2fragmentsForm.buttonOK;
                    b_2.Enabled = true;
                    tutorialArrow.update(new Point(b_2.Location.X + (b_2.Size.Width >> 1) - 20, ((TabPage)creatorGUI.ms2fragmentsForm.tabPages[pgIndex]).Height), "rb");
                    tutorialWindow.update(new Size(500, 200), new Point(620, 34), "Click on'OK'", "", false);
                    
                    break;
                    
                    
                    
                    
                case (int)HLSteps.OpenFilter: 
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    Button fB = creatorGUI.filtersButton;
                    fB.Enabled = true;
                    tutorialArrow.update(new Point(fB.Location.X + (fB.Size.Width >> 1) + creatorGUI.lcStep2.Location.X, fB.Location.Y + creatorGUI.lcStep2.Location.Y), "rb");
                    
                    tutorialWindow.update(new Size(440, 200), new Point(560, 200), "Click on 'Filters'", "", false);
                    break;
                
                
                
                case (int)HLSteps.SelectFilter:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    
                    initFilterDialog();
                    
                    tutorialWindow.update(new Size(440, 200), new Point(560, 200), "Select 'Compute only heavy labeled isotopes' and click on 'Ok'", "", false);
                    
                    creatorGUI.filterDialog.groupBox2.Enabled = true;
                    break;
                
                
                    
                
                case (int)HLSteps.AddLipid:
                    setTutorialControls(creatorGUI.phospholipidsTab);
                    
                    Button alb = creatorGUI.addLipidButton;
                    tutorialArrow.update(new Point(alb.Location.X + 20 + creatorGUI.lcStep3.Location.X, alb.Location.Y + creatorGUI.lcStep3.Location.Y), "rb");
                    alb.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(34, 34), "Add phospholipid", "Add the complete lipid assembly into the basket.", false);
                    break;
                    
                    
                case (int)HLSteps.OpenInterlist:
                    setTutorialControls(creatorGUI.lipidsGroupbox, creatorGUI);
                    
                    
                    Button orfb = creatorGUI.openReviewFormButton;
                    orfb.Enabled = true;
                    tutorialArrow.update(new Point(orfb.Location.X + (orfb.Size.Width >> 1), orfb.Location.Y), "lb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Review Lipids'", "This creates the precursors and final transition list, including all precursors and fragment information.");
                    
                    break;

                    
                case (int)HLSteps.OpenReview:
                    initInterList();
                    setTutorialControls(creatorGUI.lipidsInterList);
                    
                    Button lilb = creatorGUI.lipidsInterList.continueReviewButton;
                    lilb.Enabled = true;
                    tutorialArrow.update(new Point(lilb.Location.X + (lilb.Size.Width >> 1), lilb.Location.Y), "rb");
                    
                    tutorialWindow.update(new Size(500, 250), new Point(40, 200), "Click on 'Continue' in 'Lipid Precursor Review'", "");
                    break;
                    
                    
                case (int)HLSteps.StoreList:
                    setTutorialControls(creatorGUI.lipidsReview);
                    initLipidReview();
                    
                    Button bstl = creatorGUI.lipidsReview.buttonStoreTransitionList;
                    bstl.Enabled = true;
                    
                    tutorialArrow.update(new Point(bstl.Location.X + (bstl.Size.Width >> 1), bstl.Location.Y), "lb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Store transition list'", "'Store transition list' stores  the list in csv format. If LipidCreator is started from Skyline, the transition list can be directly transfered to Skyline by clicking on 'Send to Skyline'.", false);
                    
                    break;
                    
                    
                case (int)HLSteps.Finish:
                    setTutorialControls(creatorGUI.lipidsReview);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(40, 34), "End", "Congratulations, you finished this tutorial. If you need more information, please read the documentation. Have fun with LipidCreator!");
                    
                    nextEnabled = true;
                    tutorialWindow.Refresh();
                    break;
                    
                    
                    
                default:
                    quitTutorial();
                    break;
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
        
        
        public void TutorialCEStep()
        {
        
            prepareStep();
            switch(tutorialStep)
            {   
                case (int)CESteps.Welcome:
                    setTutorialControls(creatorGUI.homeTab);
                    
                    tutorialWindow.update(new Size(540, 200), new Point(140, 200), "Click on 'Continue'", "Another feature of LipidCreator is the collision energy optization module. With this module it is possible set an optimal collision energy for a lipid species.", false);
                    nextEnabled = true;
                    break;
                    
                case (int)CESteps.ActivateCE:
                    setTutorialControls(creatorGUI.homeTab);
                    creatorGUI.menuOptions.Enabled = true;
                    creatorGUI.menuCollisionEnergy.Enabled = true;
                    
                    bool found = false;
                    foreach (MenuItem menuItem in creatorGUI.menuCollisionEnergy.MenuItems)
                    {
                        if (menuItem.Tag == null) continue;
                        if (((string[])menuItem.Tag)[0] == "MS:1002523")
                        {
                            found = true;
                            menuItem.Enabled = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        log.Error("Could not find 'MS:1002523' device in CE instrument selection.");
                        quitTutorial();
                    }
                    
                    tutorialWindow.update(new Size(640, 200), new Point(140, 200), "Select 'Options' > 'Collision Energy computation' > 'Thermo Scientific Q Exactive HF'", " ");
                    break;
                    
                case (int)CESteps.OpenCEDialog:
                    setTutorialControls(creatorGUI.homeTab);
                    creatorGUI.menuOptions.Enabled = true;
                    creatorGUI.menuCollisionEnergyOpt.Enabled = true;
                    
                    
                    tutorialWindow.update(new Size(440, 200), new Point(140, 200), "Select 'Options' > 'Collision Energy optimization'", "You activated now system wide the CE optimization independant of the assembled lipids. ");
                    break;
                    
                case (int)CESteps.SelectTXB2:
                    setTutorialControls(creatorGUI.ceInspector);
                    initCEInspector();
                    
                    ComboBox cbClass = creatorGUI.ceInspector.classCombobox;
                    cbClass.Enabled = true;
                    
                    tutorialArrow.update(new Point(cbClass.Location.X + cbClass.Width, cbClass.Location.Y + (cbClass.Height >> 1)), "tl");
                    tutorialWindow.update(new Size(440, 200), new Point(100, 300), "Select TXB2", "", false);
                    break;
                    
                case (int)CESteps.ExplainBlackCurve:
                    setTutorialControls(creatorGUI.ceInspector);
                    
                    tutorialArrow.update(new Point(340, 400), "bl");
                    tutorialWindow.update(new Size(500, 200), new Point(500, 350), "Continue", "The black curve is the automatically calculated product distribution over all selected fragment distributions from the list. Its mode indicates the optimal collision energy over all selected fragments.");
                    nextEnabled = true;
                    
                    break;
                    
                case (int)CESteps.ChangeManually:
                    setTutorialControls(creatorGUI.ceInspector);
                    
                    creatorGUI.ceInspector.radioButtonPRMFragments.Enabled = true;
                    RadioButton rbManually = creatorGUI.ceInspector.radioButtonPRMArbitrary;
                    GroupBox gbCE = creatorGUI.ceInspector.groupBoxPRMMode;
                    
                    rbManually.Enabled = true;
                    
                    tutorialArrow.update(new Point(rbManually.Location.X + gbCE.Location.X, rbManually.Location.Y + gbCE.Location.Y + (rbManually.Height >> 1)), "br");
                    tutorialWindow.update(new Size(500, 200), new Point(100, 350), "Select 'Manually'", "To adjust CE manually.");
                    break;
                    
                case (int)CESteps.CEto20:
                    setTutorialControls(creatorGUI.ceInspector);
                    
                    NumericUpDown nudCE = creatorGUI.ceInspector.numericalUpDownCurrentCE;
                    nudCE.Enabled = true;
                    GroupBox gbCE2 = creatorGUI.ceInspector.groupBoxPRMMode;

                    tutorialWindow.update(new Size(500, 200), new Point(100, 350), "Set optimal CE to '20'", "Type in number or move dashed line.");
                    tutorialArrow.update(new Point(nudCE.Location.X + gbCE2.Location.X, nudCE.Location.Y + gbCE2.Location.Y + (nudCE.Height >> 1)), "br");
                    tutorialArrow.Refresh();
                    break;
                    
                case (int)CESteps.SameForD4:
                    setTutorialControls(creatorGUI.ceInspector);
                    
                    ComboBox cbClass2 = creatorGUI.ceInspector.classCombobox;
                    cbClass2.Enabled = true;
                    
                    tutorialWindow.update(new Size(440, 200), new Point(100, 300), "Select TXB2{d4} and set CE to 20", "");
                    break;
                    
                case (int)CESteps.CloseCE:
                    setTutorialControls(creatorGUI.ceInspector);
                    
                    Button buttonOK = creatorGUI.ceInspector.button2;
                    buttonOK.Enabled = true;
                    tutorialArrow.update(new Point(buttonOK.Location.X + (buttonOK.Width >> 1), buttonOK.Location.Y ), "rb");
                    
                    tutorialWindow.update(new Size(440, 200), new Point(100, 300), "Click on 'Ok' to confirm your changes", "");
                    break;
                    
                case (int)CESteps.ChangeToMediators:
                    setTutorialControls(creatorGUI.homeTab);
                    
                    tutorialArrow.update(new Point((int)(creatorGUI.tabControl.ItemSize.Width * 5.5), 0), "rt");
                    
                    tutorialWindow.update(new Size(540, 200), new Point(140, 200), "Click on 'Mediators' tab", "", false);
                    //setTutorialControls(creatorGUI.homeTab);
                    tutorialWindow.Refresh();
                    break;
                    
                case (int)CESteps.SelectTXB2HG:
                    setTutorialControls(creatorGUI.medStep1, creatorGUI.mediatorlipidsTab);
                    
                    ListBox medHG = creatorGUI.medHgListbox;
                    
                    tutorialArrow.update(new Point(medHG.Location.X + medHG.Size.Width, medHG.Location.Y + (medHG.Height >> 1)), "tl");
                    
                    tutorialWindow.update(new Size(540, 200), new Point(460, 200), "Select 'TXB2'", "");
                    break;
                    
                    
                case (int) CESteps.AddLipid:
                    setTutorialControls(creatorGUI.mediatorlipidsTab);
                    
                    Button alb = creatorGUI.addLipidButton;
                    tutorialArrow.update(new Point(alb.Location.X + 20 + creatorGUI.lcStep3.Location.X, alb.Location.Y + creatorGUI.lcStep3.Location.Y), "rb");
                    alb.Enabled = true;
                    
                    tutorialWindow.update(new Size(500, 200), new Point(34, 34), "Click on 'Add mediators'", "Add the lipid assembly into the basket");
                    break;
                    
                    
                    
                    
                case (int)CESteps.OpenInterlist:
                    setTutorialControls(creatorGUI.lipidsGroupbox, creatorGUI);
                    
                    
                    Button orfb = creatorGUI.openReviewFormButton;
                    orfb.Enabled = true;
                    tutorialArrow.update(new Point(orfb.Location.X + (orfb.Size.Width >> 1), orfb.Location.Y), "lb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Review Lipids'", "This creates precursors and the final transition list, including all precursors, fragments and CE information.");
                    break;

                    
                case (int)CESteps.ReviewLipids:
                    initInterList();
                    setTutorialControls(creatorGUI.lipidsInterList);
                    
                    Button lilb = creatorGUI.lipidsInterList.continueReviewButton;
                    lilb.Enabled = true;
                    tutorialArrow.update(new Point(lilb.Location.X + (lilb.Size.Width >> 1), lilb.Location.Y), "rb");
                    
                    tutorialWindow.update(new Size(500, 250), new Point(40, 200), "Click on 'Continue' in 'Lipid Precursor Review'", "");
                    break;
                    
                    
                case (int)CESteps.ExplainLCasExternal:
                    setTutorialControls(creatorGUI.lipidsReview);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(40, 34), "Continue", "When use LipidCreator as external tool in Skyline, the checkbox of 'Create Spectral library' and 'Send to Skyline' are valid to use.", false);
                    nextEnabled = true;
                    break;
                    
                    
                case (int)CESteps.StoreBlib:
                    setTutorialControls(creatorGUI.lipidsReview);
                    initLipidReview();
                    
                    Button bssl = creatorGUI.lipidsReview.buttonStoreSpectralLibrary;
                    bssl.Enabled = true;
                    
                    tutorialArrow.update(new Point(bssl.Location.X + (bssl.Size.Width >> 1), bssl.Location.Y), "rb");
                    
                    tutorialWindow.update(new Size(500, 200), new Point(480, 34), "Click on 'Store spectral library'", "Save the spectral library in *.blib format");
                    break;
                    
                    
                case (int)CESteps.Finish:
                    setTutorialControls(creatorGUI.lipidsReview);
                    
                    tutorialWindow.update(new Size(500, 200), new Point(40, 34), "End", "Congratulations, you finished this tutorial. If you need more information, please read the documentation. Have fun with LipidCreator!");
                    
                    nextEnabled = true;
                    break;
                    
                default:
                    quitTutorial();
                    break;
            }
            tutorialArrow.Refresh();
            tutorialWindow.Refresh();
        }
    }    
}
