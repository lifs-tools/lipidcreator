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
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using log4net;

// For the benefit of Skyline developer systems configured to not allow nonlocalized strings
// ReSharper disable NonLocalizedString

namespace LipidCreator
{
    public enum LipidCategory {NoLipid = 0, Glycerolipid = 1, Glycerophospholipid = 2, Sphingolipid = 3, Sterollipid = 4, LipidMediator = 5, Unsupported = 99};
    
    
    
    
    
    [Serializable]
    public class PrecursorData
    {
        public LipidCategory lipidCategory;
        public string moleculeListName;
        public string fullMoleculeListName; // including heavy labeled suffix
        public string precursorExportName;
        public string precursorName;
        public string precursorSpeciesName;
        public string precursorIonFormula;
        public Adduct precursorAdduct;
        public ulong lipidHash = 0;
        public string precursorAdductFormula;
        public double precursorM_Z;
        public bool addPrecursor;
        public bool precursorSelected = true;
        public FattyAcid fa1;
        public FattyAcid fa2;
        public FattyAcid fa3;
        public FattyAcid fa4;
        public FattyAcid lcb;
        public HashSet<string> fragmentNames;
    }
    
    
    
    
    
    public class LipidException : Exception
    {
        public MS2Fragment fragment = null;
        public PrecursorData precursorData = null;
        public Molecule molecule = Molecule.C;
        public int counts = 0;
        public string heavyIsotope = "";
        public Object creatorGUI = null;
        
        public LipidException(PrecursorData _precursorData, MS2Fragment _fragment)
        {
            fragment = _fragment;
            precursorData = _precursorData;
        }
        
        public LipidException(Molecule _molecule, int _counts)
        {
            molecule = _molecule;
            counts = _counts;
        }
    }
    
    
    
    
    
    
    [Serializable]
    public abstract class Lipid
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Lipid));
        public Dictionary<string, HashSet<string>> positiveFragments;
        public Dictionary<string, HashSet<string>> negativeFragments;
        public Dictionary<string, bool> adducts;
        public bool representativeFA;
        public int onlyPrecursors;
        public int onlyHeavyLabeled;
        public List<string> headGroupNames;
        public static string ID_SEPARATOR_UNSPECIFIC = "-";
        public static string ID_SEPARATOR_SPECIFIC = "/";
        public LipidCreator lipidCreator;
        public static int MEDIATOR_PREFIX_LENGTH = 4;
        
        
        public static Dictionary<AdductType, Adduct> ALL_ADDUCTS = new Dictionary<AdductType, Adduct>(){
            {AdductType.Hp, new Adduct("+H", "+H⁺", 1, MS2Fragment.initializeElementDict(new Dictionary<string, int>(){{"H", 1}}))},
            {AdductType.HHp, new Adduct("+2H", "+2H⁺⁺", 2, MS2Fragment.initializeElementDict(new Dictionary<string, int>(){{"H", 2}}))},
            {AdductType.NHHHHp, new Adduct("+NH4", "+NH4⁺", 1, MS2Fragment.initializeElementDict(new Dictionary<string, int>(){{"H", 4}, {"N", 1}}))},
            {AdductType.Hm, new Adduct("-H", "-H⁻", -1, MS2Fragment.initializeElementDict(new Dictionary<string, int>(){{"H", -1}}))},
            {AdductType.HHm, new Adduct("-2H", "-2H⁻ ⁻", -2, MS2Fragment.initializeElementDict(new Dictionary<string, int>(){{"H", -2}}))},
            {AdductType.HCOOm, new Adduct("+HCOO", "+HCOO⁻", -1, MS2Fragment.initializeElementDict(new Dictionary<string, int>(){{"H", 1}, {"C", 1}, {"O", 2}}))},
            {AdductType.CHHHCOOm, new Adduct("+CH3COO", "+CH3COO⁻", -1, MS2Fragment.initializeElementDict(new Dictionary<string, int>(){{"H", 3}, {"C", 2}, {"O", 2}}))}
        };
        
        public static Dictionary<string, AdductType> ADDUCT_POSITIONS = ALL_ADDUCTS.Keys.ToDictionary(k=>ALL_ADDUCTS[k].name, k=>k);
        
        
        public static Dictionary<int, Adduct> chargeToAdduct = new Dictionary<int, Adduct>{{1, ALL_ADDUCTS[AdductType.Hp]}, {2, ALL_ADDUCTS[AdductType.HHp]}, {-1, ALL_ADDUCTS[AdductType.Hm]}, {-2, ALL_ADDUCTS[AdductType.HHm]}};
        
        
        
    
        public Lipid(LipidCreator _lipidCreator, LipidCategory lipidCategory)
        {
            lipidCreator = _lipidCreator;
            adducts = new Dictionary<string, bool>();
            foreach (string key in ADDUCT_POSITIONS.Keys) adducts.Add(key, false);
            positiveFragments = new Dictionary<string, HashSet<string>>();
            negativeFragments = new Dictionary<string, HashSet<string>>();
            representativeFA = false;
            onlyPrecursors = 2;
            onlyHeavyLabeled = 2;
            headGroupNames = new List<string>();
            lipidCreator.Update += new LipidUpdateEventHandler(this.Update);
            
            if (lipidCreator.categoryToClass.ContainsKey((int)lipidCategory))
            {
                foreach (string lipidClass in lipidCreator.categoryToClass[(int)lipidCategory])
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
        
        
        
        
        // synchronize the fragment list with list from LipidCreator root
        public abstract void Update(object sender, EventArgs e);
        
        
        public ArrayList getFattyAcidGroups()
        {
            return getFattyAcidGroupList();
        }
        
        public abstract ArrayList getFattyAcidGroupList();
        
        
        public void Updating(int category)
        {
            HashSet<string> headgroupsInLipid = new HashSet<string>(positiveFragments.Keys);
            
            HashSet<string> headgroupsInLC = new HashSet<string>();
            foreach (String lipidClass in lipidCreator.categoryToClass[category]) headgroupsInLC.Add(lipidClass);
            
            
            // check for adding headgroups
            HashSet<string> addHeadgroups = new HashSet<string>(headgroupsInLC);
            addHeadgroups.ExceptWith(headgroupsInLipid);
            
            foreach (string lipidClass in addHeadgroups)
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
            
            // check for adding headgroups
            HashSet<string> deleteHeadgroups = new HashSet<string>(headgroupsInLipid);
            deleteHeadgroups.ExceptWith(headgroupsInLC);
            
            foreach (string hg in deleteHeadgroups)
            {
                positiveFragments.Remove(hg);
                negativeFragments.Remove(hg);
            }
        }
        
        
        public abstract void computePrecursorData(IDictionary<String, Precursor> headgroups, HashSet<String> usedKeys, ArrayList precursorDataList);
        
        
        
        public virtual ulong getHashCode()
        {
            unchecked
            {
                ulong hashCode = representativeFA ? (1UL << 26) : (1UL << 61);
                hashCode += (((ulong)onlyPrecursors + 23UL) << 14);
                hashCode += (((ulong)onlyHeavyLabeled + 37UL) << 33);

                foreach (string adduct in adducts.Keys.Where(x => adducts[x]))
                {
                    hashCode += LipidCreator.HashCode(adduct);
                }

                foreach (string hg in headGroupNames)
                {
                    hashCode += LipidCreator.HashCode(hg);
                }

                foreach (string lipidClass in positiveFragments.Keys)
                {
                    foreach (string lipidName in positiveFragments[lipidClass])
                    {
                        if (lipidCreator.allFragments[lipidClass][true].ContainsKey(lipidName)) hashCode += lipidCreator.allFragments[lipidClass][true][lipidName].getHashCode();
                    }
                }


                foreach (string lipidClass in negativeFragments.Keys)
                {
                    foreach (string lipidName in negativeFragments[lipidClass])
                    {
                        if (lipidCreator.allFragments[lipidClass][false].ContainsKey(lipidName)) hashCode += lipidCreator.allFragments[lipidClass][false][lipidName].getHashCode();
                    }
                }
                return hashCode;
            }
        }
        
        
        
        public static void computeFragmentData(DataTable transitionList, PrecursorData precursorData, IDictionary<string, IDictionary<bool, IDictionary<string, MS2Fragment>>> allFragments, IDictionary<String, Precursor> headgroups, ArrayList parameters, CollisionEnergy collisionEnergyHandler = null, string instrument = "", MonitoringTypes monitoringType = MonitoringTypes.NoMonitoring, double CE = -1, double minCE = 0, double maxCE = 0)
        {
            
            
            if (precursorData.addPrecursor){
                DataRow lipidRowPrecursor = transitionList.NewRow();
                lipidRowPrecursor[LipidCreator.MOLECULE_LIST_NAME] = precursorData.moleculeListName;
                lipidRowPrecursor[LipidCreator.PRECURSOR_NAME] = ((int)parameters[1] != 0) ? precursorData.precursorSpeciesName : precursorData.precursorName;
                lipidRowPrecursor[LipidCreator.PRECURSOR_NEUTRAL_FORMULA] = precursorData.precursorIonFormula;
                lipidRowPrecursor[LipidCreator.PRECURSOR_ADDUCT] = precursorData.precursorAdductFormula;
                lipidRowPrecursor[LipidCreator.PRECURSOR_MZ] = string.Format(CultureInfo.InvariantCulture, "{0:N4}", precursorData.precursorM_Z).Replace(",", "");
                lipidRowPrecursor[LipidCreator.PRECURSOR_CHARGE] = ((precursorData.precursorAdduct.charge > 0) ? "+" : "") + Convert.ToString(precursorData.precursorAdduct.charge);
                lipidRowPrecursor[LipidCreator.PRODUCT_NAME] = "precursor";
                lipidRowPrecursor[LipidCreator.PRODUCT_NEUTRAL_FORMULA] = precursorData.precursorIonFormula;
                lipidRowPrecursor[LipidCreator.PRODUCT_ADDUCT] = precursorData.precursorAdductFormula;
                lipidRowPrecursor[LipidCreator.PRODUCT_MZ] = string.Format(CultureInfo.InvariantCulture, "{0:N4}", precursorData.precursorM_Z).Replace(",", "");
                lipidRowPrecursor[LipidCreator.PRODUCT_CHARGE] = ((precursorData.precursorAdduct.charge > 0) ? "+" : "") + Convert.ToString(precursorData.precursorAdduct.charge);
                lipidRowPrecursor[LipidCreator.NOTE] = "";
                lipidRowPrecursor[LipidCreator.SPECIFIC] = "1";
                transitionList.Rows.Add(lipidRowPrecursor);
                
                if (collisionEnergyHandler != null && instrument.Length > 0 && monitoringType != MonitoringTypes.NoMonitoring)
                {
                    string lipidClass = precursorData.fullMoleculeListName;
                    string adduct = LipidCreator.computeAdductFormula(null, precursorData.precursorAdduct);
                    if (monitoringType == MonitoringTypes.PRM)
                    {
                        lipidRowPrecursor[LipidCreator.COLLISION_ENERGY] = CE.ToString(CultureInfo.InvariantCulture);
                    }
                    else if (monitoringType == MonitoringTypes.SRM)
                    {
                        double ceValue = collisionEnergyHandler.getApex(instrument, lipidClass, adduct, "precursor");
                        if (ceValue != -1) ceValue = Math.Max(Math.Min(maxCE, ceValue), minCE);
                        lipidRowPrecursor[LipidCreator.COLLISION_ENERGY] = ceValue.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
            
            HashSet<string> insertedFragments = new HashSet<string>();
            
            foreach (string fragmentName in precursorData.fragmentNames)
            {
                if (!allFragments.ContainsKey(precursorData.fullMoleculeListName) || !allFragments[precursorData.fullMoleculeListName][precursorData.precursorAdduct.charge >= 0].ContainsKey(fragmentName)) continue;
            
                // Cxception for LCB, only HG fragment occurs when LCB contains no double bond
                if (precursorData.moleculeListName.Equals("LCB") && fragmentName.Equals("LCB(60)") && precursorData.lcb.db > 0) continue;
                
                
                // Cxception for LCB, only HG fragment occurs when LCB contains no double bond
                if (precursorData.moleculeListName.Equals("Cer") && fragmentName.Equals("FA1(-CH2O)") && precursorData.fa1.hydroxyl == 0) continue;
                
                
                MS2Fragment fragment = allFragments[precursorData.fullMoleculeListName][precursorData.precursorAdduct.charge >= 0][fragmentName];
                
                // Exception for lipids with NL(NH3) fragment
                if (fragment.fragmentName.Equals("-(NH3,17)") && !precursorData.precursorAdductFormula.Equals("[M+NH4]1+")) continue;
                
                
                // Exception for lipids with NL([adduct]) fragment and +H or -H as adduct
                if (fragment.fragmentName.Equals("-([adduct])") && (precursorData.precursorAdductFormula.Equals("[M+H]1+") || precursorData.precursorAdductFormula.Equals("[M-H]1-"))) continue;
                
                DataRow lipidRow = transitionList.NewRow();
                lipidRow[LipidCreator.MOLECULE_LIST_NAME] = precursorData.moleculeListName;
                lipidRow[LipidCreator.PRECURSOR_NAME] = (((int)parameters[1] == 1 && fragment.specific) || (int)parameters[1] == 2) ? precursorData.precursorSpeciesName : precursorData.precursorName;
                lipidRow[LipidCreator.PRECURSOR_NEUTRAL_FORMULA] = precursorData.precursorIonFormula;
                lipidRow[LipidCreator.PRECURSOR_ADDUCT] = precursorData.precursorAdductFormula;
                lipidRow[LipidCreator.PRECURSOR_MZ] = string.Format(CultureInfo.InvariantCulture, "{0:N4}", precursorData.precursorM_Z).Replace(",", "");
                lipidRow[LipidCreator.PRECURSOR_CHARGE] = ((precursorData.precursorAdduct.charge > 0) ? "+" : "") + Convert.ToString(precursorData.precursorAdduct.charge);
                
                
                string fragName = fragment.fragmentOutputName;
                ElementDictionary atomsCountFragment = fragment.copyElementDict();
                
                
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
                        case "HG":
                            MS2Fragment.addCounts(atomsCountFragment, headgroups[precursorData.fullMoleculeListName].elements);
                            break;
                        default:
                            break;
                    }
                }
                
                
                string chemFormFragment = LipidCreator.computeChemicalFormula(atomsCountFragment);
                string fragAdduct = LipidCreator.computeAdductFormula(atomsCountFragment, fragment.fragmentAdduct);
                MS2Fragment.addCounts(atomsCountFragment, fragment.fragmentAdduct.elements);
                double massFragment = 0;
                
                // Exceptions for mediators
                if (precursorData.lipidCategory != LipidCategory.LipidMediator)
                {
                    try
                    {
                        massFragment = LipidCreator.computeMass(atomsCountFragment, fragment.fragmentAdduct.charge);
                    }
                    catch (LipidException lipidException)
                    {
                        lipidException.precursorData = precursorData;
                        lipidException.fragment = fragment;
                        lipidException.heavyIsotope = LipidCreator.precursorNameSplit(precursorData.fullMoleculeListName)[1];
                        throw lipidException;
                    }
                }
                else
                {
                    massFragment = Convert.ToDouble(fragment.fragmentName.Substring(MEDIATOR_PREFIX_LENGTH), CultureInfo.InvariantCulture);
                    chemFormFragment = "";
                }
                
                if (fragName.IndexOf("[adduct]") > -1)
                {
                    fragName = fragName.Replace("[adduct]", precursorData.precursorAdduct.name);
                }
                if (fragName.IndexOf("[xx:x]") > -1)
                {
                    fragName = fragName.Replace("[xx:x]", precursorData.fa1.ToString(false));
                }
                if (fragName.IndexOf("[yy:y]") > -1)
                {
                    fragName = fragName.Replace("[yy:y]", precursorData.fa2.ToString(false));
                }
                if (fragName.IndexOf("[zz:z]") > -1)
                {
                    fragName = fragName.Replace("[zz:z]", precursorData.fa3.ToString(false));
                }
                if (fragName.IndexOf("[uu:u]") > -1)
                {
                    fragName = fragName.Replace("[uu:u]", precursorData.fa4.ToString(false));
                }
                if (fragName.IndexOf("[xx:x;x]") > -1)
                {
                    fragName = fragName.Replace("[xx:x;x]", precursorData.lcb.ToString());
                }
                
                
                // exclude duplicate fragments within the same lipid species
                if (insertedFragments.Contains(fragName + "/" + fragAdduct)) continue;
                insertedFragments.Add(fragName + "/" + fragAdduct);
                
                
                
                string fragCharge = ((fragment.fragmentAdduct.charge > 0) ? "+" : "") + Convert.ToString(fragment.fragmentAdduct.charge);
                
                lipidRow[LipidCreator.PRODUCT_NAME] = fragName;
                lipidRow[LipidCreator.PRODUCT_NEUTRAL_FORMULA] = chemFormFragment;
                lipidRow[LipidCreator.PRODUCT_ADDUCT] = fragAdduct;
                lipidRow[LipidCreator.PRODUCT_MZ] = string.Format(CultureInfo.InvariantCulture, "{0:N4}", massFragment).Replace(",", "");
                lipidRow[LipidCreator.PRODUCT_CHARGE] = fragCharge;
                lipidRow[LipidCreator.NOTE] = "";
                lipidRow[LipidCreator.SPECIFIC] = fragment.specific ? "1" : "0";
                transitionList.Rows.Add(lipidRow);
                
                if (collisionEnergyHandler != null && instrument.Length > 0 && monitoringType != MonitoringTypes.NoMonitoring)
                {
                    string lipidClass = precursorData.fullMoleculeListName;
                    string adduct = LipidCreator.computeAdductFormula(null, precursorData.precursorAdduct);
                    if (monitoringType == MonitoringTypes.PRM)
                    {
                        lipidRow[LipidCreator.COLLISION_ENERGY] = CE.ToString(CultureInfo.InvariantCulture);
                    }
                    else if (monitoringType == MonitoringTypes.SRM)
                    {
                        double ceValue = collisionEnergyHandler.getApex(instrument, lipidClass, adduct, fragName);
                        if (ceValue != -1) ceValue = Math.Max(Math.Min(maxCE, ceValue), minCE);
                        lipidRow[LipidCreator.COLLISION_ENERGY] = ceValue.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
        }
        
        
        
        public class PeakAnnotation
        {
            public string Name { get; set; }
            public int Charge { get; set; }
            public string Adduct { get; set; } // can be left blank for molecular ions, mostly useful for precursor ions or describing neutral losses
            public string Formula { get; set; }
            public string Comment { get; set; }
            public PeakAnnotation(string name, int z, string adduct, string formula, string comment)
            {
                Name = name;
                Charge = z;
                Adduct = adduct;
                Formula = formula;
                Comment = comment;
            }

            public override string ToString()
            {
                return (Name ?? string.Empty) + " z=" + Charge + " " + (Formula ?? String.Empty) + " " + (Adduct ?? String.Empty) + " " + (Comment ?? String.Empty);
            }
        }

        public class Peak
        {
            public double Mz { get; set; }
            public double Intensity { get; set; }
            public PeakAnnotation Annotation { get; set; }

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

        public static void SavePeaks(SQLiteCommand command, PrecursorData precursorData, IEnumerable<Peak> peaksList)
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
                "INSERT INTO RefSpectra (moleculeName, precursorMZ, precursorCharge, precursorAdduct, prevAA, nextAA, copies, numPeaks, ionMobility, collisionalCrossSectionSqA, ionMobilityHighEnergyOffset, ionMobilityType, retentionTime, fileID, SpecIDinFile, score, scoreType, inchiKey, otherKeys, peptideSeq, peptideModSeq, chemicalFormula) VALUES(@precursorName, @precursorMz, @precursorCharge "  +
                ", @precursorAdduct, '-', '-', 0, " + numFragments +
                ", 0, 0, 0, 0, 0, '1', 0, 1, 1, '', '', '', '',  @precursorIonFormula)";
            command.CommandText = sql;
            log.Debug("Inserting into RefSpectra: " + command.CommandText);
            SQLiteParameter parameterPrecursorName = new SQLiteParameter("@precursorName", precursorData.precursorExportName);
            SQLiteParameter parameterPrecursorMz = new SQLiteParameter("@precursorMz", precursorData.precursorM_Z);
            SQLiteParameter parameterPrecursorCharge = new SQLiteParameter("@precursorCharge", precursorData.precursorAdduct.charge);
            SQLiteParameter parameterPrecursorAdduct = new SQLiteParameter("@precursorAdduct", precursorData.precursorAdductFormula);
            SQLiteParameter parameterPrecursorIonFormula = new SQLiteParameter("@precursorIonFormula", precursorData.precursorIonFormula);
            command.Parameters.Add(parameterPrecursorName);
            command.Parameters.Add(parameterPrecursorMz);
            command.Parameters.Add(parameterPrecursorCharge);
            command.Parameters.Add(parameterPrecursorAdduct);
            command.Parameters.Add(parameterPrecursorIonFormula);
            command.ExecuteNonQuery();

            // add spectrum
            command.CommandText =
                "INSERT INTO RefSpectraPeaks(RefSpectraID, peakMZ, peakIntensity) VALUES((SELECT MAX(id) FROM RefSpectra), @mzvalues, @intensvalues)";
            log.Debug("Inserting into RefSpectraPeaks: " + command.CommandText);
            SQLiteParameter parameterMZ = new SQLiteParameter("@mzvalues", System.Data.DbType.Binary);
            SQLiteParameter parameterIntens = new SQLiteParameter("@intensvalues", System.Data.DbType.Binary);
            parameterMZ.Value = Compressing.Compress(valuesMZArray.ToArray());
            parameterIntens.Value = Compressing.Compress(valuesIntens.ToArray());
            command.Parameters.Add(parameterMZ);
            command.Parameters.Add(parameterIntens);
            command.ExecuteNonQuery();
        
            // add annotations
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
                        "peakIndex , name, formula, inchiKey, otherKeys, charge, adduct, comment, mzTheoretical, mzObserved) VALUES((SELECT MAX(id) FROM RefSpectra), " +
                        i + ", @annotationName, @annotationFormula, '', '', @annotationCharge, @annotationAdduct, @annotationComment, @mzTheoretical, @mzObserved)";
                    
                    SQLiteParameter parameterAnnName = new SQLiteParameter("@annotationName", ann.Name);
                    SQLiteParameter parameterAnnFormula = new SQLiteParameter("@annotationFormula", ann.Formula);
                    SQLiteParameter parameterAnnCharge = new SQLiteParameter("@annotationCharge", ann.Charge);
                    SQLiteParameter parameterAnnAdduct = new SQLiteParameter("@annotationAdduct", adduct);
                    SQLiteParameter parameterAnnComment = new SQLiteParameter("@annotationComment", ann.Comment);
                    SQLiteParameter parameterMzTheor = new SQLiteParameter("@mzTheoretical", valuesMZArray[i]);
                    SQLiteParameter parameterMzObs = new SQLiteParameter("@mzObserved", valuesMZArray[i]);
                    log.Debug("Inserting into RefSpectraPeakAnnotations: " + command.CommandText);
                    command.Parameters.Add(parameterAnnName);
                    command.Parameters.Add(parameterAnnFormula);
                    command.Parameters.Add(parameterAnnCharge);
                    command.Parameters.Add(parameterAnnAdduct);
                    command.Parameters.Add(parameterAnnComment);
                    command.Parameters.Add(parameterMzTheor);
                    command.Parameters.Add(parameterMzObs);

                    command.ExecuteNonQuery();
                }
            }
        }
        
        
        
                
        public static void addSpectra(SQLiteCommand command, PrecursorData precursorData, IDictionary<string, IDictionary<bool, IDictionary<string, MS2Fragment>>> allFragments, IDictionary<String, Precursor> headgroups, CollisionEnergy collisionEnergyHandler, string instrument)
        {
            if (precursorData.fragmentNames.Count == 0 || !allFragments.ContainsKey(precursorData.fullMoleculeListName)) return;
            
            var peaks = new List<Peak>();
            foreach (KeyValuePair<string, MS2Fragment> fragmentPair in allFragments[precursorData.fullMoleculeListName][precursorData.precursorAdduct.charge >= 0])
            {
            
                MS2Fragment fragment = fragmentPair.Value;
                
                // introduce exception for LCB, only HG fragment occurs when LCB contains no double bond
                if (precursorData.moleculeListName.Equals("LCB") && fragment.fragmentName.Equals("HG") && precursorData.lcb.db > 0) continue;
                
                // Exception for lipids with NL(NH3) fragment
                if (fragment.fragmentName.Equals("NL(NH3)") && !precursorData.precursorAdductFormula.Equals("[M+NH4]1+")) continue;
                
                
                // Exception for lipids for NL([adduct]) and +H or -H as adduct
                if (fragment.fragmentName.Equals("NL([adduct])") && (precursorData.precursorAdductFormula.Equals("[M+H]1+") || precursorData.precursorAdductFormula.Equals("[M-H]1-"))) continue;
            
            
            
                ElementDictionary atomsCountFragment = fragment.copyElementDict();
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
                        case "HG":
                            MS2Fragment.addCounts(atomsCountFragment, headgroups[precursorData.fullMoleculeListName].elements);
                            break;
                        default:
                            break;
                    }
                }
                
                /*
                string chemFormFragment = LipidCreator.computeChemicalFormula(atomsCountFragment);
                string fragAdduct = LipidCreator.computeAdductFormula(atomsCountFragment, fragment.fragmentAdduct);
                MS2Fragment.addCounts(atomsCountFragment, fragment.fragmentAdduct.elements);
                double massFragment = LipidCreator.computeMass(atomsCountFragment, fragment.fragmentAdduct.charge);
                string fragName = fragment.fragmentName;
                
                
                
                
                
                
                if (precursorData.lipidCategory == LipidCategory.LipidMediator)
                {
                    massFragment = Convert.ToDouble(fragment.fragmentName.Substring(MEDIATOR_PREFIX_LENGTH), CultureInfo.InvariantCulture);
                    chemFormFragment = "";
                }
                
                
                */
                
                
                
                
                
                
                
                
                string fragName = fragment.fragmentName;
                string chemFormFragment = LipidCreator.computeChemicalFormula(atomsCountFragment);
                string fragAdduct = LipidCreator.computeAdductFormula(atomsCountFragment, fragment.fragmentAdduct);
                MS2Fragment.addCounts(atomsCountFragment, fragment.fragmentAdduct.elements);
                double massFragment = 0;
                
                // Exceptions for mediators
                if (precursorData.lipidCategory != LipidCategory.LipidMediator)
                {
                    try
                    {
                        massFragment = LipidCreator.computeMass(atomsCountFragment, fragment.fragmentAdduct.charge);
                    }
                    catch (LipidException lipidException)
                    {
                        lipidException.precursorData = precursorData;
                        lipidException.fragment = fragment;
                        lipidException.heavyIsotope = LipidCreator.precursorNameSplit(precursorData.fullMoleculeListName)[1];
                        throw lipidException;
                    }
                }
                else
                {
                    massFragment = Convert.ToDouble(fragment.fragmentName.Substring(MEDIATOR_PREFIX_LENGTH), CultureInfo.InvariantCulture);
                    chemFormFragment = "";
                }
                if (fragName.IndexOf("[adduct]") > -1)
                {
                    fragName = fragName.Replace("[adduct]", precursorData.precursorAdduct.name);
                }
                
                if (fragName.IndexOf("[xx:x]") > -1)
                {
                    fragName = fragName.Replace("[xx:x]", precursorData.fa1.ToString(false));
                }
                if (fragName.IndexOf("[yy:y]") > -1)
                {
                    fragName = fragName.Replace("[yy:y]", precursorData.fa2.ToString(false));
                }
                if (fragName.IndexOf("[zz:z]") > -1)
                {
                    fragName = fragName.Replace("[zz:z]", precursorData.fa3.ToString(false));
                }
                if (fragName.IndexOf("[uu:u]") > -1)
                {
                    fragName = fragName.Replace("[uu:u]", precursorData.fa4.ToString(false));
                }
                if (fragName.IndexOf("[xx:x;x]") > -1)
                {
                    fragName = fragName.Replace("[xx:x;x]", precursorData.lcb.ToString());
                }
                
                
                peaks.Add(new Peak(massFragment,
                    MS2Fragment.DEFAULT_INTENSITY,
                    new PeakAnnotation(fragName,
                        fragment.fragmentAdduct.charge,
                        fragAdduct,
                        chemFormFragment,
                        fragment.CommentForSpectralLibrary)));
            }
            
            // add precursor
            peaks.Add(new Peak(precursorData.precursorM_Z,
                MS2Fragment.DEFAULT_INTENSITY,
                new PeakAnnotation("precursor",
                    precursorData.precursorAdduct.charge,
                    precursorData.precursorAdductFormula,
                    precursorData.precursorIonFormula,
                    "precursor")));
                    
            string adduct = LipidCreator.computeAdductFormula(null, precursorData.precursorAdduct);
            
            foreach (Peak peak in peaks)
            {
                string fragment = peak.Annotation.Name;
                double collisionEnergy = collisionEnergyHandler.getCollisionEnergy(instrument, precursorData.fullMoleculeListName, adduct);
                peak.Intensity = MS2Fragment.MAX_INTENSITY * collisionEnergyHandler.getIntensity(instrument, precursorData.fullMoleculeListName, adduct, fragment, collisionEnergy);
            }
            
            // Commit to .blib
            SavePeaks(command, precursorData, peaks);
        }
        
        
        
        public virtual void serialize(StringBuilder sb)
        {
            sb.Append("<representativeFA>" + (representativeFA ? 1 : 0) + "</representativeFA>\n");
            sb.Append("<onlyPrecursors>" + onlyPrecursors + "</onlyPrecursors>\n");
            sb.Append("<onlyHeavyLabeled>" + onlyHeavyLabeled + "</onlyHeavyLabeled>\n");
            foreach (string adduct in adducts.Keys.Where(x => adducts[x]))
            {
                sb.Append("<adduct type=\"" + adduct + "\" />\n");
            }
            foreach (string headgroup in headGroupNames)
            {
                sb.Append("<headGroup>" + headgroup + "</headGroup>\n");
            }
            serializeFragments(sb);
        }
        
        
        public virtual void serializeFragments(StringBuilder sb)
        {
            HashSet<string> headGroupSet = new HashSet<string>();
            
            foreach (string headGroupName in headGroupNames ){
                headGroupSet.Add(headGroupName);
                foreach (Precursor heavyPrecursor in lipidCreator.headgroups[headGroupName].heavyLabeledPrecursors){
                    headGroupSet.Add(heavyPrecursor.name);
                }
            }
            
            
            foreach (KeyValuePair<string, HashSet<string>> positiveFragment in positiveFragments)
            {
                if (headGroupSet.Contains(positiveFragment.Key))
                {
                    sb.Append("<positiveFragments lipidClass=\"" + positiveFragment.Key + "\">\n");
                    foreach (string fragment in positiveFragment.Value)
                    {
                        sb.Append("<fragment>" + fragment + "</fragment>\n");
                    }
                    sb.Append("</positiveFragments>\n");
                }
            }
            
            foreach (KeyValuePair<string, HashSet<string>> negativeFragment in negativeFragments)
            {
                if (headGroupSet.Contains(negativeFragment.Key))
                {
                    sb.Append("<negativeFragments lipidClass=\"" + negativeFragment.Key + "\">\n");
                    foreach (string fragment in negativeFragment.Value)
                    {
                        sb.Append("<fragment>" + fragment + "</fragment>\n");
                    }
                    sb.Append("</negativeFragments>\n");
                }
            }
        }
        
        
        
        
        public Lipid(Lipid copy)
        {
            lipidCreator = copy.lipidCreator;
            adducts = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, bool> adduct in copy.adducts){
                adducts.Add(adduct.Key, adduct.Value);
            }
            representativeFA = copy.representativeFA;
            onlyPrecursors = copy.onlyPrecursors;
            onlyHeavyLabeled = copy.onlyHeavyLabeled;
            headGroupNames = new List<String>();
            lipidCreator.Update += new LipidUpdateEventHandler(this.Update);
        
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
        
        
        
        
        public virtual void import(XElement node, string importVersion)
        {   
            switch (node.Name.ToString())
            {
                    
                case "representativeFA":
                    representativeFA = node.Value == "1";
                    break;
                    
                case "onlyPrecursors":
                    onlyPrecursors = Convert.ToInt32(node.Value.ToString());
                    break;
                    
                case "onlyHeavyLabeled":
                    onlyHeavyLabeled = Convert.ToInt32(node.Value.ToString());
                    break;
                    
                case "adduct":
                    string adductKey = node.Attribute("type").Value.ToString();
                    if (adducts.ContainsKey(adductKey)) adducts[adductKey] = true;
                    break;
                    
                case "headGroup":
                    string headgroup = node.Value.ToString();
                    headGroupNames.Add(headgroup);
                    break;
                    
                case "positiveFragments":
                    string posLipidClass = node.Attribute("lipidClass").Value.ToString();
                    if (!positiveFragments.ContainsKey(posLipidClass)) positiveFragments.Add(posLipidClass, new HashSet<string>());
                    else positiveFragments[posLipidClass].Clear();
                    var posFragments = node.Descendants("fragment");
                    foreach (var fragment in posFragments)
                    {
                        positiveFragments[posLipidClass].Add(fragment.Value.ToString());
                    }
                    break;
                    
                case "negativeFragments":
                    string negLipidClass = node.Attribute("lipidClass").Value.ToString();
                    if (!negativeFragments.ContainsKey(negLipidClass)) negativeFragments.Add(negLipidClass, new HashSet<string>());
                    else negativeFragments[negLipidClass].Clear();
                    var negFragments = node.Descendants("fragment");
                    foreach (var fragment in negFragments)
                    {
                        negativeFragments[negLipidClass].Add(fragment.Value.ToString());
                    }
                    break;
                    
                default:
                    log.Error("Error: " + node.Name.ToString());
                    throw new Exception("Error: " + node.Name.ToString());
            }
        }
    }
    
    
    
    
    public class UnsupportedLipid : Lipid
    {
        public UnsupportedLipid(LipidCreator lipidCreator) : base(lipidCreator, LipidCategory.Unsupported)
        {
        
        }
        
        
        
        
        public override ulong getHashCode()
        {
            unchecked
            {
                return 0UL;
            }
        }
        
        
        
        public override void Update(object sender, EventArgs e)
        {
            Updating((int)LipidCategory.Unsupported);
        }
        
        
        
        public override ArrayList getFattyAcidGroupList()
        {
            return new ArrayList();
        }
        
        
        
        public override void computePrecursorData(IDictionary<String, Precursor> headgroups, HashSet<String> usedKeys, ArrayList precursorDataList)
        {
            PrecursorData precursorData = new PrecursorData();
            precursorData.lipidCategory = LipidCategory.Unsupported;
            precursorData.moleculeListName = "Unsupported lipid";
            precursorData.fullMoleculeListName = "Unsupported lipid";
            precursorData.precursorExportName = "Unsupported lipid";
            precursorData.precursorName = "Unsupported lipid";
            precursorData.precursorSpeciesName = "Unsupported lipid";
            precursorData.precursorIonFormula = "Unsupported lipid";
            precursorData.precursorAdduct = null;
            precursorData.precursorAdductFormula = "Unsupported lipid";
            precursorData.precursorM_Z = 0;
            precursorData.addPrecursor = true;
            precursorData.fragmentNames = new HashSet<string>();
            precursorDataList.Add(precursorData);
        }
    }
}
