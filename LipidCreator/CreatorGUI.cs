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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Diagnostics;
using System.Threading;

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
        public MS2Form ms2fragmentsForm = null;
        public MediatorMS2Form mediatorMS2fragmentsForm = null;
        public AddHeavyPrecursor addHeavyPrecursor = null;
        public LipidsReview lipidsReview = null;
        public FilterDialog filterDialog = null;
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
                    outputFile.Dispose ();
                    outputFile.Close ();
                }
            }
            catch (Exception ex)
            {
            
            }
        }
        
        
        
        
        
        public bool resetLipidCreator()
        {
            DialogResult mbr = MessageBox.Show ("You are going to reset LipidCreator. All information and settings will be discarded. Are you sure?", "Reset LipidCreator", MessageBoxButtons.YesNo);
            if (mbr == DialogResult.Yes) {
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
                                                      new GLLipid(lipidCreator),
                                                      new PLLipid(lipidCreator),
                                                      new SLLipid(lipidCreator),
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
                    col.Width = w;
                }
                lipidsGridview.Columns[0].Width = 80;
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
            //Color color = (e.Index == (int)currentIndex) ? Color.White :  Color.FromArgb(199, 223, 237);
            //Color color = (e.Index == (int)currentIndex) ? Color.White :  Color.FromArgb(180, 209, 228);
            //Brush brush = (e.Index == (int)currentIndex) ? Brushes.Black :  Brushes.White;
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

        
        
        
        
        
        public void changeTabElements(int index)
        {
            //this.Refresh();
        
            // enable all adduct checkboxes
            foreach (KeyValuePair<string, Precursor> row in lipidCreator.headgroups)
            {
                int category = (int)row.Value.category;
                switch (category)
                {
                    case (int)LipidCategory.GlyceroLipid:
                        glPosAdductCheckbox1.Enabled |= row.Value.adductRestrictions["+H"];
                        glPosAdductCheckbox2.Enabled |= row.Value.adductRestrictions["+2H"];
                        glPosAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+NH4"];
                        glNegAdductCheckbox1.Enabled |= row.Value.adductRestrictions["-H"];
                        glNegAdductCheckbox2.Enabled |= row.Value.adductRestrictions["-2H"];
                        glNegAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+HCOO"];
                        glNegAdductCheckbox4.Enabled |= row.Value.adductRestrictions["+CH3COO"];
                        break;
                    
                    case (int)LipidCategory.PhosphoLipid:
                        plPosAdductCheckbox1.Enabled |= row.Value.adductRestrictions["+H"];
                        plPosAdductCheckbox2.Enabled |= row.Value.adductRestrictions["+2H"];
                        plPosAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+NH4"];
                        plNegAdductCheckbox1.Enabled |= row.Value.adductRestrictions["-H"];
                        plNegAdductCheckbox2.Enabled |= row.Value.adductRestrictions["-2H"];
                        plNegAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+HCOO"];
                        plNegAdductCheckbox4.Enabled |= row.Value.adductRestrictions["+CH3COO"];
                        break;
                        
                    case (int)LipidCategory.SphingoLipid:
                        slPosAdductCheckbox1.Enabled |= row.Value.adductRestrictions["+H"];
                        slPosAdductCheckbox2.Enabled |= row.Value.adductRestrictions["+2H"];
                        slPosAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+NH4"];
                        slNegAdductCheckbox1.Enabled |= row.Value.adductRestrictions["-H"];
                        slNegAdductCheckbox2.Enabled |= row.Value.adductRestrictions["-2H"];
                        slNegAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+HCOO"];
                        slNegAdductCheckbox4.Enabled |= row.Value.adductRestrictions["+CH3COO"];
                        break;
                        
                    case (int)LipidCategory.Mediator:
                        medNegAdductCheckbox1.Enabled |= row.Value.adductRestrictions["-H"];
                        medNegAdductCheckbox2.Enabled |= row.Value.adductRestrictions["-2H"];
                        medNegAdductCheckbox3.Enabled |= row.Value.adductRestrictions["+HCOO"];
                        medNegAdductCheckbox4.Enabled |= row.Value.adductRestrictions["+CH3COO"];
                        break;
                        
                    case (int)LipidCategory.Cholesterol:
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
        
            extendWindow(((LipidCategory)index == LipidCategory.PhosphoLipid) && ((PLLipid)currentLipid).isCL);
            
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
                    glFA1Checkbox3.Checked = currentGLLipid.fag1.faTypes["FAa"];
                    
                    glFA2Textbox.Text = currentGLLipid.fag2.lengthInfo;
                    glDB2Textbox.Text = currentGLLipid.fag2.dbInfo;
                    glHydroxyl2Textbox.Text = currentGLLipid.fag2.hydroxylInfo;
                    glFA2Combobox.SelectedIndex = currentGLLipid.fag2.chainType;
                    glFA2Checkbox1.Checked = currentGLLipid.fag2.faTypes["FA"];
                    glFA2Checkbox2.Checked = currentGLLipid.fag2.faTypes["FAp"];
                    glFA2Checkbox3.Checked = currentGLLipid.fag2.faTypes["FAa"];
                    
                    glFA3Textbox.Text = currentGLLipid.fag3.lengthInfo;
                    glDB3Textbox.Text = currentGLLipid.fag3.dbInfo;
                    glHydroxyl3Textbox.Text = currentGLLipid.fag3.hydroxylInfo;
                    glFA3Combobox.SelectedIndex = currentGLLipid.fag3.chainType;
                    glFA3Checkbox1.Checked = currentGLLipid.fag3.faTypes["FA"];
                    glFA3Checkbox2.Checked = currentGLLipid.fag3.faTypes["FAp"];
                    glFA3Checkbox3.Checked = currentGLLipid.fag3.faTypes["FAa"];
                    
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
                    
                    if (currentPLLipid.isCL) plIsCL.Checked = true;
                    else if (currentPLLipid.isLyso) plIsLyso.Checked = true;
                    else plRegular.Checked = true;
                    
                    // unset lyso
                    plFA2Combobox.Visible = true;
                    plFA2Textbox.Visible = true;
                    plDB2Textbox.Visible = true;
                    plHydroxyl2Textbox.Visible = true;
                    plDB2Label.Visible = true;
                    plHydroxyl2Label.Visible = true;
                    
                    
                    
                    if (currentPLLipid.isCL) // Cardiolipin
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
                        
                        
                        plFA1Textbox.Text = currentPLLipid.fag1.lengthInfo;
                        plDB1Textbox.Text = currentPLLipid.fag1.dbInfo;
                        plHydroxyl1Textbox.Text = currentPLLipid.fag1.hydroxylInfo;
                        plFA1Combobox.SelectedIndex = currentPLLipid.fag1.chainType;
                        plFA1Checkbox1.Checked = currentPLLipid.fag1.faTypes["FA"];
                        plFA1Checkbox2.Checked = currentPLLipid.fag1.faTypes["FAp"];
                        plFA1Checkbox3.Checked = currentPLLipid.fag1.faTypes["FAa"];
                        
                        plFA2Textbox.Text = currentPLLipid.fag2.lengthInfo;
                        plDB2Textbox.Text = currentPLLipid.fag2.dbInfo;
                        plHydroxyl2Textbox.Text = currentPLLipid.fag2.hydroxylInfo;
                        plFA2Combobox.SelectedIndex = currentPLLipid.fag2.chainType;
                        plFA2Checkbox1.Checked = currentPLLipid.fag2.faTypes["FA"];
                        plFA2Checkbox2.Checked = currentPLLipid.fag2.faTypes["FAp"];
                        plFA2Checkbox3.Checked = currentPLLipid.fag2.faTypes["FAa"];
                        
                        clFA3Textbox.Text = currentPLLipid.fag3.lengthInfo;
                        clDB3Textbox.Text = currentPLLipid.fag3.dbInfo;
                        clHydroxyl3Textbox.Text = currentPLLipid.fag3.hydroxylInfo;
                        clFA3Combobox.SelectedIndex = currentPLLipid.fag3.chainType;
                        clFA3Checkbox1.Checked = currentPLLipid.fag3.faTypes["FA"];
                        clFA3Checkbox2.Checked = currentPLLipid.fag3.faTypes["FAp"];
                        clFA3Checkbox3.Checked = currentPLLipid.fag3.faTypes["FAa"];
                        
                        clFA4Textbox.Text = currentPLLipid.fag4.lengthInfo;
                        clDB4Textbox.Text = currentPLLipid.fag4.dbInfo;
                        clHydroxyl4Textbox.Text = currentPLLipid.fag4.hydroxylInfo;
                        clFA4Combobox.SelectedIndex = currentPLLipid.fag4.chainType;
                        clFA4Checkbox1.Checked = currentPLLipid.fag4.faTypes["FA"];
                        clFA4Checkbox2.Checked = currentPLLipid.fag4.faTypes["FAp"];
                        clFA4Checkbox3.Checked = currentPLLipid.fag4.faTypes["FAa"];
                        
                        
                        plPosAdductCheckbox1.Checked = currentPLLipid.adducts["+H"];
                        plPosAdductCheckbox2.Checked = currentPLLipid.adducts["+2H"];
                        plPosAdductCheckbox3.Checked = currentPLLipid.adducts["+NH4"];
                        plNegAdductCheckbox1.Checked = currentPLLipid.adducts["-H"];
                        plNegAdductCheckbox2.Checked = currentPLLipid.adducts["-2H"];
                        plNegAdductCheckbox3.Checked = currentPLLipid.adducts["+HCOO"];
                        plNegAdductCheckbox4.Checked = currentPLLipid.adducts["+CH3COO"];
                        addLipidButton.Text = "Add cardiolipins";
                        
                        plIsCL.Checked = true;
                        
                        updateRanges(currentPLLipid.fag1, plFA1Textbox, plFA1Combobox.SelectedIndex);
                        updateRanges(currentPLLipid.fag1, plDB1Textbox, 3);
                        updateRanges(currentPLLipid.fag1, plHydroxyl1Textbox, 4);
                        updateRanges(currentPLLipid.fag2, plFA2Textbox, plFA2Combobox.SelectedIndex);
                        updateRanges(currentPLLipid.fag2, plDB2Textbox, 3);
                        updateRanges(currentPLLipid.fag2, plHydroxyl2Textbox, 4);
                        updateRanges(currentPLLipid.fag3, clFA3Textbox, clFA3Combobox.SelectedIndex);
                        updateRanges(currentPLLipid.fag3, clDB3Textbox, 3);
                        updateRanges(currentPLLipid.fag3, clHydroxyl3Textbox, 4);
                        updateRanges(currentPLLipid.fag4, clFA4Textbox, clFA4Combobox.SelectedIndex);
                        updateRanges(currentPLLipid.fag4, clDB4Textbox, 3);
                        updateRanges(currentPLLipid.fag4, clHydroxyl4Textbox, 4);
                        
                        plRepresentativeFA.Checked = currentPLLipid.representativeFA;
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
                        
                        plChangeLyso(currentPLLipid.isLyso);
                        
                        
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
                        plFA1Checkbox3.Checked = currentPLLipid.fag1.faTypes["FAa"];
                        
                        plFA2Textbox.Text = currentPLLipid.fag2.lengthInfo;
                        plDB2Textbox.Text = currentPLLipid.fag2.dbInfo;
                        plHydroxyl2Textbox.Text = currentPLLipid.fag2.hydroxylInfo;
                        plFA2Combobox.SelectedIndex = currentPLLipid.fag2.chainType;
                        plFA2Checkbox1.Checked = currentPLLipid.fag2.faTypes["FA"];
                        plFA2Checkbox2.Checked = currentPLLipid.fag2.faTypes["FAp"];
                        plFA2Checkbox3.Checked = currentPLLipid.fag2.faTypes["FAa"];
                    
                        
                        
                        plPosAdductCheckbox1.Checked = currentPLLipid.adducts["+H"];
                        plPosAdductCheckbox2.Checked = currentPLLipid.adducts["+2H"];
                        plPosAdductCheckbox3.Checked = currentPLLipid.adducts["+NH4"];
                        plNegAdductCheckbox1.Checked = currentPLLipid.adducts["-H"];
                        plNegAdductCheckbox2.Checked = currentPLLipid.adducts["-2H"];
                        plNegAdductCheckbox3.Checked = currentPLLipid.adducts["+HCOO"];
                        plNegAdductCheckbox4.Checked = currentPLLipid.adducts["+CH3COO"];
                        addLipidButton.Text = "Add phospholipids";
                        
                        
                        updateRanges(currentPLLipid.fag1, plFA1Textbox, plFA1Combobox.SelectedIndex);
                        updateRanges(currentPLLipid.fag1, plDB1Textbox, 3);
                        updateRanges(currentPLLipid.fag1, plHydroxyl1Textbox, 4);
                        if (!currentPLLipid.isLyso)
                        {
                            updateRanges(currentPLLipid.fag2, plFA2Textbox, plFA2Combobox.SelectedIndex);
                            updateRanges(currentPLLipid.fag2, plDB2Textbox, 3);
                            updateRanges(currentPLLipid.fag2, plHydroxyl2Textbox, 4);
                        }
                        plRepresentativeFA.Checked = currentPLLipid.representativeFA;
                    }
                    break;
                    
                case LipidCategory.SphingoLipid:
                    SLLipid currentSLLipid = (SLLipid)currentLipid;
                    
                    slIsLyso.Checked = currentSLLipid.isLyso;
                    slRegular.Checked = !currentSLLipid.isLyso;
                    slChangeLyso(currentSLLipid.isLyso);
                    
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
                    if (!currentSLLipid.isLyso) slFAHydroxyCombobox.SelectedIndex = currentSLLipid.fag.hydroxylCounts.First();
                    
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
            ((PLLipid)currentLipid).fag3.faTypes["FAa"] = ((CheckBox)sender).Checked;
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
            ((PLLipid)currentLipid).fag4.faTypes["FAa"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag4.faTypes["FAx"] = !((PLLipid)currentLipid).fag4.anyFAChecked();
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
            updateRanges(((PLLipid)currentLipid).fag2, plFA2Textbox, plFA2Combobox.SelectedIndex);
            updateRanges(((PLLipid)currentLipid).fag3, clFA3Textbox, clFA3Combobox.SelectedIndex);
            updateRanges(((PLLipid)currentLipid).fag4, clFA4Textbox, clFA4Combobox.SelectedIndex);
            updateRanges(((PLLipid)currentLipid).fag2, plDB2Textbox, 3);
            updateRanges(((PLLipid)currentLipid).fag3, clDB3Textbox, 3);
            updateRanges(((PLLipid)currentLipid).fag4, clDB4Textbox, 3);
            updateRanges(((PLLipid)currentLipid).fag2, plHydroxyl2Textbox, 4);
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
                if (((GLLipid)currentLipid).fag2.anyFAChecked()) glFA2Checkbox1.Checked = ((CheckBox)sender).Checked;
                if (((GLLipid)currentLipid).fag3.anyFAChecked()) glFA3Checkbox1.Checked =  ((CheckBox)sender).Checked;
            }
        }
        
        
        
        
        public void glFA1Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((GLLipid)currentLipid).fag1.faTypes["FAx"] = !((GLLipid)currentLipid).fag1.anyFAChecked();
            if (((GLLipid)currentLipid).representativeFA)
            {
                if (((GLLipid)currentLipid).fag2.anyFAChecked()) glFA2Checkbox2.Checked = ((CheckBox)sender).Checked;
                if (((GLLipid)currentLipid).fag3.anyFAChecked()) glFA3Checkbox2.Checked = ((CheckBox)sender).Checked;
            }
        }
        
        
        
        
        
        public void glFA1Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag1.faTypes["FAa"] = ((CheckBox)sender).Checked;
            ((GLLipid)currentLipid).fag1.faTypes["FAx"] = !((GLLipid)currentLipid).fag1.anyFAChecked();
            if (((GLLipid)currentLipid).representativeFA)
            {
                if (((GLLipid)currentLipid).fag2.anyFAChecked()) glFA2Checkbox3.Checked = ((CheckBox)sender).Checked;
                if (((GLLipid)currentLipid).fag3.anyFAChecked()) glFA3Checkbox3.Checked = ((CheckBox)sender).Checked;
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
            ((GLLipid)currentLipid).fag2.faTypes["FAa"] = ((CheckBox)sender).Checked;
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
            ((GLLipid)currentLipid).fag3.faTypes["FAa"] = ((CheckBox)sender).Checked;
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
            if (!((PLLipid)currentLipid).isCL)
            {
                easterText.Left = Width + 20;
                easterEggMilliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                easterText.Visible = true;
                this.timerEasterEgg.Enabled = true;
                Enabled = false;
            }
        }
        
        
        
        
        public void sugarHeady(Object sender, EventArgs e)
        {
            MessageBox.Show("Who is your sugar heady?");
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
                if (((GLLipid)currentLipid).fag2.anyFAChecked()) glFA2Checkbox1.Checked = glFA1Checkbox1.Checked;
                if (((GLLipid)currentLipid).fag3.anyFAChecked()) glFA3Checkbox1.Checked = glFA1Checkbox1.Checked;
                if (((GLLipid)currentLipid).fag2.anyFAChecked()) glFA2Checkbox2.Checked = glFA1Checkbox2.Checked;
                if (((GLLipid)currentLipid).fag3.anyFAChecked()) glFA3Checkbox2.Checked = glFA1Checkbox2.Checked;
                if (((GLLipid)currentLipid).fag2.anyFAChecked()) glFA2Checkbox3.Checked = glFA1Checkbox3.Checked;
                if (((GLLipid)currentLipid).fag3.anyFAChecked()) glFA3Checkbox3.Checked = glFA1Checkbox3.Checked;
                
                
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
                if (plIsCL.Checked)
                {
                    clFA3Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                    clFA4Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                }
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
                if (plIsCL.Checked)
                {
                    clFA3Textbox.Text = ((TextBox)sender).Text;
                    clFA4Textbox.Text = ((TextBox)sender).Text;
                }
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
                if (plIsCL.Checked)
                {
                    clDB3Textbox.Text = ((TextBox)sender).Text;
                    clDB4Textbox.Text = ((TextBox)sender).Text;
                }
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
                if (plIsCL.Checked)
                {
                    clHydroxyl3Textbox.Text = ((TextBox)sender).Text;
                    clHydroxyl4Textbox.Text = ((TextBox)sender).Text;
                }
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
                if (plIsCL.Checked)
                {
                    clFA3Checkbox1.Checked = ((CheckBox)sender).Checked;
                    clFA4Checkbox1.Checked = ((CheckBox)sender).Checked;
                }
            }
        }
        
        
        
        
        public void plFA1Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag1.faTypes["FAx"] = !((PLLipid)currentLipid).fag1.anyFAChecked();
            if (((PLLipid)currentLipid).representativeFA)
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
            ((PLLipid)currentLipid).fag1.faTypes["FAa"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag1.faTypes["FAx"] = !((PLLipid)currentLipid).fag1.anyFAChecked();
            if (((PLLipid)currentLipid).representativeFA)
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
            ((PLLipid)currentLipid).fag2.faTypes["FAa"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag2.faTypes["FAx"] = !((PLLipid)currentLipid).fag2.anyFAChecked();
        }
        
        
        
        
        public void plTypeCheckedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).isCL = plIsCL.Checked;
            ((PLLipid)currentLipid).isLyso = plIsLyso.Checked;

            changeTab((int)LipidCategory.PhosphoLipid);
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
            ((PLLipid)currentLipid).representativeFA = ((CheckBox)sender).Checked;
            if (((PLLipid)currentLipid).representativeFA)
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
            updateRanges(((PLLipid)currentLipid).fag2, plFA2Textbox, plFA2Combobox.SelectedIndex);
            updateRanges(((PLLipid)currentLipid).fag2, plDB2Textbox, 3);
            updateRanges(((PLLipid)currentLipid).fag2, plHydroxyl2Textbox, 4);
            
            
            if (plIsCL.Checked)
            {
                updateRanges(((PLLipid)currentLipid).fag3, clFA3Textbox, clFA3Combobox.SelectedIndex);
                updateRanges(((PLLipid)currentLipid).fag3, clDB3Textbox, 3);
                updateRanges(((PLLipid)currentLipid).fag3, clHydroxyl3Textbox, 4);
                
                updateRanges(((PLLipid)currentLipid).fag4, clFA4Textbox, clFA4Combobox.SelectedIndex);
                updateRanges(((PLLipid)currentLipid).fag4, clDB4Textbox, 3);
                updateRanges(((PLLipid)currentLipid).fag4, clHydroxyl4Textbox, 4);
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
                foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.PhosphoLipid])
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
                foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.PhosphoLipid])
                {
                    if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].attributes.Contains("heavy") && !lipidCreator.headgroups[headgroup].attributes.Contains("ether") && !lipidCreator.headgroups[headgroup].attributes.Contains("lyso") && !headgroup.Equals("CL") && !headgroup.Equals("MLCL")) plHgList.Add(headgroup);
                }
                plPictureBox.Left = 107;
                plPictureBox.Image = phosphoBackboneImage;
            }
            plPictureBox.SendToBack();
            plHgList.Sort();
            plHgListbox.Items.AddRange(plHgList.ToArray());
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
        
        
        
        void slIsLysoCheckedChanged(object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).isLyso = slIsLyso.Checked;
            slChangeLyso(slIsLyso.Checked);
        }
        
        
        
        void slChangeLyso(bool lyso)
        {
            
            List<String> slHgList = new List<String>();
            slHgListbox.Items.Clear();
            
            
            if (lyso)
            {
                foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.SphingoLipid])
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
                foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.SphingoLipid])
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
            int rowIndex = lipidModifications[(int)result];
            switch (result)
            {
                case LipidCategory.GlyceroLipid:
                    lipidCreator.registeredLipids[rowIndex] = new GLLipid((GLLipid)currentLipid);
                    break;
                case LipidCategory.PhosphoLipid:
                    lipidCreator.registeredLipids[rowIndex] = new PLLipid((PLLipid)currentLipid);
                    break;
                    
                case LipidCategory.SphingoLipid:
                    lipidCreator.registeredLipids[rowIndex] = new SLLipid((SLLipid)currentLipid);
                    break;
                    
                case LipidCategory.Cholesterol:
                    lipidCreator.registeredLipids[rowIndex] = new Cholesterol((Cholesterol)currentLipid);
                    break;
                    
                case LipidCategory.Mediator:
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
                case LipidCategory.GlyceroLipid:
                    lipidCreator.registeredLipids.Add(new GLLipid((GLLipid)currentLipid));
                    registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                    tabIndex = (int)LipidCategory.GlyceroLipid;
                    break;
                    
                case LipidCategory.PhosphoLipid:
                    lipidCreator.registeredLipids.Add(new PLLipid((PLLipid)currentLipid));
                    registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                    tabIndex = (int)LipidCategory.PhosphoLipid;
                    break;
                    
                case LipidCategory.SphingoLipid:
                    lipidCreator.registeredLipids.Add(new SLLipid((SLLipid)currentLipid));
                    registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                    tabIndex = (int)LipidCategory.SphingoLipid;
                    break;
                    
                case LipidCategory.Cholesterol:
                    lipidCreator.registeredLipids.Add(new Cholesterol((Cholesterol)currentLipid));
                    registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                    tabIndex = (int)LipidCategory.Cholesterol;
                    break;
                    
                case LipidCategory.Mediator:
                    lipidCreator.registeredLipids.Add(new Mediator((Mediator)currentLipid));
                    registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                    tabIndex = (int)LipidCategory.Mediator;
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
            if (currentRegisteredLipid is GLLipid)
            {
                GLLipid currentGLLipid = (GLLipid)currentRegisteredLipid;
                row["Category"] = "Glycerolipid";
                row["Building Block 1"] = FARepresentation(currentGLLipid.fag1) + currentGLLipid.fag1.lengthInfo + "; DB: " + currentGLLipid.fag1.dbInfo + "; OH: " + currentGLLipid.fag1.hydroxylInfo;
                if (!currentGLLipid.fag2.faTypes["FAx"]) row["Building Block 2"] = FARepresentation(currentGLLipid.fag2) + currentGLLipid.fag2.lengthInfo + "; DB: " + currentGLLipid.fag2.dbInfo + "; OH: " + currentGLLipid.fag2.hydroxylInfo;
                if (currentGLLipid.containsSugar)
                {
                    row["Building Block 3"] = "HG: " + String.Join(", ", currentGLLipid.headGroupNames);
                }
                else
                {
                    if (!currentGLLipid.fag3.faTypes["FAx"]) row["Building Block 3"] = FARepresentation(currentGLLipid.fag3) + currentGLLipid.fag3.lengthInfo + "; DB: " + currentGLLipid.fag3.dbInfo + "; OH: " + currentGLLipid.fag3.hydroxylInfo;
                }
            }
            else if (currentRegisteredLipid is PLLipid)
            {
                PLLipid currentPLLipid = (PLLipid)currentRegisteredLipid;
                if (currentPLLipid.isCL)
                {
                    row["Category"] = "Cardiolipin";
                    row["Building Block 1"] = FARepresentation(currentPLLipid.fag1) + currentPLLipid.fag1.lengthInfo + "; DB: " + currentPLLipid.fag1.dbInfo + "; OH: " + currentPLLipid.fag1.hydroxylInfo;
                    row["Building Block 2"] = FARepresentation(currentPLLipid.fag2) + currentPLLipid.fag2.lengthInfo + "; DB: " + currentPLLipid.fag2.dbInfo + "; OH: " + currentPLLipid.fag2.hydroxylInfo;
                    row["Building Block 3"] = FARepresentation(currentPLLipid.fag3) + currentPLLipid.fag3.lengthInfo + "; DB: " + currentPLLipid.fag3.dbInfo + "; OH: " + currentPLLipid.fag3.hydroxylInfo;
                    if (!currentPLLipid.fag4.faTypes["FAx"]) row["Building Block 4"] = FARepresentation(currentPLLipid.fag4) + currentPLLipid.fag4.lengthInfo + "; DB: " + currentPLLipid.fag4.dbInfo + "; OH: " + currentPLLipid.fag4.hydroxylInfo;
                }
                else
                {
                    row["Category"] = "Phospholipid";
                    row["Building Block 1"] = "HG: " + String.Join(", ", currentPLLipid.headGroupNames);
                    row["Building Block 2"] = FARepresentation(currentPLLipid.fag1) + currentPLLipid.fag1.lengthInfo + "; DB: " + currentPLLipid.fag1.dbInfo + "; OH: " + currentPLLipid.fag1.hydroxylInfo;
                    if (!currentPLLipid.isLyso) row["Building Block 3"] = FARepresentation(currentPLLipid.fag2) + currentPLLipid.fag2.lengthInfo + "; DB: " + currentPLLipid.fag2.dbInfo + "; OH: " + currentPLLipid.fag2.hydroxylInfo;
                }
            }
            else if (currentRegisteredLipid is SLLipid)
            {
                SLLipid currentSLLipid = (SLLipid)currentRegisteredLipid;
                row["Category"] = "Sphingolipid";
                row["Building Block 1"] = "HG: " + String.Join(", ", currentSLLipid.headGroupNames);
                row["Building Block 2"] = "LCB: " + currentSLLipid.lcb.lengthInfo + "; DB: " + currentSLLipid.lcb.dbInfo + "; OH: " + currentSLLipid.lcb.hydroxylCounts.First();
                if (!currentSLLipid.isLyso) row["Building Block 3"] = "FA: " + currentSLLipid.fag.lengthInfo + "; DB: " + currentSLLipid.fag.dbInfo + "; OH: " + currentSLLipid.fag.hydroxylCounts.First();
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
            if (currentRegisteredLipid is GLLipid) tabIndex = (int)LipidCategory.GlyceroLipid;
            else if (currentRegisteredLipid is PLLipid) tabIndex = (int)LipidCategory.PhosphoLipid;
            else if (currentRegisteredLipid is SLLipid) tabIndex = (int)LipidCategory.SphingoLipid;
            else if (currentRegisteredLipid is Cholesterol) tabIndex = (int)LipidCategory.Cholesterol;
            else if (currentRegisteredLipid is Mediator) tabIndex = (int)LipidCategory.Mediator;
            
            DataTable tmpTable = registeredLipidsDatatable.Clone();
            for (int i = 0; i < registeredLipidsDatatable.Rows.Count; ++i)
            {
                if (i != rowIndex) tmpTable.ImportRow(registeredLipidsDatatable.Rows[i]);
            }
            registeredLipidsDatatable.Rows.Clear();
            
            foreach (DataRow row in tmpTable.Rows) registeredLipidsDatatable.ImportRow(row);
            
            if ((int)lipidModifications[tabIndex] == rowIndex) lipidModifications[tabIndex] = -1;
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
            
                menuCollisionEnergyOpt.Enabled = true;
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
                
                case ((int)LipidCategory.Mediator):
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
            lipidCreator.assembleLipids(asDeveloper);
            lipidCreator.analytics("lipidcreator", "create-transition-list");
            lipidsReview = new LipidsReview(this);
            lipidsReview.Owner = this;
            lipidsReview.ShowInTaskbar = false;
            
            if (tutorial.tutorial == Tutorials.NoTutorial)
            {
                lipidsReview.ShowDialog();
                lipidsReview.Dispose();
            }
            else
            {
                lipidsReview.Show();
            }
              
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
                    updateCECondition();
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
        
        
        
        
        protected void menuCollisionEnergyOptClick(object sender, System.EventArgs e)
        {
            // TODO: after testing, delete this lines
            CEInspector ceInspector = new CEInspector(this, lipidCreator.selectedInstrumentForCE);
            ceInspector.Owner = this;
            ceInspector.ShowInTaskbar = false;
            ceInspector.ShowDialog();
            ceInspector.Dispose();
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
        
        
        
        protected void menuTranslateClick(object sender, System.EventArgs e)
        {
            TranslatorDialog translatorDialog = new TranslatorDialog(this);
            translatorDialog.Owner = this;
            translatorDialog.ShowInTaskbar = false;
            translatorDialog.ShowDialog ();
            translatorDialog.Dispose ();
        }
        

        
        protected void menuAboutClick(object sender, System.EventArgs e)
        {
            AboutDialog aboutDialog = new AboutDialog ();
            aboutDialog.Owner = this;
            aboutDialog.ShowInTaskbar = false;
            aboutDialog.ShowDialog ();
            aboutDialog.Dispose ();
        }
        
        
        
        
        public static void printHelp(string option = "")
        {
            LipidCreator lc = new LipidCreator(null);
            switch (option)
            {
                case "transitionlist":
                    Console.WriteLine("Creating a transition list from a lipid list");
                    Console.WriteLine();
                    Console.WriteLine("usage: LipidCreator.exe transitionlist input_csv output_csv [opts [opts ...]]");
                    Console.WriteLine("  opts are:");
                    Console.WriteLine("    -p 0:\t\tCompute no precursor transitions");
                    Console.WriteLine("    -p 1:\t\tCompute only precursor transitions");
                    Console.WriteLine("    -p 2:\t\tCompute with precursor transitions");
                    Console.WriteLine("    -h 0:\t\tCompute no heavy labeled isotopes");
                    Console.WriteLine("    -h 1:\t\tCompute only heavy labeled isotopes");
                    Console.WriteLine("    -h 2:\t\tCompute with heavy labeled isotopes");
                    Console.WriteLine("    -s:\t\t\tSplit in positive and negative list");
                    Console.WriteLine("    -x:\t\t\tDeveloper or Xpert mode");
                    Console.WriteLine("    -d:\t\t\tDelete replicate transitions (equal precursor and fragment mass)");
                    Console.WriteLine("    -c instrument mode:\tCompute with optimal collision energy (not available for all lipid classes)");
                    Console.WriteLine("      available instruments and modes:");
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
                            Console.WriteLine("        '" + kvp.Key + "': " + fullInstrumentName + " " + modes);
                        }
                    }
                    break;
                    
                    
                case "library":
                    Console.WriteLine("Creating a spectral library in *.blib format from a lipid list");
                    Console.WriteLine();
                    Console.WriteLine("usage: LipidCreator.exe transitionlist input_csv output_csv instrument");
                    Console.WriteLine("  available instruments:");
                    foreach (KeyValuePair<string, InstrumentData> kvp in lc.msInstruments)
                    {
                        if (kvp.Value.minCE > 0) 
                        {
                            string fullInstrumentName = kvp.Value.model;
                            Console.WriteLine("    '" + kvp.Key + "': " + fullInstrumentName);
                        }
                    }
                    break;
                    
                    
                    
                case "translate":
                    Console.WriteLine("Translating a list with old lipid names into current nomenclature");
                    Console.WriteLine();
                    Console.WriteLine("usage: LipidCreator.exe translate input_csv output_csv");
                    break;
                    
                    
                    
                case "random":
                    Console.WriteLine("Generating a random lipid name (not necessarily reasonable in terms of chemistry)");
                    Console.WriteLine();
                    Console.WriteLine("usage: LipidCreator.exe random [number]");
                    break;
                    
                    
                case "spymode":
                    Console.WriteLine("\nUnsaturated fatty acids contain one special bond - James Bond!\n\n");
                    break;
                    
                    
                default:
                    Console.WriteLine("usage: LipidCreator.exe (option)");
                    Console.WriteLine();
                    Console.WriteLine("options are:");
                    Console.WriteLine("  dev:\t\t\t\tlaunching LipidCreator as developer");
                    Console.WriteLine("  transitionlist:\t\tcreating transition list from lipid list");
                    Console.WriteLine("  translate:\t\t\ttranslating a list with old lipid names into current nomenclature");
                    Console.WriteLine("  library:\t\t\tcreating a spectral library in *.blib format from a lipid list");
                    Console.WriteLine("  random:\t\t\tgenerating a random lipid name (not necessarily reasonable in terms of chemistry)");
                    Console.WriteLine("  spymode:\t\t\tsecret spy mode");
                    Console.WriteLine("  help:\t\t\t\tprint this help");
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
                        outputFile.Dispose ();
                        outputFile.Close ();
                    }
                }
            }
            catch(Exception e) {
                
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
                            outputFile.Dispose ();
                            outputFile.Close ();
                        }
                    }
                }
            }
            catch(Exception e) {
                
            }
        }
        
        
        
    
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
        
                if ((new HashSet<string>{"external", "dev", "help", "transitionlist", "library", "random", "spymode", "translate"}).Contains(args[0]))
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
                            
                            
                        case "spymode":
                            printHelp("spymode");
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
                                        Console.WriteLine("The file '" + inputCSV + "' in line '" + lineCounter + "' could not be read:");
                                        Console.WriteLine(e.Message);
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
                                
                                lc.importLipidList(inputCSV);
                                foreach(Lipid lipid in lc.registeredLipids)
                                {
                                    lipid.onlyPrecursors = parameterPrecursor;
                                    lipid.onlyHeavyLabeled = parameterHeavy;
                                }
                                
                                MonitoringTypes monitoringType = MonitoringTypes.NoMonitoring;
                                if (mode == "PRM") monitoringType = MonitoringTypes.PRM;
                                else if (mode == "SRM") monitoringType = MonitoringTypes.SRM;
                                
                                lc.selectedInstrumentForCE = instrument;
                                lc.monitoringType = monitoringType;
                                lc.assembleLipids(asDeveloper); 
                                DataTable transitionList = deleteReplicates ? lc.transitionListUnique : lc.transitionList;
                                lc.storeTransitionList(",", split, outputCSV, transitionList);
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
