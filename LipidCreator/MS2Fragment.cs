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
using System.Xml.Linq;
using System.Linq;
using System.Text;

namespace LipidCreator
{
    
    [Serializable]
    public class MS2Fragment
    {
        public string fragmentName;
        public string fragmentOutputName;
        public String fragmentFile;
        public ElementDictionary fragmentElements;
        public Adduct fragmentAdduct;
        public ArrayList fragmentBase;
        public double intensity;
        public bool userDefined;
        public const double MAX_INTENSITY = 100.0;
        public const double DEFAULT_INTENSITY = 10.0;
        public string CommentForSpectralLibrary { get { return fragmentName; } }
        
        public static Dictionary<Molecule, Element> ALL_ELEMENTS = new Dictionary<Molecule, Element>(){
            {Molecule.C, new Element("C", "C", "C", "C", 0, 12.0, false, new Molecule[]{Molecule.C13}, Molecule.C)},
            {Molecule.H, new Element("H", "H", "H", "H", 1, 1.007825035, false, new Molecule[]{Molecule.H2}, Molecule.H)},
            {Molecule.N, new Element("N", "N", "N", "N", 2, 14.0030740, false, new Molecule[]{Molecule.N15}, Molecule.N)},
            {Molecule.O, new Element("O", "O", "O", "O", 3, 15.99491463, false, new Molecule[]{Molecule.O17, Molecule.O18}, Molecule.O)},
            {Molecule.P, new Element("P", "P", "P", "P", 4, 30.973762, false, new Molecule[]{Molecule.P32}, Molecule.P)},
            {Molecule.S, new Element("S", "S", "S", "S", 5, 31.9720707, false, new Molecule[]{Molecule.S33, Molecule.S34}, Molecule.S)},
            {Molecule.H2, new Element("H'", "2H", "H2", "[2]H", 6, 2.014101779, true, new Molecule[]{}, Molecule.H)},
            {Molecule.C13, new Element("C'", "13C", "C13", "[13]C", 7, 13.0033548378, true, new Molecule[]{}, Molecule.C)},
            {Molecule.N15, new Element("N'", "15N", "N15", "[15]N", 8, 15.0001088984, true, new Molecule[]{}, Molecule.N)},
            {Molecule.O17, new Element("O'", "17O", "O17", "[17]O", 9, 16.9991315, true, new Molecule[]{}, Molecule.O)},
            {Molecule.O18, new Element("O''", "18O", "O18", "[18]O", 10, 17.9991604, true, new Molecule[]{}, Molecule.O)},
            {Molecule.P32, new Element("P'", "32P", "P32", "[32]P", 11, 31.973907274, true, new Molecule[]{}, Molecule.P)},
            {Molecule.S33, new Element("S'", "33S", "S33", "[33]S", 12, 32.97145876, true, new Molecule[]{}, Molecule.S)},
            {Molecule.S34, new Element("S''", "34S", "S34", "[34]S", 13, 33.96786690, true, new Molecule[]{}, Molecule.S)}
        };
        
        
        
        
        public static Dictionary<string, Molecule> ELEMENT_POSITIONS = new Dictionary<string, Molecule>(){
            {"C", Molecule.C},
            {"H", Molecule.H},
            {"N", Molecule.N},
            {"O", Molecule.O},
            {"P", Molecule.P},
            {"P'", Molecule.P32},
            {"S", Molecule.S},
            {"S'", Molecule.S34},
            {"S''", Molecule.S33},
            {"H'", Molecule.H2},
            {"C'", Molecule.C13},
            {"N'", Molecule.N15},
            {"O'", Molecule.O17},
            {"O''", Molecule.O18},
            {"2H", Molecule.H2},
            {"13C", Molecule.C13},
            {"15N", Molecule.N15},
            {"17O", Molecule.O17},
            {"18O", Molecule.O18},
            {"32P", Molecule.P32},
            {"34S", Molecule.S34},
            {"33S", Molecule.S33},
            {"H2", Molecule.H2},
            {"C13", Molecule.C13},
            {"N15", Molecule.N15},
            {"O17", Molecule.O17},
            {"O18", Molecule.O18},
            {"P32", Molecule.P32},
            {"S34", Molecule.S34},
            {"S33", Molecule.S33}
        };
        
        
        
        public static ElementDictionary createEmptyElementDict()
        {
            ElementDictionary elements = new ElementDictionary();
            foreach (Molecule elementKey in ALL_ELEMENTS.Keys) elements.Add(elementKey, 0);
            return elements;
        }
        
        
        
        public static ElementDictionary initializeElementDict(Dictionary<string, int> dict)
        {
            ElementDictionary elements = createEmptyElementDict();
            foreach (KeyValuePair<string, int> elementKvp in dict)
            {
                if (ELEMENT_POSITIONS.ContainsKey(elementKvp.Key))
                {
                    elements[ELEMENT_POSITIONS[elementKvp.Key]] = elementKvp.Value;
                }
            }
            return elements;
        }
        
        
        public static bool validElementDict(ElementDictionary dict)
        {
            foreach (int count in dict.Values)
            {
                if (count < 0) return false;
            }
            return true;
        }
        
        
        
        public static ElementDictionary createFilledElementDict(DataTable dt)
        {
            ElementDictionary elements = new ElementDictionary();
            foreach (DataRow dr in dt.Rows)
            {
                elements.Add(ELEMENT_POSITIONS[(string)dr["Shortcut"]], Convert.ToInt32(dr["Count"]));
            }
            return elements;
        }
        
        
        public static ElementDictionary createFilledElementDict(ElementDictionary copy)
        {
            ElementDictionary elements = new ElementDictionary();
            foreach (KeyValuePair<Molecule, int> kvp in copy) elements.Add(kvp.Key, kvp.Value);
            return elements;
        }
        
        
        public ElementDictionary copyElementDict()
        {
            ElementDictionary elements = new ElementDictionary();
            foreach (KeyValuePair<Molecule, int> kvp in fragmentElements) elements.Add(kvp.Key, kvp.Value);
            return elements;
        }
    
    
        public void serialize(StringBuilder sb)
        {
            sb.Append("<MS2Fragment");
            sb.Append(" fragmentName=\"" + fragmentName + "\"");
            sb.Append(" fragmentOutputName=\"" + fragmentOutputName + "\"");
            sb.Append(" fragmentAdduct=\"" + fragmentAdduct.name + "\"");
            sb.Append(" fragmentFile=\"" + fragmentFile + "\"");
            sb.Append(" intensity=\"" + intensity + "\"");
            sb.Append(" userDefined=\"" + userDefined + "\">");
            foreach (string fbase in fragmentBase)
            {
                sb.Append("<fragmentBase>" + fbase + "</fragmentBase>\n");
            }
            foreach (KeyValuePair<Molecule, int> kvp in fragmentElements)
            {
                sb.Append("<Element type=\"" + ALL_ELEMENTS[kvp.Key].shortcut + "\">" + Convert.ToString(kvp.Value) + "</Element>\n");
            }
            sb.Append("</MS2Fragment>\n");
        }
        
        public void import(XElement node, string importVersion)
        {
            fragmentBase.Clear();
            fragmentName = node.Attribute("fragmentName").Value.ToString();
            fragmentOutputName = node.Attribute("fragmentOutputName").Value.ToString();
            fragmentAdduct = Lipid.ALL_ADDUCTS[Lipid.ADDUCT_POSITIONS[node.Attribute("fragmentAdduct").Value.ToString()]];
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
        
        
    
        public static void addCounts(ElementDictionary counts1, ElementDictionary counts2)
        {
            foreach (KeyValuePair<Molecule, int> kvp in counts2) counts1[kvp.Key] += kvp.Value;
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
            fragmentFile = "-";
            fragmentAdduct = Lipid.ALL_ADDUCTS[AdductType.Hp];
            fragmentElements = new ElementDictionary();
            foreach (Molecule element in ALL_ELEMENTS.Keys) fragmentElements.Add(element, 0);
            fragmentBase = new ArrayList();
            userDefined = false;
            intensity = DEFAULT_INTENSITY;
        }
        
        

        public MS2Fragment(String name, String fileName)
        {
            fragmentName = name;
            fragmentOutputName = name;
            fragmentFile = fileName;
            fragmentElements = new ElementDictionary();
            fragmentAdduct = Lipid.ALL_ADDUCTS[AdductType.Hp];
            foreach (Molecule element in ALL_ELEMENTS.Keys) fragmentElements.Add(element, 0);
            fragmentBase = new ArrayList();
            userDefined = false;
            intensity = DEFAULT_INTENSITY;
        }


        public MS2Fragment(String name, String fileName, Adduct _adduct)
        {
            fragmentName = name;
            fragmentOutputName = name;
            fragmentFile = fileName;
            fragmentElements = new ElementDictionary();
            fragmentAdduct = _adduct;
            foreach (Molecule element in ALL_ELEMENTS.Keys) fragmentElements.Add(element, 0);
            fragmentBase = new ArrayList();
            userDefined = false;
            intensity = DEFAULT_INTENSITY;
        }
        
        
        
        public MS2Fragment(String name, String outputname, Adduct _adduct, String fileName, ElementDictionary dataElements, String baseForms)
        {
            fragmentName = name;
            fragmentOutputName = outputname;
            fragmentFile = fileName;
            fragmentAdduct = _adduct;
            fragmentElements = dataElements;
            fragmentBase = new ArrayList(baseForms.Split(new char[] {';'}));
            userDefined = false;
            intensity = DEFAULT_INTENSITY;
        }
        
        
        
        public MS2Fragment(String name, String outputname, Adduct _adduct, String fileName, ElementDictionary dataElements, String baseForms, double intens)
        {
            fragmentName = name;
            fragmentOutputName = outputname;
            fragmentFile = fileName;
            fragmentAdduct = _adduct;
            fragmentElements = dataElements;
            fragmentBase = new ArrayList(baseForms.Split(new char[] {';'}));
            userDefined = false;
            intensity = Math.Min(DEFAULT_INTENSITY, Math.Max(0, intens));
        }

        
        
        public MS2Fragment(MS2Fragment copy)
        {
            fragmentName = copy.fragmentName;
            fragmentOutputName = copy.fragmentOutputName;
            fragmentFile = copy.fragmentFile;
            fragmentElements = new ElementDictionary();
            fragmentAdduct = copy.fragmentAdduct;
            foreach (KeyValuePair<Molecule, int> kvp in copy.fragmentElements) fragmentElements.Add(kvp.Key, kvp.Value);
            fragmentBase = new ArrayList();
            userDefined = copy.userDefined;
            foreach (string fbase in copy.fragmentBase) fragmentBase.Add(fbase);
            intensity = copy.intensity;
        }
    }
}