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
            Point buttonCentre = new Point(control.Width / 2, control.Height / 2);
            return control.PointToScreen(buttonCentre);
            /*
            Point point = control.PointToScreen(Point.Empty);
            //Point point = creatorGUI.PointToClient();
            //point.X += control.Size.Width >> 1;
            //point.Y += control.Size.Height >> 1;
            return point;
            */
        }
        
        public Point getOrigin(Control control)
        {
            return control.PointToScreen(control.Location);
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
                    Thread.Sleep(500);
                    
                    if (previousStep != tutorial.tutorialStep)
                    {
                        retries = 0;
                        previousStep = tutorial.tutorialStep;
                    }
                    else
                    {
                        retries += 1;
                    }
                    
                    if (retries >= 3)
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
                            foreach (string item in creatorGUI.plHgListbox.Items)
                            {
                                if (item == "PG")
                                {
                                    creatorGUI.plHgListbox.SelectedIndex = pg_I;
                                    break;
                                }
                                pg_I += 1;
                            }
                            tutorialWindow.next.PerformClick();
                            break;
                            
                        case (int)PRMSteps.SetFA:
                            creatorGUI.plFA1Textbox.Text = "14-18,20";
                            Cursor.Position = getMiddle(tutorialWindow.next);
                            DoMouseClick();
                            break;
                        
                        case (int)PRMSteps.SetDB:
                            creatorGUI.plDB1Textbox.Text = "0,1";
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
                            creatorGUI.plFA2Textbox.Text = "8-10";
                            creatorGUI.plDB2Textbox.Text = "2";
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
                            Thread.Sleep(500);
                            break;
                        
                        case (int)PRMSteps.StoreList:
                            Cursor.Position = getMiddle(creatorGUI.lipidsReview.buttonStoreTransitionList);
                            DoMouseClick();
                            Thread.Sleep(500);
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
        
        
        public TestTutorials()
        {
            creatorGUI = new CreatorGUI(null);
            creatorGUI.startFirstTutorialButton.Click += new EventHandler(firstTutorialStart);
            Application.Run(creatorGUI);
        }
    
        [STAThread]
        public static void Main(string[] Args)
        {
            new TestTutorials();
        }
    }
}