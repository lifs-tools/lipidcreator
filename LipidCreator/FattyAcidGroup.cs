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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LipidCreator
{   
    [Serializable]
    public class FattyAcidGroup
    {
        public int chainType; // 0 = no restriction, 1 = odd carbon number, 2 = even carbon number
        public String lengthInfo;
        public String dbInfo;
        public String hydroxylInfo;
        public Dictionary<String, bool> faTypes;
        public HashSet<int> carbonCounts;
        public HashSet<int> doubleBondCounts;
        public HashSet<int> hydroxylCounts;
    
        public FattyAcidGroup()
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
        
        public FattyAcidGroup(FattyAcidGroup copy)
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
            string xml = "<FattyAcidGroup";
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
            xml += "</FattyAcidGroup>\n";
            return xml;
        }
        
        
        
        public bool anyFAChecked()
        {
            return faTypes["FA"] || faTypes["FAp"] || faTypes["FAe"];
        }
    }
}