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
using System.Globalization;

namespace LipidCreator
{    
    
    [Serializable]
    public class Precursor
    {
        public string name;
        public LipidCategory category;
        public DataTable elements;
        public string pathToImage;
        public Dictionary<string, bool> adductRestrictions;
        public int buildingBlockType;
        public bool derivative;
        public bool heavyLabeled;
        public ArrayList heavyLabeledPrecursors;
        public bool userDefined;
        public ArrayList userDefinedFattyAcids;
        
        
        public Precursor()
        {
            adductRestrictions = new Dictionary<string, bool>();
            elements = MS2Fragment.createEmptyElementTable();
            heavyLabeledPrecursors = new ArrayList();
            userDefined = false;
            userDefinedFattyAcids = null;
        }
        
        public string serialize()
        {
            string xml = "<Precursor name=\"" + name + "\" category=\"" + ((int)category).ToString() + "\" pathToImage=\"" + pathToImage + "\" buildingBlockType=\"" + buildingBlockType.ToString() + "\">\n";
            foreach (KeyValuePair<string, bool> adductRestriction in adductRestrictions)
            {
                xml += "<AdductRestriction key=\"" + adductRestriction.Key + "\" value=\"" + adductRestriction.Value + "\" />\n";
            }
            
            foreach (DataRow dr in elements.Rows)
            {
                xml += "<Element type=\"" + dr["Shortcut"] + "\">" + dr["Count"] + "</Element>\n";
            }
            
            if (userDefined)
            {
                xml += "<userDefinedFattyAcids>\n";
                foreach (DataTable table in userDefinedFattyAcids)
                {
                    xml += "<DataTable>\n";
                    foreach (DataRow dr in table.Rows)
                    {
                        xml += "<Element type=\"" + dr["Shortcut"] + "\">" + dr["Count"] + "</Element>\n";
                    }
                    xml += "</DataTable>\n";
                }
                xml += "</userDefinedFattyAcids>\n";
            }
            xml += "</Precursor>\n";
            return xml;
        }
        
        public void import(XElement node, string importVersion)
        {
            name = node.Attribute("name").Value;
            category = (LipidCategory)Convert.ToInt32(node.Attribute("category").Value);
            pathToImage = node.Attribute("pathToImage").Value;
            buildingBlockType = Convert.ToInt32(node.Attribute("buildingBlockType").Value);
            derivative = false;
            heavyLabeled = true;
            userDefined = true;            
            
            foreach(XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "AdductRestriction":
                        adductRestrictions.Add(child.Attribute("key").Value.ToString(), child.Attribute("value").Value.Equals("True"));
                        
                        break;
                        
                    case "Element":
                        elements.Rows[MS2Fragment.ELEMENT_POSITIONS[child.Attribute("type").Value.ToString()]]["Count"] = child.Value.ToString();
                        break;
                        
                    case "userDefinedFattyAcids":
                        userDefinedFattyAcids = new ArrayList();
                        var dataTables = child.Descendants("DataTable");
                        foreach ( var dataTable in dataTables)
                        {
                            DataTable elements = MS2Fragment.createEmptyElementTable();
                            foreach(XElement row in dataTable.Elements())
                            {
                                if (row.Name.ToString().Equals("Elements"))
                                {
                                    elements.Rows[MS2Fragment.ELEMENT_POSITIONS[row.Attribute("type").Value.ToString()]]["Count"] = row.Value.ToString();
                                }
                            }
                            userDefinedFattyAcids.Add(elements);
                        }
                        break;
                        
                    default:
                        throw new Exception();
                }
            }
        }
    }
}