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
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace LipidCreator
{

    
    public class Overlay : Control
    {
        public Dictionary<string, Image> arrows;
        public Dictionary<string, Point> fixPoints;
        public string direction;
        Bitmap bmp;
        
        
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | 0x20;
                return cp;
            }
        }
    
        public Overlay(string prefixPath)
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            this.BackColor = Color.Transparent;
            direction = "tl";
            arrows = new Dictionary<string, Image>();
            arrows.Add("tl", Image.FromFile(Path.Combine(prefixPath, "images", "Tutorial", "arrow-top-left.png")));
            arrows.Add("tr", Image.FromFile(Path.Combine(prefixPath, "images", "Tutorial", "arrow-top-right.png")));
            arrows.Add("bl", Image.FromFile(Path.Combine(prefixPath, "images", "Tutorial", "arrow-bottom-left.png")));
            arrows.Add("br", Image.FromFile(Path.Combine(prefixPath, "images", "Tutorial", "arrow-bottom-right.png")));
            arrows.Add("lt", Image.FromFile(Path.Combine(prefixPath, "images", "Tutorial", "arrow-left-top.png")));
            arrows.Add("lb", Image.FromFile(Path.Combine(prefixPath, "images", "Tutorial", "arrow-left-bottom.png")));
            arrows.Add("rt", Image.FromFile(Path.Combine(prefixPath, "images", "Tutorial", "arrow-right-top.png")));
            arrows.Add("rb", Image.FromFile(Path.Combine(prefixPath, "images", "Tutorial", "arrow-right-bottom.png")));
            fixPoints = new Dictionary<string, Point>();
            fixPoints.Add("tl", new Point(0, 26));
            fixPoints.Add("tr", new Point(120, 26));
            fixPoints.Add("bl", new Point(0, 134));
            fixPoints.Add("br", new Point(120, 134));
            fixPoints.Add("lt", new Point(26, 0));
            fixPoints.Add("lb", new Point(26, 120));
            fixPoints.Add("rt", new Point(134, 0));
            fixPoints.Add("rb", new Point(134, 120));
        }
        
        public void update(Point location, string dir)
        {
            if (Parent == null) return;
            direction = dir;
            this.Location = new Point(location.X - fixPoints[direction].X, location.Y - fixPoints[direction].Y);
            Size = arrows[direction].Size;
            
            // to create the illusion of transprency for the red arrows, a snapshot is taken from the current
            // frame as the background
            Visible = false;
            
            Rectangle screenRectangle = RectangleToScreen(Parent.ClientRectangle);
            int titleHeight = screenRectangle.Top - Parent.Top;
            int Right = screenRectangle.Left - Parent.Left;

            bmp = new Bitmap(Parent.Width, Parent.Height);
            Parent.DrawToBitmap(bmp, new Rectangle(0, 0, Parent.Width, Parent.Height));
            Bitmap bmpImage = new Bitmap(5000, 5000);
            int x = Math.Max(Location.X + Right, 0);
            int y = Math.Max(Location.Y + titleHeight, 0);
            Rectangle copy = new Rectangle(x, y, Width, Height);
            bmp = bmpImage.Clone(copy, bmp.PixelFormat);
            BackgroundImage = bmp;
            
            Visible = true;
            BringToFront();
            Refresh();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            BringToFront();
            Graphics g = e.Graphics;
            g.DrawImage(arrows[direction], 0, 0, arrows[direction].Size.Width, arrows[direction].Size.Height);
            g.Dispose();
            base.OnPaint(e);
        }
    }
    
    
    public class TutorialWindow : Control
    {
        public Tutorial tutorial;
        public PictureBox closeButton;
        public PictureBox previous;
        public Button next;
        string text;
        string instruction;
        public Label paging;
        public Image previousEnabledImage;
        public Image previousDisabledImage;
        public bool previousEnabled;
    
        public TutorialWindow(Tutorial _tutorial, string prefixPath)
        {
            tutorial = _tutorial;
            BackColor = Color.White;
            
            closeButton = new PictureBox();
            previous = new PictureBox();
            next = new Button();
            text = "";
            instruction = "";
            paging = new Label();
            
            closeButton.Image = Image.FromFile(Path.Combine(prefixPath, "images", "Tutorial", "close-x.png"));
            closeButton.Click += closeTutorialWindow;
            closeButton.Size = closeButton.Image.Size;
            this.Controls.Add(closeButton);
            
            previousEnabledImage = Image.FromFile(Path.Combine(prefixPath, "images", "Tutorial", "previous-enabled.png"));
            previousDisabledImage = Image.FromFile(Path.Combine(prefixPath, "images", "Tutorial", "previous-disabled.png"));
            
            previous.Click += previousTutorialWindow;
            this.Controls.Add(previous);
            
            next.Click += nextTutorialWindow;
            next.Text = "Continue";
            next.Width = 80;
            next.Height = 26;
            next.BackColor = SystemColors.Control;
            this.Controls.Add(next);
            
            paging.Text = " 1 / 20";
            paging.Font = new Font("Arial", 10);
            paging.Size = new Size(50, 14);
            paging.AutoSize = false;    
            paging.TextAlign = ContentAlignment.MiddleRight;
            this.Controls.Add(paging);
        }
        
        
        
        public void update(Size size, Point location, string instr, string txt, bool prevEnabled = true)
        {
            Size = size;
            Location = location;
            instruction = instr;
            text = txt;
            Visible = true;
            previousEnabled = prevEnabled;
            BringToFront();
            Refresh();
        }
        
        
        
        public void closeTutorialWindow(Object sender, EventArgs e)
        {
            tutorial.quitTutorial();
        }
        
        
        
        public void previousTutorialWindow(Object sender, EventArgs e)
        {
            if (previousEnabled) tutorial.nextTutorialStep(false);
        }
        
        
        
        public void nextTutorialWindow(Object sender, EventArgs e)
        {
            if (tutorial.nextEnabled) tutorial.nextTutorialStep(true);
        }
        
        
    
        protected override void OnPaint(PaintEventArgs e)
        {
            // draw border
            Graphics g = e.Graphics;
            Pen blackPen = new Pen(Color.Black, 10);
            g.DrawRectangle(blackPen, 0, 0, this.Size.Width, this.Size.Height);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            
            int fontSizeInstruction = (int)(14.0 * CreatorGUI.FONT_SIZE_FACTOR);
            Font instructionFont = new Font("Arial", fontSizeInstruction, FontStyle.Bold);
            RectangleF instructionRect = new RectangleF(20, 20, this.Size.Width - 40 - next.Size.Width, 40);
            g.DrawString(instruction, instructionFont, drawBrush, instructionRect);
            
            
            int fontSizeText = (int)(13.0 * CreatorGUI.FONT_SIZE_FACTOR);
            Font textFont = new Font("Arial", fontSizeText);
            RectangleF textRect = new RectangleF(20, 80, this.Size.Width - 40, this.Size.Height - 100);
            g.DrawString(text, textFont, drawBrush, textRect);
            g.Dispose();
            
            
            previous.Image = previousEnabled ? previousEnabledImage : previousDisabledImage;
            
            next.Enabled = tutorial.nextEnabled;
            next.Location = new Point(this.Size.Width - next.Size.Width - 20, 40);
            
            paging.Text = tutorial.tutorialStep.ToString() + " / " + tutorial.maxSteps[(int)tutorial.tutorial];
            paging.Location = new Point(this.Size.Width - 20 - paging.Size.Width, this.Size.Height - 15 - paging.Size.Height);
        
            closeButton.Location = new Point(this.Size.Width - 5 - closeButton.Size.Width, 5);
            
            previous.Location = new Point(paging.Location.X - previous.Size.Width - 4, this.Size.Height - 15 - previous.Size.Height);
            previous.Size = previous.Image.Size;
            
            base.OnPaint(e);
        }
    }

    public class CustomPictureBox : PictureBox
    {
        public event EventHandler ImageChanged;
        new public Image Image
        {
            get
            {
                return base.Image;
            }
            set
            {
                if (base.Image != value)
                {
                    base.Image = value;
                    SendToBack();
                    if (this.ImageChanged != null) this.ImageChanged(this, new EventArgs());
                }
            }
        }
    }
    
    
    
    
    
    public class AdductCheckedEventArgs : EventArgs
    {
        public string adduct;
        public Lipid lipid;
        
        public AdductCheckedEventArgs(string _adduct, Lipid _lipid) : base()
        {
            adduct = _adduct;
            lipid = _lipid;
        }
    }
    
    
    
    public class FattyAcidEventArgs : EventArgs
    {
        public FattyAcidGroup fag;
        public string fType;
        public TextBox textbox;
        
        public FattyAcidEventArgs(FattyAcidGroup _fag, string _fType) : base()
        {
            fag = _fag;
            fType = _fType;
            textbox = null;
        }
        
        public FattyAcidEventArgs(FattyAcidGroup _fag, TextBox _textbox) : base()
        {
            fag = _fag;
            fType = "";
            textbox = _textbox;
        }
    }
    
    
    
    public class ImageEventArgs : EventArgs
    {
        public Image image;
        
        public ImageEventArgs(Image _image) : base()
        {
            image = _image;
        }
    }
    
    
    partial class CreatorGUI
    {

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        
        
        
        
        #region Windows Form Designer generated code
        public Image deleteImage;
        public Image editImage;
        public Image addImage;
        public bool initialCall = true;
        
        List<String> stHgList = new List<String>();
        List<String> stEsterHgList = new List<String>();

        [NonSerialized]
        public System.Timers.Timer timerEasterEgg;
        [NonSerialized]
        public System.Windows.Forms.MainMenu mainMenuLipidCreator;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuFile;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuImport;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuImportList;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuImportSettings;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuImportPredefined;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuExport;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuWizard;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuExportSettings;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuDash;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuDash2;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuDash3;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuDash4;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuDash5;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuExit;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuStatistics;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuToolDirectory;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuOptions;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuTranslate;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuCollisionEnergy;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuCollisionEnergyOpt;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuCollisionEnergyNone;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuMS2Fragments;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuIsotopes;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuClearLipidList;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuResetCategory;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuResetLipidCreator;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuHelp;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuAbout;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuLog;
        [NonSerialized]
        public System.Windows.Forms.MenuItem menuDocs;


        [NonSerialized]
        public TabControl tabControl = new TabControl();
        [NonSerialized]
        public TabPage homeTab;
        [NonSerialized]
        public TabPage glycerolipidsTab;
        [NonSerialized]
        public TabPage phospholipidsTab;
        [NonSerialized]
        public TabPage sphingolipidsTab;
        [NonSerialized]
        public TabPage sterollipidsTab;
        [NonSerialized]
        public TabPage mediatorlipidsTab;
        [NonSerialized]
        public GroupBox lipidsGroupbox;
        [NonSerialized]
        public int DefaultCheckboxBGR;
        [NonSerialized]
        public int DefaultCheckboxBGG;
        [NonSerialized]
        public int DefaultCheckboxBGB;

        [NonSerialized]
        public Button addLipidButton;
        [NonSerialized]
        public Button modifyLipidButton;
        [NonSerialized]
        public Button MS2fragmentsLipidButton;
        [NonSerialized]
        public Button addHeavyIsotopeButton;
        [NonSerialized]
        public Button filtersButton;
        [NonSerialized]
        public Button startFirstTutorialButton;
        [NonSerialized]
        public Button startSecondTutorialButton;
        [NonSerialized]
        public Button startThirdTutorialButton;
        public Button startFourthTutorialButton;
        

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
        Image phosphoLysoBackboneImage;
        Image phosphoLysoBackboneImageFA1e;
        Image phosphoLysoBackboneImageFA1p;
        Image sphingoBackboneImage;
        Image sphingoLysoBackboneImage;

        [NonSerialized]
        public CustomPictureBox glPictureBox;
        [NonSerialized]
        public CustomPictureBox plPictureBox;
        [NonSerialized]
        public PictureBox slPictureBox;
        [NonSerialized]
        public CustomPictureBox medPictureBox;
        [NonSerialized]
        public CustomPictureBox stPictureBox;

        [NonSerialized]
        public ListBox glHgListbox;
        [NonSerialized]
        public ListBox plHgListbox;
        [NonSerialized]
        public ListBox slHgListbox;
        [NonSerialized]
        public ListBox medHgListbox;
        [NonSerialized]
        public ListBox stHgListbox;

        [NonSerialized]
        public TextBox clFA3Textbox;
        [NonSerialized]
        public TextBox clFA4Textbox;
        [NonSerialized]
        public TextBox glFA1Textbox;
        [NonSerialized]
        public TextBox glFA2Textbox;
        [NonSerialized]
        public TextBox glFA3Textbox;
        [NonSerialized]
        public TextBox plFA1Textbox;
        [NonSerialized]
        public TextBox plFA2Textbox;
        [NonSerialized]
        public TextBox slLCBTextbox;
        [NonSerialized]
        public TextBox slFATextbox;
        [NonSerialized]
        public TextBox stFATextbox;

        [NonSerialized]
        public ComboBox clFA3Combobox;
        [NonSerialized]
        public ComboBox clFA4Combobox;
        [NonSerialized]
        public ComboBox glFA1Combobox;
        [NonSerialized]
        public ComboBox glFA2Combobox;
        [NonSerialized]
        public ComboBox glFA3Combobox;
        [NonSerialized]
        public ComboBox plFA1Combobox;
        [NonSerialized]
        public ComboBox plFA2Combobox;
        [NonSerialized]
        public ComboBox slLCBCombobox;
        [NonSerialized]
        public ComboBox slFACombobox;
        [NonSerialized]
        public ComboBox stFACombobox;

        [NonSerialized]
        public CheckBox clFA3Checkbox1;
        [NonSerialized]
        public CheckBox clFA3Checkbox2;
        [NonSerialized]
        public CheckBox clFA3Checkbox3;
        [NonSerialized]
        public CheckBox clFA4Checkbox1;
        [NonSerialized]
        public CheckBox clFA4Checkbox2;
        [NonSerialized]
        public CheckBox clFA4Checkbox3;
        [NonSerialized]
        public CheckBox glFA1Checkbox1;
        [NonSerialized]
        public CheckBox glFA1Checkbox2;
        [NonSerialized]
        public CheckBox glFA1Checkbox3;
        [NonSerialized]
        public CheckBox glFA2Checkbox1;
        [NonSerialized]
        public CheckBox glFA2Checkbox2;
        [NonSerialized]
        public CheckBox glFA2Checkbox3;
        [NonSerialized]
        public CheckBox glFA3Checkbox1;
        [NonSerialized]
        public CheckBox glFA3Checkbox2;
        [NonSerialized]
        public CheckBox glFA3Checkbox3;
        [NonSerialized]
        public CheckBox plFA1Checkbox1;
        [NonSerialized]
        public CheckBox plFA1Checkbox2;
        [NonSerialized]
        public CheckBox plFA1Checkbox3;
        [NonSerialized]
        public CheckBox plFA2Checkbox1;
        [NonSerialized]
        public CheckBox plFA2Checkbox2;
        [NonSerialized]
        public CheckBox plFA2Checkbox3;
        [NonSerialized]
        public CheckBox glContainsSugar;
        [NonSerialized]
        public CheckBox plHasPlasmalogen;

        [NonSerialized]
        public GroupBox glPositiveAdduct;
        [NonSerialized]
        public GroupBox glNegativeAdduct;
        [NonSerialized]
        public GroupBox plPositiveAdduct;
        [NonSerialized]
        public GroupBox plNegativeAdduct;
        [NonSerialized]
        public GroupBox slPositiveAdduct;
        [NonSerialized]
        public GroupBox slNegativeAdduct;
        [NonSerialized]
        public GroupBox stPositiveAdduct;
        [NonSerialized]
        public GroupBox stNegativeAdduct;
        [NonSerialized]
        public GroupBox medNegativeAdduct;

        [NonSerialized]
        public GroupBox glStep1;
        [NonSerialized]
        public GroupBox plStep1;
        [NonSerialized]
        public GroupBox slStep1;
        [NonSerialized]
        public GroupBox stStep1;
        [NonSerialized]
        public GroupBox medStep1;
        [NonSerialized]
        public GroupBox lcStep2;
        [NonSerialized]
        public GroupBox lcStep3;

        [NonSerialized]
        public CheckBox glPosAdductCheckbox1;
        [NonSerialized]
        public CheckBox glPosAdductCheckbox2;
        [NonSerialized]
        public CheckBox glPosAdductCheckbox3;
        [NonSerialized]
        public CheckBox glNegAdductCheckbox1;
        [NonSerialized]
        public CheckBox glNegAdductCheckbox2;
        [NonSerialized]
        public CheckBox glNegAdductCheckbox3;
        [NonSerialized]
        public CheckBox glNegAdductCheckbox4;
        [NonSerialized]
        public CheckBox plPosAdductCheckbox1;
        [NonSerialized]
        public CheckBox plPosAdductCheckbox2;
        [NonSerialized]
        public CheckBox plPosAdductCheckbox3;
        [NonSerialized]
        public CheckBox plNegAdductCheckbox1;
        [NonSerialized]
        public CheckBox plNegAdductCheckbox2;
        [NonSerialized]
        public CheckBox plNegAdductCheckbox3;
        [NonSerialized]
        public CheckBox plNegAdductCheckbox4;
        [NonSerialized]
        public CheckBox slPosAdductCheckbox1;
        [NonSerialized]
        public CheckBox slPosAdductCheckbox2;
        [NonSerialized]
        public CheckBox slPosAdductCheckbox3;
        [NonSerialized]
        public CheckBox slNegAdductCheckbox1;
        [NonSerialized]
        public CheckBox slNegAdductCheckbox2;
        [NonSerialized]
        public CheckBox slNegAdductCheckbox3;
        [NonSerialized]
        public CheckBox slNegAdductCheckbox4;
        [NonSerialized]
        public CheckBox stPosAdductCheckbox1;
        [NonSerialized]
        public CheckBox stPosAdductCheckbox2;
        [NonSerialized]
        public CheckBox stPosAdductCheckbox3;
        [NonSerialized]
        public CheckBox stNegAdductCheckbox1;
        [NonSerialized]
        public CheckBox stNegAdductCheckbox2;
        [NonSerialized]
        public CheckBox stNegAdductCheckbox3;
        [NonSerialized]
        public CheckBox stNegAdductCheckbox4;
        [NonSerialized]
        public CheckBox medNegAdductCheckbox1;
        [NonSerialized]
        public CheckBox medNegAdductCheckbox2;
        [NonSerialized]
        public CheckBox medNegAdductCheckbox3;
        [NonSerialized]
        public CheckBox medNegAdductCheckbox4;
        [NonSerialized]
        public CheckBox glRepresentativeFA;
        [NonSerialized]
        public CheckBox plRepresentativeFA;
        Color highlightedCheckboxColor;

        [NonSerialized]
        public TextBox clDB3Textbox;
        [NonSerialized]
        public TextBox clDB4Textbox;
        [NonSerialized]
        public TextBox glDB1Textbox;
        [NonSerialized]
        public TextBox glDB2Textbox;
        [NonSerialized]
        public TextBox glDB3Textbox;
        [NonSerialized]
        public TextBox plDB1Textbox;
        [NonSerialized]
        public TextBox plDB2Textbox;
        [NonSerialized]
        public TextBox slDB1Textbox;
        [NonSerialized]
        public TextBox slDB2Textbox;
        [NonSerialized]
        public TextBox stDBTextbox;

        [NonSerialized]
        public TextBox clHydroxyl3Textbox;
        [NonSerialized]
        public TextBox clHydroxyl4Textbox;
        [NonSerialized]
        public TextBox glHydroxyl1Textbox;
        [NonSerialized]
        public TextBox glHydroxyl2Textbox;
        [NonSerialized]
        public TextBox glHydroxyl3Textbox;
        [NonSerialized]
        public TextBox plHydroxyl1Textbox;
        [NonSerialized]
        public TextBox plHydroxyl2Textbox;
        [NonSerialized]
        public TextBox stHydroxylTextbox;

        [NonSerialized]
        public GroupBox plTypeGroup;
        [NonSerialized]
        public RadioButton plRegular;
        [NonSerialized]
        public RadioButton plIsCL;
        [NonSerialized]
        public RadioButton plIsLyso;

        [NonSerialized]
        public GroupBox slTypeGroup;
        [NonSerialized]
        public RadioButton slRegular;
        [NonSerialized]
        public RadioButton slIsLyso;

        [NonSerialized]
        public GroupBox stTypeGroup;
        [NonSerialized]
        public RadioButton stRegular;
        [NonSerialized]
        public RadioButton stIsEster;

        [NonSerialized]
        Label clDB3Label;
        [NonSerialized]
        Label clDB4Label;
        [NonSerialized]
        Label glDB1Label;
        [NonSerialized]
        Label glDB2Label;
        [NonSerialized]
        Label glDB3Label;
        [NonSerialized]
        Label plDB1Label;
        [NonSerialized]
        Label plDB2Label;
        [NonSerialized]
        Label slDB1Label;
        [NonSerialized]
        Label slDB2Label;
        [NonSerialized]
        Label stDBLabel;
        [NonSerialized]
        Label slLCBHydroxyLabel;
        [NonSerialized]
        Label slFAHydroxyLabel;
        [NonSerialized]
        Label clHydroxyl3Label;
        [NonSerialized]
        Label clHydroxyl4Label;
        [NonSerialized]
        Label glHydroxyl1Label;
        [NonSerialized]
        Label glHydroxyl2Label;
        [NonSerialized]
        Label glHydroxyl3Label;
        [NonSerialized]
        Label plHydroxyl1Label;
        [NonSerialized]
        Label plHydroxyl2Label;
        [NonSerialized]
        Label stFAHydroxyLabel;

        [NonSerialized]
        Label homeText;
        [NonSerialized]
        Label homeText3;

        [NonSerialized]
        public CustomPictureBox glArrow;
        [NonSerialized]
        Image glArrowImage;
        [NonSerialized]
        Label easterText;

        [NonSerialized]
        Label glHGLabel;
        [NonSerialized]
        Label plHGLabel;
        [NonSerialized]
        Label slHGLabel;

        [NonSerialized]
        public ComboBox slLCBHydroxyCombobox;
        [NonSerialized]
        public ComboBox slFAHydroxyCombobox;

        [NonSerialized]
        ToolTip toolTip;

        [NonSerialized]
        Panel lipidsGridviewPanel;
        [NonSerialized]
        Panel lipidsReviewButtonPanel;
        [NonSerialized]
        DataGridView lipidsGridview;
        [NonSerialized]
        public System.Windows.Forms.Button openReviewFormButton;
        
        public ArrayList controlElements;
        
        
        public int minWindowHeight = 720;
        public int minWindowHeightDefault = 720;
        public int minWindowHeightExtended = 800;
        public int windowWidth = 1060;
        public int minLipidGridHeight = 180;
        public int step1Height = 320;
        public int step1HeightExtended = 400;
        public bool windowExtended = false;
        public int mediatorMiddleHeight = 164;
        public long easterEggMilliseconds;
        

        public Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
        
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            this.Font = new Font(Font.Name, REGULAR_FONT_SIZE * FONT_SIZE_FACTOR, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Text = "LipidCreator";
            
            this.timerEasterEgg = new System.Timers.Timer(15);
            this.timerEasterEgg.Elapsed += this.timerEasterEggTick;
            
            this.mainMenuLipidCreator = new System.Windows.Forms.MainMenu();
            this.Menu = this.mainMenuLipidCreator;
            
            this.menuFile = new System.Windows.Forms.MenuItem ();
            this.menuImport = new System.Windows.Forms.MenuItem ();
            this.menuImportList = new System.Windows.Forms.MenuItem ();
            this.menuImportSettings = new System.Windows.Forms.MenuItem ();
            this.menuImportPredefined = new System.Windows.Forms.MenuItem();
            this.menuExport = new System.Windows.Forms.MenuItem ();
            this.menuWizard = new System.Windows.Forms.MenuItem ();
            this.menuExportSettings = new System.Windows.Forms.MenuItem ();
            this.menuDash = new System.Windows.Forms.MenuItem ();
            this.menuDash2 = new System.Windows.Forms.MenuItem ();
            this.menuDash3 = new System.Windows.Forms.MenuItem ();
            this.menuDash4 = new System.Windows.Forms.MenuItem ();
            this.menuDash5 = new System.Windows.Forms.MenuItem ();
            this.menuExit = new System.Windows.Forms.MenuItem ();
            this.menuStatistics = new System.Windows.Forms.MenuItem ();
            this.menuToolDirectory = new System.Windows.Forms.MenuItem();
            this.menuClearLipidList = new System.Windows.Forms.MenuItem ();
            this.menuResetCategory = new System.Windows.Forms.MenuItem ();
            this.menuResetLipidCreator = new System.Windows.Forms.MenuItem ();
            this.menuOptions = new System.Windows.Forms.MenuItem ();
            this.menuTranslate = new System.Windows.Forms.MenuItem ();
            this.menuCollisionEnergy = new System.Windows.Forms.MenuItem ();
            this.menuCollisionEnergyOpt = new System.Windows.Forms.MenuItem ();
            this.menuCollisionEnergyNone = new System.Windows.Forms.MenuItem ();
            this.menuMS2Fragments = new System.Windows.Forms.MenuItem ();
            this.menuIsotopes = new System.Windows.Forms.MenuItem ();
            this.menuFile.MenuItems.AddRange(new MenuItem[]{ menuImport, menuImportList, menuImportPredefined, menuExport, menuDash, menuWizard, menuDash5, menuImportSettings, menuExportSettings, menuDash3, menuExit});
            this.menuFile.Text = "File";
            
            this.menuImport.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
            this.menuImport.Text = "&Import Project";
            this.menuImport.Click += new System.EventHandler (menuImportClick);
            
            this.menuImportList.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
            this.menuImportList.Text = "Import &Lipid List";
            this.menuImportList.Click += new System.EventHandler (menuImportListClick);
            
            this.menuImportSettings.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftI;
            this.menuImportSettings.Text = "&Import Settings";
            this.menuImportSettings.Click += new System.EventHandler (menuImportSettingsClick);
            
            this.menuImportPredefined.Text = "Import Predefined";
            
            this.menuExport.Shortcut = System.Windows.Forms.Shortcut.CtrlE;
            this.menuExport.Text = "&Export Project";
            this.menuExport.Click += new System.EventHandler (menuExportClick);
            
            this.menuExportSettings.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftE;
            this.menuExportSettings.Text = "&Export Settings";
            this.menuExportSettings.Click += new System.EventHandler (menuExportSettingsClick);
            
            this.menuWizard.Shortcut = System.Windows.Forms.Shortcut.CtrlW;
            this.menuWizard.Text = "Run &Wizard";
            this.menuWizard.Click += new System.EventHandler (menuWizardClick);
            
            this.menuDash.Text = "-";
            this.menuDash2.Text = "-";
            this.menuDash3.Text = "-";
            this.menuDash4.Text = "-";
            this.menuDash5.Text = "-";
            
            this.menuExit.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler (menuExitClick);
            
            
            this.menuTranslate.Shortcut = System.Windows.Forms.Shortcut.CtrlT;
            this.menuTranslate.Text = "Lipid name &translator";
            this.menuTranslate.Click += new System.EventHandler (menuTranslateClick);

            this.menuOptions.MenuItems.AddRange(new MenuItem[]{ menuTranslate, menuCollisionEnergy, menuCollisionEnergyOpt, menuMS2Fragments, menuIsotopes, menuDash2, menuClearLipidList, menuResetCategory, menuResetLipidCreator, menuDash4, menuStatistics, menuToolDirectory});
            this.menuOptions.Text = "Options";
            
            this.menuCollisionEnergy.MenuItems.AddRange(new MenuItem[]{ menuCollisionEnergyNone});
            this.menuCollisionEnergy.Text = "Collision Energy computation";
            
            this.menuCollisionEnergyOpt.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.menuCollisionEnergyOpt.Text = "Collision Energy &optimization";
            this.menuCollisionEnergyOpt.Click += new System.EventHandler (menuCollisionEnergyOptClick);
            
            this.menuCollisionEnergyNone.Text = "No computation";
            this.menuCollisionEnergyNone.RadioCheck = true;
            this.menuCollisionEnergyNone.Checked = true;
            this.menuCollisionEnergyNone.Click += new System.EventHandler (unsetInstrument);
            
            this.menuMS2Fragments.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
            this.menuMS2Fragments.Text = "&MS2 fragments";
            this.menuMS2Fragments.Click += new System.EventHandler (openMS2Form);
            
            this.menuIsotopes.Shortcut = System.Windows.Forms.Shortcut.CtrlH;
            this.menuIsotopes.Text = "Manage &heavy isotopes";
            this.menuIsotopes.Click += new System.EventHandler (openHeavyIsotopeForm);
            
            this.menuClearLipidList.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.menuClearLipidList.Text = "&Clear lipid list";
            this.menuClearLipidList.Click += new System.EventHandler (clearLipidList);
            
            this.menuResetCategory.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
            this.menuResetCategory.Text = "Reset &lipid category";
            this.menuResetCategory.Click += new System.EventHandler (resetLipid);
            
            this.menuResetLipidCreator.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
            this.menuResetLipidCreator.Text = "&Reset LipidCreator";
            this.menuResetLipidCreator.Click += new System.EventHandler (resetLipidCreatorMenu);
            
            
            this.menuStatistics.Shortcut = System.Windows.Forms.Shortcut.CtrlU;
            this.menuStatistics.Text = "Send &anonymous statistics";
            this.menuStatistics.Click += new System.EventHandler (statisticsMenu);
            if (!lipidCreatorInitError) this.menuStatistics.Checked = lipidCreator.enableAnalytics;

            this.menuToolDirectory.Text = "Open tool directory";
            this.menuToolDirectory.Click += new System.EventHandler (toolDirectoryMenu);

            this.menuHelp = new System.Windows.Forms.MenuItem ();
            this.menuHelp.Text = "&Help";

            this.menuAbout = new System.Windows.Forms.MenuItem ();
            this.menuAbout.Shortcut = System.Windows.Forms.Shortcut.CtrlB;
            this.menuAbout.Text = "A&bout";
            this.menuAbout.Click += new System.EventHandler (menuAboutClick);

            this.menuLog = new System.Windows.Forms.MenuItem ();
            this.menuLog.Text = "Log messages";
            this.menuLog.Click += new System.EventHandler (menuLogClick);

            this.menuDocs = new System.Windows.Forms.MenuItem();
            this.menuDocs.Text = "Documentation";
            this.menuDocs.Click += new System.EventHandler (menuDocsClick);

            this.menuHelp.MenuItems.AddRange(new MenuItem[]{ this.menuAbout, this.menuLog, this.menuDocs });
            this.mainMenuLipidCreator.MenuItems.AddRange(new MenuItem[] { this.menuFile, this.menuOptions, this.menuHelp } );
            
            tabControl = new TabControl();
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(tabControl_DrawItem);

            
            
            
            this.Size = new System.Drawing.Size(windowWidth, minWindowHeight);
            this.MinimumSize = new System.Drawing.Size(windowWidth, minWindowHeight);
            this.MaximumSize = new System.Drawing.Size(windowWidth, int.MaxValue);
            homeTab = new TabPage();
            glycerolipidsTab = new TabPage();
            phospholipidsTab = new TabPage();
            sphingolipidsTab = new TabPage();
            sterollipidsTab = new TabPage();
            mediatorlipidsTab = new TabPage();
            lipidsGroupbox = new GroupBox();
            addLipidButton = new Button();
            addHeavyIsotopeButton = new Button();
            filtersButton = new Button();
            modifyLipidButton = new Button();
            MS2fragmentsLipidButton = new Button();

            highlightedCheckboxColor = Color.FromArgb(156, 232, 189);
            lipidsGridview = new DataGridView();
            openReviewFormButton = new Button();
            

            glPictureBox = new CustomPictureBox();
            plPictureBox = new CustomPictureBox();
            slPictureBox = new PictureBox();
            medPictureBox = new CustomPictureBox();
            stPictureBox = new CustomPictureBox();
            glArrow = new CustomPictureBox();
            
            String dbText = "No. DB";
            String hydroxylText = "No. Hydroxy";
            int dbLength = 70;
            int sep = 15;
            int sepText = 20;
            int faLength = 150;
            int topLowButtons = 20;
            int leftGroupboxes = 1000;
            int topGroupboxes = 30;

            
            glHgListbox = new ListBox();
            glHgListbox.SelectionMode = SelectionMode.MultiExtended;
            plHgListbox = new ListBox();
            plHgListbox.SelectionMode = SelectionMode.MultiExtended;
            slHgListbox = new ListBox();
            slHgListbox.SelectionMode = SelectionMode.MultiExtended;
            medHgListbox = new ListBox();
            medHgListbox.SelectionMode = SelectionMode.MultiExtended;
            stHgListbox = new ListBox();
            stHgListbox.SelectionMode = SelectionMode.MultiExtended;

            if (!lipidCreatorInitError)
            {
                deleteImage = ScaleImage(Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "delete.png")), 32, 26);
                editImage = ScaleImage(Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "edit.png")), 32, 26);
                addImage = ScaleImage(Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "add.png")), 24, 24);
            
                
                List<String> glHgList = new List<String>();
                foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.Glycerolipid])
                {
                    if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].derivative && !lipidCreator.headgroups[headgroup].attributes.Contains("heavy") && headgroup.Length > 3) glHgList.Add(headgroup);
                }
                glHgList.Sort();
                
                List<String> medHgList = new List<String>();
                foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.LipidMediator])
                {
                    if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].derivative && !lipidCreator.headgroups[headgroup].attributes.Contains("heavy")) medHgList.Add(headgroup);
                }
                medHgList.Sort();
                
                
                
                foreach(string headgroup in lipidCreator.categoryToClass[(int)LipidCategory.Sterollipid])
                {
                    if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].derivative && !lipidCreator.headgroups[headgroup].attributes.Contains("heavy")&& !lipidCreator.headgroups[headgroup].attributes.Contains("ester")) stHgList.Add(headgroup);
                
                    else if (lipidCreator.headgroups.ContainsKey(headgroup) && !lipidCreator.headgroups[headgroup].derivative && !lipidCreator.headgroups[headgroup].attributes.Contains("heavy")&& lipidCreator.headgroups[headgroup].attributes.Contains("ester")) stEsterHgList.Add(headgroup);
                }
                stHgList.Sort();
                stEsterHgList.Sort();
                
                
                
                glHgListbox.Items.AddRange(glHgList.ToArray());
                medHgListbox.Items.AddRange(medHgList.ToArray());
                stHgListbox.Items.AddRange(stHgList.ToArray());
                
                
                
                cardioBackboneImage = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "CL_backbones.png"));
                cardioBackboneImageFA1e = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "CL_FAe1.png"));
                cardioBackboneImageFA2e = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "CL_FAe2.png"));
                cardioBackboneImageFA3e = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "CL_FAe3.png"));
                cardioBackboneImageFA4e = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "CL_FAe4.png"));
                cardioBackboneImageFA1p = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "CL_FAp1.png"));
                cardioBackboneImageFA2p = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "CL_FAp2.png"));
                cardioBackboneImageFA3p = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "CL_FAp3.png"));
                cardioBackboneImageFA4p = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "CL_FAp4.png"));
                glyceroBackboneImageOrig = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "GL_backbones.png"));
                glyceroBackboneImageFA1eOrig = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "GL_FAe1.png"));
                glyceroBackboneImageFA2eOrig = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "GL_FAe2.png"));
                glyceroBackboneImageFA3eOrig = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "GL_FAe3.png"));
                glyceroBackboneImageFA1pOrig = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "GL_FAp1.png"));
                glyceroBackboneImageFA2pOrig = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "GL_FAp2.png"));
                glyceroBackboneImageFA3pOrig = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "GL_FAp3.png"));
                glyceroBackboneImagePlant = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "GL_backbones_plant.png"));
                glyceroBackboneImageFA1ePlant = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "GL_plant_FAe1.png"));
                glyceroBackboneImageFA2ePlant = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "GL_plant_FAe2.png"));
                glyceroBackboneImageFA1pPlant = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "GL_plant_FAp1.png"));
                glyceroBackboneImageFA2pPlant = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "GL_plant_FAp2.png"));
                phosphoBackboneImage = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "PL_backbones.png"));
                phosphoBackboneImageFA1e = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "PL_FAe1.png"));
                phosphoBackboneImageFA2e = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "PL_FAe2.png"));
                phosphoBackboneImageFA1p = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "PL_FAp1.png"));
                phosphoBackboneImageFA2p = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "PL_FAp2.png"));
                phosphoLysoBackboneImage = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "PL_lyso_backbone.png"));
                phosphoLysoBackboneImageFA1e = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "PL_lyso_FAe.png"));
                phosphoLysoBackboneImageFA1p = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "PL_lyso_FAp.png"));
                sphingoBackboneImage = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "SL_backbones.png"));
                sphingoLysoBackboneImage = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "backbones", "SL_backbones_onlyLCB.png"));
                glArrowImage = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "arrow.png"));

                
                glyceroBackboneImage = glyceroBackboneImageOrig;
                glyceroBackboneImageFAe = glyceroBackboneImageFA1eOrig;
                glyceroBackboneImageFA2e = glyceroBackboneImageFA2eOrig;
                glyceroBackboneImageFA3e = glyceroBackboneImageFA3eOrig;
                glyceroBackboneImageFA1p = glyceroBackboneImageFA1pOrig;
                glyceroBackboneImageFA2p = glyceroBackboneImageFA2pOrig;
                glyceroBackboneImageFA3p = glyceroBackboneImageFA3pOrig;
            }
            
            
            clFA3Textbox = new TextBox();
            clFA4Textbox = new TextBox();
            clFA3Combobox = new ComboBox();
            clFA3Combobox.Items.Add("Fatty acyl chain");
            clFA3Combobox.Items.Add("Fatty acyl chain - odd");
            clFA3Combobox.Items.Add("Fatty acyl chain - even");
            clFA4Combobox = new ComboBox();
            clFA4Combobox.Items.Add("Fatty acyl chain");
            clFA4Combobox.Items.Add("Fatty acyl chain - odd");
            clFA4Combobox.Items.Add("Fatty acyl chain - even");
            clDB3Textbox = new TextBox();
            clDB4Textbox = new TextBox();
            clHydroxyl3Textbox = new TextBox();
            clHydroxyl4Textbox = new TextBox();
            clHydroxyl3Label = new Label();
            clHydroxyl4Label = new Label();
            clDB3Label = new Label();
            clDB4Label = new Label();
            glFA1Textbox = new TextBox();
            glFA2Textbox = new TextBox();
            glFA3Textbox = new TextBox();
            glFA1Combobox = new ComboBox();
            glFA1Combobox.Items.Add("Fatty acyl chain");
            glFA1Combobox.Items.Add("Fatty acyl chain - odd");
            glFA1Combobox.Items.Add("Fatty acyl chain - even");
            glFA2Combobox = new ComboBox();
            glFA2Combobox.Items.Add("Fatty acyl chain");
            glFA2Combobox.Items.Add("Fatty acyl chain - odd");
            glFA2Combobox.Items.Add("Fatty acyl chain - even");
            glFA3Combobox = new ComboBox();
            glFA3Combobox.Items.Add("Fatty acyl chain");
            glFA3Combobox.Items.Add("Fatty acyl chain - odd");
            glFA3Combobox.Items.Add("Fatty acyl chain - even");
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
            plFA1Combobox.Items.Add("Fatty acyl chain");
            plFA1Combobox.Items.Add("Fatty acyl chain - odd");
            plFA1Combobox.Items.Add("Fatty acyl chain - even");
            plFA2Combobox = new ComboBox();
            plFA2Combobox.Items.Add("Fatty acyl chain");
            plFA2Combobox.Items.Add("Fatty acyl chain - odd");
            plFA2Combobox.Items.Add("Fatty acyl chain - even");
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
            slLCBCombobox.Items.Add("Long chain base - odd");
            slLCBCombobox.Items.Add("Long chain base - even");
            slFACombobox = new ComboBox();
            slFACombobox.Items.Add("Fatty acyl chain");
            slFACombobox.Items.Add("Fatty acyl chain - odd");
            slFACombobox.Items.Add("Fatty acyl chain - even");
            slDB1Textbox = new TextBox();
            slDB2Textbox = new TextBox();
            slDB1Label = new Label();
            slDB2Label = new Label();
            slHGLabel = new Label();
            slLCBHydroxyLabel = new Label();
            slFAHydroxyLabel = new Label();
            easterText = new Label();
            stFACombobox = new ComboBox();
            stFACombobox.Items.Add("Fatty acyl chain");
            stFACombobox.Items.Add("Fatty acyl chain - odd");
            stFACombobox.Items.Add("Fatty acyl chain - even");
            stFATextbox = new TextBox();
            stDBLabel = new Label();
            stDBTextbox = new TextBox();
            stHydroxylTextbox = new TextBox();
            stFAHydroxyLabel = new Label();
            homeText = new Label();
            homeText3 = new Label();

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
            glRepresentativeFA = new CheckBox();
            plRepresentativeFA = new CheckBox();
            glContainsSugar = new CheckBox();
            plHasPlasmalogen = new CheckBox();
            
            plTypeGroup = new GroupBox();
            plRegular = new RadioButton();
            plIsCL = new RadioButton();
            plIsLyso = new RadioButton();
            
            slTypeGroup = new GroupBox();
            slRegular = new RadioButton();
            slIsLyso = new RadioButton();
            
            stTypeGroup = new GroupBox();
            stRegular = new RadioButton();
            stIsEster = new RadioButton();
            
            glPositiveAdduct = new GroupBox();
            glNegativeAdduct = new GroupBox();
            plPositiveAdduct = new GroupBox();
            plNegativeAdduct = new GroupBox();
            slPositiveAdduct = new GroupBox();
            slNegativeAdduct = new GroupBox();
            stPositiveAdduct = new GroupBox();
            stNegativeAdduct = new GroupBox();
            medNegativeAdduct = new GroupBox();
            
            glStep1 = new GroupBox();
            plStep1 = new GroupBox();
            slStep1 = new GroupBox();
            stStep1 = new GroupBox();
            medStep1 = new GroupBox();
            lcStep2 = new GroupBox();
            lcStep3 = new GroupBox();

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
            stPosAdductCheckbox1 = new CheckBox();
            stPosAdductCheckbox2 = new CheckBox();
            stPosAdductCheckbox3 = new CheckBox();
            stNegAdductCheckbox1 = new CheckBox();
            stNegAdductCheckbox2 = new CheckBox();
            stNegAdductCheckbox3 = new CheckBox();
            stNegAdductCheckbox4 = new CheckBox();
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
            string repFAText = "All fatty acyl parameters will be copied from the first FA to all remaining FAs";


            tabControl.Controls.Add(homeTab);
            tabControl.Controls.Add(glycerolipidsTab);
            tabControl.Controls.Add(phospholipidsTab);
            tabControl.Controls.Add(sphingolipidsTab);
            tabControl.Controls.Add(sterollipidsTab);
            tabControl.Controls.Add(mediatorlipidsTab);
            tabControl.ShowToolTips = true;
            tabControl.Dock = DockStyle.Fill;
            tabControl.Height = 300;
            Font tabFont = new Font(tabControl.Font.FontFamily, (REGULAR_FONT_SIZE + 5) * FONT_SIZE_FACTOR);
            tabControl.Font = tabFont;
            tabControl.Selecting += new TabControlCancelEventHandler(tabIndexChanged);
            tabControl.ItemSize = new Size(170, 50);
            tabControl.SizeMode = TabSizeMode.Fixed;
            tabControl.AutoSize = false;
            
            
            homeTab.Text = "Home";
            if (!lipidCreatorInitError) homeTab.BackgroundImage = Image.FromFile(Path.Combine(lipidCreator.prefixPath, "images", "LIFS", "hometab.png"));
            homeTab.Font = Font;
            

            
            // tab for cardiolipins
            phospholipidsTab.Controls.Add(plStep1);
            plStep1.Controls.Add(clFA3Checkbox3);
            plStep1.Controls.Add(clFA3Checkbox2);
            plStep1.Controls.Add(clFA3Checkbox1);
            plStep1.Controls.Add(clFA4Checkbox3);
            plStep1.Controls.Add(clFA4Checkbox2);
            plStep1.Controls.Add(clFA4Checkbox1);
            plStep1.Controls.Add(clFA3Textbox);
            plStep1.Controls.Add(clFA4Textbox);
            plStep1.Controls.Add(clDB3Textbox);
            plStep1.Controls.Add(clDB4Textbox);
            plStep1.Controls.Add(clHydroxyl3Textbox);
            plStep1.Controls.Add(clHydroxyl4Textbox);
            plStep1.Controls.Add(clFA3Combobox);
            plStep1.Controls.Add(clFA4Combobox);
            plStep1.Controls.Add(clDB3Label);
            plStep1.Controls.Add(clDB4Label);
            plStep1.Controls.Add(clHydroxyl3Label);
            plStep1.Controls.Add(clHydroxyl4Label);
            phospholipidsTab.Font = Font;
            
            
            clFA3Textbox.Visible = false;
            clFA4Textbox.Visible = false;
            clDB3Textbox.Visible = false;
            clDB4Textbox.Visible = false;
            clHydroxyl3Textbox.Visible = false;
            clHydroxyl4Textbox.Visible = false;
            clFA3Combobox.Visible = false;
            clFA4Combobox.Visible = false;
            clDB3Label.Visible = false;
            clDB4Label.Visible = false;
            clHydroxyl3Label.Visible = false;
            clHydroxyl4Label.Visible = false;
            


            clFA3Combobox.BringToFront();
            clFA3Textbox.BringToFront();
            clFA3Textbox.Location = new Point(440, 256);
            clFA3Textbox.Width = faLength;
            clFA3Textbox.TextChanged += delegate(object s, EventArgs e){ updateCarbon(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag3, "" )); };
            toolTip.SetToolTip(clFA3Textbox, formattingFA);
            clFA3Combobox.Location = new Point(clFA3Textbox.Left, clFA3Textbox.Top - sepText);
            clFA3Combobox.Width = faLength;
            clFA3Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            clFA3Combobox.SelectedIndexChanged += delegate(object s, EventArgs e){ updateOddEven(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag3, clFA3Textbox )); };
            clDB3Textbox.Location = new Point(clFA3Textbox.Left + clFA3Textbox.Width + sep, clFA3Textbox.Top);
            clDB3Textbox.Width = dbLength;
            clDB3Textbox.TextChanged += delegate(object s, EventArgs e){ updateDB(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag3, "" )); };
            toolTip.SetToolTip(clDB3Textbox, formattingDB);
            clDB3Label.Location = new Point(clDB3Textbox.Left, clDB3Textbox.Top - sep);
            clDB3Label.Width = dbLength;
            clDB3Label.Text = dbText;
            clHydroxyl3Textbox.Width = dbLength;
            clHydroxyl3Textbox.Location = new Point(clDB3Textbox.Left + clDB3Textbox.Width + sep, clDB3Textbox.Top);
            clHydroxyl3Textbox.TextChanged += delegate(object s, EventArgs e){ updateHydroxyl(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag3, "" )); };
            toolTip.SetToolTip(clHydroxyl3Textbox, formattingHydroxyl);
            clHydroxyl3Label.Width = dbLength;
            clHydroxyl3Label.Location = new Point(clHydroxyl3Textbox.Left, clHydroxyl3Textbox.Top - sep);
            clHydroxyl3Label.Text = hydroxylText;


            clFA3Checkbox3.Location = new Point(clFA3Textbox.Left + 90, clFA3Textbox.Top + clFA3Textbox.Height);
            clFA3Checkbox3.Text = "FAa";
            clFA3Checkbox3.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag3, "FAa" )); };
            clFA3Checkbox3.MouseLeave += delegate(object s, EventArgs e){ plPictureBox.Image = cardioBackboneImage; };
            clFA3Checkbox3.MouseMove += delegate(object s, MouseEventArgs e){ plPictureBox.Image = cardioBackboneImageFA3e; };
            clFA3Checkbox2.Location = new Point(clFA3Textbox.Left + 40, clFA3Textbox.Top + clFA3Textbox.Height);
            clFA3Checkbox2.Text = "FAp";
            toolTip.SetToolTip(clFA3Checkbox2, FApInformation);
            clFA3Checkbox2.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag3, "FAp" )); };
            clFA3Checkbox2.MouseLeave += delegate(object s, EventArgs e){ plPictureBox.Image = cardioBackboneImage; };
            clFA3Checkbox2.MouseMove += delegate(object s, MouseEventArgs e){ plPictureBox.Image = cardioBackboneImageFA3p; };
            clFA3Checkbox1.Location = new Point(clFA3Textbox.Left, clFA3Textbox.Top + clFA3Textbox.Height);
            clFA3Checkbox1.Text = "FA";
            clFA3Checkbox1.Checked = true;
            clFA3Checkbox1.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag3, "FA" )); };




            clFA4Combobox.BringToFront();
            clFA4Textbox.BringToFront();
            clFA4Textbox.Location = new Point(352, 336);
            clFA4Textbox.Width = faLength;
            clFA4Textbox.TextChanged += delegate(object s, EventArgs e){ updateCarbon(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag4, "" )); };
            toolTip.SetToolTip(clFA4Textbox, formattingFA);
            clFA4Combobox.Location = new Point(clFA4Textbox.Left, clFA4Textbox.Top - sepText);
            clFA4Combobox.Width = faLength;
            clFA4Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            clFA4Combobox.SelectedIndexChanged += delegate(object s, EventArgs e){ updateOddEven(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag4, clFA4Textbox )); };
            clDB4Textbox.Location = new Point(clFA4Textbox.Left + clFA4Textbox.Width + sep, clFA4Textbox.Top);
            clDB4Textbox.Width = dbLength;
            clDB4Textbox.TextChanged += delegate(object s, EventArgs e){ updateDB(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag4, "" )); };
            toolTip.SetToolTip(clDB4Textbox, formattingDB);
            clDB4Label.Location = new Point(clDB4Textbox.Left, clDB4Textbox.Top - sep);
            clDB4Label.Width = dbLength;
            clDB4Label.Text = dbText;
            clHydroxyl4Textbox.Width = dbLength;
            clHydroxyl4Textbox.Location = new Point(clDB4Textbox.Left + clDB4Textbox.Width + sep, clDB4Textbox.Top);
            clHydroxyl4Textbox.TextChanged += delegate(object s, EventArgs e){ updateHydroxyl(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag4, "" )); };
            toolTip.SetToolTip(clHydroxyl4Textbox, formattingHydroxyl);
            clHydroxyl4Label.Width = dbLength;
            clHydroxyl4Label.Location = new Point(clHydroxyl4Textbox.Left, clHydroxyl4Textbox.Top - sep);
            clHydroxyl4Label.Text = hydroxylText;

            clFA4Checkbox3.Location = new Point(clFA4Textbox.Left + 90, clFA4Textbox.Top + clFA4Textbox.Height);
            clFA4Checkbox3.Text = "FAa";
            clFA4Checkbox3.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag4, "FAa" )); };
            clFA4Checkbox3.MouseLeave += delegate(object s, EventArgs e){ plPictureBox.Image = cardioBackboneImage; };
            clFA4Checkbox3.MouseMove += delegate(object s, MouseEventArgs e){ plPictureBox.Image = cardioBackboneImageFA4e; };
            clFA4Checkbox2.Location = new Point(clFA4Textbox.Left + 40, clFA4Textbox.Top + clFA4Textbox.Height);
            clFA4Checkbox2.Text = "FAp";
            toolTip.SetToolTip(clFA4Checkbox2, FApInformation);
            clFA4Checkbox2.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag4, "FAp" )); };
            clFA4Checkbox2.MouseLeave += delegate(object s, EventArgs e){ plPictureBox.Image = cardioBackboneImage; };
            clFA4Checkbox2.MouseMove += delegate(object s, MouseEventArgs e){ plPictureBox.Image = cardioBackboneImageFA4p; };
            clFA4Checkbox1.Location = new Point(clFA4Textbox.Left, clFA4Textbox.Top + clFA4Textbox.Height);
            clFA4Checkbox1.Text = "FA";
            clFA4Checkbox1.Checked = true;
            clFA4Checkbox1.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag4, "FA" )); };



            // tab for glycerolipids
            glycerolipidsTab.Controls.Add(glStep1);
            glStep1.Controls.Add(glFA1Checkbox3);
            glStep1.Controls.Add(glFA1Checkbox2);
            glStep1.Controls.Add(glFA1Checkbox1);
            glStep1.Controls.Add(glFA2Checkbox3);
            glStep1.Controls.Add(glFA2Checkbox2);
            glStep1.Controls.Add(glFA2Checkbox1);
            glStep1.Controls.Add(glFA3Checkbox3);
            glStep1.Controls.Add(glFA3Checkbox2);
            glStep1.Controls.Add(glFA3Checkbox1);
            glStep1.Controls.Add(glPictureBox);
            glStep1.Controls.Add(glArrow);
            glStep1.Controls.Add(glFA1Textbox);
            glStep1.Controls.Add(glFA2Textbox);
            glStep1.Controls.Add(glFA3Textbox);
            glStep1.Controls.Add(glDB1Textbox);
            glStep1.Controls.Add(glDB2Textbox);
            glStep1.Controls.Add(glDB3Textbox);
            glStep1.Controls.Add(glHydroxyl1Textbox);
            glStep1.Controls.Add(glHydroxyl2Textbox);
            glStep1.Controls.Add(glHydroxyl3Textbox);
            glStep1.Controls.Add(glFA1Combobox);
            glStep1.Controls.Add(glFA2Combobox);
            glStep1.Controls.Add(glFA3Combobox);
            glStep1.Controls.Add(glHgListbox);
            glStep1.Controls.Add(glHGLabel);
            glStep1.Controls.Add(glContainsSugar);
            glStep1.Controls.Add(glDB1Label);
            glStep1.Controls.Add(glDB2Label);
            glStep1.Controls.Add(glDB3Label);
            glStep1.Controls.Add(glHydroxyl1Label);
            glStep1.Controls.Add(glHydroxyl2Label);
            glStep1.Controls.Add(glHydroxyl3Label);
            glStep1.Controls.Add(glRepresentativeFA);
            glStep1.Controls.Add(glPositiveAdduct);
            glStep1.Controls.Add(glNegativeAdduct);
            glycerolipidsTab.Parent = tabControl;
            glycerolipidsTab.Text = "Glycerolipids";
            //glycerolipidsTab.ToolTipText = "Glycerolipids";
            glycerolipidsTab.Location = new Point(0, 0);
            glycerolipidsTab.Size = this.Size;
            glycerolipidsTab.AutoSize = true;
            glycerolipidsTab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            glycerolipidsTab.BackColor = Color.White;
            glycerolipidsTab.Font = Font;
            
            glStep1.SendToBack();
            glStep1.Location = new Point(10, 10);
            glStep1.Width = Width - 50;
            glStep1.Height = step1Height;
            glStep1.Text = "Step 1: Precursor selection";
            
            
            

            glFA1Combobox.BringToFront();
            glFA1Textbox.BringToFront();
            glFA1Textbox.Location = new Point(236, 70);
            glFA1Textbox.Width = faLength;
            glFA1Textbox.Text = "0, 2, 4, 6-7";
            glFA1Textbox.TextChanged += delegate(object s, EventArgs e){ updateCarbon(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag1, "" )); updateGLRepresentative(); };
            toolTip.SetToolTip(glFA1Textbox, formattingFA);
            glFA1Combobox.Location = new Point(glFA1Textbox.Left, glFA1Textbox.Top - sepText);
            glFA1Combobox.Width = faLength;
            glFA1Combobox.SelectedItem = "Fatty acyl chain";
            glFA1Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            glFA1Combobox.SelectedIndexChanged += delegate(object s, EventArgs e){ updateOddEven(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag1, glFA2Textbox )); updateGLRepresentative(); };
            glDB1Textbox.Location = new Point(glFA1Textbox.Left + glFA1Textbox.Width + sep, glFA1Textbox.Top);
            glDB1Textbox.Width = dbLength;
            glDB1Textbox.Text = "0-2";
            glDB1Textbox.TextChanged += delegate(object s, EventArgs e){ updateDB(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag1, "" )); updateGLRepresentative(); };
            toolTip.SetToolTip(glDB1Textbox, formattingDB);
            glDB1Label.Location = new Point(glDB1Textbox.Left, glDB1Textbox.Top - sep);
            glDB1Label.Width = dbLength;
            glDB1Label.Text = dbText;
            glHydroxyl1Textbox.Width = dbLength;
            glHydroxyl1Textbox.Location = new Point(glDB1Textbox.Left + glDB1Textbox.Width + sep, glDB1Textbox.Top);
            glHydroxyl1Textbox.TextChanged += delegate(object s, EventArgs e){ updateHydroxyl(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag1, "" )); updateGLRepresentative(); };
            toolTip.SetToolTip(glHydroxyl1Textbox, formattingHydroxyl);
            glHydroxyl1Label.Width = dbLength;
            glHydroxyl1Label.Location = new Point(glHydroxyl1Textbox.Left, glHydroxyl1Textbox.Top - sep);
            glHydroxyl1Label.Text = hydroxylText;

            glFA1Checkbox3.Location = new Point(glFA1Textbox.Left + 90, glFA1Textbox.Top + glFA1Textbox.Height);
            glFA1Checkbox3.Text = "FAa";
            glFA1Checkbox3.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag1, "FAa" )); updateGLRepresentative(); };
            glFA1Checkbox3.MouseLeave += delegate(object s, EventArgs e){ glPictureBox.Image = glyceroBackboneImage; };
            glFA1Checkbox3.MouseMove += delegate(object s, MouseEventArgs e){ glPictureBox.Image = glyceroBackboneImageFAe; };
            glFA1Checkbox2.Location = new Point(glFA1Textbox.Left + 40, glFA1Textbox.Top + glFA1Textbox.Height);
            glFA1Checkbox2.Text = "FAp";
            toolTip.SetToolTip(glFA1Checkbox2, FApInformation);
            glFA1Checkbox2.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag1, "FAp" )); updateGLRepresentative(); };
            glFA1Checkbox2.MouseLeave += delegate(object s, EventArgs e){ glPictureBox.Image = glyceroBackboneImage; };
            glFA1Checkbox2.MouseMove += delegate(object s, MouseEventArgs e){ glPictureBox.Image = glyceroBackboneImageFA1p; };
            glFA1Checkbox1.Location = new Point(glFA1Textbox.Left, glFA1Textbox.Top + glFA1Textbox.Height);
            glFA1Checkbox1.Text = "FA";
            glFA1Checkbox1.Checked = true;
            glFA1Checkbox1.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag1, "FA" )); updateGLRepresentative(); };

            glFA2Combobox.BringToFront();
            glFA2Textbox.BringToFront();
            glFA2Textbox.Location = new Point(330, 142);
            glFA2Textbox.Width = faLength;
            glFA2Textbox.Text = "0, 5, 17-19";
            glFA2Textbox.TextChanged += delegate(object s, EventArgs e){ updateCarbon(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag2, "" )); };
            toolTip.SetToolTip(glFA2Textbox, formattingFA);
            glFA2Combobox.Location = new Point(glFA2Textbox.Left, glFA2Textbox.Top - sepText);
            glFA2Combobox.Width = faLength;
            glFA2Combobox.SelectedItem = "Fatty acyl chain";
            glFA2Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            glFA2Combobox.SelectedIndexChanged += delegate(object s, EventArgs e){ updateOddEven(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag2, glFA2Textbox )); };
            glDB2Textbox.Location = new Point(glFA2Textbox.Left + glFA2Textbox.Width + sep, glFA2Textbox.Top);
            glDB2Textbox.Width = dbLength;
            glDB2Textbox.Text = "5-6";
            glDB2Textbox.TextChanged += delegate(object s, EventArgs e){ updateDB(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag2, "" )); };
            toolTip.SetToolTip(glDB2Textbox, formattingDB);
            glDB2Label.Location = new Point(glDB2Textbox.Left, glDB2Textbox.Top - sep);
            glDB2Label.Width = dbLength;
            glDB2Label.Text = dbText;
            glHydroxyl2Textbox.Width = dbLength;
            glHydroxyl2Textbox.Location = new Point(glDB2Textbox.Left + glDB2Textbox.Width + sep, glDB2Textbox.Top);
            glHydroxyl2Textbox.TextChanged += delegate(object s, EventArgs e){ updateHydroxyl(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag2, "" )); };
            toolTip.SetToolTip(glHydroxyl2Textbox, formattingHydroxyl);
            glHydroxyl2Label.Width = dbLength;
            glHydroxyl2Label.Location = new Point(glHydroxyl2Textbox.Left, glHydroxyl2Textbox.Top - sep);
            glHydroxyl2Label.Text = hydroxylText;

            glFA2Checkbox3.Location = new Point(glFA2Textbox.Left + 90, glFA2Textbox.Top + glFA2Textbox.Height);
            glFA2Checkbox3.Text = "FAa";
            glFA2Checkbox3.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag2, "FAa" )); };
            glFA2Checkbox3.MouseLeave += delegate(object s, EventArgs e){ glPictureBox.Image = glyceroBackboneImage; };
            glFA2Checkbox3.MouseMove += delegate(object s, MouseEventArgs e){ glPictureBox.Image = glyceroBackboneImageFA2e; };
            glFA2Checkbox2.Location = new Point(glFA2Textbox.Left + 40, glFA2Textbox.Top + glFA2Textbox.Height);
            glFA2Checkbox2.Text = "FAp";
            toolTip.SetToolTip(glFA2Checkbox2, FApInformation);
            glFA2Checkbox2.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag2, "FAp" )); };
            glFA2Checkbox2.MouseLeave += delegate(object s, EventArgs e){ glPictureBox.Image = glyceroBackboneImage; };
            glFA2Checkbox2.MouseMove += delegate(object s, MouseEventArgs e){ glPictureBox.Image = glyceroBackboneImageFA2p; };
            glFA2Checkbox1.Location = new Point(glFA2Textbox.Left, glFA2Textbox.Top + glFA2Textbox.Height);
            glFA2Checkbox1.Text = "FA";
            glFA2Checkbox1.Checked = true;
            //glFA2Checkbox1.CheckedChanged += new EventHandler(glFA2Checkbox1CheckedChanged);
            glFA2Checkbox1.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag2, "FA" )); };

            glFA3Combobox.BringToFront();
            glFA3Textbox.BringToFront();
            glFA3Textbox.Location = new Point(198, 242);
            glFA3Textbox.Width = faLength;
            glFA3Textbox.Text = "20-22";
            glFA3Textbox.TextChanged += delegate(object s, EventArgs e){ updateCarbon(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag3, "" )); };
            toolTip.SetToolTip(glFA3Textbox, formattingFA);
            glFA3Combobox.Location = new Point(glFA3Textbox.Left, glFA3Textbox.Top - sepText);
            glFA3Combobox.Width = faLength;
            glFA3Combobox.SelectedItem = "Fatty acyl chain";
            glFA3Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            glFA3Combobox.SelectedIndexChanged += delegate(object s, EventArgs e){ updateOddEven(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag3, glFA3Textbox )); };
            glDB3Textbox.Location = new Point(glFA3Textbox.Left + glFA3Textbox.Width + sep, glFA3Textbox.Top);
            glDB3Textbox.Width = dbLength;
            glDB3Textbox.Text = "0";
            glDB3Textbox.TextChanged += delegate(object s, EventArgs e){ updateDB(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag3, "" )); };
            toolTip.SetToolTip(glDB3Textbox, formattingDB);
            glDB3Label.Location = new Point(glDB3Textbox.Left, glDB3Textbox.Top - sep);
            glDB3Label.Width = dbLength;
            glDB3Label.Text = dbText;
            glHydroxyl3Textbox.Width = dbLength;
            glHydroxyl3Textbox.Location = new Point(glDB3Textbox.Left + glDB3Textbox.Width + sep, glDB3Textbox.Top);
            glHydroxyl3Textbox.TextChanged += delegate(object s, EventArgs e){ updateHydroxyl(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag3, "" )); };
            toolTip.SetToolTip(glHydroxyl3Textbox, formattingHydroxyl);
            glHydroxyl3Label.Width = dbLength;
            glHydroxyl3Label.Location = new Point(glHydroxyl3Textbox.Left, glHydroxyl3Textbox.Top - sep);
            glHydroxyl3Label.Text = hydroxylText;

            glFA3Checkbox3.Location = new Point(glFA3Textbox.Left + 90, glFA3Textbox.Top + glFA3Textbox.Height);
            glFA3Checkbox3.Text = "FAa";
            glFA3Checkbox3.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag3, "FAa" )); };
            glFA3Checkbox3.MouseLeave += delegate(object s, EventArgs e){ glPictureBox.Image = glyceroBackboneImage; };
            glFA3Checkbox3.MouseMove += delegate(object s, MouseEventArgs e){ glPictureBox.Image = glyceroBackboneImageFA3e; };
            glFA3Checkbox2.Location = new Point(glFA3Textbox.Left + 40, glFA3Textbox.Top + glFA3Textbox.Height);
            glFA3Checkbox2.Text = "FAp";
            toolTip.SetToolTip(glFA3Checkbox2, FApInformation);
            glFA3Checkbox2.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag3, "FAp" )); };
            glFA3Checkbox2.MouseLeave += delegate(object s, EventArgs e){ glPictureBox.Image = glyceroBackboneImage; };
            glFA3Checkbox2.MouseMove += delegate(object s, MouseEventArgs e){ glPictureBox.Image = glyceroBackboneImageFA3p; };
            glFA3Checkbox1.Location = new Point(glFA3Textbox.Left, glFA3Textbox.Top + glFA3Textbox.Height);
            glFA3Checkbox1.Text = "FA";
            glFA3Checkbox1.Checked = true;
            glFA3Checkbox1.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Glycerolipid)currentLipid).fag3, "FA" )); };

            
            glHgListbox.Location = new Point(172, 228);
            glHgListbox.Size = new Size(70, 50);
            glHgListbox.BringToFront();
            glHgListbox.BorderStyle = BorderStyle.Fixed3D;
            glHgListbox.SelectedValueChanged += new System.EventHandler(glHGListboxSelectedValueChanged);
            glHgListbox.Visible = false;
            
            glHGLabel.Location = new Point(glHgListbox.Left, glHgListbox.Top - sep);
            glHGLabel.Text = "Sugar head";
            glHGLabel.Visible = false;
            
            
            

            glPositiveAdduct.Width = 120;
            glPositiveAdduct.Location = new Point(leftGroupboxes - glPositiveAdduct.Width, topGroupboxes);
            glPositiveAdduct.Height = 120;
            glPositiveAdduct.Text = "Positive adducts";
            glPosAdductCheckbox1.Parent = glPositiveAdduct;
            glPosAdductCheckbox1.Location = new Point(10, 15);
            glPosAdductCheckbox1.Text = "+H⁺";
            glPosAdductCheckbox1.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+H", currentLipid));};
            glPosAdductCheckbox1.Enabled = false;
            glPosAdductCheckbox2.Parent = glPositiveAdduct;
            glPosAdductCheckbox2.Location = new Point(10, 35);
            glPosAdductCheckbox2.Text = "+2H⁺⁺";
            glPosAdductCheckbox2.Enabled = false;
            glPosAdductCheckbox2.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+2H", currentLipid));};
            glPosAdductCheckbox3.Parent = glPositiveAdduct;
            glPosAdductCheckbox3.Location = new Point(10, 55);
            glPosAdductCheckbox3.Text = "+NH4⁺";
            glPosAdductCheckbox3.Checked = true;
            glPosAdductCheckbox3.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+NH4", currentLipid));};
            glNegativeAdduct.Width = 120;
            glNegativeAdduct.Location = new Point(leftGroupboxes - glNegativeAdduct.Width, glPositiveAdduct.Top + 140);
            glNegativeAdduct.Height = 120;
            glNegativeAdduct.Text = "Negative adducts";
            glNegAdductCheckbox1.Parent = glNegativeAdduct;
            glNegAdductCheckbox1.Location = new Point(10, 15);
            glNegAdductCheckbox1.Text = "-H⁻";
            glNegAdductCheckbox1.Enabled = false;
            glNegAdductCheckbox1.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("-H", currentLipid));};
            glNegAdductCheckbox2.Parent = glNegativeAdduct;
            glNegAdductCheckbox2.Location = new Point(10, 35);
            glNegAdductCheckbox2.Text = "-2H⁻ ⁻";
            glNegAdductCheckbox2.Enabled = false;
            glNegAdductCheckbox2.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("-2H", currentLipid));};
            glNegAdductCheckbox3.Parent = glNegativeAdduct;
            glNegAdductCheckbox3.Location = new Point(10, 55);
            glNegAdductCheckbox3.Text = "+HCOO⁻";
            glNegAdductCheckbox3.Enabled = false;
            glNegAdductCheckbox3.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+HCOO", currentLipid));};
            glNegAdductCheckbox4.Parent = glNegativeAdduct;
            glNegAdductCheckbox4.Location = new Point(10, 75);
            glNegAdductCheckbox4.Text = "+CH3COO⁻";
            glNegAdductCheckbox4.Enabled = false;
            glNegAdductCheckbox4.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+CH3COO", currentLipid));};

            glPictureBox.Image = glyceroBackboneImage;
            glPictureBox.Location = new Point(117, 19);
            glPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            glPictureBox.SendToBack();

            glArrow.Image = glArrowImage;
            glArrow.Location = new Point(10, 30);
            glArrow.SizeMode = PictureBoxSizeMode.AutoSize;
            glArrow.SendToBack();


            glContainsSugar.Location = new Point(198, 290);
            glContainsSugar.Width = 120;
            glContainsSugar.Text = "Contains sugar";
            glContainsSugar.CheckedChanged += new EventHandler(glContainsSugarCheckedChanged);
            glContainsSugar.BringToFront();
            
            glRepresentativeFA.Location = new Point(glHydroxyl1Textbox.Left + glHydroxyl1Textbox.Width + sep, glHydroxyl1Textbox.Top);
            glRepresentativeFA.Width = 150;
            glRepresentativeFA.Text = "First FA representative";
            toolTip.SetToolTip(glRepresentativeFA, repFAText);
            glRepresentativeFA.CheckedChanged += new EventHandler(glRepresentativeFACheckedChanged);
            glRepresentativeFA.SendToBack();

            

            // tab for phospholipids
            
            phospholipidsTab.Controls.Add(plStep1);
            plStep1.Controls.Add(plFA1Checkbox3);
            plStep1.Controls.Add(plFA1Checkbox2);
            plStep1.Controls.Add(plFA1Checkbox1);
            plStep1.Controls.Add(plFA2Checkbox3);
            plStep1.Controls.Add(plFA2Checkbox2);
            plStep1.Controls.Add(plFA2Checkbox1);
            plStep1.Controls.Add(plTypeGroup);
            plStep1.Controls.Add(plPictureBox);
            plStep1.Controls.Add(plFA1Textbox);
            plStep1.Controls.Add(plFA2Textbox);
            plStep1.Controls.Add(plDB1Textbox);
            plStep1.Controls.Add(plDB2Textbox);
            plStep1.Controls.Add(plHydroxyl1Textbox);
            plStep1.Controls.Add(plHydroxyl2Textbox);
            plStep1.Controls.Add(plFA1Combobox);
            plStep1.Controls.Add(plFA2Combobox);
            plStep1.Controls.Add(plDB1Label);
            plStep1.Controls.Add(plDB2Label);
            plStep1.Controls.Add(plHydroxyl1Label);
            plStep1.Controls.Add(plHydroxyl2Label);
            plStep1.Controls.Add(plHgListbox);
            plStep1.Controls.Add(plHGLabel);
            plStep1.Controls.Add(plRepresentativeFA);
            plStep1.Controls.Add(plPositiveAdduct);
            plStep1.Controls.Add(plNegativeAdduct);
            plStep1.Controls.Add(easterText);
            phospholipidsTab.Parent = tabControl;
            phospholipidsTab.Text = "    Glycero-\nphospholipids";
            //phospholipidsTab.ToolTipText = "Glycerophospholipids";
            phospholipidsTab.Location = new Point(0, 0);
            phospholipidsTab.Size = this.Size;
            phospholipidsTab.AutoSize = true;
            phospholipidsTab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            phospholipidsTab.BackColor = Color.White;
            
            plStep1.SendToBack();
            plStep1.Location = new Point(10, 10);
            plStep1.Width = Width - 50;
            plStep1.Height = step1Height;
            plStep1.Text = "Step 1: Precursor selection";

            plFA1Combobox.BringToFront();
            plFA1Textbox.BringToFront();
            plFA1Textbox.Location = new Point(400, 74);
            plFA1Textbox.Width = faLength;
            plFA1Textbox.Text = "0, 2, 4, 6-7";
            plFA1Textbox.TextChanged += delegate(object s, EventArgs e){ updateCarbon(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag1, "" )); updatePLRepresentative(); };
            toolTip.SetToolTip(plFA1Textbox, formattingFA);
            plFA1Combobox.Location = new Point(plFA1Textbox.Left, plFA1Textbox.Top - sepText);
            plFA1Combobox.Width = faLength;
            plFA1Combobox.SelectedItem = "Fatty acyl chain";
            plFA1Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            plFA1Combobox.SelectedIndexChanged += delegate(object s, EventArgs e){ updateOddEven(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag1, plFA2Textbox )); updatePLRepresentative(); };
            plDB1Textbox.Location = new Point(plFA1Textbox.Left + plFA1Textbox.Width + sep, plFA1Textbox.Top);
            plDB1Textbox.Width = dbLength;
            plDB1Textbox.Text = "0-2";
            plDB1Textbox.TextChanged += delegate(object s, EventArgs e){ updateDB(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag1, "" )); updatePLRepresentative(); };
            toolTip.SetToolTip(plDB1Textbox, formattingDB);
            plDB1Label.Location = new Point(plDB1Textbox.Left, plDB1Textbox.Top - sep);
            plDB1Label.Width = dbLength;
            plDB1Label.Text = dbText;
            plHydroxyl1Textbox.Width = dbLength;
            plHydroxyl1Textbox.Location = new Point(plDB1Textbox.Left + plDB1Textbox.Width + sep, plDB1Textbox.Top);
            plHydroxyl1Textbox.TextChanged += delegate(object s, EventArgs e){ updateHydroxyl(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag1, "" )); updatePLRepresentative(); };
            toolTip.SetToolTip(plHydroxyl1Textbox, formattingHydroxyl);
            plHydroxyl1Label.Width = dbLength;
            plHydroxyl1Label.Location = new Point(plHydroxyl1Textbox.Left, plHydroxyl1Textbox.Top - sep);
            plHydroxyl1Label.Text = hydroxylText;

            plFA1Checkbox3.Location = new Point(plFA1Textbox.Left + 90, plFA1Textbox.Top + plFA1Textbox.Height);
            plFA1Checkbox3.Text = "FAa";
            plFA1Checkbox3.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag1, "FAa" )); updatePLRepresentative(); };
            plFA1Checkbox3.MouseLeave += delegate(object s, EventArgs e){ plPictureBox.Image = plIsCL.Checked ? cardioBackboneImage : (plIsLyso.Checked ? phosphoLysoBackboneImage : phosphoBackboneImage); };
            plFA1Checkbox3.MouseMove += delegate(object s, MouseEventArgs e){ plPictureBox.Image = plIsCL.Checked ? cardioBackboneImageFA1e : (plIsLyso.Checked ? phosphoLysoBackboneImageFA1e : phosphoBackboneImageFA1e); };
            plFA1Checkbox2.Location = new Point(plFA1Textbox.Left + 40, plFA1Textbox.Top + plFA1Textbox.Height);
            plFA1Checkbox2.Text = "FAp";
            toolTip.SetToolTip(plFA1Checkbox2, FApInformation);
            plFA1Checkbox2.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag1, "FAp" )); updatePLRepresentative(); };
            plFA1Checkbox2.MouseLeave += delegate(object s, EventArgs e){ plPictureBox.Image = plIsCL.Checked ? cardioBackboneImage : (plIsLyso.Checked ? phosphoLysoBackboneImage : phosphoBackboneImage); };
            plFA1Checkbox2.MouseMove += delegate(object s, MouseEventArgs e){ plPictureBox.Image = plIsCL.Checked ? cardioBackboneImageFA1p : (plIsLyso.Checked ? phosphoLysoBackboneImageFA1p : phosphoBackboneImageFA1p); };
            plFA1Checkbox1.Location = new Point(plFA1Textbox.Left, plFA1Textbox.Top + plFA1Textbox.Height);
            plFA1Checkbox1.Text = "FA";
            plFA1Checkbox1.Checked = true;
            plFA1Checkbox1.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag1, "FA" )); updatePLRepresentative(); };

            plFA2Combobox.BringToFront();
            plFA2Textbox.BringToFront();
            plFA2Textbox.Location = new Point(312, 154);
            plFA2Textbox.Width = faLength;
            plFA2Textbox.Text = "2, 5, 17-19";
            plFA2Textbox.TextChanged += delegate(object s, EventArgs e){ updateCarbon(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag2, "" )); };
            toolTip.SetToolTip(plFA2Textbox, formattingFA);
            plFA2Combobox.Location = new Point(plFA2Textbox.Left, plFA2Textbox.Top - sepText);
            plFA2Combobox.Width = faLength;
            plFA2Combobox.SelectedItem = "Fatty acyl chain";
            plFA2Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            plFA2Combobox.SelectedIndexChanged += delegate(object s, EventArgs e){ updateOddEven(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag2, plFA2Textbox )); };
            plDB2Textbox.Location = new Point(plFA2Textbox.Left + plFA2Textbox.Width + sep, plFA2Textbox.Top);
            plDB2Textbox.Width = dbLength;
            plDB2Textbox.Text = "5-6";
            plDB2Textbox.TextChanged += delegate(object s, EventArgs e){ updateDB(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag2, "" )); };
            toolTip.SetToolTip(plDB2Textbox, formattingDB);
            plDB2Label.Location = new Point(plDB2Textbox.Left, plDB2Textbox.Top - sep);
            plDB2Label.Width = dbLength;
            plDB2Label.Text = dbText;
            plHydroxyl2Textbox.Width = dbLength;
            plHydroxyl2Textbox.Location = new Point(plDB2Textbox.Left + plDB2Textbox.Width + sep, plDB2Textbox.Top);
            plHydroxyl2Textbox.TextChanged += delegate(object s, EventArgs e){ updateHydroxyl(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag2, "" )); };
            toolTip.SetToolTip(plHydroxyl2Textbox, formattingHydroxyl);
            plHydroxyl2Label.Width = dbLength;
            plHydroxyl2Label.Location = new Point(plHydroxyl2Textbox.Left, plHydroxyl2Textbox.Top - sep);
            plHydroxyl2Label.Text = hydroxylText;

            plFA2Checkbox3.Location = new Point(plFA2Textbox.Left + 90, plFA2Textbox.Top + plFA2Textbox.Height);
            plFA2Checkbox3.Text = "FAa";
            plFA2Checkbox3.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag2, "FAa" )); };
            plFA2Checkbox3.MouseLeave += delegate(object s, EventArgs e){ plPictureBox.Image = plIsCL.Checked ? cardioBackboneImage : phosphoBackboneImage; };
            plFA2Checkbox3.MouseMove += delegate(object s, MouseEventArgs e){ plPictureBox.Image = plIsCL.Checked ? cardioBackboneImageFA2e : phosphoBackboneImageFA2e; };
            plFA2Checkbox2.Location = new Point(plFA2Textbox.Left + 40, plFA2Textbox.Top + plFA2Textbox.Height);
            plFA2Checkbox2.Text = "FAp";
            toolTip.SetToolTip(plFA1Checkbox2, FApInformation);
            plFA2Checkbox2.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag2, "FAp" )); };
            plFA2Checkbox2.MouseLeave += delegate(object s, EventArgs e){ plPictureBox.Image = plIsCL.Checked ? cardioBackboneImage : phosphoBackboneImage; };
            plFA2Checkbox2.MouseMove += delegate(object s, MouseEventArgs e){ plPictureBox.Image = plIsCL.Checked ? cardioBackboneImageFA2p : phosphoBackboneImageFA2p; };
            plFA2Checkbox1.Location = new Point(plFA2Textbox.Left, plFA2Textbox.Top + plFA2Textbox.Height);
            plFA2Checkbox1.Text = "FA";
            plFA2Checkbox1.Checked = true;
            plFA2Checkbox1.CheckedChanged += delegate(object s, EventArgs e){ FattyAcidCheckboxCheckChanged(s, new FattyAcidEventArgs( ((Phospholipid)currentLipid).fag2, "FA" )); };
            
            
            plTypeGroup.Location = new Point(400, 8);
            plTypeGroup.Size = new Size(360, 40);
            plTypeGroup.Text = "Type";
            plTypeGroup.Controls.Add(plIsCL);
            plTypeGroup.Controls.Add(plIsLyso);
            plTypeGroup.Controls.Add(plRegular);
            plTypeGroup.Controls.Add(plHasPlasmalogen);
            
            
            plRegular.Location = new Point(10, 14);
            plRegular.Text = "Regular";
            plRegular.CheckedChanged += new EventHandler(plTypeCheckedChanged);
            
            plIsLyso.Location = new Point(90, 14);
            plIsLyso.Text = "Lyso";
            plIsLyso.CheckedChanged += new EventHandler(plTypeCheckedChanged);
            
            plIsCL.Location = new Point(150, 14);
            plIsCL.Text = "Cardiolipin";
            plIsCL.Width = 80;
            plIsCL.CheckedChanged += new EventHandler(plTypeCheckedChanged);
            
            plHasPlasmalogen.Location = new Point(260, 14);
            plHasPlasmalogen.Text = "Plasmalogen";
            plHasPlasmalogen.Width = 90;
            plHasPlasmalogen.CheckedChanged += new EventHandler(plTypeCheckedChanged);

            
            plHgListbox.Location = new Point(25, 50);
            plHgListbox.Size = new Size(70, 260);
            plHgListbox.BringToFront();
            plHgListbox.BorderStyle = BorderStyle.Fixed3D;
            plHgListbox.SelectedValueChanged += new System.EventHandler(plHGListboxSelectedValueChanged);
            plHgListbox.MouseLeave += new System.EventHandler(plHGListboxMouseLeave);
            plHgListbox.MouseMove += new System.Windows.Forms.MouseEventHandler(plHGListboxMouseHover);
            plHgListbox.KeyDown += ListboxSelectAll;
            
            plHGLabel.Location = new Point(plHgListbox.Left, plHgListbox.Top - sep);
            plHGLabel.Text = "Head group";
            
            
            easterText.Location = new Point(1030, 250);
            easterText.Text = "Fat is unfair, it sticks 2 minutes in your mouth, 2 hours in your stomach and 2 decades at your hips.";
            easterText.Visible = false;
            easterText.Font = new Font(new Font(easterText.Font.FontFamily, (int)(40.0 * FONT_SIZE_FACTOR)), FontStyle.Bold);
            easterText.AutoSize = true;

            plPositiveAdduct.Width = 120;
            plPositiveAdduct.Location = new Point(leftGroupboxes - plPositiveAdduct.Width, topGroupboxes);
            plPositiveAdduct.Height = 120;
            plPositiveAdduct.Text = "Positive adducts";
            plPositiveAdduct.DoubleClick += new EventHandler(triggerEasteregg);
            plPosAdductCheckbox1.Parent = plPositiveAdduct;
            plPosAdductCheckbox1.Location = new Point(10, 15);
            plPosAdductCheckbox1.Text = "+H⁺";
            plPosAdductCheckbox1.Checked = true;
            plPosAdductCheckbox1.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+H", currentLipid));};
            plPosAdductCheckbox2.Parent = plPositiveAdduct;
            plPosAdductCheckbox2.Location = new Point(10, 35);
            plPosAdductCheckbox2.Text = "+2H⁺⁺";
            plPosAdductCheckbox2.Enabled = false;
            plPosAdductCheckbox2.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+2H", currentLipid));};
            plPosAdductCheckbox3.Parent = plPositiveAdduct;
            plPosAdductCheckbox3.Location = new Point(10, 55);
            plPosAdductCheckbox3.Text = "+NH4⁺";
            plPosAdductCheckbox3.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+NH4", currentLipid));};
            plNegativeAdduct.Width = 120;
            plNegativeAdduct.Location = new Point(leftGroupboxes - plNegativeAdduct.Width, plPositiveAdduct.Top + 140);
            plNegativeAdduct.Height = 120;
            plNegativeAdduct.Text = "Negative adducts";
            plNegAdductCheckbox1.Parent = plNegativeAdduct;
            plNegAdductCheckbox1.Location = new Point(10, 15);
            plNegAdductCheckbox1.Text = "-H⁻";
            plNegAdductCheckbox1.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("-H", currentLipid));};
            plNegAdductCheckbox2.Parent = plNegativeAdduct;
            plNegAdductCheckbox2.Location = new Point(10, 35);
            plNegAdductCheckbox2.Text = "-2H⁻ ⁻";
            plNegAdductCheckbox2.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("-2H", currentLipid));};
            plNegAdductCheckbox3.Parent = plNegativeAdduct;
            plNegAdductCheckbox3.Location = new Point(10, 55);
            plNegAdductCheckbox3.Text = "+HCOO⁻";
            plNegAdductCheckbox3.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+HCOO", currentLipid));};
            plNegAdductCheckbox4.Parent = plNegativeAdduct;
            plNegAdductCheckbox4.Location = new Point(10, 75);
            plNegAdductCheckbox4.Text = "+CH3COO⁻";
            plNegAdductCheckbox4.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+CH3COO", currentLipid));};


            plPictureBox.Image = phosphoBackboneImage;
            plPictureBox.Location = new Point(107, 23);
            plPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            plPictureBox.SendToBack();
            
            plRepresentativeFA.Location = new Point(plHydroxyl1Textbox.Left + plHydroxyl1Textbox.Width + sep, plHydroxyl1Textbox.Top);
            plRepresentativeFA.Width = 150;
            plRepresentativeFA.Text = "First FA representative";
            toolTip.SetToolTip(plRepresentativeFA, repFAText);
            plRepresentativeFA.CheckedChanged += new EventHandler(plRepresentativeFACheckedChanged);
            plRepresentativeFA.SendToBack();



            // tab for sphingolipids
            sphingolipidsTab.Controls.Add(slStep1);
            slStep1.Controls.Add(slPictureBox);
            slStep1.Controls.Add(slLCBTextbox);
            slStep1.Controls.Add(slFATextbox);
            slStep1.Controls.Add(slDB1Textbox);
            slStep1.Controls.Add(slDB2Textbox);
            slStep1.Controls.Add(slLCBCombobox);
            slStep1.Controls.Add(slFACombobox);
            slStep1.Controls.Add(slTypeGroup);
            slStep1.Controls.Add(slDB1Label);
            slStep1.Controls.Add(slDB2Label);
            slStep1.Controls.Add(slHGLabel);
            slStep1.Controls.Add(slHgListbox);
            slStep1.Controls.Add(slLCBHydroxyCombobox);
            slStep1.Controls.Add(slFAHydroxyCombobox);
            slStep1.Controls.Add(slFAHydroxyLabel);
            slStep1.Controls.Add(slLCBHydroxyLabel);
            slStep1.Controls.Add(slPositiveAdduct);
            slStep1.Controls.Add(slNegativeAdduct);
            sphingolipidsTab.Parent = tabControl;
            sphingolipidsTab.Text = "Sphingolipids";
            //sphingolipidsTab.ToolTipText = "Sphingolipids";
            sphingolipidsTab.Location = new Point(0, 0);
            sphingolipidsTab.Size = this.Size;
            sphingolipidsTab.AutoSize = true;
            sphingolipidsTab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            sphingolipidsTab.BackColor = Color.White;
            sphingolipidsTab.Font = Font;
            
            slStep1.SendToBack();
            slStep1.Location = new Point(10, 10);
            slStep1.Width = Width - 50;
            slStep1.Height = step1Height;
            slStep1.Text = "Step 1: Precursor selection";
            
            slTypeGroup.Location = new Point(400, 8);
            slTypeGroup.Size = new Size(180, 40);
            slTypeGroup.Text = "Type";
            slTypeGroup.Controls.Add(slIsLyso);
            slTypeGroup.Controls.Add(slRegular);
            
            slRegular.Location = new Point(10, 14);
            slRegular.Text = "Regular";
            slRegular.CheckedChanged += new EventHandler(slIsLysoCheckedChanged);
            
            slIsLyso.Location = new Point(90, 14);
            slIsLyso.Text = "Lyso";
            slIsLyso.Width = 70;
            slIsLyso.CheckedChanged += new EventHandler(slIsLysoCheckedChanged);
            

            slFACombobox.BringToFront();
            slFATextbox.BringToFront();
            slFATextbox.Location = new Point(258, 235);
            slFATextbox.Width = faLength;
            slFATextbox.Text = "2, 5, 17-19";
            slFATextbox.TextChanged += delegate(object s, EventArgs e){ updateCarbon(s, new FattyAcidEventArgs( ((Sphingolipid)currentLipid).fag, "" )); };
            toolTip.SetToolTip(slFATextbox, formattingFA);
            slFACombobox.Location = new Point(slFATextbox.Left, slFATextbox.Top - sepText);
            slFACombobox.Width = faLength;
            slFACombobox.SelectedItem = "Fatty acyl chain";
            slFACombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            slFACombobox.SelectedIndexChanged += delegate(object s, EventArgs e){ updateOddEven(s, new FattyAcidEventArgs( ((Sphingolipid)currentLipid).fag, slFATextbox )); };
            slDB1Textbox.Location = new Point(slFATextbox.Left + slFATextbox.Width + sep, slFATextbox.Top);
            slDB1Textbox.Width = dbLength;
            slDB1Textbox.Text = "5-6";
            slDB1Textbox.TextChanged += delegate(object s, EventArgs e){ updateDB(s, new FattyAcidEventArgs( ((Sphingolipid)currentLipid).fag, "" )); };
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
            slLCBTextbox.Location = new Point(294, 158);
            slLCBTextbox.Width = faLength;
            slLCBTextbox.Text = "14, 16-18, 22";
            slLCBTextbox.TextChanged += delegate(object s, EventArgs e){ updateCarbon(s, new FattyAcidEventArgs( ((Sphingolipid)currentLipid).lcb, "" )); };
            toolTip.SetToolTip(slLCBTextbox, formattingFA);
            slLCBCombobox.Location = new Point(slLCBTextbox.Left, slLCBTextbox.Top - sepText);
            slLCBCombobox.Width = faLength;
            slLCBCombobox.SelectedItem = "Long chain base";
            slLCBCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            slLCBCombobox.SelectedIndexChanged += delegate(object s, EventArgs e){ updateOddEven(s, new FattyAcidEventArgs( ((Sphingolipid)currentLipid).lcb, slLCBTextbox )); };
            slDB2Textbox.Location = new Point(slLCBTextbox.Left + slLCBTextbox.Width + sep, slLCBTextbox.Top);
            slDB2Textbox.Width = dbLength;
            slDB2Textbox.Text = "0-2";
            slDB2Textbox.TextChanged += delegate(object s, EventArgs e){ updateDB(s, new FattyAcidEventArgs( ((Sphingolipid)currentLipid).lcb, "" )); };
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
            slHgListbox.Location = new Point(54, 40);
            slHgListbox.Size = new Size(80, 260);
            slHgListbox.BringToFront();
            slHgListbox.BorderStyle = BorderStyle.Fixed3D;
            slHgListbox.SelectedValueChanged += new System.EventHandler(slHGListboxSelectedValueChanged);
            slHgListbox.MouseLeave += new System.EventHandler(slHGListboxMouseLeave);
            slHgListbox.MouseMove += new System.Windows.Forms.MouseEventHandler(slHGListboxMouseHover);
            slHgListbox.KeyDown += ListboxSelectAll;
            slHGLabel.Location = new Point(slHgListbox.Left, slHgListbox.Top - sep);
            slHGLabel.Text = "Head group";
            

            slPositiveAdduct.Width = 120;
            slPositiveAdduct.Location = new Point(leftGroupboxes - slPositiveAdduct.Width, topGroupboxes);
            slPositiveAdduct.Height = 120;
            slPositiveAdduct.Text = "Positive adducts";
            slPosAdductCheckbox1.Parent = slPositiveAdduct;
            slPosAdductCheckbox1.Location = new Point(10, 15);
            slPosAdductCheckbox1.Text = "+H⁺";
            slPosAdductCheckbox1.Checked = true;
            slPosAdductCheckbox1.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+H", currentLipid));};
            slPosAdductCheckbox2.Parent = slPositiveAdduct;
            slPosAdductCheckbox2.Location = new Point(10, 35);
            slPosAdductCheckbox2.Text = "+2H⁺⁺";
            slPosAdductCheckbox2.Enabled = false;
            slPosAdductCheckbox2.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+2H", currentLipid));};
            slPosAdductCheckbox3.Parent = slPositiveAdduct;
            slPosAdductCheckbox3.Location = new Point(10, 55);
            slPosAdductCheckbox3.Text = "+NH4⁺";
            slPosAdductCheckbox3.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+NH4", currentLipid));};
            slNegativeAdduct.Width = 120;
            slNegativeAdduct.Location = new Point(leftGroupboxes - slNegativeAdduct.Width, slPositiveAdduct.Top + 140);
            slNegativeAdduct.Height = 120;
            slNegativeAdduct.Text = "Negative adducts";
            slNegAdductCheckbox1.Parent = slNegativeAdduct;
            slNegAdductCheckbox1.Location = new Point(10, 15);
            slNegAdductCheckbox1.Text = "-H⁻";
            slNegAdductCheckbox1.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("-H", currentLipid));};
            slNegAdductCheckbox2.Parent = slNegativeAdduct;
            slNegAdductCheckbox2.Location = new Point(10, 35);
            slNegAdductCheckbox2.Text = "-2H⁻⁻";
            slNegAdductCheckbox2.Enabled = false;
            slNegAdductCheckbox2.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("-2H", currentLipid));};
            slNegAdductCheckbox3.Parent = slNegativeAdduct;
            slNegAdductCheckbox3.Location = new Point(10, 55);
            slNegAdductCheckbox3.Text = "+HCOO⁻";
            slNegAdductCheckbox3.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+HCOO", currentLipid));};
            slNegAdductCheckbox4.Parent = slNegativeAdduct;
            slNegAdductCheckbox4.Location = new Point(10, 75);
            slNegAdductCheckbox4.Text = "+CH3COO⁻";
            slNegAdductCheckbox4.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+CH3COO", currentLipid));};

            if (!lipidCreatorInitError)
            {
                slPictureBox.Image = sphingoBackboneImage;
                slPictureBox.Location = new Point(214 - (sphingoBackboneImage.Width >> 1), 159 - (sphingoBackboneImage.Height >> 1));
                slPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                slPictureBox.SendToBack();
            }
            
            
            
            
            
            // tab for sterols
            sterollipidsTab.Controls.Add(stStep1);
            stStep1.Controls.Add(stPositiveAdduct);
            //stStep1.Controls.Add(stNegativeAdduct);
            stStep1.Controls.Add(stFACombobox);
            stStep1.Controls.Add(stFATextbox);
            stStep1.Controls.Add(stDBTextbox);
            stStep1.Controls.Add(stTypeGroup);
            stStep1.Controls.Add(stDBLabel);
            stStep1.Controls.Add(stHydroxylTextbox);
            stStep1.Controls.Add(stFAHydroxyLabel);
            stStep1.Controls.Add(stPictureBox);
            stStep1.Controls.Add(stHgListbox);
            
            sterollipidsTab.Text = "Sterol lipids";
            //sterollipidsTab.ToolTipText = "Sterol lipids";
            sterollipidsTab.Location = new Point(0, 0);
            sterollipidsTab.Size = this.Size;
            sterollipidsTab.AutoSize = true;
            sterollipidsTab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            sterollipidsTab.BackColor = Color.White;
            sterollipidsTab.Font = Font;
            
            stStep1.SendToBack();
            stStep1.Location = new Point(10, 10);
            stStep1.Width = Width - 50;
            stStep1.Height = step1Height;
            stStep1.Text = "Step 1: Precursor selection";
            
            stPositiveAdduct.Width = 120;
            stPositiveAdduct.Location = new Point(leftGroupboxes - stPositiveAdduct.Width, topGroupboxes);
            stPositiveAdduct.Height = 120;
            stPositiveAdduct.Text = "Positive adducts";
            stPosAdductCheckbox1.Parent = stPositiveAdduct;
            stPosAdductCheckbox1.Location = new Point(10, 15);
            stPosAdductCheckbox1.Text = "+H⁺";
            stPosAdductCheckbox1.Enabled = false;
            stPosAdductCheckbox1.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+H", currentLipid));};
            stPosAdductCheckbox2.Parent = stPositiveAdduct;
            stPosAdductCheckbox2.Location = new Point(10, 35);
            stPosAdductCheckbox2.Text = "+2H⁺⁺";
            stPosAdductCheckbox2.Enabled = false;
            stPosAdductCheckbox2.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+2H", currentLipid));};
            stPosAdductCheckbox3.Parent = stPositiveAdduct;
            stPosAdductCheckbox3.Location = new Point(10, 55);
            stPosAdductCheckbox3.Text = "+NH4⁺";
            stPosAdductCheckbox3.Checked = true;
            stPosAdductCheckbox3.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+NH4", currentLipid));};
            stNegativeAdduct.Width = 120;
            stNegativeAdduct.Location = new Point(leftGroupboxes - stNegativeAdduct.Width, stPositiveAdduct.Top + 140);
            stNegativeAdduct.Height = 120;
            stNegativeAdduct.Text = "Negative adducts";
            stNegAdductCheckbox1.Parent = stNegativeAdduct;
            stNegAdductCheckbox1.Location = new Point(10, 15);
            stNegAdductCheckbox1.Text = "-H⁻";
            stNegAdductCheckbox1.Enabled = false;
            stNegAdductCheckbox1.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("-H", currentLipid));};
            stNegAdductCheckbox2.Parent = stNegativeAdduct;
            stNegAdductCheckbox2.Location = new Point(10, 35);
            stNegAdductCheckbox2.Text = "-2H⁻⁻";
            stNegAdductCheckbox2.Enabled = false;
            stNegAdductCheckbox2.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("-2H", currentLipid));};
            stNegAdductCheckbox3.Parent = stNegativeAdduct;
            stNegAdductCheckbox3.Location = new Point(10, 55);
            stNegAdductCheckbox3.Text = "+HCOO⁻";
            stNegAdductCheckbox3.Enabled = false;
            stNegAdductCheckbox3.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+HCOO", currentLipid));};
            stNegAdductCheckbox4.Parent = stNegativeAdduct;
            stNegAdductCheckbox4.Location = new Point(10, 75);
            stNegAdductCheckbox4.Text = "+CH3COO⁻";
            stNegAdductCheckbox4.Enabled = false;
            stNegAdductCheckbox4.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("+CH3COO", currentLipid));};
            
            
            
            
            stFACombobox.BringToFront();
            stFATextbox.BringToFront();
            stFATextbox.Location = new Point(574, 270);
            stFATextbox.Width = faLength;
            stFATextbox.Text = "2, 5, 17-19";
            stFATextbox.TextChanged += delegate(object s, EventArgs e){ updateCarbon(s, new FattyAcidEventArgs( ((Sterol)currentLipid).fag, "" )); };
            toolTip.SetToolTip(stFATextbox, formattingFA);
            stFACombobox.Location = new Point(stFATextbox.Left, stFATextbox.Top - sepText);
            stFACombobox.Width = faLength;
            stFACombobox.SelectedItem = "Fatty acyl chain";
            stFACombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            stFACombobox.SelectedIndexChanged += delegate(object s, EventArgs e){ updateOddEven(s, new FattyAcidEventArgs( ((Sterol)currentLipid).fag, stFATextbox )); };
            stDBTextbox.Location = new Point(stFATextbox.Left + stFATextbox.Width + sep, stFATextbox.Top);
            stDBTextbox.Width = dbLength;
            stDBTextbox.Text = "5-6";
            stDBTextbox.TextChanged += delegate(object s, EventArgs e){ updateDB(s, new FattyAcidEventArgs( ((Sterol)currentLipid).fag, "" )); };
            toolTip.SetToolTip(stDBTextbox, formattingDB);
            stDBLabel.Location = new Point(stDBTextbox.Left, stDBTextbox.Top - sep);
            stDBLabel.Width = dbLength;
            stDBLabel.Text = dbText;
            stHydroxylTextbox.Width = dbLength;
            stHydroxylTextbox.Location = new Point(stDBTextbox.Left + stDBTextbox.Width + sep, stDBTextbox.Top);
            stHydroxylTextbox.TextChanged += delegate(object s, EventArgs e){ updateHydroxyl(s, new FattyAcidEventArgs( ((Sterol)currentLipid).fag, "" )); };
            toolTip.SetToolTip(stHydroxylTextbox, formattingHydroxyl);
            stFAHydroxyLabel.Location = new Point(stHydroxylTextbox.Left, stHydroxylTextbox.Top - sep);
            stFAHydroxyLabel.Text = hydroxylText;
            
            stFACombobox.Visible = false;
            stFATextbox.Visible = false;
            stDBTextbox.Visible = false;
            stDBLabel.Visible = false;
            stHydroxylTextbox.Visible = false;
            stFAHydroxyLabel.Visible = false;
            
            
            
            
            
            stHgListbox.Location = new Point(24, 35);
            stHgListbox.Size = new Size(80, 260);
            stHgListbox.BringToFront();
            stHgListbox.BorderStyle = BorderStyle.Fixed3D;
            stHgListbox.SelectedValueChanged += new System.EventHandler(stHGListboxSelectedValueChanged);
            stHgListbox.MouseMove += new System.Windows.Forms.MouseEventHandler(stHGListboxMouseHover);
            stHgListbox.KeyDown += ListboxSelectAll;
            stPictureBox.Location = new Point(110, 40);
            if (!lipidCreatorInitError && stHgListbox.Items.Count > 0)
            {
                stPictureBox.Image = Image.FromFile(lipidCreator.headgroups[stHgListbox.Items[0].ToString()].pathToBackboneImage);
            }
            stPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            stHgListbox.SendToBack();
            
            
            
            stTypeGroup.Location = new Point(400, 8);
            stTypeGroup.Size = new Size(180, 40);
            stTypeGroup.Text = "Type";
            stTypeGroup.Controls.Add(stIsEster);
            stTypeGroup.Controls.Add(stRegular);
            
            stRegular.Location = new Point(10, 14);
            stRegular.Text = "Regular";
            stRegular.CheckedChanged += new EventHandler(chContainsEsterCheckedChanged);
            
            stIsEster.Location = new Point(90, 14);
            stIsEster.Text = "Ester";
            stIsEster.Width = 70;
            stIsEster.CheckedChanged += new EventHandler(chContainsEsterCheckedChanged);
            
            
            
            
            
            
            
            
            
            // tab for mediators
            mediatorlipidsTab.Controls.Add(medStep1);
            medStep1.Controls.Add(medNegativeAdduct);
            medStep1.Controls.Add(medHgListbox);
            medStep1.Controls.Add(medPictureBox);
            
            mediatorlipidsTab.Text = "Lipid Mediators";
            //mediatorlipidsTab.ToolTipText = "Lipid Mediators";
            mediatorlipidsTab.Location = new Point(0, 0);
            mediatorlipidsTab.Size = this.Size;
            mediatorlipidsTab.AutoSize = true;
            mediatorlipidsTab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mediatorlipidsTab.BackColor = Color.White;
            mediatorlipidsTab.Font = Font;
            
            medStep1.SendToBack();
            medStep1.Location = new Point(10, 10);
            medStep1.Width = Width - 50;
            medStep1.Height = step1Height;
            medStep1.Text = "Step 1: Precursor selection";
            
            medNegativeAdduct.Width = 120;
            medNegativeAdduct.Location = new Point(leftGroupboxes - medNegativeAdduct.Width, 170);
            medNegativeAdduct.Height = 120;
            medNegativeAdduct.Text = "Negative adducts";
            
            medNegAdductCheckbox1.Parent = medNegativeAdduct;
            medNegAdductCheckbox1.Location = new Point(10, 15);
            medNegAdductCheckbox1.Text = "-H⁻";
            medNegAdductCheckbox1.CheckedChanged += delegate(object s, EventArgs e){AdductCheckBoxChecked(s, new AdductCheckedEventArgs("-H", currentLipid));};
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
            
            
            medHgListbox.Location = new Point(34, 35);
            medHgListbox.Size = new Size(140, 260);
            medHgListbox.BringToFront();
            medHgListbox.BorderStyle = BorderStyle.Fixed3D;
            medHgListbox.SelectedValueChanged += new System.EventHandler(medHGListboxSelectedValueChanged);
            medHgListbox.MouseMove += new System.Windows.Forms.MouseEventHandler(medHGListboxMouseHover);
            medHgListbox.KeyDown += ListboxSelectAll;
            medPictureBox.Location = new Point(210, 30);
            if (!lipidCreatorInitError && medHgListbox.Items.Count > 0)
            {
                medPictureBox.Image = Image.FromFile(lipidCreator.headgroups[medHgListbox.Items[0].ToString()].pathToImage);
                medPictureBox.Top = mediatorMiddleHeight - (medPictureBox.Image.Height >> 1);
            }
            medPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            medPictureBox.SendToBack();

            lipidsGridview.Dock = DockStyle.Fill;
            lipidsGridview.DataSource = registeredLipidsDatatable;
            lipidsGridview.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            lipidsGridview.AllowUserToResizeColumns = false;
            lipidsGridview.AllowUserToAddRows = false;
            lipidsGridview.AllowUserToResizeRows = false;
            lipidsGridview.ReadOnly = true;
            lipidsGridview.MultiSelect = false;
            lipidsGridview.RowTemplate.Height = 34;
            lipidsGridview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            lipidsGridview.RowHeadersVisible = false;
            lipidsGridview.ScrollBars = ScrollBars.Vertical;
            if (!lipidCreatorInitError)
            {
                lipidsGridview.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(lipidsGridviewDataBindingComplete);
                lipidsGridview.DoubleClick += new EventHandler(lipidsGridviewDoubleClick);
                lipidsGridview.KeyDown += new KeyEventHandler(lipidsGridviewKeydown);
                lipidsGridview.EditMode = DataGridViewEditMode.EditOnEnter;
            }
            
            lipidsGridviewPanel = new Panel();
            lipidsGridviewPanel.Dock = DockStyle.Fill;
            lipidsGridviewPanel.AutoSize = true;
            lipidsGridviewPanel.AutoScroll = true;
            lipidsGridviewPanel.Controls.Add(lipidsGridview);

            lipidsReviewButtonPanel = new Panel();
            lipidsReviewButtonPanel.AutoSize = true;
            lipidsReviewButtonPanel.Dock = DockStyle.Bottom;
            lipidsReviewButtonPanel.Controls.Add(openReviewFormButton);

            lipidsGroupbox.Dock = DockStyle.Bottom;
            lipidsGroupbox.Text = "Lipid list";
            lipidsGroupbox.Height = minLipidGridHeight;
            lipidsGroupbox.Controls.Add(lipidsGridviewPanel);
            lipidsGroupbox.Controls.Add(lipidsReviewButtonPanel);

            
            lcStep2.Controls.Add(addHeavyIsotopeButton);
            lcStep2.Controls.Add(MS2fragmentsLipidButton);
            lcStep2.Controls.Add(filtersButton);
            lcStep2.SendToBack();
            lcStep2.Location = new Point(10, step1Height + 20);
            lcStep2.Width = 430;
            lcStep2.Height = 60;
            lcStep2.Text = "Step 2: MS/MS selection";

            
            lcStep3.Controls.Add(modifyLipidButton);
            lcStep3.Controls.Add(addLipidButton);
            lcStep3.SendToBack();
            lcStep3.Width = 320;
            lcStep3.Height = 60;
            lcStep3.Location = new Point(plStep1.Left + plStep1.Width - lcStep3.Width, step1Height + 20);
            lcStep3.Text = "Step 3: Assembly registration";
            
            

            addHeavyIsotopeButton.Text = "Manage heavy isotopes";
            addHeavyIsotopeButton.Width = 150;
            addHeavyIsotopeButton.Height = 26;
            addHeavyIsotopeButton.Location = new Point(10, topLowButtons);
            addHeavyIsotopeButton.BackColor = SystemColors.Control;
            addHeavyIsotopeButton.Click += openHeavyIsotopeForm;

            MS2fragmentsLipidButton.Text = "MS2 fragments";
            MS2fragmentsLipidButton.Width = 130;
            MS2fragmentsLipidButton.Height = 26;
            MS2fragmentsLipidButton.Location = new Point(170, topLowButtons);
            MS2fragmentsLipidButton.BackColor = SystemColors.Control;
            MS2fragmentsLipidButton.Click += openMS2Form;

            filtersButton.Text = "Filters";
            filtersButton.Width = 100;
            filtersButton.Height = 26;
            filtersButton.Location = new Point(310, topLowButtons);
            filtersButton.BackColor = SystemColors.Control;
            filtersButton.Click += openFilterDialog;
            
            
            

            addLipidButton.Text = "Add lipid";
            addLipidButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            addLipidButton.Width = 146;
            addLipidButton.Height = 26;
            addLipidButton.Image = addImage;
            addLipidButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            addLipidButton.Location = new Point(lcStep3.Width - addLipidButton.Width - 10, topLowButtons);
            addLipidButton.BackColor = SystemColors.Control;
            addLipidButton.Click += registerLipid;
            
            modifyLipidButton.Text = "Modify lipid";
            modifyLipidButton.Width = 130;
            modifyLipidButton.Height = 26;
            modifyLipidButton.Location = new Point(lcStep3.Width - addLipidButton.Width - modifyLipidButton.Width - 20, topLowButtons);
            modifyLipidButton.BackColor = SystemColors.Control;
            modifyLipidButton.Click += modifyLipid;

            openReviewFormButton.Text = "Review Lipids";
            openReviewFormButton.Width = 130;
            openReviewFormButton.BackColor = SystemColors.Control;
            openReviewFormButton.Dock = DockStyle.Bottom;
            openReviewFormButton.Click += openReviewForm;
            openReviewFormButton.Enabled = false;

            this.Controls.Add(tabControl);
            this.Controls.Add(lipidsGroupbox);
            if (!lipidCreatorInitError)
            {
                this.Text = "LipidCreator  " + LipidCreator.LC_RELEASE_NUMBER;
            }
            else
            {
                this.Text = "LipidCreator  -  error mode";
            }
            this.MaximizeBox = false;
            this.Padding = new Padding(5);

            DefaultCheckboxBGR = plPosAdductCheckbox1.BackColor.R;
            DefaultCheckboxBGG = plPosAdductCheckbox1.BackColor.G;
            DefaultCheckboxBGB = plPosAdductCheckbox1.BackColor.B;
            
            
            
            //Font tabFont2 = new Font(homeTab.Font.FontFamily, 16);
            //homeTab.Font = tabFont2;
            
            homeTab.Controls.Add(homeText);
            homeTab.Controls.Add(homeText3);
            
            homeText.Width = 560;
            homeText.Height = 140;
            homeText.Location = new Point(60, 170);
            homeText.Text = "Targeted assays development based on lipid building blocks:" + Environment.NewLine +
            " • Lipid fragmentation prediction" + Environment.NewLine +
            " • Generation of class specific target lists" + Environment.NewLine +
            " • In-silico spectral library generator" + Environment.NewLine +
            //" • Latest lipid nomenclature" + Environment.NewLine +
            " • Full integration with new small molecule support in Skyline." + Environment.NewLine + Environment.NewLine +
            "LipidCreator offers several interactive tutorials for an easy introduction into" + Environment.NewLine +
            "its functionality:";
            homeText.BackColor = Color.Transparent;
            homeText.ForeColor = Color.White;
            homeText.Font = new Font(homeTab.Font.FontFamily, (int)((REGULAR_FONT_SIZE + 3.0) * FONT_SIZE_FACTOR));
            
            
            
            homeText3.Width = 560;
            homeText3.Height = 80;
            homeText3.Location = new Point(60, 390);
            homeText3.Text = "Citation: Peng, Bing, et al. Nature Communications 11(1):1-14, 2020.";
            homeText3.BackColor = Color.Transparent;
            homeText3.Font = new Font(homeTab.Font.FontFamily, (int)(10.0 * FONT_SIZE_FACTOR), FontStyle.Bold);
            homeText3.ForeColor = Color.White;
            homeText3.Click += homeText3LinkClicked;
            
            
            startFirstTutorialButton = new Button();
            homeTab.Controls.Add(startFirstTutorialButton);
            startFirstTutorialButton.Text = "Start PRM tutorial";
            startFirstTutorialButton.Width = 200;
            startFirstTutorialButton.Height = 26;
            startFirstTutorialButton.Location = new Point(60, 316);
            startFirstTutorialButton.BackColor = SystemColors.Control;
            startFirstTutorialButton.Click += startFirstTutorial;
            
            
            startSecondTutorialButton = new Button();
            homeTab.Controls.Add(startSecondTutorialButton);
            startSecondTutorialButton.Text = "Start SRM tutorial";
            startSecondTutorialButton.Width = 200;
            startSecondTutorialButton.Height = 26;
            startSecondTutorialButton.Location = new Point(300, 316);
            startSecondTutorialButton.BackColor = SystemColors.Control;
            startSecondTutorialButton.Click += startSecondTutorial;
            
            
            startThirdTutorialButton = new Button();
            homeTab.Controls.Add(startThirdTutorialButton);
            startThirdTutorialButton.Text = "Start heavy isotope tutorial";
            startThirdTutorialButton.Width = 200;
            startThirdTutorialButton.Height = 26;
            startThirdTutorialButton.Location = new Point(60, 350);
            startThirdTutorialButton.BackColor = SystemColors.Control;
            startThirdTutorialButton.Click += startThirdTutorial;
            
            
            startFourthTutorialButton = new Button();
            homeTab.Controls.Add(startFourthTutorialButton);
            startFourthTutorialButton.Text = "Start collision energy tutorial";
            startFourthTutorialButton.Width = 200;
            startFourthTutorialButton.Height = 26;
            startFourthTutorialButton.Location = new Point(300, 350);
            startFourthTutorialButton.BackColor = SystemColors.Control;
            startFourthTutorialButton.Click += startFourthTutorial;
            
            if (LipidCreator.LC_OS == PlatformID.Unix)
            {
                
                startFirstTutorialButton.Click -= startFirstTutorial;
                startSecondTutorialButton.Click -= startSecondTutorial;
                startThirdTutorialButton.Click -= startThirdTutorial;
                startFourthTutorialButton.Click -= startFourthTutorial;
                
                string incompatible = "Interactive tutorials are currently not supported on Linux-based operating systems. Please check the manual under Help -> Documentation. We apologize for the inconvenience.";
                
                startFirstTutorialButton.Click += delegate(object s, EventArgs e){ MessageBox.Show(incompatible, "Incompatible operating system"); };
                startSecondTutorialButton.Click += delegate(object s, EventArgs e){ MessageBox.Show(incompatible, "Incompatible operating system"); };
                startThirdTutorialButton.Click += delegate(object s, EventArgs e){ MessageBox.Show(incompatible, "Incompatible operating system"); };
                startFourthTutorialButton.Click += delegate(object s, EventArgs e){ MessageBox.Show(incompatible, "Incompatible operating system"); };
            }
            
            this.SizeChanged += new EventHandler(windowSizeChanged);
            this.FormClosing += new FormClosingEventHandler(windowOnClosing);
            
            controlElements = new ArrayList(){menuFile, menuOptions, menuHelp, addLipidButton, modifyLipidButton, MS2fragmentsLipidButton, addHeavyIsotopeButton, filtersButton, plFA1Checkbox3, plFA1Checkbox2, plFA1Checkbox1, plFA2Checkbox1, plPosAdductCheckbox2, plPosAdductCheckbox3, plIsCL, plRegular, plIsLyso, plFA1Textbox, plFA2Textbox, plDB1Textbox, plDB2Textbox, plHydroxyl1Textbox, plHydroxyl2Textbox, plFA1Combobox, plFA2Combobox, plHgListbox, plHGLabel, plRepresentativeFA, plPositiveAdduct, plNegativeAdduct, openReviewFormButton, startFirstTutorialButton, startSecondTutorialButton, startThirdTutorialButton, startFourthTutorialButton, lipidsGridview, menuTranslate, menuWizard, menuCollisionEnergy, menuCollisionEnergyOpt, menuMS2Fragments, menuIsotopes, menuClearLipidList, menuResetCategory, menuResetLipidCreator, menuStatistics, glFA1Checkbox3, glFA1Checkbox2, glFA1Checkbox1, glFA2Checkbox3, glFA2Checkbox2, glFA2Checkbox1, glFA3Checkbox3, glFA3Checkbox2, glFA3Checkbox1, glPictureBox, glArrow, glFA1Textbox, glFA2Textbox, glFA3Textbox, glDB1Textbox, glDB2Textbox, glDB3Textbox, glHydroxyl1Textbox, glHydroxyl2Textbox, glHydroxyl3Textbox, glFA1Combobox, glFA2Combobox, glFA3Combobox, glHgListbox, glHGLabel, glContainsSugar, glRepresentativeFA, glPositiveAdduct, glNegativeAdduct, plHasPlasmalogen};
            
            
        }

        #endregion
    }
}
