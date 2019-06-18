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
    public class Sphingolipid : Lipid
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Sphingolipid));
        public FattyAcidGroup fag;
        public FattyAcidGroup lcb;
        public bool isLyso;
    
        public Sphingolipid(LipidCreator lipidCreator) : base(lipidCreator, LipidCategory.Sphingolipid)
        {
            lcb = new FattyAcidGroup(true);
            fag = new FattyAcidGroup();
            lcb.hydroxylCounts.Add(2);
            fag.hydroxylCounts.Add(0);
            isLyso = false;
            adducts["+H"] = true;
            adducts["-H"] = false;
        }
    
        public Sphingolipid(Sphingolipid copy) : base((Lipid)copy)
        {
            lcb = new FattyAcidGroup(copy.lcb);
            fag = new FattyAcidGroup(copy.fag);
            isLyso = copy.isLyso;
        }
        
        public override ArrayList getFattyAcidGroupList()
        {
            return new ArrayList{fag};
        }
        
        
        
        
        public override long getHashCode()
        {
            long hashCode = base.getHashCode() + 6930302729454L;
            hashCode += lcb.getHashCode();
            hashCode += fag.getHashCode();
            hashCode += isLyso ? (1L << 12) : (1L << 54);
            if (hashCode == 0) hashCode += 1;
            return hashCode;
        }
        
        
        public override void serialize(StringBuilder sb)
        {
            sb.Append("<lipid type=\"SL\" isLyso=\"" + isLyso + "\">\n");
            lcb.serialize(sb);
            fag.serialize(sb);
            base.serialize(sb);
            sb.Append("</lipid>\n");
        }
        
        // synchronize the fragment list with list from LipidCreator root
        public override void Update(object sender, EventArgs e)
        {
            Updating((int)LipidCategory.Sphingolipid);
        }
        
        public override void import(XElement node, string importVersion)
        {
            int fattyAcidCounter = 0;
            isLyso = node.Attribute("isLyso").Value == "True";
            headGroupNames.Clear();
            foreach (XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "FattyAcidGroup":
                        if (fattyAcidCounter == 0)
                        {
                            lcb.import(child, importVersion);
                        }
                        else if (fattyAcidCounter == 1)
                        {
                            fag.import(child, importVersion);
                        }
                        else
                        {   
                            log.Error("A sphingolipid can have at most 2 fatty acid chains. Found: " + (fattyAcidCounter + 1) + "");
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
            foreach (FattyAcid lcbType in lcb.getFattyAcids())
            {
                foreach (string headgroup in headGroupNames)
                {
                 
                    if (!isLyso) // sphingolipids with fatty acid
                    {
                        foreach (FattyAcid fa in fag.getFattyAcids())
                        {
                    
                            String key = " ";
                            key += Convert.ToString(lcbType.length) + ":" + Convert.ToString(lcbType.db) + ";" + Convert.ToString(lcbType.hydroxyl);
                            key += ID_SEPARATOR_SPECIFIC;                            
                            key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                            if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                            

                            
                            foreach (string adductKey in adducts.Keys.Where(x => adducts[x]))
                            {
                                if (!headgroups[headgroup].adductRestrictions[adductKey]) continue;
                                if (usedKeys.Contains(headgroup + key + adductKey)) continue;
                                
                                usedKeys.Add(headgroup + key + adductKey);
                                
                                ElementDictionary atomsCount = MS2Fragment.createEmptyElementDict();
                                MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                                MS2Fragment.addCounts(atomsCount, fa.atomsCount);
                                MS2Fragment.addCounts(atomsCount, lcbType.atomsCount);
                                // do not change the order, chem formula must be computed before adding the adduct
                                string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                                Adduct adduct = Lipid.ALL_ADDUCTS[Lipid.ADDUCT_POSITIONS[adductKey]];
                                string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct);
                                int charge = adduct.charge;
                                MS2Fragment.addCounts(atomsCount, adduct.elements);
                                double mass = LipidCreator.computeMass(atomsCount, charge);
                                
                                // species name
                                FattyAcid speciesFA = new FattyAcid(lcbType);
                                speciesFA.merge(fa);
                                string speciesName = headgroup + " " + speciesFA.ToString();
                            
                                PrecursorData precursorData = new PrecursorData();
                                precursorData.lipidCategory = LipidCategory.Sphingolipid;
                                precursorData.moleculeListName = headgroup;
                                precursorData.fullMoleculeListName = headgroup;
                                precursorData.precursorExportName = headgroup + key;
                                precursorData.precursorName = headgroup + key;
                                precursorData.precursorSpeciesName = speciesName;
                                precursorData.precursorIonFormula = chemForm;
                                precursorData.precursorAdduct = adduct;
                                precursorData.precursorAdductFormula = adductForm;
                                precursorData.precursorM_Z = mass;
                                precursorData.fa1 = fa;
                                precursorData.fa2 = null;
                                precursorData.fa3 = null;
                                precursorData.fa4 = null;
                                precursorData.lcb = lcbType;
                                precursorData.addPrecursor = (onlyPrecursors != 0);
                                precursorData.fragmentNames = (onlyPrecursors != 1) ? ((charge > 0) ? positiveFragments[headgroup] : negativeFragments[headgroup]) : new HashSet<string>();
                                
                                if (onlyHeavyLabeled != 1) precursorDataList.Add(precursorData);
                                
                                if (onlyHeavyLabeled == 0) continue;
                                foreach (Precursor heavyPrecursor  in headgroups[headgroup].heavyLabeledPrecursors)
                                {
                                    string heavyHeadgroup = heavyPrecursor.name;
                                    
                                    if (!headgroups[heavyHeadgroup].adductRestrictions[adductKey]) continue;
                                
                                    FattyAcid heavyFA = new FattyAcid(fa);
                                    FattyAcid heavyLCB = new FattyAcid(lcbType);
                                    heavyLCB.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[0]);
                                    heavyFA.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[1]);
                        
                                    ElementDictionary heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyLCB.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                    string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                    string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct);
                                    MS2Fragment.addCounts(heavyAtomsCount, adduct.elements);
                                    double heavyMass = LipidCreator.computeMass(heavyAtomsCount, charge);
                                    
                                    
                                    string heavyKey = LipidCreator.precursorNameSplit(heavyHeadgroup)[0] + LipidCreator.computeHeavyIsotopeLabel(heavyAtomsCount);
                                                                        

                                    PrecursorData heavyPrecursorData = new PrecursorData();
                                    heavyPrecursorData.lipidCategory = LipidCategory.Sphingolipid;
                                    heavyPrecursorData.moleculeListName = headgroup;
                                    heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                                    heavyPrecursorData.precursorExportName = headgroup + key;
                                    heavyPrecursorData.precursorName = heavyKey + key;
                                    heavyPrecursorData.precursorSpeciesName = speciesName;
                                    heavyPrecursorData.precursorIonFormula = heavyChemForm;
                                    heavyPrecursorData.precursorAdduct = adduct;
                                    heavyPrecursorData.precursorAdductFormula = heavyAdductForm;
                                    heavyPrecursorData.precursorM_Z = heavyMass;
                                    heavyPrecursorData.fa1 = heavyFA;
                                    heavyPrecursorData.fa2 = null;
                                    heavyPrecursorData.fa3 = null;
                                    heavyPrecursorData.fa4 = null;
                                    heavyPrecursorData.lcb = heavyLCB;
                                    heavyPrecursorData.addPrecursor = (onlyPrecursors != 0);
                                    heavyPrecursorData.fragmentNames = (onlyPrecursors != 1) ? ((charge > 0) ? positiveFragments[heavyHeadgroup] : negativeFragments[heavyHeadgroup]) : new HashSet<string>();
                                    
                                    precursorDataList.Add(heavyPrecursorData);
                                }
                            }
                        }
                    }
                    else
                    {
                        String key = " ";
                        key += Convert.ToString(lcbType.length) + ":" + Convert.ToString(lcbType.db) + ";" + Convert.ToString(lcbType.hydroxyl);
                        
                        foreach (string adductKey in adducts.Keys.Where(x => adducts[x]))
                        {
                            if (!headgroups[headgroup].adductRestrictions[adductKey]) continue;
                            if (usedKeys.Contains(headgroup + key + adductKey)) continue;
                            
                            usedKeys.Add(headgroup + key + adductKey);
                            
                            ElementDictionary atomsCount = MS2Fragment.createEmptyElementDict();
                            MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                            MS2Fragment.addCounts(atomsCount, lcbType.atomsCount);
                            // do not change the order, chem formula must be computed before adding the adduct
                            string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                            Adduct adduct = Lipid.ALL_ADDUCTS[Lipid.ADDUCT_POSITIONS[adductKey]];
                            string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct);
                            int charge = adduct.charge;
                            MS2Fragment.addCounts(atomsCount, adduct.elements);
                            double mass = LipidCreator.computeMass(atomsCount, charge);
                                    
                                
                            PrecursorData precursorData = new PrecursorData();
                            precursorData.lipidCategory = LipidCategory.Sphingolipid;
                            precursorData.moleculeListName = headgroup;
                            precursorData.fullMoleculeListName = headgroup;
                            precursorData.precursorExportName = headgroup + key;
                            precursorData.precursorName = headgroup + key;
                            precursorData.precursorSpeciesName = headgroup + key;
                            precursorData.precursorIonFormula = chemForm;
                            precursorData.precursorAdduct = adduct;
                            precursorData.precursorAdductFormula = adductForm;
                            precursorData.precursorM_Z = mass;
                            precursorData.fa1 = null;
                            precursorData.fa2 = null;
                            precursorData.fa3 = null;
                            precursorData.fa4 = null;
                            precursorData.lcb = lcbType;
                            precursorData.addPrecursor = (onlyPrecursors != 0);
                            precursorData.fragmentNames = (onlyPrecursors != 1) ? ((charge > 0) ? positiveFragments[headgroup] : negativeFragments[headgroup]) : new HashSet<string>();
                            
                            
                            
                            if (onlyHeavyLabeled != 1) precursorDataList.Add(precursorData);
                                
                            if (onlyHeavyLabeled == 0) continue;
                            foreach (Precursor heavyPrecursor  in headgroups[headgroup].heavyLabeledPrecursors)
                            {
                                string heavyHeadgroup = heavyPrecursor.name;
                                
                                if (!headgroups[heavyHeadgroup].adductRestrictions[adductKey]) continue;
                            
                                FattyAcid heavyLCB = new FattyAcid(lcbType);
                                heavyLCB.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[0]);
                    
                                ElementDictionary heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                MS2Fragment.addCounts(heavyAtomsCount, heavyLCB.atomsCount);
                                MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct);
                                MS2Fragment.addCounts(heavyAtomsCount, adduct.elements);
                                double heavyMass = LipidCreator.computeMass(heavyAtomsCount, charge);
                                    
                                
                                string heavyKey = LipidCreator.precursorNameSplit(heavyHeadgroup)[0] + LipidCreator.computeHeavyIsotopeLabel(heavyAtomsCount);
                                                                    

                                PrecursorData heavyPrecursorData = new PrecursorData();
                                heavyPrecursorData.lipidCategory = LipidCategory.Sphingolipid;
                                heavyPrecursorData.moleculeListName = headgroup;
                                heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                                heavyPrecursorData.precursorExportName = headgroup + key;
                                heavyPrecursorData.precursorName = heavyKey + key;
                                heavyPrecursorData.precursorSpeciesName = heavyKey + key;
                                heavyPrecursorData.precursorIonFormula = heavyChemForm;
                                heavyPrecursorData.precursorAdduct = adduct;
                                heavyPrecursorData.precursorAdductFormula = heavyAdductForm;
                                heavyPrecursorData.precursorM_Z = heavyMass;
                                heavyPrecursorData.fa1 = null;
                                heavyPrecursorData.fa2 = null;
                                heavyPrecursorData.fa3 = null;
                                heavyPrecursorData.fa4 = null;
                                heavyPrecursorData.lcb = heavyLCB;
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
