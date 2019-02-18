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
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace LipidCreator
{
    
    [Serializable]
    public partial class TestTutorials
    {
    
        [DllImport("user32.dll",CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        
        
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const int MAX_RETRIES = 3;
        public const int STEP_SLEEP = 500;
        
        
        
        public void DoMouseClick()
        {
            //Call the imported function with the cursor's current position
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }
        
        
        
        public void DoMouseRightClick()
        {
            //Call the imported function with the cursor's current position
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
        }
        
        
        
        
        
        public CreatorGUI creatorGUI;
        
        public Point getMiddle(Control control)
        {
            Point controlCenter = new Point(control.Width >> 1, control.Height >> 1);
            return control.PointToScreen(controlCenter);
        }
        
        
        
        
        
        public Point getOrigin(Control control)
        {
            return getOrigin(control, new Point(1, 1));
        }
        
        
        
        
        
        public Point getOrigin(Control control, Point controlLeft)
        {
            return control.PointToScreen(controlLeft);
        }
        
        
        
        
        
        
        public void firstTutorialTest()
        {
            try 
            {
                bool passed = false;
                Tutorial tutorial = creatorGUI.tutorial;
                TutorialWindow tutorialWindow = tutorial.tutorialWindow;
                int retries = 0;
                int previousStep = 0;
                
                
                while (!passed && tutorial.inTutorial)
                {
                    Thread.Sleep(STEP_SLEEP);
                    
                    if (previousStep != tutorial.tutorialStep)
                    {
                        retries = 0;
                        previousStep = tutorial.tutorialStep;
                    }
                    else
                    {
                        retries += 1;
                    }
                    
                    if (retries >= MAX_RETRIES)
                    {
                        throw new Exception("Tutorial doesn't react at step " + tutorial.tutorialStep.ToString());
                    }
                    
                    switch (tutorial.tutorialStep)
                    {
                        case (int)PRMSteps.Welcome:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)PRMSteps.PhosphoTab:
                            Point p2 = getOrigin(creatorGUI.tabControl);
                            p2.X += (int)(creatorGUI.tabControl.ItemSize.Width * 2.5);
                            p2.Y += creatorGUI.tabControl.ItemSize.Height >> 1;
                            Cursor.Position = p2;
                            DoMouseClick();
                            break;
                            
                            
                        case (int)PRMSteps.PGheadgroup:
                            int pg_I = 0;
                            
                            ListBox plHG = creatorGUI.plHgListbox;
                            for (; pg_I < plHG.Items.Count; ++pg_I) if (plHG.Items[pg_I].ToString().Equals("PG")) break;
                            
                            Point p3 = getOrigin(plHG);
                            p3.X += plHG.Size.Width >> 1;
                            p3.Y += (int)((pg_I + 0.5) * plHG.ItemHeight);
                            Cursor.Position = p3;
                            DoMouseClick();
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)PRMSteps.SetFA:
                            Cursor.Position = getMiddle(creatorGUI.plFA1Textbox);
                            DoMouseClick();
                            for (int i = 0; i < 20; ++i) SendKeys.SendWait("{BACKSPACE}");
                            for (int i = 0; i < 20; ++i) SendKeys.SendWait("{DEL}");
                            SendKeys.SendWait("{1}");
                            SendKeys.SendWait("{4}");
                            SendKeys.SendWait("{-}");
                            SendKeys.SendWait("{1}");
                            SendKeys.SendWait("{8}");
                            SendKeys.SendWait("{,}");
                            SendKeys.SendWait("{2}");
                            SendKeys.SendWait("{0}");
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.SetDB:
                            Cursor.Position = getMiddle(creatorGUI.plDB1Textbox);
                            DoMouseClick();
                            for (int i = 0; i < 10; ++i) SendKeys.SendWait("{BACKSPACE}");
                            for (int i = 0; i < 10; ++i) SendKeys.SendWait("{DEL}");
                            SendKeys.SendWait("{0}");
                            SendKeys.SendWait("{-}");
                            SendKeys.SendWait("{1}");
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.MoreParameters:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.RepresentitativeFA:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.Ether:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.SecondFADB:
                            Cursor.Position = getMiddle(creatorGUI.plFA2Textbox);
                            DoMouseClick();
                            for (int i = 0; i < 20; ++i) SendKeys.SendWait("{BACKSPACE}");
                            for (int i = 0; i < 20; ++i) SendKeys.SendWait("{DEL}");
                            SendKeys.SendWait("{8}");
                            SendKeys.SendWait("{-}");
                            SendKeys.SendWait("{1}");
                            SendKeys.SendWait("{0}");
                            Cursor.Position = getMiddle(creatorGUI.plDB2Textbox);
                            DoMouseClick();
                            for (int i = 0; i < 10; ++i) SendKeys.SendWait("{BACKSPACE}");
                            for (int i = 0; i < 10; ++i) SendKeys.SendWait("{DEL}");
                            SendKeys.SendWait("{2}");
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.SelectAdduct:
                            Cursor.Position = getMiddle(creatorGUI.plPosAdductCheckbox1);
                            DoMouseClick();
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.OpenFilter:
                            Cursor.Position = getMiddle(creatorGUI.filtersButton);
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.SelectFilter:
                            Cursor.Position = getMiddle(creatorGUI.filterDialog.radioButton2);
                            DoMouseClick();
                            Cursor.Position = getMiddle(creatorGUI.filterDialog.button2);
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.AddLipid:
                            Cursor.Position = getMiddle(creatorGUI.addLipidButton);
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.OpenInterlist:
                            Cursor.Position = getMiddle(creatorGUI.openReviewFormButton);
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.ExplainInterlist:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.OpenReview:
                            Cursor.Position = getMiddle(creatorGUI.lipidsInterList.continueReviewButton);
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            break;
                        
                        case (int)PRMSteps.StoreList:
                            Cursor.Position = getMiddle(creatorGUI.lipidsReview.buttonStoreTransitionList);
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            SendKeys.SendWait("{ESC}");
                            break;
                        
                        case (int)PRMSteps.Finish:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            passed = true;
                            break;
                            
                        case (int)PRMSteps.Null:
                            break;
                    
                        default:
                            passed = true;
                            break;
                    }
                    creatorGUI.Refresh();
                }
                if (passed)
                {
                    MessageBox.Show("First test passed without any problem.");
                }
                else 
                {
                    MessageBox.Show("First test interrupted");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        
        
        
        
        
        
        public void secondTutorialTest()
        {
            try 
            {
                bool passed = false;
                Tutorial tutorial = creatorGUI.tutorial;
                TutorialWindow tutorialWindow = tutorial.tutorialWindow;
                int retries = 0;
                int previousStep = 0;
                
                
                while (!passed && tutorial.inTutorial)
                {
                    Thread.Sleep(STEP_SLEEP);
                    
                    if (previousStep != tutorial.tutorialStep)
                    {
                        retries = 0;
                        previousStep = tutorial.tutorialStep;
                    }
                    else
                    {
                        retries += 1;
                    }
                    
                    if (retries >= MAX_RETRIES)
                    {
                        throw new Exception("Tutorial doesn't react at step " + tutorial.tutorialStep.ToString());
                    }
                    
                    switch (tutorial.tutorialStep)
                    {
                            
                        case (int)SRMSteps.Null:
                            break;
                            
                        case (int)SRMSteps.Welcome:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.PhosphoTab:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.OpenMS2:
                            Cursor.Position = getMiddle(creatorGUI.MS2fragmentsLipidButton);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.InMS2:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.SelectPG:
                            Point p2 = getOrigin(creatorGUI.ms2fragmentsForm.tabControlFragments);
                            p2.X += (int)(creatorGUI.ms2fragmentsForm.tabControlFragments.ItemSize.Width * ((creatorGUI.tutorial.pgIndex % 16) + 0.5));
                            p2.Y += creatorGUI.tabControl.ItemSize.Height >> 2;
                            Cursor.Position = p2;
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.SelectFragments:
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.labelNegativeDeselectAll);
                            DoMouseClick();
                            int hgt = creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.ItemHeight;
                            int i = 0;
                            foreach (string itemName in creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.Items)
                            {
                                if (itemName == "FA1(+O)" || itemName == "HG(PG,171)")
                                {
                                    Point p3 = getOrigin(creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments);
                                    p3.X += creatorGUI.ms2fragmentsForm.tabControlFragments.ItemSize.Width >> 1;
                                    p3.Y += (int)(hgt * (i + 0.5));
                                    Cursor.Position = p3;
                                    DoMouseClick();
                                }
                                ++i;
                            }
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.AddFragment:
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.buttonAddFragment);
                            DoMouseClick();
                        
                            break;
                            
                        case (int)SRMSteps.InFragment:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.NameFragment:
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName);
                            DoMouseClick();
                            for (int i2 = 0; i2 < 10; ++i2) SendKeys.SendWait("{BACKSPACE}");
                            for (int i2 = 0; i2 < 10; ++i2) SendKeys.SendWait("{DEL}");
                            SendKeys.SendWait("{t}");
                            SendKeys.SendWait("{e}");
                            SendKeys.SendWait("{s}");
                            SendKeys.SendWait("{t}");
                            SendKeys.SendWait("{F}");
                            SendKeys.SendWait("{r}");
                            SendKeys.SendWait("{a}");
                            SendKeys.SendWait("{g}");
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox);
                            DoMouseClick();
                            Thread.Sleep(200);
                            Point pp = Cursor.Position;
                            pp.Y += (int)(creatorGUI.ms2fragmentsForm.newFragment.selectBaseCombobox.Height * 1.5);
                            Cursor.Position = pp;
                            Thread.Sleep(200);
                            DoMouseClick();
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.SetCharge:
                            Point p4 = getOrigin(creatorGUI.ms2fragmentsForm.newFragment.numericUpDownCharge);
                            p4.X += (int)(creatorGUI.ms2fragmentsForm.newFragment.numericUpDownCharge.Width * 0.75);
                            p4.X += creatorGUI.ms2fragmentsForm.newFragment.numericUpDownCharge.Height >> 2;
                            Cursor.Position = p4;
                            Thread.Sleep(200);
                            DoMouseClick();
                            DoMouseClick();
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.SetElements:
                            DataGridView dgv = creatorGUI.ms2fragmentsForm.newFragment.dataGridViewElements;
                            int x1 = dgv.Columns[0].Width + (dgv.Columns[1].Width >> 1);
                            int y1 = dgv.ColumnHeadersHeight;
                            foreach (DataGridViewRow dgvRow in dgv.Rows)
                            {
                                
                                if ((string)dgvRow.Cells[0].Value == "H")
                                {   
                                    Cursor.Position = getOrigin(dgv, new Point(x1, y1 + (dgvRow.Height >> 1)));
                                    DoMouseClick();
                                    for (int i2 = 0; i2 < 2; ++i2) SendKeys.SendWait("{BACKSPACE}");
                                    for (int i2 = 0; i2 < 2; ++i2) SendKeys.SendWait("{DEL}");
                                    SendKeys.SendWait("{3}");
                                    SendKeys.SendWait("{ENTER}");
                                }
                                else if ((string)dgvRow.Cells[0].Value == "O")
                                {   
                                    Cursor.Position = getOrigin(dgv, new Point(x1, y1 + (dgvRow.Height >> 1)));
                                    DoMouseClick();
                                    for (int i2 = 0; i2 < 2; ++i2) SendKeys.SendWait("{BACKSPACE}");
                                    for (int i2 = 0; i2 < 2; ++i2) SendKeys.SendWait("{DEL}");
                                    SendKeys.SendWait("{2}");
                                    SendKeys.SendWait("{ENTER}");
                                }
                                y1 += dgvRow.Height;
                            }
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.AddingFragment:
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.newFragment.addButton);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.SelectNew:
                            int hgtPos = creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.ItemHeight;
                            int i6 = 0;
                            foreach (string itemName in creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.Items)
                            {
                                if (itemName == "testFrag")
                                {
                                    Cursor.Position = getOrigin(creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments, new Point(creatorGUI.ms2fragmentsForm.tabControlFragments.ItemSize.Width >> 1, (int)(hgtPos * (i6 + 0.5))));
                                    DoMouseClick();
                                }
                                ++i6;
                            }
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.ClickOK:
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.buttonOK);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.AddLipid:
                            Cursor.Position = getMiddle(creatorGUI.addLipidButton);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.OpenInterlist:
                            Cursor.Position = getMiddle(creatorGUI.openReviewFormButton);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.OpenReview:
                            Cursor.Position = getMiddle(creatorGUI.lipidsInterList.continueReviewButton);
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            break;
                            
                        case (int)SRMSteps.StoreList:
                            Cursor.Position = getMiddle(creatorGUI.lipidsReview.buttonStoreTransitionList);
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            SendKeys.SendWait("{ESC}");
                            break;
                            
                        case (int)SRMSteps.Finish:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            passed = true;
                            break;
                    
                        default:
                            passed = true;
                            break;
                    }
                    creatorGUI.Refresh();
                }
                if (passed)
                {
                    MessageBox.Show("Second test passed without any problem.");
                }
                else 
                {
                    MessageBox.Show("Second test interrupted");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        
        
        
        
        
        
        public void thirdTutorialTest()
        {
            try 
            {
                bool passed = false;
                Tutorial tutorial = creatorGUI.tutorial;
                TutorialWindow tutorialWindow = tutorial.tutorialWindow;
                int retries = 0;
                int previousStep = 0;
                
                
                while (!passed && tutorial.inTutorial)
                {
                    Thread.Sleep(STEP_SLEEP);
                    
                    if (previousStep != tutorial.tutorialStep)
                    {
                        retries = 0;
                        previousStep = tutorial.tutorialStep;
                    }
                    else
                    {
                        retries += 1;
                    }
                    
                    if (retries >= MAX_RETRIES)
                    {
                        throw new Exception("Tutorial doesn't react at step " + tutorial.tutorialStep.ToString());
                    }
                    
                    switch (tutorial.tutorialStep)
                    {
                            
                        case (int)HLSteps.Null:
                            break;
                            
                        case (int)HLSteps.Welcome:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.OpenHeavy:
                            Cursor.Position = getMiddle(creatorGUI.addHeavyIsotopeButton);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.HeavyPanel:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.NameHeavy:
                            ComboBox cb = creatorGUI.addHeavyPrecursor.comboBox1;
                            Cursor.Position = getOrigin(cb, new Point((int)(cb.Width * 0.9), cb.Height >> 1));
                            DoMouseClick();
                            int ii = 0;
                            while (ii++ < 100)
                            {
                                if ((string)cb.Items[cb.SelectedIndex] == "PG")
                                {
                                    SendKeys.SendWait("{ENTER}");
                                    break;
                                }
                                else
                                {
                                    SendKeys.SendWait("{DOWN}");
                                    Thread.Sleep(10);
                                }
                            }
                            Cursor.Position = getOrigin(creatorGUI.addHeavyPrecursor.textBox1);
                            DoMouseClick();
                            for (int i2 = 0; i2 < 10; ++i2) SendKeys.SendWait("{BACKSPACE}");
                            for (int i2 = 0; i2 < 10; ++i2) SendKeys.SendWait("{DEL}");
                            SendKeys.SendWait("{1}");
                            SendKeys.SendWait("{3}");
                            SendKeys.SendWait("{C}");
                            SendKeys.SendWait("{6}");
                            SendKeys.SendWait("{d}");
                            SendKeys.SendWait("{3}");
                            SendKeys.SendWait("{0}");
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.OptionsExplain:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.SetElements:
                            DataGridView dgv = creatorGUI.addHeavyPrecursor.dataGridView1;
                            int x1 = dgv.Columns[0].Width + dgv.Columns[1].Width + (dgv.Columns[2].Width >> 1);
                            int y1 = dgv.ColumnHeadersHeight;
                            foreach (DataGridViewRow dgvRow in dgv.Rows)
                            {
                                
                                if ((string)dgvRow.Cells[0].Value == "C")
                                {   
                                    Cursor.Position = getOrigin(dgv, new Point(x1, y1 + (dgvRow.Height >> 1)));
                                    DoMouseClick();
                                    SendKeys.SendWait("{6}");
                                    SendKeys.SendWait("{ENTER}");
                                }
                                y1 += dgvRow.Height;
                            }
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.ChangeBuildingBlock:
                            Cursor.Position = getMiddle(creatorGUI.addHeavyPrecursor.comboBox2);
                            DoMouseClick();
                            Thread.Sleep(200);
                            Point pp = Cursor.Position;
                            pp.Y += (int)(creatorGUI.addHeavyPrecursor.comboBox2.Height * 1.5);
                            Cursor.Position = pp;
                            Thread.Sleep(200);
                            DoMouseClick();
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.SetElements2:
                            DataGridView dgv2 = creatorGUI.addHeavyPrecursor.dataGridView1;
                            int x2 = dgv2.Columns[0].Width + dgv2.Columns[1].Width + (dgv2.Columns[2].Width >> 1);
                            int y2 = dgv2.ColumnHeadersHeight;
                            foreach (DataGridViewRow dgvRow in dgv2.Rows)
                            {
                                
                                if ((string)dgvRow.Cells[0].Value == "H")
                                {   
                                    Cursor.Position = getOrigin(dgv2, new Point(x2, y2 + (dgvRow.Height >> 1)));
                                    DoMouseClick();
                                    SendKeys.SendWait("{3}");
                                    SendKeys.SendWait("{0}");
                                    SendKeys.SendWait("{ENTER}");
                                }
                                y2 += dgvRow.Height;
                            }
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.AddIsotope:
                            Cursor.Position = getMiddle(creatorGUI.addHeavyPrecursor.button2);
                            DoMouseClick();
                            SendKeys.SendWait("{ENTER}");
                            break;
                            
                        case (int)HLSteps.EditExplain:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.CloseHeavy:
                            Cursor.Position = getMiddle(creatorGUI.addHeavyPrecursor.button1);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.OpenMS2:
                            Cursor.Position = getMiddle(creatorGUI.MS2fragmentsLipidButton);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.SelectPG:
                            Point p2 = getOrigin(creatorGUI.ms2fragmentsForm.tabControlFragments);
                            p2.X += (int)(creatorGUI.ms2fragmentsForm.tabControlFragments.ItemSize.Width * ((creatorGUI.tutorial.pgIndex % 16) + 0.5));
                            p2.Y += creatorGUI.tabControl.ItemSize.Height >> 2;
                            Cursor.Position = p2;
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.SelectHeavy:
                            ComboBox cb2 = creatorGUI.ms2fragmentsForm.isotopeList;
                            Cursor.Position = getOrigin(cb2, new Point((int)(cb2.Width * 0.9), cb2.Height >> 1));
                            DoMouseClick();
                            SendKeys.SendWait("{DOWN}");
                            SendKeys.SendWait("{ENTER}");
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.SelectFragments:
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.labelPositiveDeselectAll);
                            DoMouseClick();
                            int hgtp = creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.ItemHeight;
                            int i = 0;
                            foreach (string itemName in creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.Items)
                            {
                                if (itemName == "-HG(PG,172)")
                                {
                                    Point p3 = getOrigin(creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments);
                                    p3.X += creatorGUI.ms2fragmentsForm.tabControlFragments.ItemSize.Width >> 1;
                                    p3.Y += (int)(hgtp * (i + 0.5));
                                    Cursor.Position = p3;
                                    DoMouseClick();
                                }
                                ++i;
                            }
                        
                        
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.labelNegativeDeselectAll);
                            DoMouseClick();
                            int hgt = creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.ItemHeight;
                            int i3 = 0;
                            foreach (string itemName in creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.Items)
                            {
                                if (itemName == "FA1(+O)" || itemName == "HG(PG,171)")
                                {
                                    Point p3 = getOrigin(creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments);
                                    p3.X += creatorGUI.ms2fragmentsForm.tabControlFragments.ItemSize.Width >> 1;
                                    p3.Y += (int)(hgt * (i3 + 0.5));
                                    Cursor.Position = p3;
                                    DoMouseClick();
                                }
                                ++i3;
                            }
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.CheckFragment:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.EditFragment:
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.labelNegativeDeselectAll);
                            DoMouseClick();
                            CheckedListBox clb = creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments;
                            int hgtn = clb.ItemHeight;
                            int i4 = 0;
                            foreach (string itemName in clb.Items)
                            {
                                if (itemName == "FA1(+O)")
                                {
                                    
                                    Cursor.Position = getOrigin(clb, new Point(0, -40));
                                    SendKeys.SendWait("{ESC}");
                                    Point p3 = getOrigin(clb);
                                    p3.X += creatorGUI.ms2fragmentsForm.tabControlFragments.ItemSize.Width >> 1;
                                    p3.Y += (int)(hgtn * (i4 + 0.5));
                                    Cursor.Position = p3;
                                    DoMouseRightClick();
                                    Thread.Sleep(200);
                                    p3.X += 20;
                                    p3.Y += 10;
                                    Cursor.Position = p3;
                                    DoMouseClick();
                                }
                                ++i4;
                            }
                            break;
                            
                        case (int)HLSteps.SetFragElement:
                            DataGridView dgvFrag = creatorGUI.ms2fragmentsForm.newFragment.dataGridViewElements;
                            int x3 = dgvFrag.Columns[0].Width + (dgvFrag.Columns[2].Width >> 1);
                            int y3 = dgvFrag.ColumnHeadersHeight;
                            foreach (DataGridViewRow dgvRow in dgvFrag.Rows)
                            {
                                
                                if ((string)dgvRow.Cells[0].Value == "H")
                                {   
                                    Cursor.Position = getOrigin(dgvFrag, new Point(x3, y3 + (dgvRow.Height >> 1)));
                                    DoMouseClick();
                                    for (int i2 = 0; i2 < 2; ++i2) SendKeys.SendWait("{BACKSPACE}");
                                    for (int i2 = 0; i2 < 2; ++i2) SendKeys.SendWait("{DEL}");
                                    SendKeys.SendWait("{0}");
                                    SendKeys.SendWait("{ENTER}");
                                    Cursor.Position = getOrigin(dgvFrag, new Point(x3 + dgvFrag.Columns[1].Width, y3 + (dgvRow.Height >> 1)));
                                    DoMouseClick();
                                    for (int i2 = 0; i2 < 2; ++i2) SendKeys.SendWait("{BACKSPACE}");
                                    for (int i2 = 0; i2 < 2; ++i2) SendKeys.SendWait("{DEL}");
                                    SendKeys.SendWait("{1}");
                                    SendKeys.SendWait("{ENTER}");
                                    break;
                                }
                                y3 += dgvRow.Height;
                            }
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.ConfirmEdit:
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.newFragment.addButton);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.CloseFragment:
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.buttonOK);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.OpenFilter:
                            Cursor.Position = getMiddle(creatorGUI.filtersButton);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.SelectFilter:
                            Cursor.Position = getMiddle(creatorGUI.filterDialog.radioButton5);
                            DoMouseClick();
                            Cursor.Position = getMiddle(creatorGUI.filterDialog.button2);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.AddLipid:
                            Cursor.Position = getMiddle(creatorGUI.addLipidButton);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.OpenInterlist:
                            Cursor.Position = getMiddle(creatorGUI.openReviewFormButton);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.OpenReview:
                            Cursor.Position = getMiddle(creatorGUI.lipidsInterList.continueReviewButton);
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            break;
                            
                        case (int)HLSteps.StoreList:
                            Cursor.Position = getMiddle(creatorGUI.lipidsReview.buttonStoreTransitionList);
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            SendKeys.SendWait("{ESC}");
                            break;
                            
                        case (int)HLSteps.Finish:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            passed = true;
                            break;
                    
                        default:
                            passed = true;
                            break;
                    }
                    creatorGUI.Refresh();
                }
                if (passed)
                {
                    MessageBox.Show("Third test passed without any problem.");
                }
                else 
                {
                    MessageBox.Show("Third test interrupted");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        
        
        
        
        
        

        public void firstTutorialStart(Object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(firstTutorialTest));
            t.Start();
            try
            {
                t.TrySetApartmentState(ApartmentState.STA);
            }
            catch (ThreadStateException)
            {
                Console.WriteLine("ThreadStateException occurs if apartment state is set after starting thread.");
            }
        }
        
        

        public void secondTutorialStart(Object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(secondTutorialTest));
            t.Start();
            try
            {
                t.TrySetApartmentState(ApartmentState.STA);
            }
            catch (ThreadStateException)
            {
                Console.WriteLine("ThreadStateException occurs if apartment state is set after starting thread.");
            }
        }
        
        

        public void thirdTutorialStart(Object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(thirdTutorialTest));
            t.Start();
            try
            {
                t.TrySetApartmentState(ApartmentState.STA);
            }
            catch (ThreadStateException)
            {
                Console.WriteLine("ThreadStateException occurs if apartment state is set after starting thread.");
            }
        }
        
        
        
        
        
        public TestTutorials()
        {
            creatorGUI = new CreatorGUI(null);
            creatorGUI.startFirstTutorialButton.Click += new EventHandler(firstTutorialStart);
            creatorGUI.startSecondTutorialButton.Click += new EventHandler(secondTutorialStart);
            creatorGUI.startThirdTutorialButton.Click += new EventHandler(thirdTutorialStart);
            Application.Run(creatorGUI);
        }
    
        [STAThread]
        public static void Main(string[] Args)
        {
            new TestTutorials();
        }
    }
}