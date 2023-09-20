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
using System.Diagnostics;
using log4net;
using log4net.Config;
using System.IO;

namespace LipidCreator
{
    public partial class AboutDialog : Form
    {
        public CreatorGUI creatorGUI;
        private static readonly ILog log = LogManager.GetLogger(typeof(AboutDialog));

        public AboutDialog(CreatorGUI _creatorGUI, bool log = false)
        {
            creatorGUI = _creatorGUI;

            InitializeComponent();
            InitializeCustom();

            if (log)
            {
                Text = "Log messages";
                showLogFile();
                ContextMenu contextMenu = new ContextMenu();
                int p = (int)Environment.OSVersion.Platform;
                if ((p == 4) || (p == 6) || (p == 128))
                {

                    MenuItem menuItem = new MenuItem("Open log file directory");
                    menuItem.Click += OpenAction;
                    contextMenu.MenuItems.Add(menuItem);

                }
                else
                {
                    MenuItem menuItem = new MenuItem("Copy");
                    menuItem.Click += CopyAction;
                    contextMenu.MenuItems.Add(menuItem);
                }

                textLibraryName.ContextMenu = contextMenu;
            }
            else
            {
                InitializeDialogText();
            }
        }



        private void buttonOKClick(object sender, EventArgs e)
        {
            this.Close();
        }



        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url;
            if (e.Link.LinkData != null)
                url = e.Link.LinkData.ToString();
            else
                url = linkLabel.Text.Substring(e.Link.Start, e.Link.Length);

            if (!url.Contains("://"))
                url = "https://" + url;

            new CrossPlatform().OpenUri(url);
            linkLabel.LinkVisited = true;
        }



        private void showLogFile()
        {

            try
            {
                string logFile = Path.Combine(creatorGUI.lipidCreator.prefixPath, "data", "lipidcreator.log");
                Console.WriteLine(logFile);
                if (File.Exists(logFile))
                {
                    using (FileStream fileStream = new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            this.textLibraryName.Text = streamReader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    throw new Exception("");
                }
            }
            catch (Exception e)
            {
                this.textLibraryName.Text = "Log file could not be opened." + e;

                if (creatorGUI.lipidCreator.openedAsExternal)
                {
                    this.textLibraryName.Text = "\n\nPlease be sure, that you installed LipidCreator in Skyline using the zip file with the name 'LipidCreator.zip'. Any renaming of the file before installation will cause a malfunction of LipidCreator. In case, please uninstall LipidCreator in Skyline, rename the zip file and re-install LipidCreator again.";
                }
                log.Error("Log file could not be opened.");
            }
        }

        protected void CopyAction(object sender, System.EventArgs e)
        {
            Clipboard.SetText(this.textLibraryName.Text);
        }

        protected void OpenAction(object sender, System.EventArgs e)
        {
            string logDir = Path.Combine(System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath), "data");
            log.Debug("Directory to open is " + logDir);
            new CrossPlatform().OpenFileOrDir(logDir);
        }
    }
}
