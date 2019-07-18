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


namespace LipidCreator
{
    public class TestTransitionList
    {
        public static void Assert(bool condition, string message = "")
        {
            if (!condition)
            {
                throw new Exception("Assert failed: " + message);
            }
        }
        
        public static void Assert(int i1, int i2, string message = "")
        {
            if (i1 != i2)
            {
                throw new Exception("Assert failed: " + message + i1 + " != " + i2);
            }
        }
        
        public static void Assert(double d1, double d2, string message = "")
        {
            if (Math.Abs(d1 - d2) > 2e-4)
            {
                throw new Exception("Assert failed: " + message + d1 + " != " + d2);
            }
        }
        
        public static void Assert(string s1, string s2, string message = "")
        {
            if (!s1.Equals(s2))
            {
                throw new Exception("Assert failed: " + message + s1 + " != " + s2);
            }
        }
    
        [STAThread]
        public static void Main(string[] args)
        {
            LipidCreator lcf = new LipidCreator(null);
            ArrayList unitTestData = new ArrayList();
            
            string grammarFilename = "data/goslin/Goslin.g4";
            char quote = '\'';
            ParserEventHandler parserEventHandler = new ParserEventHandler(lcf);
            Parser parser = new Parser(parserEventHandler, grammarFilename, quote);
            
            try {
                int lineCounter = 1;
                string unitTestFile = "test/unit-test-transition-list.csv";
                
                
                if (File.Exists(unitTestFile))
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(unitTestFile))
                        {
                            String line = sr.ReadLine(); // omit titles
                            while((line = sr.ReadLine()) != null)
                            {
                                lineCounter++;
                                if (line.Length < 2) continue;
                                if (line[0] == '#') continue;
                                
                                string[] tokens = LipidCreator.parseLine(line);
                                unitTestData.Add(tokens);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("The file '" + unitTestFile + "' in line '" + lineCounter + "' could not be read:");
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Error: file '" + unitTestFile + "' does not exist or can not be opened.");
                }
            
                // loop over each row of unit test file
                foreach (string[] unitTestRow in unitTestData)
                {
                
                    Console.WriteLine("Testing: " + String.Join(" / ", unitTestRow));
                    
                    
                    // parse species identifier
                    Lipid lipid = null;
                    parser.parse(unitTestRow[1]);
                    if (parser.wordInGrammar)
                    {
                        parser.raiseEvents();
                        if (parserEventHandler.lipid != null) lipid = parserEventHandler.lipid;
                    }
                    if (lipid == null) throw new Exception("Error: '" + unitTestRow[1] + "' could not be parsed");
                    
                    
                    // extract headgroup name
                    string headgroup = unitTestRow[1];
                    // exception for PC -_-
                    if (headgroup.IndexOf("PC O") >= 0) headgroup = headgroup.Replace("PC O", "PC O-" + (headgroup.IndexOf("a") > 0 ? "a" : "p"));
                    else if (headgroup.IndexOf("PE O") >= 0) headgroup = headgroup.Replace("PE O", "PE O-" + (headgroup.IndexOf("a") > 0 ? "a" : "p"));
                    
                    string[] speciesToken = headgroup.Split(new char[]{' '});
                    headgroup = speciesToken[0];
                    if (speciesToken.Length > 2)
                    {
                        headgroup = speciesToken[0] + " " + speciesToken[1];
                        if (!lcf.headgroups.ContainsKey(headgroup))
                        {
                            headgroup = speciesToken[0];
                            if (!lcf.headgroups.ContainsKey(headgroup)) throw new Exception("Error: headgroup could not be determined");
                        }
                    }
                    
                    
                    // subtracting adduct from precursor
                    string adduct = unitTestRow[3];
                    adduct = adduct.Substring(2, adduct.Length - 2);
                    adduct = adduct.Split(new char[]{']'})[0];

                    if (!lipid.adducts.ContainsKey(adduct)) throw new Exception("Error: unknown precursor adduct '" + unitTestRow[3] + "'");
                    
                    // setting fragment in either positive or negative mode
                    string[] keys = new string[lipid.adducts.Keys.Count];
                    lipid.adducts.Keys.CopyTo(keys, 0);
                    foreach (string key in keys) lipid.adducts[key] = false;
                    lipid.adducts[adduct] = true;
                    
                    if (!lipid.adducts[adduct] || !lcf.headgroups[headgroup].adductRestrictions[adduct]) throw new Exception("Error: combination '" + headgroup + "' and '" + unitTestRow[3] + "' are not valid");
                    
                    
                    ulong lipidHash = 0;
                    if (lipid is Glycerolipid) lipidHash = ((Glycerolipid)lipid).getHashCode();
                    else if (lipid is Phospholipid) lipidHash = ((Phospholipid)lipid).getHashCode();
                    else if (lipid is Sphingolipid) lipidHash = ((Sphingolipid)lipid).getHashCode();
                    else if (lipid is Cholesterol) lipidHash = ((Cholesterol)lipid).getHashCode();
                    else if (lipid is Mediator) lipidHash = ((Mediator)lipid).getHashCode();
                    else if (lipid is UnsupportedLipid) lipidHash = ((UnsupportedLipid)lipid).getHashCode();
                    
                    
                    // create transition
                    lcf.registeredLipids.Clear();
                    lcf.registeredLipidDictionary.Clear();
                    lcf.registeredLipidDictionary.Add(lipidHash, lipid);
                    lcf.registeredLipids.Add(lipidHash);
                    lcf.assembleLipids(false, new ArrayList(){false, 0});
                    
                    // resolve the [adduct] wildcards if present
                    string fragmentName = unitTestRow[6];
                    if (fragmentName.IndexOf("[adduct]") > -1) fragmentName = fragmentName.Replace("[adduct]", adduct);
                    
                    if (lcf.transitionList.Rows.Count == 0) throw new Exception("Error: no fragment computed.");
                    int cnt = 0;
                    foreach (DataRow row in lcf.transitionList.Rows)
                    {
                        if (row[LipidCreator.PRODUCT_NAME].Equals(fragmentName) && row[LipidCreator.PRODUCT_ADDUCT].Equals(unitTestRow[8]))
                        {
                            // precursor
                            Assert((string)row[LipidCreator.MOLECULE_LIST_NAME], unitTestRow[0], "class: ");
                            Assert((string)row[LipidCreator.PRECURSOR_NAME], unitTestRow[1], "precursor name: ");
                            Assert((string)row[LipidCreator.PRECURSOR_NEUTRAL_FORMULA], unitTestRow[2], "precursor formula: ");
                            Assert((string)row[LipidCreator.PRECURSOR_ADDUCT], unitTestRow[3], "precursor adduct: ");
                            Assert(Convert.ToDouble(row[LipidCreator.PRECURSOR_MZ], CultureInfo.InvariantCulture), Convert.ToDouble(unitTestRow[4], CultureInfo.InvariantCulture), "precursor mass: ");
                            Assert(Convert.ToInt32(row[LipidCreator.PRECURSOR_CHARGE]), Convert.ToInt32(unitTestRow[5]), "precursor charge: ");
                            
                            // product
                            Assert((string)row[LipidCreator.PRODUCT_NAME], fragmentName, "product name: ");
                            Assert((string)row[LipidCreator.PRODUCT_NEUTRAL_FORMULA], unitTestRow[7], "product formula: ");
                            Assert((string)row[LipidCreator.PRODUCT_ADDUCT], unitTestRow[8], "product adduct: ");
                            Assert(Convert.ToDouble(row[LipidCreator.PRODUCT_MZ], CultureInfo.InvariantCulture), Convert.ToDouble(unitTestRow[9], CultureInfo.InvariantCulture), "product mass: ");
                            Assert(Convert.ToInt32(row[LipidCreator.PRODUCT_CHARGE]), Convert.ToInt32(unitTestRow[10]), "product charge: ");
                            ++cnt;
                        }
                    }
                    if (cnt == 0) throw new Exception("Error: fragment '" + fragmentName + "' with adduct '" + unitTestRow[8] + "' not found.");
                  
                    
                }
                Console.WriteLine("Test passed, no errors found");  
                    
            }
            
            
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine();
            }
        }
    }
}
