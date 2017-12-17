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
                {(int)Tutorials.TutorialPRM, 10},
                {(int)Tutorials.TutorialMRM, 0},
                {(int)Tutorials.TutorialHeavyLabeled, 0}
            };
            creatorGUI.plHgListbox.SelectedValueChanged += new System.EventHandler(listBoxInteraction);
            creatorGUI.tabControl.SelectedIndexChanged += new System.EventHandler(tabInteraction);
            creatorGUI.plFA1Textbox.TextChanged += new EventHandler(textBoxInteraction);
        }
        
        
        
        public void startTutorial(Tutorials t)
        {
            tutorial = t;
            tutorialStep = 3;
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
                ((TabPage)creatorGUI.tabList[(int)currentTab]).Enabled = true;
            }
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
            else nextEnabled = false;
            creatorGUI.tutorialWindow.Refresh();
        }
        
        
        
        public void TutorialPRMStep()
        {
            disableEverything();
            creatorGUI.tutorialArrow.Visible = false;
            switch(tutorialStep)
            {   
                case 1:
                    changeTab(LipidCategory.NoLipid);
                    
                    creatorGUI.tutorialWindow.update(new Size(540, 160), new Point(140, 200), "Welcome to the first tutorial of LipidCreator. It will guide you interactively through this tool by showing you all necessary steps to create both a transition list and a spectral library for targeted lipidomics.");
                    nextEnabled = true;
                    
                    
                    break;
                    
                case 2:
                    changeTab(LipidCategory.NoLipid);
                    creatorGUI.tutorialWindow.update(new Size(540, 160), new Point(140, 200), "Let's start. LipidCreator offers computation for five lipid categories, namely glycerolipids, phopholipids, sphingolipids, cholesterols and mediators. To go on the lipid assembly form for phopholipids, please click at the 'Phospholipids' tab.");
                    
                    creatorGUI.tutorialArrow.update(new Point(290, 40), "lt");
                    
                    nextEnabled = false;
                    
                    break;
                    
                case 3:
                    changeTab(LipidCategory.PhosphoLipid);
                    creatorGUI.tutorialWindow.update(new Size(540, 160), new Point(340, 200),                     "Great, phospholipids have multiple headgroups. The user can multiply select them. We are interested in phosphatidylglycerol (PG). Please select only PG as headgroup and continue.");
                    
                    
                    creatorGUI.tutorialArrow.update(new Point(110, 186), "tl");
                    
                    creatorGUI.plHgListbox.Enabled = true;
                    nextEnabled = false;
                    break;
                    
                case 4:
                    changeTab(LipidCategory.PhosphoLipid);
                    creatorGUI.tutorialWindow.update(new Size(500, 200), new Point(460, 200),                     "LipidCreator was designed to describe a set of fatty acids (FAs) instead of FA separately. PG contains two FAs. We want to create a transition list of PGs with carbon length of first FA between 14 and 18 and additionally 20. Please type in first FA carbon field '14-18, 20'.");
                    
                    creatorGUI.tutorialArrow.update(new Point(286, 88), "tr");
                    
                    creatorGUI.plFA1Textbox.Enabled = true;
                    nextEnabled = false;
                    break;
                    
                case 5:
                    changeTab(LipidCategory.PhosphoLipid);
                    break;
                    
                case 6:
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