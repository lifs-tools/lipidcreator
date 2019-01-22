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
using log4net;

namespace LipidCreator
{
    [Serializable]
    public class Phospholipid : Lipid
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Phospholipid));
        public FattyAcidGroup fag1;
        public FattyAcidGroup fag2;
        public FattyAcidGroup fag3;
        public FattyAcidGroup fag4;
        public bool isCL;
        public bool isLyso;
    
        public Phospholipid(LipidCreator lipidCreator) : base(lipidCreator, LipidCategory.PhosphoLipid)
        {
            fag1 = new FattyAcidGroup();
            fag2 = new FattyAcidGroup();
            fag3 = new FattyAcidGroup();
            fag4 = new FattyAcidGroup();
            isCL = false;
            isLyso = false;
        }
    
        public Phospholipid(Phospholipid copy) : base((Lipid)copy)
        {
            fag1 = new FattyAcidGroup(copy.fag1);
            fag2 = new FattyAcidGroup(copy.fag2);
            fag3 = new FattyAcidGroup(copy.fag3);
            fag4 = new FattyAcidGroup(copy.fag4);
            isCL = copy.isCL;
            isLyso = copy.isLyso;
        }
        
        public override ArrayList getFattyAcidGroupList()
        {
            return new ArrayList{fag1, fag2, fag3, fag4};
        }
        
        public override string serialize()
        {
            string xml = "<lipid type=\"PL\" isCL=\"" + isCL + "\" isLyso=\"" + isLyso + "\">\n";
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
        
        // synchronize the fragment list with list from LipidCreator root
        public override void Update(object sender, EventArgs e)
        {
            Updating((int)LipidCategory.PhosphoLipid);
        }
        
        public override void import(XElement node, string importVersion)
        {
            int fattyAcidCounter = 0;
            headGroupNames.Clear();
            isCL = node.Attribute("isCL").Value == "True";
            isLyso = node.Attribute("isLyso").Value == "True";
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
                        else if (fattyAcidCounter == 3)
                        {
                            fag4.import(child, importVersion);
                        }
                        else
                        {
                            log.Error("A phospholipid can have at most 4 fatty acid chains. Found: " + (fattyAcidCounter + 1) + "");
                            throw new Exception("Error, fatty acid");
                        }
                        ++fattyAcidCounter;
                        break;
                        
                    case "headGroup":
                        headGroupNames.Add(child.Value.ToString());
                        break;
                        
                        
                    default:
                        base.import(child, importVersion);
                        break;
                }
            }
        }
        
        
        public override void computePrecursorData(IDictionary<String, Precursor> headgroups, HashSet<String> usedKeys, ArrayList precursorDataList)
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
                    
                        // calling all possible fatty acid 3 combinations
                        foreach (FattyAcid fa3 in fag3.getFattyAcids())
                        {
                            containsMonoLyso &= ~4;
                            if (fa3.suffix == "x") containsMonoLyso |= 4;
                    
                            // calling all possible fatty acid 4 combinations
                            foreach (FattyAcid fa4 in fag4.getFattyAcids())
                            {
                                containsMonoLyso &= ~8;
                                if (fa4.suffix == "x") containsMonoLyso |= 8;
                                                
                                // sort fatty acids and check if lipid is monolyso cardiolipin
                                List<FattyAcid> sortedAcids = new List<FattyAcid>();
                                sortedAcids.Add(fa1);
                                sortedAcids.Add(fa2);
                                sortedAcids.Add(fa3);
                                sortedAcids.Add(fa4);
                                sortedAcids.Sort();
                                String headgroup = (containsMonoLyso == 0) ? "CL" : "MLCL";
                                
                                // create species id i.e. key for avoiding double entries
                                String key = " ";
                                int i = 0;
                                foreach (FattyAcid fa in sortedAcids)
                                {
                                    if (fa.length > 0 && fa.suffix != "x"){
                                        if (i++ > 0) key += ID_SEPARATOR_UNSPECIFIC;
                                        key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                        if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                        key += fa.suffix;
                                    }
                                }
                                
                                foreach (KeyValuePair<string, bool> adduct in adducts)
                                {
                                    if (!adduct.Value || !headgroups[headgroup].adductRestrictions[adduct.Key]) continue;
                                    if (usedKeys.Contains(headgroup + key + adduct.Key)) continue;
                                    
                                    usedKeys.Add(headgroup + key + adduct.Key);
                                    
                                    // adding element counts for all building blocks
                                    Dictionary<int, int> atomsCount = MS2Fragment.createEmptyElementDict();
                                    MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                    MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                    MS2Fragment.addCounts(atomsCount, fa3.atomsCount);
                                    MS2Fragment.addCounts(atomsCount, fa4.atomsCount);
                                    MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                                    string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                                    string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct.Key);
                                    int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                    double mass = LipidCreator.computeMass(atomsCount, charge);
                                    
                                    // filling information on MS1 level for cardiolipin
                                    PrecursorData precursorData = new PrecursorData();
                                    precursorData.lipidCategory = LipidCategory.PhosphoLipid;
                                    precursorData.moleculeListName = headgroup;
                                    precursorData.fullMoleculeListName = headgroup;
                                    precursorData.precursorExportName = headgroup + key;
                                    precursorData.precursorName = headgroup + key;
                                    precursorData.precursorIonFormula = chemForm;
                                    precursorData.precursorAdduct = adduct.Key;
                                    precursorData.precursorAdductFormula = adductForm;
                                    precursorData.precursorM_Z = mass / (double)(Math.Abs(charge));
                                    precursorData.precursorCharge = charge;
                                    precursorData.atomsCount = headgroups[headgroup].elements;
                                    precursorData.fa1 = sortedAcids[0];
                                    precursorData.fa2 = sortedAcids[1];
                                    precursorData.fa3 = sortedAcids[2];
                                    precursorData.fa4 = sortedAcids[3];
                                    precursorData.lcb = null;
                                    precursorData.addPrecursor = (onlyPrecursors != 0);
                                    precursorData.fragmentNames = (onlyPrecursors != 1) ? ((charge > 0) ? positiveFragments[headgroup] : negativeFragments[headgroup]) : new HashSet<string>();
                                    
                                    if (onlyHeavyLabeled != 1) precursorDataList.Add(precursorData);
                                
                                    if (onlyHeavyLabeled == 0) continue;
                                    foreach (Precursor heavyPrecursor in headgroups[headgroup].heavyLabeledPrecursors)
                                    {
                                        string heavyHeadgroup = heavyPrecursor.name;
                                        
                                        if (!headgroups[heavyHeadgroup].adductRestrictions[adduct.Key]) continue;
                                        
                                    
                                        FattyAcid heavyFA1 = new FattyAcid(fa1);
                                        FattyAcid heavyFA2 = new FattyAcid(fa2);
                                        FattyAcid heavyFA3 = new FattyAcid(fa3);
                                        FattyAcid heavyFA4 = new FattyAcid(fa4);
                                        heavyFA1.updateForHeavyLabeled((Dictionary<int, int>)heavyPrecursor.userDefinedFattyAcids[0]);
                                        heavyFA2.updateForHeavyLabeled((Dictionary<int, int>)heavyPrecursor.userDefinedFattyAcids[1]);
                                        heavyFA3.updateForHeavyLabeled((Dictionary<int, int>)heavyPrecursor.userDefinedFattyAcids[2]);
                                        if (headgroup.Equals("CL")) heavyFA4.updateForHeavyLabeled((Dictionary<int, int>)heavyPrecursor.userDefinedFattyAcids[3]);
                                        List<FattyAcid> heavySortedAcids = new List<FattyAcid>();
                                        heavySortedAcids.Add(heavyFA1);
                                        heavySortedAcids.Add(heavyFA2);
                                        heavySortedAcids.Add(heavyFA3);
                                        heavySortedAcids.Add(heavyFA4);
                                        heavySortedAcids.Sort();
                            
                                        Dictionary<int, int> heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                        MS2Fragment.addCounts(heavyAtomsCount, heavyFA1.atomsCount);
                                        MS2Fragment.addCounts(heavyAtomsCount, heavyFA2.atomsCount);
                                        MS2Fragment.addCounts(heavyAtomsCount, heavyFA3.atomsCount);
                                        MS2Fragment.addCounts(heavyAtomsCount, heavyFA4.atomsCount);
                                        MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                        string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                        string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct.Key);
                                        int heavyCharge = getChargeAndAddAdduct(heavyAtomsCount, adduct.Key);
                                        double heavyMass = LipidCreator.computeMass(heavyAtomsCount, heavyCharge);
                                        
                                        string heavyKey = LipidCreator.precursorNameSplit(heavyHeadgroup)[0] + LipidCreator.computeHeavyIsotopeLabel(heavyAtomsCount);
                                                                            
                                        // filling information on MS1 level for heavy cardiolipin
                                        PrecursorData heavyPrecursorData = new PrecursorData();
                                        heavyPrecursorData.lipidCategory = LipidCategory.PhosphoLipid;
                                        heavyPrecursorData.moleculeListName = headgroup;
                                        heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                                        heavyPrecursorData.precursorExportName = headgroup + key;
                                        heavyPrecursorData.precursorName = heavyKey + key;
                                        heavyPrecursorData.precursorIonFormula = heavyChemForm;
                                        heavyPrecursorData.precursorAdduct = adduct.Key;
                                        heavyPrecursorData.precursorAdductFormula = heavyAdductForm;
                                        heavyPrecursorData.precursorM_Z = heavyMass / (double)(Math.Abs(heavyCharge));
                                        heavyPrecursorData.precursorCharge = heavyCharge;
                                        heavyPrecursorData.atomsCount = headgroups[heavyHeadgroup].elements;
                                        heavyPrecursorData.fa1 = heavySortedAcids[0];
                                        heavyPrecursorData.fa2 = heavySortedAcids[1];
                                        heavyPrecursorData.fa3 = heavySortedAcids[2];
                                        heavyPrecursorData.fa4 = heavySortedAcids[3];
                                        heavyPrecursorData.lcb = null;
                                        heavyPrecursorData.addPrecursor = (onlyPrecursors != 0);
                                        heavyPrecursorData.fragmentNames = (onlyPrecursors != 1) ? ((heavyCharge > 0) ? positiveFragments[heavyHeadgroup] : negativeFragments[heavyHeadgroup]) : new HashSet<string>();
                                        
                                        precursorDataList.Add(heavyPrecursorData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            else if (isLyso)
            {
            
                if (headGroupNames.Count == 0) return;
                if (fag1.faTypes["FAx"]) return;
                
                bool isPlamalogen = false;
                bool isFAa = false;
                foreach (FattyAcid fa1 in fag1.getFattyAcids())
                {
                    switch (fa1.suffix)
                    {
                        case "a": isFAa = true; break;
                        case "p": isPlamalogen = true; break;
                        default: break;
                    }
                    
                    foreach(string headgroupIter in headGroupNames)
                    {   
                        string headgroup = headgroupIter;
                        if (headgroup.Equals("PA") || headgroup.Equals("PC") || headgroup.Equals("PE") || headgroup.Equals("PG") || headgroup.Equals("PI") || headgroup.Equals("PS"))
                        {
                            if (headgroup.Equals("PC") || headgroup.Equals("PE"))
                            {
                                if (isPlamalogen) headgroup = headgroup + " O-p";
                                else if (isFAa) headgroup = headgroup + " O-a";
                            }
                        }
                        
                        String key = " ";
                        if (isPlamalogen) key = key.Replace("O-p", "O");
                        else if (isFAa) key = key.Replace("O-a", "O");
                        
                        if (fa1.length > 0){
                            key += Convert.ToString(fa1.length) + ":" + Convert.ToString(fa1.db);
                            if (fa1.hydroxyl > 0) key += ";" + Convert.ToString(fa1.hydroxyl);
                            key += fa1.suffix;
                        }
                        
                        foreach (KeyValuePair<string, bool> adduct in adducts)
                        {
                            string completeKey = headgroup;
                            if (isPlamalogen) completeKey = completeKey.Replace("O-p", "O");
                            else if (isFAa) completeKey = completeKey.Replace("O-a", "O");
                            completeKey += key;
                            
                            if (!adduct.Value || !headgroups[headgroup].adductRestrictions[adduct.Key]) continue;
                            if (usedKeys.Contains(completeKey + adduct.Key)) continue;
                            
                            usedKeys.Add(completeKey + adduct.Key);
                            
                            Dictionary<int, int> atomsCount = MS2Fragment.createEmptyElementDict();
                            MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                            MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                            string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                            string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct.Key);
                            int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                            double mass = LipidCreator.computeMass(atomsCount, charge);
                                                                
                            // filling information on MS1 level for phospholipid
                            PrecursorData precursorData = new PrecursorData();
                            precursorData.lipidCategory = LipidCategory.PhosphoLipid;
                            precursorData.moleculeListName = headgroup;
                            precursorData.fullMoleculeListName = headgroup;
                            precursorData.precursorExportName = completeKey;
                            precursorData.precursorName = completeKey;
                            precursorData.precursorIonFormula = chemForm;
                            precursorData.precursorAdduct = adduct.Key;
                            precursorData.precursorAdductFormula = adductForm;
                            precursorData.precursorM_Z = mass / (double)(Math.Abs(charge));
                            precursorData.precursorCharge = charge;
                            precursorData.atomsCount = headgroups[headgroup].elements;
                            precursorData.fa1 = fa1;
                            precursorData.fa2 = null;
                            precursorData.fa3 = null;
                            precursorData.fa4 = null;
                            precursorData.lcb = null;
                            precursorData.addPrecursor = (onlyPrecursors != 0);
                            precursorData.fragmentNames = (onlyPrecursors != 1) ? ((charge > 0) ? positiveFragments[headgroup] : negativeFragments[headgroup]) : new HashSet<string>();
                            
                            if (onlyHeavyLabeled != 1) precursorDataList.Add(precursorData);
                            
                            if (onlyHeavyLabeled == 0) continue;
                            foreach (Precursor heavyPrecursor  in headgroups[headgroup].heavyLabeledPrecursors)
                            {
                                
                                string heavyHeadgroup = heavyPrecursor.name;
                                
                                if (!headgroups[heavyHeadgroup].adductRestrictions[adduct.Key]) continue;
                                
                            
                                FattyAcid heavyFA1 = new FattyAcid(fa1);
                                
                                heavyFA1.updateForHeavyLabeled((Dictionary<int, int>)heavyPrecursor.userDefinedFattyAcids[0]);
                                
                                Dictionary<int, int> heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                MS2Fragment.addCounts(heavyAtomsCount, heavyFA1.atomsCount);
                                MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct.Key);
                                int heavyCharge = getChargeAndAddAdduct(heavyAtomsCount, adduct.Key);
                                double heavyMass = LipidCreator.computeMass(heavyAtomsCount, heavyCharge);
                                
                                string heavyKey = LipidCreator.precursorNameSplit(heavyHeadgroup)[0] + LipidCreator.computeHeavyIsotopeLabel(heavyAtomsCount);
                                                                    
                                // filling information on MS1 level for heavy phospholipid
                                PrecursorData heavyPrecursorData = new PrecursorData();
                                heavyPrecursorData.lipidCategory = LipidCategory.PhosphoLipid;
                                heavyPrecursorData.moleculeListName = headgroup;
                                heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                                heavyPrecursorData.precursorExportName = completeKey;
                                heavyPrecursorData.precursorName = heavyKey + key;
                                heavyPrecursorData.precursorIonFormula = heavyChemForm;
                                heavyPrecursorData.precursorAdduct = adduct.Key;
                                heavyPrecursorData.precursorAdductFormula = heavyAdductForm;
                                heavyPrecursorData.precursorM_Z = heavyMass / (double)(Math.Abs(heavyCharge));
                                heavyPrecursorData.precursorCharge = heavyCharge;
                                heavyPrecursorData.atomsCount = headgroups[heavyHeadgroup].elements;
                                heavyPrecursorData.fa1 = heavyFA1;
                                heavyPrecursorData.fa2 = null;
                                heavyPrecursorData.fa3 = null;
                                heavyPrecursorData.fa4 = null;
                                heavyPrecursorData.lcb = null;
                                heavyPrecursorData.addPrecursor = (onlyPrecursors != 0);
                                heavyPrecursorData.fragmentNames = (onlyPrecursors != 1) ? ((heavyCharge > 0) ? positiveFragments[heavyHeadgroup] : negativeFragments[heavyHeadgroup]) : new HashSet<string>();
                                
                                precursorDataList.Add(heavyPrecursorData);
                            }
                        }
                    }
                }
            }
            
            else
            {
                        
            
                if (headGroupNames.Count == 0) return;
                foreach (FattyAcid fa1 in fag1.getFattyAcids())
                {
                    bool isPlamalogen1 = false;
                    bool isFAa1 = false;
                        
                    switch (fa1.suffix)
                    {
                        case "a": isFAa1 = true; break;
                        case "p": isPlamalogen1 = true; break;
                        default: break;
                    }
                    
                    foreach (FattyAcid fa2 in fag2.getFattyAcids())
                    {
                        bool isPlamalogen = isPlamalogen1;
                        bool isFAa = isFAa1;
                        switch (fa2.suffix)
                        {
                            case "a": isFAa = true; break;
                            case "p": isPlamalogen = true; break;
                            default: break;
                        }        
                        List<FattyAcid> sortedAcids = new List<FattyAcid>();
                        List<FattyAcid> unsortedAcids = new List<FattyAcid>();
                        sortedAcids.Add(fa1);
                        sortedAcids.Add(fa2);
                        unsortedAcids.Add(fa1);
                        unsortedAcids.Add(fa2);
                        sortedAcids.Sort();
                        
                        foreach(string headgroupIter in headGroupNames)
                        {   
                            string headgroup = headgroupIter;
                            bool isSorted = true;
                            
                            
                            if (headgroup.Equals("PA") || headgroup.Equals("PC") || headgroup.Equals("PE") || headgroup.Equals("PG") || headgroup.Equals("PI") || headgroup.Equals("PS"))
                            {
                                if (headgroup.Equals("PC") || headgroup.Equals("PE"))
                                {
                                    if (isPlamalogen)
                                    {
                                        headgroup = headgroup + " O-p";
                                        isSorted = false;
                                    }
                                    else if (isFAa)
                                    {
                                        headgroup = headgroup + " O-a";
                                        isSorted = false;
                                    }
                                }
                            }
                            
                            
                            String key = " ";
                            int i = 0;
                            if (isSorted){
                                foreach (FattyAcid fa in sortedAcids)
                                {
                                    if (fa.length > 0 && fa.suffix != "x"){
                                        if (i++ > 0) key += ID_SEPARATOR_UNSPECIFIC;
                                        key += fa.ToString();
                                    }
                                }
                            }
                            else
                            {
                                foreach (FattyAcid fa in unsortedAcids)
                                {
                                    if (fa.length > 0 && fa.suffix != "x"){
                                        if (i++ > 0) key += ID_SEPARATOR_SPECIFIC;
                                        key += fa.ToString();
                                    }
                                }
                            }
                            
                          
                            foreach (KeyValuePair<string, bool> adduct in adducts)
                            {
                                string completeKey = headgroup;
                                if (isPlamalogen) completeKey = completeKey.Replace("O-p", "O");
                                else if (isFAa) completeKey = completeKey.Replace("O-a", "O");
                                completeKey += key;
                                
                                if (!adduct.Value || !headgroups[headgroup].adductRestrictions[adduct.Key]) continue;
                                if (usedKeys.Contains(completeKey + adduct.Key)) continue;
                                
                                usedKeys.Add(completeKey + adduct.Key);
                                
                                Dictionary<int, int> atomsCount = MS2Fragment.createEmptyElementDict();
                                MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                                string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                                string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct.Key);
                                int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                double mass = LipidCreator.computeMass(atomsCount, charge);
                                                                    
                                // filling information on MS1 level for phospholipid
                                PrecursorData precursorData = new PrecursorData();
                                precursorData.lipidCategory = LipidCategory.PhosphoLipid;
                                precursorData.moleculeListName = headgroup;
                                precursorData.fullMoleculeListName = headgroup;
                                precursorData.precursorExportName = completeKey;
                                precursorData.precursorName = completeKey;
                                precursorData.precursorIonFormula = chemForm;
                                precursorData.precursorAdduct = adduct.Key;
                                precursorData.precursorAdductFormula = adductForm;
                                precursorData.precursorM_Z = mass / (double)(Math.Abs(charge));
                                precursorData.precursorCharge = charge;
                                precursorData.atomsCount = headgroups[headgroup].elements;
                                
                                if (!isSorted)
                                {
                                    precursorData.fa1 = fa1;
                                    precursorData.fa2 = fa2;
                                }
                                else
                                {
                                    precursorData.fa1 = sortedAcids[0];
                                    precursorData.fa2 = sortedAcids[1];
                                }
                                precursorData.fa3 = null;
                                precursorData.fa4 = null;
                                precursorData.lcb = null;
                                precursorData.addPrecursor = (onlyPrecursors != 0);
                                precursorData.fragmentNames = (onlyPrecursors != 1) ? ((charge > 0) ? positiveFragments[headgroup] : negativeFragments[headgroup]) : new HashSet<string>();
                                
                                if (onlyHeavyLabeled != 1) precursorDataList.Add(precursorData);
                                
                                if (onlyHeavyLabeled == 0) continue;
                                foreach (Precursor heavyPrecursor  in headgroups[headgroup].heavyLabeledPrecursors)
                                {
                                    
                                    string heavyHeadgroup = heavyPrecursor.name;
                                    
                                    if (!headgroups[heavyHeadgroup].adductRestrictions[adduct.Key]) continue;
                                
                                    FattyAcid heavyFA1 = new FattyAcid(fa1);
                                    FattyAcid heavyFA2 = new FattyAcid(fa2);
                                    
                                    heavyFA1.updateForHeavyLabeled((Dictionary<int, int>)heavyPrecursor.userDefinedFattyAcids[0]);
                                    heavyFA2.updateForHeavyLabeled((Dictionary<int, int>)heavyPrecursor.userDefinedFattyAcids[1]);
                                    List<FattyAcid> heavySortedAcids = new List<FattyAcid>();
                                    heavySortedAcids.Add(heavyFA1);
                                    heavySortedAcids.Add(heavyFA2);
                                    if (!isFAa && !isPlamalogen) heavySortedAcids.Sort();
                        
                                    Dictionary<int, int> heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA1.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA2.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                    string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                    string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct.Key);
                                    int heavyCharge = getChargeAndAddAdduct(heavyAtomsCount, adduct.Key);
                                    double heavyMass = LipidCreator.computeMass(heavyAtomsCount, heavyCharge);
                                    
                                    string heavyKey = LipidCreator.precursorNameSplit(heavyHeadgroup)[0] + LipidCreator.computeHeavyIsotopeLabel(heavyAtomsCount);
                                                                        
                                    // filling information on MS1 level for heavy phospholipid
                                    PrecursorData heavyPrecursorData = new PrecursorData();
                                    heavyPrecursorData.lipidCategory = LipidCategory.PhosphoLipid;
                                    heavyPrecursorData.moleculeListName = headgroup;
                                    heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                                    heavyPrecursorData.precursorExportName = completeKey;
                                    heavyPrecursorData.precursorName = heavyKey + key;
                                    heavyPrecursorData.precursorIonFormula = heavyChemForm;
                                    heavyPrecursorData.precursorAdduct = adduct.Key;
                                    heavyPrecursorData.precursorAdductFormula = heavyAdductForm;
                                    heavyPrecursorData.precursorM_Z = heavyMass / (double)(Math.Abs(heavyCharge));
                                    heavyPrecursorData.precursorCharge = heavyCharge;
                                    heavyPrecursorData.atomsCount = headgroups[heavyHeadgroup].elements;
                                    heavyPrecursorData.fa1 = heavySortedAcids[0];
                                    heavyPrecursorData.fa2 = heavySortedAcids[1];
                                    heavyPrecursorData.fa3 = null;
                                    heavyPrecursorData.fa4 = null;
                                    heavyPrecursorData.lcb = null;
                                    heavyPrecursorData.addPrecursor = (onlyPrecursors != 0);
                                    heavyPrecursorData.fragmentNames = (onlyPrecursors != 1) ? ((heavyCharge > 0) ? positiveFragments[heavyHeadgroup] : negativeFragments[heavyHeadgroup]) : new HashSet<string>();
                                    
                                    precursorDataList.Add(heavyPrecursorData);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
