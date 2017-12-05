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
        public CreatorGUI creatorGUI;
        public bool senderInterupt;
        public bool loading;
        
        public MS2Form(CreatorGUI creatorGUI, Lipid currentLipid)
        {
            this.creatorGUI = creatorGUI;
            senderInterupt = false;
            loading = false;
            
            
            if (currentLipid is GLLipid){
                this.currentLipid = new GLLipid((GLLipid)currentLipid);
            }
            else if (currentLipid is PLLipid)
            {
                this.currentLipid = new PLLipid((PLLipid)currentLipid);
            }
            else if (currentLipid is SLLipid)
            {
                this.currentLipid = new SLLipid((SLLipid)currentLipid);
            }
            else if (currentLipid is Cholesterol)
            {
                this.currentLipid = new Cholesterol((Cholesterol)currentLipid);
            }
            
            InitializeComponent();
            
            foreach (string lipidClass in creatorGUI.lipidCreator.categoryToClass[(int)creatorGUI.currentIndex])
            {
                if (!creatorGUI.lipidCreator.headgroups[lipidClass].heavyLabeled)
                {
                    TabPage tp = new TabPage();
                    tp.Location = new System.Drawing.Point(4, 22);
                    tp.Name = lipidClass;
                    tp.Padding = new System.Windows.Forms.Padding(3);
                    tp.Size = new System.Drawing.Size(766, 392);
                    tp.TabIndex = 0;
                    tp.Text = lipidClass;
                    tp.UseVisualStyleBackColor = true;
                    this.tabControlFragments.Controls.Add(tp);
                    this.tabPages.Add(tp);
                }
            }
            if (tabPages.Count > 16) {
                tabControlFragments.Multiline = true;
                tabControlFragments.ItemSize = new Size((tabControlFragments.Width - 3) / 16, 20);
                tabControlFragments.SizeMode = TabSizeMode.Fixed;
            }
            
            tabChange(0);
        }
        
        public string getHeadgroup()
        {
            if (isotopeList.SelectedIndex == 0) return ((TabPage)tabPages[tabControlFragments.SelectedIndex]).Name;
            return ((string)isotopeList.Items[isotopeList.SelectedIndex]).Replace(Lipid.HEAVY_LABEL_SEPARATOR, "/");
        }

        void checkedListBoxMouseLeave(object sender, EventArgs e)
        {
            pictureBoxFragments.Image = fragmentComplete;
        }
        
        void checkedListBoxPositiveSelectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            currentLipid.positiveFragments.Clear();
            for (int i = 0; i < checkedListBoxPositiveFragments.Items.Count; ++i)
            {
                currentLipid.positiveFragments[getHeadgroup()].Add((string)checkedListBoxPositiveFragments.Items[i]);
                checkedListBoxPositiveFragments.SetItemChecked(i, true);
            }
            senderInterupt = false;
        }
        
        void checkedListBoxPositiveDeselectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            currentLipid.positiveFragments[getHeadgroup()].Clear();
            for (int i = 0; i < checkedListBoxPositiveFragments.Items.Count; ++i)
            {
                checkedListBoxPositiveFragments.SetItemChecked(i, false);
            }
            senderInterupt = false;
        }
        
        void checkedListBoxNegativeSelectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            currentLipid.negativeFragments[getHeadgroup()].Clear();
            for (int i = 0; i < checkedListBoxNegativeFragments.Items.Count; ++i)
            {
                currentLipid.negativeFragments[getHeadgroup()].Add((string)checkedListBoxNegativeFragments.Items[i]);
                checkedListBoxNegativeFragments.SetItemChecked(i, true);
            }
            senderInterupt = false;
        }
        
        void checkedListBoxNegativeDeselectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            currentLipid.negativeFragments[getHeadgroup()].Clear();
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
                string lipidClass = getHeadgroup();
                string fragmentName = (string)checkedListBoxPositiveFragments.Items[hoveredIndex];
                MS2Fragment fragment = creatorGUI.lipidCreator.allFragments[lipidClass][true][fragmentName];
                if (fragment.fragmentFile != null) pictureBoxFragments.Image = Image.FromFile(fragment.fragmentFile);
                
                // create tool tip           
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
                
                foreach (KeyValuePair<int, int> row in fragment.fragmentElements)
                {
                    if (row.Value != 0)
                    {
                        chemForm += MS2Fragment.ELEMENT_SHORTCUTS[row.Key] + Convert.ToString(Math.Abs(row.Value));
                        chemAdding = row.Value > 0;
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
                String lipidClass = getHeadgroup();
                string fragmentName = (string)checkedListBoxNegativeFragments.Items[hoveredIndex];
                MS2Fragment fragment = creatorGUI.lipidCreator.allFragments[lipidClass][false][fragmentName];
                if (fragment.fragmentFile != null) pictureBoxFragments.Image = Image.FromFile(fragment.fragmentFile);
                
                // create tool tip              
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
                
                foreach (KeyValuePair<int, int> row in fragment.fragmentElements)
                {
                    if (row.Value != 0)
                    {
                        chemForm += MS2Fragment.ELEMENT_SHORTCUTS[row.Key] + Convert.ToString(Math.Abs(row.Value));
                        chemAdding = row.Value > 0;
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
        
        
        public void isotopeListComboBoxValueChanged(object sender, EventArgs e)
        {
            if (loading) return;
            String lipidClass = getHeadgroup();
            
            checkedListBoxPositiveFragments.Items.Clear();
            checkedListBoxNegativeFragments.Items.Clear();
            
            foreach (KeyValuePair<string, MS2Fragment> currentFragment in creatorGUI.lipidCreator.allFragments[lipidClass][true])
            {
                checkedListBoxPositiveFragments.Items.Add(currentFragment.Value.fragmentName);
                checkedListBoxPositiveFragments.SetItemChecked(checkedListBoxPositiveFragments.Items.Count - 1, currentLipid.positiveFragments[lipidClass].Contains(currentFragment.Value.fragmentName));
            }
            foreach (KeyValuePair<string, MS2Fragment> currentFragment in creatorGUI.lipidCreator.allFragments[lipidClass][false])
            {
                checkedListBoxNegativeFragments.Items.Add(currentFragment.Value.fragmentName);
                checkedListBoxNegativeFragments.SetItemChecked(checkedListBoxNegativeFragments.Items.Count - 1, currentLipid.negativeFragments[lipidClass].Contains(currentFragment.Value.fragmentName));
            }
            
            if (creatorGUI.lipidCreator.headgroups.ContainsKey(lipidClass) && creatorGUI.lipidCreator.headgroups[lipidClass].pathToImage.Length > 0)
            {
                fragmentComplete = Image.FromFile(creatorGUI.lipidCreator.headgroups[lipidClass].pathToImage);
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
        

        public void tabChange(int index)
        {
            loading = true;
            isotopeList.Items.Clear();
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
            ((TabPage)tabPages[index]).Controls.Add(isotopeList);
            
            isotopeList.Items.Add("Monoisotopic");
            String lipidClass = ((TabPage)tabPages[tabControlFragments.SelectedIndex]).Name;
            foreach(Precursor heavyPrecursor in creatorGUI.lipidCreator.headgroups[lipidClass].heavyLabeledPrecursors)
            {
                isotopeList.Items.Add(heavyPrecursor.name.Replace("/", Lipid.HEAVY_LABEL_SEPARATOR));
            }
            
            loading = false;
            isotopeList.SelectedIndex = 0;
        }


        public void CheckedListBoxPositiveItemCheck(Object sender, ItemCheckEventArgs e)
        {
            if (senderInterupt) return;
            if (e.NewValue == CheckState.Checked)
            {
                currentLipid.positiveFragments[getHeadgroup()].Add((string)checkedListBoxPositiveFragments.Items[e.Index]);
            }
            else
            {
                currentLipid.positiveFragments[getHeadgroup()].Remove((string)checkedListBoxPositiveFragments.Items[e.Index]);
            }
        }
        
        
        public void CheckedListBoxNegativeItemCheck(Object sender, ItemCheckEventArgs e)
        {
            if (senderInterupt) return;
            if (e.NewValue == CheckState.Checked)
            {
                currentLipid.negativeFragments[getHeadgroup()].Add((string)checkedListBoxNegativeFragments.Items[e.Index]);
            }
            else
            {
                currentLipid.negativeFragments[getHeadgroup()].Remove((string)checkedListBoxNegativeFragments.Items[e.Index]);
            }
        }
        
        
        private void List_RightClick(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                int index = checkedListBoxNegativeFragments.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    Console.WriteLine(index);
                }
            }

        }
        
        private void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void okClick(object sender, EventArgs e)
        {
            if (currentLipid is GLLipid)
            {
                creatorGUI.lipidTabList[(int)LipidCategory.GlyceroLipid] = new GLLipid((GLLipid)currentLipid);
                creatorGUI.currentLipid = (Lipid)creatorGUI.lipidTabList[(int)LipidCategory.GlyceroLipid];
            }
            else if (currentLipid is PLLipid)
            {
                creatorGUI.lipidTabList[(int)LipidCategory.PhosphoLipid] = new PLLipid((PLLipid)currentLipid);
                creatorGUI.currentLipid = (Lipid)creatorGUI.lipidTabList[(int)LipidCategory.PhosphoLipid];
            }
            else if (currentLipid is SLLipid)
            {
                creatorGUI.lipidTabList[(int)LipidCategory.SphingoLipid] = new SLLipid((SLLipid)currentLipid);
                creatorGUI.currentLipid = (Lipid)creatorGUI.lipidTabList[(int)LipidCategory.SphingoLipid];
            }
            else if (currentLipid is Cholesterol)
            {
                creatorGUI.lipidTabList[(int)LipidCategory.Cholesterol] = new Cholesterol((Cholesterol)currentLipid);
                creatorGUI.currentLipid = (Lipid)creatorGUI.lipidTabList[(int)LipidCategory.Cholesterol];
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