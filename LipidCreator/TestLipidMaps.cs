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
        
            string grammarFilename = "data/goslin/LipidMaps.g4";
            char quote = '\'';
            int lineCounter;
            
            LipidCreator lipidCreator = new LipidCreator(null);
            LipidMapsParserEventHandler lipidMapsParserEventHandler = new LipidMapsParserEventHandler(lipidCreator);
            Parser parser = new Parser(lipidMapsParserEventHandler, grammarFilename, quote);
            
                        
            string headgroupsFile = "test/lipidmaps.csv";
            if (File.Exists(headgroupsFile))
            {
                lineCounter = 0;
                try
                {
                    HashSet<String> usedKeys = new HashSet<String>();
                    ArrayList precursorDataList = new ArrayList();
                    
                    using (StreamReader sr = new StreamReader(headgroupsFile))
                    {
                        String line;
                        while((line = sr.ReadLine()) != null)
                        {
                            if (lineCounter % 1000 == 0 && lineCounter > 0) Console.WriteLine(lineCounter);
                            lineCounter++;
                            if (line.Length < 2) continue;
                            if (line[0] == '#') continue;
                            
                            string[] tokens = LipidCreator.parseLine(line, ',', '"');
                            if (tokens.Length < 2 || tokens[1].Equals("")) continue;
                            parser.parse(tokens[0]);
                            string translatedName = "";
                            if (parser.wordInGrammar)
                            {
                                parser.raiseEvents();
                                if (lipidMapsParserEventHandler.lipid != null)
                                {
                                
                                    Lipid currentLipid = lipidMapsParserEventHandler.lipid;
                                    currentLipid.computePrecursorData(lipidCreator.headgroups, usedKeys, precursorDataList);
                                    if (precursorDataList.Count == 0)
                                    {
                                        Console.WriteLine("Error: could not correctly translate '" + tokens[0] + "' into '" + tokens[1] + "', no precursor created!");
                                        Environment.Exit(-1);
                                    }
                                    translatedName = ((PrecursorData)precursorDataList[0]).precursorName;
                                    usedKeys.Clear();
                                    precursorDataList.Clear();
                                }
                            
                                if (tokens[1] != translatedName)
                                {
                                    Console.WriteLine("Error: could not correctly translate '" + tokens[0] + "' into '" + tokens[1] + "', got '" + translatedName + "'!");
                                    //Environment.Exit(-1);
                                }
                            }
                            
                            else
                            {
                                Console.WriteLine("Error: could not parse '" + tokens[0] + "'!");
                                Environment.Exit(-1);
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