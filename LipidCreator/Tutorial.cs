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
    
    public enum Tutorials {NoTutorial = -1, TutorialPRM = 0, TutorialMRM = 1, TutorialHeavyLabeled = 2};
    

    [Serializable]
    public class Tutorial
    {
        public Tutorials tutorial;
        public int tutorialStep;
        public CreatorGUI creatorGUI;
        public ArrayList elementsEnabledState;
        public LipidCategory currentTab;
        public Dictionary<int, int> maxSteps;
        public bool nextEnabled;
        
        public Tutorial(CreatorGUI creatorGUI)
        {
            tutorial = Tutorials.NoTutorial;
            tutorialStep = 0;
            nextEnabled = true;
            this.creatorGUI = creatorGUI;
            currentTab = LipidCategory.NoLipid;
            maxSteps = new Dictionary<int, int>(){
                {(int)Tutorials.NoTutorial, 0},
                {(int)Tutorials.TutorialPRM, 20},
                {(int)Tutorials.TutorialMRM, 0},
                {(int)Tutorials.TutorialHeavyLabeled, 0}
            };
            creatorGUI.plHgListbox.SelectedValueChanged += new System.EventHandler(listBoxInteraction);
            creatorGUI.tabControl.SelectedIndexChanged += new System.EventHandler(tabInteraction);
            creatorGUI.plFA1Textbox.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.plDB1Textbox.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.plFA2Textbox.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.plDB2Textbox.TextChanged += new EventHandler(textBoxInteraction);
            creatorGUI.plPosAdductCheckbox1.CheckedChanged += new EventHandler(checkBoxInteraction);
            creatorGUI.plPosAdductCheckbox3.CheckedChanged += new EventHandler(checkBoxInteraction);
            creatorGUI.addLipidButton.Click += buttonInteraction;
        }
        
        
        
        public void startTutorial(Tutorials t)
        {
            tutorial = t;
            tutorialStep = 0;
            nextEnabled = true;
            currentTab = LipidCategory.NoLipid;
            creatorGUI.tutorialArrow.BringToFront();
            elementsEnabledState = new ArrayList();
            foreach (Object element in creatorGUI.controlElements)
            {
                if (element is MenuItem) 
                {
                    elementsEnabledState.Add(((MenuItem)element).Enabled);
                    ((MenuItem)element).Enabled = false;
                }
                else
                {
                    elementsEnabledState.Add(((Control)element).Enabled);
                    ((Control)element).Enabled = false;
                }
            }
            creatorGUI.tutorialWindow.Visible = true;
            creatorGUI.tutorialWindow.BringToFront();
            nextTutorialStep(true);
        }
        
        
        
        public void nextTutorialStep(bool forward)
        {
            tutorialStep = forward ? (tutorialStep + 1) : (Math.Max(tutorialStep - 1, 1));
            if (tutorial == Tutorials.TutorialPRM) TutorialPRMStep();
            else if (tutorial == Tutorials.TutorialMRM) TutorialMRMStep();
            else if (tutorial == Tutorials.TutorialHeavyLabeled) TutorialHeavyLabeledStep();
            else quitTutorial();
        }
        
        
        
        public void quitTutorial()
        {
            tutorial = Tutorials.NoTutorial;
            tutorialStep = 0;
            creatorGUI.tutorialArrow.Visible = false;
            creatorGUI.tutorialWindow.Visible = false;
            
            for (int i = 0; i < elementsEnabledState.Count; ++i)
            {
                Object element = creatorGUI.controlElements[i];
                if (element is MenuItem) 
                {
                    ((MenuItem)element).Enabled = (bool)elementsEnabledState[i];
                }
                else
                {
                    ((Control)element).Enabled = (bool)elementsEnabledState[i];
                }
            }
            creatorGUI.changeTab((int)currentTab);
        }
        
        
        
        public void disableEverything()
        {
            foreach (Object element in creatorGUI.controlElements)
            {
                if (element is MenuItem) 
                {
                    ((MenuItem)element).Enabled = false;
                }
                else
                {
                    ((Control)element).Enabled = false;
                }
            }
            creatorGUI.tutorialArrow.Visible = false;
        }
        
        
        
        public void changeTab(LipidCategory lip)
        {
            if (currentTab != lip)
            {
                currentTab = lip;
                creatorGUI.changeTab((int)currentTab);
                ((TabPage)creatorGUI.tabList[(int)currentTab]).Controls.Add(creatorGUI.tutorialArrow);
                creatorGUI.tutorialArrow.BringToFront();
            }
            ((TabPage)creatorGUI.tabList[(int)currentTab]).Enabled = true;
        }
        
        
        public void listBoxInteraction(object sender, System.EventArgs e)
        {
            ListBox box = (ListBox)sender;
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 3 && box.SelectedItems.Count == 1 && box.SelectedItems[0].ToString().Equals("PG")) nextEnabled = true;
            else nextEnabled = false;
            creatorGUI.tutorialWindow.Refresh();
        }
        
        
        
        public void tabInteraction(Object sender,  EventArgs e)
        {
            if (creatorGUI.changingTabForced) return;
            if (tutorial == Tutorials.NoTutorial) return;
            if (creatorGUI.currentTabIndex == (int)LipidCategory.PhosphoLipid && tutorial == Tutorials.TutorialPRM && tutorialStep == 2)
            {
                nextTutorialStep(true);
                return;
            }
            else if (creatorGUI.currentTabIndex == (int)LipidCategory.SphingoLipid && tutorial == Tutorials.TutorialPRM && tutorialStep == 11)
            {
                nextTutorialStep(true);
                return;
            }
            creatorGUI.changeTab((int)currentTab);
        }
        
        
        
        public void textBoxInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 4)
            {
                HashSet<int> expected = new HashSet<int>(){14, 15, 16, 17, 18, 20};
                HashSet<int> carbonCounts = ((PLLipid)creatorGUI.lipidTabList[(int)LipidCategory.PhosphoLipid]).fag1.carbonCounts;
                nextEnabled = carbonCounts != null && carbonCounts.Intersect(expected).Count() == 6;
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == 5)
            {
                HashSet<int> expected = new HashSet<int>(){0, 1};
                HashSet<int> doubleBondCounts = ((PLLipid)creatorGUI.lipidTabList[(int)LipidCategory.PhosphoLipid]).fag1.doubleBondCounts;
                nextEnabled = doubleBondCounts != null && doubleBondCounts.Intersect(expected).Count() == 2;
            }
            else if (tutorial == Tutorials.TutorialPRM && tutorialStep == 8)
            {
                nextEnabled = true;
                
                HashSet<int> expectedFA = new HashSet<int>(){8, 9, 10};
                HashSet<int> carbonCounts = ((PLLipid)creatorGUI.lipidTabList[(int)LipidCategory.PhosphoLipid]).fag2.carbonCounts;
                nextEnabled = carbonCounts != null && carbonCounts.Intersect(expectedFA).Count() == 3;
                
                HashSet<int> expectedDB = new HashSet<int>(){2};
                HashSet<int> doubleBondCounts = ((PLLipid)creatorGUI.lipidTabList[(int)LipidCategory.PhosphoLipid]).fag2.doubleBondCounts;
                nextEnabled = nextEnabled && doubleBondCounts != null && doubleBondCounts.Intersect(expectedDB).Count() == 1;
            }
            creatorGUI.tutorialWindow.Refresh();
        }
        
        
        public void checkBoxInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 9)
            {
                nextEnabled = creatorGUI.plPosAdductCheckbox1.Checked && !creatorGUI.plPosAdductCheckbox3.Checked;
            }
            creatorGUI.tutorialWindow.Refresh();
        }
        
        
        
        public void buttonInteraction(Object sender, EventArgs e)
        {
            if (tutorial == Tutorials.TutorialPRM && tutorialStep == 10)
            {
                nextTutorialStep(true);
            }
        }
        
        
        public void TutorialPRMStep()
        {
            disableEverything();
            creatorGUI.tutorialWindow.Visible = false;
            creatorGUI.tutorialArrow.Visible = false;
            switch(tutorialStep)
            {   
                case 1:
                    changeTab(LipidCategory.NoLipid);
                    
                    creatorGUI.tutorialWindow.update(new Size(540, 200), new Point(140, 200), "Click on continue", "Welcome to the first tutorial of LipidCreator. It will guide you interactively through this tool by showing you all necessary steps to create both a transition list and a spectral library for targeted lipidomics.");
                    
                    nextEnabled = true;
                    break;
                    
                    
                case 2:
                    changeTab(LipidCategory.NoLipid);
                    TabPage p = (TabPage)creatorGUI.tabList[(int)LipidCategory.PhosphoLipid];
                    creatorGUI.tutorialArrow.update(new Point((int)(creatorGUI.tabControl.ItemSize.Width * 2.5), 40), "lt");
                    
                    creatorGUI.tutorialWindow.update(new Size(540, 200), new Point(140, 200), "click on 'Phosholipids' tab", "Let's start. LipidCreator offers computation for five lipid categories, namely glycerolipids, phopholipids, sphingolipids, cholesterols and mediators. To go on the lipid assembly form for phopholipids, please click at the 'Phospholipids' tab.");
                    
                    nextEnabled = false;
                    break;
                    
                    
                case 3:
                    changeTab(LipidCategory.PhosphoLipid);
                    
                    ListBox plHG = creatorGUI.plHgListbox;
                    int plHGpg = 0;
                    for (; plHGpg < plHG.Items.Count; ++plHGpg) if (plHG.Items[plHGpg].ToString().Equals("PG")) break;
                    creatorGUI.tutorialArrow.update(new Point(plHG.Location.X + plHG.Size.Width, plHG.Location.Y + (int)((plHGpg + 0.5) * plHG.ItemHeight)), "tl");
                    
                    creatorGUI.tutorialWindow.update(new Size(500, 200), new Point(460, 300), "Select solely 'PG' headgroup", "Great, phospholipids have multiple headgroups. The user can multiply select them. Notice that when hovering above the headgroups, the according adducts are highlighted. We are interested in phosphatidylglycerol (PG). Please select only PG as headgroup and continue.");
                    
                    creatorGUI.plHgListbox.SelectedItems.Clear();
                    creatorGUI.plHgListbox.Enabled = true;
                    nextEnabled = false;
                    break;
                    
                    
                case 4:
                    changeTab(LipidCategory.PhosphoLipid);
                    TextBox plFA1 = creatorGUI.plFA1Textbox;
                    creatorGUI.tutorialArrow.update(new Point(plFA1.Location.X, plFA1.Location.Y + (plFA1.Size.Height >> 1)), "tr");
                    
                    creatorGUI.tutorialWindow.update(new Size(500, 200), new Point(460, 300), "Set first fatty acid carbon lengths to '14-18, 20'", "LipidCreator was designed to describe a set of fatty acids (FAs) instead of FA separately. PG contains two FAs. We want to create a transition list of PGs with carbon length of first FA between 14 and 18 and additionally 20. Please type in first FA carbon field '14-18, 20'.");
                                      
                    
                    plFA1.Text = "12 - 15";
                    plFA1.Enabled = true;
                    nextEnabled = false;
                    break;
                    
                    
                case 5:
                    changeTab(LipidCategory.PhosphoLipid);
                    
                    TextBox plDB1 = creatorGUI.plDB1Textbox;
                    creatorGUI.tutorialArrow.update(new Point(plDB1.Location.X + plDB1.Size.Width, plDB1.Location.Y + (plDB1.Size.Height >> 1)), "tl");
                    
                    creatorGUI.tutorialWindow.update(new Size(500, 200), new Point(460, 300), "Set first double bond occurrences to '0-1'", "Here, one can specify the number of double bonds (DBs) for first FA. We are for example interested zero and one DBs. Please type in first FA double bond field '0-1' or '0,1'.");
                                      
                    
                    plDB1.Text = "0";
                    plDB1.Enabled = true;
                    nextEnabled = false;
                    break;
                    
                    
                case 6:
                    changeTab(LipidCategory.PhosphoLipid);
                    TextBox plHyd1 = creatorGUI.plHydroxyl1Textbox;
                    creatorGUI.tutorialArrow.update(new Point(plHyd1.Location.X + (plHyd1.Size.Width >> 1), plHyd1.Location.Y + plHyd1.Size.Height), "rt");
                    
                    creatorGUI.tutorialWindow.update(new Size(500, 200), new Point(460, 300), "Continue", "Even more parameters can be set for fatty acids. For instance, up to ten hydroxyl groups can be adjusted to FAs. In this tutorial, we stick to zero hydroxyls.");
                    
                    break;
                    
                    
                case 7:
                    changeTab(LipidCategory.PhosphoLipid);
                    CheckBox plFACheck1 = creatorGUI.plFA1Checkbox1;
                    creatorGUI.tutorialArrow.update(new Point(plFACheck1.Location.X, plFACheck1.Location.Y + (plFACheck1.Size.Height >> 1)), "tr");
                    
                    creatorGUI.tutorialWindow.update(new Size(500, 200), new Point(460, 300), "Continue", "Additionally, fatty acids with ether bond or with ester bond (plasmenyl and plasmanyl) can be created.");
                    
                    break;
                    
                    
                case 8:
                    changeTab(LipidCategory.PhosphoLipid);
                    TextBox plFA2 = creatorGUI.plFA2Textbox;
                    creatorGUI.tutorialArrow.update(new Point(plFA2.Location.X, plFA2.Location.Y + (plFA2.Size.Height >> 1)), "tr");
                    
                    creatorGUI.tutorialWindow.update(new Size(500, 200), new Point(460, 300), "Set second FA carbon lengths to '8-10' and DB to '2'", "For the second fatty acid, we are interested in carbon length 8-10 and exactly 2 double bonds. Please make the following adjustments.");
                    
                    plFA2.Text = "12 - 15";
                    plFA2.Enabled = true;
                    creatorGUI.plDB2Textbox.Text = "0";
                    creatorGUI.plDB2Textbox.Enabled = true;
                    nextEnabled = false;
                    break;
                    
                    
                case 9:
                    changeTab(LipidCategory.PhosphoLipid);
                    CheckBox adductP1 = creatorGUI.plPosAdductCheckbox1;
                    GroupBox P1 = creatorGUI.plPositiveAdduct;
                    creatorGUI.tutorialArrow.update(new Point(P1.Location.X, P1.Location.Y + (P1.Size.Height >> 1)), "tr");
                    
                    creatorGUI.tutorialWindow.update(new Size(500, 200), new Point(160, 300), "Select +H(+) adduct", "Several adducts are possible for selection. By default, for PG only the negative adduct -H(-) is selected. Please select the positive adduct +H(+) and proceed.");
                    
                    
                    adductP1.Checked = false;
                    P1.Enabled = true;
                    adductP1.Enabled = true;
                    break;
                    
                    
                case 10:
                    changeTab(LipidCategory.PhosphoLipid);
                    Button plAddLipid = creatorGUI.addLipidButton;
                    creatorGUI.tutorialArrow.update(new Point(plAddLipid.Location.X + (plAddLipid.Size.Width >> 1), plAddLipid.Location.Y), "rb");
                    
                    creatorGUI.tutorialWindow.update(new Size(500, 200), new Point(160, 300), "Add lipid", "LipidCreator has a basket system. Once a lipid assembly is set, one can put the assembly into the basket and proceed with other assemblies. Please add the lipid.");
                    
                    
                    creatorGUI.plPosAdductCheckbox1.Checked = true; // TODO: remove
                    creatorGUI.plHgListbox.SelectedIndices.Add(8); // TODO: remove
                    creatorGUI.lipidCreator.registeredLipids.Clear();
                    creatorGUI.refreshRegisteredLipidsTable();
                    creatorGUI.addLipidButton.Enabled = true;
                    nextEnabled = false;
                    break;
                    
                /*    
                case 11:
                    changeTab(LipidCategory.PhosphoLipid);
                    TabPage s = (TabPage)creatorGUI.tabList[(int)LipidCategory.SphingoLipid];
                    creatorGUI.tutorialArrow.update(new Point((int)(creatorGUI.tabControl.ItemSize.Width * 3.5), 0), "rt");
                    
                    creatorGUI.tutorialWindow.update(new Size(500, 200), new Point(160, 300), "Foo", "Ok, we continue with as second assembly. Let's take randomly sphingolipids. Please change the view to sphingolipids.");
                    break;
                    
                    
                case 12:
                    changeTab(LipidCategory.SphingoLipid);
                    
                    creatorGUI.tutorialWindow.update(new Size(500, 200), new Point(460, 300), "The structure of the sphingolipids is very similar to phopholipids. The only difference is that for the long chain base either two or three hydroxyl groups are selectable and the fatty acid is restricted to the ether bond. The headgroup (class) selection remains the same.");
                    break;
                    
                    
                case 13:
                    changeTab(LipidCategory.SphingoLipid);
                    
                    creatorGUI.tutorialWindow.update(new Size(500, 200), new Point(460, 300), "The structure of the sphingolipids is very similar to phopholipids. The only difference is that for the long chain base either two or three hydroxyl groups are selectable and the fatty acid is restricted to the ether bond. The headgroup (class) selection remains the same.");
                    break;
                */
                default:
                    quitTutorial();
                    break;
            }
        
            creatorGUI.tutorialWindow.Refresh();
        }
        
        
        public void TutorialMRMStep()
        {
        
        }
        
        
        public void TutorialHeavyLabeledStep()
        {
        
        }
    }    
}