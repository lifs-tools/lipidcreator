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
        
        
        public override string serialize()
        {
            string xml = "<lipid type=\"SL\" isLyso=\"" + isLyso + "\">\n";
            xml += lcb.serialize();
            xml += fag.serialize();
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
                            

                            
                            foreach (KeyValuePair<string, bool> adduct in adducts)
                            {
                                if (!adduct.Value || !headgroups[headgroup].adductRestrictions[adduct.Key]) continue;
                                if (usedKeys.Contains(headgroup + key + adduct.Key)) continue;
                                
                                usedKeys.Add(headgroup + key + adduct.Key);
                                
                                ElementDictionary atomsCount = MS2Fragment.createEmptyElementDict();
                                MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                                MS2Fragment.addCounts(atomsCount, fa.atomsCount);
                                MS2Fragment.addCounts(atomsCount, lcbType.atomsCount);
                                // do not change the order, chem formula must be computed before adding the adduct
                                string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                                string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct.Key);
                                int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                double mass = LipidCreator.computeMass(atomsCount, charge);
                            
                                PrecursorData precursorData = new PrecursorData();
                                precursorData.lipidCategory = LipidCategory.Sphingolipid;
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
                                    
                                    if (!headgroups[heavyHeadgroup].adductRestrictions[adduct.Key]) continue;
                                
                                    FattyAcid heavyFA = new FattyAcid(fa);
                                    FattyAcid heavyLCB = new FattyAcid(lcbType);
                                    heavyLCB.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[0]);
                                    heavyFA.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[1]);
                        
                                    ElementDictionary heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyLCB.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                    string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                    string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct.Key);
                                    int heavyCharge = getChargeAndAddAdduct(heavyAtomsCount, adduct.Key);
                                    double heavyMass = LipidCreator.computeMass(heavyAtomsCount, heavyCharge);
                                    
                                    
                                    string heavyKey = LipidCreator.precursorNameSplit(heavyHeadgroup)[0] + LipidCreator.computeHeavyIsotopeLabel(heavyAtomsCount);
                                                                        

                                    PrecursorData heavyPrecursorData = new PrecursorData();
                                    heavyPrecursorData.lipidCategory = LipidCategory.Sphingolipid;
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
                                    heavyPrecursorData.fa1 = heavyFA;
                                    heavyPrecursorData.fa2 = null;
                                    heavyPrecursorData.fa3 = null;
                                    heavyPrecursorData.fa4 = null;
                                    heavyPrecursorData.lcb = heavyLCB;
                                    heavyPrecursorData.addPrecursor = (onlyPrecursors != 0);
                                    heavyPrecursorData.fragmentNames = (onlyPrecursors != 1) ? ((heavyCharge > 0) ? positiveFragments[heavyHeadgroup] : negativeFragments[heavyHeadgroup]) : new HashSet<string>();
                                    
                                    precursorDataList.Add(heavyPrecursorData);
                                }
                            }
                        }
                    }
                    else
                    {
                        String key = " ";
                        key += Convert.ToString(lcbType.length) + ":" + Convert.ToString(lcbType.db) + ";" + Convert.ToString(lcbType.hydroxyl);
                        
                        foreach (KeyValuePair<string, bool> adduct in adducts)
                        {
                            if (!adduct.Value || !headgroups[headgroup].adductRestrictions[adduct.Key]) continue;
                            if (usedKeys.Contains(headgroup + key + adduct.Key)) continue;
                            
                            usedKeys.Add(headgroup + key + adduct.Key);
                            
                            ElementDictionary atomsCount = MS2Fragment.createEmptyElementDict();
                            MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                            MS2Fragment.addCounts(atomsCount, lcbType.atomsCount);
                            // do not change the order, chem formula must be computed before adding the adduct
                            string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                            string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct.Key);
                            int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                            double mass = LipidCreator.computeMass(atomsCount, charge);
                                    
                                
                            PrecursorData precursorData = new PrecursorData();
                            precursorData.lipidCategory = LipidCategory.Sphingolipid;
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
                                
                                if (!headgroups[heavyHeadgroup].adductRestrictions[adduct.Key]) continue;
                            
                                FattyAcid heavyLCB = new FattyAcid(lcbType);
                                heavyLCB.updateForHeavyLabeled((ElementDictionary)heavyPrecursor.userDefinedFattyAcids[0]);
                    
                                ElementDictionary heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                MS2Fragment.addCounts(heavyAtomsCount, heavyLCB.atomsCount);
                                MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct.Key);
                                int heavyCharge = getChargeAndAddAdduct(heavyAtomsCount, adduct.Key);
                                double heavyMass = LipidCreator.computeMass(heavyAtomsCount, heavyCharge);
                                    
                                
                                string heavyKey = LipidCreator.precursorNameSplit(heavyHeadgroup)[0] + LipidCreator.computeHeavyIsotopeLabel(heavyAtomsCount);
                                                                    

                                PrecursorData heavyPrecursorData = new PrecursorData();
                                heavyPrecursorData.lipidCategory = LipidCategory.Sphingolipid;
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
                                heavyPrecursorData.fa1 = null;
                                heavyPrecursorData.fa2 = null;
                                heavyPrecursorData.fa3 = null;
                                heavyPrecursorData.fa4 = null;
                                heavyPrecursorData.lcb = heavyLCB;
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
