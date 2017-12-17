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
using System.Drawing.Drawing2D;

namespace LipidCreator
{


    public class Overlay : Control
    {
        public Dictionary<string, Image> arrows;
        public string direction;

        public Overlay(string prefixPath)
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            this.BackColor = Color.Transparent;
            direction = "tl";
            arrows = new Dictionary<string, Image>();
            arrows.Add("tl", Image.FromFile(prefixPath + "images/Tutorial/arrow-top-left.png"));
            arrows.Add("tr", Image.FromFile(prefixPath + "images/Tutorial/arrow-top-right.png"));
            arrows.Add("bl", Image.FromFile(prefixPath + "images/Tutorial/arrow-bottom-left.png"));
            arrows.Add("br", Image.FromFile(prefixPath + "images/Tutorial/arrow-bottom-right.png"));
            arrows.Add("lt", Image.FromFile(prefixPath + "images/Tutorial/arrow-left-top.png"));
            arrows.Add("lb", Image.FromFile(prefixPath + "images/Tutorial/arrow-left-bottom.png"));
            arrows.Add("rt", Image.FromFile(prefixPath + "images/Tutorial/arrow-right-top.png"));
            arrows.Add("rb", Image.FromFile(prefixPath + "images/Tutorial/arrow-right-bottom.png"));
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            this.Size = arrows[direction].Size;
            Graphics g = e.Graphics;
            g.DrawImage(arrows[direction], 0, 0, arrows[direction].Size.Width, arrows[direction].Size.Height);
            g.Dispose();
            base.OnPaint(e);
        }
    }
    
    
    
    public class TutorialWindow : Control
    {
        public CreatorGUI creatorGUI;
        public PictureBox closeButton;
        public PictureBox previous;
        public PictureBox next;
        public Label text;
        public Label paging;
        public Image nextEnabledImage;
        public Image nextDisabledImage;
        public Image previousEnabledImage;
        public Image previousDisabledImage;
    
        public TutorialWindow(CreatorGUI creatorGUI, string prefixPath)
        {
            this.creatorGUI = creatorGUI;
            this.BackColor = Color.White;
            
            closeButton = new PictureBox();
            previous = new PictureBox();
            next = new PictureBox();
            text = new Label();
            paging = new Label();
            
            closeButton.Image = Image.FromFile(prefixPath + "images/Tutorial/close-x.png");
            closeButton.Click += closeTutorialWindow;
            closeButton.Size = closeButton.Image.Size;
            this.Controls.Add(closeButton);
            
            nextEnabledImage = Image.FromFile(prefixPath + "images/Tutorial/next-enabled.png");
            nextDisabledImage = Image.FromFile(prefixPath + "images/Tutorial/next-disabled.png");
            previousEnabledImage = Image.FromFile(prefixPath + "images/Tutorial/previous-enabled.png");
            previousDisabledImage = Image.FromFile(prefixPath + "images/Tutorial/previous-disabled.png");
            
            previous.Click += previousTutorialWindow;
            this.Controls.Add(previous);
            
            next.Click += nextTutorialWindow;
            this.Controls.Add(next);
            
            text.Font = new Font("Arial", 14);
            text.AutoSize = true;
            //this.Controls.Add(text);
            
            paging.Text = " 1 / 20";
            paging.Font = new Font("Arial", 10);
            paging.Size = new Size(40, 14);
            this.Controls.Add(paging);
        }
        
        public void closeTutorialWindow(Object sender, EventArgs e)
        {
            creatorGUI.tutorial.quitTutorial();
        }
        
        public void previousTutorialWindow(Object sender, EventArgs e)
        {
            if (creatorGUI.tutorial.tutorialStep > 1) creatorGUI.tutorial.nextTutorialStep(false);
        }
        
        public void nextTutorialWindow(Object sender, EventArgs e)
        {
            if (creatorGUI.tutorial.nextEnabled) creatorGUI.tutorial.nextTutorialStep(true);
        }
    
        protected override void OnPaint(PaintEventArgs e)
        {
            // draw border
            Graphics g = e.Graphics;
            Pen blackPen = new Pen(Color.Black, 10);
            g.DrawRectangle(blackPen, 0, 0, this.Size.Width, this.Size.Height);
            
            
            Font drawFont = new Font("Arial", 14);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            RectangleF drawRect = new RectangleF(20, 30, this.Size.Width - 40, this.Size.Height - 60);
            g.DrawString(text.Text, drawFont, drawBrush, drawRect);
            g.Dispose();
            
            if (creatorGUI.tutorial.tutorialStep > 1) previous.Image = previousEnabledImage;
            else previous.Image = previousDisabledImage;
            
            if (creatorGUI.tutorial.nextEnabled) next.Image = nextEnabledImage;
            else next.Image = nextDisabledImage;
            
            previous.Size = previous.Image.Size;
            next.Size = next.Image.Size;
            
            
            paging.Text = creatorGUI.tutorial.tutorialStep.ToString() + " / " + creatorGUI.tutorial.maxSteps[(int)creatorGUI.tutorial.tutorial];
            paging.Location = new Point(this.Size.Width - 20 - paging.Size.Width, this.Size.Height - 12 - paging.Size.Height);
        
            closeButton.Location = new Point(this.Size.Width - 12 - closeButton.Size.Width, 12);
            
            previous.Location = new Point(paging.Location.X - previous.Size.Width - 4, this.Size.Height - 12 - previous.Size.Height);
            next.Location = new Point(paging.Location.X + paging.Size.Width + 4, this.Size.Height - 12 - next.Size.Height);
            
            base.OnPaint(e);
        }
    }
    

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
        public Image deleteImage;
        public Image editImage;
        public Image addImage;
        public bool initialCall = true;
        public Overlay tutorialArrow;
        public TutorialWindow tutorialWindow;
        
        public System.Timers.Timer timerEasterEgg;
        public System.Windows.Forms.MainMenu mainMenuLipidCreator;
        public System.Windows.Forms.MenuItem menuFile;
        public System.Windows.Forms.MenuItem menuImport;
        public System.Windows.Forms.MenuItem menuImportSettings;
        public System.Windows.Forms.MenuItem menuImportPredefined;
        public System.Windows.Forms.MenuItem menuExport;
        public System.Windows.Forms.MenuItem menuExportSettings;
        public System.Windows.Forms.MenuItem menuDash;
        public System.Windows.Forms.MenuItem menuDash2;
        public System.Windows.Forms.MenuItem menuDash3;
        public System.Windows.Forms.MenuItem menuExit;
        public System.Windows.Forms.MenuItem menuOptions;
        public System.Windows.Forms.MenuItem menuMS2Fragments;
        public System.Windows.Forms.MenuItem menuIsotopes;
        public System.Windows.Forms.MenuItem menuResetCategory;
        public System.Windows.Forms.MenuItem menuResetLipidCreator;
        public System.Windows.Forms.MenuItem menuHelp;
        public System.Windows.Forms.MenuItem menuAbout;
        
        
        public TabControl tabControl = new TabControl();
        public TabPage homeTab;
        public TabPage glycerolipidsTab;
        public TabPage phospholipidsTab;
        public TabPage sphingolipidsTab;
        public TabPage cholesterollipidsTab;
        public TabPage mediatorlipidsTab;
        public GroupBox lipidsGroupbox;
        public int DefaultCheckboxBGR;
        public int DefaultCheckboxBGG;
        public int DefaultCheckboxBGB;

        public Button addLipidButton;
        public Button modifyLipidButton;
        public Button MS2fragmentsLipidButton;
        public Button addHeavyIsotopeButton;
        public Button tutorialStart;
        
        

        Image cardioBackboneImage;
        Image cardioBackboneImageFA1e;
        Image cardioBackboneImageFA1p;
        Image cardioBackboneImageFA2e;
        Image cardioBackboneImageFA2p;
        Image cardioBackboneImageFA3e;
        Image cardioBackboneImageFA3p;
        Image cardioBackboneImageFA4e;
        Image cardioBackboneImageFA4p;
        Image glyceroBackboneImage;
        Image glyceroBackboneImageOrig;
        Image glyceroBackboneImagePlant;
        Image glyceroBackboneImageFAe;
        Image glyceroBackboneImageFA1p;
        Image glyceroBackboneImageFA2e;
        Image glyceroBackboneImageFA2p;
        Image glyceroBackboneImageFA3e;
        Image glyceroBackboneImageFA3p;
        Image glyceroBackboneImageFA1eOrig;
        Image glyceroBackboneImageFA1pOrig;
        Image glyceroBackboneImageFA2eOrig;
        Image glyceroBackboneImageFA2pOrig;
        Image glyceroBackboneImageFA3eOrig;
        Image glyceroBackboneImageFA3pOrig;
        Image glyceroBackboneImageFA1ePlant;
        Image glyceroBackboneImageFA1pPlant;
        Image glyceroBackboneImageFA2ePlant;
        Image glyceroBackboneImageFA2pPlant;
        Image phosphoBackboneImage;
        Image phosphoBackboneImageFA1e;
        Image phosphoBackboneImageFA1p;
        Image phosphoBackboneImageFA2e;
        Image phosphoBackboneImageFA2p;
        Image sphingoBackboneImage;
        Image cholesterolBackboneImage;
        Image cholesterolEsterBackboneImage;
        

        public PictureBox clPictureBox;
        public PictureBox glPictureBox;
        public PictureBox plPictureBox;
        public PictureBox slPictureBox;
        public PictureBox chPictureBox;
        public PictureBox medPictureBox;
        
        public ListBox glHgListbox;
        public ListBox plHgListbox;
        public ListBox slHgListbox;
        public ListBox medHgListbox;

        public TextBox clFA1Textbox;
        public TextBox clFA2Textbox;
        public TextBox clFA3Textbox;
        public TextBox clFA4Textbox;
        public TextBox glFA1Textbox;
        public TextBox glFA2Textbox;
        public TextBox glFA3Textbox;
        public TextBox plFA1Textbox;
        public TextBox plFA2Textbox;
        public TextBox slLCBTextbox;
        public TextBox slFATextbox;
        public TextBox chFATextbox;

        public ComboBox clFA1Combobox;
        public ComboBox clFA2Combobox;
        public ComboBox clFA3Combobox;
        public ComboBox clFA4Combobox;
        public ComboBox glFA1Combobox;
        public ComboBox glFA2Combobox;
        public ComboBox glFA3Combobox;
        public ComboBox plFA1Combobox;
        public ComboBox plFA2Combobox;
        public ComboBox slLCBCombobox;
        public ComboBox slFACombobox;
        public ComboBox chFACombobox;


        public CheckBox clFA1Checkbox1;
        public CheckBox clFA1Checkbox2;
        public CheckBox clFA1Checkbox3;
        public CheckBox clFA2Checkbox1;
        public CheckBox clFA2Checkbox2;
        public CheckBox clFA2Checkbox3;
        public CheckBox clFA3Checkbox1;
        public CheckBox clFA3Checkbox2;
        public CheckBox clFA3Checkbox3;
        public CheckBox clFA4Checkbox1;
        public CheckBox clFA4Checkbox2;
        public CheckBox clFA4Checkbox3;
        public CheckBox glFA1Checkbox1;
        public CheckBox glFA1Checkbox2;
        public CheckBox glFA1Checkbox3;
        public CheckBox glFA2Checkbox1;
        public CheckBox glFA2Checkbox2;
        public CheckBox glFA2Checkbox3;
        public CheckBox glFA3Checkbox1;
        public CheckBox glFA3Checkbox2;
        public CheckBox glFA3Checkbox3;
        public CheckBox plFA1Checkbox1;
        public CheckBox plFA1Checkbox2;
        public CheckBox plFA1Checkbox3;
        public CheckBox plFA2Checkbox1;
        public CheckBox plFA2Checkbox2;
        public CheckBox plFA2Checkbox3;
        public CheckBox plIsCL;
        public CheckBox glContainsSugar;
        public CheckBox chContainsEster;

        public GroupBox clPositiveAdduct;
        public GroupBox clNegativeAdduct;
        public GroupBox glPositiveAdduct;
        public GroupBox glNegativeAdduct;
        public GroupBox plPositiveAdduct;
        public GroupBox plNegativeAdduct;
        public GroupBox slPositiveAdduct;
        public GroupBox slNegativeAdduct;
        public GroupBox chPositiveAdduct;
        public GroupBox chNegativeAdduct;
        public GroupBox medNegativeAdduct;

        public CheckBox clPosAdductCheckbox1;
        public CheckBox clPosAdductCheckbox2;
        public CheckBox clPosAdductCheckbox3;
        public CheckBox clNegAdductCheckbox1;
        public CheckBox clNegAdductCheckbox2;
        public CheckBox clNegAdductCheckbox3;
        public CheckBox clNegAdductCheckbox4;
        public CheckBox glPosAdductCheckbox1;
        public CheckBox glPosAdductCheckbox2;
        public CheckBox glPosAdductCheckbox3;
        public CheckBox glNegAdductCheckbox1;
        public CheckBox glNegAdductCheckbox2;
        public CheckBox glNegAdductCheckbox3;
        public CheckBox glNegAdductCheckbox4;
        public CheckBox plPosAdductCheckbox1;
        public CheckBox plPosAdductCheckbox2;
        public CheckBox plPosAdductCheckbox3;
        public CheckBox plNegAdductCheckbox1;
        public CheckBox plNegAdductCheckbox2;
        public CheckBox plNegAdductCheckbox3;
        public CheckBox plNegAdductCheckbox4;
        public CheckBox slPosAdductCheckbox1;
        public CheckBox slPosAdductCheckbox2;
        public CheckBox slPosAdductCheckbox3;
        public CheckBox slNegAdductCheckbox1;
        public CheckBox slNegAdductCheckbox2;
        public CheckBox slNegAdductCheckbox3;
        public CheckBox slNegAdductCheckbox4;
        public CheckBox chPosAdductCheckbox1;
        public CheckBox chPosAdductCheckbox2;
        public CheckBox chPosAdductCheckbox3;
        public CheckBox chNegAdductCheckbox1;
        public CheckBox chNegAdductCheckbox2;
        public CheckBox chNegAdductCheckbox3;
        public CheckBox chNegAdductCheckbox4;
        public CheckBox medNegAdductCheckbox1;
        public CheckBox medNegAdductCheckbox2;
        public CheckBox medNegAdductCheckbox3;
        public CheckBox medNegAdductCheckbox4;
        
        public CheckBox clRepresentativeFA;
        public CheckBox glRepresentativeFA;
        public CheckBox plRepresentativeFA;
        Color highlightedCheckboxColor;

        public TextBox clDB1Textbox;
        public TextBox clDB2Textbox;
        public TextBox clDB3Textbox;
        public TextBox clDB4Textbox;
        public TextBox glDB1Textbox;
        public TextBox glDB2Textbox;
        public TextBox glDB3Textbox;
        public TextBox plDB1Textbox;
        public TextBox plDB2Textbox;
        public TextBox slDB1Textbox;
        public TextBox slDB2Textbox;
        public TextBox chDBTextbox;
        
        public TextBox clHydroxyl1Textbox;
        public TextBox clHydroxyl2Textbox;
        public TextBox clHydroxyl3Textbox;
        public TextBox clHydroxyl4Textbox;
        public TextBox glHydroxyl1Textbox;
        public TextBox glHydroxyl2Textbox;
        public TextBox glHydroxyl3Textbox;
        public TextBox plHydroxyl1Textbox;
        public TextBox plHydroxyl2Textbox;
        public TextBox chHydroxylTextbox;
        

        Label clDB1Label;
        Label clDB2Label;
        Label clDB3Label;
        Label clDB4Label;
        Label glDB1Label;
        Label glDB2Label;
        Label glDB3Label;
        Label plDB1Label;
        Label plDB2Label;
        Label slDB1Label;
        Label slDB2Label;
        Label chDBLabel;
        Label slLCBHydroxyLabel;
        Label slFAHydroxyLabel;
        Label clHydroxyl1Label;
        Label clHydroxyl2Label;
        Label clHydroxyl3Label;
        Label clHydroxyl4Label;
        Label glHydroxyl1Label;
        Label glHydroxyl2Label;
        Label glHydroxyl3Label;
        Label plHydroxyl1Label;
        Label plHydroxyl2Label;
        Label chFAHydroxyLabel;
        
        PictureBox glArrow;
        Image glArrowImage;
        Label easterText;

        Label glHGLabel;
        Label plHGLabel;
        Label slHGLabel;
        
        public ComboBox slLCBHydroxyCombobox;
        public ComboBox slFAHydroxyCombobox;

        ToolTip toolTip;

        DataGridView lipidsGridview;
        public Button openReviewFormButton;
        
        public ArrayList controlElements;
        
        

        

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
            this.timerEasterEgg = new System.Timers.Timer(5);
            this.timerEasterEgg.Elapsed += this.timerEasterEggTick;
            
            this.mainMenuLipidCreator = new System.Windows.Forms.MainMenu();
            this.Menu = this.mainMenuLipidCreator;
            tutorialArrow = new Overlay(lipidCreator.prefixPath);
            tutorialArrow.Visible = false;
            tutorialWindow = new TutorialWindow(this, lipidCreator.prefixPath);
            tutorialWindow.Visible = false;
            
            this.menuFile = new System.Windows.Forms.MenuItem ();
            this.menuImport = new System.Windows.Forms.MenuItem ();
            this.menuImportSettings = new System.Windows.Forms.MenuItem ();
            this.menuImportPredefined = new System.Windows.Forms.MenuItem();
            this.menuExport = new System.Windows.Forms.MenuItem ();
            this.menuExportSettings = new System.Windows.Forms.MenuItem ();
            this.menuDash = new System.Windows.Forms.MenuItem ();
            this.menuDash2 = new System.Windows.Forms.MenuItem ();
            this.menuDash3 = new System.Windows.Forms.MenuItem ();
            this.menuExit = new System.Windows.Forms.MenuItem ();
            this.menuResetCategory = new System.Windows.Forms.MenuItem ();
            this.menuResetLipidCreator = new System.Windows.Forms.MenuItem ();
            this.menuOptions = new System.Windows.Forms.MenuItem ();
            this.menuMS2Fragments = new System.Windows.Forms.MenuItem ();
            this.menuIsotopes = new System.Windows.Forms.MenuItem ();
            this.menuFile.MenuItems.AddRange(new MenuItem[]{ menuImport, menuImportPredefined, menuExport, menuDash, menuImportSettings, menuExportSettings, menuDash3, menuExit});
            this.menuFile.Text = "File";
            
            this.menuImport.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
            this.menuImport.Text = "&Import";
            this.menuImport.Click += new System.EventHandler (menuImportClick);
            
            this.menuImportSettings.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.menuImportSettings.Text = "Import &Settings";
            this.menuImportSettings.Click += new System.EventHandler (menuImportSettingsClick);
            
            this.menuImportPredefined.Text = "Import Predefined";
            
            this.menuExport.Shortcut = System.Windows.Forms.Shortcut.CtrlE;
            this.menuExport.Text = "&Export";
            this.menuExport.Click += new System.EventHandler (menuExportClick);
            
            this.menuExportSettings.Shortcut = System.Windows.Forms.Shortcut.CtrlT;
            this.menuExportSettings.Text = "Expor&t Settings";
            this.menuExportSettings.Click += new System.EventHandler (menuExportSettingsClick);
            
            this.menuDash.Text = "-";
            this.menuDash2.Text = "-";
            this.menuDash3.Text = "-";
            
            this.menuExit.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler (menuExitClick);
            
            
            this.menuOptions.MenuItems.AddRange(new MenuItem[]{ menuMS2Fragments, menuIsotopes, menuDash2, menuResetCategory, menuResetLipidCreator});
            this.menuOptions.Text = "Options";
            
            this.menuMS2Fragments.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
            this.menuMS2Fragments.Text = "&MS2 fragments";
            this.menuMS2Fragments.Click += new System.EventHandler (openMS2Form);
            
            this.menuIsotopes.Shortcut = System.Windows.Forms.Shortcut.CtrlH;
            this.menuIsotopes.Text = "Manage &heavy isotopes";
            this.menuIsotopes.Click += new System.EventHandler (openHeavyIsotopeForm);
            
            this.menuResetCategory.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
            this.menuResetCategory.Text = "Reset &lipid category";
            this.menuResetCategory.Click += new System.EventHandler (resetLipid);
            
            this.menuResetLipidCreator.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.menuResetLipidCreator.Text = "Reset Lipid&Creator";
            this.menuResetLipidCreator.Click += new System.EventHandler (resetLipidCreator);

            this.menuHelp = new System.Windows.Forms.MenuItem ();
            this.menuHelp.Text = "&Help";

            this.menuAbout = new System.Windows.Forms.MenuItem ();
            this.menuHelp.MenuItems.AddRange(new MenuItem[]{ menuAbout });

            
            this.menuAbout.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.menuAbout.Text = "&About";
            this.menuAbout.Click += new System.EventHandler (menuAboutClick);

            this.mainMenuLipidCreator.MenuItems.AddRange(new MenuItem[] { this.menuFile, this.menuOptions, this.menuHelp } );
            
            tabControl = new TabControl();
            this.Size = new System.Drawing.Size(1060, 700);
            this.MinimumSize = new System.Drawing.Size(1060, 700);
            homeTab = new TabPage();
            glycerolipidsTab = new TabPage();
            phospholipidsTab = new TabPage();
            sphingolipidsTab = new TabPage();
            cholesterollipidsTab = new TabPage();
            mediatorlipidsTab = new TabPage();
            lipidsGroupbox = new GroupBox();
            addLipidButton = new Button();
            addHeavyIsotopeButton = new Button();
            modifyLipidButton = new Button();
            MS2fragmentsLipidButton = new Button();

            highlightedCheckboxColor = Color.FromArgb(156, 232, 189);
            lipidsGridview = new DataGridView();
            openReviewFormButton = new Button();
            

            clPictureBox = new PictureBox();
            glPictureBox = new PictureBox();
            plPictureBox = new PictureBox();
            slPictureBox = new PictureBox();
            chPictureBox = new PictureBox();
            medPictureBox = new PictureBox();
            glArrow = new PictureBox();
            
            String dbText = "No. DB";
            String hydroxylText = "Hydroxy No.";
            int dbLength = 70;
            int sep = 15;
            int sepText = 20;
            int faLength = 150;
            int topLowButtons = 390;
            int leftGroupboxes = 1000;

            
            deleteImage = Image.FromFile(lipidCreator.prefixPath + "images/delete-small.png");
            editImage = Image.FromFile(lipidCreator.prefixPath + "images/edit-small.png");
            addImage = Image.FromFile(lipidCreator.prefixPath + "images/add-small.png");
            
            glHgListbox = new ListBox();
            plHgListbox = new ListBox();
            slHgListbox = new ListBox();
            medHgListbox = new ListBox();
            
            List<String> glHgList = new List<String>();
            foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.GlyceroLipid])
            {
                if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].derivative && !lipidCreator.headgroups[headgroup].heavyLabeled && headgroup.Length > 2) glHgList.Add(headgroup);
            }
            glHgList.Sort();
            
            List<String> plHgList = new List<String>();
            foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.PhosphoLipid])
            {
                if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].derivative && !lipidCreator.headgroups[headgroup].heavyLabeled && !headgroup.Equals("CL") && !headgroup.Equals("MLCL")) plHgList.Add(headgroup);
            }
            plHgList.Sort();
            
            List<String> slHgList = new List<String>();
            foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.SphingoLipid])
            {
                if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].derivative && !lipidCreator.headgroups[headgroup].heavyLabeled) slHgList.Add(headgroup);
            }
            slHgList.Sort();
            
            List<String> medHgList = new List<String>();
            foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.Mediator])
            {
                if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].derivative && !lipidCreator.headgroups[headgroup].heavyLabeled) medHgList.Add(headgroup);
            }
            medHgList.Sort();
            
            
            
            
            glHgListbox.Items.AddRange(glHgList.ToArray());
            plHgListbox.Items.AddRange(plHgList.ToArray());
            slHgListbox.Items.AddRange(slHgList.ToArray());
            medHgListbox.Items.AddRange(medHgList.ToArray());
            
            cardioBackboneImage = Image.FromFile(lipidCreator.prefixPath + "images/backbones/CL_backbones.png");
            cardioBackboneImageFA1e = Image.FromFile(lipidCreator.prefixPath + "images/backbones/CL_FAe1.png");
            cardioBackboneImageFA2e = Image.FromFile(lipidCreator.prefixPath + "images/backbones/CL_FAe2.png");
            cardioBackboneImageFA3e = Image.FromFile(lipidCreator.prefixPath + "images/backbones/CL_FAe3.png");
            cardioBackboneImageFA4e = Image.FromFile(lipidCreator.prefixPath + "images/backbones/CL_FAe4.png");
            cardioBackboneImageFA1p = Image.FromFile(lipidCreator.prefixPath + "images/backbones/CL_FAp1.png");
            cardioBackboneImageFA2p = Image.FromFile(lipidCreator.prefixPath + "images/backbones/CL_FAp2.png");
            cardioBackboneImageFA3p = Image.FromFile(lipidCreator.prefixPath + "images/backbones/CL_FAp3.png");
            cardioBackboneImageFA4p = Image.FromFile(lipidCreator.prefixPath + "images/backbones/CL_FAp4.png");
            glyceroBackboneImageOrig = Image.FromFile(lipidCreator.prefixPath + "images/backbones/GL_backbones.png");
            glyceroBackboneImageFA1eOrig = Image.FromFile(lipidCreator.prefixPath + "images/backbones/GL_FAe1.png");
            glyceroBackboneImageFA2eOrig = Image.FromFile(lipidCreator.prefixPath + "images/backbones/GL_FAe2.png");
            glyceroBackboneImageFA3eOrig = Image.FromFile(lipidCreator.prefixPath + "images/backbones/GL_FAe3.png");
            glyceroBackboneImageFA1pOrig = Image.FromFile(lipidCreator.prefixPath + "images/backbones/GL_FAp1.png");
            glyceroBackboneImageFA2pOrig = Image.FromFile(lipidCreator.prefixPath + "images/backbones/GL_FAp2.png");
            glyceroBackboneImageFA3pOrig = Image.FromFile(lipidCreator.prefixPath + "images/backbones/GL_FAp3.png");
            glyceroBackboneImagePlant = Image.FromFile(lipidCreator.prefixPath + "images/backbones/GL_backbones_plant.png");
            glyceroBackboneImageFA1ePlant = Image.FromFile(lipidCreator.prefixPath + "images/backbones/GL_plant_FAe1.png");
            glyceroBackboneImageFA2ePlant = Image.FromFile(lipidCreator.prefixPath + "images/backbones/GL_plant_FAe2.png");
            glyceroBackboneImageFA1pPlant = Image.FromFile(lipidCreator.prefixPath + "images/backbones/GL_plant_FAp1.png");
            glyceroBackboneImageFA2pPlant = Image.FromFile(lipidCreator.prefixPath + "images/backbones/GL_plant_FAp2.png");
            phosphoBackboneImage = Image.FromFile(lipidCreator.prefixPath + "images/backbones/PL_backbones.png");
            phosphoBackboneImageFA1e = Image.FromFile(lipidCreator.prefixPath + "images/backbones/PL_FAe1.png");
            phosphoBackboneImageFA2e = Image.FromFile(lipidCreator.prefixPath + "images/backbones/PL_FAe2.png");
            phosphoBackboneImageFA1p = Image.FromFile(lipidCreator.prefixPath + "images/backbones/PL_FAp1.png");
            phosphoBackboneImageFA2p = Image.FromFile(lipidCreator.prefixPath + "images/backbones/PL_FAp2.png");
            sphingoBackboneImage = Image.FromFile(lipidCreator.prefixPath + "images/backbones/SL_backbones.png");
            cholesterolBackboneImage = Image.FromFile(lipidCreator.prefixPath + "images/backbones/Ch.png");
            cholesterolEsterBackboneImage = Image.FromFile(lipidCreator.prefixPath + "images/backbones/ChE.png");
            glArrowImage = Image.FromFile(lipidCreator.prefixPath + "images/arrow.png");

            
            glyceroBackboneImage = glyceroBackboneImageOrig;
            glyceroBackboneImageFAe = glyceroBackboneImageFA1eOrig;
            glyceroBackboneImageFA2e = glyceroBackboneImageFA2eOrig;
            glyceroBackboneImageFA3e = glyceroBackboneImageFA3eOrig;
            glyceroBackboneImageFA1p = glyceroBackboneImageFA1pOrig;
            glyceroBackboneImageFA2p = glyceroBackboneImageFA2pOrig;
            glyceroBackboneImageFA3p = glyceroBackboneImageFA3pOrig;
            
            
            clFA1Textbox = new TextBox();
            clFA2Textbox = new TextBox();
            clFA3Textbox = new TextBox();
            clFA4Textbox = new TextBox();
            clFA1Combobox = new ComboBox();
            clFA1Combobox.Items.Add("Fatty acid chain");
            clFA1Combobox.Items.Add("Fatty acid chain - odd carbon no.");
            clFA1Combobox.Items.Add("Fatty acid chain - even carbon no.");
            clFA2Combobox = new ComboBox();
            clFA2Combobox.Items.Add("Fatty acid chain");
            clFA2Combobox.Items.Add("Fatty acid chain - odd carbon no.");
            clFA2Combobox.Items.Add("Fatty acid chain - even carbon no.");
            clFA3Combobox = new ComboBox();
            clFA3Combobox.Items.Add("Fatty acid chain");
            clFA3Combobox.Items.Add("Fatty acid chain - odd carbon no.");
            clFA3Combobox.Items.Add("Fatty acid chain - even carbon no.");
            clFA4Combobox = new ComboBox();
            clFA4Combobox.Items.Add("Fatty acid chain");
            clFA4Combobox.Items.Add("Fatty acid chain - odd carbon no.");
            clFA4Combobox.Items.Add("Fatty acid chain - even carbon no.");
            clDB1Textbox = new TextBox();
            clDB2Textbox = new TextBox();
            clDB3Textbox = new TextBox();
            clDB4Textbox = new TextBox();
            clHydroxyl1Textbox = new TextBox();
            clHydroxyl2Textbox = new TextBox();
            clHydroxyl3Textbox = new TextBox();
            clHydroxyl4Textbox = new TextBox();
            clHydroxyl1Label = new Label();
            clHydroxyl2Label = new Label();
            clHydroxyl3Label = new Label();
            clHydroxyl4Label = new Label();
            clDB1Label = new Label();
            clDB2Label = new Label();
            clDB3Label = new Label();
            clDB4Label = new Label();
            glFA1Textbox = new TextBox();
            glFA2Textbox = new TextBox();
            glFA3Textbox = new TextBox();
            glFA1Combobox = new ComboBox();
            glFA1Combobox.Items.Add("Fatty acid chain");
            glFA1Combobox.Items.Add("Fatty acid chain - odd carbon no.");
            glFA1Combobox.Items.Add("Fatty acid chain - even carbon no.");
            glFA2Combobox = new ComboBox();
            glFA2Combobox.Items.Add("Fatty acid chain");
            glFA2Combobox.Items.Add("Fatty acid chain - odd carbon no.");
            glFA2Combobox.Items.Add("Fatty acid chain - even carbon no.");
            glFA3Combobox = new ComboBox();
            glFA3Combobox.Items.Add("Fatty acid chain");
            glFA3Combobox.Items.Add("Fatty acid chain - odd carbon no.");
            glFA3Combobox.Items.Add("Fatty acid chain - even carbon no.");
            glDB1Textbox = new TextBox();
            glDB2Textbox = new TextBox();
            glDB3Textbox = new TextBox();
            glHydroxyl1Textbox = new TextBox();
            glHydroxyl2Textbox = new TextBox();
            glHydroxyl3Textbox = new TextBox();
            glDB1Label = new Label();
            glDB2Label = new Label();
            glDB3Label = new Label();
            glHGLabel = new Label();
            glHydroxyl1Label = new Label();
            glHydroxyl2Label = new Label();
            glHydroxyl3Label = new Label();
            plFA1Textbox = new TextBox();
            plFA2Textbox = new TextBox();
            plFA1Combobox = new ComboBox();
            plFA1Combobox.Items.Add("Fatty acid chain");
            plFA1Combobox.Items.Add("Fatty acid chain - odd carbon no.");
            plFA1Combobox.Items.Add("Fatty acid chain - even carbon no.");
            plFA2Combobox = new ComboBox();
            plFA2Combobox.Items.Add("Fatty acid chain");
            plFA2Combobox.Items.Add("Fatty acid chain - odd carbon no.");
            plFA2Combobox.Items.Add("Fatty acid chain - even carbon no.");
            plDB1Textbox = new TextBox();
            plDB2Textbox = new TextBox();
            plHydroxyl1Textbox = new TextBox();
            plHydroxyl2Textbox = new TextBox();
            plDB1Label = new Label();
            plDB2Label = new Label();
            plHydroxyl1Label = new Label();
            plHydroxyl2Label = new Label();
            plHGLabel = new Label();
            slLCBTextbox = new TextBox();
            slFATextbox = new TextBox();
            slLCBCombobox = new ComboBox();
            slLCBCombobox.Items.Add("Long chain base");
            slLCBCombobox.Items.Add("Long chain base - odd carbon no.");
            slLCBCombobox.Items.Add("Long chain base - even carbon no.");
            slFACombobox = new ComboBox();
            slFACombobox.Items.Add("Fatty acid chain");
            slFACombobox.Items.Add("Fatty acid chain - odd carbon no.");
            slFACombobox.Items.Add("Fatty acid chain - even carbon no.");
            slDB1Textbox = new TextBox();
            slDB2Textbox = new TextBox();
            slDB1Label = new Label();
            slDB2Label = new Label();
            slHGLabel = new Label();
            slLCBHydroxyLabel = new Label();
            slFAHydroxyLabel = new Label();
            easterText = new Label();
            chFACombobox = new ComboBox();
            chFACombobox.Items.Add("Fatty acid chain");
            chFACombobox.Items.Add("Fatty acid chain - odd carbon no.");
            chFACombobox.Items.Add("Fatty acid chain - even carbon no.");
            chFATextbox = new TextBox();
            chDBLabel = new Label();
            chDBTextbox = new TextBox();
            chHydroxylTextbox = new TextBox();
            chFAHydroxyLabel = new Label();

            clFA1Checkbox1 = new CheckBox();
            clFA1Checkbox2 = new CheckBox();
            clFA1Checkbox3 = new CheckBox();
            clFA2Checkbox1 = new CheckBox();
            clFA2Checkbox2 = new CheckBox();
            clFA2Checkbox3 = new CheckBox();
            clFA3Checkbox1 = new CheckBox();
            clFA3Checkbox2 = new CheckBox();
            clFA3Checkbox3 = new CheckBox();
            clFA4Checkbox1 = new CheckBox();
            clFA4Checkbox2 = new CheckBox();
            clFA4Checkbox3 = new CheckBox();
            glFA1Checkbox1 = new CheckBox();
            glFA1Checkbox2 = new CheckBox();
            glFA1Checkbox3 = new CheckBox();
            glFA2Checkbox1 = new CheckBox();
            glFA2Checkbox2 = new CheckBox();
            glFA2Checkbox3 = new CheckBox();
            glFA3Checkbox1 = new CheckBox();
            glFA3Checkbox2 = new CheckBox();
            glFA3Checkbox3 = new CheckBox();
            plFA1Checkbox1 = new CheckBox();
            plFA1Checkbox2 = new CheckBox();
            plFA1Checkbox3 = new CheckBox();
            plFA2Checkbox1 = new CheckBox();
            plFA2Checkbox2 = new CheckBox();
            plFA2Checkbox3 = new CheckBox();
            clRepresentativeFA = new CheckBox();
            glRepresentativeFA = new CheckBox();
            plRepresentativeFA = new CheckBox();
            plIsCL = new CheckBox();
            glContainsSugar = new CheckBox();
            chContainsEster = new CheckBox();

            clPositiveAdduct = new GroupBox();
            clNegativeAdduct = new GroupBox();
            glPositiveAdduct = new GroupBox();
            glNegativeAdduct = new GroupBox();
            plPositiveAdduct = new GroupBox();
            plNegativeAdduct = new GroupBox();
            slPositiveAdduct = new GroupBox();
            slNegativeAdduct = new GroupBox();
            chPositiveAdduct = new GroupBox();
            chNegativeAdduct = new GroupBox();
            medNegativeAdduct = new GroupBox();

            clPosAdductCheckbox1 = new CheckBox();
            clPosAdductCheckbox2 = new CheckBox();
            clPosAdductCheckbox3 = new CheckBox();
            clNegAdductCheckbox1 = new CheckBox();
            clNegAdductCheckbox2 = new CheckBox();
            clNegAdductCheckbox3 = new CheckBox();
            clNegAdductCheckbox4 = new CheckBox();
            glPosAdductCheckbox1 = new CheckBox();
            glPosAdductCheckbox2 = new CheckBox();
            glPosAdductCheckbox3 = new CheckBox();
            glNegAdductCheckbox1 = new CheckBox();
            glNegAdductCheckbox2 = new CheckBox();
            glNegAdductCheckbox3 = new CheckBox();
            glNegAdductCheckbox4 = new CheckBox();
            plPosAdductCheckbox1 = new CheckBox();
            plPosAdductCheckbox2 = new CheckBox();
            plPosAdductCheckbox3 = new CheckBox();
            plNegAdductCheckbox1 = new CheckBox();
            plNegAdductCheckbox2 = new CheckBox();
            plNegAdductCheckbox3 = new CheckBox();
            plNegAdductCheckbox4 = new CheckBox();
            slPosAdductCheckbox1 = new CheckBox();
            slPosAdductCheckbox2 = new CheckBox();
            slPosAdductCheckbox3 = new CheckBox();
            slNegAdductCheckbox1 = new CheckBox();
            slNegAdductCheckbox2 = new CheckBox();
            slNegAdductCheckbox3 = new CheckBox();
            slNegAdductCheckbox4 = new CheckBox();
            chPosAdductCheckbox1 = new CheckBox();
            chPosAdductCheckbox2 = new CheckBox();
            chPosAdductCheckbox3 = new CheckBox();
            chNegAdductCheckbox1 = new CheckBox();
            chNegAdductCheckbox2 = new CheckBox();
            chNegAdductCheckbox3 = new CheckBox();
            chNegAdductCheckbox4 = new CheckBox();
            medNegAdductCheckbox1 = new CheckBox();
            medNegAdductCheckbox2 = new CheckBox();
            medNegAdductCheckbox3 = new CheckBox();
            medNegAdductCheckbox4 = new CheckBox();
            
            
            slLCBHydroxyCombobox = new ComboBox();
            slLCBHydroxyCombobox.Items.Add("2");
            slLCBHydroxyCombobox.Items.Add("3");
            
            slFAHydroxyCombobox = new ComboBox();
            slFAHydroxyCombobox.Items.Add("0");
            slFAHydroxyCombobox.Items.Add("1");
            slFAHydroxyCombobox.Items.Add("2");
            slFAHydroxyCombobox.Items.Add("3");



            toolTip = new ToolTip();
            
            

            string formattingFA = "Comma seperated single entries or intervals. Example formatting: 2, 3, 5-6, 13-20";
            string formattingDB = "Comma seperated single entries or intervals. Example formatting: 2, 3-4, 6";
            string formattingHydroxyl = "Comma seperated single entries or intervals. Example formatting: 2-4, 10, 12";
            string FApInformation = "Plasmenyl fatty acids need at least one double bond";


            tabControl.Controls.Add(homeTab);
            tabControl.Controls.Add(glycerolipidsTab);
            tabControl.Controls.Add(phospholipidsTab);
            tabControl.Controls.Add(sphingolipidsTab);
            tabControl.Controls.Add(cholesterollipidsTab);
            tabControl.Controls.Add(mediatorlipidsTab);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Height = 300;
            Font tabFont = new Font(tabControl.Font.FontFamily, 16);
            tabControl.Font = tabFont;
            tabControl.SelectedIndexChanged += new System.EventHandler(tabIndexChanged);
            tabControl.AutoSize = false;
            
            
            homeTab.Text = "Home";
            
            
            

            // tab for cardiolipins
            phospholipidsTab.Controls.Add(clFA1Checkbox3);
            phospholipidsTab.Controls.Add(clFA1Checkbox2);
            phospholipidsTab.Controls.Add(clFA1Checkbox1);
            phospholipidsTab.Controls.Add(clFA2Checkbox3);
            phospholipidsTab.Controls.Add(clFA2Checkbox2);
            phospholipidsTab.Controls.Add(clFA2Checkbox1);
            phospholipidsTab.Controls.Add(clFA3Checkbox3);
            phospholipidsTab.Controls.Add(clFA3Checkbox2);
            phospholipidsTab.Controls.Add(clFA3Checkbox1);
            phospholipidsTab.Controls.Add(clFA4Checkbox3);
            phospholipidsTab.Controls.Add(clFA4Checkbox2);
            phospholipidsTab.Controls.Add(clFA4Checkbox1);
            phospholipidsTab.Controls.Add(clPositiveAdduct);
            phospholipidsTab.Controls.Add(clNegativeAdduct);
            phospholipidsTab.Controls.Add(clPictureBox);
            phospholipidsTab.Controls.Add(clFA1Textbox);
            phospholipidsTab.Controls.Add(clFA2Textbox);
            phospholipidsTab.Controls.Add(clFA3Textbox);
            phospholipidsTab.Controls.Add(clFA4Textbox);
            phospholipidsTab.Controls.Add(clDB1Textbox);
            phospholipidsTab.Controls.Add(clDB2Textbox);
            phospholipidsTab.Controls.Add(clDB3Textbox);
            phospholipidsTab.Controls.Add(clDB4Textbox);
            phospholipidsTab.Controls.Add(clRepresentativeFA);
            phospholipidsTab.Controls.Add(clHydroxyl1Textbox);
            phospholipidsTab.Controls.Add(clHydroxyl2Textbox);
            phospholipidsTab.Controls.Add(clHydroxyl3Textbox);
            phospholipidsTab.Controls.Add(clHydroxyl4Textbox);
            phospholipidsTab.Controls.Add(clFA1Combobox);
            phospholipidsTab.Controls.Add(clFA2Combobox);
            phospholipidsTab.Controls.Add(clFA3Combobox);
            phospholipidsTab.Controls.Add(clFA4Combobox);
            phospholipidsTab.Controls.Add(clDB1Label);
            phospholipidsTab.Controls.Add(clDB2Label);
            phospholipidsTab.Controls.Add(clDB3Label);
            phospholipidsTab.Controls.Add(clDB4Label);
            phospholipidsTab.Controls.Add(clHydroxyl1Label);
            phospholipidsTab.Controls.Add(clHydroxyl2Label);
            phospholipidsTab.Controls.Add(clHydroxyl3Label);
            phospholipidsTab.Controls.Add(clHydroxyl4Label);
            Font plFont = new Font(phospholipidsTab.Font.FontFamily, 8.25F);
            phospholipidsTab.Font = plFont;
            
            
            
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
            clFA1Combobox.Visible = false;
            clFA2Combobox.Visible = false;
            clFA3Combobox.Visible = false;
            clFA4Combobox.Visible = false;
            clDB1Label.Visible = false;
            clDB2Label.Visible = false;
            clDB3Label.Visible = false;
            clDB4Label.Visible = false;
            clHydroxyl1Label.Visible = false;
            clHydroxyl2Label.Visible = false;
            clHydroxyl3Label.Visible = false;
            clHydroxyl4Label.Visible = false;
            clRepresentativeFA.Visible = false;
            

            clPictureBox.Image = cardioBackboneImage;
            clPictureBox.Location = new Point(5, 5);
            clPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            clPictureBox.SendToBack();
            


            clFA1Combobox.BringToFront();
            clFA1Textbox.BringToFront();
            clFA1Textbox.Location = new Point(400, 64);
            clFA1Textbox.Width = faLength;
            clFA1Textbox.TextChanged += new EventHandler(clFA1TextboxValueChanged);
            toolTip.SetToolTip(clFA1Textbox, formattingFA);
            clFA1Combobox.Location = new Point(clFA1Textbox.Left, clFA1Textbox.Top - sepText);
            clFA1Combobox.Width = faLength;
            clFA1Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            clFA1Combobox.SelectedIndexChanged += new EventHandler(clFA1ComboboxValueChanged);
            clDB1Textbox.Location = new Point(clFA1Textbox.Left + clFA1Textbox.Width + sep, clFA1Textbox.Top);
            clDB1Textbox.Width = dbLength;
            clDB1Textbox.TextChanged += new EventHandler(clDB1TextboxValueChanged);
            toolTip.SetToolTip(clDB1Textbox, formattingDB);
            clDB1Label.Width = dbLength;
            clDB1Label.Location = new Point(clDB1Textbox.Left, clDB1Textbox.Top - sep);
            clDB1Label.Text = dbText;
            clHydroxyl1Textbox.Width = dbLength;
            clHydroxyl1Textbox.Location = new Point(clDB1Textbox.Left + clDB1Textbox.Width + sep, clDB1Textbox.Top);
            clHydroxyl1Textbox.TextChanged += new EventHandler(clHydroxyl1TextboxValueChanged);
            toolTip.SetToolTip(clHydroxyl1Textbox, formattingHydroxyl);
            clHydroxyl1Label.Width = dbLength;
            clHydroxyl1Label.Location = new Point(clHydroxyl1Textbox.Left, clHydroxyl1Textbox.Top - sep);
            clHydroxyl1Label.Text = hydroxylText;
            

            clFA1Checkbox3.Location = new Point(clFA1Textbox.Left + 90, clFA1Textbox.Top + clFA1Textbox.Height);
            clFA1Checkbox3.Text = "FAe";
            clFA1Checkbox3.CheckedChanged += new EventHandler(clFA1Checkbox3CheckedChanged);
            clFA1Checkbox3.MouseLeave += new System.EventHandler(clFA1Checkbox3MouseLeave);
            clFA1Checkbox3.MouseMove += new System.Windows.Forms.MouseEventHandler(clFA1Checkbox3MouseHover);
            clFA1Checkbox2.Location = new Point(clFA1Textbox.Left + 40, clFA1Textbox.Top + clFA1Textbox.Height);
            clFA1Checkbox2.Text = "FAp";
            toolTip.SetToolTip(clFA1Checkbox2, FApInformation);
            clFA1Checkbox2.CheckedChanged += new EventHandler(clFA1Checkbox2CheckedChanged);
            clFA1Checkbox2.MouseLeave += new System.EventHandler(clFA1Checkbox2MouseLeave);
            clFA1Checkbox2.MouseMove += new System.Windows.Forms.MouseEventHandler(clFA1Checkbox2MouseHover);
            clFA1Checkbox1.Location = new Point(clFA1Textbox.Left, clFA1Textbox.Top + clFA1Textbox.Height);
            clFA1Checkbox1.Text = "FA";
            clFA1Checkbox1.Checked = true;
            clFA1Checkbox1.CheckedChanged += new EventHandler(clFA1Checkbox1CheckedChanged);

            clPositiveAdduct.Width = 120;
            clPositiveAdduct.Location = new Point(leftGroupboxes - clPositiveAdduct.Width, 60);
            clPositiveAdduct.Height = 120;
            clPositiveAdduct.Text = "Positive adducts";
            clPosAdductCheckbox1.Parent = clPositiveAdduct;
            clPosAdductCheckbox1.Location = new Point(10, 15);
            clPosAdductCheckbox1.Text = "+H⁺";
            clPosAdductCheckbox1.Checked = true;
            clPosAdductCheckbox1.CheckedChanged += new EventHandler(clPosAdductCheckbox1CheckedChanged);
            clPosAdductCheckbox2.Parent = clPositiveAdduct;
            clPosAdductCheckbox2.Location = new Point(10, 35);
            clPosAdductCheckbox2.Text = "+2H⁺⁺";
            clPosAdductCheckbox2.CheckedChanged += new EventHandler(clPosAdductCheckbox2CheckedChanged);
            clPosAdductCheckbox3.Parent = clPositiveAdduct;
            clPosAdductCheckbox3.Location = new Point(10, 55);
            clPosAdductCheckbox3.Text = "+NH4⁺";
            clPosAdductCheckbox3.Enabled = false;
            clPosAdductCheckbox3.CheckedChanged += new EventHandler(clPosAdductCheckbox3CheckedChanged);
            clNegativeAdduct.Width = 120;
            clNegativeAdduct.Location = new Point(leftGroupboxes - clNegativeAdduct.Width, 200);
            clNegativeAdduct.Height = 120;
            clNegativeAdduct.Text = "Negative adducts";
            clNegAdductCheckbox1.Parent = clNegativeAdduct;
            clNegAdductCheckbox1.Location = new Point(10, 15);
            clNegAdductCheckbox1.Text = "-H⁻";
            clNegAdductCheckbox1.CheckedChanged += new EventHandler(clNegAdductCheckbox1CheckedChanged);
            clNegAdductCheckbox2.Parent = clNegativeAdduct;
            clNegAdductCheckbox2.Location = new Point(10, 35);
            clNegAdductCheckbox2.Text = "-2H⁻ ⁻";
            clNegAdductCheckbox2.CheckedChanged += new EventHandler(clNegAdductCheckbox2CheckedChanged);
            clNegAdductCheckbox3.Parent = clNegativeAdduct;
            clNegAdductCheckbox3.Location = new Point(10, 55);
            clNegAdductCheckbox3.Text = "+HCOO⁻";
            clNegAdductCheckbox3.Enabled = false;
            clNegAdductCheckbox3.CheckedChanged += new EventHandler(clNegAdductCheckbox3CheckedChanged);
            clNegAdductCheckbox4.Parent = clNegativeAdduct;
            clNegAdductCheckbox4.Location = new Point(10, 75);
            clNegAdductCheckbox4.Text = "+CH3COO⁻";
            clNegAdductCheckbox4.Enabled = false;
            clNegAdductCheckbox4.CheckedChanged += new EventHandler(clNegAdductCheckbox4CheckedChanged);



            clFA2Combobox.BringToFront();
            clFA2Textbox.BringToFront();
            clFA2Textbox.Location = new Point(312, 144);
            clFA2Textbox.Width = faLength;
            clFA2Textbox.TextChanged += new EventHandler(clFA2TextboxValueChanged);
            toolTip.SetToolTip(clFA2Textbox, formattingFA);
            clFA2Combobox.Location = new Point(clFA2Textbox.Left, clFA2Textbox.Top - sepText);
            clFA2Combobox.Width = faLength;
            clFA2Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            clFA2Combobox.SelectedIndexChanged += new EventHandler(clFA2ComboboxValueChanged);
            clDB2Textbox.Location = new Point(clFA2Textbox.Left + clFA2Textbox.Width + sep, clFA2Textbox.Top);
            clDB2Textbox.Width = dbLength;
            clDB2Textbox.TextChanged += new EventHandler(clDB2TextboxValueChanged);
            toolTip.SetToolTip(clDB2Textbox, formattingDB);
            clDB2Label.Location = new Point(clDB2Textbox.Left, clDB2Textbox.Top - sep);
            clDB2Label.Width = dbLength;
            clDB2Label.Text = dbText;
            clHydroxyl2Textbox.Width = dbLength;
            clHydroxyl2Textbox.Location = new Point(clDB2Textbox.Left + clDB2Textbox.Width + sep, clDB2Textbox.Top);
            clHydroxyl2Textbox.TextChanged += new EventHandler(clHydroxyl2TextboxValueChanged);
            toolTip.SetToolTip(clHydroxyl2Textbox, formattingHydroxyl);
            clHydroxyl2Label.Width = dbLength;
            clHydroxyl2Label.Location = new Point(clHydroxyl2Textbox.Left, clHydroxyl2Textbox.Top - sep);
            clHydroxyl2Label.Text = hydroxylText;

            clFA2Checkbox3.Location = new Point(clFA2Textbox.Left + 90, clFA2Textbox.Top + clFA2Textbox.Height);
            clFA2Checkbox3.Text = "FAe";
            clFA2Checkbox3.CheckedChanged += new EventHandler(clFA2Checkbox3CheckedChanged);
            clFA2Checkbox3.MouseLeave += new System.EventHandler(clFA2Checkbox3MouseLeave);
            clFA2Checkbox3.MouseMove += new System.Windows.Forms.MouseEventHandler(clFA2Checkbox3MouseHover);
            clFA2Checkbox2.Location = new Point(clFA2Textbox.Left + 40, clFA2Textbox.Top + clFA2Textbox.Height);
            clFA2Checkbox2.Text = "FAp";
            toolTip.SetToolTip(clFA2Checkbox2, FApInformation);
            clFA2Checkbox2.CheckedChanged += new EventHandler(clFA2Checkbox2CheckedChanged);
            clFA2Checkbox2.MouseLeave += new System.EventHandler(clFA2Checkbox2MouseLeave);
            clFA2Checkbox2.MouseMove += new System.Windows.Forms.MouseEventHandler(clFA2Checkbox2MouseHover);
            clFA2Checkbox1.Location = new Point(clFA2Textbox.Left, clFA2Textbox.Top + clFA2Textbox.Height);
            clFA2Checkbox1.Text = "FA";
            clFA2Checkbox1.Checked = true;
            clFA2Checkbox1.CheckedChanged += new EventHandler(clFA2Checkbox1CheckedChanged);






            clFA3Combobox.BringToFront();
            clFA3Textbox.BringToFront();
            clFA3Textbox.Location = new Point(436, 260);
            clFA3Textbox.Width = faLength;
            clFA3Textbox.TextChanged += new EventHandler(clFA3TextboxValueChanged);
            toolTip.SetToolTip(clFA3Textbox, formattingFA);
            clFA3Combobox.Location = new Point(clFA3Textbox.Left, clFA3Textbox.Top - sepText);
            clFA3Combobox.Width = faLength;
            clFA3Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            clFA3Combobox.SelectedIndexChanged += new EventHandler(clFA3ComboboxValueChanged);
            clDB3Textbox.Location = new Point(clFA3Textbox.Left + clFA3Textbox.Width + sep, clFA3Textbox.Top);
            clDB3Textbox.Width = dbLength;
            clDB3Textbox.TextChanged += new EventHandler(clDB3TextboxValueChanged);
            toolTip.SetToolTip(clDB3Textbox, formattingDB);
            clDB3Label.Location = new Point(clDB3Textbox.Left, clDB3Textbox.Top - sep);
            clDB3Label.Width = dbLength;
            clDB3Label.Text = dbText;
            clHydroxyl3Textbox.Width = dbLength;
            clHydroxyl3Textbox.Location = new Point(clDB3Textbox.Left + clDB3Textbox.Width + sep, clDB3Textbox.Top);
            clHydroxyl3Textbox.TextChanged += new EventHandler(clHydroxyl3TextboxValueChanged);
            toolTip.SetToolTip(clHydroxyl3Textbox, formattingHydroxyl);
            clHydroxyl3Label.Width = dbLength;
            clHydroxyl3Label.Location = new Point(clHydroxyl3Textbox.Left, clHydroxyl3Textbox.Top - sep);
            clHydroxyl3Label.Text = hydroxylText;

            clFA3Checkbox3.Location = new Point(clFA3Textbox.Left + 90, clFA3Textbox.Top + clFA3Textbox.Height);
            clFA3Checkbox3.Text = "FAe";
            clFA3Checkbox3.CheckedChanged += new EventHandler(clFA3Checkbox3CheckedChanged);
            clFA3Checkbox3.MouseLeave += new System.EventHandler(clFA3Checkbox3MouseLeave);
            clFA3Checkbox3.MouseMove += new System.Windows.Forms.MouseEventHandler(clFA3Checkbox3MouseHover);
            clFA3Checkbox2.Location = new Point(clFA3Textbox.Left + 40, clFA3Textbox.Top + clFA3Textbox.Height);
            clFA3Checkbox2.Text = "FAp";
            toolTip.SetToolTip(clFA3Checkbox2, FApInformation);
            clFA3Checkbox2.CheckedChanged += new EventHandler(clFA3Checkbox2CheckedChanged);
            clFA3Checkbox2.MouseLeave += new System.EventHandler(clFA3Checkbox2MouseLeave);
            clFA3Checkbox2.MouseMove += new System.Windows.Forms.MouseEventHandler(clFA3Checkbox2MouseHover);
            clFA3Checkbox1.Location = new Point(clFA3Textbox.Left, clFA3Textbox.Top + clFA3Textbox.Height);
            clFA3Checkbox1.Text = "FA";
            clFA3Checkbox1.Checked = true;
            clFA3Checkbox1.CheckedChanged += new EventHandler(clFA3Checkbox1CheckedChanged);


            
            clRepresentativeFA.Location = new Point(clHydroxyl1Textbox.Left + clHydroxyl1Textbox.Width + sep, clHydroxyl1Textbox.Top);
            clRepresentativeFA.Width = 120;
            clRepresentativeFA.Text = "First FA representative";
            clRepresentativeFA.CheckedChanged += new EventHandler(clRepresentativeFACheckedChanged);
            clRepresentativeFA.SendToBack();



            clFA4Combobox.BringToFront();
            clFA4Textbox.BringToFront();
            clFA4Textbox.Location = new Point(350, 344);
            clFA4Textbox.Width = faLength;
            clFA4Textbox.TextChanged += new EventHandler(clFA4TextboxValueChanged);
            toolTip.SetToolTip(clFA4Textbox, formattingFA);
            clFA4Combobox.Location = new Point(clFA4Textbox.Left, clFA4Textbox.Top - sepText);
            clFA4Combobox.Width = faLength;
            clFA4Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            clFA4Combobox.SelectedIndexChanged += new EventHandler(clFA4ComboboxValueChanged);
            clDB4Textbox.Location = new Point(clFA4Textbox.Left + clFA4Textbox.Width + sep, clFA4Textbox.Top);
            clDB4Textbox.Width = dbLength;
            clDB4Textbox.TextChanged += new EventHandler(clDB4TextboxValueChanged);
            toolTip.SetToolTip(clDB4Textbox, formattingDB);
            clDB4Label.Location = new Point(clDB4Textbox.Left, clDB4Textbox.Top - sep);
            clDB4Label.Width = dbLength;
            clDB4Label.Text = dbText;
            clHydroxyl4Textbox.Width = dbLength;
            clHydroxyl4Textbox.Location = new Point(clDB4Textbox.Left + clDB4Textbox.Width + sep, clDB4Textbox.Top);
            clHydroxyl4Textbox.TextChanged += new EventHandler(clHydroxyl4TextboxValueChanged);
            toolTip.SetToolTip(clHydroxyl4Textbox, formattingHydroxyl);
            clHydroxyl4Label.Width = dbLength;
            clHydroxyl4Label.Location = new Point(clHydroxyl4Textbox.Left, clHydroxyl4Textbox.Top - sep);
            clHydroxyl4Label.Text = hydroxylText;

            clFA4Checkbox3.Location = new Point(clFA4Textbox.Left + 90, clFA4Textbox.Top + clFA4Textbox.Height);
            clFA4Checkbox3.Text = "FAe";
            clFA4Checkbox3.CheckedChanged += new EventHandler(clFA4Checkbox3CheckedChanged);
            clFA4Checkbox3.MouseLeave += new System.EventHandler(clFA4Checkbox3MouseLeave);
            clFA4Checkbox3.MouseMove += new System.Windows.Forms.MouseEventHandler(clFA4Checkbox3MouseHover);
            clFA4Checkbox2.Location = new Point(clFA4Textbox.Left + 40, clFA4Textbox.Top + clFA4Textbox.Height);
            clFA4Checkbox2.Text = "FAp";
            toolTip.SetToolTip(clFA4Checkbox2, FApInformation);
            clFA4Checkbox2.CheckedChanged += new EventHandler(clFA4Checkbox2CheckedChanged);
            clFA4Checkbox2.MouseLeave += new System.EventHandler(clFA4Checkbox2MouseLeave);
            clFA4Checkbox2.MouseMove += new System.Windows.Forms.MouseEventHandler(clFA4Checkbox2MouseHover);
            clFA4Checkbox1.Location = new Point(clFA4Textbox.Left, clFA4Textbox.Top + clFA4Textbox.Height);
            clFA4Checkbox1.Text = "FA";
            clFA4Checkbox1.Checked = true;
            clFA4Checkbox1.CheckedChanged += new EventHandler(clFA4Checkbox1CheckedChanged);



            // tab for glycerolipids
            glycerolipidsTab.Controls.Add(glFA1Checkbox3);
            glycerolipidsTab.Controls.Add(glFA1Checkbox2);
            glycerolipidsTab.Controls.Add(glFA1Checkbox1);
            glycerolipidsTab.Controls.Add(glFA2Checkbox3);
            glycerolipidsTab.Controls.Add(glFA2Checkbox2);
            glycerolipidsTab.Controls.Add(glFA2Checkbox1);
            glycerolipidsTab.Controls.Add(glFA3Checkbox3);
            glycerolipidsTab.Controls.Add(glFA3Checkbox2);
            glycerolipidsTab.Controls.Add(glFA3Checkbox1);
            glycerolipidsTab.Controls.Add(glPictureBox);
            glycerolipidsTab.Controls.Add(glArrow);
            glycerolipidsTab.Controls.Add(glFA1Textbox);
            glycerolipidsTab.Controls.Add(glFA2Textbox);
            glycerolipidsTab.Controls.Add(glFA3Textbox);
            glycerolipidsTab.Controls.Add(glDB1Textbox);
            glycerolipidsTab.Controls.Add(glDB2Textbox);
            glycerolipidsTab.Controls.Add(glDB3Textbox);
            glycerolipidsTab.Controls.Add(glHydroxyl1Textbox);
            glycerolipidsTab.Controls.Add(glHydroxyl2Textbox);
            glycerolipidsTab.Controls.Add(glHydroxyl3Textbox);
            glycerolipidsTab.Controls.Add(glFA1Combobox);
            glycerolipidsTab.Controls.Add(glFA2Combobox);
            glycerolipidsTab.Controls.Add(glFA3Combobox);
            glycerolipidsTab.Controls.Add(glHgListbox);
            glycerolipidsTab.Controls.Add(glHGLabel);
            glycerolipidsTab.Controls.Add(glContainsSugar);
            glycerolipidsTab.Controls.Add(glDB1Label);
            glycerolipidsTab.Controls.Add(glDB2Label);
            glycerolipidsTab.Controls.Add(glDB3Label);
            glycerolipidsTab.Controls.Add(glHydroxyl1Label);
            glycerolipidsTab.Controls.Add(glHydroxyl2Label);
            glycerolipidsTab.Controls.Add(glHydroxyl3Label);
            glycerolipidsTab.Controls.Add(glRepresentativeFA);
            glycerolipidsTab.Controls.Add(glPositiveAdduct);
            glycerolipidsTab.Controls.Add(glNegativeAdduct);
            glycerolipidsTab.Controls.Add(easterText);
            glycerolipidsTab.Parent = tabControl;
            glycerolipidsTab.Text = "Glycerolipids";
            glycerolipidsTab.Location = new Point(0, 0);
            glycerolipidsTab.Size = this.Size;
            glycerolipidsTab.AutoSize = true;
            glycerolipidsTab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            glycerolipidsTab.BackColor = Color.White;
            Font glFont = new Font(glycerolipidsTab.Font.FontFamily, 8.25F);
            glycerolipidsTab.Font = glFont;
            

            
            
            easterText.Location = new Point(1030, 5);
            easterText.Text = "Fat is unfair, it sticks 2 minutes in your mouth, 2 hours in your stomach and 2 decades at your hips.";
            easterText.Visible = false;
            easterText.Font = new Font(new Font(easterText.Font.FontFamily, 40), FontStyle.Bold);
            easterText.AutoSize = true;
            

            glFA1Combobox.BringToFront();
            glFA1Textbox.BringToFront();
            glFA1Textbox.Location = new Point(196, 130);
            glFA1Textbox.Width = faLength;
            glFA1Textbox.Text = "0, 2, 4, 6-7";
            glFA1Textbox.TextChanged += new EventHandler(glFA1TextboxValueChanged);
            toolTip.SetToolTip(glFA1Textbox, formattingFA);
            glFA1Combobox.Location = new Point(glFA1Textbox.Left, glFA1Textbox.Top - sepText);
            glFA1Combobox.Width = faLength;
            glFA1Combobox.SelectedItem = "Fatty acid chain";
            glFA1Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            glFA1Combobox.SelectedIndexChanged += new EventHandler(glFA1ComboboxValueChanged);
            glDB1Textbox.Location = new Point(glFA1Textbox.Left + glFA1Textbox.Width + sep, glFA1Textbox.Top);
            glDB1Textbox.Width = dbLength;
            glDB1Textbox.Text = "0-2";
            glDB1Textbox.TextChanged += new EventHandler(glDB1TextboxValueChanged);
            toolTip.SetToolTip(glDB1Textbox, formattingDB);
            glDB1Label.Location = new Point(glDB1Textbox.Left, glDB1Textbox.Top - sep);
            glDB1Label.Width = dbLength;
            glDB1Label.Text = dbText;
            glHydroxyl1Textbox.Width = dbLength;
            glHydroxyl1Textbox.Location = new Point(glDB1Textbox.Left + glDB1Textbox.Width + sep, glDB1Textbox.Top);
            glHydroxyl1Textbox.TextChanged += new EventHandler(glHydroxyl1TextboxValueChanged);
            toolTip.SetToolTip(glHydroxyl1Textbox, formattingHydroxyl);
            glHydroxyl1Label.Width = dbLength;
            glHydroxyl1Label.Location = new Point(glHydroxyl1Textbox.Left, glHydroxyl1Textbox.Top - sep);
            glHydroxyl1Label.Text = hydroxylText;

            glFA1Checkbox3.Location = new Point(glFA1Textbox.Left + 90, glFA1Textbox.Top + glFA1Textbox.Height);
            glFA1Checkbox3.Text = "FAe";
            glFA1Checkbox3.CheckedChanged += new EventHandler(glFA1Checkbox3CheckedChanged);
            glFA1Checkbox3.MouseLeave += new System.EventHandler(glFA1Checkbox3MouseLeave);
            glFA1Checkbox3.MouseMove += new System.Windows.Forms.MouseEventHandler(glFA1Checkbox3MouseHover);
            glFA1Checkbox2.Location = new Point(glFA1Textbox.Left + 40, glFA1Textbox.Top + glFA1Textbox.Height);
            glFA1Checkbox2.Text = "FAp";
            toolTip.SetToolTip(glFA1Checkbox2, FApInformation);
            glFA1Checkbox2.CheckedChanged += new EventHandler(glFA1Checkbox2CheckedChanged);
            glFA1Checkbox2.MouseLeave += new System.EventHandler(glFA1Checkbox2MouseLeave);
            glFA1Checkbox2.MouseMove += new System.Windows.Forms.MouseEventHandler(glFA1Checkbox2MouseHover);
            glFA1Checkbox1.Location = new Point(glFA1Textbox.Left, glFA1Textbox.Top + glFA1Textbox.Height);
            glFA1Checkbox1.Text = "FA";
            glFA1Checkbox1.Checked = true;
            glFA1Checkbox1.CheckedChanged += new EventHandler(glFA1Checkbox1CheckedChanged);

            glFA2Combobox.BringToFront();
            glFA2Textbox.BringToFront();
            glFA2Textbox.Location = new Point(290, 202);
            glFA2Textbox.Width = faLength;
            glFA2Textbox.Text = "0, 5, 17-19";
            glFA2Textbox.TextChanged += new EventHandler(glFA2TextboxValueChanged);
            toolTip.SetToolTip(glFA2Textbox, formattingFA);
            glFA2Combobox.Location = new Point(glFA2Textbox.Left, glFA2Textbox.Top - sepText);
            glFA2Combobox.Width = faLength;
            glFA2Combobox.SelectedItem = "Fatty acid chain";
            glFA2Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            glFA2Combobox.SelectedIndexChanged += new EventHandler(glFA2ComboboxValueChanged);
            glDB2Textbox.Location = new Point(glFA2Textbox.Left + glFA2Textbox.Width + sep, glFA2Textbox.Top);
            glDB2Textbox.Width = dbLength;
            glDB2Textbox.Text = "5-6";
            glDB2Textbox.TextChanged += new EventHandler(glDB2TextboxValueChanged);
            toolTip.SetToolTip(glDB2Textbox, formattingDB);
            glDB2Label.Location = new Point(glDB2Textbox.Left, glDB2Textbox.Top - sep);
            glDB2Label.Width = dbLength;
            glDB2Label.Text = dbText;
            glHydroxyl2Textbox.Width = dbLength;
            glHydroxyl2Textbox.Location = new Point(glDB2Textbox.Left + glDB2Textbox.Width + sep, glDB2Textbox.Top);
            glHydroxyl2Textbox.TextChanged += new EventHandler(glHydroxyl2TextboxValueChanged);
            toolTip.SetToolTip(glHydroxyl2Textbox, formattingHydroxyl);
            glHydroxyl2Label.Width = dbLength;
            glHydroxyl2Label.Location = new Point(glHydroxyl2Textbox.Left, glHydroxyl2Textbox.Top - sep);
            glHydroxyl2Label.Text = hydroxylText;

            glFA2Checkbox3.Location = new Point(glFA2Textbox.Left + 90, glFA2Textbox.Top + glFA2Textbox.Height);
            glFA2Checkbox3.Text = "FAe";
            glFA2Checkbox3.CheckedChanged += new EventHandler(glFA2Checkbox3CheckedChanged);
            glFA2Checkbox3.MouseLeave += new System.EventHandler(glFA2Checkbox3MouseLeave);
            glFA2Checkbox3.MouseMove += new System.Windows.Forms.MouseEventHandler(glFA2Checkbox3MouseHover);
            glFA2Checkbox2.Location = new Point(glFA2Textbox.Left + 40, glFA2Textbox.Top + glFA2Textbox.Height);
            glFA2Checkbox2.Text = "FAp";
            toolTip.SetToolTip(glFA2Checkbox2, FApInformation);
            glFA2Checkbox2.CheckedChanged += new EventHandler(glFA2Checkbox2CheckedChanged);
            glFA2Checkbox2.MouseLeave += new System.EventHandler(glFA2Checkbox2MouseLeave);
            glFA2Checkbox2.MouseMove += new System.Windows.Forms.MouseEventHandler(glFA2Checkbox2MouseHover);
            glFA2Checkbox1.Location = new Point(glFA2Textbox.Left, glFA2Textbox.Top + glFA2Textbox.Height);
            glFA2Checkbox1.Text = "FA";
            glFA2Checkbox1.Checked = true;
            glFA2Checkbox1.CheckedChanged += new EventHandler(glFA2Checkbox1CheckedChanged);

            glFA3Combobox.BringToFront();
            glFA3Textbox.BringToFront();
            glFA3Textbox.Location = new Point(158, 302);
            glFA3Textbox.Width = faLength;
            glFA3Textbox.Text = "20-22";
            glFA3Textbox.TextChanged += new EventHandler(glFA3TextboxValueChanged);
            toolTip.SetToolTip(glFA3Textbox, formattingFA);
            glFA3Combobox.Location = new Point(glFA3Textbox.Left, glFA3Textbox.Top - sepText);
            glFA3Combobox.Width = faLength;
            glFA3Combobox.SelectedItem = "Fatty acid chain";
            glFA3Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            glFA3Combobox.SelectedIndexChanged += new EventHandler(glFA3ComboboxValueChanged);
            glDB3Textbox.Location = new Point(glFA3Textbox.Left + glFA3Textbox.Width + sep, glFA3Textbox.Top);
            glDB3Textbox.Width = dbLength;
            glDB3Textbox.Text = "0";
            glDB3Textbox.TextChanged += new EventHandler(glDB3TextboxValueChanged);
            toolTip.SetToolTip(glDB3Textbox, formattingDB);
            glDB3Label.Location = new Point(glDB3Textbox.Left, glDB3Textbox.Top - sep);
            glDB3Label.Width = dbLength;
            glDB3Label.Text = dbText;
            glHydroxyl3Textbox.Width = dbLength;
            glHydroxyl3Textbox.Location = new Point(glDB3Textbox.Left + glDB3Textbox.Width + sep, glDB3Textbox.Top);
            glHydroxyl3Textbox.TextChanged += new EventHandler(glHydroxyl3TextboxValueChanged);
            toolTip.SetToolTip(glHydroxyl3Textbox, formattingHydroxyl);
            glHydroxyl3Label.Width = dbLength;
            glHydroxyl3Label.Location = new Point(glHydroxyl3Textbox.Left, glHydroxyl3Textbox.Top - sep);
            glHydroxyl3Label.Text = hydroxylText;

            glFA3Checkbox3.Location = new Point(glFA3Textbox.Left + 90, glFA3Textbox.Top + glFA3Textbox.Height);
            glFA3Checkbox3.Text = "FAe";
            glFA3Checkbox3.CheckedChanged += new EventHandler(glFA3Checkbox3CheckedChanged);
            glFA3Checkbox3.MouseLeave += new System.EventHandler(glFA3Checkbox3MouseLeave);
            glFA3Checkbox3.MouseMove += new System.Windows.Forms.MouseEventHandler(glFA3Checkbox3MouseHover);
            glFA3Checkbox2.Location = new Point(glFA3Textbox.Left + 40, glFA3Textbox.Top + glFA3Textbox.Height);
            glFA3Checkbox2.Text = "FAp";
            toolTip.SetToolTip(glFA3Checkbox2, FApInformation);
            glFA3Checkbox2.CheckedChanged += new EventHandler(glFA3Checkbox2CheckedChanged);
            glFA3Checkbox2.MouseLeave += new System.EventHandler(glFA3Checkbox2MouseLeave);
            glFA3Checkbox2.MouseMove += new System.Windows.Forms.MouseEventHandler(glFA3Checkbox2MouseHover);
            glFA3Checkbox1.Location = new Point(glFA3Textbox.Left, glFA3Textbox.Top + glFA3Textbox.Height);
            glFA3Checkbox1.Text = "FA";
            glFA3Checkbox1.Checked = true;
            glFA3Checkbox1.CheckedChanged += new EventHandler(glFA3Checkbox1CheckedChanged);

            
            glHgListbox.Location = new Point(132, 288);
            glHgListbox.Size = new Size(70, 50);
            glHgListbox.BringToFront();
            glHgListbox.BorderStyle = BorderStyle.Fixed3D;
            glHgListbox.SelectionMode = SelectionMode.MultiSimple;
            glHgListbox.SelectedValueChanged += new System.EventHandler(glHGListboxSelectedValueChanged);
            glHgListbox.Visible = false;
            
            glHGLabel.Location = new Point(glHgListbox.Left, glHgListbox.Top - sep);
            glHGLabel.Text = "Sugar head";
            glHGLabel.DoubleClick += new EventHandler(sugarHeady);
            glHGLabel.Visible = false;
            

            glPositiveAdduct.Width = 120;
            glPositiveAdduct.Location = new Point(leftGroupboxes - glPositiveAdduct.Width, 60);
            glPositiveAdduct.Height = 120;
            glPositiveAdduct.Text = "Positive adducts";
            glPositiveAdduct.DoubleClick += new EventHandler(triggerEasteregg);
            glPosAdductCheckbox1.Parent = glPositiveAdduct;
            glPosAdductCheckbox1.Location = new Point(10, 15);
            glPosAdductCheckbox1.Text = "+H⁺";
            glPosAdductCheckbox1.CheckedChanged += new EventHandler(glPosAdductCheckbox1CheckedChanged);
            glPosAdductCheckbox1.Enabled = false;
            glPosAdductCheckbox2.Parent = glPositiveAdduct;
            glPosAdductCheckbox2.Location = new Point(10, 35);
            glPosAdductCheckbox2.Text = "+2H⁺⁺";
            glPosAdductCheckbox2.Enabled = false;
            glPosAdductCheckbox2.CheckedChanged += new EventHandler(glPosAdductCheckbox2CheckedChanged);
            glPosAdductCheckbox3.Parent = glPositiveAdduct;
            glPosAdductCheckbox3.Location = new Point(10, 55);
            glPosAdductCheckbox3.Text = "+NH4⁺";
            glPosAdductCheckbox3.Checked = true;
            glPosAdductCheckbox3.CheckedChanged += new EventHandler(glPosAdductCheckbox3CheckedChanged);
            glNegativeAdduct.Width = 120;
            glNegativeAdduct.Location = new Point(leftGroupboxes - glNegativeAdduct.Width, 200);
            glNegativeAdduct.Height = 120;
            glNegativeAdduct.Text = "Negative adducts";
            glNegAdductCheckbox1.Parent = glNegativeAdduct;
            glNegAdductCheckbox1.Location = new Point(10, 15);
            glNegAdductCheckbox1.Text = "-H⁻";
            glNegAdductCheckbox1.Enabled = false;
            glNegAdductCheckbox1.CheckedChanged += new EventHandler(glNegAdductCheckbox1CheckedChanged);
            glNegAdductCheckbox2.Parent = glNegativeAdduct;
            glNegAdductCheckbox2.Location = new Point(10, 35);
            glNegAdductCheckbox2.Text = "-2H⁻ ⁻";
            glNegAdductCheckbox2.Enabled = false;
            glNegAdductCheckbox2.CheckedChanged += new EventHandler(glNegAdductCheckbox2CheckedChanged);
            glNegAdductCheckbox3.Parent = glNegativeAdduct;
            glNegAdductCheckbox3.Location = new Point(10, 55);
            glNegAdductCheckbox3.Text = "+HCOO⁻";
            glNegAdductCheckbox3.Enabled = false;
            glNegAdductCheckbox3.CheckedChanged += new EventHandler(glNegAdductCheckbox3CheckedChanged);
            glNegAdductCheckbox4.Parent = glNegativeAdduct;
            glNegAdductCheckbox4.Location = new Point(10, 75);
            glNegAdductCheckbox4.Text = "+CH3COO⁻";
            glNegAdductCheckbox4.Enabled = false;
            glNegAdductCheckbox4.CheckedChanged += new EventHandler(glNegAdductCheckbox4CheckedChanged);

            glPictureBox.Image = glyceroBackboneImage;
            glPictureBox.Location = new Point(77, 79);
            glPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            glPictureBox.SendToBack();

            glPictureBox.Image = glyceroBackboneImage;
            glPictureBox.Location = new Point(77, 79);
            glPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            glPictureBox.SendToBack();

            glArrow.Image = glArrowImage;
            glArrow.Location = new Point(10, 90);
            glArrow.SizeMode = PictureBoxSizeMode.AutoSize;
            glArrow.SendToBack();


            glContainsSugar.Location = new Point(158, 350);
            glContainsSugar.Width = 120;
            glContainsSugar.Text = "Contains sugar";
            glContainsSugar.CheckedChanged += new EventHandler(glContainsSugarCheckedChanged);
            glContainsSugar.BringToFront();
            
            glRepresentativeFA.Location = new Point(glHydroxyl1Textbox.Left + glHydroxyl1Textbox.Width + sep, glHydroxyl1Textbox.Top);
            glRepresentativeFA.Width = 120;
            glRepresentativeFA.Text = "First FA representative";
            glRepresentativeFA.CheckedChanged += new EventHandler(glRepresentativeFACheckedChanged);
            glRepresentativeFA.SendToBack();


            // tab for phospholipids
            phospholipidsTab.Controls.Add(plFA1Checkbox3);
            phospholipidsTab.Controls.Add(plFA1Checkbox2);
            phospholipidsTab.Controls.Add(plFA1Checkbox1);
            //phospholipidsTab.Controls.Add(plFA2Checkbox3);
            //phospholipidsTab.Controls.Add(plFA2Checkbox2);
            phospholipidsTab.Controls.Add(plFA2Checkbox1);
            phospholipidsTab.Controls.Add(plIsCL);
            phospholipidsTab.Controls.Add(plPictureBox);
            phospholipidsTab.Controls.Add(plFA1Textbox);
            phospholipidsTab.Controls.Add(plFA2Textbox);
            phospholipidsTab.Controls.Add(plDB1Textbox);
            phospholipidsTab.Controls.Add(plDB2Textbox);
            phospholipidsTab.Controls.Add(plHydroxyl1Textbox);
            phospholipidsTab.Controls.Add(plHydroxyl2Textbox);
            phospholipidsTab.Controls.Add(plFA1Combobox);
            phospholipidsTab.Controls.Add(plFA2Combobox);
            phospholipidsTab.Controls.Add(plDB1Label);
            phospholipidsTab.Controls.Add(plDB2Label);
            phospholipidsTab.Controls.Add(plHydroxyl1Label);
            phospholipidsTab.Controls.Add(plHydroxyl2Label);
            phospholipidsTab.Controls.Add(plHgListbox);
            phospholipidsTab.Controls.Add(plHGLabel);
            phospholipidsTab.Controls.Add(plRepresentativeFA);
            phospholipidsTab.Controls.Add(plPositiveAdduct);
            phospholipidsTab.Controls.Add(plNegativeAdduct);
            phospholipidsTab.Parent = tabControl;
            phospholipidsTab.Text = "Phospholipids";
            phospholipidsTab.Location = new Point(0, 0);
            phospholipidsTab.Size = this.Size;
            phospholipidsTab.AutoSize = true;
            phospholipidsTab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            phospholipidsTab.BackColor = Color.White;

            plFA1Combobox.BringToFront();
            plFA1Textbox.BringToFront();
            plFA1Textbox.Location = new Point(400, 64);
            plFA1Textbox.Width = faLength;
            plFA1Textbox.Text = "0, 2, 4, 6-7";
            plFA1Textbox.TextChanged += new EventHandler(plFA1TextboxValueChanged);
            toolTip.SetToolTip(plFA1Textbox, formattingFA);
            plFA1Combobox.Location = new Point(plFA1Textbox.Left, plFA1Textbox.Top - sepText);
            plFA1Combobox.Width = faLength;
            plFA1Combobox.SelectedItem = "Fatty acid chain";
            plFA1Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            plFA1Combobox.SelectedIndexChanged += new EventHandler(plFA1ComboboxValueChanged);
            plDB1Textbox.Location = new Point(plFA1Textbox.Left + plFA1Textbox.Width + sep, plFA1Textbox.Top);
            plDB1Textbox.Width = dbLength;
            plDB1Textbox.Text = "0-2";
            plDB1Textbox.TextChanged += new EventHandler(plDB1TextboxValueChanged);
            toolTip.SetToolTip(plDB1Textbox, formattingDB);
            plDB1Label.Location = new Point(plDB1Textbox.Left, plDB1Textbox.Top - sep);
            plDB1Label.Width = dbLength;
            plDB1Label.Text = dbText;
            plHydroxyl1Textbox.Width = dbLength;
            plHydroxyl1Textbox.Location = new Point(plDB1Textbox.Left + plDB1Textbox.Width + sep, plDB1Textbox.Top);
            plHydroxyl1Textbox.TextChanged += new EventHandler(plHydroxyl1TextboxValueChanged);
            toolTip.SetToolTip(plHydroxyl1Textbox, formattingHydroxyl);
            plHydroxyl1Label.Width = dbLength;
            plHydroxyl1Label.Location = new Point(plHydroxyl1Textbox.Left, plHydroxyl1Textbox.Top - sep);
            plHydroxyl1Label.Text = hydroxylText;

            plFA1Checkbox3.Location = new Point(plFA1Textbox.Left + 90, plFA1Textbox.Top + plFA1Textbox.Height);
            plFA1Checkbox3.Text = "FAe";
            plFA1Checkbox3.CheckedChanged += new EventHandler(plFA1Checkbox3CheckedChanged);
            plFA1Checkbox3.MouseLeave += new System.EventHandler(plFA1Checkbox3MouseLeave);
            plFA1Checkbox3.MouseMove += new System.Windows.Forms.MouseEventHandler(plFA1Checkbox3MouseHover);
            plFA1Checkbox2.Location = new Point(plFA1Textbox.Left + 40, plFA1Textbox.Top + plFA1Textbox.Height);
            plFA1Checkbox2.Text = "FAp";
            toolTip.SetToolTip(plFA1Checkbox2, FApInformation);
            plFA1Checkbox2.CheckedChanged += new EventHandler(plFA1Checkbox2CheckedChanged);
            plFA1Checkbox2.MouseLeave += new System.EventHandler(plFA1Checkbox2MouseLeave);
            plFA1Checkbox2.MouseMove += new System.Windows.Forms.MouseEventHandler(plFA1Checkbox2MouseHover);
            plFA1Checkbox1.Location = new Point(plFA1Textbox.Left, plFA1Textbox.Top + plFA1Textbox.Height);
            plFA1Checkbox1.Text = "FA";
            plFA1Checkbox1.Checked = true;
            plFA1Checkbox1.CheckedChanged += new EventHandler(plFA1Checkbox1CheckedChanged);

            plFA2Combobox.BringToFront();
            plFA2Textbox.BringToFront();
            plFA2Textbox.Location = new Point(312, 144);
            plFA2Textbox.Width = faLength;
            plFA2Textbox.Text = "2, 5, 17-19";
            plFA2Textbox.TextChanged += new EventHandler(plFA2TextboxValueChanged);
            toolTip.SetToolTip(plFA2Textbox, formattingFA);
            plFA2Combobox.Location = new Point(plFA2Textbox.Left, plFA2Textbox.Top - sepText);
            plFA2Combobox.Width = faLength;
            plFA2Combobox.SelectedItem = "Fatty acid chain";
            plFA2Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            plFA2Combobox.SelectedIndexChanged += new EventHandler(plFA2ComboboxValueChanged);
            plDB2Textbox.Location = new Point(plFA2Textbox.Left + plFA2Textbox.Width + sep, plFA2Textbox.Top);
            plDB2Textbox.Width = dbLength;
            plDB2Textbox.Text = "5-6";
            plDB2Textbox.TextChanged += new EventHandler(plDB2TextboxValueChanged);
            toolTip.SetToolTip(plDB2Textbox, formattingDB);
            plDB2Label.Location = new Point(plDB2Textbox.Left, plDB2Textbox.Top - sep);
            plDB2Label.Width = dbLength;
            plDB2Label.Text = dbText;
            plHydroxyl2Textbox.Width = dbLength;
            plHydroxyl2Textbox.Location = new Point(plDB2Textbox.Left + plDB2Textbox.Width + sep, plDB2Textbox.Top);
            plHydroxyl2Textbox.TextChanged += new EventHandler(plHydroxyl2TextboxValueChanged);
            toolTip.SetToolTip(plHydroxyl2Textbox, formattingHydroxyl);
            plHydroxyl2Label.Width = dbLength;
            plHydroxyl2Label.Location = new Point(plHydroxyl2Textbox.Left, plHydroxyl2Textbox.Top - sep);
            plHydroxyl2Label.Text = hydroxylText;

            plFA2Checkbox3.Location = new Point(plFA2Textbox.Left + 90, plFA2Textbox.Top + plFA2Textbox.Height);
            plFA2Checkbox3.Text = "FAe";
            plFA2Checkbox3.CheckedChanged += new EventHandler(plFA2Checkbox3CheckedChanged);
            plFA2Checkbox3.MouseLeave += new System.EventHandler(plFA2Checkbox3MouseLeave);
            plFA2Checkbox3.MouseMove += new System.Windows.Forms.MouseEventHandler(plFA2Checkbox3MouseHover);
            plFA2Checkbox2.Location = new Point(plFA2Textbox.Left + 40, plFA2Textbox.Top + plFA2Textbox.Height);
            plFA2Checkbox2.Text = "FAp";
            toolTip.SetToolTip(plFA1Checkbox2, FApInformation);
            plFA2Checkbox2.CheckedChanged += new EventHandler(plFA2Checkbox2CheckedChanged);
            plFA2Checkbox2.MouseLeave += new System.EventHandler(plFA2Checkbox2MouseLeave);
            plFA2Checkbox2.MouseMove += new System.Windows.Forms.MouseEventHandler(plFA2checkbox2MouseHover);
            plFA2Checkbox1.Location = new Point(plFA2Textbox.Left, plFA2Textbox.Top + plFA2Textbox.Height);
            plFA2Checkbox1.Text = "FA";
            plFA2Checkbox1.Checked = true;
            plFA2Checkbox1.CheckedChanged += new EventHandler(plF2Checkbox1CheckedChanged);
            
            plIsCL.Location = new Point(130, 36);
            plIsCL.Text = "Is cardiolipin";
            plIsCL.CheckedChanged += new EventHandler(plIsCLCheckedChanged);
            plIsCL.BringToFront();

            
            plHgListbox.Location = new Point(25, 50);
            plHgListbox.Size = new Size(70, 190);
            plHgListbox.BringToFront();
            plHgListbox.BorderStyle = BorderStyle.Fixed3D;
            plHgListbox.SelectionMode = SelectionMode.MultiSimple;
            plHgListbox.SelectedValueChanged += new System.EventHandler(plHGListboxSelectedValueChanged);
            plHgListbox.MouseLeave += new System.EventHandler(plHGListboxMouseLeave);
            plHgListbox.MouseMove += new System.Windows.Forms.MouseEventHandler(plHGListboxMouseHover);
            
            plHGLabel.Location = new Point(plHgListbox.Left, plHgListbox.Top - sep);
            plHGLabel.Text = "Head group";

            plPositiveAdduct.Width = 120;
            plPositiveAdduct.Location = new Point(leftGroupboxes - plPositiveAdduct.Width, 60);
            plPositiveAdduct.Height = 120;
            plPositiveAdduct.Text = "Positive adducts";
            plPosAdductCheckbox1.Parent = plPositiveAdduct;
            plPosAdductCheckbox1.Location = new Point(10, 15);
            plPosAdductCheckbox1.Text = "+H⁺";
            plPosAdductCheckbox1.Checked = true;
            plPosAdductCheckbox1.CheckedChanged += new EventHandler(plPosAdductCheckbox1CheckedChanged);
            plPosAdductCheckbox2.Parent = plPositiveAdduct;
            plPosAdductCheckbox2.Location = new Point(10, 35);
            plPosAdductCheckbox2.Text = "+2H⁺⁺";
            plPosAdductCheckbox2.Enabled = false;
            plPosAdductCheckbox2.CheckedChanged += new EventHandler(plPosAdductCheckbox2CheckedChanged);
            plPosAdductCheckbox3.Parent = plPositiveAdduct;
            plPosAdductCheckbox3.Location = new Point(10, 55);
            plPosAdductCheckbox3.Text = "+NH4⁺";
            plPosAdductCheckbox3.CheckedChanged += new EventHandler(plPosAdductCheckbox3CheckedChanged);
            plNegativeAdduct.Width = 120;
            plNegativeAdduct.Location = new Point(leftGroupboxes - plNegativeAdduct.Width, 200);
            plNegativeAdduct.Height = 120;
            plNegativeAdduct.Text = "Negative adducts";
            plNegAdductCheckbox1.Parent = plNegativeAdduct;
            plNegAdductCheckbox1.Location = new Point(10, 15);
            plNegAdductCheckbox1.Text = "-H⁻";
            plNegAdductCheckbox1.CheckedChanged += new EventHandler(plNegAdductCheckbox1CheckedChanged);
            plNegAdductCheckbox2.Parent = plNegativeAdduct;
            plNegAdductCheckbox2.Location = new Point(10, 35);
            plNegAdductCheckbox2.Text = "-2H⁻ ⁻";
            plNegAdductCheckbox2.Enabled = false;
            plNegAdductCheckbox2.CheckedChanged += new EventHandler(plNegAdductCheckbox2CheckedChanged);
            plNegAdductCheckbox3.Parent = plNegativeAdduct;
            plNegAdductCheckbox3.Location = new Point(10, 55);
            plNegAdductCheckbox3.Text = "+HCOO⁻";
            plNegAdductCheckbox3.CheckedChanged += new EventHandler(plNegAdductCheckbox3CheckedChanged);
            plNegAdductCheckbox4.Parent = plNegativeAdduct;
            plNegAdductCheckbox4.Location = new Point(10, 75);
            plNegAdductCheckbox4.Text = "+CH3COO⁻";
            plNegAdductCheckbox4.CheckedChanged += new EventHandler(plNegAdductCheckbox4CheckedChanged);


            plPictureBox.Image = phosphoBackboneImage;
            plPictureBox.Location = new Point(107, 13);
            plPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            plPictureBox.SendToBack();
            
            plRepresentativeFA.Location = new Point(clHydroxyl1Textbox.Left + clHydroxyl1Textbox.Width + sep, clHydroxyl1Textbox.Top);
            plRepresentativeFA.Width = 120;
            plRepresentativeFA.Text = "First FA representative";
            plRepresentativeFA.CheckedChanged += new EventHandler(plRepresentativeFACheckedChanged);
            plRepresentativeFA.SendToBack();



            // tab for sphingolipids
            sphingolipidsTab.Controls.Add(slPictureBox);
            sphingolipidsTab.Controls.Add(slLCBTextbox);
            sphingolipidsTab.Controls.Add(slFATextbox);
            sphingolipidsTab.Controls.Add(slDB1Textbox);
            sphingolipidsTab.Controls.Add(slDB2Textbox);
            sphingolipidsTab.Controls.Add(slLCBCombobox);
            sphingolipidsTab.Controls.Add(slFACombobox);
            sphingolipidsTab.Controls.Add(slDB1Label);
            sphingolipidsTab.Controls.Add(slDB2Label);
            sphingolipidsTab.Controls.Add(slHGLabel);
            sphingolipidsTab.Controls.Add(slHgListbox);
            sphingolipidsTab.Controls.Add(slLCBHydroxyCombobox);
            sphingolipidsTab.Controls.Add(slFAHydroxyCombobox);
            sphingolipidsTab.Controls.Add(slFAHydroxyLabel);
            sphingolipidsTab.Controls.Add(slLCBHydroxyLabel);
            sphingolipidsTab.Controls.Add(slPositiveAdduct);
            sphingolipidsTab.Controls.Add(slNegativeAdduct);
            sphingolipidsTab.Parent = tabControl;
            sphingolipidsTab.Text = "Sphingolipids";
            sphingolipidsTab.Location = new Point(0, 0);
            sphingolipidsTab.Size = this.Size;
            sphingolipidsTab.AutoSize = true;
            sphingolipidsTab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            sphingolipidsTab.BackColor = Color.White;
            Font slFont = new Font(sphingolipidsTab.Font.FontFamily, 8.25F);
            sphingolipidsTab.Font = slFont;

            slFACombobox.BringToFront();
            slFATextbox.BringToFront();
            slFATextbox.Location = new Point(258, 280);
            slFATextbox.Width = faLength;
            slFATextbox.Text = "2, 5, 17-19";
            slFATextbox.TextChanged += new EventHandler(slFATextboxValueChanged);
            toolTip.SetToolTip(slFATextbox, formattingFA);
            slFACombobox.Location = new Point(slFATextbox.Left, slFATextbox.Top - sepText);
            slFACombobox.Width = faLength;
            slFACombobox.SelectedItem = "Fatty acid chain";
            slFACombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            slFACombobox.SelectedIndexChanged += new EventHandler(slFAComboboxValueChanged);
            slDB1Textbox.Location = new Point(slFATextbox.Left + slFATextbox.Width + sep, slFATextbox.Top);
            slDB1Textbox.Width = dbLength;
            slDB1Textbox.Text = "5-6";
            slDB1Textbox.TextChanged += new EventHandler(slDB1TextboxValueChanged);
            toolTip.SetToolTip(slDB1Textbox, formattingDB);
            slDB1Label.Location = new Point(slDB1Textbox.Left, slDB1Textbox.Top - sep);
            slDB1Label.Width = dbLength;
            slDB1Label.Text = dbText;
            slFAHydroxyCombobox.Location = new Point(slDB1Textbox.Left + slDB1Textbox.Width + sep, slDB1Textbox.Top);
            slFAHydroxyCombobox.SelectedItem = "2";
            slFAHydroxyCombobox.Width = dbLength;
            slFAHydroxyCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            slFAHydroxyCombobox.SelectedIndexChanged += new EventHandler(slFAHydroxyComboboxValueChanged);
            slFAHydroxyLabel.Location = new Point(slFAHydroxyCombobox.Left, slFAHydroxyCombobox.Top - sep);
            slFAHydroxyLabel.Text = hydroxylText;


            slLCBCombobox.BringToFront();
            slLCBTextbox.BringToFront();
            slLCBHydroxyCombobox.BringToFront();
            slFAHydroxyCombobox.BringToFront();
            slLCBTextbox.Location = new Point(294, 203);
            slLCBTextbox.Width = faLength;
            slLCBTextbox.Text = "14, 16-18, 22";
            slLCBTextbox.TextChanged += new EventHandler(slLCBTextboxValueChanged);
            toolTip.SetToolTip(slLCBTextbox, formattingFA);
            slLCBCombobox.Location = new Point(slLCBTextbox.Left, slLCBTextbox.Top - sepText);
            slLCBCombobox.Width = faLength;
            slLCBCombobox.SelectedItem = "Long chain base";
            slLCBCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            slLCBCombobox.SelectedIndexChanged += new EventHandler(slLCBComboboxValueChanged);
            slDB2Textbox.Location = new Point(slLCBTextbox.Left + slLCBTextbox.Width + sep, slLCBTextbox.Top);
            slDB2Textbox.Width = dbLength;
            slDB2Textbox.Text = "0-2";
            slDB2Textbox.TextChanged += new EventHandler(slDB2TextboxValueChanged);
            toolTip.SetToolTip(slDB2Textbox, formattingDB);
            slDB2Label.Location = new Point(slDB2Textbox.Left, slDB2Textbox.Top - sep);
            slDB2Label.Width = dbLength;
            slDB2Label.Text = dbText;
            slLCBHydroxyCombobox.Location = new Point(slDB2Textbox.Left + slDB2Textbox.Width + sep, slDB2Textbox.Top);
            slLCBHydroxyCombobox.SelectedItem = "2";
            slLCBHydroxyCombobox.Width = dbLength;
            slLCBHydroxyCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            slLCBHydroxyCombobox.SelectedIndexChanged += new EventHandler(slLCBHydroxyComboboxValueChanged);
            slLCBHydroxyLabel.Location = new Point(slLCBHydroxyCombobox.Left, slLCBHydroxyCombobox.Top - sep);
            slLCBHydroxyLabel.Text = hydroxylText;

            slHGLabel.BringToFront();
            slHgListbox.Location = new Point(54, 105);
            slHgListbox.Size = new Size(80, 260);
            slHgListbox.BringToFront();
            slHgListbox.BorderStyle = BorderStyle.Fixed3D;
            slHgListbox.SelectionMode = SelectionMode.MultiSimple;
            slHgListbox.SelectedValueChanged += new System.EventHandler(slHGListboxSelectedValueChanged);
            slHgListbox.MouseLeave += new System.EventHandler(slHGListboxMouseLeave);
            slHgListbox.MouseMove += new System.Windows.Forms.MouseEventHandler(slHGListboxMouseHover);
            slHGLabel.Location = new Point(slHgListbox.Left, slHgListbox.Top - sep);
            slHGLabel.Text = "Head group";
            

            slPositiveAdduct.Width = 120;
            slPositiveAdduct.Location = new Point(leftGroupboxes - slPositiveAdduct.Width, 60);
            slPositiveAdduct.Height = 120;
            slPositiveAdduct.Text = "Positive adducts";
            slPosAdductCheckbox1.Parent = slPositiveAdduct;
            slPosAdductCheckbox1.Location = new Point(10, 15);
            slPosAdductCheckbox1.Text = "+H⁺";
            slPosAdductCheckbox1.Checked = true;
            slPosAdductCheckbox1.CheckedChanged += new EventHandler(slPosAdductCheckbox1CheckedChanged);
            slPosAdductCheckbox2.Parent = slPositiveAdduct;
            slPosAdductCheckbox2.Location = new Point(10, 35);
            slPosAdductCheckbox2.Text = "+2H⁺⁺";
            slPosAdductCheckbox2.Enabled = false;
            slPosAdductCheckbox2.CheckedChanged += new EventHandler(slPosAdductCheckbox2CheckedChanged);
            slPosAdductCheckbox3.Parent = slPositiveAdduct;
            slPosAdductCheckbox3.Location = new Point(10, 55);
            slPosAdductCheckbox3.Text = "+NH4⁺";
            slPosAdductCheckbox3.CheckedChanged += new EventHandler(slPosAdductCheckbox3CheckedChanged);
            slNegativeAdduct.Width = 120;
            slNegativeAdduct.Location = new Point(leftGroupboxes - slNegativeAdduct.Width, 200);
            slNegativeAdduct.Height = 120;
            slNegativeAdduct.Text = "Negative adducts";
            slNegAdductCheckbox1.Parent = slNegativeAdduct;
            slNegAdductCheckbox1.Location = new Point(10, 15);
            slNegAdductCheckbox1.Text = "-H⁻";
            slNegAdductCheckbox1.CheckedChanged += new EventHandler(slNegAdductCheckbox1CheckedChanged);
            slNegAdductCheckbox2.Parent = slNegativeAdduct;
            slNegAdductCheckbox2.Location = new Point(10, 35);
            slNegAdductCheckbox2.Text = "-2H⁻⁻";
            slNegAdductCheckbox2.Enabled = false;
            slNegAdductCheckbox2.CheckedChanged += new EventHandler(slNegAdductCheckbox2CheckedChanged);
            slNegAdductCheckbox3.Parent = slNegativeAdduct;
            slNegAdductCheckbox3.Location = new Point(10, 55);
            slNegAdductCheckbox3.Text = "+HCOO⁻";
            slNegAdductCheckbox3.CheckedChanged += new EventHandler(slNegAdductCheckbox3CheckedChanged);
            slNegAdductCheckbox4.Parent = slNegativeAdduct;
            slNegAdductCheckbox4.Location = new Point(10, 75);
            slNegAdductCheckbox4.Text = "+CH3COO⁻";
            slNegAdductCheckbox4.CheckedChanged += new EventHandler(slNegAdductCheckbox4CheckedChanged);

            slPictureBox.Image = sphingoBackboneImage;
            slPictureBox.Location = new Point((int)(214 - sphingoBackboneImage.Width * 0.5), (int)(204 - sphingoBackboneImage.Height * 0.5));
            slPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            slPictureBox.SendToBack();
            
            
            
            
            // tab for cholesterols
            cholesterollipidsTab.Controls.Add(chPictureBox);
            cholesterollipidsTab.Controls.Add(chPositiveAdduct);
            cholesterollipidsTab.Controls.Add(chNegativeAdduct);
            cholesterollipidsTab.Controls.Add(chContainsEster);
            cholesterollipidsTab.Controls.Add(chFACombobox);
            cholesterollipidsTab.Controls.Add(chFATextbox);
            cholesterollipidsTab.Controls.Add(chDBTextbox);
            cholesterollipidsTab.Controls.Add(chDBLabel);
            cholesterollipidsTab.Controls.Add(chHydroxylTextbox);
            cholesterollipidsTab.Controls.Add(chFAHydroxyLabel);
            
            cholesterollipidsTab.Text = "Cholesterols";
            cholesterollipidsTab.Location = new Point(0, 0);
            cholesterollipidsTab.Size = this.Size;
            cholesterollipidsTab.AutoSize = true;
            cholesterollipidsTab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cholesterollipidsTab.BackColor = Color.White;
            Font cholFont = new Font(cholesterollipidsTab.Font.FontFamily, 8.25F);
            cholesterollipidsTab.Font = cholFont;
            
            chPositiveAdduct.Width = 120;
            chPositiveAdduct.Location = new Point(leftGroupboxes - chPositiveAdduct.Width, 60);
            chPositiveAdduct.Height = 120;
            chPositiveAdduct.Text = "Positive adducts";
            chPosAdductCheckbox1.Parent = chPositiveAdduct;
            chPosAdductCheckbox1.Location = new Point(10, 15);
            chPosAdductCheckbox1.Text = "+H⁺";
            chPosAdductCheckbox1.Enabled = false;
            chPosAdductCheckbox1.CheckedChanged += new EventHandler(chPosAdductCheckbox1CheckedChanged);
            chPosAdductCheckbox2.Parent = chPositiveAdduct;
            chPosAdductCheckbox2.Location = new Point(10, 35);
            chPosAdductCheckbox2.Text = "+2H⁺⁺";
            chPosAdductCheckbox2.Enabled = false;
            chPosAdductCheckbox2.CheckedChanged += new EventHandler(chPosAdductCheckbox2CheckedChanged);
            chPosAdductCheckbox3.Parent = chPositiveAdduct;
            chPosAdductCheckbox3.Location = new Point(10, 55);
            chPosAdductCheckbox3.Text = "+NH4⁺";
            chPosAdductCheckbox3.Checked = true;
            chPosAdductCheckbox3.CheckedChanged += new EventHandler(chPosAdductCheckbox3CheckedChanged);
            chNegativeAdduct.Width = 120;
            chNegativeAdduct.Location = new Point(leftGroupboxes - chNegativeAdduct.Width, 200);
            chNegativeAdduct.Height = 120;
            chNegativeAdduct.Text = "Negative adducts";
            chNegAdductCheckbox1.Parent = chNegativeAdduct;
            chNegAdductCheckbox1.Location = new Point(10, 15);
            chNegAdductCheckbox1.Text = "-H⁻";
            chNegAdductCheckbox1.Enabled = false;
            chNegAdductCheckbox1.CheckedChanged += new EventHandler(chNegAdductCheckbox1CheckedChanged);
            chNegAdductCheckbox2.Parent = chNegativeAdduct;
            chNegAdductCheckbox2.Location = new Point(10, 35);
            chNegAdductCheckbox2.Text = "-2H⁻⁻";
            chNegAdductCheckbox2.Enabled = false;
            chNegAdductCheckbox2.CheckedChanged += new EventHandler(chNegAdductCheckbox2CheckedChanged);
            chNegAdductCheckbox3.Parent = chNegativeAdduct;
            chNegAdductCheckbox3.Location = new Point(10, 55);
            chNegAdductCheckbox3.Text = "+HCOO⁻";
            chNegAdductCheckbox3.Enabled = false;
            chNegAdductCheckbox3.CheckedChanged += new EventHandler(chNegAdductCheckbox3CheckedChanged);
            chNegAdductCheckbox4.Parent = chNegativeAdduct;
            chNegAdductCheckbox4.Location = new Point(10, 75);
            chNegAdductCheckbox4.Text = "+CH3COO⁻";
            chNegAdductCheckbox4.Enabled = false;
            chNegAdductCheckbox4.CheckedChanged += new EventHandler(chNegAdductCheckbox4CheckedChanged);
            
            chPictureBox.Image = cholesterolBackboneImage;
            chPictureBox.Location = new Point(30, 130);
            chPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            chPictureBox.SendToBack();
            
            chContainsEster.Location = new Point(480, 390);
            chContainsEster.Width = 120;
            chContainsEster.Text = "Contains Ester";
            chContainsEster.CheckedChanged += new EventHandler(chContainsEsterCheckedChanged);
            chContainsEster.BringToFront();
            
            chFACombobox.BringToFront();
            chFATextbox.BringToFront();
            chFATextbox.Location = new Point(616, 358);
            chFATextbox.Width = faLength;
            chFATextbox.Text = "2, 5, 17-19";
            chFATextbox.TextChanged += new EventHandler(chFATextboxValueChanged);
            toolTip.SetToolTip(chFATextbox, formattingFA);
            chFACombobox.Location = new Point(chFATextbox.Left, chFATextbox.Top - sepText);
            chFACombobox.Width = faLength;
            chFACombobox.SelectedItem = "Fatty acid chain";
            chFACombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            chFACombobox.SelectedIndexChanged += new EventHandler(chFAComboboxValueChanged);
            chDBTextbox.Location = new Point(chFATextbox.Left + chFATextbox.Width + sep, chFATextbox.Top);
            chDBTextbox.Width = dbLength;
            chDBTextbox.Text = "5-6";
            chDBTextbox.TextChanged += new EventHandler(chDBTextboxValueChanged);
            toolTip.SetToolTip(chDBTextbox, formattingDB);
            chDBLabel.Location = new Point(chDBTextbox.Left, chDBTextbox.Top - sep);
            chDBLabel.Width = dbLength;
            chDBLabel.Text = dbText;
            chHydroxylTextbox.Width = dbLength;
            chHydroxylTextbox.Location = new Point(chDBTextbox.Left + chDBTextbox.Width + sep, chDBTextbox.Top);
            chHydroxylTextbox.TextChanged += new EventHandler(chHydroxylTextboxValueChanged);
            toolTip.SetToolTip(chHydroxylTextbox, formattingHydroxyl);
            chFAHydroxyLabel.Location = new Point(chHydroxylTextbox.Left, chHydroxylTextbox.Top - sep);
            chFAHydroxyLabel.Text = hydroxylText;
            
            chFACombobox.Visible = false;
            chFATextbox.Visible = false;
            chDBTextbox.Visible = false;
            chDBLabel.Visible = false;
            chHydroxylTextbox.Visible = false;
            chFAHydroxyLabel.Visible = false;
            
            
            
            
            
            
            // tab for mediators
            mediatorlipidsTab.Controls.Add(medNegativeAdduct);
            mediatorlipidsTab.Controls.Add(medHgListbox);
            mediatorlipidsTab.Controls.Add(medPictureBox);
            
            mediatorlipidsTab.Text = "Mediators";
            mediatorlipidsTab.Location = new Point(0, 0);
            mediatorlipidsTab.Size = this.Size;
            mediatorlipidsTab.AutoSize = true;
            mediatorlipidsTab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mediatorlipidsTab.BackColor = Color.White;
            Font medFont = new Font(mediatorlipidsTab.Font.FontFamily, 8.25F);
            mediatorlipidsTab.Font = medFont;
            
            medNegativeAdduct.Width = 120;
            medNegativeAdduct.Location = new Point(leftGroupboxes - medNegativeAdduct.Width, 200);
            medNegativeAdduct.Height = 120;
            medNegativeAdduct.Text = "Negative adducts";
            
            medNegAdductCheckbox1.Parent = medNegativeAdduct;
            medNegAdductCheckbox1.Location = new Point(10, 15);
            medNegAdductCheckbox1.Text = "-H⁻";
            medNegAdductCheckbox1.CheckedChanged += new EventHandler(medNegAdductCheckbox1CheckedChanged);
            medNegAdductCheckbox2.Parent = medNegativeAdduct;
            medNegAdductCheckbox2.Location = new Point(10, 35);
            medNegAdductCheckbox2.Text = "-2H⁻⁻";
            medNegAdductCheckbox2.Enabled = false;
            medNegAdductCheckbox3.Parent = medNegativeAdduct;
            medNegAdductCheckbox3.Location = new Point(10, 55);
            medNegAdductCheckbox3.Text = "+HCOO⁻";
            medNegAdductCheckbox3.Enabled = false;
            medNegAdductCheckbox4.Parent = medNegativeAdduct;
            medNegAdductCheckbox4.Location = new Point(10, 75);
            medNegAdductCheckbox4.Text = "+CH3COO⁻";
            medNegAdductCheckbox4.Enabled = false;
            
            medHgListbox.Location = new Point(34, 105);
            medHgListbox.Size = new Size(140, 260);
            medHgListbox.BringToFront();
            medHgListbox.BorderStyle = BorderStyle.Fixed3D;
            medHgListbox.SelectionMode = SelectionMode.MultiSimple;
            medHgListbox.SelectedValueChanged += new System.EventHandler(medHGListboxSelectedValueChanged);
            medHgListbox.MouseLeave += new System.EventHandler(medHGListboxMouseLeave);
            medHgListbox.MouseMove += new System.Windows.Forms.MouseEventHandler(medHGListboxMouseHover);
            
            medPictureBox.Location = new Point(210, 100);
            if (medHgListbox.Items.Count > 0)
            {
                medPictureBox.Image = Image.FromFile(lipidCreator.headgroups[medHgListbox.Items[0].ToString()].pathToImage);
                medPictureBox.Top = 230 - (medPictureBox.Image.Height >> 1);
            }
            medPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            medPictureBox.SendToBack();
            
            

            lipidsGroupbox.Controls.Add(lipidsGridview);
            lipidsGroupbox.Controls.Add(openReviewFormButton);
            lipidsGroupbox.Dock = DockStyle.Bottom;
            lipidsGroupbox.Text = "Lipid list";
            lipidsGroupbox.Height = 180;

            addLipidButton.Text = "Add lipid";
            addLipidButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            addLipidButton.Width = 146;
            addLipidButton.Height = 26;
            addLipidButton.Image = addImage;
            addLipidButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            addLipidButton.Location = new Point(leftGroupboxes - addLipidButton.Width, topLowButtons);
            addLipidButton.BackColor = SystemColors.Control;
            addLipidButton.Click += registerLipid;

            modifyLipidButton.Text = "Modify lipid";
            modifyLipidButton.Width = 130;
            modifyLipidButton.Height = 26;
            modifyLipidButton.Location = new Point(leftGroupboxes - addLipidButton.Width - modifyLipidButton.Width - 10, topLowButtons);
            modifyLipidButton.BackColor = SystemColors.Control;
            modifyLipidButton.Click += modifyLipid;

            MS2fragmentsLipidButton.Text = "MS2 fragments";
            MS2fragmentsLipidButton.Width = 130;
            MS2fragmentsLipidButton.Height = 26;
            MS2fragmentsLipidButton.Location = new Point(20, topLowButtons);
            MS2fragmentsLipidButton.BackColor = SystemColors.Control;
            MS2fragmentsLipidButton.Click += openMS2Form;

            addHeavyIsotopeButton.Text = "Manage heavy isotopes";
            addHeavyIsotopeButton.Width = 150;
            addHeavyIsotopeButton.Height = 26;
            addHeavyIsotopeButton.Location = new Point(160, topLowButtons);
            addHeavyIsotopeButton.BackColor = SystemColors.Control;
            addHeavyIsotopeButton.Click += openHeavyIsotopeForm;

            
            
            lipidsGridview.AutoSize = true;
            lipidsGridview.Dock = DockStyle.Fill;
            lipidsGridview.DataSource = registeredLipidsDatatable;
            lipidsGridview.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            lipidsGridview.AllowUserToResizeColumns = false;
            lipidsGridview.AllowUserToAddRows = false;
            lipidsGridview.Width = this.Width;
            lipidsGridview.AllowUserToResizeRows = false;
            lipidsGridview.ReadOnly = true;
            lipidsGridview.MultiSelect = false;
            lipidsGridview.RowTemplate.Height = 34;
            lipidsGridview.DoubleClick += new EventHandler(lipidsGridviewDoubleClick);
            lipidsGridview.KeyDown += new KeyEventHandler(lipidsGridviewKeydown);
            lipidsGridview.EditMode = DataGridViewEditMode.EditOnEnter;
            lipidsGridview.RowHeadersVisible = false;
            lipidsGridview.ScrollBars = ScrollBars.Vertical;
            lipidsGridview.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(lipidsGridviewDataBindingComplete);
            
            

            openReviewFormButton.Text = "Review Lipids";
            openReviewFormButton.Width = 130;
            openReviewFormButton.BackColor = SystemColors.Control;
            openReviewFormButton.Dock = DockStyle.Bottom;
            openReviewFormButton.Click += openReviewForm;

            this.Controls.Add(tabControl);
            this.Controls.Add(lipidsGroupbox);
            this.Text = "LipidCreator";
            this.MaximizeBox = false;
            this.Padding = new Padding(5);

            DefaultCheckboxBGR = clPosAdductCheckbox1.BackColor.R;
            DefaultCheckboxBGG = clPosAdductCheckbox1.BackColor.G;
            DefaultCheckboxBGB = clPosAdductCheckbox1.BackColor.B;
            
            
            tutorialStart = new Button();
            homeTab.Controls.Add(tutorialStart);
            tutorialStart.Text = "Start tutorial";
            tutorialStart.Width = 150;
            tutorialStart.Height = 26;
            tutorialStart.Location = new Point(40, 40);
            tutorialStart.BackColor = SystemColors.Control;
            tutorialStart.Click += startTutorial1;
            
            tutorialArrow.Size = new Size(120, 160);
            tutorialArrow.Location = new Point(552, 60);
            this.Controls.Add(tutorialArrow);
            
            tutorialWindow.Size = new Size(240, 160);
            tutorialWindow.Location = new Point(40, 60);
            this.Controls.Add(tutorialWindow);
        
            controlElements = new ArrayList(){menuFile, menuOptions, menuHelp, addLipidButton, modifyLipidButton, MS2fragmentsLipidButton, addHeavyIsotopeButton, plFA1Checkbox3, plFA1Checkbox2, plFA1Checkbox1, plFA2Checkbox1, plIsCL, plFA1Textbox, plFA2Textbox, plDB1Textbox, plDB2Textbox, plHydroxyl1Textbox, plHydroxyl2Textbox, plFA1Combobox, plFA2Combobox, plHgListbox, plHGLabel, plRepresentativeFA, plPositiveAdduct, plNegativeAdduct, homeTab, glycerolipidsTab, phospholipidsTab, sphingolipidsTab, cholesterollipidsTab, mediatorlipidsTab};
            
            
        }

        #endregion
    }
}