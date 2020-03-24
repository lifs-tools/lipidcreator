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
using System.Data;
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
        public bool senderInterupt = false;
        
        
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
            backButton.Text = "Back";
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
                    fag = selectFAG(currentFA);
                }
            }
            else if (wizardStep == (int)WizardSteps.SelectFA)
            {
                currentFA -= 1;
                if (currentFA >= 0)
                {
                    wizardStep += 1;
                    fag = selectFAG(currentFA);
                }
            }
            
            else if (wizardStep == (int)WizardSteps.AddLipid)
            {
                if (filter == 1) wizardStep -= 1;
            }
            
            wizardStep -= 1;
            processWizardSteps();
        }
        
        

        private void cancelClick(object sender, EventArgs e)
        {
            if (closeWizardDialog(true))
            {
                this.Close();
            }
        }
        
        
        
        private FattyAcidGroup selectFAG(int currFA)
        {
            FattyAcidGroup currFAG = null;
            if (currFA <= -1 || faList.Count <= currFA) return currFAG;
            switch (headgroupCategory)
            {
                case (int)LipidCategory.Glycerolipid:
                    if ((string)faList[currFA] == "FA1" || (string)faList[currFA] == "FA") currFAG = ((Glycerolipid)lipid).fag1;
                    else if ((string)faList[currFA] == "FA2") currFAG = ((Glycerolipid)lipid).fag2;
                    else if ((string)faList[currFA] == "FA3") currFAG = ((Glycerolipid)lipid).fag3;
                    break;
                    
                case (int)LipidCategory.Glycerophospholipid:
                    if ((string)faList[currFA] == "FA1" || (string)faList[currFA] == "FA") currFAG = ((Phospholipid)lipid).fag1;
                    else if ((string)faList[currFA] == "FA2") currFAG = ((Phospholipid)lipid).fag2;
                    else if ((string)faList[currFA] == "FA3") currFAG = ((Phospholipid)lipid).fag3;
                    else if ((string)faList[currFA] == "FA4") currFAG = ((Phospholipid)lipid).fag4;
                    break;
                    
                case (int)LipidCategory.Sphingolipid:
                    if ((string)faList[currFA] == "LCB") currFAG = ((Sphingolipid)lipid).lcb;
                    else if ((string)faList[currFA] == "FA") currFAG = ((Sphingolipid)lipid).fag;
                    break;
                    
                case (int)LipidCategory.Sterollipid:
                    if ((string)faList[currFA] == "FA") currFAG = ((Cholesterol)lipid).fag;
                    break;
                
                default:
                    break;
            }
            
            return currFAG;
        }
        
        
        
        
        public bool faCheck()
        {
            if (faTextbox.BackColor == creatorGUI.alertColor)
            {
                MessageBox.Show("Fatty acyl length content not valid!", "Values not valid");
                return  false;
            }
            if (dbTextbox.BackColor == creatorGUI.alertColor)
            {
                MessageBox.Show("Double bond content not valid!", "Values not valid");
                return  false;
            }
            if (hydroxylTextbox.BackColor == creatorGUI.alertColor)
            {
                MessageBox.Show("Hydroxyl content not valid!", "Values not valid");
                return  false;
            }
            if (!faCheckbox1.Checked && !faCheckbox2.Checked && !faCheckbox3.Checked)
            {
                MessageBox.Show("Please select at least fatty acyl type!", "Values not valid");
                return  false;
            }
            return true;
        }
        
        
        

        private void continueClick(object sender, EventArgs e)
        {
            switch (wizardStep){
                case (int)WizardSteps.Finish:
                    wizardStep = 0; // restart wizard
                    break;
                    
                case (int)WizardSteps.SelectClass:
                    hgComboboxSelectedValueChanged(null, null);
                    
                    if (faList.Count == 0) wizardStep += 1;
                    else fag = selectFAG(currentFA);
                    
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
                    if (!faCheck()) return;
                    // update ranges
                    creatorGUI.updateRanges(fag, faTextbox, faCombobox.SelectedIndex);
                    creatorGUI.updateRanges(fag, dbTextbox, 3);
                    if (headgroupCategory != (int)LipidCategory.Sphingolipid) creatorGUI.updateRanges(fag, hydroxylTextbox, 4);
                    
                    
                    if (currentFA == 0 && faRepresentative.Checked && faList.Count > 1)
                    {
                        FattyAcidGroup repFAG = selectFAG(0);
                        for(int j = 1; j < faList.Count; ++j)
                        {
                            FattyAcidGroup currFAG = selectFAG(j);
                            currFAG.lengthInfo = repFAG.lengthInfo;
                            currFAG.dbInfo = repFAG.dbInfo;
                            currFAG.hydroxylInfo = repFAG.hydroxylInfo;
                            currFAG.chainType = repFAG.chainType;
                            
                            
                            creatorGUI.updateCarbon(faTextbox, new FattyAcidEventArgs(currFAG, "" ));
                            creatorGUI.updateOddEven(faCombobox, new FattyAcidEventArgs(currFAG, faTextbox ));
                            creatorGUI.updateDB(dbTextbox, new FattyAcidEventArgs(currFAG, "" ));
                            creatorGUI.updateHydroxyl(hydroxylTextbox, new FattyAcidEventArgs(currFAG, "" ));
                            
                        }
                    }
                    
                    
                    
                    currentFA += 1;
                    if (currentFA < faList.Count)
                    {
                        wizardStep -= 1;
                        fag = selectFAG(currentFA);
                    }
                    break;
                    
                    
                
                case (int)WizardSteps.SelectAdduct:
                    bool adductSelected = false;
                    foreach (bool val in lipid.adducts.Values) adductSelected |= val;
                    if (!adductSelected)
                    {
                        MessageBox.Show("No adduct selected!", "Adduct required");
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
        
        
        void checkedListBoxPositiveSelectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            lipid.positiveFragments[headgroup].Clear();
            for (int i = 0; i < checkedListBoxPositiveFragments.Items.Count; ++i)
            {
                lipid.positiveFragments[headgroup].Add((string)checkedListBoxPositiveFragments.Items[i]);
                checkedListBoxPositiveFragments.SetItemChecked(i, true);
            }
            senderInterupt = false;
        }
        
        
        
        void checkedListBoxNegativeSelectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            lipid.negativeFragments[headgroup].Clear();
            for (int i = 0; i < checkedListBoxNegativeFragments.Items.Count; ++i)
            {
                lipid.negativeFragments[headgroup].Add((string)checkedListBoxNegativeFragments.Items[i]);
                checkedListBoxNegativeFragments.SetItemChecked(i, true);
            }
            senderInterupt = false;
        }
        
        
        
        void checkedListBoxPositiveDeselectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            lipid.positiveFragments[headgroup].Clear();
            for (int i = 0; i < checkedListBoxPositiveFragments.Items.Count; ++i)
            {
                checkedListBoxPositiveFragments.SetItemChecked(i, false);
            }
            senderInterupt = false;
        }
        
        
        
        void checkedListBoxNegativeDeselectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            lipid.negativeFragments[headgroup].Clear();
            for (int i = 0; i < checkedListBoxNegativeFragments.Items.Count; ++i)
            {
                checkedListBoxNegativeFragments.SetItemChecked(i, false);
            }
            senderInterupt = false;
        }
        
        
        
        public void CheckedListBoxPositiveItemCheck(Object sender, ItemCheckEventArgs e)
        {
            if (senderInterupt) return;
            if (e.NewValue == CheckState.Checked)
            {
                lipid.positiveFragments[headgroup].Add((string)checkedListBoxPositiveFragments.Items[e.Index]);
            }
            else
            {
                lipid.positiveFragments[headgroup].Remove((string)checkedListBoxPositiveFragments.Items[e.Index]);
            }
        }
        
        
        public void CheckedListBoxNegativeItemCheck(Object sender, ItemCheckEventArgs e)
        {
            if (senderInterupt) return;
            if (e.NewValue == CheckState.Checked)
            {
                lipid.negativeFragments[headgroup].Add((string)checkedListBoxNegativeFragments.Items[e.Index]);
            }
            else
            {
                lipid.negativeFragments[headgroup].Remove((string)checkedListBoxNegativeFragments.Items[e.Index]);
            }
        }
        
        
        
        private void hgComboboxSelectedValueChanged(object sender, System.EventArgs e)
        {
            if (senderInterupt) return;
            lipid.headGroupNames.Clear();
            headgroup = (string)hgCombobox.Items[hgCombobox.SelectedIndex];
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
                    throw new Exception("invalid lipid type");
                }
            }
            else
            {
                throw new Exception("invalid lipid '" + headgroup + "'");
            }
        }

        private bool closeWizardDialog(bool ignoreWizardCondition)
        {
            bool closingCondition = ignoreWizardCondition || ((wizardStep == (int)WizardSteps.Welcome) || (wizardStep == (int)WizardSteps.Finish));
            if (!closingCondition)
            {
                DialogResult result = MessageBox.Show("Do you want to close the wizard?", "Close Wizard", MessageBoxButtons.YesNo);
                closingCondition = result == DialogResult.Yes;
            }
            return closingCondition;
        }
        
        
        private void closing(Object sender, FormClosingEventArgs e)
        {
            //received after this.Close() has been called, do not call Close again here!
            if(!closeWizardDialog(false))
            {
                e.Cancel = true;
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
                    labelTitle.Text = "LipidCreator Wizard";
                    labelInformation.Text = "Welcome to the magic world of LipidCreator. This wizard will guide you through" + Environment.NewLine +
                    "the enchanted world of lipids. Please continue if you feel ready to face the challenge.";
                    cancelButton.Text = "Close";
                    break;
                    
                    
                    
                case (int)WizardSteps.SelectCategory:
                    labelTitle.Text = "Select a lipid category";
                    labelInformation.Text = "Every journey begins with the first step. Let us assemble your hydrophobic or amphiphilic party." + Environment.NewLine +
                    "Which category do you desire?";
                    categoryCombobox.Visible = true;
                    headgroup = "";
                    categoryCombobox.SelectedIndex = 0;
                    break;
                    
                    
                    
                case (int)WizardSteps.SelectClass:
                    labelTitle.Text = "Select a lipid class";
                    labelInformation.Text = "Thou shalt delve deeper into the matter. Choose your lipid class, but choose wisely." + Environment.NewLine + "You may go back at any time to repent and revise.";
                    backButton.Enabled = true;
                    faRepresentative.Checked = false;
                    switch((string)categoryCombobox.Items[categoryCombobox.SelectedIndex])
                    {
                        case "Glycerolipids":
                            headgroupCategory = (int)LipidCategory.Glycerolipid;
                            lipid = new Glycerolipid(creatorGUI.lipidCreator);
                            break;
                        case "Glycerophospholipids":
                            headgroupCategory = (int)LipidCategory.Glycerophospholipid;
                            lipid = new Phospholipid(creatorGUI.lipidCreator);
                            break;
                        case "Sphingolipids":
                            headgroupCategory = (int)LipidCategory.Sphingolipid;
                            lipid = new Sphingolipid(creatorGUI.lipidCreator);
                            break;
                        case "Sterol lipids":
                            headgroupCategory = (int)LipidCategory.Sterollipid;
                            lipid = new Cholesterol(creatorGUI.lipidCreator);
                            break;
                        case "Lipid Mediators":
                            headgroupCategory = (int)LipidCategory.LipidMediator;
                            lipid = new Mediator(creatorGUI.lipidCreator);
                            break;
                        default:
                            throw new Exception("invalid lipid category");
                    }
                    // clear all adducts
                    ArrayList adducts = new ArrayList();
                    lipid.onlyHeavyLabeled = 0;
                    foreach (string adduct in lipid.adducts.Keys) adducts.Add(adduct);
                    foreach (string adduct in adducts) lipid.adducts[adduct] = false;
                    
                    hgCombobox.Visible = true;
                    senderInterupt = true;
                    hgCombobox.Items.Clear();
                    int hgListSelect = 0;
                    int ii = 0;
                    foreach (string hg in creatorGUI.lipidCreator.categoryToClass[headgroupCategory])
                    {
                        if (!creatorGUI.lipidCreator.headgroups[hg].attributes.Contains("heavy"))
                        {
                            if (hg == headgroup) hgListSelect = ii;
                            hgCombobox.Items.Add(hg);
                        }
                        ii += 1;
                    }
                    hgCombobox.SelectedIndex = hgListSelect;
                    senderInterupt = false;
                    break;
                    
                    
                    
                case (int)WizardSteps.SelectFA:
                    labelTitle.Text = "Select parameters for ";
                    
                    switch((string)faList[currentFA])
                    {
                        case "LCB":
                            labelTitle.Text += "long chain base";
                            break;
                            
                        case "FA":
                            labelTitle.Text += "fatty acyl chain";
                            break;
                            
                        case "FA1":
                            labelTitle.Text += "first fatty acyl chain";
                            break;
                            
                        case "FA2":
                            labelTitle.Text += "second fatty acyl chain";
                            break;
                            
                        case "FA3":
                            labelTitle.Text += "third fatty acyl chain";
                            break;
                            
                        case "FA4":
                            labelTitle.Text += "fourth fatty acyl chain";
                            break;
                    }
                    
                
                
                    labelInformation.Text = "It's the inner attributes that matter. For '" + (string)faList[currentFA] + "', please select:" + Environment.NewLine +
                    "carbon length, number of double bonds and number of hydroxyl groups.";
                    backButton.Enabled = true;
                    faCombobox.Visible = true;
                    faCheckbox1.Visible = true;
                    faCheckbox2.Visible = true;
                    faCheckbox3.Visible = true;
                    
                    faCheckbox1.Enabled = true;
                    faCheckbox2.Enabled = true;
                    faCheckbox3.Enabled = true;
                    
                    if (faList.Count > 1 && currentFA == 0 && (headgroupCategory == (int)LipidCategory.Glycerolipid) || (headgroupCategory == (int)LipidCategory.Glycerophospholipid) || (headgroupCategory == (int)LipidCategory.Sphingolipid))
                    {
                        faRepresentative.Visible = true;
                    }
                    
                    faTextbox.Visible = true;
                    dbTextbox.Visible = true;
                    dbLabel.Visible = true;
                    hydroxylTextbox.Visible = true;
                    hydroxylLabel.Visible = true;
                    faTextbox.Text = fag.lengthInfo;
                    dbTextbox.Text = fag.dbInfo;
                    hydroxylTextbox.Text = fag.hydroxylInfo;
                    
                    faCheckbox1.Checked = fag.faTypes["FA"];
                    faCheckbox2.Checked = fag.faTypes["FAp"];
                    faCheckbox3.Checked = fag.faTypes["FAa"];
                    
                    faCombobox.Items.Clear();
                    faCombobox.Items.Add("Fatty acyl chain");
                    faCombobox.Items.Add("Fatty acyl chain - odd");
                    faCombobox.Items.Add("Fatty acyl chain - even");
                    
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
                        
                        if ((string)faList[currentFA] == "LCB")
                        {
                            faCombobox.Items.Clear();
                            faCombobox.Items.Add("Long chain base");
                            faCombobox.Items.Add("Long chain base - odd");
                            faCombobox.Items.Add("Long chain base - even");
                            lcbHydroxyCombobox.Visible = true;
                        }
                        else faHydroxyCombobox.Visible = true;
                    }
                    
                    else if (headgroupCategory == (int)LipidCategory.Sterollipid)
                    {
                        faCheckbox1.Visible = false;
                        faCheckbox2.Visible = false;
                        faCheckbox3.Visible = false;
                    }
                    faCombobox.SelectedIndex = fag.chainType;
                    break;
                    
                    
                    
                case (int)WizardSteps.SelectAdduct:
                    labelTitle.Text = "Select adducts";
                    labelInformation.Text = "We need ionized fellows for the battle. Please select at least one fine adduct.";
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
                    labelTitle.Text = "Select precursor filter mode";
                    labelInformation.Text = "To be or not to be fragmented. Please choose the right filter mode.";
                    backButton.Enabled = true;
                    filterGroupbox.Visible = true;
                    noPrecursorRadiobutton.Checked = filter == 0;
                    onlyPrecursorRadiobutton.Checked = filter == 1;
                    withPrecursorRadiobutton.Checked = filter == 2;
                    break;
                    
                    
                    
                case (int)WizardSteps.SelectFragments:
                    labelTitle.Text = "Select fragments";
                    labelInformation.Text = "The encounter requires a quick but sensible selection of fragments.";
                    backButton.Enabled = true;
                    
                    if (lipid.adducts["+H"] || lipid.adducts["+2H"] || lipid.adducts["+NH4"])
                    {
                        checkedListBoxPositiveFragments.Visible = true;
                        labelPositiveFragments.Visible = true;
                        labelPositiveSelectAll.Visible = true;
                        labelPositiveDeselectAll.Visible = true;
                        labelSlashPositive.Visible = true;
                    
                        checkedListBoxPositiveFragments.Items.Clear();
                        foreach (KeyValuePair<string, MS2Fragment> currentFragment in creatorGUI.lipidCreator.allFragments[headgroup][true])
                        {
                            checkedListBoxPositiveFragments.Items.Add(currentFragment.Value.fragmentName);
                            checkedListBoxPositiveFragments.SetItemChecked(checkedListBoxPositiveFragments.Items.Count - 1, lipid.positiveFragments[headgroup].Contains(currentFragment.Value.fragmentName));
                        }
                    }
                    
                    if (lipid.adducts["-H"] || lipid.adducts["-2H"] || lipid.adducts["+HCOO"] || lipid.adducts["+CH3COO"])
                    {
                    
                        checkedListBoxNegativeFragments.Visible = true;
                        labelNegativeFragments.Visible = true;
                        labelNegativeSelectAll.Visible = true;
                        labelNegativeDeselectAll.Visible = true;
                        labelSlashNegative.Visible = true;
                        
                        checkedListBoxNegativeFragments.Items.Clear();
                        foreach (KeyValuePair<string, MS2Fragment> currentFragment in creatorGUI.lipidCreator.allFragments[headgroup][false])
                        {
                            checkedListBoxNegativeFragments.Items.Add(currentFragment.Value.fragmentName);
                            checkedListBoxNegativeFragments.SetItemChecked(checkedListBoxNegativeFragments.Items.Count - 1, lipid.negativeFragments[headgroup].Contains(currentFragment.Value.fragmentName));
                        }
                    }
                    
                    
                    break;
                    
                    
                case (int)WizardSteps.AddLipid:
                    labelTitle.Text = "Confirm your selection";
                    labelInformation.Text = "Thou shalt not pass with your party of lipids before you confirm to continue." + Environment.NewLine + 
                    "Do you really want to proceed beyond the realm and protection of this wizard?";
                    backButton.Enabled = true;
                    lipidPreview.Visible = true;
                    
                    lipidDataTable.Rows.Clear();
                    
                    DataRow rowCategory = lipidDataTable.NewRow();
                    rowCategory["Key"] = "Category";
                    switch (headgroupCategory)
                    {
                        case (int)LipidCategory.Glycerolipid:
                            rowCategory["Value"] = "Glycerolipids";
                            break;
                            
                        case (int)LipidCategory.Glycerophospholipid:
                            rowCategory["Value"] = "Glycerophospholipids";
                            break;
                            
                        case (int)LipidCategory.Sphingolipid:
                            rowCategory["Value"] = "Sphingolipids";
                            break;
                            
                        case (int)LipidCategory.Sterollipid:
                            rowCategory["Value"] = "Sterol lipids";
                            break;
                            
                        case (int)LipidCategory.LipidMediator:
                            rowCategory["Value"] = "Lipid Mediators";
                            break;
                    }
                    
                    lipidDataTable.Rows.Add(rowCategory);
                    
                    
                    DataRow rowClass = lipidDataTable.NewRow();
                    rowClass["Key"] = "Class";
                    rowClass["Value"] = headgroup;
                    lipidDataTable.Rows.Add(rowClass);
                    
                    for (int i = 0; i < faList.Count; ++i)
                    {
                        DataRow rowBlock = lipidDataTable.NewRow();
                        rowBlock["Key"] = faList[i];
                        
                        FattyAcidGroup currFAG = selectFAG(i);
                        rowBlock["Value"] = creatorGUI.FARepresentation(currFAG) + currFAG.lengthInfo + "; DB: " + currFAG.dbInfo + "; OH: " + currFAG.hydroxylInfo;
                        lipidDataTable.Rows.Add(rowBlock);
                    }
                    
                    string adductsStr = "";
                    foreach (Adduct adduct in Lipid.ALL_ADDUCTS.Values)
                    {
                        if (lipid.adducts[adduct.name]) adductsStr += (adductsStr.Length > 0 ? ", " : "") + adduct.visualization;
                    }
                    DataRow rowAdduct = lipidDataTable.NewRow();
                    rowAdduct["Key"] = "Adduct(s)";
                    rowAdduct["Value"] = adductsStr;
                    lipidDataTable.Rows.Add(rowAdduct);
                    
                    string filtersStr = "";
                    switch (lipid.onlyPrecursors)
                    {
                        case 0: filtersStr = "no precursors, "; break;
                        case 1: filtersStr = "only precursors, "; break;
                        case 2: filtersStr = "with precursors, "; break;
                    }
                    
                    switch (lipid.onlyHeavyLabeled)
                    {
                        case 0: filtersStr += "no heavy"; break;
                        case 1: filtersStr += "only heavy"; break;
                        case 2: filtersStr += "with heavy"; break;
                    }
                    DataRow rowFilter = lipidDataTable.NewRow();
                    rowFilter["Key"] = "Filter";
                    rowFilter["Value"] = filtersStr;
                    lipidDataTable.Rows.Add(rowFilter);
                    
                    lipidPreview.Update ();
                    lipidPreview.Refresh ();
                    
                    break;
                    
                    
                case (int)WizardSteps.Finish:
                    cancelButton.Text = "Cancel";
                    continueButton.Text = "Yes";
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
                    labelTitle.Text = "Lipid was added to LipidCreator's Lipid List";
                    labelInformation.Text = "Congratulations, it was a great adventure. Do you wish to repeat your journey?";
                    break;
        
            }
        }
    }
}
