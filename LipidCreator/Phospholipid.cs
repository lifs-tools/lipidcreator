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
        public bool hasPlasmalogen;
    
        public Phospholipid(LipidCreator lipidCreator) : base(lipidCreator, LipidCategory.Glycerophospholipid)
        {
            fag1 = new FattyAcidGroup();
            fag2 = new FattyAcidGroup();
            fag3 = new FattyAcidGroup();
            fag4 = new FattyAcidGroup();
            isCL = false;
            isLyso = false;
            hasPlasmalogen = false;
            adducts["-H"] = true;
        }
    
    
    
    
        public Phospholipid(Phospholipid copy) : base((Lipid)copy)
        {
            fag1 = new FattyAcidGroup(copy.fag1);
            fag2 = new FattyAcidGroup(copy.fag2);
            fag3 = new FattyAcidGroup(copy.fag3);
            fag4 = new FattyAcidGroup(copy.fag4);
            isCL = copy.isCL;
            isLyso = copy.isLyso;
            hasPlasmalogen = copy.hasPlasmalogen;
        }
        
        
        
        
        public override ArrayList getFattyAcidGroupList()
        {
            return new ArrayList{fag1, fag2, fag3, fag4};
        }
        
        
        
        
        public override ulong getHashCode()
        {
            unchecked {
                ulong hashCode = base.getHashCode() + 13034653259202UL;
                hashCode += LipidCreator.rotateHash(fag1.getHashCode(), 13);
                if (!isLyso)
                {
                    hashCode += LipidCreator.rotateHash(fag2.getHashCode(), 23);
                }
                if (isCL)
                {
                    hashCode += LipidCreator.rotateHash(fag3.getHashCode(), 37);
                    hashCode += LipidCreator.rotateHash(fag4.getHashCode(), 53);
                }
                hashCode += isLyso ? (1UL << 21) : (1UL << 55);
                hashCode += isCL ? (1UL << 13) : (1UL << 44);
                if (hashCode == 0) hashCode += 1UL;
                return hashCode;
            }
        }
        
        
        
        
        public override void serialize(StringBuilder sb)
        {
            sb.Append("<lipid type=\"PL\" isCL=\"" + isCL + "\" isLyso=\"" + isLyso + "\" hasPlasmalogen=\"" + hasPlasmalogen + "\">\n");
            fag1.serialize(sb);
            fag2.serialize(sb);
            fag3.serialize(sb);
            fag4.serialize(sb);
            base.serialize(sb);
            sb.Append("</lipid>\n");
        }
        
        // synchronize the fragment list with list from LipidCreator root
        public override void Update(object sender, EventArgs e)
        {
            Updating((int)LipidCategory.Glycerophospholipid);
        }
        
        
        
        public override void import(XElement node, string importVersion)
        {
            int fattyAcidCounter = 0;
            headGroupNames.Clear();
            isCL = ((string)node.Attribute("isCL") != null) ? node.Attribute("isCL").Value == "True" : false;
            isLyso = ((string)node.Attribute("isLyso") != null) ? node.Attribute("isLyso").Value == "True" : false;
            hasPlasmalogen = ((string)node.Attribute("hasPlasmalogen") != null) ? node.Attribute("hasPlasmalogen").Value == "True" : false;
            
            clearAdducts();
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
                                String headgroup = (containsMonoLyso == 0) ? "CL" : "LCL";
                                
                                // create species id i.e. key for avoiding double entries
                                var fattys = from fa in sortedAcids where fa.length > 0 && fa.suffix != "x" select fa.ToString();
                                string key = " " + string.Join(ID_SEPARATOR_UNSPECIFIC, fattys);
                                
                                    
                                
                                // goslin
                                csgoslin.LipidSpecies lipidSpecies = convertLipid(headgroup, sortedAcids);
                                    
                                // species name
                                FattyAcid speciesFA = new FattyAcid(fa1);
                                speciesFA.merge(fa2);
                                speciesFA.merge(fa3);
                                speciesFA.merge(fa4);
                                string speciesName = lipidSpecies.get_lipid_string(csgoslin.LipidLevel.SPECIES);
                                
                                
                                foreach (string adductKey in adducts.Keys.Where(x => adducts[x]))
                                {
                                    if (!headgroups[headgroup].adductRestrictions[adductKey]) continue;
                                    if (usedKeys.Contains(headgroup + key + adductKey)) continue;
                                    
                                    usedKeys.Add(headgroup + key + adductKey);
                                    
                                    // adding element counts for all building blocks
                                    ElementDictionary atomsCount = MS2Fragment.createEmptyElementDict();
                                    MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                    MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                    MS2Fragment.addCounts(atomsCount, fa3.atomsCount);
                                    MS2Fragment.addCounts(atomsCount, fa4.atomsCount);
                                    MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                                    string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                                    Adduct adduct = Lipid.ALL_ADDUCTS[Lipid.ADDUCT_POSITIONS[adductKey]];
                                    string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct);
                                    int charge = adduct.charge;
                                    MS2Fragment.addCounts(atomsCount, adduct.elements);
                                    double mass = LipidCreator.computeMass(atomsCount, charge);
                                    
                                    
                                    // filling information on MS1 level for cardiolipin
                                    PrecursorData precursorData = new PrecursorData();
                                    precursorData.lipidCategory = LipidCategory.Glycerophospholipid;
                                    precursorData.moleculeListName = lipidSpecies.get_lipid_string(csgoslin.LipidLevel.CLASS);;
                                    precursorData.fullMoleculeListName = headgroup;
                                    precursorData.precursorExportName = lipidSpecies.get_lipid_string();
                                    precursorData.precursorName = lipidSpecies.get_lipid_string();
                                    precursorData.precursorSpeciesName = speciesName;
                                    precursorData.precursorIonFormula = chemForm;
                                    precursorData.precursorAdduct = adduct;
                                    precursorData.precursorAdductFormula = adductForm;
                                    precursorData.precursorM_Z = mass;
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
                                        
                                        if (!headgroups[heavyHeadgroup].adductRestrictions[adductKey]) continue;
                                        
                                    
                                        FattyAcid heavyFA1 = new FattyAcid(fa1);
                                        FattyAcid heavyFA2 = new FattyAcid(fa2);
                                        FattyAcid heavyFA3 = new FattyAcid(fa3);
                                        FattyAcid heavyFA4 = new FattyAcid(fa4);
                                        heavyFA1.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[0]);
                                        heavyFA2.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[1]);
                                        heavyFA3.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[2]);
                                        if (headgroup.Equals("CL")) heavyFA4.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[3]);
                                        List<FattyAcid> heavySortedAcids = new List<FattyAcid>();
                                        heavySortedAcids.Add(heavyFA1);
                                        heavySortedAcids.Add(heavyFA2);
                                        heavySortedAcids.Add(heavyFA3);
                                        heavySortedAcids.Add(heavyFA4);
                                        heavySortedAcids.Sort();
                            
                                        ElementDictionary heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                        MS2Fragment.addCounts(heavyAtomsCount, heavyFA1.atomsCount);
                                        MS2Fragment.addCounts(heavyAtomsCount, heavyFA2.atomsCount);
                                        MS2Fragment.addCounts(heavyAtomsCount, heavyFA3.atomsCount);
                                        MS2Fragment.addCounts(heavyAtomsCount, heavyFA4.atomsCount);
                                        MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                        string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                        string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct);
                                        MS2Fragment.addCounts(heavyAtomsCount, adduct.elements);
                                        double heavyMass = LipidCreator.computeMass(heavyAtomsCount, charge);
                                        
                                        string heavyKey = LipidCreator.precursorNameSplit(heavyHeadgroup)[0] + LipidCreator.computeHeavyIsotopeLabel(headgroups[heavyHeadgroup].elements);
                                        
                                        
                                        // species name
                                        FattyAcid heavySpeciesFA = new FattyAcid(heavyFA1);
                                        heavySpeciesFA.merge(heavyFA2);
                                        heavySpeciesFA.merge(heavyFA3);
                                        heavySpeciesFA.merge(heavyFA4);
                                        
                                        
                                        var heavyFattys = from fa in heavySortedAcids where fa.length > 0 && fa.suffix != "x" select fa.ToString();
                                        string heavyFattyComp = " " + string.Join(ID_SEPARATOR_UNSPECIFIC, heavyFattys);
                                                                            
                                        // filling information on MS1 level for heavy cardiolipin
                                        PrecursorData heavyPrecursorData = new PrecursorData();
                                        heavyPrecursorData.lipidCategory = LipidCategory.Glycerophospholipid;
                                        heavyPrecursorData.moleculeListName = headgroup;
                                        heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                                        heavyPrecursorData.precursorExportName = headgroup + heavyFattyComp;
                                        heavyPrecursorData.precursorName = heavyKey + heavyFattyComp;
                                        heavyPrecursorData.precursorSpeciesName = heavyKey + " " + heavySpeciesFA.ToString();
                                        heavyPrecursorData.precursorIonFormula = heavyChemForm;
                                        heavyPrecursorData.precursorAdduct = adduct;
                                        heavyPrecursorData.precursorAdductFormula = heavyAdductForm;
                                        heavyPrecursorData.precursorM_Z = heavyMass;
                                        heavyPrecursorData.fa1 = heavySortedAcids[0];
                                        heavyPrecursorData.fa2 = heavySortedAcids[1];
                                        heavyPrecursorData.fa3 = heavySortedAcids[2];
                                        heavyPrecursorData.fa4 = heavySortedAcids[3];
                                        heavyPrecursorData.lcb = null;
                                        heavyPrecursorData.addPrecursor = (onlyPrecursors != 0);
                                        heavyPrecursorData.fragmentNames = (onlyPrecursors != 1) ? ((charge > 0) ? positiveFragments[heavyHeadgroup] : negativeFragments[heavyHeadgroup]) : new HashSet<string>();
                                        
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
                    
                    
                    foreach(string headgroupIter in headGroupNames)
                    {   
                        bool isPlamalogen = isPlamalogen1;
                        bool isFAa = isFAa1;
                        string headgroup = headgroupIter;
                        
                        
                        string PLsep = " ";
                        string modifiedHeadgroup = headgroup;
                        string goslinHeadgroup = headgroup;
                        
                        
                        if ((isFAa || isPlamalogen) && (headgroup.Equals("LPC") || headgroup.Equals("LPE"))) continue;
                        if (headgroup.Equals("LPC O-p") || headgroup.Equals("LPE O-p"))
                        {
                            if (isFAa || !isPlamalogen) continue;
                            modifiedHeadgroup = modifiedHeadgroup.Replace("O-p", "O-");
                            goslinHeadgroup = headgroup.Replace(" O-p", "");
                            PLsep = "";
                            fa1.suffix = "p";
                        }
                        else if (headgroup.Equals("LPC O-a") || headgroup.Equals("LPE O-a"))
                        {
                            if (!isFAa || isPlamalogen) continue;
                            modifiedHeadgroup = modifiedHeadgroup.Replace("O-a", "O-");
                            goslinHeadgroup = headgroup.Replace(" O-a", "");
                            PLsep = "";
                            fa1.suffix = "a";
                        }
                        
                        // goslin
                        csgoslin.LipidSpecies lipidSpecies = convertLipid(goslinHeadgroup, new List<FattyAcid>{fa1});
                        
                        String key = PLsep + fa1.ToString();
                        string completeKey = modifiedHeadgroup + key;
                        string speciesName = lipidSpecies.get_lipid_string(csgoslin.LipidLevel.SPECIES);
                        
                        foreach (string adductKey in adducts.Keys.Where(x => adducts[x]))
                        {
                            
                        
                            if (!headgroups[headgroup].adductRestrictions[adductKey]) continue;
                            if (usedKeys.Contains(completeKey + adductKey)) continue;
                            
                            
                            
                            usedKeys.Add(completeKey + adductKey);
                            
                            ElementDictionary atomsCount = MS2Fragment.createEmptyElementDict();
                            MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                            MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                            string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                            Adduct adduct = Lipid.ALL_ADDUCTS[Lipid.ADDUCT_POSITIONS[adductKey]];
                            string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct);
                            int charge = adduct.charge;
                            MS2Fragment.addCounts(atomsCount, adduct.elements);
                            double mass = LipidCreator.computeMass(atomsCount, charge);
                                                                
                            // filling information on MS1 level for phospholipid
                            PrecursorData precursorData = new PrecursorData();
                            precursorData.lipidCategory = LipidCategory.Glycerophospholipid;
                            precursorData.moleculeListName = headgroup;
                            precursorData.fullMoleculeListName = headgroup;
                            precursorData.precursorExportName = lipidSpecies.get_lipid_string();
                            precursorData.precursorName = lipidSpecies.get_lipid_string();
                            precursorData.precursorSpeciesName = speciesName;
                            precursorData.precursorIonFormula = chemForm;
                            precursorData.precursorAdduct = adduct;
                            precursorData.precursorAdductFormula = adductForm;
                            precursorData.precursorM_Z = mass;
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
                                string heavyModifiedHeadgroup = LipidCreator.precursorNameSplit(heavyHeadgroup)[0];
                                if (isPlamalogen)
                                {
                                    heavyModifiedHeadgroup = heavyModifiedHeadgroup.Replace("O-p", "O-");
                                }
                                else if (isFAa)
                                {
                                    heavyModifiedHeadgroup = heavyModifiedHeadgroup.Replace("O-a", "O-");
                                }
                                
                                if (!headgroups[heavyHeadgroup].adductRestrictions[adductKey]) continue;
                                
                            
                                FattyAcid heavyFA1 = new FattyAcid(fa1);
                                heavyFA1.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[0]);
                                
                                ElementDictionary heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                MS2Fragment.addCounts(heavyAtomsCount, heavyFA1.atomsCount);
                                MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct);
                                MS2Fragment.addCounts(heavyAtomsCount, adduct.elements);
                                double heavyMass = LipidCreator.computeMass(heavyAtomsCount, charge);
                                
                                string heavyKey = (heavyModifiedHeadgroup.IndexOf(" O-") > -1) ? heavyModifiedHeadgroup.Replace(" O-", LipidCreator.computeHeavyIsotopeLabel(headgroups[heavyHeadgroup].elements) + " O-") : heavyModifiedHeadgroup + LipidCreator.computeHeavyIsotopeLabel(headgroups[heavyHeadgroup].elements);
                                
                                                                    
                                // filling information on MS1 level for heavy phospholipid
                                PrecursorData heavyPrecursorData = new PrecursorData();
                                heavyPrecursorData.lipidCategory = LipidCategory.Glycerophospholipid;
                                heavyPrecursorData.moleculeListName = headgroup;
                                heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                                heavyPrecursorData.precursorExportName = completeKey;
                                heavyPrecursorData.precursorName = heavyKey + PLsep+ heavyFA1.ToString();
                                heavyPrecursorData.precursorSpeciesName = heavyKey + PLsep + heavyFA1.ToString();
                                heavyPrecursorData.precursorIonFormula = heavyChemForm;
                                heavyPrecursorData.precursorAdduct = adduct;
                                heavyPrecursorData.precursorAdductFormula = heavyAdductForm;
                                heavyPrecursorData.precursorM_Z = heavyMass;
                                heavyPrecursorData.fa1 = heavyFA1;
                                heavyPrecursorData.fa2 = null;
                                heavyPrecursorData.fa3 = null;
                                heavyPrecursorData.fa4 = null;
                                heavyPrecursorData.lcb = null;
                                heavyPrecursorData.addPrecursor = (onlyPrecursors != 0);
                                heavyPrecursorData.fragmentNames = (onlyPrecursors != 1) ? ((charge > 0) ? positiveFragments[heavyHeadgroup] : negativeFragments[heavyHeadgroup]) : new HashSet<string>();
                                
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
                            bool isPlamalogen = isPlamalogen1;
                            bool isFAa = isFAa1;
                            
                            bool isSorted = true;
                            string PLsep = " ";
                            string modifiedHeadgroup = headgroup;
                        string goslinHeadgroup = headgroup;
                            if ((isFAa || isPlamalogen) && (headgroup.Equals("PC") || headgroup.Equals("PE"))) continue;
                            
                            
                            if (headgroup.Equals("PC O-p") || headgroup.Equals("PE O-p"))
                            {
                                if (isFAa || !isPlamalogen) continue;
                                modifiedHeadgroup = modifiedHeadgroup.Replace("O-p", "O-");
                                goslinHeadgroup = headgroup.Replace(" O-p", "");
                                PLsep = "";
                                fa1.suffix = "p";
                            }
                            else if (headgroup.Equals("PC O-a") || headgroup.Equals("PE O-a"))
                            {
                                if (!isFAa || isPlamalogen) continue;
                                modifiedHeadgroup = modifiedHeadgroup.Replace("O-a", "O-");
                                goslinHeadgroup = headgroup.Replace(" O-a", "");
                                PLsep = "";
                                fa1.suffix = "a";
                            }
                            if (isPlamalogen || isFAa) isSorted = false;
                            
                            
                            
                            
                            var fattys = from fa in (isSorted ? sortedAcids : unsortedAcids) where fa.length > 0 && fa.suffix != "x" select fa.ToString();
                            string key = PLsep + string.Join(isSorted ? ID_SEPARATOR_UNSPECIFIC : ID_SEPARATOR_SPECIFIC, fattys);
                        
                            // goslin
                            csgoslin.LipidSpecies lipidSpecies = convertLipid(goslinHeadgroup, sortedAcids);
                            
                            
                            // species name
                            FattyAcid speciesFA = new FattyAcid(fa1);
                            speciesFA.merge(fa2);
                            string speciesName = lipidSpecies.get_lipid_string(csgoslin.LipidLevel.SPECIES);
                            string completeKey = modifiedHeadgroup + key;
                            
                            
                            

                          
                            foreach (string adductKey in adducts.Keys.Where(x => adducts[x]))
                            {
                                
                                if (!headgroups[headgroup].adductRestrictions[adductKey]) continue;
                                if (usedKeys.Contains(completeKey + adductKey)) continue;
                                
                                usedKeys.Add(completeKey + adductKey);
                                
                                ElementDictionary atomsCount = MS2Fragment.createEmptyElementDict();
                                MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                                string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                                Adduct adduct = Lipid.ALL_ADDUCTS[Lipid.ADDUCT_POSITIONS[adductKey]];
                                string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct);
                                int charge = adduct.charge;
                                MS2Fragment.addCounts(atomsCount, adduct.elements);
                                double mass = LipidCreator.computeMass(atomsCount, charge);
                                    
                                    
                                    
                                                                    
                                // filling information on MS1 level for phospholipid
                                PrecursorData precursorData = new PrecursorData();
                                precursorData.lipidCategory = LipidCategory.Glycerophospholipid;
                                precursorData.moleculeListName = headgroup;
                                precursorData.fullMoleculeListName = headgroup;
                                precursorData.precursorExportName = lipidSpecies.get_lipid_string();
                                precursorData.precursorName = lipidSpecies.get_lipid_string();
                                precursorData.precursorSpeciesName = speciesName;
                                precursorData.precursorIonFormula = chemForm;
                                precursorData.precursorAdduct = adduct;
                                precursorData.precursorAdductFormula = adductForm;
                                precursorData.precursorM_Z = mass;
                                
                                
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
                                foreach (Precursor heavyPrecursor in headgroups[headgroup].heavyLabeledPrecursors)
                                {
                                    
                                    string heavyHeadgroup = heavyPrecursor.name;
                                    string heavyModifiedHeadgroup = LipidCreator.precursorNameSplit(heavyHeadgroup)[0];
                                    if (isPlamalogen)
                                    {
                                        heavyModifiedHeadgroup = heavyModifiedHeadgroup.Replace("O-p", "O-");
                                    }
                                    else if (isFAa)
                                    {
                                        heavyModifiedHeadgroup = heavyModifiedHeadgroup.Replace("O-a", "O-");
                                    }
                                    
                                    if (!headgroups[heavyHeadgroup].adductRestrictions[adductKey]) continue;
                                
                                    FattyAcid heavyFA1 = new FattyAcid(fa1);
                                    FattyAcid heavyFA2 = new FattyAcid(fa2);
                                    
                                    heavyFA1.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[0]);
                                    heavyFA2.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[1]);
                                    List<FattyAcid> heavySortedAcids = new List<FattyAcid>();
                                    List<FattyAcid> heavyUnsortedAcids = new List<FattyAcid>();
                                    heavySortedAcids.Add(heavyFA1);
                                    heavySortedAcids.Add(heavyFA2);
                                    heavyUnsortedAcids.Add(heavyFA1);
                                    heavyUnsortedAcids.Add(heavyFA2);
                                    if (!isFAa && !isPlamalogen) heavySortedAcids.Sort();
                        
                                    ElementDictionary heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA1.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA2.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                    string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                    string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct);
                                    MS2Fragment.addCounts(heavyAtomsCount, adduct.elements);
                                    double heavyMass = LipidCreator.computeMass(heavyAtomsCount, charge);
                                    
                                    string heavyKey = (heavyModifiedHeadgroup.IndexOf(" O-") > -1) ? heavyModifiedHeadgroup.Replace(" O-", LipidCreator.computeHeavyIsotopeLabel(headgroups[heavyHeadgroup].elements) + " O-") : heavyModifiedHeadgroup + LipidCreator.computeHeavyIsotopeLabel(headgroups[heavyHeadgroup].elements);
                                    
                                    
                                    var heavyFattys = from fa in (isSorted ? heavySortedAcids : heavyUnsortedAcids) where fa.length > 0 && fa.suffix != "x" select fa.ToString();
                                    string heavyFattyComp = PLsep + string.Join(isSorted ? ID_SEPARATOR_UNSPECIFIC : ID_SEPARATOR_SPECIFIC, heavyFattys);
                                    
                                    
                                    // species name
                                    FattyAcid heavySpeciesFA = new FattyAcid(heavyFA1);
                                    heavySpeciesFA.merge(heavyFA2);
                                    
                                                                        
                                    // filling information on MS1 level for heavy phospholipid
                                    PrecursorData heavyPrecursorData = new PrecursorData();
                                    heavyPrecursorData.lipidCategory = LipidCategory.Glycerophospholipid;
                                    heavyPrecursorData.moleculeListName = headgroup;
                                    heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                                    heavyPrecursorData.precursorExportName = completeKey;
                                    heavyPrecursorData.precursorName = heavyKey + heavyFattyComp;
                                    heavyPrecursorData.precursorSpeciesName = heavyKey + PLsep + heavySpeciesFA.ToString();
                                    heavyPrecursorData.precursorIonFormula = heavyChemForm;
                                    heavyPrecursorData.precursorAdduct = adduct;
                                    heavyPrecursorData.precursorAdductFormula = heavyAdductForm;
                                    heavyPrecursorData.precursorM_Z = heavyMass;
                                    heavyPrecursorData.fa1 = heavySortedAcids[0];
                                    heavyPrecursorData.fa2 = heavySortedAcids[1];
                                    heavyPrecursorData.fa3 = null;
                                    heavyPrecursorData.fa4 = null;
                                    heavyPrecursorData.lcb = null;
                                    heavyPrecursorData.addPrecursor = (onlyPrecursors != 0);
                                    heavyPrecursorData.fragmentNames = (onlyPrecursors != 1) ? ((charge > 0) ? positiveFragments[heavyHeadgroup] : negativeFragments[heavyHeadgroup]) : new HashSet<string>();
                                    
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
