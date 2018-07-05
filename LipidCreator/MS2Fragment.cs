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
    public enum Molecules {C = 0, C13 = 1, H = 2, H2 = 3, N = 4, N15 = 5, O = 6, O17 = 7, O18 = 8, P = 9, P32 = 10, S = 11, S34 = 12, S33 = 13};
    
    [Serializable]
    public class MS2Fragment
    {
        public String fragmentName;
        public String fragmentOutputName;
        public int fragmentCharge;
        public String fragmentFile;
        public Dictionary<int, int> fragmentElements;
        public ArrayList fragmentBase;
        public double intensity;
        public bool userDefined;
        public const double MAX_INTENSITY = 100.0;
        public const double DEFAULT_INTENSITY = 10.0;
        public string CommentForSpectralLibrary { get { return fragmentName; } }
        public static Dictionary<string, int> ELEMENT_POSITIONS = new Dictionary<string, int>(){
            {"C", (int)Molecules.C},
            {"H", (int)Molecules.H},
            {"N", (int)Molecules.N},
            {"O", (int)Molecules.O},
            {"P", (int)Molecules.P},
            {"P'", (int)Molecules.P32},
            {"S", (int)Molecules.S},
            {"S'", (int)Molecules.S34},
            {"S''", (int)Molecules.S33},
            {"H'", (int)Molecules.H2},
            {"C'", (int)Molecules.C13},
            {"N'", (int)Molecules.N15},
            {"O'", (int)Molecules.O17},
            {"O''", (int)Molecules.O18}
        };
        
        
        public static Dictionary<int, int> MONOISOTOPE_POSITIONS = new Dictionary<int, int>(){
            {(int)Molecules.C, 0},
            {(int)Molecules.H, 1},
            {(int)Molecules.N, 2},
            {(int)Molecules.O, 3},
            {(int)Molecules.P, 4},
            {(int)Molecules.S, 5}
        };
        
        
        public static Dictionary<int, double> ELEMENT_MASSES = new Dictionary<int, double>(){
            {(int)Molecules.C, 12.0},
            {(int)Molecules.H, 1.007825035},
            {(int)Molecules.N, 14.003074},
            {(int)Molecules.O, 15.99491463},
            {(int)Molecules.P, 30.973762},
            {(int)Molecules.S, 31.9720707},
            {(int)Molecules.H2, 2.014101779},
            {(int)Molecules.C13, 13.0033548378},
            {(int)Molecules.N15, 15.0001088984},
            {(int)Molecules.O17, 16.9991315},
            {(int)Molecules.O18, 17.9991604},
            {(int)Molecules.P32, 31.973907274},
            {(int)Molecules.S34, 33.96786690},
            {(int)Molecules.S33, 32.97145876}
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
            {(int)Molecules.O17, "O'"},
            {(int)Molecules.O18, "O''"},
            {(int)Molecules.P32, "P'"},
            {(int)Molecules.S34, "S'"},
            {(int)Molecules.S33, "S''"}
        };
        
        
        public static Dictionary<int, string> HEAVY_SHORTCUTS = new Dictionary<int, string>(){
            {(int)Molecules.H2, "2H"},
            {(int)Molecules.C13, "13C"},
            {(int)Molecules.N15, "15N"},
            {(int)Molecules.O17, "17O"},
            {(int)Molecules.O18, "18O"},
            {(int)Molecules.P32, "32P"},
            {(int)Molecules.S34, "34S"},
            {(int)Molecules.S33, "33S"}
        };
        
        
        public static Dictionary<int, string> HEAVY_SHORTCUTS_IUPAC = new Dictionary<int, string>(){
            {(int)Molecules.H2, "H2"},
            {(int)Molecules.C13, "C13"},
            {(int)Molecules.N15, "N15"},
            {(int)Molecules.O17, "O17"},
            {(int)Molecules.O18, "O18"},
            {(int)Molecules.P32, "P32"},
            {(int)Molecules.S34, "S34"},
            {(int)Molecules.S33, "S33"}
        };
        
        public static Dictionary<int, ArrayList> HEAVY_DERIVATIVE = new Dictionary<int, ArrayList>()
        {
            {(int)Molecules.C, new ArrayList(){Molecules.C13}},
            {(int)Molecules.H, new ArrayList(){Molecules.H2}},
            {(int)Molecules.O, new ArrayList(){Molecules.O17, Molecules.O18}},
            {(int)Molecules.N, new ArrayList(){Molecules.N15}},
            {(int)Molecules.P, new ArrayList(){Molecules.P32}},
            {(int)Molecules.S, new ArrayList(){Molecules.S33, Molecules.S34}}
        };
        
        
        public static Dictionary<string, int> HEAVY_POSITIONS = new Dictionary<string, int>(){
            {"2H", (int)Molecules.H2},
            {"13C", (int)Molecules.C13},
            {"15N", (int)Molecules.N15},
            {"17O", (int)Molecules.O17},
            {"18O", (int)Molecules.O18},
            {"32P", (int)Molecules.P32},
            {"34S", (int)Molecules.S34},
            {"33S", (int)Molecules.S33}
        };
        
        public static Dictionary<int, int> LIGHT_ORIGIN = new Dictionary<int, int>()
        {
            {(int)Molecules.C13, (int)Molecules.C},
            {(int)Molecules.H2, (int)Molecules.H},
            {(int)Molecules.O17, (int)Molecules.O},
            {(int)Molecules.N15, (int)Molecules.N},
            {(int)Molecules.P32, (int)Molecules.P},
            {(int)Molecules.O18, (int)Molecules.O},
            {(int)Molecules.S34, (int)Molecules.S},
            {(int)Molecules.S33, (int)Molecules.S}
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
        
        
        public static Dictionary<int, int> createFilledElementDict(Dictionary<int, int> copy)
        {
            Dictionary<int, int> elements = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> kvp in copy) elements.Add(kvp.Key, kvp.Value);
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
            xml += " fragmentOutputName=\"" + fragmentOutputName + "\"";
            xml += " fragmentCharge=\"" + fragmentCharge + "\"";
            xml += " fragmentFile=\"" + fragmentFile + "\"";
            xml += " intensity=\"" + intensity + "\"";
            xml += " userDefined=\"" + userDefined + "\">";
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
            fragmentBase.Clear();
            fragmentName = node.Attribute("fragmentName").Value.ToString();
            fragmentOutputName = node.Attribute("fragmentOutputName").Value.ToString();
            fragmentCharge = Convert.ToInt32(node.Attribute("fragmentCharge").Value.ToString());
            fragmentFile = node.Attribute("fragmentFile").Value.ToString();
            intensity = Convert.ToInt32(node.Attribute("intensity").Value.ToString());
            userDefined = node.Attribute("userDefined").Value.Equals("True");
            
            
            foreach(XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {       
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
        
        
        // TODO: compute fragment intensity based on parameterized
        // model depending on collision energy 
        public double computeIntensity(double collisionEnergy = 0)
        {
            return intensity;
        }
        
        
    
        public MS2Fragment()
        {
            fragmentName = "-";
            fragmentOutputName = "-";
            fragmentCharge = -1;
            fragmentFile = "-";
            fragmentElements = new Dictionary<int, int>();
            foreach (KeyValuePair<int, string> kvp in ELEMENT_SHORTCUTS) fragmentElements.Add(kvp.Key, 0);
            fragmentBase = new ArrayList();
            userDefined = false;
            intensity = DEFAULT_INTENSITY;
        }
        
        

        public MS2Fragment(String name, String fileName)
        {
            fragmentName = name;
            fragmentOutputName = name;
            fragmentCharge = -1;
            fragmentFile = fileName;
            fragmentElements = new Dictionary<int, int>();
            foreach (KeyValuePair<int, string> kvp in ELEMENT_SHORTCUTS) fragmentElements.Add(kvp.Key, 0);
            fragmentBase = new ArrayList();
            userDefined = false;
            intensity = DEFAULT_INTENSITY;
        }


        public MS2Fragment(String name, String fileName, int charge)
        {
            fragmentName = name;
            fragmentOutputName = name;
            fragmentCharge = charge;
            fragmentFile = fileName;
            fragmentElements = new Dictionary<int, int>();
            foreach (KeyValuePair<int, string> kvp in ELEMENT_SHORTCUTS) fragmentElements.Add(kvp.Key, 0);
            fragmentBase = new ArrayList();
            userDefined = false;
            intensity = DEFAULT_INTENSITY;
        }
        
        
        
        public MS2Fragment(String name, String outputname, int charge, String fileName, Dictionary<int, int> dataElements, String baseForms)
        {
            fragmentName = name;
            fragmentOutputName = outputname;
            fragmentCharge = charge;
            fragmentFile = fileName;
            fragmentElements = dataElements;
            fragmentBase = new ArrayList(baseForms.Split(new char[] {';'}));
            userDefined = false;
            intensity = DEFAULT_INTENSITY;
        }
        
        
        
        public MS2Fragment(String name, String outputname, int charge, String fileName, Dictionary<int, int> dataElements, String baseForms, double intens)
        {
            fragmentName = name;
            fragmentOutputName = outputname;
            fragmentCharge = charge;
            fragmentFile = fileName;
            fragmentElements = dataElements;
            fragmentBase = new ArrayList(baseForms.Split(new char[] {';'}));
            userDefined = false;
            intensity = Math.Min(DEFAULT_INTENSITY, Math.Max(0, intens));
        }

        
        
        public MS2Fragment(MS2Fragment copy)
        {
            fragmentName = copy.fragmentName;
            fragmentOutputName = copy.fragmentOutputName;
            fragmentCharge = copy.fragmentCharge;
            fragmentFile = copy.fragmentFile;
            fragmentElements = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> kvp in copy.fragmentElements) fragmentElements.Add(kvp.Key, kvp.Value);
            fragmentBase = new ArrayList();
            userDefined = copy.userDefined;
            foreach (string fbase in copy.fragmentBase) fragmentBase.Add(fbase);
            intensity = copy.intensity;
        }
    }
}