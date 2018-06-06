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
            Parser p = new Parser(lcf, "data/lipidnames.grammer", '"');
            
            
            
            
            p.parse("PE O 18:0a-22:6");
            p.raiseEvents();
            
            lcf.registeredLipids.Clear();
            lcf.registeredLipids.Add(p.lipid);
            lcf.assembleLipids();
            
            /*
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
                            if (p.lipid == null) throw new Exception("Error: lipid name " + line + " was not parsed.");
                            
                            p.lipid.onlyPrecursors = 1;
                            lcf.registeredLipids.Clear();
                            lcf.registeredLipids.Add(p.lipid);
                            lcf.assembleLipids();
                            
                            DataRow row = lcf.transitionList.Rows[0];
                            Console.WriteLine((string)row[LipidCreator.PRECURSOR_NAME]);
                        }
                    }
                }
                
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            
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
                            p.raiseEvents();
                            if (p.lipid != null) throw new Exception("Error: lipid name " + line + " was parsed.");
                        }
                    }
                }
                
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            */
        }
    }
}