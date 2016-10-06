using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Collections;
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
        
        private TabControl tab_control = new TabControl();
        private TabPage cardiolipins_tab;
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
        Image glycero_backbone_image;
        Image phospho_backbone_image;
        Image sphingo_backbone_image;

        PictureBox cl_picture_box;
        PictureBox gl_picture_box;
        PictureBox pl_picture_box;
        PictureBox sl_picture_box;

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
        CheckBox cl_fa_1_gb_1_checkbox_4;
        CheckBox cl_fa_2_gb_1_checkbox_1;
        CheckBox cl_fa_2_gb_1_checkbox_2;
        CheckBox cl_fa_2_gb_1_checkbox_3;
        CheckBox cl_fa_2_gb_1_checkbox_4;
        CheckBox cl_fa_3_gb_1_checkbox_1;
        CheckBox cl_fa_3_gb_1_checkbox_2;
        CheckBox cl_fa_3_gb_1_checkbox_3;
        CheckBox cl_fa_3_gb_1_checkbox_4;
        CheckBox cl_fa_4_gb_1_checkbox_1;
        CheckBox cl_fa_4_gb_1_checkbox_2;
        CheckBox cl_fa_4_gb_1_checkbox_3;
        CheckBox cl_fa_4_gb_1_checkbox_4;
        CheckBox gl_fa_1_gb_1_checkbox_1;
        CheckBox gl_fa_1_gb_1_checkbox_2;
        CheckBox gl_fa_1_gb_1_checkbox_3;
        CheckBox gl_fa_1_gb_1_checkbox_4;
        CheckBox gl_fa_2_gb_1_checkbox_1;
        CheckBox gl_fa_2_gb_1_checkbox_2;
        CheckBox gl_fa_2_gb_1_checkbox_3;
        CheckBox gl_fa_2_gb_1_checkbox_4;
        CheckBox gl_fa_3_gb_1_checkbox_1;
        CheckBox gl_fa_3_gb_1_checkbox_2;
        CheckBox gl_fa_3_gb_1_checkbox_3;
        CheckBox gl_fa_3_gb_1_checkbox_4;
        CheckBox pl_fa_1_gb_1_checkbox_1;
        CheckBox pl_fa_1_gb_1_checkbox_2;
        CheckBox pl_fa_1_gb_1_checkbox_3;
        CheckBox pl_fa_1_gb_1_checkbox_4;
        CheckBox pl_fa_2_gb_1_checkbox_1;
        CheckBox pl_fa_2_gb_1_checkbox_2;
        CheckBox pl_fa_2_gb_1_checkbox_3;
        CheckBox pl_fa_2_gb_1_checkbox_4;
        CheckBox sl_fa_gb_1_checkbox_1;
        CheckBox sl_fa_gb_1_checkbox_2;
        CheckBox sl_fa_gb_1_checkbox_3;
        CheckBox sl_fa_gb_1_checkbox_4;

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
        CheckBox cl_pos_adduct_checkbox_4;
        CheckBox cl_neg_adduct_checkbox_1;
        CheckBox cl_neg_adduct_checkbox_2;
        CheckBox cl_neg_adduct_checkbox_3;
        CheckBox cl_neg_adduct_checkbox_4;
        CheckBox gl_pos_adduct_checkbox_1;
        CheckBox gl_pos_adduct_checkbox_2;
        CheckBox gl_pos_adduct_checkbox_3;
        CheckBox gl_pos_adduct_checkbox_4;
        CheckBox gl_neg_adduct_checkbox_1;
        CheckBox gl_neg_adduct_checkbox_2;
        CheckBox gl_neg_adduct_checkbox_3;
        CheckBox gl_neg_adduct_checkbox_4;
        CheckBox pl_pos_adduct_checkbox_1;
        CheckBox pl_pos_adduct_checkbox_2;
        CheckBox pl_pos_adduct_checkbox_3;
        CheckBox pl_pos_adduct_checkbox_4;
        CheckBox pl_neg_adduct_checkbox_1;
        CheckBox pl_neg_adduct_checkbox_2;
        CheckBox pl_neg_adduct_checkbox_3;
        CheckBox pl_neg_adduct_checkbox_4;
        CheckBox sl_pos_adduct_checkbox_1;
        CheckBox sl_pos_adduct_checkbox_2;
        CheckBox sl_pos_adduct_checkbox_3;
        CheckBox sl_pos_adduct_checkbox_4;
        CheckBox sl_neg_adduct_checkbox_1;
        CheckBox sl_neg_adduct_checkbox_2;
        CheckBox sl_neg_adduct_checkbox_3;
        CheckBox sl_neg_adduct_checkbox_4;

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
        Label sl_hydroxy_label;

        Label pl_hg_label;
        ComboBox pl_hg_combobox;

        Label sl_hg_label;
        ComboBox sl_hg_combobox;
        ComboBox sl_hydroxy_combobox;

        ToolTip toolTip1;

        DataGridView lipids_gridview;
        Button send_to_skyline_button;
        DataTable dt;

        ArrayList all_lipids;
        

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "LipidCreator";
            
        
            tab_control = new TabControl();
            this.Size = new System.Drawing.Size(1024, 768);
            cardiolipins_tab = new TabPage();
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
            send_to_skyline_button = new Button();

            cl_picture_box = new PictureBox();
            gl_picture_box = new PictureBox();
            pl_picture_box = new PictureBox();
            sl_picture_box = new PictureBox();

            cardio_backbone_image = Image.FromFile("images/cardio_backbone.png");
            glycero_backbone_image = Image.FromFile("images/GL backbone_2.png");
            phospho_backbone_image = Image.FromFile("images/GL backbone_2.png");
            sphingo_backbone_image = Image.FromFile("images/SL backbone_30.png");

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
            gl_db_1_label = new Label();
            gl_db_2_label = new Label();
            gl_db_3_label = new Label();
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
            pl_db_1_label = new Label();
            pl_db_2_label = new Label();
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
            sl_hydroxy_label = new Label();

            cl_fa_1_gb_1_checkbox_1 = new CheckBox();
            cl_fa_1_gb_1_checkbox_2 = new CheckBox();
            cl_fa_1_gb_1_checkbox_3 = new CheckBox();
            cl_fa_1_gb_1_checkbox_4 = new CheckBox();
            cl_fa_2_gb_1_checkbox_1 = new CheckBox();
            cl_fa_2_gb_1_checkbox_2 = new CheckBox();
            cl_fa_2_gb_1_checkbox_3 = new CheckBox();
            cl_fa_2_gb_1_checkbox_4 = new CheckBox();
            cl_fa_3_gb_1_checkbox_1 = new CheckBox();
            cl_fa_3_gb_1_checkbox_2 = new CheckBox();
            cl_fa_3_gb_1_checkbox_3 = new CheckBox();
            cl_fa_3_gb_1_checkbox_4 = new CheckBox();
            cl_fa_4_gb_1_checkbox_1 = new CheckBox();
            cl_fa_4_gb_1_checkbox_2 = new CheckBox();
            cl_fa_4_gb_1_checkbox_3 = new CheckBox();
            cl_fa_4_gb_1_checkbox_4 = new CheckBox();
            gl_fa_1_gb_1_checkbox_1 = new CheckBox();
            gl_fa_1_gb_1_checkbox_2 = new CheckBox();
            gl_fa_1_gb_1_checkbox_3 = new CheckBox();
            gl_fa_1_gb_1_checkbox_4 = new CheckBox();
            gl_fa_2_gb_1_checkbox_1 = new CheckBox();
            gl_fa_2_gb_1_checkbox_2 = new CheckBox();
            gl_fa_2_gb_1_checkbox_3 = new CheckBox();
            gl_fa_2_gb_1_checkbox_4 = new CheckBox();
            gl_fa_3_gb_1_checkbox_1 = new CheckBox();
            gl_fa_3_gb_1_checkbox_2 = new CheckBox();
            gl_fa_3_gb_1_checkbox_3 = new CheckBox();
            gl_fa_3_gb_1_checkbox_4 = new CheckBox();
            pl_fa_1_gb_1_checkbox_1 = new CheckBox();
            pl_fa_1_gb_1_checkbox_2 = new CheckBox();
            pl_fa_1_gb_1_checkbox_3 = new CheckBox();
            pl_fa_1_gb_1_checkbox_4 = new CheckBox();
            pl_fa_2_gb_1_checkbox_1 = new CheckBox();
            pl_fa_2_gb_1_checkbox_2 = new CheckBox();
            pl_fa_2_gb_1_checkbox_3 = new CheckBox();
            pl_fa_2_gb_1_checkbox_4 = new CheckBox();
            sl_fa_gb_1_checkbox_1 = new CheckBox();
            sl_fa_gb_1_checkbox_2 = new CheckBox();
            sl_fa_gb_1_checkbox_3 = new CheckBox();
            sl_fa_gb_1_checkbox_4 = new CheckBox();

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
            cl_pos_adduct_checkbox_4 = new CheckBox();
            cl_neg_adduct_checkbox_1 = new CheckBox();
            cl_neg_adduct_checkbox_2 = new CheckBox();
            cl_neg_adduct_checkbox_3 = new CheckBox();
            cl_neg_adduct_checkbox_4 = new CheckBox();
            gl_pos_adduct_checkbox_1 = new CheckBox();
            gl_pos_adduct_checkbox_2 = new CheckBox();
            gl_pos_adduct_checkbox_3 = new CheckBox();
            gl_pos_adduct_checkbox_4 = new CheckBox();
            gl_neg_adduct_checkbox_1 = new CheckBox();
            gl_neg_adduct_checkbox_2 = new CheckBox();
            gl_neg_adduct_checkbox_3 = new CheckBox();
            gl_neg_adduct_checkbox_4 = new CheckBox();
            pl_pos_adduct_checkbox_1 = new CheckBox();
            pl_pos_adduct_checkbox_2 = new CheckBox();
            pl_pos_adduct_checkbox_3 = new CheckBox();
            pl_pos_adduct_checkbox_4 = new CheckBox();
            pl_neg_adduct_checkbox_1 = new CheckBox();
            pl_neg_adduct_checkbox_2 = new CheckBox();
            pl_neg_adduct_checkbox_3 = new CheckBox();
            pl_neg_adduct_checkbox_4 = new CheckBox();
            sl_pos_adduct_checkbox_1 = new CheckBox();
            sl_pos_adduct_checkbox_2 = new CheckBox();
            sl_pos_adduct_checkbox_3 = new CheckBox();
            sl_pos_adduct_checkbox_4 = new CheckBox();
            sl_neg_adduct_checkbox_1 = new CheckBox();
            sl_neg_adduct_checkbox_2 = new CheckBox();
            sl_neg_adduct_checkbox_3 = new CheckBox();
            sl_neg_adduct_checkbox_4 = new CheckBox();

            pl_hg_combobox = new ComboBox();
            pl_hg_combobox.Items.Add("PA");
            pl_hg_combobox.Items.Add("PC");
            pl_hg_combobox.Items.Add("PE");
            pl_hg_combobox.Items.Add("PG");
            pl_hg_combobox.Items.Add("PI");
            pl_hg_combobox.Items.Add("PIP");
            pl_hg_combobox.Items.Add("PIP2");
            pl_hg_combobox.Items.Add("PIP3");
            pl_hg_combobox.Items.Add("PS");

            sl_hg_combobox = new ComboBox();
            sl_hg_combobox.Items.Add("Cer");
            sl_hg_combobox.Items.Add("CerP");
            sl_hg_combobox.Items.Add("GB3Cer");
            sl_hg_combobox.Items.Add("GM3Cer");
            sl_hg_combobox.Items.Add("GM4Cer");
            sl_hg_combobox.Items.Add("HexCer");
            sl_hg_combobox.Items.Add("LacCer");
            sl_hg_combobox.Items.Add("Lc3Cer");
            sl_hg_combobox.Items.Add("MIPCer");
            sl_hg_combobox.Items.Add("MIP2Cer");
            sl_hg_combobox.Items.Add("PECer");
            sl_hg_combobox.Items.Add("PICer");
            sl_hg_combobox.Items.Add("SM");

            sl_hydroxy_combobox = new ComboBox();
            sl_hydroxy_combobox.Items.Add("2");
            sl_hydroxy_combobox.Items.Add("3");



            toolTip1 = new ToolTip();
            
            

            string formatting_fa = "Comma seperated single entries or intervals. Example formatting: 2, 3, 5-6, 13-20";
            string formatting_db = "Comma seperated single entries or intervals. Example formatting: 2, 3-4, 6";


            tab_control.Controls.Add(cardiolipins_tab);
            tab_control.Controls.Add(glycerolipids_tab);
            tab_control.Controls.Add(phospholipids_tab);
            tab_control.Controls.Add(sphingolipids_tab);
            tab_control.Dock = DockStyle.Fill;
            tab_control.SelectedIndexChanged += new System.EventHandler(tabIndexChanged);


            // tab for cardiolipins
            cardiolipins_tab.Controls.Add(cl_fa_1_gb_1_checkbox_4);
            cardiolipins_tab.Controls.Add(cl_fa_1_gb_1_checkbox_3);
            cardiolipins_tab.Controls.Add(cl_fa_1_gb_1_checkbox_2);
            cardiolipins_tab.Controls.Add(cl_fa_1_gb_1_checkbox_1);
            cardiolipins_tab.Controls.Add(cl_fa_2_gb_1_checkbox_4);
            cardiolipins_tab.Controls.Add(cl_fa_2_gb_1_checkbox_3);
            cardiolipins_tab.Controls.Add(cl_fa_2_gb_1_checkbox_2);
            cardiolipins_tab.Controls.Add(cl_fa_2_gb_1_checkbox_1);
            cardiolipins_tab.Controls.Add(cl_fa_3_gb_1_checkbox_4);
            cardiolipins_tab.Controls.Add(cl_fa_3_gb_1_checkbox_3);
            cardiolipins_tab.Controls.Add(cl_fa_3_gb_1_checkbox_2);
            cardiolipins_tab.Controls.Add(cl_fa_3_gb_1_checkbox_1);
            cardiolipins_tab.Controls.Add(cl_fa_4_gb_1_checkbox_4);
            cardiolipins_tab.Controls.Add(cl_fa_4_gb_1_checkbox_3);
            cardiolipins_tab.Controls.Add(cl_fa_4_gb_1_checkbox_2);
            cardiolipins_tab.Controls.Add(cl_fa_4_gb_1_checkbox_1);
            cardiolipins_tab.Controls.Add(cl_positive_adduct);
            cardiolipins_tab.Controls.Add(cl_negative_adduct);
            cardiolipins_tab.Controls.Add(cl_add_lipid_button);
            cardiolipins_tab.Controls.Add(cl_reset_lipid_button);
            cardiolipins_tab.Controls.Add(cl_modify_lipid_button);
            cardiolipins_tab.Controls.Add(cl_ms2fragments_lipid_button);
            cardiolipins_tab.Controls.Add(cl_picture_box);
            cardiolipins_tab.Controls.Add(cl_fa_1_textbox);
            cardiolipins_tab.Controls.Add(cl_fa_2_textbox);
            cardiolipins_tab.Controls.Add(cl_fa_3_textbox);
            cardiolipins_tab.Controls.Add(cl_fa_4_textbox);
            cardiolipins_tab.Controls.Add(cl_db_1_textbox);
            cardiolipins_tab.Controls.Add(cl_db_2_textbox);
            cardiolipins_tab.Controls.Add(cl_db_3_textbox);
            cardiolipins_tab.Controls.Add(cl_db_4_textbox);
            cardiolipins_tab.Controls.Add(cl_fa_1_combobox);
            cardiolipins_tab.Controls.Add(cl_fa_2_combobox);
            cardiolipins_tab.Controls.Add(cl_fa_3_combobox);
            cardiolipins_tab.Controls.Add(cl_fa_4_combobox);
            cardiolipins_tab.Controls.Add(cl_db_1_label);
            cardiolipins_tab.Controls.Add(cl_db_2_label);
            cardiolipins_tab.Controls.Add(cl_db_3_label);
            cardiolipins_tab.Controls.Add(cl_db_4_label);
            cardiolipins_tab.Parent = tab_control;
            cardiolipins_tab.Text = "Cardiolipins";
            cardiolipins_tab.Location = new Point(0, 0);
            cardiolipins_tab.Size = this.Size;
            cardiolipins_tab.AutoSize = true;
            cardiolipins_tab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cardiolipins_tab.BackColor = Color.White;

            cl_picture_box.Image = cardio_backbone_image;
            cl_picture_box.Location = new Point((int)(214 - cardio_backbone_image.Width * 0.5), (int)(204 - cardio_backbone_image.Height * 0.5));
            cl_picture_box.SizeMode = PictureBoxSizeMode.AutoSize;

            int sep = 15;
            int sepText = 20;





            cl_fa_1_combobox.BringToFront();
            cl_fa_1_textbox.BringToFront();
            cl_fa_1_textbox.Location = new Point(360, 80);
            cl_fa_1_textbox.Width = 200;
            cl_fa_1_textbox.LostFocus += new EventHandler(cl_fa_1_textbox_valueChanged);
            cl_fa_1_textbox.GotFocus += new EventHandler(resetTextBoxBackground);
            toolTip1.SetToolTip(cl_fa_1_textbox, formatting_fa);
            cl_fa_1_combobox.Location = new Point(cl_fa_1_textbox.Left, cl_fa_1_textbox.Top - sepText);
            cl_fa_1_combobox.Width = 200;
            cl_fa_1_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            cl_fa_1_combobox.SelectedIndexChanged += new EventHandler(cl_fa_1_combobox_valueChanged);
            cl_db_1_textbox.Location = new Point(cl_fa_1_textbox.Left + cl_fa_1_textbox.Width + sep, cl_fa_1_textbox.Top);
            cl_db_1_textbox.Width = 150;
            cl_db_1_textbox.LostFocus += new EventHandler(cl_db_1_textbox_valueChanged);
            cl_db_1_textbox.GotFocus += new EventHandler(resetTextBoxBackground);
            toolTip1.SetToolTip(cl_db_1_textbox, formatting_db);
            cl_db_1_label.Width = 150;
            cl_db_1_label.Location = new Point(cl_db_1_textbox.Left, cl_db_1_textbox.Top - sepText);
            cl_db_1_label.Text = "No. of double bonds";

            cl_fa_1_gb_1_checkbox_4.Location = new Point(cl_fa_1_textbox.Left + 140, cl_fa_1_textbox.Top + cl_fa_1_textbox.Height);
            cl_fa_1_gb_1_checkbox_4.Text = "FAh";
            cl_fa_1_gb_1_checkbox_4.CheckedChanged += new EventHandler(cl_fa_1_gb_1_checkbox_4_checkedChanged);
            cl_fa_1_gb_1_checkbox_3.Location = new Point(cl_fa_1_textbox.Left + 90, cl_fa_1_textbox.Top + cl_fa_1_textbox.Height);
            cl_fa_1_gb_1_checkbox_3.Text = "FAe";
            cl_fa_1_gb_1_checkbox_3.CheckedChanged += new EventHandler(cl_fa_1_gb_1_checkbox_3_checkedChanged);
            cl_fa_1_gb_1_checkbox_2.Location = new Point(cl_fa_1_textbox.Left + 40, cl_fa_1_textbox.Top + cl_fa_1_textbox.Height);
            cl_fa_1_gb_1_checkbox_2.Text = "FAp";
            cl_fa_1_gb_1_checkbox_2.CheckedChanged += new EventHandler(cl_fa_1_gb_1_checkbox_2_checkedChanged);
            cl_fa_1_gb_1_checkbox_1.Location = new Point(cl_fa_1_textbox.Left, cl_fa_1_textbox.Top + cl_fa_1_textbox.Height);
            cl_fa_1_gb_1_checkbox_1.Text = "FA";
            cl_fa_1_gb_1_checkbox_1.Checked = true;
            cl_fa_1_gb_1_checkbox_1.CheckedChanged += new EventHandler(cl_fa_1_gb_1_checkbox_1_checkedChanged);

            cl_positive_adduct.Location = new Point(800, 60);
            cl_positive_adduct.Width = 120;
            cl_positive_adduct.Height = 120;
            cl_positive_adduct.Text = "Positive adducts";
            cl_pos_adduct_checkbox_1.Parent = cl_positive_adduct;
            cl_pos_adduct_checkbox_1.Location = new Point(10, 15);
            cl_pos_adduct_checkbox_1.Text = "+H";
            cl_pos_adduct_checkbox_1.Checked = true;
            cl_pos_adduct_checkbox_1.CheckedChanged += new EventHandler(cl_pos_adduct_checkbox_1_checkedChanged);
            cl_pos_adduct_checkbox_2.Parent = cl_positive_adduct;
            cl_pos_adduct_checkbox_2.Location = new Point(10, 35);
            cl_pos_adduct_checkbox_2.Text = "+2H";
            cl_pos_adduct_checkbox_2.CheckedChanged += new EventHandler(cl_pos_adduct_checkbox_2_checkedChanged);
            cl_pos_adduct_checkbox_3.Parent = cl_positive_adduct;
            cl_pos_adduct_checkbox_3.Location = new Point(10, 55);
            cl_pos_adduct_checkbox_3.Text = "+NH4";
            cl_pos_adduct_checkbox_3.CheckedChanged += new EventHandler(cl_pos_adduct_checkbox_3_checkedChanged);
            cl_pos_adduct_checkbox_4.Parent = cl_positive_adduct;
            cl_pos_adduct_checkbox_4.Location = new Point(10, 75);
            cl_pos_adduct_checkbox_4.Text = "+Na";
            cl_pos_adduct_checkbox_4.CheckedChanged += new EventHandler(cl_pos_adduct_checkbox_4_checkedChanged);
            cl_negative_adduct.Location = new Point(800, 200);
            cl_negative_adduct.Width = 120;
            cl_negative_adduct.Height = 120;
            cl_negative_adduct.Text = "Negative adducts";
            cl_neg_adduct_checkbox_1.Parent = cl_negative_adduct;
            cl_neg_adduct_checkbox_1.Location = new Point(10, 15);
            cl_neg_adduct_checkbox_1.Text = "-H";
            cl_neg_adduct_checkbox_1.CheckedChanged += new EventHandler(cl_neg_adduct_checkbox_1_checkedChanged);
            cl_neg_adduct_checkbox_2.Parent = cl_negative_adduct;
            cl_neg_adduct_checkbox_2.Location = new Point(10, 35);
            cl_neg_adduct_checkbox_2.Text = "-2H";
            cl_neg_adduct_checkbox_2.CheckedChanged += new EventHandler(cl_neg_adduct_checkbox_2_checkedChanged);
            cl_neg_adduct_checkbox_3.Parent = cl_negative_adduct;
            cl_neg_adduct_checkbox_3.Location = new Point(10, 55);
            cl_neg_adduct_checkbox_3.Text = "+HCOO";
            cl_neg_adduct_checkbox_3.CheckedChanged += new EventHandler(cl_neg_adduct_checkbox_3_checkedChanged);
            cl_neg_adduct_checkbox_4.Parent = cl_negative_adduct;
            cl_neg_adduct_checkbox_4.Location = new Point(10, 75);
            cl_neg_adduct_checkbox_4.Text = "+CH3COO";
            cl_neg_adduct_checkbox_4.CheckedChanged += new EventHandler(cl_neg_adduct_checkbox_4_checkedChanged);



            cl_fa_2_combobox.BringToFront();
            cl_fa_2_textbox.BringToFront();
            cl_fa_2_textbox.Location = new Point(284, 166);
            cl_fa_2_textbox.Width = 200;
            cl_fa_2_textbox.LostFocus += new EventHandler(cl_fa_2_textbox_valueChanged);
            cl_fa_2_textbox.GotFocus += new EventHandler(resetTextBoxBackground);
            toolTip1.SetToolTip(cl_fa_2_textbox, formatting_fa);
            cl_fa_2_combobox.Location = new Point(cl_fa_2_textbox.Left, cl_fa_2_textbox.Top - sepText);
            cl_fa_2_combobox.Width = 200;
            cl_fa_2_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            cl_fa_2_combobox.SelectedIndexChanged += new EventHandler(cl_fa_2_combobox_valueChanged);
            cl_db_2_textbox.Location = new Point(cl_fa_2_textbox.Left + cl_fa_2_textbox.Width + sep, cl_fa_2_textbox.Top);
            cl_db_2_textbox.Width = 150;
            cl_db_2_textbox.LostFocus += new EventHandler(cl_db_2_textbox_valueChanged);
            cl_db_2_textbox.GotFocus += new EventHandler(resetTextBoxBackground);
            toolTip1.SetToolTip(cl_db_2_textbox, formatting_db);
            cl_db_2_label.Location = new Point(cl_db_2_textbox.Left, cl_db_2_textbox.Top - sepText);
            cl_db_2_label.Width = 150;
            cl_db_2_label.Text = "No. of double bonds";

            cl_fa_2_gb_1_checkbox_4.Location = new Point(cl_fa_2_textbox.Left + 140, cl_fa_2_textbox.Top + cl_fa_2_textbox.Height);
            cl_fa_2_gb_1_checkbox_4.Text = "FAh";
            cl_fa_2_gb_1_checkbox_4.CheckedChanged += new EventHandler(cl_fa_2_gb_1_checkbox_4_checkedChanged);
            cl_fa_2_gb_1_checkbox_3.Location = new Point(cl_fa_2_textbox.Left + 90, cl_fa_2_textbox.Top + cl_fa_2_textbox.Height);
            cl_fa_2_gb_1_checkbox_3.Text = "FAe";
            cl_fa_2_gb_1_checkbox_3.CheckedChanged += new EventHandler(cl_fa_2_gb_1_checkbox_3_checkedChanged);
            cl_fa_2_gb_1_checkbox_2.Location = new Point(cl_fa_2_textbox.Left + 40, cl_fa_2_textbox.Top + cl_fa_2_textbox.Height);
            cl_fa_2_gb_1_checkbox_2.Text = "FAp";
            cl_fa_2_gb_1_checkbox_2.CheckedChanged += new EventHandler(cl_fa_2_gb_1_checkbox_2_checkedChanged);
            cl_fa_2_gb_1_checkbox_1.Location = new Point(cl_fa_2_textbox.Left, cl_fa_2_textbox.Top + cl_fa_2_textbox.Height);
            cl_fa_2_gb_1_checkbox_1.Text = "FA";
            cl_fa_2_gb_1_checkbox_1.Checked = true;
            cl_fa_2_gb_1_checkbox_1.CheckedChanged += new EventHandler(cl_fa_2_gb_1_checkbox_1_checkedChanged);






            cl_fa_3_combobox.BringToFront();
            cl_fa_3_textbox.BringToFront();
            cl_fa_3_textbox.Location = new Point(390, 260);
            cl_fa_3_textbox.Width = 200;
            cl_fa_3_textbox.LostFocus += new EventHandler(cl_fa_3_textbox_valueChanged);
            cl_fa_3_textbox.GotFocus += new EventHandler(resetTextBoxBackground);
            toolTip1.SetToolTip(cl_fa_3_textbox, formatting_fa);
            cl_fa_3_combobox.Location = new Point(cl_fa_3_textbox.Left, cl_fa_3_textbox.Top - sepText);
            cl_fa_3_combobox.Width = 200;
            cl_fa_3_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            cl_fa_3_combobox.SelectedIndexChanged += new EventHandler(cl_fa_3_combobox_valueChanged);
            cl_db_3_textbox.Location = new Point(cl_fa_3_textbox.Left + cl_fa_3_textbox.Width + sep, cl_fa_3_textbox.Top);
            cl_db_3_textbox.Width = 150;
            cl_db_3_textbox.LostFocus += new EventHandler(cl_db_3_textbox_valueChanged);
            cl_db_3_textbox.GotFocus += new EventHandler(resetTextBoxBackground);
            toolTip1.SetToolTip(cl_db_3_textbox, formatting_db);
            cl_db_3_label.Location = new Point(cl_db_3_textbox.Left, cl_db_3_textbox.Top - sepText);
            cl_db_3_label.Width = 150;
            cl_db_3_label.Text = "No. of double bonds";

            cl_fa_3_gb_1_checkbox_4.Location = new Point(cl_fa_3_textbox.Left + 140, cl_fa_3_textbox.Top + cl_fa_3_textbox.Height);
            cl_fa_3_gb_1_checkbox_4.Text = "FAh";
            cl_fa_3_gb_1_checkbox_4.CheckedChanged += new EventHandler(cl_fa_3_gb_1_checkbox_4_checkedChanged);
            cl_fa_3_gb_1_checkbox_3.Location = new Point(cl_fa_3_textbox.Left + 90, cl_fa_3_textbox.Top + cl_fa_3_textbox.Height);
            cl_fa_3_gb_1_checkbox_3.Text = "FAe";
            cl_fa_3_gb_1_checkbox_3.CheckedChanged += new EventHandler(cl_fa_3_gb_1_checkbox_3_checkedChanged);
            cl_fa_3_gb_1_checkbox_2.Location = new Point(cl_fa_3_textbox.Left + 40, cl_fa_3_textbox.Top + cl_fa_3_textbox.Height);
            cl_fa_3_gb_1_checkbox_2.Text = "FAp";
            cl_fa_3_gb_1_checkbox_2.CheckedChanged += new EventHandler(cl_fa_3_gb_1_checkbox_2_checkedChanged);
            cl_fa_3_gb_1_checkbox_1.Location = new Point(cl_fa_3_textbox.Left, cl_fa_3_textbox.Top + cl_fa_3_textbox.Height);
            cl_fa_3_gb_1_checkbox_1.Text = "FA";
            cl_fa_3_gb_1_checkbox_1.Checked = true;
            cl_fa_3_gb_1_checkbox_1.CheckedChanged += new EventHandler(cl_fa_3_gb_1_checkbox_1_checkedChanged);





            cl_fa_4_combobox.BringToFront();
            cl_fa_4_textbox.BringToFront();
            cl_fa_4_textbox.Location = new Point(312, 346);
            cl_fa_4_textbox.Width = 200;
            cl_fa_4_textbox.LostFocus += new EventHandler(cl_fa_4_textbox_valueChanged);
            cl_fa_4_textbox.GotFocus += new EventHandler(resetTextBoxBackground);
            toolTip1.SetToolTip(cl_fa_4_textbox, formatting_fa);
            cl_fa_4_combobox.Location = new Point(cl_fa_4_textbox.Left, cl_fa_4_textbox.Top - sepText);
            cl_fa_4_combobox.Width = 200;
            cl_fa_4_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            cl_fa_4_combobox.SelectedIndexChanged += new EventHandler(cl_fa_4_combobox_valueChanged);
            cl_db_4_textbox.Location = new Point(cl_fa_4_textbox.Left + cl_fa_4_textbox.Width + sep, cl_fa_4_textbox.Top);
            cl_db_4_textbox.Width = 150;
            cl_db_4_textbox.LostFocus += new EventHandler(cl_db_4_textbox_valueChanged);
            cl_db_4_textbox.GotFocus += new EventHandler(resetTextBoxBackground);
            toolTip1.SetToolTip(cl_db_4_textbox, formatting_db);
            cl_db_4_label.Location = new Point(cl_db_4_textbox.Left, cl_db_4_textbox.Top - sepText);
            cl_db_4_label.Width = 150;
            cl_db_4_label.Text = "No. of double bonds";

            cl_fa_4_gb_1_checkbox_4.Location = new Point(cl_fa_4_textbox.Left + 140, cl_fa_4_textbox.Top + cl_fa_4_textbox.Height);
            cl_fa_4_gb_1_checkbox_4.Text = "FAh";
            cl_fa_4_gb_1_checkbox_4.CheckedChanged += new EventHandler(cl_fa_4_gb_1_checkbox_4_checkedChanged);
            cl_fa_4_gb_1_checkbox_3.Location = new Point(cl_fa_4_textbox.Left + 90, cl_fa_4_textbox.Top + cl_fa_4_textbox.Height);
            cl_fa_4_gb_1_checkbox_3.Text = "FAe";
            cl_fa_4_gb_1_checkbox_3.CheckedChanged += new EventHandler(cl_fa_4_gb_1_checkbox_3_checkedChanged);
            cl_fa_4_gb_1_checkbox_2.Location = new Point(cl_fa_4_textbox.Left + 40, cl_fa_4_textbox.Top + cl_fa_4_textbox.Height);
            cl_fa_4_gb_1_checkbox_2.Text = "FAp";
            cl_fa_4_gb_1_checkbox_2.CheckedChanged += new EventHandler(cl_fa_4_gb_1_checkbox_2_checkedChanged);
            cl_fa_4_gb_1_checkbox_1.Location = new Point(cl_fa_4_textbox.Left, cl_fa_4_textbox.Top + cl_fa_4_textbox.Height);
            cl_fa_4_gb_1_checkbox_1.Text = "FA";
            cl_fa_4_gb_1_checkbox_1.Checked = true;
            cl_fa_4_gb_1_checkbox_1.CheckedChanged += new EventHandler(cl_fa_4_gb_1_checkbox_1_checkedChanged);



            // tab for glycerolipids
            glycerolipids_tab.Controls.Add(gl_add_lipid_button);
            glycerolipids_tab.Controls.Add(gl_reset_lipid_button);
            glycerolipids_tab.Controls.Add(gl_modify_lipid_button);
            glycerolipids_tab.Controls.Add(gl_ms2fragments_lipid_button);
            glycerolipids_tab.Controls.Add(gl_fa_1_gb_1_checkbox_4);
            glycerolipids_tab.Controls.Add(gl_fa_1_gb_1_checkbox_3);
            glycerolipids_tab.Controls.Add(gl_fa_1_gb_1_checkbox_2);
            glycerolipids_tab.Controls.Add(gl_fa_1_gb_1_checkbox_1);
            glycerolipids_tab.Controls.Add(gl_fa_2_gb_1_checkbox_4);
            glycerolipids_tab.Controls.Add(gl_fa_2_gb_1_checkbox_3);
            glycerolipids_tab.Controls.Add(gl_fa_2_gb_1_checkbox_2);
            glycerolipids_tab.Controls.Add(gl_fa_2_gb_1_checkbox_1);
            glycerolipids_tab.Controls.Add(gl_fa_3_gb_1_checkbox_4);
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
            glycerolipids_tab.Controls.Add(gl_fa_1_combobox);
            glycerolipids_tab.Controls.Add(gl_fa_2_combobox);
            glycerolipids_tab.Controls.Add(gl_fa_3_combobox);
            glycerolipids_tab.Controls.Add(gl_db_1_label);
            glycerolipids_tab.Controls.Add(gl_db_2_label);
            glycerolipids_tab.Controls.Add(gl_db_3_label);
            glycerolipids_tab.Controls.Add(gl_positive_adduct);
            glycerolipids_tab.Controls.Add(gl_negative_adduct);
            glycerolipids_tab.Parent = tab_control;
            glycerolipids_tab.Text = "Glycerolipids";
            glycerolipids_tab.Location = new Point(0, 0);
            glycerolipids_tab.Size = this.Size;
            glycerolipids_tab.AutoSize = true;
            glycerolipids_tab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            glycerolipids_tab.BackColor = Color.White;


            gl_fa_1_combobox.BringToFront();
            gl_fa_1_textbox.BringToFront();
            gl_fa_1_textbox.Location = new Point(240, 90);
            gl_fa_1_textbox.Width = 200;
            gl_fa_1_textbox.Text = "0, 2, 4, 6-7";
            gl_fa_1_textbox.TextChanged += new EventHandler(gl_fa_1_textbox_valueChanged);
            toolTip1.SetToolTip(gl_fa_1_textbox, formatting_fa);
            gl_fa_1_combobox.Location = new Point(gl_fa_1_textbox.Left, gl_fa_1_textbox.Top - sepText);
            gl_fa_1_combobox.Width = 200;
            gl_fa_1_combobox.SelectedItem = "Fatty acid chain";
            gl_fa_1_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            gl_fa_1_combobox.SelectedIndexChanged += new EventHandler(gl_fa_1_combobox_valueChanged);
            gl_db_1_textbox.Location = new Point(gl_fa_1_textbox.Left + gl_fa_1_textbox.Width + sep, gl_fa_1_textbox.Top);
            gl_db_1_textbox.Width = 150;
            gl_db_1_textbox.Text = "0-2";
            gl_db_1_textbox.TextChanged += new EventHandler(gl_db_1_textbox_valueChanged);
            toolTip1.SetToolTip(gl_db_1_textbox, formatting_db);
            gl_db_1_label.Location = new Point(gl_db_1_textbox.Left, gl_db_1_textbox.Top - sep);
            gl_db_1_label.Width = 150;
            gl_db_1_label.Text = "No. of double bonds";

            gl_fa_1_gb_1_checkbox_4.Location = new Point(gl_fa_1_textbox.Left + 140, gl_fa_1_textbox.Top + gl_fa_1_textbox.Height);
            gl_fa_1_gb_1_checkbox_4.Text = "FAh";
            gl_fa_1_gb_1_checkbox_4.CheckedChanged += new EventHandler(gl_fa_1_gb_1_checkbox_4_checkedChanged);
            gl_fa_1_gb_1_checkbox_3.Location = new Point(gl_fa_1_textbox.Left + 90, gl_fa_1_textbox.Top + gl_fa_1_textbox.Height);
            gl_fa_1_gb_1_checkbox_3.Text = "FAe";
            gl_fa_1_gb_1_checkbox_3.CheckedChanged += new EventHandler(gl_fa_1_gb_1_checkbox_3_checkedChanged);
            gl_fa_1_gb_1_checkbox_2.Location = new Point(gl_fa_1_textbox.Left + 40, gl_fa_1_textbox.Top + gl_fa_1_textbox.Height);
            gl_fa_1_gb_1_checkbox_2.Text = "FAp";
            gl_fa_1_gb_1_checkbox_2.CheckedChanged += new EventHandler(gl_fa_1_gb_1_checkbox_2_checkedChanged);
            gl_fa_1_gb_1_checkbox_1.Location = new Point(gl_fa_1_textbox.Left, gl_fa_1_textbox.Top + gl_fa_1_textbox.Height);
            gl_fa_1_gb_1_checkbox_1.Text = "FA";
            gl_fa_1_gb_1_checkbox_1.Checked = true;
            gl_fa_1_gb_1_checkbox_1.CheckedChanged += new EventHandler(gl_fa_1_gb_1_checkbox_1_checkedChanged);

            gl_fa_2_combobox.BringToFront();
            gl_fa_2_textbox.BringToFront();
            gl_fa_2_textbox.Location = new Point(298, 186);
            gl_fa_2_textbox.Width = 200;
            gl_fa_2_textbox.Text = "0, 5, 17-19";
            gl_fa_2_textbox.TextChanged += new EventHandler(gl_fa_2_textbox_valueChanged);
            toolTip1.SetToolTip(gl_fa_2_textbox, formatting_fa);
            gl_fa_2_combobox.Location = new Point(gl_fa_2_textbox.Left, gl_fa_2_textbox.Top - sepText);
            gl_fa_2_combobox.Width = 200;
            gl_fa_2_combobox.SelectedItem = "Fatty acid chain";
            gl_fa_2_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            gl_fa_2_combobox.SelectedIndexChanged += new EventHandler(gl_fa_2_combobox_valueChanged);
            gl_db_2_textbox.Location = new Point(gl_fa_2_textbox.Left + gl_fa_2_textbox.Width + sep, gl_fa_2_textbox.Top);
            gl_db_2_textbox.Width = 150;
            gl_db_2_textbox.Text = "5-6";
            gl_db_2_textbox.TextChanged += new EventHandler(gl_db_2_textbox_valueChanged);
            toolTip1.SetToolTip(gl_db_2_textbox, formatting_db);
            gl_db_2_label.Location = new Point(gl_db_2_textbox.Left, gl_db_2_textbox.Top - sep);
            gl_db_2_label.Width = 150;
            gl_db_2_label.Text = "No. of double bonds";

            gl_fa_2_gb_1_checkbox_4.Location = new Point(gl_fa_2_textbox.Left + 140, gl_fa_2_textbox.Top + gl_fa_2_textbox.Height);
            gl_fa_2_gb_1_checkbox_4.Text = "FAh";
            gl_fa_2_gb_1_checkbox_4.CheckedChanged += new EventHandler(gl_fa_2_gb_1_checkbox_4_checkedChanged);
            gl_fa_2_gb_1_checkbox_3.Location = new Point(gl_fa_2_textbox.Left + 90, gl_fa_2_textbox.Top + gl_fa_2_textbox.Height);
            gl_fa_2_gb_1_checkbox_3.Text = "FAe";
            gl_fa_2_gb_1_checkbox_3.CheckedChanged += new EventHandler(gl_fa_2_gb_1_checkbox_3_checkedChanged);
            gl_fa_2_gb_1_checkbox_2.Location = new Point(gl_fa_2_textbox.Left + 40, gl_fa_2_textbox.Top + gl_fa_2_textbox.Height);
            gl_fa_2_gb_1_checkbox_2.Text = "FAp";
            gl_fa_2_gb_1_checkbox_2.CheckedChanged += new EventHandler(gl_fa_2_gb_1_checkbox_2_checkedChanged);
            gl_fa_2_gb_1_checkbox_1.Location = new Point(gl_fa_2_textbox.Left, gl_fa_2_textbox.Top + gl_fa_2_textbox.Height);
            gl_fa_2_gb_1_checkbox_1.Text = "FA";
            gl_fa_2_gb_1_checkbox_1.Checked = true;
            gl_fa_2_gb_1_checkbox_1.CheckedChanged += new EventHandler(gl_fa_2_gb_1_checkbox_1_checkedChanged);

            gl_fa_3_combobox.BringToFront();
            gl_fa_3_textbox.BringToFront();
            gl_fa_3_textbox.Location = new Point(238, 290);
            gl_fa_3_textbox.Width = 200;
            gl_fa_3_textbox.Text = "20-22";
            gl_fa_3_textbox.TextChanged += new EventHandler(gl_fa_3_textbox_valueChanged);
            toolTip1.SetToolTip(gl_fa_3_textbox, formatting_fa);
            gl_fa_3_combobox.Location = new Point(gl_fa_3_textbox.Left, gl_fa_3_textbox.Top - sepText);
            gl_fa_3_combobox.Width = 200;
            gl_fa_3_combobox.SelectedItem = "Fatty acid chain";
            gl_fa_3_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            gl_fa_3_combobox.SelectedIndexChanged += new EventHandler(gl_fa_3_combobox_valueChanged);
            gl_db_3_textbox.Location = new Point(gl_fa_3_textbox.Left + gl_fa_3_textbox.Width + sep, gl_fa_3_textbox.Top);
            gl_db_3_textbox.Width = 150;
            gl_db_3_textbox.Text = "0";
            gl_db_3_textbox.TextChanged += new EventHandler(gl_db_3_textbox_valueChanged);
            toolTip1.SetToolTip(gl_db_3_textbox, formatting_db);
            gl_db_3_label.Location = new Point(gl_db_3_textbox.Left, gl_db_3_textbox.Top - sep);
            gl_db_3_label.Width = 150;
            gl_db_3_label.Text = "No. of double bonds";

            gl_fa_3_gb_1_checkbox_4.Location = new Point(gl_fa_3_textbox.Left + 140, gl_fa_3_textbox.Top + gl_fa_3_textbox.Height);
            gl_fa_3_gb_1_checkbox_4.Text = "FAh";
            gl_fa_3_gb_1_checkbox_4.CheckedChanged += new EventHandler(gl_fa_3_gb_1_checkbox_4_checkedChanged);
            gl_fa_3_gb_1_checkbox_3.Location = new Point(gl_fa_3_textbox.Left + 90, gl_fa_3_textbox.Top + gl_fa_3_textbox.Height);
            gl_fa_3_gb_1_checkbox_3.Text = "FAe";
            gl_fa_3_gb_1_checkbox_3.CheckedChanged += new EventHandler(gl_fa_3_gb_1_checkbox_3_checkedChanged);
            gl_fa_3_gb_1_checkbox_2.Location = new Point(gl_fa_3_textbox.Left + 40, gl_fa_3_textbox.Top + gl_fa_3_textbox.Height);
            gl_fa_3_gb_1_checkbox_2.Text = "FAp";
            gl_fa_3_gb_1_checkbox_2.CheckedChanged += new EventHandler(gl_fa_3_gb_1_checkbox_2_checkedChanged);
            gl_fa_3_gb_1_checkbox_1.Location = new Point(gl_fa_3_textbox.Left, gl_fa_3_textbox.Top + gl_fa_3_textbox.Height);
            gl_fa_3_gb_1_checkbox_1.Text = "FA";
            gl_fa_3_gb_1_checkbox_1.Checked = true;
            gl_fa_3_gb_1_checkbox_1.CheckedChanged += new EventHandler(gl_fa_3_gb_1_checkbox_1_checkedChanged);


            gl_positive_adduct.Location = new Point(800, 60);
            gl_positive_adduct.Width = 120;
            gl_positive_adduct.Height = 120;
            gl_positive_adduct.Text = "Positive adducts";
            gl_pos_adduct_checkbox_1.Parent = gl_positive_adduct;
            gl_pos_adduct_checkbox_1.Location = new Point(10, 15);
            gl_pos_adduct_checkbox_1.Text = "+H";
            gl_pos_adduct_checkbox_1.Checked = true;
            gl_pos_adduct_checkbox_1.CheckedChanged += new EventHandler(gl_pos_adduct_checkbox_1_checkedChanged);
            gl_pos_adduct_checkbox_2.Parent = gl_positive_adduct;
            gl_pos_adduct_checkbox_2.Location = new Point(10, 35);
            gl_pos_adduct_checkbox_2.Text = "+2H";
            gl_pos_adduct_checkbox_2.CheckedChanged += new EventHandler(gl_pos_adduct_checkbox_2_checkedChanged);
            gl_pos_adduct_checkbox_3.Parent = gl_positive_adduct;
            gl_pos_adduct_checkbox_3.Location = new Point(10, 55);
            gl_pos_adduct_checkbox_3.Text = "+NH4";
            gl_pos_adduct_checkbox_3.CheckedChanged += new EventHandler(gl_pos_adduct_checkbox_3_checkedChanged);
            gl_pos_adduct_checkbox_4.Parent = gl_positive_adduct;
            gl_pos_adduct_checkbox_4.Location = new Point(10, 75);
            gl_pos_adduct_checkbox_4.Text = "+Na";
            gl_pos_adduct_checkbox_4.CheckedChanged += new EventHandler(gl_pos_adduct_checkbox_4_checkedChanged);
            gl_negative_adduct.Location = new Point(800, 200);
            gl_negative_adduct.Width = 120;
            gl_negative_adduct.Height = 120;
            gl_negative_adduct.Text = "Negative adducts";
            gl_neg_adduct_checkbox_1.Parent = gl_negative_adduct;
            gl_neg_adduct_checkbox_1.Location = new Point(10, 15);
            gl_neg_adduct_checkbox_1.Text = "-H";
            gl_neg_adduct_checkbox_1.CheckedChanged += new EventHandler(gl_neg_adduct_checkbox_1_checkedChanged);
            gl_neg_adduct_checkbox_2.Parent = gl_negative_adduct;
            gl_neg_adduct_checkbox_2.Location = new Point(10, 35);
            gl_neg_adduct_checkbox_2.Text = "-2H";
            gl_neg_adduct_checkbox_2.CheckedChanged += new EventHandler(gl_neg_adduct_checkbox_2_checkedChanged);
            gl_neg_adduct_checkbox_3.Parent = gl_negative_adduct;
            gl_neg_adduct_checkbox_3.Location = new Point(10, 55);
            gl_neg_adduct_checkbox_3.Text = "+HCOO";
            gl_neg_adduct_checkbox_3.CheckedChanged += new EventHandler(gl_neg_adduct_checkbox_3_checkedChanged);
            gl_neg_adduct_checkbox_4.Parent = gl_negative_adduct;
            gl_neg_adduct_checkbox_4.Location = new Point(10, 75);
            gl_neg_adduct_checkbox_4.Text = "+CH3COO";
            gl_neg_adduct_checkbox_4.CheckedChanged += new EventHandler(gl_neg_adduct_checkbox_4_checkedChanged);

            gl_picture_box.Image = glycero_backbone_image;
            gl_picture_box.Location = new Point((int)(214 - glycero_backbone_image.Width * 0.5), (int)(204 - glycero_backbone_image.Height * 0.5));
            gl_picture_box.SizeMode = PictureBoxSizeMode.AutoSize;




            // tab for phospholipids
            phospholipids_tab.Controls.Add(pl_add_lipid_button);
            phospholipids_tab.Controls.Add(pl_reset_lipid_button);
            phospholipids_tab.Controls.Add(pl_modify_lipid_button);
            phospholipids_tab.Controls.Add(pl_ms2fragments_lipid_button);
            phospholipids_tab.Controls.Add(pl_fa_1_gb_1_checkbox_4);
            phospholipids_tab.Controls.Add(pl_fa_1_gb_1_checkbox_3);
            phospholipids_tab.Controls.Add(pl_fa_1_gb_1_checkbox_2);
            phospholipids_tab.Controls.Add(pl_fa_1_gb_1_checkbox_1);
            phospholipids_tab.Controls.Add(pl_fa_2_gb_1_checkbox_4);
            phospholipids_tab.Controls.Add(pl_fa_2_gb_1_checkbox_3);
            phospholipids_tab.Controls.Add(pl_fa_2_gb_1_checkbox_2);
            phospholipids_tab.Controls.Add(pl_fa_2_gb_1_checkbox_1);
            phospholipids_tab.Controls.Add(pl_picture_box);
            phospholipids_tab.Controls.Add(pl_fa_1_textbox);
            phospholipids_tab.Controls.Add(pl_fa_2_textbox);
            phospholipids_tab.Controls.Add(pl_db_1_textbox);
            phospholipids_tab.Controls.Add(pl_db_2_textbox);
            phospholipids_tab.Controls.Add(pl_fa_1_combobox);
            phospholipids_tab.Controls.Add(pl_fa_2_combobox);
            phospholipids_tab.Controls.Add(pl_db_1_label);
            phospholipids_tab.Controls.Add(pl_db_2_label);
            phospholipids_tab.Controls.Add(pl_hg_label);
            phospholipids_tab.Controls.Add(pl_hg_combobox);
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
            pl_fa_1_textbox.Location = new Point(298, 186);
            pl_fa_1_textbox.Width = 200;
            pl_fa_1_textbox.Text = "0, 2, 4, 6-7";
            pl_fa_1_textbox.TextChanged += new EventHandler(pl_fa_1_textbox_valueChanged);
            toolTip1.SetToolTip(pl_fa_1_textbox, formatting_fa);
            pl_fa_1_combobox.Location = new Point(pl_fa_1_textbox.Left, pl_fa_1_textbox.Top - sepText);
            pl_fa_1_combobox.Width = 200;
            pl_fa_1_combobox.SelectedItem = "Fatty acid chain";
            pl_fa_1_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            pl_fa_1_combobox.SelectedIndexChanged += new EventHandler(pl_fa_1_combobox_valueChanged);
            pl_db_1_textbox.Location = new Point(pl_fa_1_textbox.Left + pl_fa_1_textbox.Width + sep, pl_fa_1_textbox.Top);
            pl_db_1_textbox.Width = 150;
            pl_db_1_textbox.Text = "0-2";
            pl_db_1_textbox.TextChanged += new EventHandler(pl_db_1_textbox_valueChanged);
            toolTip1.SetToolTip(pl_db_1_textbox, formatting_db);
            pl_db_1_label.Location = new Point(pl_db_1_textbox.Left, pl_db_1_textbox.Top - sep);
            pl_db_1_label.Width = 150;
            pl_db_1_label.Text = "No. of double bonds";

            pl_fa_1_gb_1_checkbox_4.Location = new Point(pl_fa_1_textbox.Left + 140, pl_fa_1_textbox.Top + pl_fa_1_textbox.Height);
            pl_fa_1_gb_1_checkbox_4.Text = "FAh";
            pl_fa_1_gb_1_checkbox_4.CheckedChanged += new EventHandler(pl_fa_1_gb_1_checkbox_4_checkedChanged);
            pl_fa_1_gb_1_checkbox_3.Location = new Point(pl_fa_1_textbox.Left + 90, pl_fa_1_textbox.Top + pl_fa_1_textbox.Height);
            pl_fa_1_gb_1_checkbox_3.Text = "FAe";
            pl_fa_1_gb_1_checkbox_3.CheckedChanged += new EventHandler(pl_fa_1_gb_1_checkbox_3_checkedChanged);
            pl_fa_1_gb_1_checkbox_2.Location = new Point(pl_fa_1_textbox.Left + 40, pl_fa_1_textbox.Top + pl_fa_1_textbox.Height);
            pl_fa_1_gb_1_checkbox_2.Text = "FAp";
            pl_fa_1_gb_1_checkbox_2.CheckedChanged += new EventHandler(pl_fa_1_gb_1_checkbox_2_checkedChanged);
            pl_fa_1_gb_1_checkbox_1.Location = new Point(pl_fa_1_textbox.Left, pl_fa_1_textbox.Top + pl_fa_1_textbox.Height);
            pl_fa_1_gb_1_checkbox_1.Text = "FA";
            pl_fa_1_gb_1_checkbox_1.Checked = true;
            pl_fa_1_gb_1_checkbox_1.CheckedChanged += new EventHandler(pl_fa_1_gb_1_checkbox_1_checkedChanged);

            pl_fa_2_combobox.BringToFront();
            pl_fa_2_textbox.BringToFront();
            pl_fa_2_textbox.Location = new Point(238, 290);
            pl_fa_2_textbox.Width = 200;
            pl_fa_2_textbox.Text = "2, 5, 17-19";
            pl_fa_2_textbox.TextChanged += new EventHandler(pl_fa_2_textbox_valueChanged);
            toolTip1.SetToolTip(pl_fa_2_textbox, formatting_fa);
            pl_fa_2_combobox.Location = new Point(pl_fa_2_textbox.Left, pl_fa_2_textbox.Top - sepText);
            pl_fa_2_combobox.Width = 200;
            pl_fa_2_combobox.SelectedItem = "Fatty acid chain";
            pl_fa_2_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            pl_fa_2_combobox.SelectedIndexChanged += new EventHandler(pl_fa_2_combobox_valueChanged);
            pl_db_2_textbox.Location = new Point(pl_fa_2_textbox.Left + pl_fa_2_textbox.Width + sep, pl_fa_2_textbox.Top);
            pl_db_2_textbox.Width = 150;
            pl_db_2_textbox.Text = "5-6";
            pl_db_2_textbox.TextChanged += new EventHandler(pl_db_2_textbox_valueChanged);
            toolTip1.SetToolTip(pl_db_2_textbox, formatting_db);
            pl_db_2_label.Location = new Point(pl_db_2_textbox.Left, pl_db_2_textbox.Top - sep);
            pl_db_2_label.Width = 150;
            pl_db_2_label.Text = "No. of double bonds";

            pl_fa_2_gb_1_checkbox_4.Location = new Point(pl_fa_2_textbox.Left + 140, pl_fa_2_textbox.Top + pl_fa_2_textbox.Height);
            pl_fa_2_gb_1_checkbox_4.Text = "FAh";
            pl_fa_2_gb_1_checkbox_4.CheckedChanged += new EventHandler(pl_fa_2_gb_1_checkbox_4_checkedChanged);
            pl_fa_2_gb_1_checkbox_3.Location = new Point(pl_fa_2_textbox.Left + 90, pl_fa_2_textbox.Top + pl_fa_2_textbox.Height);
            pl_fa_2_gb_1_checkbox_3.Text = "FAe";
            pl_fa_2_gb_1_checkbox_3.CheckedChanged += new EventHandler(pl_fa_2_gb_1_checkbox_3_checkedChanged);
            pl_fa_2_gb_1_checkbox_2.Location = new Point(pl_fa_2_textbox.Left + 40, pl_fa_2_textbox.Top + pl_fa_2_textbox.Height);
            pl_fa_2_gb_1_checkbox_2.Text = "FAp";
            pl_fa_2_gb_1_checkbox_2.CheckedChanged += new EventHandler(pl_fa_2_gb_1_checkbox_2_checkedChanged);
            pl_fa_2_gb_1_checkbox_1.Location = new Point(pl_fa_2_textbox.Left, pl_fa_2_textbox.Top + pl_fa_2_textbox.Height);
            pl_fa_2_gb_1_checkbox_1.Text = "FA";
            pl_fa_2_gb_1_checkbox_1.Checked = true;
            pl_fa_2_gb_1_checkbox_1.CheckedChanged += new EventHandler(pl_fa_2_gb_1_checkbox_1_checkedChanged);

            pl_hg_label.BringToFront();
            pl_hg_combobox.BringToFront();
            pl_hg_combobox.Location = new Point(240, 92);
            pl_hg_combobox.SelectedItem = "PA";
            pl_hg_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            pl_hg_combobox.SelectedIndexChanged += new EventHandler(pl_hg_combobox_valueChanged);
            pl_hg_label.Location = new Point(pl_hg_combobox.Left, pl_hg_combobox.Top - sep);
            pl_hg_label.Text = "Head group";

            pl_positive_adduct.Location = new Point(800, 60);
            pl_positive_adduct.Width = 120;
            pl_positive_adduct.Height = 120;
            pl_positive_adduct.Text = "Positive adducts";
            pl_pos_adduct_checkbox_1.Parent = pl_positive_adduct;
            pl_pos_adduct_checkbox_1.Location = new Point(10, 15);
            pl_pos_adduct_checkbox_1.Text = "+H";
            pl_pos_adduct_checkbox_1.Checked = true;
            pl_pos_adduct_checkbox_1.CheckedChanged += new EventHandler(pl_pos_adduct_checkbox_1_checkedChanged);
            pl_pos_adduct_checkbox_2.Parent = pl_positive_adduct;
            pl_pos_adduct_checkbox_2.Location = new Point(10, 35);
            pl_pos_adduct_checkbox_2.Text = "+2H";
            pl_pos_adduct_checkbox_2.CheckedChanged += new EventHandler(pl_pos_adduct_checkbox_2_checkedChanged);
            pl_pos_adduct_checkbox_3.Parent = pl_positive_adduct;
            pl_pos_adduct_checkbox_3.Location = new Point(10, 55);
            pl_pos_adduct_checkbox_3.Text = "+NH4";
            pl_pos_adduct_checkbox_3.CheckedChanged += new EventHandler(pl_pos_adduct_checkbox_3_checkedChanged);
            pl_pos_adduct_checkbox_4.Parent = pl_positive_adduct;
            pl_pos_adduct_checkbox_4.Location = new Point(10, 75);
            pl_pos_adduct_checkbox_4.Text = "+Na";
            pl_pos_adduct_checkbox_4.CheckedChanged += new EventHandler(pl_pos_adduct_checkbox_4_checkedChanged);
            pl_negative_adduct.Location = new Point(800, 200);
            pl_negative_adduct.Width = 120;
            pl_negative_adduct.Height = 120;
            pl_negative_adduct.Text = "Negative adducts";
            pl_neg_adduct_checkbox_1.Parent = pl_negative_adduct;
            pl_neg_adduct_checkbox_1.Location = new Point(10, 15);
            pl_neg_adduct_checkbox_1.Text = "-H";
            pl_neg_adduct_checkbox_1.CheckedChanged += new EventHandler(pl_neg_adduct_checkbox_1_checkedChanged);
            pl_neg_adduct_checkbox_2.Parent = pl_negative_adduct;
            pl_neg_adduct_checkbox_2.Location = new Point(10, 35);
            pl_neg_adduct_checkbox_2.Text = "-2H";
            pl_neg_adduct_checkbox_2.CheckedChanged += new EventHandler(pl_neg_adduct_checkbox_2_checkedChanged);
            pl_neg_adduct_checkbox_3.Parent = pl_negative_adduct;
            pl_neg_adduct_checkbox_3.Location = new Point(10, 55);
            pl_neg_adduct_checkbox_3.Text = "+HCOO";
            pl_neg_adduct_checkbox_3.CheckedChanged += new EventHandler(pl_neg_adduct_checkbox_3_checkedChanged);
            pl_neg_adduct_checkbox_4.Parent = pl_negative_adduct;
            pl_neg_adduct_checkbox_4.Location = new Point(10, 75);
            pl_neg_adduct_checkbox_4.Text = "+CH3COO";
            pl_neg_adduct_checkbox_4.CheckedChanged += new EventHandler(pl_neg_adduct_checkbox_4_checkedChanged);


            pl_picture_box.Image = phospho_backbone_image;
            pl_picture_box.Location = new Point((int)(214 - phospho_backbone_image.Width * 0.5), (int)(204 - phospho_backbone_image.Height * 0.5));
            pl_picture_box.SizeMode = PictureBoxSizeMode.AutoSize;



            // tab for sphingolipids
            sphingolipids_tab.Controls.Add(sl_add_lipid_button);
            sphingolipids_tab.Controls.Add(sl_reset_lipid_button);
            sphingolipids_tab.Controls.Add(sl_modify_lipid_button);
            sphingolipids_tab.Controls.Add(sl_ms2fragments_lipid_button);
            sphingolipids_tab.Controls.Add(sl_fa_gb_1_checkbox_4);
            sphingolipids_tab.Controls.Add(sl_fa_gb_1_checkbox_3);
            sphingolipids_tab.Controls.Add(sl_fa_gb_1_checkbox_2);
            sphingolipids_tab.Controls.Add(sl_fa_gb_1_checkbox_1);
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
            sphingolipids_tab.Controls.Add(sl_hg_combobox);
            sphingolipids_tab.Controls.Add(sl_hydroxy_combobox);
            sphingolipids_tab.Controls.Add(sl_hydroxy_label);
            sphingolipids_tab.Controls.Add(sl_positive_adduct);
            sphingolipids_tab.Controls.Add(sl_negative_adduct);
            sphingolipids_tab.Parent = tab_control;
            sphingolipids_tab.Text = "Sphingolipids";
            sphingolipids_tab.Location = new Point(0, 0);
            sphingolipids_tab.Size = this.Size;
            sphingolipids_tab.AutoSize = true;
            sphingolipids_tab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            sphingolipids_tab.BackColor = Color.White;

            sl_fa_combobox.BringToFront();
            sl_fa_textbox.BringToFront();
            sl_fa_textbox.Location = new Point(242, 272);
            sl_fa_textbox.Width = 200;
            sl_fa_textbox.Text = "2, 5, 17-19";
            sl_fa_textbox.TextChanged += new EventHandler(sl_fa_textbox_valueChanged);
            toolTip1.SetToolTip(sl_fa_textbox, formatting_fa);
            sl_fa_combobox.Location = new Point(sl_fa_textbox.Left, sl_fa_textbox.Top - sepText);
            sl_fa_combobox.Width = 200;
            sl_fa_combobox.SelectedItem = "Fatty acid chain";
            sl_fa_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            sl_fa_combobox.SelectedIndexChanged += new EventHandler(sl_fa_combobox_valueChanged);
            sl_db_1_textbox.Location = new Point(sl_fa_textbox.Left + sl_fa_textbox.Width + sep, sl_fa_textbox.Top);
            sl_db_1_textbox.Width = 150;
            sl_db_1_textbox.Text = "5-6";
            sl_db_1_textbox.TextChanged += new EventHandler(sl_db_1_textbox_valueChanged);
            toolTip1.SetToolTip(sl_db_1_textbox, formatting_db);
            sl_db_1_label.Location = new Point(sl_db_1_textbox.Left, sl_db_1_textbox.Top - sep);
            sl_db_1_label.Width = 150;
            sl_db_1_label.Text = "No. of double bonds";

            sl_fa_gb_1_checkbox_1.Location = new Point(sl_fa_textbox.Left, sl_fa_textbox.Top + sl_fa_textbox.Height);
            sl_fa_gb_1_checkbox_1.Text = "FA";
            sl_fa_gb_1_checkbox_1.Checked = true;
            sl_fa_gb_1_checkbox_1.CheckedChanged += new EventHandler(sl_fa_gb_1_checkbox_1_checkedChanged);
            sl_fa_gb_1_checkbox_2.Location = new Point(sl_fa_textbox.Left + 40, sl_fa_textbox.Top + sl_fa_textbox.Height);
            sl_fa_gb_1_checkbox_2.Text = "FAp";
            sl_fa_gb_1_checkbox_2.CheckedChanged += new EventHandler(sl_fa_gb_1_checkbox_2_checkedChanged);
            sl_fa_gb_1_checkbox_3.Location = new Point(sl_fa_textbox.Left + 90, sl_fa_textbox.Top + sl_fa_textbox.Height);
            sl_fa_gb_1_checkbox_3.Text = "FAe";
            sl_fa_gb_1_checkbox_3.CheckedChanged += new EventHandler(sl_fa_gb_1_checkbox_3_checkedChanged);
            sl_fa_gb_1_checkbox_4.Location = new Point(sl_fa_textbox.Left + 140, sl_fa_textbox.Top + sl_fa_textbox.Height);
            sl_fa_gb_1_checkbox_4.Text = "FAh";
            sl_fa_gb_1_checkbox_4.CheckedChanged += new EventHandler(sl_fa_gb_1_checkbox_4_checkedChanged);

            sl_lcb_combobox.BringToFront();
            sl_lcb_textbox.BringToFront();
            sl_hydroxy_combobox.BringToFront();
            sl_lcb_textbox.Location = new Point(280, 198);
            sl_lcb_textbox.Width = 200;
            sl_lcb_textbox.Text = "14, 16-18, 22";
            sl_lcb_textbox.TextChanged += new EventHandler(sl_lcb_textbox_valueChanged);
            toolTip1.SetToolTip(sl_lcb_textbox, formatting_fa);
            sl_lcb_combobox.Location = new Point(sl_lcb_textbox.Left, sl_lcb_textbox.Top - sepText);
            sl_lcb_combobox.Width = 200;
            sl_lcb_combobox.SelectedItem = "Long chain base";
            sl_lcb_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            sl_lcb_combobox.SelectedIndexChanged += new EventHandler(sl_lcb_combobox_valueChanged);
            sl_db_2_textbox.Location = new Point(sl_lcb_textbox.Left + sl_lcb_textbox.Width + sep, sl_lcb_textbox.Top);
            sl_db_2_textbox.Width = 150;
            sl_db_2_textbox.Text = "0-2";
            sl_db_2_textbox.TextChanged += new EventHandler(sl_db_2_textbox_valueChanged);
            toolTip1.SetToolTip(sl_db_2_textbox, formatting_db);
            sl_db_2_label.Location = new Point(sl_db_2_textbox.Left, sl_db_2_textbox.Top - sep);
            sl_db_2_label.Width = 150;
            sl_db_2_label.Text = "No. of double bonds";
            sl_hydroxy_combobox.Location = new Point(sl_db_2_textbox.Left + sl_db_2_textbox.Width + sep, sl_db_2_textbox.Top);
            sl_hydroxy_combobox.SelectedItem = "2";
            sl_hydroxy_combobox.Width = 60;
            sl_hydroxy_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            sl_hydroxy_combobox.SelectedIndexChanged += new EventHandler(sl_hydroxy_combobox_valueChanged);
            sl_hydroxy_label.Location = new Point(sl_hydroxy_combobox.Left, sl_hydroxy_combobox.Top - sep);
            sl_hydroxy_label.Text = "Hydroxy No.";

            sl_hg_label.BringToFront();
            sl_hg_combobox.BringToFront();
            sl_hg_combobox.Location = new Point(212, 112);
            sl_hg_combobox.SelectedItem = "Cer";
            sl_hg_combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            sl_hg_combobox.SelectedIndexChanged += new EventHandler(sl_hg_combobox_valueChanged);
            sl_hg_label.Location = new Point(sl_hg_combobox.Left, sl_hg_combobox.Top - sep);
            sl_hg_label.Text = "Head group";

            sl_positive_adduct.Location = new Point(800, 60);
            sl_positive_adduct.Width = 120;
            sl_positive_adduct.Height = 120;
            sl_positive_adduct.Text = "Positive adducts";
            sl_pos_adduct_checkbox_1.Parent = sl_positive_adduct;
            sl_pos_adduct_checkbox_1.Location = new Point(10, 15);
            sl_pos_adduct_checkbox_1.Text = "+H";
            sl_pos_adduct_checkbox_1.Checked = true;
            sl_pos_adduct_checkbox_1.CheckedChanged += new EventHandler(sl_pos_adduct_checkbox_1_checkedChanged);
            sl_pos_adduct_checkbox_2.Parent = sl_positive_adduct;
            sl_pos_adduct_checkbox_2.Location = new Point(10, 35);
            sl_pos_adduct_checkbox_2.Text = "+2H";
            sl_pos_adduct_checkbox_2.CheckedChanged += new EventHandler(sl_pos_adduct_checkbox_2_checkedChanged);
            sl_pos_adduct_checkbox_3.Parent = sl_positive_adduct;
            sl_pos_adduct_checkbox_3.Location = new Point(10, 55);
            sl_pos_adduct_checkbox_3.Text = "+NH4";
            sl_pos_adduct_checkbox_3.CheckedChanged += new EventHandler(sl_pos_adduct_checkbox_3_checkedChanged);
            sl_pos_adduct_checkbox_4.Parent = sl_positive_adduct;
            sl_pos_adduct_checkbox_4.Location = new Point(10, 75);
            sl_pos_adduct_checkbox_4.Text = "+Na";
            sl_pos_adduct_checkbox_4.CheckedChanged += new EventHandler(sl_pos_adduct_checkbox_4_checkedChanged);
            sl_negative_adduct.Location = new Point(800, 200);
            sl_negative_adduct.Width = 120;
            sl_negative_adduct.Height = 120;
            sl_negative_adduct.Text = "Negative adducts";
            sl_neg_adduct_checkbox_1.Parent = sl_negative_adduct;
            sl_neg_adduct_checkbox_1.Location = new Point(10, 15);
            sl_neg_adduct_checkbox_1.Text = "-H";
            sl_neg_adduct_checkbox_1.CheckedChanged += new EventHandler(sl_neg_adduct_checkbox_1_checkedChanged);
            sl_neg_adduct_checkbox_2.Parent = sl_negative_adduct;
            sl_neg_adduct_checkbox_2.Location = new Point(10, 35);
            sl_neg_adduct_checkbox_2.Text = "-2H";
            sl_neg_adduct_checkbox_2.CheckedChanged += new EventHandler(sl_neg_adduct_checkbox_2_checkedChanged);
            sl_neg_adduct_checkbox_3.Parent = sl_negative_adduct;
            sl_neg_adduct_checkbox_3.Location = new Point(10, 55);
            sl_neg_adduct_checkbox_3.Text = "+HCOO";
            sl_neg_adduct_checkbox_3.CheckedChanged += new EventHandler(sl_neg_adduct_checkbox_3_checkedChanged);
            sl_neg_adduct_checkbox_4.Parent = sl_negative_adduct;
            sl_neg_adduct_checkbox_4.Location = new Point(10, 75);
            sl_neg_adduct_checkbox_4.Text = "+CH3COO";
            sl_neg_adduct_checkbox_4.CheckedChanged += new EventHandler(sl_neg_adduct_checkbox_4_checkedChanged);

            sl_picture_box.Image = sphingo_backbone_image;
            sl_picture_box.Location = new Point((int)(214 - sphingo_backbone_image.Width * 0.5), (int)(204 - sphingo_backbone_image.Height * 0.5));
            sl_picture_box.SizeMode = PictureBoxSizeMode.AutoSize;



            lipids_groupbox.Controls.Add(lipids_gridview);
            lipids_groupbox.Controls.Add(send_to_skyline_button);
            lipids_groupbox.Dock = DockStyle.Bottom;
            lipids_groupbox.Text = "Lipid list";
            lipids_groupbox.Height = 280;

            cl_add_lipid_button.Text = "Add cardiolipins";
            cl_add_lipid_button.Width = 130;
            cl_add_lipid_button.Location = new Point(800, 380);
            cl_add_lipid_button.BackColor = SystemColors.Control;
            cl_add_lipid_button.Click += register_lipid;

            cl_reset_lipid_button.Text = "Reset lipid";
            cl_reset_lipid_button.Width = 130;
            cl_reset_lipid_button.Location = new Point(20, 380);
            cl_reset_lipid_button.BackColor = SystemColors.Control;
            cl_reset_lipid_button.Click += reset_cl_lipid;

            cl_modify_lipid_button.Text = "Modify lipid";
            cl_modify_lipid_button.Width = 130;
            cl_modify_lipid_button.Location = new Point(660, 380);
            cl_modify_lipid_button.BackColor = SystemColors.Control;
            cl_modify_lipid_button.Click += modify_cl_lipid;

            cl_ms2fragments_lipid_button.Text = "MS2 fragments";
            cl_ms2fragments_lipid_button.Width = 130;
            cl_ms2fragments_lipid_button.Location = new Point(160, 380);
            cl_ms2fragments_lipid_button.BackColor = SystemColors.Control;
            cl_ms2fragments_lipid_button.Click += open_ms2_form;

            gl_add_lipid_button.Text = "Add glycerolipids";
            gl_add_lipid_button.Width = 130;
            gl_add_lipid_button.Location = new Point(800, 380);
            gl_add_lipid_button.BackColor = SystemColors.Control;
            gl_add_lipid_button.Click += register_lipid;

            gl_reset_lipid_button.Text = "Reset lipid";
            gl_reset_lipid_button.Width = 130;
            gl_reset_lipid_button.Location = new Point(20, 380);
            gl_reset_lipid_button.BackColor = SystemColors.Control;
            gl_reset_lipid_button.Click += reset_gl_lipid;

            gl_modify_lipid_button.Text = "Modify lipid";
            gl_modify_lipid_button.Width = 130;
            gl_modify_lipid_button.Location = new Point(660, 380);
            gl_modify_lipid_button.BackColor = SystemColors.Control;
            gl_modify_lipid_button.Click += modify_gl_lipid;

            gl_ms2fragments_lipid_button.Text = "MS2 fragments";
            gl_ms2fragments_lipid_button.Width = 130;
            gl_ms2fragments_lipid_button.Location = new Point(160, 380);
            gl_ms2fragments_lipid_button.BackColor = SystemColors.Control;
            gl_ms2fragments_lipid_button.Click += open_ms2_form;

            pl_add_lipid_button.Text = "Add phospholipids";
            pl_add_lipid_button.Width = 130;
            pl_add_lipid_button.Location = new Point(800, 380);
            pl_add_lipid_button.BackColor = SystemColors.Control;
            pl_add_lipid_button.Click += register_lipid;

            pl_reset_lipid_button.Text = "Reset lipid";
            pl_reset_lipid_button.Width = 130;
            pl_reset_lipid_button.Location = new Point(20, 380);
            pl_reset_lipid_button.BackColor = SystemColors.Control;
            pl_reset_lipid_button.Click += reset_pl_lipid;

            pl_modify_lipid_button.Text = "Modify lipid";
            pl_modify_lipid_button.Width = 130;
            pl_modify_lipid_button.Location = new Point(660, 380);
            pl_modify_lipid_button.BackColor = SystemColors.Control;
            pl_modify_lipid_button.Click += modify_pl_lipid;

            pl_ms2fragments_lipid_button.Text = "MS2 fragments";
            pl_ms2fragments_lipid_button.Width = 130;
            pl_ms2fragments_lipid_button.Location = new Point(160, 380);
            pl_ms2fragments_lipid_button.BackColor = SystemColors.Control;
            pl_ms2fragments_lipid_button.Click += open_ms2_form;

            sl_add_lipid_button.Text = "Add sphingolipids";
            sl_add_lipid_button.Width = 130;
            sl_add_lipid_button.Location = new Point(800, 380);
            sl_add_lipid_button.BackColor = SystemColors.Control;
            sl_add_lipid_button.Click += register_lipid;

            sl_reset_lipid_button.Text = "Reset lipid";
            sl_reset_lipid_button.Width = 130;
            sl_reset_lipid_button.Location = new Point(20, 380);
            sl_reset_lipid_button.BackColor = SystemColors.Control;
            sl_reset_lipid_button.Click += reset_sl_lipid;

            sl_modify_lipid_button.Text = "Modify lipid";
            sl_modify_lipid_button.Width = 130;
            sl_modify_lipid_button.Location = new Point(660, 380);
            sl_modify_lipid_button.BackColor = SystemColors.Control;
            sl_modify_lipid_button.Click += modify_sl_lipid;

            sl_ms2fragments_lipid_button.Text = "MS2 fragments";
            sl_ms2fragments_lipid_button.Width = 130;
            sl_ms2fragments_lipid_button.Location = new Point(160, 380);
            sl_ms2fragments_lipid_button.BackColor = SystemColors.Control;
            sl_ms2fragments_lipid_button.Click += open_ms2_form;


            lipids_gridview.AutoSize = true;
            lipids_gridview.Dock = DockStyle.Fill;
            
            lipids_gridview.DataSource = registered_lipids_datatable;
            lipids_gridview.ReadOnly = true;
            lipids_gridview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            foreach (DataGridViewColumn col in lipids_gridview.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            lipids_gridview.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            lipids_gridview.AllowUserToResizeColumns = false;
            lipids_gridview.AllowUserToAddRows = false;
            lipids_gridview.Width = this.Width;
            //lipids_gridview.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            //lipids_gridview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            lipids_gridview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            lipids_gridview.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            lipids_gridview.AllowUserToResizeRows = false;
            lipids_gridview.RowTemplate.Height = 34;
            lipids_gridview.DoubleClick += new EventHandler(lipids_gridview_double_click);
            //lipids_gridview.UserDeletingRow += new DataGridViewRowCancelEventHandler(lipids_gridview_delete_row);

            send_to_skyline_button.Text = "Send to Skyline";
            send_to_skyline_button.Width = 130;
            send_to_skyline_button.BackColor = SystemColors.Control;
            send_to_skyline_button.Dock = DockStyle.Bottom;
            send_to_skyline_button.Click += send_to_Skyline;

            this.Controls.Add(tab_control);
            this.Controls.Add(lipids_groupbox);
            this.Text = "LipidCreator";
            this.MaximizeBox = false;
            this.Padding = new Padding(5);
        }

        #endregion
    }
}