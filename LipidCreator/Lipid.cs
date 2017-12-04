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
using System.Globalization;

namespace LipidCreator
{
    public enum LipidCategory {NoLipid = 0, GlyceroLipid = 1, PhosphoLipid = 2, SphingoLipid = 3, Cholesterol = 4, Mediator = 5};
    
    
    [Serializable]
    public class PrecursorData
    {
        public LipidCategory lipidCategory;
        public string moleculeListName;
        public string lipidClass;
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
        public HashSet<string> fragmentNames;
    }
    
    
    [Serializable]
    public class Lipid
    {
        public string className;
        public Dictionary<string, HashSet<string>> positiveFragments;
        public Dictionary<string, HashSet<string>> negativeFragments;
        public Dictionary<String, bool> adducts;
        public bool representativeFA;
        public List<String> headGroupNames;
        public static string ID_SEPARATOR_UNSPECIFIC = "_";
        public static string ID_SEPARATOR_SPECIFIC = "/";
        public static string HEAVY_LABEL_SEPARATOR = "-";
        public static Dictionary<int, string> chargeToAdduct = new Dictionary<int, string>{{1, "+H"}, {2, "+2H"}, {-1, "-H"}, {-2, "-2H"}};
    
        public Lipid(LipidCreator lipidCreator, LipidCategory lipidCategory)
        {
            adducts = new Dictionary<String, bool>();
            adducts.Add("+H", false);
            adducts.Add("+2H", false);
            adducts.Add("+NH4", false);
            adducts.Add("-H", true);
            adducts.Add("-2H", false);
            adducts.Add("+HCOO", false);
            adducts.Add("+CH3COO", false);
            positiveFragments = new Dictionary<string, HashSet<string>>();
            negativeFragments = new Dictionary<string, HashSet<string>>();
            representativeFA = false;
            headGroupNames = new List<String>();
            
            if (lipidCreator.categoryToClass.ContainsKey((int)lipidCategory))
            {
                foreach (String lipidClass in lipidCreator.categoryToClass[(int)lipidCategory])
                {
                    if (!positiveFragments.ContainsKey(lipidClass)) positiveFragments.Add(lipidClass, new HashSet<string>());
                    if (!negativeFragments.ContainsKey(lipidClass)) negativeFragments.Add(lipidClass, new HashSet<string>());
                    
                    foreach (KeyValuePair<string, MS2Fragment> fragment in lipidCreator.allFragments[lipidClass][true])
                    {
                        positiveFragments[lipidClass].Add(fragment.Value.fragmentName);
                    }
                    
                    foreach (KeyValuePair<string, MS2Fragment> fragment in lipidCreator.allFragments[lipidClass][false])
                    {
                        negativeFragments[lipidClass].Add(fragment.Value.fragmentName);
                    }
                }
            }
        }
        
        public virtual void computePrecursorData(Dictionary<String, Precursor> headgroups, HashSet<String> usedKeys, ArrayList precursorDataList)
        {
        }
        
        
        
        public static void computeFragmentData(DataTable transitionList, PrecursorData precursorData, Dictionary<string, Dictionary<bool, Dictionary<string, MS2Fragment>>> allFragments)
        {                    
            int reportedFragments = 0;
            foreach (string fragmentName in precursorData.fragmentNames)
            {
                MS2Fragment fragment = allFragments[precursorData.lipidClass][precursorData.precursorCharge >= 0][fragmentName];
                //if (((precursorData.precursorCharge < 0 && fragment.fragmentCharge < 0) || (precursorData.precursorCharge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(precursorData.adduct)))
                if (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(precursorData.adduct))
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
                    
                    String chemFormFragment = LipidCreator.computeChemicalFormula(atomsCountFragment);
                    getChargeAndAddAdduct(atomsCountFragment, Lipid.chargeToAdduct[fragment.fragmentCharge]);
                    double massFragment = LipidCreator.computeMass(atomsCountFragment, fragment.fragmentCharge) / (double)(Math.Abs(fragment.fragmentCharge));
                    string fragName = fragment.fragmentName;
                    
                    // Exceptions for mediators
                    if (precursorData.lipidCategory == LipidCategory.Mediator)
                    {
                        massFragment = Convert.ToDouble(fragment.fragmentName, CultureInfo.InvariantCulture); // - fragment.fragmentCharge * 0.00054857990946;
                        fragName = string.Format("{0:0.000}", Convert.ToDouble(fragName, CultureInfo.InvariantCulture));
                    }
                    
                    DataRow lipidRow = transitionList.NewRow();
                    lipidRow[LipidCreator.MOLECULE_LIST_NAME] = precursorData.moleculeListName;
                    lipidRow[LipidCreator.PRECURSOR_NAME] = precursorData.precursorName;
                    lipidRow[LipidCreator.PRECURSOR_NEUTRAL_FORMULA] = precursorData.precursorIonFormula;
                    lipidRow[LipidCreator.PRECURSOR_ADDUCT] = precursorData.precursorAdduct;
                    lipidRow[LipidCreator.PRECURSOR_MZ] = precursorData.precursorM_Z;
                    lipidRow[LipidCreator.PRECURSOR_CHARGE] = ((precursorData.precursorCharge > 0) ? "+" : "") + Convert.ToString(precursorData.precursorCharge);
                    lipidRow[LipidCreator.PRODUCT_NAME] = fragName;
                    lipidRow[LipidCreator.PRODUCT_NEUTRAL_FORMULA] = chemFormFragment;
                    lipidRow[LipidCreator.PRODUCT_MZ] = massFragment / (double)(Math.Abs(fragment.fragmentCharge));
                    lipidRow[LipidCreator.PRODUCT_CHARGE] = ((fragment.fragmentCharge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentCharge);
                    transitionList.Rows.Add(lipidRow);
                    
                    ++reportedFragments;
                }
            }
            
            if(reportedFragments > 0)
            {
                DataRow lipidRowPrecursor = transitionList.NewRow();
                lipidRowPrecursor[LipidCreator.MOLECULE_LIST_NAME] = precursorData.moleculeListName;
                lipidRowPrecursor[LipidCreator.PRECURSOR_NAME] = precursorData.precursorName;
                lipidRowPrecursor[LipidCreator.PRECURSOR_NEUTRAL_FORMULA] = precursorData.precursorIonFormula;
                lipidRowPrecursor[LipidCreator.PRECURSOR_ADDUCT] = precursorData.precursorAdduct;
                lipidRowPrecursor[LipidCreator.PRECURSOR_MZ] = precursorData.precursorM_Z;
                lipidRowPrecursor[LipidCreator.PRECURSOR_CHARGE] = ((precursorData.precursorCharge > 0) ? "+" : "") + Convert.ToString(precursorData.precursorCharge);
                lipidRowPrecursor[LipidCreator.PRODUCT_NAME] = "Pre";
                lipidRowPrecursor[LipidCreator.PRODUCT_NEUTRAL_FORMULA] = precursorData.precursorIonFormula;
                lipidRowPrecursor[LipidCreator.PRODUCT_MZ] = precursorData.precursorM_Z;
                lipidRowPrecursor[LipidCreator.PRODUCT_CHARGE] = ((precursorData.precursorCharge > 0) ? "+" : "") + Convert.ToString(precursorData.precursorCharge);
                transitionList.Rows.Add(lipidRowPrecursor);
            }
        }
        
        
        
        public static void addSpectra(SQLiteCommand command, PrecursorData precursorData)
        {
            /*
            ArrayList valuesMZ = new ArrayList();
            ArrayList valuesIntensity = new ArrayList();
            String sql;
            
            bool reportFragments = false;
            foreach (MS2Fragment fragment in precursorData.MS2Fragments)
            {
                if (fragment.fragmentSelected && ((precursorData.precursorCharge < 0 && fragment.fragmentCharge < 0) || (precursorData.precursorCharge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(precursorData.adduct)))
                {
                    reportFragments = true;
                    break;
                }
            }
            
            if (reportFragments)
            {
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
                        
                        String chemFormFragment = LipidCreator.computeChemicalFormula(atomsCountFragment);
                        getChargeAndAddAdduct(atomsCountFragment, Lipid.chargeToAdduct[fragment.fragmentCharge]);
                        double massFragment = LipidCreator.computeMass(atomsCountFragment, fragment.fragmentCharge) / (double)(Math.Abs(fragment.fragmentCharge));
                        string fragName = fragment.fragmentName;
                        
                        if (precursorData.lipidCategory == LipidCategory.Mediator)
                        {
                            massFragment = Convert.ToDouble(fragment.fragmentName, CultureInfo.InvariantCulture); // - fragment.fragmentCharge * 0.00054857990946;
                            fragName = string.Format("{0:0.000}", Convert.ToDouble(fragName, CultureInfo.InvariantCulture));
                        }
                        
                        valuesMZ.Add(massFragment);
                        valuesIntensity.Add(fragment.intensity);
                        
                        // add Annotation
                        sql = "INSERT INTO Annotations(RefSpectraID, fragmentMZ, sumComposition, shortName) VALUES ((SELECT COUNT(*) FROM RefSpectra) + 1, " + massFragment + ", '" + chemFormFragment + "', @fragmentName)";
                        SQLiteParameter parameterName = new SQLiteParameter("@fragmentName", System.Data.DbType.String);
                        parameterName.Value = fragName;
                        command.CommandText = sql;
                        command.Parameters.Add(parameterName);
                        command.ExecuteNonQuery();
                    }
                }
                
                // add Annotation for precursor
                sql = "INSERT INTO Annotations(RefSpectraID, fragmentMZ, sumComposition, shortName) VALUES ((SELECT COUNT(*) FROM RefSpectra) + 1, " + precursorData.precursorM_Z + ", '" + precursorData.precursorIonFormula + "', @fragmentName)";
                SQLiteParameter parameterNamePre = new SQLiteParameter("@fragmentName", System.Data.DbType.String);
                parameterNamePre.Value = precursorData.precursorName;
                command.CommandText = sql;
                command.Parameters.Add(parameterNamePre);
                command.ExecuteNonQuery();
                valuesMZ.Add(precursorData.precursorM_Z);
                valuesIntensity.Add(100);
                
                
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
            */
        }
        
        
        
        public virtual string serialize()
        {
            string xml = "<className>" + className + "</className>\n";
            xml += "<representativeFA>" + (representativeFA ? 1 : 0) + "</representativeFA>\n";
            foreach (KeyValuePair<String, bool> item in adducts)
            {
                xml += "<adduct type=\"" + item.Key + "\">" + (item.Value ? 1 : 0) + "</adduct>\n";
            }
            
            foreach (KeyValuePair<string, HashSet<string>> positiveFragment in positiveFragments)
            {
                xml += "<positiveFragments lipidClass=\"" + positiveFragment.Key + "\">\n";
                foreach (string fragment in positiveFragment.Value)
                {
                    xml += "<fragment>" + fragment + "</fragment>\n";
                }
                xml += "</positiveFragments>\n";
            }
            
            foreach (KeyValuePair<string, HashSet<string>> negativeFragment in negativeFragments)
            {
                xml += "<negativeFragments lipidClass=\"" + negativeFragment.Key + "\">\n";
                foreach (string fragment in negativeFragment.Value)
                {
                    xml += "<fragment>" + fragment + "</fragment>\n";
                }
                xml += "</negativeFragments>\n";
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
            headGroupNames = new List<String>();
        
            positiveFragments = new Dictionary<string, HashSet<string>>();
            negativeFragments = new Dictionary<string, HashSet<string>>();
            foreach (KeyValuePair<string, HashSet<string>> positiveFragment in copy.positiveFragments)
            {
                positiveFragments.Add(positiveFragment.Key, new HashSet<string>());
                foreach(string fragment in positiveFragment.Value) positiveFragments[positiveFragment.Key].Add(fragment);
            }
            foreach (KeyValuePair<string, HashSet<string>> negativeFragment in copy.negativeFragments)
            {
                negativeFragments.Add(negativeFragment.Key, new HashSet<string>());
                foreach(string fragment in negativeFragment.Value) negativeFragments[negativeFragment.Key].Add(fragment);
            }
            foreach (string headgroup in copy.headGroupNames)
            {
                headGroupNames.Add(headgroup);
            }
        }
        
        
        
        public static void subtractAdduct(DataTable atomsCount, String adduct)
        {
            switch (adduct)
            {            
                case "+NH4":
                    atomsCount.Rows[(int)Molecules.H]["Count"] = (int)atomsCount.Rows[(int)Molecules.H]["Count"] - 3;
                    atomsCount.Rows[(int)Molecules.N]["Count"] = (int)atomsCount.Rows[(int)Molecules.N]["Count"] - 1;
                    break;
                case "+HCOO":
                    atomsCount.Rows[(int)Molecules.H]["Count"] = (int)atomsCount.Rows[(int)Molecules.H]["Count"] - 2;
                    atomsCount.Rows[(int)Molecules.C]["Count"] = (int)atomsCount.Rows[(int)Molecules.C]["Count"] - 1;
                    atomsCount.Rows[(int)Molecules.O]["Count"] = (int)atomsCount.Rows[(int)Molecules.O]["Count"] - 2;
                    break;
                case "+CH3COO":
                    atomsCount.Rows[(int)Molecules.C]["Count"] = (int)atomsCount.Rows[(int)Molecules.C]["Count"] - 2;
                    atomsCount.Rows[(int)Molecules.H]["Count"] = (int)atomsCount.Rows[(int)Molecules.H]["Count"] - 4;
                    atomsCount.Rows[(int)Molecules.O]["Count"] = (int)atomsCount.Rows[(int)Molecules.O]["Count"] - 2;
                    break;
            }
        }
        
        public static int getChargeAndAddAdduct(DataTable atomsCount, String adduct)
        {
            int charge = 0;
            switch (adduct)
            {
                                                                                                
                case "+H":
                    atomsCount.Rows[(int)Molecules.H]["Count"] = (int)atomsCount.Rows[(int)Molecules.H]["Count"] + 1;
                    charge = 1;
                    break;
                case "+2H":
                    atomsCount.Rows[(int)Molecules.H]["Count"] = (int)atomsCount.Rows[(int)Molecules.H]["Count"] + 2;
                    charge = 2;
                    break;
                case "+NH4":
                    atomsCount.Rows[(int)Molecules.H]["Count"] = (int)atomsCount.Rows[(int)Molecules.H]["Count"] + 4;
                    atomsCount.Rows[(int)Molecules.N]["Count"] = (int)atomsCount.Rows[(int)Molecules.N]["Count"] + 1;
                    charge = 1;
                    break;
                case "-H":
                    atomsCount.Rows[(int)Molecules.H]["Count"] = (int)atomsCount.Rows[(int)Molecules.H]["Count"] - 1;
                    charge = -1;
                    break;
                case "-2H":
                    atomsCount.Rows[(int)Molecules.H]["Count"] = (int)atomsCount.Rows[(int)Molecules.H]["Count"] - 2;
                    charge = -2;
                    break;
                case "+HCOO":
                    atomsCount.Rows[(int)Molecules.H]["Count"] = (int)atomsCount.Rows[(int)Molecules.H]["Count"] + 1;
                    atomsCount.Rows[(int)Molecules.C]["Count"] = (int)atomsCount.Rows[(int)Molecules.C]["Count"] + 1;
                    atomsCount.Rows[(int)Molecules.O]["Count"] = (int)atomsCount.Rows[(int)Molecules.O]["Count"] + 2;
                    charge = -1;
                    break;
                case "+CH3COO":
                    atomsCount.Rows[(int)Molecules.C]["Count"] = (int)atomsCount.Rows[(int)Molecules.C]["Count"] + 2;
                    atomsCount.Rows[(int)Molecules.H]["Count"] = (int)atomsCount.Rows[(int)Molecules.H]["Count"] + 3;
                    atomsCount.Rows[(int)Molecules.O]["Count"] = (int)atomsCount.Rows[(int)Molecules.O]["Count"] + 2;
                    charge = -1;
                    break;
            }
            return charge;
        }
        
        
        public virtual void import(XElement node, string importVersion)
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
                    
                case "positiveFragments":
                    string posLipidClass = node.Attribute("lipidClass").Value.ToString();
                    if (!positiveFragments.ContainsKey(posLipidClass)) positiveFragments.Add(posLipidClass, new HashSet<string>());
                    var posFragments = node.Descendants("fragments");
                    foreach (var fragment in posFragments)
                    {
                        positiveFragments[posLipidClass].Add(fragment.Value.ToString());
                    }
                    break;
                    
                case "negativeFragments":
                    string negLipidClass = node.Attribute("lipidClass").Value.ToString();
                    if (!negativeFragments.ContainsKey(negLipidClass)) negativeFragments.Add(negLipidClass, new HashSet<string>());
                    var negFragments = node.Descendants("fragments");
                    foreach (var fragment in negFragments)
                    {
                        negativeFragments[negLipidClass].Add(fragment.Value.ToString());
                    }
                    break;
                    
                default:
                    Console.WriteLine("Error: " + node.Name.ToString());
                    throw new Exception("Error: " + node.Name.ToString());
            }
        }
    }
}