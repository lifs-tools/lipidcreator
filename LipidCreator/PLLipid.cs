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
        public List<int> hgValues;
        
        public List<String> headGroupNames = new List<String>{"CDP-DAG", "PA", "PC", "PE", "PEt", "DMPE", "MMPE", "PG", "PI", "PIP", "PIP2", "PIP3", "PS", "LPA", "LPC", "LPE", "LPG", "LPI", "LPS"};
    
        public PLLipid(Dictionary<String, String> allPaths, Dictionary<String, ArrayList> allFragments)
        {
            fag1 = new FattyAcidGroup();
            fag2 = new FattyAcidGroup();
            fag3 = new FattyAcidGroup();
            fag4 = new FattyAcidGroup();
            hgValues = new List<int>();
            isCL = false;
            MS2Fragments.Add("CDP-DAG", new ArrayList());
            MS2Fragments.Add("CL", new ArrayList());
            MS2Fragments.Add("MLCL", new ArrayList());
            MS2Fragments.Add("PA", new ArrayList());
            MS2Fragments.Add("PC", new ArrayList());
            MS2Fragments.Add("pPC", new ArrayList());
            MS2Fragments.Add("ppPC", new ArrayList());
            MS2Fragments.Add("PE", new ArrayList());
            MS2Fragments.Add("PEt", new ArrayList());
            MS2Fragments.Add("pPE", new ArrayList());
            MS2Fragments.Add("ppPE", new ArrayList());
            MS2Fragments.Add("DMPE", new ArrayList());
            MS2Fragments.Add("MMPE", new ArrayList());
            MS2Fragments.Add("PG", new ArrayList());
            MS2Fragments.Add("PI", new ArrayList());
            MS2Fragments.Add("PIP", new ArrayList());
            MS2Fragments.Add("PIP2", new ArrayList());
            MS2Fragments.Add("PIP3", new ArrayList());
            MS2Fragments.Add("PS", new ArrayList());
            MS2Fragments.Add("LPA", new ArrayList());
            MS2Fragments.Add("LPC", new ArrayList());
            MS2Fragments.Add("LPE", new ArrayList());
            MS2Fragments.Add("LPG", new ArrayList());
            MS2Fragments.Add("LPI", new ArrayList());
            MS2Fragments.Add("LPS", new ArrayList());
            
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
    
        public PLLipid(PLLipid copy) : base((Lipid)copy)
        {
            fag1 = new FattyAcidGroup(copy.fag1);
            fag2 = new FattyAcidGroup(copy.fag2);
            fag3 = new FattyAcidGroup(copy.fag3);
            fag4 = new FattyAcidGroup(copy.fag4);
            hgValues = new List<int>();
            isCL = copy.isCL;
            foreach (int hgValue in copy.hgValues)
            {
                hgValues.Add(hgValue);
            }
        }
        
        public override string serialize()
        {
            string xml = "<lipid type=\"PL\" isCL=\"" + isCL + "\">\n";
            xml += fag1.serialize();
            xml += fag2.serialize();
            xml += fag3.serialize();
            xml += fag4.serialize();
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
                        hgValues.Add(Convert.ToInt32(child.Value.ToString()));
                        break;
                        
                        
                    default:
                        base.import(child);
                        break;
                }
            }
        }
        
        
        public override void addLipids(DataTable allLipids, DataTable allLipidsUnique, Dictionary<String, DataTable> headGroupsTable, Dictionary<String, Dictionary<String, bool>> headgroupAdductRestrictions, HashSet<String> usedKeys, HashSet<String> replicates)
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
                                if (fattyAcidKeyValuePair1.Value && maxDoubleBond1 >= fattyAcidDoubleBond1)
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
                                            foreach (int fattyAcidHydroxyl2 in fag1.hydroxylCounts)
                                            {
                                                foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair2 in fag2.faTypes)
                                                {
                                                    if (fattyAcidKeyValuePair2.Value && maxDoubleBond2 >= fattyAcidDoubleBond2)
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
                                                                foreach (int fattyAcidHydroxyl3 in fag1.hydroxylCounts)
                                                                {
                                                                    foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair3 in fag3.faTypes)
                                                                    {
                                                                        if (fattyAcidKeyValuePair3.Value && maxDoubleBond3 >= fattyAcidDoubleBond3)
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
                                                                                    foreach (int fattyAcidHydroxyl4 in fag1.hydroxylCounts)
                                                                                    {
                                                                                        foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair4 in fag4.faTypes)
                                                                                        {
                                                                                            if (fattyAcidKeyValuePair4.Value && maxDoubleBond4 >= fattyAcidDoubleBond4)
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
                                                                                                            
                                                                                                            
                                                                                                            
                                                                                                            
                                                                                                            foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                                                                                            {
                                                                                                                if (fragment.fragmentSelected && ((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)))
                                                                                                                {
                                                                                                                    DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                                                                                    foreach (string fbase in fragment.fragmentBase)
                                                                                                                    {
                                                                                                                        switch(fbase)
                                                                                                                        {
                                                                                                                            case "FA1":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa1.atomsCount);
                                                                                                                                break;
                                                                                                                            case "FA2":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa2.atomsCount);
                                                                                                                                break;
                                                                                                                            case "FA3":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa3.atomsCount);
                                                                                                                                break;
                                                                                                                            case "FA4":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa4.atomsCount);
                                                                                                                                break;
                                                                                                                            case "PRE":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                                                                                break;
                                                                                                                            default:
                                                                                                                                break;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                                                                                    //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                                                                                    double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge);
                                                                                                                    
                                                                                                                
                                                                                                                    DataRow lipidRow = allLipids.NewRow();
                                                                                                                    lipidRow["Molecule List Name"] = headgroup;
                                                                                                                    lipidRow["Precursor Name"] = key;
                                                                                                                    lipidRow["Precursor Ion Formula"] = chemForm;
                                                                                                                    lipidRow["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                                                                                    lipidRow["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                                                                    lipidRow["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                                                                    lipidRow["Product Name"] = fragment.fragmentName;
                                                                                                                    lipidRow["Product Ion Formula"] = chemFormFragment;
                                                                                                                    lipidRow["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                                                                                    lipidRow["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                                                                                    allLipids.Rows.Add(lipidRow);
                                                                                                                    
                                                                                                                    String replicatesKey = chemFormComplete + "/" + chemFormFragment;
                                                                                                                    if (!replicates.Contains(replicatesKey))
                                                                                                                    {
                                                                                                                        replicates.Add(replicatesKey);
                                                                                                                        DataRow lipidRowUnique = allLipidsUnique.NewRow();
                                                                                                                        lipidRowUnique["Molecule List Name"] = headgroup;
                                                                                                                        lipidRowUnique["Precursor Name"] = key;
                                                                                                                        lipidRowUnique["Precursor Ion Formula"] = chemForm;
                                                                                                                        lipidRowUnique["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                                                                                        lipidRowUnique["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                                                                        lipidRowUnique["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                                                                        lipidRowUnique["Product Name"] = fragment.fragmentName;
                                                                                                                        lipidRowUnique["Product Ion Formula"] = chemFormFragment;
                                                                                                                        lipidRowUnique["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                                                                                        lipidRowUnique["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                                                                                        allLipidsUnique.Rows.Add(lipidRowUnique);
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
                    }
                }
            }
            else
            {
                // check if more than one fatty acids are 0:0
                int checkFattyAcids = 0;
                checkFattyAcids += fag1.faTypes["FAx"] ? 1 : 0;
                checkFattyAcids += fag2.faTypes["FAx"] ? 1 : 0;
                if (checkFattyAcids > 0) return;
                if (hgValues.Count == 0) return;
                int isPlamalogen = 0;
                
                foreach (int fattyAcidLength1 in fag1.carbonCounts)
                {
                    int maxDoubleBond1 = (fattyAcidLength1 - 1) >> 1;
                    foreach (int fattyAcidDoubleBond1 in fag1.doubleBondCounts)
                    {
                        foreach (int fattyAcidHydroxyl1 in fag1.hydroxylCounts)
                        {
                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair1 in fag1.faTypes)
                            {
                                if (fattyAcidKeyValuePair1.Value && maxDoubleBond1 >= fattyAcidDoubleBond1)
                                {
                                    FattyAcid fa1 = new FattyAcid(fattyAcidLength1, fattyAcidDoubleBond1, fattyAcidHydroxyl1, fattyAcidKeyValuePair1.Key);
                                    if (fattyAcidKeyValuePair1.Key == "FAx")
                                    {
                                        fa1 = new FattyAcid(0, 0, 0, "FA");
                                    }
                                    if (fattyAcidKeyValuePair1.Key == "FAp")
                                    {
                                        isPlamalogen += 1;
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
                                                    if (fattyAcidKeyValuePair2.Value && maxDoubleBond2 >= fattyAcidDoubleBond2)
                                                    {
                                                        FattyAcid fa2 = new FattyAcid(fattyAcidLength2, fattyAcidDoubleBond2, fattyAcidHydroxyl2, fattyAcidKeyValuePair2.Key);
                                                        if (fattyAcidKeyValuePair2.Key == "FAx")
                                                        {
                                                            fa2 = new FattyAcid(0, 0, 0, "FA");
                                                        }
                                                        if (fattyAcidKeyValuePair2.Key == "FAp")
                                                        {
                                                            isPlamalogen += 1;
                                                        }             
                                                        List<FattyAcid> sortedAcids = new List<FattyAcid>();
                                                        sortedAcids.Add(fa1);
                                                        sortedAcids.Add(fa2);
                                                        sortedAcids.Sort();
                                                        
                                                        foreach(int hgValue in hgValues)
                                                        {
                                                        
                                                            String headgroup = headGroupNames[hgValue];
                                                            String headgroupSearch = headgroup;
                                                            String key = headgroup + " ";
                                                            if (headgroup.Equals("PC") || headgroup.Equals("PE"))
                                                            {
                                                                if (isPlamalogen >= 1) headgroupSearch = "p" + headgroupSearch;
                                                                if (isPlamalogen == 2) headgroupSearch = "p" + headgroupSearch;
                                                            }
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
                                                                    if (adduct.Value && headgroupAdductRestrictions[headgroupSearch][adduct.Key])
                                                                    {
                                                                        usedKeys.Add(key);
                                                                        
                                                                        DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                                        MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                                                        MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                                                        MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroupSearch]);
                                                                        String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                        int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                                        String chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                        double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                                                                        
                                                                        
                                                                        // add precursor as fragment
                                                                        
                                                                        DataRow lipidRowPrec = allLipids.NewRow();
                                                                        lipidRowPrec["Molecule List Name"] = headgroup;
                                                                        lipidRowPrec["Precursor Name"] = key;
                                                                        lipidRowPrec["Precursor Ion Formula"] = chemForm;
                                                                        lipidRowPrec["Precursor Adduct"] = "[M]";
                                                                        lipidRowPrec["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                        lipidRowPrec["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                        lipidRowPrec["Product Name"] = "Precursor";;
                                                                        lipidRowPrec["Product Ion Formula"] = chemForm;
                                                                        lipidRowPrec["Product m/z"] = mass / (double)(Math.Abs(charge));
                                                                        lipidRowPrec["Product Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                        allLipids.Rows.Add(lipidRowPrec);
                                                                                                            
                                                                        String replicatesPrecKey = chemFormComplete + "/" + chemFormComplete;
                                                                        if (!replicates.Contains(replicatesPrecKey))
                                                                        {
                                                                            replicates.Add(replicatesPrecKey);
                                                                            DataRow lipidRowPrecUnique = allLipidsUnique.NewRow();
                                                                            lipidRowPrecUnique["Molecule List Name"] = headgroup;
                                                                            lipidRowPrecUnique["Precursor Name"] = key;
                                                                            lipidRowPrecUnique["Precursor Ion Formula"] = chemForm;
                                                                            lipidRowPrecUnique["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                                            lipidRowPrecUnique["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                            lipidRowPrecUnique["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                            lipidRowPrecUnique["Product Name"] = "Precursor";
                                                                            lipidRowPrecUnique["Product Ion Formula"] = chemForm;
                                                                            lipidRowPrecUnique["Product m/z"] = mass / (double)(Math.Abs(charge));
                                                                            lipidRowPrecUnique["Product Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                            allLipidsUnique.Rows.Add(lipidRowPrecUnique);
                                                                        }
                                                                        
                                                                        
                                                                        
                                                                        foreach (MS2Fragment fragment in MS2Fragments[headgroupSearch])
                                                                        {
                                                                            if (fragment.fragmentSelected && ((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                                                            {
                                                                                DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                                                foreach (string fbase in fragment.fragmentBase)
                                                                                {
                                                                                    switch(fbase)
                                                                                    {
                                                                                        case "FA1":
                                                                                            MS2Fragment.addCounts(atomsCountFragment, fa1.atomsCount);
                                                                                            break;
                                                                                        case "FA2":
                                                                                            MS2Fragment.addCounts(atomsCountFragment, fa2.atomsCount);
                                                                                            break;
                                                                                        case "PRE":
                                                                                            MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                                            break;
                                                                                        default:
                                                                                            break;
                                                                                    }
                                                                                }
                                                                                String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                                                //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                                                double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge);
                                                                                
                                                                            
                                                                                DataRow lipidRow = allLipids.NewRow();
                                                                                lipidRow["Molecule List Name"] = headgroup;
                                                                                lipidRow["Precursor Name"] = key;
                                                                                lipidRow["Precursor Ion Formula"] = chemForm;
                                                                                lipidRow["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                                                lipidRow["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                                lipidRow["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                                lipidRow["Product Name"] = fragment.fragmentName;
                                                                                lipidRow["Product Ion Formula"] = chemFormFragment;
                                                                                lipidRow["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                                                lipidRow["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                                                allLipids.Rows.Add(lipidRow);
                                                                                                                    
                                                                                String replicatesKey = chemFormComplete + "/" + chemFormFragment;
                                                                                if (!replicates.Contains(replicatesKey))
                                                                                {
                                                                                    replicates.Add(replicatesKey);
                                                                                    DataRow lipidRowUnique = allLipidsUnique.NewRow();
                                                                                    lipidRowUnique["Molecule List Name"] = headgroup;
                                                                                    lipidRowUnique["Precursor Name"] = key;
                                                                                    lipidRowUnique["Precursor Ion Formula"] = chemForm;
                                                                                    lipidRowUnique["Precursor Adduct"] = "[M" + adduct.Key + "]";
                                                                                    lipidRowUnique["Precursor m/z"] = mass / (double)(Math.Abs(charge));
                                                                                    lipidRowUnique["Precursor Charge"] = ((charge > 0) ? "+" : "") + Convert.ToString(charge);
                                                                                    lipidRowUnique["Product Name"] = fragment.fragmentName;
                                                                                    lipidRowUnique["Product Ion Formula"] = chemFormFragment;
                                                                                    lipidRowUnique["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                                                                                    lipidRowUnique["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                                                                                    allLipidsUnique.Rows.Add(lipidRowUnique);
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
        
        public override void addSpectrum(SQLiteCommand command, Dictionary<String, DataTable> headGroupsTable, HashSet<String> usedKeys)
        {
            if (isCL){
                // check if more than one fatty acids are 0:0
                int checkFattyAcids = 0;
                string sql;
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
                                if (fattyAcidKeyValuePair1.Value && maxDoubleBond1 >= fattyAcidDoubleBond1)
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
                                            foreach (int fattyAcidHydroxyl2 in fag1.hydroxylCounts)
                                            {
                                                foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair2 in fag2.faTypes)
                                                {
                                                    if (fattyAcidKeyValuePair2.Value && maxDoubleBond2 >= fattyAcidDoubleBond2)
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
                                                                foreach (int fattyAcidHydroxyl3 in fag1.hydroxylCounts)
                                                                {
                                                                    foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair3 in fag3.faTypes)
                                                                    {
                                                                        if (fattyAcidKeyValuePair3.Value && maxDoubleBond3 >= fattyAcidDoubleBond3)
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
                                                                                    foreach (int fattyAcidHydroxyl4 in fag1.hydroxylCounts)
                                                                                    {
                                                                                        foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair4 in fag4.faTypes)
                                                                                        {
                                                                                            if (fattyAcidKeyValuePair4.Value && maxDoubleBond4 >= fattyAcidDoubleBond4)
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
                                                                                                
                                                                                                foreach (KeyValuePair<string, bool> adduct in adducts)
                                                                                                {
                                                                                                    if (adduct.Value)
                                                                                                    {
                                                                                                        String keyAdduct = key + " " + adduct.Key;
                                                                                                        String precursorAdduct = "[M" + adduct.Key + "]";
                                                                                                        if (!usedKeys.Contains(keyAdduct))
                                                                                                        {
                                                                                                            usedKeys.Add(keyAdduct);
                                                                                                            
                                                                                                            DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                                                                            MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                                                                                            MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                                                                                            MS2Fragment.addCounts(atomsCount, fa3.atomsCount);
                                                                                                            MS2Fragment.addCounts(atomsCount, fa4.atomsCount);
                                                                                                            MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                                                                            String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                                                            int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                                                                            double mass = LipidCreatorForm.computeMass(atomsCount, charge) / (double)(Math.Abs(charge));                                                
                                                    
                                                                                                            ArrayList valuesMZ = new ArrayList();
                                                                                                            ArrayList valuesIntensity = new ArrayList();
                                                                                                            
                                                                                                            foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                                                                                            {
                                                                                                                if (((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)))
                                                                                                                {
                                                                                                                    DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                                                                                    foreach (string fbase in fragment.fragmentBase)
                                                                                                                    {
                                                                                                                        switch(fbase)
                                                                                                                        {
                                                                                                                            case "FA1":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa1.atomsCount);
                                                                                                                                break;
                                                                                                                            case "FA2":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa2.atomsCount);
                                                                                                                                break;
                                                                                                                            case "FA3":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa3.atomsCount);
                                                                                                                                break;
                                                                                                                            case "FA4":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, fa4.atomsCount);
                                                                                                                                break;
                                                                                                                            case "PRE":
                                                                                                                                MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                                                                                break;
                                                                                                                            default:
                                                                                                                                break;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    //String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                                                                                    //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                                                                                    double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge) / (double)(Math.Abs(fragment.fragmentCharge));
                                                            
                                                                                                                    valuesMZ.Add(massFragment);
                                                                                                                    valuesIntensity.Add(fragment.intensity);
                                                                                                                    
                                                                                                                    
                                                                                                                    // add Annotation
                                                                                                                    /*
                                                                                                                    sql = "INSERT INTO Annotations(RefSpectraID, fragmentMZ, sumComposition, shortName) VALUES ((SELECT COUNT(*) FROM RefSpectra) + 1, " + massFragment + ", '" + chemFormFragment + "', @fragmentName)";
                                                                                                                    SQLiteParameter parameterName = new SQLiteParameter("@fragmentName", System.Data.DbType.String);
                                                                                                                    parameterName.Value = fragment.fragmentName;
                                                                                                                    command.CommandText = sql;
                                                                                                                    command.Parameters.Add(parameterName);
                                                                                                                    command.ExecuteNonQuery();
                                                                                                                    */
                                                                                                                }
                                                                                                            }
                                                    
                                                    
                                                                                                            int numFragments = valuesMZ.Count;
                                                                                                            double[] valuesMZArray = new double[numFragments];
                                                                                                            float[] valuesIntens = new float[numFragments];
                                                                                                            for(int j = 0; j < numFragments; ++j)
                                                                                                            {
                                                                                                                valuesMZArray[j] = (double)valuesMZ[j];
                                                                                                                valuesIntens[j] = 100 * (float)((double)valuesIntensity[j]);
                                                                                                            }
                                                                                                            
                                                                                                            
                                                                                                            // add MS1 information
                                                                                                            sql = "INSERT INTO RefSpectra (moleculeName, precursorMZ, precursorCharge, precursorAdduct, prevAA, nextAA, copies, numPeaks, driftTimeMsec, collisionalCrossSectionSqA, driftTimeHighEnergyOffsetMsec, retentionTime, fileID, SpecIDinFile, score, scoreType, inchiKey, otherKeys, peptideSeq, peptideModSeq, chemicalFormula) VALUES('" + key + "', " + mass + ", " + charge + ", '" + precursorAdduct + "', '-', '-', 0, " + numFragments + ", 0, 0, 0, 0, '0', 0, 1, 1, '', '', '', '',  '" + chemForm + "')";
                                                                                                            command.CommandText = sql;
                                                                                                            command.ExecuteNonQuery();
                                                                                                            
                                                                                                            // add spectrum
                                                                                                            command.CommandText = "INSERT INTO RefSpectraPeaks(RefSpectraID, peakMZ, peakIntensity) VALUES((SELECT MAX(id) FROM RefSpectra), @mzvalues, @intensvalues)";
                                                                                                            SQLiteParameter parameterMZ = new SQLiteParameter("@mzvalues", System.Data.DbType.Binary);
                                                                                                            SQLiteParameter parameterIntens = new SQLiteParameter("@intensvalues", System.Data.DbType.Binary);
                                                                                                            parameterMZ.Value = Compressing.Compress(valuesMZArray);
                                                                                                            parameterIntens.Value = Compressing.Compress(valuesIntens);
                                                                                                            command.Parameters.Add(parameterMZ);
                                                                                                            command.Parameters.Add(parameterIntens);
                                                                                                            command.ExecuteNonQuery();
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
                // check if more than one fatty acids are 0:0
                int checkFattyAcids = 0;
                string sql;
                checkFattyAcids += fag1.faTypes["FAx"] ? 1 : 0;
                checkFattyAcids += fag2.faTypes["FAx"] ? 1 : 0;
                if (checkFattyAcids > 0) return;
                if (hgValues.Count == 0) return;
                int isPlamalogen = 0;
                
                foreach (int fattyAcidLength1 in fag1.carbonCounts)
                {
                    int maxDoubleBond1 = (fattyAcidLength1 - 1) >> 1;
                    foreach (int fattyAcidDoubleBond1 in fag1.doubleBondCounts)
                    {
                        foreach (int fattyAcidHydroxyl1 in fag1.hydroxylCounts)
                        {
                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair1 in fag1.faTypes)
                            {
                                if (fattyAcidKeyValuePair1.Value && maxDoubleBond1 >= fattyAcidDoubleBond1)
                                {
                                    FattyAcid fa1 = new FattyAcid(fattyAcidLength1, fattyAcidDoubleBond1, fattyAcidHydroxyl1, fattyAcidKeyValuePair1.Key);
                                    if (fattyAcidKeyValuePair1.Key == "FAx")
                                    {
                                        fa1 = new FattyAcid(0, 0, 0, "FA");
                                    }
                                    if (fattyAcidKeyValuePair1.Key == "FAp")
                                    {
                                        isPlamalogen += 1;
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
                                                    if (fattyAcidKeyValuePair2.Value && maxDoubleBond2 >= fattyAcidDoubleBond2)
                                                    {
                                                        FattyAcid fa2 = new FattyAcid(fattyAcidLength2, fattyAcidDoubleBond2, fattyAcidHydroxyl2, fattyAcidKeyValuePair2.Key);
                                                        if (fattyAcidKeyValuePair2.Key == "FAx")
                                                        {
                                                            fa2 = new FattyAcid(0, 0, 0, "FA");
                                                        }
                                                        if (fattyAcidKeyValuePair2.Key == "FAp")
                                                        {
                                                            isPlamalogen += 1;
                                                        } 
                                                                            
                                                        List<FattyAcid> sortedAcids = new List<FattyAcid>();
                                                        sortedAcids.Add(fa1);
                                                        sortedAcids.Add(fa2);
                                                        sortedAcids.Sort();
                                                        
                                                        foreach(int hgValue in hgValues)
                                                        {
                                                        
                                                            String headgroup = headGroupNames[hgValue];
                                                            String headgroupSearch = headgroup;
                                                            String key = headgroup + " ";
                                                            if (headgroup.Equals("PC") || headgroup.Equals("PE"))
                                                            {
                                                                if (isPlamalogen >= 1) headgroupSearch = "p" + headgroupSearch;
                                                                if (isPlamalogen == 2) headgroupSearch = "p" + headgroupSearch;
                                                            }
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
                                                            
                                                            
                                                            foreach (KeyValuePair<string, bool> adduct in adducts)
                                                            {
                                                                if (adduct.Value)
                                                                {
                                                                    String keyAdduct = key + " " + adduct.Key;
                                                                    String precursorAdduct = "[M" + adduct.Key + "]";
                                                                    if (!usedKeys.Contains(keyAdduct))
                                                                    {
                                                                        usedKeys.Add(keyAdduct);
                                                            
                                                                        
                                                                        DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                                        MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                                                        MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                                                        MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroupSearch]);
                                                                        String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                                        int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                                        double mass = LipidCreatorForm.computeMass(atomsCount, charge) / (double)(Math.Abs(charge));                                                
                                                    
                                                                        ArrayList valuesMZ = new ArrayList();
                                                                        ArrayList valuesIntensity = new ArrayList();
                                                                        
                                                                        foreach (MS2Fragment fragment in MS2Fragments[headgroupSearch])
                                                                        {
                                                                            if (((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                                                            {
                                                                                DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                                                foreach (string fbase in fragment.fragmentBase)
                                                                                {
                                                                                    switch(fbase)
                                                                                    {
                                                                                        case "FA1":
                                                                                            MS2Fragment.addCounts(atomsCountFragment, fa1.atomsCount);
                                                                                            break;
                                                                                        case "FA2":
                                                                                            MS2Fragment.addCounts(atomsCountFragment, fa2.atomsCount);
                                                                                            break;
                                                                                        case "PRE":
                                                                                            MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                                            break;
                                                                                        default:
                                                                                            break;
                                                                                    }
                                                                                }
                                                                                //String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                                                                                //int chargeFragment = getChargeAndAddAdduct(atomsCountFragment, adduct.Key);
                                                                                double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge) / (double)(Math.Abs(fragment.fragmentCharge));
                                                            
                                                                                valuesMZ.Add(massFragment);
                                                                                valuesIntensity.Add(fragment.intensity);
                                                                                
                                                                                
                                                                                // add Annotation
                                                                                /*
                                                                                sql = "INSERT INTO Annotations(RefSpectraID, fragmentMZ, sumComposition, shortName) VALUES ((SELECT COUNT(*) FROM RefSpectra) + 1, " + massFragment + ", '" + chemFormFragment + "', @fragmentName)";
                                                                                SQLiteParameter parameterName = new SQLiteParameter("@fragmentName", System.Data.DbType.String);
                                                                                parameterName.Value = fragment.fragmentName;
                                                                                command.CommandText = sql;
                                                                                command.Parameters.Add(parameterName);
                                                                                command.ExecuteNonQuery();
                                                                                */
                                                                            }
                                                                        }
                                                    
                                                                        int numFragments = valuesMZ.Count;
                                                                        double[] valuesMZArray = new double[numFragments];
                                                                        float[] valuesIntens = new float[numFragments];
                                                                        for(int j = 0; j < numFragments; ++j)
                                                                        {
                                                                            valuesMZArray[j] = (double)valuesMZ[j];
                                                                            valuesIntens[j] = 100 * (float)((double)valuesIntensity[j]);
                                                                        }
                                                                        
                                                                        
                                                                        // add MS1 information
                                                                        sql = "INSERT INTO RefSpectra (moleculeName, precursorMZ, precursorCharge, precursorAdduct, prevAA, nextAA, copies, numPeaks, driftTimeMsec, collisionalCrossSectionSqA, driftTimeHighEnergyOffsetMsec, retentionTime, fileID, SpecIDinFile, score, scoreType, inchiKey, otherKeys, peptideSeq, peptideModSeq, chemicalFormula) VALUES('" + key + "', " + mass + ", " + charge + ", '" + precursorAdduct + "', '-', '-', 0, " + numFragments + ", 0, 0, 0, 0, '0', 0, 1, 1, '', '', '', '',  '" + chemForm + "')";
                                                                        command.CommandText = sql;
                                                                        command.ExecuteNonQuery();
                                                                        
                                                                        // add spectrum
                                                                        command.CommandText = "INSERT INTO RefSpectraPeaks(RefSpectraID, peakMZ, peakIntensity) VALUES((SELECT MAX(id) FROM RefSpectra), @mzvalues, @intensvalues)";
                                                                        SQLiteParameter parameterMZ = new SQLiteParameter("@mzvalues", System.Data.DbType.Binary);
                                                                        SQLiteParameter parameterIntens = new SQLiteParameter("@intensvalues", System.Data.DbType.Binary);
                                                                        parameterMZ.Value = Compressing.Compress(valuesMZArray);
                                                                        parameterIntens.Value = Compressing.Compress(valuesIntens);
                                                                        command.Parameters.Add(parameterMZ);
                                                                        command.Parameters.Add(parameterIntens);
                                                                        command.ExecuteNonQuery();
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