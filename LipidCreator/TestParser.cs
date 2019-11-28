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
            
            string grammarFilename = "data/goslin/Goslin.g4";
        
            LipidCreator lcf = new LipidCreator(null);
            ParserEventHandler peh = new ParserEventHandler(lcf);
            Parser p = new Parser(peh, grammarFilename, '\'');
            
            
            
            bool continueTesting = true;
            
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
                            
                            p.parse(lipidForTest);
                            if (!p.wordInGrammar)
                            {
                                throw new Exception("Error: lipid name '" + lipidForTest + "' couldn't be parsed.");
                            }
                            p.raiseEvents();
                            if (peh.lipid == null) throw new Exception("Error: lipid name '" + lipidForTest + "' couldn't be handled.");
                            
                            peh.lipid.onlyPrecursors = 1;
                            lcf.registeredLipids.Clear();
                            
                            ulong lipidHash = 0;
                            if (peh.lipid is Glycerolipid) lipidHash = ((Glycerolipid)peh.lipid).getHashCode();
                            else if (peh.lipid is Phospholipid) lipidHash = ((Phospholipid)peh.lipid).getHashCode();
                            else if (peh.lipid is Sphingolipid) lipidHash = ((Sphingolipid)peh.lipid).getHashCode();
                            else if (peh.lipid is Cholesterol) lipidHash = ((Cholesterol)peh.lipid).getHashCode();
                            else if (peh.lipid is Mediator) lipidHash = ((Mediator)peh.lipid).getHashCode();
                            else if (peh.lipid is UnsupportedLipid) lipidHash = ((UnsupportedLipid)peh.lipid).getHashCode();
                            
                            
                            if (!lcf.registeredLipidDictionary.ContainsKey(lipidHash))
                            {
                                lcf.registeredLipidDictionary.Add(lipidHash, peh.lipid);
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
                                        Console.WriteLine("found: " + (string)row[LipidCreator.PRECURSOR_NAME]);
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
                    continueTesting = false;
                }
            }
            
            if (!continueTesting) return;
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
                                if (peh.lipid != null) throw new Exception("Error: lipid name '" + line + "' was parsed.");
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
            
            Console.WriteLine();
            Console.WriteLine("Test passed, no errors found");
        }
    }
}