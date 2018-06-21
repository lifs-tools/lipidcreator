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
        
    
    
        public GLLipid(LipidCreator lipidCreator) : base(lipidCreator, LipidCategory.GlyceroLipid)
        {
            fag1 = new FattyAcidGroup();
            fag2 = new FattyAcidGroup();
            fag3 = new FattyAcidGroup();
            containsSugar = false;
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
        
        public override ArrayList getFattyAcidGroupList()
        {
            return new ArrayList{fag1, fag2, fag3};
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
        
        // synchronize the fragment list with list from LipidCreator root
        public override void Update(object sender, EventArgs e)
        {
            Updating((int)LipidCategory.GlyceroLipid);
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
        
        
        public override void computePrecursorData(Dictionary<String, Precursor> headgroups, HashSet<String> usedKeys, ArrayList precursorDataList)
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
                                    if (i++ > 0) key += ID_SEPARATOR_UNSPECIFIC;
                                    key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                    if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                    key += fa.suffix;
                                }
                            }
                            
                            if (usedKeys.Contains(key)) continue;
                            
                            
                            foreach (KeyValuePair<string, bool> adduct in adducts)
                            {
                                if (!adduct.Value || !headgroups[headgroup].adductRestrictions[adduct.Key]) continue;
                                
                                usedKeys.Add(key);
                                
                                Dictionary<int, int> atomsCount = MS2Fragment.createEmptyElementDict();
                                MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                                string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                                string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct.Key);
                                int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                double mass = LipidCreator.computeMass(atomsCount, charge);
                                                                                                                    
                                
                                PrecursorData precursorData = new PrecursorData();
                                precursorData.lipidCategory = LipidCategory.GlyceroLipid;
                                precursorData.moleculeListName = headgroup;
                                precursorData.fullMoleculeListName = headgroup;
                                precursorData.lipidClass = headgroup;
                                precursorData.precursorName = key;
                                precursorData.precursorIonFormula = chemForm;
                                precursorData.precursorAdduct = adduct.Key;
                                precursorData.precursorAdductFormula = adductForm;
                                precursorData.precursorM_Z = mass / (double)(Math.Abs(charge));
                                precursorData.precursorCharge = charge;
                                precursorData.atomsCount = headgroups[headgroup].elements;
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
                                    
                                    if (!headgroups[heavyHeadgroup].adductRestrictions[adduct.Key]) continue;
                                    
                                    string suffix = heavyHeadgroup.Split(new Char[]{'/'})[1];
                                    string heavyKey = key + HEAVY_LABEL_SEPARATOR + suffix;
                                    
                                    FattyAcid heavyFA1 = new FattyAcid(fa1);
                                    FattyAcid heavyFA2 = new FattyAcid(fa2);
                                    heavyFA1.updateForHeavyLabeled((Dictionary<int, int>)heavyPrecursor.userDefinedFattyAcids[0]);
                                    heavyFA2.updateForHeavyLabeled((Dictionary<int, int>)heavyPrecursor.userDefinedFattyAcids[1]);
                                    List<FattyAcid> heavySortedAcids = new List<FattyAcid>();
                                    heavySortedAcids.Add(heavyFA1);
                                    heavySortedAcids.Add(heavyFA2);
                                    heavySortedAcids.Sort();
                        
                                    Dictionary<int, int> heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA1.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA2.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                    string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                    string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct.Key);
                                    int heavyCharge = getChargeAndAddAdduct(heavyAtomsCount, adduct.Key);
                                    double heavyMass = LipidCreator.computeMass(heavyAtomsCount, heavyCharge);
                                                                        

                                    PrecursorData heavyPrecursorData = new PrecursorData();
                                    heavyPrecursorData.lipidCategory = LipidCategory.GlyceroLipid;
                                    heavyPrecursorData.moleculeListName = headgroup;
                                    heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                                    heavyPrecursorData.lipidClass = heavyHeadgroup;
                                    heavyPrecursorData.precursorName = heavyKey;
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
                                    headgroup = "TAG";
                                    break;
                                case 4:
                                case 2:
                                case 1:
                                    headgroup = "DAG";
                                    break;
                                case 6:
                                case 5:
                                case 3:
                                    headgroup = "MAG";
                                    break;
                            }
                            String key = headgroup + " ";
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
                            
                            if (usedKeys.Contains(key)) continue;
                            
                            foreach (KeyValuePair<string, bool> adduct in adducts)
                            {
                                if (!adduct.Value || !headgroups[headgroup].adductRestrictions[adduct.Key]) continue;
                                
                                usedKeys.Add(key);
                                
                                Dictionary<int, int> atomsCount = MS2Fragment.createEmptyElementDict();
                                MS2Fragment.addCounts(atomsCount, fa1.atomsCount);
                                MS2Fragment.addCounts(atomsCount, fa2.atomsCount);
                                MS2Fragment.addCounts(atomsCount, fa3.atomsCount);
                                MS2Fragment.addCounts(atomsCount, headgroups[headgroup].elements);
                                string chemForm = LipidCreator.computeChemicalFormula(atomsCount);
                                string adductForm = LipidCreator.computeAdductFormula(atomsCount, adduct.Key);
                                int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                double mass = LipidCreator.computeMass(atomsCount, charge);
                                                                                                                    
                                
                                PrecursorData precursorData = new PrecursorData();
                                precursorData.lipidCategory = LipidCategory.GlyceroLipid;
                                precursorData.moleculeListName = headgroup;
                                precursorData.fullMoleculeListName = headgroup;
                                precursorData.lipidClass = headgroup;
                                precursorData.precursorName = key;
                                precursorData.precursorIonFormula = chemForm;
                                precursorData.precursorAdduct = adduct.Key;
                                precursorData.precursorAdductFormula = adductForm;
                                precursorData.precursorM_Z = mass / (double)(Math.Abs(charge));
                                precursorData.precursorCharge = charge;
                                precursorData.atomsCount = headgroups[headgroup].elements;
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
                                    
                                    if (!headgroups[heavyHeadgroup].adductRestrictions[adduct.Key]) continue;
                                    
                                    string suffix = heavyHeadgroup.Split(new Char[]{'/'})[1];
                                    string heavyKey = key + HEAVY_LABEL_SEPARATOR + suffix;
                                        
                                    FattyAcid heavyFA1 = new FattyAcid(fa1);
                                    FattyAcid heavyFA2 = new FattyAcid(fa2);
                                    FattyAcid heavyFA3 = new FattyAcid(fa3);
                                    heavyFA1.updateForHeavyLabeled((Dictionary<int, int>)heavyPrecursor.userDefinedFattyAcids[0]);
                                    if (headgroup.Equals("DAG") || headgroup.Equals("TAG")) heavyFA2.updateForHeavyLabeled((Dictionary<int, int>)heavyPrecursor.userDefinedFattyAcids[1]);
                                    if (headgroup.Equals("TAG")) heavyFA3.updateForHeavyLabeled((Dictionary<int, int>)heavyPrecursor.userDefinedFattyAcids[2]);
                                    List<FattyAcid> heavySortedAcids = new List<FattyAcid>();
                                    heavySortedAcids.Add(heavyFA1);
                                    heavySortedAcids.Add(heavyFA2);
                                    heavySortedAcids.Add(heavyFA3);
                                    heavySortedAcids.Sort();
                        
                                    Dictionary<int, int> heavyAtomsCount = MS2Fragment.createEmptyElementDict();
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA1.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA2.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, heavyFA3.atomsCount);
                                    MS2Fragment.addCounts(heavyAtomsCount, headgroups[heavyHeadgroup].elements);
                                    string heavyChemForm = LipidCreator.computeChemicalFormula(heavyAtomsCount);
                                    string heavyAdductForm = LipidCreator.computeAdductFormula(heavyAtomsCount, adduct.Key);
                                    int heavyCharge = getChargeAndAddAdduct(heavyAtomsCount, adduct.Key);
                                    double heavyMass = LipidCreator.computeMass(heavyAtomsCount, heavyCharge);
                                                                        

                                    PrecursorData heavyPrecursorData = new PrecursorData();
                                    heavyPrecursorData.lipidCategory = LipidCategory.GlyceroLipid;
                                    heavyPrecursorData.moleculeListName = headgroup;
                                    heavyPrecursorData.fullMoleculeListName = heavyHeadgroup;
                                    heavyPrecursorData.lipidClass = heavyHeadgroup;
                                    heavyPrecursorData.precursorName = heavyKey;
                                    heavyPrecursorData.precursorIonFormula = heavyChemForm;
                                    heavyPrecursorData.precursorAdduct = adduct.Key;
                                    heavyPrecursorData.precursorAdductFormula = heavyAdductForm;
                                    heavyPrecursorData.precursorM_Z = heavyMass / (double)(Math.Abs(heavyCharge));
                                    heavyPrecursorData.precursorCharge = heavyCharge;
                                    heavyPrecursorData.atomsCount = headgroups[heavyHeadgroup].elements;
                                    heavyPrecursorData.fa1 = heavySortedAcids[0];
                                    heavyPrecursorData.fa2 = heavySortedAcids[1];
                                    heavyPrecursorData.fa3 = heavySortedAcids[2];
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