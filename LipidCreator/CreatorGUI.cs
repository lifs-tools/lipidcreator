using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LipidCreator
{
    public partial class CreatorGUI : Form
    {
    
        public LipidCreatorForm lipidCreatorForm;
        public lipid currentLipid;
        public DataTable registered_lipids_datatable;
        public int[] lipid_modifications;
        public Color alert_color = Color.FromArgb(255, 180, 180);
        
        public CreatorGUI(LipidCreatorForm lipidCreatorForm)
        {
            this.lipidCreatorForm = lipidCreatorForm;
            InitializeComponent();
            lipid_modifications = new int[]{-1, -1, -1, -1};
            
            registered_lipids_datatable = new DataTable("Daten");
            registered_lipids_datatable.Columns.Add(new DataColumn("Class"));
            registered_lipids_datatable.Columns.Add(new DataColumn("Building Block 1"));
            registered_lipids_datatable.Columns.Add(new DataColumn("Building Block 2"));
            registered_lipids_datatable.Columns.Add(new DataColumn("Building Block 3"));
            registered_lipids_datatable.Columns.Add(new DataColumn("Building Block 4"));
            
        }
        
        public void tabIndexChanged(Object sender, EventArgs e)
        {
            changeTab(((TabControl)sender).SelectedIndex);
        }
        
        public void resetTextBoxBackground(Object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.White;
        }

        public void changeTab(int index)
        {
            currentLipid = (lipid)lipidCreatorForm.lipidTabList[index];
            
            if (index == 0)
            {
                cl_lipid currentCLLipid = (cl_lipid)currentLipid;
                cl_fa_1_textbox.Text = currentCLLipid.fag1.lengthInfo;
                cl_db_1_textbox.Text = currentCLLipid.fag1.dbInfo;
                cl_fa_1_combobox.SelectedIndex = currentCLLipid.fag1.chainType;
                cl_fa_1_gb_1_checkbox_1.Checked = currentCLLipid.fag1.faTypes["FA"];
                cl_fa_1_gb_1_checkbox_2.Checked = currentCLLipid.fag1.faTypes["FAp"];
                cl_fa_1_gb_1_checkbox_3.Checked = currentCLLipid.fag1.faTypes["FAe"];
                cl_fa_1_gb_1_checkbox_4.Checked = currentCLLipid.fag1.faTypes["FAh"];
                
                cl_fa_2_textbox.Text = currentCLLipid.fag2.lengthInfo;
                cl_db_2_textbox.Text = currentCLLipid.fag2.dbInfo;
                cl_fa_2_combobox.SelectedIndex = currentCLLipid.fag2.chainType;
                cl_fa_2_gb_1_checkbox_1.Checked = currentCLLipid.fag2.faTypes["FA"];
                cl_fa_2_gb_1_checkbox_2.Checked = currentCLLipid.fag2.faTypes["FAp"];
                cl_fa_2_gb_1_checkbox_3.Checked = currentCLLipid.fag2.faTypes["FAe"];
                cl_fa_2_gb_1_checkbox_4.Checked = currentCLLipid.fag2.faTypes["FAh"];
                
                cl_fa_3_textbox.Text = currentCLLipid.fag3.lengthInfo;
                cl_db_3_textbox.Text = currentCLLipid.fag3.dbInfo;
                cl_fa_3_combobox.SelectedIndex = currentCLLipid.fag3.chainType;
                cl_fa_3_gb_1_checkbox_1.Checked = currentCLLipid.fag3.faTypes["FA"];
                cl_fa_3_gb_1_checkbox_2.Checked = currentCLLipid.fag3.faTypes["FAp"];
                cl_fa_3_gb_1_checkbox_3.Checked = currentCLLipid.fag3.faTypes["FAe"];
                cl_fa_3_gb_1_checkbox_4.Checked = currentCLLipid.fag3.faTypes["FAh"];
                
                cl_fa_4_textbox.Text = currentCLLipid.fag4.lengthInfo;
                cl_db_4_textbox.Text = currentCLLipid.fag4.dbInfo;
                cl_fa_4_combobox.SelectedIndex = currentCLLipid.fag4.chainType;
                cl_fa_4_gb_1_checkbox_1.Checked = currentCLLipid.fag4.faTypes["FA"];
                cl_fa_4_gb_1_checkbox_2.Checked = currentCLLipid.fag4.faTypes["FAp"];
                cl_fa_4_gb_1_checkbox_3.Checked = currentCLLipid.fag4.faTypes["FAe"];
                cl_fa_4_gb_1_checkbox_4.Checked = currentCLLipid.fag4.faTypes["FAh"];
                
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
                
                update_ranges(currentCLLipid.fag1, cl_fa_1_textbox, cl_fa_1_combobox);
                update_ranges(currentCLLipid.fag1, cl_db_1_textbox, null);
                update_ranges(currentCLLipid.fag2, cl_fa_2_textbox, cl_fa_2_combobox);
                update_ranges(currentCLLipid.fag2, cl_db_2_textbox, null);
                update_ranges(currentCLLipid.fag3, cl_fa_3_textbox, cl_fa_3_combobox);
                update_ranges(currentCLLipid.fag3, cl_db_3_textbox, null);
                update_ranges(currentCLLipid.fag4, cl_fa_4_textbox, cl_fa_4_combobox);
                update_ranges(currentCLLipid.fag4, cl_db_4_textbox, null);
            }
            else if (index == 1)
            {
            
                gl_lipid currentGLLipid = (gl_lipid)currentLipid;
                gl_fa_1_textbox.Text = currentGLLipid.fag1.lengthInfo;
                gl_db_1_textbox.Text = currentGLLipid.fag1.dbInfo;
                gl_fa_1_combobox.SelectedIndex = currentGLLipid.fag1.chainType;
                gl_fa_1_gb_1_checkbox_1.Checked = currentGLLipid.fag1.faTypes["FA"];
                gl_fa_1_gb_1_checkbox_2.Checked = currentGLLipid.fag1.faTypes["FAp"];
                gl_fa_1_gb_1_checkbox_3.Checked = currentGLLipid.fag1.faTypes["FAe"];
                gl_fa_1_gb_1_checkbox_4.Checked = currentGLLipid.fag1.faTypes["FAh"];
                
                gl_fa_2_textbox.Text = currentGLLipid.fag2.lengthInfo;
                gl_db_2_textbox.Text = currentGLLipid.fag2.dbInfo;
                gl_fa_2_combobox.SelectedIndex = currentGLLipid.fag2.chainType;
                gl_fa_2_gb_1_checkbox_1.Checked = currentGLLipid.fag2.faTypes["FA"];
                gl_fa_2_gb_1_checkbox_2.Checked = currentGLLipid.fag2.faTypes["FAp"];
                gl_fa_2_gb_1_checkbox_3.Checked = currentGLLipid.fag2.faTypes["FAe"];
                gl_fa_2_gb_1_checkbox_4.Checked = currentGLLipid.fag2.faTypes["FAh"];
                
                gl_fa_3_textbox.Text = currentGLLipid.fag3.lengthInfo;
                gl_db_3_textbox.Text = currentGLLipid.fag3.dbInfo;
                gl_fa_3_combobox.SelectedIndex = currentGLLipid.fag3.chainType;
                gl_fa_3_gb_1_checkbox_1.Checked = currentGLLipid.fag3.faTypes["FA"];
                gl_fa_3_gb_1_checkbox_2.Checked = currentGLLipid.fag3.faTypes["FAp"];
                gl_fa_3_gb_1_checkbox_3.Checked = currentGLLipid.fag3.faTypes["FAe"];
                gl_fa_3_gb_1_checkbox_4.Checked = currentGLLipid.fag3.faTypes["FAh"];
                
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
            }
            else if (index == 2)
            {
                pl_lipid currentPLLipid = (pl_lipid)currentLipid;
                
                pl_hg_combobox.SelectedIndex = currentPLLipid.hgValue;
                
                pl_fa_1_textbox.Text = currentPLLipid.fag1.lengthInfo;
                pl_db_1_textbox.Text = currentPLLipid.fag1.dbInfo;
                pl_fa_1_combobox.SelectedIndex = currentPLLipid.fag1.chainType;
                pl_fa_1_gb_1_checkbox_1.Checked = currentPLLipid.fag1.faTypes["FA"];
                pl_fa_1_gb_1_checkbox_2.Checked = currentPLLipid.fag1.faTypes["FAp"];
                pl_fa_1_gb_1_checkbox_3.Checked = currentPLLipid.fag1.faTypes["FAe"];
                pl_fa_1_gb_1_checkbox_4.Checked = currentPLLipid.fag1.faTypes["FAh"];
                
                pl_fa_2_textbox.Text = currentPLLipid.fag2.lengthInfo;
                pl_db_2_textbox.Text = currentPLLipid.fag2.dbInfo;
                pl_fa_2_combobox.SelectedIndex = currentPLLipid.fag2.chainType;
                pl_fa_2_gb_1_checkbox_1.Checked = currentPLLipid.fag2.faTypes["FA"];
                pl_fa_2_gb_1_checkbox_2.Checked = currentPLLipid.fag2.faTypes["FAp"];
                pl_fa_2_gb_1_checkbox_3.Checked = currentPLLipid.fag2.faTypes["FAe"];
                pl_fa_2_gb_1_checkbox_4.Checked = currentPLLipid.fag2.faTypes["FAh"];
            
                
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
                
                if (currentPLLipid.hg[0] == 'L') {
                    pl_fa_2_gb_1_checkbox_1.Enabled = false;
                    pl_fa_2_gb_1_checkbox_2.Enabled = false;
                    pl_fa_2_gb_1_checkbox_3.Enabled = false;
                    pl_fa_2_gb_1_checkbox_4.Enabled = false;
                    pl_fa_2_combobox.Enabled = false;
                    pl_fa_2_textbox.Enabled = false;
                    pl_db_2_textbox.Enabled = false;
                }
            }
            else if (index == 3)
            {
                sl_lipid currentSLLipid = (sl_lipid)currentLipid;
                
                sl_hg_combobox.SelectedIndex = currentSLLipid.hgValue;
                
                sl_lcb_textbox.Text = currentSLLipid.lcb;
                sl_db_2_textbox.Text = currentSLLipid.lcb_db;
                sl_lcb_combobox.SelectedIndex = currentSLLipid.lcbType;
                sl_hydroxy_combobox.SelectedIndex = currentSLLipid.hydroxyValue;
                
                sl_fa_textbox.Text = currentSLLipid.fag.lengthInfo;
                sl_db_1_textbox.Text = currentSLLipid.fag.dbInfo;
                sl_fa_combobox.SelectedIndex = currentSLLipid.fag.chainType;
                sl_fa_gb_1_checkbox_1.Checked = currentSLLipid.fag.faTypes["FA"];
                sl_fa_gb_1_checkbox_2.Checked = currentSLLipid.fag.faTypes["FAp"];
                sl_fa_gb_1_checkbox_3.Checked = currentSLLipid.fag.faTypes["FAe"];
                sl_fa_gb_1_checkbox_4.Checked = currentSLLipid.fag.faTypes["FAh"];
            
                
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
            update_ranges(((cl_lipid)currentLipid).fag1, cl_fa_1_textbox, ((ComboBox)sender));
        }
        public void cl_fa_2_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((cl_lipid)currentLipid).fag1, cl_fa_2_textbox, ((ComboBox)sender));
        }
        public void cl_fa_3_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag3.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((cl_lipid)currentLipid).fag1, cl_fa_3_textbox, ((ComboBox)sender));
        }
        public void cl_fa_4_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag4.chainType = ((ComboBox)sender).SelectedIndex;
            update_ranges(((cl_lipid)currentLipid).fag1, cl_fa_4_textbox, ((ComboBox)sender));
        }
        
        
        public void update_ranges(fattyAcidGroup fag, TextBox tb, ComboBox cb)
        {
            HashSet<int> lengths = lipidCreatorForm.parseRange(tb.Text, (cb != null) ? 2 : 0,  (cb != null) ? 30 : 6, (cb != null) ? cb.SelectedIndex : 0);
            if (cb == null)
            {
                fag.dbs = lengths;
            }
            else
            {
                fag.lengths = lengths;
            }
            tb.BackColor = (lengths == null) ? alert_color : Color.White;
        }
        
        public void cl_fa_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag1, (TextBox)sender, cl_fa_1_combobox);
        }
        public void cl_fa_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag2, (TextBox)sender, cl_fa_2_combobox);
        }
        public void cl_fa_3_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag3.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag3, (TextBox)sender, cl_fa_3_combobox);
        }
        public void cl_fa_4_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag4.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag4, (TextBox)sender, cl_fa_4_combobox);
        }
        
        public void cl_db_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag1, (TextBox)sender, null);
        }
        public void cl_db_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag2, (TextBox)sender, null);
        }
        public void cl_db_3_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag3.dbInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag3, (TextBox)sender, null);
        }
        public void cl_db_4_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag4.dbInfo = ((TextBox)sender).Text;
            update_ranges(((cl_lipid)currentLipid).fag4, (TextBox)sender, null);
        }
        
        public void cl_fa_1_gb_1_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag1.faTypes["FA"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag1.faTypes["FAx"] = !((cl_lipid)currentLipid).fag1.any_fa_checked();
        }
        public void cl_fa_1_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag1.faTypes["FAx"] = !((cl_lipid)currentLipid).fag1.any_fa_checked();
        }
        public void cl_fa_1_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag1.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag1.faTypes["FAx"] = !((cl_lipid)currentLipid).fag1.any_fa_checked();
        }
        public void cl_fa_1_gb_1_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag1.faTypes["FAh"] = ((CheckBox)sender).Checked;
            ((cl_lipid)currentLipid).fag1.faTypes["FAx"] = !((cl_lipid)currentLipid).fag1.any_fa_checked();
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
        public void cl_fa_2_gb_1_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag2.faTypes["FAh"] = ((CheckBox)sender).Checked;
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
        public void cl_fa_3_gb_1_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag3.faTypes["FAh"] = ((CheckBox)sender).Checked;
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
        public void cl_fa_4_gb_1_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((cl_lipid)currentLipid).fag4.faTypes["FAh"] = ((CheckBox)sender).Checked;
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
        
        
        ////////////////////// GL ////////////////////////////////
        
        public void gl_fa_1_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag1.chainType = ((ComboBox)sender).SelectedIndex;
        }
        public void gl_fa_2_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
        }
        public void gl_fa_3_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag3.chainType = ((ComboBox)sender).SelectedIndex;
        }
        
        public void gl_fa_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag1, (TextBox)sender, gl_fa_1_combobox);
        }
        public void gl_fa_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag2, (TextBox)sender, gl_fa_2_combobox);
        }
        public void gl_fa_3_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag3.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag3, (TextBox)sender, gl_fa_3_combobox);
        }
        
        public void gl_db_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag1, (TextBox)sender, null);
        }
        public void gl_db_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag2, (TextBox)sender, null);
        }
        public void gl_db_3_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag3.dbInfo = ((TextBox)sender).Text;
            update_ranges(((gl_lipid)currentLipid).fag3, (TextBox)sender, null);
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
        }
        public void gl_fa_1_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((gl_lipid)currentLipid).fag1.faTypes["FAx"] = !((gl_lipid)currentLipid).fag1.any_fa_checked();
        }
        public void gl_fa_1_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag1.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((gl_lipid)currentLipid).fag1.faTypes["FAx"] = !((gl_lipid)currentLipid).fag1.any_fa_checked();
        }
        public void gl_fa_1_gb_1_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag1.faTypes["FAh"] = ((CheckBox)sender).Checked;
            ((gl_lipid)currentLipid).fag1.faTypes["FAx"] = !((gl_lipid)currentLipid).fag1.any_fa_checked();
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
        public void gl_fa_2_gb_1_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag2.faTypes["FAh"] = ((CheckBox)sender).Checked;
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
        public void gl_fa_3_gb_1_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((gl_lipid)currentLipid).fag3.faTypes["FAh"] = ((CheckBox)sender).Checked;
            ((gl_lipid)currentLipid).fag3.faTypes["FAx"] = !((gl_lipid)currentLipid).fag3.any_fa_checked();
        }
        
        
        ////////////////////// PL ////////////////////////////////
        
        public void pl_hg_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).hgValue = ((ComboBox)sender).SelectedIndex;
        }
    
        public void pl_fa_1_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag1.chainType = ((ComboBox)sender).SelectedIndex;
        }
        public void pl_fa_2_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag2.chainType = ((ComboBox)sender).SelectedIndex;
        }
        
        public void pl_fa_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag1.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((pl_lipid)currentLipid).fag1, (TextBox)sender, pl_fa_1_combobox);
        }
        public void pl_fa_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag2.lengthInfo = ((TextBox)sender).Text;
            update_ranges(((pl_lipid)currentLipid).fag2, (TextBox)sender, pl_fa_2_combobox);
        }
        
        public void pl_db_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag1.dbInfo = ((TextBox)sender).Text;
            update_ranges(((pl_lipid)currentLipid).fag1, (TextBox)sender, null);
        }
        public void pl_db_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag2.dbInfo = ((TextBox)sender).Text;
            update_ranges(((pl_lipid)currentLipid).fag2, (TextBox)sender, null);
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
        }
        public void pl_fa_1_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag1.faTypes["FAp"] = ((CheckBox)sender).Checked;
            ((pl_lipid)currentLipid).fag1.faTypes["FAx"] = !((pl_lipid)currentLipid).fag1.any_fa_checked();
        }
        public void pl_fa_1_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag1.faTypes["FAe"] = ((CheckBox)sender).Checked;
            ((pl_lipid)currentLipid).fag1.faTypes["FAx"] = !((pl_lipid)currentLipid).fag1.any_fa_checked();
        }
        public void pl_fa_1_gb_1_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag1.faTypes["FAh"] = ((CheckBox)sender).Checked;
            ((pl_lipid)currentLipid).fag1.faTypes["FAx"] = !((pl_lipid)currentLipid).fag1.any_fa_checked();
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
        public void pl_fa_2_gb_1_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((pl_lipid)currentLipid).fag2.faTypes["FAh"] = ((CheckBox)sender).Checked;
            ((pl_lipid)currentLipid).fag2.faTypes["FAx"] = !((pl_lipid)currentLipid).fag2.any_fa_checked();
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
        
        public void sl_fa_gb_1_checkbox_1_checkedChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).fag.faTypes["FA"] = ((CheckBox)sender).Checked;
        }
        public void sl_fa_gb_1_checkbox_2_checkedChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).fag.faTypes["FAp"] = ((CheckBox)sender).Checked;
        }
        public void sl_fa_gb_1_checkbox_3_checkedChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).fag.faTypes["FAe"] = ((CheckBox)sender).Checked;
        }
        public void sl_fa_gb_1_checkbox_4_checkedChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).fag.faTypes["FAh"] = ((CheckBox)sender).Checked;
        }
        
        public void sl_hg_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).hgValue = ((ComboBox)sender).SelectedIndex;
        }
        
        public void sl_db_1_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).fag.dbInfo = ((TextBox)sender).Text;
        }
        public void sl_db_2_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).lcb_db = ((TextBox)sender).Text;
        }
        
        public void sl_fa_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).fag.lengthInfo = ((TextBox)sender).Text;
        }
        public void sl_lcb_textbox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).lcb = ((TextBox)sender).Text;
        }
        
        
        public void sl_fa_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).fag.chainType = ((ComboBox)sender).SelectedIndex;
        }
        
        public void sl_lcb_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).lcbType = ((ComboBox)sender).SelectedIndex;
        }
        
        public void sl_hydroxy_combobox_valueChanged(Object sender, EventArgs e)
        {
            ((sl_lipid)currentLipid).hydroxyValue = ((ComboBox)sender).SelectedIndex;
        }
        
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
            if (currentLipid is cl_lipid)
            {
                lipidCreatorForm.registered_lipids.Add(new cl_lipid((cl_lipid)currentLipid));
            }
            else if (currentLipid is gl_lipid)
            {
                lipidCreatorForm.registered_lipids.Add(new gl_lipid((gl_lipid)currentLipid));
            }
            else if (currentLipid is pl_lipid)
            {
                lipidCreatorForm.registered_lipids.Add(new pl_lipid((pl_lipid)currentLipid));
            }
            else if (currentLipid is sl_lipid)
            {
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
                    row["Building Block 1"] = "FA: " + currentCLLipid.fag1.lengthInfo + "; DB: " + currentCLLipid.fag1.dbInfo;
                    row["Building Block 2"] = "FA: " + currentCLLipid.fag2.lengthInfo + "; DB: " + currentCLLipid.fag2.dbInfo;
                    row["Building Block 3"] = "FA: " + currentCLLipid.fag3.lengthInfo + "; DB: " + currentCLLipid.fag3.dbInfo;
                    row["Building Block 4"] = "FA: " + currentCLLipid.fag4.lengthInfo + "; DB: " + currentCLLipid.fag4.dbInfo;
                }
                else if (current_lipid is gl_lipid)
                {
                    gl_lipid currentGLLipid = (gl_lipid)current_lipid;
                    row["Class"] = "Glycerolipid";
                    row["Building Block 1"] = "FA: " + currentGLLipid.fag1.lengthInfo + "; DB: " + currentGLLipid.fag1.dbInfo;
                    row["Building Block 2"] = "FA: " + currentGLLipid.fag2.lengthInfo + "; DB: " + currentGLLipid.fag2.dbInfo;
                    row["Building Block 3"] = "FA: " + currentGLLipid.fag3.lengthInfo + "; DB: " + currentGLLipid.fag3.dbInfo;
                }
                else if (current_lipid is pl_lipid)
                {
                    pl_lipid currentPLLipid = (pl_lipid)current_lipid;
                    row["Class"] = "Phospholipid";
                    row["Building Block 1"] = "HG: " + pl_hg_combobox.Items[currentPLLipid.hgValue];
                    row["Building Block 2"] = "FA: " + currentPLLipid.fag1.lengthInfo + "; DB: " + currentPLLipid.fag1.dbInfo;
                    row["Building Block 3"] = "FA: " + currentPLLipid.fag2.lengthInfo + "; DB: " + currentPLLipid.fag2.dbInfo;
                }
                else if (current_lipid is sl_lipid)
                {
                    sl_lipid currentSLLipid = (sl_lipid)current_lipid;
                    row["Class"] = "Sphingolipid";
                    row["Building Block 1"] = "HG: " + sl_hg_combobox.Items[currentSLLipid.hgValue];
                    row["Building Block 2"] = "LCB: " + currentSLLipid.lcb + "; DB: " + currentSLLipid.lcb_db;
                    row["Building Block 3"] = "FA: " + currentSLLipid.fag.lengthInfo + "; DB: " + currentSLLipid.fag.dbInfo;
                }
                registered_lipids_datatable.Rows.Add(row);
                lipids_gridview.DataSource = registered_lipids_datatable;
                lipids_gridview.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                lipids_gridview.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                lipids_gridview.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
                lipids_gridview.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
                lipids_gridview.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
                lipids_gridview.Update();
                lipids_gridview.Refresh();
            }
        }
        
        public void lipids_gridview_double_click(Object sender, EventArgs e)
        {
            int index = ((DataGridView)sender).CurrentCell.RowIndex;
            lipid current_lipid = (lipid)lipidCreatorForm.registered_lipids[index];
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
            lipid_modifications[tabIndex] = index;
            tab_control.SelectedIndex = tabIndex;
            changeTab(tabIndex);
        }
        
        
        public void lipids_gridview_keydown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lipidCreatorForm.registered_lipids.Count > 0 && ((DataGridView)sender).SelectedRows.Count > 0)
            {   
                Console.WriteLine(((DataGridView)sender).SelectedRows[0].Index);
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
            LipidsReview lipidsReview = new LipidsReview(lipidCreatorForm.all_lipids);
            lipidsReview.Owner = this;
            lipidsReview.ShowInTaskbar = false;
            lipidsReview.ShowDialog();
            lipidsReview.Dispose();
        }
    }
}
