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
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System.Data.SQLite;
using Ionic.Zlib;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SkylineTool;

namespace LipidCreator
{    
    [Serializable]
    public class FattyAcid : IComparable<FattyAcid>
    {
        public int length;
        public int db;
        public int hydroxyl;
        public String suffix;
        public DataTable atomsCount;
        public FattyAcid(int l, int db, int hydro){
        
        }
        

        public FattyAcid(int l, int db, int hydro, bool isLCB = false)
        {
            length = l;
            this.db = db;
            hydroxyl = hydro;
            suffix = "";
            atomsCount = MS2Fragment.createEmptyElementTable();
            if (l > 0 || db > 0){
                if (!isLCB){
                    atomsCount.Rows[0]["Count"] = l; // C
                    atomsCount.Rows[1]["Count"] = 2 * l - 1 - 2 * db; // H
                    atomsCount.Rows[2]["Count"] = 1 + hydro; // O
                }
                else {
                    // long chain base
                    atomsCount.Rows[0]["Count"] = l; // C
                    atomsCount.Rows[1]["Count"] = (2 * (l - db) + 1); // H
                    atomsCount.Rows[2]["Count"] = hydro; // O
                    atomsCount.Rows[3]["Count"] = 1; // N
                }
            }
        }

        
        public FattyAcid(int l, int db, int hydro, String suffix)
        {
            length = l;
            this.db = db;
            hydroxyl = hydro;
            this.suffix = (suffix.Length > 2) ? suffix.Substring(2, 1) : "";
            atomsCount = MS2Fragment.createEmptyElementTable();
            if (l > 0 || db > 0){
                atomsCount.Rows[0]["Count"] = l; // C
                switch(this.suffix)
                {
                    case "":
                        atomsCount.Rows[1]["Count"] = 2 * l - 1 - 2 * db; // H
                        atomsCount.Rows[2]["Count"] = 1 + hydro; // O
                        break;
                    case "p":
                        atomsCount.Rows[1]["Count"] = 2 * l - 1 - 2 * db + 2; // H
                        atomsCount.Rows[2]["Count"] = hydro; // O
                        break;
                    case "e":
                        atomsCount.Rows[1]["Count"] = (l + 1) * 2 - 1 - 2 * db; // H
                        atomsCount.Rows[2]["Count"] = hydro; // O
                        break;
                }
            }
        }
        
        public FattyAcid(FattyAcid copy)
        {
            length = copy.length;
            db = copy.length;
            hydroxyl = copy.hydroxyl;
            suffix = copy.suffix;
            atomsCount = MS2Fragment.createEmptyElementTable(copy.atomsCount);
        }

        public int CompareTo(FattyAcid other)
        {
            if (length != other.length)
            {
                return length - other.length;
            }
            else if (db != other.db)
            {
                return db - other.db;
            }
            else if (suffix.Length != other.suffix.Length)
            {
                return suffix.Length - other.suffix.Length;
            }
            else if (suffix.Length > 0 && suffix[0] != other.suffix[0])
            {
                return suffix[0] - other.suffix[0];
            }
            return 0;
        }
    }
    
    
    [Serializable]
    public class FattyAcidComparer : EqualityComparer<FattyAcid>
    {
        public override int GetHashCode(FattyAcid obj)
        {
            return obj.length * 31 * 31 * 6 + obj.db * 31 + obj.hydroxyl;
        }
        
        public override bool Equals(FattyAcid obj, FattyAcid obj2)
        { 
            return (obj.length == obj2.length) && (obj.db == obj2.db) && (obj.suffix == obj2.suffix) && (obj.hydroxyl == obj2.hydroxyl);
        }
    }

    [Serializable]
    public class fattyAcidGroup
    {
        public int chainType; // 0 = no restriction, 1 = odd carbon number, 2 = even carbon number
        public String lengthInfo;
        public String dbInfo;
        public String hydroxylInfo;
        public Dictionary<String, bool> faTypes;
        public HashSet<int> carbonCounts;
        public HashSet<int> doubleBondCounts;
        public HashSet<int> hydroxylCounts;
    
        public fattyAcidGroup()
        {
            chainType = 0;
            lengthInfo = "12-15";
            dbInfo = "0";
            hydroxylInfo = "0";
            faTypes = new Dictionary<String, bool>();
            faTypes.Add("FA", true);
            faTypes.Add("FAp", false);
            faTypes.Add("FAe", false);
            faTypes.Add("FAx", false);  // no fatty acid dummy
            carbonCounts = new HashSet<int>();
            doubleBondCounts = new HashSet<int>();
            hydroxylCounts = new HashSet<int>();
        }
        
        public fattyAcidGroup(fattyAcidGroup copy)
        {
            chainType = copy.chainType;
            lengthInfo = copy.lengthInfo;
            dbInfo = copy.dbInfo;
            hydroxylInfo = copy.hydroxylInfo;
            faTypes = new Dictionary<String, bool>();
            faTypes.Add("FA", copy.faTypes["FA"]);
            faTypes.Add("FAp", copy.faTypes["FAp"]);
            faTypes.Add("FAe", copy.faTypes["FAe"]);
            faTypes.Add("FAx", copy.faTypes["FAx"]);  // no fatty acid dummy
            carbonCounts = new HashSet<int>();
            foreach (int l in copy.carbonCounts)
            {
                carbonCounts.Add(l);
            }
            doubleBondCounts = new HashSet<int>();
            foreach (int d in copy.doubleBondCounts)
            {
                doubleBondCounts.Add(d);
            }
            hydroxylCounts = new HashSet<int>();
            foreach (int d in copy.hydroxylCounts)
            {
                hydroxylCounts.Add(d);
            }
        }
        
        public void import(XElement node)
        {
            chainType = Convert.ToInt32(node.Attribute("chainType").Value);
            lengthInfo = node.Attribute("lengthInfo").Value;
            dbInfo = node.Attribute("dbInfo").Value;
            hydroxylInfo = node.Attribute("hydroxylInfo").Value;
            
            carbonCounts.Clear();
            doubleBondCounts.Clear();
            hydroxylCounts.Clear();
        
            foreach(XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "faType":
                        faTypes[child.Attribute("type").Value.ToString()] = child.Value == "1";
                        break;
                        
                    case "length":
                        carbonCounts.Add(Convert.ToInt32(child.Value.ToString()));
                        break;
                        
                    case "doublebond":
                        doubleBondCounts.Add(Convert.ToInt32(child.Value.ToString()));
                        break;
                        
                    case "hydroxyl":
                        hydroxylCounts.Add(Convert.ToInt32(child.Value.ToString()));
                        break;
                        
                    default:
                        throw new Exception();
                }
            }
        }
        
        public string serialize()
        {
            string xml = "<fattyAcidGroup";
            xml += " chainType=\"" + chainType + "\"";
            xml += " lengthInfo=\"" + lengthInfo + "\"";
            xml += " dbInfo=\"" + dbInfo + "\"";
            xml += " hydroxylInfo=\"" + hydroxylInfo + "\">\n";
            foreach (KeyValuePair<String, bool> item in faTypes)
            {
                xml += "<faType";
                xml += " type=\"" + item.Key + "\">";
                xml += (item.Value ? 1 : 0);
                xml += "</faType>\n";
            }
            foreach (int len in carbonCounts)
            {
                xml += "<length>" + len + "</length>\n";
            }
            foreach (int db in doubleBondCounts)
            {
                xml += "<doublebond>" + db + "</doublebond>\n";
            }
            foreach (int hydroxyl in hydroxylCounts)
            {
                xml += "<hydroxyl>" + hydroxyl + "</hydroxyl>\n";
            }
            xml += "</fattyAcidGroup>\n";
            return xml;
        }
        
        
        
        public bool anyFAChecked()
        {
            return faTypes["FA"] || faTypes["FAp"] || faTypes["FAe"];
        }
    }

    [Serializable]
    public class MS2Fragment
    {
        public String fragmentName;
        public int fragmentCharge;
        public String fragmentFile;
        public bool fragmentSelected;
        public DataTable fragmentElements;
        public ArrayList fragmentBase;
        public HashSet<string> restrictions;
        public double intensity;
    
        public string serialize()
        {
            string xml = "<MS2Fragment";
            xml += " fragmentName=\"" + fragmentName + "\"";
            xml += " fragmentCharge=\"" + fragmentCharge + "\"";
            xml += " fragmentFile=\"" + fragmentFile + "\"";
            xml += " intensity=\"" + intensity + "\"";
            xml += " fragmentSelected=\"" + (fragmentSelected ? 1 : 0) + "\">\n";
            foreach (string restriction in restrictions)
            {
                xml += "<restriction>" + restriction + "</restriction>\n";
            }
            foreach (string fbase in fragmentBase)
            {
                xml += "<fragmentBase>" + fbase + "</fragmentBase>\n";
            }
            foreach (DataRow dr in fragmentElements.Rows)
            {
                xml += "<Element type=\"" + dr["Shortcut"] + "\">" + dr["Count"] + "</Element>\n";
            }
            xml += "</MS2Fragment>\n";
            return xml;
        }
        
        public void import(XElement node)
        {
            Dictionary<String, int> ElementPositions = new Dictionary<String, int>(){
                {"C", 0},
                {"H", 1},
                {"O", 2},
                {"N", 3},
                {"P", 4},
                {"S", 5},
                {"Na", 6}
            };
        
            restrictions.Clear();
            fragmentBase.Clear();
        
            fragmentName = node.Attribute("fragmentName").Value.ToString();
            fragmentCharge = Convert.ToInt32(node.Attribute("fragmentCharge").Value.ToString());
            fragmentFile = node.Attribute("fragmentFile").Value.ToString();
            intensity = Convert.ToInt32(node.Attribute("intensity").Value.ToString());
            fragmentSelected = node.Attribute("fragmentSelected").Value.ToString() == "1";
            
            
            foreach(XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "restriction":
                        restrictions.Add(child.Value.ToString());
                        break;
                        
                    case "fragmentBase":
                        fragmentBase.Add(child.Value.ToString());
                        break;
                        
                    case "Element":
                        fragmentElements.Rows[ElementPositions[child.Attribute("type").Value.ToString()]]["Count"] = child.Value.ToString();
                        break;
                        
                    default:
                        throw new Exception();
                }
            }
        }
    
        public static void addCounts(DataTable dt1, DataTable dt2)
        {
            String count = "Count";
            dt1.Rows[0][count] = (int)dt1.Rows[0][count] + (int)dt2.Rows[0][count];  // carbon
            dt1.Rows[1][count] = (int)dt1.Rows[1][count] + (int)dt2.Rows[1][count];  // hydrogen
            dt1.Rows[2][count] = (int)dt1.Rows[2][count] + (int)dt2.Rows[2][count];  // oxygen
            dt1.Rows[3][count] = (int)dt1.Rows[3][count] + (int)dt2.Rows[3][count];  // nitrogen
            dt1.Rows[4][count] = (int)dt1.Rows[4][count] + (int)dt2.Rows[4][count];  // phosphor
            dt1.Rows[5][count] = (int)dt1.Rows[5][count] + (int)dt2.Rows[5][count];  // sulfur
            dt1.Rows[6][count] = (int)dt1.Rows[6][count] + (int)dt2.Rows[6][count];  // sodium
        }
        
        public static DataTable createEmptyElementTable()
        {
            DataTable elements = new DataTable();
            elements.Clear();
            String count = "Count";
            String shortcut = "Shortcut";
            String element = "Element";
            String monoMass = "mass";

            DataColumn columnCount = elements.Columns.Add(count);
            DataColumn columnShortcut = elements.Columns.Add(shortcut);
            DataColumn columnElement = elements.Columns.Add(element);
            elements.Columns.Add(monoMass);

            columnCount.DataType = System.Type.GetType("System.Int32");
            columnShortcut.ReadOnly = true;
            columnElement.ReadOnly = true;

            DataRow carbon = elements.NewRow();
            carbon[count] = "0";
            carbon[shortcut] = "C";
            carbon[element] = "carbon";
            carbon[monoMass] = 12;
            elements.Rows.Add(carbon);

            DataRow hydrogen = elements.NewRow();
            hydrogen[count] = "0";
            hydrogen[shortcut] = "H";
            hydrogen[element] = "hydrogen";
            hydrogen[monoMass] = 1.007825035;
            elements.Rows.Add(hydrogen);

            DataRow oxygen = elements.NewRow();
            oxygen[count] = "0";
            oxygen[shortcut] = "O";
            oxygen[element] = "oxygen";
            oxygen[monoMass] = 15.99491463;
            elements.Rows.Add(oxygen);

            DataRow nitrogen = elements.NewRow();
            nitrogen[count] = "0";
            nitrogen[shortcut] = "N";
            nitrogen[element] = "nitrogen";
            nitrogen[monoMass] = 14.003074;
            elements.Rows.Add(nitrogen);

            DataRow phosphor = elements.NewRow();
            phosphor[count] = "0";
            phosphor[shortcut] = "P";
            phosphor[element] = "phosphor";
            phosphor[monoMass] = 30.973762;
            elements.Rows.Add(phosphor);

            DataRow sulfur = elements.NewRow();
            sulfur[count] = "0";
            sulfur[shortcut] = "S";
            sulfur[element] = "sulfur";
            sulfur[monoMass] = 31.9720707;
            elements.Rows.Add(sulfur);

            DataRow sodium = elements.NewRow();
            sodium[count] = "0";
            sodium[shortcut] = "Na";
            sodium[element] = "sodium";
            sodium[monoMass] = 22.9897677;
            elements.Rows.Add(sodium);
            return elements;
        }
        
        public static DataTable createEmptyElementTable(DataTable copy)
        {
            DataTable elements = new DataTable();
            elements.Clear();
            String count = "Count";
            String shortcut = "Shortcut";
            String element = "Element";
            String monoMass = "mass";

            DataColumn columnCount = elements.Columns.Add(count);
            DataColumn columnShortcut = elements.Columns.Add(shortcut);
            DataColumn columnElement = elements.Columns.Add(element);
            elements.Columns.Add(monoMass);

            columnCount.DataType = System.Type.GetType("System.Int32");
            columnShortcut.ReadOnly = true;
            columnElement.ReadOnly = true;

            DataRow carbon = elements.NewRow();
            carbon[count] = copy.Rows[0][count];
            carbon[shortcut] = "C";
            carbon[element] = "carbon";
            carbon[monoMass] = 12;
            elements.Rows.Add(carbon);

            DataRow hydrogen = elements.NewRow();
            hydrogen[count] = copy.Rows[1][count];
            hydrogen[shortcut] = "H";
            hydrogen[element] = "hydrogen";
            hydrogen[monoMass] = 1.007825035;
            elements.Rows.Add(hydrogen);

            DataRow oxygen = elements.NewRow();
            oxygen[count] = copy.Rows[2][count];
            oxygen[shortcut] = "O";
            oxygen[element] = "oxygen";
            oxygen[monoMass] = 15.99491463;
            elements.Rows.Add(oxygen);

            DataRow nitrogen = elements.NewRow();
            nitrogen[count] = copy.Rows[3][count];
            nitrogen[shortcut] = "N";
            nitrogen[element] = "nitrogen";
            nitrogen[monoMass] = 14.003074;
            elements.Rows.Add(nitrogen);

            DataRow phosphor = elements.NewRow();
            phosphor[count] = copy.Rows[4][count];
            phosphor[shortcut] = "P";
            phosphor[element] = "phosphor";
            phosphor[monoMass] = 30.973762;
            elements.Rows.Add(phosphor);

            DataRow sulfur = elements.NewRow();
            sulfur[count] = copy.Rows[5][count];
            sulfur[shortcut] = "S";
            sulfur[element] = "sulfur";
            sulfur[monoMass] =  31.9720707;
            elements.Rows.Add(sulfur);

            DataRow sodium = elements.NewRow();
            sodium[count] = copy.Rows[6][count];
            sodium[shortcut] = "Na";
            sodium[element] = "sodium";
            sodium[monoMass] = 22.9897677;
            elements.Rows.Add(sodium);
            return elements;
        }
    
        public MS2Fragment()
        {
            fragmentName = "-";
            fragmentCharge = -1;
            fragmentFile = "-";
            fragmentSelected = true;
            fragmentElements = createEmptyElementTable();
            fragmentBase = new ArrayList();
            restrictions = new HashSet<string>();
            intensity = 100;
        }

        public MS2Fragment(String name, String fileName)
        {
            fragmentName = name;
            fragmentCharge = -1;
            fragmentFile = fileName;
            fragmentSelected = true;
            fragmentElements = createEmptyElementTable();
            fragmentBase = new ArrayList();
            restrictions = new HashSet<string>();
            intensity = 100;
        }


        public MS2Fragment(String name, String fileName, int charge)
        {
            fragmentName = name;
            fragmentCharge = charge;
            fragmentFile = fileName;
            fragmentSelected = true;
            fragmentElements = createEmptyElementTable();
            fragmentBase = new ArrayList();
            restrictions = new HashSet<string>();
            intensity = 100;
        }
        
        public MS2Fragment(String name, int charge, String fileName, bool selected, DataTable dataElements, String baseForms, String restrictions)
        {
            fragmentName = name;
            fragmentCharge = charge;
            fragmentFile = fileName;
            fragmentSelected = selected;
            fragmentElements = dataElements;
            this.restrictions = new HashSet<string>();
            fragmentBase = new ArrayList(baseForms.Split(new char[] {';'}));
            intensity = 100;
            if (restrictions.Length > 0) foreach (string restriction in restrictions.Split(new char[] {';'})) this.restrictions.Add(restriction);
        }
        
        public MS2Fragment(String name, int charge, String fileName, bool selected, DataTable dataElements, String baseForms, String restrictions, double intens)
        {
            fragmentName = name;
            fragmentCharge = charge;
            fragmentFile = fileName;
            fragmentSelected = selected;
            fragmentElements = dataElements;
            this.restrictions = new HashSet<string>();
            fragmentBase = new ArrayList(baseForms.Split(new char[] {';'}));
            intensity = Math.Min(100, Math.Max(0, intens));
            if (restrictions.Length > 0) foreach (string restriction in restrictions.Split(new char[] {';'})) this.restrictions.Add(restriction);
        }

        public MS2Fragment(MS2Fragment copy)
        {
            fragmentName = copy.fragmentName;
            fragmentCharge = copy.fragmentCharge;
            fragmentFile = copy.fragmentFile;
            fragmentSelected = copy.fragmentSelected;
            fragmentElements = createEmptyElementTable(copy.fragmentElements);
            fragmentBase = new ArrayList();
            restrictions = new HashSet<string>();
            foreach (string fbase in copy.fragmentBase) fragmentBase.Add(fbase);
            foreach (string restriction in copy.restrictions) restrictions.Add(restriction);
            intensity = copy.intensity;
        }
    }


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
    
    

    [Serializable]
    public class GLLipid : Lipid
    {
        public fattyAcidGroup fag1;
        public fattyAcidGroup fag2;
        public fattyAcidGroup fag3;
        public bool containsSugar;
        public List<int> hgValues;
        public List<String> headGroupNames = new List<String>{"MGDG", "DGDG", "SQDG"};
    
    
        public GLLipid(Dictionary<String, String> allPaths, Dictionary<String, ArrayList> allFragments)
        {
            fag1 = new fattyAcidGroup();
            fag2 = new fattyAcidGroup();
            fag3 = new fattyAcidGroup();
            containsSugar = false;
            hgValues = new List<int>();
            MS2Fragments.Add("MG", new ArrayList());
            MS2Fragments.Add("DG", new ArrayList());
            MS2Fragments.Add("MGDG", new ArrayList());
            MS2Fragments.Add("DGDG", new ArrayList());
            MS2Fragments.Add("SQDG", new ArrayList());
            MS2Fragments.Add("TG", new ArrayList());
            adducts["+NH4"] = true;
            adducts["-H"] = false;
            
            foreach(KeyValuePair<String, ArrayList> kvp in MS2Fragments)
            {
                if (allPaths.ContainsKey(kvp.Key)) pathsToFullImage.Add(kvp.Key, allPaths[kvp.Key]);
                if (allFragments != null && allFragments.ContainsKey(kvp.Key))
                {
                    foreach (MS2Fragment fragment in allFragments[kvp.Key])
                    {
                        MS2Fragments[kvp.Key].Add(new MS2Fragment(fragment));
                    }
                }
            }
        }
    
        public GLLipid(GLLipid copy) : base((Lipid)copy) 
        {
            fag1 = new fattyAcidGroup(copy.fag1);
            fag2 = new fattyAcidGroup(copy.fag2);
            fag3 = new fattyAcidGroup(copy.fag3);
            containsSugar = copy.containsSugar;
            hgValues = new List<int>();
            foreach (int hgValue in copy.hgValues)
            {
                hgValues.Add(hgValue);
            }
            
        }
        
        
        public override string serialize()
        {
            string xml = "<lipid type=\"GL\">\n";
            xml += fag1.serialize();
            xml += fag2.serialize();
            xml += fag3.serialize();
            xml += "<containsSugar>" + (containsSugar ? 1 : 0) + "</containsSugar>\n";
            foreach (int hgValue in hgValues)
            {
                xml += "<headGroup>" + hgValue + "</headGroup>\n";
            }
            xml += base.serialize();
            xml += "</lipid>\n";
            return xml;
        }
        
        
        public override void import(XElement node)
        {
            int fattyAcidCounter = 0;
            hgValues.Clear();
            foreach (XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "fattyAcidGroup":
                        if (fattyAcidCounter == 0)
                        {
                            fag1.import(child);
                        }
                        else if (fattyAcidCounter == 1)
                        {
                            fag2.import(child);
                        }
                        else if (fattyAcidCounter == 2)
                        {
                            fag3.import(child);
                        }
                        else
                        {   
                            Console.WriteLine("Error, fatty acid");
                            throw new Exception();
                        }
                        ++fattyAcidCounter;
                        break;
                        
                    case "headGroup":
                        hgValues.Add(Convert.ToInt32(child.Value.ToString()));
                        break;
                        
                    case "containsSugar":
                        containsSugar = child.Value == "1";
                        break;
                        
                        
                    default:
                        base.import(child);
                        break;
                }
            }
        }
        
        public override void addLipids(DataTable allLipids, DataTable allLipidsUnique, Dictionary<String, DataTable> headGroupsTable, Dictionary<String, Dictionary<String, bool>> headgroupAdductRestrictions, HashSet<String> usedKeys, HashSet<String> replicates)
        {
            // check if more than one fatty acids are 0:0
            int checkFattyAcids = 0;
            checkFattyAcids += fag1.faTypes["FAx"] ? 1 : 0;
            checkFattyAcids += fag2.faTypes["FAx"] ? 1 : 0;
            checkFattyAcids += fag3.faTypes["FAx"] ? 1 : 0;
            if (checkFattyAcids > 2) return;
            
            int containsMonoLyso = 0;
            foreach (int fattyAcidLength1 in fag1.carbonCounts)
            {
                int maxDoubleBond1 = (fattyAcidLength1 - 1) >> 1;
                foreach (int fattyAcidDoubleBond1 in fag1.doubleBondCounts)
                {
                    foreach (int fattyAcidHydroxyl1 in fag1.hydroxylCounts)
                    {
                        foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair1 in fag1.faTypes)
                        {
                            if (fattyAcidKeyValuePair1.Value && maxDoubleBond1 >= fattyAcidDoubleBond1)
                            {
                                FattyAcid fa1 = new FattyAcid(fattyAcidLength1, fattyAcidDoubleBond1, fattyAcidHydroxyl1, fattyAcidKeyValuePair1.Key);
                                containsMonoLyso &= ~1;
                                if (fattyAcidKeyValuePair1.Key == "FAx")
                                {
                                    fa1 = new FattyAcid(0, 0, 0, "FA");
                                    containsMonoLyso |= 1;
                                }
                                foreach (int fattyAcidLength2 in fag2.carbonCounts)
                                {
                                    int maxDoubleBond2 = (fattyAcidLength2 - 1) >> 1;
                                    foreach (int fattyAcidDoubleBond2 in fag2.doubleBondCounts)
                                    {
                                        foreach (int fattyAcidHydroxyl2 in fag1.hydroxylCounts)
                                        {
                                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair2 in fag2.faTypes)
                                            {
                                                if (fattyAcidKeyValuePair2.Value && maxDoubleBond2 >= fattyAcidDoubleBond2)
                                                {
                                                    FattyAcid fa2 = new FattyAcid(fattyAcidLength2, fattyAcidDoubleBond2, fattyAcidHydroxyl2, fattyAcidKeyValuePair2.Key);
                                                    containsMonoLyso &= ~2;
                                                    if (fattyAcidKeyValuePair2.Key == "FAx")
                                                    {
                                                        fa2 = new FattyAcid(0, 0, 0, "FA");
                                                        containsMonoLyso |= 2;
                                                    }
                                                    foreach (int fattyAcidLength3 in fag3.carbonCounts)
                                                    {
                                                        int maxDoubleBond3 = (fattyAcidLength3 - 1) >> 1;
                                                        foreach (int fattyAcidDoubleBond3 in fag3.doubleBondCounts)
                                                        {
                                                            foreach (int fattyAcidHydroxyl3 in fag1.hydroxylCounts)
                                                            {
                                                                foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair3 in fag3.faTypes)
                                                                {
                                                                    if (fattyAcidKeyValuePair3.Value && maxDoubleBond3 >= fattyAcidDoubleBond3)
                                                                    {
                                                                        FattyAcid fa3 = new FattyAcid(fattyAcidLength3, fattyAcidDoubleBond3, fattyAcidHydroxyl3, fattyAcidKeyValuePair3.Key);
                                                                        containsMonoLyso &= ~4;
                                                                        if (fattyAcidKeyValuePair3.Key == "FAx")
                                                                        {
                                                                            fa3 = new FattyAcid(0, 0, 0, "FA");
                                                                            containsMonoLyso |= 4;
                                                                        }
                                                                                
                                                                                        
                                                                        List<FattyAcid> sortedAcids = new List<FattyAcid>();
                                                                        sortedAcids.Add(fa1);
                                                                        sortedAcids.Add(fa2);
                                                                        sortedAcids.Add(fa3);
                                                                        sortedAcids.Sort();
                                                                        
                                                                        // popcount
                                                                        int pcContainsMonoLyso = containsMonoLyso - ((containsMonoLyso >> 1) & 0x55555555);
                                                                        pcContainsMonoLyso = (pcContainsMonoLyso & 0x33333333) + ((pcContainsMonoLyso >> 2) & 0x33333333);
                                                                        pcContainsMonoLyso = ((pcContainsMonoLyso + (pcContainsMonoLyso >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
                                                                        
                                                                        String headgroup = "";
                                                                        switch(pcContainsMonoLyso)
                                                                        {
                                                                            case 0:
                                                                            headgroup = "TG";
                                                                                break;
                                                                            case 1:
                                                                            headgroup = "DG";
                                                                                break;
                                                                            case 2:
                                                                            headgroup = "MG";
                                                                                break;
                                                                        }
                                                                        String key = headgroup + " ";
                                                                        int i = 0;
                                                                        foreach (FattyAcid fa in sortedAcids)
                                                                        {
                                                                            if (fa.length > 0){
                                                                                if (i++ > 0) key += "_";
                                                                                key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                                                                if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                                                                key += fa.suffix;
                                                                            }
                                                                        }
                                                                        
                                                                        if (!usedKeys.Contains(key))
                                                                        {
                                                                            foreach (KeyValuePair<string, bool> adduct in adducts)
                                                                            {
                                                                                if (adduct.Value && headgroupAdductRestrictions[headgroup][adduct.Key])
                                                                                {
                                                                                    usedKeys.Add(key);
                                                                                    
                                                                                    DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                                                    MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                                                                    MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                                                                    MS2Fragment.addCounts(atomsCount, fa3.atomsCount);
                                                                                    MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                                                    String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                                    int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                                                    String chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                                    double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                                                                                    
                                                                                    foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                                                                    {
                                                                                        if (fragment.fragmentSelected && ((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                                                                        {
                                                                                            DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                                                            foreach (string fbase in fragment.fragmentBase)
                                                                                            {
                                                                                                switch(fbase)
                                                                                                {
                                                                                                    case "FA1":
                                                                                                        MS2Fragment.addCounts(atomsCountFragment, fa1.atomsCount);
                                                                                                        break;
                                                                                                    case "FA2":
                                                                                                        MS2Fragment.addCounts(atomsCountFragment, fa2.atomsCount);
                                                                                                        break;
                                                                                                    case "FA3":
                                                                                                        MS2Fragment.addCounts(atomsCountFragment, fa3.atomsCount);
                                                                                                        break;
                                                                                                    case "PRE":
                                                                                                        MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                                                        break;
                                                                                                    default:
                                                                                                        break;
                                                                                                }
                                                                                            }
                                                                                            String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                                                            //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                                                            double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge);
                                                                                            
                                                                                            
                                                                                        
                                                                                            DataRow lipidRow = allLipids.NewRow();
                                                                                            lipidRow["Molecule List Name"] = headgroup;
                                                                                            lipidRow["Precursor Name"] = key;
                                                                                            lipidRow["Precursor Ion Formula"] = chemForm;
                                                                                            lipidRow["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                                                            lipidRow["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                                            lipidRow["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                                            lipidRow["Product Name"] = fragment.fragmentName;
                                                                                            lipidRow["Product Ion Formula"] = chemFormFragment;
                                                                                            lipidRow["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                                                            lipidRow["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                                                            allLipids.Rows.Add(lipidRow);
                                                                                                                
                                                                                            String replicatesKey = chemFormComplete + "/" + chemFormFragment;
                                                                                            if (!replicates.Contains(replicatesKey))
                                                                                            {
                                                                                                replicates.Add(replicatesKey);
                                                                                                DataRow lipidRowUnique = allLipidsUnique.NewRow();
                                                                                                lipidRowUnique["Molecule List Name"] = headgroup;
                                                                                                lipidRowUnique["Precursor Name"] = key;
                                                                                                lipidRowUnique["Precursor Ion Formula"] = chemForm;
                                                                                                lipidRowUnique["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                                                                lipidRowUnique["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                                                lipidRowUnique["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                                                lipidRowUnique["Product Name"] = fragment.fragmentName;
                                                                                                lipidRowUnique["Product Ion Formula"] = chemFormFragment;
                                                                                                lipidRowUnique["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                                                                lipidRowUnique["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                                                                allLipidsUnique.Rows.Add(lipidRowUnique);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                       
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public override void addSpectrum(SQLiteCommand command, Dictionary<String, DataTable> headGroupsTable, HashSet<String> usedKeys)
        {
        // check if more than one fatty acids are 0:0
            int checkFattyAcids = 0;
            string sql;
            checkFattyAcids += fag1.faTypes["FAx"] ? 1 : 0;
            checkFattyAcids += fag2.faTypes["FAx"] ? 1 : 0;
            checkFattyAcids += fag3.faTypes["FAx"] ? 1 : 0;
            if (checkFattyAcids > 2) return;
            
            int containsMonoLyso = 0;
            foreach (int fattyAcidLength1 in fag1.carbonCounts)
            {
                int maxDoubleBond1 = (fattyAcidLength1 - 1) >> 1;
                foreach (int fattyAcidDoubleBond1 in fag1.doubleBondCounts)
                {
                    foreach (int fattyAcidHydroxyl1 in fag1.hydroxylCounts)
                    {
                        foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair1 in fag1.faTypes)
                        {
                            if (fattyAcidKeyValuePair1.Value && maxDoubleBond1 >= fattyAcidDoubleBond1)
                            {
                                FattyAcid fa1 = new FattyAcid(fattyAcidLength1, fattyAcidDoubleBond1, fattyAcidHydroxyl1, fattyAcidKeyValuePair1.Key);
                                containsMonoLyso &= ~1;
                                if (fattyAcidKeyValuePair1.Key == "FAx")
                                {
                                    fa1 = new FattyAcid(0, 0, 0, "FA");
                                    containsMonoLyso |= 1;
                                }
                                foreach (int fattyAcidLength2 in fag2.carbonCounts)
                                {
                                    int maxDoubleBond2 = (fattyAcidLength2 - 1) >> 1;
                                    foreach (int fattyAcidDoubleBond2 in fag2.doubleBondCounts)
                                    {
                                        foreach (int fattyAcidHydroxyl2 in fag1.hydroxylCounts)
                                        {
                                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair2 in fag2.faTypes)
                                            {
                                                if (fattyAcidKeyValuePair2.Value && maxDoubleBond2 >= fattyAcidDoubleBond2)
                                                {
                                                    FattyAcid fa2 = new FattyAcid(fattyAcidLength2, fattyAcidDoubleBond2, fattyAcidHydroxyl2, fattyAcidKeyValuePair2.Key);
                                                    containsMonoLyso &= ~2;
                                                    if (fattyAcidKeyValuePair2.Key == "FAx")
                                                    {
                                                        fa2 = new FattyAcid(0, 0, 0, "FA");
                                                        containsMonoLyso |= 2;
                                                    }
                                                    foreach (int fattyAcidLength3 in fag3.carbonCounts)
                                                    {
                                                        int maxDoubleBond3 = (fattyAcidLength3 - 1) >> 1;
                                                        foreach (int fattyAcidDoubleBond3 in fag3.doubleBondCounts)
                                                        {
                                                            foreach (int fattyAcidHydroxyl3 in fag1.hydroxylCounts)
                                                            {
                                                                foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair3 in fag3.faTypes)
                                                                {
                                                                    if (fattyAcidKeyValuePair3.Value && maxDoubleBond3 >= fattyAcidDoubleBond3)
                                                                    {
                                                                        FattyAcid fa3 = new FattyAcid(fattyAcidLength3, fattyAcidDoubleBond3, fattyAcidHydroxyl3, fattyAcidKeyValuePair3.Key);
                                                                        containsMonoLyso &= ~4;
                                                                        if (fattyAcidKeyValuePair3.Key == "FAx")
                                                                        {
                                                                            fa3 = new FattyAcid(0, 0, 0, "FA");
                                                                            containsMonoLyso |= 4;
                                                                        }
                                                                                
                                                                                        
                                                                        List<FattyAcid> sortedAcids = new List<FattyAcid>();
                                                                        sortedAcids.Add(fa1);
                                                                        sortedAcids.Add(fa2);
                                                                        sortedAcids.Add(fa3);
                                                                        sortedAcids.Sort();
                                                                        
                                                                        // popcount
                                                                        int pcContainsMonoLyso = containsMonoLyso - ((containsMonoLyso >> 1) & 0x55555555);
                                                                        pcContainsMonoLyso = (pcContainsMonoLyso & 0x33333333) + ((pcContainsMonoLyso >> 2) & 0x33333333);
                                                                        pcContainsMonoLyso = ((pcContainsMonoLyso + (pcContainsMonoLyso >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
                                                                        
                                                                        String headgroup = "";
                                                                        switch(pcContainsMonoLyso)
                                                                        {
                                                                            case 0:
                                                                            headgroup = "TG";
                                                                                break;
                                                                            case 1:
                                                                            headgroup = "DG";
                                                                                break;
                                                                            case 2:
                                                                            headgroup = "MG";
                                                                                break;
                                                                        }
                                                                        String key = headgroup + " ";
                                                                        int i = 0;
                                                                        foreach (FattyAcid fa in sortedAcids)
                                                                        {
                                                                            if (fa.length > 0){
                                                                                if (i++ > 0) key += "_";
                                                                                key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                                                                if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                                                                key += fa.suffix;
                                                                            }
                                                                        }
                                                                        
                                                                        
                                                                        foreach (KeyValuePair<string, bool> adduct in adducts)
                                                                        {
                                                                            if (adduct.Value)
                                                                            {
                                                                                String keyAdduct = key + " " + adduct.Key;
                                                                                String precursorAdduct = "[M" + adduct.Key + "]";
                                                                                if (!usedKeys.Contains(keyAdduct))
                                                                                {
                                                                                    usedKeys.Add(keyAdduct);
                                                                                    
                                                                                    DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                                                    MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                                                                    MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                                                                    MS2Fragment.addCounts(atomsCount, fa3.atomsCount);
                                                                                    MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                                                    String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                                    int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                                                    double mass = LipidCreatorForm.computeMass(atomsCount, charge) / (double)(Math.Abs(charge));                                                
                                                
                                                                                    ArrayList valuesMZ = new ArrayList();
                                                                                    ArrayList valuesIntensity = new ArrayList();
                                                                                    
                                                                                    foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                                                                    {
                                                                                        if (((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                                                                        {
                                                                                            DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                                                            foreach (string fbase in fragment.fragmentBase)
                                                                                            {
                                                                                                switch(fbase)
                                                                                                {
                                                                                                    case "FA1":
                                                                                                        MS2Fragment.addCounts(atomsCountFragment, fa1.atomsCount);
                                                                                                        break;
                                                                                                    case "FA2":
                                                                                                        MS2Fragment.addCounts(atomsCountFragment, fa2.atomsCount);
                                                                                                        break;
                                                                                                    case "FA3":
                                                                                                        MS2Fragment.addCounts(atomsCountFragment, fa3.atomsCount);
                                                                                                        break;
                                                                                                    case "PRE":
                                                                                                        MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                                                        break;
                                                                                                    default:
                                                                                                        break;
                                                                                                }
                                                                                            }
                                                                                            //String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                                                            //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                                                            double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge) / (double)(Math.Abs(fragment.fragmentCharge));
                                                        
                                                                                            valuesMZ.Add(massFragment);
                                                                                            valuesIntensity.Add(fragment.intensity);
                                                                                            
                                                                                            
                                                                                            // add Annotation
                                                                                            /*
                                                                                            sql = "INSERT INTO Annotations(RefSpectraID, fragmentMZ, sumComposition, shortName) VALUES ((SELECT COUNT(*) FROM RefSpectra) + 1, " + massFragment + ", '" + chemFormFragment + "', @fragmentName)";
                                                                                            SQLiteParameter parameterName = new SQLiteParameter("@fragmentName", System.Data.DbType.String);
                                                                                            parameterName.Value = fragment.fragmentName;
                                                                                            command.CommandText = sql;
                                                                                            command.Parameters.Add(parameterName);
                                                                                            command.ExecuteNonQuery();
                                                                                            */
                                                                                        }
                                                                                    }
                                                
                                                
                                                                                    int numFragments = valuesMZ.Count;
                                                                                    double[] valuesMZArray = new double[numFragments];
                                                                                    float[] valuesIntens = new float[numFragments];
                                                                                    for(int j = 0; j < numFragments; ++j)
                                                                                    {
                                                                                        valuesMZArray[j] = (double)valuesMZ[j];
                                                                                        valuesIntens[j] = 100 * (float)((double)valuesIntensity[j]);
                                                                                    }
                                                                                    
                                                                                    
                                                                                    // add MS1 information
                                                                                    sql = "INSERT INTO RefSpectra (moleculeName, precursorMZ, precursorCharge, precursorAdduct, prevAA, nextAA, copies, numPeaks, driftTimeMsec, collisionalCrossSectionSqA, driftTimeHighEnergyOffsetMsec, retentionTime, fileID, SpecIDinFile, score, scoreType, inchiKey, otherKeys, peptideSeq, peptideModSeq, chemicalFormula) VALUES('" + key + "', " + mass + ", " + charge + ", '" + precursorAdduct + "', '-', '-', 0, " + numFragments + ", 0, 0, 0, 0, '0', 0, 1, 1, '', '', '', '',  '" + chemForm + "')";
                                                                                    command.CommandText = sql;
                                                                                    command.ExecuteNonQuery();
                                                                                    
                                                                                    // add spectrum
                                                                                    command.CommandText = "INSERT INTO RefSpectraPeaks(RefSpectraID, peakMZ, peakIntensity) VALUES((SELECT MAX(id) FROM RefSpectra), @mzvalues, @intensvalues)";
                                                                                    SQLiteParameter parameterMZ = new SQLiteParameter("@mzvalues", System.Data.DbType.Binary);
                                                                                    SQLiteParameter parameterIntens = new SQLiteParameter("@intensvalues", System.Data.DbType.Binary);
                                                                                    parameterMZ.Value = Compressing.Compress(valuesMZArray);
                                                                                    parameterIntens.Value = Compressing.Compress(valuesIntens);
                                                                                    command.Parameters.Add(parameterMZ);
                                                                                    command.Parameters.Add(parameterIntens);
                                                                                    command.ExecuteNonQuery();
                                                                                }
                                                                            }
                                                                        }
                                                                       
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [Serializable]
    public class PLLipid : Lipid
    {
        public fattyAcidGroup fag1;
        public fattyAcidGroup fag2;
        public fattyAcidGroup fag3;
        public fattyAcidGroup fag4;
        public bool isCL;
        public List<int> hgValues;
        
        public List<String> headGroupNames = new List<String>{"CDP-DAG", "PA", "PC", "PE", "PEt", "DMPE", "MMPE", "PG", "PI", "PIP", "PIP2", "PIP3", "PS", "LPA", "LPC", "LPE", "LPG", "LPI", "LPS"};
    
        public PLLipid(Dictionary<String, String> allPaths, Dictionary<String, ArrayList> allFragments)
        {
            fag1 = new fattyAcidGroup();
            fag2 = new fattyAcidGroup();
            fag3 = new fattyAcidGroup();
            fag4 = new fattyAcidGroup();
            hgValues = new List<int>();
            isCL = false;
            MS2Fragments.Add("CDP-DAG", new ArrayList());
            MS2Fragments.Add("CL", new ArrayList());
            MS2Fragments.Add("MLCL", new ArrayList());
            MS2Fragments.Add("PA", new ArrayList());
            MS2Fragments.Add("PC", new ArrayList());
            MS2Fragments.Add("pPC", new ArrayList());
            MS2Fragments.Add("ppPC", new ArrayList());
            MS2Fragments.Add("PE", new ArrayList());
            MS2Fragments.Add("PEt", new ArrayList());
            MS2Fragments.Add("pPE", new ArrayList());
            MS2Fragments.Add("ppPE", new ArrayList());
            MS2Fragments.Add("DMPE", new ArrayList());
            MS2Fragments.Add("MMPE", new ArrayList());
            MS2Fragments.Add("PG", new ArrayList());
            MS2Fragments.Add("PI", new ArrayList());
            MS2Fragments.Add("PIP", new ArrayList());
            MS2Fragments.Add("PIP2", new ArrayList());
            MS2Fragments.Add("PIP3", new ArrayList());
            MS2Fragments.Add("PS", new ArrayList());
            MS2Fragments.Add("LPA", new ArrayList());
            MS2Fragments.Add("LPC", new ArrayList());
            MS2Fragments.Add("LPE", new ArrayList());
            MS2Fragments.Add("LPG", new ArrayList());
            MS2Fragments.Add("LPI", new ArrayList());
            MS2Fragments.Add("LPS", new ArrayList());
            
            foreach(KeyValuePair<String, ArrayList> kvp in MS2Fragments)
            {
                if (allPaths.ContainsKey(kvp.Key)) pathsToFullImage.Add(kvp.Key, allPaths[kvp.Key]);
                if (allFragments != null && allFragments.ContainsKey(kvp.Key))
                {
                    foreach (MS2Fragment fragment in allFragments[kvp.Key])
                    {
                        MS2Fragments[kvp.Key].Add(new MS2Fragment(fragment));
                    }
                }
            }
        }
    
        public PLLipid(PLLipid copy) : base((Lipid)copy)
        {
            fag1 = new fattyAcidGroup(copy.fag1);
            fag2 = new fattyAcidGroup(copy.fag2);
            fag3 = new fattyAcidGroup(copy.fag3);
            fag4 = new fattyAcidGroup(copy.fag4);
            hgValues = new List<int>();
            isCL = copy.isCL;
            foreach (int hgValue in copy.hgValues)
            {
                hgValues.Add(hgValue);
            }
        }
        
        public override string serialize()
        {
            string xml = "<lipid type=\"PL\" isCL=\"" + isCL + "\">\n";
            xml += fag1.serialize();
            xml += fag2.serialize();
            xml += fag3.serialize();
            xml += fag4.serialize();
            foreach (int hgValue in hgValues)
            {
                xml += "<headGroup>" + hgValue + "</headGroup>\n";
            }
            xml += base.serialize();
            xml += "</lipid>\n";
            return xml;
        }
        
        public override void import(XElement node)
        {
            int fattyAcidCounter = 0;
            hgValues.Clear();
            isCL = node.Attribute("type").Value == "True";
            foreach (XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "fattyAcidGroup":
                        if (fattyAcidCounter == 0)
                        {
                            fag1.import(child);
                        }
                        else if (fattyAcidCounter == 1)
                        {
                            fag2.import(child);
                        }
                        else if (fattyAcidCounter == 2)
                        {
                            fag3.import(child);
                        }
                        else if (fattyAcidCounter == 3)
                        {
                            fag4.import(child);
                        }
                        else
                        {   
                            Console.WriteLine("Error, fatty acid");
                            throw new Exception();
                        }
                        ++fattyAcidCounter;
                        break;
                        
                    case "headGroup":
                        hgValues.Add(Convert.ToInt32(child.Value.ToString()));
                        break;
                        
                        
                    default:
                        base.import(child);
                        break;
                }
            }
        }
        
        
        public override void addLipids(DataTable allLipids, DataTable allLipidsUnique, Dictionary<String, DataTable> headGroupsTable, Dictionary<String, Dictionary<String, bool>> headgroupAdductRestrictions, HashSet<String> usedKeys, HashSet<String> replicates)
        {
            if (isCL)
            {
                // check if more than one fatty acids are 0:0
                int checkFattyAcids = 0;
                checkFattyAcids += fag1.faTypes["FAx"] ? 1 : 0;
                checkFattyAcids += fag2.faTypes["FAx"] ? 1 : 0;
                checkFattyAcids += fag3.faTypes["FAx"] ? 1 : 0;
                checkFattyAcids += fag4.faTypes["FAx"] ? 1 : 0;
                if (checkFattyAcids > 1) return;
                
                
                int containsMonoLyso = 0;
                foreach (int fattyAcidLength1 in fag1.carbonCounts)
                {
                    int maxDoubleBond1 = (fattyAcidLength1 - 1) >> 1;
                    foreach (int fattyAcidDoubleBond1 in fag1.doubleBondCounts)
                    {
                        foreach (int fattyAcidHydroxyl1 in fag1.hydroxylCounts)
                        {
                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair1 in fag1.faTypes)
                            {
                                if (fattyAcidKeyValuePair1.Value && maxDoubleBond1 >= fattyAcidDoubleBond1)
                                {
                                    FattyAcid fa1 = new FattyAcid(fattyAcidLength1, fattyAcidDoubleBond1, fattyAcidHydroxyl1, fattyAcidKeyValuePair1.Key);
                                    containsMonoLyso &= ~1;
                                    if (fattyAcidKeyValuePair1.Key == "FAx")
                                    {
                                        fa1 = new FattyAcid(0, 0, 0, "FA");
                                        containsMonoLyso |= 1;
                                    }
                                    foreach (int fattyAcidLength2 in fag2.carbonCounts)
                                    {
                                        int maxDoubleBond2 = (fattyAcidLength2 - 1) >> 1;
                                        foreach (int fattyAcidDoubleBond2 in fag2.doubleBondCounts)
                                        {
                                            foreach (int fattyAcidHydroxyl2 in fag1.hydroxylCounts)
                                            {
                                                foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair2 in fag2.faTypes)
                                                {
                                                    if (fattyAcidKeyValuePair2.Value && maxDoubleBond2 >= fattyAcidDoubleBond2)
                                                    {
                                                        FattyAcid fa2 = new FattyAcid(fattyAcidLength2, fattyAcidDoubleBond2, fattyAcidHydroxyl2, fattyAcidKeyValuePair2.Key);
                                                        containsMonoLyso &= ~2;
                                                        if (fattyAcidKeyValuePair2.Key == "FAx")
                                                        {
                                                            fa2 = new FattyAcid(0, 0, 0, "FA");
                                                            containsMonoLyso |= 2;
                                                        }
                                                        foreach (int fattyAcidLength3 in fag3.carbonCounts)
                                                        {
                                                            int maxDoubleBond3 = (fattyAcidLength3 - 1) >> 1;
                                                            foreach (int fattyAcidDoubleBond3 in fag3.doubleBondCounts)
                                                            {
                                                                foreach (int fattyAcidHydroxyl3 in fag1.hydroxylCounts)
                                                                {
                                                                    foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair3 in fag3.faTypes)
                                                                    {
                                                                        if (fattyAcidKeyValuePair3.Value && maxDoubleBond3 >= fattyAcidDoubleBond3)
                                                                        {
                                                                            FattyAcid fa3 = new FattyAcid(fattyAcidLength3, fattyAcidDoubleBond3, fattyAcidHydroxyl3, fattyAcidKeyValuePair3.Key);
                                                                            containsMonoLyso &= ~4;
                                                                            if (fattyAcidKeyValuePair3.Key == "FAx")
                                                                            {
                                                                                fa3 = new FattyAcid(0, 0, 0, "FA");
                                                                                containsMonoLyso |= 4;
                                                                            }
                                                                            foreach (int fattyAcidLength4 in fag4.carbonCounts)
                                                                            {
                                                                                int maxDoubleBond4 = (fattyAcidLength4 - 1) >> 1;
                                                                                foreach (int fattyAcidDoubleBond4 in fag4.doubleBondCounts)
                                                                                {
                                                                                    foreach (int fattyAcidHydroxyl4 in fag1.hydroxylCounts)
                                                                                    {
                                                                                        foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair4 in fag4.faTypes)
                                                                                        {
                                                                                            if (fattyAcidKeyValuePair4.Value && maxDoubleBond4 >= fattyAcidDoubleBond4)
                                                                                            {
                                                                                                FattyAcid fa4 = new FattyAcid(fattyAcidLength4, fattyAcidDoubleBond4, fattyAcidHydroxyl4, fattyAcidKeyValuePair4.Key);
                                                                                                containsMonoLyso &= ~8;
                                                                                                if (fattyAcidKeyValuePair4.Key == "FAx")
                                                                                                {
                                                                                                    fa4 = new FattyAcid(0, 0, 0, "FA");
                                                                                                    containsMonoLyso |= 8;
                                                                                                }
                                                                                                
                                                                                                
                                                                                                
                                                                                                
                                                                                                List<FattyAcid> sortedAcids = new List<FattyAcid>();
                                                                                                sortedAcids.Add(fa1);
                                                                                                sortedAcids.Add(fa2);
                                                                                                sortedAcids.Add(fa3);
                                                                                                sortedAcids.Add(fa4);
                                                                                                sortedAcids.Sort();
                                                                                                String headgroup = (containsMonoLyso == 0) ? "CL" : "MLCL";
                                                                                                String key = headgroup + " ";
                                                                                                int i = 0;
                                                                                                foreach (FattyAcid fa in sortedAcids)
                                                                                                {
                                                                                                    if (fa.length > 0){
                                                                                                        if (i++ > 0) key += "_";
                                                                                                        key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                                                                                        if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                                                                                        key += fa.suffix;
                                                                                                    }
                                                                                                }
                                                                                                if (!usedKeys.Contains(key))
                                                                                                {
                                                                                                
                                                                                                
                                                                                                    foreach (KeyValuePair<string, bool> adduct in adducts)
                                                                                                    {
                                                                                                        if (adduct.Value && headgroupAdductRestrictions[headgroup][adduct.Key])
                                                                                                        {
                                                                                                            usedKeys.Add(key);
                                                                                                            
                                                                                                            DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                                                                            MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                                                                                            MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                                                                                            MS2Fragment.addCounts(atomsCount, fa3.atomsCount);
                                                                                                            MS2Fragment.addCounts(atomsCount, fa4.atomsCount);
                                                                                                            MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                                                                            String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                                                            int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                                                                            String chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                                                            double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                                                                                                            
                                                                                                            
                                                                                                            
                                                                                                            
                                                                                                            foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                                                                                            {
                                                                                                                if (fragment.fragmentSelected && ((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)))
                                                                                                                {
                                                                                                                    DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                                                                                    foreach (string fbase in fragment.fragmentBase)
                                                                                                                    {
                                                                                                                        switch(fbase)
                                                                                                                        {
                                                                                                                            case "FA1":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa1.atomsCount);
                                                                                                                                break;
                                                                                                                            case "FA2":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa2.atomsCount);
                                                                                                                                break;
                                                                                                                            case "FA3":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa3.atomsCount);
                                                                                                                                break;
                                                                                                                            case "FA4":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa4.atomsCount);
                                                                                                                                break;
                                                                                                                            case "PRE":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                                                                                break;
                                                                                                                            default:
                                                                                                                                break;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                                                                                    //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                                                                                    double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge);
                                                                                                                    
                                                                                                                
                                                                                                                    DataRow lipidRow = allLipids.NewRow();
                                                                                                                    lipidRow["Molecule List Name"] = headgroup;
                                                                                                                    lipidRow["Precursor Name"] = key;
                                                                                                                    lipidRow["Precursor Ion Formula"] = chemForm;
                                                                                                                    lipidRow["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                                                                                    lipidRow["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                                                                    lipidRow["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                                                                    lipidRow["Product Name"] = fragment.fragmentName;
                                                                                                                    lipidRow["Product Ion Formula"] = chemFormFragment;
                                                                                                                    lipidRow["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                                                                                    lipidRow["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                                                                                    allLipids.Rows.Add(lipidRow);
                                                                                                                    
                                                                                                                    String replicatesKey = chemFormComplete + "/" + chemFormFragment;
                                                                                                                    if (!replicates.Contains(replicatesKey))
                                                                                                                    {
                                                                                                                        replicates.Add(replicatesKey);
                                                                                                                        DataRow lipidRowUnique = allLipidsUnique.NewRow();
                                                                                                                        lipidRowUnique["Molecule List Name"] = headgroup;
                                                                                                                        lipidRowUnique["Precursor Name"] = key;
                                                                                                                        lipidRowUnique["Precursor Ion Formula"] = chemForm;
                                                                                                                        lipidRowUnique["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                                                                                        lipidRowUnique["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                                                                        lipidRowUnique["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                                                                        lipidRowUnique["Product Name"] = fragment.fragmentName;
                                                                                                                        lipidRowUnique["Product Ion Formula"] = chemFormFragment;
                                                                                                                        lipidRowUnique["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                                                                                        lipidRowUnique["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                                                                                        allLipidsUnique.Rows.Add(lipidRowUnique);
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // check if more than one fatty acids are 0:0
                int checkFattyAcids = 0;
                checkFattyAcids += fag1.faTypes["FAx"] ? 1 : 0;
                checkFattyAcids += fag2.faTypes["FAx"] ? 1 : 0;
                if (checkFattyAcids > 0) return;
                if (hgValues.Count == 0) return;
                int isPlamalogen = 0;
                
                foreach (int fattyAcidLength1 in fag1.carbonCounts)
                {
                    int maxDoubleBond1 = (fattyAcidLength1 - 1) >> 1;
                    foreach (int fattyAcidDoubleBond1 in fag1.doubleBondCounts)
                    {
                        foreach (int fattyAcidHydroxyl1 in fag1.hydroxylCounts)
                        {
                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair1 in fag1.faTypes)
                            {
                                if (fattyAcidKeyValuePair1.Value && maxDoubleBond1 >= fattyAcidDoubleBond1)
                                {
                                    FattyAcid fa1 = new FattyAcid(fattyAcidLength1, fattyAcidDoubleBond1, fattyAcidHydroxyl1, fattyAcidKeyValuePair1.Key);
                                    if (fattyAcidKeyValuePair1.Key == "FAx")
                                    {
                                        fa1 = new FattyAcid(0, 0, 0, "FA");
                                    }
                                    if (fattyAcidKeyValuePair1.Key == "FAp")
                                    {
                                        isPlamalogen += 1;
                                    }
                                    foreach (int fattyAcidLength2 in fag2.carbonCounts)
                                    {
                                        int maxDoubleBond2 = (fattyAcidLength2 - 1) >> 1;
                                        foreach (int fattyAcidDoubleBond2 in fag2.doubleBondCounts)
                                        {
                                            foreach (int fattyAcidHydroxyl2 in fag2.hydroxylCounts)
                                            {
                                                foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair2 in fag2.faTypes)
                                                {
                                                    if (fattyAcidKeyValuePair2.Value && maxDoubleBond2 >= fattyAcidDoubleBond2)
                                                    {
                                                        FattyAcid fa2 = new FattyAcid(fattyAcidLength2, fattyAcidDoubleBond2, fattyAcidHydroxyl2, fattyAcidKeyValuePair2.Key);
                                                        if (fattyAcidKeyValuePair2.Key == "FAx")
                                                        {
                                                            fa2 = new FattyAcid(0, 0, 0, "FA");
                                                        }
                                                        if (fattyAcidKeyValuePair2.Key == "FAp")
                                                        {
                                                            isPlamalogen += 1;
                                                        }             
                                                        List<FattyAcid> sortedAcids = new List<FattyAcid>();
                                                        sortedAcids.Add(fa1);
                                                        sortedAcids.Add(fa2);
                                                        sortedAcids.Sort();
                                                        
                                                        foreach(int hgValue in hgValues)
                                                        {
                                                        
                                                            String headgroup = headGroupNames[hgValue];
                                                            String headgroupSearch = headgroup;
                                                            String key = headgroup + " ";
                                                            if (headgroup.Equals("PC") || headgroup.Equals("PE"))
                                                            {
                                                                if (isPlamalogen >= 1) headgroupSearch = "p" + headgroupSearch;
                                                                if (isPlamalogen == 2) headgroupSearch = "p" + headgroupSearch;
                                                            }
                                                            int i = 0;
                                                            foreach (FattyAcid fa in sortedAcids)
                                                            {
                                                                if (fa.length > 0){
                                                                    if (i++ > 0) key += "_";
                                                                    key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                                                    if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                                                    key += fa.suffix;
                                                                }
                                                            }
                                                            if (!usedKeys.Contains(key))
                                                            {
                                                                foreach (KeyValuePair<string, bool> adduct in adducts)
                                                                {
                                                                    if (adduct.Value && headgroupAdductRestrictions[headgroupSearch][adduct.Key])
                                                                    {
                                                                        usedKeys.Add(key);
                                                                        
                                                                        DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                                        MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                                                        MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                                                        MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroupSearch]);
                                                                        String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                        int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                                        String chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                        double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                                                                        
                                                                        
                                                                        // add precursor as fragment
                                                                        
                                                                        DataRow lipidRowPrec = allLipids.NewRow();
                                                                        lipidRowPrec["Molecule List Name"] = headgroup;
                                                                        lipidRowPrec["Precursor Name"] = key;
                                                                        lipidRowPrec["Precursor Ion Formula"] = chemForm;
                                                                        lipidRowPrec["Precursor Adduct"] = "[M]";
                                                                        lipidRowPrec["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                        lipidRowPrec["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                        lipidRowPrec["Product Name"] = "Precursor";;
                                                                        lipidRowPrec["Product Ion Formula"] = chemForm;
                                                                        lipidRowPrec["Product m/z"] = mass / (double)(Math.Abs(charge));
                                                                        lipidRowPrec["Product Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                        allLipids.Rows.Add(lipidRowPrec);
                                                                                                            
                                                                        String replicatesPrecKey = chemFormComplete + "/" + chemFormComplete;
                                                                        if (!replicates.Contains(replicatesPrecKey))
                                                                        {
                                                                            replicates.Add(replicatesPrecKey);
                                                                            DataRow lipidRowPrecUnique = allLipidsUnique.NewRow();
                                                                            lipidRowPrecUnique["Molecule List Name"] = headgroup;
                                                                            lipidRowPrecUnique["Precursor Name"] = key;
                                                                            lipidRowPrecUnique["Precursor Ion Formula"] = chemForm;
                                                                            lipidRowPrecUnique["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                                            lipidRowPrecUnique["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                            lipidRowPrecUnique["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                            lipidRowPrecUnique["Product Name"] = "Precursor";
                                                                            lipidRowPrecUnique["Product Ion Formula"] = chemForm;
                                                                            lipidRowPrecUnique["Product m/z"] = mass / (double)(Math.Abs(charge));
                                                                            lipidRowPrecUnique["Product Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                            allLipidsUnique.Rows.Add(lipidRowPrecUnique);
                                                                        }
                                                                        
                                                                        
                                                                        
                                                                        foreach (MS2Fragment fragment in MS2Fragments[headgroupSearch])
                                                                        {
                                                                            if (fragment.fragmentSelected && ((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                                                            {
                                                                                DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                                                foreach (string fbase in fragment.fragmentBase)
                                                                                {
                                                                                    switch(fbase)
                                                                                    {
                                                                                        case "FA1":
                                                                                            MS2Fragment.addCounts(atomsCountFragment, fa1.atomsCount);
                                                                                            break;
                                                                                        case "FA2":
                                                                                            MS2Fragment.addCounts(atomsCountFragment, fa2.atomsCount);
                                                                                            break;
                                                                                        case "PRE":
                                                                                            MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                                            break;
                                                                                        default:
                                                                                            break;
                                                                                    }
                                                                                }
                                                                                String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                                                //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                                                double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge);
                                                                                
                                                                            
                                                                                DataRow lipidRow = allLipids.NewRow();
                                                                                lipidRow["Molecule List Name"] = headgroup;
                                                                                lipidRow["Precursor Name"] = key;
                                                                                lipidRow["Precursor Ion Formula"] = chemForm;
                                                                                lipidRow["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                                                lipidRow["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                                lipidRow["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                                lipidRow["Product Name"] = fragment.fragmentName;
                                                                                lipidRow["Product Ion Formula"] = chemFormFragment;
                                                                                lipidRow["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                                                lipidRow["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                                                allLipids.Rows.Add(lipidRow);
                                                                                                                    
                                                                                String replicatesKey = chemFormComplete + "/" + chemFormFragment;
                                                                                if (!replicates.Contains(replicatesKey))
                                                                                {
                                                                                    replicates.Add(replicatesKey);
                                                                                    DataRow lipidRowUnique = allLipidsUnique.NewRow();
                                                                                    lipidRowUnique["Molecule List Name"] = headgroup;
                                                                                    lipidRowUnique["Precursor Name"] = key;
                                                                                    lipidRowUnique["Precursor Ion Formula"] = chemForm;
                                                                                    lipidRowUnique["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                                                    lipidRowUnique["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                                    lipidRowUnique["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                                    lipidRowUnique["Product Name"] = fragment.fragmentName;
                                                                                    lipidRowUnique["Product Ion Formula"] = chemFormFragment;
                                                                                    lipidRowUnique["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                                                    lipidRowUnique["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                                                    allLipidsUnique.Rows.Add(lipidRowUnique);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public override void addSpectrum(SQLiteCommand command, Dictionary<String, DataTable> headGroupsTable, HashSet<String> usedKeys)
        {
            if (isCL){
                // check if more than one fatty acids are 0:0
                int checkFattyAcids = 0;
                string sql;
                checkFattyAcids += fag1.faTypes["FAx"] ? 1 : 0;
                checkFattyAcids += fag2.faTypes["FAx"] ? 1 : 0;
                checkFattyAcids += fag3.faTypes["FAx"] ? 1 : 0;
                checkFattyAcids += fag4.faTypes["FAx"] ? 1 : 0;
                if (checkFattyAcids > 1) return;
                
                
                int containsMonoLyso = 0;
                foreach (int fattyAcidLength1 in fag1.carbonCounts)
                {
                    int maxDoubleBond1 = (fattyAcidLength1 - 1) >> 1;
                    foreach (int fattyAcidDoubleBond1 in fag1.doubleBondCounts)
                    {
                        foreach (int fattyAcidHydroxyl1 in fag1.hydroxylCounts)
                        {
                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair1 in fag1.faTypes)
                            {
                                if (fattyAcidKeyValuePair1.Value && maxDoubleBond1 >= fattyAcidDoubleBond1)
                                {
                                    FattyAcid fa1 = new FattyAcid(fattyAcidLength1, fattyAcidDoubleBond1, fattyAcidHydroxyl1, fattyAcidKeyValuePair1.Key);
                                    containsMonoLyso &= ~1;
                                    if (fattyAcidKeyValuePair1.Key == "FAx")
                                    {
                                        fa1 = new FattyAcid(0, 0, 0, "FA");
                                        containsMonoLyso |= 1;
                                    }
                                    foreach (int fattyAcidLength2 in fag2.carbonCounts)
                                    {
                                        int maxDoubleBond2 = (fattyAcidLength2 - 1) >> 1;
                                        foreach (int fattyAcidDoubleBond2 in fag2.doubleBondCounts)
                                        {
                                            foreach (int fattyAcidHydroxyl2 in fag1.hydroxylCounts)
                                            {
                                                foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair2 in fag2.faTypes)
                                                {
                                                    if (fattyAcidKeyValuePair2.Value && maxDoubleBond2 >= fattyAcidDoubleBond2)
                                                    {
                                                        FattyAcid fa2 = new FattyAcid(fattyAcidLength2, fattyAcidDoubleBond2, fattyAcidHydroxyl2, fattyAcidKeyValuePair2.Key);
                                                        containsMonoLyso &= ~2;
                                                        if (fattyAcidKeyValuePair2.Key == "FAx")
                                                        {
                                                            fa2 = new FattyAcid(0, 0, 0, "FA");
                                                            containsMonoLyso |= 2;
                                                        }
                                                        foreach (int fattyAcidLength3 in fag3.carbonCounts)
                                                        {
                                                            int maxDoubleBond3 = (fattyAcidLength3 - 1) >> 1;
                                                            foreach (int fattyAcidDoubleBond3 in fag3.doubleBondCounts)
                                                            {
                                                                foreach (int fattyAcidHydroxyl3 in fag1.hydroxylCounts)
                                                                {
                                                                    foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair3 in fag3.faTypes)
                                                                    {
                                                                        if (fattyAcidKeyValuePair3.Value && maxDoubleBond3 >= fattyAcidDoubleBond3)
                                                                        {
                                                                            FattyAcid fa3 = new FattyAcid(fattyAcidLength3, fattyAcidDoubleBond3, fattyAcidHydroxyl3, fattyAcidKeyValuePair3.Key);
                                                                            containsMonoLyso &= ~4;
                                                                            if (fattyAcidKeyValuePair3.Key == "FAx")
                                                                            {
                                                                                fa3 = new FattyAcid(0, 0, 0, "FA");
                                                                                containsMonoLyso |= 4;
                                                                            }
                                                                            foreach (int fattyAcidLength4 in fag4.carbonCounts)
                                                                            {
                                                                                int maxDoubleBond4 = (fattyAcidLength4 - 1) >> 1;
                                                                                foreach (int fattyAcidDoubleBond4 in fag4.doubleBondCounts)
                                                                                {
                                                                                    foreach (int fattyAcidHydroxyl4 in fag1.hydroxylCounts)
                                                                                    {
                                                                                        foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair4 in fag4.faTypes)
                                                                                        {
                                                                                            if (fattyAcidKeyValuePair4.Value && maxDoubleBond4 >= fattyAcidDoubleBond4)
                                                                                            {
                                                                                                FattyAcid fa4 = new FattyAcid(fattyAcidLength4, fattyAcidDoubleBond4, fattyAcidHydroxyl4, fattyAcidKeyValuePair4.Key);
                                                                                                containsMonoLyso &= ~8;
                                                                                                if (fattyAcidKeyValuePair4.Key == "FAx")
                                                                                                {
                                                                                                    fa4 = new FattyAcid(0, 0, 0, "FA");
                                                                                                    containsMonoLyso |= 8;
                                                                                                }
                                                                                                
                                                                                                
                                                                                                
                                                                                                
                                                                                                List<FattyAcid> sortedAcids = new List<FattyAcid>();
                                                                                                sortedAcids.Add(fa1);
                                                                                                sortedAcids.Add(fa2);
                                                                                                sortedAcids.Add(fa3);
                                                                                                sortedAcids.Add(fa4);
                                                                                                sortedAcids.Sort();
                                                                                                String headgroup = (containsMonoLyso == 0) ? "CL" : "MLCL";
                                                                                                String key = headgroup + " ";
                                                                                                int i = 0;
                                                                                                foreach (FattyAcid fa in sortedAcids)
                                                                                                {
                                                                                                    if (fa.length > 0){
                                                                                                        if (i++ > 0) key += "_";
                                                                                                        key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                                                                                        if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                                                                                        key += fa.suffix;
                                                                                                    }
                                                                                                }
                                                                                                
                                                                                                foreach (KeyValuePair<string, bool> adduct in adducts)
                                                                                                {
                                                                                                    if (adduct.Value)
                                                                                                    {
                                                                                                        String keyAdduct = key + " " + adduct.Key;
                                                                                                        String precursorAdduct = "[M" + adduct.Key + "]";
                                                                                                        if (!usedKeys.Contains(keyAdduct))
                                                                                                        {
                                                                                                            usedKeys.Add(keyAdduct);
                                                                                                            
                                                                                                            DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                                                                            MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                                                                                            MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                                                                                            MS2Fragment.addCounts(atomsCount, fa3.atomsCount);
                                                                                                            MS2Fragment.addCounts(atomsCount, fa4.atomsCount);
                                                                                                            MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                                                                            String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                                                            int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                                                                            double mass = LipidCreatorForm.computeMass(atomsCount, charge) / (double)(Math.Abs(charge));                                                
                                                    
                                                                                                            ArrayList valuesMZ = new ArrayList();
                                                                                                            ArrayList valuesIntensity = new ArrayList();
                                                                                                            
                                                                                                            foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                                                                                            {
                                                                                                                if (((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)))
                                                                                                                {
                                                                                                                    DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                                                                                    foreach (string fbase in fragment.fragmentBase)
                                                                                                                    {
                                                                                                                        switch(fbase)
                                                                                                                        {
                                                                                                                            case "FA1":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa1.atomsCount);
                                                                                                                                break;
                                                                                                                            case "FA2":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa2.atomsCount);
                                                                                                                                break;
                                                                                                                            case "FA3":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa3.atomsCount);
                                                                                                                                break;
                                                                                                                            case "FA4":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa4.atomsCount);
                                                                                                                                break;
                                                                                                                            case "PRE":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                                                                                break;
                                                                                                                            default:
                                                                                                                                break;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    //String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                                                                                    //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                                                                                    double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge) / (double)(Math.Abs(fragment.fragmentCharge));
                                                            
                                                                                                                    valuesMZ.Add(massFragment);
                                                                                                                    valuesIntensity.Add(fragment.intensity);
                                                                                                                    
                                                                                                                    
                                                                                                                    // add Annotation
                                                                                                                    /*
                                                                                                                    sql = "INSERT INTO Annotations(RefSpectraID, fragmentMZ, sumComposition, shortName) VALUES ((SELECT COUNT(*) FROM RefSpectra) + 1, " + massFragment + ", '" + chemFormFragment + "', @fragmentName)";
                                                                                                                    SQLiteParameter parameterName = new SQLiteParameter("@fragmentName", System.Data.DbType.String);
                                                                                                                    parameterName.Value = fragment.fragmentName;
                                                                                                                    command.CommandText = sql;
                                                                                                                    command.Parameters.Add(parameterName);
                                                                                                                    command.ExecuteNonQuery();
                                                                                                                    */
                                                                                                                }
                                                                                                            }
                                                    
                                                    
                                                                                                            int numFragments = valuesMZ.Count;
                                                                                                            double[] valuesMZArray = new double[numFragments];
                                                                                                            float[] valuesIntens = new float[numFragments];
                                                                                                            for(int j = 0; j < numFragments; ++j)
                                                                                                            {
                                                                                                                valuesMZArray[j] = (double)valuesMZ[j];
                                                                                                                valuesIntens[j] = 100 * (float)((double)valuesIntensity[j]);
                                                                                                            }
                                                                                                            
                                                                                                            
                                                                                                            // add MS1 information
                                                                                                            sql = "INSERT INTO RefSpectra (moleculeName, precursorMZ, precursorCharge, precursorAdduct, prevAA, nextAA, copies, numPeaks, driftTimeMsec, collisionalCrossSectionSqA, driftTimeHighEnergyOffsetMsec, retentionTime, fileID, SpecIDinFile, score, scoreType, inchiKey, otherKeys, peptideSeq, peptideModSeq, chemicalFormula) VALUES('" + key + "', " + mass + ", " + charge + ", '" + precursorAdduct + "', '-', '-', 0, " + numFragments + ", 0, 0, 0, 0, '0', 0, 1, 1, '', '', '', '',  '" + chemForm + "')";
                                                                                                            command.CommandText = sql;
                                                                                                            command.ExecuteNonQuery();
                                                                                                            
                                                                                                            // add spectrum
                                                                                                            command.CommandText = "INSERT INTO RefSpectraPeaks(RefSpectraID, peakMZ, peakIntensity) VALUES((SELECT MAX(id) FROM RefSpectra), @mzvalues, @intensvalues)";
                                                                                                            SQLiteParameter parameterMZ = new SQLiteParameter("@mzvalues", System.Data.DbType.Binary);
                                                                                                            SQLiteParameter parameterIntens = new SQLiteParameter("@intensvalues", System.Data.DbType.Binary);
                                                                                                            parameterMZ.Value = Compressing.Compress(valuesMZArray);
                                                                                                            parameterIntens.Value = Compressing.Compress(valuesIntens);
                                                                                                            command.Parameters.Add(parameterMZ);
                                                                                                            command.Parameters.Add(parameterIntens);
                                                                                                            command.ExecuteNonQuery();
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // check if more than one fatty acids are 0:0
                int checkFattyAcids = 0;
                string sql;
                checkFattyAcids += fag1.faTypes["FAx"] ? 1 : 0;
                checkFattyAcids += fag2.faTypes["FAx"] ? 1 : 0;
                if (checkFattyAcids > 0) return;
                if (hgValues.Count == 0) return;
                int isPlamalogen = 0;
                
                foreach (int fattyAcidLength1 in fag1.carbonCounts)
                {
                    int maxDoubleBond1 = (fattyAcidLength1 - 1) >> 1;
                    foreach (int fattyAcidDoubleBond1 in fag1.doubleBondCounts)
                    {
                        foreach (int fattyAcidHydroxyl1 in fag1.hydroxylCounts)
                        {
                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair1 in fag1.faTypes)
                            {
                                if (fattyAcidKeyValuePair1.Value && maxDoubleBond1 >= fattyAcidDoubleBond1)
                                {
                                    FattyAcid fa1 = new FattyAcid(fattyAcidLength1, fattyAcidDoubleBond1, fattyAcidHydroxyl1, fattyAcidKeyValuePair1.Key);
                                    if (fattyAcidKeyValuePair1.Key == "FAx")
                                    {
                                        fa1 = new FattyAcid(0, 0, 0, "FA");
                                    }
                                    if (fattyAcidKeyValuePair1.Key == "FAp")
                                    {
                                        isPlamalogen += 1;
                                    }
                                    foreach (int fattyAcidLength2 in fag2.carbonCounts)
                                    {
                                        int maxDoubleBond2 = (fattyAcidLength2 - 1) >> 1;
                                        foreach (int fattyAcidDoubleBond2 in fag2.doubleBondCounts)
                                        {
                                            foreach (int fattyAcidHydroxyl2 in fag2.hydroxylCounts)
                                            {
                                                foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair2 in fag2.faTypes)
                                                {
                                                    if (fattyAcidKeyValuePair2.Value && maxDoubleBond2 >= fattyAcidDoubleBond2)
                                                    {
                                                        FattyAcid fa2 = new FattyAcid(fattyAcidLength2, fattyAcidDoubleBond2, fattyAcidHydroxyl2, fattyAcidKeyValuePair2.Key);
                                                        if (fattyAcidKeyValuePair2.Key == "FAx")
                                                        {
                                                            fa2 = new FattyAcid(0, 0, 0, "FA");
                                                        }
                                                        if (fattyAcidKeyValuePair2.Key == "FAp")
                                                        {
                                                            isPlamalogen += 1;
                                                        } 
                                                                            
                                                        List<FattyAcid> sortedAcids = new List<FattyAcid>();
                                                        sortedAcids.Add(fa1);
                                                        sortedAcids.Add(fa2);
                                                        sortedAcids.Sort();
                                                        
                                                        foreach(int hgValue in hgValues)
                                                        {
                                                        
                                                            String headgroup = headGroupNames[hgValue];
                                                            String headgroupSearch = headgroup;
                                                            String key = headgroup + " ";
                                                            if (headgroup.Equals("PC") || headgroup.Equals("PE"))
                                                            {
                                                                if (isPlamalogen >= 1) headgroupSearch = "p" + headgroupSearch;
                                                                if (isPlamalogen == 2) headgroupSearch = "p" + headgroupSearch;
                                                            }
                                                            int i = 0;
                                                            foreach (FattyAcid fa in sortedAcids)
                                                            {
                                                                if (fa.length > 0){
                                                                    if (i++ > 0) key += "_";
                                                                    key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                                                    if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                                                    key += fa.suffix;
                                                                }
                                                            }
                                                            
                                                            
                                                            foreach (KeyValuePair<string, bool> adduct in adducts)
                                                            {
                                                                if (adduct.Value)
                                                                {
                                                                    String keyAdduct = key + " " + adduct.Key;
                                                                    String precursorAdduct = "[M" + adduct.Key + "]";
                                                                    if (!usedKeys.Contains(keyAdduct))
                                                                    {
                                                                        usedKeys.Add(keyAdduct);
                                                            
                                                                        
                                                                        DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                                        MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                                                        MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                                                        MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroupSearch]);
                                                                        String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                        int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                                        double mass = LipidCreatorForm.computeMass(atomsCount, charge) / (double)(Math.Abs(charge));                                                
                                                    
                                                                        ArrayList valuesMZ = new ArrayList();
                                                                        ArrayList valuesIntensity = new ArrayList();
                                                                        
                                                                        foreach (MS2Fragment fragment in MS2Fragments[headgroupSearch])
                                                                        {
                                                                            if (((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                                                            {
                                                                                DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                                                foreach (string fbase in fragment.fragmentBase)
                                                                                {
                                                                                    switch(fbase)
                                                                                    {
                                                                                        case "FA1":
                                                                                            MS2Fragment.addCounts(atomsCountFragment, fa1.atomsCount);
                                                                                            break;
                                                                                        case "FA2":
                                                                                            MS2Fragment.addCounts(atomsCountFragment, fa2.atomsCount);
                                                                                            break;
                                                                                        case "PRE":
                                                                                            MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                                            break;
                                                                                        default:
                                                                                            break;
                                                                                    }
                                                                                }
                                                                                //String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                                                //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                                                double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge) / (double)(Math.Abs(fragment.fragmentCharge));
                                                            
                                                                                valuesMZ.Add(massFragment);
                                                                                valuesIntensity.Add(fragment.intensity);
                                                                                
                                                                                
                                                                                // add Annotation
                                                                                /*
                                                                                sql = "INSERT INTO Annotations(RefSpectraID, fragmentMZ, sumComposition, shortName) VALUES ((SELECT COUNT(*) FROM RefSpectra) + 1, " + massFragment + ", '" + chemFormFragment + "', @fragmentName)";
                                                                                SQLiteParameter parameterName = new SQLiteParameter("@fragmentName", System.Data.DbType.String);
                                                                                parameterName.Value = fragment.fragmentName;
                                                                                command.CommandText = sql;
                                                                                command.Parameters.Add(parameterName);
                                                                                command.ExecuteNonQuery();
                                                                                */
                                                                            }
                                                                        }
                                                    
                                                                        int numFragments = valuesMZ.Count;
                                                                        double[] valuesMZArray = new double[numFragments];
                                                                        float[] valuesIntens = new float[numFragments];
                                                                        for(int j = 0; j < numFragments; ++j)
                                                                        {
                                                                            valuesMZArray[j] = (double)valuesMZ[j];
                                                                            valuesIntens[j] = 100 * (float)((double)valuesIntensity[j]);
                                                                        }
                                                                        
                                                                        
                                                                        // add MS1 information
                                                                        sql = "INSERT INTO RefSpectra (moleculeName, precursorMZ, precursorCharge, precursorAdduct, prevAA, nextAA, copies, numPeaks, driftTimeMsec, collisionalCrossSectionSqA, driftTimeHighEnergyOffsetMsec, retentionTime, fileID, SpecIDinFile, score, scoreType, inchiKey, otherKeys, peptideSeq, peptideModSeq, chemicalFormula) VALUES('" + key + "', " + mass + ", " + charge + ", '" + precursorAdduct + "', '-', '-', 0, " + numFragments + ", 0, 0, 0, 0, '0', 0, 1, 1, '', '', '', '',  '" + chemForm + "')";
                                                                        command.CommandText = sql;
                                                                        command.ExecuteNonQuery();
                                                                        
                                                                        // add spectrum
                                                                        command.CommandText = "INSERT INTO RefSpectraPeaks(RefSpectraID, peakMZ, peakIntensity) VALUES((SELECT MAX(id) FROM RefSpectra), @mzvalues, @intensvalues)";
                                                                        SQLiteParameter parameterMZ = new SQLiteParameter("@mzvalues", System.Data.DbType.Binary);
                                                                        SQLiteParameter parameterIntens = new SQLiteParameter("@intensvalues", System.Data.DbType.Binary);
                                                                        parameterMZ.Value = Compressing.Compress(valuesMZArray);
                                                                        parameterIntens.Value = Compressing.Compress(valuesIntens);
                                                                        command.Parameters.Add(parameterMZ);
                                                                        command.Parameters.Add(parameterIntens);
                                                                        command.ExecuteNonQuery();
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [Serializable]
    public class SLLipid : Lipid
    {
        public List<string> headGroupNames = new List<string>{"Cer", "CerP", "GB3Cer", "GB4Cer", "GD3Cer", "GM3Cer", "GM4Cer", "HexCer", "HexCerS", "LacCer", "MIPCer", "MIP2Cer", "PECer", "PICer", "SM", "SPC", "SPH", "SPH-P"};
        public List<int> hgValues;
        public fattyAcidGroup fag;
        public fattyAcidGroup lcb;       
        public int longChainBaseHydroxyl;        
        public int fattyAcidHydroxyl;
    
        public SLLipid(Dictionary<String, String> allPaths, Dictionary<String, ArrayList> allFragments)
        {
            lcb = new fattyAcidGroup();
            fag = new fattyAcidGroup();
            hgValues = new List<int>();
            longChainBaseHydroxyl = 2;
            fattyAcidHydroxyl = 0;
            MS2Fragments.Add("Cer", new ArrayList());
            MS2Fragments.Add("CerP", new ArrayList());
            MS2Fragments.Add("GB3Cer", new ArrayList());
            MS2Fragments.Add("GB4Cer", new ArrayList());
            MS2Fragments.Add("GD3Cer", new ArrayList());
            MS2Fragments.Add("GM3Cer", new ArrayList());
            MS2Fragments.Add("GM4Cer", new ArrayList());
            MS2Fragments.Add("HexCer", new ArrayList());
            MS2Fragments.Add("HexCerS", new ArrayList());
            MS2Fragments.Add("LacCer", new ArrayList());
            MS2Fragments.Add("MIPCer", new ArrayList());
            MS2Fragments.Add("MIP2Cer", new ArrayList());
            MS2Fragments.Add("PECer", new ArrayList());
            MS2Fragments.Add("PICer", new ArrayList());
            MS2Fragments.Add("SM", new ArrayList());
            MS2Fragments.Add("SPC", new ArrayList());
            MS2Fragments.Add("SPH", new ArrayList());
            MS2Fragments.Add("SPH-P", new ArrayList());
            adducts["+H"] = true;
            adducts["-H"] = false;
            
            
            foreach(KeyValuePair<String, ArrayList> kvp in MS2Fragments)
            {
                if (allPaths.ContainsKey(kvp.Key)) pathsToFullImage.Add(kvp.Key, allPaths[kvp.Key]);
                if (allFragments != null && allFragments.ContainsKey(kvp.Key))
                {
                    foreach (MS2Fragment fragment in allFragments[kvp.Key])
                    {
                        MS2Fragments[kvp.Key].Add(new MS2Fragment(fragment));
                    }
                }
            }
        }
    
        public SLLipid(SLLipid copy) : base((Lipid)copy)
        {
            lcb = new fattyAcidGroup(copy.lcb);
            fag = new fattyAcidGroup(copy.fag);
            longChainBaseHydroxyl = copy.longChainBaseHydroxyl;
            fattyAcidHydroxyl = copy.fattyAcidHydroxyl;
            hgValues = new List<int>();
            foreach (int hgValue in copy.hgValues)
            {
                hgValues.Add(hgValue);
            }
        }
        
        
        public override string serialize()
        {
            string xml = "<lipid type=\"SL\">\n";
            xml += lcb.serialize();
            xml += fag.serialize();
            xml += "<lcbHydroxyValue>" + longChainBaseHydroxyl + "</lcbHydroxyValue>\n";
            xml += "<faHydroxyValue>" + fattyAcidHydroxyl + "</faHydroxyValue>\n";
            foreach (int hgValue in hgValues)
            {
                xml += "<headGroup>" + hgValue + "</headGroup>\n";
            }
            xml += base.serialize();
            xml += "</lipid>\n";
            return xml;
        }
        
        public override void import(XElement node)
        {
            int fattyAcidCounter = 0;
            hgValues.Clear();
            foreach (XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "fattyAcidGroup":
                        if (fattyAcidCounter == 0)
                        {
                            lcb.import(child);
                        }
                        else if (fattyAcidCounter == 1)
                        {
                            fag.import(child);
                        }
                        else
                        {   
                            Console.WriteLine("Error, fatty acid");
                            throw new Exception();
                        }
                        ++fattyAcidCounter;
                        break;
                        
                    case "lcbHydroxyValue":
                        longChainBaseHydroxyl = Convert.ToInt32(child.Value.ToString());
                        break;
                        
                    case "faHydroxyValue":
                        fattyAcidHydroxyl = Convert.ToInt32(child.Value.ToString());
                        break;
                        
                    case "headGroup":
                        hgValues.Add(Convert.ToInt32(child.Value.ToString()));
                        break;
                        
                        
                    default:
                        base.import(child);
                        break;
                }
            }
        }
        
        
        public override void addLipids(DataTable allLipids, DataTable allLipidsUnique, Dictionary<String, DataTable> headGroupsTable, Dictionary<String, Dictionary<String, bool>> headgroupAdductRestrictions, HashSet<String> usedKeys, HashSet<String> replicates)
        {
            foreach (int longChainBaseLength in lcb.carbonCounts)
            {
                int maxDoubleBond1 = (longChainBaseLength - 1) >> 1;
                foreach (int longChainBaseDoubleBond in lcb.doubleBondCounts)
                {
                    if (maxDoubleBond1 < longChainBaseDoubleBond) continue;
                    FattyAcid lcbType = new FattyAcid(longChainBaseLength, longChainBaseDoubleBond, longChainBaseHydroxyl, true);
                    foreach (int hgValue in hgValues)
                    {
                        String headgroup = headGroupNames[hgValue];
                        if (headgroup != "SPH" && headgroup != "SPH-P" && headgroup != "SPC") // sphingolipids without fatty acid
                        {
                            foreach (int fattyAcidLength in fag.carbonCounts)
                            {
                                if (fattyAcidLength < fattyAcidHydroxyl + 2) continue;
                                int maxDoubleBond2 = (fattyAcidLength - 1) >> 1;
                                foreach (int fattyAcidDoubleBond2 in fag.doubleBondCounts)
                                {
                                    if (maxDoubleBond2 < fattyAcidDoubleBond2) continue;
                                    FattyAcid fa = new FattyAcid(fattyAcidLength, fattyAcidDoubleBond2, fattyAcidHydroxyl, "FA");
                        
                        
                                    String key = headgroup + " ";
                                    
                                    key += Convert.ToString(longChainBaseLength) + ":" + Convert.ToString(longChainBaseDoubleBond) + ";" + Convert.ToString(longChainBaseHydroxyl);
                                    key += "/";
                                    key += Convert.ToString(fattyAcidLength) + ":" + Convert.ToString(fattyAcidDoubleBond2);
                                    if (fattyAcidHydroxyl > 0) key += ";" + Convert.ToString(fattyAcidHydroxyl);

                                    if (!usedKeys.Contains(key))
                                    {
                                        foreach (KeyValuePair<string, bool> adduct in adducts)
                                        {
                                            if (adduct.Value && headgroupAdductRestrictions[headgroup][adduct.Key])
                                            {
                                                usedKeys.Add(key);
                                                
                                                DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                MS2Fragment.addCounts(atomsCount, fa.atomsCount);
                                                MS2Fragment.addCounts(atomsCount, lcbType.atomsCount);
                                                // do not change the order, chem formula must be computed before adding the adduct
                                                string chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                string chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                double mass = LipidCreatorForm.computeMass(atomsCount, charge);                                                
                                                
                                                
                                                foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                                {
                                                    if (fragment.fragmentSelected && ((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                                    {
                                                        DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                        foreach (string fbase in fragment.fragmentBase)
                                                        {
                                                            switch(fbase)
                                                            {
                                                                case "LCB":
                                                                    MS2Fragment.addCounts(atomsCountFragment, lcbType.atomsCount);
                                                                    break;
                                                                case "FA":
                                                                    MS2Fragment.addCounts(atomsCountFragment, fa.atomsCount);
                                                                    break;
                                                                case "PRE":
                                                                    MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                        // some exceptional if conditions
                                                        if (adduct.Key != "-H" && charge < 0 && (headgroup == "HexCer" || headgroup == "LacCer") && (fragment.fragmentName == "Y0" || fragment.fragmentName == "Y1" || fragment.fragmentName == "Z0" || fragment.fragmentName == "Z1"))
                                                        {
                                                            subtractAdduct(atomsCountFragment, adduct.Key);
                                                        }
                                                        String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                        //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                        double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge);
                                                        
                                                    
                                                        DataRow lipidRow = allLipids.NewRow();
                                                        lipidRow["Molecule List Name"] = headgroup;
                                                        lipidRow["Precursor Name"] = key;
                                                        lipidRow["Precursor Ion Formula"] = chemForm;
                                                        lipidRow["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                        lipidRow["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                        lipidRow["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                        lipidRow["Product Name"] = fragment.fragmentName;
                                                        lipidRow["Product Ion Formula"] = chemFormFragment;
                                                        lipidRow["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                        lipidRow["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                        allLipids.Rows.Add(lipidRow);
                                                                                                                
                                                        String replicatesKey = chemFormComplete + "/" + chemFormFragment;
                                                        if (!replicates.Contains(replicatesKey))
                                                        {
                                                            replicates.Add(replicatesKey);
                                                            DataRow lipidRowUnique = allLipidsUnique.NewRow();
                                                            lipidRowUnique["Molecule List Name"] = headgroup;
                                                            lipidRowUnique["Precursor Name"] = key;
                                                            lipidRowUnique["Precursor Ion Formula"] = chemForm;
                                                            lipidRowUnique["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                            lipidRowUnique["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                            lipidRowUnique["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                            lipidRowUnique["Product Name"] = fragment.fragmentName;
                                                            lipidRowUnique["Product Ion Formula"] = chemFormFragment;
                                                            lipidRowUnique["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                            lipidRowUnique["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                            allLipidsUnique.Rows.Add(lipidRowUnique);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            String key = headgroup + " " + Convert.ToString(longChainBaseLength) + ":" + Convert.ToString(longChainBaseDoubleBond) + ";" + Convert.ToString(longChainBaseHydroxyl);

                            if (!usedKeys.Contains(key))
                            {
                                foreach (KeyValuePair<string, bool> adduct in adducts)
                                {
                                    if (adduct.Value && headgroupAdductRestrictions[headgroup][adduct.Key])
                                    {
                                        usedKeys.Add(key);
                                        
                                        DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                        MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                        MS2Fragment.addCounts(atomsCount, lcbType.atomsCount);
                                        // do not change the order, chem formula must be computed before adding the adduct
                                        String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                        int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                        //String chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                        double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                                        
                                        foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                        {
                                            // Special cases that are to few to be put in own handling, thus added here as if condidions
                                            if (headgroup == "SPH" && longChainBaseDoubleBond > 0 && fragment.fragmentName == "HG") continue;
                                        
                                        
                                            if (fragment.fragmentSelected && ((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                            {
                                                DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                foreach (string fbase in fragment.fragmentBase)
                                                {
                                                    switch(fbase)
                                                    {
                                                        case "LCB":
                                                            MS2Fragment.addCounts(atomsCountFragment, lcbType.atomsCount);
                                                            break;
                                                        case "PRE":
                                                            MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                                String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge);
                                                
                                            
                                                DataRow lipidRow = allLipids.NewRow();
                                                lipidRow["Molecule List Name"] = headgroup;
                                                lipidRow["Precursor Name"] = key;
                                                lipidRow["Precursor Ion Formula"] = chemForm;
                                                lipidRow["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                lipidRow["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                lipidRow["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                lipidRow["Product Name"] = fragment.fragmentName;
                                                lipidRow["Product Ion Formula"] = chemFormFragment;
                                                lipidRow["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                lipidRow["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                allLipids.Rows.Add(lipidRow);
                                                                                                                
                                                String replicatesKey = chemForm + "/" + chemFormFragment;
                                                if (!replicates.Contains(replicatesKey))
                                                {
                                                    replicates.Add(replicatesKey);
                                                    DataRow lipidRowUnique = allLipidsUnique.NewRow();
                                                    lipidRowUnique["Molecule List Name"] = headgroup;
                                                    lipidRowUnique["Precursor Name"] = key;
                                                    lipidRowUnique["Precursor Ion Formula"] = chemForm;
                                                    lipidRowUnique["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                    lipidRowUnique["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                    lipidRowUnique["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                    lipidRowUnique["Product Name"] = fragment.fragmentName;
                                                    lipidRowUnique["Product Ion Formula"] = chemFormFragment;
                                                    lipidRowUnique["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                    lipidRowUnique["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                    allLipidsUnique.Rows.Add(lipidRowUnique);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        
        public override void addSpectrum(SQLiteCommand command, Dictionary<String, DataTable> headGroupsTable, HashSet<String> usedKeys)
        {
            String sql;
            foreach (int longChainBaseLength in lcb.carbonCounts)
            {
                int maxDoubleBond1 = (longChainBaseLength - 1) >> 1;
                foreach (int longChainBaseDoubleBond in lcb.doubleBondCounts)
                {
                    if (maxDoubleBond1 < longChainBaseDoubleBond) continue;
                    FattyAcid lcbType = new FattyAcid(longChainBaseLength, longChainBaseDoubleBond, longChainBaseHydroxyl, true);
                    foreach (int hgValue in hgValues)
                    {
                        String headgroup = headGroupNames[hgValue];
                        if (headgroup != "SPH" && headgroup != "SPH-P" && headgroup != "SPC") // sphingolipids without fatty acid
                        {
                            foreach (int fattyAcidLength in fag.carbonCounts)
                            {
                                if (fattyAcidLength < fattyAcidHydroxyl + 2) continue;
                                int maxDoubleBond2 = (fattyAcidLength - 1) >> 1;
                                foreach (int fattyAcidDoubleBond2 in fag.doubleBondCounts)
                                {
                                    if (maxDoubleBond2 < fattyAcidDoubleBond2) continue;
                                    FattyAcid fa = new FattyAcid(fattyAcidLength, fattyAcidDoubleBond2, fattyAcidHydroxyl, "FA");
                        
                        
                                    String key = headgroup + " ";
                                    
                                    key += Convert.ToString(longChainBaseLength) + ":" + Convert.ToString(longChainBaseDoubleBond) + ";" + Convert.ToString(longChainBaseHydroxyl);
                                    key += "/";
                                    key += Convert.ToString(fattyAcidLength) + ":" + Convert.ToString(fattyAcidDoubleBond2);
                                    if (fattyAcidHydroxyl > 0) key += ";" + Convert.ToString(fattyAcidHydroxyl);
                                    

                                    foreach (KeyValuePair<string, bool> adduct in adducts)
                                    {
                                        if (adduct.Value)
                                        {
                                            String keyAdduct = key + " " + adduct.Key;
                                            String precursorAdduct = "[M" + adduct.Key + "]";
                                            if (!usedKeys.Contains(keyAdduct))
                                            {
                                                usedKeys.Add(keyAdduct);
                                                
                                                DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                MS2Fragment.addCounts(atomsCount, fa.atomsCount);
                                                MS2Fragment.addCounts(atomsCount, lcbType.atomsCount);
                                                String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                // do not change the order, chem formula must be computed before adding the adduct
                                                int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                double mass = LipidCreatorForm.computeMass(atomsCount, charge) / (double)(Math.Abs(charge));                                                
                                                
                                                ArrayList valuesMZ = new ArrayList();
                                                ArrayList valuesIntensity = new ArrayList();
                                                
                                                foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                                {
                                                    if (((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                                    {
                                                        DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                        foreach (string fbase in fragment.fragmentBase)
                                                        {
                                                            switch(fbase)
                                                            {
                                                                case "LCB":
                                                                    MS2Fragment.addCounts(atomsCountFragment, lcbType.atomsCount);
                                                                    break;
                                                                case "FA":
                                                                    MS2Fragment.addCounts(atomsCountFragment, fa.atomsCount);
                                                                    break;
                                                                case "PRE":
                                                                    MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                        // some exceptional if conditions
                                                        if (adduct.Key != "-H" && charge < 0 && (headgroup == "HexCer" || headgroup == "LacCer") && (fragment.fragmentName == "Y0" || fragment.fragmentName == "Y1" || fragment.fragmentName == "Z0" || fragment.fragmentName == "Z1"))
                                                        {
                                                            subtractAdduct(atomsCountFragment, adduct.Key);
                                                        }
                                                        //String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                        //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                        double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge) / (double)(Math.Abs(fragment.fragmentCharge));
                                                        
                                                        valuesMZ.Add(massFragment);
                                                        valuesIntensity.Add(fragment.intensity);
                                                        
                                                        // add Annotation
                                                        /*
                                                        sql = "INSERT INTO Annotations(RefSpectraID, fragmentMZ, sumComposition, shortName) VALUES ((SELECT COUNT(*) FROM RefSpectra) + 1, " + massFragment + ", '" + chemFormFragment + "', @fragmentName)";
                                                        SQLiteParameter parameterName = new SQLiteParameter("@fragmentName", System.Data.DbType.String);
                                                        parameterName.Value = fragment.fragmentName;
                                                        command.CommandText = sql;
                                                        command.Parameters.Add(parameterName);
                                                        command.ExecuteNonQuery();
                                                        */
                                                        
                                                    }
                                                }
                                                
                                                
                                                int numFragments = valuesMZ.Count;
                                                double[] valuesMZArray = new double[numFragments];
                                                float[] valuesIntens = new float[numFragments];
                                                for(int i = 0; i < numFragments; ++i)
                                                {
                                                    valuesMZArray[i] = (double)valuesMZ[i];
                                                    valuesIntens[i] = 100 * (float)((double)valuesIntensity[i]);
                                                }
                                                
                                                
                                                // add MS1 information
                                                sql = "INSERT INTO RefSpectra (moleculeName, precursorMZ, precursorCharge, precursorAdduct, prevAA, nextAA, copies, numPeaks, driftTimeMsec, collisionalCrossSectionSqA, driftTimeHighEnergyOffsetMsec, retentionTime, fileID, SpecIDinFile, score, scoreType, inchiKey, otherKeys, peptideSeq, peptideModSeq, chemicalFormula) VALUES('" + key + "', " + mass + ", " + charge + ", '" + precursorAdduct + "', '-', '-', 0, " + numFragments + ", 0, 0, 0, 0, '0', 0, 1, 1, '', '', '', '',  '" + chemForm + "')";
                                                command.CommandText = sql;
                                                command.ExecuteNonQuery();
                                                
                                                // add spectrum
                                                command.CommandText = "INSERT INTO RefSpectraPeaks(RefSpectraID, peakMZ, peakIntensity) VALUES((SELECT MAX(id) FROM RefSpectra), @mzvalues, @intensvalues)";
                                                SQLiteParameter parameterMZ = new SQLiteParameter("@mzvalues", System.Data.DbType.Binary);
                                                SQLiteParameter parameterIntens = new SQLiteParameter("@intensvalues", System.Data.DbType.Binary);
                                                parameterMZ.Value = Compressing.Compress(valuesMZArray);
                                                parameterIntens.Value = Compressing.Compress(valuesIntens);
                                                command.Parameters.Add(parameterMZ);
                                                command.Parameters.Add(parameterIntens);
                                                command.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            String key = headgroup + " " + Convert.ToString(longChainBaseLength) + ":" + Convert.ToString(longChainBaseDoubleBond) + ";" + Convert.ToString(longChainBaseHydroxyl);

                            foreach (KeyValuePair<string, bool> adduct in adducts)
                            {
                                if (adduct.Value)
                                {
                                    String keyAdduct = key + " " + adduct.Key;
                                    String precursorAdduct = "[M" + adduct.Key + "]";
                                    if (!usedKeys.Contains(keyAdduct))
                                    {
                                        usedKeys.Add(keyAdduct);
                                        
                                        DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                        MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                        MS2Fragment.addCounts(atomsCount, lcbType.atomsCount);
                                        String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                        // do not change the order, chem formula must be computed before adding the adduct
                                        int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                        double mass = LipidCreatorForm.computeMass(atomsCount, charge) / (double)(Math.Abs(charge));                                                
                                        
                                        ArrayList valuesMZ = new ArrayList();
                                        ArrayList valuesIntensity = new ArrayList();
                                        
                                        foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                        {
                                            // Special cases that are to few to be put in own handling, thus added here as if condidions
                                            if (headgroup == "SPH" && longChainBaseDoubleBond > 0 && fragment.fragmentName == "HG") continue;
                                        
                                        
                                            if (((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                            {
                                                DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                foreach (string fbase in fragment.fragmentBase)
                                                {
                                                    switch(fbase)
                                                    {
                                                        case "LCB":
                                                            MS2Fragment.addCounts(atomsCountFragment, lcbType.atomsCount);
                                                            break;
                                                        case "PRE":
                                                            MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                                //String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge) / (double)(Math.Abs(fragment.fragmentCharge));
                                                        
                                                valuesMZ.Add(massFragment);
                                                valuesIntensity.Add(fragment.intensity);
                                                
                                                
                                                // add Annotation
                                                /*
                                                sql = "INSERT INTO Annotations(RefSpectraID, fragmentMZ, sumComposition, shortName) VALUES ((SELECT COUNT(*) FROM RefSpectra) + 1, " + massFragment + ", '" + chemFormFragment + "', @fragmentName)";
                                                SQLiteParameter parameterName = new SQLiteParameter("@fragmentName", System.Data.DbType.String);
                                                parameterName.Value = fragment.fragmentName;
                                                command.CommandText = sql;
                                                command.Parameters.Add(parameterName);
                                                command.ExecuteNonQuery();
                                                */
                                            }
                                        }
                                                
                                                
                                        int numFragments = valuesMZ.Count;
                                        double[] valuesMZArray = new double[numFragments];
                                        float[] valuesIntens = new float[numFragments];
                                        for(int i = 0; i < numFragments; ++i)
                                        {
                                            valuesMZArray[i] = (double)valuesMZ[i];
                                            valuesIntens[i] = 100 * (float)((double)valuesIntensity[i]);
                                        }
                                        
                                        
                                        // add MS1 information
                                        sql = "INSERT INTO RefSpectra (moleculeName, precursorMZ, precursorCharge, precursorAdduct, prevAA, nextAA, copies, numPeaks, driftTimeMsec, collisionalCrossSectionSqA, driftTimeHighEnergyOffsetMsec, retentionTime, fileID, SpecIDinFile, score, scoreType, inchiKey, otherKeys, peptideSeq, peptideModSeq, chemicalFormula) VALUES('" + key + "', " + mass + ", " + charge + ", '" + precursorAdduct + "', '-', '-', 0, " + numFragments + ", 0, 0, 0, 0, '0', 0, 1, 1, '', '', '', '',  '" + chemForm + "')";
                                        command.CommandText = sql;
                                        command.ExecuteNonQuery();
                                        
                                        // add spectrum
                                        command.CommandText = "INSERT INTO RefSpectraPeaks(RefSpectraID, peakMZ, peakIntensity) VALUES((SELECT MAX(id) FROM RefSpectra), @mzvalues, @intensvalues)";
                                        SQLiteParameter parameterMZ = new SQLiteParameter("@mzvalues", System.Data.DbType.Binary);
                                        SQLiteParameter parameterIntens = new SQLiteParameter("@intensvalues", System.Data.DbType.Binary);
                                        parameterMZ.Value = Compressing.Compress(valuesMZArray);
                                        parameterIntens.Value = Compressing.Compress(valuesIntens);
                                        command.Parameters.Add(parameterMZ);
                                        command.Parameters.Add(parameterIntens);
                                        command.ExecuteNonQuery();
                                        
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    
    
    [Serializable]
    public class Cholesterol : Lipid
    {
        public bool containsEster;
        public fattyAcidGroup fag;
        public List<String> headGroupNames = new List<String>{"Ch", "ChE"};
    
    
        public Cholesterol(Dictionary<String, String> allPaths, Dictionary<String, ArrayList> allFragments)
        {
            fag = new fattyAcidGroup();
            containsEster = false;
            MS2Fragments.Add("Ch", new ArrayList());
            MS2Fragments.Add("ChE", new ArrayList());
            adducts["+NH4"] = true;
            adducts["-H"] = false;
            
            foreach(KeyValuePair<String, ArrayList> kvp in MS2Fragments)
            {
                if (allPaths.ContainsKey(kvp.Key)) pathsToFullImage.Add(kvp.Key, allPaths[kvp.Key]);
                if (allFragments != null && allFragments.ContainsKey(kvp.Key))
                {
                    foreach (MS2Fragment fragment in allFragments[kvp.Key])
                    {
                        MS2Fragments[kvp.Key].Add(new MS2Fragment(fragment));
                    }
                }
            }
        }
    
        public Cholesterol(Cholesterol copy) : base((Lipid)copy) 
        {
            fag = new fattyAcidGroup(copy.fag);
            containsEster = copy.containsEster;
            
        }
        
        
        public override string serialize()
        {
            string xml = "<lipid type=\"Cholesterol\" containsEster=\"" + containsEster + "\">\n";
            xml += fag.serialize();
            xml += base.serialize();
            xml += "</lipid>\n";
            return xml;
        }
        
        
        public override void import(XElement node)
        {
            int fattyAcidCounter = 0;
            containsEster = node.Attribute("containsEster").Value == "True";
            foreach (XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "fattyAcidGroup":
                        if (fattyAcidCounter == 0)
                        {
                            fag.import(child);
                        }
                        else
                        {   
                            Console.WriteLine("Error, fatty acid");
                            throw new Exception();
                        }
                        ++fattyAcidCounter;
                        break;                        
                        
                    default:
                        base.import(child);
                        break;
                }
            }
        }
        
        
    
        
        public override void addLipids(DataTable allLipids, DataTable allLipidsUnique, Dictionary<String, DataTable> headGroupsTable, Dictionary<String, Dictionary<String, bool>> headgroupAdductRestrictions, HashSet<String> usedKeys, HashSet<String> replicates)
        {
            if (containsEster)
            {
                foreach (int fattyAcidLength in fag.carbonCounts)
                {
                    int maxDoubleBond = (fattyAcidLength - 1) >> 1;
                    foreach (int fattyAcidDoubleBond in fag.doubleBondCounts)
                    {
                        foreach (int fattyAcidHydroxyl in fag.hydroxylCounts)
                        {
                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair in fag.faTypes)
                            {
                                if (fattyAcidKeyValuePair.Value && maxDoubleBond >= fattyAcidDoubleBond)
                                {
                                    FattyAcid fa = new FattyAcid(fattyAcidLength, fattyAcidDoubleBond, fattyAcidHydroxyl, fattyAcidKeyValuePair.Key);
                                    
                                    String headgroup = "ChE";
                                    String key = headgroup + " ";
                                    key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                    if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                    key += fa.suffix;
                                    if (!usedKeys.Contains(key))
                                    {
                                    
                                        foreach (KeyValuePair<string, bool> adduct in adducts)
                                        {
                                            if (adduct.Value && headgroupAdductRestrictions[headgroup][adduct.Key])
                                            {
                                                usedKeys.Add(key);
                                                
                                                DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                MS2Fragment.addCounts(atomsCount, fa.atomsCount);
                                                MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                String chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                                                
                                                foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                                {
                                                    if (fragment.fragmentSelected && ((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)))
                                                    {
                                                        DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                        
                                                        String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                        //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                        double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge);
                                                        
                                                    
                                                        DataRow lipidRow = allLipids.NewRow();
                                                        lipidRow["Molecule List Name"] = headgroup;
                                                        lipidRow["Precursor Name"] = key;
                                                        lipidRow["Precursor Ion Formula"] = chemForm;
                                                        lipidRow["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                        lipidRow["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                        lipidRow["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                        lipidRow["Product Name"] = fragment.fragmentName;
                                                        lipidRow["Product Ion Formula"] = chemFormFragment;
                                                        lipidRow["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                        lipidRow["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                        allLipids.Rows.Add(lipidRow);
                                                        
                                                        String replicatesKey = chemFormComplete + "/" + chemFormFragment;
                                                        if (!replicates.Contains(replicatesKey))
                                                        {
                                                            replicates.Add(replicatesKey);
                                                            DataRow lipidRowUnique = allLipidsUnique.NewRow();
                                                            lipidRowUnique["Molecule List Name"] = headgroup;
                                                            lipidRowUnique["Precursor Name"] = key;
                                                            lipidRowUnique["Precursor Ion Formula"] = chemForm;
                                                            lipidRowUnique["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                            lipidRowUnique["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                            lipidRowUnique["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                            lipidRowUnique["Product Name"] = fragment.fragmentName;
                                                            lipidRowUnique["Product Ion Formula"] = chemFormFragment;
                                                            lipidRowUnique["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                            lipidRowUnique["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                            allLipidsUnique.Rows.Add(lipidRowUnique);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                String headgroup = "Ch";
                String key = headgroup + " ";
                if (!usedKeys.Contains(key))
                {
                
                    foreach (KeyValuePair<string, bool> adduct in adducts)
                    {
                        if (adduct.Value && headgroupAdductRestrictions[headgroup][adduct.Key])
                        {
                            usedKeys.Add(key);
                            
                            DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                            MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                            String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                            int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                            String chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                            double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                            
                            foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                            {
                                if (fragment.fragmentSelected && ((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)))
                                {
                                    DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                    
                                    String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                    //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                    double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge);
                                    
                                
                                    DataRow lipidRow = allLipids.NewRow();
                                    lipidRow["Molecule List Name"] = headgroup;
                                    lipidRow["Precursor Name"] = key;
                                    lipidRow["Precursor Ion Formula"] = chemForm;
                                    lipidRow["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                    lipidRow["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                    lipidRow["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                    lipidRow["Product Name"] = fragment.fragmentName;
                                    lipidRow["Product Ion Formula"] = chemFormFragment;
                                    lipidRow["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                    lipidRow["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                    allLipids.Rows.Add(lipidRow);
                                    
                                    String replicatesKey = chemFormComplete + "/" + chemFormFragment;
                                    if (!replicates.Contains(replicatesKey))
                                    {
                                        replicates.Add(replicatesKey);
                                        DataRow lipidRowUnique = allLipidsUnique.NewRow();
                                        lipidRowUnique["Molecule List Name"] = headgroup;
                                        lipidRowUnique["Precursor Name"] = key;
                                        lipidRowUnique["Precursor Ion Formula"] = chemForm;
                                        lipidRowUnique["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                        lipidRowUnique["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                        lipidRowUnique["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                        lipidRowUnique["Product Name"] = fragment.fragmentName;
                                        lipidRowUnique["Product Ion Formula"] = chemFormFragment;
                                        lipidRowUnique["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                        lipidRowUnique["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                        allLipidsUnique.Rows.Add(lipidRowUnique);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public override void addSpectrum(SQLiteCommand command, Dictionary<String, DataTable> headGroupsTable, HashSet<String> usedKeys)
        {
        string sql;                
        if (containsEster){                
                foreach (int fattyAcidLength in fag.carbonCounts)
                {
                    int maxDoubleBond = (fattyAcidLength - 1) >> 1;
                    foreach (int fattyAcidDoubleBond in fag.doubleBondCounts)
                    {
                        foreach (int fattyAcidHydroxyl in fag.hydroxylCounts)
                        {
                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair in fag.faTypes)
                            {
                                if (fattyAcidKeyValuePair.Value && maxDoubleBond >= fattyAcidDoubleBond)
                                {
                                    FattyAcid fa = new FattyAcid(fattyAcidLength, fattyAcidDoubleBond, fattyAcidHydroxyl, fattyAcidKeyValuePair.Key);
                                    
                                    String headgroup = "ChE";
                                    String key = headgroup + " ";
                                    key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                    if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                    key += fa.suffix;
                                    
                                    foreach (KeyValuePair<string, bool> adduct in adducts)
                                    {
                                        if (adduct.Value)
                                        {
                                            String keyAdduct = key + " " + adduct.Key;
                                            String precursorAdduct = "[M" + adduct.Key + "]";
                                            if (!usedKeys.Contains(keyAdduct))
                                            {
                                                usedKeys.Add(keyAdduct);
                                                
                                                DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                MS2Fragment.addCounts(atomsCount, fa.atomsCount);
                                                MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                double mass = LipidCreatorForm.computeMass(atomsCount, charge) / (double)(Math.Abs(charge));                                                

                                                ArrayList valuesMZ = new ArrayList();
                                                ArrayList valuesIntensity = new ArrayList();
                                                
                                                foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                                {
                                                    if (((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)))
                                                    {
                                                        DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                        //String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                        //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                        double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge) / (double)(Math.Abs(fragment.fragmentCharge));

                                                        valuesMZ.Add(massFragment);
                                                        valuesIntensity.Add(fragment.intensity);
                                                        
                                                        
                                                        // add Annotation
                                                        /*
                                                        sql = "INSERT INTO Annotations(RefSpectraID, fragmentMZ, sumComposition, shortName) VALUES ((SELECT COUNT(*) FROM RefSpectra) + 1, " + massFragment + ", '" + chemFormFragment + "', @fragmentName)";
                                                        SQLiteParameter parameterName = new SQLiteParameter("@fragmentName", System.Data.DbType.String);
                                                        parameterName.Value = fragment.fragmentName;
                                                        command.CommandText = sql;
                                                        command.Parameters.Add(parameterName);
                                                        command.ExecuteNonQuery();
                                                        */
                                                    }
                                                }


                                                int numFragments = valuesMZ.Count;
                                                double[] valuesMZArray = new double[numFragments];
                                                float[] valuesIntens = new float[numFragments];
                                                for(int j = 0; j < numFragments; ++j)
                                                {
                                                    valuesMZArray[j] = (double)valuesMZ[j];
                                                    valuesIntens[j] = 100 * (float)((double)valuesIntensity[j]);
                                                }
                                                
                                                
                                                // add MS1 information
                                                sql = "INSERT INTO RefSpectra (moleculeName, precursorMZ, precursorCharge, precursorAdduct, prevAA, nextAA, copies, numPeaks, driftTimeMsec, collisionalCrossSectionSqA, driftTimeHighEnergyOffsetMsec, retentionTime, fileID, SpecIDinFile, score, scoreType, inchiKey, otherKeys, peptideSeq, peptideModSeq, chemicalFormula) VALUES('" + key + "', " + mass + ", " + charge + ", '" + precursorAdduct + "', '-', '-', 0, " + numFragments + ", 0, 0, 0, 0, '0', 0, 1, 1, '', '', '', '',  '" + chemForm + "')";
                                                command.CommandText = sql;
                                                command.ExecuteNonQuery();
                                                
                                                // add spectrum
                                                command.CommandText = "INSERT INTO RefSpectraPeaks(RefSpectraID, peakMZ, peakIntensity) VALUES((SELECT MAX(id) FROM RefSpectra), @mzvalues, @intensvalues)";
                                                SQLiteParameter parameterMZ = new SQLiteParameter("@mzvalues", System.Data.DbType.Binary);
                                                SQLiteParameter parameterIntens = new SQLiteParameter("@intensvalues", System.Data.DbType.Binary);
                                                parameterMZ.Value = Compressing.Compress(valuesMZArray);
                                                parameterIntens.Value = Compressing.Compress(valuesIntens);
                                                command.Parameters.Add(parameterMZ);
                                                command.Parameters.Add(parameterIntens);
                                                command.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                String headgroup = "Ch";
                String key = headgroup + " ";
                
                foreach (KeyValuePair<string, bool> adduct in adducts)
                {
                    if (adduct.Value)
                    {
                        String keyAdduct = key + " " + adduct.Key;
                        String precursorAdduct = "[M" + adduct.Key + "]";
                        if (!usedKeys.Contains(keyAdduct))
                        {
                            usedKeys.Add(keyAdduct);
                            
                            DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                            MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                            String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                            int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                            double mass = LipidCreatorForm.computeMass(atomsCount, charge) / (double)(Math.Abs(charge));                                                

                            ArrayList valuesMZ = new ArrayList();
                            ArrayList valuesIntensity = new ArrayList();
                            
                            foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                            {
                                if (((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)))
                                {
                                    DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                    //String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                    //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                    double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge) / (double)(Math.Abs(fragment.fragmentCharge));

                                    valuesMZ.Add(massFragment);
                                    valuesIntensity.Add(fragment.intensity);
                                    
                                    
                                    // add Annotation
                                    /*
                                    sql = "INSERT INTO Annotations(RefSpectraID, fragmentMZ, sumComposition, shortName) VALUES ((SELECT COUNT(*) FROM RefSpectra) + 1, " + massFragment + ", '" + chemFormFragment + "', @fragmentName)";
                                    SQLiteParameter parameterName = new SQLiteParameter("@fragmentName", System.Data.DbType.String);
                                    parameterName.Value = fragment.fragmentName;
                                    command.CommandText = sql;
                                    command.Parameters.Add(parameterName);
                                    command.ExecuteNonQuery();
                                    */
                                }
                            }


                            int numFragments = valuesMZ.Count;
                            double[] valuesMZArray = new double[numFragments];
                            float[] valuesIntens = new float[numFragments];
                            for(int j = 0; j < numFragments; ++j)
                            {
                                valuesMZArray[j] = (double)valuesMZ[j];
                                valuesIntens[j] = 100 * (float)((double)valuesIntensity[j]);
                            }
                            
                            
                            // add MS1 information
                            sql = "INSERT INTO RefSpectra (moleculeName, precursorMZ, precursorCharge, precursorAdduct, prevAA, nextAA, copies, numPeaks, driftTimeMsec, collisionalCrossSectionSqA, driftTimeHighEnergyOffsetMsec, retentionTime, fileID, SpecIDinFile, score, scoreType, inchiKey, otherKeys, peptideSeq, peptideModSeq, chemicalFormula) VALUES('" + key + "', " + mass + ", " + charge + ", '" + precursorAdduct + "', '-', '-', 0, " + numFragments + ", 0, 0, 0, 0, '0', 0, 1, 1, '', '', '', '',  '" + chemForm + "')";
                            command.CommandText = sql;
                            command.ExecuteNonQuery();
                            
                            // add spectrum
                            command.CommandText = "INSERT INTO RefSpectraPeaks(RefSpectraID, peakMZ, peakIntensity) VALUES((SELECT MAX(id) FROM RefSpectra), @mzvalues, @intensvalues)";
                            SQLiteParameter parameterMZ = new SQLiteParameter("@mzvalues", System.Data.DbType.Binary);
                            SQLiteParameter parameterIntens = new SQLiteParameter("@intensvalues", System.Data.DbType.Binary);
                            parameterMZ.Value = Compressing.Compress(valuesMZArray);
                            parameterIntens.Value = Compressing.Compress(valuesIntens);
                            command.Parameters.Add(parameterMZ);
                            command.Parameters.Add(parameterIntens);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            
        }
    }


    [Serializable]
    public class LipidCreatorForm
    {
        public ArrayList lipidTabList;
        public ArrayList registeredLipids;
        public CreatorGUI creatorGUI;
        public Dictionary<String, ArrayList> allFragments;
        public Dictionary<String, String> allPathsToPrecursorImages;
        public Dictionary<String, DataTable> headgroups;
        public Dictionary<String, int> buildingBlockTypes;
        public Dictionary<String, Dictionary<String, bool>> headgroupAdductRestrictions;
        public DataTable allLipids;
        public DataTable allLipidsUnique;
        public SkylineToolClient skylineToolClient;
        public bool openedAsExternal;
        public string prefixPath = "Tools/LipidCreator/";
        
        public LipidCreatorForm(String pipe)
        {
            openedAsExternal = (pipe != null);
            skylineToolClient = openedAsExternal ? new SkylineToolClient(pipe, "LipidCreator") : null;
            registeredLipids = new ArrayList();
            allPathsToPrecursorImages = new Dictionary<String, String>();
            allFragments = new Dictionary<String, ArrayList>();
            headgroups = new Dictionary<String, DataTable>();
            buildingBlockTypes = new Dictionary<String, int>();
            headgroupAdductRestrictions = new Dictionary<String, Dictionary<String, bool>>();
            allLipids = new DataTable();
            allLipids.Columns.Add("Molecule List Name");
            allLipids.Columns.Add("Precursor Name");
            allLipids.Columns.Add("Precursor Ion Formula");
            allLipids.Columns.Add("Precursor Adduct");
            allLipids.Columns.Add("Precursor m/z");
            allLipids.Columns.Add("Precursor Charge");
            allLipids.Columns.Add("Product Name");
            allLipids.Columns.Add("Product Ion Formula");
            allLipids.Columns.Add("Product m/z");
            allLipids.Columns.Add("Product Charge");
            
            allLipidsUnique = new DataTable();
            allLipidsUnique.Columns.Add("Molecule List Name");
            allLipidsUnique.Columns.Add("Precursor Name");
            allLipidsUnique.Columns.Add("Precursor Ion Formula");
            allLipidsUnique.Columns.Add("Precursor Adduct");
            allLipidsUnique.Columns.Add("Precursor m/z");
            allLipidsUnique.Columns.Add("Precursor Charge");
            allLipidsUnique.Columns.Add("Product Name");
            allLipidsUnique.Columns.Add("Product Ion Formula");
            allLipidsUnique.Columns.Add("Product m/z");
            allLipidsUnique.Columns.Add("Product Charge");
            

            int lineCounter = 1;
            string ms2FragmentsFile = (openedAsExternal ? prefixPath : "") + "data/ms2fragments.csv";
            if (File.Exists(ms2FragmentsFile))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(ms2FragmentsFile))
                    {
                        String line = sr.ReadLine(); // omit titles
                        while((line = sr.ReadLine()) != null)
                        {
                            lineCounter++;
                            if (line.Length < 2) continue;
                            if (line[0] == '#') continue;
                            String[] tokens = line.Split(new char[] {','});
                            if (!allFragments.ContainsKey(tokens[0]))
                            {
                                allFragments.Add(tokens[0], new ArrayList());
                            }
                            DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                            atomsCount.Rows[0]["Count"] = Convert.ToInt32(tokens[5]);
                            atomsCount.Rows[1]["Count"] = Convert.ToInt32(tokens[6]);
                            atomsCount.Rows[2]["Count"] = Convert.ToInt32(tokens[7]);
                            atomsCount.Rows[3]["Count"] = Convert.ToInt32(tokens[8]);
                            atomsCount.Rows[4]["Count"] = Convert.ToInt32(tokens[9]);
                            atomsCount.Rows[5]["Count"] = Convert.ToInt32(tokens[10]);
                            atomsCount.Rows[6]["Count"] = Convert.ToInt32(tokens[11]);
                            string fragmentFile = (openedAsExternal ? prefixPath : "") + tokens[2];
                            if (!File.Exists(fragmentFile))
                            {
                                Console.WriteLine("Error in line (" + lineCounter + "): file '" + fragmentFile + "' does not exist or can not be opened.");
                            }
                            
                            if (tokens[13].Length > 0)
                            {
                                allFragments[tokens[0]].Add(new MS2Fragment(tokens[1], Convert.ToInt32(tokens[3]), fragmentFile, true, atomsCount, tokens[4], tokens[12], Convert.ToDouble(tokens[13])));
                            }
                            else 
                            {
                                allFragments[tokens[0]].Add(new MS2Fragment(tokens[1], Convert.ToInt32(tokens[3]), fragmentFile, true, atomsCount, tokens[4], tokens[12]));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file '" + ms2FragmentsFile + "' in line '" + lineCounter + "' could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("Error: file '" + ms2FragmentsFile + "' does not exist or can not be opened.");
            }
            
            
            
            
            string headgroupsFile = (openedAsExternal ? prefixPath : "") + "data/headgroups.csv";
            if (File.Exists(headgroupsFile))
            {
                lineCounter = 1;
                try
                {
                    using (StreamReader sr = new StreamReader(headgroupsFile))
                    {
                        String line = sr.ReadLine(); // omit titles
                        while((line = sr.ReadLine()) != null)
                        {
                            lineCounter++;
                            if (line.Length < 2) continue;
                            if (line[0] == '#') continue;
                            String[] tokens = line.Split(new char[] {','}); // StringSplitOptions.RemoveEmptyEntries
                            if (tokens.Length != 17) throw new Exception("invalid line in file");
                            headgroups.Add(tokens[0], MS2Fragment.createEmptyElementTable());
                            headgroups[tokens[0]].Rows[0]["Count"] = Convert.ToInt32(tokens[1]);
                            headgroups[tokens[0]].Rows[1]["Count"] = Convert.ToInt32(tokens[2]);
                            headgroups[tokens[0]].Rows[2]["Count"] = Convert.ToInt32(tokens[3]);
                            headgroups[tokens[0]].Rows[3]["Count"] = Convert.ToInt32(tokens[4]);
                            headgroups[tokens[0]].Rows[4]["Count"] = Convert.ToInt32(tokens[5]);
                            headgroups[tokens[0]].Rows[5]["Count"] = Convert.ToInt32(tokens[6]);
                            headgroups[tokens[0]].Rows[6]["Count"] = Convert.ToInt32(tokens[7]);
                            string precursorFile = (openedAsExternal ? prefixPath : "") + tokens[8];
                            if (!File.Exists(precursorFile))
                            {
                                Console.WriteLine("Error (" + lineCounter + "): precursor file " + precursorFile + " does not exist or can not be opened.");
                            }
                            allPathsToPrecursorImages.Add(tokens[0], precursorFile);
                            headgroupAdductRestrictions.Add(tokens[0], new Dictionary<String, bool>());
                            headgroupAdductRestrictions[tokens[0]].Add("+H", tokens[9].Equals("Yes"));
                            headgroupAdductRestrictions[tokens[0]].Add("+2H", tokens[10].Equals("Yes"));
                            headgroupAdductRestrictions[tokens[0]].Add("+NH4", tokens[11].Equals("Yes"));
                            headgroupAdductRestrictions[tokens[0]].Add("-H", tokens[12].Equals("Yes"));
                            headgroupAdductRestrictions[tokens[0]].Add("-2H", tokens[13].Equals("Yes"));
                            headgroupAdductRestrictions[tokens[0]].Add("+HCOO", tokens[14].Equals("Yes"));
                            headgroupAdductRestrictions[tokens[0]].Add("+CH3COO", tokens[15].Equals("Yes"));
                            buildingBlockTypes.Add(tokens[0], Convert.ToInt32(tokens[16]));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file '" + headgroupsFile + "' in line '" + lineCounter + "' could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("Error: file " + headgroupsFile + " does not exist or can not be opened.");
            }
            
            lipidTabList = new ArrayList(new Lipid[] {null,
                                                      new GLLipid(allPathsToPrecursorImages, allFragments),
                                                      new PLLipid(allPathsToPrecursorImages, allFragments),
                                                      new SLLipid(allPathsToPrecursorImages, allFragments),
                                                      new Cholesterol(allPathsToPrecursorImages, allFragments)
                                                      });
                                                      
                                                      
            creatorGUI = new CreatorGUI(this);
        }


        public HashSet<int> parseRange(String text, int lower, int upper)
        {
            return parseRange(text, lower, upper, 0);
        }
        
        // obType (Object type): 0 = carbon length, 1 = carbon length odd, 2 = carbon length even, 3 = db length, 4 = hydroxyl length
        public HashSet<int> parseRange(String text, int lower, int upper, int obType)
        {
            int oddEven = (obType <= 2) ? obType : 0;
            if (text.Length == 0) return null;
            foreach (char c in text)
            {
                int ic = (int)c;
                if (!((ic == (int)',') || (ic == (int)'-') || (ic == (int)' ') || (48 <= ic && ic < 58)))
                {
                    return null;
                }
            }
        
            string[] delimitors = new string[] { "," };
            string[] delimitorsRange = new string[] { "-" };
            string[] tokens = text.Split(delimitors, StringSplitOptions.None);
            
            HashSet<int> carbonCounts = new HashSet<int>();
            
            for (int i = 0; i < tokens.Length; ++i)
            {
                if (tokens[i].Length == 0) return null;
                string[] rangeBoundaries = tokens[i].Split(delimitorsRange, StringSplitOptions.None);
                if (rangeBoundaries.Length == 1)
                {
                    int rangeStart = 0;
                    try 
                    {
                        rangeStart = Convert.ToInt32(rangeBoundaries[0]);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                    if (rangeStart < lower || upper < rangeStart) return null;
                    if (oddEven == 0 || (oddEven == 1 && (rangeStart % 2 == 1)) || (oddEven == 2 && (rangeStart % 2 == 0)))
                    {
                        carbonCounts.Add(rangeStart);
                    }
                }
                else if (rangeBoundaries.Length == 2)
                {
                    int rangeStart = 0;
                    int rangeEnd = 0;
                    try 
                    {
                        rangeStart = Convert.ToInt32(rangeBoundaries[0]);
                        rangeEnd = Convert.ToInt32(rangeBoundaries[1]);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                    if (rangeEnd < rangeStart || rangeStart < lower || upper < rangeEnd) return null;
                    for (int l = rangeStart; l <= rangeEnd; ++l)
                    {
                        if (oddEven == 0 || (oddEven == 1 && (l % 2 == 1)) || (oddEven == 2 && (l % 2 == 0)))
                        {
                            carbonCounts.Add(l);
                        }
                    }
                }
                else return null;
                
            }
            return carbonCounts;
        }

        
        public void assembleLipids()
        {
            allLipids.Clear();
            allLipidsUnique.Clear();
            HashSet<String> usedKeys = new HashSet<String>();
            HashSet<String> replicates = new HashSet<String>();
            foreach (Lipid currentLipid in registeredLipids)
            {
                currentLipid.addLipids(allLipids, allLipidsUnique, headgroups, headgroupAdductRestrictions, usedKeys, replicates);
            }
        }

        public static String computeChemicalFormula(DataTable elements)
        {
            String chemForm = "";
            foreach (DataRow row in elements.Rows)
            {
                if (Convert.ToInt32(row["Count"]) > 0)
                {
                    chemForm += Convert.ToString(row["Shortcut"]) + (((int)row["Count"] > 1) ? Convert.ToString(row["Count"]) : "");
                }
            }
            return chemForm;
        }

        public static double computeMass(DataTable elements, double charge)
        {
            double mass = 0;
            foreach (DataRow row in elements.Rows)
            {
                mass += Convert.ToDouble(row["Count"]) * Convert.ToDouble(row["mass"]);
            }
            return mass - charge * 0.00054857990946;
        }
        
        public void sendToSkyline(DataTable dt, string blibName, string blibFile)
        {
            if (skylineToolClient == null) return;
            
            var header = string.Join(",", new string[]
            {
                "MoleculeGroup",
                "PrecursorName",
                "PrecursorFormula",
                "PrecursorAdduct",
                "PrecursorMz",
                "PrecursorCharge",
                "ProductName",
                "ProductFormula",
                "ProductMz",
                "ProductCharge"
            });
            string pipeString = header + "\n";
            double maxMass = 0;
            
            foreach (DataRow entry in dt.Rows)
            {
                // Default col order is listname, preName, PreFormula, preAdduct, preMz, preCharge, prodName, ProdFormula, prodAdduct, prodMz, prodCharge
                pipeString += entry["Molecule List Name"] + ","; // listname
                pipeString += entry["Precursor Name"] + ","; // preName
                pipeString += entry["Precursor Ion Formula"] + ","; // PreFormula
                pipeString += entry["Precursor Adduct"] + ","; // preAdduct
                pipeString += entry["Precursor m/z"] + ","; // preMz
                maxMass = Math.Max(maxMass, Convert.ToDouble((string)entry["Precursor m/z"]));
                pipeString += entry["Precursor Charge"] + ","; // preCharge
                pipeString += entry["Product Name"] + ","; // prodName
                pipeString += entry["Product Ion Formula"] + ","; // ProdFormula, no prodAdduct
                pipeString += entry["Product m/z"] + ","; // prodMz
                pipeString += entry["Product Charge"]; // prodCharge
                pipeString += "\n";
            }
            try
            {
                skylineToolClient.InsertSmallMoleculeTransitionList(pipeString);
                if (blibName.Length > 0 && blibFile.Length > 0) skylineToolClient.AddSpectralLibrary(blibName, blibFile);
                skylineToolClient.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occured, data could not be send to Skyline, please check if your Skyline parameters allow precursor masses up to " + maxMass + "Da.");
            }
        }
        
        
        public string serialize()
        {
            string xml = "<LipidCreator>\n";
            foreach (Lipid currentLipid in registeredLipids)
            {
                xml += currentLipid.serialize();
            }
            xml += "</LipidCreator>\n";
            return xml;
        }
        
        public void import(XDocument doc)
        {
        
            var lipids = doc.Descendants("lipid");
            foreach ( var lipid in lipids )
            {
                string lipidType = lipid.Attribute("type").Value;
                switch (lipidType)
                {
                    case "GL":
                        GLLipid gll = new GLLipid(allPathsToPrecursorImages, null);
                        gll.import(lipid);
                        registeredLipids.Add(gll);
                        break;
                        
                    case "PL":
                        PLLipid pll = new PLLipid(allPathsToPrecursorImages, null);
                        pll.import(lipid);
                        registeredLipids.Add(pll);
                        break;
                        
                    case "SL":
                        SLLipid sll = new SLLipid(allPathsToPrecursorImages, null);
                        sll.import(lipid);
                        registeredLipids.Add(sll);
                        break;
                        
                    case "Cholesterol":
                        Cholesterol chl = new Cholesterol(allPathsToPrecursorImages, null);
                        chl.import(lipid);
                        registeredLipids.Add(chl);
                        break;
                        
                    default:
                        Console.WriteLine("Error global import");
                        throw new Exception();
                }
            }
        }
        
        
        public void createBlib(String filename)
        {
            if (File.Exists(filename)) File.Delete(filename);
        
            SQLiteConnection mDBConnection = new SQLiteConnection("Data Source=" + filename + ";Version=3;");
            mDBConnection.Open();
            SQLiteCommand command = new SQLiteCommand(mDBConnection);
            
            
            command.CommandText = "PRAGMA synchronous=OFF;";
            command.ExecuteNonQuery();
            
            command.CommandText = "PRAGMA cache_size=" + (double)(250 * 1024 / 1.5) + ";";
            command.ExecuteNonQuery();
            
            command.CommandText = "PRAGMA temp_store=MEMORY;";
            command.ExecuteNonQuery();
            
            String sql = "CREATE TABLE LibInfo(libLSID TEXT, createTime TEXT, numSpecs INTEGER, majorVersion INTEGER, minorVersion INTEGER)";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            //fill in the LibInfo first
            string lsid = "urn:lsid:isas.de:spectral_library:bibliospec:nr:1";
            sql = "INSERT INTO LibInfo values('" + lsid + "','2017-01-01',-1,1,4)";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE RefSpectra (id INTEGER primary key autoincrement not null, peptideSeq VARCHAR(150), precursorMZ REAL, precursorCharge INTEGER, peptideModSeq VARCHAR(200), prevAA CHAR(1), nextAA CHAR(1), copies INTEGER, numPeaks INTEGER, driftTimeMsec REAL, collisionalCrossSectionSqA REAL, driftTimeHighEnergyOffsetMsec REAL, retentionTime REAL, moleculeName VARCHAR(128), chemicalFormula VARCHAR(128), precursorAdduct VARCHAR(128), inchiKey VARCHAR(128), otherKeys VARCHAR(128), fileID INTEGER, SpecIDinFile VARCHAR(256), score REAL, scoreType TINYINT)";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE Modifications (id INTEGER primary key autoincrement not null, RefSpectraID INTEGER, position INTEGER, mass REAL)";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE RefSpectraPeaks(RefSpectraID INTEGER, peakMZ BLOB, peakIntensity BLOB)";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE SpectrumSourceFiles (id INTEGER PRIMARY KEY autoincrement not null, fileName VARCHAR(512), cutoffScore REAL )";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE ScoreTypes (id INTEGER PRIMARY KEY, scoreType VARCHAR(128) )";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            
            /*
            sql = "CREATE TABLE Annotations (RefSpectraID INTEGER, fragmentMZ REAL, sumComposition VARCHAR(100), shortName VARCHAR(50), chargedFragmentName VARCHAR(256), neutralFragmentName VARCHAR(256))";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE RetentionTimes(RefSpectraID INTEGER, RedundantRefSpectraID INTEGER, SpectrumSourceID INTEGER, ionMobilityValue REAL, ionMobilityType INTEGER, ionMobilityHighEnergyDriftTimeOffsetMsec REAL, retentionTime REAL, bestSpectrum INTEGER, FOREIGN KEY(RefSpectraID) REFERENCES RefSpectra(id))";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            */
            
            string[] scoreTypeNames = new string[]{"UNKNOWN", "PERCOLATOR QVALUE", "PEPTIDE PROPHET SOMETHING", "SPECTRUM MILL", "IDPICKER FDR", "MASCOT IONS SCORE",  "TANDEM EXPECTATION VALUE",  "PROTEIN PILOT CONFIDENCE", "SCAFFOLD SOMETHING", "WATERS MSE PEPTIDE SCORE", "OMSSA EXPECTATION SCORE", "PROTEIN PROSPECTOR EXPECTATION SCORE", "SEQUEST XCORR", "MAXQUANT SCORE", "MORPHEUS SCORE", "MSGF+ SCORE", "PEAKS CONFIDENCE SCORE", "BYONIC SCORE", "PEPTIDE SHAKER CONFIDENCE"};
            
            for(int i=0; i < scoreTypeNames.Length; ++i){
                sql = "INSERT INTO ScoreTypes(id, scoreType) VALUES(" + i + ", '" + scoreTypeNames[i] + "')";
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            
            command.CommandText = "CREATE INDEX idxPeptide ON RefSpectra (peptideSeq, precursorCharge)";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE INDEX idxPeptideMod ON RefSpectra (peptideModSeq, precursorCharge)";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE INDEX idxRefIdPeaks ON RefSpectraPeaks (RefSpectraID)";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE INDEX idxInChiKey ON RefSpectra (inchiKey, precursorAdduct)";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE INDEX idxMoleculeName ON RefSpectra (moleculeName, precursorAdduct)";
            command.ExecuteNonQuery();
            //command.CommandText = "CREATE INDEX idxRefIdAnnotations ON Annotations (RefSpectraID)";
            //command.ExecuteNonQuery();
            
            
            
            HashSet<String> usedKeys = new HashSet<String>();
            foreach (Lipid currLipid in registeredLipids)
            {
                currLipid.addSpectrum(command, headgroups, usedKeys);
            }
            
            
            // update numspecs
            sql = "UPDATE LibInfo SET numSpecs = (SELECT MAX(id) FROM RefSpectra);";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
        }
        
        
        

        [STAThread]
        public static void Main(string[] args)
        {
            LipidCreatorForm lcf = new LipidCreatorForm((args.Length > 0) ? args[0] : null);
            Application.Run(lcf.creatorGUI);
        }
    }
    
    public static class Compressing
    {
        public static byte[] GetBytes(double[] values)
        {
            var result = new byte[values.Length * sizeof(double)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }
        
        public static byte[] GetBytes(float[] values)
        {
            var result = new byte[values.Length * sizeof(float)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }
    
        public static byte[] Compress(this double[] uncompressed)
        {
            return Compress(GetBytes(uncompressed), 3);
            //return GetBytes(uncompressed);
        }
    
        public static byte[] Compress(this float[] uncompressed)
        {
            return Compress(GetBytes(uncompressed), 3);
            //return GetBytes(uncompressed);
        }
        
        public static byte[] Compress(this byte[] uncompressed, int level)
        {
            byte[] result;
            using (var ms = new MemoryStream())
            {
                using (var compressor = new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.Level0 + level))
                    compressor.Write(uncompressed, 0, uncompressed.Length);
                result =  ms.ToArray();
            }


            // If compression did not improve the situation, then use
            // uncompressed bytes.
            if (result.Length >= uncompressed.Length)
                return uncompressed;

            return result;
        }    
    }

}


