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
        public bool senderInterupt;
        Dictionary<string, ArrayList> isotopeDict;
        public CreatorGUI creatorGUI;
        
        public MediatorMS2Form(CreatorGUI creatorGUI, Mediator currentLipid)
        {
            this.currentLipid = currentLipid;
            this.creatorGUI = creatorGUI;
            
            isotopeDict = new Dictionary<string, ArrayList>();
            foreach (string lipidClass in creatorGUI.lipidCreator.categoryToClass[(int)LipidCategory.Mediator])
            {
                if (creatorGUI.lipidCreator.headgroups[lipidClass].attributes.Contains("heavy"))
                {
                    string monoName = lipidClass.Split(new char[]{'/'})[0];
                    string deuterium = lipidClass.Split(new char[]{'/'})[1];
                    
                    if (!isotopeDict.ContainsKey(monoName)) isotopeDict.Add(monoName, new ArrayList());
                    isotopeDict[monoName].Add(deuterium);
                }
            }
            senderInterupt = true;
            InitializeComponent();
            senderInterupt = false;
            
            
            List<String> medHgList = new List<String>();
            foreach (string lipidClass in creatorGUI.lipidCreator.categoryToClass[(int)LipidCategory.Mediator])
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
                    deuteratedMediatorHeadgroups.Items.Add(headgroup + "/" + deuterated);
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
            if (creatorGUI.lipidCreator.headgroups.ContainsKey(headgroup))
            {
                string mediatorFile = creatorGUI.lipidCreator.headgroups[headgroup].pathToImage;
                pictureBoxFragments.Image = Image.FromFile(mediatorFile);
                pictureBoxFragments.SendToBack();
                senderInterupt = false;
            }
        }
        
        
        private void checkedListBoxDeuteratedeMouseHover(object sender, MouseEventArgs e)
        {
            if (deuteratedMediatorHeadgroups.SelectedIndex == -1) return;
            string headgroup = deuteratedMediatorHeadgroups.Items[deuteratedMediatorHeadgroups.SelectedIndex].ToString();
            
            if (creatorGUI.lipidCreator.headgroups.ContainsKey(headgroup))
            {
                string mediatorFile = creatorGUI.lipidCreator.headgroups[headgroup].pathToImage;
                pictureBoxFragments.Image = Image.FromFile(mediatorFile);
                pictureBoxFragments.SendToBack();
                senderInterupt = false;
            }
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
            creatorGUI.lipidTabList[(int)LipidCategory.Mediator] = new Mediator((Mediator)currentLipid);
            creatorGUI.currentLipid = (Lipid)creatorGUI.lipidTabList[(int)LipidCategory.Mediator];            
            this.Close();
        }
        
        /*
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