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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace LipidCreator
{
    public enum Molecules {C = 0, C13 = 1, H = 2, H2 = 3, O = 4, O17 = 5, N = 6, N15 = 7, P = 8, S = 9};
    
    [Serializable]
    public class MS2Fragment
    {
        public String fragmentName;
        public int fragmentCharge;
        public String fragmentFile;
        public bool fragmentSelected;
        public Dictionary<int, int> fragmentElements;
        public ArrayList fragmentBase;
        public HashSet<string> restrictions;
        public double intensity;
        public bool userDefined;
        public bool independent;
        public const double DEFAULT_INTENSITY = 100.0;
        public string CommentForSpectralLibrary { get { return fragmentFile; } }
        public static Dictionary<string, int> ELEMENT_POSITIONS = new Dictionary<string, int>(){
            {"C", (int)Molecules.C},
            {"H", (int)Molecules.H},
            {"O", (int)Molecules.O},
            {"N", (int)Molecules.N},
            {"P", (int)Molecules.P},
            {"S", (int)Molecules.S},
            {"H'", (int)Molecules.H2},
            {"C'", (int)Molecules.C13},
            {"N'", (int)Molecules.N15},
            {"O'", (int)Molecules.O17}
        };
        
        
        public static Dictionary<int, double> ELEMENT_MASSES = new Dictionary<int, double>(){
            {(int)Molecules.C, 12.0},
            {(int)Molecules.H, 1.007825035},
            {(int)Molecules.O, 15.99491463},
            {(int)Molecules.N, 14.003074},
            {(int)Molecules.P, 30.973762},
            {(int)Molecules.S, 31.9720707},
            {(int)Molecules.H2, 2.014101779},
            {(int)Molecules.C13, 13.0033548378},
            {(int)Molecules.N15, 15.0001088984},
            {(int)Molecules.O17, 16.9991315}
        };
        
        
        public static Dictionary<int, string> ELEMENT_SHORTCUTS = new Dictionary<int, string>(){
            {(int)Molecules.C, "C"},
            {(int)Molecules.H, "H"},
            {(int)Molecules.O, "O"},
            {(int)Molecules.N, "N"},
            {(int)Molecules.P, "P"},
            {(int)Molecules.S, "S"},
            {(int)Molecules.H2, "H'"},
            {(int)Molecules.C13, "C'"},
            {(int)Molecules.N15, "N'"},
            {(int)Molecules.O17, "O'"}
        };
        
        public static Dictionary<int, int> HEAVY_DERIVATIVE = new Dictionary<int, int>()
        {
            {(int)Molecules.C, (int)Molecules.C13},
            {(int)Molecules.H, (int)Molecules.H2},
            {(int)Molecules.O, (int)Molecules.O17},
            {(int)Molecules.N, (int)Molecules.N15},
        };
        
        public static Dictionary<int, int> LIGHT_ORIGIN = new Dictionary<int, int>()
        {
            {(int)Molecules.C13, (int)Molecules.C},
            {(int)Molecules.H2, (int)Molecules.H},
            {(int)Molecules.O17, (int)Molecules.O},
            {(int)Molecules.N15, (int)Molecules.N},
        };
        
        
        public static Dictionary<int, int> createEmptyElementDict()
        {
            Dictionary<int, int> elements = new Dictionary<int, int>();
            foreach (KeyValuePair<int, double> kvp in ELEMENT_MASSES) elements.Add(kvp.Key, 0);
            return elements;
        }
        
        
        public static Dictionary<int, int> createFilledElementDict(DataTable dt)
        {
            Dictionary<int, int> elements = new Dictionary<int, int>();
            foreach (DataRow dr in dt.Rows)
            {
                elements.Add(ELEMENT_POSITIONS[(string)dr["Shortcut"]], Convert.ToInt32(dr["Count"]));
            }
            return elements;
        }
        
        
        public Dictionary<int, int> copyElementDict()
        {
            Dictionary<int, int> elements = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> kvp in fragmentElements) elements.Add(kvp.Key, kvp.Value);
            return elements;
        }
    
    
        public string serialize()
        {
            string xml = "<MS2Fragment";
            xml += " fragmentName=\"" + fragmentName + "\"";
            xml += " fragmentCharge=\"" + fragmentCharge + "\"";
            xml += " fragmentFile=\"" + fragmentFile + "\"";
            xml += " intensity=\"" + intensity + "\"";
            xml += " userDefined=\"" + userDefined + "\"";
            xml += " independent=\"" + independent + "\"";
            xml += " fragmentSelected=\"" + (fragmentSelected ? 1 : 0) + "\">\n";
            foreach (string restriction in restrictions)
            {
                xml += "<restriction>" + restriction + "</restriction>\n";
            }
            foreach (string fbase in fragmentBase)
            {
                xml += "<fragmentBase>" + fbase + "</fragmentBase>\n";
            }
            foreach (KeyValuePair<int, int> kvp in fragmentElements)
            {
                xml += "<Element type=\"" + ELEMENT_SHORTCUTS[kvp.Key] + "\">" + Convert.ToString(kvp.Value) + "</Element>\n";
            }
            xml += "</MS2Fragment>\n";
            return xml;
        }
        
        public void import(XElement node, string importVersion)
        {
            
        
            restrictions.Clear();
            fragmentBase.Clear();
        
            fragmentName = node.Attribute("fragmentName").Value.ToString();
            fragmentCharge = Convert.ToInt32(node.Attribute("fragmentCharge").Value.ToString());
            fragmentFile = node.Attribute("fragmentFile").Value.ToString();
            intensity = Convert.ToInt32(node.Attribute("intensity").Value.ToString());
            userDefined = node.Attribute("userDefined").Value.Equals("True");
            independent = node.Attribute("independent").Value.Equals("True");
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
                        fragmentElements[ELEMENT_POSITIONS[child.Attribute("type").Value.ToString()]] = Convert.ToInt32(child.Value.ToString());
                        break;
                        
                    default:
                        throw new Exception();
                }
            }
        }
        
        
    
        public static void addCounts(Dictionary<int, int> counts1, Dictionary<int, int> counts2)
        {
            foreach (KeyValuePair<int, int> kvp in counts2) counts1[kvp.Key] += kvp.Value;
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
            
            for (int i = 0; i < ELEMENT_POSITIONS.Count; ++i) elements.Rows.Add(elements.NewRow());

            elements.Rows[(int)Molecules.C][count] = "0";
            elements.Rows[(int)Molecules.C][shortcut] = "C";
            elements.Rows[(int)Molecules.C][element] = "carbon";
            elements.Rows[(int)Molecules.C][monoMass] = 12;
            
            elements.Rows[(int)Molecules.C13][count] = "0";
            elements.Rows[(int)Molecules.C13][shortcut] = "C'";
            elements.Rows[(int)Molecules.C13][element] = "carbon 13";
            elements.Rows[(int)Molecules.C13][monoMass] = 13.0033548378;

            elements.Rows[(int)Molecules.H][count] = "0";
            elements.Rows[(int)Molecules.H][shortcut] = "H";
            elements.Rows[(int)Molecules.H][element] = "hydrogen";
            elements.Rows[(int)Molecules.H][monoMass] = 1.007825035;

            elements.Rows[(int)Molecules.H2][count] = "0";
            elements.Rows[(int)Molecules.H2][shortcut] = "H'";
            elements.Rows[(int)Molecules.H2][element] = "deuterium";
            elements.Rows[(int)Molecules.H2][monoMass] = 2.014101779;

            elements.Rows[(int)Molecules.O][count] = "0";
            elements.Rows[(int)Molecules.O][shortcut] = "O";
            elements.Rows[(int)Molecules.O][element] = "oxygen";
            elements.Rows[(int)Molecules.O][monoMass] = 15.99491463;

            elements.Rows[(int)Molecules.O17][count] = "0";
            elements.Rows[(int)Molecules.O17][shortcut] = "O'";
            elements.Rows[(int)Molecules.O17][element] = "oxygen 17";
            elements.Rows[(int)Molecules.O17][monoMass] = 16.9991315;

            elements.Rows[(int)Molecules.N][count] = "0";
            elements.Rows[(int)Molecules.N][shortcut] = "N";
            elements.Rows[(int)Molecules.N][element] = "nitrogen";
            elements.Rows[(int)Molecules.N][monoMass] = 14.003074;

            elements.Rows[(int)Molecules.N15][count] = "0";
            elements.Rows[(int)Molecules.N15][shortcut] = "N'";
            elements.Rows[(int)Molecules.N15][element] = "nitrogen 15";
            elements.Rows[(int)Molecules.N15][monoMass] = 15.0001088984;

            elements.Rows[(int)Molecules.P][count] = "0";
            elements.Rows[(int)Molecules.P][shortcut] = "P";
            elements.Rows[(int)Molecules.P][element] = "phosphor";
            elements.Rows[(int)Molecules.P][monoMass] = 30.973762;

            elements.Rows[(int)Molecules.S][count] = "0";
            elements.Rows[(int)Molecules.S][shortcut] = "S";
            elements.Rows[(int)Molecules.S][element] = "sulfur";
            elements.Rows[(int)Molecules.S][monoMass] = 31.9720707;
            
            columnShortcut.ReadOnly = true;
            columnElement.ReadOnly = true;
            return elements;
        }
        
        
        
        public void setElements(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                fragmentElements[ELEMENT_POSITIONS[(string)dr["Shortcut"]]] = Convert.ToInt32(dr["Count"]);
            }
        }
        
        
        
        public static DataTable createFilledElementTable(Dictionary<int, int> fragmentElements)
        {
            DataTable elements = createEmptyElementTable();
            foreach(KeyValuePair<int, int> kvp in fragmentElements)
            {
                elements.Rows[kvp.Key]["Count"] = kvp.Value;
            }
            return elements;
        }
        
        
        
        public static DataTable createEmptyElementTable(DataTable copy)
        {
            DataTable elements = createEmptyElementTable();
            for (int i = 0; i < elements.Rows.Count; ++i)
            {
                if (elements.Rows[i]["Shortcut"].Equals(copy.Rows[i]["Shortcut"]))
                {
                    elements.Rows[i]["Count"] = copy.Rows[i]["Count"];
                }
                else
                {
                    throw new Exception("Copying element table failed");
                }
            }
            return elements;
        }
        
        
    
        public MS2Fragment()
        {
            fragmentName = "-";
            fragmentCharge = -1;
            fragmentFile = "-";
            fragmentSelected = true;
            fragmentElements = new Dictionary<int, int>();
            foreach (KeyValuePair<int, string> kvp in ELEMENT_SHORTCUTS) fragmentElements.Add(kvp.Key, 0);
            fragmentBase = new ArrayList();
            restrictions = new HashSet<string>();
            userDefined = false;
            independent = false;
            intensity = DEFAULT_INTENSITY;
        }
        
        

        public MS2Fragment(String name, String fileName)
        {
            fragmentName = name;
            fragmentCharge = -1;
            fragmentFile = fileName;
            fragmentSelected = true;
            fragmentElements = new Dictionary<int, int>();
            foreach (KeyValuePair<int, string> kvp in ELEMENT_SHORTCUTS) fragmentElements.Add(kvp.Key, 0);
            fragmentBase = new ArrayList();
            restrictions = new HashSet<string>();
            userDefined = false;
            independent = false;
            intensity = DEFAULT_INTENSITY;
        }


        public MS2Fragment(String name, String fileName, int charge)
        {
            fragmentName = name;
            fragmentCharge = charge;
            fragmentFile = fileName;
            fragmentSelected = true;
            fragmentElements = new Dictionary<int, int>();
            foreach (KeyValuePair<int, string> kvp in ELEMENT_SHORTCUTS) fragmentElements.Add(kvp.Key, 0);
            fragmentBase = new ArrayList();
            restrictions = new HashSet<string>();
            userDefined = false;
            independent = false;
            intensity = DEFAULT_INTENSITY;
        }
        
        
        
        public MS2Fragment(String name, int charge, String fileName, bool selected, Dictionary<int, int> dataElements, String baseForms, String restrictions)
        {
            fragmentName = name;
            fragmentCharge = charge;
            fragmentFile = fileName;
            fragmentSelected = selected;
            fragmentElements = dataElements;
            this.restrictions = new HashSet<string>();
            fragmentBase = new ArrayList(baseForms.Split(new char[] {';'}));
            userDefined = false;
            independent = false;
            intensity = DEFAULT_INTENSITY;
            if (restrictions.Length > 0) foreach (string restriction in restrictions.Split(new char[] {';'})) this.restrictions.Add(restriction);
        }
        
        
        
        public MS2Fragment(String name, int charge, String fileName, bool selected, Dictionary<int, int> dataElements, String baseForms, String restrictions, double intens)
        {
            fragmentName = name;
            fragmentCharge = charge;
            fragmentFile = fileName;
            fragmentSelected = selected;
            fragmentElements = dataElements;
            this.restrictions = new HashSet<string>();
            fragmentBase = new ArrayList(baseForms.Split(new char[] {';'}));
            userDefined = false;
            independent = false;
            intensity = Math.Min(DEFAULT_INTENSITY, Math.Max(0, intens));
            if (restrictions.Length > 0) foreach (string restriction in restrictions.Split(new char[] {';'})) this.restrictions.Add(restriction);
        }

        
        
        public MS2Fragment(MS2Fragment copy)
        {
            fragmentName = copy.fragmentName;
            fragmentCharge = copy.fragmentCharge;
            fragmentFile = copy.fragmentFile;
            fragmentSelected = copy.fragmentSelected;
            fragmentElements = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> kvp in copy.fragmentElements) fragmentElements.Add(kvp.Key, kvp.Value);
            fragmentBase = new ArrayList();
            userDefined = copy.userDefined;
            independent = copy.independent;
            restrictions = new HashSet<string>();
            foreach (string fbase in copy.fragmentBase) fragmentBase.Add(fbase);
            foreach (string restriction in copy.restrictions) restrictions.Add(restriction);
            intensity = copy.intensity;
        }
    }
}