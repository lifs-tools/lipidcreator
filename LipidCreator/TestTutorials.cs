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
        
        public CreatorGUI creatorGUI;
        
        public Point getMiddle(Control control)
        {
            Point point = control.PointToScreen(control.Location);
            point.X += control.Size.Width >> 1;
            point.Y += control.Size.Height >> 1;
            return point;
        }
        
        public Point getOrigin(Control control)
        {
            return control.PointToScreen(control.Location);
        }
        
        public void firstTutorialTest()
        {
            Console.WriteLine("starteed test");
            try 
            {
                bool passed = false;
                Tutorial tutorial = creatorGUI.tutorial;
                TutorialWindow tutorialWindow = tutorial.tutorialWindow;
                int retries = 0;
                int previousStep = 0;
                
                
                while (!passed && tutorial.inTutorial)
                {
                    Console.WriteLine("in test");
                    Thread.Sleep(1000);
                    
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
                        case 1:
                            tutorialWindow.next.PerformClick();
                            break;
                            
                        case 2:
                            Point p2 = getOrigin(creatorGUI.tabControl);
                            p2.X += (int)(creatorGUI.tabControl.ItemSize.Width * 2.5);
                            p2.Y += creatorGUI.tabControl.ItemSize.Height >> 1;
                            Cursor.Position = p2;
                            creatorGUI.tabControl.SelectedIndex = 2;
                            break;
                            
                            
                        case 3:
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
                            
                        case 0:
                            break;
                    
                        default:
                            passed = true;
                            break;
                    }
                    creatorGUI.Refresh();
                }
                if (passed)
                {
                    Console.WriteLine("First test passed");
                }
                else 
                {
                    Console.WriteLine("First test interrupteds");
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
        }
        
        public TestTutorials()
        {
            creatorGUI = new CreatorGUI(null);
            creatorGUI.startFirstTutorialButton.Click += new EventHandler(firstTutorialStart);
            Application.Run(creatorGUI);
        }
    
        public static void Main(string[] Args)
        {
            new TestTutorials();
        }
    }
}