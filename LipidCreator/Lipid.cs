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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Data.SQLite;

namespace LipidCreator
{
    public enum LipidCategory {GlyceroLipid, PhosphoLipid, SphingoLipid, Cholesterol};
    
    [Serializable]
    public class PrecursorData
    {
        public LipidCategory lipidCategory;
        public string moleculeListName;
        public string precursorName;
        public string precursorIonFormula;
        public string precursorAdduct;
        public double precursorM_Z;
        public int precursorCharge;
        public string adduct;
        public DataTable atomsCount;
        public FattyAcid fa1;
        public FattyAcid fa2;
        public FattyAcid fa3;
        public FattyAcid fa4;
        public FattyAcid lcb;
        public string chemFormComplete;
        public ArrayList MS2Fragments;
    }
    
    
    [Serializable]
    public class Lipid
    {
        public string className;
        public Dictionary<String, ArrayList> MS2Fragments;
        public Dictionary<String, String> pathsToFullImage;
        public Dictionary<String, bool> adducts;
        public bool representativeFA;
    
        public Lipid()
        {
            adducts = new Dictionary<String, bool>();
            adducts.Add("+H", false);
            adducts.Add("+2H", false);
            adducts.Add("+NH4", false);
            adducts.Add("-H", true);
            adducts.Add("-2H", false);
            adducts.Add("+HCOO", false);
            adducts.Add("+CH3COO", false);
            MS2Fragments = new Dictionary<String, ArrayList>();
            pathsToFullImage = new Dictionary<String, String>();
            representativeFA = false;
        }
        
        public virtual void computePrecursorData(Dictionary<String, DataTable> headGroupsTable, Dictionary<String, Dictionary<String, bool>> headgroupAdductRestrictions, HashSet<String> usedKeys, ArrayList precursorDataList)
        {
        }
        
        
        
        public static void computeFragmentData(DataTable allLipids, DataTable allLipidsUnique, PrecursorData precursorData, HashSet<String> replicates)
        {
            foreach (MS2Fragment fragment in precursorData.MS2Fragments)
            {
                if (fragment.fragmentSelected && ((precursorData.precursorCharge < 0 && fragment.fragmentCharge < 0) || (precursorData.precursorCharge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(precursorData.adduct)))
                {
                    DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                    foreach (string fbase in fragment.fragmentBase)
                    {
                        switch(fbase)
                        {
                            case "LCB":
                                MS2Fragment.addCounts(atomsCountFragment, precursorData.lcb.atomsCount);
                                break;
                            case "FA":
                            case "FA1":
                                MS2Fragment.addCounts(atomsCountFragment, precursorData.fa1.atomsCount);
                                break;
                            case "FA2":
                                MS2Fragment.addCounts(atomsCountFragment, precursorData.fa2.atomsCount);
                                break;
                            case "FA3":
                                MS2Fragment.addCounts(atomsCountFragment, precursorData.fa3.atomsCount);
                                break;
                            case "FA4":
                                MS2Fragment.addCounts(atomsCountFragment, precursorData.fa4.atomsCount);
                                break;
                            case "PRE":
                                MS2Fragment.addCounts(atomsCountFragment, precursorData.atomsCount);
                                break;
                            default:
                                break;
                        }
                    }
                    // some exceptional if conditions
                    if (precursorData.lipidCategory == LipidCategory.SphingoLipid && precursorData.adduct != "-H" && precursorData.precursorCharge < 0 && (precursorData.moleculeListName == "HexCer" || precursorData.moleculeListName == "LacCer") && (fragment.fragmentName == "Y0" || fragment.fragmentName == "Y1" || fragment.fragmentName == "Z0" || fragment.fragmentName == "Z1"))
                    {
                        Lipid.subtractAdduct(atomsCountFragment, precursorData.adduct);
                    }
                    
                    String chemFormFragment = LipidCreatorForm.computeChemicalFormula(atomsCountFragment);
                    double massFragment = LipidCreatorForm.computeMass(atomsCountFragment, fragment.fragmentCharge);
                    
                    DataRow lipidRow = allLipids.NewRow();
                    lipidRow["Molecule List Name"] = precursorData.moleculeListName;
                    lipidRow["Precursor Name"] = precursorData.precursorName;
                    lipidRow["Precursor Ion Formula"] = precursorData.precursorIonFormula;
                    lipidRow["Precursor Adduct"] = precursorData.precursorAdduct;
                    lipidRow["Precursor m/z"] = precursorData.precursorM_Z;
                    lipidRow["Precursor Charge"] = ((precursorData.precursorCharge > 0) ? "+" : "") + Convert.ToString(precursorData.precursorCharge);
                    lipidRow["Product Name"] = fragment.fragmentName;
                    lipidRow["Product Ion Formula"] = chemFormFragment;
                    lipidRow["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                    lipidRow["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                    allLipids.Rows.Add(lipidRow);
                                        
                    String replicatesKey = precursorData.chemFormComplete + "/" + chemFormFragment;
                    if (!replicates.Contains(replicatesKey))
                    {
                        replicates.Add(replicatesKey);
                        DataRow lipidRowUnique = allLipidsUnique.NewRow();
                        lipidRowUnique["Molecule List Name"] = precursorData.moleculeListName;
                        lipidRowUnique["Precursor Name"] = precursorData.precursorName;
                        lipidRowUnique["Precursor Ion Formula"] = precursorData.precursorIonFormula;
                        lipidRowUnique["Precursor Adduct"] = precursorData.precursorAdduct;
                        lipidRowUnique["Precursor m/z"] = precursorData.precursorM_Z;
                        lipidRowUnique["Precursor Charge"] = ((precursorData.precursorCharge > 0) ? "+" : "") + Convert.ToString(precursorData.precursorCharge);
                        lipidRowUnique["Product Name"] = fragment.fragmentName;
                        lipidRowUnique["Product Ion Formula"] = chemFormFragment;
                        lipidRowUnique["Product m/z"] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                        lipidRowUnique["Product Charge"] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                        allLipidsUnique.Rows.Add(lipidRowUnique);
                    }
                }
            }
        }
        
        
        
        public static void addSpectra(SQLiteCommand command, PrecursorData precursorData)
        {
            ArrayList valuesMZ = new ArrayList();
            ArrayList valuesIntensity = new ArrayList();
            String sql;
            
            foreach (MS2Fragment fragment in precursorData.MS2Fragments)
            {
                if (((precursorData.precursorCharge < 0 && fragment.fragmentCharge < 0) || (precursorData.precursorCharge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(precursorData.adduct)))
                {
                    DataTable atomsCountFragment = MS2Fragment.createEmptyElementTable(fragment.fragmentElements);
                    foreach (string fbase in fragment.fragmentBase)
                    {
                        switch(fbase)
                        {
                            case "LCB":
                                MS2Fragment.addCounts(atomsCountFragment, precursorData.lcb.atomsCount);
                                break;
                            case "FA":
                            case "FA1":
                                MS2Fragment.addCounts(atomsCountFragment, precursorData.fa1.atomsCount);
                                break;
                            case "FA2":
                                MS2Fragment.addCounts(atomsCountFragment, precursorData.fa2.atomsCount);
                                break;
                            case "FA3":
                                MS2Fragment.addCounts(atomsCountFragment, precursorData.fa3.atomsCount);
                                break;
                            case "FA4":
                                MS2Fragment.addCounts(atomsCountFragment, precursorData.fa4.atomsCount);
                                break;
                            case "PRE":
                                MS2Fragment.addCounts(atomsCountFragment, precursorData.atomsCount);
                                break;
                            default:
                                break;
                        }
                    }
                    // some exceptional if conditions
                    if (precursorData.lipidCategory == LipidCategory.SphingoLipid && precursorData.adduct != "-H" && precursorData.precursorCharge < 0 && (precursorData.moleculeListName == "HexCer" || precursorData.moleculeListName == "LacCer") && (fragment.fragmentName == "Y0" || fragment.fragmentName == "Y1" || fragment.fragmentName == "Z0" || fragment.fragmentName == "Z1"))
                    {
                        Lipid.subtractAdduct(atomsCountFragment, precursorData.adduct);
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
            sql = "INSERT INTO RefSpectra (moleculeName, precursorMZ, precursorCharge, precursorAdduct, prevAA, nextAA, copies, numPeaks, driftTimeMsec, collisionalCrossSectionSqA, driftTimeHighEnergyOffsetMsec, retentionTime, fileID, SpecIDinFile, score, scoreType, inchiKey, otherKeys, peptideSeq, peptideModSeq, chemicalFormula) VALUES('" + precursorData.precursorName + "', " + precursorData.precursorM_Z + ", " + precursorData.precursorCharge + ", '" + precursorData.precursorAdduct + "', '-', '-', 0, " + numFragments + ", 0, 0, 0, 0, '0', 0, 1, 1, '', '', '', '',  '" + precursorData.precursorIonFormula + "')";
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
        
        
        
        public virtual string serialize()
        {
            string xml = "<className>" + className + "</className>\n";
            xml += "<representativeFA>" + (representativeFA ? 1 : 0) + "</representativeFA>\n";
            foreach (KeyValuePair<String, bool> item in adducts)
            {
                xml += "<adduct type=\"" + item.Key + "\">" + (item.Value ? 1 : 0) + "</adduct>\n";
            }
            
            foreach (KeyValuePair<String, ArrayList> item in MS2Fragments)
            {
                xml += "<MS2FragmentGroup name=\"" + item.Key + "\">\n";
                foreach (MS2Fragment fragment in item.Value)
                {
                    xml += fragment.serialize();
                }
                xml += "</MS2FragmentGroup>\n";
            }
            return xml;
        }
        
        public Lipid(Lipid copy)
        {
            adducts = new Dictionary<String, bool>();
            adducts.Add("+H", copy.adducts["+H"]);
            adducts.Add("+2H", copy.adducts["+2H"]);
            adducts.Add("+NH4", copy.adducts["+NH4"]);
            adducts.Add("-H", copy.adducts["-H"]);
            adducts.Add("-2H", copy.adducts["-2H"]);
            adducts.Add("+HCOO", copy.adducts["+HCOO"]);
            adducts.Add("+CH3COO", copy.adducts["+CH3COO"]);
            className = copy.className;
            representativeFA = copy.representativeFA;
            MS2Fragments = new Dictionary<String, ArrayList>();
            pathsToFullImage = new Dictionary<String, String>();
            foreach (KeyValuePair<String, String> item in copy.pathsToFullImage)
            {
                pathsToFullImage.Add(item.Key, item.Value);
            }
            
            foreach (KeyValuePair<String, ArrayList> item in copy.MS2Fragments)
            {
                MS2Fragments.Add(item.Key, new ArrayList());
                foreach (MS2Fragment fragment in item.Value)
                {
                    MS2Fragments[item.Key].Add(new MS2Fragment(fragment));
                }
            }
        }
        
        
        
        public static void subtractAdduct(DataTable atomsCount, String adduct)
        {
            switch (adduct)
            {
                             
                case "+NH4":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] - 3;
                    atomsCount.Rows[3]["Count"] = (int)atomsCount.Rows[3]["Count"] - 1;
                    break;
                case "+HCOO":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] - 2;
                    atomsCount.Rows[0]["Count"] = (int)atomsCount.Rows[0]["Count"] - 1;
                    atomsCount.Rows[2]["Count"] = (int)atomsCount.Rows[2]["Count"] - 2;
                    break;
                case "+CH3COO":
                    atomsCount.Rows[0]["Count"] = (int)atomsCount.Rows[0]["Count"] - 2;
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] - 4;
                    atomsCount.Rows[2]["Count"] = (int)atomsCount.Rows[2]["Count"] - 2;
                    break;
            }
        }
        
        public int getChargeAndAddAdduct(DataTable atomsCount, String adduct)
        {
            int charge = 0;
            switch (adduct)
            {
                                                                                                
                case "+H":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] + 1;
                    charge = 1;
                    break;
                case "+2H":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] + 2;
                    charge = 2;
                    break;
                case "+NH4":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] + 4;
                    atomsCount.Rows[3]["Count"] = (int)atomsCount.Rows[3]["Count"] + 1;
                    charge = 1;
                    break;
                case "-H":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] - 1;
                    charge = -1;
                    break;
                case "-2H":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] - 2;
                    charge = -2;
                    break;
                case "+HCOO":
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] + 1;
                    atomsCount.Rows[0]["Count"] = (int)atomsCount.Rows[0]["Count"] + 1;
                    atomsCount.Rows[2]["Count"] = (int)atomsCount.Rows[2]["Count"] + 2;
                    charge = -1;
                    break;
                case "+CH3COO":
                    atomsCount.Rows[0]["Count"] = (int)atomsCount.Rows[0]["Count"] + 2;
                    atomsCount.Rows[1]["Count"] = (int)atomsCount.Rows[1]["Count"] + 3;
                    atomsCount.Rows[2]["Count"] = (int)atomsCount.Rows[2]["Count"] + 2;
                    charge = -1;
                    break;
            }
            return charge;
        }
        
        
        public virtual void import(XElement node)
        {
            switch (node.Name.ToString())
            {
                case "className":
                    className = node.Value.ToString();
                    break;
                    
                case "representativeFA":
                    representativeFA = node.Value == "1";
                    break;
                    
                case "adduct":
                    string adductKey = node.Attribute("type").Value.ToString();
                    adducts[adductKey] = node.Value == "1";
                    break;
                    
                case "MS2FragmentGroup":
                    string fragmentKey = node.Attribute("name").Value.ToString();
                    foreach (XElement fragment in node.Elements()){
                        MS2Fragment ms2Fragment = new MS2Fragment();
                        ms2Fragment.import(fragment);
                        MS2Fragments[fragmentKey].Add(ms2Fragment);
                    }
                    break;
                    
                default:
                    Console.WriteLine("Error: " + node.Name.ToString());
                    throw new Exception();
            }
        }
    }
}