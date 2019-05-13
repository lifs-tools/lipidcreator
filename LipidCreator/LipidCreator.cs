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
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.SQLite;
using Ionic.Zlib;

using System.Xml.Linq;
using System.Text;
using SkylineTool;
using System.Net;
using System.Threading;
using System.Security.Cryptography;

using log4net;
using log4net.Config;
using System.Globalization;

namespace LipidCreator
{   
    public delegate void LipidUpdateEventHandler(object sender, EventArgs e);

    public enum MonitoringTypes {NoMonitoring, SRM, PRM};
    public enum PRMTypes {PRMAutomatically, PRMManually};
    public enum RunMode {commandline, standalone, external};

    [Serializable]
    public class LipidCreator : IDisposable
    {
        [NonSerialized]
        private static readonly ILog log = LogManager.GetLogger(typeof(LipidCreator));
        public event LipidUpdateEventHandler Update;
        public static string LC_VERSION_NUMBER = "1.0.0";
        public static PlatformID LC_OS;
        public ArrayList registeredLipids;
        public IDictionary<string, IDictionary<bool, IDictionary<string, MS2Fragment>>> allFragments; // lipid class -> positive charge -> fragment name -> fragment
        public IDictionary<int, ArrayList> categoryToClass;
        public IDictionary<string, Precursor> headgroups;
        public DataTable transitionList;
        public DataTable transitionListUnique;
        public ArrayList precursorDataList;
        [NonSerialized]
        public SkylineToolClient skylineToolClient;
        public bool openedAsExternal;
        public HashSet<string> lysoSphingoLipids;
        public HashSet<string> lysoPhosphoLipids;
        public IDictionary<string, InstrumentData> msInstruments;
        public ArrayList availableInstruments;
        public CollisionEnergy collisionEnergyHandler;
        public bool enableAnalytics = false;
        public static string EXTERNAL_PREFIX_PATH = "Tools/LipidCreator/";
        public string prefixPath = "";
        public RunMode runMode;
        public static string ANALYTICS_CATEGORY;
        
        // collision energy parameters
        public string selectedInstrumentForCE = "";
        public MonitoringTypes monitoringType = MonitoringTypes.NoMonitoring;
        public PRMTypes PRMMode = PRMTypes.PRMAutomatically;
        
        public LipidMapsParserEventHandler lipidMapsParserEventHandler;
        public Parser lipidMapsParser;
        
        public ParserEventHandler parserEventHandler;
        public Parser lipidNamesParser;
        
        ListingParserEventHandler listingParserEventHandler;
        Parser listingParser;
        
        public static char HEAVY_LABEL_OPENING_BRACKET = '{';
        public static char HEAVY_LABEL_CLOSING_BRACKET = '}';
        
        public static int MIN_CARBON_LENGTH = 2;
        public static int MAX_CARBON_LENGTH = 30;
        public static int MIN_DB_LENGTH = 0;
        public static int MAX_DB_LENGTH = 6;
        public static int MIN_HYDROXY_LENGTH = 0;
        public static int MAX_HYDROXY_LENGTH = 10;
        public static int MIN_LCB_HYDROXY_LENGTH = 2;
        public static int MAX_LCB_HYDROXY_LENGTH = 3;
        public static int MIN_SPHINGO_FA_HYDROXY_LENGTH = 0;
        public static int MAX_SPHINGO_FA_HYDROXY_LENGTH = 3;
        
        public const char QUOTE = '"';
        public const string MOLECULE_LIST_NAME = "Molecule List Name";
        public const string PRECURSOR_NAME = "Precursor Name";
        public const string PRECURSOR_NEUTRAL_FORMULA = "Precursor Molecule Formula";
        public const string PRECURSOR_ION_FORMULA = "Precursor Ion Formula";
        public const string PRECURSOR_ADDUCT = "Precursor Adduct";
        public const string PRECURSOR_MZ = "Precursor Ion m/z";
        public const string PRECURSOR_CHARGE = "Precursor Charge";
        public const string PRODUCT_NAME = "Product Name";
        public const string PRODUCT_NEUTRAL_FORMULA = "Product Molecule Formula";
        public const string PRODUCT_ADDUCT = "Product Adduct";
        public const string PRODUCT_MZ = "Product Ion m/z";
        public const string PRODUCT_CHARGE = "Product Charge";
        public const string NOTE = "Note";
        public const string UNIQUE = "unique";
        public const string COLLISION_ENERGY = "Explicit Collision Energy";
        public const string SKYLINE_API_COLLISION_ENERGY = "PrecursorCE";
        public readonly static string[] STATIC_SKYLINE_API_HEADER = {
            "MoleculeGroup",
            "PrecursorName",
            "PrecursorFormula",
            "PrecursorAdduct",
            "PrecursorMz",
            "PrecursorCharge",
            "ProductName",
            "ProductFormula",
            "ProductAdduct",
            "ProductMz",
            "ProductCharge",
            "Note"
        };
        public readonly static string[] STATIC_DATA_COLUMN_KEYS = {
            UNIQUE,
            MOLECULE_LIST_NAME,
            PRECURSOR_NAME,
            PRECURSOR_NEUTRAL_FORMULA,
            PRECURSOR_ADDUCT,
            PRECURSOR_MZ,
            PRECURSOR_CHARGE,
            PRODUCT_NAME,
            PRODUCT_NEUTRAL_FORMULA,
            PRODUCT_ADDUCT,
            PRODUCT_MZ,
            PRODUCT_CHARGE,
            NOTE
        };
        
        public static string[] DATA_COLUMN_KEYS;
        public static string[] SKYLINE_API_HEADER;
        
        public virtual void OnUpdate(EventArgs e)
        {
            LipidUpdateEventHandler handler = Update;
            if (handler != null) handler(this, e);
        }
        
        public void readInputFiles()
        {
            int lineCounter = 1;
            string ms2FragmentsFile = prefixPath + "data/ms2fragments.csv";
            if (File.Exists(ms2FragmentsFile))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(ms2FragmentsFile))
                    {
                        String line = sr.ReadLine(); // omit titles
                        while((line = sr.ReadLine()) != null)
                        {
                            lineCounter++;
                            if (line.Length < 2) continue;
                            if (line[0] == '#') continue;
                            
                            string[] tokens = parseLine(line);
                            
                            if (!allFragments.ContainsKey(tokens[0]))
                            {
                                allFragments.Add(tokens[0], new Dictionary<bool, IDictionary<string, MS2Fragment>>());
                                allFragments[tokens[0]].Add(false, new Dictionary<string, MS2Fragment>());
                                allFragments[tokens[0]].Add(true, new Dictionary<string, MS2Fragment>());
                            }
                            ElementDictionary atomsCount = MS2Fragment.createEmptyElementDict();
                            atomsCount[Molecule.C] = Convert.ToInt32(tokens[6]);
                            atomsCount[Molecule.H] = Convert.ToInt32(tokens[7]);
                            atomsCount[Molecule.O] = Convert.ToInt32(tokens[8]);
                            atomsCount[Molecule.N] = Convert.ToInt32(tokens[9]);
                            atomsCount[Molecule.P] = Convert.ToInt32(tokens[10]);
                            atomsCount[Molecule.S] = Convert.ToInt32(tokens[11]);
                            string fragmentFile = prefixPath + tokens[3];
                            if (tokens[3] != "%" && !File.Exists(fragmentFile))
                            {
                                log.Error("Error in line (" + lineCounter + "): file '" + fragmentFile + "' does not exist or can not be opened.");
                            }
                            int charge = Convert.ToInt32(tokens[4]);
                            Adduct adduct = Lipid.chargeToAdduct[charge];
                            if (tokens[12].Length > 0)
                            {
                                allFragments[tokens[0]][charge >= 0].Add(tokens[2], new MS2Fragment(tokens[2], tokens[1], adduct, fragmentFile, atomsCount, tokens[5], Convert.ToDouble(tokens[12])));
                            }
                            else 
                            {
                                allFragments[tokens[0]][charge >= 0].Add(tokens[2], new MS2Fragment(tokens[2], tokens[1], adduct, fragmentFile, atomsCount, tokens[5]));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error("The file '" + ms2FragmentsFile + "' in line '" + lineCounter + "' could not be read:", e);
                }
            }
            else
            {
                log.Error("Error: file '" + ms2FragmentsFile + "' does not exist or can not be opened.");
            }
            
            
            
            string headgroupsFile = prefixPath + "data/headgroups.csv";
            if (File.Exists(headgroupsFile))
            {
                lineCounter = 1;
                try
                {
                    using (StreamReader sr = new StreamReader(headgroupsFile))
                    {
                        String line = sr.ReadLine(); // omit titles
                        while((line = sr.ReadLine()) != null)
                        {
                            lineCounter++;
                            if (line.Length < 2) continue;
                            if (line[0] == '#') continue;
                            
                            string[] tokens = parseLine(line);
                            if (tokens.Length < 20) throw new Exception("invalid line in file, number of columns in line < 19");
                            
                            Precursor headgroup = new Precursor();
                            //headgroup.catogory
                            switch(tokens[0])
                            {
                                case "GL":
                                    headgroup.category = LipidCategory.Glycerolipid;
                                    break;
                                case "PL":
                                    headgroup.category = LipidCategory.Glycerophospholipid;
                                    break;
                                case "SL":
                                    headgroup.category = LipidCategory.Sphingolipid;
                                    break;
                                case "Mediator":
                                    headgroup.category = LipidCategory.LipidMediator;
                                    break;
                                case "Cholesterol":
                                    headgroup.category = LipidCategory.Sterollipid;
                                    break;
                                default:
                                    throw new Exception("invalid lipid category");
                            }
                            if (!categoryToClass.ContainsKey((int)headgroup.category)) categoryToClass.Add((int)headgroup.category, new ArrayList());
                            categoryToClass[(int)headgroup.category].Add(tokens[1]);
                            
                            headgroup.name = tokens[1];
                            headgroup.elements[Molecule.C] = Convert.ToInt32(tokens[2]); // carbon
                            headgroup.elements[Molecule.H] = Convert.ToInt32(tokens[3]); // hydrogen
                            headgroup.elements[Molecule.H2] = Convert.ToInt32(tokens[8]); // hydrogen 2
                            headgroup.elements[Molecule.O] = Convert.ToInt32(tokens[4]); // oxygen
                            headgroup.elements[Molecule.N] = Convert.ToInt32(tokens[5]); // nytrogen
                            headgroup.elements[Molecule.P] = Convert.ToInt32(tokens[6]); // phosphor
                            headgroup.elements[Molecule.S] = Convert.ToInt32(tokens[7]); // sulfor
                            string precursorFile = prefixPath + tokens[9];
                            if (!File.Exists(precursorFile))
                            {
                                throw new Exception("Error (" + lineCounter + "): precursor file " + precursorFile + " does not exist or can not be opened.");
                            }
                            headgroup.pathToImage = precursorFile;
                            headgroup.adductRestrictions.Add("+H", tokens[10].Equals("Yes"));
                            headgroup.adductRestrictions.Add("+2H", tokens[11].Equals("Yes"));
                            headgroup.adductRestrictions.Add("+NH4", tokens[12].Equals("Yes"));
                            headgroup.adductRestrictions.Add("-H", tokens[13].Equals("Yes"));
                            headgroup.adductRestrictions.Add("-2H", tokens[14].Equals("Yes"));
                            headgroup.adductRestrictions.Add("+HCOO", tokens[15].Equals("Yes"));
                            headgroup.adductRestrictions.Add("+CH3COO", tokens[16].Equals("Yes"));
                            headgroup.defaultAdduct = tokens[17];
                            headgroup.buildingBlockType = Convert.ToInt32(tokens[18]);
                            if (tokens[19].Length > 0) headgroup.attributes = new HashSet<string>(tokens[19].Split(new char[]{';'}));
                            headgroup.derivative = headgroup.attributes.Contains("lyso") || headgroup.attributes.Contains("ether");
                            
                            if (headgroup.attributes.Contains("heavy"))
                            {
                                string monoName = precursorNameSplit(headgroup.name)[0];
                                if (headgroups.ContainsKey(monoName))
                                {
                                    headgroups[monoName].heavyLabeledPrecursors.Add(headgroup);
                                }
                                else
                                {
                                    throw new Exception("cannot find monoisotopic class");
                                }
                            }
                            
                            headgroups.Add(headgroup.name, headgroup);
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error("The file '" + headgroupsFile + "' in line '" + lineCounter + "' could not be read:", e);
                }
            }
            else
            {
                log.Error("Error: file " + headgroupsFile + " does not exist or can not be opened.");
            }
            
            HashSet<string>[] buildingBlockSets = new HashSet<string>[7];
            buildingBlockSets[0] = new HashSet<string>{"FA1", "FA2", "FA3", "FA4", "HG"};
            buildingBlockSets[1] = new HashSet<string>{"FA1", "FA2", "FA3", "HG"};
            buildingBlockSets[2] = new HashSet<string>{"FA1", "FA2", "HG"};
            buildingBlockSets[3] = new HashSet<string>{"FA", "HG"};
            buildingBlockSets[4] = new HashSet<string>{"LCB", "FA", "HG"};
            buildingBlockSets[5] = new HashSet<string>{"LCB", "HG"};
            buildingBlockSets[6] = new HashSet<string>{"HG"};
            
            // check fragment building block list against precursor type
            foreach (KeyValuePair<string, Precursor> kvpHeadgroups in headgroups)
            {
                int headgroupType = kvpHeadgroups.Value.buildingBlockType;
                string headgroupName = kvpHeadgroups.Value.name;
                if (!allFragments.ContainsKey(headgroupName)) continue;
                
                foreach (MS2Fragment ms2fragment in allFragments[headgroupName][true].Values)
                {
                    HashSet<string> blocks = new HashSet<string>();
                    foreach (string fragmentBase in ms2fragment.fragmentBase) blocks.Add(fragmentBase);
                    blocks.ExceptWith(buildingBlockSets[headgroupType]);
                    if (blocks.Count > 0)
                    {
                        log.Error("Error: building blocks of fragement '" + headgroupName + " / " + ms2fragment.fragmentName + "' do not match with 'Building Blocks' type in headgroups file.");
                    }
                }
            }
            
            
            
            
            string instrumentsFile = prefixPath + "data/ms-instruments.csv";
            if (File.Exists(instrumentsFile))
            {
                lineCounter = 1;
                try
                {
                    using (StreamReader sr = new StreamReader(instrumentsFile))
                    {
                        String line = sr.ReadLine(); // omit titles
                        while((line = sr.ReadLine()) != null)
                        {
                            lineCounter++;
                            if (line.Length < 2) continue;
                            if (line[0] == '#') continue;
                            
                            string[] tokens = parseLine(line);
                            if (tokens.Length != 6) throw new Exception("invalid line in file, number of columns in line != 6");
                            
                            
                            InstrumentData instrumentData = new InstrumentData();
                            instrumentData.CVTerm = tokens[0];
                            instrumentData.model = tokens[1];
                            double.TryParse(tokens[2], out instrumentData.minCE);
                            double.TryParse(tokens[3], out instrumentData.maxCE);
                            instrumentData.xAxisLabel = tokens[4];
                            instrumentData.modes = new HashSet<string>(tokens[5].Split(new char[]{';'}));
                            msInstruments.Add(instrumentData.CVTerm, instrumentData);
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error("The file '" + instrumentsFile + "' in line '" + lineCounter + "' could not be read:", e);
                }
            }
            else
            {
                log.Error("Error: file " + instrumentsFile + " does not exist or can not be opened.");
            }
            
            
            
            string ceParametersDir = prefixPath + "data/ce-parameters";
            if (Directory.Exists(ceParametersDir))
            {
                string[] ceFilePaths = Directory.GetFiles(prefixPath + "data/ce-parameters/", "*.csv", SearchOption.TopDirectoryOnly);
                foreach(string ceParametersFile in ceFilePaths)
                {
                    lineCounter = 0;
                    try
                    {
                        using (StreamReader sr = new StreamReader(ceParametersFile))
                        {
                            String line = null;
                            Dictionary<String, int> columnKeys = null;
                            int nTokens = -1;
                            while((line = sr.ReadLine()) != null)
                            {
                                lineCounter++;
                                if (line.Length < 2) continue;
                                if (line[0] == '#') continue;
                                string[] tokens = parseLine(line);
                                //initialize column header key lookup
                                if (columnKeys == null)
                                {
                                    nTokens = tokens.Length;
                                    log.Debug("Parsing line " + lineCounter + " " + line);
                                    log.Debug("CE Parameter file header: " + string.Join(", ", tokens) + " for file " + ceParametersFile);
                                    columnKeys = tokens.Select((value, index) => new { value, index })
                                        .ToDictionary(pair => pair.value, pair => pair.index);
                                    log.Debug("Column Keys: " + string.Join(", ", columnKeys));
                                }
                                else
                                {

                                    if (tokens.Length != nTokens)
                                    {
                                        log.Error("Mismatch on line " + lineCounter + "! Should have " + nTokens + " columns, but had " + tokens.Length);
                                        throw new Exception("Invalid line in file, number of columns in line must equal number of columns in header!");
                                    }

                                    string instrument = tokens[columnKeys["instrument"]];
                                    string lipidClass = tokens[columnKeys["class"]];
                                    string precursorAdduct = tokens[columnKeys["precursorAdduct"]];
                                    string fragment = tokens[columnKeys["fragment"]];
                                    string paramKey = tokens[columnKeys["ParKey"]];
                                    string paramValue = tokens[columnKeys["ParValue"]];


                                    if (!collisionEnergyHandler.instrumentParameters.ContainsKey(instrument))
                                    {
                                        collisionEnergyHandler.instrumentParameters.Add(instrument, new Dictionary<string, IDictionary<string, IDictionary<string, IDictionary<string, string>>>>());
                                    }

                                    if (!collisionEnergyHandler.instrumentParameters[instrument].ContainsKey(lipidClass))
                                    {
                                        collisionEnergyHandler.instrumentParameters[instrument].Add(lipidClass, new Dictionary<string, IDictionary<string, IDictionary<string, string>>>());
                                    }

                                    if (!collisionEnergyHandler.instrumentParameters[instrument][lipidClass].ContainsKey(precursorAdduct))
                                    {
                                        collisionEnergyHandler.instrumentParameters[instrument][lipidClass].Add(precursorAdduct, new Dictionary<string, IDictionary<string, string>>());
                                    }

                                    if (!collisionEnergyHandler.instrumentParameters[instrument][lipidClass][precursorAdduct].ContainsKey(fragment))
                                    {
                                        collisionEnergyHandler.instrumentParameters[instrument][lipidClass][precursorAdduct].Add(fragment, new Dictionary<string, string>());
                                    }

                                    if (!collisionEnergyHandler.instrumentParameters[instrument][lipidClass][precursorAdduct][fragment].ContainsKey(paramKey))
                                    {
                                        collisionEnergyHandler.instrumentParameters[instrument][lipidClass][precursorAdduct][fragment].Add(paramKey, paramValue);
                                    }
                                    else
                                    {
                                        throw new Exception("ParamKey for " + instrument + " " + lipidClass + " " + precursorAdduct + " " + fragment + " " + paramKey + " was already assigned! ParamKeys can only be assigned once for any unique combination!");
                                    }
                                }
                            }
                        }

                    
                    }
                    catch (Exception e)
                    {
                        log.Error("Encountered an error in file '" + ceParametersFile + "' on line '" + lineCounter + "':", e);
                    }
                }
            }
            else
            {
                log.Error("Error: directory " + ceParametersDir + " does not exist or can not be opened.");
            }
            
            string analyticsFile = prefixPath + "data/analytics.txt";
            try {
                if (File.Exists(analyticsFile))
                {
                    {
                        using (StreamReader sr = new StreamReader(analyticsFile))
                        {
                            // check if first letter in first line is a '1'
                            String line = sr.ReadLine();
                            enableAnalytics = line[0] == '1';
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Warn("Warning: Analytics file could not be opened at " + analyticsFile + ". LipidCreator will continue without analytics enabled!", e);
            }
        }
        

        
        public LipidCreator(string pipe)
        {
            openedAsExternal = (pipe != null);
            skylineToolClient = openedAsExternal ? new SkylineToolClient(pipe, "LipidCreator") : null;
            prefixPath = (openedAsExternal ? EXTERNAL_PREFIX_PATH : "");
            XmlConfigurator.Configure(new System.IO.FileInfo(prefixPath + "data/log4net.xml"));
            LC_VERSION_NUMBER = Application.ProductVersion;
            LC_OS = Environment.OSVersion.Platform;
            ANALYTICS_CATEGORY = "lipidcreator-" + LC_VERSION_NUMBER;
            log.Info("Running LipidCreator version " + LC_VERSION_NUMBER + " in " + (skylineToolClient == null ? "standalone":"skyline tool") + " mode on " + LC_OS.ToString());
            registeredLipids = new ArrayList();
            categoryToClass = new Dictionary<int, ArrayList>();
            allFragments = new Dictionary<string, IDictionary<bool, IDictionary<string, MS2Fragment>>>();
            headgroups = new Dictionary<String, Precursor>();
            precursorDataList = new ArrayList();
            lysoSphingoLipids = new HashSet<string>();
            lysoPhosphoLipids = new HashSet<string>();
            msInstruments = new Dictionary<string, InstrumentData>();
            collisionEnergyHandler = new CollisionEnergy();
            availableInstruments = new ArrayList();
            availableInstruments.Add("");
            readInputFiles();
            collisionEnergyHandler.addCollisionEnergyFields();
            
            foreach(string instrument in collisionEnergyHandler.instrumentParameters.Keys) {
                availableInstruments.Add(instrument);
            }
            
            foreach(string lipidClass in allFragments.Keys)
            {
                if (!headgroups.ContainsKey(lipidClass))
                {
                    log.Error("Inconsistency of fragment lipid classes: '" + lipidClass + "' doesn't occur in headgroups table");
                }
            }
            
            foreach(string lipidClass in headgroups.Keys)
            {
                if (!allFragments.ContainsKey(lipidClass))
                {
                    log.Error("Inconsistency of fragment lipid classes: '" + lipidClass + "' doesn't occur in fragments table");
                }
            }
            
            
            lipidMapsParserEventHandler = new LipidMapsParserEventHandler(this);
            lipidMapsParser = new Parser(lipidMapsParserEventHandler, prefixPath + "data/lipidmaps.grammar", QUOTE);
            
            parserEventHandler = new ParserEventHandler(this);
            lipidNamesParser = new Parser(parserEventHandler, prefixPath + "data/lipidnames.grammar", QUOTE);
            
            listingParserEventHandler = new ListingParserEventHandler();
            listingParser = new Parser(listingParserEventHandler, prefixPath + "data/listing.grammar", QUOTE);
        }
        
        
        // parser for reading the csv lines with comma separation and "" quotation (if present)
        // using a Moore automaton based approach
        public static string[] parseLine(string line, char separator = ',', char quote = QUOTE)
        {
            List<string> listTokens = new List<string>();
            int start = 0;
            int length = 0;
            int state = 1;
            for (int i = 0; i < line.Length; ++i)
            {
                switch (state)
                {
                    case 0:
                        if (line[i] == quote)
                        {
                            throw new Exception("invalid line in file");
                        }
                        else if (line[i] == separator)
                        {
                            listTokens.Add(line.Substring(start, length));
                            length = 0;
                            state = 1;
                        }
                        else
                        {
                            ++length;
                        }
                        break;
                        
                    case 1:
                        if (line[i] == quote)
                        {
                            length = 0;
                            start = i + 1;
                            state = 2;
                        }
                        else if (line[i] == separator)
                        {
                            listTokens.Add("");
                            length = 0;
                        }
                        else
                        {
                            length = 1;
                            start = i;
                            state = 0;
                        }
                        break;
                        
                    case 2:
                        if (line[i] != quote) ++length;
                        else state = 3;
                        break;
                        
                    case 3:
                        if (line[i] == separator)
                        {
                            listTokens.Add(line.Substring(start, length));
                            length = 0;
                            state = 1;
                        }    
                        else throw new Exception("invalid line in file");
                        break;
                }
            }
            if (state != 2) listTokens.Add(line.Substring(start, length));
            else throw new Exception("invalid line in file");
            
            return listTokens.ToArray();
        }


        
        
        // obType (Object type): 0 = carbon length, 1 = carbon length odd, 2 = carbon length even, 3 = db length, 4 = hydroxyl length
        public HashSet<int> parseRange(string text, int lower, int upper, int obType = 0)
        {
            int oddEven = (obType <= 2) ? obType : 0;
            if (text.Length == 0) return null;
            
            text = text.Replace(" ", "");
            
            listingParserEventHandler.changeOddEvenFlag(oddEven);
            listingParser.parse(text);
            if (listingParser.wordInGrammar)
            {
                listingParser.raiseEvents();
                if (lower <= listingParserEventHandler.min && listingParserEventHandler.max <= upper)
                {
                    return listingParserEventHandler.counts;
                }
            }
            
            return null;
        }
        
        
        
        
        public void createPrecursorList()
        {
            HashSet<String> usedKeys = new HashSet<String>();
            precursorDataList.Clear();
            
            // create precursor list
            foreach (Lipid currentLipid in registeredLipids)
            {
                currentLipid.computePrecursorData(headgroups, usedKeys, precursorDataList);
            }
        }
        
        
        
        
        
        public void createFragmentList(string instrument, MonitoringTypes monitoringType)
        {
            analytics(ANALYTICS_CATEGORY, "create-transition-list-" + runMode);
            
            transitionList = addDataColumns(new DataTable ());
            transitionListUnique = addDataColumns (new DataTable ());
            
            // create fragment list   
            if (instrument.Length == 0)
            {         
                foreach (PrecursorData precursorData in this.precursorDataList)
                {
                    if (precursorData.precursorSelected)
                    {
                        Lipid.computeFragmentData (transitionList, precursorData, allFragments);
                    }
                }
            }
            else 
            {
                double minCE = msInstruments[instrument].minCE;
                double maxCE = msInstruments[instrument].maxCE;
                foreach (PrecursorData precursorData in this.precursorDataList)
                {
                    if (!precursorData.precursorSelected) continue;
                
                    double CE = -1;
                    string precursorName = precursorData.fullMoleculeListName;
                    string adduct = computeAdductFormula(null, precursorData.precursorAdduct);
                    if (PRMMode == PRMTypes.PRMAutomatically)
                    {
                        collisionEnergyHandler.computeDefaultCollisionEnergy(msInstruments[instrument], precursorName, adduct);
                        CE = collisionEnergyHandler.getCollisionEnergy(instrument, precursorName, adduct);
                    }
                    else if (PRMMode == PRMTypes.PRMManually)
                    {
                        
                        if (collisionEnergyHandler.getCollisionEnergy(instrument, precursorName, adduct) == -1)
                        {
                            collisionEnergyHandler.computeDefaultCollisionEnergy(msInstruments[instrument], precursorName, adduct);
                        }
                        CE = collisionEnergyHandler.getCollisionEnergy(instrument, precursorName, adduct);
                    }
                    Lipid.computeFragmentData(transitionList, precursorData, allFragments, collisionEnergyHandler, instrument, monitoringType, CE, minCE, maxCE);
                }
            }
            
            
            // check for duplicates
            IDictionary<String, ArrayList> replicateKeys = new Dictionary<String, ArrayList> ();
            foreach (DataRow row in transitionList.Rows)
            {
                string prec_mass = string.Format("{0:N4}%", (String)row [LipidCreator.PRECURSOR_MZ]);
                string prod_mass = string.Format("{0:N4}%", (((String)row [LipidCreator.PRODUCT_NEUTRAL_FORMULA]) != "" ? (String)row [LipidCreator.PRODUCT_MZ] : (String)row [LipidCreator.PRODUCT_NAME]));
                string replicateKey = prec_mass + "/" + prod_mass;
                if (!replicateKeys.ContainsKey (replicateKey)) replicateKeys.Add(replicateKey, new ArrayList());
                replicateKeys[replicateKey].Add(row);
            }
                
            foreach (string replicateKey in replicateKeys.Keys)
            {
                DataRow row = (DataRow)replicateKeys[replicateKey][0];
                
                
                if (replicateKeys[replicateKey].Count > 1)
                {
                    for (int i = 0; i < replicateKeys[replicateKey].Count; ++i)
                    {
                        DataRow dr1 = (DataRow)replicateKeys[replicateKey][i];
                        dr1[UNIQUE] = false;
                        
                        string note = "";
                        for (int j = 0; j < replicateKeys[replicateKey].Count; ++j)
                        {
                            if (i == j) continue;
                            DataRow dr2 = (DataRow)replicateKeys[replicateKey][j];
                            
                            if (note.Length > 0)
                            {
                                note += " and with ";
                            }
                            
                            else
                            {
                                note = "Interference with ";
                            }
                            note += (string)dr2[LipidCreator.PRECURSOR_NAME] + " " + (string)dr2[LipidCreator.PRECURSOR_ADDUCT] + " " + (string)dr2[LipidCreator.PRODUCT_NAME];
                        }
                        dr1[LipidCreator.NOTE] = note;
                    }
                }
                else
                {
                    row[UNIQUE] = true;
                }
                transitionListUnique.ImportRow (row);
            }
        }
        
        
        
        public void assembleLipids(bool asDeveloper)
        {

            List<string> headerList = new List<string>();
            headerList.AddRange(STATIC_DATA_COLUMN_KEYS);
            if (selectedInstrumentForCE.Length > 0) headerList.Add(COLLISION_ENERGY);
            DATA_COLUMN_KEYS = headerList.ToArray();
            
            
            List<string> apiList = new List<string>();
            apiList.AddRange(STATIC_SKYLINE_API_HEADER);
            if (selectedInstrumentForCE.Length > 0) apiList.Add(SKYLINE_API_COLLISION_ENERGY);
            SKYLINE_API_HEADER = apiList.ToArray();
            
            createPrecursorList();
            if (asDeveloper)
            {
                ElementDictionary emptyAtomsCount = MS2Fragment.createEmptyElementDict();
                foreach (PrecursorData precursorData in precursorDataList)
                {
                    precursorData.precursorName = precursorData.fullMoleculeListName;
                    precursorData.precursorAdductFormula = computeAdductFormula(emptyAtomsCount, precursorData.precursorAdduct);
                }
            }
            
            createFragmentList(selectedInstrumentForCE, monitoringType);
        }
        
        
        
        public void assemblePrecursors()
        {

            List<string> headerList = new List<string>();
            headerList.AddRange(STATIC_DATA_COLUMN_KEYS);
            if (selectedInstrumentForCE.Length > 0) headerList.Add(COLLISION_ENERGY);
            DATA_COLUMN_KEYS = headerList.ToArray();
            
            
            List<string> apiList = new List<string>();
            apiList.AddRange(STATIC_SKYLINE_API_HEADER);
            if (selectedInstrumentForCE.Length > 0) apiList.Add(SKYLINE_API_COLLISION_ENERGY);
            SKYLINE_API_HEADER = apiList.ToArray();
            
            createPrecursorList();
        }
        
        
        
        public void assembleFragments(bool asDeveloper)
        {
            if (asDeveloper)
            {
                ElementDictionary emptyAtomsCount = MS2Fragment.createEmptyElementDict();
                foreach (PrecursorData precursorData in precursorDataList)
                {
                    precursorData.precursorName = precursorData.fullMoleculeListName;
                    precursorData.precursorAdductFormula = computeAdductFormula(emptyAtomsCount, precursorData.precursorAdduct);
                }
            }
            
            createFragmentList(selectedInstrumentForCE, monitoringType);
        }
        
        
        
        public int[] importLipidList (string lipidListFile, int[] filterParameters = null)
        {
            if (File.Exists(lipidListFile))
            {
                int total = 0;
                int valid = 0;
                try
                {
                    ArrayList lipidsToImport = new ArrayList();
                    using (StreamReader sr = new StreamReader(lipidListFile))
                    {
                        string line;
                        while((line = sr.ReadLine()) != null)
                        {
                            foreach (string lipidName in parseLine(line))
                            {
                                ++total;
                                if (lipidName.Length == 0) continue;
                                lipidsToImport.Add(lipidName);
                            }
                        }
                                
                        ArrayList importedLipids = translate(lipidsToImport, true);
                        foreach (Lipid lipid in importedLipids)
                        {
                            if (filterParameters != null)
                            {
                                lipid.onlyPrecursors = filterParameters[0];
                                lipid.onlyHeavyLabeled = filterParameters[1];
                            }
                            registeredLipids.Add(lipid);
                            ++valid;
                        }
                    }
                }
                
                catch (Exception ee)
                {
                    log.Error("Reading lipids from file " + lipidListFile + " failed on line " + total, ee);
                }
                return new int[]{valid, total};
            }
            else
            {
                throw new Exception("Could not read file, " + lipidListFile);
            }
        }
        
        
        
        
        
        public void storeTransitionList(string separator, bool split, string filename, DataTable currentView, string mode = ".csv")
        {
        
            string outputDir = System.IO.Path.GetDirectoryName(filename);
            if (outputDir.Length > 0) System.IO.Directory.CreateDirectory(outputDir);
            if (!filename.EndsWith(mode)) filename += mode;
            if (split)
            {
                using (StreamWriter outputFile = new StreamWriter (filename.Replace (mode, "_positive" + mode)))
                {
                    outputFile.WriteLine (toHeaderLine (separator, LipidCreator.SKYLINE_API_HEADER));
                    foreach (DataRow row in currentView.Rows)
                    {
                        if (((String)row [LipidCreator.PRECURSOR_CHARGE]) == "+1" || ((String)row [LipidCreator.PRECURSOR_CHARGE]) == "+2")
                        {
                            outputFile.WriteLine (toLine (row, LipidCreator.DATA_COLUMN_KEYS, separator));
                        }
                    }
                }
                using (StreamWriter outputFile = new StreamWriter (filename.Replace (mode, "_negative" + mode)))
                {
                    outputFile.WriteLine (toHeaderLine (separator, LipidCreator.SKYLINE_API_HEADER));
                    foreach (DataRow row in currentView.Rows)
                    {
                        if (((String)row [LipidCreator.PRECURSOR_CHARGE]) == "-1" || ((String)row [LipidCreator.PRECURSOR_CHARGE]) == "-2")
                        {
                            outputFile.WriteLine (toLine (row, LipidCreator.DATA_COLUMN_KEYS, separator));
                        }
                    }
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine(toHeaderLine(separator, LipidCreator.SKYLINE_API_HEADER));
                    foreach (DataRow row in currentView.Rows)
                    {
                        writer.WriteLine(toLine(row, LipidCreator.DATA_COLUMN_KEYS, separator));
                    }
                }
            }
        }
        

        public static string toHeaderLine(string separator, string[] columnKeys)
        {
            string quote = "";
            if(separator==",")
            {
                quote = "\"";
            }
            return String.Join(separator, columnKeys.ToList().ConvertAll<string>(key => quote+key+quote).ToArray());
        }
        
        

        public static string toLine (DataRow row, string[] columnKeys, string separator)
        {
            List<string> line = new List<string> ();
            foreach (string columnKey in columnKeys) {
                if (columnKey == UNIQUE) continue;
                if (columnKey == LipidCreator.PRODUCT_MZ || columnKey == LipidCreator.PRECURSOR_MZ)
                {
                    line.Add (((String)row [columnKey]).Replace (",", "."));
                }
                else
                {
                    //quote strings when we are in csv mode
                    if (separator == ",")
                    {
                        line.Add("\""+((String)row[columnKey])+"\"");
                    }
                    else
                    { //otherwise just add the plain string
                        line.Add(((String)row[columnKey]));
                    }
                }
            }
            return String.Join (separator, line.ToArray ());
        }
        
        
        
        public static string computeChemicalFormula(ElementDictionary elements)
        {
            String chemForm = "";            
            foreach (Molecule molecule in MS2Fragment.ALL_ELEMENTS.Keys.OrderBy(x => MS2Fragment.ALL_ELEMENTS[x].position).Where(x => !MS2Fragment.ALL_ELEMENTS[x].isHeavy))
            {
                int numElements = elements[molecule];
                foreach (Molecule heavyMolecule in MS2Fragment.ALL_ELEMENTS[molecule].derivatives)
                {
                    numElements += elements[heavyMolecule];
                }
            
                if (numElements > 0)
                {
                    chemForm += MS2Fragment.ALL_ELEMENTS[molecule].shortcut + ((numElements > 1) ? Convert.ToString(numElements) : "");
                }
            }
            return chemForm;
        }
        
        
        
        
        public static string computeAdductFormula(ElementDictionary elements, Adduct adduct, int charge = 0)
        {
            if (charge == 0) charge = adduct.charge;
            
            String adductForm = "[M";
            if (elements != null)
            {
                foreach (Molecule molecule in MS2Fragment.ALL_ELEMENTS.Keys.Where(x => MS2Fragment.ALL_ELEMENTS[x].isHeavy))
                {
                    if (elements[molecule] > 0)
                    {
                        adductForm += Convert.ToString(elements[molecule]) + MS2Fragment.ALL_ELEMENTS[molecule].shortcutIUPAC;
                    }
                }
            }
            adductForm += adduct.name + "]";
            adductForm += Convert.ToString(Math.Abs(charge));
            adductForm += (charge > 0) ? "+" : "-";
            return adductForm;
        }
        
        
        
        public static string computeHeavyIsotopeLabel(ElementDictionary elements)
        {
            string label = "";
            foreach (Molecule molecule in MS2Fragment.ALL_ELEMENTS.Keys.Where(x => MS2Fragment.ALL_ELEMENTS[x].isHeavy))
            {
                if (elements[molecule] > 0)
                {
                    label += MS2Fragment.ALL_ELEMENTS[molecule].shortcutNomenclature + Convert.ToString(elements[molecule]);
                }
            }
            if (label.Length > 0) label = "(+" + label + ")";
            return label;
        }

        
        
        
        public static double computeMass(ElementDictionary elements, double charge)
        {
            double mass = 0;
            foreach (KeyValuePair<Molecule, int> row in elements)
            {
                mass += row.Value * MS2Fragment.ALL_ELEMENTS[row.Key].mass;
            }
            return mass - charge * 0.00054857990946;
        }
        
        
        
        public void sendToSkyline(DataTable dt, string blibName, string blibFile)
        {
            if (skylineToolClient == null) return;
            
            
            Dictionary<string, string> nameToExportName = new Dictionary<string, string>();
            foreach (PrecursorData precursorData in precursorDataList)
            {
                if (!nameToExportName.ContainsKey(precursorData.precursorName)) nameToExportName.Add(precursorData.precursorName, precursorData.precursorExportName);
            }            
            

            string header = string.Join(",", SKYLINE_API_HEADER);

            StringBuilder sb = new StringBuilder(header, header.Length);
            sb.AppendLine();
            double maxMass = 0;
            bool withCE = SKYLINE_API_HEADER[SKYLINE_API_HEADER.Length - 1].Equals(SKYLINE_API_COLLISION_ENERGY);
            
            foreach (DataRow entry in dt.Rows)
            {
                try
                {                    
                    string exportName = nameToExportName.ContainsKey((string)entry[LipidCreator.PRECURSOR_NAME]) ? nameToExportName[(string)entry[LipidCreator.PRECURSOR_NAME]] : (string)entry[LipidCreator.PRECURSOR_NAME];
                    // Default col order is listname, preName, PreFormula, preAdduct, preMz, preCharge, prodName, ProdFormula, prodAdduct, prodMz, prodCharge
                    sb.Append("\"").Append(entry[LipidCreator.MOLECULE_LIST_NAME]).Append("\","); // listname
                    sb.Append("\"").Append(exportName).Append("\","); // preName
                    sb.Append("\"").Append(entry[LipidCreator.PRECURSOR_NEUTRAL_FORMULA]).Append("\","); // PreFormula
                    sb.Append("\"").Append(entry[LipidCreator.PRECURSOR_ADDUCT]).Append("\","); // preAdduct
                    sb.Append("\"").Append(entry[LipidCreator.PRECURSOR_MZ]).Append("\","); // preMz
                    maxMass = Math.Max(maxMass, Convert.ToDouble((string)entry[LipidCreator.PRECURSOR_MZ], CultureInfo.InvariantCulture));
                    sb.Append("\"").Append(entry[LipidCreator.PRECURSOR_CHARGE]).Append("\","); // preCharge
                    sb.Append("\"").Append(entry[LipidCreator.PRODUCT_NAME]).Append("\","); // prodName
                    sb.Append("\"").Append(entry[LipidCreator.PRODUCT_NEUTRAL_FORMULA]).Append("\","); // ProdFormula, no prodAdduct
                    sb.Append("\"").Append(entry[LipidCreator.PRODUCT_ADDUCT]).Append("\","); // preAdduct
                    sb.Append("\"").Append(entry[LipidCreator.PRODUCT_MZ]).Append("\","); // prodMz
                    sb.Append("\"").Append(entry[LipidCreator.PRODUCT_CHARGE]).Append("\","); // prodCharge
                    sb.Append("\"").Append(entry[LipidCreator.NOTE]).Append("\""); // note
                    if (withCE) sb.Append(",\"").Append(entry[LipidCreator.COLLISION_ENERGY]).Append("\""); // note
                    sb.AppendLine();
                } 
                catch(Exception e)
                {
                    MessageBox.Show("An error occured during creation of the transition list!");
                    log.Error("An error occured during creation of the transition list: ", e);
                }
            }
            try
            {
                skylineToolClient.InsertSmallMoleculeTransitionList(sb.ToString());
                if (blibName.Length > 0 && blibFile.Length > 0) skylineToolClient.AddSpectralLibrary(blibName, blibFile);
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occured, data could not be sent to Skyline, please check if your Skyline parameters allow precursor masses up to " + maxMass.ToString(CultureInfo.InvariantCulture) + "Da.");
                log.Error("An error occured, data could not be sent to Skyline, please check if your Skyline parameters allow precursor masses up to " + maxMass.ToString(CultureInfo.InvariantCulture) + "Da.", e);
            }
        }
        
        
        
        
        public static RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
    
    
    
        
        public static ArrayList createRandomLipidNames(int num = 1)
        {
        
            string grammarFilename = "data/lipidnames.grammar";
            char quote = '"';
            ArrayList lipidNames = new ArrayList();
            if (File.Exists(grammarFilename))
            {
                int lineCounter = 0;
                int ruleNum = 1;
                IDictionary<int, ArrayList> rules = new Dictionary<int, ArrayList>();
                IDictionary<int, string> terminals = new Dictionary<int, string>();
                IDictionary<string, int> ruleToNT = new Dictionary<string, int>();
                using (StreamReader sr = new StreamReader(grammarFilename))
                {
                    string line;
                    while((line = sr.ReadLine()) != null)
                    {
                        lineCounter++;
                        // skip empty lines and comments
                        if (line.Length < 1) continue;
                        if (line.IndexOf("#") > -1) line = line.Substring(0, line.IndexOf("#"));
                        if (line.Length < 1) continue;
                        line = Parser.strip(line, ' ');
                        if (line.Length < 2) continue;
                        
                        ArrayList tokens_level_1 = new ArrayList();
                        foreach (string t in Parser.splitString(line, '=', quote)) tokens_level_1.Add(Parser.strip(t, ' '));
                        if (tokens_level_1.Count != 2) throw new Exception("Error: corrupted token in grammar");

                        string rule = (string)tokens_level_1[0];
                        
                        ArrayList products = new ArrayList();
                        foreach (string pt in Parser.splitString((string)tokens_level_1[1], '|', quote)) products.Add(Parser.strip(pt, ' '));
                        
                        
                        if (!ruleToNT.ContainsKey(rule))
                        {
                            ruleToNT.Add(rule, ruleNum++);
                        }
                        int currentRule = ruleToNT[rule];
                        
                        if (!rules.ContainsKey(currentRule)) rules.Add(currentRule, new ArrayList());
                        
                        foreach (string product in products)
                        {
                            ArrayList productRules = new ArrayList();
                            rules[currentRule].Add(productRules);
                            
                            LinkedList<string> nonTerminals = new LinkedList<string>();
                            foreach (string NT in Parser.splitString(product, ' ', quote)) nonTerminals.AddLast(Parser.strip(NT, ' '));
        
                            foreach (string nonTerminal in nonTerminals)
                            {
                                int nextRule = ruleNum++;
                                if (Parser.isTerminal(nonTerminal, quote))
                                {
                                    nextRule = ruleNum++;
                                    terminals.Add(nextRule, Parser.strip(nonTerminal, quote));
                                }
                                else
                                {
                                    if (!ruleToNT.ContainsKey(nonTerminal))
                                    {
                                        ruleToNT[nonTerminal] = ruleNum++;
                                    }
                                    nextRule = ruleToNT[nonTerminal];
                                }
                                productRules.Add(nextRule);
                            }
                        }
                    }
                }
                int i = 0;
                while (i++ < num)
                {
                    LinkedList<string> lipidnameList = assembleLipidname(rules, terminals, new LinkedList<string>(), 1, -1);
                    lipidNames.Add(String.Join("", lipidnameList));
                }
            }
            return lipidNames;
        }
        
        
        
        
        public static LinkedList<string> assembleLipidname(IDictionary<int, ArrayList> rules, IDictionary<int, string> terminals, LinkedList<string> lipidname, int rule, int prevRandom)
        {
            int p = -2;
            byte[] byteArray = new byte[4];
            do {
                provider.GetBytes(byteArray);
                p = (int)(BitConverter.ToUInt32(byteArray, 0) % rules[rule].Count);
            }
            while (p == prevRandom && prevRandom != 0);
            
            
            
            foreach (int r in (ArrayList)rules[rule][p])
            {
                if (terminals.ContainsKey(r)) lipidname.AddLast(terminals[r]);
                else lipidname = assembleLipidname(rules, terminals, lipidname, r, p);
            }
            return lipidname;
        }
        
        
        
        
        public string serialize(bool onlySettings = false)
        {
            string xml = "<LipidCreator version=\"" + LC_VERSION_NUMBER + "\" CEinstrument=\"" + selectedInstrumentForCE + "\" monitoringType=\"" + monitoringType + "\"  PRMMode=\"" + PRMMode + "\">\n";
            
            xml += collisionEnergyHandler.serialize();
            
            foreach (KeyValuePair<string, Precursor> precursor in headgroups)
            {
                if (precursor.Value.userDefined)
                {
                    xml += precursor.Value.serialize();
                }
            }
            
            foreach (KeyValuePair<string, IDictionary<bool, IDictionary<string, MS2Fragment>>> headgroup in allFragments)
            {
                foreach (KeyValuePair<string, MS2Fragment> fragment in allFragments[headgroup.Key][true])
                {
                    if (fragment.Value.userDefined)
                    {
                        xml += "<userDefinedFragment headgroup=\"" + headgroup.Key + "\">\n";
                        xml += fragment.Value.serialize();
                        xml += "</userDefinedFragment>\n";
                    }
                }
                foreach (KeyValuePair<string, MS2Fragment> fragment in allFragments[headgroup.Key][false])
                {
                    if (fragment.Value.userDefined)
                    {
                        xml += "<userDefinedFragment headgroup=\"" + headgroup.Key + "\">\n";
                        xml += fragment.Value.serialize();
                        xml += "</userDefinedFragment>\n";
                    }
                }
            }
            if (!onlySettings)
            {
                foreach (Lipid currentLipid in registeredLipids)
                {
                    xml += currentLipid.serialize();
                }
            }
            xml += "</LipidCreator>\n";
            return xml;
        }
        
        
        
        
        public void analytics(string category, string action)
        {
            if (enableAnalytics)
            {
                Thread th = new Thread(() => analyticsRequest(category, action));
                th.Start();
            }
        }
        
        
        
        
        
        public ArrayList translate(ArrayList lipidNamesList, bool reportError = false)
        {
            ArrayList parsedLipids = new ArrayList();
            foreach (string lipidName in lipidNamesList)
            {
                Lipid lipid = null;
                if (lipidName.Length > 0)
                {
                    lipidMapsParser.parse(lipidName);
                    if (lipidMapsParser.wordInGrammar)
                    {
                        lipidMapsParser.raiseEvents();
                        if (lipidMapsParserEventHandler.lipid != null)
                        {
                            lipid = lipidMapsParserEventHandler.lipid;
                        }
                        else if (reportError)
                        {
                            log.Error("Warning: lipid '" + lipidName + "' could not parsed.");
                        }
                    }
                    else {
                    
                        lipidNamesParser.parse(lipidName);
                        if (lipidNamesParser.wordInGrammar)
                        {
                            lipidNamesParser.raiseEvents();
                            if (parserEventHandler.lipid != null)
                            {
                                lipid = parserEventHandler.lipid;
                            }
                            else if (reportError)
                            {
                                log.Error("Warning: lipid '" + lipidName + "' could not parsed.");
                            }
                        }
                        else if (reportError)
                        {
                            log.Error("Warning: lipid '" + lipidName + "' could not parsed.");
                        }
                    }
                }
                parsedLipids.Add(lipid);
            }
            return parsedLipids;
        }
        
        
        
        
        
        public static void analyticsRequest(string category, string action)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp("https://lifs.isas.de/piwik/piwik.php?idsite=2&rec=1&e_c=" + category + "&e_a=" + action);
                request.Timeout = 2000;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()){}
            } 
            catch (WebException ex) 
            {
                log.Warn("Failed to contact analytics endpoint!", ex);
            }
        }
        
        
        public static string[] precursorNameSplit(string precursorName)
        {
            string[] names = new string[]{precursorName, ""};
            int n = precursorName.Length;
            if (precursorName[n - 1] != HEAVY_LABEL_CLOSING_BRACKET) return names;
            if (precursorName.IndexOf(HEAVY_LABEL_OPENING_BRACKET) == -1) return names;
            
            precursorName = precursorName.Split(new char[]{HEAVY_LABEL_CLOSING_BRACKET})[0];
            names[1] = precursorName.Split(new char[]{HEAVY_LABEL_OPENING_BRACKET})[1];
            names[0] = precursorName.Split(new char[]{HEAVY_LABEL_OPENING_BRACKET})[0];
            return names;
        }
        
        
        
        
        public void import(XDocument doc, bool onlySettings = false)
        {
            string importVersion = doc.Element("LipidCreator").Attribute("version").Value;
            
            // CE information
            string instrument = doc.Element("LipidCreator").Attribute("CEinstrument").Value;
            monitoringType = (MonitoringTypes)Enum.Parse(typeof(MonitoringTypes), doc.Element("LipidCreator").Attribute("monitoringType").Value.ToString(), true);
            PRMMode = (PRMTypes)Enum.Parse(typeof(PRMTypes), doc.Element("LipidCreator").Attribute("PRMMode").Value.ToString(), true);
            
            if (instrument == "" || (instrument != "" && msInstruments.ContainsKey(instrument)))
            {
                selectedInstrumentForCE = instrument;
            }
            
            var CESettings = doc.Descendants("CE");
            foreach ( var ceXML in CESettings )
            {
                collisionEnergyHandler.import(ceXML, importVersion);
            }
            
            
            
            var precursors = doc.Descendants("Precursor");
            bool precursorImportIgnored = false;
            foreach ( var precursorXML in precursors )
            {
                Precursor precursor = new Precursor();
                precursor.import(precursorXML, importVersion);
                string monoisotopic = precursorNameSplit(precursor.name)[0];
                if (categoryToClass.ContainsKey((int)precursor.category) && !headgroups.ContainsKey(precursor.name) && headgroups.ContainsKey(monoisotopic))
                {
                    categoryToClass[(int)precursor.category].Add(precursor.name);
                    headgroups.Add(precursor.name, precursor);
                    headgroups[monoisotopic].heavyLabeledPrecursors.Add(precursor);
                }
                else
                {
                    precursorImportIgnored = true;
                }
            }
            if (precursorImportIgnored)
            {
                MessageBox.Show("Some precursors are already registered and thus ignored during import.", "Warning");
            }
            
            var userDefinedFragments = doc.Descendants("userDefinedFragment");
            bool fragmentImportIgnored = false;
            foreach ( var userDefinedFragment in userDefinedFragments )
            {
                string headgroup = userDefinedFragment.Attribute("headgroup").Value;
                if (!allFragments.ContainsKey(headgroup))
                {
                    allFragments.Add(headgroup, new Dictionary<bool, IDictionary<string, MS2Fragment>>());
                    allFragments[headgroup].Add(true, new Dictionary<string, MS2Fragment>());
                    allFragments[headgroup].Add(false, new Dictionary<string, MS2Fragment>());
                }
                foreach (var ms2fragmentXML in userDefinedFragment.Descendants("MS2Fragment"))
                {
                    MS2Fragment ms2fragment = new MS2Fragment();
                    ms2fragment.import(ms2fragmentXML, importVersion);
                    if (!allFragments[headgroup][ms2fragment.fragmentAdduct.charge >= 0].ContainsKey(ms2fragment.fragmentName)) allFragments[headgroup][ms2fragment.fragmentAdduct.charge >= 0].Add(ms2fragment.fragmentName, ms2fragment);
                    else fragmentImportIgnored = true;
                }
            }
            if (fragmentImportIgnored)
            {
                MessageBox.Show("Some fragments are already registered and thus ignored during import.", "Warning");
            }
            
            if (onlySettings) return;
            
            var lipids = doc.Descendants("lipid");
            foreach ( var lipid in lipids )
            {
                string lipidType = lipid.Attribute("type").Value;
                switch (lipidType)
                {
                    case "GL":
                        Glycerolipid gll = new Glycerolipid(this);
                        gll.import(lipid, importVersion);
                        registeredLipids.Add(gll);
                        break;
                        
                    case "PL":
                        Phospholipid pll = new Phospholipid(this);
                        pll.import(lipid, importVersion);
                        registeredLipids.Add(pll);
                        break;
                        
                    case "SL":
                        Sphingolipid sll = new Sphingolipid(this);
                        sll.import(lipid, importVersion);
                        registeredLipids.Add(sll);
                        break;
                        
                    case "Cholesterol":
                        Cholesterol chl = new Cholesterol(this);
                        chl.import(lipid, importVersion);
                        registeredLipids.Add(chl);
                        break;
                        
                    case "Mediator":
                        Mediator med = new Mediator(this);
                        med.import(lipid, importVersion);
                        registeredLipids.Add(med);
                        break;
                        
                    default:
                        log.Error("Encountered unknown lipid type '"+lipidType+"' during global import!");
                        throw new Exception("Encountered unknown lipid type '" + lipidType + "' during global import!");
                }
            }
            OnUpdate(new EventArgs());
        }
        
        
        
        public void createBlib(String filename)
        {
            string outputDir = System.IO.Path.GetDirectoryName(filename);
            if (outputDir.Length > 0) System.IO.Directory.CreateDirectory(outputDir);
            System.IO.Directory.CreateDirectory(outputDir);
            if (File.Exists(filename)) File.Delete(filename);

            log.Debug("Connection to sqlite " + filename);
            SQLiteConnection mDBConnection = new SQLiteConnection("Data Source=" + filename + ";Version=3;");
            mDBConnection.Open();
            log.Debug("Opened connection to sqlite " + filename);
            SQLiteCommand command = new SQLiteCommand(mDBConnection);

            log.Debug("Adjusting settings on sqlite " + filename );
            command.CommandText = "PRAGMA synchronous=OFF;";
            command.ExecuteNonQuery();

            log.Debug("Adjusting settings on sqlite " + filename);
            command.CommandText = "PRAGMA cache_size=" + (int)(250 * 1024 / 1.5) + ";";
            command.ExecuteNonQuery();

            log.Debug("Adjusting settings on sqlite " + filename);
            command.CommandText = "PRAGMA temp_store=MEMORY;";
            command.ExecuteNonQuery();

            log.Debug("Creating LibInfo table in sqlite " + filename);

            String sql = "CREATE TABLE LibInfo(libLSID TEXT, createTime TEXT, numSpecs INTEGER, majorVersion INTEGER, minorVersion INTEGER)";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            log.Debug("Inserting LibInfo table data in sqlite " + filename);
            //fill in the LibInfo first
            string lsid = "urn:lsid:isas.de:spectral_library:bibliospec:nr:1";
            // Simulate ctime(d), which is what BlibBuild uses.
            var createTime = string.Format("{0:ddd MMM dd HH:mm:ss yyyy}", DateTime.Now); 
            sql = "INSERT INTO LibInfo values('" + lsid + "','" + createTime + "',-1,1,7)";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            log.Debug("Creating table RefSpectra in sqlite " + filename);
            sql = "CREATE TABLE RefSpectra (id INTEGER primary key autoincrement not null, peptideSeq VARCHAR(150), precursorMZ REAL, precursorCharge INTEGER, peptideModSeq VARCHAR(200), prevAA CHAR(1), nextAA CHAR(1), copies INTEGER, numPeaks INTEGER, ionMobility REAL, collisionalCrossSectionSqA REAL, ionMobilityHighEnergyOffset REAL, ionMobilityType TINYINT, retentionTime REAL, moleculeName VARCHAR(128), chemicalFormula VARCHAR(128), precursorAdduct VARCHAR(128), inchiKey VARCHAR(128), otherKeys VARCHAR(128), fileID INTEGER, SpecIDinFile VARCHAR(256), score REAL, scoreType TINYINT)";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            log.Debug("Creating table Modifications in sqlite " + filename);
            sql = "CREATE TABLE Modifications (id INTEGER primary key autoincrement not null, RefSpectraID INTEGER, position INTEGER, mass REAL)";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            log.Debug("Creating table RefSpectraPeaks in sqlite " + filename);
            sql = "CREATE TABLE RefSpectraPeaks(RefSpectraID INTEGER, peakMZ BLOB, peakIntensity BLOB )";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            log.Debug("Creating table SpectrumSourceFiles in sqlite " + filename);
            sql = "CREATE TABLE SpectrumSourceFiles (id INTEGER PRIMARY KEY autoincrement not null, fileName VARCHAR(512), cutoffScore REAL )";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            log.Debug("Inserting SpectrumSourceFiles table data in sqlite " + filename);
            sql = "INSERT INTO SpectrumSourceFiles(id, fileName, cutoffScore) VALUES(1, 'Generated By LipidCreator', 0.0)"; // An empty table causes trouble for Skyline
            command.CommandText = sql;
            command.ExecuteNonQuery();

            log.Debug("Creating table IonMobilityTypes in sqlite " + filename);
            sql = "CREATE TABLE IonMobilityTypes (id INTEGER PRIMARY KEY, ionMobilityType VARCHAR(128) )";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            
            
            string[] ionMobilityType = { "none", "driftTime(msec)", "inverseK0(Vsec/cm^2)"};
            log.Debug("Inserting IonMobilityTypes table data in sqlite " + filename);
            for (int i=0; i < ionMobilityType.Length; ++i){
                sql = "INSERT INTO IonMobilityTypes(id, ionMobilityType) VALUES(" + i + ", '" + ionMobilityType[i] + "')";
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }

            log.Debug("Creating table ScoreTypes in sqlite " + filename);
            sql = "CREATE TABLE ScoreTypes (id INTEGER PRIMARY KEY, scoreType VARCHAR(128), probabilityType VARCHAR(128) )";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            log.Debug("Creating table RefSpectraPeakAnnotations in sqlite " + filename);
            sql = "CREATE TABLE RefSpectraPeakAnnotations ("+
                "id INTEGER primary key autoincrement not null, " +
                "RefSpectraID INTEGER, " +
                "peakIndex INTEGER, " +
                "name VARCHAR(256), " +
                "formula VARCHAR(256), " +
                "inchiKey VARCHAR(256), " + // molecular identifier for structure retrieval
                "otherKeys VARCHAR(256), " + // alternative molecular identifiers for structure retrieval (CAS or hmdb etc)
                "charge INTEGER, " +
                "adduct VARCHAR(256), " +
                "comment VARCHAR(256), " +
                "mzTheoretical REAL, " +
                "mzObserved REAL )";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            log.Debug("Creating table RetentionTimes in sqlite " + filename);
            sql = "CREATE TABLE RetentionTimes(RefSpectraID INTEGER, RedundantRefSpectraID INTEGER, SpectrumSourceID INTEGER, driftTimeMsec REAL, collisionalCrossSectionSqA REAL, driftTimeHighEnergyOffsetMsec REAL, retentionTime REAL, bestSpectrum INTEGER, FOREIGN KEY(RefSpectraID) REFERENCES RefSpectra(id))";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            Tuple<string, string>[] scoreType = 
            {
                Tuple.Create("UNKNOWN", "NOT_A_PROBABILITY_VALUE"), // default for ssl files
                Tuple.Create("PERCOLATOR QVALUE", "PROBABILITY_THAT_IDENTIFICATION_IS_CORRECT"), // sequest/percolator .sqt files
                Tuple.Create("PEPTIDE PROPHET SOMETHING", "PROBABILITY_THAT_IDENTIFICATION_IS_INCORRECT"), // pepxml files
                Tuple.Create("SPECTRUM MILL", "NOT_A_PROBABILITY_VALUE"), // pepxml files (score is not in range 0-1)
                Tuple.Create("IDPICKER FDR", "PROBABILITY_THAT_IDENTIFICATION_IS_CORRECT"), // idpxml files
                Tuple.Create("MASCOT IONS SCORE", "PROBABILITY_THAT_IDENTIFICATION_IS_CORRECT"), // mascot .dat files (.pep.xml?, .mzid?)
                Tuple.Create("TANDEM EXPECTATION VALUE", "PROBABILITY_THAT_IDENTIFICATION_IS_CORRECT"), // tandem .xtan.xml files
                Tuple.Create("PROTEIN PILOT CONFIDENCE", "PROBABILITY_THAT_IDENTIFICATION_IS_INCORRECT"), // protein pilot .group.xml files
                Tuple.Create("SCAFFOLD SOMETHING", "PROBABILITY_THAT_IDENTIFICATION_IS_INCORRECT"), // scaffold .mzid files
                Tuple.Create("WATERS MSE PEPTIDE SCORE", "NOT_A_PROBABILITY_VALUE"), // Waters MSE .csv files (score is not in range 0-1)
                Tuple.Create("OMSSA EXPECTATION SCORE", "PROBABILITY_THAT_IDENTIFICATION_IS_CORRECT"), // pepxml files
                Tuple.Create("PROTEIN PROSPECTOR EXPECTATION SCORE", "PROBABILITY_THAT_IDENTIFICATION_IS_CORRECT"), // pepxml with expectation score
                Tuple.Create("SEQUEST XCORR", "PROBABILITY_THAT_IDENTIFICATION_IS_CORRECT"), // sequest (no percolator) .sqt files - actually the associated qvalue, not the raw xcorr
                Tuple.Create("MAXQUANT SCORE", "PROBABILITY_THAT_IDENTIFICATION_IS_CORRECT"), // maxquant msms.txt files
                Tuple.Create("MORPHEUS SCORE", "PROBABILITY_THAT_IDENTIFICATION_IS_CORRECT"), // pepxml files with morpehus scores
                Tuple.Create("MSGF+ SCORE", "PROBABILITY_THAT_IDENTIFICATION_IS_CORRECT"), // pepxml files with ms-gfdb scores
                Tuple.Create("PEAKS CONFIDENCE SCORE", "PROBABILITY_THAT_IDENTIFICATION_IS_CORRECT"), // pepxml files with peaks confidence scores
                Tuple.Create("BYONIC SCORE", "PROBABILITY_THAT_IDENTIFICATION_IS_CORRECT"), // byonic .mzid files
                Tuple.Create("PEPTIDE SHAKER CONFIDENCE", "PROBABILITY_THAT_IDENTIFICATION_IS_INCORRECT"), // peptideshaker .mzid files
                Tuple.Create("GENERIC Q-VALUE", "PROBABILITY_THAT_IDENTIFICATION_IS_CORRECT")
            };

            log.Debug("Inserting ScoreTypes table data in sqlite " + filename);
            for (int i=0; i < scoreType.Length; ++i){
                sql = "INSERT INTO ScoreTypes(id, scoreType, probabilityType) VALUES(" + i + ", '" + scoreType[i].Item1 + "', '" + scoreType[i].Item2 + "')";
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }

            log.Debug("Opening transaction to write data in sqlite " + filename);
            sql = "BEGIN TRANSACTION;";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            // Write the annotated spectra
            foreach (PrecursorData precursorData in precursorDataList)
            {
                string precursorName = precursorData.fullMoleculeListName;
                string adduct = precursorData.precursorAdductFormula;
                log.Info("Adding precursor " + precursorName + " and adduct " + adduct);
                if (collisionEnergyHandler.getCollisionEnergy(selectedInstrumentForCE, precursorName, adduct) == -1)
                {
                    collisionEnergyHandler.computeDefaultCollisionEnergy(msInstruments[selectedInstrumentForCE], precursorName, adduct);
                }
                try
                {
                    Lipid.addSpectra(command, precursorData, allFragments, collisionEnergyHandler, selectedInstrumentForCE);
                }
                catch(Exception e)
                {
                    log.Error("Caught exception while trying to add spectra for " + precursorName + " " + adduct + ":", e);
                }
            }
            
            
            sql = "COMMIT;";
            log.Debug("Committing transaction in sqlite " + filename);
            command.CommandText = sql;
            command.ExecuteNonQuery();


            // update numspecs
            log.Debug("Updating LibInfo numSpecs in sqlite " + filename);
            sql = "UPDATE LibInfo SET numSpecs = (SELECT MAX(id) FROM RefSpectra);";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            // indexing
            log.Debug("Creating INDICES in sqlite " + filename);
            command.CommandText = "CREATE INDEX idxPeptide ON RefSpectra (peptideSeq, precursorCharge)";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE INDEX idxPeptideMod ON RefSpectra (peptideModSeq, precursorCharge)";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE INDEX idxRefIdPeaks ON RefSpectraPeaks (RefSpectraID)";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE INDEX idxInChiKey ON RefSpectra (inchiKey, precursorAdduct)";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE INDEX idxMoleculeName ON RefSpectra (moleculeName, precursorAdduct)";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE INDEX idxRefIdPeakAnnotations ON RefSpectraPeakAnnotations (RefSpectraID)";
            command.ExecuteNonQuery();
            log.Debug("Done creating sqlite " + filename);

        }

        public DataTable addDataColumns (DataTable dataTable)
        {
            foreach (string columnKey in DATA_COLUMN_KEYS) {
                dataTable.Columns.Add (columnKey);
            }
            return dataTable;
        }

        public void Dispose()
        {
            if (skylineToolClient != null)
            {
                log.Info("Disposing SkylineToolClient!");
                try
                {
                    ((IDisposable)skylineToolClient).Dispose();
                }
                catch (System.TimeoutException e)
                {
                    log.Warn("Disposing SkylineToolClient timed out!");
                }
            }
        }
    }
    
    public static class Compressing
    {
        public static byte[] GetBytes(double[] values)
        {
            var result = new byte[values.Length * sizeof(double)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }
        
        public static byte[] GetBytes(float[] values)
        {
            var result = new byte[values.Length * sizeof(float)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }
    
        public static byte[] Compress(this double[] uncompressed)
        {
            return Compress(GetBytes(uncompressed), 3);
            //return GetBytes(uncompressed);
        }
    
        public static byte[] Compress(this float[] uncompressed)
        {
            return Compress(GetBytes(uncompressed), 3);
            //return GetBytes(uncompressed);
        }
        
        public static byte[] Compress(this byte[] uncompressed, int level)
        {
            byte[] result;
            using (var ms = new MemoryStream())
            {
                using (var compressor = new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.Level0 + level))
                    compressor.Write(uncompressed, 0, uncompressed.Length);
                result =  ms.ToArray();
            }


            // If compression did not improve the situation, then use
            // uncompressed bytes.
            if (result.Length >= uncompressed.Length)
                return uncompressed;

            return result;
        }
    }
}


