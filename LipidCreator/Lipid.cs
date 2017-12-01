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
using System.Linq;

// For the benefit of Skyline developer systems configured to not allow nonlocalized strings
// ReSharper disable NonLocalizedString

namespace LipidCreator
{
    public enum LipidCategory {NoLipid = 0, GlyceroLipid = 1, PhosphoLipid = 2, SphingoLipid = 3, Cholesterol = 4, Mediator = 5};
    
    
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
        public List<String> headGroupNames;
        public static string ID_SEPARATOR_UNSPECIFIC = "_";
        public static string ID_SEPARATOR_SPECIFIC = "/";
        public static string HEAVY_LABEL_SEPARATOR = "-";
        public static Dictionary<int, string> chargeToAdduct = new Dictionary<int, string>{{1, "+H"}, {2, "+2H"}, {-1, "-H"}, {-2, "-2H"}};
    
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
            headGroupNames = new List<String>();
        }
        
        public virtual void computePrecursorData(Dictionary<String, Precursor> headgroups, HashSet<String> usedKeys, ArrayList precursorDataList)
        {
        }
        
        
        
        public static void computeFragmentData(DataTable transitionList, PrecursorData precursorData)
        {                    
            int reportedFragments = 0;
            foreach (MS2Fragment fragment in precursorData.MS2Fragments)
            {
            //Console.WriteLine(precursorData.precursorName);
                if (fragment.fragmentSelected && ((precursorData.precursorCharge < 0 && fragment.fragmentCharge < 0) || (precursorData.precursorCharge > 0 && fragment.fragmentCharge > 0)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(precursorData.adduct)))
                //if (fragment.fragmentSelected && ((precursorData.precursorCharge < 0 && fragment.fragmentCharge < 0 && precursorData.precursorCharge <= fragment.fragmentCharge) || (precursorData.precursorCharge > 0 && fragment.fragmentCharge > 0 && precursorData.precursorCharge >= fragment.fragmentCharge)) && (fragment.restrictions.Count == 0 || fragment.restrictions.Contains(precursorData.adduct)))
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
        
        protected class PeakAnnotation
        {
            public string Name { get; private set; }
            public int Charge { get; private set; }
            public string Adduct { get; private set; } // can be left blank for molecular ions, mostly useful for precursor ions or describing neutral losses
            public string Formula { get; private set; }
            public string Comment { get; private set; }
            public PeakAnnotation(string name, int z, string adduct, string formula, string comment)
            {
                Name = name.Replace("'", "''"); // escape single quotes for sqlite insertion
                Charge = z;
                Adduct = adduct;
                Formula = formula.Replace("'", "''"); // escape single quotes for sqlite insertion;
                Comment = comment.Replace("'", "''"); // escape single quotes for sqlite insertion;
            }

            public override string ToString()
            {
                return (Name ?? string.Empty) + " z=" + Charge + " " + (Formula ?? String.Empty) + " " + (Adduct ?? String.Empty) + " " + (Comment ?? String.Empty);
            }
        }

        protected class Peak
        {
            public double Mz { get; private set; }
            public double Intensity { get; private set; }
            public PeakAnnotation Annotation { get; private set; }

            public Peak(double mz, double intensity, PeakAnnotation annotation)
            {
                Mz = mz;
                Intensity = intensity;
                Annotation = annotation;
            }

            public override string ToString()
            {
                return Mz + " " + Intensity + " " + Annotation;
            }
        }

        protected static void SavePeaks(SQLiteCommand command, PrecursorData precursorData, IEnumerable<Peak> peaksList)
        {
            string sql;
            var peaks = peaksList.OrderBy(o => o.Mz).ToArray(); // .blib expects  ascending mz
            int numFragments = peaks.Length;
            if (numFragments == 0)
            {
                return; // How does this happen?
            }

            var valuesMZArray = new List<double>();
            var valuesIntens = new List<float>();
            var annotations = new List<List<PeakAnnotation>>(); // We anticipate more than one possible annotation per peak

            // Deal with mz conflicts by combining annotations
            for (int j = 0; j < numFragments; ++j)
            {
                if (j == 0 || (peaks[j].Mz != peaks[j - 1].Mz))
                {
                    valuesMZArray.Add(peaks[j].Mz);
                    valuesIntens.Add(100 * (float)peaks[j].Intensity);
                    annotations.Add(new List<PeakAnnotation>());
                }
                annotations.Last().Add(peaks[j].Annotation);
            }
            numFragments = valuesMZArray.Count;

            // add MS1 information - always claim FileId=1 (SpectrumSourceFiles has an entry for this, saying that these are generated spectra)
            sql =
                "INSERT INTO RefSpectra (moleculeName, precursorMZ, precursorCharge, precursorAdduct, prevAA, nextAA, copies, numPeaks, ionMobility, collisionalCrossSectionSqA, ionMobilityHighEnergyOffset, ionMobilityType, retentionTime, fileID, SpecIDinFile, score, scoreType, inchiKey, otherKeys, peptideSeq, peptideModSeq, chemicalFormula) VALUES('" +
                precursorData.precursorName + "', " + precursorData.precursorM_Z + ", " + precursorData.precursorCharge +
                ", '" + precursorData.precursorAdduct + "', '-', '-', 0, " + numFragments +
                ", 0, 0, 0, 0, 0, '1', 0, 1, 1, '', '', '', '',  '" + precursorData.precursorIonFormula + "')";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            // add spectrum
            command.CommandText =
                "INSERT INTO RefSpectraPeaks(RefSpectraID, peakMZ, peakIntensity) VALUES((SELECT MAX(id) FROM RefSpectra), @mzvalues, @intensvalues)";
            SQLiteParameter parameterMZ = new SQLiteParameter("@mzvalues", System.Data.DbType.Binary);
            SQLiteParameter parameterIntens = new SQLiteParameter("@intensvalues", System.Data.DbType.Binary);
            parameterMZ.Value = Compressing.Compress(valuesMZArray.ToArray());
            parameterIntens.Value = Compressing.Compress(valuesIntens.ToArray());
            command.Parameters.Add(parameterMZ);
            command.Parameters.Add(parameterIntens);
            command.ExecuteNonQuery();
        
            // add annotations
            // TODO: what about precursor peaks?
            for (int i = 0; i < annotations.Count; i++)
            {
                foreach (var ann in annotations[i]) // Each peak may have multiple annotations
                {
                    var adduct = ann.Adduct;
                    if (string.IsNullOrEmpty(adduct))
                    {
                        switch (ann.Charge)
                        {
                            case 1:
                                adduct = "[M+]";
                                break;
                            case -1:
                                adduct = "[M-]";
                                break;
                            default:
                                adduct = "[M" + ann.Charge.ToString("+#;-#;0") + "]";
                                break;
                        }
                    }
                    else if (!adduct.StartsWith("[M"))
                    {
                        // Consistent adduct declaration style
                        if (adduct.StartsWith("M"))
                            adduct = "[" + adduct + "]";
                        else
                            adduct = "[M" + adduct + "]";
                    }
                    command.CommandText =
                        "INSERT INTO RefSpectraPeakAnnotations(RefSpectraID, " +
                        "peakIndex , name , formula, charge, adduct, comment, mzTheoretical, mzObserved) VALUES((SELECT MAX(id) FROM RefSpectra), " +
                        i + ", '" + ann.Name + "', '" + ann.Formula + "', " + ann.Charge + ", '" + adduct + "', '" + ann.Comment + "', " + valuesMZArray[i] + ", " + valuesMZArray[i] + ")";
                    command.ExecuteNonQuery();
                }
            }
        }
                
        
        public static void addSpectra(SQLiteCommand command, PrecursorData precursorData)
        {
            
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
                var peaks = new List<Peak>();
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
                        
                        peaks.Add(new Peak(massFragment,
                            fragment.intensity,
                            new PeakAnnotation(fragName,
                                fragment.fragmentCharge,
                                Lipid.chargeToAdduct[fragment.fragmentCharge],
                                chemFormFragment,
                                fragment.CommentForSpectralLibrary)));
                }
                }
                
                // add precursor
                peaks.Add(new Peak(precursorData.precursorM_Z,
                    MS2Fragment.DEFAULT_INTENSITY,
                    new PeakAnnotation(precursorData.precursorName,
                        precursorData.precursorCharge,
                        precursorData.precursorAdduct,
                        precursorData.precursorIonFormula,
                        "precursor")));
                
                // Commit to .blib
                SavePeaks(command, precursorData, peaks);
                
            }
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
            headGroupNames = new List<String>();
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
                    
                case "MS2FragmentGroup":
                    string fragmentKey = node.Attribute("name").Value.ToString();
                    foreach (XElement fragment in node.Elements()){
                        MS2Fragment ms2Fragment = new MS2Fragment();
                        ms2Fragment.import(fragment, importVersion);
                        MS2Fragments[fragmentKey].Add(ms2Fragment);
                    }
                    break;
                    
                default:
                    Console.WriteLine("Error: " + node.Name.ToString());
                    throw new Exception("Error: " + node.Name.ToString());
            }
        }
    }
}