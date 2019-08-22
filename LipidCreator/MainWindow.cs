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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using log4net;
using System.ComponentModel;
using System.Diagnostics;

namespace LipidCreator
{
        
    [Serializable]
    public partial class MainApplication
    {
        private static RunMode runMode;
        private static readonly ILog log = LogManager.GetLogger(typeof(MainApplication));
        
        public static void printHelp(string option = "")
        {
            LipidCreator lc = null;
            try
            {
                lc = new LipidCreator(null);
                lc.runMode = runMode;
            }
            catch
            {
                log.Error("An error occurred during the initialization of LipidCreator. For more details, please read the log message 'data/lipidreator.log' and get in contact with the developers.");
                return;
            }
            StringBuilder b;
            switch (option)
            {
                case "transitionlist":
                    b = new StringBuilder("Creating a transition list from a lipid list");
                    b.AppendLine().
                    AppendLine("usage: LipidCreator.exe transitionlist input_csv output_csv [opts [opts ...]]").
                     AppendLine("  opts are:").
                     AppendLine("    -p 0:\t\tCompute no precursor transitions").
                     AppendLine("    -p 1:\t\tCompute only precursor transitions").
                     AppendLine("    -p 2:\t\tCompute with precursor transitions").
                     AppendLine("    -h 0:\t\tCompute no heavy labeled isotopes").
                     AppendLine("    -h 1:\t\tCompute only heavy labeled isotopes").
                     AppendLine("    -h 2:\t\tCompute with heavy labeled isotopes").
                     AppendLine("    -s:\t\t\tSplit in positive and negative list").
                     AppendLine("    -S:\t\t\tCompute on species level").
                     AppendLine("    -x:\t\t\tDeveloper or Xpert mode").
                     AppendLine("    -l:\t\t\tCreate LipidCreator project file instead of transition list").
                     AppendLine("    -d:\t\t\tDelete replicate transitions (equal precursor and fragment mass)").
                     AppendLine("    -c instrument mode:\tCompute with optimal collision energy (not available for all lipid classes)").
                     AppendLine("      available instruments and modes:");
                    foreach (KeyValuePair<string, InstrumentData> kvp in lc.msInstruments)
                    {
                        if (kvp.Value.minCE > 0 && kvp.Value.maxCE > 0 && kvp.Value.minCE < kvp.Value.maxCE) 
                        {
                            string fullInstrumentName = (string)(kvp.Value.model);
                            string modes = "(";
                            foreach (string mode in kvp.Value.modes)
                            {
                                if (modes.Length > 1) modes += " / ";
                                modes += mode;
                            }
                            modes += ")";
                            b.AppendLine("        '" + kvp.Key + "': " + fullInstrumentName + " " + modes);
                        }
                    }
                    Console.Write(b.ToString());
                    break;
                    
                    
                case "library":
                    b = new StringBuilder("Creating a spectral library in *.blib format from a lipid list").
                    AppendLine().
                    AppendLine("usage: LipidCreator.exe library input_csv output_blib instrument").
                    AppendLine("  available instruments:");
                    foreach (KeyValuePair<string, InstrumentData> kvp in lc.msInstruments)
                    {
                        if (kvp.Value.minCE > 0) 
                        {
                            string fullInstrumentName = kvp.Value.model;
                            b.AppendLine("    '" + kvp.Key + "': " + fullInstrumentName);
                        }
                    }
                    Console.Write(b.ToString());
                    break;
                    
                    
                    
                case "translate":
                    b = new StringBuilder("Translating a list with old lipid names into current nomenclature").
                    AppendLine().
                    AppendLine("usage: LipidCreator.exe translate input_csv output_csv");
                    Console.WriteLine(b.ToString());
                    break;
                    
                    
                    
                case "random":
                    b = new StringBuilder("Generating a random lipid name (not necessarily reasonable in terms of chemistry)").
                    AppendLine().
                    AppendLine("usage: LipidCreator.exe random [number]");
                    Console.Write(b.ToString());
                    break;
                    
                    
                case "agentmode":
                    b = new StringBuilder("\nUnsaturated fatty acids contain at least one special bond - James Bond!\n\n");
                    Console.Write(b.ToString());
                    break;
                    
                    
                default:
                    b = new StringBuilder("usage: LipidCreator.exe (option)").
                    AppendLine().
                    AppendLine("options are:").
                    AppendLine("  dev:\t\t\t\tlaunching LipidCreator as developer").
                    AppendLine("  transitionlist:\t\tcreating transition list from lipid list").
                    AppendLine("  translate:\t\t\ttranslating a list with old lipid names into current nomenclature").
                    AppendLine("  library:\t\t\tcreating a spectral library in *.blib format from a lipid list").
                    AppendLine("  random:\t\t\tgenerating a random lipid name (not necessarily reasonable in terms of chemistry)").
                    AppendLine("  agentmode:\t\t\tsecret agent mode").
                    AppendLine("  help:\t\t\t\tprint this help");
                    Console.Write(b.ToString());
                    break;
            }
            
            Environment.Exit(1);
        }
        
        
        
        
        public static void checkForAnalytics(RunMode runMode, bool withPrefix)
        {
            string analyticsFile = (withPrefix ? LipidCreator.EXTERNAL_PREFIX_PATH : "") + "data/analytics.txt";
            try {
                if (!File.Exists(analyticsFile))
                {
                    using (StreamWriter outputFile = new StreamWriter (analyticsFile))
                    {
                        outputFile.WriteLine ("-1");
                    }
                }
            }
            catch(Exception e) {
                log.Warn("Warning: Analytics file could not be opened for writing at " + analyticsFile + ". LipidCreator will continue without analytics enabled!", e);
            }
            
            try
            {
                if (File.Exists(analyticsFile))
                {
                    string analyticsContent = "";
                    using (StreamReader sr = new StreamReader(analyticsFile))
                    {
                        // check if first letter in first line is a '1'
                        String line = sr.ReadLine();
                        analyticsContent = line;
                    }
                    
                    if (analyticsContent == "-1")
                    {
                        bool enableAnalytics = false;
                        if (runMode != RunMode.commandline)
                        {
                            DialogResult mbr = MessageBox.Show ("Thank you for choosing LipidCreator.\n\n"+
                                                            "LipidCreator is funded by the German federal ministry of education and research (BMBF) as part of the de.NBI initiative.\nThe project administration requires us to report ANONYMIZED usage statistics for this tool to evaluate its usefulness for the community.\n\n" +
                                                            "With your permission, we collect the following ANONYMIZED statistics:\n - # of LipidCreator launches\n - # of generated transition lists\n\n" + 
                                                            "We do NOT collect any of the following statistics:\n - IP address\n - operating system\n - any information that may traced back to the user\n\n" +
                                                            "When you click 'Yes':\n - you agree to allow us to collect ANONYMIZED usage statistics.\n\n" + 
                                                            "When you click 'No':\n - no data will be sent\n - you can use LipidCreator without any restrictions.\n\n" + 
                                                            "We would highly appreciate your help to secure further funding for the continued development of LipidCreator.", "LipidCreator note", MessageBoxButtons.YesNo);
                            enableAnalytics = mbr == DialogResult.Yes;
                        }
                        else 
                        {
                            Console.WriteLine("Thank you for choosing LipidCreator.\n\n"+
                                                            "LipidCreator is funded by the German federal ministry of education and research (BMBF) as part of the de.NBI initiative.\nThe project administration requires us to report ANONYMIZED usage statistics for this tool to evaluate its usefulness for the community.\n\n" +
                                                            "With your permission, we collect the following ANONYMIZED statistics:\n - # of LipidCreator launches\n - # of generated transition lists\n\n" + 
                                                            "We do NOT collect any of the following statistics:\n - IP address\n - operating system\n - any information that may traced back to the user\n\n" +
                                                            "When you type 'Yes':\n - you agree to allow us to collect ANONYMIZED usage statistics.\n\n" + 
                                                            "When you type 'No':\n - no data will be sent\n - you can use LipidCreator without any restrictions.\n\n" + 
                                                            "We would highly appreciate your help to secure further funding for the continued development of LipidCreator.\nDo you agree on the statistics usage? [Yes / No]");
                            while (true)
                            {
                                String inline = Console.ReadLine();
                                enableAnalytics = inline.ToLower().Equals("yes");
                                
                                if (inline.ToLower().Equals("yes") || inline.ToLower().Equals("no")) break;
                                Console.WriteLine("\n\n\nDo you agree on the statistics usage? [Yes / No]");
                            }
                        }
                        
                        using (StreamWriter outputFile = new StreamWriter (analyticsFile))
                        {
                            outputFile.WriteLine ((enableAnalytics ? "1" : "0"));
                        }
                    }
                }
            }
            catch(Exception e)
            {
                log.Warn("Warning: Analytics file could not be opened at " + analyticsFile + ". LipidCreator will continue without analytics enabled!", e);
            }
        }
        
        
        
        
        
        
        
    
        [STAThread]
        public static void Main(string[] args)
        {
        
            if (args.Length > 0)
            {
        
                if ((new HashSet<string>{"external", "dev", "help", "transitionlist", "library", "random", "agentmode", "translate"}).Contains(args[0]))
                {
                    switch (args[0])
                    {
                        
                        case "dev":
                            runMode = RunMode.standalone;
                            checkForAnalytics(runMode, false);
                            if (File.Exists("data/lipidcreator.log"))
                            {
                                System.IO.File.WriteAllText("data/lipidcreator.log", string.Empty); // Clearing the log file
                            }
                            CreatorGUI creatorGUIDev = new CreatorGUI(null);
                            if (!creatorGUIDev.lipidCreatorInitError)
                            {
                                creatorGUIDev.lipidCreator.runMode = runMode;
                                creatorGUIDev.asDeveloper = true;
                                creatorGUIDev.lipidCreator.analytics(LipidCreator.ANALYTICS_CATEGORY, "launch-" + runMode);
                            }
                            Application.Run(creatorGUIDev);
                            break;
                            
                        
                        case "external":
                            runMode = RunMode.external;
                            if (File.Exists(LipidCreator.EXTERNAL_PREFIX_PATH + "data/lipidcreator.log"))
                            {
                                System.IO.File.WriteAllText(LipidCreator.EXTERNAL_PREFIX_PATH + "data/lipidcreator.log", string.Empty); // Clearing the log file
                            }
                            checkForAnalytics(runMode, true);
                            CreatorGUI creatorGUI = new CreatorGUI(args[1]);
                            if (!creatorGUI.lipidCreatorInitError)
                            {
                                creatorGUI.lipidCreator.runMode = runMode;
                                creatorGUI.lipidCreator.analytics(LipidCreator.ANALYTICS_CATEGORY, "launch-" + runMode);
                            }
                            Application.Run(creatorGUI);
                            break;
                            
                            
                        case "agentmode":
                            runMode = RunMode.commandline;
                            checkForAnalytics(runMode, false);
                            printHelp("agentmode");
                            break;
                            
                            
                        case "help":
                            runMode = RunMode.commandline;
                            checkForAnalytics(runMode, false);
                            printHelp();
                            break;
                            
                            
                        case "random":
                            runMode = RunMode.commandline;
                            checkForAnalytics(runMode, false);
                            if (args.Length > 1 && args[1].Equals("help")) printHelp("random");
                            int num = 1;
                            if (args.Length > 1) num = int.TryParse(args[1], out num) ? num : 1;
                            foreach (string lipidName in LipidCreator.createRandomLipidNames(num)) Console.WriteLine(lipidName);
                            break;
                          
                          
                        case "translate":
                            runMode = RunMode.commandline;
                            checkForAnalytics(runMode, false);
                            if (args.Length < 3)
                            {
                                printHelp("translate");
                            }
                            else
                            {
                                string inputCSV = args[1];
                                string outputCSV = args[2];
                                
                                if (File.Exists(inputCSV))
                                {
                                    int lineCounter = 1;
                                    ArrayList lipidNames = new ArrayList();
                                    try
                                    {
                                        using (StreamReader sr = new StreamReader(inputCSV))
                                        {
                                            string line;
                                            while((line = sr.ReadLine()) != null)
                                            {
                                                lipidNames.Add(line);
                                                ++lineCounter;
                                            }
                                        }
                                        
                                        LipidCreator lc = null;
                                        try
                                        {
                                            lc = new LipidCreator(null, true);
                                            lc.runMode = RunMode.commandline;
                                            lc.analytics(LipidCreator.ANALYTICS_CATEGORY, "launch-" + runMode);
                                        }
                                        catch
                                        {
                                            log.Error("An error occurred during the initialization of LipidCreator. For more details, please read the log message 'data/lipidreator.log' and get in contact with the developers.");
                                            return;
                                        }
                                        ArrayList parsedLipids = lc.translate(lipidNames);
                                        
                                        
                                        HashSet<String> usedKeys = new HashSet<String>();
                                        ArrayList precursorDataList = new ArrayList();
                                        int i = 0;
                                            
                                        using (StreamWriter outputFile = new StreamWriter (outputCSV))
                                        {
                                            foreach (Lipid currentLipid in parsedLipids)
                                            {
                                                string newLipidName = "";
                                                if (currentLipid != null)
                                                {
                                                    currentLipid.computePrecursorData(lc.headgroups, usedKeys, precursorDataList);
                                                    newLipidName = ((PrecursorData)precursorDataList[precursorDataList.Count - 1]).precursorName;
                                                    usedKeys.Clear();
                                                    
                                                }
                                                else
                                                {
                                                    newLipidName = "Unrecognized molecule";
                                                }
                                                outputFile.WriteLine ("\"" + (string)lipidNames[i] + "\",\"" + newLipidName + "\"");
                                                
                                                ++i;
                                            }
                                        }
                                        
                                    }
                                    catch (Exception e)
                                    {
                                        log.Error("The file '" + inputCSV + "' in line '" + lineCounter + "' could not be read:", e);
                                    }
                                }
                            }
                            break;
                            
                            
                        case "transitionlist":
                            runMode = RunMode.commandline;
                            checkForAnalytics(runMode, false);
                            if (args.Length < 3)
                            {
                                printHelp("transitionlist");
                            }
                            else
                            {
                                int parameterPrecursor = 0;
                                int parameterHeavy = 2;
                                string inputCSV = args[1];
                                string outputCSV = args[2];
                                string instrument = "";
                                string mode = "";
                                bool deleteReplicates = false;
                                bool split = false;
                                bool species = false;
                                bool asDeveloper = false;
                                bool createXMLFile = false;
                                int p = 3;
                                while (p < args.Length)
                                {
                                    switch (args[p])
                                    {
                                        case "-p": // precursor parameter
                                            if (!(p + 1 < args.Length) || !(int.TryParse(args[p + 1], out parameterPrecursor))) printHelp("transitionlist");
                                            if (parameterPrecursor < 0 || 2 < parameterPrecursor) printHelp("transitionlist");
                                            p += 2;
                                            break;
                                            
                                        case "-h": // heavy isotope parameter
                                            if (!(p + 1 < args.Length) || !(int.TryParse(args[p + 1], out parameterHeavy))) printHelp("transitionlist");
                                            if (parameterHeavy < 0 || 2 < parameterHeavy) printHelp("transitionlist");
                                            p += 2;
                                            break;
                                            
                                        case "-c": // compute collision optimization parameter
                                            if (!(p + 2 < args.Length)) printHelp("transitionlist");
                                            instrument = args[p + 1];
                                            mode = args[p + 2];
                                            p += 3;
                                            break;
                                            
                                        case "-s": // file split parameter
                                            split = true;
                                            p += 1;
                                            break;
                                            
                                        case "-S": // file split parameter
                                            species = true;
                                            p += 1;
                                            break;
                                            
                                        case "-d":
                                            deleteReplicates = true;
                                            p += 1;
                                            break;
                                            
                                        case "-x":
                                            asDeveloper = true;
                                            p += 1;
                                            break;
                                            
                                        case "-l":
                                            createXMLFile = true;
                                            p += 1;
                                            break;
                                            
                                        default:
                                            printHelp("transitionlist");
                                            break;
                                    }
                                }
                                
                                
                                LipidCreator lc = null;
                                try
                                {
                                    lc = new LipidCreator(null, true);
                                    lc.runMode = runMode;
                                    lc.analytics(LipidCreator.ANALYTICS_CATEGORY, "launch-" + runMode);
                                }
                                catch
                                {
                                    log.Error("An error occurred during the initialization of LipidCreator. For more details, please read the log message 'data/lipidreator.log' and get in contact with the developers.");
                                    return;
                                }
                                
                                
                                if (instrument != "" && (!lc.msInstruments.ContainsKey(instrument) || lc.msInstruments[instrument].minCE < 0)) printHelp("transitionlist");
                                
                                if (mode != "" && mode != "PRM" && mode != "SRM") printHelp("transitionlist");
                                
                                
                                XDocument doc;
                                try 
                                {
                                    doc = XDocument.Load(inputCSV);
                                    lc.import(doc);
                                }
                                catch
                                {
                                    try
                                    {
                                        lc.importLipidList(inputCSV, new int[]{parameterPrecursor, parameterHeavy});
                                    }
                                    catch
                                    {
                                        log.Warn("Closing LipidCreator.");
                                        return;
                                    }
                                }
                                
                                
                                MonitoringTypes monitoringType = MonitoringTypes.NoMonitoring;
                                if (mode == "PRM") monitoringType = MonitoringTypes.PRM;
                                else if (mode == "SRM") monitoringType = MonitoringTypes.SRM;
                                
                                lc.selectedInstrumentForCE = instrument;
                                lc.monitoringType = monitoringType;
                                
                                if (!createXMLFile)
                                {
                                    try
                                    {
                                        lc.assembleLipids(asDeveloper, new ArrayList(){false, (species ? 2 : 0)});
                                    }
                                    catch (LipidException lipidException)
                                    {
                                        string lipidName = lipidException.precursorData.precursorName;
                                        string fragmentName = lipidException.fragment.fragmentName;
                                        string elementName = MS2Fragment.ALL_ELEMENTS[lipidException.molecule].shortcut;
                                        int counts = lipidException.counts;
                                        string heavyIsotope = lipidException.heavyIsotope.Length > 0 ? " the heavy isotope '{" + lipidException.heavyIsotope + "}' of" : "";
                                        log.Error("A problem occurred during the computation of fragment '" + fragmentName + "' for" + heavyIsotope + " lipid '" + lipidName + "'. The element '" + elementName + "' contains " + counts + " counts. Please update the fragment with regard on the element counts.");
                                        Environment.Exit(-1);
                                    }
                                    DataTable transitionList = deleteReplicates ? lc.transitionListUnique : lc.transitionList;
                                    lc.storeTransitionList(",", split, false, outputCSV, transitionList);
                                }
                                else
                                {
                                    string outputDir = System.IO.Path.GetDirectoryName(outputCSV);
                                    if (outputDir.Length > 0) System.IO.Directory.CreateDirectory(outputDir);
                                    using (StreamWriter writer = new StreamWriter (outputCSV))
                                    {
                                        writer.Write(lc.serialize());
                                    }
                                }
                            }
                            break;
                            
                            
                            
                        case "library":
                            runMode = RunMode.commandline;
                            checkForAnalytics(runMode, false);
                            if (args.Length < 4)
                            {
                                printHelp("library");
                            }
                            else
                            {
                                string inputCSV = args[1];
                                string outputCSV = args[2];
                                string instrument = args[3];
                                
                                LipidCreator lc = null;
                                try
                                {
                                    lc = new LipidCreator(null, true);
                                    lc.runMode = runMode;
                                    lc.analytics(LipidCreator.ANALYTICS_CATEGORY, "launch-" + runMode);
                                }
                                catch
                                {
                                    log.Error("An error occurred during the initialization of LipidCreator. For more details, please read the log message 'data/lipidreator.log' and get in contact with the developers.");
                                    return;
                                }
                                
                                if (instrument != "" && (!lc.msInstruments.ContainsKey(instrument) || lc.msInstruments[instrument].minCE < 0)) printHelp("library");
                                
                                lc.selectedInstrumentForCE = instrument;
                                try 
                                {
                                    lc.importLipidList(inputCSV);
                                }
                                catch
                                {
                                    log.Error("An error occurred while importing the lipid list. Please check if the file exists and if has the correct formatting.");
                                    return;
                                }
                                lc.createPrecursorList();
                                lc.createBlib(outputCSV);
                            }
                            break;
                    }
                }
                else 
                {
                    runMode = RunMode.commandline;
                    checkForAnalytics(runMode, false);
                    printHelp();
                }
            }
            else 
            {
                runMode = RunMode.standalone;
                checkForAnalytics(runMode, false);
                if (File.Exists("data/lipidcreator.log")) System.IO.File.WriteAllText("data/lipidcreator.log", string.Empty); // Clearing the log file
                CreatorGUI creatorGUI = new CreatorGUI(null);
                if (!creatorGUI.lipidCreatorInitError)
                {
                    creatorGUI.lipidCreator.runMode = runMode;
                    creatorGUI.lipidCreator.analytics(LipidCreator.ANALYTICS_CATEGORY, "launch-" + runMode);
                }
                Application.Run(creatorGUI);
            }
        }
    }
}
