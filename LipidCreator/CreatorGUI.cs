using System;
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
        public LipidCreatorForm lipidCreatorForm;
        public lipid currentLipid;
        public DataTable registered_lipids_datatable;
        public int[] lipid_modifications;
        public Color alert_color = Color.FromArgb(255, 180, 180);
        public bool setting_listbox = false;
        
        public CreatorGUI(LipidCreatorForm lipidCreatorForm)
        {
            this.lipidCreatorForm = lipidCreatorForm;
            
            registered_lipids_datatable = new DataTable("Daten");
            registered_lipids_datatable.Columns.Add(new DataColumn("Class"));
            registered_lipids_datatable.Columns.Add(new DataColumn("Building Block 1"));
            registered_lipids_datatable.Columns.Add(new DataColumn("Building Block 2"));
            registered_lipids_datatable.Columns.Add(new DataColumn("Building Block 3"));
            registered_lipids_datatable.Columns.Add(new DataColumn("Building Block 4"));
            InitializeComponent();
            lipid_modifications = new int[]{-1, -1, -1, -1};
            changing_tab_forced = false;
            
        }
        
        private void lipids_gridview_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            
            if (initialCall){
                DataGridViewImageColumn editColumn = new DataGridViewImageColumn();  
                editColumn.Name = "Edit";  
                editColumn.HeaderText = "Edit";  
                editColumn.ValuesAreIcons = false;
                lipids_gridview.Columns.Add(editColumn);
                DataGridViewImageColumn deleteColumn = new DataGridViewImageColumn();  
                deleteColumn.Name = "Delete";  
                deleteColumn.HeaderText = "Delete";  
                deleteColumn.ValuesAreIcons = false;
                lipids_gridview.Columns.Add(deleteColumn);
                int w = (lipids_gridview.Width - 80) / 5 - 4;
                foreach (DataGridViewColumn col in lipids_gridview.Columns)
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
            int tab_index = index - 1 >= 0 ? index - 1 : 1;
            tab_control.SelectedIndex = tab_index;
            changeTab(index, true);
            changing_tab_forced = false;
            
        }
        
        public void tabIndexChanged(Object sender,  EventArgs e)
        {
            if (changing_tab_forced) return;
            changeTab(((TabControl)sender).SelectedIndex + 1, false);            
        }

        public void changeTab(int index, bool forced)
        {     
            if ((index == 2 || index == 0) && !forced)
            {
                if (pl_is_cl.Checked)
                {
                    index = 0;
                }
                else
                {
                    index = 2;
                }
            }
        
            currentLipid = (lipid)lipidCreatorForm.lipidTabList[index];
            
            if (index == 0)
            {
                cl_lipid currentCLLipid = (cl_lipid)currentLipid;
                cl_fa_1_textbox.Text = currentCLLipid.fag1.lengthInfo;
                cl_db_1_textbox.Text = currentCLLipid.fag1.dbInfo;
                cl_hydroxyl_1_textbox.Text = currentCLLipid.fag1.hydroxylInfo;
                cl_fa_1_combobox.SelectedIndex = currentCLLipid.fag1.chainType;
                cl_fa_1_gb_1_checkbox_1.Checked = currentCLLipid.fag1.faTypes["FA"];
                cl_fa_1_gb_1_checkbox_2.Checked = currentCLLipid.fag1.faTypes["FAp"];
                cl_fa_1_gb_1_checkbox_3.Checked = currentCLLipid.fag1.faTypes["FAe"];
                
                cl_fa_2_textbox.Text = currentCLLipid.fag2.lengthInfo;
                cl_db_2_textbox.Text = currentCLLipid.fag2.dbInfo;
                cl_hydroxyl_2_textbox.Text = currentCLLipid.fag2.hydroxylInfo;
                cl_fa_2_combobox.SelectedIndex = currentCLLipid.fag2.chainType;
                cl_fa_2_gb_1_checkbox_1.Checked = currentCLLipid.fag2.faTypes["FA"];
                cl_fa_2_gb_1_checkbox_2.Checked = currentCLLipid.fag2.faTypes["FAp"];
                cl_fa_2_gb_1_checkbox_3.Checked = currentCLLipid.fag2.faTypes["FAe"];
                
                cl_fa_3_textbox.Text = currentCLLipid.fag3.lengthInfo;
                cl_db_3_textbox.Text = currentCLLipid.fag3.dbInfo;
                cl_hydroxyl_3_textbox.Text = currentCLLipid.fag3.hydroxylInfo;
                cl_fa_3_combobox.SelectedIndex = currentCLLipid.fag3.chainType;
                cl_fa_3_gb_1_checkbox_1.Checked = currentCLLipid.fag3.faTypes["FA"];
                cl_fa_3_gb_1_checkbox_2.Checked = currentCLLipid.fag3.faTypes["FAp"];
                cl_fa_3_gb_1_checkbox_3.Checked = currentCLLipid.fag3.faTypes["FAe"];
                
                cl_fa_4_textbox.Text = currentCLLipid.fag4.lengthInfo;
                cl_db_4_textbox.Text = currentCLLipid.fag4.dbInfo;
                cl_hydroxyl_4_textbox.Text = currentCLLipid.fag4.hydroxylInfo;
                cl_fa_4_combobox.SelectedIndex = currentCLLipid.fag4.chainType;
                cl_fa_4_gb_1_checkbox_1.Checked = currentCLLipid.fag4.faTypes["FA"];
                cl_fa_4_gb_1_checkbox_2.Checked = currentCLLipid.fag4.faTypes["FAp"];
                cl_fa_4_gb_1_checkbox_3.Checked = currentCLLipid.fag4.faTypes["FAe"];
                
                cl_pos_adduct_checkbox_1.Checked = currentCLLipid.adducts["+H"];
                cl_pos_adduct_checkbox_2.Checked = currentCLLipid.adducts["+2H"];
                cl_pos_adduct_checkbox_3.Checked = currentCLLipid.adducts["+NH4"];
                cl_pos_adduct_checkbox_4.Checked = currentCLLipid.adducts["+Na"];
                cl_neg_adduct_checkbox_1.Checked = currentCLLipid.adducts["-H"];
                cl_neg_adduct_checkbox_2.Checked = currentCLLipid.adducts["-2H"];
                cl_neg_adduct_checkbox_3.Checked = currentCLLipid.adducts["+HCOO"];
                cl_neg_adduct_checkbox_4.Checked = currentCLLipid.adducts["+CH3COO"];
                if (lipid_modifications[0] > -1) cl_modify_lipid_button.Enabled = true;
                else cl_modify_lipid_button.Enabled = false;
                
                pl_is_cl.Checked = true;
                
                update_ranges(currentCLLipid.fag1, cl_fa_1_textbox, cl_fa_1_combobox.SelectedIndex);
                update_ranges(currentCLLipid.fag1, cl_db_1_textbox, 3);
                update_ranges(currentCLLipid.fag1, cl_hydroxyl_1_textbox, 4);
                update_ranges(currentCLLipid.fag2, cl_fa_2_textbox, cl_fa_2_combobox.SelectedIndex);
                update_ranges(currentCLLipid.fag2, cl_db_2_textbox, 3);
                update_ranges(currentCLLipid.fag2, cl_hydroxyl_2_textbox, 4);
                update_ranges(currentCLLipid.fag3, cl_fa_3_textbox, cl_fa_3_combobox.SelectedIndex);
                update_ranges(currentCLLipid.fag3, cl_db_3_textbox, 3);
                update_ranges(currentCLLipid.fag3, cl_hydroxyl_3_textbox, 4);
                update_ranges(currentCLLipid.fag4, cl_fa_4_textbox, cl_fa_4_combobox.SelectedIndex);
                update_ranges(currentCLLipid.fag4, cl_db_4_textbox, 3);
                update_ranges(currentCLLipid.fag4, cl_hydroxyl_4_textbox, 4);
                
                clRepresentativeFA.Checked = currentCLLipid.representativeFA;
                cl_picture_box.SendToBack();
                
            }
            else if (index == 1)
            {
                gl_lipid currentGLLipid = (gl_lipid)currentLipid;
                setting_listbox = true;
                for (int i = 0; i < gl_hg_listbox.Items.Count; ++i)
                {
                    gl_hg_listbox.SetSelected(i, false);
                }
                foreach (int hgValue in currentGLLipid.hgValues)
                {
                    gl_hg_listbox.SetSelected(hgValue, true);
                }
                setting_listbox = false;
                
                gl_fa_1_textbox.Text = currentGLLipid.fag1.lengthInfo;
                gl_db_1_textbox.Text = currentGLLipid.fag1.dbInfo;
                gl_hydroxyl_1_textbox.Text = currentGLLipid.fag1.hydroxylInfo;
                gl_fa_1_combobox.SelectedIndex = currentGLLipid.fag1.chainType;
                gl_fa_1_gb_1_checkbox_1.Checked = currentGLLipid.fag1.faTypes["FA"];
                gl_fa_1_gb_1_checkbox_2.Checked = currentGLLipid.fag1.faTypes["FAp"];
                gl_fa_1_gb_1_checkbox_3.Checked = currentGLLipid.fag1.faTypes["FAe"];
                
                
                
                gl_fa_2_textbox.Text = currentGLLipid.fag2.lengthInfo;
                gl_db_2_textbox.Text = currentGLLipid.fag2.dbInfo;
                gl_hydroxyl_2_textbox.Text = currentGLLipid.fag2.hydroxylInfo;
                gl_fa_2_combobox.SelectedIndex = currentGLLipid.fag2.chainType;
                gl_fa_2_gb_1_checkbox_1.Checked = currentGLLipid.fag2.faTypes["FA"];
                gl_fa_2_gb_1_checkbox_2.Checked = currentGLLipid.fag2.faTypes["FAp"];
                gl_fa_2_gb_1_checkbox_3.Checked = currentGLLipid.fag2.faTypes["FAe"];
                
                gl_fa_3_textbox.Text = currentGLLipid.fag3.lengthInfo;
                gl_db_3_textbox.Text = currentGLLipid.fag3.dbInfo;
                gl_hydroxyl_3_textbox.Text = currentGLLipid.fag3.hydroxylInfo;
                gl_fa_3_combobox.SelectedIndex = currentGLLipid.fag3.chainType;
                gl_fa_3_gb_1_checkbox_1.Checked = currentGLLipid.fag3.faTypes["FA"];
                gl_fa_3_gb_1_checkbox_2.Checked = currentGLLipid.fag3.faTypes["FAp"];
                gl_fa_3_gb_1_checkbox_3.Checked = currentGLLipid.fag3.faTypes["FAe"];
                
                gl_pos_adduct_checkbox_1.Checked = currentGLLipid.adducts["+H"];
                gl_pos_adduct_checkbox_2.Checked = currentGLLipid.adducts["+2H"];
                gl_pos_adduct_checkbox_3.Checked = currentGLLipid.adducts["+NH4"];
                gl_pos_adduct_checkbox_4.Checked = currentGLLipid.adducts["+Na"];
                gl_neg_adduct_checkbox_1.Checked = currentGLLipid.adducts["-H"];
                gl_neg_adduct_checkbox_2.Checked = currentGLLipid.adducts["-2H"];
                gl_neg_adduct_checkbox_3.Checked = currentGLLipid.adducts["+HCOO"];
                gl_neg_adduct_checkbox_4.Checked = currentGLLipid.adducts["+CH3COO"];
                if (lipid_modifications[1] > -1) gl_modify_lipid_button.Enabled = true;
                else gl_modify_lipid_button.Enabled = false;
                
                gl_contains_sugar.Checked = currentGLLipid.contains_sugar;
                
                
                update_ranges(currentGLLipid.fag1, gl_fa_1_textbox, gl_fa_1_combobox.SelectedIndex);
                update_ranges(currentGLLipid.fag1, gl_db_1_textbox, 3);
                update_ranges(currentGLLipid.fag1, gl_hydroxyl_1_textbox, 4);
                update_ranges(currentGLLipid.fag2, gl_fa_2_textbox, gl_fa_2_combobox.SelectedIndex);
                update_ranges(currentGLLipid.fag2, gl_db_2_textbox, 3);
                update_ranges(currentGLLipid.fag2, gl_hydroxyl_2_textbox, 4);
                update_ranges(currentGLLipid.fag3, gl_fa_3_textbox, gl_fa_3_combobox.SelectedIndex);
                update_ranges(currentGLLipid.fag3, gl_db_3_textbox, 3);
                update_ranges(currentGLLipid.fag3, gl_hydroxyl_3_textbox, 4);
                
                glRepresentativeFA.Checked = currentGLLipid.representativeFA;
                gl_picture_box.SendToBack();
            }
            else if (index == 2)
            {
                pl_lipid currentPLLipid = (pl_lipid)currentLipid;
                setting_listbox = true;
                for (int i = 0; i < pl_hg_listbox.Items.Count; ++i)
                {
                    pl_hg_listbox.SetSelected(i, false);
                }
                foreach (int hgValue in currentPLLipid.hgValues)
                {
                    pl_hg_listbox.SetSelected(hgValue, true);
                }
                setting_listbox = false;
                
                pl_fa_1_textbox.Text = currentPLLipid.fag1.lengthInfo;
                pl_db_1_textbox.Text = currentPLLipid.fag1.dbInfo;
                pl_hydroxyl_1_textbox.Text = currentPLLipid.fag1.hydroxylInfo;
                pl_fa_1_combobox.SelectedIndex = currentPLLipid.fag1.chainType;
                pl_fa_1_gb_1_checkbox_1.Checked = currentPLLipid.fag1.faTypes["FA"];
                pl_fa_1_gb_1_checkbox_2.Checked = currentPLLipid.fag1.faTypes["FAp"];
                pl_fa_1_gb_1_checkbox_3.Checked = currentPLLipid.fag1.faTypes["FAe"];
                
                pl_fa_2_textbox.Text = currentPLLipid.fag2.lengthInfo;
                pl_db_2_textbox.Text = currentPLLipid.fag2.dbInfo;
                pl_hydroxyl_2_textbox.Text = currentPLLipid.fag2.hydroxylInfo;
                pl_fa_2_combobox.SelectedIndex = currentPLLipid.fag2.chainType;
                pl_fa_2_gb_1_checkbox_1.Checked = currentPLLipid.fag2.faTypes["FA"];
                pl_fa_2_gb_1_checkbox_2.Checked = currentPLLipid.fag2.faTypes["FAp"];
                pl_fa_2_gb_1_checkbox_3.Checked = currentPLLipid.fag2.faTypes["FAe"];
            
                
                pl_pos_adduct_checkbox_1.Checked = currentPLLipid.adducts["+H"];
                pl_pos_adduct_checkbox_2.Checked = currentPLLipid.adducts["+2H"];
                pl_pos_adduct_checkbox_3.Checked = currentPLLipid.adducts["+NH4"];
                pl_pos_adduct_checkbox_4.Checked = currentPLLipid.adducts["+Na"];
                pl_neg_adduct_checkbox_1.Checked = currentPLLipid.adducts["-H"];
                pl_neg_adduct_checkbox_2.Checked = currentPLLipid.adducts["-2H"];
                pl_neg_adduct_checkbox_3.Checked = currentPLLipid.adducts["+HCOO"];
                pl_neg_adduct_checkbox_4.Checked = currentPLLipid.adducts["+CH3COO"];
                if (lipid_modifications[2] > -1) pl_modify_lipid_button.Enabled = true;
                else pl_modify_lipid_button.Enabled = false;
                pl_is_cl.Checked = false;
                
                
                update_ranges(currentPLLipid.fag1, pl_fa_1_textbox, pl_fa_1_combobox.SelectedIndex);
                update_ranges(currentPLLipid.fag1, pl_db_1_textbox, 3);
                update_ranges(currentPLLipid.fag1, pl_hydroxyl_1_textbox, 4);
                update_ranges(currentPLLipid.fag2, pl_fa_2_textbox, pl_fa_2_combobox.SelectedIndex);
                update_ranges(currentPLLipid.fag2, pl_db_2_textbox, 3);
                update_ranges(currentPLLipid.fag2, pl_hydroxyl_2_textbox, 4);
                
                plRepresentativeFA.Checked = currentPLLipid.representativeFA;
                pl_picture_box.SendToBack();
            }
            else if (index == 3)
            {
                sl_lipid currentSLLipid = (sl_lipid)currentLipid;
                setting_listbox = true;
                for (int i = 0; i < sl_hg_listbox.Items.Count; ++i)
                {
                    sl_hg_listbox.SetSelected(i, false);
                }
                foreach (int hgValue in currentSLLipid.hgValues)
                {
                    sl_hg_listbox.SetSelected(hgValue, true);
                }
                setting_listbox = false;
                
                
                sl_lcb_textbox.Text = currentSLLipid.lcb.lengthInfo;
                sl_db_2_textbox.Text = currentSLLipid.lcb.dbInfo;
                sl_lcb_combobox.SelectedIndex = currentSLLipid.lcb.chainType;
                sl_lcb_hydroxy_combobox.SelectedIndex = currentSLLipid.lcb_hydroxyValue - 2;
                sl_fa_hydroxy_combobox.SelectedIndex = currentSLLipid.fa_hydroxyValue;
                
                sl_fa_textbox.Text = currentSLLipid.fag.lengthInfo;
                sl_db_1_textbox.Text = currentSLLipid.fag.dbInfo;
                sl_fa_combobox.SelectedIndex = currentSLLipid.fag.chainType;
            
                
                sl_pos_adduct_checkbox_1.Checked = currentSLLipid.adducts["+H"];
                sl_pos_adduct_checkbox_2.Checked = currentSLLipid.adducts["+2H"];
                sl_pos_adduct_checkbox_3.Checked = currentSLLipid.adducts["+NH4"];
                sl_pos_adduct_checkbox_4.Checked = currentSLLipid.adducts["+Na"];
                sl_neg_adduct_checkbox_1.Checked = currentSLLipid.adducts["-H"];
                sl_neg_adduct_checkbox_2.Checked = currentSLLipid.adducts["-2H"];
                sl_neg_adduct_checkbox_3.Checked = currentSLLipid.adducts["+HCOO"];
                sl_neg_adduct_checkbox_4.Checked = currentSLLipid.adducts["+CH3COO"];
                if (lipid_modifications[3] > -1) sl_modify_lipid_button.Enabled = true;
                else sl_modify_lipid_button.Enabled = false;
                
                
                update_ranges(currentSLLipid.lcb, sl_lcb_textbox, sl_lcb_combobox.SelectedIndex);
                update_ranges(currentSLLipid.lcb, sl_db_2_textbox, 3);
                update_ranges(currentSLLipid.fag, sl_fa_textbox, sl_fa_combobox.SelectedIndex);
                update_ranges(currentSLLipid.fag, sl_db_1_textbox, 3);
                
                /*
                string headgroup = sl_hg_combobox.SelectedItem.ToString();
                if (headgroup == "SPH" || headgroup == "S1P" || headgroup == "SPC"){
                    ((sl_lipid)currentLipid).fag.disabled = true;
                    sl_fa_combobox.Enabled = false;
                    sl_fa_textbox.Enabled = false;
                    sl_db_1_textbox.Enabled = false;
                    sl_fa_hydroxy_combobox.Enabled = false;
                }*/
                sl_picture_box.SendToBack();
            }
            
        }
        
        public void reset_cl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.lipidTabList[0] = new cl_lipid(lipidCreatorForm.all_paths_to_precursor_images, lipidCreatorForm.all_fragments);
            lipid_modifications[0] = -1;
            cl_db_1_textbox.BackColor = Color.White;
            cl_db_2_textbox.BackColor = Color.White;
            cl_db_3_textbox.BackColor = Color.White;
            cl_db_4_textbox.BackColor = Color.White;
            cl_fa_1_textbox.BackColor = Color.White;
            cl_fa_2_textbox.BackColor = Color.White;
            cl_fa_3_textbox.BackColor = Color.White;
            cl_fa_4_textbox.BackColor = Color.White;
            changeTab(0);
        }
        
        public void reset_gl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.lipidTabList[1] = new gl_lipid(lipidCreatorForm.all_paths_to_precursor_images, lipidCreatorForm.all_fragments);
            lipid_modifications[1] = -1;
            changeTab(1);
        }
        
        public void reset_pl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.lipidTabList[2] = new pl_lipid(lipidCreatorForm.all_paths_to_precursor_images, lipidCreatorForm.all_fragments);
            lipid_modifications[2] = -1;
            changeTab(2);
        }
        
        public void reset_sl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.lipidTabList[3] = new sl_lipid(lipidCreatorForm.all_paths_to_precursor_images, lipidCreatorForm.all_fragments);
            lipid_modifications[3] = -1;
            changeTab(3);
        }
        
        
        /////////////////////// CL //////////////////////////////
        
        
        public void cl_fa_1_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag1.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((cl_lipid)currentLipid).fag1, cl_fa_1_textbox, ((ComboBox)sender).SelectedIndex);
            if (((cl_lipid)currentLipid).representativeFA)
            {
                cl_fa_2_combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                cl_fa_3_combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                cl_fa_4_combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
            }
        }
        public void cl_fa_2_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((cl_lipid)currentLipid).fag1, cl_fa_2_textbox, ((ComboBox)sender).SelectedIndex);
        }
        public void cl_fa_3_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag3.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((cl_lipid)currentLipid).fag1, cl_fa_3_textbox, ((ComboBox)sender).SelectedIndex);
        }
        public void cl_fa_4_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag4.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((cl_lipid)currentLipid).fag1, cl_fa_4_textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        // ob_type (Object type): 0 = carbon length, 1 = carbon length odd, 2 = carbon length even, 3 = db length, 4 = hydroxyl length
        public void update_ranges(fattyAcidGroup fag, TextBox tb, int ob_type)
        {
            int max_range = 30;
            int min_range = 0;
            if (ob_type < 3) min_range = 2;
            if (ob_type == 3) max_range = 6;
            else if (ob_type == 4) max_range = 29;
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
        
        public void cl_fa_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag1, (TextBox)sender, cl_fa_1_combobox.SelectedIndex);
            if (((cl_lipid)currentLipid).representativeFA)
            {
                cl_fa_2_textbox.Text = ((TextBox)sender).Text;
                cl_fa_3_textbox.Text = ((TextBox)sender).Text;
                cl_fa_4_textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void cl_fa_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag2, (TextBox)sender, cl_fa_2_combobox.SelectedIndex);
        }
        public void cl_fa_3_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag3.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag3, (TextBox)sender, cl_fa_3_combobox.SelectedIndex);
        }
        public void cl_fa_4_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag4.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag4, (TextBox)sender, cl_fa_4_combobox.SelectedIndex);
        }
        
        public void cl_db_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag1, (TextBox)sender, 3);
            if (((cl_lipid)currentLipid).representativeFA)
            {
                cl_db_2_textbox.Text = ((TextBox)sender).Text;
                cl_db_3_textbox.Text = ((TextBox)sender).Text;
                cl_db_4_textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void cl_db_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag2, (TextBox)sender, 3);
        }
        public void cl_db_3_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag3.dbInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag3, (TextBox)sender, 3);
        }
        public void cl_db_4_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag4.dbInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag4, (TextBox)sender, 3);
        }
        
        public void cl_hydroxyl_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag1.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag1, (TextBox)sender, 4);
            if (((cl_lipid)currentLipid).representativeFA)
            {
                cl_hydroxyl_2_textbox.Text = ((TextBox)sender).Text;
                cl_hydroxyl_3_textbox.Text = ((TextBox)sender).Text;
                cl_hydroxyl_4_textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void cl_hydroxyl_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag2.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag2, (TextBox)sender, 4);
        }
        public void cl_hydroxyl_3_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag3.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag3, (TextBox)sender, 4);
        }
        public void cl_hydroxyl_4_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag4.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag4, (TextBox)sender, 4);
        }
        
        public void cl_fa_1_gb_1_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag1.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag1.faTypes["FAx"] = !((cl_lipid)currentLipid).fag1.any_fa_checked();
            if (((cl_lipid)currentLipid).representativeFA)
            {
                cl_fa_2_gb_1_checkbox_1.Checked = ((CheckBox)sender).Checked;
                cl_fa_3_gb_1_checkbox_1.Checked = ((CheckBox)sender).Checked;
                cl_fa_4_gb_1_checkbox_1.Checked = ((CheckBox)sender).Checked;
            }
        }
        public void cl_fa_1_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag1.faTypes["FAx"] = !((cl_lipid)currentLipid).fag1.any_fa_checked();
            if (((cl_lipid)currentLipid).representativeFA)
            {
                cl_fa_2_gb_1_checkbox_2.Checked = ((CheckBox)sender).Checked;
                cl_fa_3_gb_1_checkbox_2.Checked = ((CheckBox)sender).Checked;
                cl_fa_4_gb_1_checkbox_2.Checked = ((CheckBox)sender).Checked;
            }
        }
        public void cl_fa_1_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag1.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag1.faTypes["FAx"] = !((cl_lipid)currentLipid).fag1.any_fa_checked();
            if (((cl_lipid)currentLipid).representativeFA)
            {
                cl_fa_2_gb_1_checkbox_3.Checked = ((CheckBox)sender).Checked;
                cl_fa_3_gb_1_checkbox_3.Checked = ((CheckBox)sender).Checked;
                cl_fa_4_gb_1_checkbox_3.Checked = ((CheckBox)sender).Checked;
            }
        }
        
        public void cl_fa_2_gb_1_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag2.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag2.faTypes["FAx"] = !((cl_lipid)currentLipid).fag2.any_fa_checked();
        }
        public void cl_fa_2_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag2.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag2.faTypes["FAx"] = !((cl_lipid)currentLipid).fag2.any_fa_checked();
        }
        public void cl_fa_2_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag2.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag2.faTypes["FAx"] = !((cl_lipid)currentLipid).fag2.any_fa_checked();
        }
        
        public void cl_fa_3_gb_1_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag3.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag3.faTypes["FAx"] = !((cl_lipid)currentLipid).fag3.any_fa_checked();
        }
        public void cl_fa_3_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag3.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag3.faTypes["FAx"] = !((cl_lipid)currentLipid).fag3.any_fa_checked();
        }
        public void cl_fa_3_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag3.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag3.faTypes["FAx"] = !((cl_lipid)currentLipid).fag3.any_fa_checked();
        }
        
        public void cl_fa_4_gb_1_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag4.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag4.faTypes["FAx"] = !((cl_lipid)currentLipid).fag4.any_fa_checked();
        }
        public void cl_fa_4_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag4.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag4.faTypes["FAx"] = !((cl_lipid)currentLipid).fag4.any_fa_checked();
        }
        public void cl_fa_4_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag4.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag4.faTypes["FAx"] = !((cl_lipid)currentLipid).fag4.any_fa_checked();
        }
        
        public void cl_pos_adduct_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        public void cl_pos_adduct_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        public void cl_pos_adduct_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        public void cl_pos_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).adducts["+Na"] = ((CheckBox)sender).Checked;
        }
        public void cl_neg_adduct_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        public void cl_neg_adduct_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        public void cl_neg_adduct_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        public void cl_neg_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
        }
        
        
        
        void cl_fa_1_gb_1_checkbox_3_MouseLeave(object sender, EventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image;
            cl_picture_box.SendToBack();
        }
        private void cl_fa_1_gb_1_checkbox_3_MouseHover(object sender, MouseEventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image_fa1e;
            cl_picture_box.SendToBack();
        }
        
        void cl_fa_1_gb_1_checkbox_2_MouseLeave(object sender, EventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image;
            cl_picture_box.SendToBack();
        }
        private void cl_fa_1_gb_1_checkbox_2_MouseHover(object sender, MouseEventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image_fa1p;
            cl_picture_box.SendToBack();
        }
        
        void cl_fa_2_gb_1_checkbox_3_MouseLeave(object sender, EventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image;
            cl_picture_box.SendToBack();
        }
        private void cl_fa_2_gb_1_checkbox_3_MouseHover(object sender, MouseEventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image_fa2e;
            cl_picture_box.SendToBack();
        }
        void cl_fa_2_gb_1_checkbox_2_MouseLeave(object sender, EventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image;
            cl_picture_box.SendToBack();
        }

        private void cl_fa_2_gb_1_checkbox_2_MouseHover(object sender, MouseEventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image_fa2p;
            cl_picture_box.SendToBack();
        }
        
        void cl_fa_3_gb_1_checkbox_3_MouseLeave(object sender, EventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image;
            cl_picture_box.SendToBack();
        }
        private void cl_fa_3_gb_1_checkbox_3_MouseHover(object sender, MouseEventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image_fa3e;
            cl_picture_box.SendToBack();
        }
        
        void cl_fa_3_gb_1_checkbox_2_MouseLeave(object sender, EventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image;
            cl_picture_box.SendToBack();
        }
        private void cl_fa_3_gb_1_checkbox_2_MouseHover(object sender, MouseEventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image_fa3p;
            cl_picture_box.SendToBack();
        }
        
        void cl_fa_4_gb_1_checkbox_3_MouseLeave(object sender, EventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image;
            cl_picture_box.SendToBack();
        }
        private void cl_fa_4_gb_1_checkbox_3_MouseHover(object sender, MouseEventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image_fa4e;
            cl_picture_box.SendToBack();
        }
        void cl_fa_4_gb_1_checkbox_2_MouseLeave(object sender, EventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image;
            cl_picture_box.SendToBack();
        }

        private void cl_fa_4_gb_1_checkbox_2_MouseHover(object sender, MouseEventArgs e)
        {
            cl_picture_box.Image = cardio_backbone_image_fa4p;
            cl_picture_box.SendToBack();
        }
        
        public void clRepresentativeFA_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).representativeFA = ((CheckBox)sender).Checked;
            if (((cl_lipid)currentLipid).representativeFA)
            {
                cl_fa_2_textbox.Enabled = false;
                cl_db_2_textbox.Enabled = false;
                cl_hydroxyl_2_textbox.Enabled = false;
                cl_fa_2_combobox.Enabled = false;
                cl_fa_2_gb_1_checkbox_1.Enabled = false;
                cl_fa_2_gb_1_checkbox_2.Enabled = false;
                cl_fa_2_gb_1_checkbox_3.Enabled = false;
                cl_fa_3_textbox.Enabled = false;
                cl_db_3_textbox.Enabled = false;
                cl_hydroxyl_3_textbox.Enabled = false;
                cl_fa_3_combobox.Enabled = false;
                cl_fa_3_gb_1_checkbox_1.Enabled = false;
                cl_fa_3_gb_1_checkbox_2.Enabled = false;
                cl_fa_3_gb_1_checkbox_3.Enabled = false;
                cl_fa_4_textbox.Enabled = false;
                cl_db_4_textbox.Enabled = false;
                cl_hydroxyl_4_textbox.Enabled = false;
                cl_fa_4_combobox.Enabled = false;
                cl_fa_4_gb_1_checkbox_1.Enabled = false;
                cl_fa_4_gb_1_checkbox_2.Enabled = false;
                cl_fa_4_gb_1_checkbox_3.Enabled = false;
                
                cl_fa_2_textbox.Text = cl_fa_1_textbox.Text;
                cl_fa_3_textbox.Text = cl_fa_1_textbox.Text;
                cl_fa_4_textbox.Text = cl_fa_1_textbox.Text;
                cl_db_2_textbox.Text = cl_db_1_textbox.Text;
                cl_db_3_textbox.Text = cl_db_1_textbox.Text;
                cl_db_4_textbox.Text = cl_db_1_textbox.Text;
                cl_hydroxyl_2_textbox.Text = cl_hydroxyl_1_textbox.Text;
                cl_hydroxyl_3_textbox.Text = cl_hydroxyl_1_textbox.Text;
                cl_hydroxyl_4_textbox.Text = cl_hydroxyl_1_textbox.Text;
                cl_fa_2_combobox.Text = cl_fa_1_combobox.Text;
                cl_fa_3_combobox.Text = cl_fa_1_combobox.Text;
                cl_fa_4_combobox.Text = cl_fa_1_combobox.Text;
                cl_fa_2_gb_1_checkbox_1.Checked = cl_fa_1_gb_1_checkbox_1.Checked;
                cl_fa_3_gb_1_checkbox_1.Checked = cl_fa_1_gb_1_checkbox_1.Checked;
                cl_fa_4_gb_1_checkbox_1.Checked = cl_fa_1_gb_1_checkbox_1.Checked;
                cl_fa_2_gb_1_checkbox_2.Checked = cl_fa_1_gb_1_checkbox_2.Checked;
                cl_fa_3_gb_1_checkbox_2.Checked = cl_fa_1_gb_1_checkbox_2.Checked;
                cl_fa_4_gb_1_checkbox_2.Checked = cl_fa_1_gb_1_checkbox_2.Checked;
                cl_fa_2_gb_1_checkbox_3.Checked = cl_fa_1_gb_1_checkbox_3.Checked;
                cl_fa_3_gb_1_checkbox_3.Checked = cl_fa_1_gb_1_checkbox_3.Checked;
                cl_fa_4_gb_1_checkbox_3.Checked = cl_fa_1_gb_1_checkbox_3.Checked;
            }
            else
            {
                cl_fa_2_textbox.Enabled = true;
                cl_db_2_textbox.Enabled = true;
                cl_hydroxyl_2_textbox.Enabled = true;
                cl_fa_2_combobox.Enabled = true;
                cl_fa_2_gb_1_checkbox_1.Enabled = true;
                cl_fa_2_gb_1_checkbox_2.Enabled = true;
                cl_fa_2_gb_1_checkbox_3.Enabled = true;
                cl_fa_3_textbox.Enabled = true;
                cl_db_3_textbox.Enabled = true;
                cl_hydroxyl_3_textbox.Enabled = true;
                cl_fa_3_combobox.Enabled = true;
                cl_fa_3_gb_1_checkbox_1.Enabled = true;
                cl_fa_3_gb_1_checkbox_2.Enabled = true;
                cl_fa_3_gb_1_checkbox_3.Enabled = true;
                cl_fa_4_textbox.Enabled = true;
                cl_db_4_textbox.Enabled = true;
                cl_hydroxyl_4_textbox.Enabled = true;
                cl_fa_4_combobox.Enabled = true;
                cl_fa_4_gb_1_checkbox_1.Enabled = true;
                cl_fa_4_gb_1_checkbox_2.Enabled = true;
                cl_fa_4_gb_1_checkbox_3.Enabled = true;
            }
            update_ranges(((cl_lipid)currentLipid).fag2, cl_fa_2_textbox, cl_fa_2_combobox.SelectedIndex);
            update_ranges(((cl_lipid)currentLipid).fag3, cl_fa_3_textbox, cl_fa_3_combobox.SelectedIndex);
            update_ranges(((cl_lipid)currentLipid).fag4, cl_fa_4_textbox, cl_fa_4_combobox.SelectedIndex);
            update_ranges(((cl_lipid)currentLipid).fag2, cl_db_2_textbox, 3);
            update_ranges(((cl_lipid)currentLipid).fag3, cl_db_3_textbox, 3);
            update_ranges(((cl_lipid)currentLipid).fag4, cl_db_4_textbox, 3);
            update_ranges(((cl_lipid)currentLipid).fag2, cl_hydroxyl_2_textbox, 4);
            update_ranges(((cl_lipid)currentLipid).fag3, cl_hydroxyl_3_textbox, 4);
            update_ranges(((cl_lipid)currentLipid).fag4, cl_hydroxyl_4_textbox, 4);
        }
        
        
        
        ////////////////////// GL ////////////////////////////////
        
        public void gl_fa_1_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag1.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((gl_lipid)currentLipid).fag1, gl_fa_1_textbox, ((ComboBox)sender).SelectedIndex);
            if (((gl_lipid)currentLipid).representativeFA)
            {
                gl_fa_2_combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
                gl_fa_3_combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
            }
        }
        public void gl_fa_2_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((gl_lipid)currentLipid).fag2, gl_fa_2_textbox, ((ComboBox)sender).SelectedIndex);
        }
        public void gl_fa_3_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag3.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((gl_lipid)currentLipid).fag3, gl_fa_3_textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void gl_fa_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag1, (TextBox)sender, gl_fa_1_combobox.SelectedIndex);
            if (((gl_lipid)currentLipid).representativeFA)
            {
                gl_fa_2_textbox.Text = ((TextBox)sender).Text;
                gl_fa_3_textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void gl_fa_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag2, (TextBox)sender, gl_fa_2_combobox.SelectedIndex);
        }
        public void gl_fa_3_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag3.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag3, (TextBox)sender, gl_fa_3_combobox.SelectedIndex);
        }
        
        public void gl_db_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag1, (TextBox)sender, 3);
            if (((gl_lipid)currentLipid).representativeFA)
            {
                gl_db_2_textbox.Text = ((TextBox)sender).Text;
                gl_db_3_textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void gl_db_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag2, (TextBox)sender, 3);
        }
        public void gl_db_3_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag3.dbInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag3, (TextBox)sender, 3);
        }
        
        public void gl_pos_adduct_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        public void gl_pos_adduct_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        public void gl_pos_adduct_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        public void gl_pos_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).adducts["+Na"] = ((CheckBox)sender).Checked;
        }
        public void gl_neg_adduct_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        public void gl_neg_adduct_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        public void gl_neg_adduct_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        public void gl_neg_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
        }
        
        
        public void gl_fa_1_gb_1_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag1.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((gl_lipid)currentLipid).fag1.faTypes["FAx"] = !((gl_lipid)currentLipid).fag1.any_fa_checked();
            if (((gl_lipid)currentLipid).representativeFA)
            {
                gl_fa_2_gb_1_checkbox_1.Checked = ((CheckBox)sender).Checked;
                gl_fa_3_gb_1_checkbox_1.Checked =  ((CheckBox)sender).Checked;
            }
        }
        public void gl_fa_1_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((gl_lipid)currentLipid).fag1.faTypes["FAx"] = !((gl_lipid)currentLipid).fag1.any_fa_checked();
            if (((gl_lipid)currentLipid).representativeFA)
            {
                gl_fa_2_gb_1_checkbox_2.Checked = ((CheckBox)sender).Checked;
                gl_fa_3_gb_1_checkbox_2.Checked =  ((CheckBox)sender).Checked;
            }
        }
        public void gl_fa_1_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag1.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((gl_lipid)currentLipid).fag1.faTypes["FAx"] = !((gl_lipid)currentLipid).fag1.any_fa_checked();
            if (((gl_lipid)currentLipid).representativeFA)
            {
                gl_fa_2_gb_1_checkbox_3.Checked = ((CheckBox)sender).Checked;
                gl_fa_3_gb_1_checkbox_3.Checked =  ((CheckBox)sender).Checked;
            }
        }
        
        public void gl_fa_2_gb_1_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag2.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((gl_lipid)currentLipid).fag2.faTypes["FAx"] = !((gl_lipid)currentLipid).fag2.any_fa_checked();
        }
        public void gl_fa_2_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag2.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((gl_lipid)currentLipid).fag2.faTypes["FAx"] = !((gl_lipid)currentLipid).fag2.any_fa_checked();
        }
        public void gl_fa_2_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag2.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((gl_lipid)currentLipid).fag2.faTypes["FAx"] = !((gl_lipid)currentLipid).fag2.any_fa_checked();
        }
        
        public void gl_fa_3_gb_1_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag3.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((gl_lipid)currentLipid).fag3.faTypes["FAx"] = !((gl_lipid)currentLipid).fag3.any_fa_checked();
        }
        public void gl_fa_3_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag3.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((gl_lipid)currentLipid).fag3.faTypes["FAx"] = !((gl_lipid)currentLipid).fag3.any_fa_checked();
        }
        public void gl_fa_3_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag3.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((gl_lipid)currentLipid).fag3.faTypes["FAx"] = !((gl_lipid)currentLipid).fag3.any_fa_checked();
        }
        
        
        public void gl_hydroxyl_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag1.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag1, (TextBox)sender, 4);
            if (((gl_lipid)currentLipid).representativeFA)
            {
                gl_hydroxyl_2_textbox.Text = ((TextBox)sender).Text;
                gl_hydroxyl_3_textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void gl_hydroxyl_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag2.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag2, (TextBox)sender, 4);
        }
        public void gl_hydroxyl_3_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag3.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag3, (TextBox)sender, 4);
        }
        
        public void trigger_easteregg(Object sender, EventArgs e)
        {
            eastertext.Left = 1030;
            eastertext.Visible = true;
            this.timer1.Enabled = true;
        }
        
        public void sugar_heady(Object sender, EventArgs e)
        {
            MessageBox.Show("Who is your sugar heady?");
        }
        
        private void timer1_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            eastertext.Left -= 2;
            if (eastertext.Left < -eastertext.Width){
                this.timer1.Enabled = false;
                eastertext.Visible = false;
            }
        }
        
        void gl_fa_1_gb_1_checkbox_3_MouseLeave(object sender, EventArgs e)
        {
            gl_picture_box.Image = glycero_backbone_image;
            gl_picture_box.SendToBack();
        }
        private void gl_fa_1_gb_1_checkbox_3_MouseHover(object sender, MouseEventArgs e)
        {
            gl_picture_box.Image = glycero_backbone_image_fa1e;
            gl_picture_box.SendToBack();
        }
        
        void gl_fa_1_gb_1_checkbox_2_MouseLeave(object sender, EventArgs e)
        {
            gl_picture_box.Image = glycero_backbone_image;
            gl_picture_box.SendToBack();
        }
        private void gl_fa_1_gb_1_checkbox_2_MouseHover(object sender, MouseEventArgs e)
        {
            gl_picture_box.Image = glycero_backbone_image_fa1p;
            gl_picture_box.SendToBack();
        }
        
        void gl_fa_2_gb_1_checkbox_3_MouseLeave(object sender, EventArgs e)
        {
            gl_picture_box.Image = glycero_backbone_image;
            gl_picture_box.SendToBack();
        }
        private void gl_fa_2_gb_1_checkbox_3_MouseHover(object sender, MouseEventArgs e)
        {
            gl_picture_box.Image = glycero_backbone_image_fa2e;
            gl_picture_box.SendToBack();
        }
        void gl_fa_2_gb_1_checkbox_2_MouseLeave(object sender, EventArgs e)
        {
            gl_picture_box.Image = glycero_backbone_image;
            gl_picture_box.SendToBack();
        }

        private void gl_fa_2_gb_1_checkbox_2_MouseHover(object sender, MouseEventArgs e)
        {
            gl_picture_box.Image = glycero_backbone_image_fa2p;
            gl_picture_box.SendToBack();
        }
        
        void gl_fa_3_gb_1_checkbox_3_MouseLeave(object sender, EventArgs e)
        {
            gl_picture_box.Image = glycero_backbone_image;
            gl_picture_box.SendToBack();
        }
        private void gl_fa_3_gb_1_checkbox_3_MouseHover(object sender, MouseEventArgs e)
        {
            gl_picture_box.Image = glycero_backbone_image_fa3e;
            gl_picture_box.SendToBack();
        }
        
        void gl_fa_3_gb_1_checkbox_2_MouseLeave(object sender, EventArgs e)
        {
            gl_picture_box.Image = glycero_backbone_image;
            gl_picture_box.SendToBack();
        }
        private void gl_fa_3_gb_1_checkbox_2_MouseHover(object sender, MouseEventArgs e)
        {
            gl_picture_box.Image = glycero_backbone_image_fa3p;
            gl_picture_box.SendToBack();
        }
        
        
        private void gl_hg_listbox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            if (setting_listbox) return;
            ((gl_lipid)currentLipid).hgValues.Clear();
            foreach(object itemChecked in ((ListBox)sender).SelectedItems)
            {
                int hgValue = ((ListBox)sender).Items.IndexOf(itemChecked);
                ((gl_lipid)currentLipid).hgValues.Add(hgValue);
            }
            
        }
        
        public void gl_contains_sugar_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).contains_sugar = ((CheckBox)sender).Checked;
            
            gl_picture_box.Visible = false;
            if (((gl_lipid)currentLipid).contains_sugar)
            {
                gl_fa_3_textbox.Visible = false;
                gl_db_3_textbox.Visible = false;
                gl_hydroxyl_3_textbox.Visible = false;
                gl_fa_3_combobox.Visible = false;
                gl_fa_3_gb_1_checkbox_1.Visible = false;
                gl_fa_3_gb_1_checkbox_2.Visible = false;
                gl_fa_3_gb_1_checkbox_3.Visible = false;
                gl_db_3_label.Visible = false;
                gl_hydroxyl_3_label.Visible = false;
                
                gl_hg_listbox.Visible = true;
                gl_hg_label.Visible = true;
                
                glycero_backbone_image = glycero_backbone_image_plant;
                glycero_backbone_image_fa1e = glycero_backbone_image_fa1e_plant;
                glycero_backbone_image_fa2e = glycero_backbone_image_fa2e_plant;
                glycero_backbone_image_fa1p = glycero_backbone_image_fa1p_plant;
                glycero_backbone_image_fa2p = glycero_backbone_image_fa2p_plant;
            }
            else
            {
                gl_fa_3_textbox.Visible = true;
                gl_db_3_textbox.Visible = true;
                gl_hydroxyl_3_textbox.Visible = true;
                gl_fa_3_combobox.Visible = true;
                gl_fa_3_gb_1_checkbox_1.Visible = true;
                gl_fa_3_gb_1_checkbox_2.Visible = true;
                gl_fa_3_gb_1_checkbox_3.Visible = true;
                gl_db_3_label.Visible = true;
                gl_hydroxyl_3_label.Visible = true;
                
                gl_hg_listbox.Visible = false;
                gl_hg_label.Visible = false;
                
                glycero_backbone_image = glycero_backbone_image_orig;
                glycero_backbone_image_fa1e = glycero_backbone_image_fa1e_orig;
                glycero_backbone_image_fa2e = glycero_backbone_image_fa2e_orig;
                glycero_backbone_image_fa3e = glycero_backbone_image_fa3e_orig;
                glycero_backbone_image_fa1p = glycero_backbone_image_fa1p_orig;
                glycero_backbone_image_fa2p = glycero_backbone_image_fa2p_orig;
                glycero_backbone_image_fa3p = glycero_backbone_image_fa3p_orig;
            }
            gl_picture_box.Image = glycero_backbone_image;
            gl_picture_box.Visible = true;
        }
        
        
        
        public void glRepresentativeFA_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).representativeFA = ((CheckBox)sender).Checked;
            if (((gl_lipid)currentLipid).representativeFA)
            {
                gl_fa_2_textbox.Enabled = false;
                gl_db_2_textbox.Enabled = false;
                gl_hydroxyl_2_textbox.Enabled = false;
                gl_fa_2_combobox.Enabled = false;
                gl_fa_2_gb_1_checkbox_1.Enabled = false;
                gl_fa_2_gb_1_checkbox_2.Enabled = false;
                gl_fa_2_gb_1_checkbox_3.Enabled = false;
                gl_fa_3_textbox.Enabled = false;
                gl_db_3_textbox.Enabled = false;
                gl_hydroxyl_3_textbox.Enabled = false;
                gl_fa_3_combobox.Enabled = false;
                gl_fa_3_gb_1_checkbox_1.Enabled = false;
                gl_fa_3_gb_1_checkbox_2.Enabled = false;
                gl_fa_3_gb_1_checkbox_3.Enabled = false;
                
                gl_fa_2_textbox.Text = gl_fa_1_textbox.Text;
                gl_fa_3_textbox.Text = gl_fa_1_textbox.Text;
                gl_db_2_textbox.Text = gl_db_1_textbox.Text;
                gl_db_3_textbox.Text = gl_db_1_textbox.Text;
                gl_hydroxyl_2_textbox.Text = gl_hydroxyl_1_textbox.Text;
                gl_hydroxyl_3_textbox.Text = gl_hydroxyl_1_textbox.Text;
                gl_fa_2_combobox.Text = gl_fa_1_combobox.Text;
                gl_fa_3_combobox.Text = gl_fa_1_combobox.Text;
                gl_fa_2_gb_1_checkbox_1.Checked = gl_fa_1_gb_1_checkbox_1.Checked;
                gl_fa_3_gb_1_checkbox_1.Checked = gl_fa_1_gb_1_checkbox_1.Checked;
                gl_fa_2_gb_1_checkbox_2.Checked = gl_fa_1_gb_1_checkbox_2.Checked;
                gl_fa_3_gb_1_checkbox_2.Checked = gl_fa_1_gb_1_checkbox_2.Checked;
                gl_fa_2_gb_1_checkbox_3.Checked = gl_fa_1_gb_1_checkbox_3.Checked;
                gl_fa_3_gb_1_checkbox_3.Checked = gl_fa_1_gb_1_checkbox_3.Checked;
                
                
            }
            else
            {
                gl_fa_2_textbox.Enabled = true;
                gl_db_2_textbox.Enabled = true;
                gl_hydroxyl_2_textbox.Enabled = true;
                gl_fa_2_combobox.Enabled = true;
                gl_fa_2_gb_1_checkbox_1.Enabled = true;
                gl_fa_2_gb_1_checkbox_2.Enabled = true;
                gl_fa_2_gb_1_checkbox_3.Enabled = true;
                gl_fa_3_textbox.Enabled = true;
                gl_db_3_textbox.Enabled = true;
                gl_hydroxyl_3_textbox.Enabled = true;
                gl_fa_3_combobox.Enabled = true;
                gl_fa_3_gb_1_checkbox_1.Enabled = true;
                gl_fa_3_gb_1_checkbox_2.Enabled = true;
                gl_fa_3_gb_1_checkbox_3.Enabled = true;
            }
            update_ranges(((gl_lipid)currentLipid).fag2, gl_fa_2_textbox, gl_fa_2_combobox.SelectedIndex);
            update_ranges(((gl_lipid)currentLipid).fag3, gl_fa_3_textbox, gl_fa_3_combobox.SelectedIndex);
            update_ranges(((gl_lipid)currentLipid).fag2, gl_db_2_textbox, 3);
            update_ranges(((gl_lipid)currentLipid).fag3, gl_db_3_textbox, 3);
            update_ranges(((gl_lipid)currentLipid).fag2, gl_hydroxyl_2_textbox, 4);
            update_ranges(((gl_lipid)currentLipid).fag3, gl_hydroxyl_3_textbox, 4);
        }
        
        
        
        ////////////////////// PL ////////////////////////////////
        
        
    
        public void pl_fa_1_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag1.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((pl_lipid)currentLipid).fag1, pl_fa_1_textbox, ((ComboBox)sender).SelectedIndex);
            if (((pl_lipid)currentLipid).representativeFA)
            {
                pl_fa_2_combobox.SelectedIndex = ((ComboBox)sender).SelectedIndex;
            }
        }
        public void pl_fa_2_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((pl_lipid)currentLipid).fag2, pl_fa_2_textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void pl_fa_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((pl_lipid)currentLipid).fag1, (TextBox)sender, pl_fa_1_combobox.SelectedIndex);
            if (((pl_lipid)currentLipid).representativeFA)
            {
                pl_fa_2_textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void pl_fa_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((pl_lipid)currentLipid).fag2, (TextBox)sender, pl_fa_2_combobox.SelectedIndex);
        }
        
        public void pl_db_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            update_ranges(((pl_lipid)currentLipid).fag1, (TextBox)sender, 3);
            if (((pl_lipid)currentLipid).representativeFA)
            {
                pl_db_2_textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void pl_db_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            update_ranges(((pl_lipid)currentLipid).fag2, (TextBox)sender, 3);
        }
        public void pl_hydroxyl_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag1.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((pl_lipid)currentLipid).fag1, (TextBox)sender, 4);
            if (((pl_lipid)currentLipid).representativeFA)
            {
                pl_hydroxyl_2_textbox.Text = ((TextBox)sender).Text;
            }
        }
        public void pl_hydroxyl_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag2.hydroxylInfo = ((TextBox)sender).Text;
            update_ranges(((pl_lipid)currentLipid).fag2, (TextBox)sender, 4);
        }
        
        public void pl_pos_adduct_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        public void pl_pos_adduct_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        public void pl_pos_adduct_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        public void pl_pos_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).adducts["+Na"] = ((CheckBox)sender).Checked;
        }
        public void pl_neg_adduct_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        public void pl_neg_adduct_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        public void pl_neg_adduct_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        public void pl_neg_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
        }
        
        public void pl_fa_1_gb_1_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag1.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((pl_lipid)currentLipid).fag1.faTypes["FAx"] = !((pl_lipid)currentLipid).fag1.any_fa_checked();
            if (((pl_lipid)currentLipid).representativeFA)
            {
                pl_fa_2_gb_1_checkbox_1.Checked = ((CheckBox)sender).Checked;
            }
        }
        public void pl_fa_1_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((pl_lipid)currentLipid).fag1.faTypes["FAx"] = !((pl_lipid)currentLipid).fag1.any_fa_checked();
            if (((pl_lipid)currentLipid).representativeFA)
            {
                pl_fa_2_gb_1_checkbox_2.Checked = ((CheckBox)sender).Checked;
            }
        }
        public void pl_fa_1_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag1.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((pl_lipid)currentLipid).fag1.faTypes["FAx"] = !((pl_lipid)currentLipid).fag1.any_fa_checked();
            if (((pl_lipid)currentLipid).representativeFA)
            {
                pl_fa_2_gb_1_checkbox_3.Checked = ((CheckBox)sender).Checked;
            }
        }
        
        public void pl_fa_2_gb_1_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag2.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((pl_lipid)currentLipid).fag2.faTypes["FAx"] = !((pl_lipid)currentLipid).fag2.any_fa_checked();
        }
        public void pl_fa_2_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag2.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((pl_lipid)currentLipid).fag2.faTypes["FAx"] = !((pl_lipid)currentLipid).fag2.any_fa_checked();
        }
        public void pl_fa_2_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag2.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((pl_lipid)currentLipid).fag2.faTypes["FAx"] = !((pl_lipid)currentLipid).fag2.any_fa_checked();
        }
        
        public void pl_is_cl_checkedChanged(Object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                pl_picture_box.Image = cardio_backbone_image;
                pl_picture_box.Location = new Point(5, 5);
                pl_hg_listbox.Visible = false;
                pl_hg_label.Visible = false;
                pl_add_lipid_button.Visible = false;
                pl_reset_lipid_button.Visible = false;
                pl_modify_lipid_button.Visible = false;
                pl_ms2fragments_lipid_button.Visible = false;
                pl_fa_1_gb_1_checkbox_3.Visible = false;
                pl_fa_1_gb_1_checkbox_2.Visible = false;
                pl_fa_1_gb_1_checkbox_1.Visible = false;
                pl_fa_2_gb_1_checkbox_3.Visible = false;
                pl_fa_2_gb_1_checkbox_2.Visible = false;
                pl_fa_2_gb_1_checkbox_1.Visible = false;
                pl_picture_box.Visible = false;
                pl_fa_1_textbox.Visible = false;
                pl_fa_2_textbox.Visible = false;
                pl_db_1_textbox.Visible = false;
                pl_db_2_textbox.Visible = false;
                pl_fa_1_combobox.Visible = false;
                pl_fa_2_combobox.Visible = false;
                pl_db_1_label.Visible = false;
                pl_db_2_label.Visible = false;
                pl_hydroxyl_1_textbox.Visible = false;
                pl_hydroxyl_2_textbox.Visible = false;
                pl_hydroxyl_1_label.Visible = false;
                pl_hydroxyl_2_label.Visible = false;
                pl_hg_label.Visible = false;
                pl_hg_listbox.Visible = false;
                pl_positive_adduct.Visible = false;
                pl_negative_adduct.Visible = false;
                plRepresentativeFA.Visible = false;
                
                cl_fa_1_gb_1_checkbox_3.Visible = true;
                cl_fa_1_gb_1_checkbox_2.Visible = true;
                cl_fa_1_gb_1_checkbox_1.Visible = true;
                cl_fa_2_gb_1_checkbox_3.Visible = true;
                cl_fa_2_gb_1_checkbox_2.Visible = true;
                cl_fa_2_gb_1_checkbox_1.Visible = true;
                cl_fa_3_gb_1_checkbox_3.Visible = true;
                cl_fa_3_gb_1_checkbox_2.Visible = true;
                cl_fa_3_gb_1_checkbox_1.Visible = true;
                cl_fa_4_gb_1_checkbox_3.Visible = true;
                cl_fa_4_gb_1_checkbox_2.Visible = true;
                cl_fa_4_gb_1_checkbox_1.Visible = true;
                cl_positive_adduct.Visible = true;
                cl_negative_adduct.Visible = true;
                cl_add_lipid_button.Visible = true;
                cl_reset_lipid_button.Visible = true;
                cl_modify_lipid_button.Visible = true;
                cl_ms2fragments_lipid_button.Visible = true;
                cl_picture_box.Visible = true;
                cl_fa_1_textbox.Visible = true;
                cl_fa_2_textbox.Visible = true;
                cl_fa_3_textbox.Visible = true;
                cl_fa_4_textbox.Visible = true;
                cl_db_1_textbox.Visible = true;
                cl_db_2_textbox.Visible = true;
                cl_db_3_textbox.Visible = true;
                cl_db_4_textbox.Visible = true;
                cl_hydroxyl_1_textbox.Visible = true;
                cl_hydroxyl_2_textbox.Visible = true;
                cl_hydroxyl_3_textbox.Visible = true;
                cl_hydroxyl_4_textbox.Visible = true;
                cl_hydroxyl_1_label.Visible = true;
                cl_hydroxyl_2_label.Visible = true;
                cl_hydroxyl_3_label.Visible = true;
                cl_hydroxyl_4_label.Visible = true;
                cl_fa_1_combobox.Visible = true;
                cl_fa_2_combobox.Visible = true;
                cl_fa_3_combobox.Visible = true;
                cl_fa_4_combobox.Visible = true;
                cl_db_1_label.Visible = true;
                cl_db_2_label.Visible = true;
                cl_db_3_label.Visible = true;
                cl_db_4_label.Visible = true;
                clRepresentativeFA.Visible = true;
                cl_db_4_label.SendToBack();
                
                changeTab(0);
            }
            else
            {
                pl_picture_box.Image = phospho_backbone_image;
                pl_picture_box.Location = new Point(107, 13);
                
                cl_fa_1_gb_1_checkbox_3.Visible = false;
                cl_fa_1_gb_1_checkbox_2.Visible = false;
                cl_fa_1_gb_1_checkbox_1.Visible = false;
                cl_fa_2_gb_1_checkbox_3.Visible = false;
                cl_fa_2_gb_1_checkbox_2.Visible = false;
                cl_fa_2_gb_1_checkbox_1.Visible = false;
                cl_fa_3_gb_1_checkbox_3.Visible = false;
                cl_fa_3_gb_1_checkbox_2.Visible = false;
                cl_fa_3_gb_1_checkbox_1.Visible = false;
                cl_fa_4_gb_1_checkbox_3.Visible = false;
                cl_fa_4_gb_1_checkbox_2.Visible = false;
                cl_fa_4_gb_1_checkbox_1.Visible = false;
                cl_positive_adduct.Visible = false;
                cl_negative_adduct.Visible = false;
                cl_add_lipid_button.Visible = false;
                cl_reset_lipid_button.Visible = false;
                cl_modify_lipid_button.Visible = false;
                cl_ms2fragments_lipid_button.Visible = false;
                cl_picture_box.Visible = false;
                cl_fa_1_textbox.Visible = false;
                cl_fa_2_textbox.Visible = false;
                cl_fa_3_textbox.Visible = false;
                cl_fa_4_textbox.Visible = false;
                cl_db_1_textbox.Visible = false;
                cl_db_2_textbox.Visible = false;
                cl_db_3_textbox.Visible = false;
                cl_db_4_textbox.Visible = false;
                cl_hydroxyl_1_textbox.Visible = false;
                cl_hydroxyl_2_textbox.Visible = false;
                cl_hydroxyl_3_textbox.Visible = false;
                cl_hydroxyl_4_textbox.Visible = false;
                cl_hydroxyl_1_label.Visible = false;
                cl_hydroxyl_2_label.Visible = false;
                cl_hydroxyl_3_label.Visible = false;
                cl_hydroxyl_4_label.Visible = false;
                cl_fa_1_combobox.Visible = false;
                cl_fa_2_combobox.Visible = false;
                cl_fa_3_combobox.Visible = false;
                cl_fa_4_combobox.Visible = false;
                cl_db_1_label.Visible = false;
                cl_db_2_label.Visible = false;
                cl_db_3_label.Visible = false;
                cl_db_4_label.Visible = false;
                clRepresentativeFA.Visible = false;
                
                pl_hg_listbox.Visible = true;
                pl_hg_label.Visible = true;
                pl_add_lipid_button.Visible = true;
                pl_reset_lipid_button.Visible = true;
                pl_modify_lipid_button.Visible = true;
                pl_ms2fragments_lipid_button.Visible = true;
                pl_fa_1_gb_1_checkbox_3.Visible = true;
                pl_fa_1_gb_1_checkbox_2.Visible = true;
                pl_fa_1_gb_1_checkbox_1.Visible = true;
                pl_fa_2_gb_1_checkbox_3.Visible = true;
                pl_fa_2_gb_1_checkbox_2.Visible = true;
                pl_fa_2_gb_1_checkbox_1.Visible = true;
                pl_picture_box.Visible = true;
                pl_fa_1_textbox.Visible = true;
                pl_fa_2_textbox.Visible = true;
                pl_db_1_textbox.Visible = true;
                pl_db_2_textbox.Visible = true;
                pl_fa_1_combobox.Visible = true;
                pl_fa_2_combobox.Visible = true;
                pl_db_1_label.Visible = true;
                pl_db_2_label.Visible = true;
                pl_hydroxyl_1_textbox.Visible = true;
                pl_hydroxyl_2_textbox.Visible = true;
                pl_hydroxyl_1_label.Visible = true;
                pl_hydroxyl_2_label.Visible = true;
                pl_hg_label.Visible = true;
                pl_hg_listbox.Visible = true;
                pl_positive_adduct.Visible = true;
                pl_negative_adduct.Visible = true;
                plRepresentativeFA.Visible = true;
                pl_picture_box.SendToBack();
                
                changeTab(2);
            }
            pl_is_cl.BringToFront();
        }
        
        void pl_fa_1_gb_1_checkbox_3_MouseLeave(object sender, EventArgs e)
        {
            pl_picture_box.Image = phospho_backbone_image;
            pl_picture_box.SendToBack();
        }
        private void pl_fa_1_gb_1_checkbox_3_MouseHover(object sender, MouseEventArgs e)
        {
            pl_picture_box.Image = phospho_backbone_image_fa1e;
            pl_picture_box.SendToBack();
        }
        
        void pl_fa_1_gb_1_checkbox_2_MouseLeave(object sender, EventArgs e)
        {
            pl_picture_box.Image = phospho_backbone_image;
            pl_picture_box.SendToBack();
        }
        private void pl_fa_1_gb_1_checkbox_2_MouseHover(object sender, MouseEventArgs e)
        {
            pl_picture_box.Image = phospho_backbone_image_fa1p;
            pl_picture_box.SendToBack();
        }

        
        void pl_fa_2_gb_1_checkbox_3_MouseLeave(object sender, EventArgs e)
        {
            pl_picture_box.Image = phospho_backbone_image;
            pl_picture_box.SendToBack();
        }
        private void pl_fa_2_gb_1_checkbox_3_MouseHover(object sender, MouseEventArgs e)
        {
            pl_picture_box.Image = phospho_backbone_image_fa2e;
            pl_picture_box.SendToBack();
        }
        void pl_fa_2_gb_1_checkbox_2_MouseLeave(object sender, EventArgs e)
        {
            pl_picture_box.Image = phospho_backbone_image;
            pl_picture_box.SendToBack();
        }

        private void pl_fa_2_gb_1_checkbox_2_MouseHover(object sender, MouseEventArgs e)
        {
            pl_picture_box.Image = phospho_backbone_image_fa2p;
            pl_picture_box.SendToBack();
        }
        
        private void pl_hg_listbox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            if (setting_listbox) return;
            ((pl_lipid)currentLipid).hgValues.Clear();
            foreach(object itemChecked in ((ListBox)sender).SelectedItems)
            {
                int hgValue = ((ListBox)sender).Items.IndexOf(itemChecked);
                ((pl_lipid)currentLipid).hgValues.Add(hgValue);
            }
            
        }
        
        public void plRepresentativeFA_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).representativeFA = ((CheckBox)sender).Checked;
            if (((pl_lipid)currentLipid).representativeFA)
            {
                pl_fa_2_textbox.Enabled = false;
                pl_db_2_textbox.Enabled = false;
                pl_hydroxyl_2_textbox.Enabled = false;
                pl_fa_2_combobox.Enabled = false;
                pl_fa_2_gb_1_checkbox_1.Enabled = false;
                pl_fa_2_gb_1_checkbox_2.Enabled = false;
                pl_fa_2_gb_1_checkbox_3.Enabled = false;
                
                pl_fa_2_textbox.Text = pl_fa_1_textbox.Text;
                pl_db_2_textbox.Text = pl_db_1_textbox.Text;
                pl_hydroxyl_2_textbox.Text = pl_hydroxyl_1_textbox.Text;
                pl_fa_2_combobox.Text = pl_fa_1_combobox.Text;
                pl_fa_2_gb_1_checkbox_1.Checked = pl_fa_1_gb_1_checkbox_1.Checked;
                pl_fa_2_gb_1_checkbox_2.Checked = pl_fa_1_gb_1_checkbox_2.Checked;
                pl_fa_2_gb_1_checkbox_3.Checked = pl_fa_1_gb_1_checkbox_3.Checked;
                
                
            }
            else
            {
                pl_fa_2_textbox.Enabled = true;
                pl_db_2_textbox.Enabled = true;
                pl_hydroxyl_2_textbox.Enabled = true;
                pl_fa_2_combobox.Enabled = true;
                pl_fa_2_gb_1_checkbox_1.Enabled = true;
                pl_fa_2_gb_1_checkbox_2.Enabled = true;
                pl_fa_2_gb_1_checkbox_3.Enabled = true;
            }
            update_ranges(((pl_lipid)currentLipid).fag2, pl_fa_2_textbox, pl_fa_2_combobox.SelectedIndex);
            update_ranges(((pl_lipid)currentLipid).fag2, pl_db_2_textbox, 3);
            update_ranges(((pl_lipid)currentLipid).fag2, pl_hydroxyl_2_textbox, 4);
        }
        
        ////////////////////// SL ////////////////////////////////
        
        
        
        public void sl_pos_adduct_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).adducts["+H"] = ((CheckBox)sender).Checked;
        }
        public void sl_pos_adduct_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).adducts["+2H"] = ((CheckBox)sender).Checked;
        }
        public void sl_pos_adduct_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).adducts["+NH4"] = ((CheckBox)sender).Checked;
        }
        public void sl_pos_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).adducts["+Na"] = ((CheckBox)sender).Checked;
        }
        public void sl_neg_adduct_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).adducts["-H"] = ((CheckBox)sender).Checked;
        }
        public void sl_neg_adduct_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).adducts["-2H"] = ((CheckBox)sender).Checked;
        }
        public void sl_neg_adduct_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).adducts["+HCOO"] = ((CheckBox)sender).Checked;
        }
        public void sl_neg_adduct_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).adducts["+CH3COO"] = ((CheckBox)sender).Checked;
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
            ((sl_lipid)currentLipid).fag.dbInfo = ((TextBox)sender).Text;
            update_ranges(((sl_lipid)currentLipid).fag, (TextBox)sender, 3);
        }
        public void sl_db_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).lcb.dbInfo = ((TextBox)sender).Text;
            update_ranges(((sl_lipid)currentLipid).lcb, (TextBox)sender, 3);
        }
        
        public void sl_fa_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).fag.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((sl_lipid)currentLipid).fag, (TextBox)sender, sl_fa_combobox.SelectedIndex);
        }
        public void sl_lcb_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).lcb.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((sl_lipid)currentLipid).lcb, (TextBox)sender, sl_lcb_combobox.SelectedIndex);
        }
        
        
        public void sl_fa_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).fag.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((sl_lipid)currentLipid).fag, sl_fa_textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void sl_lcb_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).lcb.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((sl_lipid)currentLipid).lcb, sl_lcb_textbox, ((ComboBox)sender).SelectedIndex);
        }
        
        public void sl_lcb_hydroxy_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).lcb_hydroxyValue = ((ComboBox)sender).SelectedIndex + 2;
        }
        
        public void sl_fa_hydroxy_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).fa_hydroxyValue = ((ComboBox)sender).SelectedIndex;
        }
        
        private void sl_hg_listbox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            if (setting_listbox) return;
            ((sl_lipid)currentLipid).hgValues.Clear();
            foreach(object itemChecked in ((ListBox)sender).SelectedItems)
            {
                int hgValue = ((ListBox)sender).Items.IndexOf(itemChecked);
                ((sl_lipid)currentLipid).hgValues.Add(hgValue);
            }
        }
        
        ////////////////////// Remaining parts ////////////////////////////////
        
        public void modify_cl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.registered_lipids[lipid_modifications[0]] = new cl_lipid((cl_lipid)currentLipid);
            refresh_registered_lipids_table();
        }
        
        public void modify_gl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.registered_lipids[lipid_modifications[1]] = new gl_lipid((gl_lipid)currentLipid);
            refresh_registered_lipids_table();
        }
        
        public void modify_pl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.registered_lipids[lipid_modifications[2]] = new pl_lipid((pl_lipid)currentLipid);
            refresh_registered_lipids_table();
        }
        
        public void modify_sl_lipid(Object sender, EventArgs e)
        {
            lipidCreatorForm.registered_lipids[lipid_modifications[3]] = new sl_lipid((sl_lipid)currentLipid);
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
                MessageBox.Show("No adduct selected!", "Lipid not registable");
                return;
            }
        
            if (currentLipid is cl_lipid)
            {   
                int cnt_active_fas = 0;
                cnt_active_fas += ((cl_lipid)currentLipid).fag1.faTypes["FAx"] ? 0 : 1;
                cnt_active_fas += ((cl_lipid)currentLipid).fag2.faTypes["FAx"] ? 0 : 1;
                cnt_active_fas += ((cl_lipid)currentLipid).fag3.faTypes["FAx"] ? 0 : 1;
                cnt_active_fas += ((cl_lipid)currentLipid).fag4.faTypes["FAx"] ? 0 : 1;
                if (cnt_active_fas < 3)
                {
                    MessageBox.Show("At least three fatty acids must be selected!", "Lipid not registable");
                    return;
                }
            
                if (cl_fa_1_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First fatty acid length content not valid!", "Lipid not registable");
                    return;
                }
                if (cl_fa_2_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second fatty acid length content not valid!", "Lipid not registable");
                    return;
                }
                if (cl_fa_3_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Third fatty acid length content not valid!", "Lipid not registable");
                    return;
                }
                if (cl_fa_4_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Fourth fatty acid length content not valid!", "Lipid not registable");
                    return;
                }
                if (cl_db_1_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First double bond content not valid!", "Lipid not registable");
                    return;
                }
                if (cl_db_2_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second double bond content not valid!", "Lipid not registable");
                    return;
                }
                if (cl_db_3_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Third double bond content not valid!", "Lipid not registable");
                    return;
                }
                if (cl_db_4_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Fourth double bond content not valid!", "Lipid not registable");
                    return;
                }
                if (cl_hydroxyl_1_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First hydroxyl content not valid!", "Lipid not registable");
                    return;
                }
                if (cl_hydroxyl_2_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second hydroxyl content not valid!", "Lipid not registable");
                    return;
                }
                if (cl_hydroxyl_3_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Third hydroxyl content not valid!", "Lipid not registable");
                    return;
                }
                if (cl_hydroxyl_4_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Fourth hydroxyl content not valid!", "Lipid not registable");
                    return;
                }
                
                lipidCreatorForm.registered_lipids.Add(new cl_lipid((cl_lipid)currentLipid));
            }
            
            else if (currentLipid is gl_lipid)
            {
                if (gl_fa_1_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First fatty acid length content not valid!", "Lipid not registable");
                    return;
                }
                if (gl_fa_2_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second fatty acid length content not valid!", "Lipid not registable");
                    return;
                }
                if (gl_db_1_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First double bond content not valid!", "Lipid not registable");
                    return;
                }
                if (gl_db_2_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second double bond content not valid!", "Lipid not registable");
                    return;
                }
                if (gl_hydroxyl_1_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First hydroxyl content not valid!", "Lipid not registable");
                    return;
                }
                if (gl_hydroxyl_2_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second hydroxyl content not valid!", "Lipid not registable");
                    return;
                }
                if (((gl_lipid)currentLipid).contains_sugar)
                {
                    if (((gl_lipid)currentLipid).hgValues.Count == 0)
                    {
                        MessageBox.Show("No head group selected!", "Lipid not registable");
                        return;                    
                    }
                    if (((gl_lipid)currentLipid).fag1.faTypes["FAx"] || ((gl_lipid)currentLipid).fag2.faTypes["FAx"])
                    {
                        MessageBox.Show("Both fatty acids must be selected!", "Lipid not registable");
                        return;
                    }
                }
                else
                {
                    if (((gl_lipid)currentLipid).fag1.faTypes["FAx"] && ((gl_lipid)currentLipid).fag2.faTypes["FAx"] && ((gl_lipid)currentLipid).fag3.faTypes["FAx"])
                    {
                        MessageBox.Show("No fatty acid selected!", "Lipid not registable");
                        return;
                    }
                    if (gl_fa_3_textbox.BackColor == alert_color)
                    {
                        MessageBox.Show("Third fatty acid length content not valid!", "Lipid not registable");
                        return;
                    }
                    if (gl_db_3_textbox.BackColor == alert_color)
                    {
                        MessageBox.Show("Third double bond content not valid!", "Lipid not registable");
                        return;
                    }
                    if (gl_hydroxyl_3_textbox.BackColor == alert_color)
                    {
                        MessageBox.Show("Third hydroxyl content not valid!", "Lipid not registable");
                        return;
                    }
                }
                lipidCreatorForm.registered_lipids.Add(new gl_lipid((gl_lipid)currentLipid));
            }
            
            
            else if (currentLipid is pl_lipid)
            {
                if (((pl_lipid)currentLipid).hgValues.Count == 0)
                {
                    MessageBox.Show("No head group selected!", "Lipid not registable");
                    return;                    
                }
                
                if (((pl_lipid)currentLipid).fag1.faTypes["FAx"] && ((pl_lipid)currentLipid).fag2.faTypes["FAx"])
                {
                    MessageBox.Show("At least one fatty acid must be selected!", "Lipid not registable");
                    return;
                }
                
                if (pl_fa_1_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First fatty acid length content not valid!", "Lipid not registable");
                    return;
                }
                if (pl_fa_2_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second fatty acid length content not valid!", "Lipid not registable");
                    return;
                }
                if (pl_db_1_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First double bond content not valid!", "Lipid not registable");
                    return;
                }
                if (pl_db_2_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second double bond content not valid!", "Lipid not registable");
                    return;
                }
                if (pl_hydroxyl_1_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("First hydroxyl content not valid!", "Lipid not registable");
                    return;
                }
                if (pl_hydroxyl_2_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Second hydroxyl content not valid!", "Lipid not registable");
                    return;
                }
            
                lipidCreatorForm.registered_lipids.Add(new pl_lipid((pl_lipid)currentLipid));
            }
            
            
            else if (currentLipid is sl_lipid)
            {
                if (((sl_lipid)currentLipid).hgValues.Count == 0)
                {
                    MessageBox.Show("No head group selected!", "Lipid not registable");
                    return;                    
                }
                
                if (sl_lcb_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Long chain base length content not valid!", "Lipid not registable");
                    return;
                }
                if (sl_fa_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("Fatty acid length content not valid!", "Lipid not registable");
                    return;
                }
                if (sl_db_1_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("FA double bond content not valid!", "Lipid not registable");
                    return;
                }
                if (sl_db_2_textbox.BackColor == alert_color)
                {
                    MessageBox.Show("LCB double bond content not valid!", "Lipid not registable");
                    return;
                }
                
                lipidCreatorForm.registered_lipids.Add(new sl_lipid((sl_lipid)currentLipid));
            }
            refresh_registered_lipids_table();
        }
        
        public void refresh_registered_lipids_table()
        {
            registered_lipids_datatable.Clear();
            foreach (lipid current_lipid in lipidCreatorForm.registered_lipids)
            {
                DataRow row = registered_lipids_datatable.NewRow();
                if (current_lipid is cl_lipid)
                {
                    cl_lipid currentCLLipid = (cl_lipid)current_lipid;
                    row["Class"] = "Cardiolipin";
                    row["Building Block 1"] = "FA: " + currentCLLipid.fag1.lengthInfo + "; DB: " + currentCLLipid.fag1.dbInfo + "; OH: " + currentCLLipid.fag1.hydroxylInfo;
                    row["Building Block 2"] = "FA: " + currentCLLipid.fag2.lengthInfo + "; DB: " + currentCLLipid.fag2.dbInfo + "; OH: " + currentCLLipid.fag2.hydroxylInfo;
                    row["Building Block 3"] = "FA: " + currentCLLipid.fag3.lengthInfo + "; DB: " + currentCLLipid.fag3.dbInfo + "; OH: " + currentCLLipid.fag3.hydroxylInfo;
                    row["Building Block 4"] = "FA: " + currentCLLipid.fag4.lengthInfo + "; DB: " + currentCLLipid.fag4.dbInfo + "; OH: " + currentCLLipid.fag4.hydroxylInfo;
                }
                else if (current_lipid is gl_lipid)
                {
                    gl_lipid currentGLLipid = (gl_lipid)current_lipid;
                    row["Class"] = "Glycerolipid";
                    row["Building Block 1"] = "FA: " + currentGLLipid.fag1.lengthInfo + "; DB: " + currentGLLipid.fag1.dbInfo + "; OH: " + currentGLLipid.fag1.hydroxylInfo;
                    row["Building Block 2"] = "FA: " + currentGLLipid.fag2.lengthInfo + "; DB: " + currentGLLipid.fag2.dbInfo + "; OH: " + currentGLLipid.fag2.hydroxylInfo;
                    if (currentGLLipid.contains_sugar)
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
                else if (current_lipid is pl_lipid)
                {
                    pl_lipid currentPLLipid = (pl_lipid)current_lipid;
                    String headgroups = "";
                    foreach (int hgValue in currentPLLipid.hgValues)
                    {
                        if (headgroups != "") headgroups += ", ";
                        headgroups += currentPLLipid.headGroupNames[hgValue];
                    }
                    row["Class"] = "Phospholipid";
                    row["Building Block 1"] = "HG: " + headgroups;
                    row["Building Block 2"] = "FA: " + currentPLLipid.fag1.lengthInfo + "; DB: " + currentPLLipid.fag1.dbInfo + "; OH: " + currentPLLipid.fag1.hydroxylInfo;
                    row["Building Block 3"] = "FA: " + currentPLLipid.fag2.lengthInfo + "; DB: " + currentPLLipid.fag2.dbInfo + "; OH: " + currentPLLipid.fag2.hydroxylInfo;
                }
                else if (current_lipid is sl_lipid)
                {
                    sl_lipid currentSLLipid = (sl_lipid)current_lipid;
                    String headgroups = "";
                    foreach (int hgValue in currentSLLipid.hgValues)
                    {
                        if (headgroups != "") headgroups += ", ";
                        headgroups += currentSLLipid.headGroupNames[hgValue];
                    }
                    row["Class"] = "Sphingolipid";
                    row["Building Block 1"] = "HG: " + headgroups;
                    row["Building Block 2"] = "LCB: " + currentSLLipid.lcb.lengthInfo + "; DB: " + currentSLLipid.lcb.dbInfo;
                    row["Building Block 3"] = "FA: " + currentSLLipid.fag.lengthInfo + "; DB: " + currentSLLipid.fag.dbInfo;
                }
                registered_lipids_datatable.Rows.Add(row);
                lipids_gridview.DataSource = registered_lipids_datatable;
                
                
                for (int i = 0; i < lipids_gridview.Rows.Count; ++i)
                {
                    lipids_gridview.Rows[i].Cells["Edit"].Value = editImage;
                    lipids_gridview.Rows[i].Cells["Delete"].Value = deleteImage;
                }
                lipids_gridview.Update();
                lipids_gridview.Refresh();
            }
        }
        
        public void lipids_gridview_double_click(Object sender, EventArgs e)
        {
            int rowIndex = ((DataGridView)sender).CurrentCell.RowIndex;
            int colIndex = ((DataGridView)sender).CurrentCell.ColumnIndex;
            if (((DataGridView)sender).Columns[colIndex].Name == "Edit")
            {
            
                lipid current_lipid = (lipid)lipidCreatorForm.registered_lipids[rowIndex];
                int tabIndex = 0;
                if (current_lipid is cl_lipid)
                {
                    tabIndex = 0;
                    lipidCreatorForm.lipidTabList[tabIndex] = new cl_lipid((cl_lipid)current_lipid);
                }
                else if (current_lipid is gl_lipid)
                {
                    tabIndex = 1;
                    lipidCreatorForm.lipidTabList[tabIndex] = new gl_lipid((gl_lipid)current_lipid);
                }
                else if (current_lipid is pl_lipid)
                {
                    tabIndex = 2;
                    lipidCreatorForm.lipidTabList[tabIndex] = new pl_lipid((pl_lipid)current_lipid);
                }
                else if (current_lipid is sl_lipid)
                {
                    tabIndex = 3;
                    lipidCreatorForm.lipidTabList[tabIndex] = new sl_lipid((sl_lipid)current_lipid);
                }
                currentLipid = current_lipid;
                lipid_modifications[tabIndex] = rowIndex;
                tab_control.SelectedIndex = tabIndex;
                changeTab(tabIndex);
                
            }
            else if (((DataGridView)sender).Columns[colIndex].Name == "Delete")
            {
                lipidCreatorForm.registered_lipids.RemoveAt(rowIndex);
                refresh_registered_lipids_table();
            }
        }
        
        
        public void lipids_gridview_keydown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lipidCreatorForm.registered_lipids.Count > 0 && ((DataGridView)sender).SelectedRows.Count > 0)
            {   
                lipidCreatorForm.registered_lipids.RemoveAt(((DataGridView)sender).SelectedRows[0].Index);
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
            lipidCreatorForm.assemble_lipids();
            LipidsReview lipidsReview = new LipidsReview(lipidCreatorForm, lipidCreatorForm.all_lipids, lipidCreatorForm.all_lipids_unique);
            lipidsReview.Owner = this;
            lipidsReview.ShowInTaskbar = false;
            lipidsReview.ShowDialog();
            lipidsReview.Dispose();
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
