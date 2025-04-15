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
using System.Text.Json;
using SkylineTool;
using System.Net;
using System.Threading;

using log4net;
using log4net.Config;
using System.Globalization;
using csgoslin;

using System.Diagnostics;


using ExcelLibrary.SpreadSheet;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace LipidCreator
{   
    public delegate void LipidUpdateEventHandler(object sender, EventArgs e);

    public delegate void SkylineConnectionClosedEventHandler(object sender, EventArgs e);

    public enum MonitoringTypes {NoMonitoring, SRM, PRM};
    public enum PRMTypes {PRMAutomatically, PRMManually};
    public enum RunMode {commandline, standalone, external};
    public enum ChainType {carbonLength, carbonLengthOdd, carbonLengthEven, directMass, dbLength, hydroxylLength};
    public enum LabelPosition {UNIQUE_POS = 0,
        SPECIFIC_POS = 1,
        MOLECULE_LIST_NAME_POS = 2,
        PRECURSOR_NAME_POS = 3,
        PRECURSOR_NEUTRAL_FORMULA_POS = 4,
        PRECURSOR_ADDUCT_POS = 5,
        PRECURSOR_MZ_POS = 6,
        PRECURSOR_CHARGE_POS = 7,
        PRODUCT_NAME_POS = 8,
        PRODUCT_NEUTRAL_FORMULA_POS = 9,
        PRODUCT_ADDUCT_POS = 10,
        PRODUCT_MZ_POS = 11,
        PRODUCT_CHARGE_POS = 12,
        NOTE_POS = 13,
        COLLISION_ENERGY_POS = 14
    }

    [Serializable]
    public class LipidCreator : IDisposable
    {
    
        public static ulong[] randomNumbers = new ulong[]{16807150758674723352UL, 18380906593123320808UL, 10904761884376654311UL, 10745964062774488409UL,
        1477742828810578406UL, 10519320949126946937UL, 11030906738578929825UL, 12635038863694672147UL, 14304249027662503881UL, 6964281971512434030UL,
        2679151693283468822UL, 2300026694507559385UL, 1819401041445280820UL, 4270096456899532719UL, 17426196741770376557UL, 11165407358403899727UL,
        7828390098543334859UL, 4050257391773162037UL, 2487637855913540098UL, 424358752090507202UL, 10768969052967860320UL, 17999041374394552468UL,
        17856537863126535733UL, 16702631510435194271UL, 8263450723793119231UL, 3708358241415535940UL, 6612400824029941941UL, 185530014611949143UL,
        8002454166152003334UL, 10236481197016626696UL, 16826237072142700970UL, 6903162163581243070UL, 18405029556144673699UL, 13901940633555148780UL,
        8778079944963829512UL, 14429119810236517923UL, 8504334224814318867UL, 7381148444504089035UL, 4245763182153447575UL, 2331337917526136819UL,
        5187917052044459358UL, 14096012894957178830UL, 2541795040692595450UL, 16923377782882173568UL, 7200521235738866156UL, 14446522450846959811UL,
        13882517757923439868UL, 11274960922420591833UL, 6408857322110013215UL, 8642075982579781082UL, 15816174529094065300UL, 13528139965271641745UL,
        11580479105977173941UL, 15102509459008191503UL, 11900985445198891977UL, 2950602033259726422UL, 10268661860614386000UL, 12588537465885050556UL,
        11493927009216324668UL, 6527967174358692590UL, 16071548974863358206UL, 10299794865126268035UL, 5454257948945925433UL, 16525205695480020497UL,
        8513817709506097994UL, 2550108408333906874UL, 3597104687517840085UL, 4299267286374698493UL, 13550924828287738234UL, 15627873880739776478UL,
        10088680658906872276UL, 11409150671976976417UL, 9869420137798167817UL, 9036013467956520348UL, 7043194715064298538UL, 9247625790632119692UL,
        12690043328767387385UL, 596881122425133249UL, 12721391113849492412UL, 16470704714425363448UL, 4933076311134968660UL, 15889416548279853309UL,
        997614334416192174UL, 16258471700169306381UL, 8033654069971195015UL, 17108991492795806580UL, 12304192172737976855UL, 4985864093587574921UL,
        15678754966697846769UL, 8730226029791095013UL, 12235228227547942533UL, 4769260588757930796UL, 14908830437339460602UL, 3791044379752043267UL,
        439424475377392669UL, 4836717676278630141UL, 5212419530217883565UL, 8429083707185764800UL, 8036348313981835132UL, 15146735687663783218UL,
        3390489960780566875UL, 3541784339832885676UL, 3653007312126123727UL, 13040542232139922304UL, 4979920891752463728UL, 14157059194484443025UL,
        100528671685960349UL, 17283933488090986460UL, 9589927182238962941UL, 13903822936444454038UL, 4375242081042513086UL, 558232238979648290UL,
        3921321761634178388UL, 16768617361348621604UL, 9123593725759821580UL, 10524620538689731525UL, 17927196608635849398UL, 7745103811979872953UL,
        2840961359444212731UL, 6091302967271321075UL, 6020000853250164232UL, 2801507103568297471UL, 16385454618756120836UL, 240421038822945121UL,
        8150462995568946585UL, 2340880963274631806UL, 9235942972392796480UL, 14367805199185879800UL, 9649885745533891859UL, 4104410131642614781UL,
        16264415569876607061UL, 8472935929154242895UL, 5873551638636789430UL, 8733842113130075884UL, 9569541749883751099UL, 9079326546102769198UL,
        8516290259689424080UL, 15173923686801901965UL, 9283129041254871628UL, 8148912079220243340UL, 1853719968177222996UL, 8836061648630136231UL,
        16119451308555599199UL, 12954151328630286754UL, 13498212397713005290UL, 5463566494404915327UL, 4971008001620975904UL, 1264294178831772980UL,
        15811161317337784707UL, 12288050732234978250UL, 3146914450423541806UL, 5659056307336275195UL, 10775148524726850712UL, 15494842369846397113UL,
        755565377338115738UL, 4917349309261870513UL, 7227320185735164408UL, 9147286504013119902UL, 16307563271633745658UL, 2755749258344453374UL,
        6435029354440230184UL, 4783757494917542337UL, 14836880971218361355UL, 8140436285097923622UL, 7039825332040979031UL, 12910585605474600499UL,
        1920686530662189765UL, 14448963877239521541UL, 16360916242782234245UL, 6100457520125886672UL, 6444623436511568719UL, 11922455215220158531UL,
        4915308021930651631UL, 5807278771263518256UL, 17100559908711363256UL, 17073752192943094348UL, 9461825633735739973UL, 4716347389694471166UL,
        13912546171005294212UL, 17033598898739140801UL, 7539353736296478915UL, 60707008043128237UL, 4754878873380621176UL, 4956301288698507752UL,
        7905253879873204995UL, 17554683518495934887UL, 17466836722169679802UL, 2428190730212555036UL, 15185262551162906858UL, 6064665547249522474UL,
        13460043258424803300UL, 13351750144973980328UL, 11774482013592334805UL, 7859657083387626724UL, 15473173895844495849UL, 11895064531415259713UL,
        4617838824069827831UL, 811726663598549068UL, 12474302672309933090UL, 16574788097907188536UL, 4257433997804339263UL, 6749110809434063654UL,
        18165456995984348190UL, 14378946344628038976UL, 6811173383523789832UL, 5667487623485043496UL, 3536800482707365495UL, 17591141204708801405UL,
        6977474560021256736UL, 5932657912110149414UL, 12273863937376769424UL, 17306792478417188810UL, 8682535496818977133UL, 17374731144386165601UL,
        12168575791821870397UL, 8129218082305553708UL, 8184220132885308663UL, 6210702783292772432UL, 1299774539127078803UL, 15349174242287575026UL,
        11490516500691563669UL, 15115657733172637150UL, 9965010338504166621UL, 8585328825346392464UL, 15741403707058832724UL, 5093164849686130822UL,
        2581863853800775776UL, 2432821876630702361UL, 8516484757619764917UL, 637353305406653542UL, 18010145320006943266UL, 11620399768242138285UL,
        16940839711272558753UL, 10247239853093979895UL, 8991818386597954085UL, 11973203399350422891UL, 11030391833171691347UL, 8990782562592772292UL,
        6864461297567087099UL, 768771744534228381UL, 18351610601692375553UL, 7313791788864058345UL, 16964833660821363169UL, 17217215165008873750UL,
        3854629591884091096UL, 661918874110758118UL, 4225300857789402865UL, 9891526117196313403UL, 2132585740689585242UL, 4741162551429069715UL,
        15741431012590207644UL, 9141399840687874692UL, 11485938865337797944UL, 2731325884363612863UL, 5366797957289071769UL, 10350121429669397365UL};
        
        
        
        
    
        [NonSerialized]
        //private static readonly HttpClient client = new HttpClient();
        private static readonly ILog log = LogManager.GetLogger(typeof(LipidCreator));
        public event LipidUpdateEventHandler Update;
        public event SkylineConnectionClosedEventHandler ConnectionClosed;
        public static string LC_VERSION_NUMBER = "1.0.0.0";
        public static string LC_RELEASE_NUMBER = "1.0.0";
        public static string LC_BUILD_NUMBER = "0";
        public static PlatformID LC_OS;
        public ArrayList registeredLipids;
        public Dictionary<ulong, Lipid> registeredLipidDictionary;
        public IDictionary<string, IDictionary<bool, IDictionary<string, MS2Fragment>>> allFragments; // lipid class -> positive/negative charge -> fragment name -> fragment
        public IDictionary<int, ArrayList> categoryToClass;
        public IDictionary<string, Precursor> headgroups;
        public DataTable transitionList;
        public DataTable transitionListUnique;
        public ArrayList precursorDataList;
        [NonSerialized]
        public SkylineToolClient skylineToolClient;
        public bool openedAsExternal;
        public IDictionary<string, InstrumentData> msInstruments;
        public ArrayList availableInstruments;
        public CollisionEnergy collisionEnergyHandler;
        public bool enableAnalytics = false;
        public static string EXTERNAL_PREFIX_PATH = Path.Combine("Tools", "LipidCreator");
        public string prefixPath = "";
        public RunMode runMode;
        public static string ANALYTICS_CATEGORY;
        public HashSet<string>[] buildingBlockSets = new HashSet<string>[7];
        
        // collision energy parameters
        public string selectedInstrumentForCE = "";
        public MonitoringTypes monitoringType = MonitoringTypes.NoMonitoring;
        public PRMTypes PRMMode = PRMTypes.PRMAutomatically;
        
        public csgoslin.LipidParser lipidParser = new csgoslin.LipidParser();
        
        public static ListingParserEventHandler listingParserEventHandler;
        public static Parser listingParser;
        
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
        
        public static double ELECTRON_REST_MASS = 0.00054857990946;
        
        public const char QUOTE = '"';
        public const char PARSER_QUOTE = '\'';
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
        public const string SPECIFIC = "specific";
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
            SPECIFIC,
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

        public virtual void OnSkylineConnectionClosed(EventArgs e)
        {
            SkylineConnectionClosedEventHandler handler = ConnectionClosed;
            if (handler != null) handler(this, e);
        }
        
        public static void print(string s)
        {
            Console.WriteLine(s);
        }
        
        public void readInputFiles()
        {
            int lineCounter = 1;
            string ms2FragmentsFile = Path.Combine(prefixPath, "data", "LipidDataTables", "ms2fragments.csv");
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
                            atomsCount[(int)Molecule.C] = Convert.ToInt32(tokens[5]);
                            atomsCount[(int)Molecule.H] = Convert.ToInt32(tokens[6]);
                            atomsCount[(int)Molecule.O] = Convert.ToInt32(tokens[7]);
                            atomsCount[(int)Molecule.N] = Convert.ToInt32(tokens[8]);
                            atomsCount[(int)Molecule.P] = Convert.ToInt32(tokens[9]);
                            atomsCount[(int)Molecule.S] = Convert.ToInt32(tokens[10]);
                            
                            
                            int charge = Convert.ToInt32(tokens[3]);
                            Adduct adduct = Lipid.chargeToAdduct[charge];
                            if (allFragments[tokens[0]][charge >= 0].ContainsKey(tokens[2]))
                            {
                                throw new Exception(String.Format("Error: fragment '{0}{1}' already inserted in lipid '{2}'", tokens[2], (charge >= 0 ? "+" : "-"), tokens[0]));
                            }
                            allFragments[tokens[0]][charge >= 0].Add(tokens[2], new MS2Fragment(tokens[2], tokens[1], adduct, "", atomsCount, tokens[4], tokens[11] == "1"));
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error("The file '" + ms2FragmentsFile + "' in line '" + lineCounter + "' could not be read:", e);
                    throw new Exception();
                }
            }
            else
            {
                log.Error("Error: file '" + ms2FragmentsFile + "' does not exist or can not be opened.");
                throw new Exception();
            }
            
            
            
            
            
            lineCounter = 1;
            string ms2FragmentImageFile = Path.Combine(prefixPath, "data", "ms2fragment-images.csv");
            if (File.Exists(ms2FragmentImageFile))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(ms2FragmentImageFile))
                    {
                        String line = sr.ReadLine(); // omit titles
                        while((line = sr.ReadLine()) != null)
                        {
                            lineCounter++;
                            if (line.Length < 2) continue;
                            if (line[0] == '#') continue;
                            
                            string[] tokens = parseLine(line);
                            
                            int charge = Convert.ToInt32(tokens[2]);
                            
                            
                            if (!allFragments.ContainsKey(tokens[0]))
                            {
                                throw new Exception(String.Format("Error: lipid class '{0}' unknown", tokens[0]));
                            }
                            
                            if (!allFragments[tokens[0]][charge >= 0].ContainsKey(tokens[1]))
                            {
                                throw new Exception(String.Format("Error: fragment '{0}{1}' not inserted in lipid class '{2}'", tokens[1], (charge >= 0 ? "+" : "-"), tokens[0]));
                            }
                            
                            string fragmentFile = Path.Combine(prefixPath, Path.Combine(tokens[3].Split(new char[]{'/'})));
                            if (tokens[3] != "%" && !File.Exists(fragmentFile))
                            {
                                log.Error("At line " + lineCounter + ": file '" + fragmentFile + "' does not exist or can not be opened.");
                                throw new Exception();
                            }
                            
                            allFragments[tokens[0]][charge >= 0][tokens[1]].fragmentFile = fragmentFile;
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error("The file '" + ms2FragmentImageFile + "' in line '" + lineCounter + "' could not be read:", e);
                    throw new Exception();
                }
            }
            else
            {
                log.Error("Error: file '" + ms2FragmentImageFile + "' does not exist or can not be opened.");
                throw new Exception();
            }
            
            
            
            
            
            string headgroupsFile = Path.Combine(prefixPath, "data", "LipidDataTables", "lipidclasses.csv");
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
                            if (tokens.Length < 20) throw new Exception("Error in headgroup image table: number of columns is less than 20 in line " + lineCounter);
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
                                case "Sterol":
                                    headgroup.category = LipidCategory.Sterollipid;
                                    break;
                                default:
                                    throw new Exception("invalid lipid category");
                            }
                            if (!categoryToClass.ContainsKey((int)headgroup.category)) categoryToClass.Add((int)headgroup.category, new ArrayList());
                            categoryToClass[(int)headgroup.category].Add(tokens[1]);
                            
                            headgroup.name = tokens[1];
                            headgroup.trivialName = tokens[2];
                            headgroup.elements[(int)Molecule.C] = Convert.ToInt32(tokens[3]); // carbon
                            headgroup.elements[(int)Molecule.H] = Convert.ToInt32(tokens[4]); // hydrogen
                            headgroup.elements[(int)Molecule.H2] = Convert.ToInt32(tokens[9]); // hydrogen 2
                            headgroup.elements[(int)Molecule.O] = Convert.ToInt32(tokens[5]); // oxygen
                            headgroup.elements[(int)Molecule.N] = Convert.ToInt32(tokens[6]); // nytrogen
                            headgroup.elements[(int)Molecule.P] = Convert.ToInt32(tokens[7]); // phosphor
                            headgroup.elements[(int)Molecule.S] = Convert.ToInt32(tokens[8]); // sulfor
                            
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
                                    log.Error("cannot find monoisotopic class '" + monoName + "' in headgroups file.");
                                    throw new Exception();
                                }
                            }
                            
                            headgroups.Add(headgroup.name, headgroup);
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error("The file '" + headgroupsFile + "' in line '" + lineCounter + "' could not be read:", e);
                    throw new Exception();
                }
            }
            else
            {
                log.Error("Error: file " + headgroupsFile + " does not exist or can not be opened.");
                throw new Exception();
            }
            
            
            
            
            
            string headgroupImagesFile = Path.Combine(prefixPath, "data", "headgroup-images.csv");
            if (File.Exists(headgroupImagesFile))
            {
                lineCounter = 1;
                try
                {
                    using (StreamReader sr = new StreamReader(headgroupImagesFile))
                    {
                        String line = sr.ReadLine(); // omit titles
                        while((line = sr.ReadLine()) != null)
                        {
                            lineCounter++;
                            if (line.Length < 2) continue;
                            if (line[0] == '#') continue;
                            
                            string[] tokens = parseLine(line);
                            if (tokens.Length < 3) throw new Exception("Error in headgroup image table: number of columns is less than 3 in line " + lineCounter);
                            
                            if (!headgroups.ContainsKey(tokens[0])) throw new Exception("Error in headgroup image table: lipid class '" + tokens[0] + "' unknown line " + lineCounter);
                            
                            var headgroup = headgroups[tokens[0]];
                            
                            if (tokens[1].Length > 0)
                            {
                                string backboneFile = Path.Combine(prefixPath, Path.Combine(tokens[1].Split(new char[]{'/'})));
                                if (!File.Exists(backboneFile))
                                {
                                    log.Error("At line " + lineCounter + ": backbone file " + backboneFile + " does not exist or can not be opened.");
                                    throw new Exception();
                                }
                                headgroup.pathToBackboneImage = backboneFile;
                            }
                            string precursorFile = Path.Combine(prefixPath, Path.Combine(tokens[2].Split(new char[]{'/'})));
                            if (!File.Exists(precursorFile))
                            {
                                log.Error("At line " + lineCounter + ": precursor file " + precursorFile + " does not exist or can not be opened.");
                                throw new Exception();
                            }
                            headgroup.pathToImage = precursorFile;
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error("The file '" + headgroupImagesFile + "' in line '" + lineCounter + "' could not be read:", e);
                    throw new Exception();
                }
            }
            else
            {
                log.Error("Error: file " + headgroupImagesFile + " does not exist or can not be opened.");
                throw new Exception();
            }
            
            
            
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
                    if (blocks.Count == 0) continue;
                    
                    bool ms_is_molecule = blocks.Contains("M");
                    
                    blocks.ExceptWith(buildingBlockSets[headgroupType]);
                    if (!ms_is_molecule && blocks.Count > 0)
                    {
                        log.Error("Error: building blocks of fragement '" + headgroupName + " / " + ms2fragment.fragmentName + "' do not match with 'Building Blocks' type in headgroups file.");
                        throw new Exception();
                    }
                }
                
                foreach (MS2Fragment ms2fragment in allFragments[headgroupName][false].Values)
                {
                    HashSet<string> blocks = new HashSet<string>();
                    foreach (string fragmentBase in ms2fragment.fragmentBase) blocks.Add(fragmentBase);
                    if (blocks.Count == 0) continue;
                    
                    bool ms_is_molecule = blocks.Contains("M");
                    
                    blocks.ExceptWith(buildingBlockSets[headgroupType]);
                    if (!ms_is_molecule && blocks.Count > 0)
                    {
                        log.Error("Error: building blocks of fragement '" + headgroupName + " / " + ms2fragment.fragmentName + "' do not match with 'Building Blocks' type in headgroups file.");
                        throw new Exception();
                    }
                }
            }
            
            
            foreach (KeyValuePair<string, IDictionary<bool, IDictionary<string, MS2Fragment>>> headgroup_map in allFragments)
            {
                foreach (KeyValuePair<bool, IDictionary<string, MS2Fragment>> charge_map in headgroup_map.Value)
                {
                    foreach (KeyValuePair<string, MS2Fragment> ms2fragment_pair in charge_map.Value)
                    {
                        MS2Fragment ms2fragment = ms2fragment_pair.Value;
                        if (ms2fragment.fragmentBase.Count == 0) continue;
                            
                        if (ms2fragment.fragmentFile.Equals(""))
                        {
                            log.Warn(String.Format("Warning: fragment '{0}{1}' not inserted in lipid class '{2}'", ms2fragment.fragmentName, (charge_map.Key ? "+" : "-"), headgroup_map.Key));
                        }
                    }
                }
            }
            
            
            string instrumentsFile = Path.Combine(prefixPath, "data", "ms-instruments.csv");
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
                    throw new Exception();
                }
            }
            else
            {
                log.Error("Error: file " + instrumentsFile + " does not exist or can not be opened.");
                throw new Exception();
            }
            
            
            
            string ceParametersDir = Path.Combine(prefixPath, "data", "ce-parameters");
            if (Directory.Exists(ceParametersDir))
            {
                string[] ceFilePaths = Directory.GetFiles(Path.Combine(prefixPath, "data", "ce-parameters"), "*.csv", SearchOption.TopDirectoryOnly);
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
                                        log.Error("Invalid line in file, number of columns in line must equal number of columns in header!");
                                        throw new Exception();
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
                                        log.Error("ParamKey for " + instrument + " " + lipidClass + " " + precursorAdduct + " " + fragment + " " + paramKey + " was already assigned! ParamKeys can only be assigned once for any unique combination!");
                                        throw new Exception();
                                    }
                                }
                            }
                        }

                    
                    }
                    catch (Exception e)
                    {
                        log.Error("Encountered an error in file '" + ceParametersFile + "' on line '" + lineCounter + "': ", e);
                        throw new Exception();
                    }
                }
            }
            else
            {
                log.Error("Error: directory " + ceParametersDir + " does not exist or can not be opened.");
                throw new Exception();
            }
            
            string analyticsFile = Path.Combine(prefixPath, "data", "analytics.txt");
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
        

        
        public LipidCreator(string pipe, bool firstInit = false)
        {
            /*
            if (nologging)
            {
                ((log4net.Repository.Hierarchy.Logger)log.Logger).Level = log4net.Core.Level.Error;
            }*/
            
            buildingBlockSets[0] = new HashSet<string>{"FA1", "FA2", "FA3", "FA4", "HG"};
            buildingBlockSets[1] = new HashSet<string>{"FA1", "FA2", "FA3", "HG"};
            buildingBlockSets[2] = new HashSet<string>{"FA1", "FA2", "HG"};
            buildingBlockSets[3] = new HashSet<string>{"FA", "HG"};
            buildingBlockSets[4] = new HashSet<string>{"LCB", "FA", "HG"};
            buildingBlockSets[5] = new HashSet<string>{"LCB", "HG"};
            buildingBlockSets[6] = new HashSet<string>{"HG"};
        
            prefixPath = (openedAsExternal ? EXTERNAL_PREFIX_PATH : new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName);
            XmlConfigurator.Configure(new System.IO.FileInfo(Path.Combine(prefixPath, "data", "log4net.xml")));
            openedAsExternal = (pipe != null);
            skylineToolClient = null;
            if (openedAsExternal)
            {
                skylineToolClient = new SkylineToolClient(pipe, "LipidCreator");
                skylineToolClient.DocumentChanged += OnDocumentChanged;
                skylineToolClient.SelectionChanged += OnSelectionChanged;
                log.Info("LipidCreator is connected to Skyline version '" + skylineToolClient.GetSkylineVersion().ToString() + "'. Project file: '" + skylineToolClient.GetDocumentPath()+"'");
                Task.Factory.StartNew(() =>
                {
                    var client = new NamedPipeClientStream(@".", pipe, PipeDirection.In);
                    log.Info("Opening connection to Skyline through pipe " + pipe);
                    client.Connect();
                    log.Info("Connected to Skyline through pipe " + pipe);
                    while (client.NumberOfServerInstances > 0)
                    {
                        log.Debug("Checking Skyline pipe connection!");
                        var nServers = client.NumberOfServerInstances;
                        log.Debug(nServers+" servers available on other end of pipe!");
                        Thread.Sleep(1000);
                    }

                    if (client.NumberOfServerInstances == 0)
                    {
                        OnSkylineConnectionClosed(new EventArgs());
                        log.Info("Skyline connection was terminated from the other side! Bye bye!");
                        client.Dispose();
                        MessageBox.Show("The Skyline connection was closed! Please save your work (File -> Export Project), close LipidCreator and restart it from the Skyline Tool menu! Then import the saved project again (File -> Import Project).", "Skyline was closed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
                
            } 
            LC_RELEASE_NUMBER = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + "." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();
            LC_BUILD_NUMBER = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString();
            LC_VERSION_NUMBER = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            LC_OS = Environment.OSVersion.Platform;
            ANALYTICS_CATEGORY = "lipidcreator-" + LC_VERSION_NUMBER;
            if (firstInit)
            {
                log.Info("Running LipidCreator version " + LC_VERSION_NUMBER + " in " + (skylineToolClient == null ? "standalone":"skyline tool") + " mode on " + LC_OS.ToString());
                log.Info("Using " + prefixPath + " as base directory for relative resource lookup. Resolved executing assembly location: " + new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location));
            }
            /*
            if (!openedAsExternal) {
                LipidCreator.updateAvailableRequest().ConfigureAwait(false);
            }
            */
            registeredLipids = new ArrayList();
            registeredLipidDictionary = new Dictionary<ulong, Lipid>();
            categoryToClass = new Dictionary<int, ArrayList>();
            allFragments = new Dictionary<string, IDictionary<bool, IDictionary<string, MS2Fragment>>>();
            headgroups = new Dictionary<String, Precursor>();
            precursorDataList = new ArrayList();
            msInstruments = new Dictionary<string, InstrumentData>();
            collisionEnergyHandler = new CollisionEnergy();
            availableInstruments = new ArrayList();
            availableInstruments.Add("");
            readInputFiles();
            collisionEnergyHandler.addCollisionEnergyFields();
            
            
            foreach(string instrument in collisionEnergyHandler.instrumentParameters.Keys)
            {
                availableInstruments.Add(instrument);
            }
            
            foreach(string lipidClass in allFragments.Keys)
            {
                if (!headgroups.ContainsKey(lipidClass))
                {
                    log.Error("Inconsistency of fragment lipid classes: '" + lipidClass + "' doesn't occur in headgroups table");
                    throw new Exception();
                }
            }
            
            foreach(string lipidClass in headgroups.Keys)
            {
                if (!allFragments.ContainsKey(lipidClass))
                {
                    log.Error("Inconsistency of fragment lipid classes: '" + lipidClass + "' doesn't occur in fragments table");
                    throw new Exception();
                }
            }
                
            try 
            {
                listingParserEventHandler = new ListingParserEventHandler();
                listingParser = new Parser(listingParserEventHandler, Path.Combine(prefixPath, "data", "listing.grammar"), PARSER_QUOTE);
            }
            catch (Exception e)
            {
                log.Error("Unable to read grammar file '" + Path.Combine(prefixPath, "data", "listing.grammar") + "': " + e);
                throw new Exception();
            }
        }
        
        
        // parser for reading the csv lines with comma separation and "" quotation (if present)
        // using a Moore automaton based approach. I avoided to write a grammar based parser,
        // because this solution runs in O(n) whereas our Cocke-Younger-Kasami algorithm needs
        // O(n^3) runtime
        
        // TODO: add capability of handling with escape signs
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
        
        
        
        
        
        public static ulong HashCode(string read)
        {
            unchecked {
                ulong hashedValue = 0;
                int i = 0;
                while (i < read.Length)
                {
                    hashedValue += rotateHash(randomNumbers[(int)read[i] & 255], i & 63);
                    i++;
                }
                return hashedValue;
            }
        }
        
        
        
        
        public static ulong rotateHash(ulong hash, int length)
        {
            unchecked {
                if (length <= 0 || 64 <= length) return hash;
                return (hash << length) | (hash >> (64 - length));
            }
        }
        
        
        // obType (Object type): 0 = carbon length, 1 = carbon length odd, 2 = carbon length even, 3 = db length, 4 = hydroxyl length
        public static HashSet<int> parseRange(string text, int lower, int upper, ChainType obType = ChainType.carbonLength)
        {
            int oddEven = ((int)obType <= 2) ? (int)obType : 0;
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
            foreach (ulong lipidHash in registeredLipids)
            {
                Lipid currentLipid = registeredLipidDictionary[lipidHash];
                currentLipid.computePrecursorData(headgroups, usedKeys, precursorDataList);
                int i = precursorDataList.Count - 1;
                while (i >= 0 && ((PrecursorData)precursorDataList[i]).lipidHash == 0)
                {
                    ((PrecursorData)precursorDataList[i]).lipidHash = lipidHash;
                    --i;
                }
            }
        }
        
        
        
        
        
        public void createFragmentList(string instrument, MonitoringTypes monitoringType, ArrayList parameters)
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
                        Lipid.computeFragmentData (transitionList, precursorData, allFragments, headgroups, parameters);
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
                    Lipid.computeFragmentData(transitionList, precursorData, allFragments, headgroups, parameters, collisionEnergyHandler, instrument, monitoringType, CE, minCE, maxCE);
                }
            }
            
            if ((int)parameters[1] != 0)
            {
            
                HashSet<string> transitionListSpecies = new HashSet<string>();
                for (int i = transitionList.Rows.Count - 1; i >= 0; --i)
                {
                    DataRow row = transitionList.Rows[i];
                    if ((string)row[SPECIFIC] == "1")
                    {
                        string key = (string)row[PRECURSOR_NAME] + "-" + (string)row[PRECURSOR_ADDUCT] + "-" + (string)row[PRODUCT_NAME] + "-" + (string)row[PRODUCT_ADDUCT];
                        
                        if (!transitionListSpecies.Contains(key))
                        {
                            transitionListSpecies.Add(key);
                        }
                        else
                        {
                            row.Delete();
                        }
                    }
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
                    string[] duplicate_strings = new string[replicateKeys[replicateKey].Count];
                    for (int i = 0; i < replicateKeys[replicateKey].Count; ++i)
                    {
                        StringBuilder note = new StringBuilder();
                        DataRow dr = (DataRow)replicateKeys[replicateKey][i];
                            
                        note.Append((string)dr[LipidCreator.PRECURSOR_NAME]).Append(" ").Append((string)dr[LipidCreator.PRECURSOR_ADDUCT]).Append(" ").Append((string)dr[LipidCreator.PRODUCT_NAME]);
                        duplicate_strings[i] = note.ToString();
                    }
                    
                    for (int i = 0; i < replicateKeys[replicateKey].Count; ++i)
                    {
                        DataRow dr1 = (DataRow)replicateKeys[replicateKey][i];
                        dr1[UNIQUE] = 0;
                        
                        StringBuilder note = new StringBuilder();
                        for (int j = 0; j < replicateKeys[replicateKey].Count; ++j)
                        {
                            if (i == j) continue;
                            DataRow dr2 = (DataRow)replicateKeys[replicateKey][j];
                            
                            if (note.Length > 0)
                            {
                                note.Append(" and with ");
                            }
                            
                            else
                            {
                                note.Append("Interference with ");
                            }
                            note.Append(duplicate_strings[j]);
                        }
                        dr1[LipidCreator.NOTE] = note.ToString();
                    }
                }
                else
                {
                    row[UNIQUE] = 1;
                }
                transitionListUnique.ImportRow(row);
            }
        }
        
        
        
        public void assembleLipids(bool asDeveloper, ArrayList parameters)
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
            
            createFragmentList(selectedInstrumentForCE, monitoringType, parameters);
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
        
        
        
        
        
        
        public void assembleFragments(bool asDeveloper, ArrayList parameters)
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
            
            
            createFragmentList(selectedInstrumentForCE, monitoringType, parameters);
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

                        int lipidNameIndex = 0;
                        ArrayList importedLipids = translate(lipidsToImport, true);
                        
                        
                        
                        Dictionary<string, Lipid> parsedLipidsDict = new Dictionary<string, Lipid>();
                        ArrayList lipidListForInsertion = new ArrayList();
                        
                        foreach (object[] lipidRow in importedLipids)
                        {
                            Lipid currentLipid = (Lipid)lipidRow[0];
                            string lipidName = (string)lipidRow[2];
                            if (currentLipid == null || (currentLipid is UnsupportedLipid)) continue;
                            ++valid;
                            
                            if (!parsedLipidsDict.ContainsKey(lipidName))
                            {
                                parsedLipidsDict.Add(lipidName, currentLipid);
                                lipidListForInsertion.Add(currentLipid);
                            }
                            else
                            {
                                Lipid lipidForMerging = parsedLipidsDict[lipidName];
                                foreach (string lipidClass in lipidForMerging.positiveFragments.Keys)
                                {
                                    lipidForMerging.positiveFragments[lipidClass].UnionWith(currentLipid.positiveFragments[lipidClass]);
                                }
                                foreach (string lipidClass in lipidForMerging.negativeFragments.Keys)
                                {
                                    lipidForMerging.negativeFragments[lipidClass].UnionWith(currentLipid.negativeFragments[lipidClass]);
                                }
                            }
                        }
                        
                        
                        foreach (Lipid lipid in lipidListForInsertion)
                        {
                            if (lipid == null || (lipid is UnsupportedLipid)) continue;
                            
                            if (filterParameters != null)
                            {
                                lipid.onlyPrecursors = filterParameters[0];
                                lipid.onlyHeavyLabeled = filterParameters[1];
                            }
                            
                            ulong lipidHash = 0;
                            if (lipid is Glycerolipid) lipidHash = ((Glycerolipid)lipid).getHashCode();
                            else if (lipid is Phospholipid) lipidHash = ((Phospholipid)lipid).getHashCode();
                            else if (lipid is Sphingolipid) lipidHash = ((Sphingolipid)lipid).getHashCode();
                            else if (lipid is Sterol) lipidHash = ((Sterol)lipid).getHashCode();
                            else if (lipid is Mediator) lipidHash = ((Mediator)lipid).getHashCode();
                            else if (lipid is UnsupportedLipid) lipidHash = ((UnsupportedLipid)lipid).getHashCode();

                            if (!registeredLipidDictionary.ContainsKey(lipidHash))
                            {
                                registeredLipidDictionary.Add(lipidHash, lipid);
                                registeredLipids.Add(lipidHash);
                            }
                            else
                            {
                                --valid;
                                StringBuilder lipidXml = new StringBuilder(50);
                                lipid.serialize(lipidXml);
                                StringBuilder lipidDictXml = new StringBuilder(50);
                                registeredLipidDictionary[lipidHash].serialize(lipidDictXml);
                                log.Warn("Lipid " + lipidsToImport[lipidNameIndex] + " was already defined! Please check your input for duplicates!");
                            }
                            ++lipidNameIndex;
                        }
                    }
                }
                
                catch (Exception ee)
                {
                    log.Error("Reading lipids from file " + lipidListFile + " failed on line " + total, ee);
                    throw new Exception();
                }
                return new int[]{valid, total};
            }
            else
            {
                log.Error("Could not read file, " + lipidListFile);
                throw new Exception();
            }
        }
        
        
        
        
        public int[] insertLipidList(string[] lipidsToImport, int[] filterParameters = null)
        {
            int total = 0;
            int valid = 0;
            int lipidNameIndex = 0;
            ArrayList importedLipids = translate(new ArrayList(lipidsToImport), true);
            
            
            
            Dictionary<string, Lipid> parsedLipidsDict = new Dictionary<string, Lipid>();
            ArrayList lipidListForInsertion = new ArrayList();
            
            foreach (object[] lipidRow in importedLipids)
            {
                Lipid currentLipid = (Lipid)lipidRow[0];
                string lipidName = (string)lipidRow[2];
                if (currentLipid == null || (currentLipid is UnsupportedLipid)) continue;
                ++valid;
                
                if (!parsedLipidsDict.ContainsKey(lipidName))
                {
                    parsedLipidsDict.Add(lipidName, currentLipid);
                    lipidListForInsertion.Add(currentLipid);
                }
                else
                {
                    Lipid lipidForMerging = parsedLipidsDict[lipidName];
                    foreach (string lipidClass in lipidForMerging.positiveFragments.Keys)
                    {
                        lipidForMerging.positiveFragments[lipidClass].UnionWith(currentLipid.positiveFragments[lipidClass]);
                    }
                    foreach (string lipidClass in lipidForMerging.negativeFragments.Keys)
                    {
                        lipidForMerging.negativeFragments[lipidClass].UnionWith(currentLipid.negativeFragments[lipidClass]);
                    }
                }
            }
            
            
            foreach (Lipid lipid in lipidListForInsertion)
            {
                if (lipid == null || (lipid is UnsupportedLipid)) continue;
                
                if (filterParameters != null)
                {
                    lipid.onlyPrecursors = filterParameters[0];
                    lipid.onlyHeavyLabeled = filterParameters[1];
                }
                
                ulong lipidHash = 0;
                if (lipid is Glycerolipid) lipidHash = ((Glycerolipid)lipid).getHashCode();
                else if (lipid is Phospholipid) lipidHash = ((Phospholipid)lipid).getHashCode();
                else if (lipid is Sphingolipid) lipidHash = ((Sphingolipid)lipid).getHashCode();
                else if (lipid is Sterol) lipidHash = ((Sterol)lipid).getHashCode();
                else if (lipid is Mediator) lipidHash = ((Mediator)lipid).getHashCode();
                else if (lipid is UnsupportedLipid) lipidHash = ((UnsupportedLipid)lipid).getHashCode();

                if (!registeredLipidDictionary.ContainsKey(lipidHash))
                {
                    registeredLipidDictionary.Add(lipidHash, lipid);
                    registeredLipids.Add(lipidHash);
                }
                else
                {
                    --valid;
                    StringBuilder lipidXml = new StringBuilder(50);
                    lipid.serialize(lipidXml);
                    StringBuilder lipidDictXml = new StringBuilder(50);
                    registeredLipidDictionary[lipidHash].serialize(lipidDictXml);
                    log.Warn("Lipid " + lipidsToImport[lipidNameIndex] + " was already defined! Please check your input for duplicates!");
                }
                ++lipidNameIndex;
            }
            return new int[]{valid, total}; 
        }
        
        
        
        
        public void storeTransitionList(string separator, bool split, bool xls, string filename, DataTable currentView, string mode = ".csv")
        {
            
            string outputDir = System.IO.Path.GetDirectoryName(filename);
            if (outputDir.Length > 0) System.IO.Directory.CreateDirectory(outputDir);
            if (!filename.EndsWith(mode)) filename += mode;
            
            if (xls)
            {
                if (split)
                {
                    { // positive
                        Workbook workbook = new Workbook();
                        Worksheet worksheet = new Worksheet("transition list");
                        
                        // adding headers
                        int ii = 0;
                        foreach (string header in LipidCreator.SKYLINE_API_HEADER)
                        {
                            worksheet.Cells[0, ii++] = new Cell(header);
                        }
                        
                        int jj = 1;
                        foreach (DataRow row in currentView.Rows)
                        {
                            if (((String)row [LipidCreator.PRECURSOR_CHARGE]) != "+1" && ((String)row [LipidCreator.PRECURSOR_CHARGE]) != "+2") continue;
                            ii = 0;
                            foreach (string col in LipidCreator.DATA_COLUMN_KEYS)
                            {
                                if (col.Equals(UNIQUE) || col.Equals(SPECIFIC)) continue;
                                string val = (string)row[col];
                                if (col.Equals(LipidCreator.PRODUCT_MZ) || col.Equals(LipidCreator.PRECURSOR_MZ))
                                {
                                    val = val.Replace (",", ".");
                                }
                                worksheet.Cells[jj, ii++] = new Cell(val);
                            }
                            jj++;
                        }
                        workbook.Worksheets.Add(worksheet);
                        workbook.Save(filename.Replace (mode, "_positive" + mode));
                    }
                    
                    
                    { // positive
                        Workbook workbook = new Workbook();
                        Worksheet worksheet = new Worksheet("transition list");
                        
                        // adding headers
                        int ii = 0;
                        foreach (string header in LipidCreator.SKYLINE_API_HEADER)
                        {
                            worksheet.Cells[0, ii++] = new Cell(header);
                        }
                        
                        int jj = 1;
                        foreach (DataRow row in currentView.Rows)
                        {
                            if (((String)row [LipidCreator.PRECURSOR_CHARGE]) != "-1" && ((String)row [LipidCreator.PRECURSOR_CHARGE]) != "-2") continue;
                            ii = 0;
                            foreach (string col in LipidCreator.DATA_COLUMN_KEYS)
                            {
                                if (col.Equals(UNIQUE) || col.Equals(SPECIFIC)) continue;
                                string val = (string)row[col];
                                if (col.Equals(LipidCreator.PRODUCT_MZ) || col.Equals(LipidCreator.PRECURSOR_MZ))
                                {
                                    val = val.Replace (",", ".");
                                }
                                worksheet.Cells[jj, ii++] = new Cell(val);
                            }
                            jj++;
                        }
                        workbook.Worksheets.Add(worksheet);
                        workbook.Save(filename.Replace (mode, "_negative" + mode));
                    }
                }
                
                else 
                {
                    Workbook workbook = new Workbook();
                    Worksheet worksheet = new Worksheet("transition list");
                    
                    // adding headers
                    int ii = 0;
                    foreach (string header in LipidCreator.SKYLINE_API_HEADER)
                    {
                        worksheet.Cells[0, ii++] = new Cell(header);
                    }
                    
                    int jj = 1;
                    foreach (DataRow row in currentView.Rows)
                    {
                        ii = 0;
                        foreach (string col in LipidCreator.DATA_COLUMN_KEYS)
                        {
                            if (col.Equals(UNIQUE) || col.Equals(SPECIFIC)) continue;
                            string val = (string)row[col];
                            if (col.Equals(LipidCreator.PRODUCT_MZ) || col.Equals(LipidCreator.PRECURSOR_MZ))
                            {
                                val = val.Replace (",", ".");
                            }
                            worksheet.Cells[jj, ii++] = new Cell(val);
                        }
                        jj++;
                    }
                    workbook.Worksheets.Add(worksheet);
                    workbook.Save(filename);
                }
            }
            
            else 
            {
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
                if (columnKey == UNIQUE || columnKey == SPECIFIC) continue;
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
                int numElements = elements[(int)molecule];
                foreach (Molecule heavyMolecule in MS2Fragment.ALL_ELEMENTS[molecule].derivatives)
                {
                    numElements += elements[(int)heavyMolecule];
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
                if (adduct.name.Equals("-H"))
                {
                    if (elements[(int)Molecule.H] < 1)
                    {
                        elements[(int)Molecule.H2] += elements[(int)Molecule.H] - 1;
                        elements[(int)Molecule.H] = 1;
                    }
                }
                else if (adduct.name.Equals("-2H"))
                {
                    if (elements[(int)Molecule.H] < 2)
                    {
                        elements[(int)Molecule.H2] += elements[(int)Molecule.H] - 2;
                        elements[(int)Molecule.H] = 2;
                    }
                }
            
            
                foreach (Molecule molecule in MS2Fragment.ALL_ELEMENTS.Keys.Where(x => MS2Fragment.ALL_ELEMENTS[x].isHeavy))
                {
                    if (elements[(int)molecule] > 0)
                    {
                        adductForm += Convert.ToString(elements[(int)molecule]) + MS2Fragment.ALL_ELEMENTS[molecule].shortcutIUPAC;
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
                if (elements[(int)molecule] > 0)
                {
                    label += MS2Fragment.ALL_ELEMENTS[molecule].shortcutNomenclature + ((elements[(int)molecule] > 1) ? Convert.ToString(elements[(int)molecule]) : "");
                }
            }
            if (label.Length > 0) label = "(+" + label + ")";
            return label;
        }

        
        
        
        public static double computeMass(ElementDictionary elements, double charge, double extraMass = 0)
        {
            double mass = 0;
            for (int m = 0; m < elements.Count; ++m)
            {
                if (elements[m] < 0) throw new LipidException((Molecule)m, elements[m], "For element '" + MS2Fragment.ALL_ELEMENTS[(Molecule)m].shortcut + "' the count dropped below zero to " + elements[m]);
                mass += elements[m] * MS2Fragment.ALL_ELEMENTS[(Molecule)m].mass;
            }
            mass += extraMass;
            return (mass - charge * ELECTRON_REST_MASS) / Math.Abs(charge);
        }
        
        
        
        
        
        
        public bool sendToSkyline(DataTable dt, string blibName, string blibFile)
        {
            if (skylineToolClient == null) return false;
            
            bool success = true;
            
            Dictionary<string, string> nameToExportName = new Dictionary<string, string>();
            foreach (PrecursorData precursorData in precursorDataList)
            {
                if (!nameToExportName.ContainsKey(precursorData.precursorName)) nameToExportName.Add(precursorData.precursorName, precursorData.precursorExportName);
            }            
            
            string skylineSep = ",";

            string header = string.Join(skylineSep, SKYLINE_API_HEADER);

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
                    sb.Append("\"").Append(entry[LipidCreator.MOLECULE_LIST_NAME]).Append("\"" + skylineSep); // listname
                    sb.Append("\"").Append(exportName).Append("\"" + skylineSep); // preName
                    sb.Append("\"").Append(entry[LipidCreator.PRECURSOR_NEUTRAL_FORMULA]).Append("\"" + skylineSep); // PreFormula
                    sb.Append("\"").Append(entry[LipidCreator.PRECURSOR_ADDUCT]).Append("\"" + skylineSep); // preAdduct
                    sb.Append(entry[LipidCreator.PRECURSOR_MZ]).Append(skylineSep); // preMz
                    maxMass = Math.Max(maxMass, Convert.ToDouble((string)entry[LipidCreator.PRECURSOR_MZ], CultureInfo.InvariantCulture));
                    sb.Append(entry[LipidCreator.PRECURSOR_CHARGE]).Append(skylineSep); // preCharge
                    sb.Append("\"").Append(entry[LipidCreator.PRODUCT_NAME]).Append("\"" + skylineSep); // prodName
                    sb.Append("\"").Append(entry[LipidCreator.PRODUCT_NEUTRAL_FORMULA]).Append("\"" + skylineSep); // ProdFormula, no prodAdduct
                    sb.Append("\"").Append(entry[LipidCreator.PRODUCT_ADDUCT]).Append("\"" + skylineSep); // preAdduct
                    sb.Append(entry[LipidCreator.PRODUCT_MZ]).Append(skylineSep); // prodMz
                    sb.Append(entry[LipidCreator.PRODUCT_CHARGE]).Append(skylineSep); // prodCharge
                    sb.Append("\"").Append(entry[LipidCreator.NOTE]).Append("\""); // note
                    if (withCE) sb.Append(skylineSep + "\"").Append(entry[LipidCreator.COLLISION_ENERGY]).Append("\""); // note
                    sb.AppendLine();
                } 
                catch(Exception e)
                {
                    log.Error("An error occured during creation of the transition list: ", e);
                    success = false;
                }
            }
            try
            {
                log.Error(sb.ToString());
                log.Debug("Sending small molecule transition list to Skyline: " + sb.ToString());
                skylineToolClient.InsertSmallMoleculeTransitionList(sb.ToString());
                try
                {
                    if (blibName.Length > 0 && blibFile.Length > 0) skylineToolClient.AddSpectralLibrary(blibName, blibFile);
                } catch (IOException ioex)
                {
                    log.Error("An error occured, the spectral library could not be created by Skyline. Please check that your spectral library name is unique or delete an existing spectral library with the same name in Skyline!", ioex);
                    success = false;
                }
            }
            catch (Exception e)
            {
                log.Error("An error occured, data could not be sent to Skyline, please check if your Skyline parameters allow precursor masses up to " + maxMass.ToString(CultureInfo.InvariantCulture) + "Da.", e);
                success = false;
            }
            
            return success;
        }
        
        
        public static void assembleLipidname(IDictionary<long, ArrayList> rules, IDictionary<long, string> terminals, long rule, StringBuilder sb, Random rnd)
        {
            if (terminals.ContainsKey(rule))
            {
                sb.Append(terminals[rule]);
            }
            else
            {
                int rand = rnd.Next(0, rules[rule].Count);
            
                foreach (long r in (ArrayList)rules[rule][rand])
                {
                    assembleLipidname(rules, terminals, r, sb, rnd);
                }
            }
        }
        
        
        
        
        public string serialize(bool onlySettings = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<LipidCreator version=\"" + LC_VERSION_NUMBER + "\" CEinstrument=\"" + selectedInstrumentForCE + "\" monitoringType=\"" + monitoringType + "\"  PRMMode=\"" + PRMMode + "\">\n");
            
            collisionEnergyHandler.serialize(sb);
            
            foreach (KeyValuePair<string, Precursor> precursor in headgroups)
            {
                if (precursor.Value.userDefined)
                {
                    precursor.Value.serialize(sb);
                }
            }
            
            foreach (KeyValuePair<string, IDictionary<bool, IDictionary<string, MS2Fragment>>> headgroup in allFragments)
            {
                foreach (KeyValuePair<string, MS2Fragment> fragment in allFragments[headgroup.Key][true])
                {
                    if (fragment.Value.userDefined)
                    {
                        sb.Append("<userDefinedFragment headgroup=\"" + headgroup.Key + "\">\n");
                        fragment.Value.serialize(sb);
                        sb.Append("</userDefinedFragment>\n");
                    }
                }
                foreach (KeyValuePair<string, MS2Fragment> fragment in allFragments[headgroup.Key][false])
                {
                    if (fragment.Value.userDefined)
                    {
                        sb.Append("<userDefinedFragment headgroup=\"" + headgroup.Key + "\">\n");
                        fragment.Value.serialize(sb);
                        sb.Append("</userDefinedFragment>\n");
                    }
                }
            }
            if (!onlySettings)
            {
                foreach (ulong lipidHash in registeredLipids)
                {
                    registeredLipidDictionary[lipidHash].serialize(sb);
                }
            }
            sb.Append("</LipidCreator>\n");
            return sb.ToString();
        }
        
        
        
        
        public void analytics(string category, string action)
        {
            if (enableAnalytics)
            {
                Thread th = new Thread(() => analyticsRequest(category, action));
                th.Start();
            }
        }
        
        
        
        // handling all events
        public Lipid checkLipid(Lipid lipid, int charge = 0, string adduct = "")
        {
            // first of all, finish ether PC, PE, LPC, LPE
            // flip fatty acids if necessary
            if (lipid != null && lipid.headGroupNames.Count > 0 && (lipid is Phospholipid))
            {
                if (!((Phospholipid)lipid).isLyso && !((Phospholipid)lipid).isCL)
                {
                    bool firstFAHasPlamalogen = false;
                    bool secondFAHasPlamalogen = false;
                    foreach (KeyValuePair<FattyAcidType, bool> kvp in ((Phospholipid)lipid).fag1.faTypes)
                    {
                        firstFAHasPlamalogen |= ((kvp.Key == FattyAcidType.Plasmanyl && kvp.Value) || (kvp.Key == FattyAcidType.Plasmenyl && kvp.Value));
                    }
                    foreach (KeyValuePair<FattyAcidType, bool> kvp in ((Phospholipid)lipid).fag2.faTypes)
                    {
                        secondFAHasPlamalogen |= ((kvp.Key == FattyAcidType.Plasmanyl && kvp.Value) || (kvp.Key == FattyAcidType.Plasmenyl && kvp.Value));
                    }
                    
                    // flip fatty acids
                    if (!firstFAHasPlamalogen && secondFAHasPlamalogen)
                    {
                        FattyAcidGroup tmp = ((Phospholipid)lipid).fag1;
                        ((Phospholipid)lipid).fag1 = ((Phospholipid)lipid).fag2;
                        ((Phospholipid)lipid).fag2 = tmp;
                    }
                    
                    else if (firstFAHasPlamalogen && secondFAHasPlamalogen)
                    {
                        lipid = new UnsupportedLipid(this);
                    }
                }
            }
            
            if (lipid != null)
            {
                int faNum = 1;
                if (lipid.speciesLevel)
                {
                    string headgroup = lipid.headGroupNames[0];
                    faNum = Precursor.fattyAcidCount[headgroups[headgroup].buildingBlockType];
                }
                
                foreach (FattyAcidGroup fag in lipid.getFattyAcidGroupList())
                {
                    foreach (int val in fag.carbonCounts)
                    {
                        if (val < 2 * faNum || 30 * faNum < val)
                        {
                            lipid = new UnsupportedLipid(this);
                            break;
                        }
                    }
                    foreach (int val in fag.doubleBondCounts)
                    {
                        if (val < 0 || 6 * faNum < val)
                        {
                            lipid = new UnsupportedLipid(this);
                            break;
                        }
                    }
                    if (fag.isLCB)
                    {
                        foreach (int val in fag.hydroxylCounts)
                        {
                            if (val < 1 || 3 < val)
                            {
                                lipid = new UnsupportedLipid(this);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (fag.hydroxylCounts.Count > 1 || (fag.hydroxylCounts.Count == 1 && (new List<int>(fag.hydroxylCounts))[0] != 0))
                        {
                            lipid = new UnsupportedLipid(this);
                            break;
                        }
                    }
                }
            }
            

            if (lipid != null && !(lipid is UnsupportedLipid) && lipid.headGroupNames.Count > 0 && headgroups.ContainsKey(lipid.headGroupNames[0]))
            {
            
                foreach (string lipidAdduct in Lipid.ADDUCT_POSITIONS.Keys) lipid.adducts[lipidAdduct] = false;
            
                
                if (charge != 0)
                {
                    if (Lipid.ADDUCT_POSITIONS.ContainsKey(adduct) && Lipid.ALL_ADDUCTS[Lipid.ADDUCT_POSITIONS[adduct]].charge == charge && headgroups[lipid.headGroupNames[0]].adductRestrictions[adduct])
                    {
                        lipid.adducts[adduct] = true;
                    }
                    else
                    {
                        lipid = null;
                    }
                }
                else
                {
                    lipid.adducts[headgroups[lipid.headGroupNames[0]].defaultAdduct] = true;
                }
            }
            else 
            {
                lipid = null;
            }
            
            if (lipid != null)
            {
                lipid.onlyHeavyLabeled = 0;
            }
            
            
            return lipid;
        }
        
        
        
        public Lipid translateLipid(csgoslin.LipidAdduct lipidAdduct, bool allowSpecies = false)
        {
            Lipid lipid = null;
            FattyAcidGroupEnumerator fagEnum = null;
            // translate csgoslin.LipidAdduct into LipidCreator.Lipid
            switch(lipidAdduct.lipid.headgroup.lipid_category)
            {
                case csgoslin.LipidCategory.GL:
                    lipid = new Glycerolipid(this);
                    lipid.headGroupNames.Add(lipidAdduct.get_lipid_string(csgoslin.LipidLevel.CLASS));
                    ((Glycerolipid)lipid).fag1 = new FattyAcidGroup(false, true);
                    ((Glycerolipid)lipid).fag2 = new FattyAcidGroup(false, true);
                    ((Glycerolipid)lipid).fag3 = new FattyAcidGroup(false, true);
                    ((Glycerolipid)lipid).containsSugar = lipidAdduct.contains_sugar();
                    fagEnum = new FattyAcidGroupEnumerator((Glycerolipid)lipid);
                    string hgGL = lipidAdduct.get_lipid_string(csgoslin.LipidLevel.CLASS);
                    if ((new HashSet<string>(){"DGDG", "MGDG", "SQDG"}).Contains(hgGL)) ((Glycerolipid)lipid).containsSugar = true;
                    break;
                    
                case csgoslin.LipidCategory.GP:
                    lipid = new Phospholipid(this);
                    
                    string hg = lipidAdduct.get_lipid_string(csgoslin.LipidLevel.CLASS);
                    if ((new HashSet<string>(){"PC", "PE", "LPC", "LPE"}).Contains(hg))
                    {
                        if (lipidAdduct.lipid.fa_list.Count > 0)
                        {
                            if (lipidAdduct.lipid.fa_list[0].lipid_FA_bond_type == csgoslin.LipidFaBondType.ETHER_PLASMANYL)
                            {
                                hg += " O";
                                ((Phospholipid)lipid).hasPlasmalogen = true;
                            }
                            else if (lipidAdduct.lipid.fa_list[0].lipid_FA_bond_type == csgoslin.LipidFaBondType.ETHER_PLASMENYL)
                            {
                                hg += " P";
                                ((Phospholipid)lipid).hasPlasmalogen = true;
                            }
                        }
                    }
                    
                    
                    
                    ((Phospholipid)lipid).isLyso = lipidAdduct.is_lyso();
                    ((Phospholipid)lipid).isCL = lipidAdduct.is_cardio_lipin();
                    ((Phospholipid)lipid).fag1 = new FattyAcidGroup(false, true);
                    ((Phospholipid)lipid).fag2 = new FattyAcidGroup(false, true);
                    ((Phospholipid)lipid).fag3 = new FattyAcidGroup(false, true);
                    ((Phospholipid)lipid).fag4 = new FattyAcidGroup(false, true);
                    fagEnum = new FattyAcidGroupEnumerator((Phospholipid)lipid);
                    foreach (csgoslin.FattyAcid fa in lipidAdduct.lipid.fa_list)
                    {
                        if (fa.lipid_FA_bond_type == csgoslin.LipidFaBondType.ETHER_PLASMANYL || fa.lipid_FA_bond_type == csgoslin.LipidFaBondType.ETHER_PLASMENYL)
                        {
                            ((Phospholipid)lipid).hasPlasmalogen = true;
                            if ((new HashSet<string>(){"PC", "PE", "LPC", "LPE"}).Contains(hg))
                            {
                                if (fa.lipid_FA_bond_type == csgoslin.LipidFaBondType.ETHER_PLASMANYL) hg += " O";
                                else if (fa.lipid_FA_bond_type == csgoslin.LipidFaBondType.ETHER_PLASMENYL) hg += " P";
                                break;
                            }
                        }
                    }
                    lipid.headGroupNames.Add(hg);
                    break;
                    
                case csgoslin.LipidCategory.SP:
                    lipid = new Sphingolipid(this);
                    lipid.headGroupNames.Add(lipidAdduct.get_lipid_string(csgoslin.LipidLevel.CLASS));
                    ((Sphingolipid)lipid).isLyso = lipidAdduct.is_lyso();
                    ((Sphingolipid)lipid).lcb = new FattyAcidGroup(true, true);
                    ((Sphingolipid)lipid).fag = new FattyAcidGroup(false, true);
                    fagEnum = new FattyAcidGroupEnumerator((Sphingolipid)lipid);
                    break;
                    
                case csgoslin.LipidCategory.ST:
                    lipid = new Sterol(this);
                    lipid.headGroupNames.Add(lipidAdduct.get_lipid_string(csgoslin.LipidLevel.CLASS));
                    ((Sterol)lipid).containsEster = lipidAdduct.contains_ester();
                    ((Sterol)lipid).fag = new FattyAcidGroup(false, true);
                    fagEnum = new FattyAcidGroupEnumerator((Sterol)lipid);
                    break;
                    
                case csgoslin.LipidCategory.FA:
                    lipid = new Mediator(this);
                    lipid.headGroupNames.Add(lipidAdduct.get_lipid_string(csgoslin.LipidLevel.CLASS));
                    break;
                
                default:
                    break;
            }
                    
            if (headgroups.ContainsKey(lipidAdduct.lipid.headgroup.unaltered_headgroup) && !headgroups.ContainsKey(lipid.headGroupNames[0]))
            {
                lipid.headGroupNames[0] = lipidAdduct.lipid.headgroup.unaltered_headgroup;
            }
            
            
            if (lipid != null && lipidAdduct.lipid.info.level == csgoslin.LipidLevel.SPECIES)
            {
                if (allowSpecies)
                {
                    lipid.speciesLevel = true;
                }
                else
                {
                    lipid = null;
                    throw new Exception("Lipid on species level not supported.");
                }
            }
                
            if (lipid != null && !(lipid is Mediator))
            {
                List<csgoslin.FattyAcid> faList = lipid.speciesLevel ? new List<csgoslin.FattyAcid>(){lipidAdduct.lipid.info} : lipidAdduct.lipid.fa_list;
                
                foreach (csgoslin.FattyAcid fa in faList)
                {
                    FattyAcidGroup fag = (fagEnum != null && fagEnum.MoveNext()) ? fagEnum.Current : null;
                    if (fag == null)
                    {
                        lipid = null;
                        throw new Exception("Fatty acids number inconsistency");
                    }
                    fag.carbonCounts.Clear();
                    fag.doubleBondCounts.Clear();
                    fag.hydroxylCounts.Clear();
                    
                    if (fa.num_carbon > 0)
                    {
                        fag.carbonCounts.Add(fa.num_carbon);
                        fag.doubleBondCounts.Add(fa.double_bonds.get_num());
                        
                        fag.lengthInfo = Convert.ToString(fa.num_carbon);
                        fag.dbInfo = Convert.ToString(fa.double_bonds.get_num());
                        
                        fag.isLCB = (fa.lipid_FA_bond_type == csgoslin.LipidFaBondType.LCB_EXCEPTION || fa.lipid_FA_bond_type == csgoslin.LipidFaBondType.LCB_REGULAR);
                        
                        if (fa.functional_groups.ContainsKey("OH") && fa.functional_groups.ContainsKey("O"))
                        {
                            lipid = null;
                            throw new Exception("Lipid with functional groups 'OH' and 'O' in one FA description not supported.");
                        }
                        
                        int checked_functional_groups = fa.functional_groups.ContainsKey("[X]") ? 1 : 0;
                        if (fa.functional_groups.ContainsKey("OH") || fa.functional_groups.ContainsKey("O"))
                        {
                            checked_functional_groups += 1;
                            string fg = fa.functional_groups.ContainsKey("OH") ? "OH" : "O";
                            int cnt = (fa.functional_groups.ContainsKey("[X]") && !lipidAdduct.lipid.headgroup.sp_exception) ? 1 : 0;
                            if (fa.functional_groups[fg].Count == 1)
                            {
                                cnt += fa.functional_groups[fg][0].count;
                            }
                            else
                            {
                                foreach (csgoslin.FunctionalGroup func_group in fa.functional_groups[fg]) cnt += func_group.count;
                            }
                            fag.hydroxylCounts.Add(cnt);
                            fag.hydroxylInfo = Convert.ToString(cnt);
                        }
                        else
                        {
                            fag.hydroxylCounts.Add(0);
                        }
                        if (fa.functional_groups.Count > checked_functional_groups)
                        {
                            lipid = null;
                            throw new Exception("Unknown functional groups not supported.");
                        }
                        
                        if (!fag.isLCB)
                        {
                            switch (fa.lipid_FA_bond_type)
                            {
                                case csgoslin.LipidFaBondType.ESTER:
                                    fag.faTypes[FattyAcidType.Ester] = true;
                                    fag.faTypes[FattyAcidType.NoType] = false;
                                    break;
                                case csgoslin.LipidFaBondType.ETHER_PLASMANYL:
                                    fag.faTypes[FattyAcidType.Plasmanyl] = true;
                                    fag.faTypes[FattyAcidType.NoType] = false;
                                    break;
                                case csgoslin.LipidFaBondType.ETHER_PLASMENYL:
                                    fag.faTypes[FattyAcidType.Plasmenyl] = true;
                                    fag.faTypes[FattyAcidType.NoType] = false;
                                    break;
                                default: lipid = null; throw new Exception("Fatty acid bond type not supported.");
                            }
                        }
                        else
                        {
                            switch (fa.lipid_FA_bond_type)
                            {
                                case csgoslin.LipidFaBondType.LCB_EXCEPTION:
                                case csgoslin.LipidFaBondType.LCB_REGULAR:
                                    fag.faTypes[FattyAcidType.Ester] = true;
                                    fag.faTypes[FattyAcidType.NoType] = false;
                                    break;
                                default: lipid = null; throw new Exception("Long chain base bond type not supported.");
                            }
                        }
                    }
                }
                int charge = lipidAdduct.adduct != null ? lipidAdduct.adduct.charge * lipidAdduct.adduct.charge_sign : 0;
                string adduct = lipidAdduct.adduct != null ? lipidAdduct.adduct.adduct_string : "";
            
                
                lipid = checkLipid(lipid, charge, adduct);
            }
            
            if (lipid != null && !(lipid is UnsupportedLipid) && lipid.speciesLevel)
            {
                if ((lipid is Glycerolipid) || (lipid is Sphingolipid) || (lipid is Phospholipid))
                {
                    FattyAcidGroup fag1 = null;
                    FattyAcidGroup fag2 = null;
                    FattyAcidGroup fag3 = null;
                    FattyAcidGroup fag4 = null;
                    if (lipid is Glycerolipid)
                    {
                        fag1 = ((Glycerolipid)lipid).fag1;
                        fag2 = ((Glycerolipid)lipid).fag2;
                        fag3 = ((Glycerolipid)lipid).fag3;
                    }
                    if (lipid is Phospholipid)
                    {
                        fag1 = ((Phospholipid)lipid).fag1;
                        fag2 = ((Phospholipid)lipid).fag2;
                        fag3 = ((Phospholipid)lipid).fag3;
                        fag4 = ((Phospholipid)lipid).fag4;
                    }
                    if (lipid is Sphingolipid)
                    {
                        fag1 = ((Sphingolipid)lipid).lcb;
                        fag2 = ((Sphingolipid)lipid).fag;
                    }
                    
                    int carbonCounts = fag1.carbonCounts.ToList()[0];
                    string headgroup = lipid.headGroupNames[0];
                    switch(headgroups[headgroup].buildingBlockType)
                    {
                        case 0:
                            if (carbonCounts < 8) lipid = null;
                            else
                            {
                                fag1.carbonCounts.Clear();
                                fag1.carbonCounts.Add(carbonCounts - 6);
                                fag2.carbonCounts.Add(2); fag2.doubleBondCounts.Add(0); fag2.hydroxylCounts.Add(0);
                                fag2.faTypes[FattyAcidType.Ester] = true;
                                fag2.faTypes[FattyAcidType.NoType] = false;
                                fag3.carbonCounts.Add(2); fag3.doubleBondCounts.Add(0); fag3.hydroxylCounts.Add(0);
                                fag3.faTypes[FattyAcidType.Ester] = true;
                                fag3.faTypes[FattyAcidType.NoType] = false;
                                fag4.carbonCounts.Add(2); fag4.doubleBondCounts.Add(0); fag4.hydroxylCounts.Add(0);
                                fag4.faTypes[FattyAcidType.Ester] = true;
                                fag4.faTypes[FattyAcidType.NoType] = false;
                            }
                            break;
                            
                            
                        case 1:
                            if (carbonCounts < 6) lipid = null;
                            else
                            {
                                fag1.carbonCounts.Clear();
                                fag1.carbonCounts.Add(carbonCounts - 4);
                                fag2.carbonCounts.Add(2); fag2.doubleBondCounts.Add(0); fag2.hydroxylCounts.Add(0);
                                fag2.faTypes[FattyAcidType.Ester] = true;
                                fag2.faTypes[FattyAcidType.NoType] = false;
                                fag3.carbonCounts.Add(2); fag3.doubleBondCounts.Add(0); fag3.hydroxylCounts.Add(0);
                                fag3.faTypes[FattyAcidType.Ester] = true;
                                fag3.faTypes[FattyAcidType.NoType] = false;
                            }
                            break;
                            
                            
                        case 2:
                        case 4:
                            if (carbonCounts < 4) lipid = null;
                            else
                            {
                                fag1.carbonCounts.Clear();
                                fag1.carbonCounts.Add(carbonCounts - 2);
                                fag2.carbonCounts.Add(2); fag2.doubleBondCounts.Add(0); fag2.hydroxylCounts.Add(0);
                                fag2.faTypes[FattyAcidType.Ester] = true;
                                fag2.faTypes[FattyAcidType.NoType] = false;
                            }
                            break;
                            
                        default:
                            break;
                    }
                    
                    
                    
                    
                    
                    /*
            buildingBlockSets[0] = new HashSet<string>{"FA1", "FA2", "FA3", "FA4", "HG"};
            buildingBlockSets[1] = new HashSet<string>{"FA1", "FA2", "FA3", "HG"};
            buildingBlockSets[2] = new HashSet<string>{"FA1", "FA2", "HG"};
            buildingBlockSets[3] = new HashSet<string>{"FA", "HG"};
            buildingBlockSets[4] = new HashSet<string>{"LCB", "FA", "HG"};
            buildingBlockSets[5] = new HashSet<string>{"LCB", "HG"};
            buildingBlockSets[6] = new HashSet<string>{"HG"};
                    */
                    
                }
                
            }
            
            return lipid;
        }
        
        
        
        
        public ArrayList translate(ArrayList lipidNamesList, bool reportError = false)
        {
            ArrayList parsedLipids = new ArrayList();
            foreach (string lipidName in lipidNamesList)
            {
                if (lipidName.Length > 0)
                {
                    Lipid lipid = null;
                    string heavyName = "";
                    string purePrecursor = lipidName;
                    
                    try 
                    {
                        if (headgroups.ContainsKey(lipidName) && headgroups[lipidName].category == LipidCategory.LipidMediator)
                        {
                            lipid = new Mediator(this);
                            lipid.headGroupNames.Add(lipidName);
                        }
                        else
                        {
                            csgoslin.LipidAdduct lipidAdduct = lipidParser.parse(lipidName);
                            purePrecursor = lipidAdduct.get_lipid_string();
                            lipid = translateLipid(lipidAdduct);
                        }
                    }
                    catch (Exception)
                    {
                        if (reportError)
                        {
                            log.Error("Warning: lipid '" + lipidName + "' could not parsed.");
                        }
                    }
                    
                    parsedLipids.Add(new object[]{lipid, heavyName, purePrecursor});
                }
            }
            return parsedLipids;
        }
        
        
        
        
        
        public static void analyticsRequest(string category, string action)
        {
            try
            {
                // piwik webserver only allows TLS1.2
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = WebRequest.CreateHttp("https://lifs-tools.org/matomo/matomo.php?idsite=2&rec=1&e_c=" + category + "&e_a=" + action);
                request.Timeout = 2000;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()){}
            } 
            catch (WebException ex) 
            {
                log.Warn("Failed to contact analytics endpoint!", ex);
            }
        }


        /*
        public static async Task<bool> updateAvailableRequest() {
            try	
            {
                log.Info("Checking for LipidCreator updates!");
                // check for updates
                var buildNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision;
                var stringBody = await client.GetStringAsync("https://api.github.com/repos/lifs-tools/lipidcreator/releases/latest");
                using(JsonDocument doc = JsonDocument.Parse(stringBody)) {
                    JsonElement root = doc.RootElement;
                    JsonElement tagName;
                    bool tagNameAvailable = root.TryGetProperty("tag_name", out tagName);
                    if(tagNameAvailable) {
                        int tagNumber = 0;
                        var tagNameString = tagName.GetString();
                        bool success = int.TryParse(tagNameString, out tagNumber);
                        if(success) {
                            if(tagNumber > buildNumber) {
                                log.Info("An update for LipidCreator is available! Please check https://github.com/lifs-tools/lipidcreator for details!");
                                return true;
                            }
                        } else {
                            log.Warn("Could not parse tag name "+ tagNameString+ " as int!");
                        }
                    } else {
                        log.Warn("Could not extract tag_name from response!");
                    }
                }
            }
            catch(HttpRequestException e)
            {
                log.Warn("Failed to contact update endpoint!", e);
            } 
            return false;
        }
        */
        
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
        
        
        
        
        

        
        
        public void addHeavyPrecursor(string headgroup, string name, ArrayList buildingBlocks)
        {
            name = headgroup + LipidCreator.HEAVY_LABEL_OPENING_BRACKET + name + LipidCreator.HEAVY_LABEL_CLOSING_BRACKET;
            
            if (headgroups.ContainsKey(name)) return;
            
            
            // create and set precursor properties
            Precursor precursor = headgroups[headgroup];
            Precursor heavyPrecursor = new Precursor();
            if (buildingBlocks[0] is ElementDictionary)
            {
                heavyPrecursor.elements = (ElementDictionary)buildingBlocks[0];
            }
            else
            {
                heavyPrecursor.elements = createElementData((Dictionary<string, object[]>)buildingBlocks[0]);
            }
            
            heavyPrecursor.name = name;
            heavyPrecursor.category = precursor.category;
            heavyPrecursor.pathToImage = precursor.pathToImage;
            heavyPrecursor.buildingBlockType = precursor.buildingBlockType;
            foreach (KeyValuePair<string, bool> kvp in precursor.adductRestrictions) heavyPrecursor.adductRestrictions.Add(kvp.Key, kvp.Value);
            heavyPrecursor.derivative = precursor.derivative;
            heavyPrecursor.attributes.Add("heavy");
            heavyPrecursor.userDefined = true;
            heavyPrecursor.userDefinedFattyAcids = new ArrayList();
            for (int i = 1; i < buildingBlocks.Count; ++i)
            {
                ElementDictionary newElements = null;
                if (buildingBlocks[i] is ElementDictionary)
                {
                    newElements = (ElementDictionary)buildingBlocks[i];
                }
                else
                {
                    newElements = createElementData((Dictionary<string, object[]>)buildingBlocks[i]);
                }
                heavyPrecursor.userDefinedFattyAcids.Add(newElements);
            }
        
            headgroups.Add(name, heavyPrecursor);
            precursor.heavyLabeledPrecursors.Add(heavyPrecursor);
            
            categoryToClass[(int)heavyPrecursor.category].Add(name);
            
            // copy all MS2Fragments
            allFragments.Add(name, new Dictionary<bool, IDictionary<string, MS2Fragment>>());
            allFragments[name].Add(true, new Dictionary<string, MS2Fragment>());
            allFragments[name].Add(false, new Dictionary<string, MS2Fragment>());
            
            
            if (heavyPrecursor.category != LipidCategory.LipidMediator)
            {
                foreach (KeyValuePair<string, MS2Fragment> ms2Fragment in allFragments[precursor.name][true])
                {
                    MS2Fragment fragment = new MS2Fragment(ms2Fragment.Value);
                    fragment.userDefined = true;
                    allFragments[name][true].Add(ms2Fragment.Key, fragment);
                    
                }
                foreach (KeyValuePair<string, MS2Fragment> ms2Fragment in allFragments[precursor.name][false])
                {
                    MS2Fragment fragment = new MS2Fragment(ms2Fragment.Value);
                    fragment.userDefined = true;
                    allFragments[name][false].Add(ms2Fragment.Key, fragment);
                }
            }
            OnUpdate(new EventArgs());
        }
        
        
        
        
        
        
        
        public static ElementDictionary createElementData(Dictionary<string, object[]> input)
        {
            ElementDictionary elements = MS2Fragment.createEmptyElementDict();
            foreach (KeyValuePair<string, object[]> row in input)
            {
                Molecule elementIndex = MS2Fragment.ELEMENT_POSITIONS[row.Key];
                Molecule heavyIndex = MS2Fragment.ELEMENT_POSITIONS[(string)row.Value[2]];
                
                elements[(int)elementIndex] = (int)row.Value[0];
                elements[(int)heavyIndex] = (int)row.Value[1];
                
            }
            return elements;
        }
        
        
        
        
        
        public void import(string importFilename, bool onlySettings = false)
        {
            if (File.Exists(importFilename))
            {
                XDocument doc = XDocument.Load(importFilename);
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
                    ulong lipidHash = 0;
                    switch (lipidType)
                    {
                        case "GL":
                            Glycerolipid gll = new Glycerolipid(this);
                            gll.import(lipid, importVersion);
                            lipidHash = gll.getHashCode();
                            if (!registeredLipidDictionary.ContainsKey(lipidHash))
                            {
                                registeredLipidDictionary.Add(lipidHash, gll);
                                registeredLipids.Add(lipidHash);
                            }
                            break;
                            
                        case "PL":
                            Phospholipid pll = new Phospholipid(this);
                            pll.import(lipid, importVersion);
                            lipidHash = pll.getHashCode();
                            if (!registeredLipidDictionary.ContainsKey(lipidHash))
                            {
                                registeredLipidDictionary.Add(lipidHash, pll);
                                registeredLipids.Add(lipidHash);
                            }
                            break;
                            
                        case "SL":
                            Sphingolipid sll = new Sphingolipid(this);
                            sll.import(lipid, importVersion);
                            lipidHash = sll.getHashCode();
                            if (!registeredLipidDictionary.ContainsKey(lipidHash))
                            {
                                registeredLipidDictionary.Add(lipidHash, sll);
                                registeredLipids.Add(lipidHash);
                            }
                            break;
                            
                        case "Sterol":
                            Sterol chl = new Sterol(this);
                            chl.import(lipid, importVersion);
                            lipidHash = chl.getHashCode();
                            if (!registeredLipidDictionary.ContainsKey(lipidHash))
                            {
                                registeredLipidDictionary.Add(lipidHash, chl);
                                registeredLipids.Add(lipidHash);
                            }
                            break;
                            
                        case "Mediator":
                            Mediator med = new Mediator(this);
                            med.import(lipid, importVersion);
                            lipidHash = med.getHashCode();
                            if (!registeredLipidDictionary.ContainsKey(lipidHash))
                            {
                                registeredLipidDictionary.Add(lipidHash, med);
                                registeredLipids.Add(lipidHash);
                            }
                            break;
                            
                        default:
                            log.Error("Encountered unknown lipid type '"+lipidType+"' during global import!");
                            throw new Exception("Encountered unknown lipid type '" + lipidType + "' during global import!");
                    }
                }
                OnUpdate(new EventArgs());
            }
            else
            {
                log.Error("File '"+importFilename+"' not found.");
                throw new Exception("File '"+importFilename+"' not found.");
            }
        }
        
        
        
        public void createBlib(String filename)
        {
            string outputDir = System.IO.Path.GetDirectoryName(filename);
            if (outputDir.Length > 0) System.IO.Directory.CreateDirectory(outputDir);
            System.IO.Directory.CreateDirectory(outputDir);
            if (File.Exists(filename)) File.Delete(filename);

            log.Debug("Connection to sqlite " + filename);
            using (SQLiteConnection mDBConnection = new SQLiteConnection("Data Source=" + filename + ";Version=3;"))
            {
                mDBConnection.Open();
                log.Debug("Opened connection to sqlite " + filename);
                using(SQLiteCommand command = new SQLiteCommand(mDBConnection))
                {
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
                    string lsid = "urn:lsid:lifs.isas.de:spectral_library:lipidcreator:nr:1";
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
                        log.Debug("Adding precursor " + precursorName + " and adduct " + adduct);
                        if (collisionEnergyHandler.getCollisionEnergy(selectedInstrumentForCE, precursorName, adduct) == -1)
                        {
                            collisionEnergyHandler.computeDefaultCollisionEnergy(msInstruments[selectedInstrumentForCE], precursorName, adduct);
                        }
                        try
                        {
                            Lipid.addSpectra(command, precursorData, allFragments, headgroups, collisionEnergyHandler, selectedInstrumentForCE);
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
            }
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
                    skylineToolClient.DocumentChanged -= OnDocumentChanged;
                    skylineToolClient.SelectionChanged -= OnSelectionChanged;
                    ((IDisposable)skylineToolClient).Dispose();
                    skylineToolClient = null;
                }
                catch
                {
                    log.Warn("Disposing SkylineToolClient timed out!");
                }
            }
        }

        private void OnDocumentChanged(object sender, EventArgs eventArgs)
        {
            log.Debug("Received a document changed event from Skyline!");
        }

        private void OnSelectionChanged(object sender, EventArgs eventArgs)
        {
            log.Debug("Received a selection changed event from Skyline!");
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


