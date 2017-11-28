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
using System.Data.SQLite;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace LipidCreator
{
    public class TestTransitionList
    {
        public static void Assert(bool condition, string message = "")
        {
            if (!condition)
            {
                throw new Exception("Assert failed: " + message);
            }
        }
        
        public static void Assert(int i1, int i2, string message = "")
        {
            if (i1 != i2)
            {
                throw new Exception("Assert failed: " + message);
            }
        }
        
        public static void Assert(double d1, double d2, string message = "")
        {
            if (Math.Abs(d1 - d2) > 1e-4)
            {
                throw new Exception("Assert failed: " + message);
            }
        }
    
        [STAThread]
        public static void Main(string[] args)
        {
            LipidCreatorForm lcf = new LipidCreatorForm(null);
            
            try {
                //LPC	LPC 18:0	C26H54O7NP	[M+H]	        524.3710668	1	HG(PC)	C5H15O4NP	184.0733215	1
                PLLipid lpc = new PLLipid(lcf.allPathsToPrecursorImages, lcf.allFragments);
                lpc.headGroupNames.Add("LPC"); // set PC
                lpc.adducts["+H"] = true;   // set adduct
                
                lpc.fag1.carbonCounts.Add(18); // set first fatty acid parameters
                lpc.fag1.doubleBondCounts.Add(0);
                lpc.fag1.hydroxylCounts.Add(0);
                lpc.fag2.faTypes["FA"] = false; // unset second fatty acid
                lpc.fag2.faTypes["FAx"] = true;
                
                // unset all fragments except HG(PC)
                foreach (MS2Fragment ms2fragment in lpc.MS2Fragments["LPC"])
                {
                    ms2fragment.fragmentSelected = ms2fragment.fragmentName.Equals("HG(PC)");
                }
                
                lcf.registeredLipids.Add(lpc);
                lcf.assembleLipids();
                Assert(lcf.transitionList.Rows.Count == 2, "1st LPC transition length");
                
                foreach (DataRow row in lcf.transitionList.Rows)
                {
                    if (row[LipidCreatorForm.PRODUCT_NAME].Equals("HG(PC)"))
                    {
                        // precursor
                        Assert(row[LipidCreatorForm.MOLECULE_LIST_NAME].Equals("LPC"), "1st LPC category");
                        Assert(row[LipidCreatorForm.PRECURSOR_NAME].Equals("LPC 18:0"), "1st LPC precursor name");
                        Assert(row[LipidCreatorForm.PRECURSOR_ION_FORMULA].Equals("C26H54O7NP"), "1st LPC precursor formula");
                        Assert(row[LipidCreatorForm.PRECURSOR_ADDUCT].Equals("[M+H]"), "1st LPC precursor adduct");
                        Assert(Convert.ToDouble(row[LipidCreatorForm.PRECURSOR_MZ]), 524.3710668, "1st LPC precursor mass");
                        Assert(Convert.ToInt32(row[LipidCreatorForm.PRECURSOR_CHARGE]), 1, "1st LPC precursor charge");
                        // product
                        Assert(row[LipidCreatorForm.PRODUCT_NAME].Equals("HG(PC)"), "1st LPC product name");
                        Assert(row[LipidCreatorForm.PRODUCT_ION_FORMULA].Equals("C5H15O4NP"), "1st LPC product formula");
                        Assert(Convert.ToDouble(row[LipidCreatorForm.PRODUCT_MZ]), 184.0733215, "1st LPC product mass");
                        Assert(Convert.ToInt32(row[LipidCreatorForm.PRODUCT_CHARGE]), 1, "1st LPC product charge");
                    }
                }
                
                
                
                
                
                //LPC	LPC 18:0	C26H54O7NP	[M+HCOO]	568.3619932	-1	FA	C18H35O2	283.2642541	-1
                // unset all fragments except FA
                foreach (MS2Fragment ms2fragment in lpc.MS2Fragments["LPC"])
                {
                    ms2fragment.fragmentSelected = ms2fragment.fragmentName.Equals("FA");
                }
                lpc.adducts["+H"] = false;   // unset adduct
                lpc.adducts["+HCOO"] = true;   // set adduct
                
                lcf.registeredLipids.Clear();
                lcf.registeredLipids.Add(lpc);
                lcf.assembleLipids();
                Assert(lcf.transitionList.Rows.Count == 2, "2nd LPC transition length");
                
                
                foreach (DataRow row in lcf.transitionList.Rows)
                {
                    if (row[LipidCreatorForm.PRODUCT_NAME].Equals("FA"))
                    {
                        // precursor
                        Assert(row[LipidCreatorForm.MOLECULE_LIST_NAME].Equals("LPC"), "2nd LPC category");
                        Assert(row[LipidCreatorForm.PRECURSOR_NAME].Equals("LPC 18:0"), "2nd LPC precursor name");
                        Assert(row[LipidCreatorForm.PRECURSOR_ION_FORMULA].Equals("C26H54O7NP"), "2nd LPC precursor formula");
                        Assert(row[LipidCreatorForm.PRECURSOR_ADDUCT].Equals("[M+HCOO]"), "2nd LPC precursor adduct");
                        Assert(Convert.ToDouble(row[LipidCreatorForm.PRECURSOR_MZ]), 568.3619932, "2nd LPC precursor mass");
                        Assert(Convert.ToInt32(row[LipidCreatorForm.PRECURSOR_CHARGE]), -1, "2nd LPC precursor charge");
                        // product
                        Assert(row[LipidCreatorForm.PRODUCT_NAME].Equals("FA"), "2nd LPC product name");
                        Assert(row[LipidCreatorForm.PRODUCT_ION_FORMULA].Equals("C18H35O2"), "2nd LPC product formula");
                        Assert(Convert.ToDouble(row[LipidCreatorForm.PRODUCT_MZ]), 283.2642541, "2nd LPC product mass");
                        Assert(Convert.ToInt32(row[LipidCreatorForm.PRODUCT_CHARGE]), -1, "2nd LPC product charge");
                    }
                }
                
                
                
                
                
                //LPC	LPC 18:0	C26H54O7NP	[M+CH3COO]	582.3776432	-1	FA	C18H35O2	283.2642541	-1
                // unset all fragments except HG(PC)
                lpc.adducts["+HCOO"] = false;   // unset adduct
                lpc.adducts["+CH3COO"] = true;   // set adduct
                
                lcf.registeredLipids.Clear();
                lcf.registeredLipids.Add(lpc);
                lcf.assembleLipids();
                Assert(lcf.transitionList.Rows.Count == 2, "3nd LPC transition length");
                
                
                foreach (DataRow row in lcf.transitionList.Rows)
                {
                    if (row[LipidCreatorForm.PRODUCT_NAME].Equals("FA"))
                    {
                        // precursor
                        Assert(row[LipidCreatorForm.MOLECULE_LIST_NAME].Equals("LPC"), "3nd LPC category");
                        Assert(row[LipidCreatorForm.PRECURSOR_NAME].Equals("LPC 18:0"), "3nd LPC precursor name");
                        Assert(row[LipidCreatorForm.PRECURSOR_ION_FORMULA].Equals("C26H54O7NP"), "3nd LPC precursor formula");
                        Assert(row[LipidCreatorForm.PRECURSOR_ADDUCT].Equals("[M+CH3COO]"), "3nd LPC precursor adduct");
                        Assert(Convert.ToDouble(row[LipidCreatorForm.PRECURSOR_MZ]), 582.3776432, "3nd LPC precursor mass");
                        Assert(Convert.ToInt32(row[LipidCreatorForm.PRECURSOR_CHARGE]), -1, "3nd LPC precursor charge");
                        // product
                        Assert(row[LipidCreatorForm.PRODUCT_NAME].Equals("FA"), "3nd LPC product name");
                        Assert(row[LipidCreatorForm.PRODUCT_ION_FORMULA].Equals("C18H35O2"), "3nd LPC product formula");
                        Assert(Convert.ToDouble(row[LipidCreatorForm.PRODUCT_MZ]), 283.2642541, "3nd LPC product mass");
                        Assert(Convert.ToInt32(row[LipidCreatorForm.PRODUCT_CHARGE]), -1, "3nd LPC product charge");
                    }
                }
                
                
                
                
                
                Console.WriteLine("Test passed, no errors found");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}