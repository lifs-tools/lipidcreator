using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LipidCreator
{
    public partial class MS2Form : Form
    {
        
        public Image fragment_complete;
        public lipid currentLipid;

        public MS2Form(lipid currentLipid)
        {
            this.currentLipid = currentLipid;
            InitializeComponent(currentLipid.MS2Fragments);
            tabChange(0);
            /*
            for (int i = 0; i < currentLipid.MS2Fragments.Count; ++i)
            {
                if (((MS2Fragment)currentLipid.MS2Fragments[i]).fragmentCharge > 0)
                {
                    checkedListBox1.Items.Add(((MS2Fragment)currentLipid.MS2Fragments[i]).fragmentName);
                    checkedListBox1.SetItemChecked(checkedListBox1.Items.Count - 1, ((MS2Fragment)currentLipid.MS2Fragments[i]).fragmentSelected);
                }
                else 
                {
                    checkedListBox2.Items.Add(((MS2Fragment)currentLipid.MS2Fragments[i]).fragmentName);
                    checkedListBox2.SetItemChecked(checkedListBox2.Items.Count - 1, ((MS2Fragment)currentLipid.MS2Fragments[i]).fragmentSelected);
                }
            }
            fragment_complete = Image.FromFile(currentLipid.path_to_full_image);
            pictureBox1.Image = fragment_complete;*/
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