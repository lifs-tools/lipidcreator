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
using csgoslin;

namespace LipidCreator
{
    [Serializable]
    public class Glycerolipid : Lipid
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Glycerolipid));
        public FattyAcidGroup fag1;
        public FattyAcidGroup fag2;
        public FattyAcidGroup fag3;
        public bool containsSugar;
        
    
    
        public Glycerolipid(LipidCreator lipidCreator) : base(lipidCreator, LipidCategory.Glycerolipid)
        {
            fag1 = new FattyAcidGroup();
            fag2 = new FattyAcidGroup();
            fag3 = new FattyAcidGroup();
            containsSugar = false;
            adducts["+NH4"] = true;
            adducts["-H"] = false;
        }
    
        public Glycerolipid(Glycerolipid copy) : base((Lipid)copy) 
        {
            fag1 = new FattyAcidGroup(copy.fag1);
            fag2 = new FattyAcidGroup(copy.fag2);
            fag3 = new FattyAcidGroup(copy.fag3);
            containsSugar = copy.containsSugar;
        }
        
        
        
        
        public override ArrayList getFattyAcidGroupList()
        {
            return new ArrayList{fag1, fag2, fag3};
        }
        
        
        
        
        public override ulong getHashCode()
        {
            unchecked
            {
                ulong hashCode = base.getHashCode() + 77489593927525UL;
                hashCode += LipidCreator.rotateHash(fag1.getHashCode(), 5);
                hashCode += LipidCreator.rotateHash(fag2.getHashCode(), 29);
                hashCode += LipidCreator.rotateHash(fag3.getHashCode(), 59);
                hashCode += containsSugar ? (1UL << 18) : (1UL << 58);
                if (hashCode == 0) hashCode += 1UL;
                return hashCode;
            }
        }
        
        
        public override void serialize(StringBuilder sb)
        {
            sb.Append("<lipid type=\"GL\">\n");
            fag1.serialize(sb);
            fag2.serialize(sb);
            fag3.serialize(sb);
            sb.Append("<containsSugar>" + (containsSugar ? 1 : 0) + "</containsSugar>\n");
            base.serialize(sb);
            sb.Append("</lipid>\n");
        }
        
        
        
        
        public override void serializeFragments(StringBuilder sb)
        {
            foreach (KeyValuePair<string, HashSet<string>> positiveFragment in positiveFragments)
            {                
                sb.Append("<positiveFragments lipidClass=\"" + positiveFragment.Key + "\">\n");
                foreach (string fragment in positiveFragment.Value)
                {
                    sb.Append("<fragment>" + fragment + "</fragment>\n");
                }
                sb.Append("</positiveFragments>\n");
            }
            
            foreach (KeyValuePair<string, HashSet<string>> negativeFragment in negativeFragments)
            {
                sb.Append("<negativeFragments lipidClass=\"" + negativeFragment.Key + "\">\n");
                foreach (string fragment in negativeFragment.Value)
                {
                    sb.Append("<fragment>" + fragment + "</fragment>\n");
                }
                sb.Append("</negativeFragments>\n");
            }
        }
        
        
        
        // synchronize the fragment list with list from LipidCreator root
        public override void Update(object sender, EventArgs e)
        {
            Updating((int)LipidCategory.Glycerolipid);
        }
        
        
        
        
        
        
        public override void import(XElement node, string importVersion)
        {
            int fattyAcidCounter = 0;
            headGroupNames.Clear();
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
                        else
                        {   
                            log.Error("A Glycerolipid can have at most 3 fatty acid chains. Found: " + (fattyAcidCounter + 1) + "");
                            throw new Exception();
                        }
                        ++fattyAcidCounter;
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
        
        
        public override void computePrecursorData(IDictionary<String, Precursor> headgroups, HashSet<String> usedKeys, ArrayList precursorDataList)
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
                            // create species id i.e. key for avoiding double entries
                            var fattys = from fa in sortedAcids where fa.length > 0 && fa.suffix != "x" select fa.ToString();
                            string key = " " + string.Join(ID_SEPARATOR_UNSPECIFIC, fattys);
                            
                            
                            // goslin
                            csgoslin.LipidSpecies lipidSpecies = convertLipid(headgroup, sortedAcids);
                            
                            
                            // species name
                            FattyAcid speciesFA = new FattyAcid(fa1);
                            speciesFA.merge(fa2);
                            string speciesName = lipidSpecies.get_lipid_string(csgoslin.LipidLevel.SPECIES);
                            
                            
                            foreach (string adductKey in adducts.Keys.Where(x => adducts[x]))
                            {
                                if (!headgroups[headgroup].adductRestrictions[adductKey]) continue;
                                if (usedKeys.Contains(headgroup + key + adductKey)) continue;
                                
                                usedKeys.Add(headgroup + key + adductKey);
                                
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
                                
                                PrecursorData precursorData = new PrecursorData();
                                precursorData.lipidCategory = LipidCategory.Glycerolipid;
                                precursorData.moleculeListName = lipidSpecies.get_lipid_string(csgoslin.LipidLevel.CLASS);
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
                                    
                                    if (!headgroups[heavyHeadgroup].adductRestrictions[adductKey]) continue;
                                    
                                    FattyAcid heavyFA1 = new FattyAcid(fa1);
                                    FattyAcid heavyFA2 = new FattyAcid(fa2);
                                    heavyFA1.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[0]);
                                    heavyFA2.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[1]);
                                    List<FattyAcid> heavySortedAcids = new List<FattyAcid>();
                                    heavySortedAcids.Add(heavyFA1);
                                    heavySortedAcids.Add(heavyFA2);
                                    heavySortedAcids.Sort();
                        
                                    ElementDictionary heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA1.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA2.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                    string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                    string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct);
                                    MS2Fragment.addCounts(heavyAtomsCount, adduct.elements);
                                    double heavyMass = LipidCreator.computeMass(heavyAtomsCount, charge);
                                    
                                    
                                    string heavyKey = LipidCreator.precursorNameSplit(heavyHeadgroup)[0] + LipidCreator.computeHeavyIsotopeLabel(headgroups[heavyHeadgroup].elements);
                                    
                                    
                                    var heavyFattys = from fa in heavySortedAcids where fa.length > 0 && fa.suffix != "x" select fa.ToString();
                                    string heavyFattyComp = " " + string.Join(ID_SEPARATOR_UNSPECIFIC, heavyFattys);
                                    
                                    
                                    // species name
                                    FattyAcid heavySpeciesFA = new FattyAcid(heavyFA1);
                                    heavySpeciesFA.merge(heavyFA2);
                                                                        

                                    PrecursorData heavyPrecursorData = new PrecursorData();
                                    heavyPrecursorData.lipidCategory = LipidCategory.Glycerolipid;
                                    heavyPrecursorData.moleculeListName = headgroup;
                                    heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                                    heavyPrecursorData.precursorExportName = headgroup + key;
                                    heavyPrecursorData.precursorName = heavyKey + heavyFattyComp.ToString();
                                    heavyPrecursorData.precursorSpeciesName = heavyKey + " " + heavySpeciesFA.ToString();
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
                            
                                
                            // create species id i.e. key for avoiding double entries
                            var fattys = from fa in sortedAcids where fa.length > 0 && fa.suffix != "x" select fa.ToString();
                            string key = " " + string.Join(ID_SEPARATOR_UNSPECIFIC, fattys);
    
                            // goslin
                            csgoslin.LipidSpecies lipidSpecies = convertLipid(headgroup, sortedAcids);
                            
                            // species name
                            FattyAcid speciesFA = new FattyAcid(fa1);
                            speciesFA.merge(fa2);
                            speciesFA.merge(fa3);
                            string speciesName = lipidSpecies.get_lipid_string(csgoslin.LipidLevel.SPECIES);
                            
                            
                            foreach (string adductKey in adducts.Keys.Where(x => adducts[x]))
                            {
                                if (!headgroups[headgroup].adductRestrictions[adductKey]) continue;
                                if (usedKeys.Contains(headgroup + key + adductKey)) continue;
                                
                                usedKeys.Add(headgroup + key + adductKey);
                                
                                ElementDictionary atomsCount = MS2Fragment.createEmptyElementDict();
                                MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                MS2Fragment.addCounts(atomsCount, fa3.atomsCount);
                                MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                                string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                                Adduct adduct = Lipid.ALL_ADDUCTS[Lipid.ADDUCT_POSITIONS[adductKey]];
                                string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct);
                                int charge = adduct.charge;
                                MS2Fragment.addCounts(atomsCount, adduct.elements);
                                double mass = LipidCreator.computeMass(atomsCount, charge);
                                
                                PrecursorData precursorData = new PrecursorData();
                                precursorData.lipidCategory = LipidCategory.Glycerolipid;
                                precursorData.moleculeListName = lipidSpecies.get_lipid_string(csgoslin.LipidLevel.CLASS);
                                precursorData.fullMoleculeListName = headgroup;
                                precursorData.precursorExportName = lipidSpecies.get_lipid_string();
                                precursorData.precursorSpeciesName = speciesName;
                                precursorData.precursorName = lipidSpecies.get_lipid_string();
                                precursorData.precursorIonFormula = chemForm;
                                precursorData.precursorAdduct = adduct;
                                precursorData.precursorAdductFormula = adductForm;
                                precursorData.precursorM_Z = mass;
                                precursorData.fa1 = sortedAcids[0];
                                precursorData.fa2 = sortedAcids[1];
                                precursorData.fa3 = sortedAcids[2];
                                precursorData.fa4 = null;
                                precursorData.lcb = null;
                                precursorData.addPrecursor = (onlyPrecursors != 0);
                                precursorData.fragmentNames = (onlyPrecursors != 1) ? ((charge > 0) ? positiveFragments[headgroup] : negativeFragments[headgroup]) : new HashSet<string>();
                                
                                
                                
                                
                                
                                if (onlyHeavyLabeled != 1) precursorDataList.Add(precursorData);
                                
                                
                                if (onlyHeavyLabeled == 0) continue;
                                foreach (Precursor heavyPrecursor  in headgroups[headgroup].heavyLabeledPrecursors)
                                {
                                    string heavyHeadgroup = heavyPrecursor.name;
                                    
                                    if (!headgroups[heavyHeadgroup].adductRestrictions[adductKey]) continue;
                                        
                                    FattyAcid heavyFA1 = new FattyAcid(fa1);
                                    FattyAcid heavyFA2 = new FattyAcid(fa2);
                                    FattyAcid heavyFA3 = new FattyAcid(fa3);
                                    heavyFA1.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[0]);
                                    if (headgroup.Equals("DAG") || headgroup.Equals("TAG")) heavyFA2.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[1]);
                                    if (headgroup.Equals("TAG")) heavyFA3.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[2]);
                                    List<FattyAcid> heavySortedAcids = new List<FattyAcid>();
                                    heavySortedAcids.Add(heavyFA1);
                                    heavySortedAcids.Add(heavyFA2);
                                    heavySortedAcids.Add(heavyFA3);
                                    heavySortedAcids.Sort();
                        
                                    ElementDictionary heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA1.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA2.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA3.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                    string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                    string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct);
                                    MS2Fragment.addCounts(heavyAtomsCount, adduct.elements);
                                    double heavyMass = LipidCreator.computeMass(heavyAtomsCount, charge);
                                    
                                    
                                    string heavyKey = LipidCreator.precursorNameSplit(heavyHeadgroup)[0] + LipidCreator.computeHeavyIsotopeLabel(headgroups[heavyHeadgroup].elements);
                                                            
                                                            
                                                            
                                    var heavyFattys = from fa in heavySortedAcids where fa.length > 0 && fa.suffix != "x" select fa.ToString();
                                    string heavyFattyComp = " " + string.Join(ID_SEPARATOR_UNSPECIFIC, heavyFattys);
                                    
                                    
                                    // species name
                                    FattyAcid heavySpeciesFA = new FattyAcid(heavyFA1);
                                    heavySpeciesFA.merge(heavyFA2);            

                                    PrecursorData heavyPrecursorData = new PrecursorData();
                                    heavyPrecursorData.lipidCategory = LipidCategory.Glycerolipid;
                                    heavyPrecursorData.moleculeListName = headgroup;
                                    heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                                    heavyPrecursorData.precursorExportName = headgroup + key;
                                    heavyPrecursorData.precursorName = heavyKey + heavyFattyComp.ToString();
                                    heavyPrecursorData.precursorSpeciesName = heavyKey + " " + heavySpeciesFA.ToString();
                                    heavyPrecursorData.precursorIonFormula = heavyChemForm;
                                    heavyPrecursorData.precursorAdduct = adduct;
                                    heavyPrecursorData.precursorAdductFormula = heavyAdductForm;
                                    heavyPrecursorData.precursorM_Z = heavyMass;
                                    heavyPrecursorData.fa1 = heavySortedAcids[0];
                                    heavyPrecursorData.fa2 = heavySortedAcids[1];
                                    heavyPrecursorData.fa3 = heavySortedAcids[2];
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
