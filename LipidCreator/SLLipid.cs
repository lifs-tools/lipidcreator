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
        public List<string> headGroupNames = new List<string>{"Cer", "CerP", "GB3Cer", "GB4Cer", "GD3Cer", "GM3Cer", "GM4Cer", "HexCer", "HexCerS", "LacCer", "MIPCer", "MIP2Cer", "PECer", "PICer", "SM", "SPC", "SPH", "SPH-P"};
        public List<int> hgValues;
        public FattyAcidGroup fag;
        public FattyAcidGroup lcb;       
        public int longChainBaseHydroxyl;        
        public int fattyAcidHydroxyl;
    
        public SLLipid(Dictionary<String, String> allPaths, Dictionary<String, ArrayList> allFragments)
        {
            lcb = new FattyAcidGroup();
            fag = new FattyAcidGroup();
            hgValues = new List<int>();
            longChainBaseHydroxyl = 2;
            fattyAcidHydroxyl = 0;
            MS2Fragments.Add("Cer", new ArrayList());
            MS2Fragments.Add("CerP", new ArrayList());
            MS2Fragments.Add("GB3Cer", new ArrayList());
            MS2Fragments.Add("GB4Cer", new ArrayList());
            MS2Fragments.Add("GD3Cer", new ArrayList());
            MS2Fragments.Add("GM3Cer", new ArrayList());
            MS2Fragments.Add("GM4Cer", new ArrayList());
            MS2Fragments.Add("HexCer", new ArrayList());
            MS2Fragments.Add("HexCerS", new ArrayList());
            MS2Fragments.Add("LacCer", new ArrayList());
            MS2Fragments.Add("MIPCer", new ArrayList());
            MS2Fragments.Add("MIP2Cer", new ArrayList());
            MS2Fragments.Add("PECer", new ArrayList());
            MS2Fragments.Add("PICer", new ArrayList());
            MS2Fragments.Add("SM", new ArrayList());
            MS2Fragments.Add("SPC", new ArrayList());
            MS2Fragments.Add("SPH", new ArrayList());
            MS2Fragments.Add("SPH-P", new ArrayList());
            adducts["+H"] = true;
            adducts["-H"] = false;
            
            
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
    
        public SLLipid(SLLipid copy) : base((Lipid)copy)
        {
            lcb = new FattyAcidGroup(copy.lcb);
            fag = new FattyAcidGroup(copy.fag);
            longChainBaseHydroxyl = copy.longChainBaseHydroxyl;
            fattyAcidHydroxyl = copy.fattyAcidHydroxyl;
            hgValues = new List<int>();
            foreach (int hgValue in copy.hgValues)
            {
                hgValues.Add(hgValue);
            }
        }
        
        
        public override string serialize()
        {
            string xml = "<lipid type=\"SL\">\n";
            xml += lcb.serialize();
            xml += fag.serialize();
            xml += "<lcbHydroxyValue>" + longChainBaseHydroxyl + "</lcbHydroxyValue>\n";
            xml += "<faHydroxyValue>" + fattyAcidHydroxyl + "</faHydroxyValue>\n";
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
            foreach (int longChainBaseLength in lcb.carbonCounts)
            {
                int maxDoubleBond1 = (longChainBaseLength - 1) >> 1;
                foreach (int longChainBaseDoubleBond in lcb.doubleBondCounts)
                {
                    if (maxDoubleBond1 < longChainBaseDoubleBond) continue;
                    FattyAcid lcbType = new FattyAcid(longChainBaseLength, longChainBaseDoubleBond, longChainBaseHydroxyl, true);
                    foreach (int hgValue in hgValues)
                    {
                        String headgroup = headGroupNames[hgValue];
                        if (headgroup != "SPH" && headgroup != "SPH-P" && headgroup != "SPC") // sphingolipids without fatty acid
                        {
                            foreach (int fattyAcidLength in fag.carbonCounts)
                            {
                                if (fattyAcidLength < fattyAcidHydroxyl + 2) continue;
                                int maxDoubleBond2 = (fattyAcidLength - 1) >> 1;
                                foreach (int fattyAcidDoubleBond2 in fag.doubleBondCounts)
                                {
                                    if (maxDoubleBond2 < fattyAcidDoubleBond2) continue;
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
                                                
                                                
                                                foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                                {
                                                    if (fragment.fragmentSelected && ((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                                    {
                                                        DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                        foreach (string fbase in fragment.fragmentBase)
                                                        {
                                                            switch(fbase)
                                                            {
                                                                case "LCB":
                                                                    MS2Fragment.addCounts(atomsCountFragment, lcbType.atomsCount);
                                                                    break;
                                                                case "FA":
                                                                    MS2Fragment.addCounts(atomsCountFragment, fa.atomsCount);
                                                                    break;
                                                                case "PRE":
                                                                    MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                        // some exceptional if conditions
                                                        if (adduct.Key != "-H" && charge < 0 && (headgroup == "HexCer" || headgroup == "LacCer") && (fragment.fragmentName == "Y0" || fragment.fragmentName == "Y1" || fragment.fragmentName == "Z0" || fragment.fragmentName == "Z1"))
                                                        {
                                                            subtractAdduct(atomsCountFragment, adduct.Key);
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
                                        //String chemFormComplete = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                        double mass = LipidCreatorForm.computeMass(atomsCount, charge);
                                        
                                        foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                        {
                                            // Special cases that are to few to be put in own handling, thus added here as if condidions
                                            if (headgroup == "SPH" && longChainBaseDoubleBond > 0 && fragment.fragmentName == "HG") continue;
                                        
                                        
                                            if (fragment.fragmentSelected && ((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                            {
                                                DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                foreach (string fbase in fragment.fragmentBase)
                                                {
                                                    switch(fbase)
                                                    {
                                                        case "LCB":
                                                            MS2Fragment.addCounts(atomsCountFragment, lcbType.atomsCount);
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
                                                                                                                
                                                String replicatesKey = chemForm + "/" + chemFormFragment;
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
        
        
        public override void addSpectrum(SQLiteCommand command, Dictionary<String, DataTable> headGroupsTable, HashSet<String> usedKeys)
        {
            String sql;
            foreach (int longChainBaseLength in lcb.carbonCounts)
            {
                int maxDoubleBond1 = (longChainBaseLength - 1) >> 1;
                foreach (int longChainBaseDoubleBond in lcb.doubleBondCounts)
                {
                    if (maxDoubleBond1 < longChainBaseDoubleBond) continue;
                    FattyAcid lcbType = new FattyAcid(longChainBaseLength, longChainBaseDoubleBond, longChainBaseHydroxyl, true);
                    foreach (int hgValue in hgValues)
                    {
                        String headgroup = headGroupNames[hgValue];
                        if (headgroup != "SPH" && headgroup != "SPH-P" && headgroup != "SPC") // sphingolipids without fatty acid
                        {
                            foreach (int fattyAcidLength in fag.carbonCounts)
                            {
                                if (fattyAcidLength < fattyAcidHydroxyl + 2) continue;
                                int maxDoubleBond2 = (fattyAcidLength - 1) >> 1;
                                foreach (int fattyAcidDoubleBond2 in fag.doubleBondCounts)
                                {
                                    if (maxDoubleBond2 < fattyAcidDoubleBond2) continue;
                                    FattyAcid fa = new FattyAcid(fattyAcidLength, fattyAcidDoubleBond2, fattyAcidHydroxyl, "FA");
                        
                        
                                    String key = headgroup + " ";
                                    
                                    key += Convert.ToString(longChainBaseLength) + ":" + Convert.ToString(longChainBaseDoubleBond) + ";" + Convert.ToString(longChainBaseHydroxyl);
                                    key += "/";
                                    key += Convert.ToString(fattyAcidLength) + ":" + Convert.ToString(fattyAcidDoubleBond2);
                                    if (fattyAcidHydroxyl > 0) key += ";" + Convert.ToString(fattyAcidHydroxyl);
                                    

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
                                                MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                                MS2Fragment.addCounts(atomsCount, fa.atomsCount);
                                                MS2Fragment.addCounts(atomsCount, lcbType.atomsCount);
                                                String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                                // do not change the order, chem formula must be computed before adding the adduct
                                                int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                                double mass = LipidCreatorForm.computeMass(atomsCount, charge) / (double)(Math.Abs(charge));                                                
                                                
                                                ArrayList valuesMZ = new ArrayList();
                                                ArrayList valuesIntensity = new ArrayList();
                                                
                                                foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                                {
                                                    if (((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                                    {
                                                        DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                        foreach (string fbase in fragment.fragmentBase)
                                                        {
                                                            switch(fbase)
                                                            {
                                                                case "LCB":
                                                                    MS2Fragment.addCounts(atomsCountFragment, lcbType.atomsCount);
                                                                    break;
                                                                case "FA":
                                                                    MS2Fragment.addCounts(atomsCountFragment, fa.atomsCount);
                                                                    break;
                                                                case "PRE":
                                                                    MS2Fragment.addCounts(atomsCountFragment, atomsCount);
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                        // some exceptional if conditions
                                                        if (adduct.Key != "-H" && charge < 0 && (headgroup == "HexCer" || headgroup == "LacCer") && (fragment.fragmentName == "Y0" || fragment.fragmentName == "Y1" || fragment.fragmentName == "Z0" || fragment.fragmentName == "Z1"))
                                                        {
                                                            subtractAdduct(atomsCountFragment, adduct.Key);
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
                                                for(int i = 0; i < numFragments; ++i)
                                                {
                                                    valuesMZArray[i] = (double)valuesMZ[i];
                                                    valuesIntens[i] = 100 * (float)((double)valuesIntensity[i]);
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
                        else
                        {
                            String key = headgroup + " " + Convert.ToString(longChainBaseLength) + ":" + Convert.ToString(longChainBaseDoubleBond) + ";" + Convert.ToString(longChainBaseHydroxyl);

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
                                        MS2Fragment.addCounts(atomsCount, headGroupsTable[headgroup]);
                                        MS2Fragment.addCounts(atomsCount, lcbType.atomsCount);
                                        String chemForm = LipidCreatorForm.computeChemicalFormula(atomsCount);
                                        // do not change the order, chem formula must be computed before adding the adduct
                                        int charge = getChargeAndAddAdduct(atomsCount, adduct.Key);
                                        double mass = LipidCreatorForm.computeMass(atomsCount, charge) / (double)(Math.Abs(charge));                                                
                                        
                                        ArrayList valuesMZ = new ArrayList();
                                        ArrayList valuesIntensity = new ArrayList();
                                        
                                        foreach (MS2Fragment fragment in MS2Fragments[headgroup])
                                        {
                                            // Special cases that are to few to be put in own handling, thus added here as if condidions
                                            if (headgroup == "SPH" && longChainBaseDoubleBond > 0 && fragment.fragmentName == "HG") continue;
                                        
                                        
                                            if (((charge < 0 && fragment.fragmentCharge < 0) || (charge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(adduct.Key)))
                                            {
                                                DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                                                foreach (string fbase in fragment.fragmentBase)
                                                {
                                                    switch(fbase)
                                                    {
                                                        case "LCB":
                                                            MS2Fragment.addCounts(atomsCountFragment, lcbType.atomsCount);
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
                                        for(int i = 0; i < numFragments; ++i)
                                        {
                                            valuesMZArray[i] = (double)valuesMZ[i];
                                            valuesIntens[i] = 100 * (float)((double)valuesIntensity[i]);
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