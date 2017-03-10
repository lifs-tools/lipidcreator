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
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

namespace LipidCreator
{
    partial class CreatorGUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private Image deleteImage;
        private Image editImage;
        private Image addImage;
        private bool initialCall = true;
        
        private System.Timers.Timer timer1;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuFile;
        private System.Windows.Forms.MenuItem menuImport;
        private System.Windows.Forms.MenuItem menuImportPredefined;
        private System.Windows.Forms.MenuItem menuExport;
        private System.Windows.Forms.MenuItem menuDash;
        private System.Windows.Forms.MenuItem menuExit;
        
        private TabControl tab_control = new TabControl();
        private TabPage glycerolipids_tab;
        private TabPage phospholipids_tab;
        private TabPage sphingolipids_tab;
        private GroupBox lipids_groupbox;

        private Button cl_add_lipid_button;
        private Button gl_add_lipid_button;
        private Button pl_add_lipid_button;
        private Button sl_add_lipid_button;
        private Button cl_reset_lipid_button;
        private Button gl_reset_lipid_button;
        private Button pl_reset_lipid_button;
        private Button sl_reset_lipid_button;
        private Button cl_modify_lipid_button;
        private Button gl_modify_lipid_button;
        private Button pl_modify_lipid_button;
        private Button sl_modify_lipid_button;
        private Button cl_ms2fragments_lipid_button;
        private Button gl_ms2fragments_lipid_button;
        private Button pl_ms2fragments_lipid_button;
        private Button sl_ms2fragments_lipid_button;

        Image cardio_backbone_image;
        Image cardio_backbone_image_fa1e;
        Image cardio_backbone_image_fa1p;
        Image cardio_backbone_image_fa2e;
        Image cardio_backbone_image_fa2p;
        Image cardio_backbone_image_fa3e;
        Image cardio_backbone_image_fa3p;
        Image cardio_backbone_image_fa4e;
        Image cardio_backbone_image_fa4p;
        Image glycero_backbone_image;
        Image glycero_backbone_image_orig;
        Image glycero_backbone_image_plant;
        Image glycero_backbone_image_fa1e;
        Image glycero_backbone_image_fa1p;
        Image glycero_backbone_image_fa2e;
        Image glycero_backbone_image_fa2p;
        Image glycero_backbone_image_fa3e;
        Image glycero_backbone_image_fa3p;
        Image glycero_backbone_image_fa1e_orig;
        Image glycero_backbone_image_fa1p_orig;
        Image glycero_backbone_image_fa2e_orig;
        Image glycero_backbone_image_fa2p_orig;
        Image glycero_backbone_image_fa3e_orig;
        Image glycero_backbone_image_fa3p_orig;
        Image glycero_backbone_image_fa1e_plant;
        Image glycero_backbone_image_fa1p_plant;
        Image glycero_backbone_image_fa2e_plant;
        Image glycero_backbone_image_fa2p_plant;
        Image phospho_backbone_image;
        Image phospho_backbone_image_fa1e;
        Image phospho_backbone_image_fa1p;
        Image phospho_backbone_image_fa2e;
        Image phospho_backbone_image_fa2p;
        Image sphingo_backbone_image;

        PictureBox cl_picture_box;
        PictureBox gl_picture_box;
        PictureBox pl_picture_box;
        PictureBox sl_picture_box;
        
        ListBox gl_hg_listbox;
        ListBox pl_hg_listbox;
        ListBox sl_hg_listbox;

        TextBox cl_fa_1_textbox;
        TextBox cl_fa_2_textbox;
        TextBox cl_fa_3_textbox;
        TextBox cl_fa_4_textbox;
        TextBox gl_fa_1_textbox;
        TextBox gl_fa_2_textbox;
        TextBox gl_fa_3_textbox;
        TextBox pl_fa_1_textbox;
        TextBox pl_fa_2_textbox;
        TextBox sl_lcb_textbox;
        TextBox sl_fa_textbox;

        ComboBox cl_fa_1_combobox;
        ComboBox cl_fa_2_combobox;
        ComboBox cl_fa_3_combobox;
        ComboBox cl_fa_4_combobox;
        ComboBox gl_fa_1_combobox;
        ComboBox gl_fa_2_combobox;
        ComboBox gl_fa_3_combobox;
        ComboBox pl_fa_1_combobox;
        ComboBox pl_fa_2_combobox;
        ComboBox sl_lcb_combobox;
        ComboBox sl_fa_combobox;


        CheckBox cl_fa_1_gb_1_checkbox_1;
        CheckBox cl_fa_1_gb_1_checkbox_2;
        CheckBox cl_fa_1_gb_1_checkbox_3;
        CheckBox cl_fa_2_gb_1_checkbox_1;
        CheckBox cl_fa_2_gb_1_checkbox_2;
        CheckBox cl_fa_2_gb_1_checkbox_3;
        CheckBox cl_fa_3_gb_1_checkbox_1;
        CheckBox cl_fa_3_gb_1_checkbox_2;
        CheckBox cl_fa_3_gb_1_checkbox_3;
        CheckBox cl_fa_4_gb_1_checkbox_1;
        CheckBox cl_fa_4_gb_1_checkbox_2;
        CheckBox cl_fa_4_gb_1_checkbox_3;
        CheckBox gl_fa_1_gb_1_checkbox_1;
        CheckBox gl_fa_1_gb_1_checkbox_2;
        CheckBox gl_fa_1_gb_1_checkbox_3;
        CheckBox gl_fa_2_gb_1_checkbox_1;
        CheckBox gl_fa_2_gb_1_checkbox_2;
        CheckBox gl_fa_2_gb_1_checkbox_3;
        CheckBox gl_fa_3_gb_1_checkbox_1;
        CheckBox gl_fa_3_gb_1_checkbox_2;
        CheckBox gl_fa_3_gb_1_checkbox_3;
        CheckBox pl_fa_1_gb_1_checkbox_1;
        CheckBox pl_fa_1_gb_1_checkbox_2;
        CheckBox pl_fa_1_gb_1_checkbox_3;
        CheckBox pl_fa_2_gb_1_checkbox_1;
        CheckBox pl_fa_2_gb_1_checkbox_2;
        CheckBox pl_fa_2_gb_1_checkbox_3;
        CheckBox pl_is_cl;
        CheckBox gl_contains_sugar;

        GroupBox cl_positive_adduct;
        GroupBox cl_negative_adduct;
        GroupBox gl_positive_adduct;
        GroupBox gl_negative_adduct;
        GroupBox pl_positive_adduct;
        GroupBox pl_negative_adduct;
        GroupBox sl_positive_adduct;
        GroupBox sl_negative_adduct;

        CheckBox cl_pos_adduct_checkbox_1;
        CheckBox cl_pos_adduct_checkbox_2;
        CheckBox cl_pos_adduct_checkbox_3;
        CheckBox cl_neg_adduct_checkbox_1;
        CheckBox cl_neg_adduct_checkbox_2;
        CheckBox cl_neg_adduct_checkbox_3;
        CheckBox cl_neg_adduct_checkbox_4;
        CheckBox gl_pos_adduct_checkbox_1;
        CheckBox gl_pos_adduct_checkbox_2;
        CheckBox gl_pos_adduct_checkbox_3;
        CheckBox gl_neg_adduct_checkbox_1;
        CheckBox gl_neg_adduct_checkbox_2;
        CheckBox gl_neg_adduct_checkbox_3;
        CheckBox gl_neg_adduct_checkbox_4;
        CheckBox pl_pos_adduct_checkbox_1;
        CheckBox pl_pos_adduct_checkbox_2;
        CheckBox pl_pos_adduct_checkbox_3;
        CheckBox pl_neg_adduct_checkbox_1;
        CheckBox pl_neg_adduct_checkbox_2;
        CheckBox pl_neg_adduct_checkbox_3;
        CheckBox pl_neg_adduct_checkbox_4;
        CheckBox sl_pos_adduct_checkbox_1;
        CheckBox sl_pos_adduct_checkbox_2;
        CheckBox sl_pos_adduct_checkbox_3;
        CheckBox sl_neg_adduct_checkbox_1;
        CheckBox sl_neg_adduct_checkbox_2;
        CheckBox sl_neg_adduct_checkbox_3;
        CheckBox sl_neg_adduct_checkbox_4;
        
        CheckBox clRepresentativeFA;
        CheckBox glRepresentativeFA;
        CheckBox plRepresentativeFA;

        TextBox cl_db_1_textbox;
        TextBox cl_db_2_textbox;
        TextBox cl_db_3_textbox;
        TextBox cl_db_4_textbox;
        TextBox gl_db_1_textbox;
        TextBox gl_db_2_textbox;
        TextBox gl_db_3_textbox;
        TextBox pl_db_1_textbox;
        TextBox pl_db_2_textbox;
        TextBox sl_db_1_textbox;
        TextBox sl_db_2_textbox;
        
        TextBox cl_hydroxyl_1_textbox;
        TextBox cl_hydroxyl_2_textbox;
        TextBox cl_hydroxyl_3_textbox;
        TextBox cl_hydroxyl_4_textbox;
        TextBox gl_hydroxyl_1_textbox;
        TextBox gl_hydroxyl_2_textbox;
        TextBox gl_hydroxyl_3_textbox;
        TextBox pl_hydroxyl_1_textbox;
        TextBox pl_hydroxyl_2_textbox;
        

        Label cl_db_1_label;
        Label cl_db_2_label;
        Label cl_db_3_label;
        Label cl_db_4_label;
        Label gl_db_1_label;
        Label gl_db_2_label;
        Label gl_db_3_label;
        Label pl_db_1_label;
        Label pl_db_2_label;
        Label sl_db_1_label;
        Label sl_db_2_label;
        Label sl_lcb_hydroxy_label;
        Label sl_fa_hydroxy_label;
        Label cl_hydroxyl_1_label;
        Label cl_hydroxyl_2_label;
        Label cl_hydroxyl_3_label;
        Label cl_hydroxyl_4_label;
        Label gl_hydroxyl_1_label;
        Label gl_hydroxyl_2_label;
        Label gl_hydroxyl_3_label;
        Label pl_hydroxyl_1_label;
        Label pl_hydroxyl_2_label;
        
        
        Label eastertext;

        Label gl_hg_label;
        Label pl_hg_label;
        Label sl_hg_label;
        
        ComboBox sl_lcb_hydroxy_combobox;
        ComboBox sl_fa_hydroxy_combobox;

        ToolTip toolTip1;

        DataGridView lipids_gridview;
        Button open_review_form_button;

        

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "LipidCreator";
            
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Timers.Timer(5);
            this.timer1.Elapsed += this.timer1_Tick;
            
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.Menu = this.mainMenu1;
            this.menuFile = new System.Windows.Forms.MenuItem ();
            this.menuImport = new System.Windows.Forms.MenuItem ();
            this.menuImportPredefined = new System.Windows.Forms.MenuItem();
            this.menuExport = new System.Windows.Forms.MenuItem ();
            this.menuDash = new System.Windows.Forms.MenuItem ();
            this.menuExit = new System.Windows.Forms.MenuItem ();
            this.mainMenu1.MenuItems.AddRange(new MenuItem[] { this.menuFile } );
            this.menuFile.MenuItems.AddRange(new MenuItem[]{ menuImport, menuImportPredefined, menuExport, menuDash, menuExit});
            this.menuFile.Index = 0;
            this.menuFile.Text = "&File";
            
            this.menuImport.Index = 0;
            this.menuImport.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
            this.menuImport.Text = "&Import";
            this.menuImport.Click += new System.EventHandler (menuImport_Click);
            
            this.menuImportPredefined.Index = 1;
            this.menuImportPredefined.Text = "Import Predefined";
            
            this.menuExport.Index = 2;
            this.menuExport.Shortcut = System.Windows.Forms.Shortcut.CtrlE;
            this.menuExport.Text = "&Export";
            this.menuExport.Click += new System.EventHandler (menuExport_Click);
            
            this.menuDash.Index = 3;
            this.menuDash.Text = "-";
            
            this.menuExit.Index = 4;
            this.menuExit.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            this.menuExit.Text = "E&xit";
            
            tab_control = new TabControl();
            this.Size = new System.Drawing.Size(1060, 800);
            glycerolipids_tab = new TabPage();
            phospholipids_tab = new TabPage();
            sphingolipids_tab = new TabPage();
            lipids_groupbox = new GroupBox();
            cl_add_lipid_button = new Button();
            gl_add_lipid_button = new Button();
            pl_add_lipid_button = new Button();
            sl_add_lipid_button = new Button();
            cl_reset_lipid_button = new Button();
            gl_reset_lipid_button = new Button();
            pl_reset_lipid_button = new Button();
            sl_reset_lipid_button = new Button();
            cl_modify_lipid_button = new Button();
            gl_modify_lipid_button = new Button();
            pl_modify_lipid_button = new Button();
            sl_modify_lipid_button = new Button();
            cl_ms2fragments_lipid_button = new Button();
            gl_ms2fragments_lipid_button = new Button();
            pl_ms2fragments_lipid_button = new Button();
            sl_ms2fragments_lipid_button = new Button();

            lipids_gridview = new DataGridView();
            open_review_form_button = new Button();

            cl_picture_box = new PictureBox();
            gl_picture_box = new PictureBox();
            pl_picture_box = new PictureBox();
            sl_picture_box = new PictureBox();
            
            String db_text = "No. DB";
            String hydroxyl_text = "Hydroxy No.";
            int db_length = 70;
            int sep = 15;
            int sepText = 20;
            int fa_length = 150;
            int top_low_buttons = 420;
            int left_groupboxes = 850;

            
            deleteImage = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/delete-small.png");
            editImage = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/edit-small.png");
            addImage = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/add-small.png");
            
            gl_hg_listbox = new ListBox();
            pl_hg_listbox = new ListBox();
            sl_hg_listbox = new ListBox();
            gl_hg_listbox.Items.AddRange(new String[]{"MGDG", "DGDG", "SQDG"});
            pl_hg_listbox.Items.AddRange(new String[]{"PA", "PC", "PE", "DMPE", "MMPE", "PG", "PI", "PIP", "PIP2", "PIP3", "PS"});
            sl_hg_listbox.Items.AddRange(new String[]{"Cer", "CerP", "GB3Cer", "GB4Cer", "GD3Cer", "GM3Cer", "GM4Cer", "HexCer", "HexCerS", "LacCer", "MIPCer", "MIP2Cer", "PECer", "PICer", "SM", "SPC", "SPH", "SPH-P"});
            
            cardio_backbone_image = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/CL_backbones.png");
            cardio_backbone_image_fa1e = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/CL_FAe1.png");
            cardio_backbone_image_fa2e = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/CL_FAe2.png");
            cardio_backbone_image_fa3e = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/CL_FAe3.png");
            cardio_backbone_image_fa4e = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/CL_FAe4.png");
            cardio_backbone_image_fa1p = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/CL_FAp1.png");
            cardio_backbone_image_fa2p = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/CL_FAp2.png");
            cardio_backbone_image_fa3p = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/CL_FAp3.png");
            cardio_backbone_image_fa4p = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/CL_FAp4.png");
            glycero_backbone_image_orig = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/GL_backbones.png");
            glycero_backbone_image_fa1e_orig = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/GL_FAe1.png");
            glycero_backbone_image_fa2e_orig = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/GL_FAe2.png");
            glycero_backbone_image_fa3e_orig = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/GL_FAe3.png");
            glycero_backbone_image_fa1p_orig = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/GL_FAp1.png");
            glycero_backbone_image_fa2p_orig = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/GL_FAp2.png");
            glycero_backbone_image_fa3p_orig = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/GL_FAp3.png");
            glycero_backbone_image_plant = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/GL_backbones_plant.png");
            glycero_backbone_image_fa1e_plant = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/GL_plant_FAe1.png");
            glycero_backbone_image_fa2e_plant = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/GL_plant_FAe2.png");
            glycero_backbone_image_fa1p_plant = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/GL_plant_FAp1.png");
            glycero_backbone_image_fa2p_plant = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/GL_plant_FAp2.png");
            phospho_backbone_image = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/PL_backbones.png");
            phospho_backbone_image_fa1e = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/PL_FAe1.png");
            phospho_backbone_image_fa2e = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/PL_FAe2.png");
            phospho_backbone_image_fa1p = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/PL_FAp1.png");
            phospho_backbone_image_fa2p = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/PL_FAp2.png");
            sphingo_backbone_image = Image.FromFile((lipidCreatorForm.opened_as_external ? lipidCreatorForm.prefix_path : "") + "images/backbones/SL_backbones.png");

            
            glycero_backbone_image = glycero_backbone_image_orig;
            glycero_backbone_image_fa1e = glycero_backbone_image_fa1e_orig;
            glycero_backbone_image_fa2e = glycero_backbone_image_fa2e_orig;
            glycero_backbone_image_fa3e = glycero_backbone_image_fa3e_orig;
            glycero_backbone_image_fa1p = glycero_backbone_image_fa1p_orig;
            glycero_backbone_image_fa2p = glycero_backbone_image_fa2p_orig;
            glycero_backbone_image_fa3p = glycero_backbone_image_fa3p_orig;
            
            
            cl_fa_1_textbox = new TextBox();
            cl_fa_2_textbox = new TextBox();
            cl_fa_3_textbox = new TextBox();
            cl_fa_4_textbox = new TextBox();
            cl_fa_1_combobox = new ComboBox();
            cl_fa_1_combobox.Items.Add("Fatty acid chain");
            cl_fa_1_combobox.Items.Add("Fatty acid chain - odd carbon no.");
            cl_fa_1_combobox.Items.Add("Fatty acid chain - even carbon no.");
            cl_fa_2_combobox = new ComboBox();
            cl_fa_2_combobox.Items.Add("Fatty acid chain");
            cl_fa_2_combobox.Items.Add("Fatty acid chain - odd carbon no.");
            cl_fa_2_combobox.Items.Add("Fatty acid chain - even carbon no.");
            cl_fa_3_combobox = new ComboBox();
            cl_fa_3_combobox.Items.Add("Fatty acid chain");
            cl_fa_3_combobox.Items.Add("Fatty acid chain - odd carbon no.");
            cl_fa_3_combobox.Items.Add("Fatty acid chain - even carbon no.");
            cl_fa_4_combobox = new ComboBox();
            cl_fa_4_combobox.Items.Add("Fatty acid chain");
            cl_fa_4_combobox.Items.Add("Fatty acid chain - odd carbon no.");
            cl_fa_4_combobox.Items.Add("Fatty acid chain - even carbon no.");
            cl_db_1_textbox = new TextBox();
            cl_db_2_textbox = new TextBox();
            cl_db_3_textbox = new TextBox();
            cl_db_4_textbox = new TextBox();
            cl_hydroxyl_1_textbox = new TextBox();
            cl_hydroxyl_2_textbox = new TextBox();
            cl_hydroxyl_3_textbox = new TextBox();
            cl_hydroxyl_4_textbox = new TextBox();
            cl_hydroxyl_1_label = new Label();
            cl_hydroxyl_2_label = new Label();
            cl_hydroxyl_3_label = new Label();
            cl_hydroxyl_4_label = new Label();
            cl_db_1_label = new Label();
            cl_db_2_label = new Label();
            cl_db_3_label = new Label();
            cl_db_4_label = new Label();
            gl_fa_1_textbox = new TextBox();
            gl_fa_2_textbox = new TextBox();
            gl_fa_3_textbox = new TextBox();
            gl_fa_1_combobox = new ComboBox();
            gl_fa_1_combobox.Items.Add("Fatty acid chain");
            gl_fa_1_combobox.Items.Add("Fatty acid chain - odd carbon no.");
            gl_fa_1_combobox.Items.Add("Fatty acid chain - even carbon no.");
            gl_fa_2_combobox = new ComboBox();
            gl_fa_2_combobox.Items.Add("Fatty acid chain");
            gl_fa_2_combobox.Items.Add("Fatty acid chain - odd carbon no.");
            gl_fa_2_combobox.Items.Add("Fatty acid chain - even carbon no.");
            gl_fa_3_combobox = new ComboBox();
            gl_fa_3_combobox.Items.Add("Fatty acid chain");
            gl_fa_3_combobox.Items.Add("Fatty acid chain - odd carbon no.");
            gl_fa_3_combobox.Items.Add("Fatty acid chain - even carbon no.");
            gl_db_1_textbox = new TextBox();
            gl_db_2_textbox = new TextBox();
            gl_db_3_textbox = new TextBox();
            gl_hydroxyl_1_textbox = new TextBox();
            gl_hydroxyl_2_textbox = new TextBox();
            gl_hydroxyl_3_textbox = new TextBox();
            gl_db_1_label = new Label();
            gl_db_2_label = new Label();
            gl_db_3_label = new Label();
            gl_hg_label = new Label();
            gl_hydroxyl_1_label = new Label();
            gl_hydroxyl_2_label = new Label();
            gl_hydroxyl_3_label = new Label();
            pl_fa_1_textbox = new TextBox();
            pl_fa_2_textbox = new TextBox();
            pl_fa_1_combobox = new ComboBox();
            pl_fa_1_combobox.Items.Add("Fatty acid chain");
            pl_fa_1_combobox.Items.Add("Fatty acid chain - odd carbon no.");
            pl_fa_1_combobox.Items.Add("Fatty acid chain - even carbon no.");
            pl_fa_2_combobox = new ComboBox();
            pl_fa_2_combobox.Items.Add("Fatty acid chain");
            pl_fa_2_combobox.Items.Add("Fatty acid chain - odd carbon no.");
            pl_fa_2_combobox.Items.Add("Fatty acid chain - even carbon no.");
            pl_db_1_textbox = new TextBox();
            pl_db_2_textbox = new TextBox();
            pl_hydroxyl_1_textbox = new TextBox();
            pl_hydroxyl_2_textbox = new TextBox();
            pl_db_1_label = new Label();
            pl_db_2_label = new Label();
            pl_hydroxyl_1_label = new Label();
            pl_hydroxyl_2_label = new Label();
            pl_hg_label = new Label();
            sl_lcb_textbox = new TextBox();
            sl_fa_textbox = new TextBox();
            sl_lcb_combobox = new ComboBox();
            sl_lcb_combobox.Items.Add("Long chain base");
            sl_lcb_combobox.Items.Add("Long chain base - odd carbon no.");
            sl_lcb_combobox.Items.Add("Long chain base - even carbon no.");
            sl_fa_combobox = new ComboBox();
            sl_fa_combobox.Items.Add("Fatty acid chain");
            sl_fa_combobox.Items.Add("Fatty acid chain - odd carbon no.");
            sl_fa_combobox.Items.Add("Fatty acid chain - even carbon no.");
            sl_db_1_textbox = new TextBox();
            sl_db_2_textbox = new TextBox();
            sl_db_1_label = new Label();
            sl_db_2_label = new Label();
            sl_hg_label = new Label();
            sl_lcb_hydroxy_label = new Label();
            sl_fa_hydroxy_label = new Label();
            eastertext = new Label();

            cl_fa_1_gb_1_checkbox_1 = new CheckBox();
            cl_fa_1_gb_1_checkbox_2 = new CheckBox();
            cl_fa_1_gb_1_checkbox_3 = new CheckBox();
            cl_fa_2_gb_1_checkbox_1 = new CheckBox();
            cl_fa_2_gb_1_checkbox_2 = new CheckBox();
            cl_fa_2_gb_1_checkbox_3 = new CheckBox();
            cl_fa_3_gb_1_checkbox_1 = new CheckBox();
            cl_fa_3_gb_1_checkbox_2 = new CheckBox();
            cl_fa_3_gb_1_checkbox_3 = new CheckBox();
            cl_fa_4_gb_1_checkbox_1 = new CheckBox();
            cl_fa_4_gb_1_checkbox_2 = new CheckBox();
            cl_fa_4_gb_1_checkbox_3 = new CheckBox();
            gl_fa_1_gb_1_checkbox_1 = new CheckBox();
            gl_fa_1_gb_1_checkbox_2 = new CheckBox();
            gl_fa_1_gb_1_checkbox_3 = new CheckBox();
            gl_fa_2_gb_1_checkbox_1 = new CheckBox();
            gl_fa_2_gb_1_checkbox_2 = new CheckBox();
            gl_fa_2_gb_1_checkbox_3 = new CheckBox();
            gl_fa_3_gb_1_checkbox_1 = new CheckBox();
            gl_fa_3_gb_1_checkbox_2 = new CheckBox();
            gl_fa_3_gb_1_checkbox_3 = new CheckBox();
            pl_fa_1_gb_1_checkbox_1 = new CheckBox();
            pl_fa_1_gb_1_checkbox_2 = new CheckBox();
            pl_fa_1_gb_1_checkbox_3 = new CheckBox();
            pl_fa_2_gb_1_checkbox_1 = new CheckBox();
            pl_fa_2_gb_1_checkbox_2 = new CheckBox();
            pl_fa_2_gb_1_checkbox_3 = new CheckBox();
            clRepresentativeFA = new CheckBox();
            glRepresentativeFA = new CheckBox();
            plRepresentativeFA = new CheckBox();
            pl_is_cl = new CheckBox();
            gl_contains_sugar = new CheckBox();

            cl_positive_adduct = new GroupBox();
            cl_negative_adduct = new GroupBox();
            gl_positive_adduct = new GroupBox();
            gl_negative_adduct = new GroupBox();
            pl_positive_adduct = new GroupBox();
            pl_negative_adduct = new GroupBox();
            sl_positive_adduct = new GroupBox();
            sl_negative_adduct = new GroupBox();

            cl_pos_adduct_checkbox_1 = new CheckBox();
            cl_pos_adduct_checkbox_2 = new CheckBox();
            cl_pos_adduct_checkbox_3 = new CheckBox();
            cl_neg_adduct_checkbox_1 = new CheckBox();
            cl_neg_adduct_checkbox_2 = new CheckBox();
            cl_neg_adduct_checkbox_3 = new CheckBox();
            cl_neg_adduct_checkbox_4 = new CheckBox();
            gl_pos_adduct_checkbox_1 = new CheckBox();
            gl_pos_adduct_checkbox_2 = new CheckBox();
            gl_pos_adduct_checkbox_3 = new CheckBox();
            gl_neg_adduct_checkbox_1 = new CheckBox();
            gl_neg_adduct_checkbox_2 = new CheckBox();
            gl_neg_adduct_checkbox_3 = new CheckBox();
            gl_neg_adduct_checkbox_4 = new CheckBox();
            pl_pos_adduct_checkbox_1 = new CheckBox();
            pl_pos_adduct_checkbox_2 = new CheckBox();
            pl_pos_adduct_checkbox_3 = new CheckBox();
            pl_neg_adduct_checkbox_1 = new CheckBox();
            pl_neg_adduct_checkbox_2 = new CheckBox();
            pl_neg_adduct_checkbox_3 = new CheckBox();
            pl_neg_adduct_checkbox_4 = new CheckBox();
            sl_pos_adduct_checkbox_1 = new CheckBox();
            sl_pos_adduct_checkbox_2 = new CheckBox();
            sl_pos_adduct_checkbox_3 = new CheckBox();
            sl_neg_adduct_checkbox_1 = new CheckBox();
            sl_neg_adduct_checkbox_2 = new CheckBox();
            sl_neg_adduct_checkbox_3 = new CheckBox();
            sl_neg_adduct_checkbox_4 = new CheckBox();

            
            
            sl_lcb_hydroxy_combobox = new ComboBox();
            sl_lcb_hydroxy_combobox.Items.Add("2");
            sl_lcb_hydroxy_combobox.Items.Add("3");
            
            sl_fa_hydroxy_combobox = new ComboBox();
            sl_fa_hydroxy_combobox.Items.Add("0");
            sl_fa_hydroxy_combobox.Items.Add("1");
            sl_fa_hydroxy_combobox.Items.Add("2");
            sl_fa_hydroxy_combobox.Items.Add("3");



            toolTip1 = new ToolTip();
            
            

            string formatting_fa = "Comma seperated single entries or intervals. Example formatting: 2, 3, 5-6, 13-20";
            string formatting_db = "Comma seperated single entries or intervals. Example formatting: 2, 3-4, 6";
            string formatting_hydroxyl = "Comma seperated single entries or intervals. Example formatting: 2-4, 10, 12";


            tab_control.Controls.Add(glycerolipids_tab);
            tab_control.Controls.Add(phospholipids_tab);
            tab_control.Controls.Add(sphingolipids_tab);
            tab_control.Dock = DockStyle.Fill;
            Font tab_fnt = new Font(tab_control.Font.FontFamily, 16);
            tab_control.Font = tab_fnt;
            tab_control.SelectedIndexChanged += new System.EventHandler(tabIndexChanged);


            // tab for cardiolipins

            phospholipids_tab.Controls.Add(cl_fa_1_gb_1_checkbox_3);
            phospholipids_tab.Controls.Add(cl_fa_1_gb_1_checkbox_2);
            phospholipids_tab.Controls.Add(cl_fa_1_gb_1_checkbox_1);
            phospholipids_tab.Controls.Add(cl_fa_2_gb_1_checkbox_3);
            phospholipids_tab.Controls.Add(cl_fa_2_gb_1_checkbox_2);
            phospholipids_tab.Controls.Add(cl_fa_2_gb_1_checkbox_1);
            phospholipids_tab.Controls.Add(cl_fa_3_gb_1_checkbox_3);
            phospholipids_tab.Controls.Add(cl_fa_3_gb_1_checkbox_2);
            phospholipids_tab.Controls.Add(cl_fa_3_gb_1_checkbox_1);
            phospholipids_tab.Controls.Add(cl_fa_4_gb_1_checkbox_3);
            phospholipids_tab.Controls.Add(cl_fa_4_gb_1_checkbox_2);
            phospholipids_tab.Controls.Add(cl_fa_4_gb_1_checkbox_1);
            phospholipids_tab.Controls.Add(cl_positive_adduct);
            phospholipids_tab.Controls.Add(cl_negative_adduct);
            phospholipids_tab.Controls.Add(cl_add_lipid_button);
            phospholipids_tab.Controls.Add(cl_reset_lipid_button);
            phospholipids_tab.Controls.Add(cl_modify_lipid_button);
            phospholipids_tab.Controls.Add(cl_ms2fragments_lipid_button);
            phospholipids_tab.Controls.Add(cl_picture_box);
            phospholipids_tab.Controls.Add(cl_fa_1_textbox);
            phospholipids_tab.Controls.Add(cl_fa_2_textbox);
            phospholipids_tab.Controls.Add(cl_fa_3_textbox);
            phospholipids_tab.Controls.Add(cl_fa_4_textbox);
            phospholipids_tab.Controls.Add(cl_db_1_textbox);
            phospholipids_tab.Controls.Add(cl_db_2_textbox);
            phospholipids_tab.Controls.Add(cl_db_3_textbox);
            phospholipids_tab.Controls.Add(cl_db_4_textbox);
            phospholipids_tab.Controls.Add(clRepresentativeFA);
            phospholipids_tab.Controls.Add(cl_hydroxyl_1_textbox);
            phospholipids_tab.Controls.Add(cl_hydroxyl_2_textbox);
            phospholipids_tab.Controls.Add(cl_hydroxyl_3_textbox);
            phospholipids_tab.Controls.Add(cl_hydroxyl_4_textbox);
            phospholipids_tab.Controls.Add(cl_fa_1_combobox);
            phospholipids_tab.Controls.Add(cl_fa_2_combobox);
            phospholipids_tab.Controls.Add(cl_fa_3_combobox);
            phospholipids_tab.Controls.Add(cl_fa_4_combobox);
            phospholipids_tab.Controls.Add(cl_db_1_label);
            phospholipids_tab.Controls.Add(cl_db_2_label);
            phospholipids_tab.Controls.Add(cl_db_3_label);
            phospholipids_tab.Controls.Add(cl_db_4_label);
            phospholipids_tab.Controls.Add(cl_hydroxyl_1_label);
            phospholipids_tab.Controls.Add(cl_hydroxyl_2_label);
            phospholipids_tab.Controls.Add(cl_hydroxyl_3_label);
            phospholipids_tab.Controls.Add(cl_hydroxyl_4_label);
            Font pl_fnt = new Font(phospholipids_tab.Font.FontFamily, 8.25F);
            phospholipids_tab.Font = pl_fnt;
            
            
            
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
            cl_fa_1_combobox.Visible = false;
            cl_fa_2_combobox.Visible = false;
            cl_fa_3_combobox.Visible = false;
            cl_fa_4_combobox.Visible = false;
            cl_db_1_label.Visible = false;
            cl_db_2_label.Visible = false;
            cl_db_3_label.Visible = false;
            cl_db_4_label.Visible = false;
            cl_hydroxyl_1_label.Visible = false;
            cl_hydroxyl_2_label.Visible = false;
            cl_hydroxyl_3_label.Visible = false;
            cl_hydroxyl_4_label.Visible = false;
            clRepresentativeFA.Visible = false;
            

            cl_picture_box.Image = cardio_backbone_image;
            cl_picture_box.Location = new Point(5, 5);
            cl_picture_box.SizeMode = PictureBoxSizeMode.AutoSize;
            cl_picture_box.SendToBack();
            


            cl_fa_1_combobox.BringToFront();
            cl_fa_1_textbox.BringToFront();
            cl_fa_1_textbox.Location = new Point(400, 64);
            cl_fa_1_textbox.Width = fa_length;
            cl_fa_1_textbox.TextChanged += new EventHandler(cl_fa_1_textbox_valueChanged);
            toolTip1.SetToolTip(cl_fa_1_textbox, formatting_fa);
            cl_fa_1_combobox.Location = new Point(cl_fa_1_textbox.Left, cl_fa_1_textbox.Top - sepText);
            cl_fa_1_combobox.Width = fa_length;
            cl_fa_1_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            cl_fa_1_combobox.SelectedIndexChanged += new EventHandler(cl_fa_1_combobox_valueChanged);
            cl_db_1_textbox.Location = new Point(cl_fa_1_textbox.Left + cl_fa_1_textbox.Width + sep, cl_fa_1_textbox.Top);
            cl_db_1_textbox.Width = db_length;
            cl_db_1_textbox.TextChanged += new EventHandler(cl_db_1_textbox_valueChanged);
            toolTip1.SetToolTip(cl_db_1_textbox, formatting_db);
            cl_db_1_label.Width = db_length;
            cl_db_1_label.Location = new Point(cl_db_1_textbox.Left, cl_db_1_textbox.Top - sep);
            cl_db_1_label.Text = db_text;
            cl_hydroxyl_1_textbox.Width = db_length;
            cl_hydroxyl_1_textbox.Location = new Point(cl_db_1_textbox.Left + cl_db_1_textbox.Width + sep, cl_db_1_textbox.Top);
            cl_hydroxyl_1_textbox.TextChanged += new EventHandler(cl_hydroxyl_1_textbox_valueChanged);
            toolTip1.SetToolTip(cl_hydroxyl_1_textbox, formatting_hydroxyl);
            cl_hydroxyl_1_label.Width = db_length;
            cl_hydroxyl_1_label.Location = new Point(cl_hydroxyl_1_textbox.Left, cl_hydroxyl_1_textbox.Top - sep);
            cl_hydroxyl_1_label.Text = hydroxyl_text;
            

            cl_fa_1_gb_1_checkbox_3.Location = new Point(cl_fa_1_textbox.Left + 90, cl_fa_1_textbox.Top + cl_fa_1_textbox.Height);
            cl_fa_1_gb_1_checkbox_3.Text = "FAe";
            cl_fa_1_gb_1_checkbox_3.CheckedChanged += new EventHandler(cl_fa_1_gb_1_checkbox_3_checkedChanged);
            cl_fa_1_gb_1_checkbox_3.MouseLeave += new System.EventHandler(cl_fa_1_gb_1_checkbox_3_MouseLeave);
            cl_fa_1_gb_1_checkbox_3.MouseMove += new System.Windows.Forms.MouseEventHandler(cl_fa_1_gb_1_checkbox_3_MouseHover);
            cl_fa_1_gb_1_checkbox_2.Location = new Point(cl_fa_1_textbox.Left + 40, cl_fa_1_textbox.Top + cl_fa_1_textbox.Height);
            cl_fa_1_gb_1_checkbox_2.Text = "FAp";
            cl_fa_1_gb_1_checkbox_2.CheckedChanged += new EventHandler(cl_fa_1_gb_1_checkbox_2_checkedChanged);
            cl_fa_1_gb_1_checkbox_2.MouseLeave += new System.EventHandler(cl_fa_1_gb_1_checkbox_2_MouseLeave);
            cl_fa_1_gb_1_checkbox_2.MouseMove += new System.Windows.Forms.MouseEventHandler(cl_fa_1_gb_1_checkbox_2_MouseHover);
            cl_fa_1_gb_1_checkbox_1.Location = new Point(cl_fa_1_textbox.Left, cl_fa_1_textbox.Top + cl_fa_1_textbox.Height);
            cl_fa_1_gb_1_checkbox_1.Text = "FA";
            cl_fa_1_gb_1_checkbox_1.Checked = true;
            cl_fa_1_gb_1_checkbox_1.CheckedChanged += new EventHandler(cl_fa_1_gb_1_checkbox_1_checkedChanged);

            cl_positive_adduct.Location = new Point(left_groupboxes, 60);
            cl_positive_adduct.Width = 120;
            cl_positive_adduct.Height = 120;
            cl_positive_adduct.Text = "Positive adducts";
            cl_pos_adduct_checkbox_1.Parent = cl_positive_adduct;
            cl_pos_adduct_checkbox_1.Location = new Point(10, 15);
            cl_pos_adduct_checkbox_1.Text = "+H⁺";
            cl_pos_adduct_checkbox_1.Checked = true;
            cl_pos_adduct_checkbox_1.CheckedChanged += new EventHandler(cl_pos_adduct_checkbox_1_checkedChanged);
            cl_pos_adduct_checkbox_2.Parent = cl_positive_adduct;
            cl_pos_adduct_checkbox_2.Location = new Point(10, 35);
            cl_pos_adduct_checkbox_2.Text = "+2H⁺⁺";
            cl_pos_adduct_checkbox_2.CheckedChanged += new EventHandler(cl_pos_adduct_checkbox_2_checkedChanged);
            cl_pos_adduct_checkbox_3.Parent = cl_positive_adduct;
            cl_pos_adduct_checkbox_3.Location = new Point(10, 55);
            cl_pos_adduct_checkbox_3.Text = "+NH4⁺";
            cl_pos_adduct_checkbox_3.Enabled = false;
            cl_pos_adduct_checkbox_3.CheckedChanged += new EventHandler(cl_pos_adduct_checkbox_3_checkedChanged);
            cl_negative_adduct.Location = new Point(left_groupboxes, 200);
            cl_negative_adduct.Width = 120;
            cl_negative_adduct.Height = 120;
            cl_negative_adduct.Text = "Negative adducts";
            cl_neg_adduct_checkbox_1.Parent = cl_negative_adduct;
            cl_neg_adduct_checkbox_1.Location = new Point(10, 15);
            cl_neg_adduct_checkbox_1.Text = "-H⁻";
            cl_neg_adduct_checkbox_1.CheckedChanged += new EventHandler(cl_neg_adduct_checkbox_1_checkedChanged);
            cl_neg_adduct_checkbox_2.Parent = cl_negative_adduct;
            cl_neg_adduct_checkbox_2.Location = new Point(10, 35);
            cl_neg_adduct_checkbox_2.Text = "-2H⁻ ⁻";
            cl_neg_adduct_checkbox_2.CheckedChanged += new EventHandler(cl_neg_adduct_checkbox_2_checkedChanged);
            cl_neg_adduct_checkbox_3.Parent = cl_negative_adduct;
            cl_neg_adduct_checkbox_3.Location = new Point(10, 55);
            cl_neg_adduct_checkbox_3.Text = "+HCOO⁻";
            cl_neg_adduct_checkbox_3.Enabled = false;
            cl_neg_adduct_checkbox_3.CheckedChanged += new EventHandler(cl_neg_adduct_checkbox_3_checkedChanged);
            cl_neg_adduct_checkbox_4.Parent = cl_negative_adduct;
            cl_neg_adduct_checkbox_4.Location = new Point(10, 75);
            cl_neg_adduct_checkbox_4.Text = "+CH3COO⁻";
            cl_neg_adduct_checkbox_4.Enabled = false;
            cl_neg_adduct_checkbox_4.CheckedChanged += new EventHandler(cl_neg_adduct_checkbox_4_checkedChanged);



            cl_fa_2_combobox.BringToFront();
            cl_fa_2_textbox.BringToFront();
            cl_fa_2_textbox.Location = new Point(312, 144);
            cl_fa_2_textbox.Width = fa_length;
            cl_fa_2_textbox.TextChanged += new EventHandler(cl_fa_2_textbox_valueChanged);
            toolTip1.SetToolTip(cl_fa_2_textbox, formatting_fa);
            cl_fa_2_combobox.Location = new Point(cl_fa_2_textbox.Left, cl_fa_2_textbox.Top - sepText);
            cl_fa_2_combobox.Width = fa_length;
            cl_fa_2_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            cl_fa_2_combobox.SelectedIndexChanged += new EventHandler(cl_fa_2_combobox_valueChanged);
            cl_db_2_textbox.Location = new Point(cl_fa_2_textbox.Left + cl_fa_2_textbox.Width + sep, cl_fa_2_textbox.Top);
            cl_db_2_textbox.Width = db_length;
            cl_db_2_textbox.TextChanged += new EventHandler(cl_db_2_textbox_valueChanged);
            toolTip1.SetToolTip(cl_db_2_textbox, formatting_db);
            cl_db_2_label.Location = new Point(cl_db_2_textbox.Left, cl_db_2_textbox.Top - sep);
            cl_db_2_label.Width = db_length;
            cl_db_2_label.Text = db_text;
            cl_hydroxyl_2_textbox.Width = db_length;
            cl_hydroxyl_2_textbox.Location = new Point(cl_db_2_textbox.Left + cl_db_2_textbox.Width + sep, cl_db_2_textbox.Top);
            cl_hydroxyl_2_textbox.TextChanged += new EventHandler(cl_hydroxyl_2_textbox_valueChanged);
            toolTip1.SetToolTip(cl_hydroxyl_2_textbox, formatting_hydroxyl);
            cl_hydroxyl_2_label.Width = db_length;
            cl_hydroxyl_2_label.Location = new Point(cl_hydroxyl_2_textbox.Left, cl_hydroxyl_2_textbox.Top - sep);
            cl_hydroxyl_2_label.Text = hydroxyl_text;

            cl_fa_2_gb_1_checkbox_3.Location = new Point(cl_fa_2_textbox.Left + 90, cl_fa_2_textbox.Top + cl_fa_2_textbox.Height);
            cl_fa_2_gb_1_checkbox_3.Text = "FAe";
            cl_fa_2_gb_1_checkbox_3.CheckedChanged += new EventHandler(cl_fa_2_gb_1_checkbox_3_checkedChanged);
            cl_fa_2_gb_1_checkbox_3.MouseLeave += new System.EventHandler(cl_fa_2_gb_1_checkbox_3_MouseLeave);
            cl_fa_2_gb_1_checkbox_3.MouseMove += new System.Windows.Forms.MouseEventHandler(cl_fa_2_gb_1_checkbox_3_MouseHover);
            cl_fa_2_gb_1_checkbox_2.Location = new Point(cl_fa_2_textbox.Left + 40, cl_fa_2_textbox.Top + cl_fa_2_textbox.Height);
            cl_fa_2_gb_1_checkbox_2.Text = "FAp";
            cl_fa_2_gb_1_checkbox_2.CheckedChanged += new EventHandler(cl_fa_2_gb_1_checkbox_2_checkedChanged);
            cl_fa_2_gb_1_checkbox_2.MouseLeave += new System.EventHandler(cl_fa_2_gb_1_checkbox_2_MouseLeave);
            cl_fa_2_gb_1_checkbox_2.MouseMove += new System.Windows.Forms.MouseEventHandler(cl_fa_2_gb_1_checkbox_2_MouseHover);
            cl_fa_2_gb_1_checkbox_1.Location = new Point(cl_fa_2_textbox.Left, cl_fa_2_textbox.Top + cl_fa_2_textbox.Height);
            cl_fa_2_gb_1_checkbox_1.Text = "FA";
            cl_fa_2_gb_1_checkbox_1.Checked = true;
            cl_fa_2_gb_1_checkbox_1.CheckedChanged += new EventHandler(cl_fa_2_gb_1_checkbox_1_checkedChanged);






            cl_fa_3_combobox.BringToFront();
            cl_fa_3_textbox.BringToFront();
            cl_fa_3_textbox.Location = new Point(436, 260);
            cl_fa_3_textbox.Width = fa_length;
            cl_fa_3_textbox.TextChanged += new EventHandler(cl_fa_3_textbox_valueChanged);
            toolTip1.SetToolTip(cl_fa_3_textbox, formatting_fa);
            cl_fa_3_combobox.Location = new Point(cl_fa_3_textbox.Left, cl_fa_3_textbox.Top - sepText);
            cl_fa_3_combobox.Width = fa_length;
            cl_fa_3_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            cl_fa_3_combobox.SelectedIndexChanged += new EventHandler(cl_fa_3_combobox_valueChanged);
            cl_db_3_textbox.Location = new Point(cl_fa_3_textbox.Left + cl_fa_3_textbox.Width + sep, cl_fa_3_textbox.Top);
            cl_db_3_textbox.Width = db_length;
            cl_db_3_textbox.TextChanged += new EventHandler(cl_db_3_textbox_valueChanged);
            toolTip1.SetToolTip(cl_db_3_textbox, formatting_db);
            cl_db_3_label.Location = new Point(cl_db_3_textbox.Left, cl_db_3_textbox.Top - sep);
            cl_db_3_label.Width = db_length;
            cl_db_3_label.Text = db_text;
            cl_hydroxyl_3_textbox.Width = db_length;
            cl_hydroxyl_3_textbox.Location = new Point(cl_db_3_textbox.Left + cl_db_3_textbox.Width + sep, cl_db_3_textbox.Top);
            cl_hydroxyl_3_textbox.TextChanged += new EventHandler(cl_hydroxyl_3_textbox_valueChanged);
            toolTip1.SetToolTip(cl_hydroxyl_3_textbox, formatting_hydroxyl);
            cl_hydroxyl_3_label.Width = db_length;
            cl_hydroxyl_3_label.Location = new Point(cl_hydroxyl_3_textbox.Left, cl_hydroxyl_3_textbox.Top - sep);
            cl_hydroxyl_3_label.Text = hydroxyl_text;

            cl_fa_3_gb_1_checkbox_3.Location = new Point(cl_fa_3_textbox.Left + 90, cl_fa_3_textbox.Top + cl_fa_3_textbox.Height);
            cl_fa_3_gb_1_checkbox_3.Text = "FAe";
            cl_fa_3_gb_1_checkbox_3.CheckedChanged += new EventHandler(cl_fa_3_gb_1_checkbox_3_checkedChanged);
            cl_fa_3_gb_1_checkbox_3.MouseLeave += new System.EventHandler(cl_fa_3_gb_1_checkbox_3_MouseLeave);
            cl_fa_3_gb_1_checkbox_3.MouseMove += new System.Windows.Forms.MouseEventHandler(cl_fa_3_gb_1_checkbox_3_MouseHover);
            cl_fa_3_gb_1_checkbox_2.Location = new Point(cl_fa_3_textbox.Left + 40, cl_fa_3_textbox.Top + cl_fa_3_textbox.Height);
            cl_fa_3_gb_1_checkbox_2.Text = "FAp";
            cl_fa_3_gb_1_checkbox_2.CheckedChanged += new EventHandler(cl_fa_3_gb_1_checkbox_2_checkedChanged);
            cl_fa_3_gb_1_checkbox_2.MouseLeave += new System.EventHandler(cl_fa_3_gb_1_checkbox_2_MouseLeave);
            cl_fa_3_gb_1_checkbox_2.MouseMove += new System.Windows.Forms.MouseEventHandler(cl_fa_3_gb_1_checkbox_2_MouseHover);
            cl_fa_3_gb_1_checkbox_1.Location = new Point(cl_fa_3_textbox.Left, cl_fa_3_textbox.Top + cl_fa_3_textbox.Height);
            cl_fa_3_gb_1_checkbox_1.Text = "FA";
            cl_fa_3_gb_1_checkbox_1.Checked = true;
            cl_fa_3_gb_1_checkbox_1.CheckedChanged += new EventHandler(cl_fa_3_gb_1_checkbox_1_checkedChanged);


            
            clRepresentativeFA.Location = new Point(cl_hydroxyl_1_textbox.Left + cl_hydroxyl_1_textbox.Width + sep, cl_hydroxyl_1_textbox.Top);
            clRepresentativeFA.Width = 120;
            clRepresentativeFA.Text = "First FA representative";
            clRepresentativeFA.CheckedChanged += new EventHandler(clRepresentativeFA_checkedChanged);
            clRepresentativeFA.SendToBack();



            cl_fa_4_combobox.BringToFront();
            cl_fa_4_textbox.BringToFront();
            cl_fa_4_textbox.Location = new Point(350, 344);
            cl_fa_4_textbox.Width = fa_length;
            cl_fa_4_textbox.TextChanged += new EventHandler(cl_fa_4_textbox_valueChanged);
            toolTip1.SetToolTip(cl_fa_4_textbox, formatting_fa);
            cl_fa_4_combobox.Location = new Point(cl_fa_4_textbox.Left, cl_fa_4_textbox.Top - sepText);
            cl_fa_4_combobox.Width = fa_length;
            cl_fa_4_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            cl_fa_4_combobox.SelectedIndexChanged += new EventHandler(cl_fa_4_combobox_valueChanged);
            cl_db_4_textbox.Location = new Point(cl_fa_4_textbox.Left + cl_fa_4_textbox.Width + sep, cl_fa_4_textbox.Top);
            cl_db_4_textbox.Width = db_length;
            cl_db_4_textbox.TextChanged += new EventHandler(cl_db_4_textbox_valueChanged);
            toolTip1.SetToolTip(cl_db_4_textbox, formatting_db);
            cl_db_4_label.Location = new Point(cl_db_4_textbox.Left, cl_db_4_textbox.Top - sep);
            cl_db_4_label.Width = db_length;
            cl_db_4_label.Text = db_text;
            cl_hydroxyl_4_textbox.Width = db_length;
            cl_hydroxyl_4_textbox.Location = new Point(cl_db_4_textbox.Left + cl_db_4_textbox.Width + sep, cl_db_4_textbox.Top);
            cl_hydroxyl_4_textbox.TextChanged += new EventHandler(cl_hydroxyl_4_textbox_valueChanged);
            toolTip1.SetToolTip(cl_hydroxyl_4_textbox, formatting_hydroxyl);
            cl_hydroxyl_4_label.Width = db_length;
            cl_hydroxyl_4_label.Location = new Point(cl_hydroxyl_4_textbox.Left, cl_hydroxyl_4_textbox.Top - sep);
            cl_hydroxyl_4_label.Text = hydroxyl_text;

            cl_fa_4_gb_1_checkbox_3.Location = new Point(cl_fa_4_textbox.Left + 90, cl_fa_4_textbox.Top + cl_fa_4_textbox.Height);
            cl_fa_4_gb_1_checkbox_3.Text = "FAe";
            cl_fa_4_gb_1_checkbox_3.CheckedChanged += new EventHandler(cl_fa_4_gb_1_checkbox_3_checkedChanged);
            cl_fa_4_gb_1_checkbox_3.MouseLeave += new System.EventHandler(cl_fa_4_gb_1_checkbox_3_MouseLeave);
            cl_fa_4_gb_1_checkbox_3.MouseMove += new System.Windows.Forms.MouseEventHandler(cl_fa_4_gb_1_checkbox_3_MouseHover);
            cl_fa_4_gb_1_checkbox_2.Location = new Point(cl_fa_4_textbox.Left + 40, cl_fa_4_textbox.Top + cl_fa_4_textbox.Height);
            cl_fa_4_gb_1_checkbox_2.Text = "FAp";
            cl_fa_4_gb_1_checkbox_2.CheckedChanged += new EventHandler(cl_fa_4_gb_1_checkbox_2_checkedChanged);
            cl_fa_4_gb_1_checkbox_2.MouseLeave += new System.EventHandler(cl_fa_4_gb_1_checkbox_2_MouseLeave);
            cl_fa_4_gb_1_checkbox_2.MouseMove += new System.Windows.Forms.MouseEventHandler(cl_fa_4_gb_1_checkbox_2_MouseHover);
            cl_fa_4_gb_1_checkbox_1.Location = new Point(cl_fa_4_textbox.Left, cl_fa_4_textbox.Top + cl_fa_4_textbox.Height);
            cl_fa_4_gb_1_checkbox_1.Text = "FA";
            cl_fa_4_gb_1_checkbox_1.Checked = true;
            cl_fa_4_gb_1_checkbox_1.CheckedChanged += new EventHandler(cl_fa_4_gb_1_checkbox_1_checkedChanged);



            // tab for glycerolipids
            glycerolipids_tab.Controls.Add(gl_add_lipid_button);
            glycerolipids_tab.Controls.Add(gl_reset_lipid_button);
            glycerolipids_tab.Controls.Add(gl_modify_lipid_button);
            glycerolipids_tab.Controls.Add(gl_ms2fragments_lipid_button);
            glycerolipids_tab.Controls.Add(gl_fa_1_gb_1_checkbox_3);
            glycerolipids_tab.Controls.Add(gl_fa_1_gb_1_checkbox_2);
            glycerolipids_tab.Controls.Add(gl_fa_1_gb_1_checkbox_1);
            glycerolipids_tab.Controls.Add(gl_fa_2_gb_1_checkbox_3);
            glycerolipids_tab.Controls.Add(gl_fa_2_gb_1_checkbox_2);
            glycerolipids_tab.Controls.Add(gl_fa_2_gb_1_checkbox_1);
            glycerolipids_tab.Controls.Add(gl_fa_3_gb_1_checkbox_3);
            glycerolipids_tab.Controls.Add(gl_fa_3_gb_1_checkbox_2);
            glycerolipids_tab.Controls.Add(gl_fa_3_gb_1_checkbox_1);
            glycerolipids_tab.Controls.Add(gl_picture_box);
            glycerolipids_tab.Controls.Add(gl_fa_1_textbox);
            glycerolipids_tab.Controls.Add(gl_fa_2_textbox);
            glycerolipids_tab.Controls.Add(gl_fa_3_textbox);
            glycerolipids_tab.Controls.Add(gl_db_1_textbox);
            glycerolipids_tab.Controls.Add(gl_db_2_textbox);
            glycerolipids_tab.Controls.Add(gl_db_3_textbox);
            glycerolipids_tab.Controls.Add(gl_hydroxyl_1_textbox);
            glycerolipids_tab.Controls.Add(gl_hydroxyl_2_textbox);
            glycerolipids_tab.Controls.Add(gl_hydroxyl_3_textbox);
            glycerolipids_tab.Controls.Add(gl_fa_1_combobox);
            glycerolipids_tab.Controls.Add(gl_fa_2_combobox);
            glycerolipids_tab.Controls.Add(gl_fa_3_combobox);
            glycerolipids_tab.Controls.Add(gl_hg_listbox);
            glycerolipids_tab.Controls.Add(gl_hg_label);
            glycerolipids_tab.Controls.Add(gl_contains_sugar);
            glycerolipids_tab.Controls.Add(gl_db_1_label);
            glycerolipids_tab.Controls.Add(gl_db_2_label);
            glycerolipids_tab.Controls.Add(gl_db_3_label);
            glycerolipids_tab.Controls.Add(gl_hydroxyl_1_label);
            glycerolipids_tab.Controls.Add(gl_hydroxyl_2_label);
            glycerolipids_tab.Controls.Add(gl_hydroxyl_3_label);
            glycerolipids_tab.Controls.Add(glRepresentativeFA);
            glycerolipids_tab.Controls.Add(gl_positive_adduct);
            glycerolipids_tab.Controls.Add(gl_negative_adduct);
            glycerolipids_tab.Controls.Add(eastertext);
            glycerolipids_tab.Parent = tab_control;
            glycerolipids_tab.Text = "Glycerolipids";
            glycerolipids_tab.Location = new Point(0, 0);
            glycerolipids_tab.Size = this.Size;
            glycerolipids_tab.AutoSize = true;
            glycerolipids_tab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            glycerolipids_tab.BackColor = Color.White;
            Font gl_fnt = new Font(glycerolipids_tab.Font.FontFamily, 8.25F);
            glycerolipids_tab.Font = gl_fnt;
            

            
            
            eastertext.Location = new Point(1030, 5);
            eastertext.Text = "Fat is unfair, it sticks 2 minutes in your mouth, 2 hours in your stomach and 2 decades at your hips.";
            eastertext.Visible = false;
            eastertext.Font = new Font(new Font(eastertext.Font.FontFamily, 40), FontStyle.Bold);
            eastertext.AutoSize = true;
            

            gl_fa_1_combobox.BringToFront();
            gl_fa_1_textbox.BringToFront();
            gl_fa_1_textbox.Location = new Point(196, 130);
            gl_fa_1_textbox.Width = fa_length;
            gl_fa_1_textbox.Text = "0, 2, 4, 6-7";
            gl_fa_1_textbox.TextChanged += new EventHandler(gl_fa_1_textbox_valueChanged);
            toolTip1.SetToolTip(gl_fa_1_textbox, formatting_fa);
            gl_fa_1_combobox.Location = new Point(gl_fa_1_textbox.Left, gl_fa_1_textbox.Top - sepText);
            gl_fa_1_combobox.Width = fa_length;
            gl_fa_1_combobox.SelectedItem = "Fatty acid chain";
            gl_fa_1_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            gl_fa_1_combobox.SelectedIndexChanged += new EventHandler(gl_fa_1_combobox_valueChanged);
            gl_db_1_textbox.Location = new Point(gl_fa_1_textbox.Left + gl_fa_1_textbox.Width + sep, gl_fa_1_textbox.Top);
            gl_db_1_textbox.Width = db_length;
            gl_db_1_textbox.Text = "0-2";
            gl_db_1_textbox.TextChanged += new EventHandler(gl_db_1_textbox_valueChanged);
            toolTip1.SetToolTip(gl_db_1_textbox, formatting_db);
            gl_db_1_label.Location = new Point(gl_db_1_textbox.Left, gl_db_1_textbox.Top - sep);
            gl_db_1_label.Width = db_length;
            gl_db_1_label.Text = db_text;
            gl_hydroxyl_1_textbox.Width = db_length;
            gl_hydroxyl_1_textbox.Location = new Point(gl_db_1_textbox.Left + gl_db_1_textbox.Width + sep, gl_db_1_textbox.Top);
            gl_hydroxyl_1_textbox.TextChanged += new EventHandler(gl_hydroxyl_1_textbox_valueChanged);
            toolTip1.SetToolTip(gl_hydroxyl_1_textbox, formatting_hydroxyl);
            gl_hydroxyl_1_label.Width = db_length;
            gl_hydroxyl_1_label.Location = new Point(gl_hydroxyl_1_textbox.Left, gl_hydroxyl_1_textbox.Top - sep);
            gl_hydroxyl_1_label.Text = hydroxyl_text;

            gl_fa_1_gb_1_checkbox_3.Location = new Point(gl_fa_1_textbox.Left + 90, gl_fa_1_textbox.Top + gl_fa_1_textbox.Height);
            gl_fa_1_gb_1_checkbox_3.Text = "FAe";
            gl_fa_1_gb_1_checkbox_3.CheckedChanged += new EventHandler(gl_fa_1_gb_1_checkbox_3_checkedChanged);
            gl_fa_1_gb_1_checkbox_3.MouseLeave += new System.EventHandler(gl_fa_1_gb_1_checkbox_3_MouseLeave);
            gl_fa_1_gb_1_checkbox_3.MouseMove += new System.Windows.Forms.MouseEventHandler(gl_fa_1_gb_1_checkbox_3_MouseHover);
            gl_fa_1_gb_1_checkbox_2.Location = new Point(gl_fa_1_textbox.Left + 40, gl_fa_1_textbox.Top + gl_fa_1_textbox.Height);
            gl_fa_1_gb_1_checkbox_2.Text = "FAp";
            gl_fa_1_gb_1_checkbox_2.CheckedChanged += new EventHandler(gl_fa_1_gb_1_checkbox_2_checkedChanged);
            gl_fa_1_gb_1_checkbox_2.MouseLeave += new System.EventHandler(gl_fa_1_gb_1_checkbox_2_MouseLeave);
            gl_fa_1_gb_1_checkbox_2.MouseMove += new System.Windows.Forms.MouseEventHandler(gl_fa_1_gb_1_checkbox_2_MouseHover);
            gl_fa_1_gb_1_checkbox_1.Location = new Point(gl_fa_1_textbox.Left, gl_fa_1_textbox.Top + gl_fa_1_textbox.Height);
            gl_fa_1_gb_1_checkbox_1.Text = "FA";
            gl_fa_1_gb_1_checkbox_1.Checked = true;
            gl_fa_1_gb_1_checkbox_1.CheckedChanged += new EventHandler(gl_fa_1_gb_1_checkbox_1_checkedChanged);

            gl_fa_2_combobox.BringToFront();
            gl_fa_2_textbox.BringToFront();
            gl_fa_2_textbox.Location = new Point(290, 202);
            gl_fa_2_textbox.Width = fa_length;
            gl_fa_2_textbox.Text = "0, 5, 17-19";
            gl_fa_2_textbox.TextChanged += new EventHandler(gl_fa_2_textbox_valueChanged);
            toolTip1.SetToolTip(gl_fa_2_textbox, formatting_fa);
            gl_fa_2_combobox.Location = new Point(gl_fa_2_textbox.Left, gl_fa_2_textbox.Top - sepText);
            gl_fa_2_combobox.Width = fa_length;
            gl_fa_2_combobox.SelectedItem = "Fatty acid chain";
            gl_fa_2_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            gl_fa_2_combobox.SelectedIndexChanged += new EventHandler(gl_fa_2_combobox_valueChanged);
            gl_db_2_textbox.Location = new Point(gl_fa_2_textbox.Left + gl_fa_2_textbox.Width + sep, gl_fa_2_textbox.Top);
            gl_db_2_textbox.Width = db_length;
            gl_db_2_textbox.Text = "5-6";
            gl_db_2_textbox.TextChanged += new EventHandler(gl_db_2_textbox_valueChanged);
            toolTip1.SetToolTip(gl_db_2_textbox, formatting_db);
            gl_db_2_label.Location = new Point(gl_db_2_textbox.Left, gl_db_2_textbox.Top - sep);
            gl_db_2_label.Width = db_length;
            gl_db_2_label.Text = db_text;
            gl_hydroxyl_2_textbox.Width = db_length;
            gl_hydroxyl_2_textbox.Location = new Point(gl_db_2_textbox.Left + gl_db_2_textbox.Width + sep, gl_db_2_textbox.Top);
            gl_hydroxyl_2_textbox.TextChanged += new EventHandler(gl_hydroxyl_2_textbox_valueChanged);
            toolTip1.SetToolTip(gl_hydroxyl_2_textbox, formatting_hydroxyl);
            gl_hydroxyl_2_label.Width = db_length;
            gl_hydroxyl_2_label.Location = new Point(gl_hydroxyl_2_textbox.Left, gl_hydroxyl_2_textbox.Top - sep);
            gl_hydroxyl_2_label.Text = hydroxyl_text;

            gl_fa_2_gb_1_checkbox_3.Location = new Point(gl_fa_2_textbox.Left + 90, gl_fa_2_textbox.Top + gl_fa_2_textbox.Height);
            gl_fa_2_gb_1_checkbox_3.Text = "FAe";
            gl_fa_2_gb_1_checkbox_3.CheckedChanged += new EventHandler(gl_fa_2_gb_1_checkbox_3_checkedChanged);
            gl_fa_2_gb_1_checkbox_3.MouseLeave += new System.EventHandler(gl_fa_2_gb_1_checkbox_3_MouseLeave);
            gl_fa_2_gb_1_checkbox_3.MouseMove += new System.Windows.Forms.MouseEventHandler(gl_fa_2_gb_1_checkbox_3_MouseHover);
            gl_fa_2_gb_1_checkbox_2.Location = new Point(gl_fa_2_textbox.Left + 40, gl_fa_2_textbox.Top + gl_fa_2_textbox.Height);
            gl_fa_2_gb_1_checkbox_2.Text = "FAp";
            gl_fa_2_gb_1_checkbox_2.CheckedChanged += new EventHandler(gl_fa_2_gb_1_checkbox_2_checkedChanged);
            gl_fa_2_gb_1_checkbox_2.MouseLeave += new System.EventHandler(gl_fa_2_gb_1_checkbox_2_MouseLeave);
            gl_fa_2_gb_1_checkbox_2.MouseMove += new System.Windows.Forms.MouseEventHandler(gl_fa_2_gb_1_checkbox_2_MouseHover);
            gl_fa_2_gb_1_checkbox_1.Location = new Point(gl_fa_2_textbox.Left, gl_fa_2_textbox.Top + gl_fa_2_textbox.Height);
            gl_fa_2_gb_1_checkbox_1.Text = "FA";
            gl_fa_2_gb_1_checkbox_1.Checked = true;
            gl_fa_2_gb_1_checkbox_1.CheckedChanged += new EventHandler(gl_fa_2_gb_1_checkbox_1_checkedChanged);

            gl_fa_3_combobox.BringToFront();
            gl_fa_3_textbox.BringToFront();
            gl_fa_3_textbox.Location = new Point(158, 302);
            gl_fa_3_textbox.Width = fa_length;
            gl_fa_3_textbox.Text = "20-22";
            gl_fa_3_textbox.TextChanged += new EventHandler(gl_fa_3_textbox_valueChanged);
            toolTip1.SetToolTip(gl_fa_3_textbox, formatting_fa);
            gl_fa_3_combobox.Location = new Point(gl_fa_3_textbox.Left, gl_fa_3_textbox.Top - sepText);
            gl_fa_3_combobox.Width = fa_length;
            gl_fa_3_combobox.SelectedItem = "Fatty acid chain";
            gl_fa_3_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            gl_fa_3_combobox.SelectedIndexChanged += new EventHandler(gl_fa_3_combobox_valueChanged);
            gl_db_3_textbox.Location = new Point(gl_fa_3_textbox.Left + gl_fa_3_textbox.Width + sep, gl_fa_3_textbox.Top);
            gl_db_3_textbox.Width = db_length;
            gl_db_3_textbox.Text = "0";
            gl_db_3_textbox.TextChanged += new EventHandler(gl_db_3_textbox_valueChanged);
            toolTip1.SetToolTip(gl_db_3_textbox, formatting_db);
            gl_db_3_label.Location = new Point(gl_db_3_textbox.Left, gl_db_3_textbox.Top - sep);
            gl_db_3_label.Width = db_length;
            gl_db_3_label.Text = db_text;
            gl_hydroxyl_3_textbox.Width = db_length;
            gl_hydroxyl_3_textbox.Location = new Point(gl_db_3_textbox.Left + gl_db_3_textbox.Width + sep, gl_db_3_textbox.Top);
            gl_hydroxyl_3_textbox.TextChanged += new EventHandler(gl_hydroxyl_3_textbox_valueChanged);
            toolTip1.SetToolTip(gl_hydroxyl_3_textbox, formatting_hydroxyl);
            gl_hydroxyl_3_label.Width = db_length;
            gl_hydroxyl_3_label.Location = new Point(gl_hydroxyl_3_textbox.Left, gl_hydroxyl_3_textbox.Top - sep);
            gl_hydroxyl_3_label.Text = hydroxyl_text;

            gl_fa_3_gb_1_checkbox_3.Location = new Point(gl_fa_3_textbox.Left + 90, gl_fa_3_textbox.Top + gl_fa_3_textbox.Height);
            gl_fa_3_gb_1_checkbox_3.Text = "FAe";
            gl_fa_3_gb_1_checkbox_3.CheckedChanged += new EventHandler(gl_fa_3_gb_1_checkbox_3_checkedChanged);
            gl_fa_3_gb_1_checkbox_3.MouseLeave += new System.EventHandler(gl_fa_3_gb_1_checkbox_3_MouseLeave);
            gl_fa_3_gb_1_checkbox_3.MouseMove += new System.Windows.Forms.MouseEventHandler(gl_fa_3_gb_1_checkbox_3_MouseHover);
            gl_fa_3_gb_1_checkbox_2.Location = new Point(gl_fa_3_textbox.Left + 40, gl_fa_3_textbox.Top + gl_fa_3_textbox.Height);
            gl_fa_3_gb_1_checkbox_2.Text = "FAp";
            gl_fa_3_gb_1_checkbox_2.CheckedChanged += new EventHandler(gl_fa_3_gb_1_checkbox_2_checkedChanged);
            gl_fa_3_gb_1_checkbox_2.MouseLeave += new System.EventHandler(gl_fa_3_gb_1_checkbox_2_MouseLeave);
            gl_fa_3_gb_1_checkbox_2.MouseMove += new System.Windows.Forms.MouseEventHandler(gl_fa_3_gb_1_checkbox_2_MouseHover);
            gl_fa_3_gb_1_checkbox_1.Location = new Point(gl_fa_3_textbox.Left, gl_fa_3_textbox.Top + gl_fa_3_textbox.Height);
            gl_fa_3_gb_1_checkbox_1.Text = "FA";
            gl_fa_3_gb_1_checkbox_1.Checked = true;
            gl_fa_3_gb_1_checkbox_1.CheckedChanged += new EventHandler(gl_fa_3_gb_1_checkbox_1_checkedChanged);

            
            gl_hg_listbox.Location = new Point(132, 288);
            gl_hg_listbox.Size = new Size(70, 50);
            gl_hg_listbox.BringToFront();
            gl_hg_listbox.BorderStyle = BorderStyle.Fixed3D;
            gl_hg_listbox.SelectionMode = SelectionMode.MultiSimple;
            gl_hg_listbox.SelectedValueChanged += new System.EventHandler(gl_hg_listbox_SelectedValueChanged);
            gl_hg_listbox.Visible = false;
            
            gl_hg_label.Location = new Point(gl_hg_listbox.Left, gl_hg_listbox.Top - sep);
            gl_hg_label.Text = "Sugar head";
            gl_hg_label.DoubleClick += new EventHandler(sugar_heady);
            gl_hg_label.Visible = false;
            

            gl_positive_adduct.Location = new Point(left_groupboxes, 60);
            gl_positive_adduct.Width = 120;
            gl_positive_adduct.Height = 120;
            gl_positive_adduct.Text = "Positive adducts";
            gl_positive_adduct.DoubleClick += new EventHandler(trigger_easteregg);
            gl_pos_adduct_checkbox_1.Parent = gl_positive_adduct;
            gl_pos_adduct_checkbox_1.Location = new Point(10, 15);
            gl_pos_adduct_checkbox_1.Text = "+H⁺";
            gl_pos_adduct_checkbox_1.CheckedChanged += new EventHandler(gl_pos_adduct_checkbox_1_checkedChanged);
            gl_pos_adduct_checkbox_1.Enabled = false;
            gl_pos_adduct_checkbox_2.Parent = gl_positive_adduct;
            gl_pos_adduct_checkbox_2.Location = new Point(10, 35);
            gl_pos_adduct_checkbox_2.Text = "+2H⁺⁺";
            gl_pos_adduct_checkbox_2.Enabled = false;
            gl_pos_adduct_checkbox_2.CheckedChanged += new EventHandler(gl_pos_adduct_checkbox_2_checkedChanged);
            gl_pos_adduct_checkbox_3.Parent = gl_positive_adduct;
            gl_pos_adduct_checkbox_3.Location = new Point(10, 55);
            gl_pos_adduct_checkbox_3.Text = "+NH4⁺";
            gl_pos_adduct_checkbox_3.Checked = true;
            gl_pos_adduct_checkbox_3.CheckedChanged += new EventHandler(gl_pos_adduct_checkbox_3_checkedChanged);
            gl_negative_adduct.Location = new Point(left_groupboxes, 200);
            gl_negative_adduct.Width = 120;
            gl_negative_adduct.Height = 120;
            gl_negative_adduct.Text = "Negative adducts";
            gl_neg_adduct_checkbox_1.Parent = gl_negative_adduct;
            gl_neg_adduct_checkbox_1.Location = new Point(10, 15);
            gl_neg_adduct_checkbox_1.Text = "-H⁻";
            gl_neg_adduct_checkbox_1.Enabled = false;
            gl_neg_adduct_checkbox_1.CheckedChanged += new EventHandler(gl_neg_adduct_checkbox_1_checkedChanged);
            gl_neg_adduct_checkbox_2.Parent = gl_negative_adduct;
            gl_neg_adduct_checkbox_2.Location = new Point(10, 35);
            gl_neg_adduct_checkbox_2.Text = "-2H⁻ ⁻";
            gl_neg_adduct_checkbox_2.Enabled = false;
            gl_neg_adduct_checkbox_2.CheckedChanged += new EventHandler(gl_neg_adduct_checkbox_2_checkedChanged);
            gl_neg_adduct_checkbox_3.Parent = gl_negative_adduct;
            gl_neg_adduct_checkbox_3.Location = new Point(10, 55);
            gl_neg_adduct_checkbox_3.Text = "+HCOO⁻";
            gl_neg_adduct_checkbox_3.Enabled = false;
            gl_neg_adduct_checkbox_3.CheckedChanged += new EventHandler(gl_neg_adduct_checkbox_3_checkedChanged);
            gl_neg_adduct_checkbox_4.Parent = gl_negative_adduct;
            gl_neg_adduct_checkbox_4.Location = new Point(10, 75);
            gl_neg_adduct_checkbox_4.Text = "+CH3COO⁻";
            gl_neg_adduct_checkbox_4.Enabled = false;
            gl_neg_adduct_checkbox_4.CheckedChanged += new EventHandler(gl_neg_adduct_checkbox_4_checkedChanged);

            gl_picture_box.Image = glycero_backbone_image;
            gl_picture_box.Location = new Point(77, 79);
            gl_picture_box.SizeMode = PictureBoxSizeMode.AutoSize;
            gl_picture_box.SendToBack();


            gl_contains_sugar.Location = new Point(158, 350);
            gl_contains_sugar.Width = 120;
            gl_contains_sugar.Text = "Contains sugar";
            gl_contains_sugar.CheckedChanged += new EventHandler(gl_contains_sugar_checkedChanged);
            gl_contains_sugar.BringToFront();
            
            glRepresentativeFA.Location = new Point(gl_hydroxyl_1_textbox.Left + gl_hydroxyl_1_textbox.Width + sep, gl_hydroxyl_1_textbox.Top);
            glRepresentativeFA.Width = 120;
            glRepresentativeFA.Text = "First FA representative";
            glRepresentativeFA.CheckedChanged += new EventHandler(glRepresentativeFA_checkedChanged);
            glRepresentativeFA.SendToBack();


            // tab for phospholipids
            phospholipids_tab.Controls.Add(pl_add_lipid_button);
            phospholipids_tab.Controls.Add(pl_reset_lipid_button);
            phospholipids_tab.Controls.Add(pl_modify_lipid_button);
            phospholipids_tab.Controls.Add(pl_ms2fragments_lipid_button);
            phospholipids_tab.Controls.Add(pl_fa_1_gb_1_checkbox_3);
            phospholipids_tab.Controls.Add(pl_fa_1_gb_1_checkbox_2);
            phospholipids_tab.Controls.Add(pl_fa_1_gb_1_checkbox_1);
            phospholipids_tab.Controls.Add(pl_fa_2_gb_1_checkbox_3);
            phospholipids_tab.Controls.Add(pl_fa_2_gb_1_checkbox_2);
            phospholipids_tab.Controls.Add(pl_fa_2_gb_1_checkbox_1);
            phospholipids_tab.Controls.Add(pl_is_cl);
            phospholipids_tab.Controls.Add(pl_picture_box);
            phospholipids_tab.Controls.Add(pl_fa_1_textbox);
            phospholipids_tab.Controls.Add(pl_fa_2_textbox);
            phospholipids_tab.Controls.Add(pl_db_1_textbox);
            phospholipids_tab.Controls.Add(pl_db_2_textbox);
            phospholipids_tab.Controls.Add(pl_hydroxyl_1_textbox);
            phospholipids_tab.Controls.Add(pl_hydroxyl_2_textbox);
            phospholipids_tab.Controls.Add(pl_fa_1_combobox);
            phospholipids_tab.Controls.Add(pl_fa_2_combobox);
            phospholipids_tab.Controls.Add(pl_db_1_label);
            phospholipids_tab.Controls.Add(pl_db_2_label);
            phospholipids_tab.Controls.Add(pl_hydroxyl_1_label);
            phospholipids_tab.Controls.Add(pl_hydroxyl_2_label);
            phospholipids_tab.Controls.Add(pl_hg_listbox);
            phospholipids_tab.Controls.Add(pl_hg_label);
            phospholipids_tab.Controls.Add(plRepresentativeFA);
            phospholipids_tab.Controls.Add(pl_positive_adduct);
            phospholipids_tab.Controls.Add(pl_negative_adduct);
            phospholipids_tab.Parent = tab_control;
            phospholipids_tab.Text = "Phospholipids";
            phospholipids_tab.Location = new Point(0, 0);
            phospholipids_tab.Size = this.Size;
            phospholipids_tab.AutoSize = true;
            phospholipids_tab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            phospholipids_tab.BackColor = Color.White;


            pl_fa_1_combobox.BringToFront();
            pl_fa_1_textbox.BringToFront();
            pl_fa_1_textbox.Location = new Point(400, 64);
            pl_fa_1_textbox.Width = fa_length;
            pl_fa_1_textbox.Text = "0, 2, 4, 6-7";
            pl_fa_1_textbox.TextChanged += new EventHandler(pl_fa_1_textbox_valueChanged);
            toolTip1.SetToolTip(pl_fa_1_textbox, formatting_fa);
            pl_fa_1_combobox.Location = new Point(pl_fa_1_textbox.Left, pl_fa_1_textbox.Top - sepText);
            pl_fa_1_combobox.Width = fa_length;
            pl_fa_1_combobox.SelectedItem = "Fatty acid chain";
            pl_fa_1_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            pl_fa_1_combobox.SelectedIndexChanged += new EventHandler(pl_fa_1_combobox_valueChanged);
            pl_db_1_textbox.Location = new Point(pl_fa_1_textbox.Left + pl_fa_1_textbox.Width + sep, pl_fa_1_textbox.Top);
            pl_db_1_textbox.Width = db_length;
            pl_db_1_textbox.Text = "0-2";
            pl_db_1_textbox.TextChanged += new EventHandler(pl_db_1_textbox_valueChanged);
            toolTip1.SetToolTip(pl_db_1_textbox, formatting_db);
            pl_db_1_label.Location = new Point(pl_db_1_textbox.Left, pl_db_1_textbox.Top - sep);
            pl_db_1_label.Width = db_length;
            pl_db_1_label.Text = db_text;
            pl_hydroxyl_1_textbox.Width = db_length;
            pl_hydroxyl_1_textbox.Location = new Point(pl_db_1_textbox.Left + pl_db_1_textbox.Width + sep, pl_db_1_textbox.Top);
            pl_hydroxyl_1_textbox.TextChanged += new EventHandler(pl_hydroxyl_1_textbox_valueChanged);
            toolTip1.SetToolTip(pl_hydroxyl_1_textbox, formatting_hydroxyl);
            pl_hydroxyl_1_label.Width = db_length;
            pl_hydroxyl_1_label.Location = new Point(pl_hydroxyl_1_textbox.Left, pl_hydroxyl_1_textbox.Top - sep);
            pl_hydroxyl_1_label.Text = hydroxyl_text;

            pl_fa_1_gb_1_checkbox_3.Location = new Point(pl_fa_1_textbox.Left + 90, pl_fa_1_textbox.Top + pl_fa_1_textbox.Height);
            pl_fa_1_gb_1_checkbox_3.Text = "FAe";
            pl_fa_1_gb_1_checkbox_3.CheckedChanged += new EventHandler(pl_fa_1_gb_1_checkbox_3_checkedChanged);
            pl_fa_1_gb_1_checkbox_3.MouseLeave += new System.EventHandler(pl_fa_1_gb_1_checkbox_3_MouseLeave);
            pl_fa_1_gb_1_checkbox_3.MouseMove += new System.Windows.Forms.MouseEventHandler(pl_fa_1_gb_1_checkbox_3_MouseHover);
            pl_fa_1_gb_1_checkbox_2.Location = new Point(pl_fa_1_textbox.Left + 40, pl_fa_1_textbox.Top + pl_fa_1_textbox.Height);
            pl_fa_1_gb_1_checkbox_2.Text = "FAp";
            pl_fa_1_gb_1_checkbox_2.CheckedChanged += new EventHandler(pl_fa_1_gb_1_checkbox_2_checkedChanged);
            pl_fa_1_gb_1_checkbox_2.MouseLeave += new System.EventHandler(pl_fa_1_gb_1_checkbox_2_MouseLeave);
            pl_fa_1_gb_1_checkbox_2.MouseMove += new System.Windows.Forms.MouseEventHandler(pl_fa_1_gb_1_checkbox_2_MouseHover);
            pl_fa_1_gb_1_checkbox_1.Location = new Point(pl_fa_1_textbox.Left, pl_fa_1_textbox.Top + pl_fa_1_textbox.Height);
            pl_fa_1_gb_1_checkbox_1.Text = "FA";
            pl_fa_1_gb_1_checkbox_1.Checked = true;
            pl_fa_1_gb_1_checkbox_1.CheckedChanged += new EventHandler(pl_fa_1_gb_1_checkbox_1_checkedChanged);

            pl_fa_2_combobox.BringToFront();
            pl_fa_2_textbox.BringToFront();
            pl_fa_2_textbox.Location = new Point(312, 144);
            pl_fa_2_textbox.Width = fa_length;
            pl_fa_2_textbox.Text = "2, 5, 17-19";
            pl_fa_2_textbox.TextChanged += new EventHandler(pl_fa_2_textbox_valueChanged);
            toolTip1.SetToolTip(pl_fa_2_textbox, formatting_fa);
            pl_fa_2_combobox.Location = new Point(pl_fa_2_textbox.Left, pl_fa_2_textbox.Top - sepText);
            pl_fa_2_combobox.Width = fa_length;
            pl_fa_2_combobox.SelectedItem = "Fatty acid chain";
            pl_fa_2_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            pl_fa_2_combobox.SelectedIndexChanged += new EventHandler(pl_fa_2_combobox_valueChanged);
            pl_db_2_textbox.Location = new Point(pl_fa_2_textbox.Left + pl_fa_2_textbox.Width + sep, pl_fa_2_textbox.Top);
            pl_db_2_textbox.Width = db_length;
            pl_db_2_textbox.Text = "5-6";
            pl_db_2_textbox.TextChanged += new EventHandler(pl_db_2_textbox_valueChanged);
            toolTip1.SetToolTip(pl_db_2_textbox, formatting_db);
            pl_db_2_label.Location = new Point(pl_db_2_textbox.Left, pl_db_2_textbox.Top - sep);
            pl_db_2_label.Width = db_length;
            pl_db_2_label.Text = db_text;
            pl_hydroxyl_2_textbox.Width = db_length;
            pl_hydroxyl_2_textbox.Location = new Point(pl_db_2_textbox.Left + pl_db_2_textbox.Width + sep, pl_db_2_textbox.Top);
            pl_hydroxyl_2_textbox.TextChanged += new EventHandler(pl_hydroxyl_2_textbox_valueChanged);
            toolTip1.SetToolTip(pl_hydroxyl_2_textbox, formatting_hydroxyl);
            pl_hydroxyl_2_label.Width = db_length;
            pl_hydroxyl_2_label.Location = new Point(pl_hydroxyl_2_textbox.Left, pl_hydroxyl_2_textbox.Top - sep);
            pl_hydroxyl_2_label.Text = hydroxyl_text;

            pl_fa_2_gb_1_checkbox_3.Location = new Point(pl_fa_2_textbox.Left + 90, pl_fa_2_textbox.Top + pl_fa_2_textbox.Height);
            pl_fa_2_gb_1_checkbox_3.Text = "FAe";
            pl_fa_2_gb_1_checkbox_3.CheckedChanged += new EventHandler(pl_fa_2_gb_1_checkbox_3_checkedChanged);
            pl_fa_2_gb_1_checkbox_3.MouseLeave += new System.EventHandler(pl_fa_2_gb_1_checkbox_3_MouseLeave);
            pl_fa_2_gb_1_checkbox_3.MouseMove += new System.Windows.Forms.MouseEventHandler(pl_fa_2_gb_1_checkbox_3_MouseHover);
            pl_fa_2_gb_1_checkbox_2.Location = new Point(pl_fa_2_textbox.Left + 40, pl_fa_2_textbox.Top + pl_fa_2_textbox.Height);
            pl_fa_2_gb_1_checkbox_2.Text = "FAp";
            pl_fa_2_gb_1_checkbox_2.CheckedChanged += new EventHandler(pl_fa_2_gb_1_checkbox_2_checkedChanged);
            pl_fa_2_gb_1_checkbox_2.MouseLeave += new System.EventHandler(pl_fa_2_gb_1_checkbox_2_MouseLeave);
            pl_fa_2_gb_1_checkbox_2.MouseMove += new System.Windows.Forms.MouseEventHandler(pl_fa_2_gb_1_checkbox_2_MouseHover);
            pl_fa_2_gb_1_checkbox_1.Location = new Point(pl_fa_2_textbox.Left, pl_fa_2_textbox.Top + pl_fa_2_textbox.Height);
            pl_fa_2_gb_1_checkbox_1.Text = "FA";
            pl_fa_2_gb_1_checkbox_1.Checked = true;
            pl_fa_2_gb_1_checkbox_1.CheckedChanged += new EventHandler(pl_fa_2_gb_1_checkbox_1_checkedChanged);
            
            pl_is_cl.Location = new Point(130, 36);
            pl_is_cl.Text = "Is cardiolipin";
            pl_is_cl.CheckedChanged += new EventHandler(pl_is_cl_checkedChanged);
            pl_is_cl.BringToFront();

            
            pl_hg_listbox.Location = new Point(25, 50);
            pl_hg_listbox.Size = new Size(70, 150);
            pl_hg_listbox.BringToFront();
            pl_hg_listbox.BorderStyle = BorderStyle.Fixed3D;
            pl_hg_listbox.SelectionMode = SelectionMode.MultiSimple;
            pl_hg_listbox.SelectedValueChanged += new System.EventHandler(pl_hg_listbox_SelectedValueChanged);
            pl_hg_listbox.MouseLeave += new System.EventHandler(pl_hg_listbox_MouseLeave);
            pl_hg_listbox.MouseMove += new System.Windows.Forms.MouseEventHandler(pl_hg_listbox_MouseHover);
            
            pl_hg_label.Location = new Point(pl_hg_listbox.Left, pl_hg_listbox.Top - sep);
            pl_hg_label.Text = "Head group";

            pl_positive_adduct.Location = new Point(left_groupboxes, 60);
            pl_positive_adduct.Width = 120;
            pl_positive_adduct.Height = 120;
            pl_positive_adduct.Text = "Positive adducts";
            pl_pos_adduct_checkbox_1.Parent = pl_positive_adduct;
            pl_pos_adduct_checkbox_1.Location = new Point(10, 15);
            pl_pos_adduct_checkbox_1.Text = "+H⁺";
            pl_pos_adduct_checkbox_1.Checked = true;
            pl_pos_adduct_checkbox_1.CheckedChanged += new EventHandler(pl_pos_adduct_checkbox_1_checkedChanged);
            pl_pos_adduct_checkbox_2.Parent = pl_positive_adduct;
            pl_pos_adduct_checkbox_2.Location = new Point(10, 35);
            pl_pos_adduct_checkbox_2.Text = "+2H⁺⁺";
            pl_pos_adduct_checkbox_2.Enabled = false;
            pl_pos_adduct_checkbox_2.CheckedChanged += new EventHandler(pl_pos_adduct_checkbox_2_checkedChanged);
            pl_pos_adduct_checkbox_3.Parent = pl_positive_adduct;
            pl_pos_adduct_checkbox_3.Location = new Point(10, 55);
            pl_pos_adduct_checkbox_3.Text = "+NH4⁺";
            pl_pos_adduct_checkbox_3.CheckedChanged += new EventHandler(pl_pos_adduct_checkbox_3_checkedChanged);
            pl_negative_adduct.Location = new Point(left_groupboxes, 200);
            pl_negative_adduct.Width = 120;
            pl_negative_adduct.Height = 120;
            pl_negative_adduct.Text = "Negative adducts";
            pl_neg_adduct_checkbox_1.Parent = pl_negative_adduct;
            pl_neg_adduct_checkbox_1.Location = new Point(10, 15);
            pl_neg_adduct_checkbox_1.Text = "-H⁻";
            pl_neg_adduct_checkbox_1.CheckedChanged += new EventHandler(pl_neg_adduct_checkbox_1_checkedChanged);
            pl_neg_adduct_checkbox_2.Parent = pl_negative_adduct;
            pl_neg_adduct_checkbox_2.Location = new Point(10, 35);
            pl_neg_adduct_checkbox_2.Text = "-2H⁻ ⁻";
            pl_neg_adduct_checkbox_2.Enabled = false;
            pl_neg_adduct_checkbox_2.CheckedChanged += new EventHandler(pl_neg_adduct_checkbox_2_checkedChanged);
            pl_neg_adduct_checkbox_3.Parent = pl_negative_adduct;
            pl_neg_adduct_checkbox_3.Location = new Point(10, 55);
            pl_neg_adduct_checkbox_3.Text = "+HCOO⁻";
            pl_neg_adduct_checkbox_3.CheckedChanged += new EventHandler(pl_neg_adduct_checkbox_3_checkedChanged);
            pl_neg_adduct_checkbox_4.Parent = pl_negative_adduct;
            pl_neg_adduct_checkbox_4.Location = new Point(10, 75);
            pl_neg_adduct_checkbox_4.Text = "+CH3COO⁻";
            pl_neg_adduct_checkbox_4.CheckedChanged += new EventHandler(pl_neg_adduct_checkbox_4_checkedChanged);


            pl_picture_box.Image = phospho_backbone_image;
            //pl_picture_box.Location = new Point((int)(214 - phospho_backbone_image.Width * 0.5), (int)(204 - phospho_backbone_image.Height * 0.5));
            pl_picture_box.Location = new Point(107, 13);
            pl_picture_box.SizeMode = PictureBoxSizeMode.AutoSize;
            pl_picture_box.SendToBack();
            
            plRepresentativeFA.Location = new Point(cl_hydroxyl_1_textbox.Left + cl_hydroxyl_1_textbox.Width + sep, cl_hydroxyl_1_textbox.Top);
            plRepresentativeFA.Width = 120;
            plRepresentativeFA.Text = "First FA representative";
            plRepresentativeFA.CheckedChanged += new EventHandler(plRepresentativeFA_checkedChanged);
            plRepresentativeFA.SendToBack();



            // tab for sphingolipids
            sphingolipids_tab.Controls.Add(sl_add_lipid_button);
            sphingolipids_tab.Controls.Add(sl_reset_lipid_button);
            sphingolipids_tab.Controls.Add(sl_modify_lipid_button);
            sphingolipids_tab.Controls.Add(sl_ms2fragments_lipid_button);
            sphingolipids_tab.Controls.Add(sl_picture_box);
            sphingolipids_tab.Controls.Add(sl_lcb_textbox);
            sphingolipids_tab.Controls.Add(sl_fa_textbox);
            sphingolipids_tab.Controls.Add(sl_db_1_textbox);
            sphingolipids_tab.Controls.Add(sl_db_2_textbox);
            sphingolipids_tab.Controls.Add(sl_lcb_combobox);
            sphingolipids_tab.Controls.Add(sl_fa_combobox);
            sphingolipids_tab.Controls.Add(sl_db_1_label);
            sphingolipids_tab.Controls.Add(sl_db_2_label);
            sphingolipids_tab.Controls.Add(sl_hg_label);
            sphingolipids_tab.Controls.Add(sl_hg_listbox);
            sphingolipids_tab.Controls.Add(sl_lcb_hydroxy_combobox);
            sphingolipids_tab.Controls.Add(sl_fa_hydroxy_combobox);
            sphingolipids_tab.Controls.Add(sl_fa_hydroxy_label);
            sphingolipids_tab.Controls.Add(sl_lcb_hydroxy_label);
            sphingolipids_tab.Controls.Add(sl_positive_adduct);
            sphingolipids_tab.Controls.Add(sl_negative_adduct);
            sphingolipids_tab.Parent = tab_control;
            sphingolipids_tab.Text = "Sphingolipids";
            sphingolipids_tab.Location = new Point(0, 0);
            sphingolipids_tab.Size = this.Size;
            sphingolipids_tab.AutoSize = true;
            sphingolipids_tab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            sphingolipids_tab.BackColor = Color.White;
            Font sl_fnt = new Font(sphingolipids_tab.Font.FontFamily, 8.25F);
            sphingolipids_tab.Font = sl_fnt;

            sl_fa_combobox.BringToFront();
            sl_fa_textbox.BringToFront();
            sl_fa_textbox.Location = new Point(258, 280);
            sl_fa_textbox.Width = fa_length;
            sl_fa_textbox.Text = "2, 5, 17-19";
            sl_fa_textbox.TextChanged += new EventHandler(sl_fa_textbox_valueChanged);
            toolTip1.SetToolTip(sl_fa_textbox, formatting_fa);
            sl_fa_combobox.Location = new Point(sl_fa_textbox.Left, sl_fa_textbox.Top - sepText);
            sl_fa_combobox.Width = fa_length;
            sl_fa_combobox.SelectedItem = "Fatty acid chain";
            sl_fa_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            sl_fa_combobox.SelectedIndexChanged += new EventHandler(sl_fa_combobox_valueChanged);
            sl_db_1_textbox.Location = new Point(sl_fa_textbox.Left + sl_fa_textbox.Width + sep, sl_fa_textbox.Top);
            sl_db_1_textbox.Width = db_length;
            sl_db_1_textbox.Text = "5-6";
            sl_db_1_textbox.TextChanged += new EventHandler(sl_db_1_textbox_valueChanged);
            toolTip1.SetToolTip(sl_db_1_textbox, formatting_db);
            sl_db_1_label.Location = new Point(sl_db_1_textbox.Left, sl_db_1_textbox.Top - sep);
            sl_db_1_label.Width = db_length;
            sl_db_1_label.Text = db_text;
            sl_fa_hydroxy_combobox.Location = new Point(sl_db_1_textbox.Left + sl_db_1_textbox.Width + sep, sl_db_1_textbox.Top);
            sl_fa_hydroxy_combobox.SelectedItem = "2";
            sl_fa_hydroxy_combobox.Width = db_length;
            sl_fa_hydroxy_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            sl_fa_hydroxy_combobox.SelectedIndexChanged += new EventHandler(sl_fa_hydroxy_combobox_valueChanged);
            sl_fa_hydroxy_label.Location = new Point(sl_fa_hydroxy_combobox.Left, sl_fa_hydroxy_combobox.Top - sep);
            sl_fa_hydroxy_label.Text = hydroxyl_text;


            sl_lcb_combobox.BringToFront();
            sl_lcb_textbox.BringToFront();
            sl_lcb_hydroxy_combobox.BringToFront();
            sl_fa_hydroxy_combobox.BringToFront();
            sl_lcb_textbox.Location = new Point(294, 203);
            sl_lcb_textbox.Width = fa_length;
            sl_lcb_textbox.Text = "14, 16-18, 22";
            sl_lcb_textbox.TextChanged += new EventHandler(sl_lcb_textbox_valueChanged);
            toolTip1.SetToolTip(sl_lcb_textbox, formatting_fa);
            sl_lcb_combobox.Location = new Point(sl_lcb_textbox.Left, sl_lcb_textbox.Top - sepText);
            sl_lcb_combobox.Width = fa_length;
            sl_lcb_combobox.SelectedItem = "Long chain base";
            sl_lcb_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            sl_lcb_combobox.SelectedIndexChanged += new EventHandler(sl_lcb_combobox_valueChanged);
            sl_db_2_textbox.Location = new Point(sl_lcb_textbox.Left + sl_lcb_textbox.Width + sep, sl_lcb_textbox.Top);
            sl_db_2_textbox.Width = db_length;
            sl_db_2_textbox.Text = "0-2";
            sl_db_2_textbox.TextChanged += new EventHandler(sl_db_2_textbox_valueChanged);
            toolTip1.SetToolTip(sl_db_2_textbox, formatting_db);
            sl_db_2_label.Location = new Point(sl_db_2_textbox.Left, sl_db_2_textbox.Top - sep);
            sl_db_2_label.Width = db_length;
            sl_db_2_label.Text = db_text;
            sl_lcb_hydroxy_combobox.Location = new Point(sl_db_2_textbox.Left + sl_db_2_textbox.Width + sep, sl_db_2_textbox.Top);
            sl_lcb_hydroxy_combobox.SelectedItem = "2";
            sl_lcb_hydroxy_combobox.Width = db_length;
            sl_lcb_hydroxy_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            sl_lcb_hydroxy_combobox.SelectedIndexChanged += new EventHandler(sl_lcb_hydroxy_combobox_valueChanged);
            sl_lcb_hydroxy_label.Location = new Point(sl_lcb_hydroxy_combobox.Left, sl_lcb_hydroxy_combobox.Top - sep);
            sl_lcb_hydroxy_label.Text = hydroxyl_text;

            sl_hg_label.BringToFront();
            sl_hg_listbox.Location = new Point(54, 105);
            sl_hg_listbox.Size = new Size(80, 250);
            sl_hg_listbox.BringToFront();
            sl_hg_listbox.BorderStyle = BorderStyle.Fixed3D;
            sl_hg_listbox.SelectionMode = SelectionMode.MultiSimple;
            sl_hg_listbox.SelectedValueChanged += new System.EventHandler(sl_hg_listbox_SelectedValueChanged);
            sl_hg_listbox.MouseLeave += new System.EventHandler(sl_hg_listbox_MouseLeave);
            sl_hg_listbox.MouseMove += new System.Windows.Forms.MouseEventHandler(sl_hg_listbox_MouseHover);
            sl_hg_label.Location = new Point(sl_hg_listbox.Left, sl_hg_listbox.Top - sep);
            sl_hg_label.Text = "Head group";
            

            sl_positive_adduct.Location = new Point(left_groupboxes, 60);
            sl_positive_adduct.Width = 120;
            sl_positive_adduct.Height = 120;
            sl_positive_adduct.Text = "Positive adducts";
            
            sl_pos_adduct_checkbox_1.Parent = sl_positive_adduct;
            sl_pos_adduct_checkbox_1.Location = new Point(10, 15);
            sl_pos_adduct_checkbox_1.Text = "+H⁺";
            sl_pos_adduct_checkbox_1.Checked = true;
            sl_pos_adduct_checkbox_1.CheckedChanged += new EventHandler(sl_pos_adduct_checkbox_1_checkedChanged);
            sl_pos_adduct_checkbox_2.Parent = sl_positive_adduct;
            sl_pos_adduct_checkbox_2.Location = new Point(10, 35);
            sl_pos_adduct_checkbox_2.Text = "+2H⁺⁺";
            sl_pos_adduct_checkbox_2.Enabled = false;
            sl_pos_adduct_checkbox_2.CheckedChanged += new EventHandler(sl_pos_adduct_checkbox_2_checkedChanged);
            sl_pos_adduct_checkbox_3.Parent = sl_positive_adduct;
            sl_pos_adduct_checkbox_3.Location = new Point(10, 55);
            sl_pos_adduct_checkbox_3.Text = "+NH4⁺";
            sl_pos_adduct_checkbox_3.CheckedChanged += new EventHandler(sl_pos_adduct_checkbox_3_checkedChanged);
            sl_negative_adduct.Location = new Point(left_groupboxes, 200);
            sl_negative_adduct.Width = 120;
            sl_negative_adduct.Height = 120;
            sl_negative_adduct.Text = "Negative adducts";
            sl_neg_adduct_checkbox_1.Parent = sl_negative_adduct;
            sl_neg_adduct_checkbox_1.Location = new Point(10, 15);
            sl_neg_adduct_checkbox_1.Text = "-H⁻";
            sl_neg_adduct_checkbox_1.CheckedChanged += new EventHandler(sl_neg_adduct_checkbox_1_checkedChanged);
            sl_neg_adduct_checkbox_2.Parent = sl_negative_adduct;
            sl_neg_adduct_checkbox_2.Location = new Point(10, 35);
            sl_neg_adduct_checkbox_2.Text = "-2H⁻⁻";
            sl_neg_adduct_checkbox_2.Enabled = false;
            sl_neg_adduct_checkbox_2.CheckedChanged += new EventHandler(sl_neg_adduct_checkbox_2_checkedChanged);
            sl_neg_adduct_checkbox_3.Parent = sl_negative_adduct;
            sl_neg_adduct_checkbox_3.Location = new Point(10, 55);
            sl_neg_adduct_checkbox_3.Text = "+HCOO⁻";
            sl_neg_adduct_checkbox_3.CheckedChanged += new EventHandler(sl_neg_adduct_checkbox_3_checkedChanged);
            sl_neg_adduct_checkbox_4.Parent = sl_negative_adduct;
            sl_neg_adduct_checkbox_4.Location = new Point(10, 75);
            sl_neg_adduct_checkbox_4.Text = "+CH3COO⁻";
            sl_neg_adduct_checkbox_4.CheckedChanged += new EventHandler(sl_neg_adduct_checkbox_4_checkedChanged);

            sl_picture_box.Image = sphingo_backbone_image;
            sl_picture_box.Location = new Point((int)(214 - sphingo_backbone_image.Width * 0.5), (int)(204 - sphingo_backbone_image.Height * 0.5));
            sl_picture_box.SizeMode = PictureBoxSizeMode.AutoSize;
            sl_picture_box.SendToBack();



            lipids_groupbox.Controls.Add(lipids_gridview);
            lipids_groupbox.Controls.Add(open_review_form_button);
            lipids_groupbox.Dock = DockStyle.Bottom;
            lipids_groupbox.Text = "Lipid list";
            lipids_groupbox.Height = 180;

            cl_add_lipid_button.Text = "Add cardiolipins";
            cl_add_lipid_button.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            cl_add_lipid_button.Width = 146;
            cl_add_lipid_button.Height = 26;
            cl_add_lipid_button.Image = addImage;
            cl_add_lipid_button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            cl_add_lipid_button.Location = new Point(left_groupboxes, top_low_buttons);
            cl_add_lipid_button.BackColor = SystemColors.Control;
            cl_add_lipid_button.Click += register_lipid;

            cl_reset_lipid_button.Text = "Reset lipid";
            cl_reset_lipid_button.Width = 130;
            cl_reset_lipid_button.Height = 26;
            cl_reset_lipid_button.Location = new Point(20, top_low_buttons);
            cl_reset_lipid_button.BackColor = SystemColors.Control;
            cl_reset_lipid_button.Click += reset_cl_lipid;

            cl_modify_lipid_button.Text = "Modify lipid";
            cl_modify_lipid_button.Width = 130;
            cl_modify_lipid_button.Height = 26;
            cl_modify_lipid_button.Location = new Point(left_groupboxes - 140, top_low_buttons);
            cl_modify_lipid_button.BackColor = SystemColors.Control;
            cl_modify_lipid_button.Click += modify_cl_lipid;

            cl_ms2fragments_lipid_button.Text = "MS2 fragments";
            cl_ms2fragments_lipid_button.Width = 130;
            cl_ms2fragments_lipid_button.Height = 26;
            cl_ms2fragments_lipid_button.Location = new Point(160, top_low_buttons);
            cl_ms2fragments_lipid_button.BackColor = SystemColors.Control;
            cl_ms2fragments_lipid_button.Click += open_ms2_form;

            gl_add_lipid_button.Text = "Add glycerolipids";
            gl_add_lipid_button.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            gl_add_lipid_button.Image = addImage;
            gl_add_lipid_button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            gl_add_lipid_button.Width = 146;
            gl_add_lipid_button.Height = 26;
            gl_add_lipid_button.Location = new Point(left_groupboxes, top_low_buttons);
            gl_add_lipid_button.BackColor = SystemColors.Control;
            gl_add_lipid_button.Click += register_lipid;

            gl_reset_lipid_button.Text = "Reset lipid";
            gl_reset_lipid_button.Width = 130;
            gl_reset_lipid_button.Height = 26;
            gl_reset_lipid_button.Location = new Point(20, top_low_buttons);
            gl_reset_lipid_button.BackColor = SystemColors.Control;
            gl_reset_lipid_button.Click += reset_gl_lipid;

            gl_modify_lipid_button.Text = "Modify lipid";
            gl_modify_lipid_button.Width = 130;
            gl_modify_lipid_button.Height = 26;
            gl_modify_lipid_button.Location = new Point(left_groupboxes - 140, top_low_buttons);
            gl_modify_lipid_button.BackColor = SystemColors.Control;
            gl_modify_lipid_button.Click += modify_gl_lipid;

            gl_ms2fragments_lipid_button.Text = "MS2 fragments";
            gl_ms2fragments_lipid_button.Width = 130;
            gl_ms2fragments_lipid_button.Height = 26;
            gl_ms2fragments_lipid_button.Location = new Point(160, top_low_buttons);
            gl_ms2fragments_lipid_button.BackColor = SystemColors.Control;
            gl_ms2fragments_lipid_button.Click += open_ms2_form;

            pl_add_lipid_button.Text = "Add phospholipids";
            pl_add_lipid_button.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            pl_add_lipid_button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            pl_add_lipid_button.Image = addImage;
            pl_add_lipid_button.Width = 146;
            pl_add_lipid_button.Height = 26;
            pl_add_lipid_button.Location = new Point(left_groupboxes, top_low_buttons);
            pl_add_lipid_button.BackColor = SystemColors.Control;
            pl_add_lipid_button.Click += register_lipid;

            pl_reset_lipid_button.Text = "Reset lipid";
            pl_reset_lipid_button.Width = 130;
            pl_reset_lipid_button.Height = 26;
            pl_reset_lipid_button.Location = new Point(20, top_low_buttons);
            pl_reset_lipid_button.BackColor = SystemColors.Control;
            pl_reset_lipid_button.Click += reset_pl_lipid;

            pl_modify_lipid_button.Text = "Modify lipid";
            pl_modify_lipid_button.Width = 130;
            pl_modify_lipid_button.Height = 26;
            pl_modify_lipid_button.Location = new Point(left_groupboxes - 140, top_low_buttons);
            pl_modify_lipid_button.BackColor = SystemColors.Control;
            pl_modify_lipid_button.Click += modify_pl_lipid;

            pl_ms2fragments_lipid_button.Text = "MS2 fragments";
            pl_ms2fragments_lipid_button.Width = 130;
            pl_ms2fragments_lipid_button.Height = 26;
            pl_ms2fragments_lipid_button.Location = new Point(160, top_low_buttons);
            pl_ms2fragments_lipid_button.BackColor = SystemColors.Control;
            pl_ms2fragments_lipid_button.Click += open_ms2_form;

            sl_add_lipid_button.Text = "Add sphingolipids";
            sl_add_lipid_button.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            sl_add_lipid_button.Height = 26;
            sl_add_lipid_button.Width = 146;
            sl_add_lipid_button.Location = new Point(left_groupboxes, top_low_buttons);
            sl_add_lipid_button.BackColor = SystemColors.Control;
            sl_add_lipid_button.Click += register_lipid;
            sl_add_lipid_button.Image = addImage;
            sl_add_lipid_button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;

            sl_reset_lipid_button.Text = "Reset lipid";
            sl_reset_lipid_button.Width = 130;
            sl_reset_lipid_button.Height = 26;
            sl_reset_lipid_button.Location = new Point(20, top_low_buttons);
            sl_reset_lipid_button.BackColor = SystemColors.Control;
            sl_reset_lipid_button.Click += reset_sl_lipid;

            sl_modify_lipid_button.Text = "Modify lipid";
            sl_modify_lipid_button.Width = 130;
            sl_modify_lipid_button.Height = 26;
            sl_modify_lipid_button.Location = new Point(left_groupboxes - 140, top_low_buttons);
            sl_modify_lipid_button.BackColor = SystemColors.Control;
            sl_modify_lipid_button.Click += modify_sl_lipid;

            sl_ms2fragments_lipid_button.Text = "MS2 fragments";
            sl_ms2fragments_lipid_button.Width = 130;
            sl_ms2fragments_lipid_button.Height = 26;
            sl_ms2fragments_lipid_button.Location = new Point(160, top_low_buttons);
            sl_ms2fragments_lipid_button.BackColor = SystemColors.Control;
            sl_ms2fragments_lipid_button.Click += open_ms2_form;


            lipids_gridview.AutoSize = true;
            lipids_gridview.Dock = DockStyle.Fill;
            lipids_gridview.DataSource = registered_lipids_datatable;
            lipids_gridview.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            lipids_gridview.AllowUserToResizeColumns = false;
            lipids_gridview.AllowUserToAddRows = false;
            lipids_gridview.Width = this.Width;
            //lipids_gridview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //lipids_gridview.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            lipids_gridview.AllowUserToResizeRows = false;
            lipids_gridview.ReadOnly = true;
            //lipids_gridview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            lipids_gridview.MultiSelect = false;
            lipids_gridview.RowTemplate.Height = 34;
            lipids_gridview.DoubleClick += new EventHandler(lipids_gridview_double_click);
            lipids_gridview.KeyDown += new KeyEventHandler(lipids_gridview_keydown);
            lipids_gridview.EditMode = DataGridViewEditMode.EditOnEnter;
            lipids_gridview.RowHeadersVisible = false;
            lipids_gridview.ScrollBars = ScrollBars.Vertical;
            lipids_gridview.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(lipids_gridview_DataBindingComplete);
            
            
            

            open_review_form_button.Text = "Review Lipids";
            open_review_form_button.Width = 130;
            open_review_form_button.BackColor = SystemColors.Control;
            open_review_form_button.Dock = DockStyle.Bottom;
            open_review_form_button.Click += open_review_Form;

            this.Controls.Add(tab_control);
            this.Controls.Add(lipids_groupbox);
            this.Text = "LipidCreator";
            this.MaximizeBox = false;
            this.Padding = new Padding(5);
        }

        #endregion
    }
}