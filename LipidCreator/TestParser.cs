/*
MIT License

Copyright (c) 2017 Dominik Kopczynski   -   dominik.kopczynski {at} isas.de
                   Bing Peng   -   bing.peng {at} isas.de

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


namespace LipidCreator
{
    public class TestParser
    {
        [STAThread]
        public static void Main(string[] args)
        {
            LipidCreator lcf = new LipidCreator(null);
            
            ParserEventHandler peh = new ParserEventHandler(lcf);
            Parser p = new Parser(peh, "data/lipidnames.grammer", '"');
            
            /*
            p.parse("LPE 12:2(12Z,13E)");
            p.raiseEvents();
            
            Console.WriteLine(p.wordInGrammer);
            Console.WriteLine(peh.lipid != null);
            
            Environment.Exit(-1);
            */
            
            bool continueTesting = true;
            
            Console.WriteLine("testing valid lipid names:");
            if (File.Exists("data/lipidnames.txt"))
            {
                try
                {
                    using (StreamReader sr = new StreamReader("data/lipidnames.txt"))
                    {
                        string line;
                        while((line = sr.ReadLine()) != null)
                        {
                            Console.WriteLine("testing: " + line);
                            p.parse(line);
                            p.raiseEvents();
                            if (peh.lipid == null) throw new Exception("Error: lipid name '" + line + "' was not parsed.");
                            
                            peh.lipid.onlyPrecursors = 1;
                            lcf.registeredLipids.Clear();
                            lcf.registeredLipids.Add(peh.lipid);
                            lcf.assembleLipids();
                            
                            DataRow row = lcf.transitionList.Rows[0];
                            if (!line.Equals((string)row[LipidCreator.PRECURSOR_NAME]))
                            {
                                throw new Exception("Error: inserted lipid name '" + line + "' does not equal to computed name '" + row[LipidCreator.PRECURSOR_NAME] + "'.");
                            }
                        }
                    }
                }
                
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continueTesting = false;
                }
            }
            
            if (!continueTesting) return;
            Console.WriteLine("\ntesting invalid lipid names:");
            if (File.Exists("data/lipidnames-invalid.txt"))
            {
                try
                {
                    using (StreamReader sr = new StreamReader("data/lipidnames-invalid.txt"))
                    {
                        string line;
                        while((line = sr.ReadLine()) != null)
                        {
                            Console.WriteLine("testing: " + line);
                            p.parse(line);
                            if (p.wordInGrammer) throw new Exception("Error: lipid name '" + line + "' was parsed.");
                        }
                    }
                }
                
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continueTesting = false;
                }
            }
            
            if (!continueTesting) return;
            
            Console.WriteLine();
            Console.WriteLine("Test passed, no errors found");
        }
    }
}