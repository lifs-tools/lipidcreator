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
                            int iDGV = dgv.ColumnHeadersHeight ;
                            foreach (DataGridViewRow dgvRow in dgv.Rows)
                            {
                            Console.WriteLine(dgvRow.Cells[0].Value.ToString());
                                if (dgvRow.Cells[0].Value.ToString() == "H")
                                {
                                    Cursor.Position = getMiddle(dgvRow.Cells[1]);
                                    DoMouseClick();
                                    SendKeys.SendWait("{3}");
                                    SendKeys.SendWait("{ENTER}");
                                }
                                else if (dgvRow.Cells[0].Value.ToString() == "O")
                                {
                                    Cursor.Position = getMiddle(dgvRow.Cells[1]);
                                    DoMouseClick();
                                    SendKeys.SendWait("{3}");
                                    SendKeys.SendWait("{ENTER}");
                                }
                            }
                            break;
                            
                        case (int)SRMSteps.AddingFragment:
                            break;
                            
                        case (int)SRMSteps.SelectNew:
                            break;
                            
                        case (int)SRMSteps.ClickOK:
                            break;
                            
                        case (int)SRMSteps.AddLipid:
                            break;
                            
                        case (int)SRMSteps.OpenInterlist:
                            break;
                            
                        case (int)SRMSteps.OpenReview:
                            break;
                            
                        case (int)SRMSteps.StoreList:
                            break;
                            
                        case (int)SRMSteps.Finish:
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
        
        
        
        
        
        public TestTutorials()
        {
            creatorGUI = new CreatorGUI(null);
            creatorGUI.startFirstTutorialButton.Click += new EventHandler(firstTutorialStart);
            creatorGUI.startSecondTutorialButton.Click += new EventHandler(secondTutorialStart);
            Application.Run(creatorGUI);
        }
    
        [STAThread]
        public static void Main(string[] Args)
        {
            new TestTutorials();
        }
    }
}