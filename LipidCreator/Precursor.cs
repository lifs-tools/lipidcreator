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
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace LipidCreator
{    
    
    [Serializable]
    public class Precursor
    {
        public string name;
        public LipidCategory category;
        public ElementDictionary elements;
        public string pathToImage;
        public string pathToBackboneImage;
        public Dictionary<string, bool> adductRestrictions;
        public int buildingBlockType;
        public string trivialName;
        public bool derivative;
        public ArrayList heavyLabeledPrecursors;
        public bool userDefined;
        public string defaultAdduct;
        public ArrayList userDefinedFattyAcids;
        public HashSet<string> attributes;
        
        public static int[] fattyAcidCount = new int[]{4, 3, 2, 1, 2, 1, 0};
        
        
        public Precursor()
        {
            adductRestrictions = new Dictionary<string, bool>();
            elements = MS2Fragment.createEmptyElementDict();
            heavyLabeledPrecursors = new ArrayList();
            userDefined = false;
            trivialName = "";
            pathToImage = "";
            pathToBackboneImage = "";
            userDefinedFattyAcids = null;
            attributes = new HashSet<string>();
        }
        
        public void serialize(StringBuilder sb)
        {
            sb.Append("<Precursor name=\"" + name + "\" category=\"" + ((int)category).ToString() + "\" pathToImage=\"" + pathToImage + "\" buildingBlockType=\"" + buildingBlockType.ToString() + "\" derivative=\"" + derivative + "\" userDefined=\"" + userDefined + "\">\n");
            foreach (KeyValuePair<string, bool> adductRestriction in adductRestrictions)
            {
                sb.Append("<AdductRestriction key=\"" + adductRestriction.Key + "\" value=\"" + adductRestriction.Value + "\" />\n");
            }
            foreach (KeyValuePair<Molecule, int> kvp in elements)
            {
                sb.Append("<Element type=\"" + MS2Fragment.ALL_ELEMENTS[kvp.Key].shortcut + "\">" + Convert.ToString(kvp.Value) + "</Element>\n");
            }
            foreach (string attribute in attributes)
            {
                sb.Append("<Attribute>" + attribute + "</Attribute>\n");
            }
            
            if (userDefined)
            {
                sb.Append("<userDefinedFattyAcids>\n");
                foreach (ElementDictionary table in userDefinedFattyAcids)
                {
                    sb.Append("<DataTable>\n");
                    foreach (KeyValuePair<Molecule, int> kvp in table)
                    {
                        sb.Append("<Element type=\"" + MS2Fragment.ALL_ELEMENTS[kvp.Key].shortcut + "\">" + Convert.ToString(kvp.Value) + "</Element>\n");
                    }
                    sb.Append("</DataTable>\n");
                }
                sb.Append("</userDefinedFattyAcids>\n");
            }
            sb.Append("</Precursor>\n");
        }
        
        
        
        
        public void import(XElement node, string importVersion)
        {
            name = node.Attribute("name").Value;
            category = (LipidCategory)Convert.ToInt32(node.Attribute("category").Value);
            pathToImage = node.Attribute("pathToImage").Value;
            buildingBlockType = Convert.ToInt32(node.Attribute("buildingBlockType").Value);
            derivative = node.Attribute("derivative").Value.Equals("True");
            userDefined = node.Attribute("userDefined").Value.Equals("True");
            
            foreach(XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "AdductRestriction":
                        adductRestrictions.Add(child.Attribute("key").Value.ToString(), child.Attribute("value").Value.Equals("True"));
                        break;
                        
                    case "Element":
                        elements[MS2Fragment.ELEMENT_POSITIONS[child.Attribute("type").Value.ToString()]] = Convert.ToInt32(child.Value.ToString());
                        break;
                        
                    case "Attribute":
                        attributes.Add(child.Value.ToString());
                        break;
                        
                    case "userDefinedFattyAcids":
                        userDefinedFattyAcids = new ArrayList();
                        var dataTables = child.Descendants("DataTable");
                        foreach ( var dataTable in dataTables)
                        {
                            ElementDictionary fattyElements = MS2Fragment.createEmptyElementDict();
                            foreach(XElement row in dataTable.Elements())
                            {
                                if (row.Name.ToString().Equals("Element"))
                                {
                                    fattyElements[MS2Fragment.ELEMENT_POSITIONS[row.Attribute("type").Value.ToString()]] = Convert.ToInt32(row.Value.ToString());
                                }
                            }
                            userDefinedFattyAcids.Add(fattyElements);
                        }
                        break;
                        
                    default:
                        throw new Exception();
                }
            }
        }
    }
}
