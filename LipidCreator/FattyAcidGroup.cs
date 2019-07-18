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
using System.Linq;
using System.Text;

namespace LipidCreator
{   

    [Serializable]
    public class FattyAcidGroupEnumerator : IEnumerator
    {
        public ArrayList list;
        public int index;
        public FattyAcidGroup Current
        {
            get { return (FattyAcidGroup)list[index]; }
        }
        
        public FattyAcidGroupEnumerator(Lipid lipid)
        {
            index = -1;
            list = lipid.getFattyAcidGroups();
        }
        
        public bool MoveNext()
        {
            ++index;
            return index < list.Count;
        }
        
        public void Reset()
        {
            index = -1;
        }
        

        object IEnumerator.Current { get { return Current; } }
    }


    [Serializable]
    public class FattyAcidGroup
    {
        public int chainType; // 0 = no restriction, 1 = odd carbon number, 2 = even carbon number
        public string lengthInfo;
        public string dbInfo;
        public string hydroxylInfo;
        public bool isLCB;
        public Dictionary<String, bool> faTypes;
        public HashSet<int> carbonCounts;
        public HashSet<int> doubleBondCounts;
        public HashSet<int> hydroxylCounts;
    
        public FattyAcidGroup(bool isLCB = false)
        {
            this.isLCB = isLCB;
            chainType = 0;
            lengthInfo = "12-15";
            dbInfo = "0";
            hydroxylInfo = "0";
            faTypes = new Dictionary<String, bool>();
            faTypes.Add("FA", true);
            faTypes.Add("FAp", false);
            faTypes.Add("FAa", false);
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
            isLCB = copy.isLCB;
            hydroxylInfo = copy.hydroxylInfo;
            faTypes = new Dictionary<String, bool>();
            faTypes.Add("FA", copy.faTypes["FA"]);
            faTypes.Add("FAp", copy.faTypes["FAp"]);
            faTypes.Add("FAa", copy.faTypes["FAa"]);
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
        
        
        
        
        public ulong getHashCode()
        {
            unchecked
            {
                ulong hashCode = (ulong)(chainType << 10);
                
                int i = 1;
                foreach(int c in carbonCounts)
                {
                    hashCode += LipidCreator.rotateHash(LipidCreator.randomNumbers[c & 255], i & 63);
                    i++;
                }
                foreach(int db in doubleBondCounts)
                {
                    hashCode += LipidCreator.rotateHash(LipidCreator.randomNumbers[db & 255], i & 63);
                    i++;
                }
                foreach(int h in hydroxylCounts)
                {
                    hashCode += LipidCreator.rotateHash(LipidCreator.randomNumbers[h & 255], i & 63);
                    i++;
                }
                
                hashCode += LipidCreator.rotateHash(LipidCreator.randomNumbers[isLCB ? 2 : 8], i & 63);
                
                foreach (string faType in faTypes.Keys.Where(x => faTypes[x]))
                {
                    hashCode += LipidCreator.HashCode(faType);
                }
                
                return hashCode;
            }
        }
        
        
        
        public void import(XElement node, string importVersion)
        {
            chainType = Convert.ToInt32(node.Attribute("chainType").Value);
            lengthInfo = node.Attribute("lengthInfo").Value;
            dbInfo = node.Attribute("dbInfo").Value;
            hydroxylInfo = node.Attribute("hydroxylInfo").Value;
            isLCB = node.Attribute("isLCB").Value == "True";
            
            carbonCounts = LipidCreator.parseRange(lengthInfo, LipidCreator.MIN_CARBON_LENGTH,  LipidCreator.MAX_CARBON_LENGTH, (ChainType)chainType);
            doubleBondCounts = LipidCreator.parseRange(dbInfo, LipidCreator.MIN_DB_LENGTH,  LipidCreator.MAX_DB_LENGTH, ChainType.dbLength);
            hydroxylCounts = LipidCreator.parseRange(hydroxylInfo, LipidCreator.MIN_HYDROXY_LENGTH,  LipidCreator.MAX_HYDROXY_LENGTH, ChainType.hydroxylLength);
            
            
        
            foreach(XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "faType":
                        faTypes[child.Attribute("type").Value.ToString()] = child.Value == "1";
                        break;
                    /*
                    case "length":
                        carbonCounts.Add(Convert.ToInt32(child.Value.ToString()));
                        break;
                        
                    case "doublebond":
                        doubleBondCounts.Add(Convert.ToInt32(child.Value.ToString()));
                        break;
                        
                    case "hydroxyl":
                        hydroxylCounts.Add(Convert.ToInt32(child.Value.ToString()));
                        break;
                    */
                    default:
                        throw new Exception("Error for fatty acid group import");
                }
            }
        }
        
        
        
        
        
        public void serialize(StringBuilder sb)
        {
            sb.Append("<FattyAcidGroup");
            sb.Append(" chainType=\"" + (int)chainType + "\"");
            sb.Append(" isLCB=\"" + isLCB + "\"");
            sb.Append(" lengthInfo=\"" + lengthInfo + "\"");
            sb.Append(" dbInfo=\"" + dbInfo + "\"");
            sb.Append(" hydroxylInfo=\"" + hydroxylInfo + "\">\n");
            foreach (KeyValuePair<String, bool> item in faTypes)
            {
                sb.Append("<faType");
                sb.Append(" type=\"" + item.Key + "\">");
                sb.Append((item.Value ? 1 : 0));
                sb.Append("</faType>\n");
            }
            
            /*
            foreach (int len in carbonCounts)
            {
                sb.Append("<length>" + len + "</length>\n");
            }
            foreach (int db in doubleBondCounts)
            {
                sb.Append("<doublebond>" + db + "</doublebond>\n");
            }
            foreach (int hydroxyl in hydroxylCounts)
            {
                sb.Append("<hydroxyl>" + hydroxyl + "</hydroxyl>\n");
            }
            
            */
            sb.Append("</FattyAcidGroup>\n");
        }
        
        
        
        public bool anyFAChecked()
        {
            return faTypes["FA"] || faTypes["FAp"] || faTypes["FAa"];
        }
        
        // generator function for providing all possible carbon length / double bond /
        // hydroxyl / (ether / ester) bonding combinations for a fatty acid
        public System.Collections.Generic.IEnumerable<FattyAcid> getFattyAcids()
        {
            if (!faTypes["FAx"])
            {
                // iterate for all carbon lengths
                foreach (int fattyAcidLength in carbonCounts)
                {
                    int maxDoubleBond = (fattyAcidLength - 1) >> 1;
                    // iterate for all double bonds
                    foreach (int fattyAcidDoubleBond in doubleBondCounts)
                    {
                        // iterate for all hydroxyls
                        if (maxDoubleBond < fattyAcidDoubleBond) continue;
                        foreach (int fattyAcidHydroxyl in hydroxylCounts)
                        {
                            // iterate for all bondings
                            if (fattyAcidLength <= fattyAcidHydroxyl) continue;
                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair in faTypes)
                            {
                                if (fattyAcidKeyValuePair.Value && !(fattyAcidKeyValuePair.Key.Equals("FAp") && fattyAcidDoubleBond == 0)) yield return new FattyAcid(fattyAcidLength, fattyAcidDoubleBond, fattyAcidHydroxyl, fattyAcidKeyValuePair.Key, isLCB);
                            }
                        }
                    }
                }
            }
            else
            {
                yield return new FattyAcid(0, 0, 0, "FAx");
            }
        }
    }
}