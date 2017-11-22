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
/*
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.IO;
using Ionic.Zlib;

using SkylineTool;
*/
namespace LipidCreator
{
    [Serializable]
    public class PLLipid : Lipid
    {
        public FattyAcidGroup fag1;
        public FattyAcidGroup fag2;
        public FattyAcidGroup fag3;
        public FattyAcidGroup fag4;
        public bool isCL;
    
        public PLLipid(Dictionary<String, String> allPaths, Dictionary<String, Dictionary<String, ArrayList>> allFragments)
        {
            fag1 = new FattyAcidGroup();
            fag2 = new FattyAcidGroup();
            fag3 = new FattyAcidGroup();
            fag4 = new FattyAcidGroup();
            isCL = false;
            
            foreach (KeyValuePair<String, ArrayList> PLFragments in allFragments["PL"])
            {
                if (allPaths.ContainsKey(PLFragments.Key)) pathsToFullImage.Add(PLFragments.Key, allPaths[PLFragments.Key]);
                MS2Fragments.Add(PLFragments.Key, new ArrayList());
                foreach (MS2Fragment fragment in PLFragments.Value)
                {
                    MS2Fragments[PLFragments.Key].Add(new MS2Fragment(fragment));
                }
            }
            headGroupNames.Sort();
        }
    
        public PLLipid(PLLipid copy) : base((Lipid)copy)
        {
            headGroupNames = new List<String>();
            fag1 = new FattyAcidGroup(copy.fag1);
            fag2 = new FattyAcidGroup(copy.fag2);
            fag3 = new FattyAcidGroup(copy.fag3);
            fag4 = new FattyAcidGroup(copy.fag4);
            isCL = copy.isCL;
            foreach (string headgroup in copy.headGroupNames)
            {
                headGroupNames.Add(headgroup);
            }
        }
        
        public override string serialize()
        {
            string xml = "<lipid type=\"PL\" isCL=\"" + isCL + "\">\n";
            xml += fag1.serialize();
            xml += fag2.serialize();
            xml += fag3.serialize();
            xml += fag4.serialize();
            foreach (string headgroup in headGroupNames)
            {
                xml += "<headGroup>" + headgroup + "</headGroup>\n";
            }
            xml += base.serialize();
            xml += "</lipid>\n";
            return xml;
        }
        
        public override void import(XElement node)
        {
            int fattyAcidCounter = 0;
            headGroupNames.Clear();
            isCL = node.Attribute("type").Value == "True";
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
                        else if (fattyAcidCounter == 3)
                        {
                            fag4.import(child);
                        }
                        else
                        {   
                            Console.WriteLine("Error, fatty acid");
                            throw new Exception();
                        }
                        ++fattyAcidCounter;
                        break;
                        
                    case "headGroup":
                        headGroupNames.Add(child.Value.ToString());
                        break;
                        
                        
                    default:
                        base.import(child);
                        break;
                }
            }
        }
        
        
        public override void computePrecursorData(Dictionary<String, DataTable> headGroupsTable, Dictionary<String, Dictionary<String, bool>> headgroupAdductRestrictions, HashSet<String> usedKeys, ArrayList precursorDataList)
        {
            if (isCL)
            {
                // check if more than one fatty acids are 0:0
                int checkFattyAcids = 0;
                checkFattyAcids += fag1.faTypes["FAx"] ? 1 : 0;
                checkFattyAcids += fag2.faTypes["FAx"] ? 1 : 0;
                checkFattyAcids += fag3.faTypes["FAx"] ? 1 : 0;
                checkFattyAcids += fag4.faTypes["FAx"] ? 1 : 0;
                if (checkFattyAcids > 1) return;
                
                
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
                                                                            foreach (int fattyAcidLength4 in fag4.carbonCounts)
                                                                            {
                                                                                int maxDoubleBond4 = (fattyAcidLength4 - 1) >> 1;
                                                                                foreach (int fattyAcidDoubleBond4 in fag4.doubleBondCounts)
                                                                                {
                                                                                    foreach (int fattyAcidHydroxyl4 in fag4.hydroxylCounts)
                                                                                    {
                                                                                        foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair4 in fag4.faTypes)
                                                                                        {
                                                                                            if (fattyAcidKeyValuePair4.Value && maxDoubleBond4 >= fattyAcidDoubleBond4 && fattyAcidLength4 >= fattyAcidHydroxyl4)
                                                                                            {
                                                                                                FattyAcid fa4 = new FattyAcid(fattyAcidLength4, fattyAcidDoubleBond4, fattyAcidHydroxyl4, fattyAcidKeyValuePair4.Key);
                                                                                                containsMonoLyso &= ~8;
                                                                                                if (fattyAcidKeyValuePair4.Key == "FAx")
                                                                                                {
                                                                                                    fa4 = new FattyAcid(0, 0, 0, "FA");
                                                                                                    containsMonoLyso |= 8;
                                                                                                }
                                                                                                
                                                                                                
                                                                                                List<FattyAcid> sortedAcids = new List<FattyAcid>();
                                                                                                sortedAcids.Add(fa1);
                                                                                                sortedAcids.Add(fa2);
                                                                                                sortedAcids.Add(fa3);
                                                                                                sortedAcids.Add(fa4);
                                                                                                sortedAcids.Sort();
                                                                                                String headgroup = (containsMonoLyso == 0) ? "CL" : "MLCL";
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
                                                                                                            MS2Fragment.addCounts(atomsCount, fa4.atomsCount);
                                                                                                            MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                                                                            String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                                                            int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                                                                            String chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                                                            double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                                                                                                            
                                            
                                                                                                            PrecursorData precursorData = new PrecursorData();
                                                                                                            precursorData.lipidCategory = LipidCategory.PhosphoLipid;
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
                                                                                                            precursorData.fa4 = fa4;
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
                }
            }
            else
            {

                if (headGroupNames.Count == 0) return;
                bool isPlamalogen = false;
                bool isFAe = false;
                bool isLyso = false;
                
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
                                    switch (fattyAcidKeyValuePair1.Key)
                                    {
                                        case "FAx": fa1 = new FattyAcid(0, 0, 0, "FA"); isLyso = true; break;
                                        case "FAe": isFAe = true; break;
                                        case "FAp": isPlamalogen = true; break;
                                        default: break;
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
                                                        switch (fattyAcidKeyValuePair2.Key)
                                                        {
                                                            case "FAx": fa2 = new FattyAcid(0, 0, 0, "FA"); isLyso = true; break;
                                                            case "FAe": isFAe = true; break;
                                                            case "FAp": isPlamalogen = true; break;
                                                            default: break;
                                                        }            
                                                        List<FattyAcid> sortedAcids = new List<FattyAcid>();
                                                        sortedAcids.Add(fa1);
                                                        sortedAcids.Add(fa2);
                                                        sortedAcids.Sort();
                                                        
                                                        
                                                        foreach(string headgroupIter in headGroupNames)
                                                        {   
                                                            string headgroup = headgroupIter;
                                                            if (headgroup.Equals("PC") || headgroup.Equals("PE"))
                                                            {
                                                                if (isLyso) headgroup = "L" + headgroup;
                                                                if (isPlamalogen) headgroup = "p" + headgroup;
                                                                else if (isFAe) headgroup = "e" + headgroup;
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
                                                                        MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                                        String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                        int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                                        String chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                        double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                                                                                                            
                                            
                                                                        PrecursorData precursorData = new PrecursorData();
                                                                        precursorData.lipidCategory = LipidCategory.PhosphoLipid;
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