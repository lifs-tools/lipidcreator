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
        public bool changing_tab_forced;
        public int currentTabIndex = 1;
        public LipidCreatorForm lipidCreatorForm;
        public Lipid currentLipid;
        public DataTable registered_lipids_datatable;
        public int[] lipid_modifications;
        public Color alert_color = Color.FromArgb(255, 180, 180);
        public bool setting_listbox = false;
        public Dictionary<System.Windows.Forms.MenuItem, string> predefinedFiles;
        
        public CreatorGUI(LipidCreatorForm lipidCreatorForm)
        {
            this.lipidCreatorForm = lipidCreatorForm;
            
            registered_lipids_datatable = new DataTable("Daten");
            registered_lipids_datatable.Columns.Add(new DataColumn("Category"));
            registered_lipids_datatable.Columns.Add(new DataColumn("Building Block 1"));
            registered_lipids_datatable.Columns.Add(new DataColumn("Building Block 2"));
            registered_lipids_datatable.Columns.Add(new DataColumn("Building Block 3"));
            registered_lipids_datatable.Columns.Add(new DataColumn("Building Block 4"));
            InitializeComponent();
            lipid_modifications = new int[]{-1, -1, -1, -1};
            changing_tab_forced = false;
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
                            predefFile.Click += new System.EventHandler (menuImportPredefined_Click);
                            predefFolder.MenuItems.Add(predefFile);
                        }
                    }
                }
            }
        }
        
        private void lipids_gridview_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            
            if (initialCall){
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
                int w = (lipidsGridview.Width - 80) / 5 - 4;
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
            changing_tab_forced = true;
            currentTabIndex = index;
            tabControl.SelectedIndex = currentTabIndex > 0 ? currentTabIndex : 2;
            changeTab(currentTabIndex, true);
            changing_tab_forced = false;
            
        }
        
        public void tabIndexChanged(Object sender,  EventArgs e)
        {
            if (changing_tab_forced) return;
            currentTabIndex = ((TabControl)sender).SelectedIndex;
            changeTab(currentTabIndex, false);            
        }

        public void changeTab(int index, bool forced)
        {     
            if ((index == 2 || index == 0) && !forced)
            {
                if (plIsCL.Checked)
                {
                    index = 0;
                }
                else
                {
                    index = 2;
                }
            }
        
            currentLipid = (Lipid)lipidCreatorForm.lipidTabList[index];
            
            if (index == 0)
            {
                CLLipid currentCLLipid = (CLLipid)currentLipid;
                clFA1Textbox.Text = currentCLLipid.fag1.lengthInfo;
                clDB1Textbox.Text = currentCLLipid.fag1.dbInfo;
                clHydroxyl1Textbox.Text = currentCLLipid.fag1.hydroxylInfo;
                clFA1Combobox.SelectedIndex = currentCLLipid.fag1.chainType;
                clFA1Checkbox1.Checked = currentCLLipid.fag1.faTypes["FA"];
                clFA1Checkbox2.Checked = currentCLLipid.fag1.faTypes["FAp"];
                clFA1Checkbox3.Checked = currentCLLipid.fag1.faTypes["FAe"];
                
                clFA2Textbox.Text = currentCLLipid.fag2.lengthInfo;
                clDB2Textbox.Text = currentCLLipid.fag2.dbInfo;
                clHydroxyl2Textbox.Text = currentCLLipid.fag2.hydroxylInfo;
                clFA2Combobox.SelectedIndex = currentCLLipid.fag2.chainType;
                clFA2Checkbox1.Checked = currentCLLipid.fag2.faTypes["FA"];
                clFA2Checkbox2.Checked = currentCLLipid.fag2.faTypes["FAp"];
                clFA2Checkbox3.Checked = currentCLLipid.fag2.faTypes["FAe"];
                
                clFA3Textbox.Text = currentCLLipid.fag3.lengthInfo;
                clDB3Textbox.Text = currentCLLipid.fag3.dbInfo;
                clHydroxyl3Textbox.Text = currentCLLipid.fag3.hydroxylInfo;
                clFA3Combobox.SelectedIndex = currentCLLipid.fag3.chainType;
                clFA3Checkbox1.Checked = currentCLLipid.fag3.faTypes["FA"];
                clFA3Checkbox2.Checked = currentCLLipid.fag3.faTypes["FAp"];
                clFA3Checkbox3.Checked = currentCLLipid.fag3.faTypes["FAe"];
                
                clFA4Textbox.Text = currentCLLipid.fag4.lengthInfo;
                clDB4Textbox.Text = currentCLLipid.fag4.dbInfo;
                clHydroxyl4Textbox.Text = currentCLLipid.fag4.hydroxylInfo;
                clFA4Combobox.SelectedIndex = currentCLLipid.fag4.chainType;
                clFA4Checkbox1.Checked = currentCLLipid.fag4.faTypes["FA"];
                clFA4Checkbox2.Checked = currentCLLipid.fag4.faTypes["FAp"];
                clFA4Checkbox3.Checked = currentCLLipid.fag4.faTypes["FAe"];
                
                clPosAdductCheckbox1.Checked = currentCLLipid.adducts["+H"];
                clPosAdductCheckbox2.Checked = currentCLLipid.adducts["+2H"];
                clPosAdductCheckbox3.Checked = currentCLLipid.adducts["+NH4"];
                clNegAdductCheckbox1.Checked = currentCLLipid.adducts["-H"];
                clNegAdductCheckbox2.Checked = currentCLLipid.adducts["-2H"];
                clNegAdductCheckbox3.Checked = currentCLLipid.adducts["+HCOO"];
                clNegAdductCheckbox4.Checked = currentCLLipid.adducts["+CH3COO"];
                if (lipid_modifications[0] > -1) clModifyLipidButton.Enabled = true;
                else clModifyLipidButton.Enabled = false;
                
                plIsCL.Checked = true;
                
                update_ranges(currentCLLipid.fag1, clFA1Textbox, clFA1Combobox.SelectedIndex);
                update_ranges(currentCLLipid.fag1, clDB1Textbox, 3);
                update_ranges(currentCLLipid.fag1, clHydroxyl1Textbox, 4);
                update_ranges(currentCLLipid.fag2, clFA2Textbox, clFA2Combobox.SelectedIndex);
                update_ranges(currentCLLipid.fag2, clDB2Textbox, 3);
                update_ranges(currentCLLipid.fag2, clHydroxyl2Textbox, 4);
                update_ranges(currentCLLipid.fag3, clFA3Textbox, clFA3Combobox.SelectedIndex);
                update_ranges(currentCLLipid.fag3, clDB3Textbox, 3);
                update_ranges(currentCLLipid.fag3, clHydroxyl3Textbox, 4);
                update_ranges(currentCLLipid.fag4, clFA4Textbox, clFA4Combobox.SelectedIndex);
                update_ranges(currentCLLipid.fag4, clDB4Textbox, 3);
                update_ranges(currentCLLipid.fag4, clHydroxyl4Textbox, 4);
                
                clRepresentativeFA.Checked = currentCLLipid.representativeFA;
                clPictureBox.SendToBack();
                
            }
            else if (index == 1)
            {
                GLLipid currentGLLipid = (GLLipid)currentLipid;
                setting_listbox = true;
                for (int i = 0; i < glHgListbox.Items.Count; ++i)
                {
                    glHgListbox.SetSelected(i, false);
                }
                foreach (int hgValue in currentGLLipid.hgValues)
                {
                    glHgListbox.SetSelected(hgValue, true);
                }
                setting_listbox = false;
                
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
                if (lipid_modifications[1] > -1) glModifyLipidButton.Enabled = true;
                else glModifyLipidButton.Enabled = false;
                
                glContainsSugar.Checked = currentGLLipid.containsSugar;
                
                
                update_ranges(currentGLLipid.fag1, glFA1Textbox, glFA1Combobox.SelectedIndex);
                update_ranges(currentGLLipid.fag1, glDB1Textbox, 3);
                update_ranges(currentGLLipid.fag1, glHydroxyl1Textbox, 4);
                update_ranges(currentGLLipid.fag2, glFA2Textbox, glFA2Combobox.SelectedIndex);
                update_ranges(currentGLLipid.fag2, glDB2Textbox, 3);
                update_ranges(currentGLLipid.fag2, glHydroxyl2Textbox, 4);
                update_ranges(currentGLLipid.fag3, glFA3Textbox, glFA3Combobox.SelectedIndex);
                update_ranges(currentGLLipid.fag3, glDB3Textbox, 3);
                update_ranges(currentGLLipid.fag3, glHydroxyl3Textbox, 4);
                
                glRepresentativeFA.Checked = currentGLLipid.representativeFA;
                glPictureBox.SendToBack();
            }
            else if (index == 2)
            {
                PLLipid currentPLLipid = (PLLipid)currentLipid;
                setting_listbox = true;
                for (int i = 0; i < plHgListbox.Items.Count; ++i)
                {
                    plHgListbox.SetSelected(i, false);
                }
                foreach (int hgValue in currentPLLipid.hgValues)
                {
                    plHgListbox.SetSelected(hgValue, true);
                }
                setting_listbox = false;
                
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
                if (lipid_modifications[2] > -1) plModifyLipidButton.Enabled = true;
                else plModifyLipidButton.Enabled = false;
                plIsCL.Checked = false;
                
                
                update_ranges(currentPLLipid.fag1, plFA1Textbox, plFA1Combobox.SelectedIndex);
                update_ranges(currentPLLipid.fag1, plDB1Textbox, 3);
                update_ranges(currentPLLipid.fag1, plHydroxyl1Textbox, 4);
                update_ranges(currentPLLipid.fag2, plFA2Textbox, plFA2Combobox.SelectedIndex);
                update_ranges(currentPLLipid.fag2, plDB2Textbox, 3);
                update_ranges(currentPLLipid.fag2, plHydroxyl2Textbox, 4);
                
                plRepresentativeFA.Checked = currentPLLipid.representativeFA;
                plPictureBox.SendToBack();
            }
            else if (index == 3)
            {
                SLLipid currentSLLipid = (SLLipid)currentLipid;
                setting_listbox = true;
                for (int i = 0; i < slHgListbox.Items.Count; ++i)
                {
                    slHgListbox.SetSelected(i, false);
                }
                foreach (int hgValue in currentSLLipid.hgValues)
                {
                    slHgListbox.SetSelected(hgValue, true);
                }
                setting_listbox = false;
                
                
                slLCBTextbox.Text = currentSLLipid.lcb.lengthInfo;
                slDB2Textbox.Text = currentSLLipid.lcb.dbInfo;
                slLCBCombobox.SelectedIndex = currentSLLipid.lcb.chainType;
                slLCBHydroxyCombobox.SelectedIndex = currentSLLipid.longChainBaseHydroxyl - 2;
                slFAHydroxyCombobox.SelectedIndex = currentSLLipid.fattyAcidHydroxyl;
                
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
                if (lipid_modifications[3] > -1) slModifyLipidButton.Enabled = true;
                else slModifyLipidButton.Enabled = false;
                
                
                update_ranges(currentSLLipid.lcb, slLCBTextbox, slLCBCombobox.SelectedIndex, true);
                update_ranges(currentSLLipid.lcb, slDB2Textbox, 3);
                update_ranges(currentSLLipid.fag, slFATextbox, slFACombobox.SelectedIndex);
                update_ranges(currentSLLipid.fag, slDB1Textbox, 3);
                
                /*
                string headgroup = sl_hg_combobox.SelectedItem.ToString();
                if (headgroup == "SPH" || headgroup == "S1P" || headgroup == "SPC"){
                    ((sl_lipid)currentLipid).fag.disabled = true;
                    sl_fa_combobox.Enabled = false;
                    sl_fa_textbox.Enabled = false;
                    sl_db_1_textbox.Enabled = false;
                    sl_fa_hydroxy_combobox.Enabled = false;
                }*/
                slPictureBox.SendToBack();
            }
            
        }
        
        public void reset_cl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.lipidTabList[0] = new CLLipid(lipidCreatorForm.allPathsToPrecursorImages, lipidCreatorForm.allFragments);
            lipid_modifications[0] = -1;
            clDB1Textbox.BackColor = Color.White;
            clDB2Textbox.BackColor = Color.White;
            clDB3Textbox.BackColor = Color.White;
            clDB4Textbox.BackColor = Color.White;
            clFA1Textbox.BackColor = Color.White;
            clFA2Textbox.BackColor = Color.White;
            clFA3Textbox.BackColor = Color.White;
            clFA4Textbox.BackColor = Color.White;
            changeTab(0);
        }
        
        public void reset_gl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.lipidTabList[1] = new GLLipid(lipidCreatorForm.allPathsToPrecursorImages, lipidCreatorForm.allFragments);
            lipid_modifications[1] = -1;
            changeTab(1);
        }
        
        public void reset_pl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.lipidTabList[2] = new PLLipid(lipidCreatorForm.allPathsToPrecursorImages, lipidCreatorForm.allFragments);
            lipid_modifications[2] = -1;
            changeTab(2);
        }
        
        public void reset_sl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.lipidTabList[3] = new SLLipid(lipidCreatorForm.allPathsToPrecursorImages, lipidCreatorForm.allFragments);
            lipid_modifications[3] = -1;
            changeTab(3);
        }
        
        
        /////////////////////// CL //////////////////////////////
        
        
        public void clFA1ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag1.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((CLLipid)currentLipid).fag1, clFA1Textbox, ((ComboBox)sender).SelectedIndex);
            if (((CLLipid)currentLipid).representativeFA)
            {
                clFA2Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                clFA3Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                clFA4Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
            }
        }
        public void clFA2ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((CLLipid)currentLipid).fag1, clFA2Textbox, ((ComboBox)sender).SelectedIndex);
        }
        public void clFA3ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag3.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((CLLipid)currentLipid).fag1, clFA3Textbox, ((ComboBox)sender).SelectedIndex);
        }
        public void clFA4ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag4.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((CLLipid)currentLipid).fag1, clFA4Textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        
        
        public void update_ranges(fattyAcidGroup fag, TextBox tb, int ob_type)
        {
            update_ranges(fag, tb, ob_type, false);
        }
        
        // ob_type (Object type): 0 = carbon length, 1 = carbon length odd, 2 = carbon length even, 3 = db length, 4 = hydroxyl length
        public void update_ranges(fattyAcidGroup fag, TextBox tb, int ob_type, bool isLCB)
        {
            int max_range = 30;
            int min_range = 0;
            if (ob_type < 3) min_range = 2;
            if (ob_type == 3) max_range = 6;
            else if (ob_type == 4) max_range = 29;
            if (isLCB) min_range = 8;
            HashSet<int> lengths = lipidCreatorForm.parseRange(tb.Text, min_range,  max_range, ob_type);
            if (ob_type <= 2)
            {
                fag.carbonCounts = lengths; 
            }
            else if (ob_type == 3)
            { 
                fag.doubleBondCounts = lengths;          
            }
            else if (ob_type == 4)
            {
                fag.hydroxylCounts = lengths;
            }
            tb.BackColor = (lengths == null) ? alert_color : Color.White;
        }
        
        public void clFA1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((CLLipid)currentLipid).fag1, (TextBox)sender, clFA1Combobox.SelectedIndex);
            if (((CLLipid)currentLipid).representativeFA)
            {
                clFA2Textbox.Text = ((TextBox)sender).Text;
                clFA3Textbox.Text = ((TextBox)sender).Text;
                clFA4Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void clFA2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((CLLipid)currentLipid).fag2, (TextBox)sender, clFA2Combobox.SelectedIndex);
        }
        public void clFA3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag3.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((CLLipid)currentLipid).fag3, (TextBox)sender, clFA3Combobox.SelectedIndex);
        }
        public void clFA4TextboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag4.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((CLLipid)currentLipid).fag4, (TextBox)sender, clFA4Combobox.SelectedIndex);
        }
        
        public void clDB1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            update_ranges(((CLLipid)currentLipid).fag1, (TextBox)sender, 3);
            if (((CLLipid)currentLipid).representativeFA)
            {
                clDB2Textbox.Text = ((TextBox)sender).Text;
                clDB3Textbox.Text = ((TextBox)sender).Text;
                clDB4Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void clDB2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            update_ranges(((CLLipid)currentLipid).fag2, (TextBox)sender, 3);
        }
        public void clDB3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag3.dbInfo = ((TextBox)sender).Text;
            update_ranges(((CLLipid)currentLipid).fag3, (TextBox)sender, 3);
        }
        public void clDB4TextboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag4.dbInfo = ((TextBox)sender).Text;
            update_ranges(((CLLipid)currentLipid).fag4, (TextBox)sender, 3);
        }
        
        public void clHydroxyl1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag1.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((CLLipid)currentLipid).fag1, (TextBox)sender, 4);
            if (((CLLipid)currentLipid).representativeFA)
            {
                clHydroxyl2Textbox.Text = ((TextBox)sender).Text;
                clHydroxyl3Textbox.Text = ((TextBox)sender).Text;
                clHydroxyl4Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void clHydroxyl2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag2.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((CLLipid)currentLipid).fag2, (TextBox)sender, 4);
        }
        public void clHydroxyl3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag3.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((CLLipid)currentLipid).fag3, (TextBox)sender, 4);
        }
        public void clHydroxyl4TextboxValueChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag4.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((CLLipid)currentLipid).fag4, (TextBox)sender, 4);
        }
        
        public void clFA1Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag1.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((CLLipid)currentLipid).fag1.faTypes["FAx"] = !((CLLipid)currentLipid).fag1.anyFAChecked();
            if (((CLLipid)currentLipid).representativeFA)
            {
                clFA2Checkbox1.Checked = ((CheckBox)sender).Checked;
                clFA3Checkbox1.Checked = ((CheckBox)sender).Checked;
                clFA4Checkbox1.Checked = ((CheckBox)sender).Checked;
            }
        }
        public void clFA1Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((CLLipid)currentLipid).fag1.faTypes["FAx"] = !((CLLipid)currentLipid).fag1.anyFAChecked();
            if (((CLLipid)currentLipid).representativeFA)
            {
                clFA2Checkbox2.Checked = ((CheckBox)sender).Checked;
                clFA3Checkbox2.Checked = ((CheckBox)sender).Checked;
                clFA4Checkbox2.Checked = ((CheckBox)sender).Checked;
            }
        }
        public void clFA1Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag1.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((CLLipid)currentLipid).fag1.faTypes["FAx"] = !((CLLipid)currentLipid).fag1.anyFAChecked();
            if (((CLLipid)currentLipid).representativeFA)
            {
                clFA2Checkbox3.Checked = ((CheckBox)sender).Checked;
                clFA3Checkbox3.Checked = ((CheckBox)sender).Checked;
                clFA4Checkbox3.Checked = ((CheckBox)sender).Checked;
            }
        }
        
        public void clFA2Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag2.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((CLLipid)currentLipid).fag2.faTypes["FAx"] = !((CLLipid)currentLipid).fag2.anyFAChecked();
        }
        public void clFA2Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag2.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((CLLipid)currentLipid).fag2.faTypes["FAx"] = !((CLLipid)currentLipid).fag2.anyFAChecked();
        }
        public void clFA2Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag2.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((CLLipid)currentLipid).fag2.faTypes["FAx"] = !((CLLipid)currentLipid).fag2.anyFAChecked();
        }
        
        public void clFA3Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag3.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((CLLipid)currentLipid).fag3.faTypes["FAx"] = !((CLLipid)currentLipid).fag3.anyFAChecked();
        }
        public void clFA3Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag3.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((CLLipid)currentLipid).fag3.faTypes["FAx"] = !((CLLipid)currentLipid).fag3.anyFAChecked();
        }
        public void clFA3Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag3.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((CLLipid)currentLipid).fag3.faTypes["FAx"] = !((CLLipid)currentLipid).fag3.anyFAChecked();
        }
        
        public void clFA4Checkbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag4.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((CLLipid)currentLipid).fag4.faTypes["FAx"] = !((CLLipid)currentLipid).fag4.anyFAChecked();
        }
        public void clFA4Checkbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag4.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((CLLipid)currentLipid).fag4.faTypes["FAx"] = !((CLLipid)currentLipid).fag4.anyFAChecked();
        }
        public void clFA4Checkbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).fag4.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((CLLipid)currentLipid).fag4.faTypes["FAx"] = !((CLLipid)currentLipid).fag4.anyFAChecked();
        }
        
        public void clPosAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        public void clPosAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        public void clPosAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        public void cl_pos_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).adducts["+Na"] = ((CheckBox)sender).Checked;
        }
        public void clNegAdductCheckbox1CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        public void clNegAdductCheckbox2CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        public void clNegAdductCheckbox3CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        public void clNegAdductCheckbox4CheckedChanged(Object sender, EventArgs e)
        {
            ((CLLipid)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
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
            ((CLLipid)currentLipid).representativeFA = ((CheckBox)sender).Checked;
            if (((CLLipid)currentLipid).representativeFA)
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
            update_ranges(((CLLipid)currentLipid).fag2, clFA2Textbox, clFA2Combobox.SelectedIndex);
            update_ranges(((CLLipid)currentLipid).fag3, clFA3Textbox, clFA3Combobox.SelectedIndex);
            update_ranges(((CLLipid)currentLipid).fag4, clFA4Textbox, clFA4Combobox.SelectedIndex);
            update_ranges(((CLLipid)currentLipid).fag2, clDB2Textbox, 3);
            update_ranges(((CLLipid)currentLipid).fag3, clDB3Textbox, 3);
            update_ranges(((CLLipid)currentLipid).fag4, clDB4Textbox, 3);
            update_ranges(((CLLipid)currentLipid).fag2, clHydroxyl2Textbox, 4);
            update_ranges(((CLLipid)currentLipid).fag3, clHydroxyl3Textbox, 4);
            update_ranges(((CLLipid)currentLipid).fag4, clHydroxyl4Textbox, 4);
        }
        
        
        
        ////////////////////// GL ////////////////////////////////
        
        public void glFA1ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag1.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((GLLipid)currentLipid).fag1, glFA1Textbox, ((ComboBox)sender).SelectedIndex);
            if (((GLLipid)currentLipid).representativeFA)
            {
                glFA2Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                glFA3Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
            }
        }
        public void glFA2ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((GLLipid)currentLipid).fag2, glFA2Textbox, ((ComboBox)sender).SelectedIndex);
        }
        public void glFA3ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag3.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((GLLipid)currentLipid).fag3, glFA3Textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void glFA1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((GLLipid)currentLipid).fag1, (TextBox)sender, glFA1Combobox.SelectedIndex);
            if (((GLLipid)currentLipid).representativeFA)
            {
                glFA2Textbox.Text = ((TextBox)sender).Text;
                glFA3Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void glFA2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((GLLipid)currentLipid).fag2, (TextBox)sender, glFA2Combobox.SelectedIndex);
        }
        public void glFA3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag3.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((GLLipid)currentLipid).fag3, (TextBox)sender, glFA3Combobox.SelectedIndex);
        }
        
        public void glDB1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            update_ranges(((GLLipid)currentLipid).fag1, (TextBox)sender, 3);
            if (((GLLipid)currentLipid).representativeFA)
            {
                glDB2Textbox.Text = ((TextBox)sender).Text;
                glDB3Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void glDB2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            update_ranges(((GLLipid)currentLipid).fag2, (TextBox)sender, 3);
        }
        public void glDB3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag3.dbInfo = ((TextBox)sender).Text;
            update_ranges(((GLLipid)currentLipid).fag3, (TextBox)sender, 3);
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
        public void gl_pos_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).adducts["+Na"] = ((CheckBox)sender).Checked;
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
            update_ranges(((GLLipid)currentLipid).fag1, (TextBox)sender, 4);
            if (((GLLipid)currentLipid).representativeFA)
            {
                glHydroxyl2Textbox.Text = ((TextBox)sender).Text;
                glHydroxyl3Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void glHydroxyl2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag2.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((GLLipid)currentLipid).fag2, (TextBox)sender, 4);
        }
        public void glHydroxyl3TextboxValueChanged(Object sender, EventArgs e)
        {
            ((GLLipid)currentLipid).fag3.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((GLLipid)currentLipid).fag3, (TextBox)sender, 4);
        }
        
        public void triggerEasteregg(Object sender, EventArgs e)
        {
            easterText.Left = 1030;
            easterText.Visible = true;
            this.timer1.Enabled = true;
        }
        
        public void sugarHeady(Object sender, EventArgs e)
        {
            MessageBox.Show("Who is your sugar heady?");
        }
        
        private void timer1_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            easterText.Left -= 10;
            if (easterText.Left < -easterText.Width){
                this.timer1.Enabled = false;
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
            if (setting_listbox) return;
            ((GLLipid)currentLipid).hgValues.Clear();
            foreach(object itemChecked in ((ListBox)sender).SelectedItems)
            {
                int hgValue = ((ListBox)sender).Items.IndexOf(itemChecked);
                ((GLLipid)currentLipid).hgValues.Add(hgValue);
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
            update_ranges(((GLLipid)currentLipid).fag2, glFA2Textbox, glFA2Combobox.SelectedIndex);
            update_ranges(((GLLipid)currentLipid).fag3, glFA3Textbox, glFA3Combobox.SelectedIndex);
            update_ranges(((GLLipid)currentLipid).fag2, glDB2Textbox, 3);
            update_ranges(((GLLipid)currentLipid).fag3, glDB3Textbox, 3);
            update_ranges(((GLLipid)currentLipid).fag2, glHydroxyl2Textbox, 4);
            update_ranges(((GLLipid)currentLipid).fag3, glHydroxyl3Textbox, 4);
        }
        
        
        
        ////////////////////// PL ////////////////////////////////
        
        
    
        public void plFA1ComboboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((PLLipid)currentLipid).fag1, plFA1Textbox, ((ComboBox)sender).SelectedIndex);
            if (((PLLipid)currentLipid).representativeFA)
            {
                plFA2Combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
            }
        }
        public void pl_fa_2_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((PLLipid)currentLipid).fag2, plFA2Textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void plFA1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((PLLipid)currentLipid).fag1, (TextBox)sender, plFA1Combobox.SelectedIndex);
            if (((PLLipid)currentLipid).representativeFA)
            {
                plFA2Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void plFA2TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((PLLipid)currentLipid).fag2, (TextBox)sender, plFA2Combobox.SelectedIndex);
        }
        
        public void plDB1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            update_ranges(((PLLipid)currentLipid).fag1, (TextBox)sender, 3);
            if (((PLLipid)currentLipid).representativeFA)
            {
                plDB2Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void pl_db_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            update_ranges(((PLLipid)currentLipid).fag2, (TextBox)sender, 3);
        }
        public void plHydroxyl1TextboxValueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag1.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((PLLipid)currentLipid).fag1, (TextBox)sender, 4);
            if (((PLLipid)currentLipid).representativeFA)
            {
                plHydroxyl2Textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void pl_hydroxyl_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((PLLipid)currentLipid).fag2, (TextBox)sender, 4);
        }
        
        public void pl_pos_adduct_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        public void pl_pos_adduct_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        public void pl_pos_adduct_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        public void pl_pos_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+Na"] = ((CheckBox)sender).Checked;
        }
        public void pl_neg_adduct_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        public void pl_neg_adduct_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        public void pl_neg_adduct_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        public void pl_neg_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
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
        
        public void pl_fa_2_gb_1_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag2.faTypes["FAx"] = !((PLLipid)currentLipid).fag2.anyFAChecked();
        }
        public void pl_fa_2_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag2.faTypes["FAx"] = !((PLLipid)currentLipid).fag2.anyFAChecked();
        }
        public void pl_fa_2_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((PLLipid)currentLipid).fag2.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((PLLipid)currentLipid).fag2.faTypes["FAx"] = !((PLLipid)currentLipid).fag2.anyFAChecked();
        }
        
        public void pl_is_cl_checkedChanged(Object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                plPictureBox.Image = cardioBackboneImage;
                plPictureBox.Location = new Point(5, 5);
                plHgListbox.Visible = false;
                plHGLabel.Visible = false;
                plAddLipidButton.Visible = false;
                plResetLipidButton.Visible = false;
                plModifyLipidButton.Visible = false;
                plMS2fragmentsLipidButton.Visible = false;
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
                clAddLipidButton.Visible = true;
                clResetLipidButton.Visible = true;
                clModifyLipidButton.Visible = true;
                clMS2fragmentsLipidButton.Visible = true;
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
                
                changeTab(0);
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
                clAddLipidButton.Visible = false;
                clResetLipidButton.Visible = false;
                clModifyLipidButton.Visible = false;
                clMS2fragmentsLipidButton.Visible = false;
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
                plAddLipidButton.Visible = true;
                plResetLipidButton.Visible = true;
                plModifyLipidButton.Visible = true;
                plMS2fragmentsLipidButton.Visible = true;
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
                
                changeTab(2);
            }
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

        
        void pl_fa_2_gb_1_checkbox_3_MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = phosphoBackboneImage;
            plPictureBox.SendToBack();
        }
        private void pl_fa_2_gb_1_checkbox_3_MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = phosphoBackboneImageFA2e;
            plPictureBox.SendToBack();
        }
        void pl_fa_2_gb_1_checkbox_2_MouseLeave(object sender, EventArgs e)
        {
            plPictureBox.Image = phosphoBackboneImage;
            plPictureBox.SendToBack();
        }

        private void pl_fa_2_gb_1_checkbox_2_MouseHover(object sender, MouseEventArgs e)
        {
            plPictureBox.Image = phosphoBackboneImageFA2p;
            plPictureBox.SendToBack();
        }
        
        private void pl_hg_listbox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            if (setting_listbox) return;
            ((PLLipid)currentLipid).hgValues.Clear();
            foreach(object itemChecked in ((ListBox)sender).SelectedItems)
            {
                int hgValue = ((ListBox)sender).Items.IndexOf(itemChecked);
                ((PLLipid)currentLipid).hgValues.Add(hgValue);
            }
            
        }
        
        public void plRepresentativeFA_checkedChanged(Object sender, EventArgs e)
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
                plFA2Checkbox2.Checked = plFA1Checkbox2.Checked;
                plFA2Checkbox3.Checked = plFA1Checkbox3.Checked;
                
                
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
            update_ranges(((PLLipid)currentLipid).fag2, plFA2Textbox, plFA2Combobox.SelectedIndex);
            update_ranges(((PLLipid)currentLipid).fag2, plDB2Textbox, 3);
            update_ranges(((PLLipid)currentLipid).fag2, plHydroxyl2Textbox, 4);
        }
        
        void pl_hg_listbox_MouseLeave(object sender, EventArgs e)
        {
            plPosAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            plPosAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            plPosAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            plNegAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            plNegAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            plNegAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            plNegAdductCheckbox4.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
        }

        void pl_hg_listbox_MouseHover(object sender, EventArgs e)
        {
            Point point = plHgListbox.PointToClient(Cursor.Position);
            int hoveredIndex = plHgListbox.IndexFromPoint(point);

            if (hoveredIndex != -1)
            {
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)plHgListbox.Items[hoveredIndex]]["+H"]) plPosAdductCheckbox1.BackColor = highlightedCheckboxColor;
                else plPosAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)plHgListbox.Items[hoveredIndex]]["+2H"]) plPosAdductCheckbox2.BackColor = highlightedCheckboxColor;
                else plPosAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)plHgListbox.Items[hoveredIndex]]["+NH4"]) plPosAdductCheckbox3.BackColor = highlightedCheckboxColor;
                else plPosAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)plHgListbox.Items[hoveredIndex]]["-H"]) plNegAdductCheckbox1.BackColor = highlightedCheckboxColor;
                else plNegAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)plHgListbox.Items[hoveredIndex]]["-2H"]) plNegAdductCheckbox2.BackColor = highlightedCheckboxColor;
                else plNegAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)plHgListbox.Items[hoveredIndex]]["+HCOO"]) plNegAdductCheckbox3.BackColor = highlightedCheckboxColor;
                else plNegAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)plHgListbox.Items[hoveredIndex]]["+CH3COO"]) plNegAdductCheckbox4.BackColor = highlightedCheckboxColor;
                else plNegAdductCheckbox4.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            }
        }
        
        ////////////////////// SL ////////////////////////////////
        
        
        
        public void sl_pos_adduct_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        public void sl_pos_adduct_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        public void sl_pos_adduct_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        public void sl_pos_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["+Na"] = ((CheckBox)sender).Checked;
        }
        public void sl_neg_adduct_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        public void sl_neg_adduct_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        public void sl_neg_adduct_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        public void sl_neg_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
        }
        
        /*
        public void sl_hg_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).hgValue = ((ComboBox)sender).SelectedIndex;
            String headgroup = ((ComboBox)sender).SelectedItem.ToString();
            if (headgroup == "SPH" || headgroup == "S1P" || headgroup == "SPC"){
                ((sl_lipid)currentLipid).fag.disabled = true;
                sl_fa_combobox.Enabled = false;
                sl_fa_textbox.Enabled = false;
                sl_db_1_textbox.Enabled = false;
                sl_fa_hydroxy_combobox.Enabled = false;
            }
            else
            {
                ((sl_lipid)currentLipid).fag.disabled = false;
                sl_fa_combobox.Enabled = true;
                sl_fa_textbox.Enabled = true;
                sl_db_1_textbox.Enabled = true;
                sl_fa_hydroxy_combobox.Enabled = true;
            }
        }*/
        
        public void sl_db_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).fag.dbInfo = ((TextBox)sender).Text;
            update_ranges(((SLLipid)currentLipid).fag, (TextBox)sender, 3);
        }
        public void sl_db_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).lcb.dbInfo = ((TextBox)sender).Text;
            update_ranges(((SLLipid)currentLipid).lcb, (TextBox)sender, 3);
        }
        
        public void sl_fa_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).fag.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((SLLipid)currentLipid).fag, (TextBox)sender, slFACombobox.SelectedIndex);
        }
        public void sl_lcb_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).lcb.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((SLLipid)currentLipid).lcb, (TextBox)sender, slLCBCombobox.SelectedIndex, true);
        }
        
        
        public void sl_fa_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).fag.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((SLLipid)currentLipid).fag, slFATextbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void sl_lcb_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).lcb.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((SLLipid)currentLipid).lcb, slLCBTextbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void sl_lcb_hydroxy_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).longChainBaseHydroxyl = ((ComboBox)sender).SelectedIndex + 2;
        }
        
        public void sl_fa_hydroxy_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((SLLipid)currentLipid).fattyAcidHydroxyl = ((ComboBox)sender).SelectedIndex;
        }
        
        private void sl_hg_listbox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            if (setting_listbox) return;
            ((SLLipid)currentLipid).hgValues.Clear();
            foreach(object itemChecked in ((ListBox)sender).SelectedItems)
            {
                int hgValue = ((ListBox)sender).Items.IndexOf(itemChecked);
                ((SLLipid)currentLipid).hgValues.Add(hgValue);
            }
        }
        
        void sl_hg_listbox_MouseLeave(object sender, EventArgs e)
        {
            slPosAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            slPosAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            slPosAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            slNegAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            slNegAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            slNegAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            slNegAdductCheckbox4.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
        }

        void sl_hg_listbox_MouseHover(object sender, EventArgs e)
        {
            Point point = slHgListbox.PointToClient(Cursor.Position);
            int hoveredIndex = slHgListbox.IndexFromPoint(point);

            if (hoveredIndex != -1)
            {
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)slHgListbox.Items[hoveredIndex]]["+H"]) slPosAdductCheckbox1.BackColor = highlightedCheckboxColor;
                else slPosAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)slHgListbox.Items[hoveredIndex]]["+2H"]) slPosAdductCheckbox2.BackColor = highlightedCheckboxColor;
                else slPosAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)slHgListbox.Items[hoveredIndex]]["+NH4"]) slPosAdductCheckbox3.BackColor = highlightedCheckboxColor;
                else slPosAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)slHgListbox.Items[hoveredIndex]]["-H"]) slNegAdductCheckbox1.BackColor = highlightedCheckboxColor;
                else slNegAdductCheckbox1.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)slHgListbox.Items[hoveredIndex]]["-2H"]) slNegAdductCheckbox2.BackColor = highlightedCheckboxColor;
                else slNegAdductCheckbox2.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)slHgListbox.Items[hoveredIndex]]["+HCOO"]) slNegAdductCheckbox3.BackColor = highlightedCheckboxColor;
                else slNegAdductCheckbox3.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
                
                if (lipidCreatorForm.headgroupAdductRestrictions[(string)slHgListbox.Items[hoveredIndex]]["+CH3COO"]) slNegAdductCheckbox4.BackColor = highlightedCheckboxColor;
                else slNegAdductCheckbox4.BackColor = Color.FromArgb(DefaultCheckboxBGR, DefaultCheckboxBGG, DefaultCheckboxBGB);
            }
        }
        
        
        
        ////////////////////// Remaining parts ////////////////////////////////
        
        public void modify_cl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.registeredLipids[lipid_modifications[0]] = new CLLipid((CLLipid)currentLipid);
            refresh_registered_lipids_table();
        }
        
        public void modify_gl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.registeredLipids[lipid_modifications[1]] = new GLLipid((GLLipid)currentLipid);
            refresh_registered_lipids_table();
        }
        
        public void modify_pl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.registeredLipids[lipid_modifications[2]] = new PLLipid((PLLipid)currentLipid);
            refresh_registered_lipids_table();
        }
        
        public void modify_sl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.registeredLipids[lipid_modifications[3]] = new SLLipid((SLLipid)currentLipid);
            refresh_registered_lipids_table();
        }
        
        public void modify_lipid(Object sender, EventArgs e)
        {
            int cnt_active_adducts = 0;
            foreach (KeyValuePair<String, bool> adduct in currentLipid.adducts)
            {
                cnt_active_adducts += adduct.Value ? 1 : 0;
            }
            if (cnt_active_adducts < 1)
            {
                MessageBox.Show("No adduct selected!", "Not registrable");
                return;
            }
        
            if (currentLipid is CLLipid)
            {   
                if (((CLLipid)currentLipid).fag1.faTypes["FAx"] || ((CLLipid)currentLipid).fag2.faTypes["FAx"] || ((CLLipid)currentLipid).fag3.faTypes["FAx"])
                {
                    MessageBox.Show("At least the top three fatty acids must be selected!", "Not registrable");
                    return;
                }
            
                if (clFA1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (clFA2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (clFA3Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Third fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (clFA4Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Fourth fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (clDB1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First double bond content not valid!", "Not registrable");
                    return;
                }
                if (clDB2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second double bond content not valid!", "Not registrable");
                    return;
                }
                if (clDB3Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Third double bond content not valid!", "Not registrable");
                    return;
                }
                if (clDB4Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Fourth double bond content not valid!", "Not registrable");
                    return;
                }
                if (clHydroxyl1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First hydroxyl content not valid!", "Not registrable");
                    return;
                }
                if (clHydroxyl2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second hydroxyl content not valid!", "Not registrable");
                    return;
                }
                if (clHydroxyl3Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Third hydroxyl content not valid!", "Not registrable");
                    return;
                }
                if (clHydroxyl4Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Fourth hydroxyl content not valid!", "Not registrable");
                    return;
                }
                lipidCreatorForm.registeredLipids[lipid_modifications[0]] = new CLLipid((CLLipid)currentLipid);
            }
            
            else if (currentLipid is GLLipid)
            {
                if (((GLLipid)currentLipid).fag1.faTypes["FAx"])
                {
                    MessageBox.Show("Please always select the top fatty acid!", "Not registrable");
                    return;
                }
                else if (((GLLipid)currentLipid).fag2.faTypes["FAx"] && !((GLLipid)currentLipid).fag3.faTypes["FAx"])
                {
                    MessageBox.Show("Please select the middle fatty acid for DG!", "Not registrable");
                    return;
                }
                
                if (glFA1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (glFA2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (glDB1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First double bond content not valid!", "Not registrable");
                    return;
                }
                if (glDB2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second double bond content not valid!", "Not registrable");
                    return;
                }
                if (glHydroxyl1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First hydroxyl content not valid!", "Not registrable");
                    return;
                }
                if (glHydroxyl2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second hydroxyl content not valid!", "Not registrable");
                    return;
                }
                if (((GLLipid)currentLipid).containsSugar)
                {
                    if (((GLLipid)currentLipid).hgValues.Count == 0)
                    {
                        MessageBox.Show("No head group selected!", "Not registrable");
                        return;                    
                    }
                    if (((GLLipid)currentLipid).fag1.faTypes["FAx"] || ((GLLipid)currentLipid).fag2.faTypes["FAx"])
                    {
                        MessageBox.Show("Both fatty acids must be selected!", "Not registrable");
                        return;
                    }
                }
                else
                {
                    if (((GLLipid)currentLipid).fag1.faTypes["FAx"] && ((GLLipid)currentLipid).fag2.faTypes["FAx"] && ((GLLipid)currentLipid).fag3.faTypes["FAx"])
                    {
                        MessageBox.Show("No fatty acid selected!", "Not registrable");
                        return;
                    }
                    if (glFA3Textbox.BackColor == alert_color)
                    {
                        MessageBox.Show("Third fatty acid length content not valid!", "Not registrable");
                        return;
                    }
                    if (glDB3Textbox.BackColor == alert_color)
                    {
                        MessageBox.Show("Third double bond content not valid!", "Not registrable");
                        return;
                    }
                    if (glHydroxyl3Textbox.BackColor == alert_color)
                    {
                        MessageBox.Show("Third hydroxyl content not valid!", "Not registrable");
                        return;
                    }
                }
                lipidCreatorForm.registeredLipids[lipid_modifications[1]] = new GLLipid((GLLipid)currentLipid);
            }
            
            
            else if (currentLipid is PLLipid)
            {
                if (((PLLipid)currentLipid).hgValues.Count == 0)
                {
                    MessageBox.Show("No head group selected!", "Not registrable");
                    return;                    
                }
                
                if (((PLLipid)currentLipid).fag1.faTypes["FAx"])
                {
                    MessageBox.Show("Please select at least the top fatty acid!", "Not registrable");
                    return;
                }
                
                if (plFA1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (plFA2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (plDB1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First double bond content not valid!", "Not registrable");
                    return;
                }
                if (plDB2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second double bond content not valid!", "Not registrable");
                    return;
                }
                if (plHydroxyl1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First hydroxyl content not valid!", "Not registrable");
                    return;
                }
                if (plHydroxyl2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second hydroxyl content not valid!", "Not registrable");
                    return;
                }
            
                lipidCreatorForm.registeredLipids[lipid_modifications[2]] = new PLLipid((PLLipid)currentLipid);
            }
            
            
            else if (currentLipid is SLLipid)
            {
                if (((SLLipid)currentLipid).hgValues.Count == 0)
                {
                    MessageBox.Show("No head group selected!", "Not registrable");
                    return;                    
                }
                
                if (slLCBTextbox.BackColor == alert_color)
                {
                    MessageBox.Show("Long chain base length content not valid!", "Not registrable");
                    return;
                }
                if (slFATextbox.BackColor == alert_color)
                {
                    MessageBox.Show("Fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (slDB1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("FA double bond content not valid!", "Not registrable");
                    return;
                }
                if (slDB2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("LCB double bond content not valid!", "Not registrable");
                    return;
                }
                
                lipidCreatorForm.registeredLipids[lipid_modifications[3]] = new SLLipid((SLLipid)currentLipid);
            }
            refresh_registered_lipids_table();
        }
        
        
        
        public void register_lipid(Object sender, EventArgs e)
        {
            int cnt_active_adducts = 0;
            foreach (KeyValuePair<String, bool> adduct in currentLipid.adducts)
            {
                cnt_active_adducts += adduct.Value ? 1 : 0;
            }
            if (cnt_active_adducts < 1)
            {
                MessageBox.Show("No adduct selected!", "Not registrable");
                return;
            }
        
            if (currentLipid is CLLipid)
            {   
                if (((CLLipid)currentLipid).fag1.faTypes["FAx"] || ((CLLipid)currentLipid).fag2.faTypes["FAx"] || ((CLLipid)currentLipid).fag3.faTypes["FAx"])
                {
                    MessageBox.Show("At least the top three fatty acids must be selected!", "Not registrable");
                    return;
                }
            
                if (clFA1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (clFA2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (clFA3Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Third fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (clFA4Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Fourth fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (clDB1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First double bond content not valid!", "Not registrable");
                    return;
                }
                if (clDB2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second double bond content not valid!", "Not registrable");
                    return;
                }
                if (clDB3Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Third double bond content not valid!", "Not registrable");
                    return;
                }
                if (clDB4Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Fourth double bond content not valid!", "Not registrable");
                    return;
                }
                if (clHydroxyl1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First hydroxyl content not valid!", "Not registrable");
                    return;
                }
                if (clHydroxyl2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second hydroxyl content not valid!", "Not registrable");
                    return;
                }
                if (clHydroxyl3Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Third hydroxyl content not valid!", "Not registrable");
                    return;
                }
                if (clHydroxyl4Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Fourth hydroxyl content not valid!", "Not registrable");
                    return;
                }
                
                lipidCreatorForm.registeredLipids.Add(new CLLipid((CLLipid)currentLipid));
            }
            
            else if (currentLipid is GLLipid)
            {
                if (((GLLipid)currentLipid).fag1.faTypes["FAx"])
                {
                    MessageBox.Show("Please always select the top fatty acid!", "Not registrable");
                    return;
                }
                else if (((GLLipid)currentLipid).fag2.faTypes["FAx"] && !((GLLipid)currentLipid).fag3.faTypes["FAx"])
                {
                    MessageBox.Show("Please select the middle fatty acid for DG!", "Not registrable");
                    return;
                }
            
                if (glFA1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (glFA2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (glDB1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First double bond content not valid!", "Not registrable");
                    return;
                }
                if (glDB2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second double bond content not valid!", "Not registrable");
                    return;
                }
                if (glHydroxyl1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First hydroxyl content not valid!", "Not registrable");
                    return;
                }
                if (glHydroxyl2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second hydroxyl content not valid!", "Not registrable");
                    return;
                }
                if (((GLLipid)currentLipid).containsSugar)
                {
                    if (((GLLipid)currentLipid).hgValues.Count == 0)
                    {
                        MessageBox.Show("No head group selected!", "Not registrable");
                        return;                    
                    }
                    if (((GLLipid)currentLipid).fag1.faTypes["FAx"] || ((GLLipid)currentLipid).fag2.faTypes["FAx"])
                    {
                        MessageBox.Show("Both fatty acids must be selected!", "Not registrable");
                        return;
                    }
                }
                else
                {
                    if (((GLLipid)currentLipid).fag1.faTypes["FAx"] && ((GLLipid)currentLipid).fag2.faTypes["FAx"] && ((GLLipid)currentLipid).fag3.faTypes["FAx"])
                    {
                        MessageBox.Show("No fatty acid selected!", "Not registrable");
                        return;
                    }
                    if (glFA3Textbox.BackColor == alert_color)
                    {
                        MessageBox.Show("Third fatty acid length content not valid!", "Not registrable");
                        return;
                    }
                    if (glDB3Textbox.BackColor == alert_color)
                    {
                        MessageBox.Show("Third double bond content not valid!", "Not registrable");
                        return;
                    }
                    if (glHydroxyl3Textbox.BackColor == alert_color)
                    {
                        MessageBox.Show("Third hydroxyl content not valid!", "Not registrable");
                        return;
                    }
                }
                lipidCreatorForm.registeredLipids.Add(new GLLipid((GLLipid)currentLipid));
            }
            
            
            else if (currentLipid is PLLipid)
            {
                if (((PLLipid)currentLipid).hgValues.Count == 0)
                {
                    MessageBox.Show("No head group selected!", "Not registrable");
                    return;                    
                }
                
                if (((PLLipid)currentLipid).fag1.faTypes["FAx"])
                {
                    MessageBox.Show("Please select at least the top fatty acid!", "Not registrable");
                    return;
                }
                
                if (plFA1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (plFA2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (plDB1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First double bond content not valid!", "Not registrable");
                    return;
                }
                if (plDB2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second double bond content not valid!", "Not registrable");
                    return;
                }
                if (plHydroxyl1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First hydroxyl content not valid!", "Not registrable");
                    return;
                }
                if (plHydroxyl2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second hydroxyl content not valid!", "Not registrable");
                    return;
                }
            
                lipidCreatorForm.registeredLipids.Add(new PLLipid((PLLipid)currentLipid));
            }
            
            
            else if (currentLipid is SLLipid)
            {
                if (((SLLipid)currentLipid).hgValues.Count == 0)
                {
                    MessageBox.Show("No head group selected!", "Not registrable");
                    return;                    
                }
                
                if (slLCBTextbox.BackColor == alert_color)
                {
                    MessageBox.Show("Long chain base length content not valid!", "Not registrable");
                    return;
                }
                if (slFATextbox.BackColor == alert_color)
                {
                    MessageBox.Show("Fatty acid length content not valid!", "Not registrable");
                    return;
                }
                if (slDB1Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("FA double bond content not valid!", "Not registrable");
                    return;
                }
                if (slDB2Textbox.BackColor == alert_color)
                {
                    MessageBox.Show("LCB double bond content not valid!", "Not registrable");
                    return;
                }
                
                lipidCreatorForm.registeredLipids.Add(new SLLipid((SLLipid)currentLipid));
            }
            refresh_registered_lipids_table();
        }
        
        public void refresh_registered_lipids_table()
        {
            registered_lipids_datatable.Clear();
            foreach (Lipid current_lipid in lipidCreatorForm.registeredLipids)
            {
                DataRow row = registered_lipids_datatable.NewRow();
                if (current_lipid is CLLipid)
                {
                    CLLipid currentCLLipid = (CLLipid)current_lipid;
                    row["Category"] = "Cardiolipin";
                    row["Building Block 1"] = "FA: " + currentCLLipid.fag1.lengthInfo + "; DB: " + currentCLLipid.fag1.dbInfo + "; OH: " + currentCLLipid.fag1.hydroxylInfo;
                    row["Building Block 2"] = "FA: " + currentCLLipid.fag2.lengthInfo + "; DB: " + currentCLLipid.fag2.dbInfo + "; OH: " + currentCLLipid.fag2.hydroxylInfo;
                    row["Building Block 3"] = "FA: " + currentCLLipid.fag3.lengthInfo + "; DB: " + currentCLLipid.fag3.dbInfo + "; OH: " + currentCLLipid.fag3.hydroxylInfo;
                    row["Building Block 4"] = "FA: " + currentCLLipid.fag4.lengthInfo + "; DB: " + currentCLLipid.fag4.dbInfo + "; OH: " + currentCLLipid.fag4.hydroxylInfo;
                }
                else if (current_lipid is GLLipid)
                {
                    GLLipid currentGLLipid = (GLLipid)current_lipid;
                    row["Category"] = "Glycerolipid";
                    row["Building Block 1"] = "FA: " + currentGLLipid.fag1.lengthInfo + "; DB: " + currentGLLipid.fag1.dbInfo + "; OH: " + currentGLLipid.fag1.hydroxylInfo;
                    row["Building Block 2"] = "FA: " + currentGLLipid.fag2.lengthInfo + "; DB: " + currentGLLipid.fag2.dbInfo + "; OH: " + currentGLLipid.fag2.hydroxylInfo;
                    if (currentGLLipid.containsSugar)
                    {
                        String headgroups = "";
                        foreach (int hgValue in currentGLLipid.hgValues)
                        {
                            if (headgroups != "") headgroups += ", ";
                            headgroups += currentGLLipid.headGroupNames[hgValue];
                        }
                        row["Building Block 3"] = "HG: " + headgroups;
                    }
                    else
                    {
                        row["Building Block 3"] = "FA: " + currentGLLipid.fag3.lengthInfo + "; DB: " + currentGLLipid.fag3.dbInfo + "; OH: " + currentGLLipid.fag3.hydroxylInfo;
                    }
                }
                else if (current_lipid is PLLipid)
                {
                    PLLipid currentPLLipid = (PLLipid)current_lipid;
                    String headgroups = "";
                    foreach (int hgValue in currentPLLipid.hgValues)
                    {
                        if (headgroups != "") headgroups += ", ";
                        headgroups += currentPLLipid.headGroupNames[hgValue];
                    }
                    row["Category"] = "Phospholipid";
                    row["Building Block 1"] = "HG: " + headgroups;
                    row["Building Block 2"] = "FA: " + currentPLLipid.fag1.lengthInfo + "; DB: " + currentPLLipid.fag1.dbInfo + "; OH: " + currentPLLipid.fag1.hydroxylInfo;
                    row["Building Block 3"] = "FA: " + currentPLLipid.fag2.lengthInfo + "; DB: " + currentPLLipid.fag2.dbInfo + "; OH: " + currentPLLipid.fag2.hydroxylInfo;
                }
                else if (current_lipid is SLLipid)
                {
                    SLLipid currentSLLipid = (SLLipid)current_lipid;
                    String headgroups = "";
                    foreach (int hgValue in currentSLLipid.hgValues)
                    {
                        if (headgroups != "") headgroups += ", ";
                        headgroups += currentSLLipid.headGroupNames[hgValue];
                    }
                    row["Category"] = "Sphingolipid";
                    row["Building Block 1"] = "HG: " + headgroups;
                    row["Building Block 2"] = "LCB: " + currentSLLipid.lcb.lengthInfo + "; DB: " + currentSLLipid.lcb.dbInfo;
                    row["Building Block 3"] = "FA: " + currentSLLipid.fag.lengthInfo + "; DB: " + currentSLLipid.fag.dbInfo;
                }
                registered_lipids_datatable.Rows.Add(row);
                lipidsGridview.DataSource = registered_lipids_datatable;
                
                
                for (int i = 0; i < lipidsGridview.Rows.Count; ++i)
                {
                    lipidsGridview.Rows[i].Cells["Edit"].Value = editImage;
                    lipidsGridview.Rows[i].Cells["Delete"].Value = deleteImage;
                }
                lipidsGridview.Update();
                lipidsGridview.Refresh();
            }
        }
        
        public void lipids_gridview_double_click(Object sender, EventArgs e)
        {
            int rowIndex = ((DataGridView)sender).CurrentCell.RowIndex;
            int colIndex = ((DataGridView)sender).CurrentCell.ColumnIndex;
            if (((DataGridView)sender).Columns[colIndex].Name == "Edit")
            {
            
                Lipid current_lipid = (Lipid)lipidCreatorForm.registeredLipids[rowIndex];
                int tabIndex = 0;
                if (current_lipid is CLLipid)
                {
                    tabIndex = 0;
                    lipidCreatorForm.lipidTabList[tabIndex] = new CLLipid((CLLipid)current_lipid);
                }
                else if (current_lipid is GLLipid)
                {
                    tabIndex = 1;
                    lipidCreatorForm.lipidTabList[tabIndex] = new GLLipid((GLLipid)current_lipid);
                }
                else if (current_lipid is PLLipid)
                {
                    tabIndex = 2;
                    lipidCreatorForm.lipidTabList[tabIndex] = new PLLipid((PLLipid)current_lipid);
                }
                else if (current_lipid is SLLipid)
                {
                    tabIndex = 3;
                    lipidCreatorForm.lipidTabList[tabIndex] = new SLLipid((SLLipid)current_lipid);
                }
                currentLipid = current_lipid;
                lipid_modifications[tabIndex] = rowIndex;
                tabControl.SelectedIndex = tabIndex;
                changeTab(tabIndex);
                
            }
            else if (((DataGridView)sender).Columns[colIndex].Name == "Delete")
            {
                Lipid current_lipid = (Lipid)lipidCreatorForm.registeredLipids[rowIndex];
                int tabIndex = 0;
                if (current_lipid is CLLipid)
                {
                    tabIndex = 0;
                }
                else if (current_lipid is GLLipid)
                {
                    tabIndex = 1;
                }
                else if (current_lipid is PLLipid)
                {
                    tabIndex = 2;
                }
                else if (current_lipid is SLLipid)
                {
                    tabIndex = 3;
                }
                lipidCreatorForm.registeredLipids.RemoveAt(rowIndex);
                refresh_registered_lipids_table();
                lipid_modifications[tabIndex] = -1;
                if (tabIndex == currentTabIndex)
                {
                    changeTab(tabIndex);
                }
            }
        }
        
        
        public void lipids_gridview_keydown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lipidCreatorForm.registeredLipids.Count > 0 && ((DataGridView)sender).SelectedRows.Count > 0)
            {   
                lipidCreatorForm.registeredLipids.RemoveAt(((DataGridView)sender).SelectedRows[0].Index);
                refresh_registered_lipids_table();
                e.Handled = true;
            }
        }
        
        
        public void open_ms2_form(Object sender, EventArgs e)
        {
            MS2Form ms2fragments = new MS2Form(this, currentLipid);
            ms2fragments.Owner = this;
            ms2fragments.ShowInTaskbar = false;
            ms2fragments.ShowDialog();
            ms2fragments.Dispose();
        }
        
        
        public void open_review_Form(Object sender, EventArgs e)
        {
            lipidCreatorForm.assembleLipids();
            LipidsReview lipidsReview = new LipidsReview(lipidCreatorForm, lipidCreatorForm.allLipids, lipidCreatorForm.allLipidsUnique);
            lipidsReview.Owner = this;
            lipidsReview.ShowInTaskbar = false;
            lipidsReview.ShowDialog();
            lipidsReview.Dispose();
        }
        
        protected void menuImportPredefined_Click(object sender, System.EventArgs e)
        {
            System.Windows.Forms.MenuItem PredefItem = (System.Windows.Forms.MenuItem)sender;
            string filePath = (string)PredefItem.Tag;
            XDocument doc;
            try 
            {
                doc = XDocument.Load(filePath);
                lipidCreatorForm.import(doc);
                refresh_registered_lipids_table();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not read file", "Error while reading", MessageBoxButtons.OK);
            }
        }
        
        
        
        protected void menuImport_Click(object sender, System.EventArgs e)
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
                    lipidCreatorForm.import(doc);
                    refresh_registered_lipids_table();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not read file", "Error while reading", MessageBoxButtons.OK);
                }
            }
        }
        
        protected void menuExport_Click(object sender, System.EventArgs e)
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
                    writer.Write(lipidCreatorForm.serialize());
                    writer.Dispose();
                    writer.Close();
                }
            }
        }
        
        protected void menuExit_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }
    }
}
