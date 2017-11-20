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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Data.SQLite;

namespace LipidCreator
{
    [Serializable]
    public class Lipid
    {
        public string className;
        public Dictionary<String, ArrayList> MS2Fragments;
        public Dictionary<String, String> pathsToFullImage;
        public Dictionary<String, bool> adducts;
        public bool representativeFA;
    
        public Lipid(){
            adducts = new Dictionary<String, bool>();
            adducts.Add("+H", false);
            adducts.Add("+2H", false);
            adducts.Add("+NH4", false);
            adducts.Add("-H", true);
            adducts.Add("-2H", false);
            adducts.Add("+HCOO", false);
            adducts.Add("+CH3COO", false);
            MS2Fragments = new Dictionary<String, ArrayList>();
            pathsToFullImage = new Dictionary<String, String>();
            representativeFA = false;
        }
        
        public virtual void addSpectrum(SQLiteCommand command, Dictionary<String, DataTable> headGroupsTable, HashSet<String> usedKeys)
        {
        }
        
        public virtual void addLipids(DataTable dt, DataTable allLipidsUnique, Dictionary<String, DataTable> headGroupsTable, Dictionary<String, Dictionary<String, bool>> headgroupAdductRestrictions, HashSet<String> usedKeys, HashSet<String> replicates)
        {
        }
        
        public virtual string serialize()
        {
            string xml = "<className>" + className + "</className>\n";
            xml += "<representativeFA>" + (representativeFA ? 1 : 0) + "</representativeFA>\n";
            foreach (KeyValuePair<String, bool> item in adducts)
            {
                xml += "<adduct type=\"" + item.Key + "\">" + (item.Value ? 1 : 0) + "</adduct>\n";
            }
            
            foreach (KeyValuePair<String, ArrayList> item in MS2Fragments)
            {
                xml += "<MS2FragmentGroup name=\"" + item.Key + "\">\n";
                foreach (MS2Fragment fragment in item.Value)
                {
                    xml += fragment.serialize();
                }
                xml += "</MS2FragmentGroup>\n";
            }
            return xml;
        }
        
        public Lipid(Lipid copy)
        {
            adducts = new Dictionary<String, bool>();
            adducts.Add("+H", copy.adducts["+H"]);
            adducts.Add("+2H", copy.adducts["+2H"]);
            adducts.Add("+NH4", copy.adducts["+NH4"]);
            adducts.Add("-H", copy.adducts["-H"]);
            adducts.Add("-2H", copy.adducts["-2H"]);
            adducts.Add("+HCOO", copy.adducts["+HCOO"]);
            adducts.Add("+CH3COO", copy.adducts["+CH3COO"]);
            className = copy.className;
            representativeFA = copy.representativeFA;
            MS2Fragments = new Dictionary<String, ArrayList>();
            pathsToFullImage = new Dictionary<String, String>();
            foreach (KeyValuePair<String, String> item in copy.pathsToFullImage)
            {
                pathsToFullImage.Add(item.Key, item.Value);
            }
            
            foreach (KeyValuePair<String, ArrayList> item in copy.MS2Fragments)
            {
                MS2Fragments.Add(item.Key, new ArrayList());
                foreach (MS2Fragment fragment in item.Value)
                {
                    MS2Fragments[item.Key].Add(new MS2Fragment(fragment));
                }
            }
        }
        
        
        
        public void subtractAdduct(DataTable atomsCount, String adduct)
        {
            switch (adduct)
            {
                             
                case "+NH4":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] - 3;
                    atomsCount.Rows[3]["Count"] = (int)atomsCount.Rows[3]["Count"] - 1;
                    break;
                case "+HCOO":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] - 2;
                    atomsCount.Rows[0]["Count"] = (int)atomsCount.Rows[0]["Count"] - 1;
                    atomsCount.Rows[2]["Count"] = (int)atomsCount.Rows[2]["Count"] - 2;
                    break;
                case "+CH3COO":
                    atomsCount.Rows[0]["Count"] = (int)atomsCount.Rows[0]["Count"] - 2;
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] - 4;
                    atomsCount.Rows[2]["Count"] = (int)atomsCount.Rows[2]["Count"] - 2;
                    break;
            }
        }
        
        public int getChargeAndAddAdduct(DataTable atomsCount, String adduct)
        {
            int charge = 0;
            switch (adduct)
            {
                                                                                                
                case "+H":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] + 1;
                    charge = 1;
                    break;
                case "+2H":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] + 2;
                    charge = 2;
                    break;
                case "+NH4":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] + 4;
                    atomsCount.Rows[3]["Count"] = (int)atomsCount.Rows[3]["Count"] + 1;
                    charge = 1;
                    break;
                case "-H":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] - 1;
                    charge = -1;
                    break;
                case "-2H":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] - 2;
                    charge = -2;
                    break;
                case "+HCOO":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] + 1;
                    atomsCount.Rows[0]["Count"] = (int)atomsCount.Rows[0]["Count"] + 1;
                    atomsCount.Rows[2]["Count"] = (int)atomsCount.Rows[2]["Count"] + 2;
                    charge = -1;
                    break;
                case "+CH3COO":
                    atomsCount.Rows[0]["Count"] = (int)atomsCount.Rows[0]["Count"] + 2;
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] + 3;
                    atomsCount.Rows[2]["Count"] = (int)atomsCount.Rows[2]["Count"] + 2;
                    charge = -1;
                    break;
            }
            return charge;
        }
        
        
        public virtual void import(XElement node)
        {
            switch (node.Name.ToString())
            {
                case "className":
                    className = node.Value.ToString();
                    break;
                    
                case "representativeFA":
                    representativeFA = node.Value == "1";
                    break;
                    
                case "adduct":
                    string adductKey = node.Attribute("type").Value.ToString();
                    adducts[adductKey] = node.Value == "1";
                    break;
                    
                case "MS2FragmentGroup":
                    string fragmentKey = node.Attribute("name").Value.ToString();
                    foreach (XElement fragment in node.Elements()){
                        MS2Fragment ms2Fragment = new MS2Fragment();
                        ms2Fragment.import(fragment);
                        MS2Fragments[fragmentKey].Add(ms2Fragment);
                    }
                    break;
                    
                default:
                    Console.WriteLine("Error: " + node.Name.ToString());
                    throw new Exception();
            }
        }
    }
}