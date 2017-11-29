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
    
    
        public GLLipid(Dictionary<String, String> allPaths, Dictionary<String, Dictionary<String, ArrayList>> allFragments)
        {
            fag1 = new FattyAcidGroup();
            fag2 = new FattyAcidGroup();
            fag3 = new FattyAcidGroup();
            containsSugar = false;
            
            if (allFragments.ContainsKey("GL"))
            {
                foreach (KeyValuePair<String, ArrayList> PLFragments in allFragments["GL"])
                {
                    if (allPaths.ContainsKey(PLFragments.Key)) pathsToFullImage.Add(PLFragments.Key, allPaths[PLFragments.Key]);
                    MS2Fragments.Add(PLFragments.Key, new ArrayList());
                    foreach (MS2Fragment fragment in PLFragments.Value)
                    {
                        MS2Fragments[PLFragments.Key].Add(new MS2Fragment(fragment));
                    }
                }
            }
            adducts["+NH4"] = true;
            adducts["-H"] = false;
        }
    
        public GLLipid(GLLipid copy) : base((Lipid)copy) 
        {
            fag1 = new FattyAcidGroup(copy.fag1);
            fag2 = new FattyAcidGroup(copy.fag2);
            fag3 = new FattyAcidGroup(copy.fag3);
            containsSugar = copy.containsSugar;
        }
        
        
        public override string serialize()
        {
            string xml = "<lipid type=\"GL\">\n";
            xml += fag1.serialize();
            xml += fag2.serialize();
            xml += fag3.serialize();
            xml += "<containsSugar>" + (containsSugar ? 1 : 0) + "</containsSugar>\n";
            foreach (string headgroup in headGroupNames)
            {
                xml += "<headGroup>" + headgroup + "</headGroup>\n";
            }
            xml += base.serialize();
            xml += "</lipid>\n";
            return xml;
        }
        
        
        public override void import(XElement node, string importVersion)
        {
            int fattyAcidCounter = 0;
            headGroupNames.Clear();
            foreach (XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "FattyAcidGroup":
                        if (fattyAcidCounter == 0)
                        {
                            fag1.import(child, importVersion);
                        }
                        else if (fattyAcidCounter == 1)
                        {
                            fag2.import(child, importVersion);
                        }
                        else if (fattyAcidCounter == 2)
                        {
                            fag3.import(child, importVersion);
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
                        
                    case "containsSugar":
                        containsSugar = child.Value == "1";
                        break;
                        
                        
                    default:
                        base.import(child, importVersion);
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
            
            // calling all possible fatty acid 1 combinations
            foreach (FattyAcid fa1 in fag1.getFattyAcids())
            {
                containsMonoLyso &= ~1;
                if (fa1.suffix == "x") containsMonoLyso |= 1;
                    
                // calling all possible fatty acid 2 combinations
                foreach (FattyAcid fa2 in fag2.getFattyAcids())
                {
                    containsMonoLyso &= ~2;
                    if (fa2.suffix == "x") containsMonoLyso |= 2;
                    if (containsSugar)
                    {
                        List<FattyAcid> sortedAcids = new List<FattyAcid>();
                        sortedAcids.Add(fa1);
                        sortedAcids.Add(fa2);
                        sortedAcids.Sort();
                        
                        
                        foreach (string headgroup in headGroupNames)
                        {
                            String key = headgroup + " ";
                            int i = 0;
                            foreach (FattyAcid fa in sortedAcids)
                            {
                                if (fa.length > 0 && fa.suffix != "x")
                                {
                                    if (i++ > 0) key += Lipid.IdSeparatorUnspecific;
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
                                        String chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                                        int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                        double mass = LipidCreator.computeMass(atomsCount, charge);
                                                                                                                            
                                        
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
                                        precursorData.fa1 = sortedAcids[0];
                                        precursorData.fa2 = sortedAcids[1];
                                        precursorData.fa3 = null;
                                        precursorData.fa4 = null;
                                        precursorData.lcb = null;
                                        precursorData.MS2Fragments = MS2Fragments[headgroup];
                                        
                                        precursorDataList.Add(precursorData);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // calling all possible fatty acid 3 combinations
                        foreach (FattyAcid fa3 in fag3.getFattyAcids())
                        {
                            containsMonoLyso &= ~4;
                            if (fa3.suffix == "x") containsMonoLyso |= 4;
                                                            
                            List<FattyAcid> sortedAcids = new List<FattyAcid>();
                            sortedAcids.Add(fa1);
                            sortedAcids.Add(fa2);
                            sortedAcids.Add(fa3);
                            sortedAcids.Sort();
                            
                            String headgroup = "";
                            switch(containsMonoLyso)
                            {
                                case 0:
                                    headgroup = "TG";
                                    break;
                                case 4:
                                case 2:
                                case 1:
                                    headgroup = "DG";
                                    break;
                                case 6:
                                case 5:
                                case 3:
                                    headgroup = "MG";
                                    break;
                            }
                            String key = headgroup + " ";
                            int i = 0;
                            foreach (FattyAcid fa in sortedAcids)
                            {
                                if (fa.length > 0 && fa.suffix != "x"){
                                    if (i++ > 0) key += Lipid.IdSeparatorUnspecific;
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
                                        String chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                                        int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                        double mass = LipidCreator.computeMass(atomsCount, charge);
                                                                                                                            
                                        
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
                                        precursorData.fa1 = sortedAcids[0];
                                        precursorData.fa2 = sortedAcids[1];
                                        precursorData.fa3 = sortedAcids[2];
                                        precursorData.fa4 = null;
                                        precursorData.lcb = null;
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