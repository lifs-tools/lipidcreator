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
    public class Cholesterol : Lipid
    {
        public bool containsEster;
        public FattyAcidGroup fag;
        public List<String> headGroupNames = new List<String>{"Ch", "ChE"};
    
    
        public Cholesterol(Dictionary<String, String> allPaths, Dictionary<String, ArrayList> allFragments)
        {
            fag = new FattyAcidGroup();
            containsEster = false;
            MS2Fragments.Add("Ch", new ArrayList());
            MS2Fragments.Add("ChE", new ArrayList());
            adducts["+NH4"] = true;
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
    
        public Cholesterol(Cholesterol copy) : base((Lipid)copy) 
        {
            fag = new FattyAcidGroup(copy.fag);
            containsEster = copy.containsEster;
            
        }
        
        
        public override string serialize()
        {
            string xml = "<lipid type=\"Cholesterol\" containsEster=\"" + containsEster + "\">\n";
            xml += fag.serialize();
            xml += base.serialize();
            xml += "</lipid>\n";
            return xml;
        }
        
        
        public override void import(XElement node)
        {
            int fattyAcidCounter = 0;
            containsEster = node.Attribute("containsEster").Value == "True";
            foreach (XElement child in node.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "FattyAcidGroup":
                        if (fattyAcidCounter == 0)
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
                        
                    default:
                        base.import(child);
                        break;
                }
            }
        }
        
        
    
        
        public override void addLipids(DataTable allLipids, DataTable allLipidsUnique, Dictionary<String, DataTable> headGroupsTable, Dictionary<String, Dictionary<String, bool>> headgroupAdductRestrictions, HashSet<String> usedKeys, HashSet<String> replicates)
        {
            if (containsEster)
            {
                foreach (int fattyAcidLength in fag.carbonCounts)
                {
                    int maxDoubleBond = (fattyAcidLength - 1) >> 1;
                    foreach (int fattyAcidDoubleBond in fag.doubleBondCounts)
                    {
                        foreach (int fattyAcidHydroxyl in fag.hydroxylCounts)
                        {
                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair in fag.faTypes)
                            {
                                if (fattyAcidKeyValuePair.Value && maxDoubleBond >= fattyAcidDoubleBond)
                                {
                                    FattyAcid fa = new FattyAcid(fattyAcidLength, fattyAcidDoubleBond, fattyAcidHydroxyl, fattyAcidKeyValuePair.Key);
                                    
                                    String headgroup = "ChE";
                                    String key = headgroup + " ";
                                    key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                    if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                    key += fa.suffix;
                                    if (!usedKeys.Contains(key))
                                    {
                                    
                                        foreach (KeyValuePair<string, bool> adduct in adducts)
                                        {
                                            if (adduct.Value && headgroupAdductRestrictions[headgroup][adduct.Key])
                                            {
                                                usedKeys.Add(key);
                                                
                                                DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                                                MS2Fragment.addCounts(atomsCount, fa.atomsCount);
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
            else
            {
                String headgroup = "Ch";
                String key = headgroup + " ";
                if (!usedKeys.Contains(key))
                {
                
                    foreach (KeyValuePair<string, bool> adduct in adducts)
                    {
                        if (adduct.Value && headgroupAdductRestrictions[headgroup][adduct.Key])
                        {
                            usedKeys.Add(key);
                            
                            DataTable atomsCount = MS2Fragment.createEmptyElementTable();
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
        
        public override void addSpectrum(SQLiteCommand command, Dictionary<String, DataTable> headGroupsTable, HashSet<String> usedKeys)
        {
        string sql;                
        if (containsEster){                
                foreach (int fattyAcidLength in fag.carbonCounts)
                {
                    int maxDoubleBond = (fattyAcidLength - 1) >> 1;
                    foreach (int fattyAcidDoubleBond in fag.doubleBondCounts)
                    {
                        foreach (int fattyAcidHydroxyl in fag.hydroxylCounts)
                        {
                            foreach (KeyValuePair<string, bool> fattyAcidKeyValuePair in fag.faTypes)
                            {
                                if (fattyAcidKeyValuePair.Value && maxDoubleBond >= fattyAcidDoubleBond)
                                {
                                    FattyAcid fa = new FattyAcid(fattyAcidLength, fattyAcidDoubleBond, fattyAcidHydroxyl, fattyAcidKeyValuePair.Key);
                                    
                                    String headgroup = "ChE";
                                    String key = headgroup + " ";
                                    key += Convert.ToString(fa.length) + ":" + Convert.ToString(fa.db);
                                    if (fa.hydroxyl > 0) key += ";" + Convert.ToString(fa.hydroxyl);
                                    key += fa.suffix;
                                    
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
                                                MS2Fragment.addCounts(atomsCount, fa.atomsCount);
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
            else
            {
                String headgroup = "Ch";
                String key = headgroup + " ";
                
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