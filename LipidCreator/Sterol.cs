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
    public class Sterol : Lipid
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Sterol));
        public bool containsEster;
        public FattyAcidGroup fag;
    
    
        public Sterol(LipidCreator lipidCreator) : base(lipidCreator, LipidCategory.Sterollipid)
        {
            fag = new FattyAcidGroup();
            containsEster = false;
            adducts["+NH4"] = true;
            adducts["-H"] = false;
        }
    
        public Sterol(Sterol copy) : base((Lipid)copy) 
        {
            fag = new FattyAcidGroup(copy.fag);
            containsEster = copy.containsEster;
        }
        
        
        
        public override ArrayList getFattyAcidGroupList()
        {
            return new ArrayList{fag};
        }
        
        
        
        
        public override ulong getHashCode()
        {
            unchecked
            {
                ulong hashCode = base.getHashCode() + 59829043095020UL;
                hashCode += fag.getHashCode();
                hashCode += containsEster ? (1UL << 31) : (1UL << 15);
                if (hashCode == 0) hashCode += 1UL;
                return hashCode;
            }
        }
        
        
        public override void serialize(StringBuilder sb)
        {
            sb.Append("<lipid type=\"Sterol\" containsEster=\"" + containsEster + "\">\n");
            fag.serialize(sb);
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
            Updating((int)LipidCategory.Sterollipid);
        }
        
        
        
        
        
        public override void import(XElement node, string importVersion)
        {
            int fattyAcidCounter = 0;
            containsEster = node.Attribute("containsEster").Value == "True";
            clearAdducts();
            foreach (XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "FattyAcidGroup":
                        if (fattyAcidCounter == 0)
                        {
                            fag.import(child, importVersion);
                        }
                        else
                        {   
                            log.Error("A Sterol can have at most 1 fatty acid chains. Found: " + (fattyAcidCounter + 1) + "");
                            throw new Exception();
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
            if (containsEster)
            {   
                foreach (FattyAcid fa in fag.getFattyAcids())
                {
                    foreach (string headgroup in headGroupNames)
                    {
                        string key = "/";
                        key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                        if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                        key += fa.suffix;
                        
                        // goslin
                        csgoslin.LipidSpecies lipidSpecies = convertLipid(headgroup, new List<FattyAcid>{fa});
                        string speciesName = lipidSpecies.get_lipid_string(csgoslin.LipidLevel.SPECIES);
                        
                        foreach (string adductKey in adducts.Keys.Where(x => adducts[x]))
                        {
                            if (!headgroups[headgroup].adductRestrictions[adductKey]) continue;
                            if (usedKeys.Contains(headgroup + key + adductKey)) continue;
                            
                            usedKeys.Add(headgroup + key + adductKey);
                            
                            ElementDictionary atomsCount = MS2Fragment.createEmptyElementDict();
                            MS2Fragment.addCounts(atomsCount, fa.atomsCount);
                            MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                            string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                            Adduct adduct = Lipid.ALL_ADDUCTS[Lipid.ADDUCT_POSITIONS[adductKey]];
                            string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct);
                            int charge = adduct.charge;
                            MS2Fragment.addCounts(atomsCount, adduct.elements);
                            double mass = LipidCreator.computeMass(atomsCount, charge);
                        
                            PrecursorData precursorData = new PrecursorData();
                            precursorData.lipidCategory = LipidCategory.Sterollipid;
                            precursorData.moleculeListName = lipidSpecies.get_lipid_string(csgoslin.LipidLevel.CLASS);
                            precursorData.fullMoleculeListName = headgroup;
                            precursorData.precursorExportName = lipidSpecies.get_lipid_string();
                            precursorData.precursorName = lipidSpecies.get_lipid_string();
                            precursorData.precursorSpeciesName = speciesName;
                            precursorData.precursorIonFormula = chemForm;
                            precursorData.precursorAdduct = adduct;
                            precursorData.precursorAdductFormula = adductForm;
                            precursorData.precursorM_Z = mass;
                            precursorData.fa1 = fa;
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
                                
                                if (!headgroups[heavyHeadgroup].adductRestrictions[adductKey]) continue;
                                
                                
                                FattyAcid heavyFA1 = new FattyAcid(fa);
                                heavyFA1.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[0]);
                    
                                ElementDictionary heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                MS2Fragment.addCounts(heavyAtomsCount, heavyFA1.atomsCount);
                                MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct);
                                MS2Fragment.addCounts(heavyAtomsCount, adduct.elements);
                                double heavyMass = LipidCreator.computeMass(heavyAtomsCount, charge);
                                
                                string heavyKey = LipidCreator.precursorNameSplit(heavyHeadgroup)[0] + LipidCreator.computeHeavyIsotopeLabel(headgroups[heavyHeadgroup].elements);
                                                                    

                                PrecursorData heavyPrecursorData = new PrecursorData();
                                heavyPrecursorData.lipidCategory = LipidCategory.Sterollipid;
                                heavyPrecursorData.moleculeListName = headgroup;
                                heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                                heavyPrecursorData.precursorExportName = headgroup + key;
                                heavyPrecursorData.precursorName = heavyKey + " " + heavyFA1.ToString();
                                heavyPrecursorData.precursorSpeciesName = heavyKey + " " + heavyFA1.ToString();
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
                
                foreach (string headgroup in headGroupNames)
                {
                    string key = headgroup;
                    csgoslin.LipidSpecies lipidSpecies = convertLipid(headgroup, new List<FattyAcid>());
                    string speciesName = lipidSpecies.get_lipid_string(csgoslin.LipidLevel.SPECIES);
                    
                    foreach (string adductKey in adducts.Keys.Where(x => adducts[x]))
                    {
                        if (!headgroups[headgroup].adductRestrictions[adductKey]) continue;
                        if (usedKeys.Contains(key + adductKey)) continue;
                        
                        usedKeys.Add(key + adductKey);
                        
                        
                        ElementDictionary atomsCount = MS2Fragment.createEmptyElementDict();
                        MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                        string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                        Adduct adduct = Lipid.ALL_ADDUCTS[Lipid.ADDUCT_POSITIONS[adductKey]];
                        string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct);
                        int charge = adduct.charge;
                        MS2Fragment.addCounts(atomsCount, adduct.elements);
                        double mass = LipidCreator.computeMass(atomsCount, charge);
                                        
                        PrecursorData precursorData = new PrecursorData();
                        precursorData.lipidCategory = LipidCategory.Sterollipid;
                        precursorData.moleculeListName = lipidSpecies.get_lipid_string(csgoslin.LipidLevel.CLASS);
                        precursorData.fullMoleculeListName = headgroup;
                        precursorData.precursorExportName = lipidSpecies.get_lipid_string();
                        precursorData.precursorName = lipidSpecies.get_lipid_string();
                        precursorData.precursorSpeciesName = speciesName;
                        precursorData.precursorIonFormula = chemForm;
                        precursorData.precursorAdduct = adduct;
                        precursorData.precursorAdductFormula = adductForm;
                        precursorData.precursorM_Z = mass;
                        precursorData.fa1 = null;
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
                            
                            if (!headgroups[heavyHeadgroup].adductRestrictions[adductKey]) continue;
                
                            ElementDictionary heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                            MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                            string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                            string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct);
                            MS2Fragment.addCounts(heavyAtomsCount, adduct.elements);
                            double heavyMass = LipidCreator.computeMass(heavyAtomsCount, charge);
                                        
                            
                            string heavyKey = LipidCreator.precursorNameSplit(heavyHeadgroup)[0] + LipidCreator.computeHeavyIsotopeLabel(heavyAtomsCount);
                                                                

                            PrecursorData heavyPrecursorData = new PrecursorData();
                            heavyPrecursorData.lipidCategory = LipidCategory.Sterollipid;
                            heavyPrecursorData.moleculeListName = headgroup;
                            heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                            heavyPrecursorData.precursorExportName = headgroup;
                            heavyPrecursorData.precursorName = heavyKey;
                            heavyPrecursorData.precursorSpeciesName = heavyKey;
                            heavyPrecursorData.precursorIonFormula = heavyChemForm;
                            heavyPrecursorData.precursorAdduct = adduct;
                            heavyPrecursorData.precursorAdductFormula = heavyAdductForm;
                            heavyPrecursorData.precursorM_Z = heavyMass;
                            heavyPrecursorData.fa1 = null;
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
    }
}
