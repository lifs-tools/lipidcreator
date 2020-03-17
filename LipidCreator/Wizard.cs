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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using log4net;

namespace LipidCreator
{
    [Serializable]
    public partial class Wizard : Form
    {
        
        //private static readonly ILog log = LogManager.GetLogger(typeof(Wizard));
        public CreatorGUI creatorGUI;
        public int wizardStep;
        
        public enum WizardSteps {Welcome, SelectCategory, SelectSpecies, Finish};

        public Wizard(CreatorGUI _creatorGUI)
        {
            creatorGUI = _creatorGUI;
            InitializeComponent();
            wizardStep = (int)WizardSteps.Welcome;
            processWizardSteps();
        }
        
        
        
        private void resetAllControls(){
            cancelButton.Enabled = true;
            continueButton.Enabled = true;
            cancelButton.Text = "Cancel";
            continueButton.Text = "Continue";
        }
        
        

        private void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        
        
        

        private void continueClick(object sender, EventArgs e)
        {
            if (wizardStep == (int)WizardSteps.Finish) wizardStep = 0;
            wizardStep += 1;
            
            processWizardSteps();
        }
        
        
        public void processWizardSteps(){
            resetAllControls();
            switch (wizardStep){
                case (int)WizardSteps.Welcome:
                    labelInformation.Text = "Welcome to the magic world of LipidCreator. This wizard will guide you.";
                    break;
                    
                case (int)WizardSteps.SelectCategory:
                    cancelButton.Enabled = false;
                    break;
                    
                case (int)WizardSteps.SelectSpecies:
                    cancelButton.Text = "Close";
                    break;
                    
                case (int)WizardSteps.Finish:
                    break;
        
            }
        }
    }
}
