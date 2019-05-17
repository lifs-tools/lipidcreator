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
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;
using System.Text;


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
        public IDictionary<string, IDictionary<string, IDictionary<string, IDictionary<string, IDictionary<string, string>>>>> instrumentParameters;
        public IDictionary<string, IDictionary<string, IDictionary<string, double>>> collisionEnergies;
        public IDictionary<string, Func<IDictionary<string, string>, double, double>> intensityFunctions;
        public IDictionary<string, Func<IDictionary<string, string>, double, double, double, double, double[]>> curveFunctions;
        public IDictionary<string, Func<IDictionary<string, string>, double>> optimalCEFunctions;
        
        
        public static double square(double x)
        {
            return x * x;
        }
        
        public CollisionEnergy()
        {
            instrumentParameters = new SortedList<string, IDictionary<string, IDictionary<string, IDictionary<string, IDictionary<string, string>>>>>();
            intensityFunctions = new Dictionary<string, Func<IDictionary<string, string>, double, double>>();
            intensityFunctions.Add("dlnormPar", intensityLogNormal);
            
            curveFunctions = new Dictionary<string, Func<IDictionary<string, string>, double, double, double, double, double[]>>();
            curveFunctions.Add("dlnormPar", computeLogNormalCurve);
            
            optimalCEFunctions = new Dictionary<string, Func<IDictionary<string, string>, double>>();
            optimalCEFunctions.Add("dlnormPar", optimalCollisionEnergyLogNormal);
            
            collisionEnergies = new SortedList<string, IDictionary<string, IDictionary<string, double>>>();
        }
        
        
        public void import(XElement node, string importVersion)
        {
            foreach (var instrumentXML in node.Descendants("ins"))
            {
                
                string instrumentType = instrumentXML.Attribute("type").Value;
                if (!instrumentParameters.ContainsKey(instrumentType)) continue;
                IDictionary<string, IDictionary<string, IDictionary<string, IDictionary<string, string>>>> d1 = instrumentParameters[instrumentType];
                
                IDictionary<string, IDictionary<string, double>> ce1 = null;
                if (collisionEnergies.ContainsKey(instrumentType)) ce1 = collisionEnergies[instrumentType];
                
                foreach (var lipidClassXML in instrumentXML.Descendants("lCl"))
                {
                    string lipidClassType = lipidClassXML.Attribute("type").Value;
                    if (!d1.ContainsKey(lipidClassType)) continue;
                    IDictionary<string, IDictionary<string, IDictionary<string, string>>> d2 = d1[lipidClassType];
                    IDictionary<string, double> ce2 = null;
                    if (ce1.ContainsKey(lipidClassType)) ce2 = ce1[lipidClassType];
                
                    foreach (var adductXML in lipidClassXML.Descendants("adt"))
                    {
                        string adductType = adductXML.Attribute("type").Value;
                        if (!d2.ContainsKey(adductType)) continue;
                        IDictionary<string, IDictionary<string, string>> d3 = d2[adductType];
                        double ce = Convert.ToDouble(adductXML.Attribute("ce").Value, CultureInfo.InvariantCulture);
                        if (ce2.ContainsKey(adductType)) ce2[adductType] = ce;
                                                
                        foreach (var fragmentXML in adductXML.Descendants("fr"))
                        {
                            string fragmentType = fragmentXML.Attribute("type").Value;
                            if (!d3.ContainsKey(fragmentType)) continue;
                            IDictionary<string, string> d4 = d3[fragmentType];
                            
                            string fragmentSelected = fragmentXML.Attribute("sel").Value;
                            if (!d4.ContainsKey("selected"))
                            {
                                d4.Add("selected", fragmentSelected);
                            }
                            else 
                            {
                                d4["selected"] = fragmentSelected;
                            }
                        }
                    }
                }
            }
        }
        
        
        
        public void serialize(StringBuilder sb)
        {
            sb.Append("<CE>\n");
            foreach(KeyValuePair<string, IDictionary<string, IDictionary<string, IDictionary<string, IDictionary<string, string>>>>> kvp1 in instrumentParameters)
            {
                sb.Append("<ins type=\"" + kvp1.Key + "\">\n");
                
            
                // foreach class
                foreach(KeyValuePair<string, IDictionary<string, IDictionary<string, IDictionary<string, string>>>> kvp2 in kvp1.Value)
                {
                    sb.Append("<lCl type=\"" + kvp2.Key + "\">\n");
                
                    // foreach adduct
                    foreach(KeyValuePair<string, IDictionary<string, IDictionary<string, string>>> kvp3 in kvp2.Value)
                    {
                        sb.Append("<adt type=\"" + kvp3.Key + "\" ce=\""+ string.Format(new CultureInfo("en-US"), "{0:0.000}", collisionEnergies[kvp1.Key][kvp2.Key][kvp3.Key]) + "\">\n");
                    
                        foreach(KeyValuePair<string, IDictionary<string, string>> kvp4 in kvp3.Value)
                        {
                            sb.Append("<fr type=\"" + kvp4.Key + "\" sel=\"" + kvp4.Value["selected"] + "\" />\n");
                        }
                        sb.Append("</adt>\n");
                    }
                    sb.Append("</lCl>\n");
                }
                sb.Append("</ins>\n");
            }
            sb.Append("</CE>\n");
        }
        
        
        
        public void addCollisionEnergyFields()
        {
            foreach(KeyValuePair<string, IDictionary<string, IDictionary<string, IDictionary<string, IDictionary<string, string>>>>> kvp1 in instrumentParameters)
            {
                IDictionary<string, IDictionary<string, double>> ce1 = new Dictionary<string, IDictionary<string, double>>();
                collisionEnergies.Add(kvp1.Key, ce1);
            
                // foreach class
                foreach(KeyValuePair<string, IDictionary<string, IDictionary<string, IDictionary<string, string>>>> kvp2 in kvp1.Value)
                {
                    IDictionary<string, double> ce2 = new SortedList<string, double>();
                    ce1.Add(kvp2.Key, ce2);
                
                    // foreach adduct
                    foreach(KeyValuePair<string, IDictionary<string, IDictionary<string, string>>> kvp3 in kvp2.Value)
                    {
                        ce2.Add(kvp3.Key, -1);
                    
                        foreach(KeyValuePair<string, IDictionary<string, string>> kvp4 in kvp3.Value)
                        {
                            kvp4.Value.Add("selected", "1");
                        }
                    }
                }
            }
        }
        
        
        
        
        public void computeDefaultCollisionEnergy(InstrumentData instrumentData, string lipidClass, string adduct)
        {
            double minX = instrumentData.minCE;
            double maxX = instrumentData.maxCE;
            
            double[] product = null;
        
            if (!instrumentParameters.ContainsKey(instrumentData.CVTerm)) return;
            if (!instrumentParameters[instrumentData.CVTerm].ContainsKey(lipidClass)) return;
            if (!instrumentParameters[instrumentData.CVTerm][lipidClass].ContainsKey(adduct)) return;
                    
            // foreach fragment
            foreach(KeyValuePair<string, IDictionary<string, string>> kvp4 in instrumentParameters[instrumentData.CVTerm][lipidClass][adduct])
            {
                if (kvp4.Value["selected"] != "1") continue;
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
                
            
            double argMaxY = -1;
            if (product != null)
            {
                double maxY = 0;
                for (int i = 0; i < product.Length; ++i)
                {
                    if (maxY < product[i])
                    {
                        argMaxY = minX + (double)i / 100.0;
                        maxY = product[i];
                    }
                }
            }
            
            collisionEnergies[instrumentData.CVTerm][lipidClass][adduct] = argMaxY;
        }
        
        
        
        public double getCollisionEnergy(string instrument, string lipidClass, string adduct)
        {
            double energy = -1;
            if (collisionEnergies.ContainsKey(instrument))
            {
            
                IDictionary<string, IDictionary<string, double>> parLevel1 = collisionEnergies[instrument];
                if (parLevel1.ContainsKey(lipidClass))
                {
                
                    IDictionary<string, double> parLevel2 = parLevel1[lipidClass];
                    if (parLevel2.ContainsKey(adduct))
                    {
                        energy = parLevel2[adduct];
                    }
                }
            }
            return energy;
        }
        
        
        
        public double getApex(string instrument, string lipidClass, string adduct, string fragment)
        {
            double energy = -1;
            if (instrumentParameters.ContainsKey(instrument))
            {
            
                IDictionary<string, IDictionary<string, IDictionary<string, IDictionary<string, string>>>> parLevel1 = instrumentParameters[instrument];
                if (parLevel1.ContainsKey(lipidClass))
                {
                
                    IDictionary<string, IDictionary<string, IDictionary<string, string>>> parLevel2 = parLevel1[lipidClass];
                    if (parLevel2.ContainsKey(adduct))
                    {
                        
                        IDictionary<string, IDictionary<string, string>> parLevel3 = parLevel2[adduct];
                        if (parLevel3.ContainsKey(fragment))
                        {
                        
                            IDictionary<string, string> parLevel4 = parLevel3[fragment];
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
            
                IDictionary<string, IDictionary<string, IDictionary<string, IDictionary<string, string>>>> parLevel1 = instrumentParameters[instrument];
                if (parLevel1.ContainsKey(lipidClass))
                {
                    IDictionary<string, IDictionary<string, IDictionary<string, string>>> parLevel2 = parLevel1[lipidClass];
                    if (parLevel2.ContainsKey(adduct))
                    {
                        
                        IDictionary<string, IDictionary<string, string>> parLevel3 = parLevel2[adduct];
                        if (parLevel3.ContainsKey(fragment))
                        {
                            IDictionary<string, string> parLevel4 = parLevel3[fragment];
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
            Func<IDictionary<string, string>, double, double> intensityFunction = null;
            IDictionary<string, string> parameters = null;
            double[] curve = new double[xValues.Length];
            if (instrumentParameters.ContainsKey(instrument))
            {
            
                IDictionary<string, IDictionary<string, IDictionary<string, IDictionary<string, string>>>> parLevel1 = instrumentParameters[instrument];
                if (parLevel1.ContainsKey(lipidClass))
                {
                
                    IDictionary<string, IDictionary<string, IDictionary<string, string>>> parLevel2 = parLevel1[lipidClass];
                    if (parLevel2.ContainsKey(adduct))
                    {
                        
                        IDictionary<string, IDictionary<string, string>> parLevel3 = parLevel2[adduct];
                        if (parLevel3.ContainsKey(fragment))
                        {
                            IDictionary<string, string> parLevel4 = parLevel3[fragment];
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
        
        
        
        
        
        public static double optimalCollisionEnergyLogNormal(IDictionary<string, string> parameters)
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
        
        
        
        public static double intensityLogNormal(IDictionary<string, string> parameters, double collisionEnergy)
        {
            if (parameters.ContainsKey("meanlog") && parameters.ContainsKey("sdlog") && parameters.ContainsKey("shift") && parameters.ContainsKey("scale"))
            {
                double scl = Convert.ToDouble(parameters["scale"], CultureInfo.InvariantCulture);
                double m = Convert.ToDouble(parameters["meanlog"], CultureInfo.InvariantCulture);
                double sd = Convert.ToDouble(parameters["sdlog"], CultureInfo.InvariantCulture);
                double sft = Convert.ToDouble(parameters["shift"], CultureInfo.InvariantCulture);
                collisionEnergy += sft;
                
                if (collisionEnergy <= 0) return 0;
                
                return (scl / (collisionEnergy * sd * Math.Sqrt(2 * Math.PI)) * Math.Exp(-square(Math.Log(collisionEnergy) - m) / (2 * square(sd))));
            }
            return -1;
        }
        
        
        
        public static double[] computeLogNormalCurve(IDictionary<string, string> parameters, double start, double end, double stepsPerUnit, double scale = 1)
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
                for (int i = 0; i < first.Length; ++i) 
                { 
                    product[i] /= norm;
                }
            }
            
            return product;
        }
    }
}
