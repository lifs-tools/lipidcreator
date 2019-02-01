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
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using log4net;
using System.ComponentModel;
using System.Diagnostics;

namespace LipidCreator
{

    [Serializable]
    public partial class CreatorGUI : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CreatorGUI));
        public bool changingTabForced;
        public ArrayList lipidTabList;
        public int currentTabIndex = 1;
        public LipidCreator lipidCreator;
        [NonSerialized]
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
        public MS2Form ms2fragmentsForm = null;
        public CEInspector ceInspector = null;
        public MediatorMS2Form mediatorMS2fragmentsForm = null;
        [NonSerialized]
        public AddHeavyPrecursor addHeavyPrecursor = null;
        public LipidsReview lipidsReview = null;
        [NonSerialized]
        public LipidsInterList lipidsInterList = null;
        [NonSerialized]
        public FilterDialog filterDialog = null;
        [NonSerialized]
        public MenuItem lastCEInstrumentChecked = null;
        public bool asDeveloper = false;
        
        
        
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
            registeredLipidsDatatable.Columns.Add(new DataColumn("Filters"));
            InitializeComponent();
            
            // add predefined menu
            lipidModifications = Enumerable.Repeat(-1, Enum.GetNames(typeof(LipidCategory)).Length).ToArray();
            changingTabForced = false;
            string predefinedFolder = lipidCreator.prefixPath + "data/predefined";
            if(Directory.Exists(predefinedFolder)) 
            {
                string [] subdirectoryEntries = Directory.GetDirectories(predefinedFolder);
                foreach (string subdirectoryEntry in subdirectoryEntries)
                {
                    string subEntry = subdirectoryEntry.Replace(predefinedFolder + Path.DirectorySeparatorChar, "");   
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
            
            Rectangle r = Screen.FromControl(this).Bounds;
            double hgt = r.Height * 0.9;
            if (hgt > minWindowHeight)
            {
                this.Width = (int)hgt;
            }
            glPosAdductCheckbox1.Enabled = false;
            glPosAdductCheckbox2.Enabled = false;
            glPosAdductCheckbox3.Enabled = false;
            glNegAdductCheckbox1.Enabled = false;
            glNegAdductCheckbox2.Enabled = false;
            glNegAdductCheckbox3.Enabled = false;
            glNegAdductCheckbox4.Enabled = false;
            
            plPosAdductCheckbox1.Enabled = false;
            plPosAdductCheckbox2.Enabled = false;
            plPosAdductCheckbox3.Enabled = false;
            plNegAdductCheckbox1.Enabled = false;
            plNegAdductCheckbox2.Enabled = false;
            plNegAdductCheckbox3.Enabled = false;
            plNegAdductCheckbox4.Enabled = false;
            
            slPosAdductCheckbox1.Enabled = false;
            slPosAdductCheckbox2.Enabled = false;
            slPosAdductCheckbox3.Enabled = false;
            slNegAdductCheckbox1.Enabled = false;
            slNegAdductCheckbox2.Enabled = false;
            slNegAdductCheckbox3.Enabled = false;
            slNegAdductCheckbox4.Enabled = false;
            
            chPosAdductCheckbox1.Enabled = false;
            chPosAdductCheckbox2.Enabled = false;
            chPosAdductCheckbox3.Enabled = false;
            chNegAdductCheckbox1.Enabled = false;
            chNegAdductCheckbox2.Enabled = false;
            chNegAdductCheckbox3.Enabled = false;
            chNegAdductCheckbox4.Enabled = false;
            
            medNegAdductCheckbox1.Enabled = false;
            medNegAdductCheckbox2.Enabled = false;
            medNegAdductCheckbox3.Enabled = false;
            medNegAdductCheckbox4.Enabled = false;
            changeTab(0);
            
            // add instruments into menu for collision energy optimization
            for (int i = 1; i < lipidCreator.availableInstruments.Count; ++i)
            {
                string instrument = (string)lipidCreator.availableInstruments[i];
                if (lipidCreator.msInstruments.ContainsKey(instrument)){
                    int numModes = lipidCreator.msInstruments[instrument].modes.Count;
                    
                    foreach (string instrumentMode in lipidCreator.msInstruments[instrument].modes)
                    {
                        MenuItem instrumentItem = new MenuItem();
                        instrumentItem.Text = lipidCreator.msInstruments[instrument].model;
                        instrumentItem.RadioCheck = true;
                        instrumentItem.Tag = new string[]{instrument, ""};
                        
                        switch (instrumentMode)
                        {
                            case "PRM":
                                if (numModes > 1) instrumentItem.Text += " (PRM)";
                                instrumentItem.Click += new System.EventHandler (changeInstrumentForCEtypePRM);
                                ((string[])instrumentItem.Tag)[1] = MonitoringTypes.PRM.ToString();
                                break;
                                
                            case "SRM":
                                if (numModes > 1) instrumentItem.Text += " (SRM)";
                                instrumentItem.Click += new System.EventHandler (changeInstrumentForCEtypeSRM);
                                ((string[])instrumentItem.Tag)[1] = MonitoringTypes.SRM.ToString();
                                break;
                            
                            case "SIM/SRM":
                                if (numModes > 1) instrumentItem.Text += " (SIM/SRM)";
                                instrumentItem.Click += new System.EventHandler (changeInstrumentForCEtypeSRM);
                                ((string[])instrumentItem.Tag)[1] = MonitoringTypes.SRM.ToString();
                                break;
                                
                            default:
                                throw new Exception("Error: monitoring mode '" + instrumentMode + "' not supported for instrument '" + lipidCreator.msInstruments[instrument].model + "'");
                        }
                        menuCollisionEnergy.MenuItems.Add(instrumentItem);
                    }
                }
            }
            lastCEInstrumentChecked = menuCollisionEnergyNone;
        }
        
        
        
        
        
        
        public void resetLipidCreatorMenu(Object sender, EventArgs e)
        {
            resetLipidCreator();
        }
        
        
        
        
        
        
        public void statisticsMenu(Object sender, EventArgs e)
        {
            menuStatistics.Checked = !menuStatistics.Checked;
            lipidCreator.enableAnalytics = menuStatistics.Checked;
            string analyticsFile = lipidCreator.prefixPath + "data/analytics.txt";
            try {
                using (StreamWriter outputFile = new StreamWriter (analyticsFile))
                {
                    outputFile.WriteLine ((lipidCreator.enableAnalytics ? "1" : "0"));
                }
            }
            catch (Exception ex)
            {
                log.Warn("Could not write to data/analytics.txt:", ex);
            }
        }
        
        
        
        public void clearLipidList(Object sender, EventArgs e)
        {
            lipidCreator.registeredLipids.Clear();
            registeredLipidsDatatable.Rows.Clear();
            refreshRegisteredLipidsTable();
        }
        
        
        public bool resetLipidCreator(bool verify = true)
        {
            DialogResult mbr = DialogResult.Yes;
            if (verify) mbr = MessageBox.Show ("You are going to reset LipidCreator. All information and settings will be discarded. Are you sure?", "Reset LipidCreator", MessageBoxButtons.YesNo);
            if (!verify || (verify && mbr == DialogResult.Yes))
            {
                lipidCreator = new LipidCreator(inputParameters);
                resetAllLipids();
                updateCECondition();
                refreshRegisteredLipidsTable();
                changeTab((int)currentIndex);
                return true;
            }
            return false;
        }
        
        
        public void updateCECondition()
        {
            if (lipidCreator.selectedInstrumentForCE == "")
            {
                unsetInstrument((Object)menuCollisionEnergyNone, null);
            }
            else {
                bool unsetTheInstrument = true; 
                foreach (MenuItem menuItem in menuCollisionEnergy.MenuItems)
                {
                    if (menuItem.Tag == null) continue;
                    string menuInstrument = ((string[])menuItem.Tag)[0];
                    string menuMode = ((string[])menuItem.Tag)[1];
                    if (menuInstrument == lipidCreator.selectedInstrumentForCE && menuMode == lipidCreator.monitoringType.ToString())
                    {    
                        if (menuMode == MonitoringTypes.PRM.ToString())
                        {
                            changeInstrumentForCEtypePRM(menuItem, null);
                        }
                        else {
                            changeInstrumentForCEtypeSRM(menuItem, null);
                        }
                        unsetTheInstrument = false;
                        break;
                    }
                }
                if (unsetTheInstrument) unsetInstrument((Object)menuCollisionEnergyNone, null);
            }
        }
        
        
        public void resetAllLipids()
        {
            lipidTabList = new ArrayList(new Lipid[] {null,
                                                      new Glycerolipid(lipidCreator),
                                                      new Phospholipid(lipidCreator),
                                                      new Sphingolipid(lipidCreator),
                                                      new Cholesterol(lipidCreator),
                                                      new Mediator(lipidCreator)});
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
                int w = (lipidsGridview.Width - 160) / (numCols - 1) - 4;
                foreach (DataGridViewColumn col in lipidsGridview.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    col.Width = Math.Max(col.MinimumWidth, w);
                }
                lipidsGridview.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                editColumn.Width = 40;
                deleteColumn.Width = 40;
                initialCall = false;
            }
        }
        
        
        
        
        
        public void changeTab(int index)
        {
            tabControl.SelectedIndex = index;
            if (lipidTabList.Count <= index) return;
            changingTabForced = true;
            currentLipid = (Lipid)lipidTabList[index];
            currentIndex = (LipidCategory)index;
            changeTabElements(index);
            tabControl.Refresh();
            changingTabForced = false;
        }
        
        
        
        
        
        
        public void tabIndexChanged(Object sender, TabControlCancelEventArgs e)
        {
            if (changingTabForced) return;
            changeTab(((TabControl)sender).SelectedIndex);
        }
        
        
        
        
        
        
        public void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
        
            Color color = (e.Index == (int)currentIndex) ? Color.White : TabControl.DefaultBackColor;
            Brush brush = (e.Index == (int)currentIndex) ? Brushes.Black :  Brushes.Black;
            using (Brush br = new SolidBrush (color))
            {
                Rectangle rect = e.Bounds;
                rect.X += 4;
                rect.Y += 3;
                rect.Width -= 8;
                e.Graphics.FillRectangle(br, rect);
                
                SizeF sz = e.Graphics.MeasureString(tabControl.TabPages[e.Index].Text, e.Font);
                e.Graphics.DrawString(tabControl.TabPages[e.Index].Text, e.Font, brush, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1);
            }
        }
        
        
        
        
        
        
        public void enableDisableAdducts(int index)
        {
            glPosAdductCheckbox1.Enabled = false;
            glPosAdductCheckbox2.Enabled = false;
            glPosAdductCheckbox3.Enabled = false;
            glNegAdductCheckbox1.Enabled = false;
            glNegAdductCheckbox2.Enabled = false;
            glNegAdductCheckbox3.Enabled = false;
            glNegAdductCheckbox4.Enabled = false;
        
            plPosAdductCheckbox1.Enabled = false;
            plPosAdductCheckbox2.Enabled = false;
            plPosAdductCheckbox3.Enabled = false;
            plNegAdductCheckbox1.Enabled = false;
            plNegAdductCheckbox2.Enabled = false;
            plNegAdductCheckbox3.Enabled = false;
            plNegAdductCheckbox4.Enabled = false;
            
            slPosAdductCheckbox1.Enabled = false;
            slPosAdductCheckbox2.Enabled = false;
            slPosAdductCheckbox3.Enabled = false;
            slNegAdductCheckbox1.Enabled = false;
            slNegAdductCheckbox2.Enabled = false;
            slNegAdductCheckbox3.Enabled = false;
            slNegAdductCheckbox4.Enabled = false;
            
            medNegAdductCheckbox1.Enabled = false;
            medNegAdductCheckbox2.Enabled = false;
            medNegAdductCheckbox3.Enabled = false;
            medNegAdductCheckbox4.Enabled = false;
            
            chPosAdductCheckbox1.Enabled = false;
            chPosAdductCheckbox2.Enabled = false;
            chPosAdductCheckbox3.Enabled = false;
            chNegAdductCheckbox1.Enabled = false;
            chNegAdductCheckbox2.Enabled = false;
            chNegAdductCheckbox3.Enabled = false;
            chNegAdductCheckbox4.Enabled = false;
        
            // enable all adduct checkboxes
            foreach (KeyValuePair<string, Precursor> row in lipidCreator.headgroups)
            {
                int category = (int)row.Value.category;
                if (category != index) continue;
                switch (category)
                {
                    case (int)LipidCategory.Glycerolipid:
                        if (((Glycerolipid)currentLipid).containsSugar ^ row.Value.attributes.Contains("sugar")) break;
                        
                        glPosAdductCheckbox1.Enabled |= row.Value.adductRestrictions["+H"];
                        glPosAdductCheckbox2.Enabled |= row.Value.adductRestrictions["+2H"];
                        glPosAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+NH4"];
                        glNegAdductCheckbox1.Enabled |= row.Value.adductRestrictions["-H"];
                        glNegAdductCheckbox2.Enabled |= row.Value.adductRestrictions["-2H"];
                        glNegAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+HCOO"];
                        glNegAdductCheckbox4.Enabled |= row.Value.adductRestrictions["+CH3COO"];
                        break;
                    
                    case (int)LipidCategory.Glycerophospholipid:
                        if (((Phospholipid)currentLipid).isLyso ^ row.Value.attributes.Contains("lyso")) break;
                        if (((Phospholipid)currentLipid).isCL ^ row.Value.attributes.Contains("cardio")) break;
                    
                        plPosAdductCheckbox1.Enabled |= row.Value.adductRestrictions["+H"];
                        plPosAdductCheckbox2.Enabled |= row.Value.adductRestrictions["+2H"];
                        plPosAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+NH4"];
                        plNegAdductCheckbox1.Enabled |= row.Value.adductRestrictions["-H"];
                        plNegAdductCheckbox2.Enabled |= row.Value.adductRestrictions["-2H"];
                        plNegAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+HCOO"];
                        plNegAdductCheckbox4.Enabled |= row.Value.adductRestrictions["+CH3COO"];
                        break;
                        
                    case (int)LipidCategory.Sphingolipid:
                        if (((Sphingolipid)currentLipid).isLyso ^ row.Value.attributes.Contains("lyso")) break;
                        
                        slPosAdductCheckbox1.Enabled |= row.Value.adductRestrictions["+H"];
                        slPosAdductCheckbox2.Enabled |= row.Value.adductRestrictions["+2H"];
                        slPosAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+NH4"];
                        slNegAdductCheckbox1.Enabled |= row.Value.adductRestrictions["-H"];
                        slNegAdductCheckbox2.Enabled |= row.Value.adductRestrictions["-2H"];
                        slNegAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+HCOO"];
                        slNegAdductCheckbox4.Enabled |= row.Value.adductRestrictions["+CH3COO"];
                        break;
                        
                    case (int)LipidCategory.LipidMediator:
                        medNegAdductCheckbox1.Enabled |= row.Value.adductRestrictions["-H"];
                        medNegAdductCheckbox2.Enabled |= row.Value.adductRestrictions["-2H"];
                        medNegAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+HCOO"];
                        medNegAdductCheckbox4.Enabled |= row.Value.adductRestrictions["+CH3COO"];
                        break;
                        
                    case (int)LipidCategory.Sterollipid:
                        chPosAdductCheckbox1.Enabled |= row.Value.adductRestrictions["+H"];
                        chPosAdductCheckbox2.Enabled |= row.Value.adductRestrictions["+2H"];
                        chPosAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+NH4"];
                        chNegAdductCheckbox1.Enabled |= row.Value.adductRestrictions["-H"];
                        chNegAdductCheckbox2.Enabled |= row.Value.adductRestrictions["-2H"];
                        chNegAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+HCOO"];
                        chNegAdductCheckbox4.Enabled |= row.Value.adductRestrictions["+CH3COO"];
                        break;
                }
                
            }
        }

        
        
        
        public void changeTabElements(int index)
        {
            enableDisableAdducts(index);
        
            extendWindow(((LipidCategory)index == LipidCategory.Glycerophospholipid) && ((Phospholipid)currentLipid).isCL);
            
            switch((LipidCategory)index)
            {
                case LipidCategory.Glycerolipid:
                    Glycerolipid currentGlycerolipid = (Glycerolipid)currentLipid;
                    settingListbox = true;
                    for (int i = 0; i < glHgListbox.Items.Count; ++i)
                    {
                        glHgListbox.SetSelected(i, false);
                    }
                    foreach (string headgroup in currentGlycerolipid.headGroupNames)
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
                    
                    
                    glFA1Textbox.Text = currentGlycerolipid.fag1.lengthInfo;
                    glDB1Textbox.Text = currentGlycerolipid.fag1.dbInfo;
                    glHydroxyl1Textbox.Text = currentGlycerolipid.fag1.hydroxylInfo;
                    glFA1Combobox.SelectedIndex = currentGlycerolipid.fag1.chainType;
                    glFA1Checkbox1.Checked = currentGlycerolipid.fag1.faTypes["FA"];
                    glFA1Checkbox2.Checked = currentGlycerolipid.fag1.faTypes["FAp"];
                    glFA1Checkbox3.Checked = currentGlycerolipid.fag1.faTypes["FAa"];
                    
                    glFA2Textbox.Text = currentGlycerolipid.fag2.lengthInfo;
                    glDB2Textbox.Text = currentGlycerolipid.fag2.dbInfo;
                    glHydroxyl2Textbox.Text = currentGlycerolipid.fag2.hydroxylInfo;
                    glFA2Combobox.SelectedIndex = currentGlycerolipid.fag2.chainType;
                    glFA2Checkbox1.Checked = currentGlycerolipid.fag2.faTypes["FA"];
                    glFA2Checkbox2.Checked = currentGlycerolipid.fag2.faTypes["FAp"];
                    glFA2Checkbox3.Checked = currentGlycerolipid.fag2.faTypes["FAa"];
                    
                    glFA3Textbox.Text = currentGlycerolipid.fag3.lengthInfo;
                    glDB3Textbox.Text = currentGlycerolipid.fag3.dbInfo;
                    glHydroxyl3Textbox.Text = currentGlycerolipid.fag3.hydroxylInfo;
                    glFA3Combobox.SelectedIndex = currentGlycerolipid.fag3.chainType;
                    glFA3Checkbox1.Checked = currentGlycerolipid.fag3.faTypes["FA"];
                    glFA3Checkbox2.Checked = currentGlycerolipid.fag3.faTypes["FAp"];
                    glFA3Checkbox3.Checked = currentGlycerolipid.fag3.faTypes["FAa"];
                    
                    glPosAdductCheckbox1.Checked = currentGlycerolipid.adducts["+H"];
                    glPosAdductCheckbox2.Checked = currentGlycerolipid.adducts["+2H"];
                    glPosAdductCheckbox3.Checked = currentGlycerolipid.adducts["+NH4"];
                    glNegAdductCheckbox1.Checked = currentGlycerolipid.adducts["-H"];
                    glNegAdductCheckbox2.Checked = currentGlycerolipid.adducts["-2H"];
                    glNegAdductCheckbox3.Checked = currentGlycerolipid.adducts["+HCOO"];
                    glNegAdductCheckbox4.Checked = currentGlycerolipid.adducts["+CH3COO"];
                    addLipidButton.Text = "Add glycerolipids";
                    
                    glContainsSugar.Checked = currentGlycerolipid.containsSugar;
                    
                    
                    updateRanges(currentGlycerolipid.fag1, glFA1Textbox, glFA1Combobox.SelectedIndex);
                    updateRanges(currentGlycerolipid.fag1, glDB1Textbox, 3);
                    updateRanges(currentGlycerolipid.fag1, glHydroxyl1Textbox, 4);
                    updateRanges(currentGlycerolipid.fag2, glFA2Textbox, glFA2Combobox.SelectedIndex);
                    updateRanges(currentGlycerolipid.fag2, glDB2Textbox, 3);
                    updateRanges(currentGlycerolipid.fag2, glHydroxyl2Textbox, 4);
                    updateRanges(currentGlycerolipid.fag3, glFA3Textbox, glFA3Combobox.SelectedIndex);
                    updateRanges(currentGlycerolipid.fag3, glDB3Textbox, 3);
                    updateRanges(currentGlycerolipid.fag3, glHydroxyl3Textbox, 4);
                    
                    glRepresentativeFA.Checked = currentGlycerolipid.representativeFA;
                    glPictureBox.SendToBack();
                    break;
                    
                case LipidCategory.Glycerophospholipid:
                    Phospholipid currentPhospholipid = (Phospholipid)currentLipid;
                    
                    if (currentPhospholipid.isCL) plIsCL.Checked = true;
                    else if (currentPhospholipid.isLyso) plIsLyso.Checked = true;
                    else plRegular.Checked = true;
                    
                    // unset lyso
                    plFA2Combobox.Visible = true;
                    plFA2Textbox.Visible = true;
                    plDB2Textbox.Visible = true;
                    plHydroxyl2Textbox.Visible = true;
                    plDB2Label.Visible = true;
                    plHydroxyl2Label.Visible = true;
                    
                    
                    
                    if (currentPhospholipid.isCL) // Cardiolipin
                    {
                        plHGLabel.Visible = false;
                        plHgListbox.Visible = false;
                        
                        clFA3Checkbox3.Visible = true;
                        clFA3Checkbox2.Visible = true;
                        clFA3Checkbox1.Visible = true;
                        clFA4Checkbox3.Visible = true;
                        clFA4Checkbox2.Visible = true;
                        clFA4Checkbox1.Visible = true;
                        clFA3Textbox.Visible = true;
                        clFA4Textbox.Visible = true;
                        clDB3Textbox.Visible = true;
                        clDB4Textbox.Visible = true;
                        clHydroxyl3Textbox.Visible = true;
                        clHydroxyl4Textbox.Visible = true;
                        clHydroxyl3Label.Visible = true;
                        clHydroxyl4Label.Visible = true;
                        clFA3Combobox.Visible = true;
                        clFA4Combobox.Visible = true;
                        clDB3Label.Visible = true;
                        clDB4Label.Visible = true;
                        plFA2Checkbox3.Visible = true;
                        plFA2Checkbox2.Visible = true;
                        plFA2Checkbox1.Visible = true;
                        
                        
                        plFA1Textbox.Text = currentPhospholipid.fag1.lengthInfo;
                        plDB1Textbox.Text = currentPhospholipid.fag1.dbInfo;
                        plHydroxyl1Textbox.Text = currentPhospholipid.fag1.hydroxylInfo;
                        plFA1Combobox.SelectedIndex = currentPhospholipid.fag1.chainType;
                        plFA1Checkbox1.Checked = currentPhospholipid.fag1.faTypes["FA"];
                        plFA1Checkbox2.Checked = currentPhospholipid.fag1.faTypes["FAp"];
                        plFA1Checkbox3.Checked = currentPhospholipid.fag1.faTypes["FAa"];
                        
                        plFA2Textbox.Text = currentPhospholipid.fag2.lengthInfo;
                        plDB2Textbox.Text = currentPhospholipid.fag2.dbInfo;
                        plHydroxyl2Textbox.Text = currentPhospholipid.fag2.hydroxylInfo;
                        plFA2Combobox.SelectedIndex = currentPhospholipid.fag2.chainType;
                        plFA2Checkbox1.Checked = currentPhospholipid.fag2.faTypes["FA"];
                        plFA2Checkbox2.Checked = currentPhospholipid.fag2.faTypes["FAp"];
                        plFA2Checkbox3.Checked = currentPhospholipid.fag2.faTypes["FAa"];
                        
                        clFA3Textbox.Text = currentPhospholipid.fag3.lengthInfo;
                        clDB3Textbox.Text = currentPhospholipid.fag3.dbInfo;
                        clHydroxyl3Textbox.Text = currentPhospholipid.fag3.hydroxylInfo;
                        clFA3Combobox.SelectedIndex = currentPhospholipid.fag3.chainType;
                        clFA3Checkbox1.Checked = currentPhospholipid.fag3.faTypes["FA"];
                        clFA3Checkbox2.Checked = currentPhospholipid.fag3.faTypes["FAp"];
                        clFA3Checkbox3.Checked = currentPhospholipid.fag3.faTypes["FAa"];
                        
                        clFA4Textbox.Text = currentPhospholipid.fag4.lengthInfo;
                        clDB4Textbox.Text = currentPhospholipid.fag4.dbInfo;
                        clHydroxyl4Textbox.Text = currentPhospholipid.fag4.hydroxylInfo;
                        clFA4Combobox.SelectedIndex = currentPhospholipid.fag4.chainType;
                        clFA4Checkbox1.Checked = currentPhospholipid.fag4.faTypes["FA"];
                        clFA4Checkbox2.Checked = currentPhospholipid.fag4.faTypes["FAp"];
                        clFA4Checkbox3.Checked = currentPhospholipid.fag4.faTypes["FAa"];
                        
                        
                        plPosAdductCheckbox1.Checked = currentPhospholipid.adducts["+H"];
                        plPosAdductCheckbox2.Checked = currentPhospholipid.adducts["+2H"];
                        plPosAdductCheckbox3.Checked = currentPhospholipid.adducts["+NH4"];
                        plNegAdductCheckbox1.Checked = currentPhospholipid.adducts["-H"];
                        plNegAdductCheckbox2.Checked = currentPhospholipid.adducts["-2H"];
                        plNegAdductCheckbox3.Checked = currentPhospholipid.adducts["+HCOO"];
                        plNegAdductCheckbox4.Checked = currentPhospholipid.adducts["+CH3COO"];
                        addLipidButton.Text = "Add cardiolipins";
                        
                        plIsCL.Checked = true;
                        
                        updateRanges(currentPhospholipid.fag1, plFA1Textbox, plFA1Combobox.SelectedIndex);
                        updateRanges(currentPhospholipid.fag1, plDB1Textbox, 3);
                        updateRanges(currentPhospholipid.fag1, plHydroxyl1Textbox, 4);
                        updateRanges(currentPhospholipid.fag2, plFA2Textbox, plFA2Combobox.SelectedIndex);
                        updateRanges(currentPhospholipid.fag2, plDB2Textbox, 3);
                        updateRanges(currentPhospholipid.fag2, plHydroxyl2Textbox, 4);
                        updateRanges(currentPhospholipid.fag3, clFA3Textbox, clFA3Combobox.SelectedIndex);
                        updateRanges(currentPhospholipid.fag3, clDB3Textbox, 3);
                        updateRanges(currentPhospholipid.fag3, clHydroxyl3Textbox, 4);
                        updateRanges(currentPhospholipid.fag4, clFA4Textbox, clFA4Combobox.SelectedIndex);
                        updateRanges(currentPhospholipid.fag4, clDB4Textbox, 3);
                        updateRanges(currentPhospholipid.fag4, clHydroxyl4Textbox, 4);
                        
                        plRepresentativeFA.Checked = currentPhospholipid.representativeFA;
                        plPictureBox.Image = cardioBackboneImage;
                        plPictureBox.Location = new Point(5, 20);
                        plPictureBox.SendToBack();
                    }
                    
                    else // Phospholipid
                    {
                        clFA3Checkbox3.Visible = false;
                        clFA3Checkbox2.Visible = false;
                        clFA3Checkbox1.Visible = false;
                        clFA4Checkbox3.Visible = false;
                        clFA4Checkbox2.Visible = false;
                        clFA4Checkbox1.Visible = false;
                        clFA3Textbox.Visible = false;
                        clFA4Textbox.Visible = false;
                        clDB3Textbox.Visible = false;
                        clDB4Textbox.Visible = false;
                        clHydroxyl3Textbox.Visible = false;
                        clHydroxyl4Textbox.Visible = false;
                        clHydroxyl3Label.Visible = false;
                        clHydroxyl4Label.Visible = false;
                        clFA3Combobox.Visible = false;
                        clFA4Combobox.Visible = false;
                        clDB3Label.Visible = false;
                        clDB4Label.Visible = false;
                        
                        plFA2Checkbox2.Checked = false;
                        plFA2Checkbox3.Checked = false;
                        
                        plHgListbox.Visible = true;
                        plHGLabel.Visible = true;
                        plFA2Checkbox3.Visible = false;
                        plFA2Checkbox2.Visible = false;
                        plFA2Checkbox1.Visible = false;
                        settingListbox = true;
                        
                        plChangeLyso(currentPhospholipid.isLyso);
                        
                        
                        for (int i = 0; i < plHgListbox.Items.Count; ++i)
                        {
                            plHgListbox.SetSelected(i, false);
                        }
                        foreach (string headgroup in currentPhospholipid.headGroupNames)
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
                        
                        plFA1Textbox.Text = currentPhospholipid.fag1.lengthInfo;
                        plDB1Textbox.Text = currentPhospholipid.fag1.dbInfo;
                        plHydroxyl1Textbox.Text = currentPhospholipid.fag1.hydroxylInfo;
                        plFA1Combobox.SelectedIndex = currentPhospholipid.fag1.chainType;
                        plFA1Checkbox1.Checked = currentPhospholipid.fag1.faTypes["FA"];
                        plFA1Checkbox2.Checked = currentPhospholipid.fag1.faTypes["FAp"];
                        plFA1Checkbox3.Checked = currentPhospholipid.fag1.faTypes["FAa"];
                        
                        plFA2Textbox.Text = currentPhospholipid.fag2.lengthInfo;
                        plDB2Textbox.Text = currentPhospholipid.fag2.dbInfo;
                        plHydroxyl2Textbox.Text = currentPhospholipid.fag2.hydroxylInfo;
                        plFA2Combobox.SelectedIndex = currentPhospholipid.fag2.chainType;
                        plFA2Checkbox1.Checked = currentPhospholipid.fag2.faTypes["FA"];
                        plFA2Checkbox2.Checked = currentPhospholipid.fag2.faTypes["FAp"];
                        plFA2Checkbox3.Checked = currentPhospholipid.fag2.faTypes["FAa"];
                    
                        
                        
                        plPosAdductCheckbox1.Checked = currentPhospholipid.adducts["+H"];
                        plPosAdductCheckbox2.Checked = currentPhospholipid.adducts["+2H"];
                        plPosAdductCheckbox3.Checked = currentPhospholipid.adducts["+NH4"];
                        plNegAdductCheckbox1.Checked = currentPhospholipid.adducts["-H"];
                        plNegAdductCheckbox2.Checked = currentPhospholipid.adducts["-2H"];
                        plNegAdductCheckbox3.Checked = currentPhospholipid.adducts["+HCOO"];
                        plNegAdductCheckbox4.Checked = currentPhospholipid.adducts["+CH3COO"];
                        addLipidButton.Text = "Add phospholipids";
                        
                        
                        updateRanges(currentPhospholipid.fag1, plFA1Textbox, plFA1Combobox.SelectedIndex);
                        updateRanges(currentPhospholipid.fag1, plDB1Textbox, 3);
                        updateRanges(currentPhospholipid.fag1, plHydroxyl1Textbox, 4);
                        if (!currentPhospholipid.isLyso)
                        {
                            updateRanges(currentPhospholipid.fag2, plFA2Textbox, plFA2Combobox.SelectedIndex);
                            updateRanges(currentPhospholipid.fag2, plDB2Textbox, 3);
                            updateRanges(currentPhospholipid.fag2, plHydroxyl2Textbox, 4);
                        }
                        plRepresentativeFA.Checked = currentPhospholipid.representativeFA;
                    }
                    break;
                    
                case LipidCategory.Sphingolipid:
                    Sphingolipid currentSphingolipid = (Sphingolipid)currentLipid;
                    
                    slIsLyso.Checked = currentSphingolipid.isLyso;
                    slRegular.Checked = !currentSphingolipid.isLyso;
                    slChangeLyso(currentSphingolipid.isLyso);
                    
                    settingListbox = true;
                    for (int i = 0; i < slHgListbox.Items.Count; ++i)
                    {
                        slHgListbox.SetSelected(i, false);
                    }
                    foreach (string headgroup in currentSphingolipid.headGroupNames)
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
                    
                    
                    slLCBTextbox.Text = currentSphingolipid.lcb.lengthInfo;
                    slDB2Textbox.Text = currentSphingolipid.lcb.dbInfo;
                    slLCBCombobox.SelectedIndex = currentSphingolipid.lcb.chainType;
                    slLCBHydroxyCombobox.SelectedIndex = currentSphingolipid.lcb.hydroxylCounts.First() - 2;
                    if (!currentSphingolipid.isLyso) slFAHydroxyCombobox.SelectedIndex = currentSphingolipid.fag.hydroxylCounts.First();
                    
                    slFATextbox.Text = currentSphingolipid.fag.lengthInfo;
                    slDB1Textbox.Text = currentSphingolipid.fag.dbInfo;
                    slFACombobox.SelectedIndex = currentSphingolipid.fag.chainType;
                    
                    slPosAdductCheckbox1.Checked = currentSphingolipid.adducts["+H"];
                    slPosAdductCheckbox2.Checked = currentSphingolipid.adducts["+2H"];
                    slPosAdductCheckbox3.Checked = currentSphingolipid.adducts["+NH4"];
                    slNegAdductCheckbox1.Checked = currentSphingolipid.adducts["-H"];
                    slNegAdductCheckbox2.Checked = currentSphingolipid.adducts["-2H"];
                    slNegAdductCheckbox3.Checked = currentSphingolipid.adducts["+HCOO"];
                    slNegAdductCheckbox4.Checked = currentSphingolipid.adducts["+CH3COO"];
                    addLipidButton.Text = "Add sphingolipids";
                    
                    updateRanges(currentSphingolipid.lcb, slLCBTextbox, slLCBCombobox.SelectedIndex, true);
                    updateRanges(currentSphingolipid.lcb, slDB2Textbox, 3);
                    updateRanges(currentSphingolipid.fag, slFATextbox, slFACombobox.SelectedIndex);
                    updateRanges(currentSphingolipid.fag, slDB1Textbox, 3);
                    slPictureBox.SendToBack();
                    break;
                    
                    
                    
                case LipidCategory.Sterollipid:
                    Cholesterol currentCHLipid = (Cholesterol)currentLipid;
                    chPosAdductCheckbox1.Checked = currentCHLipid.adducts["+H"];
                    chPosAdductCheckbox2.Checked = currentCHLipid.adducts["+2H"];
                    chPosAdductCheckbox3.Checked = currentCHLipid.adducts["+NH4"];
                    chNegAdductCheckbox1.Checked = currentCHLipid.adducts["-H"];
                    chNegAdductCheckbox2.Checked = currentCHLipid.adducts["-2H"];
                    chNegAdductCheckbox3.Checked = currentCHLipid.adducts["+HCOO"];
                    chNegAdductCheckbox4.Checked = currentCHLipid.adducts["+CH3COO"];
                    addLipidButton.Text = "Add cholesterols";
                    chContainsEster.Checked = currentCHLipid.containsEster;
                    
                    chFATextbox.Text = currentCHLipid.fag.lengthInfo;
                    chDBTextbox.Text = currentCHLipid.fag.dbInfo;
                    chHydroxylTextbox.Text = currentCHLipid.fag.hydroxylInfo;
                    chFACombobox.SelectedIndex = currentCHLipid.fag.chainType;
                    updateRanges(currentCHLipid.fag, chFATextbox, chFACombobox.SelectedIndex);
                    updateRanges(currentCHLipid.fag, chDBTextbox, 3);
                    updateRanges(currentCHLipid.fag, chHydroxylTextbox, 4);
                    break;
                    
                case LipidCategory.LipidMediator:
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
                    addLipidButton.Text = "Add mediators";
                    break;
                    
                default:
                    break;
            }
            
            menuResetCategory.Enabled = currentIndex != LipidCategory.NoLipid;
            menuMS2Fragments.Enabled = currentIndex != LipidCategory.NoLipid;
            menuIsotopes.Enabled = currentIndex != LipidCategory.NoLipid;
            
            if (currentLipid != null)
            {
                ((TabPage)tabList[index]).Controls.Add(lcStep2);
                ((TabPage)tabList[index]).Controls.Add(lcStep3);                
                modifyLipidButton.Enabled = lipidModifications[(int)currentIndex] > -1;
            }
        }
        
        
        
        
        
        
        public void resetLipid(Object sender, EventArgs e)
        {
        
            if (currentIndex == LipidCategory.NoLipid) return;
            Lipid newLipid = null;
            int index = (int)currentIndex;
            switch (index)
            {
                case (int)LipidCategory.Glycerolipid:
                    newLipid = new Glycerolipid(lipidCreator);
                    break;
                    
                case (int)LipidCategory.Glycerophospholipid:
                    newLipid = new Phospholipid(lipidCreator);
                    ((Phospholipid)newLipid).isCL = plIsCL.Checked;
                    break;
                    
                case (int)LipidCategory.Sphingolipid:
                    newLipid = new Sphingolipid(lipidCreator);
                    break;
                    
                case (int)LipidCategory.Sterollipid:
                    newLipid = new Cholesterol(lipidCreator);
                    break;
                    
                case (int)LipidCategory.LipidMediator:
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
        
        
        public void clFA3ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag3.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((Phospholipid)currentLipid).fag3, clFA3Textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        
        
        
        
        
        public void clFA4ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag4.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((Phospholipid)currentLipid).fag4, clFA4Textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        
        
        
        
        public void updateRanges(FattyAcidGroup fag, TextBox tb, int objectType)
        {
            updateRanges(fag, tb, objectType, false);
        }
        
        
        
        
        
        // objectType (Object type): 0 = carbon length, 1 = carbon length odd, 2 = carbon length even, 3 = db length, 4 = hydroxyl length
        public void updateRanges(FattyAcidGroup fag, TextBox tb, int objectType, bool isLCB)
        {
            int minRange = 0, maxRange = 0;
            if (objectType <= 2)
            {
                minRange = LipidCreator.MIN_CARBON_LENGTH;
                maxRange = LipidCreator.MAX_CARBON_LENGTH;
            }
            else if (objectType == 3)
            {
                minRange = LipidCreator.MIN_DB_LENGTH;
                maxRange = LipidCreator.MAX_DB_LENGTH;
            }
            else if (objectType == 4)
            {
                minRange = LipidCreator.MIN_HYDROXY_LENGTH;
                maxRange = LipidCreator.MAX_HYDROXY_LENGTH;
            }
            
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
        
        
        
        private void homeText3LinkClicked(Object sender, EventArgs e)
        {
            string url = "http://www.google.de";
            var si = new ProcessStartInfo(url);
            Process.Start(si);
        }
        
        
        
        
        public void clFA3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag3.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((Phospholipid)currentLipid).fag3, (TextBox)sender, clFA3Combobox.SelectedIndex);
        }
        
        
        
        
        
        
        public void clFA4TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag4.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((Phospholipid)currentLipid).fag4, (TextBox)sender, clFA4Combobox.SelectedIndex);
        }
        
        
        
        
        
        
        public void clDB3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag3.dbInfo = ((TextBox)sender).Text;
            updateRanges(((Phospholipid)currentLipid).fag3, (TextBox)sender, 3);
        }
        
        
        
        
        
        public void clDB4TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag4.dbInfo = ((TextBox)sender).Text;
            updateRanges(((Phospholipid)currentLipid).fag4, (TextBox)sender, 3);
        }
        
        
        
        
        
        
        public void clHydroxyl3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag3.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((Phospholipid)currentLipid).fag3, (TextBox)sender, 4);
        }
        
        
        
        
        
        public void clHydroxyl4TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag4.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((Phospholipid)currentLipid).fag4, (TextBox)sender, 4);
        }
        
        
        
        
        
        public void clFA3Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag3.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((Phospholipid)currentLipid).fag3.faTypes["FAx"] = !((Phospholipid)currentLipid).fag3.anyFAChecked();
        }
        
        
        
        
        
        public void clFA3Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag3.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((Phospholipid)currentLipid).fag3.faTypes["FAx"] = !((Phospholipid)currentLipid).fag3.anyFAChecked();
        }
        
        
        
        
        
        public void clFA3Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag3.faTypes["FAa"] = ((CheckBox)sender).Checked;
            ((Phospholipid)currentLipid).fag3.faTypes["FAx"] = !((Phospholipid)currentLipid).fag3.anyFAChecked();
        }
        
        
        
        
        
        public void clFA4Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag4.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((Phospholipid)currentLipid).fag4.faTypes["FAx"] = !((Phospholipid)currentLipid).fag4.anyFAChecked();
        }
        
        
        
        
        
        public void clFA4Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag4.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((Phospholipid)currentLipid).fag4.faTypes["FAx"] = !((Phospholipid)currentLipid).fag4.anyFAChecked();
        }
        
        
        
        
        
        public void clFA4Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag4.faTypes["FAa"] = ((CheckBox)sender).Checked;
            ((Phospholipid)currentLipid).fag4.faTypes["FAx"] = !((Phospholipid)currentLipid).fag4.anyFAChecked();
        }
        
        
        
        
        
        
        void clFA3Checkbox3MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = cardioBackboneImage;
            plPictureBox.SendToBack();
        }
        
        
        
        
        
        private void clFA3Checkbox3MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = cardioBackboneImageFA3e;
            plPictureBox.SendToBack();
        }
        
        
        
        
        
        void clFA3Checkbox2MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = cardioBackboneImage;
            plPictureBox.SendToBack();
        }
        
        
        
        
        
        private void clFA3Checkbox2MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = cardioBackboneImageFA3p;
            plPictureBox.SendToBack();
        }
        
        
        
        
        
        void clFA4Checkbox3MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = cardioBackboneImage;
            plPictureBox.SendToBack();
        }
        
        
        
        
        
        private void clFA4Checkbox3MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = cardioBackboneImageFA4e;
            plPictureBox.SendToBack();
        }
        
        
        
        
        
        void clFA4Checkbox2MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = cardioBackboneImage;
            plPictureBox.SendToBack();
        }
        
        
        
        

        private void clFA4Checkbox2MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = cardioBackboneImageFA4p;
            plPictureBox.SendToBack();
        }
        
        
        
        
        
        public void clRepresentativeFACheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).representativeFA = ((CheckBox)sender).Checked;
            if (((Phospholipid)currentLipid).representativeFA)
            {
                plFA2Textbox.Enabled = false;
                plDB2Textbox.Enabled = false;
                plHydroxyl2Textbox.Enabled = false;
                plFA2Combobox.Enabled = false;
                plFA2Checkbox1.Enabled = false;
                plFA2Checkbox2.Enabled = false;
                plFA2Checkbox3.Enabled = false;
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
                
                plFA2Textbox.Text = plFA1Textbox.Text;
                clFA3Textbox.Text = plFA1Textbox.Text;
                clFA4Textbox.Text = plFA1Textbox.Text;
                plDB2Textbox.Text = plDB1Textbox.Text;
                clDB3Textbox.Text = plDB1Textbox.Text;
                clDB4Textbox.Text = plDB1Textbox.Text;
                plHydroxyl2Textbox.Text = plHydroxyl1Textbox.Text;
                clHydroxyl3Textbox.Text = plHydroxyl1Textbox.Text;
                clHydroxyl4Textbox.Text = plHydroxyl1Textbox.Text;
                plFA2Combobox.Text = plFA1Combobox.Text;
                clFA3Combobox.Text = plFA1Combobox.Text;
                clFA4Combobox.Text = plFA1Combobox.Text;
                plFA2Checkbox1.Checked = plFA1Checkbox1.Checked;
                clFA3Checkbox1.Checked = plFA1Checkbox1.Checked;
                clFA4Checkbox1.Checked = plFA1Checkbox1.Checked;
                plFA2Checkbox2.Checked = plFA1Checkbox2.Checked;
                clFA3Checkbox2.Checked = plFA1Checkbox2.Checked;
                clFA4Checkbox2.Checked = plFA1Checkbox2.Checked;
                plFA2Checkbox3.Checked = plFA1Checkbox3.Checked;
                clFA3Checkbox3.Checked = plFA1Checkbox3.Checked;
                clFA4Checkbox3.Checked = plFA1Checkbox3.Checked;
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
            updateRanges(((Phospholipid)currentLipid).fag2, plFA2Textbox, plFA2Combobox.SelectedIndex);
            updateRanges(((Phospholipid)currentLipid).fag3, clFA3Textbox, clFA3Combobox.SelectedIndex);
            updateRanges(((Phospholipid)currentLipid).fag4, clFA4Textbox, clFA4Combobox.SelectedIndex);
            updateRanges(((Phospholipid)currentLipid).fag2, plDB2Textbox, 3);
            updateRanges(((Phospholipid)currentLipid).fag3, clDB3Textbox, 3);
            updateRanges(((Phospholipid)currentLipid).fag4, clDB4Textbox, 3);
            updateRanges(((Phospholipid)currentLipid).fag2, plHydroxyl2Textbox, 4);
            updateRanges(((Phospholipid)currentLipid).fag3, clHydroxyl3Textbox, 4);
            updateRanges(((Phospholipid)currentLipid).fag4, clHydroxyl4Textbox, 4);
        }
        
        
        
        
        
        ////////////////////// GL ////////////////////////////////
        
        public void glFA1ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag1.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((Glycerolipid)currentLipid).fag1, glFA1Textbox, ((ComboBox)sender).SelectedIndex);
            if (((Glycerolipid)currentLipid).representativeFA)
            {
                glFA2Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                glFA3Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
            }
        }
        
        
        
        
        
        public void glFA2ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((Glycerolipid)currentLipid).fag2, glFA2Textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        
        
        
        
        public void glFA3ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag3.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((Glycerolipid)currentLipid).fag3, glFA3Textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        
        
        
        
        public void glFA1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((Glycerolipid)currentLipid).fag1, (TextBox)sender, glFA1Combobox.SelectedIndex);
            if (((Glycerolipid)currentLipid).representativeFA)
            {
                glFA2Textbox.Text = ((TextBox)sender).Text;
                glFA3Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void glFA2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((Glycerolipid)currentLipid).fag2, (TextBox)sender, glFA2Combobox.SelectedIndex);
        }
        
        
        
        
        
        public void glFA3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag3.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((Glycerolipid)currentLipid).fag3, (TextBox)sender, glFA3Combobox.SelectedIndex);
        }
        
        
        
        
        public void glDB1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            updateRanges(((Glycerolipid)currentLipid).fag1, (TextBox)sender, 3);
            if (((Glycerolipid)currentLipid).representativeFA)
            {
                glDB2Textbox.Text = ((TextBox)sender).Text;
                glDB3Textbox.Text = ((TextBox)sender).Text;
            }
        }
        
        
        
        
        public void glDB2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            updateRanges(((Glycerolipid)currentLipid).fag2, (TextBox)sender, 3);
        }
        
        
        
        
        public void glDB3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag3.dbInfo = ((TextBox)sender).Text;
            updateRanges(((Glycerolipid)currentLipid).fag3, (TextBox)sender, 3);
        }
        
        
        
        
        
        public void glPosAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        
        public void glPosAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void glPosAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void glNegAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void glNegAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void glNegAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void glNegAdductCheckbox4CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void glFA1Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag1.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((Glycerolipid)currentLipid).fag1.faTypes["FAx"] = !((Glycerolipid)currentLipid).fag1.anyFAChecked();
            if (((Glycerolipid)currentLipid).representativeFA)
            {
                if (((Glycerolipid)currentLipid).fag2.anyFAChecked()) glFA2Checkbox1.Checked = ((CheckBox)sender).Checked;
                if (((Glycerolipid)currentLipid).fag3.anyFAChecked()) glFA3Checkbox1.Checked =  ((CheckBox)sender).Checked;
            }
        }
        
        
        
        
        public void glFA1Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((Glycerolipid)currentLipid).fag1.faTypes["FAx"] = !((Glycerolipid)currentLipid).fag1.anyFAChecked();
            if (((Glycerolipid)currentLipid).representativeFA)
            {
                if (((Glycerolipid)currentLipid).fag2.anyFAChecked()) glFA2Checkbox2.Checked = ((CheckBox)sender).Checked;
                if (((Glycerolipid)currentLipid).fag3.anyFAChecked()) glFA3Checkbox2.Checked = ((CheckBox)sender).Checked;
            }
        }
        
        
        
        
        
        public void glFA1Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag1.faTypes["FAa"] = ((CheckBox)sender).Checked;
            ((Glycerolipid)currentLipid).fag1.faTypes["FAx"] = !((Glycerolipid)currentLipid).fag1.anyFAChecked();
            if (((Glycerolipid)currentLipid).representativeFA)
            {
                if (((Glycerolipid)currentLipid).fag2.anyFAChecked()) glFA2Checkbox3.Checked = ((CheckBox)sender).Checked;
                if (((Glycerolipid)currentLipid).fag3.anyFAChecked()) glFA3Checkbox3.Checked = ((CheckBox)sender).Checked;
            }
        }
        
        
        
        
        public void glFA2Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag2.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((Glycerolipid)currentLipid).fag2.faTypes["FAx"] = !((Glycerolipid)currentLipid).fag2.anyFAChecked();
        }
        
        
        
        
        
        public void glFA2Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag2.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((Glycerolipid)currentLipid).fag2.faTypes["FAx"] = !((Glycerolipid)currentLipid).fag2.anyFAChecked();
        }
        
        
        
        
        public void glFA2Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag2.faTypes["FAa"] = ((CheckBox)sender).Checked;
            ((Glycerolipid)currentLipid).fag2.faTypes["FAx"] = !((Glycerolipid)currentLipid).fag2.anyFAChecked();
        }
        
        
        
        
        
        public void glFA3Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag3.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((Glycerolipid)currentLipid).fag3.faTypes["FAx"] = !((Glycerolipid)currentLipid).fag3.anyFAChecked();
        }
        
        
        
        
        public void glFA3Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag3.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((Glycerolipid)currentLipid).fag3.faTypes["FAx"] = !((Glycerolipid)currentLipid).fag3.anyFAChecked();
        }
        
        
        
        
        public void glFA3Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag3.faTypes["FAa"] = ((CheckBox)sender).Checked;
            ((Glycerolipid)currentLipid).fag3.faTypes["FAx"] = !((Glycerolipid)currentLipid).fag3.anyFAChecked();
        }
        
        
        
        
        public void glHydroxyl1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag1.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((Glycerolipid)currentLipid).fag1, (TextBox)sender, 4);
            if (((Glycerolipid)currentLipid).representativeFA)
            {
                glHydroxyl2Textbox.Text = ((TextBox)sender).Text;
                glHydroxyl3Textbox.Text = ((TextBox)sender).Text;
            }
        }
        
        
        
        
        public void glHydroxyl2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag2.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((Glycerolipid)currentLipid).fag2, (TextBox)sender, 4);
        }
        
        
        
        
        public void glHydroxyl3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).fag3.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((Glycerolipid)currentLipid).fag3, (TextBox)sender, 4);
        }
        
        
        
        
        public void triggerEasteregg(Object sender, EventArgs e)
        {
            if (!((Phospholipid)currentLipid).isCL)
            {
                easterText.Left = Width + 20;
                easterEggMilliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                easterText.Visible = true;
                this.timerEasterEgg.Enabled = true;
                Enabled = false;
            }
        }
        
        
        
        private void timerEasterEggTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            easterText.Left -= (int)((milliseconds - easterEggMilliseconds) * 0.5);
            easterEggMilliseconds = milliseconds;
            if (easterText.Left < -easterText.Width){
                this.timerEasterEgg.Enabled = false;
                easterText.Visible = false;
                Enabled = true;
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
            ((Glycerolipid)currentLipid).containsSugar = ((CheckBox)sender).Checked;
            
            glPictureBox.Visible = false;
            if (((Glycerolipid)currentLipid).containsSugar)
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
            
            enableDisableAdducts((int)currentIndex);
        }
        
        
        
        
        public void glRepresentativeFACheckedChanged(Object sender, EventArgs e)
        {
            ((Glycerolipid)currentLipid).representativeFA = ((CheckBox)sender).Checked;
            if (((Glycerolipid)currentLipid).representativeFA)
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
                if (((Glycerolipid)currentLipid).fag2.anyFAChecked()) glFA2Checkbox1.Checked = glFA1Checkbox1.Checked;
                if (((Glycerolipid)currentLipid).fag3.anyFAChecked()) glFA3Checkbox1.Checked = glFA1Checkbox1.Checked;
                if (((Glycerolipid)currentLipid).fag2.anyFAChecked()) glFA2Checkbox2.Checked = glFA1Checkbox2.Checked;
                if (((Glycerolipid)currentLipid).fag3.anyFAChecked()) glFA3Checkbox2.Checked = glFA1Checkbox2.Checked;
                if (((Glycerolipid)currentLipid).fag2.anyFAChecked()) glFA2Checkbox3.Checked = glFA1Checkbox3.Checked;
                if (((Glycerolipid)currentLipid).fag3.anyFAChecked()) glFA3Checkbox3.Checked = glFA1Checkbox3.Checked;
                
                
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
            updateRanges(((Glycerolipid)currentLipid).fag2, glFA2Textbox, glFA2Combobox.SelectedIndex);
            updateRanges(((Glycerolipid)currentLipid).fag3, glFA3Textbox, glFA3Combobox.SelectedIndex);
            updateRanges(((Glycerolipid)currentLipid).fag2, glDB2Textbox, 3);
            updateRanges(((Glycerolipid)currentLipid).fag3, glDB3Textbox, 3);
            updateRanges(((Glycerolipid)currentLipid).fag2, glHydroxyl2Textbox, 4);
            updateRanges(((Glycerolipid)currentLipid).fag3, glHydroxyl3Textbox, 4);
        }
        
        
        
        ////////////////////// PL ////////////////////////////////
        
        
    
        public void plFA1ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag1.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((Phospholipid)currentLipid).fag1, plFA1Textbox, ((ComboBox)sender).SelectedIndex);
            if (((Phospholipid)currentLipid).representativeFA)
            {
                plFA2Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                if (plIsCL.Checked)
                {
                    clFA3Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                    clFA4Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                }
            }
        }
        
        
        
        
        public void plFA2ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((Phospholipid)currentLipid).fag2, plFA2Textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        
        
        
        public void plFA1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((Phospholipid)currentLipid).fag1, (TextBox)sender, plFA1Combobox.SelectedIndex);
            if (((Phospholipid)currentLipid).representativeFA)
            {
                plFA2Textbox.Text = ((TextBox)sender).Text;
                if (plIsCL.Checked)
                {
                    clFA3Textbox.Text = ((TextBox)sender).Text;
                    clFA4Textbox.Text = ((TextBox)sender).Text;
                }
            }
        }
        
        
        
        
        public void plFA2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((Phospholipid)currentLipid).fag2, (TextBox)sender, plFA2Combobox.SelectedIndex);
        }
        
        
        
        
        public void plDB1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            updateRanges(((Phospholipid)currentLipid).fag1, (TextBox)sender, 3);
            if (((Phospholipid)currentLipid).representativeFA)
            {
                plDB2Textbox.Text = ((TextBox)sender).Text;
                if (plIsCL.Checked)
                {
                    clDB3Textbox.Text = ((TextBox)sender).Text;
                    clDB4Textbox.Text = ((TextBox)sender).Text;
                }
            }
        }
        
        
        
        
        public void plDB2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            updateRanges(((Phospholipid)currentLipid).fag2, (TextBox)sender, 3);
        }
        
        
        
        
        public void plHydroxyl1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag1.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((Phospholipid)currentLipid).fag1, (TextBox)sender, 4);
            if (((Phospholipid)currentLipid).representativeFA)
            {
                plHydroxyl2Textbox.Text = ((TextBox)sender).Text;
                if (plIsCL.Checked)
                {
                    clHydroxyl3Textbox.Text = ((TextBox)sender).Text;
                    clHydroxyl4Textbox.Text = ((TextBox)sender).Text;
                }
            }
        }
        
        
        
        
        public void plHydroxyl2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag2.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(((Phospholipid)currentLipid).fag2, (TextBox)sender, 4);
        }
        
        
        
        
        public void plPosAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void plPosAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void plPosAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void plNegAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void plNegAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void plNegAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void plNegAdductCheckbox4CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void plFA1Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag1.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((Phospholipid)currentLipid).fag1.faTypes["FAx"] = !((Phospholipid)currentLipid).fag1.anyFAChecked();
            if (((Phospholipid)currentLipid).representativeFA)
            {
                plFA2Checkbox1.Checked = ((CheckBox)sender).Checked;
                if (plIsCL.Checked)
                {
                    clFA3Checkbox1.Checked = ((CheckBox)sender).Checked;
                    clFA4Checkbox1.Checked = ((CheckBox)sender).Checked;
                }
            }
        }
        
        
        
        
        public void plFA1Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((Phospholipid)currentLipid).fag1.faTypes["FAx"] = !((Phospholipid)currentLipid).fag1.anyFAChecked();
            if (((Phospholipid)currentLipid).representativeFA)
            {
                plFA2Checkbox2.Checked = ((CheckBox)sender).Checked;
                if (plIsCL.Checked)
                {
                    clFA3Checkbox2.Checked = ((CheckBox)sender).Checked;
                    clFA4Checkbox2.Checked = ((CheckBox)sender).Checked;
                }
            }
        }
        
        
        
        
        public void plFA1Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag1.faTypes["FAa"] = ((CheckBox)sender).Checked;
            ((Phospholipid)currentLipid).fag1.faTypes["FAx"] = !((Phospholipid)currentLipid).fag1.anyFAChecked();
            if (((Phospholipid)currentLipid).representativeFA)
            {
                plFA2Checkbox3.Checked = ((CheckBox)sender).Checked;
                if (plIsCL.Checked)
                {
                    clFA3Checkbox3.Checked = ((CheckBox)sender).Checked;
                    clFA4Checkbox3.Checked = ((CheckBox)sender).Checked;
                }
            }
        }
        
        
        
        
        public void plF2Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag2.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((Phospholipid)currentLipid).fag2.faTypes["FAx"] = !((Phospholipid)currentLipid).fag2.anyFAChecked();
        }
        
        
        
        
        
        public void plFA2Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag2.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((Phospholipid)currentLipid).fag2.faTypes["FAx"] = !((Phospholipid)currentLipid).fag2.anyFAChecked();
        }
        
        
        
        
        public void plFA2Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).fag2.faTypes["FAa"] = ((CheckBox)sender).Checked;
            ((Phospholipid)currentLipid).fag2.faTypes["FAx"] = !((Phospholipid)currentLipid).fag2.anyFAChecked();
        }
        
        
        
        
        public void plTypeCheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).isCL = plIsCL.Checked;
            ((Phospholipid)currentLipid).isLyso = plIsLyso.Checked;

            changeTab((int)LipidCategory.Glycerophospholipid);
        }
        
        
        
        void extendWindow(bool isCL)
        {
            easterText.Visible = false;
            if (isCL && !windowExtended)
            {
                windowExtended = true;
                minWindowHeight = minWindowHeightExtended;
            
                this.MinimumSize = new System.Drawing.Size(windowWidth, minWindowHeight);
                lipidsGroupbox.Height = minLipidGridHeight;
                this.Size = new System.Drawing.Size(windowWidth, minWindowHeight);
                plStep1.Height = step1HeightExtended;
                lcStep2.Top = plStep1.Height + 20;
                lcStep3.Top = plStep1.Height + 20;
            }
            else if (!isCL && windowExtended)
            {
                windowExtended = false;
                minWindowHeight = minWindowHeightDefault;
                this.MinimumSize = new System.Drawing.Size(windowWidth, minWindowHeight);
                this.Size = new System.Drawing.Size(windowWidth, minWindowHeight);
                lipidsGroupbox.Height = minLipidGridHeight;
                plStep1.Height = step1Height;
                lcStep2.Top = plStep1.Height + 20;
                lcStep3.Top = plStep1.Height + 20;
            }
            enableDisableAdducts((int)currentIndex);
        }
        
        
        
        
        void plFA1Checkbox3MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = plIsCL.Checked ? cardioBackboneImage : (plIsLyso.Checked ? phosphoLysoBackboneImage : phosphoBackboneImage);
            plPictureBox.SendToBack();
        }
        
        
        
        
        private void plFA1Checkbox3MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = plIsCL.Checked ? cardioBackboneImageFA1e : (plIsLyso.Checked ? phosphoLysoBackboneImageFA1e : phosphoBackboneImageFA1e);
            plPictureBox.SendToBack();
        }
        
        
        
        
        void plFA1Checkbox2MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = plIsCL.Checked ? cardioBackboneImage : (plIsLyso.Checked ? phosphoLysoBackboneImage : phosphoBackboneImage);
            plPictureBox.SendToBack();
        }
        
        
        
        
        private void plFA1Checkbox2MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = plIsCL.Checked ? cardioBackboneImageFA1p : (plIsLyso.Checked ? phosphoLysoBackboneImageFA1p : phosphoBackboneImageFA1p);
            plPictureBox.SendToBack();
        }

        
        
        
        void plFA2Checkbox3MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = plIsCL.Checked ? cardioBackboneImage : phosphoBackboneImage;
            plPictureBox.SendToBack();
        }
        
        
        
        
        
        private void plFA2Checkbox3MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = plIsCL.Checked ? cardioBackboneImageFA2e : phosphoBackboneImageFA2e;
            plPictureBox.SendToBack();
        }
        
        
        
        
        
        void plFA2Checkbox2MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = plIsCL.Checked ? cardioBackboneImage : phosphoBackboneImage;
            plPictureBox.SendToBack();
        }
        
        
        
        

        private void plFA2checkbox2MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = plIsCL.Checked ? cardioBackboneImageFA2p : phosphoBackboneImageFA2p;
            plPictureBox.SendToBack();
        }
        
        
        
        
        private void plHGListboxSelectedValueChanged(object sender, System.EventArgs e)
        {
            if (settingListbox) return;
            currentLipid.headGroupNames.Clear();
            foreach(string itemChecked in plHgListbox.SelectedItems)
            {
                currentLipid.headGroupNames.Add(itemChecked);
            }
            
        }
        
        
        
        
        public void plRepresentativeFACheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).representativeFA = ((CheckBox)sender).Checked;
            if (((Phospholipid)currentLipid).representativeFA)
            {
                plFA2Textbox.Enabled = false;
                plDB2Textbox.Enabled = false;
                plHydroxyl2Textbox.Enabled = false;
                plFA2Combobox.Enabled = false;
                plFA2Checkbox1.Enabled = false;
                
                plFA2Textbox.Text = plFA1Textbox.Text;
                plDB2Textbox.Text = plDB1Textbox.Text;
                plHydroxyl2Textbox.Text = plHydroxyl1Textbox.Text;
                plFA2Combobox.Text = plFA1Combobox.Text;
                plFA2Checkbox1.Checked = plFA1Checkbox1.Checked;
                
                if (plIsCL.Checked)
                {
                    plFA2Checkbox2.Enabled = false;
                    plFA2Checkbox3.Enabled = false;
                    
                    clFA3Textbox.Enabled = false;
                    clDB3Textbox.Enabled = false;
                    clHydroxyl3Textbox.Enabled = false;
                    clFA3Combobox.Enabled = false;
                    clFA3Checkbox1.Enabled = false;
                    clFA3Checkbox2.Enabled = false;
                    clFA3Checkbox3.Enabled = false;
                    
                    clFA3Textbox.Text = plFA1Textbox.Text;
                    clDB3Textbox.Text = plDB1Textbox.Text;
                    clHydroxyl3Textbox.Text = plHydroxyl1Textbox.Text;
                    clFA3Combobox.Text = plFA1Combobox.Text;
                    clFA3Checkbox1.Checked = plFA1Checkbox1.Checked;
                    clFA3Checkbox2.Checked = plFA1Checkbox2.Checked;
                    clFA3Checkbox3.Checked = plFA1Checkbox3.Checked;
                    
                    clFA4Textbox.Enabled = false;
                    clDB4Textbox.Enabled = false;
                    clHydroxyl4Textbox.Enabled = false;
                    clFA4Combobox.Enabled = false;
                    clFA4Checkbox1.Enabled = false;
                    clFA4Checkbox2.Enabled = false;
                    clFA4Checkbox3.Enabled = false;
                    
                    clFA4Textbox.Text = plFA1Textbox.Text;
                    clDB4Textbox.Text = plDB1Textbox.Text;
                    clHydroxyl4Textbox.Text = plHydroxyl1Textbox.Text;
                    clFA4Combobox.Text = plFA1Combobox.Text;
                    clFA4Checkbox1.Checked = plFA1Checkbox1.Checked;
                    clFA4Checkbox2.Checked = plFA1Checkbox2.Checked;
                    clFA4Checkbox3.Checked = plFA1Checkbox3.Checked;
                }
            }
            else
            {
                plFA2Textbox.Enabled = true;
                plDB2Textbox.Enabled = true;
                plHydroxyl2Textbox.Enabled = true;
                plFA2Combobox.Enabled = true;
                plFA2Checkbox1.Enabled = true;
                
                if (plIsCL.Checked)
                {
                    plFA2Checkbox2.Enabled = true;
                    plFA2Checkbox3.Enabled = true;
                    
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
            }
            updateRanges(((Phospholipid)currentLipid).fag2, plFA2Textbox, plFA2Combobox.SelectedIndex);
            updateRanges(((Phospholipid)currentLipid).fag2, plDB2Textbox, 3);
            updateRanges(((Phospholipid)currentLipid).fag2, plHydroxyl2Textbox, 4);
            
            
            if (plIsCL.Checked)
            {
                updateRanges(((Phospholipid)currentLipid).fag3, clFA3Textbox, clFA3Combobox.SelectedIndex);
                updateRanges(((Phospholipid)currentLipid).fag3, clDB3Textbox, 3);
                updateRanges(((Phospholipid)currentLipid).fag3, clHydroxyl3Textbox, 4);
                
                updateRanges(((Phospholipid)currentLipid).fag4, clFA4Textbox, clFA4Combobox.SelectedIndex);
                updateRanges(((Phospholipid)currentLipid).fag4, clDB4Textbox, 3);
                updateRanges(((Phospholipid)currentLipid).fag4, clHydroxyl4Textbox, 4);
            }
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
        
        void plChangeLyso(bool lyso)
        {
            
            List<String> plHgList = new List<String>();
            plHgListbox.Items.Clear();
            
            
            if (lyso)
            {
                foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.Glycerophospholipid])
                {
                    if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].attributes.Contains("heavy") && !lipidCreator.headgroups[headgroup].attributes.Contains("ether") && lipidCreator.headgroups[headgroup].attributes.Contains("lyso")) plHgList.Add(headgroup);
                }
                plPictureBox.Left = 106;
                plPictureBox.Image = phosphoLysoBackboneImage;
                
                plFA2Combobox.Visible = false;
                plFA2Textbox.Visible = false;
                plDB2Textbox.Visible = false;
                plHydroxyl2Textbox.Visible = false;
                plDB2Label.Visible = false;
                plHydroxyl2Label.Visible = false;
                plFA2Checkbox1.Visible = false;
            }
            else
            {
                foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.Glycerophospholipid])
                {
                    if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].attributes.Contains("heavy") && !lipidCreator.headgroups[headgroup].attributes.Contains("ether") && !lipidCreator.headgroups[headgroup].attributes.Contains("lyso") && !headgroup.Equals("CL") && !headgroup.Equals("MLCL")) plHgList.Add(headgroup);
                }
                plPictureBox.Left = 107;
                plPictureBox.Image = phosphoBackboneImage;
            }
            plPictureBox.SendToBack();
            plHgList.Sort();
            plHgListbox.Items.AddRange(plHgList.ToArray());
            enableDisableAdducts((int)currentIndex);
        }
        
        ////////////////////// SL ////////////////////////////////
        
        
        
        public void slPosAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        public void slPosAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        public void slPosAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        public void slNegAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        public void slNegAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        public void slNegAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        public void slNegAdductCheckbox4CheckedChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
        }
        
        public void slDB1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).fag.dbInfo = ((TextBox)sender).Text;
            updateRanges(((Sphingolipid)currentLipid).fag, (TextBox)sender, 3);
        }
        public void slDB2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).lcb.dbInfo = ((TextBox)sender).Text;
            updateRanges(((Sphingolipid)currentLipid).lcb, (TextBox)sender, 3);
        }
        
        public void slFATextboxValueChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).fag.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((Sphingolipid)currentLipid).fag, (TextBox)sender, slFACombobox.SelectedIndex);
        }
        public void slLCBTextboxValueChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).lcb.lengthInfo = ((TextBox)sender).Text;
            updateRanges(((Sphingolipid)currentLipid).lcb, (TextBox)sender, slLCBCombobox.SelectedIndex, true);
        }
        
        
        public void slFAComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).fag.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((Sphingolipid)currentLipid).fag, slFATextbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void slLCBComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).lcb.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(((Sphingolipid)currentLipid).lcb, slLCBTextbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void slLCBHydroxyComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).lcb.hydroxylCounts.Clear();
            ((Sphingolipid)currentLipid).lcb.hydroxylCounts.Add(((ComboBox)sender).SelectedIndex + 2);
        }
        
        public void slFAHydroxyComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).fag.hydroxylCounts.Clear();
            ((Sphingolipid)currentLipid).fag.hydroxylCounts.Add(((ComboBox)sender).SelectedIndex);
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
        
        
        
        void slIsLysoCheckedChanged(object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).isLyso = slIsLyso.Checked;
            slChangeLyso(slIsLyso.Checked);
        }
        
        
        
        void slChangeLyso(bool lyso)
        {
            
            List<String> slHgList = new List<String>();
            slHgListbox.Items.Clear();
            
            
            if (lyso)
            {
                foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.Sphingolipid])
                {
                    if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].attributes.Contains("heavy") && lipidCreator.headgroups[headgroup].attributes.Contains("lyso")) slHgList.Add(headgroup);
                }
                slPictureBox.Image = sphingoLysoBackboneImage;
                slFACombobox.Visible = false;
                slFATextbox.Visible = false;
                slDB1Textbox.Visible = false;
                slFAHydroxyCombobox.Visible = false;
                slDB1Label.Visible = false;
                slFAHydroxyLabel.Visible = false;
            }
            else
            {
                foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.Sphingolipid])
                {
                    if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].attributes.Contains("heavy") && !lipidCreator.headgroups[headgroup].attributes.Contains("lyso")) slHgList.Add(headgroup);
                }
                slPictureBox.Image = sphingoBackboneImage;
                slFACombobox.Visible = true;
                slFATextbox.Visible = true;
                slDB1Textbox.Visible = true;
                slFAHydroxyCombobox.Visible = true;
                slDB1Label.Visible = true;
                slFAHydroxyLabel.Visible = true;
            }
            
            slHgList.Sort();
            slHgListbox.Items.AddRange(slHgList.ToArray());
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
                medPictureBox.Top = mediatorMiddleHeight - (medPictureBox.Image.Height >> 1);
                medPictureBox.SendToBack();
            }
        }
        
        void medHGListboxMouseLeave(object sender, EventArgs e)
        {
            //medPictureBox.Image = null;
            //medPictureBox.SendToBack();
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
            
            if (currentLipid is Glycerolipid)
            {
                if (((Glycerolipid)currentLipid).fag1.faTypes["FAx"])
                {
                    MessageBox.Show("Please always select the top fatty acid!", "Not registrable");
                    return  LipidCategory.NoLipid;
                }
                else if (((Glycerolipid)currentLipid).fag2.faTypes["FAx"] && !((Glycerolipid)currentLipid).fag3.faTypes["FAx"])
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
                if (((Glycerolipid)currentLipid).containsSugar)
                {
                    if (currentLipid.headGroupNames.Count == 0)
                    {
                        MessageBox.Show("No head group selected!", "Not registrable");
                        return  LipidCategory.NoLipid;                    
                    }
                    if (((Glycerolipid)currentLipid).fag1.faTypes["FAx"] || ((Glycerolipid)currentLipid).fag2.faTypes["FAx"])
                    {
                        MessageBox.Show("Both fatty acids must be selected!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                }
                else
                {
                    if (((Glycerolipid)currentLipid).fag1.faTypes["FAx"] && ((Glycerolipid)currentLipid).fag2.faTypes["FAx"] && ((Glycerolipid)currentLipid).fag3.faTypes["FAx"])
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
                return LipidCategory.Glycerolipid;
            }
            
            
            else if (currentLipid is Phospholipid)
            {
                if (((Phospholipid)currentLipid).isCL)
                {
                    if (((Phospholipid)currentLipid).fag1.faTypes["FAx"] || ((Phospholipid)currentLipid).fag2.faTypes["FAx"] || ((Phospholipid)currentLipid).fag3.faTypes["FAx"])
                    {
                        MessageBox.Show("At least the top three fatty acids must be selected!", "Not registrable");
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
                    
                    if (((Phospholipid)currentLipid).fag1.faTypes["FAx"])
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
                return LipidCategory.Glycerophospholipid;
            }
            
            
            else if (currentLipid is Sphingolipid)
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
                return LipidCategory.Sphingolipid;
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
                return LipidCategory.Sterollipid;
            }
            
            else if (currentLipid is Mediator)
            {
                if (currentLipid.headGroupNames.Count == 0)
                {
                    MessageBox.Show("No mediator selected!", "Not registrable");
                    return  LipidCategory.NoLipid;                    
                }
                return LipidCategory.LipidMediator;
            }
            return LipidCategory.NoLipid;
        }
        
        
        public void modifyLipid(Object sender, EventArgs e)
        {
            LipidCategory result = checkPropertiesValid();
            int rowIndex = lipidModifications[(int)result];
            switch (result)
            {
                case LipidCategory.Glycerolipid:
                    lipidCreator.registeredLipids[rowIndex] = new Glycerolipid((Glycerolipid)currentLipid);
                    break;
                case LipidCategory.Glycerophospholipid:
                    lipidCreator.registeredLipids[rowIndex] = new Phospholipid((Phospholipid)currentLipid);
                    break;
                    
                case LipidCategory.Sphingolipid:
                    lipidCreator.registeredLipids[rowIndex] = new Sphingolipid((Sphingolipid)currentLipid);
                    break;
                    
                case LipidCategory.Sterollipid:
                    lipidCreator.registeredLipids[rowIndex] = new Cholesterol((Cholesterol)currentLipid);
                    break;
                    
                case LipidCategory.LipidMediator:
                    lipidCreator.registeredLipids[rowIndex] = new Mediator((Mediator)currentLipid);
                    break;
                default:
                    return;
            }
            
            DataRow tmpRow = createLipidsGridviewRow(currentLipid);
            foreach (DataColumn dc in registeredLipidsDatatable.Columns) registeredLipidsDatatable.Rows[rowIndex][dc.ColumnName] = tmpRow[dc.ColumnName];
            
            lipidsGridview.Rows[rowIndex].Cells["Edit"].Value = editImage;
            lipidsGridview.Rows[rowIndex].Cells["Delete"].Value = deleteImage;
            lipidsGridview.Update();
            lipidsGridview.Refresh();
        }
        
        
        
        public void registerLipid(Object sender, EventArgs e)
        {
            LipidCategory result = checkPropertiesValid();
            int tabIndex = 0;
            switch (result)
            {
                case LipidCategory.Glycerolipid:
                    lipidCreator.registeredLipids.Add(new Glycerolipid((Glycerolipid)currentLipid));
                    registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                    tabIndex = (int)LipidCategory.Glycerolipid;
                    break;
                    
                case LipidCategory.Glycerophospholipid:
                    lipidCreator.registeredLipids.Add(new Phospholipid((Phospholipid)currentLipid));
                    registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                    tabIndex = (int)LipidCategory.Glycerophospholipid;
                    break;
                    
                case LipidCategory.Sphingolipid:
                    lipidCreator.registeredLipids.Add(new Sphingolipid((Sphingolipid)currentLipid));
                    registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                    tabIndex = (int)LipidCategory.Sphingolipid;
                    break;
                    
                case LipidCategory.Sterollipid:
                    lipidCreator.registeredLipids.Add(new Cholesterol((Cholesterol)currentLipid));
                    registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                    tabIndex = (int)LipidCategory.Sterollipid;
                    break;
                    
                case LipidCategory.LipidMediator:
                    lipidCreator.registeredLipids.Add(new Mediator((Mediator)currentLipid));
                    registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                    tabIndex = (int)LipidCategory.LipidMediator;
                    break;
                    
                default:
                    return;
            }
            lipidsGridview.Rows[lipidsGridview.Rows.Count - 1].Cells["Edit"].Value = editImage;
            lipidsGridview.Rows[lipidsGridview.Rows.Count - 1].Cells["Delete"].Value = deleteImage;
            lipidsGridview.Update();
            lipidsGridview.Refresh();
            
            for (int i = 0; i < lipidModifications.Length; ++i) lipidModifications[i] = -1;
            lipidModifications[tabIndex] = lipidsGridview.Rows.Count - 1;
            modifyLipidButton.Enabled = true;
        }
        
        
        public string FARepresentation(FattyAcidGroup fag)
        {
            string faRepresentation = "";
            
            if (fag.faTypes["FA"])
            {
                if (faRepresentation.Length > 0) faRepresentation += ", ";
                faRepresentation += "FA";
            }
            if (fag.faTypes["FAp"])
            {
                if (faRepresentation.Length > 0) faRepresentation += ", ";
                faRepresentation += "FAp";
            }
            if (fag.faTypes["FAa"])
            {
                if (faRepresentation.Length > 0) faRepresentation += ", ";
                faRepresentation += "FAa";
            }
            faRepresentation += ": ";
            
            return faRepresentation;
        }
        
        
        public DataRow createLipidsGridviewRow(Lipid currentRegisteredLipid)
        {
            DataRow row = registeredLipidsDatatable.NewRow();
            if (currentRegisteredLipid is Glycerolipid)
            {
                Glycerolipid currentGlycerolipid = (Glycerolipid)currentRegisteredLipid;
                row["Category"] = "Glycerolipid";
                row["Building Block 1"] = FARepresentation(currentGlycerolipid.fag1) + currentGlycerolipid.fag1.lengthInfo + "; DB: " + currentGlycerolipid.fag1.dbInfo + "; OH: " + currentGlycerolipid.fag1.hydroxylInfo;
                if (!currentGlycerolipid.fag2.faTypes["FAx"]) row["Building Block 2"] = FARepresentation(currentGlycerolipid.fag2) + currentGlycerolipid.fag2.lengthInfo + "; DB: " + currentGlycerolipid.fag2.dbInfo + "; OH: " + currentGlycerolipid.fag2.hydroxylInfo;
                if (currentGlycerolipid.containsSugar)
                {
                    row["Building Block 3"] = "HG: " + String.Join(", ", currentGlycerolipid.headGroupNames);
                }
                else
                {
                    if (!currentGlycerolipid.fag3.faTypes["FAx"]) row["Building Block 3"] = FARepresentation(currentGlycerolipid.fag3) + currentGlycerolipid.fag3.lengthInfo + "; DB: " + currentGlycerolipid.fag3.dbInfo + "; OH: " + currentGlycerolipid.fag3.hydroxylInfo;
                }
            }
            else if (currentRegisteredLipid is Phospholipid)
            {
                Phospholipid currentPhospholipid = (Phospholipid)currentRegisteredLipid;
                if (currentPhospholipid.isCL)
                {
                    row["Category"] = "Cardiolipin";
                    row["Building Block 1"] = FARepresentation(currentPhospholipid.fag1) + currentPhospholipid.fag1.lengthInfo + "; DB: " + currentPhospholipid.fag1.dbInfo + "; OH: " + currentPhospholipid.fag1.hydroxylInfo;
                    row["Building Block 2"] = FARepresentation(currentPhospholipid.fag2) + currentPhospholipid.fag2.lengthInfo + "; DB: " + currentPhospholipid.fag2.dbInfo + "; OH: " + currentPhospholipid.fag2.hydroxylInfo;
                    row["Building Block 3"] = FARepresentation(currentPhospholipid.fag3) + currentPhospholipid.fag3.lengthInfo + "; DB: " + currentPhospholipid.fag3.dbInfo + "; OH: " + currentPhospholipid.fag3.hydroxylInfo;
                    if (!currentPhospholipid.fag4.faTypes["FAx"]) row["Building Block 4"] = FARepresentation(currentPhospholipid.fag4) + currentPhospholipid.fag4.lengthInfo + "; DB: " + currentPhospholipid.fag4.dbInfo + "; OH: " + currentPhospholipid.fag4.hydroxylInfo;
                }
                else
                {
                    row["Category"] = "Glycerophospholipid";
                    row["Building Block 1"] = "HG: " + String.Join(", ", currentPhospholipid.headGroupNames);
                    row["Building Block 2"] = FARepresentation(currentPhospholipid.fag1) + currentPhospholipid.fag1.lengthInfo + "; DB: " + currentPhospholipid.fag1.dbInfo + "; OH: " + currentPhospholipid.fag1.hydroxylInfo;
                    if (!currentPhospholipid.isLyso) row["Building Block 3"] = FARepresentation(currentPhospholipid.fag2) + currentPhospholipid.fag2.lengthInfo + "; DB: " + currentPhospholipid.fag2.dbInfo + "; OH: " + currentPhospholipid.fag2.hydroxylInfo;
                }
            }
            else if (currentRegisteredLipid is Sphingolipid)
            {
                Sphingolipid currentSphingolipid = (Sphingolipid)currentRegisteredLipid;
                row["Category"] = "Sphingolipid";
                row["Building Block 1"] = "HG: " + String.Join(", ", currentSphingolipid.headGroupNames);
                row["Building Block 2"] = "LCB: " + currentSphingolipid.lcb.lengthInfo + "; DB: " + currentSphingolipid.lcb.dbInfo + "; OH: " + currentSphingolipid.lcb.hydroxylCounts.First();
                if (!currentSphingolipid.isLyso) row["Building Block 3"] = "FA: " + currentSphingolipid.fag.lengthInfo + "; DB: " + currentSphingolipid.fag.dbInfo + "; OH: " + currentSphingolipid.fag.hydroxylCounts.First();
            }
            
            else if (currentRegisteredLipid is Cholesterol)
            {
                Cholesterol currentCHLipid = (Cholesterol)currentRegisteredLipid;
                row["Category"] = "Sterol lipid";
                row["Building Block 1"] = "Ch" + (currentCHLipid.containsEster ? "E" : "");
                if (currentCHLipid.containsEster) row["Building Block 2"] = "FA: " + currentCHLipid.fag.lengthInfo + "; DB: " + currentCHLipid.fag.dbInfo + "; OH: " + currentCHLipid.fag.hydroxylInfo;
            }
            
            else if (currentRegisteredLipid is Mediator)
            {
                Mediator currentMedLipid = (Mediator)currentRegisteredLipid;
                row["Building Block 1"] = String.Join(", ", currentMedLipid.headGroupNames);
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
            
            string filtersStr = "";
            if (currentRegisteredLipid.onlyPrecursors == 0) filtersStr = "no precursors,\n";
            else if (currentRegisteredLipid.onlyPrecursors == 1) filtersStr = "only precursors,\n";
            else if (currentRegisteredLipid.onlyPrecursors == 2) filtersStr = "with precursors,\n";
            
            if (currentRegisteredLipid.onlyHeavyLabeled == 0) filtersStr += "no heavy";
            else if (currentRegisteredLipid.onlyHeavyLabeled == 1) filtersStr += "only heavy";
            else if (currentRegisteredLipid.onlyHeavyLabeled == 2) filtersStr += "with heavy";
            row["Filters"] = filtersStr;
            
            return row;
        }
        
        
        
        public void refreshRegisteredLipidsTable()
        {
            registeredLipidsDatatable.Clear();
            foreach (Lipid currentRegisteredLipid in lipidCreator.registeredLipids)
            {
                registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentRegisteredLipid));
                
            }
            for (int i = 0; i < lipidsGridview.Rows.Count; ++i)
            {
                lipidsGridview.Rows[i].Cells["Edit"].Value = editImage;
                lipidsGridview.Rows[i].Cells["Delete"].Value = deleteImage;
            }
            lipidsGridview.Update();
            lipidsGridview.Refresh();
        }
        
        public void lipidsGridviewDoubleClick(Object sender, EventArgs e)
        {
            int rowIndex = ((DataGridView)sender).CurrentCell.RowIndex;
            int colIndex = ((DataGridView)sender).CurrentCell.ColumnIndex;
            if (((DataGridView)sender).Columns[colIndex].Name == "Edit")
            {
            
                Lipid currentRegisteredLipid = (Lipid)lipidCreator.registeredLipids[rowIndex];
                int tabIndex = 0;
                for (int i = 0; i < lipidModifications.Length; ++i) lipidModifications[i] = -1;
                if (currentRegisteredLipid is Glycerolipid)
                {
                    tabIndex = (int)LipidCategory.Glycerolipid;
                    lipidTabList[tabIndex] = new Glycerolipid((Glycerolipid)currentRegisteredLipid);
                }
                else if (currentRegisteredLipid is Phospholipid)
                {
                    tabIndex = (int)LipidCategory.Glycerophospholipid;
                    lipidTabList[tabIndex] = new Phospholipid((Phospholipid)currentRegisteredLipid);
                }
                else if (currentRegisteredLipid is Sphingolipid)
                {
                    tabIndex = (int)LipidCategory.Sphingolipid;
                    lipidTabList[tabIndex] = new Sphingolipid((Sphingolipid)currentRegisteredLipid);
                }
                else if (currentRegisteredLipid is Cholesterol)
                {
                    tabIndex = (int)LipidCategory.Sterollipid;
                    lipidTabList[tabIndex] = new Cholesterol((Cholesterol)currentRegisteredLipid);
                }
                else if (currentRegisteredLipid is Mediator)
                {
                    tabIndex = (int)LipidCategory.LipidMediator;
                    lipidTabList[tabIndex] = new Mediator((Mediator)currentRegisteredLipid);
                }
                currentLipid = currentRegisteredLipid;
                lipidModifications[tabIndex] = rowIndex;
                tabControl.SelectedIndex = tabIndex;
                changeTab(tabIndex);
                
            }
            else if (((DataGridView)sender).Columns[colIndex].Name == "Delete")
            {
                deleteLipidsGridviewRow(rowIndex);
            }
        }
        
        
        public void lipidsGridviewKeydown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lipidCreator.registeredLipids.Count > 0 && ((DataGridView)sender).SelectedRows.Count > 0)
            {   
                deleteLipidsGridviewRow(((DataGridView)sender).SelectedRows[0].Index);
                e.Handled = true;
            }
        }
        
        
        public void deleteLipidsGridviewRow(int rowIndex)
        {
            Lipid currentRegisteredLipid = (Lipid)lipidCreator.registeredLipids[rowIndex];
            int tabIndex = 0;
            if (currentRegisteredLipid is Glycerolipid) tabIndex = (int)LipidCategory.Glycerolipid;
            else if (currentRegisteredLipid is Phospholipid) tabIndex = (int)LipidCategory.Glycerophospholipid;
            else if (currentRegisteredLipid is Sphingolipid) tabIndex = (int)LipidCategory.Sphingolipid;
            else if (currentRegisteredLipid is Cholesterol) tabIndex = (int)LipidCategory.Sterollipid;
            else if (currentRegisteredLipid is Mediator) tabIndex = (int)LipidCategory.LipidMediator;
            
            DataTable tmpTable = registeredLipidsDatatable.Clone();
            for (int i = 0; i < registeredLipidsDatatable.Rows.Count; ++i)
            {
                if (i != rowIndex) tmpTable.ImportRow(registeredLipidsDatatable.Rows[i]);
            }
            registeredLipidsDatatable.Rows.Clear();
            
            foreach (DataRow row in tmpTable.Rows) registeredLipidsDatatable.ImportRow(row);
            
            if ((int)lipidModifications[tabIndex] == rowIndex) lipidModifications[tabIndex] = -1;
            else if ((int)lipidModifications[tabIndex] > rowIndex) lipidModifications[tabIndex] -= 1;
            if (tabIndex == (int)currentIndex) changeTab(tabIndex);
            
            for (int i = 0; i < lipidsGridview.Rows.Count; ++i)
            {
                lipidsGridview.Rows[i].Cells["Edit"].Value = editImage;
                lipidsGridview.Rows[i].Cells["Delete"].Value = deleteImage;
            }
            
            lipidCreator.registeredLipids.RemoveAt(rowIndex);
            lipidsGridview.Update();
            lipidsGridview.Refresh();
        }
        
        
        public void unsetInstrument(Object sender, EventArgs e)
        {
            lipidCreator.selectedInstrumentForCE = "";
            menuCollisionEnergyOpt.Enabled = false;
            lipidCreator.monitoringType = MonitoringTypes.NoMonitoring;
            lastCEInstrumentChecked.Checked = false;
            lastCEInstrumentChecked = (MenuItem)sender;
            lastCEInstrumentChecked.Checked = true;
        }
        
        
        public void changeInstrumentForCEtypePRM(Object sender, EventArgs e)
        {
            if(((MenuItem)sender).Tag != null) {
                string instrument = ((string[])((MenuItem)sender).Tag)[0];
                lipidCreator.selectedInstrumentForCE = (string)lipidCreator.msInstruments[instrument].CVTerm;
            
                menuCollisionEnergyOpt.Enabled = tutorial.tutorial == Tutorials.NoTutorial;
                lipidCreator.monitoringType = MonitoringTypes.PRM;
                lastCEInstrumentChecked.Checked = false;
                lastCEInstrumentChecked = (MenuItem)sender;
                lastCEInstrumentChecked.Checked = true;
            }
        }
        
        
        public void changeInstrumentForCEtypeSRM(Object sender, EventArgs e)
        {
            if(((MenuItem)sender).Tag != null) {
                string instrument = ((string[])((MenuItem)sender).Tag)[0];
                lipidCreator.selectedInstrumentForCE = (string)lipidCreator.msInstruments[instrument].CVTerm;
            
                menuCollisionEnergyOpt.Enabled = false;
                lipidCreator.monitoringType = MonitoringTypes.SRM;
                lastCEInstrumentChecked.Checked = false;
                lastCEInstrumentChecked = (MenuItem)sender;
                lastCEInstrumentChecked.Checked = true;
            }
        }
        
        
        
        
        public void openMS2Form(Object sender, EventArgs e)
        {
            Form formToOpen = null;
            switch ((int)currentIndex)
            {
                case (int)LipidCategory.NoLipid:
                    return;
                
                case ((int)LipidCategory.LipidMediator):
                    mediatorMS2fragmentsForm = new MediatorMS2Form(this, (Mediator)currentLipid);
                    formToOpen = (Form)mediatorMS2fragmentsForm;
                    break;
                
                default:
                    ms2fragmentsForm = new MS2Form(this);
                    formToOpen = (Form)ms2fragmentsForm;
                    break;
            }
            formToOpen.Owner = this;
            formToOpen.ShowInTaskbar = false;
            
            if (tutorial.tutorial == Tutorials.NoTutorial)
            {
                formToOpen.ShowDialog();
                formToOpen.Dispose();
            }
            else
            {
                formToOpen.Show();
            }
        }
        
        
        public void openFilterDialog(Object sender, EventArgs e)
        {
            filterDialog = new FilterDialog((Lipid)currentLipid);
            filterDialog.Owner = this;
            filterDialog.ShowInTaskbar = false;
            if (tutorial.tutorial == Tutorials.NoTutorial)
            {
                filterDialog.ShowDialog();
                filterDialog.Dispose();
            }
            else
            {
                filterDialog.Show();
            }
        }
        
        public void windowOnClosing(Object sender, FormClosingEventArgs e)
        {
            if (lipidCreator != null)
            {
                log.Info("Closing LipidCreator!");
                lipidCreator.Dispose();
            }
        }
        
        public void windowSizeChanged(Object sender, EventArgs e)
        {
            lipidsGroupbox.Height = minLipidGridHeight + this.Height - minWindowHeight;
        }
        
        
        
        public void startFirstTutorial(Object sender, EventArgs e)
        {
            tutorial.startTutorial(Tutorials.TutorialPRM);
        }
        
        
        
        public void startSecondTutorial(Object sender, EventArgs e)
        {
            tutorial.startTutorial(Tutorials.TutorialSRM);
        }
        
        
        
        public void startThirdTutorial(Object sender, EventArgs e)
        {
            tutorial.startTutorial(Tutorials.TutorialHL);
        }
        
        
        
        public void startFourthTutorial(Object sender, EventArgs e)
        {
            tutorial.startTutorial(Tutorials.TutorialCE);
        }
        
        
        
        public void openHeavyIsotopeForm(Object sender, EventArgs e)
        {
            if (currentIndex == LipidCategory.NoLipid) return;
            addHeavyPrecursor = new AddHeavyPrecursor(this, currentIndex);
            addHeavyPrecursor.Owner = this;
            addHeavyPrecursor.ShowInTaskbar = false;
            
            if (tutorial.tutorial == Tutorials.NoTutorial)
            {
                addHeavyPrecursor.ShowDialog();
                addHeavyPrecursor.Dispose();
            }
            else
            {
                addHeavyPrecursor.Show();
            }
            
        }
        
        
        public void openReviewForm(Object sender, EventArgs e)
        {
            lipidCreator.assemblePrecursors();
            lipidCreator.analytics("lipidcreator", "create-transition-list");
            
            
            ArrayList returnValues = new ArrayList(){false};
            
            
            if (tutorial.tutorial == Tutorials.NoTutorial)
            {
                
                while (!(bool)returnValues[0])
                {
                
                    lipidsInterList = new LipidsInterList(this, returnValues);
                    lipidsInterList.Owner = this;
                    lipidsInterList.ShowInTaskbar = false;
                
                    lipidsInterList.ShowDialog();
                    lipidsInterList.Dispose();
                    
                    if (!(bool)returnValues[0]) break;
                    
                    lipidCreator.assembleFragments(asDeveloper);   
                
                    lipidsReview = new LipidsReview(this, returnValues);
                    lipidsReview.Owner = this;
                    lipidsReview.ShowInTaskbar = false;
                    
                    lipidsReview.ShowDialog();
                    lipidsReview.Dispose();
                }
            }
            else
            {
                lipidsInterList = new LipidsInterList(this, returnValues);
                lipidsInterList.Owner = this;
                lipidsInterList.ShowInTaskbar = false;
                lipidsInterList.Show();
            }
        }
        
        
        
        
        protected void menuImportPredefinedClick(object sender, System.EventArgs e)
        {
        
            string[] returnMessage = new string[]{""};
            LCMessageBox lcmb = new LCMessageBox(returnMessage);
            lcmb.Owner = this;
            lcmb.StartPosition = FormStartPosition.CenterParent;
            lcmb.ShowInTaskbar = false;
            lcmb.ShowDialog();
            lcmb.Dispose();
            if (returnMessage[0] == "replace") resetLipidCreator(false);
        
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
                log.Error("Could not read file " + filePath + ":", ex);
            }
        }
        
        
        
        
        protected void menuImportListClick(object sender, System.EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(openFileDialog1.FileName))
                {
                    int[] filterParameters = {2, 2};
                    FilterDialog importFilterDialog = new FilterDialog(filterParameters);
                    importFilterDialog.Owner = this;
                    importFilterDialog.ShowInTaskbar = false;
                    importFilterDialog.ShowDialog();
                    importFilterDialog.Dispose();
                    
                    string[] returnMessage = new string[]{""};
                    LCMessageBox lcmb = new LCMessageBox(returnMessage);
                    lcmb.Owner = this;
                    lcmb.StartPosition = FormStartPosition.CenterParent;
                    lcmb.ShowInTaskbar = false;
                    lcmb.ShowDialog();
                    lcmb.Dispose();
                    if (returnMessage[0] == "replace") lipidCreator.registeredLipids.Clear();
                    
                    int[] importNumbers = lipidCreator.importLipidList(openFileDialog1.FileName, filterParameters);
                    refreshRegisteredLipidsTable();
                    MessageBox.Show("Here, " + importNumbers[0] + " of " + importNumbers[1] + " lipid names could be successfully imported!", "Lipid list import");
                }
                else
                {
                    MessageBox.Show("Could not read file, " + openFileDialog1.FileName, "Lipid list import");
                }
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
                string[] returnMessage = new string[]{""};
                LCMessageBox lcmb = new LCMessageBox(returnMessage);
                lcmb.Owner = this;
                lcmb.StartPosition = FormStartPosition.CenterParent;
                lcmb.ShowInTaskbar = false;
                lcmb.ShowDialog();
                lcmb.Dispose();
                if (returnMessage[0] == "replace") resetLipidCreator(false);
                
                XDocument doc;
                try 
                {
                    doc = XDocument.Load(openFileDialog1.FileName);
                    lipidCreator.import(doc);
                    resetAllLipids();
                    updateCECondition();
                    changeTab((int)currentIndex);
                    refreshRegisteredLipidsTable();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not read file, " + ex.Message, "Error while reading", MessageBoxButtons.OK);
                    log.Error("Could not read file " + openFileDialog1.FileName + ":", ex);
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
                string[] returnMessage = new string[]{""};
                LCMessageBox lcmb = new LCMessageBox(returnMessage);
                lcmb.Owner = this;
                lcmb.StartPosition = FormStartPosition.CenterParent;
                lcmb.ShowInTaskbar = false;
                lcmb.ShowDialog();
                lcmb.Dispose();
                if (returnMessage[0] == "replace") resetLipidCreator(false);
            
                XDocument doc;
                try 
                {
                    doc = XDocument.Load(openFileDialog1.FileName);
                    lipidCreator.import(doc, true);
                    resetAllLipids();
                    updateCECondition();
                    changeTab((int)currentIndex);
                    refreshRegisteredLipidsTable();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not read file, " + ex.Message, "Error while reading", MessageBoxButtons.OK);
                    log.Error("Could not read file " + openFileDialog1.FileName + ":", ex);
                }
            }
        }
        
        
        
        
        protected void menuCollisionEnergyOptClick(object sender, System.EventArgs e)
        {
            ceInspector = new CEInspector(this, lipidCreator.selectedInstrumentForCE);
            ceInspector.Owner = this;
            ceInspector.ShowInTaskbar = false;
            if (tutorial.tutorial == Tutorials.NoTutorial)
            {
                ceInspector.ShowDialog();
                ceInspector.Dispose();
            }
            else
            {
                ceInspector.Show();
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
                using (StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile()))
                {
                    writer.Write(lipidCreator.serialize());
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
                using (StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile()))
                {
                    writer.Write(lipidCreator.serialize(true));
                }
            }
        }
        
        
        
        protected void menuExitClick(object sender, System.EventArgs e)
        {
            Application.Exit();
        }
        
        
        
        protected void menuTranslateClick(object sender, System.EventArgs e)
        {
            TranslatorDialog translatorDialog = new TranslatorDialog(this);
            translatorDialog.Owner = this;
            translatorDialog.ShowInTaskbar = false;
            translatorDialog.ShowDialog ();
            translatorDialog.Dispose();
        }
        

        
        protected void menuAboutClick(object sender, System.EventArgs e)
        {
            AboutDialog aboutDialog = new AboutDialog ();
            aboutDialog.Owner = this;
            aboutDialog.ShowInTaskbar = false;
            aboutDialog.ShowDialog();
            aboutDialog.Dispose();
        }
        
        
        
        
        public static void printHelp(string option = "")
        {
            LipidCreator lc = new LipidCreator(null);
            StringBuilder b;
            switch (option)
            {
                case "transitionlist":
                    b = new StringBuilder("Creating a transition list from a lipid list");
                    b.AppendLine().
                    AppendLine("usage: LipidCreator.exe transitionlist input_csv output_csv [opts [opts ...]]").
                     AppendLine("  opts are:").
                     AppendLine("    -p 0:\t\tCompute no precursor transitions").
                     AppendLine("    -p 1:\t\tCompute only precursor transitions").
                     AppendLine("    -p 2:\t\tCompute with precursor transitions").
                     AppendLine("    -h 0:\t\tCompute no heavy labeled isotopes").
                     AppendLine("    -h 1:\t\tCompute only heavy labeled isotopes").
                     AppendLine("    -h 2:\t\tCompute with heavy labeled isotopes").
                     AppendLine("    -s:\t\t\tSplit in positive and negative list").
                     AppendLine("    -x:\t\t\tDeveloper or Xpert mode").
                     AppendLine("    -l:\t\t\tCreate LipidCreator project file instead of transition list").
                     AppendLine("    -d:\t\t\tDelete replicate transitions (equal precursor and fragment mass)").
                     AppendLine("    -c instrument mode:\tCompute with optimal collision energy (not available for all lipid classes)").
                     AppendLine("      available instruments and modes:");
                    foreach (KeyValuePair<string, InstrumentData> kvp in lc.msInstruments)
                    {
                        if (kvp.Value.minCE > 0 && kvp.Value.maxCE > 0 && kvp.Value.minCE < kvp.Value.maxCE) 
                        {
                            string fullInstrumentName = (string)(kvp.Value.model);
                            string modes = "(";
                            foreach (string mode in kvp.Value.modes)
                            {
                                if (modes.Length > 1) modes += " / ";
                                modes += mode;
                            }
                            modes += ")";
                            b.AppendLine("        '" + kvp.Key + "': " + fullInstrumentName + " " + modes);
                        }
                    }
                    Console.Write(b.ToString());
                    break;
                    
                    
                case "library":
                    b = new StringBuilder("Creating a spectral library in *.blib format from a lipid list").
                    AppendLine().
                    AppendLine("usage: LipidCreator.exe library input_csv output_blib instrument").
                    AppendLine("  available instruments:");
                    foreach (KeyValuePair<string, InstrumentData> kvp in lc.msInstruments)
                    {
                        if (kvp.Value.minCE > 0) 
                        {
                            string fullInstrumentName = kvp.Value.model;
                            b.AppendLine("    '" + kvp.Key + "': " + fullInstrumentName);
                        }
                    }
                    Console.Write(b.ToString());
                    break;
                    
                    
                    
                case "translate":
                    b = new StringBuilder("Translating a list with old lipid names into current nomenclature").
                    AppendLine().
                    AppendLine("usage: LipidCreator.exe translate input_csv output_csv");
                    Console.WriteLine(b.ToString());
                    break;
                    
                    
                    
                case "random":
                    b = new StringBuilder("Generating a random lipid name (not necessarily reasonable in terms of chemistry)").
                    AppendLine().
                    AppendLine("usage: LipidCreator.exe random [number]");
                    Console.Write(b.ToString());
                    break;
                    
                    
                case "agentmode":
                    b = new StringBuilder("\nUnsaturated fatty acids contain at least one special bond - James Bond!\n\n");
                    Console.Write(b.ToString());
                    break;
                    
                    
                default:
                    b = new StringBuilder("usage: LipidCreator.exe (option)").
                    AppendLine().
                    AppendLine("options are:").
                    AppendLine("  dev:\t\t\t\tlaunching LipidCreator as developer").
                    AppendLine("  transitionlist:\t\tcreating transition list from lipid list").
                    AppendLine("  translate:\t\t\ttranslating a list with old lipid names into current nomenclature").
                    AppendLine("  library:\t\t\tcreating a spectral library in *.blib format from a lipid list").
                    AppendLine("  random:\t\t\tgenerating a random lipid name (not necessarily reasonable in terms of chemistry)").
                    AppendLine("  agentmode:\t\t\tsecret agent mode").
                    AppendLine("  help:\t\t\t\tprint this help");
                    Console.Write(b.ToString());
                    break;
            }
            
            System.Environment.Exit(1);
        }
        
        
        
        
        public static void checkForAnalytics(bool withPrefix)
        {
            string analyticsFile = (withPrefix ? LipidCreator.EXTERNAL_PREFIX_PATH : "") + "data/analytics.txt";
            try {
                if (!File.Exists(analyticsFile))
                {
                    using (StreamWriter outputFile = new StreamWriter (analyticsFile))
                    {
                        outputFile.WriteLine ("-1");
                    }
                }
            }
            catch(Exception e) {
                log.Warn("Warning: Analytics file could not be opened for writing at " + analyticsFile + ". LipidCreator will continue without analytics enabled!", e);
            }
            
            try {
                if (File.Exists(analyticsFile))
                {
                    string analyticsContent = "";
                    using (StreamReader sr = new StreamReader(analyticsFile))
                    {
                        // check if first letter in first line is a '1'
                        String line = sr.ReadLine();
                        analyticsContent = line;
                    }
                    
                    if (analyticsContent == "-1")
                    {
                        DialogResult mbr = MessageBox.Show ("Thank you for choosing LipidCreator.\n\n"+
                                                            "LipidCreator is funded by the German federal ministry of education and research (BMBF) as part of the de.NBI initiative.\nThe project administration requires us to report ANONYMIZED usage statistics for this tool to evaluate its usefulness for the community.\n\n" +
                                                            "With your permission, we collect the following ANONYMIZED statistics:\n - # of LipidCreator launches\n - # of generated transition lists\n\n" + 
                                                            "We do NOT collect any of the following statistics:\n - IP address\n - operating system\n - any information that may traced back to the user\n\n" +
                                                            "When you click 'Yes':\n - you agree to allow us to collect ANONYMIZED usage statistics.\n\n" + 
                                                            "When you click 'No':\n - no data will be sent\n - you can use LipidCreator without any restrictions.\n\n" + 
                                                            "We would highly appreciate your help to secure further funding for the continued development of LipidCreator.", "LipidCreator note", MessageBoxButtons.YesNo);
                        
                        using (StreamWriter outputFile = new StreamWriter (analyticsFile))
                        {
                            outputFile.WriteLine ((mbr == DialogResult.Yes ? "1" : "0"));
                        }
                    }
                }
            }
            catch(Exception e)
            {
                log.Warn("Warning: Analytics file could not be opened at " + analyticsFile + ". LipidCreator will continue without analytics enabled!", e);
            }
        }
        
    
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
        
                if ((new HashSet<string>{"external", "dev", "help", "transitionlist", "library", "random", "agentmode", "translate"}).Contains(args[0]))
                {
                    switch (args[0])
                    {
                        
                        case "dev":
                            checkForAnalytics(false);
                            CreatorGUI creatorGUIDev = new CreatorGUI(null);
                            creatorGUIDev.asDeveloper = true;
                            creatorGUIDev.lipidCreator.analytics("lipidcreator", "launch");
                            Application.Run(creatorGUIDev);
                            break;
                            
                        
                        case "external":
                            checkForAnalytics(true);
                            CreatorGUI creatorGUI = new CreatorGUI(args[1]);
                            creatorGUI.lipidCreator.analytics("lipidcreator-external", "launch");
                            Application.Run(creatorGUI);
                            break;
                            
                            
                        case "agentmode":
                            printHelp("agentmode");
                            break;
                            
                            
                        case "help":
                            printHelp();
                            break;
                            
                            
                        case "random":
                            if (args.Length > 1 && args[1].Equals("help")) printHelp("random");
                            int num = 1;
                            if (args.Length > 1) num = int.TryParse(args[1], out num) ? num : 1;
                            foreach (string lipidName in LipidCreator.createRandomLipidNames(num)) Console.WriteLine(lipidName);
                            break;
                          
                          
                        case "translate":
                            if (args.Length < 3)
                            {
                                printHelp("translate");
                            }
                            else
                            {
                                string inputCSV = args[1];
                                string outputCSV = args[2];
                                
                                if (File.Exists(inputCSV))
                                {
                                    int lineCounter = 1;
                                    ArrayList lipidNames = new ArrayList();
                                    try
                                    {
                                        using (StreamReader sr = new StreamReader(inputCSV))
                                        {
                                            string line;
                                            while((line = sr.ReadLine()) != null)
                                            {
                                                lipidNames.Add(line);
                                                ++lineCounter;
                                            }
                                        }
                                        
                                        LipidCreator lc = new LipidCreator(null);
                                        lc.analytics("lipidcreator-cli", "launch");
                                        
                                        ArrayList parsedLipids = lc.translate(lipidNames);
                                        
                                        
                                        HashSet<String> usedKeys = new HashSet<String>();
                                        ArrayList precursorDataList = new ArrayList();
                                        int i = 0;
                                            
                                        using (StreamWriter outputFile = new StreamWriter (outputCSV))
                                        {
                                            foreach (Lipid currentLipid in parsedLipids)
                                            {
                                                string newLipidName = "";
                                                if (currentLipid != null)
                                                {
                                                    currentLipid.computePrecursorData(lc.headgroups, usedKeys, precursorDataList);
                                                    newLipidName = ((PrecursorData)precursorDataList[precursorDataList.Count - 1]).precursorName;
                                                    usedKeys.Clear();
                                                    
                                                }
                                                else
                                                {
                                                    newLipidName = "Unrecognized molecule";
                                                }
                                                outputFile.WriteLine ("\"" + (string)lipidNames[i] + "\",\"" + newLipidName + "\"");
                                                
                                                ++i;
                                            }
                                        }
                                        
                                    }
                                    catch (Exception e)
                                    {
                                        log.Error("The file '" + inputCSV + "' in line '" + lineCounter + "' could not be read:", e);
                                    }
                                }
                            }
                            break;
                            
                            
                        case "transitionlist":
                            if (args.Length < 3)
                            {
                                printHelp("transitionlist");
                            }
                            else
                            {
                                int parameterPrecursor = 0;
                                int parameterHeavy = 2;
                                string inputCSV = args[1];
                                string outputCSV = args[2];
                                string instrument = "";
                                string mode = "";
                                bool deleteReplicates = false;
                                bool split = false;
                                bool asDeveloper = false;
                                bool createXMLFile = false;
                                int p = 3;
                                while (p < args.Length)
                                {
                                    switch (args[p])
                                    {
                                        case "-p": // precursor parameter
                                            if (!(p + 1 < args.Length) || !(int.TryParse(args[p + 1], out parameterPrecursor))) printHelp("transitionlist");
                                            if (parameterPrecursor < 0 || 2 < parameterPrecursor) printHelp("transitionlist");
                                            p += 2;
                                            break;
                                            
                                        case "-h": // heavy isotope parameter
                                            if (!(p + 1 < args.Length) || !(int.TryParse(args[p + 1], out parameterHeavy))) printHelp("transitionlist");
                                            if (parameterHeavy < 0 || 2 < parameterHeavy) printHelp("transitionlist");
                                            p += 2;
                                            break;
                                            
                                        case "-c": // compute collision optimization parameter
                                            if (!(p + 2 < args.Length)) printHelp("transitionlist");
                                            instrument = args[p + 1];
                                            mode = args[p + 2];
                                            p += 3;
                                            break;
                                            
                                        case "-s": // file split parameter
                                            split = true;
                                            p += 1;
                                            break;
                                            
                                        case "-d":
                                            deleteReplicates = true;
                                            p += 1;
                                            break;
                                            
                                        case "-x":
                                            asDeveloper = true;
                                            p += 1;
                                            break;
                                            
                                        case "-l":
                                            createXMLFile = true;
                                            p += 1;
                                            break;
                                            
                                        default:
                                            printHelp("transitionlist");
                                            break;
                                    }
                                }
                                
                                
                                
                                LipidCreator lc = new LipidCreator(null);
                                lc.analytics("lipidcreator-cli", "launch");
                                
                                if (instrument != "" && (!lc.msInstruments.ContainsKey(instrument) || lc.msInstruments[instrument].minCE < 0)) printHelp("transitionlist");
                                
                                if (mode != "" && mode != "PRM" && mode != "SRM") printHelp("transitionlist");
                                
                                
                                XDocument doc;
                                try 
                                {
                                    doc = XDocument.Load(inputCSV);
                                    lc.import(doc);
                                }
                                catch (Exception ex)
                                {
                                    lc.importLipidList(inputCSV, new int[]{parameterPrecursor, parameterHeavy});
                                }
                                
                                
                                MonitoringTypes monitoringType = MonitoringTypes.NoMonitoring;
                                if (mode == "PRM") monitoringType = MonitoringTypes.PRM;
                                else if (mode == "SRM") monitoringType = MonitoringTypes.SRM;
                                
                                lc.selectedInstrumentForCE = instrument;
                                lc.monitoringType = monitoringType;
                                
                                if (!createXMLFile)
                                {
                                    lc.assembleLipids(asDeveloper); 
                                    DataTable transitionList = deleteReplicates ? lc.transitionListUnique : lc.transitionList;
                                    lc.storeTransitionList(",", split, outputCSV, transitionList);
                                }
                                else
                                {
                                    string outputDir = System.IO.Path.GetDirectoryName(outputCSV);
                                    System.IO.Directory.CreateDirectory(outputDir);
                                    using (StreamWriter writer = new StreamWriter (outputCSV))
                                    {
                                        writer.Write(lc.serialize());
                                    }
                                }
                            }
                            break;
                            
                            
                            
                        case "library":
                            if (args.Length < 4)
                            {
                                printHelp("library");
                            }
                            else
                            {
                                string inputCSV = args[1];
                                string outputCSV = args[2];
                                string instrument = args[3];
                                
                                
                                LipidCreator lc = new LipidCreator(null);
                                lc.analytics("lipidcreator-cli", "launch");
                                
                                if (instrument != "" && (!lc.msInstruments.ContainsKey(instrument) || lc.msInstruments[instrument].minCE < 0)) printHelp("transitionlist");
                                
                                lc.selectedInstrumentForCE = instrument;
                                lc.importLipidList(inputCSV);
                                lc.createPrecursorList();
                                lc.createBlib(outputCSV);
                            }
                            break;
                    }
                }
                else 
                {
                    printHelp();
                }
            }
            else 
            {
                checkForAnalytics(false);
                CreatorGUI creatorGUI = new CreatorGUI(null);
                creatorGUI.lipidCreator.analytics("lipidcreator-standalone", "launch");
                Application.Run(creatorGUI);
            }
        }
    }
}
