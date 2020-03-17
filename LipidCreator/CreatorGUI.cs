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
using System.Threading;
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
using ExtensionMethods;

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
        public ulong[] lipidModifications;
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
        public static float FONT_SIZE_FACTOR;
        public static readonly float REGULAR_FONT_SIZE = 8.25f;
        public bool lipidCreatorInitError = false;
        public bool openedAsExternal = false;
        public string prefixPath = "";
        
        public CreatorGUI(string _inputParameters)
        {
        
            FONT_SIZE_FACTOR = 96.0f / CreateGraphics().DpiX;
            inputParameters = _inputParameters;
            openedAsExternal = (inputParameters != null);
            prefixPath = (openedAsExternal ? LipidCreator.EXTERNAL_PREFIX_PATH : "");
            
            try
            {
                this.lipidCreator = new LipidCreator(inputParameters, true);
                currentIndex = LipidCategory.NoLipid;
                resetAllLipids();
            }
            catch
            {
                lipidCreatorInitError = true;
                MessageBox.Show ("An error occurred during the initialization of LipidCreator. For more details, please read the log message (Menu -> Help -> Log messages) and get in contact with the developers.", "LipidCreator: error occurred");
            }
            
            
            registeredLipidsDatatable = new DataTable("Daten");
            registeredLipidsDatatable.Columns.Add(new DataColumn("Category"));
            registeredLipidsDatatable.Columns.Add(new DataColumn("Building Block 1"));
            registeredLipidsDatatable.Columns.Add(new DataColumn("Building Block 2"));
            registeredLipidsDatatable.Columns.Add(new DataColumn("Building Block 3"));
            registeredLipidsDatatable.Columns.Add(new DataColumn("Building Block 4"));
            registeredLipidsDatatable.Columns.Add(new DataColumn("Adducts"));
            registeredLipidsDatatable.Columns.Add(new DataColumn("Filters"));
            registeredLipidsDatatable.Columns.Add(new DataColumn("Options"));
        
            InitializeComponent();
            
            
            if (!lipidCreatorInitError)
            {
            
                // add predefined menu
                lipidModifications = Enumerable.Repeat(0UL, Enum.GetNames(typeof(LipidCategory)).Length).ToArray();
                changingTabForced = false;
                string predefinedFolder = Path.Combine(lipidCreator.prefixPath, "data", "predefined");
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
                            if (upperFile.EndsWith(".CSV"))
                            {
                                System.Windows.Forms.MenuItem predefFile = new System.Windows.Forms.MenuItem();
                                predefFile.Text = subFile.Remove(subFile.Length - 4);
                                predefFile.Tag = subdirectoryFile;
                                predefFile.Click += new System.EventHandler (menuImportPredefinedClick);
                                predefFolder.MenuItems.Add(predefFile);
                            }
                        }
                    }
                }
                tabList = new ArrayList(new TabPage[] {homeTab, glycerolipidsTab, phospholipidsTab, sphingolipidsTab, cholesterollipidsTab, mediatorlipidsTab});
                if (!lipidCreatorInitError) tutorial = new Tutorial(this);
                
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
                                    log.Error("Error: monitoring mode '" + instrumentMode + "' not supported for instrument '" + lipidCreator.msInstruments[instrument].model + "'");
                                    break;
                            }
                            menuCollisionEnergy.MenuItems.Add(instrumentItem);
                        }
                    }
                }
                lastCEInstrumentChecked = menuCollisionEnergyNone;
            }
            else
            {
                menuFile.Enabled = false;
                menuOptions.Enabled = false;
                tabControl.Enabled = false;
                openReviewFormButton.Enabled = false;
                lipidsGridview.Enabled = false;
            }
        }
        
        
        
        
        
        
        public void resetLipidCreatorMenu(Object sender, EventArgs e)
        {
            resetLipidCreator();
        }
        
        
        
        
        
        
        
        public void statisticsMenu(Object sender, EventArgs e)
        {
            menuStatistics.Checked = !menuStatistics.Checked;
            lipidCreator.enableAnalytics = menuStatistics.Checked;
            string analyticsFile = Path.Combine(lipidCreator.prefixPath, "data", "analytics.txt");
            try {
                using (StreamWriter outputFile = new StreamWriter (analyticsFile))
                {
                    outputFile.WriteLine ((lipidCreator.enableAnalytics ? "1" : "0"));
                }
            }
            catch (Exception ex)
            {
                log.Warn("Could not write to '"+ analyticsFile + "': " + ex);
            }
        }
        
        
        
        
        
        

        public void toolDirectoryMenu(Object sender, EventArgs e)
        {
            string dataDir = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase));
            openDirectory(dataDir);
        }
        
        
        
        
        
        private void openDirectory(string dataDir)
        {
            log.Debug("LipidCreator dir to open is " + dataDir);

            try
            {
                log.Debug("Opening directory " + System.IO.Path.GetDirectoryName(dataDir));
                int p = (int)Environment.OSVersion.Platform;
                if ((p == 4) || (p == 6) || (p == 128))
                {
                    log.Debug("Running on Linux");
                    string openCmd = "xdg-open";
                    Process process = System.Diagnostics.Process.Start(openCmd, dataDir);
                    process?.WaitForExit();
                    log.Debug("Finished starting process '" + openCmd + " " + dataDir + "' with code " + process.ExitCode);
                }
                else
                {
                    log.Debug("Running on Windows");
                    Process process = System.Diagnostics.Process.Start(dataDir);
                    process?.WaitForExit();
                    log.Debug("Finished starting process '" + dataDir);
                }
            }
            catch (Exception exception)
            {
                log.Error("Error while opening dataDir folder " + dataDir + ":", exception);
                log.Error(exception.StackTrace);
            }
        }
        
        
        
        
        
        public void clearLipidList(Object sender, EventArgs e)
        {
            lipidCreator.registeredLipidDictionary.Clear();
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
                lipidCreatorInitError = false;
                try
                {
                    lipidCreator = new LipidCreator(inputParameters);
                }
                catch
                {
                    lipidCreatorInitError = true;
                    MessageBox.Show ("An error occurred during the initialization of LipidCreator. For more details, please read the log message (Menu -> Help -> Log messages) and contact the developers.", "LipidCreator: error occurred");
                }
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
            lipidsGridview.InvokeIfRequired(() =>
            {
                 if (initialCall)
                 {
                    log.Debug("Initializing lipid table!");
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
                    foreach (DataGridViewColumn col in lipidsGridview.Columns)
                    {
                        col.Frozen = false;
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    lipidsGridview.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    editColumn.Width = 40;
                    deleteColumn.Width = 40;
                    initialCall = false;
                    lipidsGridview.Enabled = true;
                    lipidsGridview.Invalidate();
                    lipidsGridview.PerformLayout();
                 }
            });
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
                        plHasPlasmalogen.Checked = false;
                        
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
                        plHasPlasmalogen.Checked = currentPhospholipid.hasPlasmalogen;
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
                modifyLipidButton.Enabled = lipidModifications[(int)currentIndex] != 0;
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
            lipidModifications[index] = 0;
            changeTab(index);
        }
        
        
        
        
        
        /////////////////////// CL //////////////////////////////
        
        
        
        
        
        
        // objectType (Object type): 0 = carbon length, 1 = carbon length odd, 2 = carbon length even, 3 = db length, 4 = hydroxyl length
        public void updateRanges(FattyAcidGroup fag, TextBox tb, int objectTypeI, bool isLCB = false)
        {
            ChainType objectType = (ChainType)objectTypeI;
            int minRange = 0, maxRange = 0;
            bool incorrectParsing = false;
            if (objectType <= ChainType.carbonLengthEven)
            {
                minRange = LipidCreator.MIN_CARBON_LENGTH;
                maxRange = LipidCreator.MAX_CARBON_LENGTH;
                fag.carbonCounts = LipidCreator.parseRange(tb.Text, minRange,  maxRange, objectType);
                if (fag.carbonCounts == null)
                {
                    incorrectParsing = true;
                    fag.carbonCounts = new HashSet<int>();
                }
            }
            else if (objectType == ChainType.dbLength)
            {
                minRange = LipidCreator.MIN_DB_LENGTH;
                maxRange = LipidCreator.MAX_DB_LENGTH;
                fag.doubleBondCounts = LipidCreator.parseRange(tb.Text, minRange,  maxRange, objectType);
                if (fag.doubleBondCounts == null)
                {
                    incorrectParsing = true;
                    fag.doubleBondCounts = new HashSet<int>();
                }
            }
            else if (objectType == ChainType.hydroxylLength)
            {
                minRange = LipidCreator.MIN_HYDROXY_LENGTH;
                maxRange = LipidCreator.MAX_HYDROXY_LENGTH;
                fag.hydroxylCounts = LipidCreator.parseRange(tb.Text, minRange,  maxRange, objectType);
                if (fag.hydroxylCounts == null)
                {
                    incorrectParsing = true;
                    fag.hydroxylCounts = new HashSet<int>();
                }
            }
            
            tb.BackColor = incorrectParsing ? alertColor : Color.White;
        }
        
        
        
        
        
        
        private void homeText3LinkClicked(Object sender, EventArgs e)
        {
            string url = "http://www.whateverjournal.com/doi/or/whatever";
            var si = new ProcessStartInfo(url);
            Process.Start(si);
        }
        
        
        
        
        
        
        
        public void updateCarbon(Object sender, FattyAcidEventArgs e)
        {
            e.fag.lengthInfo = ((TextBox)sender).Text;
            updateRanges(e.fag, (TextBox)sender, e.fag.chainType, e.fag.isLCB);
        }
        
        
        
        
        
        public void updateDB(Object sender, FattyAcidEventArgs e)
        {
            e.fag.dbInfo = ((TextBox)sender).Text;
            updateRanges(e.fag, (TextBox)sender, 3);
        }
        
        
        
        
        
        public void updateHydroxyl(Object sender, FattyAcidEventArgs e)
        {
            e.fag.hydroxylInfo = ((TextBox)sender).Text;
            updateRanges(e.fag, (TextBox)sender, 4);
        }
        
        
        
        
        public void updateOddEven(Object sender, FattyAcidEventArgs e)
        {
            e.fag.chainType = ((ComboBox)sender).SelectedIndex;
            updateRanges(e.fag, e.textbox, e.fag.chainType);
        }
        
        
        
        
        
        
        
        public void FattyAcidCheckboxCheckChanged(Object sender, FattyAcidEventArgs e)
        {
            if (sender == null || e.fag == null) return;
            e.fag.faTypes[e.fType] = ((CheckBox)sender).Checked;
            e.fag.faTypes["FAx"] = !e.fag.anyFAChecked();
        }
        
        
        
        
        
        public void AdductCheckBoxChecked(Object sender, AdductCheckedEventArgs e)
        {
            e.lipid.adducts[e.adduct] = ((CheckBox)sender).Checked;
        }
        
        
        
        
        public void updateGLRepresentative()
        {
            if (((Glycerolipid)currentLipid).representativeFA)
            {
                glFA2Combobox.SelectedIndex = glFA1Combobox.SelectedIndex;
                glFA3Combobox.SelectedIndex = glFA1Combobox.SelectedIndex;
                glFA2Textbox.Text = glFA1Textbox.Text;
                glFA3Textbox.Text = glFA1Textbox.Text;
                glDB2Textbox.Text = glDB1Textbox.Text;
                glDB3Textbox.Text = glDB1Textbox.Text;
                glHydroxyl2Textbox.Text = glHydroxyl1Textbox.Text;
                glHydroxyl3Textbox.Text = glHydroxyl1Textbox.Text;
                glFA2Checkbox1.Checked = glFA1Checkbox1.Checked;
                glFA2Checkbox2.Checked = glFA1Checkbox2.Checked;
                glFA2Checkbox3.Checked = glFA1Checkbox3.Checked;
                glFA3Checkbox1.Checked = glFA1Checkbox1.Checked;
                glFA3Checkbox2.Checked = glFA1Checkbox2.Checked;
                glFA3Checkbox3.Checked = glFA1Checkbox3.Checked;
            }
        }
        
        
        
        
        public void updatePLRepresentative()
        {
            if (((Phospholipid)currentLipid).representativeFA)
            {
                plFA2Combobox.SelectedIndex = plFA1Combobox.SelectedIndex;
                clFA3Combobox.SelectedIndex = plFA1Combobox.SelectedIndex;
                clFA4Combobox.SelectedIndex = plFA1Combobox.SelectedIndex;
                plFA2Textbox.Text = plFA1Textbox.Text;
                clFA3Textbox.Text = plFA1Textbox.Text;
                clFA4Textbox.Text = plFA1Textbox.Text;
                plDB2Textbox.Text = plDB1Textbox.Text;
                clDB3Textbox.Text = plDB1Textbox.Text;
                clDB4Textbox.Text = plDB1Textbox.Text;
                plHydroxyl2Textbox.Text = plHydroxyl1Textbox.Text;
                clHydroxyl3Textbox.Text = plHydroxyl1Textbox.Text;
                clHydroxyl4Textbox.Text = plHydroxyl1Textbox.Text;
                plFA2Checkbox1.Checked = plFA1Checkbox1.Checked;
                plFA2Checkbox2.Checked = plFA1Checkbox2.Checked;
                plFA2Checkbox3.Checked = plFA1Checkbox3.Checked;
                clFA3Checkbox1.Checked = plFA1Checkbox1.Checked;
                clFA3Checkbox2.Checked = plFA1Checkbox2.Checked;
                clFA3Checkbox3.Checked = plFA1Checkbox3.Checked;
                clFA4Checkbox1.Checked = plFA1Checkbox1.Checked;
                clFA4Checkbox2.Checked = plFA1Checkbox2.Checked;
                clFA4Checkbox3.Checked = plFA1Checkbox3.Checked;
            }
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
        
        
    
        
        
        
        
        
        public void plTypeCheckedChanged(Object sender, EventArgs e)
        {
            ((Phospholipid)currentLipid).isCL = plIsCL.Checked;
            ((Phospholipid)currentLipid).isLyso = plIsLyso.Checked;
            ((Phospholipid)currentLipid).hasPlasmalogen = plHasPlasmalogen.Checked;

            plRepresentativeFA.Visible = !((Phospholipid)currentLipid).isLyso;
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
                    
                plFA1Checkbox3.Visible = true;
                plFA1Checkbox2.Visible = true;
                plFA1Checkbox1.Visible = true;
                
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
                    plFA2Checkbox2.Checked = plFA1Checkbox2.Checked;
                    plFA2Checkbox3.Checked = plFA1Checkbox3.Checked;
                    
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
                if (plHasPlasmalogen.Checked)
                {
                    foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.Glycerophospholipid])
                    {
                        if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].attributes.Contains("heavy") && lipidCreator.headgroups[headgroup].attributes.Contains("ether") && lipidCreator.headgroups[headgroup].attributes.Contains("lyso")) plHgList.Add(headgroup);
                    }
                    plFA1Checkbox1.Checked = false;
                    plFA1Checkbox2.Checked = true;
                    plFA1Checkbox3.Checked = true;
                    
                    plFA1Checkbox3.Visible = false;
                    plFA1Checkbox2.Visible = false;
                    plFA1Checkbox1.Visible = false;
                    plPictureBox.Image = phosphoLysoBackboneImageFA1e;
                }
                else
                {
                    foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.Glycerophospholipid])
                    {
                        if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].attributes.Contains("heavy") && !lipidCreator.headgroups[headgroup].attributes.Contains("ether") && lipidCreator.headgroups[headgroup].attributes.Contains("lyso")) plHgList.Add(headgroup);
                    }
                    plFA1Checkbox3.Visible = true;
                    plFA1Checkbox2.Visible = true;
                    plFA1Checkbox1.Visible = true;
                    plPictureBox.Image = phosphoLysoBackboneImage;
                }
                plPictureBox.Left = 106;
                
                plFA2Combobox.Visible = false;
                plFA2Textbox.Visible = false;
                plDB2Textbox.Visible = false;
                plHydroxyl2Textbox.Visible = false;
                plDB2Label.Visible = false;
                plHydroxyl2Label.Visible = false;
                plFA2Checkbox1.Visible = false;
                plRepresentativeFA.Visible = false;
            }
            else
            {
                if (plHasPlasmalogen.Checked)
                {
                    foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.Glycerophospholipid])
                    {
                        if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].attributes.Contains("heavy") && lipidCreator.headgroups[headgroup].attributes.Contains("ether") && !lipidCreator.headgroups[headgroup].attributes.Contains("lyso") && !headgroup.Equals("CL") && !headgroup.Equals("MLCL")) plHgList.Add(headgroup);
                    }
                    plFA1Checkbox1.Checked = false;
                    plFA1Checkbox2.Checked = true;
                    plFA1Checkbox3.Checked = true;
                    
                    plFA1Checkbox3.Visible = false;
                    plFA1Checkbox2.Visible = false;
                    plFA1Checkbox1.Visible = false;
                    plPictureBox.Image = phosphoBackboneImageFA1e;
                }
                else
                {
                    foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.Glycerophospholipid])
                    {
                        if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].attributes.Contains("heavy") && !lipidCreator.headgroups[headgroup].attributes.Contains("ether") && !lipidCreator.headgroups[headgroup].attributes.Contains("lyso") && !headgroup.Equals("CL") && !headgroup.Equals("MLCL")) plHgList.Add(headgroup);
                    }
                    plFA1Checkbox3.Visible = true;
                    plFA1Checkbox2.Visible = true;
                    plFA1Checkbox1.Visible = true;
                    plPictureBox.Image = phosphoBackboneImage;
                }
                plPictureBox.Left = 107;
                plRepresentativeFA.Visible = true;
            }
            plPictureBox.SendToBack();
            plHgList.Sort();
            plHgListbox.Items.AddRange(plHgList.ToArray());
            enableDisableAdducts((int)currentIndex);
            
            
            
            
        }
        
        ////////////////////// SL ////////////////////////////////
        
        
        
        public void slLCBHydroxyComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).lcb.hydroxylCounts.Clear();
            ((Sphingolipid)currentLipid).lcb.hydroxylCounts.Add(((ComboBox)sender).SelectedIndex + 2);
            ((Sphingolipid)currentLipid).lcb.hydroxylInfo = (((ComboBox)sender).SelectedIndex + 2).ToString();
        }
        
        public void slFAHydroxyComboboxValueChanged(Object sender, EventArgs e)
        {
            ((Sphingolipid)currentLipid).fag.hydroxylCounts.Clear();
            ((Sphingolipid)currentLipid).fag.hydroxylCounts.Add(((ComboBox)sender).SelectedIndex);
            ((Sphingolipid)currentLipid).fag.hydroxylInfo = (((ComboBox)sender).SelectedIndex).ToString();
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
        
        
        
        ////////////////////// Mediators ////////////////////////////////
        
        
        
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
                    MessageBox.Show("Please select the middle fatty acid for DAG!", "Not registrable");
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
                    
                    
                    if (((Phospholipid)currentLipid).isLyso)
                    {
                        bool hasOneHG = false;
                        bool hasPlasmalogen = ((Phospholipid)currentLipid).hasPlasmalogen;
                        foreach (string headgroup in currentLipid.headGroupNames)
                        {
                            bool plasmalogenEquivalent = !(lipidCreator.headgroups[headgroup].attributes.Contains("ether") ^ hasPlasmalogen);
                            if (lipidCreator.headgroups[headgroup].attributes.Contains("lyso") && plasmalogenEquivalent)
                            {
                                hasOneHG = true;
                                break;
                            }
                        }
                        if (!hasOneHG)
                        {
                            MessageBox.Show("No head group selected!", "Not registrable");
                            return  LipidCategory.NoLipid;                    
                        }
                    }
                    else if (!((Phospholipid)currentLipid).isCL)
                    {
                        bool hasOneHG = false;
                        bool hasPlasmalogen = ((Phospholipid)currentLipid).hasPlasmalogen;
                        foreach (string headgroup in currentLipid.headGroupNames)
                        {
                            bool plasmalogenEquivalent = !(lipidCreator.headgroups[headgroup].attributes.Contains("ether") ^ hasPlasmalogen);
                            if (!lipidCreator.headgroups[headgroup].attributes.Contains("lyso") && plasmalogenEquivalent)
                            {
                                hasOneHG = true;
                                break;
                            }
                        }
                        if (!hasOneHG)
                        {
                            MessageBox.Show("No head group selected!", "Not registrable");
                            return  LipidCategory.NoLipid;                    
                        }                    
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
                    
                    if (plDB1Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("First double bond content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    if (plHydroxyl1Textbox.BackColor == alertColor)
                    {
                        MessageBox.Show("First hydroxyl content not valid!", "Not registrable");
                        return  LipidCategory.NoLipid;
                    }
                    
                    
                    if (!((Phospholipid)currentLipid).isLyso)
                    {
                        if (plFA2Textbox.BackColor == alertColor)
                        {
                            MessageBox.Show("Second fatty acid length content not valid!", "Not registrable");
                            return  LipidCategory.NoLipid;
                        }
                    
                        if (plDB2Textbox.BackColor == alertColor)
                        {
                            MessageBox.Show("Second double bond content not valid!", "Not registrable");
                            return  LipidCategory.NoLipid;
                        }
                    
                        if (plHydroxyl2Textbox.BackColor == alertColor)
                        {
                            MessageBox.Show("Second hydroxyl content not valid!", "Not registrable");
                            return  LipidCategory.NoLipid;
                        }
                    
                    }
                    
                    
                    
                    
                    
                    
                    
                    //
                    if ((((Phospholipid)currentLipid).fag1.faTypes["FAa"] || ((Phospholipid)currentLipid).fag1.faTypes["FAp"]) && !((Phospholipid)currentLipid).isCL  && !((Phospholipid)currentLipid).hasPlasmalogen)
                    {
                        HashSet<string> supposedPLs = ((Phospholipid)currentLipid).isLyso ? new HashSet<string>(){"LPC", "LPE"} : new HashSet<string>(){"PC", "PE"};
                        
                        HashSet<string> selectedPLs = new HashSet<string>(currentLipid.headGroupNames);
                        
                        HashSet<string> regularPLs = new HashSet<string>(selectedPLs.Except(supposedPLs));
                        HashSet<string> specialPLs = new HashSet<string>((supposedPLs).Intersect(selectedPLs));
                            
                        bool regularWarn = regularPLs.Any();
                        bool specialWarn = specialPLs.Any();
                        bool split = (regularWarn && specialWarn) || (specialWarn && ((Phospholipid)currentLipid).fag1.faTypes["FA"]);
                        
                        if (regularWarn || specialWarn)
                        {
                            
                            string warningText = "Warning: " + (regularWarn ? "You selected " + string.Join(", ", regularPLs) + " headgroup(s) with a plasmalogen on first fatty acyl chain. Be aware, that fragmentation rules are currently only available for regular fatty acyl chains and will be applied." : "") + (split && regularWarn ? "\n\nBesides, " : "") + (split ? "you selected " + string.Join(",", specialPLs) + " headgroup(s) including a plasmalogen. For convencience, these headgroup(s) will be split into an own assembly." : "") + (specialWarn && !split && !regularWarn ? "You selected " + string.Join(",", specialPLs) + " headgroup(s) with a plasmalogen. For convencience, this selection will be modified according to the plasmalogen option." : "") + " Do you want to continue?";
                        
                            if (MessageBox.Show(warningText, "Warning when registering phospholipid", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                
                                List<string> HGs = new List<string>(); 
                                foreach (string hg in specialPLs)
                                {
                                    if (((Phospholipid)currentLipid).fag1.faTypes["FAp"]) HGs.Add(hg + " O-p");
                                    if (((Phospholipid)currentLipid).fag1.faTypes["FAa"]) HGs.Add(hg + " O-a");
                                }
                                
                                if (split && regularWarn)
                                {
                                    Phospholipid newLipid = new Phospholipid((Phospholipid)currentLipid);
                                    Phospholipid pureLipid = null;
                                    if (((Phospholipid)currentLipid).fag1.faTypes["FA"])
                                    {
                                        pureLipid = new Phospholipid((Phospholipid)currentLipid);
                                        pureLipid.headGroupNames.Clear();
                                        pureLipid.fag1.faTypes["FA"] = true;
                                        pureLipid.fag1.faTypes["FAa"] = false;
                                        pureLipid.fag1.faTypes["FAp"] = false;
                                        foreach (string hg in specialPLs) pureLipid.headGroupNames.Add(hg);
                                    }
                                    
                                    foreach (string hg in supposedPLs) plHgListbox.SelectedItems.Remove(hg);
                                    newLipid.headGroupNames.Clear();
                                    
                                    foreach (string hg in HGs) newLipid.headGroupNames.Add(hg);
                                    newLipid.hasPlasmalogen = true;
                                    
                                    try {
                                        ulong lipidHash = newLipid.getHashCode();
                                        lipidCreator.registeredLipidDictionary.Add(lipidHash, newLipid);
                                        lipidCreator.registeredLipids.Add(lipidHash);
                                        registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(newLipid));
                                        
                                        if (((Phospholipid)currentLipid).fag1.faTypes["FA"])
                                        {
                                            lipidHash = pureLipid.getHashCode();
                                            lipidCreator.registeredLipidDictionary.Add(lipidHash, pureLipid);
                                            lipidCreator.registeredLipids.Add(lipidHash);
                                            registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(pureLipid));
                                        }
                                        
                                        refreshRegisteredLipidsTable();
                                    }
                                    catch
                                    {
                                        log.Debug("A lipid with this set of parameters is already registered.");
                                        MessageBox.Show("A lipid with this set of parameters is already registered.", "Lipid registered");
                                    }
                                    
                                }
                                else if (split && !regularWarn)
                                {
                                    Phospholipid newLipid = new Phospholipid((Phospholipid)currentLipid);
                                    plFA1Checkbox1.Checked = true;
                                    plFA1Checkbox2.Checked = false;
                                    plFA1Checkbox3.Checked = false;
                                    newLipid.headGroupNames.Clear();
                                    
                                    foreach (string hg in HGs) newLipid.headGroupNames.Add(hg);
                                    newLipid.hasPlasmalogen = true;
                                    
                                    try {
                                        ulong lipidHash = newLipid.getHashCode();
                                        lipidCreator.registeredLipidDictionary.Add(lipidHash, newLipid);
                                        lipidCreator.registeredLipids.Add(lipidHash);
                                        registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(newLipid));
                                        
                                        refreshRegisteredLipidsTable();
                                    }
                                    catch
                                    {
                                        log.Debug("A lipid with this set of parameters is already registered.");
                                        MessageBox.Show("A lipid with this set of parameters is already registered.", "Lipid registered");
                                    }
                                }
                                else if (!regularWarn)
                                {
                                    plHasPlasmalogen.Checked = true;
                                    plHgListbox.SelectedItems.Clear();
                                    
                                    foreach (string hg in HGs) plHgListbox.SelectedItems.Add(hg);
                                }
                            }
                            else
                            {
                                return LipidCategory.NoLipid;
                            }
                        }
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
            lipidsGridview.InvokeIfRequired(() => {
                log.Debug("Modifying row in lipid table");
                LipidCategory result = checkPropertiesValid();
                
                if (result == LipidCategory.NoLipid) return;
                
                ulong lipidHash = lipidModifications[(int)result];
                if (!lipidCreator.registeredLipidDictionary.ContainsKey(lipidHash))
                {
                    MessageBox.Show("An error happened during the modification of a lipid.", "Error during modification");
                    log.Error("An error happened during the modification of a lipid.");
                    return;
                }
                
                int lipidRow = 0;
                while (lipidRow < lipidCreator.registeredLipids.Count && (ulong)lipidCreator.registeredLipids[lipidRow] != lipidHash) lipidRow++;
                if (lipidRow >= lipidCreator.registeredLipids.Count)
                {
                    MessageBox.Show("An error happened during the modification of a lipid. Lipid is not in list any more.", "Error during modification");
                    log.Error("An error happened during the modification of a lipid.");
                    return;
                }
                
                ulong newHash = 0;
                int tabIndex = 0;
                switch (result)
                {
                    
                    case LipidCategory.Glycerolipid:
                        newHash = ((Glycerolipid)currentLipid).getHashCode();
                        lipidCreator.registeredLipids[lipidRow] = newHash;
                        lipidCreator.registeredLipidDictionary.Remove(lipidHash);
                        lipidCreator.registeredLipidDictionary.Add(newHash, new Glycerolipid((Glycerolipid)currentLipid));
                        tabIndex = (int)LipidCategory.Glycerolipid; 
                        break;
                        
                    case LipidCategory.Glycerophospholipid:
                        newHash = ((Phospholipid)currentLipid).getHashCode();
                        lipidCreator.registeredLipids[lipidRow] = newHash;
                        lipidCreator.registeredLipidDictionary.Remove(lipidHash);
                        lipidCreator.registeredLipidDictionary.Add(newHash, new Phospholipid((Phospholipid)currentLipid));
                        tabIndex = (int)LipidCategory.Glycerophospholipid;
                        break;
                        
                    case LipidCategory.Sphingolipid:
                        newHash = ((Sphingolipid)currentLipid).getHashCode();
                        lipidCreator.registeredLipids[lipidRow] = newHash;
                        lipidCreator.registeredLipidDictionary.Remove(lipidHash);
                        lipidCreator.registeredLipidDictionary.Add(newHash, new Sphingolipid((Sphingolipid)currentLipid));
                        tabIndex = (int)LipidCategory.Sphingolipid;
                        break;
                        
                    case LipidCategory.Sterollipid:
                        newHash = ((Cholesterol)currentLipid).getHashCode();
                        lipidCreator.registeredLipids[lipidRow] = newHash;
                        lipidCreator.registeredLipidDictionary.Remove(lipidHash);
                        lipidCreator.registeredLipidDictionary.Add(newHash, new Cholesterol((Cholesterol)currentLipid));
                        tabIndex = (int)LipidCategory.Sterollipid;
                        break;
                        
                    case LipidCategory.LipidMediator:
                        newHash = ((Mediator)currentLipid).getHashCode();
                        lipidCreator.registeredLipids[lipidRow] = newHash;
                        lipidCreator.registeredLipidDictionary.Remove(lipidHash);
                        lipidCreator.registeredLipidDictionary.Add(newHash, new Mediator((Mediator)currentLipid));
                        tabIndex = (int)LipidCategory.LipidMediator;
                        break;
                        
                    default:
                        return;
                }
                DataRow tmpRow = createLipidsGridviewRow(currentLipid);
                foreach (DataColumn dc in registeredLipidsDatatable.Columns) registeredLipidsDatatable.Rows[lipidRow][dc.ColumnName] = tmpRow[dc.ColumnName];
                
                for (int i = 0; i < lipidModifications.Length; ++i) lipidModifications[i] = 0;
                lipidModifications[tabIndex] = newHash;
            

                lipidsGridview.Rows[lipidRow].Cells["Edit"].Value = editImage;
                lipidsGridview.Rows[lipidRow].Cells["Delete"].Value = deleteImage;
                
            });
        }
        
        
        
        
        
        
        public void registerLipid(Object sender, EventArgs e)
        {
            lipidsGridview.InvokeIfRequired(() => {
                log.Debug("Registering lipids for lipid table");
                LipidCategory result = checkPropertiesValid();
                if (result == LipidCategory.NoLipid) return;
                int tabIndex = 0;
                ulong lipidHash = 0;
                
                try {
                    switch (result)
                    {
                        case LipidCategory.Glycerolipid:
                            lipidHash = ((Glycerolipid)currentLipid).getHashCode();
                            lipidCreator.registeredLipidDictionary.Add(lipidHash, new Glycerolipid((Glycerolipid)currentLipid));
                            lipidCreator.registeredLipids.Add(lipidHash);
                            registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                            tabIndex = (int)LipidCategory.Glycerolipid;
                            break;
                            
                        case LipidCategory.Glycerophospholipid:
                            lipidHash = ((Phospholipid)currentLipid).getHashCode();
                            lipidCreator.registeredLipidDictionary.Add(lipidHash, new Phospholipid((Phospholipid)currentLipid));
                            lipidCreator.registeredLipids.Add(lipidHash);
                            registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                            tabIndex = (int)LipidCategory.Glycerophospholipid;
                            break;
                            
                        case LipidCategory.Sphingolipid:
                            lipidHash = ((Sphingolipid)currentLipid).getHashCode();
                            lipidCreator.registeredLipidDictionary.Add(lipidHash, new Sphingolipid((Sphingolipid)currentLipid));
                            lipidCreator.registeredLipids.Add(lipidHash);
                            registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                            tabIndex = (int)LipidCategory.Sphingolipid;
                            break;
                            
                        case LipidCategory.Sterollipid:
                            lipidHash = ((Cholesterol)currentLipid).getHashCode();
                            lipidCreator.registeredLipidDictionary.Add(lipidHash, new Cholesterol((Cholesterol)currentLipid));
                            lipidCreator.registeredLipids.Add(lipidHash);
                            registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                            tabIndex = (int)LipidCategory.Sterollipid;
                            break;
                            
                        case LipidCategory.LipidMediator:
                            lipidHash = ((Mediator)currentLipid).getHashCode();
                            lipidCreator.registeredLipidDictionary.Add(lipidHash, new Mediator((Mediator)currentLipid));
                            lipidCreator.registeredLipids.Add(lipidHash);
                            registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(currentLipid));
                            tabIndex = (int)LipidCategory.LipidMediator;
                            break;
                            
                        default:
                            return;
                    }

                    lipidsGridview.Rows[lipidsGridview.Rows.Count - 1].Cells["Edit"].Value = editImage;
                    lipidsGridview.Rows[lipidsGridview.Rows.Count - 1].Cells["Delete"].Value = deleteImage;

                    for (int i = 0; i < lipidModifications.Length; ++i) lipidModifications[i] = 0;
                    lipidModifications[tabIndex] = lipidHash;
                    modifyLipidButton.Enabled = true;
                }
                catch
                {
                    log.Debug("A lipid with this set of parameters is already registered.");
                    MessageBox.Show("A lipid with this set of parameters is already registered.", "Lipid registered");
                }
            });
        }
        
        
        
        
        
        
        public string FARepresentation(FattyAcidGroup fag)
        {
            string faRepresentation = "";
            if (fag.isLCB)
            {
                faRepresentation = "LCB";
            }
            else
            {
                faRepresentation = string.Join(", ",(from faType in fag.faTypes.Keys where fag.faTypes[faType] select faType));
            }
            faRepresentation += (new string[]{":", " (odd):", " (even):"})[fag.chainType];
            return faRepresentation;
        }
        
        
        
        
        
        
        public DataRow createLipidsGridviewRow(Lipid currentRegisteredLipid)
        {
            DataRow row = registeredLipidsDatatable.NewRow();
            ArrayList headGroupNames = new ArrayList();
            if (currentRegisteredLipid is Glycerolipid)
            {
                Glycerolipid currentGlycerolipid = (Glycerolipid)currentRegisteredLipid;
                row["Category"] = "Glycerolipid";
                
                if (currentGlycerolipid.containsSugar)
                {
                    row["Building Block 3"] = "HG: " + String.Join(", ", currentGlycerolipid.headGroupNames);
                    headGroupNames.AddRange(currentGlycerolipid.headGroupNames);
                    row["Building Block 2"] = FARepresentation(currentGlycerolipid.fag2) + currentGlycerolipid.fag2.lengthInfo + "; DB: " + currentGlycerolipid.fag2.dbInfo + "; OH: " + currentGlycerolipid.fag2.hydroxylInfo;
                }
                else if (!currentGlycerolipid.fag3.faTypes["FAx"])
                {
                    row["Building Block 3"] = FARepresentation(currentGlycerolipid.fag3) + currentGlycerolipid.fag3.lengthInfo + "; DB: " + currentGlycerolipid.fag3.dbInfo + "; OH: " + currentGlycerolipid.fag3.hydroxylInfo;
                    row["Building Block 2"] = FARepresentation(currentGlycerolipid.fag2) + currentGlycerolipid.fag2.lengthInfo + "; DB: " + currentGlycerolipid.fag2.dbInfo + "; OH: " + currentGlycerolipid.fag2.hydroxylInfo;
                    headGroupNames.Add("TAG");
                }
                else if (!currentGlycerolipid.fag2.faTypes["FAx"])
                {
                    row["Building Block 2"] = FARepresentation(currentGlycerolipid.fag2) + currentGlycerolipid.fag2.lengthInfo + "; DB: " + currentGlycerolipid.fag2.dbInfo + "; OH: " + currentGlycerolipid.fag2.hydroxylInfo;
                    headGroupNames.Add("DAG"); 
                }
                else
                {
                    headGroupNames.Add("MAG");
                }
                row["Building Block 1"] = FARepresentation(currentGlycerolipid.fag1) + currentGlycerolipid.fag1.lengthInfo + "; DB: " + currentGlycerolipid.fag1.dbInfo + "; OH: " + currentGlycerolipid.fag1.hydroxylInfo;
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
                    headGroupNames.Add("CL");
                }
                else
                {
                    row["Category"] = "Glycerophospholipid";
                    headGroupNames.AddRange(currentPhospholipid.headGroupNames);
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
                    row["Building Block 1"] = "HG: " + String.Join(", ", currentSphingolipid.headGroupNames);
                row["Building Block 2"] = FARepresentation(currentSphingolipid.lcb) + currentSphingolipid.lcb.lengthInfo + "; DB: " + currentSphingolipid.lcb.dbInfo + "; OH: " + currentSphingolipid.lcb.hydroxylCounts.First();
                if (!currentSphingolipid.isLyso) row["Building Block 3"] = FARepresentation(currentSphingolipid.fag) + currentSphingolipid.fag.lengthInfo + "; DB: " + currentSphingolipid.fag.dbInfo + "; OH: " + currentSphingolipid.fag.hydroxylCounts.First();
            }
            
            else if (currentRegisteredLipid is Cholesterol)
            {
                Cholesterol currentCHLipid = (Cholesterol)currentRegisteredLipid;
                row["Category"] = "Sterol lipid";
                if (currentCHLipid.containsEster)
                {
                    row["Building Block 1"] = "ChE";
                    headGroupNames.Add("ChE");
                }
                else
                {
                    row["Building Block 1"] = "Ch";
                    headGroupNames.Add("Ch");
                }
                if (currentCHLipid.containsEster) row["Building Block 2"] = FARepresentation(currentCHLipid.fag) + currentCHLipid.fag.lengthInfo + "; DB: " + currentCHLipid.fag.dbInfo + "; OH: " + currentCHLipid.fag.hydroxylInfo;
            }
            
            else if (currentRegisteredLipid is Mediator)
            {
                Mediator currentMedLipid = (Mediator)currentRegisteredLipid;
                row["Building Block 1"] = String.Join(", ", currentMedLipid.headGroupNames);
                row["Category"] = "Mediator";
                headGroupNames.AddRange(currentMedLipid.headGroupNames);
            }
            
            
            string adductsStr = "";
            foreach (Adduct adduct in Lipid.ALL_ADDUCTS.Values)
            {
                if (currentRegisteredLipid.adducts[adduct.name]) adductsStr += (adductsStr.Length > 0 ? ", " : "") + adduct.visualization;
            }
            row["Adducts"] = adductsStr;
            
            string filtersStr = "";
            switch (currentRegisteredLipid.onlyPrecursors)
            {
                case 0: filtersStr = "no precursors,\n"; break;
                case 1: filtersStr = "only precursors,\n"; break;
                case 2: filtersStr = "with precursors,\n"; break;
            }
            
            switch (currentRegisteredLipid.onlyHeavyLabeled)
            {
                case 0: filtersStr += "no heavy"; break;
                case 1: filtersStr += "only heavy"; break;
                case 2: filtersStr += "with heavy"; break;
            }
            row["Filters"] = filtersStr;
            
            string optionsStr = "";
            if (currentRegisteredLipid.onlyHeavyLabeled != 0)
            {
                foreach (string headGroupName in headGroupNames)
                {
                    if (lipidCreator.headgroups.ContainsKey(headGroupName) && lipidCreator.headgroups[headGroupName].heavyLabeledPrecursors.Count > 0)
                    {
                        optionsStr = "+ heavy isotopes";
                        break;
                    }
                }
            }
            
            if (currentRegisteredLipid.onlyPrecursors != 1)
            {
                bool containsUserDefined = false;
                foreach (string headGroupName in currentRegisteredLipid.positiveFragments.Keys)
                {
                    foreach(string fragmentName in currentRegisteredLipid.positiveFragments[headGroupName])
                    {
                        containsUserDefined |= lipidCreator.allFragments.ContainsKey(headGroupName) && lipidCreator.allFragments[headGroupName][true].ContainsKey(fragmentName) && lipidCreator.allFragments[headGroupName][true][fragmentName].userDefined;
                        if (containsUserDefined) break;
                    }
                    if (containsUserDefined) break;
                }
                
                if (!containsUserDefined)
                {
                    foreach (string headGroupName in currentRegisteredLipid.negativeFragments.Keys)
                    {
                        foreach(string fragmentName in currentRegisteredLipid.negativeFragments[headGroupName])
                        {
                            containsUserDefined |= lipidCreator.allFragments.ContainsKey(headGroupName) && lipidCreator.allFragments[headGroupName][false].ContainsKey(fragmentName) && lipidCreator.allFragments[headGroupName][false][fragmentName].userDefined;
                            if (containsUserDefined) break;
                        }
                        if (containsUserDefined) break;
                    }
                }
                if (containsUserDefined)
                {
                    optionsStr += (optionsStr.Length > 0 ? ",\n" : "") + "+ new fragments";
                }
            }
            
            row["Options"] = optionsStr;
            return row;
        }
        
        
        
        
        
        
        
        public void refreshRegisteredLipidsTable()
        {
            lipidsGridview.InvokeIfRequired(() =>
            {
                log.Debug("Refreshing lipids table");
                registeredLipidsDatatable.Clear();
                foreach (ulong lipidHash in lipidCreator.registeredLipids)
                {
                    if (!lipidCreator.registeredLipidDictionary.ContainsKey(lipidHash))
                    {
                        log.Error("Error during refreshing table.");
                        return;
                    }
                    registeredLipidsDatatable.Rows.Add(createLipidsGridviewRow(lipidCreator.registeredLipidDictionary[lipidHash]));
                    
                }

                for (int i = 0; i < lipidsGridview.Rows.Count; ++i)
                {
                    lipidsGridview.Rows[i].Cells["Edit"].Value = editImage;
                    lipidsGridview.Rows[i].Cells["Delete"].Value = deleteImage;
                }
            });
        }
        
        
        
        
        
        
        public void lipidsGridviewDoubleClick(Object sender, EventArgs e)
        {
            if (sender == null) { return; }
            int rowIndex = ((DataGridView)sender).CurrentCell.RowIndex;
            int colIndex = ((DataGridView)sender).CurrentCell.ColumnIndex;
            if (((DataGridView)sender).Columns[colIndex].Name == "Edit")
            {
                loadLipid((ulong)lipidCreator.registeredLipids[rowIndex]);
                
            }
            else if (((DataGridView)sender).Columns[colIndex].Name == "Delete")
            {
                deleteLipidsGridviewRow(rowIndex);
            }
        }
        
        
        
        
        
        public void loadLipid(ulong lipidHash)
        {
            Lipid currentRegisteredLipid = lipidCreator.registeredLipidDictionary[lipidHash];
            int tabIndex = 0;
            for (int i = 0; i < lipidModifications.Length; ++i) lipidModifications[i] = 0;
            
            if (currentRegisteredLipid is Glycerolipid)
            {
                tabIndex = (int)LipidCategory.Glycerolipid;
                lipidTabList[tabIndex] = new Glycerolipid((Glycerolipid)currentRegisteredLipid);
                currentLipid = (Glycerolipid)lipidTabList[tabIndex];
            }
            else if (currentRegisteredLipid is Phospholipid)
            {
                tabIndex = (int)LipidCategory.Glycerophospholipid;
                lipidTabList[tabIndex] = new Phospholipid((Phospholipid)currentRegisteredLipid);
                currentLipid = (Phospholipid)lipidTabList[tabIndex];
            }
            else if (currentRegisteredLipid is Sphingolipid)
            {
                tabIndex = (int)LipidCategory.Sphingolipid;
                lipidTabList[tabIndex] = new Sphingolipid((Sphingolipid)currentRegisteredLipid);
                currentLipid = (Sphingolipid)lipidTabList[tabIndex];
            }
            else if (currentRegisteredLipid is Cholesterol)
            {
                tabIndex = (int)LipidCategory.Sterollipid;
                lipidTabList[tabIndex] = new Cholesterol((Cholesterol)currentRegisteredLipid);
                currentLipid = (Cholesterol)lipidTabList[tabIndex];
            }
            else if (currentRegisteredLipid is Mediator)
            {
                tabIndex = (int)LipidCategory.LipidMediator;
                lipidTabList[tabIndex] = new Mediator((Mediator)currentRegisteredLipid);
                currentLipid = (Mediator)lipidTabList[tabIndex];
            }
            lipidModifications[tabIndex] = lipidHash;
            
            tabControl.SelectedIndex = tabIndex;
            changeTab(tabIndex);
        }
        
        
        
        
        
        public void lipidsGridviewKeydown(Object sender, KeyEventArgs e)
        {
            if (sender == null) { return; }
            if (e.KeyCode == Keys.Delete && lipidCreator.registeredLipids.Count > 0 && ((DataGridView)sender).SelectedRows.Count > 0)
            {   
                deleteLipidsGridviewRow(((DataGridView)sender).SelectedRows[0].Index);
                e.Handled = true;
            }
        }
        
        
        
        
        
        
        
        public void deleteLipidsGridviewRow(int rowIndex)
        {
            lipidsGridview.InvokeIfRequired(() =>
            {
                log.Debug("Deleting row " + rowIndex + " from lipids table");
                ulong lipidHash = (ulong)lipidCreator.registeredLipids[rowIndex];
                Lipid currentRegisteredLipid = lipidCreator.registeredLipidDictionary[lipidHash];
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
                
                if (lipidModifications[tabIndex] == lipidHash) lipidModifications[tabIndex] = 0L;
                if (tabIndex == (int)currentIndex) changeTab(tabIndex);

                for (int i = 0; i < lipidsGridview.Rows.Count; ++i)
                {
                    lipidsGridview.Rows[i].Cells["Edit"].Value = editImage;
                    lipidsGridview.Rows[i].Cells["Delete"].Value = deleteImage;
                }
                lipidCreator.registeredLipidDictionary.Remove((ulong)lipidCreator.registeredLipids[rowIndex]);
                lipidCreator.registeredLipids.RemoveAt(rowIndex);
            });

        }
        
        
        
        
        
        
        public void unsetInstrument(Object sender, EventArgs e)
        {
            if (sender == null) { return; }
            lipidCreator.selectedInstrumentForCE = "";
            lipidCreator.monitoringType = MonitoringTypes.NoMonitoring;
            lastCEInstrumentChecked.Checked = false;
            lastCEInstrumentChecked = (MenuItem)sender;
            lastCEInstrumentChecked.Checked = true;
        }
        
        
        
        
        
        public void changeInstrumentForCEtypePRM(Object sender, EventArgs e)
        {
            if (sender == null) { return; }
            if(((MenuItem)sender).Tag != null) {
                string instrument = ((string[])((MenuItem)sender).Tag)[0];
                lipidCreator.selectedInstrumentForCE = (string)lipidCreator.msInstruments[instrument].CVTerm;
            
                lipidCreator.monitoringType = MonitoringTypes.PRM;
                lastCEInstrumentChecked.Checked = false;
                lastCEInstrumentChecked = (MenuItem)sender;
                lastCEInstrumentChecked.Checked = true;
            }
        }
        
        
        
        
        
        public void changeInstrumentForCEtypeSRM(Object sender, EventArgs e)
        {
            if (sender == null) { return; }
            if(((MenuItem)sender).Tag != null) {
                string instrument = ((string[])((MenuItem)sender).Tag)[0];
                lipidCreator.selectedInstrumentForCE = (string)lipidCreator.msInstruments[instrument].CVTerm;
            
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
            
            
            ArrayList returnValues = new ArrayList(){false, 0};
            
            
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
                    
                    try
                    {
                        lipidCreator.assembleFragments(asDeveloper, returnValues);   
                    
                        lipidsReview = new LipidsReview(this, returnValues);
                        lipidsReview.Owner = this;
                        lipidsReview.ShowInTaskbar = false;
                        
                        lipidsReview.ShowDialog();
                        lipidsReview.Dispose();
                    }
                    catch (LipidException lipidException)
                    {
                        lipidException.creatorGUI = this;
                        int[] messageBoxValues = new int[]{0};
                        LCMessageBox lcMessageBox = new LCMessageBox(messageBoxValues, 1, lipidException);
                        lcMessageBox.Owner = this;
                        lcMessageBox.ShowInTaskbar = false;
                        lcMessageBox.Show();
                        break;
                    }
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
        
            int[] filterParameters = {2, 2};
            FilterDialog importFilterDialog = new FilterDialog(filterParameters);
            importFilterDialog.Owner = this;
            importFilterDialog.ShowInTaskbar = false;
            importFilterDialog.ShowDialog();
            importFilterDialog.Dispose();
            
            int[] returnMessage = new int[]{0};
            LCMessageBox lcmb = new LCMessageBox(returnMessage, 0);
            lcmb.Owner = this;
            lcmb.StartPosition = FormStartPosition.CenterParent;
            lcmb.ShowInTaskbar = false;
            lcmb.ShowDialog();
            lcmb.Dispose();
            if (returnMessage[0] == 1){
                lipidCreator.registeredLipidDictionary.Clear();
                lipidCreator.registeredLipids.Clear(); // replace
            }
            
            System.Windows.Forms.MenuItem PredefItem = (System.Windows.Forms.MenuItem)sender;
            string filePath = (string)PredefItem.Tag;
            
            try
            {
                int[] importNumbers = lipidCreator.importLipidList(filePath, filterParameters);
                refreshRegisteredLipidsTable();
                if (importNumbers[0] != importNumbers[1])
                {
                    MessageBox.Show("Only " + importNumbers[0] + " of " + importNumbers[1] + " lipid names were imported successfully! Please check the output at 'Help' -> 'Log messages' for details!", "Lipid list import");
                }
                else
                {
                    MessageBox.Show("All " + importNumbers[0] + " of " + importNumbers[1] + " lipid names were imported successfully!", "Lipid list import");
                }
            }
            catch
            {
                MessageBox.Show ("An error occurred while importing the lipid list. For more details, please read the log message and contact the developers.", "LipidCreator: error occurred");
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
                    
                    int[] returnMessage = new int[]{0};
                    LCMessageBox lcmb = new LCMessageBox(returnMessage, 0);
                    lcmb.Owner = this;
                    lcmb.StartPosition = FormStartPosition.CenterParent;
                    lcmb.ShowInTaskbar = false;
                    lcmb.ShowDialog();
                    lcmb.Dispose();
                    if (returnMessage[0] == 1)
                    {
                        lipidCreator.registeredLipidDictionary.Clear();
                        lipidCreator.registeredLipids.Clear(); // replace
                    }
                    
                    int[] importNumbers = lipidCreator.importLipidList(openFileDialog1.FileName, filterParameters);
                    refreshRegisteredLipidsTable();
                    if (importNumbers[0] != importNumbers[1])
                    {
                        MessageBox.Show("Only " + importNumbers[0] + " of " + importNumbers[1] + " lipid names were imported successfully! Please check the output at 'Help' -> 'Log messages' for details!", "Lipid list import");
                    }
                    else
                    {
                        MessageBox.Show("All " + importNumbers[0] + " of " + importNumbers[1] + " lipid names were successfully imported!", "Lipid list import");
                    }

                }
                else
                {
                    MessageBox.Show("Could not read file, " + openFileDialog1.FileName, "Lipid list import");
                    log.Error("Could not read file, " + openFileDialog1.FileName);
                }
            }
        }
        
        
        
        
        
        public void goToFragment(LipidException lipidException)
        {
            if (lipidException == null || lipidException.precursorData.lipidHash == 0 || !lipidCreator.registeredLipidDictionary.ContainsKey(lipidException.precursorData.lipidHash))
            {
                MessageBox.Show("Could not open fragment, it seems that the lipid assembly is not registered any more.");
                log.Error("Could not open fragment, it seems that the lipid assembly is not registered any more.");
                return;
            }
            loadLipid(lipidException.precursorData.lipidHash);
            
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
                    ms2fragmentsForm = new MS2Form(this, lipidException);
                    formToOpen = (Form)ms2fragmentsForm;
                    break;
            }
            formToOpen.Owner = this;
            formToOpen.ShowInTaskbar = false;
            formToOpen.ShowDialog();
            formToOpen.Dispose();
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
                int[] returnMessage = new int[]{0};
                LCMessageBox lcmb = new LCMessageBox(returnMessage, 0);
                lcmb.Owner = this;
                lcmb.StartPosition = FormStartPosition.CenterParent;
                lcmb.ShowInTaskbar = false;
                lcmb.ShowDialog();
                lcmb.Dispose();
                if (returnMessage[0] == 1) resetLipidCreator(false); // replace
                
                try 
                {
                    lipidCreator.import(openFileDialog1.FileName);
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
                int[] returnMessage = new int[]{0};
                LCMessageBox lcmb = new LCMessageBox(returnMessage, 0);
                lcmb.Owner = this;
                lcmb.StartPosition = FormStartPosition.CenterParent;
                lcmb.ShowInTaskbar = false;
                lcmb.ShowDialog();
                lcmb.Dispose();
                if (returnMessage[0] == 1) resetLipidCreator(false); // replace
            
                try 
                {
                    lipidCreator.import(openFileDialog1.FileName, true);
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
            if (lipidCreator.selectedInstrumentForCE == "")
            {
                MessageBox.Show ("To use the collision energy optimization function, an MS device has to be selected before. Please go on Menu -> Options -> Collision Energy computation to select a device.", "LipidCreator: no device selected");
            }
            else
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
        }
        
        
        
        
        protected void menuWizardClick(object sender, System.EventArgs e)
        {
            Wizard wizard = new Wizard(this);
            wizard.Owner = this;
            wizard.ShowInTaskbar = false;
            wizard.ShowDialog();
            wizard.Dispose();
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
            AboutDialog aboutDialog = new AboutDialog (this);
            aboutDialog.Owner = this;
            aboutDialog.ShowInTaskbar = false;
            aboutDialog.ShowDialog();
            aboutDialog.Dispose();
        }
        

        
        protected void menuLogClick(object sender, System.EventArgs e)
        {
            AboutDialog aboutDialog = new AboutDialog (this, true);
            aboutDialog.Owner = this;
            aboutDialog.ShowInTaskbar = false;
            aboutDialog.ShowDialog();
            aboutDialog.Dispose();
        }
        

        protected void menuDocsClick(object sender, System.EventArgs e)
        {
            string docsDir = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase), "data", "docs");
            openDirectory(docsDir);
        }
        
        
    }
}
