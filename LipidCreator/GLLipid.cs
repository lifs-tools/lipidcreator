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
using System.Data.SQLite;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LipidCreator
{
    [Serializable]
    public class GLLipid : Lipid
    {
        public FattyAcidGroup fag1;
        public FattyAcidGroup fag2;
        public FattyAcidGroup fag3;
        public bool containsSugar;
        public List<int> hgValues;
        public List<String> headGroupNames = new List<String>{"MGDG", "DGDG", "SQDG"};
    
    
        public GLLipid(Dictionary<String, String> allPaths, Dictionary<String, ArrayList> allFragments)
        {
            fag1 = new FattyAcidGroup();
            fag2 = new FattyAcidGroup();
            fag3 = new FattyAcidGroup();
            containsSugar = false;
            hgValues = new List<int>();
            MS2Fragments.Add("MG", new ArrayList());
            MS2Fragments.Add("DG", new ArrayList());
            MS2Fragments.Add("MGDG", new ArrayList());
            MS2Fragments.Add("DGDG", new ArrayList());
            MS2Fragments.Add("SQDG", new ArrayList());
            MS2Fragments.Add("TG", new ArrayList());
            adducts["+NH4"] = true;
            adducts["-H"] = false;
            
            foreach(KeyValuePair<String, ArrayList> kvp in MS2Fragments)
            {
                if (allPaths.ContainsKey(kvp.Key)) pathsToFullImage.Add(kvp.Key, allPaths[kvp.Key]);
                if (allFragments != null && allFragments.ContainsKey(kvp.Key))
                {
                    foreach (MS2Fragment fragment in allFragments[kvp.Key])
                    {
                        MS2Fragments[kvp.Key].Add(new MS2Fragment(fragment));
                    }
                }
            }
        }
    
        public GLLipid(GLLipid copy) : base((Lipid)copy) 
        {
            fag1 = new FattyAcidGroup(copy.fag1);
            fag2 = new FattyAcidGroup(copy.fag2);
            fag3 = new FattyAcidGroup(copy.fag3);
            containsSugar = copy.containsSugar;
            hgValues = new List<int>();
            foreach (int hgValue in copy.hgValues)
            {
                hgValues.Add(hgValue);
            }
            
        }
        
        
        public override string serialize()
        {
            string xml = "<lipid type=\"GL\">\n";
            xml += fag1.serialize();
            xml += fag2.serialize();
            xml += fag3.serialize();
            xml += "<containsSugar>" + (containsSugar ? 1 : 0) + "</containsSugar>\n";
            foreach (int hgValue in hgValues)
            {
                xml += "<headGroup>" + hgValue + "</headGroup>\n";
            }
            xml += base.serialize();
            xml += "</lipid>\n";
            return xml;
        }
        
        
        public override void import(XElement node)
        {
            int fattyAcidCounter = 0;
            hgValues.Clear();
            foreach (XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "FattyAcidGroup":
                        if (fattyAcidCounter == 0)
                        {
                            fag1.import(child);
                        }
                        else if (fattyAcidCounter == 1)
                        {
                            fag2.import(child);
                        }
                        else if (fattyAcidCounter == 2)
                        {
                            fag3.import(child);
                        }
                        else
                        {   
                            Console.WriteLine("Error, fatty acid");
                            throw new Exception();
                        }
                        ++fattyAcidCounter;
                        break;
                        
                    case "headGroup":
                        hgValues.Add(Convert.ToInt32(child.Value.ToString()));
                        break;
                        
                    case "containsSugar":
                        containsSugar = child.Value == "1";
                        break;
                        
                        
                    default:
                        base.import(child);
                        break;
                }
            }
        }
        
        
        public override void computePrecursorData(Dictionary<String, DataTable> headGroupsTable, Dictionary<String, Dictionary<String, bool>> headgroupAdductRestrictions, HashSet<String> usedKeys, ArrayList precursorDataList)
        {
            // check if more than one fatty acids are 0:0
            int checkFattyAcids = 0;
            checkFattyAcids += fag1.faTypes["FAx"] ? 1 : 0;
            checkFattyAcids += fag2.faTypes["FAx"] ? 1 : 0;
            checkFattyAcids += fag3.faTypes["FAx"] ? 1 : 0;
            if (checkFattyAcids > 2) return;
            
            int containsMonoLyso = 0;
            foreach (int fattyAcidLength1 in fag1.carbonCounts)
            {
                int maxDoubleBond1 = (fattyAcidLength1 - 1) >> 1;
                foreach (int fattyAcidDoubleBond1 in fag1.doubleBondCounts)
                {
                    foreach (int fattyAcidHydroxyl1 in fag1.hydroxylCounts)
                    {
                        foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair1 in fag1.faTypes)
                        {
                            if (fattyAcidKeyValuePair1.Value && maxDoubleBond1 >= fattyAcidDoubleBond1 && fattyAcidLength1 >= fattyAcidHydroxyl1)
                            {
                                FattyAcid fa1 = new FattyAcid(fattyAcidLength1, fattyAcidDoubleBond1, fattyAcidHydroxyl1, fattyAcidKeyValuePair1.Key);
                                containsMonoLyso &= ~1;
                                if (fattyAcidKeyValuePair1.Key == "FAx")
                                {
                                    fa1 = new FattyAcid(0, 0, 0, "FA");
                                    containsMonoLyso |= 1;
                                }
                                foreach (int fattyAcidLength2 in fag2.carbonCounts)
                                {
                                    int maxDoubleBond2 = (fattyAcidLength2 - 1) >> 1;
                                    foreach (int fattyAcidDoubleBond2 in fag2.doubleBondCounts)
                                    {
                                        foreach (int fattyAcidHydroxyl2 in fag2.hydroxylCounts)
                                        {
                                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair2 in fag2.faTypes)
                                            {
                                                if (fattyAcidKeyValuePair2.Value && maxDoubleBond2 >= fattyAcidDoubleBond2 && fattyAcidLength2 >= fattyAcidHydroxyl2)
                                                {
                                                    FattyAcid fa2 = new FattyAcid(fattyAcidLength2, fattyAcidDoubleBond2, fattyAcidHydroxyl2, fattyAcidKeyValuePair2.Key);
                                                    containsMonoLyso &= ~2;
                                                    if (fattyAcidKeyValuePair2.Key == "FAx")
                                                    {
                                                        fa2 = new FattyAcid(0, 0, 0, "FA");
                                                        containsMonoLyso |= 2;
                                                    }
                                                    if (containsSugar)
                                                    {
                                                        List<FattyAcid> sortedAcids = new List<FattyAcid>();
                                                        sortedAcids.Add(fa1);
                                                        sortedAcids.Add(fa2);
                                                        sortedAcids.Sort();
                                                        
                                                        
                                                        foreach (int hgValue in hgValues)
                                                        {
                                                            String headgroup = headGroupNames[hgValue];
                                                            String key = headgroup + " ";
                                                            int i = 0;
                                                            foreach (FattyAcid fa in sortedAcids)
                                                            {
                                                                if (fa.length > 0){
                                                                    if (i++ > 0) key += "_";
                                                                    key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                                                    if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                                                    key += fa.suffix;
                                                                }
                                                            }
                                                            
                                                            if (!usedKeys.Contains(key))
                                                            {
                                                                foreach (KeyValuePair<string, bool> adduct in adducts)
                                                                {
                                                                    if (adduct.Value && headgroupAdductRestrictions[headgroup][adduct.Key])
                                                                    {
                                                                        usedKeys.Add(key);
                                                                        
                                                                        DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                                        MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                                                        MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                                                        MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                                        String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                        int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                                        String chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                        double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                                                                                                                                                            
                                                                        
                                                                        PrecursorData precursorData = new PrecursorData();
                                                                        precursorData.lipidCategory = LipidCategory.GlyceroLipid;
                                                                        precursorData.moleculeListName = headgroup;
                                                                        precursorData.precursorName = key;
                                                                        precursorData.precursorIonFormula = chemForm;
                                                                        precursorData.precursorAdduct = "[M" + adduct.Key + "]";
                                                                        precursorData.precursorM_Z = mass / (double)(Math.Abs(charge));
                                                                        precursorData.precursorCharge = charge;
                                                                        precursorData.adduct = adduct.Key;
                                                                        precursorData.atomsCount = atomsCount;
                                                                        precursorData.fa1 = fa1;
                                                                        precursorData.fa2 = fa2;
                                                                        precursorData.fa3 = null;
                                                                        precursorData.fa4 = null;
                                                                        precursorData.lcb = null;
                                                                        precursorData.chemFormComplete = chemFormComplete;
                                                                        precursorData.MS2Fragments = MS2Fragments[headgroup];
                                                                        
                                                                        precursorDataList.Add(precursorData);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        foreach (int fattyAcidLength3 in fag3.carbonCounts)
                                                        {
                                                            int maxDoubleBond3 = (fattyAcidLength3 - 1) >> 1;
                                                            foreach (int fattyAcidDoubleBond3 in fag3.doubleBondCounts)
                                                            {
                                                                foreach (int fattyAcidHydroxyl3 in fag3.hydroxylCounts)
                                                                {
                                                                    foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair3 in fag3.faTypes)
                                                                    {
                                                                        if (fattyAcidKeyValuePair3.Value && maxDoubleBond3 >= fattyAcidDoubleBond3 && fattyAcidLength3 >= fattyAcidHydroxyl3)
                                                                        {
                                                                            FattyAcid fa3 = new FattyAcid(fattyAcidLength3, fattyAcidDoubleBond3, fattyAcidHydroxyl3, fattyAcidKeyValuePair3.Key);
                                                                            containsMonoLyso &= ~4;
                                                                            if (fattyAcidKeyValuePair3.Key == "FAx")
                                                                            {
                                                                                fa3 = new FattyAcid(0, 0, 0, "FA");
                                                                                containsMonoLyso |= 4;
                                                                            }
                                                                                    
                                                                                            
                                                                            List<FattyAcid> sortedAcids = new List<FattyAcid>();
                                                                            sortedAcids.Add(fa1);
                                                                            sortedAcids.Add(fa2);
                                                                            sortedAcids.Add(fa3);
                                                                            sortedAcids.Sort();
                                                                            
                                                                            // popcount
                                                                            int pcContainsMonoLyso = containsMonoLyso - ((containsMonoLyso >> 1) & 0x55555555);
                                                                            pcContainsMonoLyso = (pcContainsMonoLyso & 0x33333333) + ((pcContainsMonoLyso >> 2) & 0x33333333);
                                                                            pcContainsMonoLyso = ((pcContainsMonoLyso + (pcContainsMonoLyso >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
                                                                            
                                                                            String headgroup = "";
                                                                            switch(pcContainsMonoLyso)
                                                                            {
                                                                                case 0:
                                                                                headgroup = "TG";
                                                                                    break;
                                                                                case 1:
                                                                                headgroup = "DG";
                                                                                    break;
                                                                                case 2:
                                                                                headgroup = "MG";
                                                                                    break;
                                                                            }
                                                                            String key = headgroup + " ";
                                                                            int i = 0;
                                                                            foreach (FattyAcid fa in sortedAcids)
                                                                            {
                                                                                if (fa.length > 0){
                                                                                    if (i++ > 0) key += "_";
                                                                                    key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                                                                    if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                                                                    key += fa.suffix;
                                                                                }
                                                                            }
                                                                            
                                                                            if (!usedKeys.Contains(key))
                                                                            {
                                                                                foreach (KeyValuePair<string, bool> adduct in adducts)
                                                                                {
                                                                                    if (adduct.Value && headgroupAdductRestrictions[headgroup][adduct.Key])
                                                                                    {
                                                                                        usedKeys.Add(key);
                                                                                        
                                                                                        DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                                                        MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                                                                        MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                                                                        MS2Fragment.addCounts(atomsCount, fa3.atomsCount);
                                                                                        MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                                                        String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                                        int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                                                        String chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                                        double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                                                                                                                                                                            
                                                                                        
                                                                                        PrecursorData precursorData = new PrecursorData();
                                                                                        precursorData.lipidCategory = LipidCategory.GlyceroLipid;
                                                                                        precursorData.moleculeListName = headgroup;
                                                                                        precursorData.precursorName = key;
                                                                                        precursorData.precursorIonFormula = chemForm;
                                                                                        precursorData.precursorAdduct = "[M" + adduct.Key + "]";
                                                                                        precursorData.precursorM_Z = mass / (double)(Math.Abs(charge));
                                                                                        precursorData.precursorCharge = charge;
                                                                                        precursorData.adduct = adduct.Key;
                                                                                        precursorData.atomsCount = atomsCount;
                                                                                        precursorData.fa1 = fa1;
                                                                                        precursorData.fa2 = fa2;
                                                                                        precursorData.fa3 = fa3;
                                                                                        precursorData.fa4 = null;
                                                                                        precursorData.lcb = null;
                                                                                        precursorData.chemFormComplete = chemFormComplete;
                                                                                        precursorData.MS2Fragments = MS2Fragments[headgroup];
                                                                                        
                                                                                        precursorDataList.Add(precursorData);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}