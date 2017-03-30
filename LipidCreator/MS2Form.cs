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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LipidCreator
{
    [Serializable]
    public partial class MS2Form : Form
    {
        
        public Image fragmentComplete = null;
        public Lipid currentLipid;
        public ArrayList positiveIDs;
        public ArrayList negativeIDs;
        public CreatorGUI creatorGUI;
        public bool senderInterupt;
        
        public MS2Form(CreatorGUI creatorGUI, Lipid currentLipid)
        {
            this.creatorGUI = creatorGUI;
            positiveIDs = new ArrayList();
            negativeIDs = new ArrayList();
            senderInterupt = false;
            
            
            if (currentLipid is CLLipid ) this.currentLipid = new CLLipid((CLLipid)currentLipid);
            else if (currentLipid is GLLipid ) this.currentLipid = new GLLipid((GLLipid)currentLipid);
            else if (currentLipid is PLLipid ) this.currentLipid = new PLLipid((PLLipid)currentLipid);
            else if (currentLipid is SLLipid ) this.currentLipid = new SLLipid((SLLipid)currentLipid);
            
            InitializeComponent(currentLipid.MS2Fragments);            
            tabChange(0);
        }

        void checkedListBoxMouseLeave(object sender, EventArgs e)
        {
            pictureBoxFragments.Image = fragmentComplete;
        }
        
        void checkedListBoxPositiveSelectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            String lipidClass = ((TabPage)tabPages[tabControlFragments.SelectedIndex]).Text;
            ArrayList currentFragments = currentLipid.MS2Fragments[lipidClass];
            for (int i = 0; i < currentFragments.Count; ++i)
            {
                if (((MS2Fragment)currentFragments[i]).fragmentCharge > 0) ((MS2Fragment)currentFragments[i]).fragmentSelected = true; 
            }
            for (int i = 0; i < checkedListBoxPositiveFragments.Items.Count; ++i)
            {
                checkedListBoxPositiveFragments.SetItemChecked(i, true);
            }
            senderInterupt = false;
        }
        
        void checkedListBoxPositiveDeselectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            String lipidClass = ((TabPage)tabPages[tabControlFragments.SelectedIndex]).Text;
            ArrayList currentFragments = currentLipid.MS2Fragments[lipidClass];
            for (int i = 0; i < currentFragments.Count; ++i)
            {
                if (((MS2Fragment)currentFragments[i]).fragmentCharge > 0) ((MS2Fragment)currentFragments[i]).fragmentSelected = false;  
            }
            for (int i = 0; i < checkedListBoxPositiveFragments.Items.Count; ++i)
            {
                checkedListBoxPositiveFragments.SetItemChecked(i, false);
            }
            senderInterupt = false;
        }
        
        void checkedListBoxNegativeSelectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            String lipidClass = ((TabPage)tabPages[tabControlFragments.SelectedIndex]).Text;
            ArrayList currentFragments = currentLipid.MS2Fragments[lipidClass];
            for (int i = 0; i < currentFragments.Count; ++i)
            {
                if (((MS2Fragment)currentFragments[i]).fragmentCharge < 0) ((MS2Fragment)currentFragments[i]).fragmentSelected = true; 
            }
            for (int i = 0; i < checkedListBoxNegativeFragments.Items.Count; ++i)
            {
                checkedListBoxNegativeFragments.SetItemChecked(i, true);
            }
            senderInterupt = false;
        }
        
        void checkedListBoxNegativeDeselectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            String lipidClass = ((TabPage)tabPages[tabControlFragments.SelectedIndex]).Text;
            ArrayList currentFragments = currentLipid.MS2Fragments[lipidClass];
            for (int i = 0; i < currentFragments.Count; ++i)
            {
                if (((MS2Fragment)currentFragments[i]).fragmentCharge < 0) ((MS2Fragment)currentFragments[i]).fragmentSelected = false; 
            }
            for (int i = 0; i < checkedListBoxNegativeFragments.Items.Count; ++i)
            {
                checkedListBoxNegativeFragments.SetItemChecked(i, false);
            }
            senderInterupt = false;
        }

        private void checkedListBoxPositiveMouseHover(object sender, MouseEventArgs e)
        {

            toolTip1.Hide(this.checkedListBoxPositiveFragments);
            toolTip1.SetToolTip(this.checkedListBoxNegativeFragments, "");
            Point point = checkedListBoxPositiveFragments.PointToClient(Cursor.Position);
            int hoveredIndex = checkedListBoxPositiveFragments.IndexFromPoint(point);

            if (hoveredIndex != -1)
            {
                int fragmentIndex = (int)positiveIDs[hoveredIndex];
                String lipidClass = ((TabPage)tabPages[tabControlFragments.SelectedIndex]).Text;
                String filePath = ((MS2Fragment)currentLipid.MS2Fragments[lipidClass][fragmentIndex]).fragmentFile;
                if (filePath != null) pictureBoxFragments.Image = Image.FromFile(filePath);
                
                // create tool tip
                MS2Fragment fragment = (MS2Fragment)currentLipid.MS2Fragments[lipidClass][fragmentIndex];                
                string chemForm = "";
                string baseName = "";
                string connector = "";
                string lBracket = "";
                string rBracket = "";
                bool chemAdding = true;
                
                if (fragment.fragmentBase.Count > 0)
                {
                    foreach (string bs in fragment.fragmentBase)
                    {
                        if (baseName.Length > 0) baseName += " + ";
                        baseName += bs;
                    }
                }
                
                foreach (DataRow row in fragment.fragmentElements.Rows)
                {
                    if (Convert.ToInt32(row["Count"]) != 0)
                    {
                        chemForm += Convert.ToString(row["Shortcut"]) + Convert.ToString(Math.Abs(Convert.ToInt32(row["Count"])));
                        chemAdding = Convert.ToInt32(row["Count"]) > 0;
                    }
                }
                if (baseName.Length > 0 && chemForm.Length > 0)
                {
                    connector = chemAdding ? " + " : " - ";
                    lBracket = "(";
                    rBracket = ")";
                }
                string toolTipText = lBracket + baseName + connector + chemForm + rBracket + "+";
                toolTip1.SetToolTip(this.checkedListBoxPositiveFragments, toolTipText);
            }
            else
            {
                pictureBoxFragments.Image = fragmentComplete;
            }
        }

        private void checkedListBoxNegativeMouseHover(object sender, MouseEventArgs e)
        {

            toolTip1.Hide(this.checkedListBoxNegativeFragments);
            toolTip1.SetToolTip(this.checkedListBoxPositiveFragments, "");
            Point point = checkedListBoxNegativeFragments.PointToClient(Cursor.Position);
            int hoveredIndex = checkedListBoxNegativeFragments.IndexFromPoint(point);

            if (hoveredIndex != -1)
            {
                int fragmentIndex = (int)negativeIDs[hoveredIndex];
                String lipidClass = ((TabPage)tabPages[tabControlFragments.SelectedIndex]).Text;
                String filePath = ((MS2Fragment)currentLipid.MS2Fragments[lipidClass][fragmentIndex]).fragmentFile;
                if (filePath != null) pictureBoxFragments.Image = Image.FromFile(filePath);
                
                // create tool tip
                MS2Fragment fragment = (MS2Fragment)currentLipid.MS2Fragments[lipidClass][fragmentIndex];                
                string chemForm = "";
                string baseName = "";
                string connector = "";
                string lBracket = "";
                string rBracket = "";
                bool chemAdding = true;
                
                if (fragment.fragmentBase.Count > 0)
                {
                    foreach (string bs in fragment.fragmentBase)
                    {
                        if (baseName.Length > 0) baseName += " + ";
                        baseName += bs;
                    }
                }
                
                foreach (DataRow row in fragment.fragmentElements.Rows)
                {
                    if (Convert.ToInt32(row["Count"]) != 0)
                    {
                        chemForm += Convert.ToString(row["Shortcut"]) + Convert.ToString(Math.Abs(Convert.ToInt32(row["Count"])));
                        chemAdding = Convert.ToInt32(row["Count"]) > 0;
                    }
                }
                if (baseName.Length > 0 && chemForm.Length > 0)
                {
                    connector = chemAdding ? " + " : " - ";
                    lBracket = "(";
                    rBracket = ")";
                }
                string toolTipText = lBracket + baseName + connector + chemForm + rBracket + "-";
                toolTip1.SetToolTip(this.checkedListBoxNegativeFragments, toolTipText);
            }
            else
            {
                pictureBoxFragments.Image = fragmentComplete;
            }
        }

        public void tabIndexChanged(Object sender, EventArgs e)
        {
            tabChange(((TabControl)sender).SelectedIndex);
        }

        public void tabChange(int index)
        {
            ((TabPage)tabPages[index]).Controls.Add(checkedListBoxNegativeFragments);
            ((TabPage)tabPages[index]).Controls.Add(labelPositiveFragments);
            ((TabPage)tabPages[index]).Controls.Add(labelNegativeFragments);
            ((TabPage)tabPages[index]).Controls.Add(labelFragmentDescriptionBlack);
            ((TabPage)tabPages[index]).Controls.Add(labelFragmentDescriptionRed);
            ((TabPage)tabPages[index]).Controls.Add(labelFragmentDescriptionBlue);
            ((TabPage)tabPages[index]).Controls.Add(labelPositiveSelectAll);
            ((TabPage)tabPages[index]).Controls.Add(labelPositiveDeselectAll);
            ((TabPage)tabPages[index]).Controls.Add(labelNegativeSelectAll);
            ((TabPage)tabPages[index]).Controls.Add(labelNegativeDeselectAll);
            ((TabPage)tabPages[index]).Controls.Add(labelSlashPositive);
            ((TabPage)tabPages[index]).Controls.Add(labelSlashNegative);
            ((TabPage)tabPages[index]).Controls.Add(checkedListBoxPositiveFragments);
            ((TabPage)tabPages[index]).Controls.Add(pictureBoxFragments);
            negativeIDs.Clear();
            positiveIDs.Clear();
            checkedListBoxPositiveFragments.Items.Clear();
            checkedListBoxNegativeFragments.Items.Clear();
            
            String lipidClass = ((TabPage)tabPages[index]).Text;
            ArrayList currentFragments = currentLipid.MS2Fragments[lipidClass];
            for (int i = 0; i < currentFragments.Count; ++i)
            {
                MS2Fragment currentFragment = (MS2Fragment)currentFragments[i];
                if (currentFragment.fragmentCharge > 0)
                {
                    checkedListBoxPositiveFragments.Items.Add(currentFragment.fragmentName);
                    positiveIDs.Add(i);
                    checkedListBoxPositiveFragments.SetItemChecked(checkedListBoxPositiveFragments.Items.Count - 1, currentFragment.fragmentSelected);
                }
                else 
                {
                    checkedListBoxNegativeFragments.Items.Add(currentFragment.fragmentName);
                    negativeIDs.Add(i);
                    checkedListBoxNegativeFragments.SetItemChecked(checkedListBoxNegativeFragments.Items.Count - 1, currentFragment.fragmentSelected);
                }
            }
            
            if (currentLipid.pathsToFullImage.ContainsKey(lipidClass))
            {
                fragmentComplete = Image.FromFile((String)currentLipid.pathsToFullImage[lipidClass]);
                pictureBoxFragments.Image = fragmentComplete;
            }
            else
            {
                if (fragmentComplete != null)
                {
                    fragmentComplete = null;
                }
                
                if (pictureBoxFragments.Image != null)
                {
                    pictureBoxFragments.Image.Dispose();
                    pictureBoxFragments.Image = null;
                }
            }
        }


        void CheckedListBoxPositiveItemCheck(Object sender, ItemCheckEventArgs e)
        {
            if (senderInterupt) return;
            int fragmentIndex = (int)positiveIDs[e.Index];
            String lipidClass = ((TabPage)tabPages[tabControlFragments.SelectedIndex]).Text;
            ((MS2Fragment)currentLipid.MS2Fragments[lipidClass][fragmentIndex]).fragmentSelected = (e.NewValue == CheckState.Checked);
        }
        
        void CheckedListBoxNegativeItemCheck(Object sender, ItemCheckEventArgs e)
        {
            if (senderInterupt) return;
            int fragmentIndex = (int)negativeIDs[e.Index];
            String lipidClass = ((TabPage)tabPages[tabControlFragments.SelectedIndex]).Text;
            ((MS2Fragment)currentLipid.MS2Fragments[lipidClass][fragmentIndex]).fragmentSelected = (e.NewValue == CheckState.Checked);
        }
        
        private void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void okClick(object sender, EventArgs e)
        {
            if (currentLipid is CLLipid)
            {
                creatorGUI.lipidCreatorForm.lipidTabList[0] = new CLLipid((CLLipid)currentLipid);
                creatorGUI.currentLipid = (Lipid)creatorGUI.lipidCreatorForm.lipidTabList[0];
                
            }
            else if (currentLipid is GLLipid)
            {
                creatorGUI.lipidCreatorForm.lipidTabList[1] = new GLLipid((GLLipid)currentLipid);
                creatorGUI.currentLipid = (Lipid)creatorGUI.lipidCreatorForm.lipidTabList[1];
            }
            else if (currentLipid is PLLipid)
            {
                creatorGUI.lipidCreatorForm.lipidTabList[2] = new PLLipid((PLLipid)currentLipid);
                creatorGUI.currentLipid = (Lipid)creatorGUI.lipidCreatorForm.lipidTabList[2];
            }
            else if (currentLipid is SLLipid)
            {
                creatorGUI.lipidCreatorForm.lipidTabList[3] = new SLLipid((SLLipid)currentLipid);
                creatorGUI.currentLipid = (Lipid)creatorGUI.lipidCreatorForm.lipidTabList[3];
            }
            
            this.Close();
        }
        
        private void addFragmentClick(object sender, EventArgs e)
        {
            NewFragment newPositiveFragment = new NewFragment(this);
            newPositiveFragment.Owner = this;
            newPositiveFragment.ShowInTaskbar = false;
            newPositiveFragment.ShowDialog();
            newPositiveFragment.Dispose();
            tabChange(tabControlFragments.SelectedIndex);
        }
    }
}