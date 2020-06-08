/*
MIT License

Copyright (c) 2020 Dominik Kopczynski   -   dominik.kopczynski {at} isas.de
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
using System.Diagnostics;
using System.IO;
using log4net;

namespace LipidCreator
{
    /*
     * Abstraction class for cross platform operations.
     */
    public class CrossPlatform
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(CrossPlatform));

        /*
         * Opens the provided file or directory string with the system's default handler, e.g. file browser, if it exists. 
         */
        public void OpenFileOrDir(string fileOrDir)
        {
            if (File.Exists(fileOrDir))
            {
                    log.Debug("Opening file '" + Path.GetFileName(fileOrDir) + "'");
                    OpenResource(fileOrDir);
            } 
            else if (Directory.Exists(fileOrDir)) 
            {
                    log.Debug("Opening directory '" + Path.GetDirectoryName(fileOrDir) + "'");
                    OpenResource(fileOrDir);
            }
            else
            {
                log.Error("Can not open file or directory: '" + fileOrDir + "'! Path does not exist!");
            }
        }

        /*
         * Opens the provided uri string with the system's default handler, e.g. the web browser, if it is prefixed with http or https, is absolute and is valid. 
         */
        public void OpenUri(string uri)
        {
            Uri uriResult;
            bool isWebUri = Uri.TryCreate(uri, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if(isWebUri)
            {
                OpenResource(uri);
            }
            else
            {
                bool isFileUri = Uri.TryCreate(uri, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeFile);
                if (isFileUri)
                {
                    OpenFileOrDir(new Uri(uri).LocalPath);
                }
                log.Error("Can not open URI: " + uri+"! Not a valid URI!");
            }
        }

        /*
         * Opens the provided resource with the system's default handler. Exceptions are logged on Error level.
         */
        void OpenResource(string resource)
        {
            try
            {
                int p = (int)Environment.OSVersion.Platform;
                if ((p == 4) || (p == 6) || (p == 128))
                {
                    log.Debug("Running on Linux");
                    string openCmd = "/usr/bin/xdg-open";
                    var startInfo = new ProcessStartInfo()
                    {
                        FileName = openCmd,
                        Arguments = "\"" +resource+ "\"",
                        UseShellExecute = false
                    };
                    Process process = Process.Start(startInfo);
                    process.WaitForExit();
                    log.Debug("Finished starting process '" + openCmd + " " + resource + "' with code " + process.ExitCode);
                }
                else
                {
                    log.Debug("Running on Windows");
                    Process process = Process.Start(resource);
                    process.WaitForExit();
                    log.Debug("Finished starting process '" + resource + "'");
                }
            }
            catch (Exception exception)
            {
                log.Error("Error while opening resource '" + resource + "':", exception);
                log.Error(exception.StackTrace);
            }
        }
    }


}
