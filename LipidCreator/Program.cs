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
using System.Data.SQLite;
using Ionic.Zlib;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SkylineTool;

namespace LipidCreator
{   
    [Serializable]
    public class LipidCreatorForm
    {
        public ArrayList lipidTabList;
        public ArrayList registeredLipids;
        public CreatorGUI creatorGUI;
        public Dictionary<String, Dictionary<String, ArrayList>> allFragments;
        public Dictionary<String, String> allPathsToPrecursorImages;
        public Dictionary<String, DataTable> headgroups;
        public Dictionary<String, int> buildingBlockTypes;
        public Dictionary<String, Dictionary<String, bool>> headgroupAdductRestrictions;
        public ArrayList precursorDataList;
        public SkylineToolClient skylineToolClient;
        public bool openedAsExternal;
        public string prefixPath = "Tools/LipidCreator/";
        
        public LipidCreatorForm(String pipe)
        {
            openedAsExternal = (pipe != null);
            skylineToolClient = openedAsExternal ? new SkylineToolClient(pipe, "LipidCreator") : null;
            registeredLipids = new ArrayList();
            allPathsToPrecursorImages = new Dictionary<String, String>();
            allFragments = new Dictionary<String, Dictionary<String, ArrayList>>();
            headgroups = new Dictionary<String, DataTable>();
            buildingBlockTypes = new Dictionary<String, int>();
            headgroupAdductRestrictions = new Dictionary<String, Dictionary<String, bool>>();
            precursorDataList = new ArrayList();
            

            int lineCounter = 1;
            string ms2FragmentsFile = (openedAsExternal ? prefixPath : "") + "data/ms2fragments.csv";
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
                            String[] tokens = line.Split(new char[] {','});
                            if (!allFragments.ContainsKey(tokens[0]))
                            {
                                allFragments.Add(tokens[0], new Dictionary<String, ArrayList>());
                            }
                            
                            
                            
                            if (!allFragments[tokens[0]].ContainsKey(tokens[1]))
                            {
                                allFragments[tokens[0]].Add(tokens[1], new ArrayList());
                            }
                            DataTable atomsCount = MS2Fragment.createEmptyElementTable();
                            atomsCount.Rows[0]["Count"] = Convert.ToInt32(tokens[6]);
                            atomsCount.Rows[1]["Count"] = Convert.ToInt32(tokens[7]);
                            atomsCount.Rows[2]["Count"] = Convert.ToInt32(tokens[8]);
                            atomsCount.Rows[3]["Count"] = Convert.ToInt32(tokens[9]);
                            atomsCount.Rows[4]["Count"] = Convert.ToInt32(tokens[10]);
                            atomsCount.Rows[5]["Count"] = Convert.ToInt32(tokens[11]);
                            atomsCount.Rows[6]["Count"] = Convert.ToInt32(tokens[12]);
                            string fragmentFile = (openedAsExternal ? prefixPath : "") + tokens[3];
                            if (!File.Exists(fragmentFile))
                            {
                                Console.WriteLine("Error in line (" + lineCounter + "): file '" + fragmentFile + "' does not exist or can not be opened.");
                            }
                            
                            if (tokens[14].Length > 0)
                            {
                                allFragments[tokens[0]][tokens[1]].Add(new MS2Fragment(tokens[2], Convert.ToInt32(tokens[4]), fragmentFile, true, atomsCount, tokens[5], tokens[13], Convert.ToDouble(tokens[14])));
                            }
                            else 
                            {
                                allFragments[tokens[0]][tokens[1]].Add(new MS2Fragment(tokens[2], Convert.ToInt32(tokens[4]), fragmentFile, true, atomsCount, tokens[5], tokens[13]));
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
            
            
            
            
            string headgroupsFile = (openedAsExternal ? prefixPath : "") + "data/headgroups.csv";
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
                            String[] tokens = line.Split(new char[] {','}); // StringSplitOptions.RemoveEmptyEntries
                            if (tokens.Length != 17) throw new Exception("invalid line in file");
                            headgroups.Add(tokens[0], MS2Fragment.createEmptyElementTable());
                            headgroups[tokens[0]].Rows[0]["Count"] = Convert.ToInt32(tokens[1]);
                            headgroups[tokens[0]].Rows[1]["Count"] = Convert.ToInt32(tokens[2]);
                            headgroups[tokens[0]].Rows[2]["Count"] = Convert.ToInt32(tokens[3]);
                            headgroups[tokens[0]].Rows[3]["Count"] = Convert.ToInt32(tokens[4]);
                            headgroups[tokens[0]].Rows[4]["Count"] = Convert.ToInt32(tokens[5]);
                            headgroups[tokens[0]].Rows[5]["Count"] = Convert.ToInt32(tokens[6]);
                            headgroups[tokens[0]].Rows[6]["Count"] = Convert.ToInt32(tokens[7]);
                            string precursorFile = (openedAsExternal ? prefixPath : "") + tokens[8];
                            if (!File.Exists(precursorFile))
                            {
                                Console.WriteLine("Error (" + lineCounter + "): precursor file " + precursorFile + " does not exist or can not be opened.");
                            }
                            allPathsToPrecursorImages.Add(tokens[0], precursorFile);
                            headgroupAdductRestrictions.Add(tokens[0], new Dictionary<String, bool>());
                            headgroupAdductRestrictions[tokens[0]].Add("+H", tokens[9].Equals("Yes"));
                            headgroupAdductRestrictions[tokens[0]].Add("+2H", tokens[10].Equals("Yes"));
                            headgroupAdductRestrictions[tokens[0]].Add("+NH4", tokens[11].Equals("Yes"));
                            headgroupAdductRestrictions[tokens[0]].Add("-H", tokens[12].Equals("Yes"));
                            headgroupAdductRestrictions[tokens[0]].Add("-2H", tokens[13].Equals("Yes"));
                            headgroupAdductRestrictions[tokens[0]].Add("+HCOO", tokens[14].Equals("Yes"));
                            headgroupAdductRestrictions[tokens[0]].Add("+CH3COO", tokens[15].Equals("Yes"));
                            buildingBlockTypes.Add(tokens[0], Convert.ToInt32(tokens[16]));
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
            
            lipidTabList = new ArrayList(new Lipid[] {null,
                                                      new GLLipid(allPathsToPrecursorImages, allFragments),
                                                      new PLLipid(allPathsToPrecursorImages, allFragments),
                                                      new SLLipid(allPathsToPrecursorImages, allFragments),
                                                      new Cholesterol(allPathsToPrecursorImages, allFragments)
                                                      });
                                                      
                                                      
                                                      
                                                      
                                                      
                                                      
                                                      
                                                      
            creatorGUI = new CreatorGUI(this);
        }


        public HashSet<int> parseRange(String text, int lower, int upper)
        {
            return parseRange(text, lower, upper, 0);
        }
        
        // obType (Object type): 0 = carbon length, 1 = carbon length odd, 2 = carbon length even, 3 = db length, 4 = hydroxyl length
        public HashSet<int> parseRange(String text, int lower, int upper, int obType)
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
            
            foreach (Lipid currentLipid in registeredLipids)
            {
                currentLipid.computePrecursorData(headgroups, headgroupAdductRestrictions, usedKeys, precursorDataList);
            }
        }

        public static String computeChemicalFormula(DataTable elements)
        {
            String chemForm = "";
            foreach (DataRow row in elements.Rows)
            {
                if (Convert.ToInt32(row["Count"]) > 0)
                {
                    chemForm += Convert.ToString(row["Shortcut"]) + (((int)row["Count"] > 1) ? Convert.ToString(row["Count"]) : "");
                }
            }
            return chemForm;
        }

        public static double computeMass(DataTable elements, double charge)
        {
            double mass = 0;
            foreach (DataRow row in elements.Rows)
            {
                mass += Convert.ToDouble(row["Count"]) * Convert.ToDouble(row["mass"]);
            }
            return mass - charge * 0.00054857990946;
        }
        
        public void sendToSkyline(DataTable dt, string blibName, string blibFile)
        {
            if (skylineToolClient == null) return;
            
            var header = string.Join(",", new string[]
            {
                "MoleculeGroup",
                "PrecursorName",
                "PrecursorFormula",
                "PrecursorAdduct",
                "PrecursorMz",
                "PrecursorCharge",
                "ProductName",
                "ProductFormula",
                "ProductMz",
                "ProductCharge"
            });
            string pipeString = header + "\n";
            double maxMass = 0;
            
            foreach (DataRow entry in dt.Rows)
            {
                // Default col order is listname, preName, PreFormula, preAdduct, preMz, preCharge, prodName, ProdFormula, prodAdduct, prodMz, prodCharge
                pipeString += entry["Molecule List Name"] + ","; // listname
                pipeString += entry["Precursor Name"] + ","; // preName
                pipeString += entry["Precursor Ion Formula"] + ","; // PreFormula
                pipeString += entry["Precursor Adduct"] + ","; // preAdduct
                pipeString += entry["Precursor m/z"] + ","; // preMz
                maxMass = Math.Max(maxMass, Convert.ToDouble((string)entry["Precursor m/z"]));
                pipeString += entry["Precursor Charge"] + ","; // preCharge
                pipeString += entry["Product Name"] + ","; // prodName
                pipeString += entry["Product Ion Formula"] + ","; // ProdFormula, no prodAdduct
                pipeString += entry["Product m/z"] + ","; // prodMz
                pipeString += entry["Product Charge"]; // prodCharge
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
        
        
        public string serialize()
        {
            string xml = "<LipidCreator>\n";
            foreach (Lipid currentLipid in registeredLipids)
            {
                xml += currentLipid.serialize();
            }
            xml += "</LipidCreator>\n";
            return xml;
        }
        
        public void import(XDocument doc)
        {
            var lipids = doc.Descendants("lipid");
            foreach ( var lipid in lipids )
            {
                string lipidType = lipid.Attribute("type").Value;
                switch (lipidType)
                {
                    case "GL":
                        GLLipid gll = new GLLipid(allPathsToPrecursorImages, allFragments);
                        gll.import(lipid);
                        registeredLipids.Add(gll);
                        break;
                        
                    case "PL":
                        PLLipid pll = new PLLipid(allPathsToPrecursorImages, allFragments);
                        pll.import(lipid);
                        registeredLipids.Add(pll);
                        break;
                        
                    case "SL":
                        SLLipid sll = new SLLipid(allPathsToPrecursorImages, allFragments);
                        sll.import(lipid);
                        registeredLipids.Add(sll);
                        break;
                        
                    case "Cholesterol":
                        Cholesterol chl = new Cholesterol(allPathsToPrecursorImages, allFragments);
                        chl.import(lipid);
                        registeredLipids.Add(chl);
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
            sql = "INSERT INTO LibInfo values('" + lsid + "','2017-01-01',-1,1,4)";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE RefSpectra (id INTEGER primary key autoincrement not null, peptideSeq VARCHAR(150), precursorMZ REAL, precursorCharge INTEGER, peptideModSeq VARCHAR(200), prevAA CHAR(1), nextAA CHAR(1), copies INTEGER, numPeaks INTEGER, driftTimeMsec REAL, collisionalCrossSectionSqA REAL, driftTimeHighEnergyOffsetMsec REAL, retentionTime REAL, moleculeName VARCHAR(128), chemicalFormula VARCHAR(128), precursorAdduct VARCHAR(128), inchiKey VARCHAR(128), otherKeys VARCHAR(128), fileID INTEGER, SpecIDinFile VARCHAR(256), score REAL, scoreType TINYINT)";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE Modifications (id INTEGER primary key autoincrement not null, RefSpectraID INTEGER, position INTEGER, mass REAL)";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE RefSpectraPeaks(RefSpectraID INTEGER, peakMZ BLOB, peakIntensity BLOB)";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE SpectrumSourceFiles (id INTEGER PRIMARY KEY autoincrement not null, fileName VARCHAR(512), cutoffScore REAL )";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            sql = "CREATE TABLE ScoreTypes (id INTEGER PRIMARY KEY, scoreType VARCHAR(128) )";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            
            sql = "CREATE TABLE Annotations (RefSpectraID INTEGER, fragmentMZ REAL, sumComposition VARCHAR(100), shortName VARCHAR(50), chargedFragmentName VARCHAR(256), neutralFragmentName VARCHAR(256))";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
            /*
            sql = "CREATE TABLE RetentionTimes(RefSpectraID INTEGER, RedundantRefSpectraID INTEGER, SpectrumSourceID INTEGER, ionMobilityValue REAL, ionMobilityType INTEGER, ionMobilityHighEnergyDriftTimeOffsetMsec REAL, retentionTime REAL, bestSpectrum INTEGER, FOREIGN KEY(RefSpectraID) REFERENCES RefSpectra(id))";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            */
            
            string[] scoreTypeNames = new string[]{"UNKNOWN", "PERCOLATOR QVALUE", "PEPTIDE PROPHET SOMETHING", "SPECTRUM MILL", "IDPICKER FDR", "MASCOT IONS SCORE",  "TANDEM EXPECTATION VALUE",  "PROTEIN PILOT CONFIDENCE", "SCAFFOLD SOMETHING", "WATERS MSE PEPTIDE SCORE", "OMSSA EXPECTATION SCORE", "PROTEIN PROSPECTOR EXPECTATION SCORE", "SEQUEST XCORR", "MAXQUANT SCORE", "MORPHEUS SCORE", "MSGF+ SCORE", "PEAKS CONFIDENCE SCORE", "BYONIC SCORE", "PEPTIDE SHAKER CONFIDENCE"};
            
            for(int i=0; i < scoreTypeNames.Length; ++i){
                sql = "INSERT INTO ScoreTypes(id, scoreType) VALUES(" + i + ", '" + scoreTypeNames[i] + "')";
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            
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
            command.CommandText = "CREATE INDEX idxRefIdAnnotations ON Annotations (RefSpectraID)";
            command.ExecuteNonQuery();
            
            
            foreach (PrecursorData precursorData in this.precursorDataList)
            {
                Lipid.addSpectra(command, precursorData);
            }
            
            
            // update numspecs
            sql = "UPDATE LibInfo SET numSpecs = (SELECT MAX(id) FROM RefSpectra);";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            
        }
        
        
        

        [STAThread]
        public static void Main(string[] args)
        {
            LipidCreatorForm lcf = new LipidCreatorForm((args.Length > 0) ? args[0] : null);
            Application.Run(lcf.creatorGUI);
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


