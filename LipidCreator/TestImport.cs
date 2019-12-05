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
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Globalization;
using System.Diagnostics;
using System.Security.Cryptography;

namespace LipidCreator
{
    public class TestImport
    {
   
    
        [STAThread]
        public static void Main(string[] args)
        {
            
            string prmImportFile = Path.Combine("test", "PRM.lcXML");
        
            LipidCreator lc = new LipidCreator(null);
            try 
            {
                lc.import(prmImportFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during import of PRM file: " + e.Message);
                Environment.Exit(-1);
            }
            
            lc.assembleLipids(false, new ArrayList(){false, 0});
            
            
            if (lc.transitionList.Rows.Count != 66)
            {
                Console.WriteLine("Expected number of 66 transitions in PRM import doesn't match with computed transitions: " + lc.transitionList.Rows.Count);
                Environment.Exit(-1);
            }
            
            
            
            string splashImportFile = Path.Combine("test", "ISOTOPES.lcXML");
            lc = new LipidCreator(null);
            try 
            {
                lc.import(splashImportFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during import of ISOTOPES file: " + e.Message);
                Console.WriteLine(e.StackTrace);
                Environment.Exit(-1);
            }
            
            lc.assembleLipids(false, new ArrayList(){false, 0});
            
            if (lc.precursorDataList.Count != 14)
            {
                Console.WriteLine("Expected number of 14 precursors in ISOTOPES import doesn't match with computed precursors: " + lc.precursorDataList.Count);
                Environment.Exit(-1);
            }
            
            
            
            if (lc.transitionList.Rows.Count != 14)
            {
                Console.WriteLine("Expected number of 14 transitions in ISOTOPES import doesn't match with computed transitions: " + lc.transitionList.Rows.Count);
                Environment.Exit(-1);
            }
            
            else
            {
                Console.WriteLine("Import passed");
            }
        }
    }
}