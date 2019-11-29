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
    public class TestParser
    {
   
    
        static RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
    
        public static LinkedList<string> assembleLipidname(Dictionary<int, ArrayList> rules, Dictionary<int, string> terminals, LinkedList<string> lipidname, int rule, int prevRandom)
        {
            int p = -2;
            byte[] byteArray = new byte[4];
            do {
                provider.GetBytes(byteArray);
                p = (int)(BitConverter.ToUInt32(byteArray, 0) % rules[rule].Count);
            }
            while (p == prevRandom && prevRandom != 0);
            
            
            
            foreach (int r in (ArrayList)rules[rule][p])
            {
                if (terminals.ContainsKey(r)) lipidname.AddLast(terminals[r]);
                else lipidname = assembleLipidname(rules, terminals, lipidname, r, p);
            }
            return lipidname;
        }
    
        [STAThread]
        public static void Main(string[] args)
        {
            
            string grammarFilename = Path.Combine("data", "goslin", "Goslin.g4");
            string grammarFragmentFilename = Path.Combine("data", "goslin", "Goslin-Fragments.g4");
        
            LipidCreator lcf = new LipidCreator(null);
            ParserEventHandler peh = new ParserEventHandler(lcf);
            Parser p = new Parser(peh, grammarFilename, '\'');
            
            ParserEventHandlerFragment pehf = new ParserEventHandlerFragment(lcf);
            Parser fp = new Parser(pehf, grammarFragmentFilename, '\'');
            
            
            
            Console.WriteLine("testing valid lipid names:");
            if (File.Exists("test/lipidnames.txt"))
            {
                try
                {
                    using (StreamReader sr = new StreamReader("test/lipidnames.txt"))
                    {
                        string line;
                        while((line = sr.ReadLine()) != null)
                        {
                            
                            if (line.IndexOf("#") > -1) line = line.Substring(0, line.IndexOf("#"));
                            if (line.Length < 2 || line.Equals("")) continue;
                            
                            string[] tokens = LipidCreator.parseLine(line, ',', '"');
                            if (tokens.Length < 2 || tokens[1].Equals(""))
                            {
                                Console.WriteLine("Error, corrupted line: " + line);
                            }
                            
                            string lipidForTest = tokens[0];
                            string expectedLipid = tokens[1];
                            Console.WriteLine("testing: " + lipidForTest);
                            
                            peh.resetLipidBuilder(null);
                            p.parse(lipidForTest);
                            Lipid resultlipid = null;
                            bool exec = true;
                            
                            // testing with precursor parser
                            if (!p.wordInGrammar)
                            {
                                exec = false;
                            }
                            if (exec)
                            {
                                p.raiseEvents();
                            }
                            if (peh.lipid != null)
                            {
                                resultlipid = peh.lipid;
                            }
                            else
                            {
                                // testing with fragment parser
                                pehf.resetLipidBuilder(null);
                                fp.parse(lipidForTest);
                                if (!fp.wordInGrammar)
                                {
                                    throw new Exception("Error: lipid name '" + lipidForTest + "' couldn't be parsed.");
                                }
                                fp.raiseEvents();
                                if (pehf.lipid != null)
                                {
                                    resultlipid = pehf.lipid;
                                }
                                else 
                                {
                                    throw new Exception("Error: lipid name '" + lipidForTest + "' couldn't be handled.");
                                }
                            }
                            
                            
                                
                            
                            resultlipid.onlyPrecursors = 1;
                            lcf.registeredLipids.Clear();
                            
                            ulong lipidHash = 0;
                            if (resultlipid is Glycerolipid) lipidHash = ((Glycerolipid)resultlipid).getHashCode();
                            else if (resultlipid is Phospholipid) lipidHash = ((Phospholipid)resultlipid).getHashCode();
                            else if (resultlipid is Sphingolipid) lipidHash = ((Sphingolipid)resultlipid).getHashCode();
                            else if (resultlipid is Cholesterol) lipidHash = ((Cholesterol)resultlipid).getHashCode();
                            else if (resultlipid is Mediator) lipidHash = ((Mediator)resultlipid).getHashCode();
                            else if (resultlipid is UnsupportedLipid) lipidHash = ((UnsupportedLipid)resultlipid).getHashCode();
                            
                            
                            if (!lcf.registeredLipidDictionary.ContainsKey(lipidHash))
                            {
                                lcf.registeredLipidDictionary.Add(lipidHash, resultlipid);
                                lcf.registeredLipids.Add(lipidHash);
                                lcf.assembleLipids(false, new ArrayList(){false, 0});
                                
                                if (lcf.transitionList.Rows.Count < 1)
                                {
                                    throw new Exception("Error: inserted lipid name '" + lipidForTest + "' could not be created");
                                }
                                else if (lcf.transitionList.Rows.Count == 1)
                                {
                                    DataRow row = lcf.transitionList.Rows[0];
                                    if (!expectedLipid.Equals((string)row[LipidCreator.PRECURSOR_NAME]))
                                    {
                                        throw new Exception("Error: inserted lipid name '" + lipidForTest + "' does not match with computed name '" + row[LipidCreator.PRECURSOR_NAME] + "', expected '" + expectedLipid + "'.");
                                    }
                                }
                                else
                                {
                                    bool found = false;
                                    foreach (DataRow row in lcf.transitionList.Rows)
                                    {
                                        found |= expectedLipid.Equals((string)row[LipidCreator.PRECURSOR_NAME]);
                                    }
                                    if (!found)
                                    {   
                                        throw new Exception("Error: inserted lipid name '" + lipidForTest + "' does not match with any computed name, expected '" + expectedLipid + "'.");
                                    
                                    }
                                }
                            }
                        }
                    }
                }
                
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
            
            Console.WriteLine("\ntesting invalid lipid names:");
            if (File.Exists("test/lipidnames-invalid.txt"))
            {
                try
                {
                    using (StreamReader sr = new StreamReader("test/lipidnames-invalid.txt"))
                    {
                        string line;
                        while((line = sr.ReadLine()) != null)
                        {
                            Console.WriteLine("testing: " + line);
                            p.parse(line);
                            if (p.wordInGrammar)
                            {
                                p.raiseEvents();
                                if (peh.lipid == null)
                                {
                                    fp.parse(line);
                                    if (fp.wordInGrammar)
                                    {
                                        fp.raiseEvents();
                                        if (pehf.lipid != null) throw new Exception("Error: lipid name '" + line + "' was parsed by fragment parser.");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Error: lipid name '" + line + "' was parsed by precursor parser.");
                                }
                            }
                        }
                    }
                }
                
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
            
            Console.WriteLine();
            Console.WriteLine("Test passed, no errors found");
        }
    }
}