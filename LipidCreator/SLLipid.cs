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
    public class SLLipid : Lipid
    {
        public FattyAcidGroup fag;
        public FattyAcidGroup lcb;
        public int longChainBaseHydroxyl;
        public int fattyAcidHydroxyl;
    
        public SLLipid(Dictionary<String, String> allPaths, Dictionary<String, Dictionary<String, ArrayList>> allFragments)
        {
            lcb = new FattyAcidGroup();
            fag = new FattyAcidGroup();
            longChainBaseHydroxyl = 2;
            fattyAcidHydroxyl = 0;
            
            
            
            foreach (KeyValuePair<String, ArrayList> PLFragments in allFragments["SL"])
            {
                if (allPaths.ContainsKey(PLFragments.Key)) pathsToFullImage.Add(PLFragments.Key, allPaths[PLFragments.Key]);
                MS2Fragments.Add(PLFragments.Key, new ArrayList());
                foreach (MS2Fragment fragment in PLFragments.Value)
                {
                    MS2Fragments[PLFragments.Key].Add(new MS2Fragment(fragment));
                }
            }
            adducts["+H"] = true;
            adducts["-H"] = false;
        }
    
        public SLLipid(SLLipid copy) : base((Lipid)copy)
        {
            headGroupNames = new List<String>();
            lcb = new FattyAcidGroup(copy.lcb);
            fag = new FattyAcidGroup(copy.fag);
            longChainBaseHydroxyl = copy.longChainBaseHydroxyl;
            fattyAcidHydroxyl = copy.fattyAcidHydroxyl;
            foreach (string headgroup in copy.headGroupNames)
            {
                headGroupNames.Add(headgroup);
            }
        }
        
        
        public override string serialize()
        {
            string xml = "<lipid type=\"SL\">\n";
            xml += lcb.serialize();
            xml += fag.serialize();
            xml += "<lcbHydroxyValue>" + longChainBaseHydroxyl + "</lcbHydroxyValue>\n";
            xml += "<faHydroxyValue>" + fattyAcidHydroxyl + "</faHydroxyValue>\n";
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
            foreach (XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "FattyAcidGroup":
                        if (fattyAcidCounter == 0)
                        {
                            lcb.import(child);
                        }
                        else if (fattyAcidCounter == 1)
                        {
                            fag.import(child);
                        }
                        else
                        {   
                            Console.WriteLine("Error, fatty acid");
                            throw new Exception();
                        }
                        ++fattyAcidCounter;
                        break;
                        
                    case "lcbHydroxyValue":
                        longChainBaseHydroxyl = Convert.ToInt32(child.Value.ToString());
                        break;
                        
                    case "faHydroxyValue":
                        fattyAcidHydroxyl = Convert.ToInt32(child.Value.ToString());
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
            foreach (int longChainBaseLength in lcb.carbonCounts)
            {
                int maxDoubleBond1 = (longChainBaseLength - 1) >> 1;
                foreach (int longChainBaseDoubleBond in lcb.doubleBondCounts)
                {
                    if (maxDoubleBond1 < longChainBaseDoubleBond && longChainBaseLength >= longChainBaseHydroxyl) continue;
                    FattyAcid lcbType = new FattyAcid(longChainBaseLength, longChainBaseDoubleBond, longChainBaseHydroxyl, true);
                    foreach (string headgroup in headGroupNames)
                    {
                        if (headgroup != "SPH" && headgroup != "SPH-P" && headgroup != "SPC" && headgroup != "HexSph") // sphingolipids without fatty acid
                        {
                            foreach (int fattyAcidLength in fag.carbonCounts)
                            {
                                if (fattyAcidLength < fattyAcidHydroxyl + 2) continue;
                                int maxDoubleBond2 = (fattyAcidLength - 1) >> 1;
                                foreach (int fattyAcidDoubleBond2 in fag.doubleBondCounts)
                                {
                                    if (maxDoubleBond2 < fattyAcidDoubleBond2 && fattyAcidLength >= fattyAcidHydroxyl) continue;
                                    FattyAcid fa = new FattyAcid(fattyAcidLength, fattyAcidDoubleBond2, fattyAcidHydroxyl, "FA");
                        
                        
                                    String key = headgroup + " ";
                                    
                                    key += Convert.ToString(longChainBaseLength) + ":" + Convert.ToString(longChainBaseDoubleBond) + ";" + Convert.ToString(longChainBaseHydroxyl);
                                    key += "/";
                                    key += Convert.ToString(fattyAcidLength) + ":" + Convert.ToString(fattyAcidDoubleBond2);
                                    if (fattyAcidHydroxyl > 0) key += ";" + Convert.ToString(fattyAcidHydroxyl);

                                    if (!usedKeys.Contains(key))
                                    {
                                        foreach (KeyValuePair<string, bool> adduct in adducts)
                                        {
                                            if (adduct.Value && headgroupAdductRestrictions[headgroup][adduct.Key])
                                            {
                                                usedKeys.Add(key);
                                                
                                                DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                MS2Fragment.addCounts(atomsCount, fa.atomsCount);
                                                MS2Fragment.addCounts(atomsCount, lcbType.atomsCount);
                                                // do not change the order, chem formula must be computed before adding the adduct
                                                string chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                string chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                                            
                                                PrecursorData precursorData = new PrecursorData();
                                                precursorData.lipidCategory = LipidCategory.SphingoLipid;
                                                precursorData.moleculeListName = headgroup;
                                                precursorData.precursorName = key;
                                                precursorData.precursorIonFormula = chemForm;
                                                precursorData.precursorAdduct = "[M" + adduct.Key + "]";
                                                precursorData.precursorM_Z = mass / (double)(Math.Abs(charge));
                                                precursorData.precursorCharge = charge;
                                                precursorData.adduct = adduct.Key;
                                                precursorData.atomsCount = atomsCount;
                                                precursorData.fa1 = fa;
                                                precursorData.fa2 = null;
                                                precursorData.fa3 = null;
                                                precursorData.fa4 = null;
                                                precursorData.lcb = lcbType;
                                                precursorData.chemFormComplete = chemFormComplete;
                                                precursorData.MS2Fragments = MS2Fragments[headgroup];
                                                
                                                precursorDataList.Add(precursorData);
                                            
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            String key = headgroup + " " + Convert.ToString(longChainBaseLength) + ":" + Convert.ToString(longChainBaseDoubleBond) + ";" + Convert.ToString(longChainBaseHydroxyl);

                            if (!usedKeys.Contains(key))
                            {
                                foreach (KeyValuePair<string, bool> adduct in adducts)
                                {
                                    if (adduct.Value && headgroupAdductRestrictions[headgroup][adduct.Key])
                                    {
                                        usedKeys.Add(key);
                                        
                                        DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                        MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                        MS2Fragment.addCounts(atomsCount, lcbType.atomsCount);
                                        // do not change the order, chem formula must be computed before adding the adduct
                                        String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                        int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                        String chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                        double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                                                
                                            
                                        PrecursorData precursorData = new PrecursorData();
                                        precursorData.lipidCategory = LipidCategory.SphingoLipid;
                                        precursorData.moleculeListName = headgroup;
                                        precursorData.precursorName = key;
                                        precursorData.precursorIonFormula = chemForm;
                                        precursorData.precursorAdduct = "[M" + adduct.Key + "]";
                                        precursorData.precursorM_Z = mass / (double)(Math.Abs(charge));
                                        precursorData.precursorCharge = charge;
                                        precursorData.adduct = adduct.Key;
                                        precursorData.atomsCount = atomsCount;
                                        precursorData.fa1 = null;
                                        precursorData.fa2 = null;
                                        precursorData.fa3 = null;
                                        precursorData.fa4 = null;
                                        precursorData.lcb = lcbType;
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