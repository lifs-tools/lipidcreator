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
using System.IO;
using System.Data.SQLite;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Globalization;
using System.Diagnostics;

namespace LipidCreator
{
    public class TestLipidMaps
    {
    
        [STAThread]
        public static void Main(string[] args)
        {
        
            string grammerFilename = "data/lipidmaps.grammer";
            char quote = '"';
            int lineCounter;
            
            LipidCreator lipidCreator = new LipidCreator(null);
            LipidMapsParserEventHandler lipidMapsParserEventHandler = new LipidMapsParserEventHandler(lipidCreator);
            Parser parser = new Parser(lipidMapsParserEventHandler, grammerFilename, quote);
            
            
            
            /*
            string lipidName = "5-Oxo-ETE-d7";
            HashSet<String> u = new HashSet<String>();
            ArrayList p = new ArrayList();
            parser.parse(lipidName);
            Lipid currLipid = null;
            if (parser.wordInGrammer)
            {
                parser.raiseEvents();
                if (lipidMapsParserEventHandler.lipid != null)
                {
                    currLipid = lipidMapsParserEventHandler.lipid;
                    currLipid.computePrecursorData(lipidCreator.headgroups, u, p);
                    Console.WriteLine(lipidName + " -> " + ((PrecursorData)p[p.Count - 1]).precursorName);
                }
                else
                {
                    Console.WriteLine("error during event raising");
                }
            }
            else
            {
                Console.WriteLine("lipid is not in grammer");
            }
            Console.WriteLine("lipid is " + (currLipid != null ? "valid" : "unvalid"));
            
            Environment.Exit(-1);
            */
            
            
            
                        
            string headgroupsFile = "test/lipidmaps.csv";
            if (File.Exists(headgroupsFile))
            {
                lineCounter = 1;
                try
                {
                    HashSet<String> usedKeys = new HashSet<String>();
                    ArrayList precursorDataList = new ArrayList();
                    
                    using (StreamReader sr = new StreamReader(headgroupsFile))
                    {
                        String line = sr.ReadLine(); // omit titles
                        while((line = sr.ReadLine()) != null)
                        {
                            if (lineCounter % 1000 == 0) Console.WriteLine(lineCounter);
                            lineCounter++;
                            if (line.Length < 2) continue;
                            if (line[0] == '#') continue;
                            
                            string[] tokens = LipidCreator.parseLine(line);
                            
                            parser.parse(tokens[0]);
                            string translatedName = "";
                            if (parser.wordInGrammer)
                            {
                                parser.raiseEvents();
                                if (lipidMapsParserEventHandler.lipid != null)
                                {
                                    Lipid currentLipid = lipidMapsParserEventHandler.lipid;
                                    currentLipid.computePrecursorData(lipidCreator.headgroups, usedKeys, precursorDataList);
                                    translatedName =  ((PrecursorData)precursorDataList[precursorDataList.Count - 1]).precursorName;
                                    usedKeys.Clear();
                                }
                            }
                            
                            if (tokens.Length >= 2)
                            {
                                if (tokens[1] != translatedName)
                                {
                                    Console.WriteLine("Error: could not correctly translate '" + tokens[0] + "' into '" + tokens[1] + "', got '" + translatedName + "'!");
                                    Environment.Exit(-1);
                                }
                            }
                        }
                    }
                    Console.WriteLine("All identified 'lipid maps' lipid names successfully translated.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file '" + headgroupsFile + "' in line '" + lineCounter + "' could not be read:");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
            else
            {
                Console.WriteLine("Error: file " + headgroupsFile + " does not exist or can not be opened.");
            }
        }
    }
}