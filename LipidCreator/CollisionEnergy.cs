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
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Globalization;



namespace LipidCreator
{  

    [Serializable]
    public class InstrumentData
    {
        public string CVTerm = "";
        public string model = "";
        public double minCE = -1;
        public double maxCE = -1;
        public string xAxisLabel = "";
        public HashSet<string> modes = new HashSet<string>();
    }

    [Serializable]
    public class CollisionEnergy
    { 
        // instrument CV term -> class -> fragment -> adduct -> charge -> parameter -> value
        public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> instrumentParameters;
        public Dictionary<string, Func<Dictionary<string, string>, double, double>> intensityFunctions;
        public Dictionary<string, Func<Dictionary<string, string>, double, double, double, double, double[]>> curveFunctions;
        public Dictionary<string, Func<Dictionary<string, string>, double>> optimalCEFunctions;
        public volatile bool fieldsComputed = false;
        
        
        public static double square(double x)
        {
            return x * x;
        }
        
        public CollisionEnergy()
        {
            instrumentParameters = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>();
            intensityFunctions = new Dictionary<string, Func<Dictionary<string, string>, double, double>>();
            intensityFunctions.Add("dlnormPar", intensityLogNormal);
            
            curveFunctions = new Dictionary<string, Func<Dictionary<string, string>, double, double, double, double, double[]>>();
            curveFunctions.Add("dlnormPar", computeLogNormalCurve);
            
            optimalCEFunctions = new Dictionary<string, Func<Dictionary<string, string>, double>>();
            optimalCEFunctions.Add("dlnormPar", optimalCollisionEnergyLogNormal);
        }
        
        
        
        
        public void addCollisionEnergyFields(Dictionary<string, InstrumentData> msInstruments)
        {
        
            Thread th = new Thread(() => addCollisionEnergyFieldsThread(msInstruments));
            th.Start();
        }
        
        
        
        
        public void addCollisionEnergyFieldsThread(Dictionary<string, InstrumentData> msInstruments)
        {
            foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> kvp1 in instrumentParameters)
            {
                double minX = (double)msInstruments[kvp1.Key].minCE;
                double maxX = (double)msInstruments[kvp1.Key].maxCE;
                // foreach class
                foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> kvp2 in kvp1.Value)
                {
                    // foreach adduct
                    foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, string>>> kvp3 in kvp2.Value)
                    {
                        
                        double[] product = null;
                    
                        // foreach fragment
                        foreach(KeyValuePair<string, Dictionary<string, string>> kvp4 in kvp3.Value)
                        {
                            if (product == null)
                            {
                                product = curveFunctions[kvp4.Value["model"]](kvp4.Value, minX, maxX, 100, 1);
                            }
                            else
                            {
                                double[] second = curveFunctions[kvp4.Value["model"]](kvp4.Value, minX, maxX, 100, 1);
                                product = productTwoDistributions(product, second);
                            }
                        }
                        
                        double argMaxY = 0;
                        double maxY = 0;
                        for (int i = 0; i < product.Length; ++i)
                        {
                            if (maxY < product[i])
                            {
                                argMaxY = minX + (double)i / 100.0;
                                maxY = product[i];
                            }
                        }
                        
                        foreach(KeyValuePair<string, Dictionary<string, string>> kvp4 in kvp3.Value)
                        {
                            kvp4.Value.Add("CE", String.Format(new CultureInfo("en-US"), "{0:0.00}", argMaxY));
                        }
                    }
                }
            }
            fieldsComputed = true;
        }
        
        
        
        public double getCollisionEnergy(string instrument, string lipidClass, string adduct, string fragment)
        {
            double energy = -1;
            if (instrumentParameters.ContainsKey(instrument))
            {
            
                Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> parLevel1 = instrumentParameters[instrument];
                if (parLevel1.ContainsKey(lipidClass))
                {
                
                    Dictionary<string, Dictionary<string, Dictionary<string, string>>> parLevel2 = parLevel1[lipidClass];
                    if (parLevel2.ContainsKey(adduct))
                    {
                        Dictionary<string, Dictionary<string, string>> parLevel3 = parLevel2[adduct];
                        if (parLevel3.ContainsKey(fragment))
                        {
                            Console.WriteLine(fragment);
                            Dictionary<string, string> parLevel4 = parLevel3[fragment];
                            if (parLevel4.ContainsKey("CE"))
                            {
                                energy = Convert.ToDouble(parLevel4["CE"], CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }
            }
            Console.WriteLine(instrument + " " + lipidClass + " " + adduct + " " + fragment + " " + energy);
            return energy;
        }
        
        
        
        public double getApex(string instrument, string lipidClass, string adduct, string fragment)
        {
            double energy = -1;
            if (instrumentParameters.ContainsKey(instrument))
            {
            
                Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> parLevel1 = instrumentParameters[instrument];
                if (parLevel1.ContainsKey(lipidClass))
                {
                
                    Dictionary<string, Dictionary<string, Dictionary<string, string>>> parLevel2 = parLevel1[lipidClass];
                    if (parLevel2.ContainsKey(adduct))
                    {
                        
                        Dictionary<string, Dictionary<string, string>> parLevel3 = parLevel2[adduct];
                        if (parLevel3.ContainsKey(fragment))
                        {
                        
                            Dictionary<string, string> parLevel4 = parLevel3[fragment];
                            if (parLevel4.ContainsKey("model"))
                            {
                            
                                string model = parLevel4["model"];
                                if (optimalCEFunctions.ContainsKey(model))
                                {
                                    energy = optimalCEFunctions[model](parLevel4);
                                }
                            }
                        }
                    }
                }
            }
            return energy;
        }
        
        
        public double getIntensity(string instrument, string lipidClass, string adduct, string fragment, double collisionEnergy)
        {
            double intensity = MS2Fragment.DEFAULT_INTENSITY;
            if (instrumentParameters.ContainsKey(instrument))
            {
            
                Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> parLevel1 = instrumentParameters[instrument];
                if (parLevel1.ContainsKey(lipidClass))
                {
                
                    Dictionary<string, Dictionary<string, Dictionary<string, string>>> parLevel2 = parLevel1[lipidClass];
                    if (parLevel2.ContainsKey(adduct))
                    {
                        
                        Dictionary<string, Dictionary<string, string>> parLevel3 = parLevel2[adduct];
                        if (parLevel3.ContainsKey(fragment))
                        {
                            Dictionary<string, string> parLevel4 = parLevel3[fragment];
                            if (parLevel4.ContainsKey("model"))
                            {
                                string model = parLevel4["model"];
                                if (intensityFunctions.ContainsKey(model))
                                {
                                    intensity = intensityFunctions[model](parLevel4, collisionEnergy);
                                }
                            }
                        }
                    }
                }
            }
            return intensity;
        }
        
        
        public double[] getIntensityCurve(string instrument, string lipidClass, string adduct, string fragment, double[] xValues, double scale = 1.0)
        {
            Func<Dictionary<string, string>, double, double> intensityFunction = null;
            Dictionary<string, string> parameters = null;
            double[] curve = new double[xValues.Length];
            if (instrumentParameters.ContainsKey(instrument))
            {
            
                Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> parLevel1 = instrumentParameters[instrument];
                if (parLevel1.ContainsKey(lipidClass))
                {
                
                    Dictionary<string, Dictionary<string, Dictionary<string, string>>> parLevel2 = parLevel1[lipidClass];
                    if (parLevel2.ContainsKey(adduct))
                    {
                        
                        Dictionary<string, Dictionary<string, string>> parLevel3 = parLevel2[adduct];
                        if (parLevel3.ContainsKey(fragment))
                        {
                            Dictionary<string, string> parLevel4 = parLevel3[fragment];
                            if (parLevel4.ContainsKey("model"))
                            {
                                string model = parLevel4["model"];
                                if (intensityFunctions.ContainsKey(model))
                                {
                                    intensityFunction = intensityFunctions[model];
                                    parameters = parLevel4;
                                }
                            }
                        }
                    }
                }
            }
            
            
            
            if (parameters != null)
            {
                int i = 0;
                while (i < curve.Length)
                {
                    curve[i] = scale * intensityFunction(parameters, xValues[i]);
                    i++;
                }
            }
            else
            {
            int i = 0;
                while (i < curve.Length)
                {
                    curve[i] = MS2Fragment.DEFAULT_INTENSITY;
                    i++;
                }
            }
            
            return curve;
        }
        
        
        
        
        
        public static double optimalCollisionEnergyLogNormal(Dictionary<string, string> parameters)
        {
        
            if (parameters.ContainsKey("meanlog") && parameters.ContainsKey("sdlog") && parameters.ContainsKey("shift"))
            {
                double m = Convert.ToDouble(parameters["meanlog"], CultureInfo.InvariantCulture);
                double sd = Convert.ToDouble(parameters["sdlog"], CultureInfo.InvariantCulture);
                double sft = Convert.ToDouble(parameters["shift"], CultureInfo.InvariantCulture);
                return Math.Exp(m - square(sd)) - sft;
            }
            return -1;
        }
        
        
        
        public static double intensityLogNormal(Dictionary<string, string> parameters, double collisionEnergy)
        {
            if (parameters.ContainsKey("meanlog") && parameters.ContainsKey("sdlog") && parameters.ContainsKey("shift") && parameters.ContainsKey("scale"))
            {
                double scl = Convert.ToDouble(parameters["scale"], CultureInfo.InvariantCulture);
                double m = Convert.ToDouble(parameters["meanlog"], CultureInfo.InvariantCulture);
                double sd = Convert.ToDouble(parameters["sdlog"], CultureInfo.InvariantCulture);
                double sft = Convert.ToDouble(parameters["shift"], CultureInfo.InvariantCulture);
                collisionEnergy += sft;
                
                if (collisionEnergy < 0) return -1;
                
                return scl / (collisionEnergy * sd * Math.Sqrt(2 * Math.PI)) * Math.Exp(-square(Math.Log(collisionEnergy) - m) / (2 * square(sd)));
            }
            return -1;
        }
        
        
        
        public static double[] computeLogNormalCurve(Dictionary<string, string> parameters, double start, double end, double stepsPerUnit, double scale = 1)
        {
            double[] curve = new double[(int)((end - start) * stepsPerUnit + 1)];
            double x = start;
            double adding = 1.0 / stepsPerUnit;
            int i = 0;
            
            while (i < curve.Length)
            {
                curve[i++] = scale * intensityLogNormal(parameters, x);
                x += adding;
            }
            
            return curve;
        }
        
        
        
        // assuming both distribution are stored in arrays of same length with same corresponding x values
        public static double[] productTwoDistributions(double[] first, double[] second)
        {
            double[] product = new double[first.Length];
            double norm = 0;
            for (int i = 0; i < first.Length; ++i)
            {
                product[i] = first[i] * second[i];
                norm += product[i];
            }
            
            if (norm > 0)
            {
                for (int i = 0; i < first.Length; ++i) product[i] /= norm;
            }
            
            return product;
        }
    }
}