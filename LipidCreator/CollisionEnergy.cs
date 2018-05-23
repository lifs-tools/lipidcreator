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
using System.Net;
using System.Threading;



namespace LipidCreator
{  
    [Serializable]
    public class CollisionEnergy
    { 
        // instrument CV term -> class -> fragment -> adduct -> charge -> parameter -> value
        public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, Dictionary<string, string>>>>>> instrumentParameters;
        public Dictionary<string, Func<Dictionary<string, string>, double, double>> intensityFunctions;
        public Dictionary<string, Func<Dictionary<string, string>, double>> optimalCEFunctions;
        
        
        public double square(double x)
        {
            return x * x;
        }
        
        public CollisionEnergy()
        {
            instrumentParameters = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, Dictionary<string, string>>>>>>();
            intensityFunctions = new Dictionary<string, Func<Dictionary<string, string>, double, double>>();
            intensityFunctions.Add("unNormDlnormPar", intensityLogNormal);
            
            optimalCEFunctions = new Dictionary<string, Func<Dictionary<string, string>, double>>();
            optimalCEFunctions.Add("unNormDlnormPar", optimalCollisionEnergyLogNormal);
        }
        
        
        
        
        public double getCollisionEnergy(string instrument, string lipidClass, string fragment, string adduct, int charge)
        {
            double energy = -1;
            if (instrumentParameters.ContainsKey(instrument))
            {
            
                Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, Dictionary<string, string>>>>> parLevel1 = instrumentParameters[instrument];
                if (parLevel1.ContainsKey(lipidClass))
                {
                
                    Dictionary<string, Dictionary<string, Dictionary<int, Dictionary<string, string>>>> parLevel2 = parLevel1[lipidClass];
                    if (parLevel2.ContainsKey(fragment))
                    {
                        
                        Dictionary<string, Dictionary<int, Dictionary<string, string>>> parLevel3 = parLevel2[fragment];
                        if (parLevel3.ContainsKey(adduct))
                        {
                        
                            Dictionary<int, Dictionary<string, string>> parLevel4 = parLevel3[adduct];
                            if (parLevel4.ContainsKey(charge))
                            {
                            
                                Dictionary<string, string> parLevel5 = parLevel4[charge];
                                if (parLevel5.ContainsKey("model"))
                                {
                                    string model = parLevel5["model"];
                                    if (optimalCEFunctions.ContainsKey(model))
                                    {
                                        energy = optimalCEFunctions[model](parLevel5);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return energy;
        }
        
        
        
        
        
        
        public double getIntensity(string instrument, string lipidClass, string fragment, string adduct, int charge, double collisionEnergy)
        {
            double intensity = MS2Fragment.DEFAULT_INTENSITY;
            if (instrumentParameters.ContainsKey(instrument))
            {
            
                Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, Dictionary<string, string>>>>> parLevel1 = instrumentParameters[instrument];
                if (parLevel1.ContainsKey(lipidClass))
                {
                
                    Dictionary<string, Dictionary<string, Dictionary<int, Dictionary<string, string>>>> parLevel2 = parLevel1[lipidClass];
                    if (parLevel2.ContainsKey(fragment))
                    {
                        
                        Dictionary<string, Dictionary<int, Dictionary<string, string>>> parLevel3 = parLevel2[fragment];
                        if (parLevel3.ContainsKey(adduct))
                        {
                        
                            Dictionary<int, Dictionary<string, string>> parLevel4 = parLevel3[adduct];
                            if (parLevel4.ContainsKey(charge))
                            {
                            
                                Dictionary<string, string> parLevel5 = parLevel4[charge];
                                if (parLevel5.ContainsKey("model"))
                                {
                                    string model = parLevel5["model"];
                                    if (intensityFunctions.ContainsKey(model))
                                    {
                                        intensity = intensityFunctions[model](parLevel5, collisionEnergy);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return intensity;
        }
        
        
        
        
        public double optimalCollisionEnergyLogNormal(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("meanlog") && parameters.ContainsKey("sdlog") && parameters.ContainsKey("shift"))
            {
                double m = Convert.ToDouble(parameters.ContainsKey("meanlog"));
                double sd = Convert.ToDouble(parameters.ContainsKey("sdlog"));
                double sft = Convert.ToDouble(parameters.ContainsKey("shift"));
                return Math.Exp(m - square(sd)) + sft;
            }
            return -1;
        }
        
        public double intensityLogNormal(Dictionary<string, string> parameters, double collisionEnergy)
        {
            if (parameters.ContainsKey("meanlog") && parameters.ContainsKey("sdlog") && parameters.ContainsKey("shift") && parameters.ContainsKey("scale"))
            {
                double m = Convert.ToDouble(parameters.ContainsKey("meanlog"));
                double scl = Convert.ToDouble(parameters.ContainsKey("scale"));
                double sd = Convert.ToDouble(parameters.ContainsKey("sdlog"));
                double sft = Convert.ToDouble(parameters.ContainsKey("shift"));
                return scl / ((collisionEnergy - sft) * sd * Math.Sqrt(2 * Math.PI)) * Math.Exp(-square(Math.Log(collisionEnergy - sft) - m) / (2 * square(sd)));
            }
            return -1;
        }
    }
}