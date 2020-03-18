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

namespace LipidCreator
{
    [Serializable]
    public partial class Wizard : Form
    {
        
        //private static readonly ILog log = LogManager.GetLogger(typeof(Wizard));
        public CreatorGUI creatorGUI;
        public int wizardStep;
        public Lipid lipid;
        public int currentFA = -1;
        public FattyAcidGroup fag = null;
        public int headgroupCategory = -1;
        public ArrayList faList = new ArrayList();
        public string headgroup = "";
        public int filter = 2;
        public bool hasPlasmalogen = false;
        
        
        public enum WizardSteps {Welcome, SelectCategory, SelectClass, SelectFA, SelectAdduct, SelectFragmentMode, SelectFragments, AddLipid, Finish};

        public Wizard(CreatorGUI _creatorGUI)
        {
            creatorGUI = _creatorGUI;
            InitializeComponent();
            wizardStep = (int)WizardSteps.Welcome;
            processWizardSteps();
        }
        
        
        
        private void resetAllControls()
        {
            cancelButton.Enabled = true;
            continueButton.Enabled = true;
            backButton.Enabled = false;
            cancelButton.Text = "Cancel";
            continueButton.Text = "Continue";
            
            foreach (Control control in controlElements)
            {
                control.Visible = false;
            }
        }
        
        

        private void backClick(object sender, EventArgs e)
        {
            if (wizardStep == (int)WizardSteps.SelectAdduct)
            {
                if (faList.Count == 0) wizardStep -= 1;
                else 
                {
                    currentFA = faList.Count - 1;
                    selectFAG();
                }
            }
            else if (wizardStep == (int)WizardSteps.SelectFA)
            {
                currentFA -= 1;
                if (currentFA >= 0)
                {
                    wizardStep += 1;
                    selectFAG();
                }
            }
            
            wizardStep -= 1;
            processWizardSteps();
        }
        
        

        private void cancelClick(object sender, EventArgs e)
        {
            if (MessageBox.Show ("Do you want to close the wizard?", "Close Wizard", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }
        
        
        
        private void selectFAG()
        {
            fag = null;
            if (currentFA <= -1 || faList.Count <= currentFA) return;
            switch (headgroupCategory)
            {
                case (int)LipidCategory.Glycerolipid:
                    if ((string)faList[currentFA] == "FA1") fag = ((Glycerolipid)lipid).fag1;
                    else if ((string)faList[currentFA] == "FA2") fag = ((Glycerolipid)lipid).fag2;
                    else if ((string)faList[currentFA] == "FA3") fag = ((Glycerolipid)lipid).fag3;
                    break;
                    
                case (int)LipidCategory.Glycerophospholipid:
                    if ((string)faList[currentFA] == "FA1") fag = ((Phospholipid)lipid).fag1;
                    else if ((string)faList[currentFA] == "FA2") fag = ((Phospholipid)lipid).fag2;
                    else if ((string)faList[currentFA] == "FA3") fag = ((Phospholipid)lipid).fag3;
                    else if ((string)faList[currentFA] == "FA4") fag = ((Phospholipid)lipid).fag4;
                    break;
                    
                case (int)LipidCategory.Sphingolipid:
                    if ((string)faList[currentFA] == "LCB") fag = ((Sphingolipid)lipid).lcb;
                    else if ((string)faList[currentFA] == "FA") fag = ((Sphingolipid)lipid).fag;
                    break;
                    
                case (int)LipidCategory.Sterollipid:
                    if ((string)faList[currentFA] == "FA") fag = ((Cholesterol)lipid).fag;
                    break;
                
                default:
                    break;
            }
        }
        
        
        

        private void continueClick(object sender, EventArgs e)
        {
            switch (wizardStep){
                case (int)WizardSteps.Finish:
                    wizardStep = 0; // restart wizard
                    break;
                    
                case (int)WizardSteps.SelectClass:
                    if (faList.Count == 0) wizardStep += 1;
                    else selectFAG();
                    
                    if (headgroupCategory == (int)LipidCategory.Glycerolipid)
                    {
                        if (creatorGUI.lipidCreator.headgroups[headgroup].attributes.Contains("sugar"))
                        {
                            ((Glycerolipid)lipid).containsSugar = true;
                        }
                        
                        if (headgroup == "DAG")
                        {
                            FattyAcidGroup currFAG = ((Glycerolipid)lipid).fag3;
                            currFAG.faTypes["FA"] = false;
                            currFAG.faTypes["FAa"] = false;
                            currFAG.faTypes["FAp"] = false;
                            currFAG.faTypes["FAx"] = true;
                        }
                        
                        else if (headgroup == "MAG")
                        {
                            FattyAcidGroup currFAG = ((Glycerolipid)lipid).fag2;
                            currFAG.faTypes["FA"] = false;
                            currFAG.faTypes["FAa"] = false;
                            currFAG.faTypes["FAp"] = false;
                            currFAG.faTypes["FAx"] = true;
                            
                            currFAG = ((Glycerolipid)lipid).fag3;
                            currFAG.faTypes["FA"] = false;
                            currFAG.faTypes["FAa"] = false;
                            currFAG.faTypes["FAp"] = false;
                            currFAG.faTypes["FAx"] = true;
                        }
                    }
                    
                    
                    else if (headgroupCategory == (int)LipidCategory.Glycerophospholipid)
                    {
                        if (creatorGUI.lipidCreator.headgroups[headgroup].attributes.Contains("lyso"))
                        {
                            ((Phospholipid)lipid).isLyso = true;
                        }
                    
                        else if (creatorGUI.lipidCreator.headgroups[headgroup].attributes.Contains("cardio"))
                        {
                            ((Phospholipid)lipid).isCL = true;
                        }
                        
                        if (headgroup == "MLCL")
                        {
                            FattyAcidGroup currFAG = ((Phospholipid)lipid).fag4;
                            currFAG.faTypes["FA"] = false;
                            currFAG.faTypes["FAa"] = false;
                            currFAG.faTypes["FAp"] = false;
                            currFAG.faTypes["FAx"] = true;
                        }
                    }
                    
                    
                    else if (headgroupCategory == (int)LipidCategory.Sphingolipid)
                    {
                        if (creatorGUI.lipidCreator.headgroups[headgroup].attributes.Contains("lyso"))
                        {
                            ((Sphingolipid)lipid).isLyso = true;
                        }
                    }
                    
                    
                    else if (headgroupCategory == (int)LipidCategory.Sterollipid)
                    {
                        if (headgroup == "ChE")
                        {
                            ((Cholesterol)lipid).containsEster = true;
                        }
                    }
                    
                    hasPlasmalogen = creatorGUI.lipidCreator.headgroups[headgroup].attributes.Contains("ether");
                    break;
                    
                    
            
                case (int)WizardSteps.SelectFA:
                    currentFA += 1;
                    if (currentFA < faList.Count)
                    {
                        wizardStep -= 1;
                        selectFAG();
                    }
                    break;
                    
                    
                
                case (int)WizardSteps.SelectAdduct:
                    bool adductSelected = false;
                    foreach (bool val in lipid.adducts.Values) adductSelected |= val;
                    if (!adductSelected)
                    {
                        MessageBox.Show("No adduct selected!", "Not registrable");
                        return;
                    }
                    break;
                
                case (int)WizardSteps.SelectFragmentMode:
                    if (filter == 1) wizardStep += 1;
                    break;
            }
            
            wizardStep += 1;
            processWizardSteps();
        }
        
        
        public void lcbHydroxyComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)lipid).lcb.hydroxylCounts.Clear();
            ((Sphingolipid)lipid).lcb.hydroxylCounts.Add(((ComboBox)sender).SelectedIndex + 2);
            ((Sphingolipid)lipid).lcb.hydroxylInfo = (((ComboBox)sender).SelectedIndex + 2).ToString();
        }
        
        public void faHydroxyComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)lipid).fag.hydroxylCounts.Clear();
            ((Sphingolipid)lipid).fag.hydroxylCounts.Add(((ComboBox)sender).SelectedIndex);
            ((Sphingolipid)lipid).fag.hydroxylInfo = (((ComboBox)sender).SelectedIndex).ToString();
        }
        
        
        private void hgListboxSelectedValueChanged(object sender, System.EventArgs e)
        {
            lipid.headGroupNames.Clear();
            headgroup = ((ListBox)sender).Items[((ListBox)sender).SelectedIndex].ToString();
            lipid.headGroupNames.Add(headgroup);
            if (creatorGUI.lipidCreator.headgroups.ContainsKey(headgroup))
            {
                int bbt = creatorGUI.lipidCreator.headgroups[headgroup].buildingBlockType;
                if (0 <= bbt && bbt < creatorGUI.lipidCreator.buildingBlockSets.Length)
                {
                    faList.Clear();
                    foreach (string bbType in creatorGUI.lipidCreator.buildingBlockSets[bbt])
                    {
                        if (bbType != "HG") faList.Add(bbType);
                    }
                    currentFA = 0;
                }
                else 
                {
                    // TODO: Exception
                }
            }
            else
            {
                // TODO: Exception
            }
        }
        
        
        void filterChanged(object sender, EventArgs e)
        {
            if (noPrecursorRadiobutton.Checked) filter = 0;
            else if (onlyPrecursorRadiobutton.Checked) filter = 1;
            else if (withPrecursorRadiobutton.Checked) filter = 2;
        }
        
        
        
        public void processWizardSteps()
        {
            resetAllControls();
            switch (wizardStep)
            {
                case (int)WizardSteps.Welcome:
                    labelInformation.Text = "Welcome to the magic world of LipidCreator. This wizard will guide you.";
                    break;
                    
                    
                    
                case (int)WizardSteps.SelectCategory:
                    labelInformation.Text = "Every journey begins with the first step. Let us begin draw your lipids. Which category do you desire?";
                    categoryCombobox.Visible = true;
                    headgroup = "";
                    categoryCombobox.SelectedIndex = 0;
                    break;
                    
                    
                    
                case (int)WizardSteps.SelectClass:
                    labelInformation.Text = "What lipid class do you wish to select?";
                    backButton.Enabled = true;
                    switch((string)categoryCombobox.Items[categoryCombobox.SelectedIndex])
                    {
                        case "Glycero lipid":
                            headgroupCategory = (int)LipidCategory.Glycerolipid;
                            lipid = new Glycerolipid(creatorGUI.lipidCreator);
                            break;
                        case "Glycerophosho lipid":
                            headgroupCategory = (int)LipidCategory.Glycerophospholipid;
                            lipid = new Phospholipid(creatorGUI.lipidCreator);
                            break;
                        case "Sphingo lipid":
                            headgroupCategory = (int)LipidCategory.Sphingolipid;
                            lipid = new Sphingolipid(creatorGUI.lipidCreator);
                            break;
                        case "Sterol lipid":
                            headgroupCategory = (int)LipidCategory.Sterollipid;
                            lipid = new Cholesterol(creatorGUI.lipidCreator);
                            break;
                        case "Lipid mediator":
                            headgroupCategory = (int)LipidCategory.LipidMediator;
                            lipid = new Mediator(creatorGUI.lipidCreator);
                            break;
                        default:
                            throw new Exception("invalid lipid category");
                    }
                    // clear all adducts
                    ArrayList adducts = new ArrayList();
                    foreach (string adduct in lipid.adducts.Keys) adducts.Add(adduct);
                    foreach (string adduct in adducts) lipid.adducts[adduct] = false;
                    
                    hgListbox.Visible = true;
                    hgListbox.Items.Clear();
                    int hgListSelect = 0;
                    int ii = 0;
                    foreach (string hg in creatorGUI.lipidCreator.categoryToClass[headgroupCategory])
                    {
                        if (!creatorGUI.lipidCreator.headgroups[hg].attributes.Contains("heavy"))
                        {
                            if (hg == headgroup) hgListSelect = ii;
                            hgListbox.Items.Add(hg);
                        }
                        ii += 1;
                    }
                    hgListbox.SelectedIndex = hgListSelect;
                    break;
                    
                    
                    
                case (int)WizardSteps.SelectFA:
                    labelInformation.Text = "Please select for '" + (string)faList[currentFA] + "' carbon length, number of double bonds and number of hydroxyl groups?";
                    backButton.Enabled = true;
                    faCombobox.Visible = true;
                    faCheckbox1.Visible = true;
                    faCheckbox2.Visible = true;
                    faCheckbox3.Visible = true;
                    
                    faCheckbox1.Enabled = true;
                    faCheckbox2.Enabled = true;
                    faCheckbox3.Enabled = true;
                    
                    faTextbox.Visible = true;
                    dbTextbox.Visible = true;
                    dbLabel.Visible = true;
                    hydroxylTextbox.Visible = true;
                    hydroxylLabel.Visible = true;
                    faTextbox.Text = fag.lengthInfo;
                    dbTextbox.Text = fag.dbInfo;
                    hydroxylTextbox.Text = fag.hydroxylInfo;
                    faCombobox.SelectedIndex = fag.chainType;
                    
                    faCheckbox1.Checked = fag.faTypes["FA"];
                    faCheckbox2.Checked = fag.faTypes["FAp"];
                    faCheckbox3.Checked = fag.faTypes["FAa"];
                    
                    if (headgroupCategory == (int)LipidCategory.Glycerophospholipid)
                    {
                        if (currentFA == 0)
                        {
                            faCheckbox1.Enabled = false;
                            faCheckbox2.Enabled = false;
                            faCheckbox3.Enabled = false;
                        
                            faCheckbox1.Checked = !headgroup.EndsWith("O-p") && !headgroup.EndsWith("O-a");
                            faCheckbox2.Checked = headgroup.EndsWith("O-p");
                            faCheckbox3.Checked = headgroup.EndsWith("O-a");
                        }
                        else if (currentFA > 0)
                        {
                            faCheckbox1.Visible = false;
                            faCheckbox2.Visible = false;
                            faCheckbox3.Visible = false;
                        }
                    }
                    else if (headgroupCategory == (int)LipidCategory.Sphingolipid)
                    {
                        faCheckbox1.Visible = false;
                        faCheckbox2.Visible = false;
                        faCheckbox3.Visible = false;
                        hydroxylTextbox.Visible = false;
                        
                        lcbHydroxyCombobox.SelectedIndex = ((Sphingolipid)lipid).lcb.hydroxylCounts.First() - 2;
                        faHydroxyCombobox.SelectedIndex = ((Sphingolipid)lipid).fag.hydroxylCounts.First();
                        
                        if ((string)faList[currentFA] == "LCB") lcbHydroxyCombobox.Visible = true;
                        else faHydroxyCombobox.Visible = true;
                    }
                    
                    else if (headgroupCategory == (int)LipidCategory.Sterollipid)
                    {
                        faCheckbox1.Visible = false;
                        faCheckbox2.Visible = false;
                        faCheckbox3.Visible = false;
                    }
                    break;
                    
                    
                    
                case (int)WizardSteps.SelectAdduct:
                    labelInformation.Text = "Select adduct(s)";
                    backButton.Enabled = true;
                    positiveAdduct.Visible = true;
                    negativeAdduct.Visible = true;
                    posAdductCheckbox1.Checked = lipid.adducts["+H"];
                    posAdductCheckbox2.Checked = lipid.adducts["+2H"];
                    posAdductCheckbox3.Checked = lipid.adducts["+NH4"];
                    negAdductCheckbox1.Checked = lipid.adducts["-H"];
                    negAdductCheckbox2.Checked = lipid.adducts["-2H"];
                    negAdductCheckbox3.Checked = lipid.adducts["+HCOO"];
                    negAdductCheckbox4.Checked = lipid.adducts["+CH3COO"];
                    
                    posAdductCheckbox1.Enabled = creatorGUI.lipidCreator.headgroups[headgroup].adductRestrictions["+H"];
                    posAdductCheckbox2.Enabled = creatorGUI.lipidCreator.headgroups[headgroup].adductRestrictions["+2H"];
                    posAdductCheckbox3.Enabled = creatorGUI.lipidCreator.headgroups[headgroup].adductRestrictions["+NH4"];
                    negAdductCheckbox1.Enabled = creatorGUI.lipidCreator.headgroups[headgroup].adductRestrictions["-H"];
                    negAdductCheckbox2.Enabled = creatorGUI.lipidCreator.headgroups[headgroup].adductRestrictions["-2H"];
                    negAdductCheckbox3.Enabled = creatorGUI.lipidCreator.headgroups[headgroup].adductRestrictions["+HCOO"];
                    negAdductCheckbox4.Enabled = creatorGUI.lipidCreator.headgroups[headgroup].adductRestrictions["+CH3COO"];
                    break;
                    
                    
                    
                case (int)WizardSteps.SelectFragmentMode:
                    labelInformation.Text = "Select filter mode";
                    backButton.Enabled = true;
                    filterGroupbox.Visible = true;
                    noPrecursorRadiobutton.Checked = filter == 0;
                    onlyPrecursorRadiobutton.Checked = filter == 1;
                    withPrecursorRadiobutton.Checked = filter == 2;
                    break;
                    
                    
                case (int)WizardSteps.SelectFragments:
                    labelInformation.Text = "Select fragment";
                    backButton.Enabled = true;
                    break;
                    
                    
                case (int)WizardSteps.AddLipid:
                    labelInformation.Text = "Please continue to add the lipid now into LipidCreator";
                    backButton.Enabled = true;
                    break;
                    
                    
                case (int)WizardSteps.Finish:
                    ulong lipidHash = 0;
                    if (lipid is Glycerolipid) lipidHash = ((Glycerolipid)lipid).getHashCode();
                    else if (lipid is Phospholipid) lipidHash = ((Phospholipid)lipid).getHashCode();
                    else if (lipid is Sphingolipid) lipidHash = ((Sphingolipid)lipid).getHashCode();
                    else if (lipid is Cholesterol) lipidHash = ((Cholesterol)lipid).getHashCode();
                    else if (lipid is Mediator) lipidHash = ((Mediator)lipid).getHashCode();
                    else if (lipid is UnsupportedLipid) lipidHash = ((UnsupportedLipid)lipid).getHashCode();
                
                    if (!creatorGUI.lipidCreator.registeredLipidDictionary.ContainsKey(lipidHash))
                    {
                        lipid.onlyPrecursors = filter;
                        lipid.onlyHeavyLabeled = 0;
                        creatorGUI.lipidCreator.registeredLipidDictionary.Add(lipidHash, lipid);
                        creatorGUI.lipidCreator.registeredLipids.Add(lipidHash);
                    }
                        
                    creatorGUI.refreshRegisteredLipidsTable();
                    labelInformation.Text = "Do you want to add another lipid?";
                    break;
        
            }
        }
    }
}
