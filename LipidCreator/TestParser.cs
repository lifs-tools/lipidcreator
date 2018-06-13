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
        
        
            /*
            
            TestParserEventHandler tpe = new TestParserEventHandler();
            Parser ppp = new Parser(tpe, "data/grammer.grammer", '\'');
            ppp.parse("Mediator = \"10-HDoHE\" | \"11-HDoHE\" | \"11-HETE\" | \"11,12-DHET\" | \"11(12)-EET\"| \"12-HEPE\" | \"12-HETE\" | \"12-HHTrE\" | \"12-OxoETE\" | \"12(13)-EpOME\" | \"13-HODE\" | \"13-HOTrE\" | \"14,15-DHET\" | \"14(15)-EET\" | \"14(15)-EpETE\" | \"15-HEPE\" | \"15-HETE\" | \"15d-PGJ2\" | \"16-HDoHE\" | \"16-HETE\" | \"18-HEPE\" | \"5-HEPE\" | \"5-HETE\" | \"5-HpETE\" | \"5-OxoETE\" | \"5,12-DiHETE\" | \"5,6-DiHETE\" | \"5,6,15-LXA4\" | \"5(6)-EET\" | \"8-HDoHE\" | \"8-HETE\" | \"8,9-DHET\" | \"8(9)-EET\" | \"9-HEPE\" | \"9-HETE\" | \"9-HODE\" | \"9-HOTrE\" | \"9(10)-EpOME\" | \"AA\" | \"alpha-LA\" | \"DHA\" | \"EPA\" | \"Linoleic acid\" | \"LTB4\" | \"LTC4\" | \"LTD4\" | \"Maresin 1\" | \"Palmitic acid\" | \"PGB2\" | \"PGD2\" | \"PGE2\" | \"PGF2alpha\" | \"PGI2\" | \"Resolvin D1\" | \"Resolvin D2\" | \"Resolvin D3\" | \"Resolvin D5\" | \"tetranor-12-HETE\" | \"TXB1\" | \"TXB2\" | \"TXB3\"");
            
            //ppp.parse("Mediator = \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\" | \"-\" | \"+\"");
            Console.WriteLine(ppp.wordInGrammer);
            Environment.Exit(0);
            
            
            */
        
        
            string grammerFilename = "data/lipidnames.grammer";
            char quote = '"';
            
            TestParserEventHandler tpeh = new TestParserEventHandler();
            Parser pp = new Parser(tpeh, grammerFilename, quote);
                        
            
            //pp.parse("PE 16:2-18:3;1");
            //Environment.Exit(0);
            
            
            // creating a random lipid name generator
            if (File.Exists(grammerFilename))
            {
                int lineCounter = 0;
                int ruleNum = 1;
                Dictionary<int, ArrayList> rules = new Dictionary<int, ArrayList>();
                Dictionary<int, string> terminals = new Dictionary<int, string>();
                Dictionary<string, int> ruleToNT = new Dictionary<string, int>();
                using (StreamReader sr = new StreamReader(grammerFilename))
                {
                    string line;
                    while((line = sr.ReadLine()) != null)
                    {
                        lineCounter++;
                        // skip empty lines and comments
                        if (line.Length < 1) continue;
                        if (line.IndexOf("#") > -1) line = line.Substring(0, line.IndexOf("#"));
                        if (line.Length < 1) continue;
                        line = Parser.strip(line, ' ');
                        if (line.Length < 2) continue;
                        
                        ArrayList tokens_level_1 = new ArrayList();
                        foreach (string t in Parser.splitString(line, '=', quote)) tokens_level_1.Add(Parser.strip(t, ' '));
                        if (tokens_level_1.Count != 2) throw new Exception("Error: corrupted token in grammer");

                        string rule = (string)tokens_level_1[0];
                        
                        ArrayList products = new ArrayList();
                        foreach (string pt in Parser.splitString((string)tokens_level_1[1], '|', quote)) products.Add(Parser.strip(pt, ' '));
                        
                        
                        if (!ruleToNT.ContainsKey(rule))
                        {
                            ruleToNT.Add(rule, ruleNum++);
                        }
                        int currentRule = ruleToNT[rule];
                        
                        if (!rules.ContainsKey(currentRule)) rules.Add(currentRule, new ArrayList());
                        
                        foreach (string product in products)
                        {
                            ArrayList productRules = new ArrayList();
                            rules[currentRule].Add(productRules);
                            
                            LinkedList<string> nonTerminals = new LinkedList<string>();
                            foreach (string NT in Parser.splitString(product, ' ', quote)) nonTerminals.AddLast(Parser.strip(NT, ' '));
        
                            foreach (string nonTerminal in nonTerminals)
                            {
                                int nextRule = ruleNum++;
                                if (Parser.isTerminal(nonTerminal, quote))
                                {
                                    nextRule = ruleNum++;
                                    terminals.Add(nextRule, Parser.strip(nonTerminal, quote));
                                }
                                else
                                {
                                    if (!ruleToNT.ContainsKey(nonTerminal))
                                    {
                                        ruleToNT[nonTerminal] = ruleNum++;
                                    }
                                    nextRule = ruleToNT[nonTerminal];
                                }
                                productRules.Add(nextRule);
                            }
                        }
                    }
                }
                
                
                Console.WriteLine("testing generated lipid names:");
                int ii = 0;
                while (ii++ < 100){
                    LinkedList<string> lipidnameList = assembleLipidname(rules, terminals, new LinkedList<string>(), 1, -1);
                    
                    string lipidname = String.Join("", lipidnameList);
                    
                    Console.WriteLine(lipidname);
                    if (lipidname.Length > 100) continue;
                    pp.parse(lipidname);
                    pp.raiseEvents();
                    string parsedName = ((TestParserEventHandler)tpeh).lipidname;
                    if (!lipidname.Equals(parsedName)) throw new Exception("Error, something went wrong: " + parsedName);
                }
            }
            else
            {
                throw new Exception("Error: file '" + grammerFilename + "' does not exist or can not be opened.");
            }
        
            Console.WriteLine("");
        
            LipidCreator lcf = new LipidCreator(null);
            ParserEventHandler peh = new ParserEventHandler(lcf);
            Parser p = new Parser(peh, grammerFilename, quote);
            
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