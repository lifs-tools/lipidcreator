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
        public Dictionary<FattyAcidType, bool> faTypes;
        public HashSet<int> carbonCounts;
        public HashSet<int> doubleBondCounts;
        public HashSet<int> hydroxylCounts;
        public DataTable functionalGroups = new DataTable();
        public Dictionary<FunctionalGroupType, HashSet<int>> functionalGroupCounts = null;
    
        public FattyAcidGroup(bool isLCB = false, bool dummy = false)
        {
            this.isLCB = isLCB;
            chainType = 0;
            lengthInfo = "12-15";
            dbInfo = "0";
            hydroxylInfo = "0";
            faTypes = new Dictionary<FattyAcidType, bool>();
            faTypes.Add(FattyAcidType.Ester, !dummy);
            faTypes.Add(FattyAcidType.Plasmanyl, false);
            faTypes.Add(FattyAcidType.Plasmenyl, false);
            faTypes.Add(FattyAcidType.NoType, dummy);  // no fatty acid dummy
            carbonCounts = new HashSet<int>();
            doubleBondCounts = new HashSet<int>();
            hydroxylCounts = new HashSet<int>();
            functionalGroups.Columns.Add("Func. group", typeof(string));
            functionalGroups.Columns.Add("Range", typeof(string));
        }
        
        public FattyAcidGroup(FattyAcidGroup copy)
        {
            chainType = copy.chainType;
            lengthInfo = copy.lengthInfo;
            dbInfo = copy.dbInfo;
            isLCB = copy.isLCB;
            hydroxylInfo = copy.hydroxylInfo;
            faTypes = new Dictionary<FattyAcidType, bool>();
            faTypes.Add(FattyAcidType.Ester, copy.faTypes[FattyAcidType.Ester]);
            faTypes.Add(FattyAcidType.Plasmanyl, copy.faTypes[FattyAcidType.Plasmanyl]);
            faTypes.Add(FattyAcidType.Plasmenyl, copy.faTypes[FattyAcidType.Plasmenyl]);
            faTypes.Add(FattyAcidType.NoType, copy.faTypes[FattyAcidType.NoType]);  // no fatty acid dummy
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
            
            functionalGroups.Columns.Add("Func. group", typeof(string));
            functionalGroups.Columns.Add("Range", typeof(string));
            
            foreach (DataRow copyDataRow in copy.functionalGroups.Rows)
            {
                DataRow dr = functionalGroups.NewRow();
                dr[0] = copyDataRow[0];
                dr[1] = copyDataRow[1];
                functionalGroups.Rows.Add(dr);
            }
            createFunctionalGroupCount();
        }
        
        
        public void createFunctionalGroupCount()
        {
            if (functionalGroupCounts == null) functionalGroupCounts = new Dictionary<FunctionalGroupType, HashSet<int>>();
            else functionalGroupCounts.Clear();
            
            foreach (DataRow dataRow in functionalGroups.Rows)
            {
                string key = (string)dataRow[0];
                FunctionalGroupType fgt = Lipid.FUNCTIONAL_GROUP_POSITIONS[key];
                string value = (string)dataRow[1];
                
                int minRange = LipidCreator.MIN_HYDROXY_LENGTH;
                int maxRange = LipidCreator.MAX_HYDROXY_LENGTH;
                var parsed = LipidCreator.parseRange(value, minRange,  maxRange, (ChainType)4);
                if (parsed == null)
                {
                    functionalGroupCounts = null;
                    break;
                }
                else
                {
                    if (!functionalGroupCounts.ContainsKey(fgt)) functionalGroupCounts.Add(fgt, new HashSet<int>());
                    functionalGroupCounts[fgt].UnionWith(parsed);
                }
            }
        }
        
        
        public override string ToString()
        {
            string faLCB = (isLCB) ? "long chain base" : "fatty acyl";
            string allFaTypes = "";
            string restrictions = chainType == 0 ? "no restriction" : (chainType == 1 ? "odd chain numbers" : "even chain numbers"); 
            foreach (KeyValuePair<FattyAcidType, bool> kvp in faTypes)
            {
                if (kvp.Value)
                {
                    if (allFaTypes.Length > 0)
                    {
                        allFaTypes += ", ";
                    }
                    allFaTypes += kvp.Key;
                }
            }
            return String.Format("The {0} group of length {1} ({2}), with double bonds {3}, hydroxlations {4}, bond types {5}, and {6}", faLCB, String.Join(",", carbonCounts), lengthInfo, dbInfo, hydroxylInfo, allFaTypes, restrictions);
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
                
                foreach (FattyAcidType faType in faTypes.Keys.Where(x => faTypes[x]))
                {
                    hashCode += Lipid.FAHashCode[faType];
                }
                
                foreach (DataRow dataRow in functionalGroups.Rows)
                {
                    hashCode += LipidCreator.HashCode((string)dataRow[0]) + LipidCreator.HashCode((string)dataRow[1]);
                }
                
                return hashCode;
            }
        }
        
        
        
        public string functionalGroupsInfo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow dataRow in functionalGroups.Rows)
            {
                if (sb.Length > 0) sb.Append(", ");
                string key = (string)dataRow[0];
                FunctionalGroupType fgt = Lipid.FUNCTIONAL_GROUP_POSITIONS[key];
                sb.Append(Lipid.ALL_FUNCTIONAL_GROUPS[fgt].abbreviation).Append(": ").Append((string)dataRow[1]);
            }
            string s = sb.ToString();
            return (s.Length == 0) ? "" : "; " + s;
        }
        
        
        
        public void import(XElement node, string importVersion)
        {
            chainType = ((string)node.Attribute("chainType") != null) ? Convert.ToInt32(node.Attribute("chainType").Value) : 0;
            lengthInfo = ((string)node.Attribute("lengthInfo") != null) ? node.Attribute("lengthInfo").Value : "0";
            dbInfo = ((string)node.Attribute("dbInfo") != null) ? node.Attribute("dbInfo").Value : "0";
            hydroxylInfo = ((string)node.Attribute("hydroxylInfo") != null) ? node.Attribute("hydroxylInfo").Value : "0";
            isLCB = ((string)node.Attribute("isLCB") != null) ? node.Attribute("isLCB").Value == "True" : false;
            
            carbonCounts = LipidCreator.parseRange(lengthInfo, LipidCreator.MIN_CARBON_LENGTH, LipidCreator.MAX_CARBON_LENGTH, (ChainType)chainType);
            doubleBondCounts = LipidCreator.parseRange(dbInfo, LipidCreator.MIN_DB_LENGTH, LipidCreator.MAX_DB_LENGTH, ChainType.dbLength);
            hydroxylCounts = LipidCreator.parseRange(hydroxylInfo, LipidCreator.MIN_HYDROXY_LENGTH, LipidCreator.MAX_HYDROXY_LENGTH, ChainType.hydroxylLength);
            
            foreach(XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "faType":
                        int fatype = Convert.ToInt32(child.Attribute("type").Value.ToString());
                        if ((string)child.Attribute("type") != null) faTypes[Lipid.FattyAcidTypeOrder[fatype]] = child.Value == "1";
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
                        break;
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
            
            foreach (KeyValuePair<FattyAcidType, bool> item in faTypes)
            {
                sb.Append("<faType");
                sb.Append(" type=\"" + (int)item.Key + "\">");
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
            return faTypes[FattyAcidType.Ester] || faTypes[FattyAcidType.Plasmanyl] || faTypes[FattyAcidType.Plasmenyl];
        }
        
        
        
        
        
        
        
        
        // generator function for providing all possible carbon length / double bond /
        // hydroxyl / (ether / ester) bonding combinations for a fatty acid
        public System.Collections.Generic.IEnumerable<FattyAcid> getFattyAcids()
        {
            if (!faTypes[FattyAcidType.NoType])
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
                        foreach (KeyValuePair<FattyAcidType, bool> fattyAcidKeyValuePair in faTypes)
                        {
                            if (!fattyAcidKeyValuePair.Value) continue;
                            
                            if (isLCB)
                            {
                                foreach (int fattyAcidHydroxyl in hydroxylCounts)
                                {
                                    if (fattyAcidLength <= fattyAcidHydroxyl) continue;
                                    yield return new FattyAcid(fattyAcidLength, fattyAcidDoubleBond, fattyAcidHydroxyl, fattyAcidKeyValuePair.Key, isLCB);
                                }
                                continue;
                            }
                            
                            if (functionalGroupCounts == null || functionalGroupCounts.Count == 0)
                            {
                                yield return new FattyAcid(fattyAcidLength, fattyAcidDoubleBond, 0, fattyAcidKeyValuePair.Key, isLCB);
                            }
                            else 
                            {
                                Dictionary<FunctionalGroupType, int> funcGroups = new Dictionary<FunctionalGroupType, int>();
                                Dictionary<FunctionalGroupType, List<int>> funcGroupsPossibilities = new Dictionary<FunctionalGroupType, List<int>>();
                                Dictionary<FunctionalGroupType, int[]> divisors = new Dictionary<FunctionalGroupType, int[]>();
                                
                                int combinations = 1;
                                foreach(KeyValuePair<FunctionalGroupType, HashSet<int>> kvp in functionalGroupCounts)
                                {
                                    funcGroupsPossibilities.Add(kvp.Key, new List<int>(kvp.Value));
                                    int tmp = combinations;
                                    combinations *= kvp.Value.Count;
                                    funcGroups.Add(kvp.Key, 0);
                                    divisors.Add(kvp.Key, new int[]{combinations, tmp});
                                }
                                for (int i = 0; i < combinations; ++i)
                                {
                                    foreach (KeyValuePair<FunctionalGroupType, HashSet<int>> kvp in functionalGroupCounts)
                                    {
                                        int pos = (i % divisors[kvp.Key][0]) / divisors[kvp.Key][1];
                                        funcGroups[kvp.Key] = funcGroupsPossibilities[kvp.Key][pos];
                                    }
                                    yield return new FattyAcid(fattyAcidLength, fattyAcidDoubleBond, 0, fattyAcidKeyValuePair.Key, isLCB, funcGroups);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                yield return new FattyAcid(0, 0, 0, FattyAcidType.NoType);
            }
        }
    }
}
