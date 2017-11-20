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

namespace LipidCreator
{
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
}