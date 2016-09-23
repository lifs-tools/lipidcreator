using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LipidCreator
{
    public partial class MS2Form : Form
    {
        
        public Image fragment_complete;
        public lipid currentLipid;
        public ArrayList positiveIDs;
        public ArrayList negativeIDs;

        public MS2Form(lipid currentLipid)
        {
        
            positiveIDs = new ArrayList();
            negativeIDs = new ArrayList();
            this.currentLipid = currentLipid;
            InitializeComponent(currentLipid.MS2Fragments);            
            tabChange(0);
        }

        void checkedListBox_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Image = fragment_complete;
        }

        private void checkedListBox1_MouseHover(object sender, MouseEventArgs e)
        {

            Point point = checkedListBox1.PointToClient(Cursor.Position);
            int hoveredIndex = checkedListBox1.IndexFromPoint(point);

            if (hoveredIndex != -1)
            {
                //pictureBox1.Image = Image.FromFile((String)positiveMS2fragments.fragment_files[hoveredIndex]);
            }
            else
            {
                pictureBox1.Image = fragment_complete;
            }
        }

        private void checkedListBox2_MouseHover(object sender, MouseEventArgs e)
        {

            Point point = checkedListBox2.PointToClient(Cursor.Position);
            int hoveredIndex = checkedListBox2.IndexFromPoint(point);

            if (hoveredIndex != -1)
            {
                //pictureBox1.Image = Image.FromFile((String)negativeMS2fragments.fragment_files[hoveredIndex]);
            }
            else
            {
                pictureBox1.Image = fragment_complete;
            }
        }

        public void tabIndexChanged(Object sender, EventArgs e)
        {
            tabChange(((TabControl)sender).SelectedIndex);
        }

        public void tabChange(int index)
        {
            ((TabPage)tabPages[index]).Controls.Add(dataGridView1);
            ((TabPage)tabPages[index]).Controls.Add(checkedListBox2);
            ((TabPage)tabPages[index]).Controls.Add(label2);
            ((TabPage)tabPages[index]).Controls.Add(label1);
            ((TabPage)tabPages[index]).Controls.Add(checkedListBox1);
            ((TabPage)tabPages[index]).Controls.Add(pictureBox1);
            negativeIDs.Clear();
            positiveIDs.Clear();
            checkedListBox1.Items.Clear();
            checkedListBox2.Items.Clear();
            
            ArrayList currentFragments = currentLipid.MS2Fragments[((TabPage)tabPages[index]).Text];
            for (int i = 0; i < currentFragments.Count; ++i)
            {
                if (((MS2Fragment)currentFragments[i]).fragmentCharge > 0)
                {
                    checkedListBox1.Items.Add(((MS2Fragment)currentFragments[i]).fragmentName);
                    positiveIDs.Add(i);
                    checkedListBox1.SetItemChecked(checkedListBox1.Items.Count - 1, ((MS2Fragment)currentFragments[i]).fragmentSelected);
                }
                else 
                {
                    checkedListBox2.Items.Add(((MS2Fragment)currentFragments[i]).fragmentName);
                    negativeIDs.Add(i);
                    checkedListBox2.SetItemChecked(checkedListBox2.Items.Count - 1, ((MS2Fragment)currentFragments[i]).fragmentSelected);
                }
            }
            
            /*
            if (currentLipid.paths_to_full_image.Count > index)
            {
                fragment_complete = Image.FromFile((String)currentLipid.paths_to_full_image[index]);
                pictureBox1.Image = fragment_complete;
            }*/
        }


        void CheckedListBox1_ItemCheck(Object sender, ItemCheckEventArgs e)
        {
            //positiveMS2fragments.fragment_selected[e.Index] = e.NewValue == CheckState.Checked;
        }
        void CheckedListBox2_ItemCheck(Object sender, ItemCheckEventArgs e)
        {
            //negativeMS2fragments.fragment_selected[e.Index] = e.NewValue == CheckState.Checked;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            /*
            NewFragment newPositiveFragment = new NewFragment();
            newPositiveFragment.Owner = this;
            newPositiveFragment.ShowInTaskbar = false;
            newPositiveFragment.ShowDialog();
            newPositiveFragment.Dispose();
            */
        }
    }
}