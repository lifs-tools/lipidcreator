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

    [Serializable]
    public partial class CreatorGUI : Form
    {
        public bool changingTabForced;
        public ArrayList lipidTabList;
        public int currentTabIndex = 1;
        public LipidCreator lipidCreator;
        public AboutDialog aboutDialog;
        public Lipid currentLipid;
        public DataTable registeredLipidsDatatable;
        public int[] lipidModifications;
        public Color alertColor = Color.FromArgb(255, 180, 180);
        public bool settingListbox = false;
        public Dictionary<System.Windows.Forms.MenuItem, string> predefinedFiles;
        public ArrayList tabList;
        public LipidCategory currentIndex;
        public string inputParameters;
        public Tutorial tutorial;
        
        
        public CreatorGUI(string inputParameters)
        {
            this.inputParameters = inputParameters;
            this.lipidCreator = new LipidCreator(this.inputParameters);
            currentIndex = LipidCategory.NoLipid;
            resetAllLipids();
            
            
            registeredLipidsDatatable = new DataTable("Daten");
            registeredLipidsDatatable.Columns.Add(new DataColumn("Category"));
            registeredLipidsDatatable.Columns.Add(new DataColumn("Building Block 1"));
            registeredLipidsDatatable.Columns.Add(new DataColumn("Building Block 2"));
            registeredLipidsDatatable.Columns.Add(new DataColumn("Building Block 3"));
            registeredLipidsDatatable.Columns.Add(new DataColumn("Building Block 4"));
            registeredLipidsDatatable.Columns.Add(new DataColumn("Adducts"));
            InitializeComponent();
            lipidModifications = Enumerable.Repeat(-1, Enum.GetNames(typeof(LipidCategory)).Length).ToArray();
            changingTabForced = false;
            if(Directory.Exists("predefined")) 
            {
                string [] subdirectoryEntries = Directory.GetDirectories("predefined");
                foreach (string subdirectoryEntry in subdirectoryEntries)
                {
                    string subEntry = subdirectoryEntry.Replace("predefined" + Path.DirectorySeparatorChar, "");   
                    System.Windows.Forms.MenuItem predefFolder = new System.Windows.Forms.MenuItem();
                    predefFolder.Text = subEntry;
                    menuImportPredefined.MenuItems.Add(predefFolder);
                    string [] subdirectoryFiles = Directory.GetFiles(subdirectoryEntry);
                    foreach(string subdirectoryFile in subdirectoryFiles)
                    {
                        string subFile = subdirectoryFile.Replace(subdirectoryEntry + Path.DirectorySeparatorChar, "");
                        string upperFile = subFile.ToUpper();
                        if (upperFile.EndsWith(".LCXML"))
                        {
                            System.Windows.Forms.MenuItem predefFile = new System.Windows.Forms.MenuItem();
                            predefFile.Text = subFile.Remove(subFile.Length - 6);
                            predefFile.Tag = subdirectoryFile;
                            predefFile.Click += new System.EventHandler (menuImportPredefinedClick);
                            predefFolder.MenuItems.Add(predefFile);
                        }
                    }
                }
            }
            tabList = new ArrayList(new TabPage[] {homeTab, glycerolipidsTab, phospholipidsTab, sphingolipidsTab, cholesterollipidsTab, mediatorlipidsTab});
            tutorial = new Tutorial(this);
            changeTab(0);
        }
        
        public void resetLipidCreator(Object sender, EventArgs e)
        {
            DialogResult mbr = MessageBox.Show ("Are you sure to reset complete LipidCreator? All information and settings will be discarded.", "Reset LipidCreator", MessageBoxButtons.YesNo);
            if (mbr == DialogResult.Yes) {
                lipidCreator = new LipidCreator(inputParameters);
                resetAllLipids();
                refreshRegisteredLipidsTable();
                changeTab((int)currentIndex);
            }
        }
        
        public void resetAllLipids()
        {
            lipidTabList = new ArrayList(new Lipid[] {null,
                                                      new GLLipid(lipidCreator),
                                                      new PLLipid(lipidCreator),
                                                      new SLLipid(lipidCreator),
                                                      new Cholesterol(lipidCreator),
                                                      new Mediator(lipidCreator)
                                                      });
        }
        
        private void lipidsGridviewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (initialCall){
                int numCols = registeredLipidsDatatable.Columns.Count;
                DataGridViewImageColumn editColumn = new DataGridViewImageColumn();  
                editColumn.Name = "Edit";  
                editColumn.HeaderText = "Edit";  
                editColumn.ValuesAreIcons = false;
                lipidsGridview.Columns.Add(editColumn);
                DataGridViewImageColumn deleteColumn = new DataGridViewImageColumn();  
                deleteColumn.Name = "Delete";  
                deleteColumn.HeaderText = "Delete";  
                deleteColumn.ValuesAreIcons = false;
                lipidsGridview.Columns.Add(deleteColumn);
                int w = (lipidsGridview.Width - 80) / numCols - 4;
                foreach (DataGridViewColumn col in lipidsGridview.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    col.Width = w;
                }
                editColumn.Width = 40;
                deleteColumn.Width = 40;
                initialCall = false;
            }
        }
        
        public void changeTab(int index)
        {
            changingTabForced = true;
            currentTabIndex = index;
            if (index == (int)LipidCategory.PhosphoLipid && tutorial.tutorial == Tutorials.TutorialPRM && tutorial.tutorialStep == 2 && tutorial.forward)
            {
                currentTabIndex = (int)tutorial.currentTab;
                tutorial.nextTutorialStep(true);
                tabControl.SelectedIndex = currentTabIndex;
                changeTabElements(currentTabIndex);
            }
            else if (tutorial.tutorial != Tutorials.NoTutorial)
            {
                currentTabIndex = (int)tutorial.currentTab;
                tabControl.SelectedIndex = currentTabIndex;
            }
            else {
                tabControl.SelectedIndex = currentTabIndex;
                changeTabElements(currentTabIndex);
            }
            changingTabForced = false;
        }
        
        public void tabIndexChanged(Object sender,  EventArgs e)
        {
            if (changingTabForced) return;
            currentTabIndex = ((TabControl)sender).SelectedIndex;
            changeTab(currentTabIndex);            
        }

        public void changeTabElements(int index)
        {
            currentLipid = (Lipid)lipidTabList[index];
            currentIndex = (LipidCategory)index;
            switch((LipidCategory)index)
            {
                case LipidCategory.GlyceroLipid:
                    GLLipid currentGLLipid = (GLLipid)currentLipid;
                    settingListbox = true;
                    for (int i = 0; i < glHgListbox.Items.Count; ++i)
                    {
                        glHgListbox.SetSelected(i, false);
                    }
                    foreach (string headgroup in currentGLLipid.headGroupNames)
                    {
                        var i = 0;
                        foreach (var item in glHgListbox.Items)
                        {
                            if (item.ToString().Equals(headgroup)) 
                            {
                                glHgListbox.SetSelected(i, true);
                                break;
                            }
                            ++i;
                        }
                    }
                    settingListbox = false;
                    
                    
                    glFA1Textbox.Text = currentGLLipid.fag1.lengthInfo;
                    glDB1Textbox.Text = currentGLLipid.fag1.dbInfo;
                    glHydroxyl1Textbox.Text = currentGLLipid.fag1.hydroxylInfo;
                    glFA1Combobox.SelectedIndex = currentGLLipid.fag1.chainType;
                    glFA1Checkbox1.Checked = currentGLLipid.fag1.faTypes["FA"];
                    glFA1Checkbox2.Checked = currentGLLipid.fag1.faTypes["FAp"];
                    glFA1Checkbox3.Checked = currentGLLipid.fag1.faTypes["FAe"];
                    
                    glFA2Textbox.Text = currentGLLipid.fag2.lengthInfo;
                    glDB2Textbox.Text = currentGLLipid.fag2.dbInfo;
                    glHydroxyl2Textbox.Text = currentGLLipid.fag2.hydroxylInfo;
                    glFA2Combobox.SelectedIndex = currentGLLipid.fag2.chainType;
                    glFA2Checkbox1.Checked = currentGLLipid.fag2.faTypes["FA"];
                    glFA2Checkbox2.Checked = currentGLLipid.fag2.faTypes["FAp"];
                    glFA2Checkbox3.Checked = currentGLLipid.fag2.faTypes["FAe"];
                    
                    glFA3Textbox.Text = currentGLLipid.fag3.lengthInfo;
                    glDB3Textbox.Text = currentGLLipid.fag3.dbInfo;
                    glHydroxyl3Textbox.Text = currentGLLipid.fag3.hydroxylInfo;
                    glFA3Combobox.SelectedIndex = currentGLLipid.fag3.chainType;
                    glFA3Checkbox1.Checked = currentGLLipid.fag3.faTypes["FA"];
                    glFA3Checkbox2.Checked = currentGLLipid.fag3.faTypes["FAp"];
                    glFA3Checkbox3.Checked = currentGLLipid.fag3.faTypes["FAe"];
                    
                    glPosAdductCheckbox1.Checked = currentGLLipid.adducts["+H"];
                    glPosAdductCheckbox2.Checked = currentGLLipid.adducts["+2H"];
                    glPosAdductCheckbox3.Checked = currentGLLipid.adducts["+NH4"];
                    glNegAdductCheckbox1.Checked = currentGLLipid.adducts["-H"];
                    glNegAdductCheckbox2.Checked = currentGLLipid.adducts["-2H"];
                    glNegAdductCheckbox3.Checked = currentGLLipid.adducts["+HCOO"];
                    glNegAdductCheckbox4.Checked = currentGLLipid.adducts["+CH3COO"];
                    addLipidButton.Text = "Add glycerolipids";
                    
                    glContainsSugar.Checked = currentGLLipid.containsSugar;
                    
                    
                    updateRanges(currentGLLipid.fag1, glFA1Textbox, glFA1Combobox.SelectedIndex);
                    updateRanges(currentGLLipid.fag1, glDB1Textbox, 3);
                    updateRanges(currentGLLipid.fag1, glHydroxyl1Textbox, 4);
                    updateRanges(currentGLLipid.fag2, glFA2Textbox, glFA2Combobox.SelectedIndex);
                    updateRanges(currentGLLipid.fag2, glDB2Textbox, 3);
                    updateRanges(currentGLLipid.fag2, glHydroxyl2Textbox, 4);
                    updateRanges(currentGLLipid.fag3, glFA3Textbox, glFA3Combobox.SelectedIndex);
                    updateRanges(currentGLLipid.fag3, glDB3Textbox, 3);
                    updateRanges(currentGLLipid.fag3, glHydroxyl3Textbox, 4);
                    
                    glRepresentativeFA.Checked = currentGLLipid.representativeFA;
                    glPictureBox.SendToBack();
                    break;
                    
                case LipidCategory.PhosphoLipid:
                    PLLipid currentPLLipid = (PLLipid)currentLipid;
                    if (currentPLLipid.isCL) // Cardiolipin
                    {
                        clFA1Textbox.Text = currentPLLipid.fag1.lengthInfo;
                        clDB1Textbox.Text = currentPLLipid.fag1.dbInfo;
                        clHydroxyl1Textbox.Text = currentPLLipid.fag1.hydroxylInfo;
                        clFA1Combobox.SelectedIndex = currentPLLipid.fag1.chainType;
                        clFA1Checkbox1.Checked = currentPLLipid.fag1.faTypes["FA"];
                        clFA1Checkbox2.Checked = currentPLLipid.fag1.faTypes["FAp"];
                        clFA1Checkbox3.Checked = currentPLLipid.fag1.faTypes["FAe"];
                        
                        clFA2Textbox.Text = currentPLLipid.fag2.lengthInfo;
                        clDB2Textbox.Text = currentPLLipid.fag2.dbInfo;
                        clHydroxyl2Textbox.Text = currentPLLipid.fag2.hydroxylInfo;
                        clFA2Combobox.SelectedIndex = currentPLLipid.fag2.chainType;
                        clFA2Checkbox1.Checked = currentPLLipid.fag2.faTypes["FA"];
                        clFA2Checkbox2.Checked = currentPLLipid.fag2.faTypes["FAp"];
                        clFA2Checkbox3.Checked = currentPLLipid.fag2.faTypes["FAe"];
                        
                        clFA3Textbox.Text = currentPLLipid.fag3.lengthInfo;
                        clDB3Textbox.Text = currentPLLipid.fag3.dbInfo;
                        clHydroxyl3Textbox.Text = currentPLLipid.fag3.hydroxylInfo;
                        clFA3Combobox.SelectedIndex = currentPLLipid.fag3.chainType;
                        clFA3Checkbox1.Checked = currentPLLipid.fag3.faTypes["FA"];
                        clFA3Checkbox2.Checked = currentPLLipid.fag3.faTypes["FAp"];
                        clFA3Checkbox3.Checked = currentPLLipid.fag3.faTypes["FAe"];
                        
                        clFA4Textbox.Text = currentPLLipid.fag4.lengthInfo;
                        clDB4Textbox.Text = currentPLLipid.fag4.dbInfo;
                        clHydroxyl4Textbox.Text = currentPLLipid.fag4.hydroxylInfo;
                        clFA4Combobox.SelectedIndex = currentPLLipid.fag4.chainType;
                        clFA4Checkbox1.Checked = currentPLLipid.fag4.faTypes["FA"];
                        clFA4Checkbox2.Checked = currentPLLipid.fag4.faTypes["FAp"];
                        clFA4Checkbox3.Checked = currentPLLipid.fag4.faTypes["FAe"];
                        
                        clPosAdductCheckbox1.Checked = currentPLLipid.adducts["+H"];
                        clPosAdductCheckbox2.Checked = currentPLLipid.adducts["+2H"];
                        clPosAdductCheckbox3.Checked = currentPLLipid.adducts["+NH4"];
                        clNegAdductCheckbox1.Checked = currentPLLipid.adducts["-H"];
                        clNegAdductCheckbox2.Checked = currentPLLipid.adducts["-2H"];
                        clNegAdductCheckbox3.Checked = currentPLLipid.adducts["+HCOO"];
                        clNegAdductCheckbox4.Checked = currentPLLipid.adducts["+CH3COO"];
                        addLipidButton.Text = "Add cardiolipins";
                        
                        plIsCL.Checked = true;
                        
                        updateRanges(currentPLLipid.fag1, clFA1Textbox, clFA1Combobox.SelectedIndex);
                        updateRanges(currentPLLipid.fag1, clDB1Textbox, 3);
                        updateRanges(currentPLLipid.fag1, clHydroxyl1Textbox, 4);
                        updateRanges(currentPLLipid.fag2, clFA2Textbox, clFA2Combobox.SelectedIndex);
                        updateRanges(currentPLLipid.fag2, clDB2Textbox, 3);
                        updateRanges(currentPLLipid.fag2, clHydroxyl2Textbox, 4);
                        updateRanges(currentPLLipid.fag3, clFA3Textbox, clFA3Combobox.SelectedIndex);
                        updateRanges(currentPLLipid.fag3, clDB3Textbox, 3);
                        updateRanges(currentPLLipid.fag3, clHydroxyl3Textbox, 4);
                        updateRanges(currentPLLipid.fag4, clFA4Textbox, clFA4Combobox.SelectedIndex);
                        updateRanges(currentPLLipid.fag4, clDB4Textbox, 3);
                        updateRanges(currentPLLipid.fag4, clHydroxyl4Textbox, 4);
                        
                        clRepresentativeFA.Checked = currentPLLipid.representativeFA;
                        clPictureBox.SendToBack();
                    }
                    
                    else // Phospholipid
                    {
                    
                        settingListbox = true;
                        for (int i = 0; i < plHgListbox.Items.Count; ++i)
                        {
                            plHgListbox.SetSelected(i, false);
                        }
                        foreach (string headgroup in currentPLLipid.headGroupNames)
                        {
                            var i = 0;
                            foreach (var item in plHgListbox.Items)
                            {
                                if (item.ToString().Equals(headgroup)) 
                                {
                                    plHgListbox.SetSelected(i, true);
                                    break;
                                }
                                ++i;
                            }
                        }
                        settingListbox = false;
                        
                        plFA1Textbox.Text = currentPLLipid.fag1.lengthInfo;
                        plDB1Textbox.Text = currentPLLipid.fag1.dbInfo;
                        plHydroxyl1Textbox.Text = currentPLLipid.fag1.hydroxylInfo;
                        plFA1Combobox.SelectedIndex = currentPLLipid.fag1.chainType;
                        plFA1Checkbox1.Checked = currentPLLipid.fag1.faTypes["FA"];
                        plFA1Checkbox2.Checked = currentPLLipid.fag1.faTypes["FAp"];
                        plFA1Checkbox3.Checked = currentPLLipid.fag1.faTypes["FAe"];
                        
                        plFA2Textbox.Text = currentPLLipid.fag2.lengthInfo;
                        plDB2Textbox.Text = currentPLLipid.fag2.dbInfo;
                        plHydroxyl2Textbox.Text = currentPLLipid.fag2.hydroxylInfo;
                        plFA2Combobox.SelectedIndex = currentPLLipid.fag2.chainType;
                        plFA2Checkbox1.Checked = currentPLLipid.fag2.faTypes["FA"];
                        plFA2Checkbox2.Checked = currentPLLipid.fag2.faTypes["FAp"];
                        plFA2Checkbox3.Checked = currentPLLipid.fag2.faTypes["FAe"];
                    
                        
                        plPosAdductCheckbox1.Checked = currentPLLipid.adducts["+H"];
                        plPosAdductCheckbox2.Checked = currentPLLipid.adducts["+2H"];
                        plPosAdductCheckbox3.Checked = currentPLLipid.adducts["+NH4"];
                        plNegAdductCheckbox1.Checked = currentPLLipid.adducts["-H"];
                        plNegAdductCheckbox2.Checked = currentPLLipid.adducts["-2H"];
                        plNegAdductCheckbox3.Checked = currentPLLipid.adducts["+HCOO"];
                        plNegAdductCheckbox4.Checked = currentPLLipid.adducts["+CH3COO"];
                        addLipidButton.Text = "Add phospholipids";
                        plIsCL.Checked = false;
                        
                        
                        updateRanges(currentPLLipid.fag1, plFA1Textbox, plFA1Combobox.SelectedIndex);
                        updateRanges(currentPLLipid.fag1, plDB1Textbox, 3);
                        updateRanges(currentPLLipid.fag1, plHydroxyl1Textbox, 4);
                        updateRanges(currentPLLipid.fag2, plFA2Textbox, plFA2Combobox.SelectedIndex);
                        updateRanges(currentPLLipid.fag2, plDB2Textbox, 3);
                        updateRanges(currentPLLipid.fag2, plHydroxyl2Textbox, 4);
                        
                        plRepresentativeFA.Checked = currentPLLipid.representativeFA;
                        plPictureBox.SendToBack();
                    }
                    break;
                    
                case LipidCategory.SphingoLipid:
                    SLLipid currentSLLipid = (SLLipid)currentLipid;
                    settingListbox = true;
                    for (int i = 0; i < slHgListbox.Items.Count; ++i)
                    {
                        slHgListbox.SetSelected(i, false);
                    }
                    foreach (string headgroup in currentSLLipid.headGroupNames)
                    {
                        var i = 0;
                        foreach (var item in slHgListbox.Items)
                        {
                            if (item.ToString().Equals(headgroup)) 
                            {
                                slHgListbox.SetSelected(i, true);
                                break;
                            }
                            ++i;
                        }
                    }
                    settingListbox = false;
                    
                    
                    slLCBTextbox.Text = currentSLLipid.lcb.lengthInfo;
                    slDB2Textbox.Text = currentSLLipid.lcb.dbInfo;
                    slLCBCombobox.SelectedIndex = currentSLLipid.lcb.chainType;
                    slLCBHydroxyCombobox.SelectedIndex = currentSLLipid.lcb.hydroxylCounts.First() - 2;
                    slFAHydroxyCombobox.SelectedIndex = currentSLLipid.fag.hydroxylCounts.First();
                    
                    slFATextbox.Text = currentSLLipid.fag.lengthInfo;
                    slDB1Textbox.Text = currentSLLipid.fag.dbInfo;
                    slFACombobox.SelectedIndex = currentSLLipid.fag.chainType;
                
                    
                    slPosAdductCheckbox1.Checked = currentSLLipid.adducts["+H"];
                    slPosAdductCheckbox2.Checked = currentSLLipid.adducts["+2H"];
                    slPosAdductCheckbox3.Checked = currentSLLipid.adducts["+NH4"];
                    slNegAdductCheckbox1.Checked = currentSLLipid.adducts["-H"];
                    slNegAdductCheckbox2.Checked = currentSLLipid.adducts["-2H"];
                    slNegAdductCheckbox3.Checked = currentSLLipid.adducts["+HCOO"];
                    slNegAdductCheckbox4.Checked = currentSLLipid.adducts["+CH3COO"];
                    addLipidButton.Text = "Add sphingolipids";
                    
                    
                    updateRanges(currentSLLipid.lcb, slLCBTextbox, slLCBCombobox.SelectedIndex, true);
                    updateRanges(currentSLLipid.lcb, slDB2Textbox, 3);
                    updateRanges(currentSLLipid.fag, slFATextbox, slFACombobox.SelectedIndex);
                    updateRanges(currentSLLipid.fag, slDB1Textbox, 3);
                    slPictureBox.SendToBack();
                    break;
                    
                case LipidCategory.Cholesterol:
                    Cholesterol currentCHLipid = (Cholesterol)currentLipid;
                    chPosAdductCheckbox1.Checked = currentCHLipid.adducts["+H"];
                    chPosAdductCheckbox2.Checked = currentCHLipid.adducts["+2H"];
                    chPosAdductCheckbox3.Checked = currentCHLipid.adducts["+NH4"];
                    chNegAdductCheckbox1.Checked = currentCHLipid.adducts["-H"];
                    chNegAdductCheckbox2.Checked = currentCHLipid.adducts["-2H"];
                    chNegAdductCheckbox3.Checked = currentCHLipid.adducts["+HCOO"];
                    chNegAdductCheckbox4.Checked = currentCHLipid.adducts["+CH3COO"];
                    addLipidButton.Text = "Add cholesterol";
                    chContainsEster.Checked = currentCHLipid.containsEster;
                    
                    chFATextbox.Text = currentCHLipid.fag.lengthInfo;
                    chDBTextbox.Text = currentCHLipid.fag.dbInfo;
                    chHydroxylTextbox.Text = currentCHLipid.fag.hydroxylInfo;
                    chFACombobox.SelectedIndex = currentCHLipid.fag.chainType;
                    updateRanges(currentCHLipid.fag, chFATextbox, chFACombobox.SelectedIndex);
                    updateRanges(currentCHLipid.fag, chDBTextbox, 3);
                    updateRanges(currentCHLipid.fag, chHydroxylTextbox, 4);
                    break;
                    
                case LipidCategory.Mediator:
                    Mediator currentMedLipid = (Mediator)currentLipid;
                    settingListbox = true;
                    for (int i = 0; i < medHgListbox.Items.Count; ++i)
                    {
                        medHgListbox.SetSelected(i, false);
                    }
                    foreach (string headgroup in currentMedLipid.headGroupNames)
                    {
                        var i = 0;
                        foreach (var item in medHgListbox.Items)
                        {
                            if (item.ToString().Equals(headgroup)) 
                            {
                                medHgListbox.SetSelected(i, true);
                                break;
                            }
                            ++i;
                        }
                    }
                    settingListbox = false;
                    medNegAdductCheckbox1.Checked = currentMedLipid.adducts["-H"];
                    medNegAdductCheckbox2.Checked = currentMedLipid.adducts["-2H"];
                    medNegAdductCheckbox3.Checked = currentMedLipid.adducts["+HCOO"];
                    medNegAdductCheckbox4.Checked = currentMedLipid.adducts["+CH3COO"];
                    addLipidButton.Text = "Add mediator";
                    break;
                    
                default:
                    break;
            }
            
            menuResetCategory.Enabled = currentIndex != LipidCategory.NoLipid;
            menuMS2Fragments.Enabled = currentIndex != LipidCategory.NoLipid;
            menuIsotopes.Enabled = currentIndex != LipidCategory.NoLipid;
            
            if (currentLipid != null)
            {
                ((TabPage)tabList[index]).Controls.Add(MS2fragmentsLipidButton);
                ((TabPage)tabList[index]).Controls.Add(addHeavyIsotopeButton);
                ((TabPage)tabList[index]).Controls.Add(modifyLipidButton);
                modifyLipidButton.Enabled = lipidModifications[(int)currentIndex] > -1;
                ((TabPage)tabList[index]).Controls.Add(addLipidButton);
            }
        }
        
        
        public void resetLipid(Object sender, EventArgs e)
        {
        
            if (currentIndex == LipidCategory.NoLipid) return;
            Lipid newLipid = null;
            int index = (int)currentIndex;
            switch (index)
            {
                case (int)LipidCategory.GlyceroLipid:
                    newLipid = new GLLipid(lipidCreator);
                    break;
                    
                case (int)LipidCategory.PhosphoLipid:
                    newLipid = new PLLipid(lipidCreator);
                    ((PLLipid)newLipid).isCL = plIsCL.Checked;
                    break;
                    
                case (int)LipidCategory.SphingoLipid:
                    newLipid = new SLLipid(lipidCreator);
                    break;
                    
                case (int)LipidCategory.Cholesterol:
                    newLipid = new Cholesterol(lipidCreator);
                    break;
                    
                case (int)LipidCategory.Mediator:
                    newLipid = new Mediator(lipidCreator);
                    break;
                    
                default:
                    break;                
            }
            lipidTabList[index] = newLipid;
            lipidModifications[index] = -1;
            changeTab(index);
        }
        
        
        /////////////////////// CL //////////////////////////////
        
        
        public void clFA1ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((PLLipid)currentLipid).fag1, clFA1Textbox, ((ComboBox)sender).SelectedIndex);
            if (((PLLipid)currentLipid).representativeFA)
            {
                clFA2Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                clFA3Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                clFA4Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
            }
        }
        public void clFA2ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((PLLipid)currentLipid).fag2, clFA2Textbox, ((ComboBox)sender).SelectedIndex);
        }
        public void clFA3ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag3.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((PLLipid)currentLipid).fag3, clFA3Textbox, ((ComboBox)sender).SelectedIndex);
        }
        public void clFA4ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag4.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((PLLipid)currentLipid).fag4, clFA4Textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        
        
        public void updateRanges(FattyAcidGroup fag, TextBox tb, int objectType)
        {
            updateRanges(fag, tb, objectType, false);
        }
        
        // objectType (Object type): 0 = carbon length, 1 = carbon length odd, 2 = carbon length even, 3 = db length, 4 = hydroxyl length
        public void updateRanges(FattyAcidGroup fag, TextBox tb, int objectType, bool isLCB)
        {
            int maxRange = 30;
            int minRange = 0;
            if (objectType < 3) minRange = 2;
            if (objectType == 3) maxRange = 6;
            else if (objectType == 4) maxRange = 29;
            if (isLCB) minRange = 8;
            HashSet<int> lengths = lipidCreator.parseRange(tb.Text, minRange,  maxRange, objectType);
            if (objectType <= 2)
            {
                fag.carbonCounts = lengths; 
            }
            else if (objectType == 3)
            { 
                fag.doubleBondCounts = lengths;          
            }
            else if (objectType == 4)
            {
                fag.hydroxylCounts = lengths;
            }
            tb.BackColor = (lengths == null) ? alertColor : Color.White;
        }
        
        public void clFA1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag1, (TextBox)sender, clFA1Combobox.SelectedIndex);
            if (((PLLipid)currentLipid).representativeFA)
            {
                clFA2Textbox.Text = ((TextBox)sender).Text;
                clFA3Textbox.Text = ((TextBox)sender).Text;
                clFA4Textbox.Text = ((TextBox)sender).Text;
            }
        }
        
        public void clFA2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag2, (TextBox)sender, clFA2Combobox.SelectedIndex);
        }
        
        public void clFA3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag3.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag3, (TextBox)sender, clFA3Combobox.SelectedIndex);
        }
        
        public void clFA4TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag4.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag4, (TextBox)sender, clFA4Combobox.SelectedIndex);
        }
        
        public void clDB1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag1, (TextBox)sender, 3);
            if (((PLLipid)currentLipid).representativeFA)
            {
                clDB2Textbox.Text = ((TextBox)sender).Text;
                clDB3Textbox.Text = ((TextBox)sender).Text;
                clDB4Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void clDB2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag2, (TextBox)sender, 3);
        }
        public void clDB3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag3.dbInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag3, (TextBox)sender, 3);
        }
        public void clDB4TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag4.dbInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag4, (TextBox)sender, 3);
        }
        
        public void clHydroxyl1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag1, (TextBox)sender, 4);
            if (((PLLipid)currentLipid).representativeFA)
            {
                clHydroxyl2Textbox.Text = ((TextBox)sender).Text;
                clHydroxyl3Textbox.Text = ((TextBox)sender).Text;
                clHydroxyl4Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void clHydroxyl2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag2, (TextBox)sender, 4);
        }
        public void clHydroxyl3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag3.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag3, (TextBox)sender, 4);
        }
        public void clHydroxyl4TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag4.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag4, (TextBox)sender, 4);
        }
        
        public void clFA1Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag1.faTypes["FAx"] = !((PLLipid)currentLipid).fag1.anyFAChecked();
            if (((PLLipid)currentLipid).representativeFA)
            {
                clFA2Checkbox1.Checked = ((CheckBox)sender).Checked;
                clFA3Checkbox1.Checked = ((CheckBox)sender).Checked;
                clFA4Checkbox1.Checked = ((CheckBox)sender).Checked;
            }
        }
        public void clFA1Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag1.faTypes["FAx"] = !((PLLipid)currentLipid).fag1.anyFAChecked();
            if (((PLLipid)currentLipid).representativeFA)
            {
                clFA2Checkbox2.Checked = ((CheckBox)sender).Checked;
                clFA3Checkbox2.Checked = ((CheckBox)sender).Checked;
                clFA4Checkbox2.Checked = ((CheckBox)sender).Checked;
            }
        }
        public void clFA1Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag1.faTypes["FAx"] = !((PLLipid)currentLipid).fag1.anyFAChecked();
            if (((PLLipid)currentLipid).representativeFA)
            {
                clFA2Checkbox3.Checked = ((CheckBox)sender).Checked;
                clFA3Checkbox3.Checked = ((CheckBox)sender).Checked;
                clFA4Checkbox3.Checked = ((CheckBox)sender).Checked;
            }
        }
        
        public void clFA2Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag2.faTypes["FAx"] = !((PLLipid)currentLipid).fag2.anyFAChecked();
        }
        public void clFA2Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag2.faTypes["FAx"] = !((PLLipid)currentLipid).fag2.anyFAChecked();
        }
        public void clFA2Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag2.faTypes["FAx"] = !((PLLipid)currentLipid).fag2.anyFAChecked();
        }
        
        public void clFA3Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag3.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag3.faTypes["FAx"] = !((PLLipid)currentLipid).fag3.anyFAChecked();
        }
        public void clFA3Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag3.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag3.faTypes["FAx"] = !((PLLipid)currentLipid).fag3.anyFAChecked();
        }
        public void clFA3Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag3.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag3.faTypes["FAx"] = !((PLLipid)currentLipid).fag3.anyFAChecked();
        }
        
        public void clFA4Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag4.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag4.faTypes["FAx"] = !((PLLipid)currentLipid).fag4.anyFAChecked();
        }
        public void clFA4Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag4.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag4.faTypes["FAx"] = !((PLLipid)currentLipid).fag4.anyFAChecked();
        }
        public void clFA4Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag4.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag4.faTypes["FAx"] = !((PLLipid)currentLipid).fag4.anyFAChecked();
        }
        
        public void clPosAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        public void clPosAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        public void clPosAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        public void clNegAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        public void clNegAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        public void clNegAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        public void clNegAdductCheckbox4CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
        }
        
        
        
        void clFA1Checkbox3MouseLeave(object sender, EventArgs e)
        {
            clPictureBox.Image = cardioBackboneImage;
            clPictureBox.SendToBack();
        }
        private void clFA1Checkbox3MouseHover(object sender, MouseEventArgs e)
        {
            clPictureBox.Image = cardioBackboneImageFA1e;
            clPictureBox.SendToBack();
        }
        
        void clFA1Checkbox2MouseLeave(object sender, EventArgs e)
        {
            clPictureBox.Image = cardioBackboneImage;
            clPictureBox.SendToBack();
        }
        private void clFA1Checkbox2MouseHover(object sender, MouseEventArgs e)
        {
            clPictureBox.Image = cardioBackboneImageFA1p;
            clPictureBox.SendToBack();
        }
        
        void clFA2Checkbox3MouseLeave(object sender, EventArgs e)
        {
            clPictureBox.Image = cardioBackboneImage;
            clPictureBox.SendToBack();
        }
        private void clFA2Checkbox3MouseHover(object sender, MouseEventArgs e)
        {
            clPictureBox.Image = cardioBackboneImageFA2e;
            clPictureBox.SendToBack();
        }
        void clFA2Checkbox2MouseLeave(object sender, EventArgs e)
        {
            clPictureBox.Image = cardioBackboneImage;
            clPictureBox.SendToBack();
        }

        private void clFA2Checkbox2MouseHover(object sender, MouseEventArgs e)
        {
            clPictureBox.Image = cardioBackboneImageFA2p;
            clPictureBox.SendToBack();
        }
        
        void clFA3Checkbox3MouseLeave(object sender, EventArgs e)
        {
            clPictureBox.Image = cardioBackboneImage;
            clPictureBox.SendToBack();
        }
        private void clFA3Checkbox3MouseHover(object sender, MouseEventArgs e)
        {
            clPictureBox.Image = cardioBackboneImageFA3e;
            clPictureBox.SendToBack();
        }
        
        void clFA3Checkbox2MouseLeave(object sender, EventArgs e)
        {
            clPictureBox.Image = cardioBackboneImage;
            clPictureBox.SendToBack();
        }
        private void clFA3Checkbox2MouseHover(object sender, MouseEventArgs e)
        {
            clPictureBox.Image = cardioBackboneImageFA3p;
            clPictureBox.SendToBack();
        }
        
        void clFA4Checkbox3MouseLeave(object sender, EventArgs e)
        {
            clPictureBox.Image = cardioBackboneImage;
            clPictureBox.SendToBack();
        }
        private void clFA4Checkbox3MouseHover(object sender, MouseEventArgs e)
        {
            clPictureBox.Image = cardioBackboneImageFA4e;
            clPictureBox.SendToBack();
        }
        void clFA4Checkbox2MouseLeave(object sender, EventArgs e)
        {
            clPictureBox.Image = cardioBackboneImage;
            clPictureBox.SendToBack();
        }

        private void clFA4Checkbox2MouseHover(object sender, MouseEventArgs e)
        {
            clPictureBox.Image = cardioBackboneImageFA4p;
            clPictureBox.SendToBack();
        }
        
        public void clRepresentativeFACheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).representativeFA = ((CheckBox)sender).Checked;
            if (((PLLipid)currentLipid).representativeFA)
            {
                clFA2Textbox.Enabled = false;
                clDB2Textbox.Enabled = false;
                clHydroxyl2Textbox.Enabled = false;
                clFA2Combobox.Enabled = false;
                clFA2Checkbox1.Enabled = false;
                clFA2Checkbox2.Enabled = false;
                clFA2Checkbox3.Enabled = false;
                clFA3Textbox.Enabled = false;
                clDB3Textbox.Enabled = false;
                clHydroxyl3Textbox.Enabled = false;
                clFA3Combobox.Enabled = false;
                clFA3Checkbox1.Enabled = false;
                clFA3Checkbox2.Enabled = false;
                clFA3Checkbox3.Enabled = false;
                clFA4Textbox.Enabled = false;
                clDB4Textbox.Enabled = false;
                clHydroxyl4Textbox.Enabled = false;
                clFA4Combobox.Enabled = false;
                clFA4Checkbox1.Enabled = false;
                clFA4Checkbox2.Enabled = false;
                clFA4Checkbox3.Enabled = false;
                
                clFA2Textbox.Text = clFA1Textbox.Text;
                clFA3Textbox.Text = clFA1Textbox.Text;
                clFA4Textbox.Text = clFA1Textbox.Text;
                clDB2Textbox.Text = clDB1Textbox.Text;
                clDB3Textbox.Text = clDB1Textbox.Text;
                clDB4Textbox.Text = clDB1Textbox.Text;
                clHydroxyl2Textbox.Text = clHydroxyl1Textbox.Text;
                clHydroxyl3Textbox.Text = clHydroxyl1Textbox.Text;
                clHydroxyl4Textbox.Text = clHydroxyl1Textbox.Text;
                clFA2Combobox.Text = clFA1Combobox.Text;
                clFA3Combobox.Text = clFA1Combobox.Text;
                clFA4Combobox.Text = clFA1Combobox.Text;
                clFA2Checkbox1.Checked = clFA1Checkbox1.Checked;
                clFA3Checkbox1.Checked = clFA1Checkbox1.Checked;
                clFA4Checkbox1.Checked = clFA1Checkbox1.Checked;
                clFA2Checkbox2.Checked = clFA1Checkbox2.Checked;
                clFA3Checkbox2.Checked = clFA1Checkbox2.Checked;
                clFA4Checkbox2.Checked = clFA1Checkbox2.Checked;
                clFA2Checkbox3.Checked = clFA1Checkbox3.Checked;
                clFA3Checkbox3.Checked = clFA1Checkbox3.Checked;
                clFA4Checkbox3.Checked = clFA1Checkbox3.Checked;
            }
            else
            {
                clFA2Textbox.Enabled = true;
                clDB2Textbox.Enabled = true;
                clHydroxyl2Textbox.Enabled = true;
                clFA2Combobox.Enabled = true;
                clFA2Checkbox1.Enabled = true;
                clFA2Checkbox2.Enabled = true;
                clFA2Checkbox3.Enabled = true;
                clFA3Textbox.Enabled = true;
                clDB3Textbox.Enabled = true;
                clHydroxyl3Textbox.Enabled = true;
                clFA3Combobox.Enabled = true;
                clFA3Checkbox1.Enabled = true;
                clFA3Checkbox2.Enabled = true;
                clFA3Checkbox3.Enabled = true;
                clFA4Textbox.Enabled = true;
                clDB4Textbox.Enabled = true;
                clHydroxyl4Textbox.Enabled = true;
                clFA4Combobox.Enabled = true;
                clFA4Checkbox1.Enabled = true;
                clFA4Checkbox2.Enabled = true;
                clFA4Checkbox3.Enabled = true;
            }
            updateRanges(((PLLipid)currentLipid).fag2, clFA2Textbox, clFA2Combobox.SelectedIndex);
            updateRanges(((PLLipid)currentLipid).fag3, clFA3Textbox, clFA3Combobox.SelectedIndex);
            updateRanges(((PLLipid)currentLipid).fag4, clFA4Textbox, clFA4Combobox.SelectedIndex);
            updateRanges(((PLLipid)currentLipid).fag2, clDB2Textbox, 3);
            updateRanges(((PLLipid)currentLipid).fag3, clDB3Textbox, 3);
            updateRanges(((PLLipid)currentLipid).fag4, clDB4Textbox, 3);
            updateRanges(((PLLipid)currentLipid).fag2, clHydroxyl2Textbox, 4);
            updateRanges(((PLLipid)currentLipid).fag3, clHydroxyl3Textbox, 4);
            updateRanges(((PLLipid)currentLipid).fag4, clHydroxyl4Textbox, 4);
        }
        
        
        
        ////////////////////// GL ////////////////////////////////
        
        public void glFA1ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag1.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((GLLipid)currentLipid).fag1, glFA1Textbox, ((ComboBox)sender).SelectedIndex);
            if (((GLLipid)currentLipid).representativeFA)
            {
                glFA2Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                glFA3Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
            }
        }
        public void glFA2ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((GLLipid)currentLipid).fag2, glFA2Textbox, ((ComboBox)sender).SelectedIndex);
        }
        public void glFA3ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag3.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((GLLipid)currentLipid).fag3, glFA3Textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void glFA1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((GLLipid)currentLipid).fag1, (TextBox)sender, glFA1Combobox.SelectedIndex);
            if (((GLLipid)currentLipid).representativeFA)
            {
                glFA2Textbox.Text = ((TextBox)sender).Text;
                glFA3Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void glFA2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((GLLipid)currentLipid).fag2, (TextBox)sender, glFA2Combobox.SelectedIndex);
        }
        public void glFA3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag3.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((GLLipid)currentLipid).fag3, (TextBox)sender, glFA3Combobox.SelectedIndex);
        }
        
        public void glDB1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            updateRanges(((GLLipid)currentLipid).fag1, (TextBox)sender, 3);
            if (((GLLipid)currentLipid).representativeFA)
            {
                glDB2Textbox.Text = ((TextBox)sender).Text;
                glDB3Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void glDB2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            updateRanges(((GLLipid)currentLipid).fag2, (TextBox)sender, 3);
        }
        public void glDB3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag3.dbInfo = ((TextBox)sender).Text;
            updateRanges(((GLLipid)currentLipid).fag3, (TextBox)sender, 3);
        }
        
        public void glPosAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        public void glPosAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        public void glPosAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        public void glNegAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        public void glNegAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        public void glNegAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        public void glNegAdductCheckbox4CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
        }
        
        
        public void glFA1Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag1.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((GLLipid)currentLipid).fag1.faTypes["FAx"] = !((GLLipid)currentLipid).fag1.anyFAChecked();
            if (((GLLipid)currentLipid).representativeFA)
            {
                glFA2Checkbox1.Checked = ((CheckBox)sender).Checked;
                glFA3Checkbox1.Checked =  ((CheckBox)sender).Checked;
            }
        }
        public void glFA1Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((GLLipid)currentLipid).fag1.faTypes["FAx"] = !((GLLipid)currentLipid).fag1.anyFAChecked();
            if (((GLLipid)currentLipid).representativeFA)
            {
                glFA2Checkbox2.Checked = ((CheckBox)sender).Checked;
                glFA3Checkbox2.Checked =  ((CheckBox)sender).Checked;
            }
        }
        public void glFA1Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag1.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((GLLipid)currentLipid).fag1.faTypes["FAx"] = !((GLLipid)currentLipid).fag1.anyFAChecked();
            if (((GLLipid)currentLipid).representativeFA)
            {
                glFA2Checkbox3.Checked = ((CheckBox)sender).Checked;
                glFA3Checkbox3.Checked =  ((CheckBox)sender).Checked;
            }
        }
        
        public void glFA2Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag2.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((GLLipid)currentLipid).fag2.faTypes["FAx"] = !((GLLipid)currentLipid).fag2.anyFAChecked();
        }
        public void glFA2Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag2.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((GLLipid)currentLipid).fag2.faTypes["FAx"] = !((GLLipid)currentLipid).fag2.anyFAChecked();
        }
        public void glFA2Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag2.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((GLLipid)currentLipid).fag2.faTypes["FAx"] = !((GLLipid)currentLipid).fag2.anyFAChecked();
        }
        
        public void glFA3Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag3.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((GLLipid)currentLipid).fag3.faTypes["FAx"] = !((GLLipid)currentLipid).fag3.anyFAChecked();
        }
        public void glFA3Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag3.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((GLLipid)currentLipid).fag3.faTypes["FAx"] = !((GLLipid)currentLipid).fag3.anyFAChecked();
        }
        public void glFA3Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag3.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((GLLipid)currentLipid).fag3.faTypes["FAx"] = !((GLLipid)currentLipid).fag3.anyFAChecked();
        }
        
        
        public void glHydroxyl1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag1.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((GLLipid)currentLipid).fag1, (TextBox)sender, 4);
            if (((GLLipid)currentLipid).representativeFA)
            {
                glHydroxyl2Textbox.Text = ((TextBox)sender).Text;
                glHydroxyl3Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void glHydroxyl2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag2.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((GLLipid)currentLipid).fag2, (TextBox)sender, 4);
        }
        public void glHydroxyl3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag3.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((GLLipid)currentLipid).fag3, (TextBox)sender, 4);
        }
        
        public void triggerEasteregg(Object sender, EventArgs e)
        {
            easterText.Left = 1030;
            easterText.Visible = true;
            this.timerEasterEgg.Enabled = true;
        }
        
        public void sugarHeady(Object sender, EventArgs e)
        {
            MessageBox.Show("Who is your sugar heady?");
        }
        
        private void timerEasterEggTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            easterText.Left -= 10;
            if (easterText.Left < -easterText.Width){
                this.timerEasterEgg.Enabled = false;
                easterText.Visible = false;
            }
        }
        
        void glFA1Checkbox3MouseLeave(object sender, EventArgs e)
        {
            glPictureBox.Image = glyceroBackboneImage;
            glPictureBox.SendToBack();
        }
        private void glFA1Checkbox3MouseHover(object sender, MouseEventArgs e)
        {
            glPictureBox.Image = glyceroBackboneImageFAe;
            glPictureBox.SendToBack();
        }
        
        void glFA1Checkbox2MouseLeave(object sender, EventArgs e)
        {
            glPictureBox.Image = glyceroBackboneImage;
            glPictureBox.SendToBack();
        }
        private void glFA1Checkbox2MouseHover(object sender, MouseEventArgs e)
        {
            glPictureBox.Image = glyceroBackboneImageFA1p;
            glPictureBox.SendToBack();
        }
        
        void glFA2Checkbox3MouseLeave(object sender, EventArgs e)
        {
            glPictureBox.Image = glyceroBackboneImage;
            glPictureBox.SendToBack();
        }
        private void glFA2Checkbox3MouseHover(object sender, MouseEventArgs e)
        {
            glPictureBox.Image = glyceroBackboneImageFA2e;
            glPictureBox.SendToBack();
        }
        void glFA2Checkbox2MouseLeave(object sender, EventArgs e)
        {
            glPictureBox.Image = glyceroBackboneImage;
            glPictureBox.SendToBack();
        }

        private void glFA2Checkbox2MouseHover(object sender, MouseEventArgs e)
        {
            glPictureBox.Image = glyceroBackboneImageFA2p;
            glPictureBox.SendToBack();
        }
        
        void glFA3Checkbox3MouseLeave(object sender, EventArgs e)
        {
            glPictureBox.Image = glyceroBackboneImage;
            glPictureBox.SendToBack();
        }
        private void glFA3Checkbox3MouseHover(object sender, MouseEventArgs e)
        {
            glPictureBox.Image = glyceroBackboneImageFA3e;
            glPictureBox.SendToBack();
        }
        
        void glFA3Checkbox2MouseLeave(object sender, EventArgs e)
        {
            glPictureBox.Image = glyceroBackboneImage;
            glPictureBox.SendToBack();
        }
        private void glFA3Checkbox2MouseHover(object sender, MouseEventArgs e)
        {
            glPictureBox.Image = glyceroBackboneImageFA3p;
            glPictureBox.SendToBack();
        }
        
        
        private void glHGListboxSelectedValueChanged(object sender, System.EventArgs e)
        {
            if (settingListbox) return;
            currentLipid.headGroupNames.Clear();
            foreach(object itemChecked in ((ListBox)sender).SelectedItems)
            {
                currentLipid.headGroupNames.Add(itemChecked.ToString());
            }
            
        }
        
        public void glContainsSugarCheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).containsSugar = ((CheckBox)sender).Checked;
            
            glPictureBox.Visible = false;
            if (((GLLipid)currentLipid).containsSugar)
            {
                glFA3Textbox.Visible = false;
                glDB3Textbox.Visible = false;
                glHydroxyl3Textbox.Visible = false;
                glFA3Combobox.Visible = false;
                glFA3Checkbox1.Visible = false;
                glFA3Checkbox2.Visible = false;
                glFA3Checkbox3.Visible = false;
                glDB3Label.Visible = false;
                glHydroxyl3Label.Visible = false;
                glPosAdductCheckbox1.Enabled = true;
                glPosAdductCheckbox2.Enabled = false;
                glPosAdductCheckbox3.Enabled = true;
                glNegAdductCheckbox1.Enabled = true;
                glNegAdductCheckbox2.Enabled = false;
                glNegAdductCheckbox3.Enabled = true;
                glNegAdductCheckbox4.Enabled = true;
                
                
                glHgListbox.Visible = true;
                glHGLabel.Visible = true;
                
                glyceroBackboneImage = glyceroBackboneImagePlant;
                glyceroBackboneImageFAe = glyceroBackboneImageFA1ePlant;
                glyceroBackboneImageFA2e = glyceroBackboneImageFA2ePlant;
                glyceroBackboneImageFA1p = glyceroBackboneImageFA1pPlant;
                glyceroBackboneImageFA2p = glyceroBackboneImageFA2pPlant;
            }
            else
            {
                glFA3Textbox.Visible = true;
                glDB3Textbox.Visible = true;
                glHydroxyl3Textbox.Visible = true;
                glFA3Combobox.Visible = true;
                glFA3Checkbox1.Visible = true;
                glFA3Checkbox2.Visible = true;
                glFA3Checkbox3.Visible = true;
                glDB3Label.Visible = true;
                glHydroxyl3Label.Visible = true;
                glPosAdductCheckbox1.Enabled = false;
                glPosAdductCheckbox2.Enabled = false;
                glPosAdductCheckbox3.Enabled = true;
                glNegAdductCheckbox1.Enabled = false;
                glNegAdductCheckbox2.Enabled = false;
                glNegAdductCheckbox3.Enabled = false;
                glNegAdductCheckbox4.Enabled = false;
                glPosAdductCheckbox1.Checked = false;
                glPosAdductCheckbox2.Checked = false;
                glNegAdductCheckbox1.Checked = false;
                glNegAdductCheckbox2.Checked = false;
                glNegAdductCheckbox3.Checked = false;
                glNegAdductCheckbox4.Checked = false;
                
                glHgListbox.Visible = false;
                glHGLabel.Visible = false;
                
                glyceroBackboneImage = glyceroBackboneImageOrig;
                glyceroBackboneImageFAe = glyceroBackboneImageFA1eOrig;
                glyceroBackboneImageFA2e = glyceroBackboneImageFA2eOrig;
                glyceroBackboneImageFA3e = glyceroBackboneImageFA3eOrig;
                glyceroBackboneImageFA1p = glyceroBackboneImageFA1pOrig;
                glyceroBackboneImageFA2p = glyceroBackboneImageFA2pOrig;
                glyceroBackboneImageFA3p = glyceroBackboneImageFA3pOrig;
            }
            glPictureBox.Image = glyceroBackboneImage;
            glPictureBox.Visible = true;
        }
        
        
        
        public void glRepresentativeFACheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).representativeFA = ((CheckBox)sender).Checked;
            if (((GLLipid)currentLipid).representativeFA)
            {
                glFA2Textbox.Enabled = false;
                glDB2Textbox.Enabled = false;
                glHydroxyl2Textbox.Enabled = false;
                glFA2Combobox.Enabled = false;
                glFA2Checkbox1.Enabled = false;
                glFA2Checkbox2.Enabled = false;
                glFA2Checkbox3.Enabled = false;
                glFA3Textbox.Enabled = false;
                glDB3Textbox.Enabled = false;
                glHydroxyl3Textbox.Enabled = false;
                glFA3Combobox.Enabled = false;
                glFA3Checkbox1.Enabled = false;
                glFA3Checkbox2.Enabled = false;
                glFA3Checkbox3.Enabled = false;
                
                glFA2Textbox.Text = glFA1Textbox.Text;
                glFA3Textbox.Text = glFA1Textbox.Text;
                glDB2Textbox.Text = glDB1Textbox.Text;
                glDB3Textbox.Text = glDB1Textbox.Text;
                glHydroxyl2Textbox.Text = glHydroxyl1Textbox.Text;
                glHydroxyl3Textbox.Text = glHydroxyl1Textbox.Text;
                glFA2Combobox.Text = glFA1Combobox.Text;
                glFA3Combobox.Text = glFA1Combobox.Text;
                glFA2Checkbox1.Checked = glFA1Checkbox1.Checked;
                glFA3Checkbox1.Checked = glFA1Checkbox1.Checked;
                glFA2Checkbox2.Checked = glFA1Checkbox2.Checked;
                glFA3Checkbox2.Checked = glFA1Checkbox2.Checked;
                glFA2Checkbox3.Checked = glFA1Checkbox3.Checked;
                glFA3Checkbox3.Checked = glFA1Checkbox3.Checked;
                
                
            }
            else
            {
                glFA2Textbox.Enabled = true;
                glDB2Textbox.Enabled = true;
                glHydroxyl2Textbox.Enabled = true;
                glFA2Combobox.Enabled = true;
                glFA2Checkbox1.Enabled = true;
                glFA2Checkbox2.Enabled = true;
                glFA2Checkbox3.Enabled = true;
                glFA3Textbox.Enabled = true;
                glDB3Textbox.Enabled = true;
                glHydroxyl3Textbox.Enabled = true;
                glFA3Combobox.Enabled = true;
                glFA3Checkbox1.Enabled = true;
                glFA3Checkbox2.Enabled = true;
                glFA3Checkbox3.Enabled = true;
            }
            updateRanges(((GLLipid)currentLipid).fag2, glFA2Textbox, glFA2Combobox.SelectedIndex);
            updateRanges(((GLLipid)currentLipid).fag3, glFA3Textbox, glFA3Combobox.SelectedIndex);
            updateRanges(((GLLipid)currentLipid).fag2, glDB2Textbox, 3);
            updateRanges(((GLLipid)currentLipid).fag3, glDB3Textbox, 3);
            updateRanges(((GLLipid)currentLipid).fag2, glHydroxyl2Textbox, 4);
            updateRanges(((GLLipid)currentLipid).fag3, glHydroxyl3Textbox, 4);
        }
        
        
        
        ////////////////////// PL ////////////////////////////////
        
        
    
        public void plFA1ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((PLLipid)currentLipid).fag1, plFA1Textbox, ((ComboBox)sender).SelectedIndex);
            if (((PLLipid)currentLipid).representativeFA)
            {
                plFA2Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
            }
        }
        public void plFA2ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((PLLipid)currentLipid).fag2, plFA2Textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void plFA1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag1, (TextBox)sender, plFA1Combobox.SelectedIndex);
            if (((PLLipid)currentLipid).representativeFA)
            {
                plFA2Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void plFA2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag2, (TextBox)sender, plFA2Combobox.SelectedIndex);
        }
        
        public void plDB1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag1, (TextBox)sender, 3);
            if (((PLLipid)currentLipid).representativeFA)
            {
                plDB2Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void plDB2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag2, (TextBox)sender, 3);
        }
        public void plHydroxyl1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag1, (TextBox)sender, 4);
            if (((PLLipid)currentLipid).representativeFA)
            {
                plHydroxyl2Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void plHydroxyl2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((PLLipid)currentLipid).fag2, (TextBox)sender, 4);
        }
        
        public void plPosAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        public void plPosAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        public void plPosAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        public void plNegAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        public void plNegAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        public void plNegAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        public void plNegAdductCheckbox4CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
        }
        
        public void plFA1Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag1.faTypes["FAx"] = !((PLLipid)currentLipid).fag1.anyFAChecked();
            if (((PLLipid)currentLipid).representativeFA)
            {
                plFA2Checkbox1.Checked = ((CheckBox)sender).Checked;
            }
        }
        public void plFA1Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag1.faTypes["FAx"] = !((PLLipid)currentLipid).fag1.anyFAChecked();
            if (((PLLipid)currentLipid).representativeFA)
            {
                plFA2Checkbox2.Checked = ((CheckBox)sender).Checked;
            }
        }
        public void plFA1Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag1.faTypes["FAx"] = !((PLLipid)currentLipid).fag1.anyFAChecked();
            if (((PLLipid)currentLipid).representativeFA)
            {
                plFA2Checkbox3.Checked = ((CheckBox)sender).Checked;
            }
        }
        
        public void plF2Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag2.faTypes["FAx"] = !((PLLipid)currentLipid).fag2.anyFAChecked();
        }
        public void plFA2Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag2.faTypes["FAx"] = !((PLLipid)currentLipid).fag2.anyFAChecked();
        }
        public void plFA2Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag2.faTypes["FAx"] = !((PLLipid)currentLipid).fag2.anyFAChecked();
        }
        
        public void plIsCLCheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).isCL = ((CheckBox)sender).Checked;
            if (((CheckBox)sender).Checked)
            {
                plPictureBox.Image = cardioBackboneImage;
                plPictureBox.Location = new Point(5, 5);
                plHgListbox.Visible = false;
                plHGLabel.Visible = false;
                plFA1Checkbox3.Visible = false;
                plFA1Checkbox2.Visible = false;
                plFA1Checkbox1.Visible = false;
                plFA2Checkbox3.Visible = false;
                plFA2Checkbox2.Visible = false;
                plFA2Checkbox1.Visible = false;
                plPictureBox.Visible = false;
                plFA1Textbox.Visible = false;
                plFA2Textbox.Visible = false;
                plDB1Textbox.Visible = false;
                plDB2Textbox.Visible = false;
                plFA1Combobox.Visible = false;
                plFA2Combobox.Visible = false;
                plDB1Label.Visible = false;
                plDB2Label.Visible = false;
                plHydroxyl1Textbox.Visible = false;
                plHydroxyl2Textbox.Visible = false;
                plHydroxyl1Label.Visible = false;
                plHydroxyl2Label.Visible = false;
                plHGLabel.Visible = false;
                plHgListbox.Visible = false;
                plPositiveAdduct.Visible = false;
                plNegativeAdduct.Visible = false;
                plRepresentativeFA.Visible = false;
                
                clFA1Checkbox3.Visible = true;
                clFA1Checkbox2.Visible = true;
                clFA1Checkbox1.Visible = true;
                clFA2Checkbox3.Visible = true;
                clFA2Checkbox2.Visible = true;
                clFA2Checkbox1.Visible = true;
                clFA3Checkbox3.Visible = true;
                clFA3Checkbox2.Visible = true;
                clFA3Checkbox1.Visible = true;
                clFA4Checkbox3.Visible = true;
                clFA4Checkbox2.Visible = true;
                clFA4Checkbox1.Visible = true;
                clPositiveAdduct.Visible = true;
                clNegativeAdduct.Visible = true;
                clPictureBox.Visible = true;
                clFA1Textbox.Visible = true;
                clFA2Textbox.Visible = true;
                clFA3Textbox.Visible = true;
                clFA4Textbox.Visible = true;
                clDB1Textbox.Visible = true;
                clDB2Textbox.Visible = true;
                clDB3Textbox.Visible = true;
                clDB4Textbox.Visible = true;
                clHydroxyl1Textbox.Visible = true;
                clHydroxyl2Textbox.Visible = true;
                clHydroxyl3Textbox.Visible = true;
                clHydroxyl4Textbox.Visible = true;
                clHydroxyl1Label.Visible = true;
                clHydroxyl2Label.Visible = true;
                clHydroxyl3Label.Visible = true;
                clHydroxyl4Label.Visible = true;
                clFA1Combobox.Visible = true;
                clFA2Combobox.Visible = true;
                clFA3Combobox.Visible = true;
                clFA4Combobox.Visible = true;
                clDB1Label.Visible = true;
                clDB2Label.Visible = true;
                clDB3Label.Visible = true;
                clDB4Label.Visible = true;
                clRepresentativeFA.Visible = true;
                clDB4Label.SendToBack();
            }
            else
            {
                plPictureBox.Image = phosphoBackboneImage;
                plPictureBox.Location = new Point(107, 13);
                
                clFA1Checkbox3.Visible = false;
                clFA1Checkbox2.Visible = false;
                clFA1Checkbox1.Visible = false;
                clFA2Checkbox3.Visible = false;
                clFA2Checkbox2.Visible = false;
                clFA2Checkbox1.Visible = false;
                clFA3Checkbox3.Visible = false;
                clFA3Checkbox2.Visible = false;
                clFA3Checkbox1.Visible = false;
                clFA4Checkbox3.Visible = false;
                clFA4Checkbox2.Visible = false;
                clFA4Checkbox1.Visible = false;
                clPositiveAdduct.Visible = false;
                clNegativeAdduct.Visible = false;
                clPictureBox.Visible = false;
                clFA1Textbox.Visible = false;
                clFA2Textbox.Visible = false;
                clFA3Textbox.Visible = false;
                clFA4Textbox.Visible = false;
                clDB1Textbox.Visible = false;
                clDB2Textbox.Visible = false;
                clDB3Textbox.Visible = false;
                clDB4Textbox.Visible = false;
                clHydroxyl1Textbox.Visible = false;
                clHydroxyl2Textbox.Visible = false;
                clHydroxyl3Textbox.Visible = false;
                clHydroxyl4Textbox.Visible = false;
                clHydroxyl1Label.Visible = false;
                clHydroxyl2Label.Visible = false;
                clHydroxyl3Label.Visible = false;
                clHydroxyl4Label.Visible = false;
                clFA1Combobox.Visible = false;
                clFA2Combobox.Visible = false;
                clFA3Combobox.Visible = false;
                clFA4Combobox.Visible = false;
                clDB1Label.Visible = false;
                clDB2Label.Visible = false;
                clDB3Label.Visible = false;
                clDB4Label.Visible = false;
                clRepresentativeFA.Visible = false;
                
                plHgListbox.Visible = true;
                plHGLabel.Visible = true;
                plFA1Checkbox3.Visible = true;
                plFA1Checkbox2.Visible = true;
                plFA1Checkbox1.Visible = true;
                plFA2Checkbox3.Visible = true;
                plFA2Checkbox2.Visible = true;
                plFA2Checkbox1.Visible = true;
                plPictureBox.Visible = true;
                plFA1Textbox.Visible = true;
                plFA2Textbox.Visible = true;
                plDB1Textbox.Visible = true;
                plDB2Textbox.Visible = true;
                plFA1Combobox.Visible = true;
                plFA2Combobox.Visible = true;
                plDB1Label.Visible = true;
                plDB2Label.Visible = true;
                plHydroxyl1Textbox.Visible = true;
                plHydroxyl2Textbox.Visible = true;
                plHydroxyl1Label.Visible = true;
                plHydroxyl2Label.Visible = true;
                plHGLabel.Visible = true;
                plHgListbox.Visible = true;
                plPositiveAdduct.Visible = true;
                plNegativeAdduct.Visible = true;
                plRepresentativeFA.Visible = true;
                plPictureBox.SendToBack();
            }
            changeTab((int)LipidCategory.PhosphoLipid);
            plIsCL.BringToFront();
        }
        
        void plFA1Checkbox3MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = phosphoBackboneImage;
            plPictureBox.SendToBack();
        }
        private void plFA1Checkbox3MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = phosphoBackboneImageFA1e;
            plPictureBox.SendToBack();
        }
        
        void plFA1Checkbox2MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = phosphoBackboneImage;
            plPictureBox.SendToBack();
        }
        private void plFA1Checkbox2MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = phosphoBackboneImageFA1p;
            plPictureBox.SendToBack();
        }

        
        void plFA2Checkbox3MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = phosphoBackboneImage;
            plPictureBox.SendToBack();
        }
        private void plFA2Checkbox3MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = phosphoBackboneImageFA2e;
            plPictureBox.SendToBack();
        }
        void plFA2Checkbox2MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = phosphoBackboneImage;
            plPictureBox.SendToBack();
        }

        private void plFA2checkbox2MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = phosphoBackboneImageFA2p;
            plPictureBox.SendToBack();
        }
        
        private void plHGListboxSelectedValueChanged(object sender, System.EventArgs e)
        {
            if (settingListbox) return;
            currentLipid.headGroupNames.Clear();
            foreach(object itemChecked in ((ListBox)sender).SelectedItems)
            {
                currentLipid.headGroupNames.Add(itemChecked.ToString());
            }
            
        }
        
        public void plRepresentativeFACheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).representativeFA = ((CheckBox)sender).Checked;
            if (((PLLipid)currentLipid).representativeFA)
            {
                plFA2Textbox.Enabled = false;
                plDB2Textbox.Enabled = false;
                plHydroxyl2Textbox.Enabled = false;
                plFA2Combobox.Enabled = false;
                plFA2Checkbox1.Enabled = false;
                plFA2Checkbox2.Enabled = false;
                plFA2Checkbox3.Enabled = false;
                
                plFA2Textbox.Text = plFA1Textbox.Text;
                plDB2Textbox.Text = plDB1Textbox.Text;
                plHydroxyl2Textbox.Text = plHydroxyl1Textbox.Text;
                plFA2Combobox.Text = plFA1Combobox.Text;
                plFA2Checkbox1.Checked = plFA1Checkbox1.Checked;
                //plFA2Checkbox2.Checked = plFA1Checkbox2.Checked;
                //plFA2Checkbox3.Checked = plFA1Checkbox3.Checked;
                
                
            }
            else
            {
                plFA2Textbox.Enabled = true;
                plDB2Textbox.Enabled = true;
                plHydroxyl2Textbox.Enabled = true;
                plFA2Combobox.Enabled = true;
                plFA2Checkbox1.Enabled = true;
                plFA2Checkbox2.Enabled = true;
                plFA2Checkbox3.Enabled = true;
            }
            updateRanges(((PLLipid)currentLipid).fag2, plFA2Textbox, plFA2Combobox.SelectedIndex);
            updateRanges(((PLLipid)currentLipid).fag2, plDB2Textbox, 3);
            updateRanges(((PLLipid)currentLipid).fag2, plHydroxyl2Textbox, 4);
        }
        
        void plHGListboxMouseLeave(object sender, EventArgs e)
        {
            plPosAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            plPosAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            plPosAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            plNegAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            plNegAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            plNegAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            plNegAdductCheckbox4.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
        }

        void plHGListboxMouseHover(object sender, EventArgs e)
        {
            Point point = plHgListbox.PointToClient(Cursor.Position);
            int hoveredIndex = plHgListbox.IndexFromPoint(point);

            if (hoveredIndex != -1)
            {
                if (lipidCreator.headgroups[(string)plHgListbox.Items[hoveredIndex]].adductRestrictions["+H"]) plPosAdductCheckbox1.BackColor = highlightedCheckboxColor;
                else plPosAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreator.headgroups[(string)plHgListbox.Items[hoveredIndex]].adductRestrictions["+2H"]) plPosAdductCheckbox2.BackColor = highlightedCheckboxColor;
                else plPosAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreator.headgroups[(string)plHgListbox.Items[hoveredIndex]].adductRestrictions["+NH4"]) plPosAdductCheckbox3.BackColor = highlightedCheckboxColor;
                else plPosAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreator.headgroups[(string)plHgListbox.Items[hoveredIndex]].adductRestrictions["-H"]) plNegAdductCheckbox1.BackColor = highlightedCheckboxColor;
                else plNegAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreator.headgroups[(string)plHgListbox.Items[hoveredIndex]].adductRestrictions["-2H"]) plNegAdductCheckbox2.BackColor = highlightedCheckboxColor;
                else plNegAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreator.headgroups[(string)plHgListbox.Items[hoveredIndex]].adductRestrictions["+HCOO"]) plNegAdductCheckbox3.BackColor = highlightedCheckboxColor;
                else plNegAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreator.headgroups[(string)plHgListbox.Items[hoveredIndex]].adductRestrictions["+CH3COO"]) plNegAdductCheckbox4.BackColor = highlightedCheckboxColor;
                else plNegAdductCheckbox4.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            }
        }
        
        ////////////////////// SL ////////////////////////////////
        
        
        
        public void slPosAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        public void slPosAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        public void slPosAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        public void slNegAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        public void slNegAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        public void slNegAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        public void slNegAdductCheckbox4CheckedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
        }
        
        public void slDB1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).fag.dbInfo = ((TextBox)sender).Text;
            updateRanges(((SLLipid)currentLipid).fag, (TextBox)sender, 3);
        }
        public void slDB2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).lcb.dbInfo = ((TextBox)sender).Text;
            updateRanges(((SLLipid)currentLipid).lcb, (TextBox)sender, 3);
        }
        
        public void slFATextboxValueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).fag.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((SLLipid)currentLipid).fag, (TextBox)sender, slFACombobox.SelectedIndex);
        }
        public void slLCBTextboxValueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).lcb.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((SLLipid)currentLipid).lcb, (TextBox)sender, slLCBCombobox.SelectedIndex, true);
        }
        
        
        public void slFAComboboxValueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).fag.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((SLLipid)currentLipid).fag, slFATextbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void slLCBComboboxValueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).lcb.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((SLLipid)currentLipid).lcb, slLCBTextbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void slLCBHydroxyComboboxValueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).lcb.hydroxylCounts.Clear();
            ((SLLipid)currentLipid).lcb.hydroxylCounts.Add(((ComboBox)sender).SelectedIndex + 2);
        }
        
        public void slFAHydroxyComboboxValueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).fag.hydroxylCounts.Clear();
            ((SLLipid)currentLipid).fag.hydroxylCounts.Add(((ComboBox)sender).SelectedIndex);
        }
        
        private void slHGListboxSelectedValueChanged(object sender, System.EventArgs e)
        {
            if (settingListbox) return;
            currentLipid.headGroupNames.Clear();
            foreach(object itemChecked in ((ListBox)sender).SelectedItems)
            {
                currentLipid.headGroupNames.Add(itemChecked.ToString());
            }
        }
        
        void slHGListboxMouseLeave(object sender, EventArgs e)
        {
            slPosAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            slPosAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            slPosAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            slNegAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            slNegAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            slNegAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            slNegAdductCheckbox4.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
        }

        void slHGListboxMouseHover(object sender, EventArgs e)
        {
            Point point = slHgListbox.PointToClient(Cursor.Position);
            int hoveredIndex = slHgListbox.IndexFromPoint(point);

            if (hoveredIndex != -1)
            {
                if (lipidCreator.headgroups[(string)slHgListbox.Items[hoveredIndex]].adductRestrictions["+H"]) slPosAdductCheckbox1.BackColor = highlightedCheckboxColor;
                else slPosAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreator.headgroups[(string)slHgListbox.Items[hoveredIndex]].adductRestrictions["+2H"]) slPosAdductCheckbox2.BackColor = highlightedCheckboxColor;
                else slPosAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreator.headgroups[(string)slHgListbox.Items[hoveredIndex]].adductRestrictions["+NH4"]) slPosAdductCheckbox3.BackColor = highlightedCheckboxColor;
                else slPosAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreator.headgroups[(string)slHgListbox.Items[hoveredIndex]].adductRestrictions["-H"]) slNegAdductCheckbox1.BackColor = highlightedCheckboxColor;
                else slNegAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreator.headgroups[(string)slHgListbox.Items[hoveredIndex]].adductRestrictions["-2H"]) slNegAdductCheckbox2.BackColor = highlightedCheckboxColor;
                else slNegAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreator.headgroups[(string)slHgListbox.Items[hoveredIndex]].adductRestrictions["+HCOO"]) slNegAdductCheckbox3.BackColor = highlightedCheckboxColor;
                else slNegAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreator.headgroups[(string)slHgListbox.Items[hoveredIndex]].adductRestrictions["+CH3COO"]) slNegAdductCheckbox4.BackColor = highlightedCheckboxColor;
                else slNegAdductCheckbox4.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            }
        }
        
        
        
        ////////////////////// Cholesterols ////////////////////////////////
        
        
        
        public void chPosAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Cholesterol)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        public void chPosAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Cholesterol)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        public void chPosAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Cholesterol)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        public void chNegAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Cholesterol)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        public void chNegAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Cholesterol)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        public void chNegAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Cholesterol)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        public void chNegAdductCheckbox4CheckedChanged(Object sender, EventArgs e)
        {
            ((Cholesterol)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
        }
        
        public void chContainsEsterCheckedChanged(Object sender, EventArgs e)
        {
            ((Cholesterol)currentLipid).containsEster = ((CheckBox)sender).Checked;
            
            chPictureBox.Visible = false;
            if (((Cholesterol)currentLipid).containsEster)
            {
                chPictureBox.Image = cholesterolEsterBackboneImage;
            }
            else
            {
                chPictureBox.Image = cholesterolBackboneImage;
            }
            chFACombobox.Visible = ((Cholesterol)currentLipid).containsEster;
            chFATextbox.Visible = ((Cholesterol)currentLipid).containsEster;
            chDBTextbox.Visible = ((Cholesterol)currentLipid).containsEster;
            chDBLabel.Visible = ((Cholesterol)currentLipid).containsEster;
            chHydroxylTextbox.Visible = ((Cholesterol)currentLipid).containsEster;
            chFAHydroxyLabel.Visible = ((Cholesterol)currentLipid).containsEster;
            chPictureBox.Visible = true;
        }
        
        public void chFAComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Cholesterol)currentLipid).fag.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((Cholesterol)currentLipid).fag, chFATextbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void chFATextboxValueChanged(Object sender, EventArgs e)
        {
            ((Cholesterol)currentLipid).fag.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((Cholesterol)currentLipid).fag, (TextBox)sender, chFACombobox.SelectedIndex);
        }
        
        public void chDBTextboxValueChanged(Object sender, EventArgs e)
        {
            ((Cholesterol)currentLipid).fag.dbInfo = ((TextBox)sender).Text;
            updateRanges(((Cholesterol)currentLipid).fag, (TextBox)sender, 3);
        }
        
        public void chHydroxylTextboxValueChanged(Object sender, EventArgs e)
        {
            ((Cholesterol)currentLipid).fag.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((Cholesterol)currentLipid).fag, (TextBox)sender, 4);
        }
        
        
        
        
        ////////////////////// Mediators ////////////////////////////////
        
        public void medNegAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Mediator)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        
        private void medHGListboxSelectedValueChanged(object sender, System.EventArgs e)
        {
            if (settingListbox) return;
            currentLipid.headGroupNames.Clear();
            foreach(object itemChecked in ((ListBox)sender).SelectedItems)
            {
                currentLipid.headGroupNames.Add(itemChecked.ToString());
            }
        }
        
        void medHGListboxMouseHover(object sender, EventArgs e)
        {
            Point point = medHgListbox.PointToClient(Cursor.Position);
            int hoveredIndex = medHgListbox.IndexFromPoint(point);

            if (hoveredIndex != -1)
            {
                string mediatorFile = lipidCreator.headgroups[medHgListbox.Items[hoveredIndex].ToString()].pathToImage;
                medPictureBox.Image = Image.FromFile(mediatorFile);
                medPictureBox.Top = 230 - (medPictureBox.Image.Height >> 1);
                medPictureBox.SendToBack();
            }
        }
        
        void medHGListboxMouseLeave(object sender, EventArgs e)
        {
            //medPictureBox.Image = null;
            //medPictureBox.SendToBack();
        }
        
        void openMediatorMS2Form(object sender, EventArgs e)
        {
            MediatorMS2Form mediatorMS2fragments = new MediatorMS2Form(this, (Mediator)currentLipid);
            mediatorMS2fragments.Owner = this;
            mediatorMS2fragments.ShowInTaskbar = false;
            mediatorMS2fragments.ShowDialog();
            mediatorMS2fragments.Dispose();
        }
        
        
        ////////////////////// Remaining parts ////////////////////////////////
        
        
        public LipidCategory checkPropertiesValid()
        {
            int cntActiveAdducts = 0;
            foreach (KeyValuePair<String, bool> adduct in currentLipid.adducts)
            {
                cntActiveAdducts += adduct.Value ? 1 : 0;
            }
            if (cntActiveAdducts < 1)
            {
                MessageBox.Show("No adduct selected!", "Not registrable");
                return  LipidCategory.NoLipid;
            }
            
            if (currentLipid is GLLipid)
            {
                if (((GLLipid)currentLipid).fag1.faTypes["FAx"])
                {
                    MessageBox.Show("Please always select the top fatty acid!", "Not registrable");
                    return  LipidCategory.NoLipid;
                }
                else if (((GLLipid)currentLipid).fag2.faTypes["FAx"] && !((GLLipid)currentLipid).fag3.faTypes["FAx"])
                {
                    MessageBox.Show("Please select the middle fatty acid for DG!", "Not registrable");
                    return  LipidCategory.NoLipid;
                }
                
                if (glFA1Textbox.BackColor == alertColor)
                {
                    MessageBox.Show("First fatty acid length content not valid!", "Not registrable");
                    return  LipidCategory.NoLipid;
                }
                if (glFA2Textbox.BackColor == alertColor)
                {
                    MessageBox.Show("Second fatty acid length content not valid!", "Not registrable");
                    return  LipidCategory.NoLipid;
                }
                if (glDB1Textbox.BackColor == alertColor)
                {
                    MessageBox.Show("First double bond content not valid!", "Not registrable");
                    return  LipidCategory.NoLipid;
                }
                if (glDB2Textbox.BackColor == alertColor)
                {
                    MessageBox.Show("Second double bond content not valid!", "Not registrable");
                    return  LipidCategory.NoLipid;
                }
                if (glHydroxyl1Textbox.BackColor == alertColor)
                {
                    MessageBox.Show("First hydroxyl content not valid!", "Not registrable");
                    return  LipidCategory.NoLipid;
                }
                if (glHydroxyl2Textbox.BackColor == alertColor)
                {
                    MessageBox.Show("Second hydroxyl content not valid!", "Not registrable");
                    return  LipidCategory.NoLipid;
                }
                if (((GLLipid)currentLipid).containsSugar)
                {
                    if (currentLipid.headGroupNames.Count == 0)
                    {
                        MessageBox.Show("No head group selected!", "Not registrable");
                        return  LipidCategory.NoLipid;                    
                    }
                    if (((GLLipid)currentLipid).fag1.faTypes["FAx"] || ((GLLipid)currentLipid).fag2.faTypes["FAx"])
                    {
                        MessageBox.Show("Both fatty acids must be selected!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                }
                else
                {
                    if (((GLLipid)currentLipid).fag1.faTypes["FAx"] && ((GLLipid)currentLipid).fag2.faTypes["FAx"] && ((GLLipid)currentLipid).fag3.faTypes["FAx"])
                    {
                        MessageBox.Show("No fatty acid selected!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (glFA3Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Third fatty acid length content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (glDB3Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Third double bond content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (glHydroxyl3Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Third hydroxyl content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                }
                return LipidCategory.GlyceroLipid;
            }
            
            
            else if (currentLipid is PLLipid)
            {
                if (((PLLipid)currentLipid).isCL)
                {
                    if (((PLLipid)currentLipid).fag1.faTypes["FAx"] || ((PLLipid)currentLipid).fag2.faTypes["FAx"] || ((PLLipid)currentLipid).fag3.faTypes["FAx"])
                    {
                        MessageBox.Show("At least the top three fatty acids must be selected!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                
                    if (clFA1Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("First fatty acid length content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (clFA2Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Second fatty acid length content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (clFA3Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Third fatty acid length content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (clFA4Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Fourth fatty acid length content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (clDB1Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("First double bond content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (clDB2Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Second double bond content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (clDB3Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Third double bond content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (clDB4Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Fourth double bond content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (clHydroxyl1Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("First hydroxyl content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (clHydroxyl2Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Second hydroxyl content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (clHydroxyl3Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Third hydroxyl content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (clHydroxyl4Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Fourth hydroxyl content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                }
                else
                {
                    if (currentLipid.headGroupNames.Count == 0)
                    {
                        MessageBox.Show("No head group selected!", "Not registrable");
                        return  LipidCategory.NoLipid;                    
                    }
                    
                    if (((PLLipid)currentLipid).fag1.faTypes["FAx"])
                    {
                        MessageBox.Show("Please select at least the top fatty acid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    
                    if (plFA1Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("First fatty acid length content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (plFA2Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Second fatty acid length content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (plDB1Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("First double bond content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (plDB2Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Second double bond content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (plHydroxyl1Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("First hydroxyl content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (plHydroxyl2Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("Second hydroxyl content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                }
                return LipidCategory.PhosphoLipid;
            }
            
            
            else if (currentLipid is SLLipid)
            {
                if (currentLipid.headGroupNames.Count == 0)
                {
                    MessageBox.Show("No head group selected!", "Not registrable");
                    return LipidCategory.NoLipid;                   
                }
                
                if (slLCBTextbox.BackColor == alertColor)
                {
                    MessageBox.Show("Long chain base length content not valid!", "Not registrable");
                    return LipidCategory.NoLipid;
                }
                if (slFATextbox.BackColor == alertColor)
                {
                    MessageBox.Show("Fatty acid length content not valid!", "Not registrable");
                    return LipidCategory.NoLipid;
                }
                if (slDB1Textbox.BackColor == alertColor)
                {
                    MessageBox.Show("FA double bond content not valid!", "Not registrable");
                    return LipidCategory.NoLipid;
                }
                if (slDB2Textbox.BackColor == alertColor)
                {
                    MessageBox.Show("LCB double bond content not valid!", "Not registrable");
                    return LipidCategory.NoLipid;
                }
                return LipidCategory.SphingoLipid;
            }
            
            
            else if (currentLipid is Cholesterol)
            {
                if (chContainsEster.Checked && chFATextbox.BackColor == alertColor)
                {
                    MessageBox.Show("Fatty acid length content not valid!", "Not registrable");
                    return LipidCategory.NoLipid;
                }
                if (chContainsEster.Checked && chDBTextbox.BackColor == alertColor)
                {
                    MessageBox.Show("FA double bond content not valid!", "Not registrable");
                    return LipidCategory.NoLipid;
                }
                if (chContainsEster.Checked && chHydroxylTextbox.BackColor == alertColor)
                {
                    MessageBox.Show("Hydroxyl content not valid!", "Not registrable");
                    return LipidCategory.NoLipid;
                }
                return LipidCategory.Cholesterol;
            }
            
            else if (currentLipid is Mediator)
            {
                if (currentLipid.headGroupNames.Count == 0)
                {
                    MessageBox.Show("No mediator selected!", "Not registrable");
                    return  LipidCategory.NoLipid;                    
                }
                return LipidCategory.Mediator;
            }
            return LipidCategory.NoLipid;
        }
        
        
        public void modifyLipid(Object sender, EventArgs e)
        {
            LipidCategory result = checkPropertiesValid();
            switch (result)
            {
                case LipidCategory.GlyceroLipid:
                    lipidCreator.registeredLipids[lipidModifications[(int)result]] = new GLLipid((GLLipid)currentLipid);
                    refreshRegisteredLipidsTable();
                    break;
                case LipidCategory.PhosphoLipid:
                    lipidCreator.registeredLipids[lipidModifications[(int)result]] = new PLLipid((PLLipid)currentLipid);
                    refreshRegisteredLipidsTable();
                    break;
                case LipidCategory.SphingoLipid:
                    lipidCreator.registeredLipids[lipidModifications[(int)result]] = new SLLipid((SLLipid)currentLipid);
                    refreshRegisteredLipidsTable();
                    break;
                case LipidCategory.Cholesterol:
                    lipidCreator.registeredLipids[lipidModifications[(int)result]] = new Cholesterol((Cholesterol)currentLipid);
                    refreshRegisteredLipidsTable();
                    break;
                case LipidCategory.Mediator:
                    lipidCreator.registeredLipids[lipidModifications[(int)result]] = new Mediator((Mediator)currentLipid);
                    refreshRegisteredLipidsTable();
                    break;
                default:
                    break;
            }
        }
        
        public void registerLipid(Object sender, EventArgs e)
        {
            LipidCategory result = checkPropertiesValid();
            switch (result)
            {
                case LipidCategory.GlyceroLipid:
                    lipidCreator.registeredLipids.Add(new GLLipid((GLLipid)currentLipid));
                    refreshRegisteredLipidsTable();
                    break;
                case LipidCategory.PhosphoLipid:
                    lipidCreator.registeredLipids.Add(new PLLipid((PLLipid)currentLipid));
                    refreshRegisteredLipidsTable();
                    break;
                case LipidCategory.SphingoLipid:
                    lipidCreator.registeredLipids.Add(new SLLipid((SLLipid)currentLipid));
                    refreshRegisteredLipidsTable();
                    break;
                case LipidCategory.Cholesterol:
                    lipidCreator.registeredLipids.Add(new Cholesterol((Cholesterol)currentLipid));
                    refreshRegisteredLipidsTable();
                    break;
                case LipidCategory.Mediator:
                    lipidCreator.registeredLipids.Add(new Mediator((Mediator)currentLipid));
                    refreshRegisteredLipidsTable();
                    break;
                default:
                    break;
            }
        }
        
        public void refreshRegisteredLipidsTable()
        {
            registeredLipidsDatatable.Clear();
            foreach (Lipid currentRegisteredLipid in lipidCreator.registeredLipids)
            {
                DataRow row = registeredLipidsDatatable.NewRow();
                if (currentRegisteredLipid is GLLipid)
                {
                    GLLipid currentGLLipid = (GLLipid)currentRegisteredLipid;
                    row["Category"] = "Glycerolipid";
                    row["Building Block 1"] = "FA: " + currentGLLipid.fag1.lengthInfo + "; DB: " + currentGLLipid.fag1.dbInfo + "; OH: " + currentGLLipid.fag1.hydroxylInfo;
                    row["Building Block 2"] = "FA: " + currentGLLipid.fag2.lengthInfo + "; DB: " + currentGLLipid.fag2.dbInfo + "; OH: " + currentGLLipid.fag2.hydroxylInfo;
                    if (currentGLLipid.containsSugar)
                    {
                        row["Building Block 3"] = "HG: " + String.Join(", ", currentGLLipid.headGroupNames);
                    }
                    else
                    {
                        row["Building Block 3"] = "FA: " + currentGLLipid.fag3.lengthInfo + "; DB: " + currentGLLipid.fag3.dbInfo + "; OH: " + currentGLLipid.fag3.hydroxylInfo;
                    }
                }
                else if (currentRegisteredLipid is PLLipid)
                {
                    PLLipid currentPLLipid = (PLLipid)currentRegisteredLipid;
                    if (currentPLLipid.isCL)
                    {
                        row["Category"] = "Cardiolipin";
                        row["Building Block 1"] = "FA: " + currentPLLipid.fag1.lengthInfo + "; DB: " + currentPLLipid.fag1.dbInfo + "; OH: " + currentPLLipid.fag1.hydroxylInfo;
                        row["Building Block 2"] = "FA: " + currentPLLipid.fag2.lengthInfo + "; DB: " + currentPLLipid.fag2.dbInfo + "; OH: " + currentPLLipid.fag2.hydroxylInfo;
                        row["Building Block 3"] = "FA: " + currentPLLipid.fag3.lengthInfo + "; DB: " + currentPLLipid.fag3.dbInfo + "; OH: " + currentPLLipid.fag3.hydroxylInfo;
                        row["Building Block 4"] = "FA: " + currentPLLipid.fag4.lengthInfo + "; DB: " + currentPLLipid.fag4.dbInfo + "; OH: " + currentPLLipid.fag4.hydroxylInfo;
                    }
                    else
                    {
                        row["Category"] = "Phospholipid";
                        row["Building Block 1"] = "HG: " + String.Join(", ", currentPLLipid.headGroupNames);
                        row["Building Block 2"] = "FA: " + currentPLLipid.fag1.lengthInfo + "; DB: " + currentPLLipid.fag1.dbInfo + "; OH: " + currentPLLipid.fag1.hydroxylInfo;
                        row["Building Block 3"] = "FA: " + currentPLLipid.fag2.lengthInfo + "; DB: " + currentPLLipid.fag2.dbInfo + "; OH: " + currentPLLipid.fag2.hydroxylInfo;
                    }
                }
                else if (currentRegisteredLipid is SLLipid)
                {
                    SLLipid currentSLLipid = (SLLipid)currentRegisteredLipid;
                    row["Category"] = "Sphingolipid";
                    row["Building Block 1"] = "HG: " + String.Join(", ", currentSLLipid.headGroupNames);
                    row["Building Block 2"] = "LCB: " + currentSLLipid.lcb.lengthInfo + "; DB: " + currentSLLipid.lcb.dbInfo;
                    row["Building Block 3"] = "FA: " + currentSLLipid.fag.lengthInfo + "; DB: " + currentSLLipid.fag.dbInfo + "; OH: " + currentSLLipid.fag.hydroxylInfo;
                }
                
                else if (currentRegisteredLipid is Cholesterol)
                {
                    Cholesterol currentCHLipid = (Cholesterol)currentRegisteredLipid;
                    row["Category"] = "Cholesterol";
                    if (currentCHLipid.containsEster) row["Building Block 1"] = "FA: " + currentCHLipid.fag.lengthInfo + "; DB: " + currentCHLipid.fag.dbInfo + "; OH: " + currentCHLipid.fag.hydroxylInfo;
                }
                
                else if (currentRegisteredLipid is Mediator)
                {
                    Mediator currentMedLipid = (Mediator)currentRegisteredLipid;
                    row["Building Block 1"] = "HG: " + String.Join(", ", currentMedLipid.headGroupNames);
                    row["Category"] = "Mediator";
                }
                
                
                string adductsStr = "";
                if (currentRegisteredLipid.adducts["+H"]) adductsStr += "+H⁺";
                if (currentRegisteredLipid.adducts["+2H"]) adductsStr += (adductsStr.Length > 0 ? ", " : "") + "+2H⁺⁺";
                if (currentRegisteredLipid.adducts["+NH4"]) adductsStr += (adductsStr.Length > 0 ? ", " : "") + "+NH4⁺";
                if (currentRegisteredLipid.adducts["-H"]) adductsStr += (adductsStr.Length > 0 ? ", " : "") + "-H⁻";
                if (currentRegisteredLipid.adducts["-2H"]) adductsStr += (adductsStr.Length > 0 ? ", " : "") + "-2H⁻ ⁻";
                if (currentRegisteredLipid.adducts["+HCOO"]) adductsStr += (adductsStr.Length > 0 ? ", " : "") + "+HCOO⁻";
                if (currentRegisteredLipid.adducts["+CH3COO"]) adductsStr += (adductsStr.Length > 0 ? ", " : "") + "+CH3COO⁻";
                row["Adducts"] = adductsStr;
                
                registeredLipidsDatatable.Rows.Add(row);
                lipidsGridview.DataSource = registeredLipidsDatatable;
                
                
                for (int i = 0; i < lipidsGridview.Rows.Count; ++i)
                {
                    lipidsGridview.Rows[i].Cells["Edit"].Value = editImage;
                    lipidsGridview.Rows[i].Cells["Delete"].Value = deleteImage;
                }
                lipidsGridview.Update();
                lipidsGridview.Refresh();
            }
        }
        
        public void lipidsGridviewDoubleClick(Object sender, EventArgs e)
        {
            int rowIndex = ((DataGridView)sender).CurrentCell.RowIndex;
            int colIndex = ((DataGridView)sender).CurrentCell.ColumnIndex;
            if (((DataGridView)sender).Columns[colIndex].Name == "Edit")
            {
            
                Lipid currentRegisteredLipid = (Lipid)lipidCreator.registeredLipids[rowIndex];
                int tabIndex = 0;
                if (currentRegisteredLipid is GLLipid)
                {
                    tabIndex = (int)LipidCategory.GlyceroLipid;
                    lipidTabList[tabIndex] = new GLLipid((GLLipid)currentRegisteredLipid);
                }
                else if (currentRegisteredLipid is PLLipid)
                {
                    tabIndex = (int)LipidCategory.PhosphoLipid;
                    lipidTabList[tabIndex] = new PLLipid((PLLipid)currentRegisteredLipid);
                }
                else if (currentRegisteredLipid is SLLipid)
                {
                    tabIndex = (int)LipidCategory.SphingoLipid;
                    lipidTabList[tabIndex] = new SLLipid((SLLipid)currentRegisteredLipid);
                }
                else if (currentRegisteredLipid is Cholesterol)
                {
                    tabIndex = (int)LipidCategory.Cholesterol;
                    lipidTabList[tabIndex] = new Cholesterol((Cholesterol)currentRegisteredLipid);
                }
                else if (currentRegisteredLipid is Mediator)
                {
                    tabIndex = (int)LipidCategory.Mediator;
                    lipidTabList[tabIndex] = new Mediator((Mediator)currentRegisteredLipid);
                }
                currentLipid = currentRegisteredLipid;
                lipidModifications[tabIndex] = rowIndex;
                tabControl.SelectedIndex = tabIndex;
                changeTab(tabIndex);
                
            }
            else if (((DataGridView)sender).Columns[colIndex].Name == "Delete")
            {
                Lipid currentRegisteredLipid = (Lipid)lipidCreator.registeredLipids[rowIndex];
                int tabIndex = 0;
                if (currentRegisteredLipid is GLLipid)
                {
                    tabIndex = (int)LipidCategory.GlyceroLipid;
                }
                else if (currentRegisteredLipid is PLLipid)
                {
                    tabIndex = (int)LipidCategory.PhosphoLipid;
                }
                else if (currentRegisteredLipid is SLLipid)
                {
                    tabIndex = (int)LipidCategory.SphingoLipid;
                }
                else if (currentRegisteredLipid is Cholesterol)
                {
                    tabIndex = (int)LipidCategory.Cholesterol;
                }
                else if (currentRegisteredLipid is Mediator)
                {
                    tabIndex = (int)LipidCategory.Mediator;
                }
                lipidCreator.registeredLipids.RemoveAt(rowIndex);
                refreshRegisteredLipidsTable();
                lipidModifications[tabIndex] = -1;
                if (tabIndex == currentTabIndex)
                {
                    changeTab(tabIndex);
                }
            }
        }
        
        
        public void lipidsGridviewKeydown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lipidCreator.registeredLipids.Count > 0 && ((DataGridView)sender).SelectedRows.Count > 0)
            {   
                lipidCreator.registeredLipids.RemoveAt(((DataGridView)sender).SelectedRows[0].Index);
                refreshRegisteredLipidsTable();
                e.Handled = true;
            }
        }
        
        
        public void openMS2Form(Object sender, EventArgs e)
        {
            if (currentIndex == LipidCategory.NoLipid) return;
            Form openForm = (currentIndex == LipidCategory.Mediator) ? ((Form)new MediatorMS2Form(this, (Mediator)currentLipid)) : ((Form)new MS2Form(this, currentLipid));
            openForm.Owner = this;
            openForm.ShowInTaskbar = false;
            openForm.ShowDialog();
            openForm.Dispose();
        }
        
        
        public void startTutorial1(Object sender, EventArgs e)
        {
            tutorial.startTutorial(Tutorials.TutorialPRM);
        }
        
        
        public void openHeavyIsotopeForm(Object sender, EventArgs e)
        {
            if (currentIndex == LipidCategory.NoLipid) return;
            AddHeavyPrecursor addHeavyPrecursor = new AddHeavyPrecursor(this, currentIndex);
            addHeavyPrecursor.Owner = this;
            addHeavyPrecursor.ShowInTaskbar = false;
            addHeavyPrecursor.ShowDialog();
            addHeavyPrecursor.Dispose();
            resetLipid(null, null);
        }
        
        
        public void openReviewForm(Object sender, EventArgs e)
        {
            lipidCreator.assembleLipids();
            LipidsReview lipidsReview = new LipidsReview(lipidCreator);
            lipidsReview.Owner = this;
            lipidsReview.ShowInTaskbar = false;
            lipidsReview.ShowDialog();
            lipidsReview.Dispose();
        }
        
        protected void menuImportPredefinedClick(object sender, System.EventArgs e)
        {
            System.Windows.Forms.MenuItem PredefItem = (System.Windows.Forms.MenuItem)sender;
            string filePath = (string)PredefItem.Tag;
            XDocument doc;
            try 
            {
                doc = XDocument.Load(filePath);
                lipidCreator.import(doc);
                
                
                refreshRegisteredLipidsTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not read file, " + ex.Message, "Error while reading", MessageBoxButtons.OK);
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        
        
        protected void menuImportClick(object sender, System.EventArgs e)
        {
        
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "lcXML files (*.lcXML)|*.lcXML|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                XDocument doc;
                try 
                {
                    doc = XDocument.Load(openFileDialog1.FileName);
                    lipidCreator.import(doc);
                    resetAllLipids();
                    changeTab((int)currentIndex);
                    refreshRegisteredLipidsTable();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not read file, " + ex.Message, "Error while reading", MessageBoxButtons.OK);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
        
        
        
        protected void menuImportSettingsClick(object sender, System.EventArgs e)
        {
        
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "lcXML files (*.lcXML)|*.lcXML|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                XDocument doc;
                try 
                {
                    doc = XDocument.Load(openFileDialog1.FileName);
                    lipidCreator.import(doc, true);
                    resetAllLipids();
                    changeTab((int)currentIndex);
                    refreshRegisteredLipidsTable();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not read file, " + ex.Message, "Error while reading", MessageBoxButtons.OK);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
        
        protected void menuExportClick(object sender, System.EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            
            saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.Filter = "lcXML files (*.lcXML)|*.lcXML|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;

            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer;
                if((writer = new StreamWriter(saveFileDialog1.OpenFile())) != null)
                {
                    writer.Write(lipidCreator.serialize());
                    writer.Dispose();
                    writer.Close();
                }
            }
        }
        
        protected void menuExportSettingsClick(object sender, System.EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            
            saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.Filter = "lcXML files (*.lcXML)|*.lcXML|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;

            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer;
                if((writer = new StreamWriter(saveFileDialog1.OpenFile())) != null)
                {
                    writer.Write(lipidCreator.serialize(true));
                    writer.Dispose();
                    writer.Close();
                }
            }
        }
        
        protected void menuExitClick(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        protected void menuAboutClick(object sender, System.EventArgs e)
        {
            AboutDialog aboutDialog = new AboutDialog ();
            aboutDialog.Owner = this;
            aboutDialog.ShowInTaskbar = false;
            aboutDialog.ShowDialog ();
            aboutDialog.Dispose ();
        }
    
    
        [STAThread]
        public static void Main(string[] args)
        {
            CreatorGUI creatorGUI = new CreatorGUI((args.Length > 0) ? args[0] : null);
            Application.Run(creatorGUI);
        }
    }
}
