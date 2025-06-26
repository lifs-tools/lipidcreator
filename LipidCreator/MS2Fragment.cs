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
        public string fragmentFile;
        public ElementDictionary fragmentElements;
        public Adduct fragmentAdduct;
        public HashSet<string> fragmentBase = new HashSet<string>();
        public bool specific;
        public bool userDefined;
        public const double MAX_INTENSITY = 100.0;
        public const double DEFAULT_INTENSITY = 10.0;
        public string CommentForSpectralLibrary { get { return fragmentName; } }
        public static HashSet<string> UNSPECIFIC_BLOCKS = new HashSet<string>(){"HG", "M"};

        public static Dictionary<Molecule, Element> ALL_ELEMENTS = new Dictionary<Molecule, Element>(){
            {Molecule.C, new Element("C", "C", "C", "C", 0, 12.0, false, new Molecule[]{Molecule.C13}, Molecule.C)},
            {Molecule.H, new Element("H", "H", "H", "H", 1, 1.007825035, false, new Molecule[]{Molecule.H2}, Molecule.H)},
            {Molecule.B, new Element("B", "B", "B", "B", 2, 11.0093052, false, new Molecule[]{Molecule.B10}, Molecule.B)},
            {Molecule.N, new Element("N", "N", "N", "N", 3, 14.0030740, false, new Molecule[]{Molecule.N15}, Molecule.N)},
            {Molecule.O, new Element("O", "O", "O", "O", 4, 15.99491463, false, new Molecule[]{Molecule.O17, Molecule.O18}, Molecule.O)},
            {Molecule.F, new Element("F", "F", "F", "F", 5, 18.998403162, false, new Molecule[]{Molecule.F}, Molecule.F)},
            {Molecule.P, new Element("P", "P", "P", "P", 6, 30.973762, false, new Molecule[]{Molecule.P32}, Molecule.P)},
            {Molecule.S, new Element("S", "S", "S", "S", 7, 31.9720707, false, new Molecule[]{Molecule.S33, Molecule.S34}, Molecule.S)},
            {Molecule.Cl, new Element("Cl", "Cl", "Cl", "Cl", 8, 34.968852694, false, new Molecule[]{Molecule.Cl37}, Molecule.Cl)},
            {Molecule.As, new Element("As", "As", "As", "As", 9, 74.92159456, false, new Molecule[]{Molecule.As}, Molecule.As)},
            {Molecule.Br, new Element("Br", "Br", "Br", "Br", 10, 78.9183376, false, new Molecule[]{Molecule.Br81}, Molecule.Br)},
            {Molecule.I, new Element("I", "I", "I", "I", 11, 126.9044726, false, new Molecule[]{Molecule.I}, Molecule.I)},
            {Molecule.H2, new Element("H'", "2H", "H2", "[2]H", 12, 2.014101779, true, new Molecule[]{}, Molecule.H)},
            {Molecule.C13, new Element("C'", "13C", "C13", "[13]C", 13, 13.0033548378, true, new Molecule[]{}, Molecule.C)},
            {Molecule.N15, new Element("N'", "15N", "N15", "[15]N", 14, 15.0001088984, true, new Molecule[]{}, Molecule.N)},
            {Molecule.O17, new Element("O'", "17O", "O17", "[17]O", 15, 16.9991315, true, new Molecule[]{}, Molecule.O)},
            {Molecule.O18, new Element("O''", "18O", "O18", "[18]O", 16, 17.9991604, true, new Molecule[]{}, Molecule.O)},
            {Molecule.P32, new Element("P'", "32P", "P32", "[32]P", 17, 31.973907274, true, new Molecule[]{}, Molecule.P)},
            {Molecule.S33, new Element("S'", "33S", "S33", "[33]S", 18, 32.97145876, true, new Molecule[]{}, Molecule.S)},
            {Molecule.S34, new Element("S''", "34S", "S34", "[34]S", 19, 33.96786690, true, new Molecule[]{}, Molecule.S)},
            {Molecule.B10, new Element("B'", "10B", "B10", "[10]B", 20, 10.01293695, true, new Molecule[]{}, Molecule.B)},
            {Molecule.Cl37, new Element("Cl'", "37Cl", "Cl37", "[37]Cl", 21, 36.965902602, true, new Molecule[]{}, Molecule.Cl)},
            {Molecule.Br81, new Element("Br'", "81Br", "B81", "[81]B", 22, 80.9162897, true, new Molecule[]{}, Molecule.Br)}
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
            {"S33", Molecule.S33},
            {"B",Molecule.B},
            {"F", Molecule.F},
            {"Cl", Molecule.Cl},
            {"As", Molecule.As},
            {"Br", Molecule.Br},
            {"I", Molecule.I},
            {"'B",Molecule.B10},
            {"10B",Molecule.B10},
            {"B10",Molecule.B10},
            {"'Cl",Molecule.Cl37},
            {"37Cl",Molecule.Cl37},
            {"Cl37",Molecule.Cl37},
            {"'Br",Molecule.Br81},
            {"81Br",Molecule.Br81},
            {"Br81",Molecule.Br81}
        };



        public static ElementDictionary createEmptyElementDict()
        {
            return new ElementDictionary();
        }



        public static ElementDictionary initializeElementDict(Dictionary<string, int> dict)
        {
            ElementDictionary elements = createEmptyElementDict();
            foreach (KeyValuePair<string, int> elementKvp in dict)
            {
                if (ELEMENT_POSITIONS.ContainsKey(elementKvp.Key))
                {
                    elements[(int)ELEMENT_POSITIONS[elementKvp.Key]] = elementKvp.Value;
                }
            }
            return elements;
        }


        public static bool validElementDict(ElementDictionary elements)
        {
            foreach (int count in elements)
            {
                if (count < 0) return false;
            }
            return true;
        }
        
        
        
        
        public static void updateForHeavyLabeled(ElementDictionary originElements, ElementDictionary updateElements)
        {
            for (int m = 0; m < updateElements.Count; ++m)
            {
                if (MS2Fragment.ALL_ELEMENTS[(Molecule)m].isHeavy) continue;
                Molecule monoIsotopic = MS2Fragment.ALL_ELEMENTS[(Molecule)m].lightOrigin;
                originElements[(int)monoIsotopic] -= updateElements[m];
                originElements[m] += updateElements[m];
            }
        }
        
        
        





        public ulong getHashCode()
        {
            unchecked {
                ulong hashCode = LipidCreator.HashCode(fragmentName);
                hashCode += LipidCreator.HashCode(fragmentOutputName);
                hashCode += LipidCreator.HashCode(fragmentFile);
                hashCode += fragmentElements.getHashCode();
                hashCode += fragmentAdduct.getHashCode();
                foreach (string fragBase in fragmentBase)
                {
                    hashCode += LipidCreator.HashCode(fragBase);
                }
                hashCode += specific ? (1UL << 23) : (1UL << 60);
                hashCode += userDefined ? (1UL << 35) : (1UL << 48);
                return hashCode;
            }            
        }
        
        
        public static ElementDictionary createFilledElementDict(DataTable dt)
        {
            ElementDictionary elements = new ElementDictionary();
            foreach (DataRow dr in dt.Rows)
            {
                elements[(int)ELEMENT_POSITIONS[(string)dr["Shortcut"]]] = Convert.ToInt32(dr["Count"]);
            }
            return elements;
        }
        
        
        public static ElementDictionary createFilledElementDict(ElementDictionary copy)
        {
            ElementDictionary elements = new ElementDictionary();
            for (int m = 0; m < copy.Count; ++m) elements[m] = copy[m];
            return elements;
        }
        
        
        public ElementDictionary copyElementDict()
        {
            ElementDictionary elements = new ElementDictionary();
            for (int m = 0; m < fragmentElements.Count; ++m) elements[m] = fragmentElements[m];
            return elements;
        }
    
    
        public void serialize(StringBuilder sb)
        {
            sb.Append("<MS2Fragment");
            sb.Append(" fragmentName=\"" + fragmentName + "\"");
            sb.Append(" fragmentOutputName=\"" + fragmentOutputName + "\"");
            sb.Append(" fragmentAdduct=\"" + fragmentAdduct.name + "\"");
            sb.Append(" fragmentFile=\"" + fragmentFile + "\"");
            sb.Append(" specific=\"" + (specific ? "1" : "0") + "\"");
            sb.Append(" userDefined=\"" + userDefined + "\">");
            foreach (string fbase in fragmentBase)
            {
                sb.Append("<fragmentBase>" + fbase + "</fragmentBase>\n");
            }
            for (int m = 0; m < fragmentElements.Count; ++m)
            {
                sb.Append("<Element type=\"" + ALL_ELEMENTS[(Molecule)m].shortcut + "\">" + Convert.ToString(fragmentElements[m]) + "</Element>\n");
            }
            sb.Append("</MS2Fragment>\n");
        }
        
        
        
        
        
        public void import(XElement node, string importVersion)
        {
            fragmentBase.Clear();
            fragmentName = ((string)node.Attribute("fragmentName") != null) ? node.Attribute("fragmentName").Value.ToString() : "";
            fragmentOutputName = ((string)node.Attribute("fragmentOutputName") != null) ? node.Attribute("fragmentOutputName").Value.ToString() : "";
            fragmentAdduct = ((string)node.Attribute("fragmentAdduct") != null) ? Lipid.ALL_ADDUCTS[Lipid.ADDUCT_POSITIONS[node.Attribute("fragmentAdduct").Value.ToString()]] : Lipid.ALL_ADDUCTS[AdductType.Hp];
            fragmentFile = ((string)node.Attribute("fragmentFile") != null) ? node.Attribute("fragmentFile").Value.ToString() : "";
            specific = ((string)node.Attribute("specific") != null) ? node.Attribute("specific").Value.ToString() == "1" : false;
            userDefined = ((string)node.Attribute("userDefined") != null) ? node.Attribute("userDefined").Value.Equals("True") : false;
            
            
            foreach(XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {       
                    case "fragmentBase":
                        fragmentBase.Add(child.Value.ToString());
                        break;
                        
                    case "Element":
                        fragmentElements[(int)ELEMENT_POSITIONS[child.Attribute("type").Value.ToString()]] = Convert.ToInt32(child.Value.ToString());
                        break;
                        
                    default:
                        break;
                }
            }
        }
        
        
        
        
        
    
        public static void addCounts(ElementDictionary counts1, ElementDictionary counts2)
        {
            for (int m = 0; m < counts2.Count; ++m)
            {
                counts1[m] += counts2[m];
            }
        }
        
        
        
        
        
    
        public static void subtractCounts(ElementDictionary counts1, ElementDictionary counts2)
        {
            for (int m = 0; m < counts2.Count; ++m)
            {
                counts1[m] -= counts2[m];
            }
        }
        
        
        
        
        
        // TODO: compute fragment intensity based on parameterized
        // model depending on collision energy 
        /*
        public double computeIntensity(double collisionEnergy = 0)
        {
            return intensity;
        }
        */
        
        
        
        
    
        public MS2Fragment()
        {
            fragmentName = "-";
            fragmentOutputName = "-";
            fragmentFile = "-";
            fragmentAdduct = Lipid.ALL_ADDUCTS[AdductType.Hp];
            fragmentElements = new ElementDictionary();
            userDefined = false;
            specific = false;
        }
        
        

        public MS2Fragment(String name, String fileName)
        {
            fragmentName = name;
            fragmentOutputName = name;
            fragmentFile = fileName;
            fragmentElements = new ElementDictionary();
            fragmentAdduct = Lipid.ALL_ADDUCTS[AdductType.Hp];
            userDefined = false;
            specific = false;
        }


        public MS2Fragment(String name, String fileName, Adduct _adduct)
        {
            fragmentName = name;
            fragmentOutputName = name;
            fragmentFile = fileName;
            fragmentElements = new ElementDictionary();
            fragmentAdduct = _adduct;
            userDefined = false;
            specific = false;
        }
        
        
        
        public MS2Fragment(String name, String outputname, Adduct _adduct, String fileName, ElementDictionary dataElements, String baseForms)
        {
            fragmentName = name;
            fragmentOutputName = outputname;
            fragmentFile = fileName;
            fragmentAdduct = _adduct;
            fragmentElements = dataElements;
            if (baseForms.Length > 0)
            {
                foreach (string bb in baseForms.Split(new char[] {';'})) fragmentBase.Add(bb);
            }
            userDefined = false;
            
            HashSet<string> f = new HashSet<string>(fragmentBase);
            f.ExceptWith(UNSPECIFIC_BLOCKS);
            specific = f.Count == 0;
        }
        
        
        
        public MS2Fragment(String name, String outputname, Adduct _adduct, String fileName, ElementDictionary dataElements, String baseForms, bool _specific)
        {
            fragmentName = name;
            fragmentOutputName = outputname;
            fragmentFile = fileName;
            fragmentAdduct = _adduct;
            fragmentElements = dataElements;
            if (baseForms.Length > 0)
            {
                foreach (string bb in baseForms.Split(new char[] {';'})) fragmentBase.Add(bb);
            }
            userDefined = false;
            
            HashSet<string> f = new HashSet<string>(fragmentBase);
            f.ExceptWith(UNSPECIFIC_BLOCKS);
            specific = f.Count == 0;
            //specific = _specific;
        }

        
        
        public MS2Fragment(MS2Fragment copy)
        {
            fragmentName = copy.fragmentName;
            fragmentOutputName = copy.fragmentOutputName;
            fragmentFile = copy.fragmentFile;
            fragmentElements = new ElementDictionary();
            fragmentAdduct = copy.fragmentAdduct;
            for (int m = 0; m < copy.fragmentElements.Count; ++m) fragmentElements[m] = copy.fragmentElements[m];
            userDefined = copy.userDefined;
            foreach (string fbase in copy.fragmentBase) fragmentBase.Add(fbase);
            specific = copy.specific;
        }
    }
}
