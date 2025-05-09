﻿/*
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
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
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
        public bool senderInterupt;
        Dictionary<string, ArrayList> isotopeDict;
        public CreatorGUI creatorGUI;
        [NonSerialized]
        public CheckedListBox deleteList;
        public int deleteIndex;
        
        public MediatorMS2Form(CreatorGUI creatorGUI, Mediator currentLipid)
        {
            this.currentLipid = currentLipid;
            this.creatorGUI = creatorGUI;
            
            isotopeDict = new Dictionary<string, ArrayList>();
            foreach (string lipidClass in creatorGUI.lipidCreator.categoryToClass[(int)LipidCategory.LipidMediator])
            {
                if (creatorGUI.lipidCreator.headgroups[lipidClass].attributes.Contains("heavy"))
                {
                    string[] precNames = LipidCreator.precursorNameSplit(lipidClass);
                    
                    if (!isotopeDict.ContainsKey(precNames[0])) isotopeDict.Add(precNames[0], new ArrayList());
                    isotopeDict[precNames[0]].Add(precNames[1]);
                }
            }
            senderInterupt = true;
            InitializeComponent();
            senderInterupt = false;
            
            
            List<String> medHgList = new List<String>();
            foreach (string lipidClass in creatorGUI.lipidCreator.categoryToClass[(int)LipidCategory.LipidMediator])
            {
                if (!creatorGUI.lipidCreator.headgroups[lipidClass].attributes.Contains("heavy")) medHgList.Add(lipidClass);
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
            foreach (KeyValuePair<string, MS2Fragment> currentFragment in creatorGUI.lipidCreator.allFragments[headgroup][false])
            {
                checkedListBoxMonoIsotopicFragments.Items.Add(currentFragment.Key);
                checkedListBoxMonoIsotopicFragments.SetItemChecked(checkedListBoxMonoIsotopicFragments.Items.Count - 1, currentLipid.negativeFragments[headgroup].Contains(currentFragment.Key));
            }
            
            deuteratedMediatorHeadgroups.Items.Clear();
            checkedListBoxDeuteratedFragments.Items.Clear();
            if (isotopeDict.ContainsKey(headgroup))
            {
                foreach (string deuterated in isotopeDict[headgroup])
                {
                    deuteratedMediatorHeadgroups.Items.Add(headgroup + LipidCreator.HEAVY_LABEL_OPENING_BRACKET + deuterated + LipidCreator.HEAVY_LABEL_CLOSING_BRACKET);
                }
                if (isotopeDict[headgroup].Count > 0) deuteratedMediatorHeadgroups.SelectedIndex = 0;
            }
            
            if (creatorGUI.lipidCreator.headgroups.ContainsKey(headgroup))
            {
                string mediatorFile = creatorGUI.lipidCreator.headgroups[headgroup].pathToImage;
                pictureBoxFragments.Image = Image.FromFile(mediatorFile);
                pictureBoxFragments.SendToBack();
                senderInterupt = false;
            }
        }
        
        
        void deuteratedCheckBoxValueChanged(object sender, EventArgs e)
        {
            checkedListBoxDeuteratedFragments.Items.Clear();
            string headgroup = deuteratedMediatorHeadgroups.Items[((ComboBox)sender).SelectedIndex].ToString();
            foreach (KeyValuePair<string, MS2Fragment> currentFragment in creatorGUI.lipidCreator.allFragments[headgroup][false])
            {
                checkedListBoxDeuteratedFragments.Items.Add(currentFragment.Key);
                checkedListBoxDeuteratedFragments.SetItemChecked(checkedListBoxDeuteratedFragments.Items.Count - 1, currentLipid.negativeFragments[headgroup].Contains(currentFragment.Key));
            }
        }
        
        
        void checkedListBoxMonoIsotopicValueChanged(Object sender, ItemCheckEventArgs e)
        {
            if (senderInterupt) return;
            string headgroup = medHgListbox.SelectedItem.ToString();
            string fragmentName = (string)checkedListBoxMonoIsotopicFragments.Items[e.Index];
            if (e.NewValue == CheckState.Checked)
            {
                currentLipid.negativeFragments[headgroup].Add(fragmentName);
            }
            else
            {
                currentLipid.negativeFragments[headgroup].Remove(fragmentName);
            }
        }
        
        
        void checkedListBoxDeuteratedValueChanged(Object sender, ItemCheckEventArgs e)
        {
            if (senderInterupt) return;
            if (deuteratedMediatorHeadgroups.SelectedIndex == -1) return;
            string headgroup = deuteratedMediatorHeadgroups.Items[deuteratedMediatorHeadgroups.SelectedIndex].ToString();
            string fragmentName = (string)checkedListBoxDeuteratedFragments.Items[e.Index];
            if (e.NewValue == CheckState.Checked)
            {
                currentLipid.negativeFragments[headgroup].Add(fragmentName);
            }
            else
            {
                currentLipid.negativeFragments[headgroup].Remove(fragmentName);
            }
        }
        
        
        private void checkedListBoxMonoIsotopicMouseHover(object sender, MouseEventArgs e)
        {
            string headgroup = medHgListbox.SelectedItem.ToString();
            this.checkedListBoxMonoIsotopicFragments.ContextMenu = this.contextMenuFragment;
            if (creatorGUI.lipidCreator.headgroups.ContainsKey(headgroup))
            {
                string mediatorFile = creatorGUI.lipidCreator.headgroups[headgroup].pathToImage;
                pictureBoxFragments.Image = Image.FromFile(mediatorFile);
                pictureBoxFragments.SendToBack();
                senderInterupt = false;
            }
        }
        
        
        public void contextMenuFragmentPopup(Object sender, EventArgs e)
        {
            deleteList = (CheckedListBox)((ContextMenu)sender).SourceControl;
            Point point = deleteList.PointToClient(Cursor.Position);
            deleteIndex = deleteList.IndexFromPoint(point);
            
            /*
            string lipidClass = deleteList.Name.Equals("checkedListBoxMonoIsotopicFragments") ? medHgListbox.SelectedItem.ToString() : deuteratedMediatorHeadgroups.SelectedItem.ToString();
            string fragmentName = (string)deleteList.Items[deleteIndex];
            
            menuFragmentItem1.Enabled = creatorGUI.lipidCreator.allFragments[lipidClass][false][fragmentName].userDefined;
            */
        }
        
        
        private void checkedListBoxDeuteratedeMouseHover(object sender, MouseEventArgs e)
        {
            if (deuteratedMediatorHeadgroups.SelectedIndex == -1) return;
            this.checkedListBoxDeuteratedFragments.ContextMenu = this.contextMenuFragment;
            string headgroup = deuteratedMediatorHeadgroups.Items[deuteratedMediatorHeadgroups.SelectedIndex].ToString();
            
            if (creatorGUI.lipidCreator.headgroups.ContainsKey(headgroup))
            {
                string mediatorFile = creatorGUI.lipidCreator.headgroups[headgroup].pathToImage;
                pictureBoxFragments.Image = Image.FromFile(mediatorFile);
                pictureBoxFragments.SendToBack();
                senderInterupt = false;
            }
        }
        
        
        public void deleteFragment(Object sender, EventArgs e)
        {
            /*
            string lipidClass = deleteList.Name.Equals("checkedListBoxMonoIsotopicFragments") ? medHgListbox.SelectedItem.ToString() : deuteratedMediatorHeadgroups.SelectedItem.ToString();
            string fragmentName = (string)deleteList.Items[deleteIndex];
            bool userDefined = creatorGUI.lipidCreator.allFragments[lipidClass][false][fragmentName].userDefined;
            if (userDefined){
                if (isMonoisotopic){
                    currentLipid.positiveFragments[lipidClass].Remove(fragmentName);
                    checkedListBoxPositiveFragments.Items.RemoveAt(editDeleteIndex);
                }
                else {
                    currentLipid.negativeFragments[lipidClass].Remove(fragmentName);
                    checkedListBoxNegativeFragments.Items.RemoveAt(editDeleteIndex);
                }
                creatorGUI.lipidCreator.allFragments[lipidClass][false].Remove(fragmentName);
            }
            deleteList.Refresh();
            */
        }
        
        
        
        void checkedListBoxMonoisotopicSelectAll(object sender, EventArgs e)
        {
            selectUnselect(checkedListBoxMonoIsotopicFragments, medHgListbox.SelectedItem.ToString(), true);
        }
        
        
        void checkedListBoxMonoisotopicDeselectAll(object sender, EventArgs e)
        {
            selectUnselect(checkedListBoxMonoIsotopicFragments, medHgListbox.SelectedItem.ToString(), false);
        }
        
        
        void checkedListBoxDeuteratedSelectAll(object sender, EventArgs e)
        {
            if (deuteratedMediatorHeadgroups.SelectedIndex == -1) return;
            selectUnselect(checkedListBoxDeuteratedFragments, deuteratedMediatorHeadgroups.Items[deuteratedMediatorHeadgroups.SelectedIndex].ToString(), true);
        }
        
        
        void checkedListBoxDeuteratedDeselectAll(object sender, EventArgs e)
        {
            if (deuteratedMediatorHeadgroups.SelectedIndex == -1) return;
            selectUnselect(checkedListBoxDeuteratedFragments, deuteratedMediatorHeadgroups.Items[deuteratedMediatorHeadgroups.SelectedIndex].ToString(), false);
        }
        
        
        void selectUnselect(CheckedListBox clb, string headgroup, bool select)
        {
            senderInterupt = true;
            currentLipid.negativeFragments[headgroup].Clear();
            if (select)
            {
                foreach (KeyValuePair<string, MS2Fragment> ms2fragment in creatorGUI.lipidCreator.allFragments[headgroup][false])
                {
                    currentLipid.negativeFragments[headgroup].Add(ms2fragment.Key);
                }
            }
            for (int i = 0; i < clb.Items.Count; ++i)
            {
                clb.SetItemChecked(i, select);
            }
            senderInterupt = false;
        }
        
        
        private void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        
        private void okClick(object sender, EventArgs e)
        {
            creatorGUI.lipidTabList[(int)LipidCategory.LipidMediator] = new Mediator((Mediator)currentLipid);
            creatorGUI.currentLipid = (Lipid)creatorGUI.lipidTabList[(int)LipidCategory.LipidMediator];            
            this.Close();
        }
        
        
        private void addFragmentClick(object sender, EventArgs e)
        {
        
            string headgroup = medHgListbox.SelectedItem.ToString();
            NewMediatorFragment newMediatorFragment = new NewMediatorFragment(this, headgroup);
            newMediatorFragment.Owner = this;
            newMediatorFragment.ShowInTaskbar = false;
            newMediatorFragment.ShowDialog();
            newMediatorFragment.Dispose();
        }
    }
}