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
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;
using System.Data.SQLite;
using Ionic.Zlib;
using System.Diagnostics;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SkylineTool;

namespace LipidCreator
{   
    [Serializable]
    public class LipidCreator
    {   
        public const string LC_VERSION_NUMBER = "1.0.0";
        public ArrayList registeredLipids;
        public Dictionary<string, Dictionary<bool, Dictionary<string, MS2Fragment>>> allFragments; // lipid class -> positive charge -> fragment name -> fragment
        public Dictionary<int, ArrayList> categoryToClass;
        public Dictionary<string, Precursor> headgroups;
        public DataTable transitionList;
        public ArrayList precursorDataList;
        public SkylineToolClient skylineToolClient;
        public bool openedAsExternal;
        public string prefixPath = "Tools/LipidCreator/";
        public const string MOLECULE_LIST_NAME = "Molecule List Name";
        public const string PRECURSOR_NAME = "Precursor Name";
        public const string PRECURSOR_NEUTRAL_FORMULA = "Precursor Molecule Formula";
        public const string PRECURSOR_ADDUCT = "Precursor Adduct";
        public const string PRECURSOR_MZ = "Precursor Ion m/z";
        public const string PRECURSOR_CHARGE = "Precursor Charge";
        public const string PRODUCT_NAME = "Product Name";
        public const string PRODUCT_NEUTRAL_FORMULA = "Product Molecule Formula";
        public const string PRODUCT_ADDUCT = "Product Adduct";
        public const string PRODUCT_MZ = "Product Ion m/z";
        public const string PRODUCT_CHARGE = "Product Charge";
        public readonly static string[] DATA_COLUMN_KEYS = {
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
            PRODUCT_CHARGE
        };
        
        
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
                                allFragments.Add(tokens[0], new Dictionary<bool, Dictionary<string, MS2Fragment>>());
                                allFragments[tokens[0]].Add(false, new Dictionary<string, MS2Fragment>());
                                allFragments[tokens[0]].Add(true, new Dictionary<string, MS2Fragment>());
                            }
                            Dictionary<int, int> atomsCount = MS2Fragment.createEmptyElementDict();
                            atomsCount[(int)Molecules.C] = Convert.ToInt32(tokens[5]);
                            atomsCount[(int)Molecules.H] = Convert.ToInt32(tokens[6]) - Convert.ToInt32(tokens[3]);
                            atomsCount[(int)Molecules.O] = Convert.ToInt32(tokens[7]);
                            atomsCount[(int)Molecules.N] = Convert.ToInt32(tokens[8]);
                            atomsCount[(int)Molecules.P] = Convert.ToInt32(tokens[9]);
                            atomsCount[(int)Molecules.S] = Convert.ToInt32(tokens[10]);
                            string fragmentFile = prefixPath + tokens[2];
                            if (tokens[2] != "%" && !File.Exists(fragmentFile))
                            {
                                Console.WriteLine("Error in line (" + lineCounter + "): file '" + fragmentFile + "' does not exist or can not be opened.");
                            }
                            
                            int charge = Convert.ToInt32(tokens[3]);
                            if (tokens[12].Length > 0)
                            {
                                allFragments[tokens[0]][charge >= 0].Add(tokens[1], new MS2Fragment(tokens[1], charge, fragmentFile, true, atomsCount, tokens[4], tokens[11], Convert.ToDouble(tokens[12])));
                            }
                            else 
                            {
                                allFragments[tokens[0]][charge >= 0].Add(tokens[1], new MS2Fragment(tokens[1], charge, fragmentFile, true, atomsCount, tokens[4], tokens[11]));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file '" + ms2FragmentsFile + "' in line '" + lineCounter + "' could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("Error: file '" + ms2FragmentsFile + "' does not exist or can not be opened.");
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
                            //String[] tokens = line.Split(new char[] {','}); // StringSplitOptions.RemoveEmptyEntries
                            if (tokens.Length != 20) throw new Exception("invalid line in file, number of columns in line != 20");
                            
                            Precursor headgroup = new Precursor();
                            //headgroup.catogory
                            switch(tokens[0])
                            {
                                case "GL":
                                    headgroup.category = LipidCategory.GlyceroLipid;
                                    break;
                                case "PL":
                                    headgroup.category = LipidCategory.PhosphoLipid;
                                    break;
                                case "SL":
                                    headgroup.category = LipidCategory.SphingoLipid;
                                    break;
                                case "Mediator":
                                    headgroup.category = LipidCategory.Mediator;
                                    break;
                                case "Cholesterol":
                                    headgroup.category = LipidCategory.Cholesterol;
                                    break;
                                default:
                                    throw new Exception("invalid lipid category");
                            }
                            if (!categoryToClass.ContainsKey((int)headgroup.category)) categoryToClass.Add((int)headgroup.category, new ArrayList());
                            categoryToClass[(int)headgroup.category].Add(tokens[1]);
                            
                            
                            headgroup.name = tokens[1];
                            headgroup.elements[(int)Molecules.C] = Convert.ToInt32(tokens[2]); // carbon
                            headgroup.elements[(int)Molecules.H] = Convert.ToInt32(tokens[3]); // hydrogen
                            headgroup.elements[(int)Molecules.H2] = Convert.ToInt32(tokens[8]); // hydrogen 2
                            headgroup.elements[(int)Molecules.O] = Convert.ToInt32(tokens[4]); // oxygen
                            headgroup.elements[(int)Molecules.N] = Convert.ToInt32(tokens[5]); // nytrogen
                            headgroup.elements[(int)Molecules.P] = Convert.ToInt32(tokens[6]); // phosphor
                            headgroup.elements[(int)Molecules.S] = Convert.ToInt32(tokens[7]); // sulfor
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
                            headgroup.buildingBlockType = Convert.ToInt32(tokens[17]);
                            headgroup.derivative = tokens[18].Equals("Yes");
                            headgroup.heavyLabeled = tokens[19].Equals("Yes");
                            
                            if (headgroup.heavyLabeled)
                            {
                                string monoName = headgroup.name.Split(new char[]{'/'})[0];
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
                    Console.WriteLine("The file '" + headgroupsFile + "' in line '" + lineCounter + "' could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("Error: file " + headgroupsFile + " does not exist or can not be opened.");
            }
        }
        

        
        public LipidCreator(String pipe)
        {
            openedAsExternal = (pipe != null);
            skylineToolClient = openedAsExternal ? new SkylineToolClient(pipe, "LipidCreator") : null;
            prefixPath = (openedAsExternal ? prefixPath : "");
            registeredLipids = new ArrayList();
            categoryToClass = new Dictionary<int, ArrayList>();
            allFragments = new Dictionary<string, Dictionary<bool, Dictionary<string, MS2Fragment>>>();
            headgroups = new Dictionary<String, Precursor>();
            transitionList = addDataColumns(new DataTable ());
            precursorDataList = new ArrayList();
            Stopwatch stopwatch = Stopwatch.StartNew();
            readInputFiles();
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }
        
        
        // parser for reading the csv lines with comma separation and "" quotation (if present)
        // using an Moore automaton based approach
        public string[] parseLine(string line)
        {
            List<string> listTokens = new List<string>();
            int start = 0;
            int length = 0;
            int state = 1;
            for (int i = 0; i < line.Length; ++i)
            {
                switch (state){
                    case 0:
                        switch (line[i])
                        {
                            case '"':
                                throw new Exception("invalid line in file");
                            case ',':
                                listTokens.Add(line.Substring(start, length));
                                state = 1;
                                break;
                            default:
                                ++length;
                                break;
                        }
                        break;
                        
                    case 1:
                        switch (line[i])
                        {
                            case '"':
                                length = 0;
                                start = i + 1;
                                state = 2;
                                break;
                            case ',':
                                listTokens.Add("");
                                break;
                            default:
                                length = 1;
                                start = i;
                                state = 0;
                                break;
                        }
                        break;
                        
                    case 2:
                        if (line[i] != '"') ++length;
                        else state = 3;
                        break;
                        
                    case 3:
                        if (line[i] == ',')
                        {
                            listTokens.Add(line.Substring(start, length));
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
        public HashSet<int> parseRange(String text, int lower, int upper, int obType = 0)
        {
            int oddEven = (obType <= 2) ? obType : 0;
            if (text.Length == 0) return null;
            foreach (char c in text)
            {
                int ic = (int)c;
                if (!((ic == (int)',') || (ic == (int)'-') || (ic == (int)' ') || (48 <= ic && ic < 58)))
                {
                    return null;
                }
            }
        
            string[] delimitors = new string[] { "," };
            string[] delimitorsRange = new string[] { "-" };
            string[] tokens = text.Split(delimitors, StringSplitOptions.None);
            
            HashSet<int> carbonCounts = new HashSet<int>();
            
            for (int i = 0; i < tokens.Length; ++i)
            {
                if (tokens[i].Length == 0) return null;
                string[] rangeBoundaries = tokens[i].Split(delimitorsRange, StringSplitOptions.None);
                if (rangeBoundaries.Length == 1)
                {
                    int rangeStart = 0;
                    try 
                    {
                        rangeStart = Convert.ToInt32(rangeBoundaries[0]);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                    if (rangeStart < lower || upper < rangeStart) return null;
                    if (oddEven == 0 || (oddEven == 1 && (rangeStart % 2 == 1)) || (oddEven == 2 && (rangeStart % 2 == 0)))
                    {
                        carbonCounts.Add(rangeStart);
                    }
                }
                else if (rangeBoundaries.Length == 2)
                {
                    int rangeStart = 0;
                    int rangeEnd = 0;
                    try 
                    {
                        rangeStart = Convert.ToInt32(rangeBoundaries[0]);
                        rangeEnd = Convert.ToInt32(rangeBoundaries[1]);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                    if (rangeEnd < rangeStart || rangeStart < lower || upper < rangeEnd) return null;
                    for (int l = rangeStart; l <= rangeEnd; ++l)
                    {
                        if (oddEven == 0 || (oddEven == 1 && (l % 2 == 1)) || (oddEven == 2 && (l % 2 == 0)))
                        {
                            carbonCounts.Add(l);
                        }
                    }
                }
                else return null;
            }
            return carbonCounts;
        }

        
        
        
        public void assembleLipids()
        {
            HashSet<String> usedKeys = new HashSet<String>();
            precursorDataList.Clear();
            transitionList.Clear();
            
            // create precursor list
            foreach (Lipid currentLipid in registeredLipids)
            {
                currentLipid.computePrecursorData(headgroups, usedKeys, precursorDataList);
            }
            
            // create fragment list            
            foreach (PrecursorData precursorData in this.precursorDataList)
            {
                Lipid.computeFragmentData (transitionList, precursorData, allFragments);
            }
        }

        
        
        
        public static String computeChemicalFormula(Dictionary<int, int> elements)
        {
            String chemForm = "";
            foreach (KeyValuePair<int, int> row in elements)
            {
                if (row.Value > 0)
                {
                    chemForm += MS2Fragment.ELEMENT_SHORTCUTS[row.Key] + ((row.Value > 1) ? Convert.ToString(row.Value) : "");
                }
            }
            return chemForm;
        }

        
        
        
        public static double computeMass(Dictionary<int, int> elements, double charge)
        {
            double mass = 0;
            foreach (KeyValuePair<int, int> row in elements)
            {
                mass += row.Value * MS2Fragment.ELEMENT_MASSES[row.Key];
            }
            return mass - charge * 0.00054857990946;
        }
        
        
        
        
        
        public void sendToSkyline(DataTable dt, string blibName, string blibFile)
        {
            if (skylineToolClient == null) return;
            
            var header = string.Join(",", new string[]
            { // Skyline specific column names, please do not touch
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
                "ProductCharge"
            });
            string pipeString = header + "\n";
            double maxMass = 0;
            
            foreach (DataRow entry in dt.Rows)
            {
                // Default col order is listname, preName, PreFormula, preAdduct, preMz, preCharge, prodName, ProdFormula, prodAdduct, prodMz, prodCharge
                pipeString += entry[LipidCreator.MOLECULE_LIST_NAME] + ","; // listname
                pipeString += entry[LipidCreator.PRECURSOR_NAME] + ","; // preName
                pipeString += entry[LipidCreator.PRECURSOR_NEUTRAL_FORMULA] + ","; // PreFormula
                pipeString += entry[LipidCreator.PRECURSOR_ADDUCT] + ","; // preAdduct
                pipeString += entry[LipidCreator.PRECURSOR_MZ] + ","; // preMz
                maxMass = Math.Max(maxMass, Convert.ToDouble((string)entry[LipidCreator.PRECURSOR_MZ]));
                pipeString += entry[LipidCreator.PRECURSOR_CHARGE] + ","; // preCharge
                pipeString += entry[LipidCreator.PRODUCT_NAME] + ","; // prodName
                pipeString += entry[LipidCreator.PRODUCT_NEUTRAL_FORMULA] + ","; // ProdFormula, no prodAdduct
                pipeString += entry[LipidCreator.PRODUCT_ADDUCT] + ","; // preAdduct
                pipeString += entry[LipidCreator.PRODUCT_MZ] + ","; // prodMz
                pipeString += entry[LipidCreator.PRODUCT_CHARGE]; // prodCharge
                pipeString += "\n";
            }
            try
            {
                skylineToolClient.InsertSmallMoleculeTransitionList(pipeString);
                if (blibName.Length > 0 && blibFile.Length > 0) skylineToolClient.AddSpectralLibrary(blibName, blibFile);
                skylineToolClient.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occured, data could not be send to Skyline, please check if your Skyline parameters allow precursor masses up to " + maxMass + "Da.");
            }
        }
        
        
        
        public string serialize(bool onlySettings = false)
        {
        
            string xml = "<LipidCreator version=\"" + LC_VERSION_NUMBER + "\">\n";
            
            foreach (KeyValuePair<string, Precursor> precursor in headgroups)
            {
                if (precursor.Value.userDefined)
                {
                    xml += precursor.Value.serialize();
                }
            }
            
            foreach (KeyValuePair<string, Dictionary<bool, Dictionary<string, MS2Fragment>>> headgroup in allFragments)
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
        
        
        
        
        
        
        public void import(XDocument doc, bool onlySettings = false)
        {
            string importVersion = doc.Element("LipidCreator").Attribute("version").Value;
            
            var precursors = doc.Descendants("Precursor");
            bool precursorImportIgnored = false;
            foreach ( var precursorXML in precursors )
            {
                Precursor precursor = new Precursor();
                precursor.import(precursorXML, importVersion);
                string monoisotopic = precursor.name.Split(new Char[]{'/'})[0];
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
                    allFragments.Add(headgroup, new Dictionary<bool, Dictionary<string, MS2Fragment>>());
                    allFragments[headgroup].Add(true, new Dictionary<string, MS2Fragment>());
                    allFragments[headgroup].Add(false, new Dictionary<string, MS2Fragment>());
                }
                foreach (var ms2fragmentXML in userDefinedFragment.Descendants("MS2Fragment"))
                {
                    MS2Fragment ms2fragment = new MS2Fragment();
                    ms2fragment.import(ms2fragmentXML, importVersion);
                    if (!allFragments[headgroup][ms2fragment.fragmentCharge >= 0].ContainsKey(ms2fragment.fragmentName)) allFragments[headgroup][ms2fragment.fragmentCharge >= 0].Add(ms2fragment.fragmentName, ms2fragment);
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
                        GLLipid gll = new GLLipid(this);
                        gll.import(lipid, importVersion);
                        registeredLipids.Add(gll);
                        break;
                        
                    case "PL":
                        PLLipid pll = new PLLipid(this);
                        pll.import(lipid, importVersion);
                        registeredLipids.Add(pll);
                        break;
                        
                    case "SL":
                        SLLipid sll = new SLLipid(this);
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
                        Console.WriteLine("Error global import");
                        throw new Exception("Error global import");
                }
            }
        }
        
        
        public void createBlib(String filename)
        {
            if (File.Exists(filename)) File.Delete(filename);
        
            SQLiteConnection mDBConnection = new SQLiteConnection("Data Source=" + filename + ";Version=3;");
            mDBConnection.Open();
            SQLiteCommand command = new SQLiteCommand(mDBConnection);
            
            
            command.CommandText = "PRAGMA synchronous=OFF;";
            command.ExecuteNonQuery();
            
            command.CommandText = "PRAGMA cache_size=" + (double)(250 * 1024 / 1.5) + ";";
            command.ExecuteNonQuery();
            
            command.CommandText = "PRAGMA temp_store=MEMORY;";
            command.ExecuteNonQuery();
            
            String sql = "CREATE TABLE LibInfo(libLSID TEXT, createTime TEXT, numSpecs INTEGER, majorVersion INTEGER, minorVersion INTEGER)";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            //fill in the LibInfo first
            string lsid = "urn:lsid:isas.de:spectral_library:bibliospec:nr:1";
            // Simulate ctime(d), which is what BlibBuild uses.
            var createTime = string.Format("{0:ddd MMM dd HH:mm:ss yyyy}", DateTime.Now); 
            sql = "INSERT INTO LibInfo values('" + lsid + "','" + createTime + "',-1,1,7)";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE RefSpectra (id INTEGER primary key autoincrement not null, peptideSeq VARCHAR(150), precursorMZ REAL, precursorCharge INTEGER, peptideModSeq VARCHAR(200), prevAA CHAR(1), nextAA CHAR(1), copies INTEGER, numPeaks INTEGER, ionMobility REAL, collisionalCrossSectionSqA REAL, ionMobilityHighEnergyOffset REAL, ionMobilityType TINYINT, retentionTime REAL, moleculeName VARCHAR(128), chemicalFormula VARCHAR(128), precursorAdduct VARCHAR(128), inchiKey VARCHAR(128), otherKeys VARCHAR(128), fileID INTEGER, SpecIDinFile VARCHAR(256), score REAL, scoreType TINYINT)";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE Modifications (id INTEGER primary key autoincrement not null, RefSpectraID INTEGER, position INTEGER, mass REAL)";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE RefSpectraPeaks(RefSpectraID INTEGER, peakMZ BLOB, peakIntensity BLOB )";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE SpectrumSourceFiles (id INTEGER PRIMARY KEY autoincrement not null, fileName VARCHAR(512), cutoffScore REAL )";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            sql = "INSERT INTO SpectrumSourceFiles(id, fileName, cutoffScore) VALUES(1, 'Generated By LipidCreator', 0.0)"; // An empty table causes trouble for Skyline
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE IonMobilityTypes (id INTEGER PRIMARY KEY, ionMobilityType VARCHAR(128) )";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            string[] ionMobilityType = { "none", "driftTime(msec)", "inverseK0(Vsec/cm^2)"};
            for(int i=0; i < ionMobilityType.Length; ++i){
                sql = "INSERT INTO IonMobilityTypes(id, ionMobilityType) VALUES(" + i + ", '" + ionMobilityType[i] + "')";
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            
            sql = "CREATE TABLE ScoreTypes (id INTEGER PRIMARY KEY, scoreType VARCHAR(128), probabilityType VARCHAR(128) )";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
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
            
            /*
            sql = "CREATE TABLE RetentionTimes(RefSpectraID INTEGER, RedundantRefSpectraID INTEGER, SpectrumSourceID INTEGER, ionMobilityValue REAL, ionMobilityType INTEGER, ionMobilityHighEnergyDriftTimeOffsetMsec REAL, retentionTime REAL, bestSpectrum INTEGER, FOREIGN KEY(RefSpectraID) REFERENCES RefSpectra(id))";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            */
            
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
            
            for(int i=0; i < scoreType.Length; ++i){
                sql = "INSERT INTO ScoreTypes(id, scoreType, probabilityType) VALUES(" + i + ", '" + scoreType[i].Item1 + "', '" + scoreType[i].Item2 + "')";
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            
            // Write the annotated spectra
            foreach (PrecursorData precursorData in this.precursorDataList)
            {
                Lipid.addSpectra(command, precursorData, allFragments);
            }
            
            
            // update numspecs
            sql = "UPDATE LibInfo SET numSpecs = (SELECT MAX(id) FROM RefSpectra);";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            // indexing
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
            
            
        }

        public DataTable addDataColumns (DataTable dataTable)
        {
            foreach (string columnKey in DATA_COLUMN_KEYS) {
                dataTable.Columns.Add (columnKey);
            }
            return dataTable;
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


