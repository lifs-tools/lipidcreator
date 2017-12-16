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
            tutorialStep = -1;
            nextEnabled = true;
            this.creatorGUI = creatorGUI;
            currentTab = LipidCategory.NoLipid;
            maxSteps = new Dictionary<int, int>(){
                {(int)Tutorials.NoTutorial, 0},
                {(int)Tutorials.TutorialPRM, 10},
                {(int)Tutorials.TutorialMRM, 0},
                {(int)Tutorials.TutorialHeavyLabeled, 0}
            };
        }
        
        public void startTutorial(Tutorials t)
        {
            tutorial = t;
            tutorialStep = -1;
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
            tutorialStep = forward ? (tutorialStep + 1) : (Math.Max(tutorialStep - 1, 0));
            if (tutorial == Tutorials.TutorialPRM) TutorialPRMStep();
            else if (tutorial == Tutorials.TutorialMRM) TutorialMRMStep();
            else if (tutorial == Tutorials.TutorialHeavyLabeled) TutorialHeavyLabeledStep();
            else quitTutorial();
        }
        
        public void quitTutorial()
        {
            tutorial = Tutorials.NoTutorial;
            tutorialStep = -1;
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
        
        public void TutorialPRMStep()
        {
            disableEverything();
            creatorGUI.tutorialArrow.Visible = false;
            switch(tutorialStep)
            {   
                case 0:
                    changeTab(LipidCategory.PhosphoLipid);
                    creatorGUI.tutorialWindow.text.Text = "Welcome to the first tutorial of LipidCreator. It will guide you interactively through this tool.";
                    nextEnabled = true;
                    
                    creatorGUI.tutorialArrow.Visible = true;
                    
                    break;
                    
                case 1:
                    changeTab(LipidCategory.SphingoLipid);
                    nextEnabled = true;
                    
                    break;
                    
                case 2:
                    changeTab(LipidCategory.PhosphoLipid);
                    nextEnabled = false;
                    break;
                    
                case 3:
                    quitTutorial();
                    break;
                    
                case 4:
                    break;
                    
                case 5:
                    break;
                    
                case 6:
                    break;
            }
        
        }
        
        
        public void TutorialMRMStep()
        {
        
        }
        
        
        public void TutorialHeavyLabeledStep()
        {
        
        }
    }    
}