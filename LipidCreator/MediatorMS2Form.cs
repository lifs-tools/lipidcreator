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
    public partial class MediatorMS2Form : Form
    {
        
        public Image fragmentComplete = null;
        public Mediator currentLipid;
        public ArrayList positiveIDs;
        public ArrayList negativeIDs;
        public CreatorGUI creatorGUI;
        public bool senderInterupt;
        Dictionary<string, ArrayList> isotopeDict;
        
        public MediatorMS2Form(CreatorGUI creatorGUI, Mediator currentLipid)
        {
            this.currentLipid = currentLipid;
            this.creatorGUI = creatorGUI;
            
            isotopeDict = new Dictionary<string, ArrayList>();
            foreach (KeyValuePair<string, ArrayList> ms2fragment in this.currentLipid.MS2Fragments)
            {
                if (ms2fragment.Key.IndexOf("/") > -1)
                {
                    string monoName = ms2fragment.Key.Split(new char[]{'/'})[0];
                    string deuterium = ms2fragment.Key.Split(new char[]{'/'})[1];
                    
                    if (!isotopeDict.ContainsKey(monoName)) isotopeDict.Add(monoName, new ArrayList());
                    isotopeDict[monoName].Add(deuterium);
                }
            }
            senderInterupt = true;
            InitializeComponent();
            senderInterupt = false;
            
            
            List<String> medHgList = new List<String>();
            foreach(KeyValuePair<String, ArrayList> fragmentList in creatorGUI.lipidCreatorForm.allFragments["Mediator"])
            {
                String headgroup = fragmentList.Key;
                if (headgroup.IndexOf("/") == -1) medHgList.Add(headgroup);
            }
            medHgList.Sort();
            medHgListbox.Items.AddRange(medHgList.ToArray());
            if (medHgList.Count > 0) medHgListbox.SetSelected(0, true);
            
        }
        
        void medHGListboxSelectedValueChanged(object sender, EventArgs e)
        {
            senderInterupt = true;
            string headgroup = ((ListBox)sender).SelectedItem.ToString();
            checkedListBoxMonoIsotopicFragments.Items.Clear();
            foreach (MS2Fragment currentFragment in currentLipid.MS2Fragments[headgroup])
            {
                checkedListBoxMonoIsotopicFragments.Items.Add(currentFragment.fragmentName);
                checkedListBoxMonoIsotopicFragments.SetItemChecked(checkedListBoxMonoIsotopicFragments.Items.Count - 1, currentFragment.fragmentSelected);
            }
            
            deuteratedMediatorHeadgroups.Items.Clear();
            checkedListBoxDeuteratedFragments.Items.Clear();
            if (isotopeDict.ContainsKey(headgroup))
            {
                foreach (string deuterated in isotopeDict[headgroup])
                {
                    deuteratedMediatorHeadgroups.Items.Add(headgroup + "/" + deuterated);
                }
                if (isotopeDict[headgroup].Count > 0) deuteratedMediatorHeadgroups.SelectedIndex = 0;
            }
            
            if (creatorGUI.lipidCreatorForm.allPathsToPrecursorImages.ContainsKey(headgroup))
            {
                string mediatorFile = creatorGUI.lipidCreatorForm.allPathsToPrecursorImages[headgroup];
                pictureBoxFragments.Image = Image.FromFile(mediatorFile);
                pictureBoxFragments.SendToBack();
                senderInterupt = false;
            }
        }
        
        
        void deuteratedCheckBoxValueChanged(object sender, EventArgs e)
        {
            string headgroup = deuteratedMediatorHeadgroups.Items[((ComboBox)sender).SelectedIndex].ToString();
            foreach (MS2Fragment currentFragment in currentLipid.MS2Fragments[headgroup])
            {
                checkedListBoxDeuteratedFragments.Items.Add(currentFragment.fragmentName);
                checkedListBoxDeuteratedFragments.SetItemChecked(checkedListBoxDeuteratedFragments.Items.Count - 1, currentFragment.fragmentSelected);
            }
        }
        
        
        void checkedListBoxMonoIsotopicValueChanged(Object sender, ItemCheckEventArgs e)
        {
            if (senderInterupt) return;
            string headgroup = medHgListbox.SelectedItem.ToString();
            ((MS2Fragment)currentLipid.MS2Fragments[headgroup][e.Index]).fragmentSelected = (e.NewValue == CheckState.Checked);
        }
        
        
        void checkedListBoxDeuteratedValueChanged(Object sender, ItemCheckEventArgs e)
        {
            if (senderInterupt) return;
            if (deuteratedMediatorHeadgroups.SelectedIndex == -1) return;
            string headgroup = deuteratedMediatorHeadgroups.Items[deuteratedMediatorHeadgroups.SelectedIndex].ToString();
            ((MS2Fragment)currentLipid.MS2Fragments[headgroup][e.Index]).fragmentSelected = (e.NewValue == CheckState.Checked);
        }
        
        
        private void checkedListBoxMonoIsotopicMouseHover(object sender, MouseEventArgs e)
        {
            string headgroup = medHgListbox.SelectedItem.ToString();
            if (creatorGUI.lipidCreatorForm.allPathsToPrecursorImages.ContainsKey(headgroup))
            {
                string mediatorFile = creatorGUI.lipidCreatorForm.allPathsToPrecursorImages[headgroup];
                pictureBoxFragments.Image = Image.FromFile(mediatorFile);
                pictureBoxFragments.SendToBack();
                senderInterupt = false;
            }
        }
        
        
        private void checkedListBoxDeuteratedeMouseHover(object sender, MouseEventArgs e)
        {
            if (deuteratedMediatorHeadgroups.SelectedIndex == -1) return;
            string headgroup = deuteratedMediatorHeadgroups.Items[deuteratedMediatorHeadgroups.SelectedIndex].ToString();
            
            if (creatorGUI.lipidCreatorForm.allPathsToPrecursorImages.ContainsKey(headgroup))
            {
                string mediatorFile = creatorGUI.lipidCreatorForm.allPathsToPrecursorImages[headgroup];
                pictureBoxFragments.Image = Image.FromFile(mediatorFile);
                pictureBoxFragments.SendToBack();
                senderInterupt = false;
            }
        }
        
        
        void checkedListBoxMonoisotopicSelectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            string headgroup = medHgListbox.SelectedItem.ToString();
            ArrayList currentFragments = currentLipid.MS2Fragments[headgroup];
            for (int i = 0; i < currentFragments.Count; ++i)
            {
                ((MS2Fragment)currentFragments[i]).fragmentSelected = true;  
            }
            for (int i = 0; i < checkedListBoxMonoIsotopicFragments.Items.Count; ++i)
            {
                checkedListBoxMonoIsotopicFragments.SetItemChecked(i, true);
            }
            senderInterupt = false;
        }
        
        
        void checkedListBoxMonoisotopicDeselectAll(object sender, EventArgs e)
        {
            if (deuteratedMediatorHeadgroups.SelectedIndex == -1) return;
            senderInterupt = true;
            string headgroup = medHgListbox.SelectedItem.ToString();
            ArrayList currentFragments = currentLipid.MS2Fragments[headgroup];
            for (int i = 0; i < currentFragments.Count; ++i)
            {
                ((MS2Fragment)currentFragments[i]).fragmentSelected = false;  
            }
            for (int i = 0; i < checkedListBoxMonoIsotopicFragments.Items.Count; ++i)
            {
                checkedListBoxMonoIsotopicFragments.SetItemChecked(i, false);
            }
            senderInterupt = false;
        }
        
        
        void checkedListBoxDeuteratedSelectAll(object sender, EventArgs e)
        {
            if (deuteratedMediatorHeadgroups.SelectedIndex == -1) return;
            senderInterupt = true;
            string headgroup = deuteratedMediatorHeadgroups.Items[deuteratedMediatorHeadgroups.SelectedIndex].ToString();
            ArrayList currentFragments = currentLipid.MS2Fragments[headgroup];
            for (int i = 0; i < currentFragments.Count; ++i)
            {
                ((MS2Fragment)currentFragments[i]).fragmentSelected = true;  
            }
            for (int i = 0; i < checkedListBoxDeuteratedFragments.Items.Count; ++i)
            {
                checkedListBoxDeuteratedFragments.SetItemChecked(i, true);
            }
            senderInterupt = false;
        }
        
        
        void checkedListBoxDeuteratedDeselectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            string headgroup = deuteratedMediatorHeadgroups.Items[deuteratedMediatorHeadgroups.SelectedIndex].ToString();
            ArrayList currentFragments = currentLipid.MS2Fragments[headgroup];
            for (int i = 0; i < currentFragments.Count; ++i)
            {
                ((MS2Fragment)currentFragments[i]).fragmentSelected = false;  
            }
            for (int i = 0; i < checkedListBoxDeuteratedFragments.Items.Count; ++i)
            {
                checkedListBoxDeuteratedFragments.SetItemChecked(i, false);
            }
            senderInterupt = false;
        }
        
        
        private void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        
        private void okClick(object sender, EventArgs e)
        {
            creatorGUI.lipidCreatorForm.lipidTabList[(int)LipidCategory.Mediator] = new Mediator((Mediator)currentLipid);
            creatorGUI.currentLipid = (Lipid)creatorGUI.lipidCreatorForm.lipidTabList[(int)LipidCategory.Mediator];            
            this.Close();
        }
        
    
        /*
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
        
        private void addFragmentClick(object sender, EventArgs e)
        {
            NewFragment newPositiveFragment = new NewFragment(this);
            newPositiveFragment.Owner = this;
            newPositiveFragment.ShowInTaskbar = false;
            newPositiveFragment.ShowDialog();
            newPositiveFragment.Dispose();
            tabChange(tabControlFragments.SelectedIndex);
        }
        
        */
    }
}