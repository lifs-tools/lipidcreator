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
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo); 
        
        
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const int MAX_RETRIES = 3;
        public const int STEP_SLEEP = 500;
        public const int KEY_SLEEP = 100;
        public const int ANIMATION_SLEEP = 10;
        public const double ANIMATION_STEPS = 100.0;
        
        
        // Keyboard keys
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        public const int VK_RCONTROL = 0xA3; //Right Control key code
        public const int VK_LCONTROL = 0xA2; //Left Control key code
        public const int VK_SHIFT = 0x10;
        public const int VK_END = 0x23;
        public const int VK_HOME = 0x24;
        
        public const int KEY_6 = 0x36;
        public const int KEY_COMMA = 0xBC;
        public const int KEY_DASH = 0xBD;
        public const int KEY_BACKSPACE = 0x08;
        public const int KEY_DEL = 0x2E;
        public const int KEY_SPACE = 0x20;
        public const int KEY_ESC = 0x1B;
        public const int KEY_ENTER = 0x0D;
        public const int KEY_PGDN = 0x22;
        public const int KEY_DOWN = 0x28;
        public const int KEY_RIGHT = 0x27;
        public const int KEY_UP = 0x26;
        public const int KEY_F10 = 0x79;
        
        
        public void keyPress(int key, bool shift = false)
        {
            Thread.Sleep(KEY_SLEEP);
            if (shift) keybd_event((byte)VK_SHIFT, 0, 0, 0);
            keybd_event((byte)key, 0, 0, 0);
            keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
            if (shift) keybd_event((byte)VK_SHIFT, 0, KEYEVENTF_KEYUP, 0);
        }
        
        public void selectAllText()
        {
            Thread.Sleep(KEY_SLEEP);
            
            keybd_event(VK_HOME, 0, 0, 0);
            Thread.Sleep(10);
            keybd_event(VK_HOME, 0, KEYEVENTF_KEYUP, 0);
            Thread.Sleep(10);
            keybd_event(VK_SHIFT,0x2A,0,0);
            Thread.Sleep(10);
            keybd_event(VK_END,0x4F,KEYEVENTF_EXTENDEDKEY | 0,0);
            Thread.Sleep(10);
            keybd_event(VK_END,0x4F,KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP,0);
            Thread.Sleep(10);
            keybd_event(VK_SHIFT,0x2A,KEYEVENTF_KEYUP,0); 
        }
        
        
        public void DoMouseClick()
        {
            //Call the imported function with the cursor's current position
            Thread.Sleep(STEP_SLEEP);
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }
        
        
        public void DoMouseDown()
        {
            //Call the imported function with the cursor's current position
            Thread.Sleep(STEP_SLEEP);
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
        }
        
        
        public void DoMouseUp()
        {
            //Call the imported function with the cursor's current position
            Thread.Sleep(STEP_SLEEP);
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
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
        
        
        
        public void relocateForm(Form form)
        {
            moveMouse(new Point(form.Left + 30, form.Top + 10));
            DoMouseDown();
            moveMouse(new Point(50, 30));
            DoMouseUp();
            Thread.Sleep(50);
        }
        
        
        
        public void moveMouse(Point destination)
        {
            int sourceX = Cursor.Position.X;
            int sourceY = Cursor.Position.Y;
            for (int i = 1; i <= (int)ANIMATION_STEPS; ++i)
            {
                Cursor.Position = new Point((int)(sourceX + (destination.X - sourceX) * ((double)(i)) / ANIMATION_STEPS), sourceY + (int)((destination.Y - sourceY) * (double)i / ANIMATION_STEPS));
                Thread.Sleep(ANIMATION_SLEEP);
            }
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
                            relocateForm(creatorGUI);
                        
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            break;
                            
                        case (int)PRMSteps.PhosphoTab:
                        
                            Point p2 = getOrigin(creatorGUI.tabControl);
                            p2.X += (int)(creatorGUI.tabControl.ItemSize.Width * 2.5);
                            p2.Y += creatorGUI.tabControl.ItemSize.Height >> 1;
                            moveMouse(p2);
                            DoMouseClick();
                            break;
                            
                            
                        case (int)PRMSteps.PGheadgroup:
                            int pg_I = 0;
                            
                            ListBox plHG = creatorGUI.plHgListbox;
                            for (; pg_I < plHG.Items.Count; ++pg_I) if (plHG.Items[pg_I].ToString().Equals("PG")) break;
                            
                            Point p3 = getOrigin(plHG);
                            p3.X += plHG.Size.Width >> 1;
                            p3.Y += (int)((pg_I + 0.5) * plHG.ItemHeight);
                            moveMouse(p3);
                            DoMouseClick();
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            break;
                            
                        case (int)PRMSteps.SetFA:
                            moveMouse(getMiddle(creatorGUI.plFA1Textbox));
                            DoMouseClick();
                            selectAllText();
                            keyPress(KEY_BACKSPACE);
                            
                            keyPress('1');
                            keyPress('4');
                            keyPress(KEY_DASH);
                            keyPress('1');
                            keyPress('8');
                            keyPress(KEY_COMMA);
                            keyPress('2');
                            keyPress('0');
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.SetDB:
                            moveMouse(getMiddle(creatorGUI.plDB1Textbox));
                            DoMouseClick();
                            selectAllText();
                            keyPress(KEY_BACKSPACE);
                            keyPress('0');
                            keyPress(KEY_DASH);
                            keyPress('1');
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.MoreParameters:
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.Ether:
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.SecondFADB:
                            moveMouse(getMiddle(creatorGUI.plFA2Textbox));
                            DoMouseClick();
                            selectAllText();
                            keyPress(KEY_BACKSPACE);
                            keyPress('8');
                            keyPress(KEY_DASH);
                            keyPress('1');
                            keyPress('0');
                            moveMouse(getMiddle(creatorGUI.plDB2Textbox));
                            DoMouseClick();
                            selectAllText();
                            keyPress(KEY_BACKSPACE);
                            keyPress('2');
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.SelectAdduct:
                            moveMouse(getMiddle(creatorGUI.plPosAdductCheckbox1));
                            DoMouseClick();
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.OpenFilter:
                            moveMouse(getMiddle(creatorGUI.filtersButton));
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.SelectFilter:
                            relocateForm(creatorGUI.filterDialog);
                            
                            moveMouse(getMiddle(creatorGUI.filterDialog.radioButton2));
                            DoMouseClick();
                            moveMouse(getMiddle(creatorGUI.filterDialog.button2));
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.AddLipid:
                            moveMouse(getMiddle(creatorGUI.addLipidButton));
                            DoMouseClick();
                            break;
                            
                            
                            
                        case (int)PRMSteps.ChangeGlycero:
                        
                            Point pg = getOrigin(creatorGUI.tabControl);
                            pg.X += (int)(creatorGUI.tabControl.ItemSize.Width * 1.5);
                            pg.Y += creatorGUI.tabControl.ItemSize.Height >> 1;
                            moveMouse(pg);
                            DoMouseClick();
                            break;
                            
                            
                        case (int)PRMSteps.SetGLFA:
                            moveMouse(getMiddle(creatorGUI.glFA1Textbox));
                            DoMouseClick();
                            selectAllText();
                            keyPress(KEY_BACKSPACE);
                            
                            keyPress('1');
                            keyPress('6');
                            keyPress(KEY_DASH);
                            keyPress('2');
                            keyPress('0');
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            break;
                            
                            
                        case (int)PRMSteps.EvenChain:
                            moveMouse(getMiddle(creatorGUI.glFA1Combobox));
                            DoMouseClick();
                            keyPress(KEY_DOWN);
                            keyPress(KEY_DOWN);
                            keyPress(KEY_ENTER);
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            break;    
                            
                            
                        
                        case (int)PRMSteps.RepresentitativeFA:
                            moveMouse(getMiddle(creatorGUI.glRepresentativeFA));
                            DoMouseClick();
                        
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            break;
                    
                    
                        case (int)PRMSteps.DeselectThirdFA:
                            moveMouse(getMiddle(creatorGUI.glFA3Checkbox1));
                            DoMouseClick();
                        
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            break;
                            
                            
                        case (int)PRMSteps.AddGL:
                            moveMouse(getMiddle(creatorGUI.addLipidButton));
                            DoMouseClick();
                            break;
                            
                        
                        case (int)PRMSteps.OpenInterlist:
                            moveMouse(getMiddle(creatorGUI.openReviewFormButton));
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.ExplainInterlist:
                            relocateForm(creatorGUI.lipidsInterList);
                            
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.OpenReview:
                            moveMouse(getMiddle(creatorGUI.lipidsInterList.continueReviewButton));
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            break;
                        
                        case (int)PRMSteps.StoreList:
                            relocateForm(creatorGUI.lipidsReview);
                            moveMouse(getMiddle(creatorGUI.lipidsReview.buttonStoreTransitionList));
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            
                            relocateForm(creatorGUI.lipidsReview.exportParameters);
                            moveMouse(getMiddle(creatorGUI.lipidsReview.exportParameters.button1));
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            keyPress(KEY_ESC);
                            Thread.Sleep(STEP_SLEEP);
                            keyPress(KEY_ESC);
                            Thread.Sleep(STEP_SLEEP);
                            keyPress(KEY_ESC);
                            break;
                        
                        case (int)PRMSteps.Finish:
                            moveMouse(getMiddle(tutorialWindow.next));
                            DoMouseClick();
                            passed = true;
                            break;
                            
                        case (int)PRMSteps.Null:
                            break;
                    
                        default:
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
                            relocateForm(creatorGUI);
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
                            relocateForm(creatorGUI.ms2fragmentsForm);
                            
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
                            ArrayList cursorPosition = new ArrayList();
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
                                    cursorPosition.Add(p3);
                                }
                                ++i;
                            }
                            foreach (Point p3 in cursorPosition)
                            {
                                Cursor.Position = p3;
                                DoMouseClick();
                            }
                            
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.AddFragment:
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.buttonAddFragment);
                            DoMouseClick();
                        
                            break;
                            
                        case (int)SRMSteps.InFragment:
                            relocateForm(creatorGUI.ms2fragmentsForm.newFragment);
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)SRMSteps.NameFragment:
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.newFragment.textBoxFragmentName);
                            DoMouseClick();
                            selectAllText();
                            keyPress(KEY_BACKSPACE);
                            keyPress('T');
                            keyPress('E');
                            keyPress('S');
                            keyPress('T');
                            keyPress('F', true);
                            keyPress('R');
                            keyPress('A');
                            keyPress('G');
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
                                    DoMouseClick();
                                    keyPress(KEY_BACKSPACE);
                                    keyPress('3');
                                    keyPress(KEY_ENTER);
                                }
                                else if ((string)dgvRow.Cells[0].Value == "O")
                                {   
                                    Cursor.Position = getOrigin(dgv, new Point(x1, y1 + (dgvRow.Height >> 1)));
                                    DoMouseClick();
                                    DoMouseClick();
                                    keyPress(KEY_BACKSPACE);
                                    keyPress('2');
                                    keyPress(KEY_ENTER);
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
                                    break;
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
                            relocateForm(creatorGUI.lipidsInterList);
                            Cursor.Position = getMiddle(creatorGUI.lipidsInterList.continueReviewButton);
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            break;
                            
                        case (int)SRMSteps.StoreList:
                            relocateForm(creatorGUI.lipidsReview);
                            moveMouse(getMiddle(creatorGUI.lipidsReview.buttonStoreTransitionList));
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            
                            relocateForm(creatorGUI.lipidsReview.exportParameters);
                            moveMouse(getMiddle(creatorGUI.lipidsReview.exportParameters.button1));
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            keyPress(KEY_ESC);
                            Thread.Sleep(STEP_SLEEP);
                            keyPress(KEY_ESC);
                            Thread.Sleep(STEP_SLEEP);
                            keyPress(KEY_ESC);
                            break;
                            
                            
                        case (int)SRMSteps.Finish:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            passed = true;
                            break;
                    
                        default:
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
                            relocateForm(creatorGUI);
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.OpenHeavy:
                            Cursor.Position = getMiddle(creatorGUI.addHeavyIsotopeButton);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.HeavyPanel:
                            relocateForm(creatorGUI.addHeavyPrecursor);
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.NameHeavy:
                            ComboBox cb = creatorGUI.addHeavyPrecursor.comboBox1;
                            Cursor.Position = getOrigin(cb, new Point((int)(cb.Width * 0.9), cb.Height >> 1));
                            DoMouseClick();
                            Thread.Sleep(50);
                            string lastSelected = "";
                            bool clickNext = false;
                            while (true)
                            {
                                
                                if ((string)cb.Items[cb.SelectedIndex] == "PG")
                                {
                                    keyPress(KEY_ENTER);
                                    break;
                                }
                                else
                                {
                                    if (clickNext)
                                    {
                                        clickNext = false;
                                        keyPress(KEY_DOWN);
                                        Thread.Sleep(30);
                                    }
                                    else if (lastSelected != (string)cb.Items[cb.SelectedIndex])
                                    {
                                        clickNext = true;
                                    }
                                }
                            }
                            Cursor.Position = getOrigin(creatorGUI.addHeavyPrecursor.textBox1);
                            DoMouseClick();
                            selectAllText();
                            keyPress(KEY_BACKSPACE);
                            keyPress('1');
                            keyPress('3');
                            keyPress('C', true);
                            keyPress('6');
                            keyPress('D');
                            keyPress('3');
                            keyPress('0');
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
                                    keyPress('6');
                                    keyPress(KEY_ENTER);
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
                                    keyPress('3');
                                    keyPress('0');
                                    keyPress(KEY_ENTER);
                                }
                                y2 += dgvRow.Height;
                            }
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.AddIsotope:
                            Cursor.Position = getMiddle(creatorGUI.addHeavyPrecursor.button2);
                            DoMouseClick();
                            keyPress(KEY_ENTER);
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
                            relocateForm(creatorGUI.ms2fragmentsForm);
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
                            keyPress(KEY_DOWN);
                            keyPress(KEY_ENTER);
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)HLSteps.SelectFragments:
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.labelPositiveDeselectAll);
                            DoMouseClick();
                            int hgtp = creatorGUI.ms2fragmentsForm.checkedListBoxPositiveFragments.ItemHeight;
                            
                            /*
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
                                    break;
                                }
                                ++i;
                            }
                            */
                        
                        
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.labelNegativeDeselectAll);
                            DoMouseClick();
                            ArrayList cursorPosition = new ArrayList();
                            int hgt = creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.ItemHeight;
                            int i3 = 0;
                            foreach (string itemName in creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments.Items)
                            {
                                if (itemName == "FA1(+O)" || itemName == "HG(PG,171)")
                                {
                                    Point p3 = getOrigin(creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments);
                                    p3.X += creatorGUI.ms2fragmentsForm.tabControlFragments.ItemSize.Width >> 1;
                                    p3.Y += (int)(hgt * (i3 + 0.5));
                                    cursorPosition.Add(p3);
                                }
                                ++i3;
                            }
                            
                            foreach (Point p3 in cursorPosition)
                            {
                                Cursor.Position = p3;
                                DoMouseClick();
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
                                    
                                    keyPress(KEY_ESC);
                                    Point p3 = getOrigin(clb);
                                    p3.X += creatorGUI.ms2fragmentsForm.tabControlFragments.ItemSize.Width >> 2;
                                    p3.Y += (int)(hgtn * (i4 + 0.5));
                                    Cursor.Position = p3;
                                    //Thread.Sleep(200);
                                    DoMouseRightClick();
                                    Thread.Sleep(200);
                                    //creatorGUI.ms2fragmentsForm.menuFragmentItem1.Enabled = true;
                                    if (creatorGUI.ms2fragmentsForm.menuFragmentItem1.Enabled)
                                    {
                                        p3.X += 20;
                                        p3.Y += 10;
                                        Cursor.Position = p3;
                                        DoMouseClick();
                                    }
                                    break;
                                    
                                }
                                ++i4;
                            }
                            break;
                            
                            
                            
                            
                            
                        case (int)HLSteps.SetFragElement:
                            relocateForm(creatorGUI.ms2fragmentsForm.newFragment);
                            DataGridView dgvFrag = creatorGUI.ms2fragmentsForm.newFragment.dataGridViewElements;
                            int x3 = dgvFrag.Columns[0].Width + (dgvFrag.Columns[2].Width >> 1);
                            int y3 = dgvFrag.ColumnHeadersHeight;
                            foreach (DataGridViewRow dgvRow in dgvFrag.Rows)
                            {
                                
                                if ((string)dgvRow.Cells[0].Value == "H")
                                {   
                                    Cursor.Position = getOrigin(dgvFrag, new Point(x3, y3 + (dgvRow.Height >> 1)));
                                    DoMouseClick();
                                    DoMouseClick();
                                    keyPress(KEY_BACKSPACE);
                                    keyPress('0');
                                    keyPress(KEY_ENTER);
                                    Cursor.Position = getOrigin(dgvFrag, new Point(x3 + dgvFrag.Columns[1].Width, y3 + (dgvRow.Height >> 1)));
                                    DoMouseClick();
                                    DoMouseClick();
                                    keyPress(KEY_BACKSPACE);
                                    keyPress('1');
                                    keyPress(KEY_ENTER);
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
                            
                            
                            
                            
                            
                            
                            
                            
                        case (int)HLSteps.EditFragmentSecond:
                            Cursor.Position = getMiddle(creatorGUI.ms2fragmentsForm.labelNegativeDeselectAll);
                            DoMouseClick();
                            CheckedListBox clb2 = creatorGUI.ms2fragmentsForm.checkedListBoxNegativeFragments;
                            int hgtn2 = clb2.ItemHeight;
                            int i5 = 0;
                            foreach (string itemName in clb2.Items)
                            {
                                if (itemName == "HG(PG,171)")
                                {
                                    
                                    keyPress(KEY_ESC);
                                    Point p3 = getOrigin(clb2);
                                    p3.X += creatorGUI.ms2fragmentsForm.tabControlFragments.ItemSize.Width >> 2;
                                    p3.Y += (int)(hgtn2 * (i5 + 0.5));
                                    Cursor.Position = p3;
                                    DoMouseRightClick();
                                    Thread.Sleep(200);
                                    if (creatorGUI.ms2fragmentsForm.menuFragmentItem1.Enabled)
                                    {
                                        p3.X += 20;
                                        p3.Y += 10;
                                        Cursor.Position = p3;
                                        DoMouseClick();
                                    }
                                    break;
                                    
                                }
                                ++i5;
                            }
                            break;
                            
                            
                            
                            
                            
                        case (int)HLSteps.SetFragElementSecond:
                            relocateForm(creatorGUI.ms2fragmentsForm.newFragment);
                            DataGridView dgvFrag2 = creatorGUI.ms2fragmentsForm.newFragment.dataGridViewElements;
                            int x4 = dgvFrag2.Columns[0].Width + (dgvFrag2.Columns[2].Width >> 1);
                            int y4 = dgvFrag2.ColumnHeadersHeight;
                            foreach (DataGridViewRow dgvRow in dgvFrag2.Rows)
                            {
                                
                                if ((string)dgvRow.Cells[0].Value == "C")
                                {   
                                    Cursor.Position = getOrigin(dgvFrag2, new Point(x4, y4 + (dgvRow.Height >> 1)));
                                    DoMouseClick();
                                    DoMouseClick();
                                    keyPress(KEY_BACKSPACE);
                                    keyPress('0');
                                    keyPress(KEY_ENTER);
                                    Cursor.Position = getOrigin(dgvFrag2, new Point(x4 + dgvFrag2.Columns[1].Width, y4 + (dgvRow.Height >> 1)));
                                    DoMouseClick();
                                    DoMouseClick();
                                    keyPress(KEY_BACKSPACE);
                                    keyPress(KEY_DASH);
                                    keyPress('3');
                                    keyPress(KEY_ENTER);
                                    break;
                                }
                                y4 += dgvRow.Height;
                            }
                            
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
                            relocateForm(creatorGUI.filterDialog);
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
                            relocateForm(creatorGUI.lipidsInterList);
                            Cursor.Position = getMiddle(creatorGUI.lipidsInterList.continueReviewButton);
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            break;
                            
                        case (int)HLSteps.StoreList:
                            relocateForm(creatorGUI.lipidsReview);
                            moveMouse(getMiddle(creatorGUI.lipidsReview.buttonStoreTransitionList));
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            
                            relocateForm(creatorGUI.lipidsReview.exportParameters);
                            moveMouse(getMiddle(creatorGUI.lipidsReview.exportParameters.button1));
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            keyPress(KEY_ESC);
                            Thread.Sleep(STEP_SLEEP);
                            keyPress(KEY_ESC);
                            Thread.Sleep(STEP_SLEEP);
                            keyPress(KEY_ESC);
                            break;
                            
                            
                        case (int)HLSteps.Finish:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            passed = true;
                            break;
                    
                        default:
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
        
        
        
        
        
        
        public void fourthTutorialTest()
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
                            
                        case (int)CESteps.Null:
                            break;
                            
                        case (int)CESteps.Welcome:
                            relocateForm(creatorGUI);
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)CESteps.ActivateCE:
                            keyPress(KEY_F10);
                            keyPress(KEY_RIGHT);
                            keyPress(KEY_DOWN);
                            keyPress(KEY_DOWN);
                            keyPress(KEY_RIGHT);
                            keyPress(KEY_DOWN);
                            keyPress(KEY_ENTER);
                            Thread.Sleep(50);
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)CESteps.OpenCEDialog:
                            keyPress(KEY_F10);
                            keyPress(KEY_RIGHT);
                            keyPress(KEY_DOWN);
                            keyPress(KEY_DOWN);
                            keyPress(KEY_DOWN);
                            keyPress(KEY_ENTER);
                            break;
                            
                        case (int)CESteps.SelectTXB2:
                            relocateForm(creatorGUI.ceInspector);
                            
                            ComboBox cb = creatorGUI.ceInspector.classCombobox;
                            Cursor.Position = getOrigin(cb, new Point((int)(cb.Width * 0.9), cb.Height >> 1));
                            DoMouseClick();
                            while (cb.SelectedIndex != cb.Items.Count - 1)
                            {
                                keyPress(KEY_PGDN);
                                Thread.Sleep(10);
                            }
                            
                            
                            string lastSelected = "";
                            bool clickNext = false;
                            while (true)
                            {
                                
                                if ((string)cb.Items[cb.SelectedIndex] == "TXB2")
                                {
                                    keyPress(KEY_ENTER);
                                    break;
                                }
                                else
                                {
                                    if (clickNext)
                                    {
                                        clickNext = false;
                                        keyPress(KEY_UP);
                                        Thread.Sleep(30);
                                    }
                                    else if (lastSelected != (string)cb.Items[cb.SelectedIndex])
                                    {
                                        clickNext = true;
                                    }
                                }
                            }
                            
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)CESteps.ExplainBlackCurve:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)CESteps.ChangeManually:
                            Cursor.Position = getMiddle(creatorGUI.ceInspector.radioButtonPRMArbitrary);
                            DoMouseClick();
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)CESteps.CEto20:
                            Cursor.Position = getMiddle(creatorGUI.ceInspector.numericalUpDownCurrentCE);
                            DoMouseClick();
                            selectAllText();
                            keyPress(KEY_BACKSPACE);
                            keyPress('2');
                            keyPress('0');
                            keyPress(KEY_ENTER);
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)CESteps.SameForD4:
                            ComboBox cbc = creatorGUI.ceInspector.classCombobox;
                            Cursor.Position = getOrigin(cbc, new Point((int)(cbc.Width * 0.9), cbc.Height >> 1));
                            DoMouseClick();
                            Thread.Sleep(10);
                            keyPress(KEY_DOWN);
                            Thread.Sleep(10);
                            keyPress(KEY_ENTER);
                            Thread.Sleep(10);
                            Cursor.Position = getMiddle(creatorGUI.ceInspector.numericalUpDownCurrentCE);
                            DoMouseClick();
                            selectAllText();
                            keyPress(KEY_BACKSPACE);
                            keyPress('2');
                            keyPress('0');
                            keyPress(KEY_ENTER);
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)CESteps.CloseCE:
                            Cursor.Position = getMiddle(creatorGUI.ceInspector.button2);
                            DoMouseClick();
                            break;
                            
                        case (int)CESteps.ChangeToMediators:
                            Cursor.Position = getOrigin(creatorGUI.tabControl, new Point((int)(creatorGUI.tabControl.ItemSize.Width * 5.5), creatorGUI.tabControl.ItemSize.Height >> 1));
                            DoMouseClick();
                            break;
                            
                        case (int)CESteps.SelectTXB2HG:
                            Cursor.Position = getOrigin(creatorGUI.medHgListbox, new Point(10, 10));
                            DoMouseClick();
                            Thread.Sleep(10);
                            
                            
                            string lastSelectedHG = "";
                            bool clickNextHG = false;
                            while (true)
                            {
                                
                                if (creatorGUI.medHgListbox.SelectedItems.Count > 0)
                                {
                                    if ((string)creatorGUI.medHgListbox.SelectedItems[0] == "TXB2")
                                    {
                                        keyPress(KEY_ENTER);
                                        break;
                                    }
                                    else
                                    {
                                        if (clickNextHG)
                                        {
                                            clickNextHG = false;
                                            keyPress(KEY_SPACE);
                                            keyPress(KEY_DOWN);
                                            keyPress(KEY_SPACE);
                                            Thread.Sleep(30);
                                        }
                                        else if (lastSelectedHG != (string)creatorGUI.medHgListbox.SelectedItems[0])
                                        {
                                            clickNextHG = true;
                                        }
                                    }
                                }
                            }
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)CESteps.AddLipid:
                            Cursor.Position = getMiddle(creatorGUI.addLipidButton);
                            DoMouseClick();
                            break;
                            
                        case (int)CESteps.OpenInterlist:
                            Cursor.Position = getMiddle(creatorGUI.openReviewFormButton);
                            DoMouseClick();
                            break;
                            
                        case (int)CESteps.ReviewLipids:
                            relocateForm(creatorGUI.lipidsInterList);
                            Cursor.Position = getMiddle(creatorGUI.lipidsInterList.continueReviewButton);
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            break;
                            
                        case (int)CESteps.ExplainLCasExternal:
                            relocateForm(creatorGUI.lipidsReview);
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                            
                        case (int)CESteps.StoreBlib:
                            Cursor.Position = getMiddle(creatorGUI.lipidsReview.buttonStoreSpectralLibrary);
                            DoMouseClick();
                            Thread.Sleep(STEP_SLEEP);
                            keyPress(KEY_ESC);
                            break;
                            
                        case (int)CESteps.Finish:
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            passed = true;
                            break;
                    
                        default:
                            break;
                    }
                    creatorGUI.Refresh();
                }
                if (passed)
                {
                    MessageBox.Show("Fourth test passed without any problem.");
                }
                else 
                {
                    MessageBox.Show("Fourth test interrupted");
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
        
        

        public void fourthTutorialStart(Object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(fourthTutorialTest));
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
            creatorGUI.startFourthTutorialButton.Click += new EventHandler(fourthTutorialStart);
            Application.Run(creatorGUI);
        }
    
        [STAThread]
        public static void Main(string[] Args)
        {
            new TestTutorials();
        }
    }
}