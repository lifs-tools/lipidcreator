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
using System.Linq;
using System.IO;






namespace LipidCreator
{    
    
    [Serializable]
    public class ParserEventHandler : BaseParserEventHandler
    {
        public LipidCreator lipidCreator;
        public Lipid lipid;
        public FattyAcidGroupEnumerator fagEnum;
        public FattyAcidGroup fag;
        public int charge;
        public string adduct;
        public bool sortedSeparator;
        public bool expectsEther;
        public int ethers;
        public bool makeUnsupported = false;
        
        public int heavyIsotope = 0;
        public string heavyElement = "";
        public int heavyCount = 1;
        public ElementDictionary heavyElementCounts = null;
        public ArrayList heavyElementCountList = new ArrayList();
        public string heavyName = "";
        public bool addHeavyPrecursor = false;
        
        public string fragmentName = "";
        public bool addHeavyFragment = false;
        
        
        public IsotopeParserEventHandler ipeh = null;
        public Parser ip = null;
    
    
        public ParserEventHandler(LipidCreator _lipidCreator) : base()
        {
            lipidCreator = _lipidCreator;
            resetLipidBuilder(null);
            
            
            string isotopeGrammarFilename = Path.Combine(lipidCreator.prefixPath, "data", "isotope-formula.grammar");
            ipeh = new IsotopeParserEventHandler();
            ip = new Parser(ipeh, isotopeGrammarFilename, '\'');
            
            
            
            registeredEvents.Add("lipid_pre_event", resetLipidBuilder);
            registeredEvents.Add("lipid_post_event", lipidPostEvent);
            
            registeredEvents.Add("fa_pre_event", FAPreEvent);
            registeredEvents.Add("fa_post_event", FAPostEvent);
            
            registeredEvents.Add("lcb_pre_event", LCBPreEvent);
            registeredEvents.Add("lcb_post_event", LCBPostEvent);
            registeredEvents.Add("carbon_pre_event", CarbonPreEvent);
            registeredEvents.Add("db_count_pre_event", DB_countPreEvent);
            registeredEvents.Add("hydroxyl_pre_event", HydroxylPreEvent);
            registeredEvents.Add("old_hydroxyl_pre_event", OldHydroxylPreEvent);
            registeredEvents.Add("ether_pre_event", EtherPreEvent);
            
            registeredEvents.Add("gl_pre_event", GLPreEvent);
            registeredEvents.Add("pl_pre_event", PLPreEvent);
            registeredEvents.Add("sl_pre_event", SLPreEvent);
            registeredEvents.Add("cholesterol_pre_event", CholesterolPreEvent);
            registeredEvents.Add("mediator_pre_event", MediatorPreEvent);
            
            registeredEvents.Add("hg_mgl_pre_event", HG_MGLPreEvent);
            registeredEvents.Add("hg_dgl_pre_event", HG_DGLPreEvent);
            registeredEvents.Add("hg_sgl_pre_event", HG_SGLPreEvent);
            registeredEvents.Add("hg_tgl_pre_event", HG_TGLPreEvent);
            
            registeredEvents.Add("hg_cl_pre_event", HG_CLPreEvent);
            registeredEvents.Add("hg_mlcl_pre_event", HG_MLCLPreEvent);
            registeredEvents.Add("hg_pl_pre_event", HG_PLPreEvent);
            registeredEvents.Add("hg_lpl_pre_event", HG_LPLPreEvent);
            registeredEvents.Add("hg_lpl_o_pre_event", HG_LPL_OPreEvent);
            registeredEvents.Add("hg_pl_o_pre_event", HG_PL_OPreEvent);
            
            registeredEvents.Add("hg_lsl_pre_event", HG_LSLPreEvent);
            registeredEvents.Add("hg_dsl_pre_event", HG_DSLPreEvent);
            
            registeredEvents.Add("ch_pre_event", ChPreEvent);
            registeredEvents.Add("hg_che_pre_event", HG_ChEPreEvent);
            
            registeredEvents.Add("dpl_post_event", DPLPostEvent);
            registeredEvents.Add("dpl_o_post_event", DPLPostEvent);
            registeredEvents.Add("sl_post_event", SLPostEvent);
            registeredEvents.Add("mediator_post_event", MediatorPostEvent);
            
            registeredEvents.Add("adduct_pre_event", adductPreEvent);
            registeredEvents.Add("charge_pre_event", chargePreEvent);
            registeredEvents.Add("charge_sign_pre_event", charge_signPreEvent);
            registeredEvents.Add("sorted_fa_separator_pre_event", sortedFASeparatorPreEvent);
            
            registeredEvents.Add("heavy_pre_event", resetHeavy);
            registeredEvents.Add("isotope_pre_event", resetHeavyIsotope);
            registeredEvents.Add("isotope_post_event", addHeavyPrecursorElement);
            registeredEvents.Add("isotope_number_pre_event", addIsotopeNumber);
            registeredEvents.Add("isotope_element_pre_event", addIsotopeElement);
            registeredEvents.Add("isotope_count_pre_event", addIsotopeCount);
            registeredEvents.Add("heavy_hg_post_event", setHGIsotopes);
            registeredEvents.Add("gl_species_pre_event", unsupportedEvent);
            registeredEvents.Add("pl_species_pre_event", unsupportedEvent);
            registeredEvents.Add("sl_species_pre_event", unsupportedEvent);
            
            
            registeredEvents.Add("fragment_name_pre_event", setFragmentName);
        }
        
        
        
        public void resetLipidBuilder(Parser.TreeNode node)
        {
            lipid = null;
            fagEnum = null;
            fag = null;
            charge = 0;
            adduct = "";
            sortedSeparator = false;
            expectsEther = false;
            ethers = 0;
            
            heavyIsotope = 0;
            heavyElement = "";
            heavyCount = 1;
            heavyElementCounts = null;
            heavyElementCountList = new ArrayList();
            heavyElementCountList.Add(MS2Fragment.createEmptyElementDict());
            heavyName = "";
            addHeavyPrecursor = false;
        
            fragmentName = "";
            addHeavyFragment = false;
        }
        
        
        
        
        public void setFragmentName(Parser.TreeNode node)
        {
            fragmentName = node.getText();
            heavyElementCounts = null;
        }
        
        
        
        
        
        public void resetHeavy(Parser.TreeNode node)
        {
            heavyIsotope = 0;
            heavyElement = "";
            heavyCount = 1;
            heavyElementCounts = MS2Fragment.createEmptyElementDict();
            heavyName += node.getText();
            addHeavyPrecursor = true;
        }
        
        
        
        
        
        public void resetHeavyIsotope(Parser.TreeNode node)
        {
            heavyIsotope = 0;
            heavyElement = "";
            heavyCount = 1;
        }
        
        
        
        
        public void addIsotopeNumber(Parser.TreeNode node)
        {
            heavyIsotope = Convert.ToInt32(node.getText());
        }
        
        
        
        
        public void addIsotopeElement(Parser.TreeNode node)
        {
            heavyElement = node.getText();
        }
        
        
        
        
        public void setHGIsotopes(Parser.TreeNode node)
        {
            heavyElementCountList[0] = heavyElementCounts;
            heavyName += "HG" + node.getText();
        }
        
        
        
        
        public void addIsotopeCount(Parser.TreeNode node)
        {
            heavyCount = Convert.ToInt32(node.getText());
        }
        
        
        
        
        public void addHeavyPrecursorElement(Parser.TreeNode node)
        {
            if (heavyCount < 1)
            {
                lipid = null;
                return;
            }
            
            string key = heavyIsotope.ToString() + heavyElement;
            if (MS2Fragment.ELEMENT_POSITIONS.ContainsKey(key))
            {
                Molecule m = MS2Fragment.ELEMENT_POSITIONS[key];
                if (heavyElementCounts.ContainsKey(m))
                {
                    heavyElementCounts[m] = heavyCount;
                }
                else
                {
                    heavyElementCounts.Add(m, heavyCount);
                }
            }
            else
            {
                lipid = null;
            }            
        }
        
        
        
        // handling all events
        public void lipidPostEvent(Parser.TreeNode node)
        {
        
            if (lipid != null && lipid.headGroupNames.Count > 0 && lipidCreator.headgroups.ContainsKey(lipid.headGroupNames[0]))
            {
            
                foreach (string adduct in Lipid.ADDUCT_POSITIONS.Keys) lipid.adducts[adduct] = false;
            
                
                if (charge != 0)
                {
                    if (Lipid.ADDUCT_POSITIONS.ContainsKey(adduct) && Lipid.ALL_ADDUCTS[Lipid.ADDUCT_POSITIONS[adduct]].charge == charge && lipidCreator.headgroups[lipid.headGroupNames[0]].adductRestrictions[adduct])
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
                    lipid.adducts[lipidCreator.headgroups[lipid.headGroupNames[0]].defaultAdduct] = true;
                }
            }
            
            if (lipid != null && expectsEther && ethers != 1)
            {
                lipid = null;
            }
            
            if (lipid == null && makeUnsupported)
            {
                lipid = new UnsupportedLipid(lipidCreator);
            }
            
            
            // flip fatty acids on phospholipids if necessary
            if ((lipid != null) && (lipid is Phospholipid))
            {
                bool firstFAHasPlamalogen = false;
                bool secondFAHasPlamalogen = false;
                foreach (KeyValuePair<string, bool> kvp in ((Phospholipid)lipid).fag1.faTypes)
                {
                    firstFAHasPlamalogen |= ((kvp.Key.Equals("FAa") && kvp.Value) || (kvp.Key.Equals("FAp") && kvp.Value));
                }
                foreach (KeyValuePair<string, bool> kvp in ((Phospholipid)lipid).fag2.faTypes)
                {
                    secondFAHasPlamalogen |= ((kvp.Key.Equals("FAa") && kvp.Value) || (kvp.Key.Equals("FAp") && kvp.Value));
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
                    lipid = new UnsupportedLipid(lipidCreator);
                }
            }
            
            
            ElementDictionary heavyFragment = null;
            string heavyPosFragmentKey = "";
            string heavyNegFragmentKey = "";
            
            // add fragment
            if (lipid != null && fragmentName.Length > 0)
            {
            
                string lipidClass = lipid.headGroupNames[0];
                FattyAcidGroupEnumerator fragmentFAGEnum = null;
                
                
                // check if fragment name contains heavy isotope label
                ip.parseSubstring(fragmentName);
                if (ip.wordInGrammar)
                {
                    ip.raiseEvents();
                    if (ipeh.heavyElementCounts != null)
                    {
                        heavyFragment = ipeh.heavyElementCounts;
                        fragmentName = fragmentName.Replace(ipeh.heavyIsotopeLabel, "");
                    }
                    else
                    {
                        lipid = null;
                    }
                }
                
                
                
                if (lipid != null)
                {
                    // check for PE O, PC O, LPE O, LPC O
                    if (lipidClass.Equals("PC") || lipidClass.Equals("PE"))
                    {
                        if (((Phospholipid)lipid).fag1.faTypes["FAp"] || ((Phospholipid)lipid).fag2.faTypes["FAp"]) lipidClass = lipidClass + " O-p";
                        else if (((Phospholipid)lipid).fag1.faTypes["FAa"] || ((Phospholipid)lipid).fag2.faTypes["FAa"]) lipidClass = lipidClass + " O-a";
                    }
                    else if  (lipidClass.Equals("LPC") || lipidClass.Equals("LPE"))
                    {
                        if (((Phospholipid)lipid).fag1.faTypes["FAp"]) lipidClass = lipidClass + " O-p";
                        else if (((Phospholipid)lipid).fag1.faTypes["FAa"]) lipidClass = lipidClass + " O-a";
                    }
                    
                    
                    ArrayList possibleFragmentNames = new ArrayList();
                    possibleFragmentNames.Add(new string[]{fragmentName, fragmentName});
                    string[] faWildcards = new string[]{"[xx:x]", "[yy:y]", "[zz:z]", "[uu:u]"};
                    
                    
                    for (int j = 0; j < possibleFragmentNames.Count; ++j)
                    {
                        
                        if (lipid is Glycerolipid) fragmentFAGEnum = new FattyAcidGroupEnumerator((Glycerolipid)lipid);
                        else if (lipid is Phospholipid) fragmentFAGEnum = new FattyAcidGroupEnumerator((Phospholipid)lipid);
                        else if (lipid is Sphingolipid) fragmentFAGEnum = new FattyAcidGroupEnumerator((Sphingolipid)lipid);
                        else if (lipid is Cholesterol) fragmentFAGEnum = new FattyAcidGroupEnumerator((Cholesterol)lipid);
                        else if (lipid is Mediator) fragmentFAGEnum = new FattyAcidGroupEnumerator((Mediator)lipid);
                        int faCNT = 0;
                    
                        string currentFragmentName = ((string[])possibleFragmentNames[j])[0];
                        string currentOutputFragmentName = ((string[])possibleFragmentNames[j])[1];
                        for (int i = 0; i < Precursor.fattyAcidCount[lipidCreator.headgroups[lipidClass].buildingBlockType]; ++i)
                        {
                            if (i == 0 && lipid is Sphingolipid)
                            {
                                string lcb_key = ((Sphingolipid)lipid).lcb.getFattyAcids().First().ToString();
                                if (currentFragmentName.IndexOf(lcb_key) >= 0)
                                {
                                    possibleFragmentNames.Add(new string[]{currentFragmentName.Replace(" " + lcb_key, ""), currentOutputFragmentName.Replace(lcb_key, "[xx:x;x]")});
                                }
                            }
                            else
                            {
                                fragmentFAGEnum.MoveNext();
                                string fa_key = (fragmentFAGEnum.Current).getFattyAcids().First().ToString();
                                if (currentFragmentName.IndexOf(fa_key) >= 0)
                                {
                                    possibleFragmentNames.Add(new string[]{currentFragmentName.Replace(" " + fa_key, (faCNT + 1).ToString()), currentOutputFragmentName.Replace(fa_key, faWildcards[faCNT])});
                                }
                                ++faCNT;
                            }
                        }
                        
                        foreach (Adduct adduct in Lipid.ALL_ADDUCTS.Values)
                        {
                            if (currentFragmentName.IndexOf(adduct.name) >= 0)
                            {
                                possibleFragmentNames.Add(new string[]{currentFragmentName.Replace(adduct.name, "[adduct]"), currentOutputFragmentName.Replace(adduct.name, "[adduct]")});
                            }
                        }
                    }
                    
                    
                    
                    ArrayList foundPosFragmentNames = new ArrayList();
                    ArrayList foundNegFragmentNames = new ArrayList();
                    foreach (string[] fragment in possibleFragmentNames)
                    {
                    
                        if (lipidCreator.allFragments[lipidClass][true].ContainsKey(fragment[0]) && lipidCreator.allFragments[lipidClass][true][fragment[0]].fragmentOutputName.Equals(fragment[1]))
                        {
                            foundPosFragmentNames.Add(fragment[0]);
                        }
                        if (lipidCreator.allFragments[lipidClass][false].ContainsKey(fragment[0]) && lipidCreator.allFragments[lipidClass][false][fragment[0]].fragmentOutputName.Equals(fragment[1]))
                        {
                            foundNegFragmentNames.Add(fragment[0]);
                        }
                    }
                    
                    if (foundNegFragmentNames.Count == 0 && foundPosFragmentNames.Count == 0)
                    {
                        lipid = null;
                    }
                    else
                    {
                        foreach (HashSet<string> sh in lipid.positiveFragments.Values) sh.Clear();
                        foreach (HashSet<string> sh in lipid.negativeFragments.Values) sh.Clear();
                        foreach (string posFragment in foundPosFragmentNames)
                        {
                            lipid.positiveFragments[lipidClass].Add(posFragment);
                            heavyPosFragmentKey = posFragment;
                        }
                        foreach (string negFragment in foundNegFragmentNames)
                        {
                            lipid.negativeFragments[lipidClass].Add(negFragment);
                            heavyNegFragmentKey = negFragment;
                        }
                    }
                }
            }
            
            
            if (lipid != null && !makeUnsupported && heavyFragment != null && !addHeavyPrecursor)
            {
                lipid = null;
            }
            
            if (lipid != null)
            {
                lipid.onlyHeavyLabeled = 0;
            }
            
            
            
            
            
            // adding heavy labeled isotopes if present
            if (lipid != null && !makeUnsupported && addHeavyPrecursor)
            {
                ElementDictionary hgDictionary = new ElementDictionary(lipidCreator.headgroups[lipid.headGroupNames[0]].elements);
                
                MS2Fragment.updateForHeavyLabeled(hgDictionary, (ElementDictionary)heavyElementCountList[0]);
                if (!MS2Fragment.validElementDict(hgDictionary)) lipid = null;
                
                
                
            
                if (lipid != null)
                {
                    
                    string lipidClass = lipid.headGroupNames[0];
                    heavyElementCountList[0] = hgDictionary;
                    lipid.onlyHeavyLabeled = 1;
                    lipidCreator.addHeavyPrecursor(lipid.headGroupNames[0], heavyName, heavyElementCountList);
                    
                    if (heavyFragment != null)
                    {
                        string fullHeavyName = lipidClass + LipidCreator.HEAVY_LABEL_OPENING_BRACKET + heavyName + LipidCreator.HEAVY_LABEL_CLOSING_BRACKET;
                        if (lipidCreator.allFragments[fullHeavyName][true].ContainsKey(heavyPosFragmentKey))
                        {
                            MS2Fragment.updateForHeavyLabeled(lipidCreator.allFragments[fullHeavyName][true][heavyPosFragmentKey].fragmentElements, heavyFragment);
                        }
                        
                        if  (lipidCreator.allFragments[fullHeavyName][false].ContainsKey(heavyNegFragmentKey))
                        {
                            MS2Fragment.updateForHeavyLabeled(lipidCreator.allFragments[fullHeavyName][false][heavyNegFragmentKey].fragmentElements, heavyFragment);
                        }
                        
                        lipid.positiveFragments[fullHeavyName].Clear();
                        lipid.negativeFragments[fullHeavyName].Clear();
                        
                        if (heavyPosFragmentKey.Length > 0) lipid.positiveFragments[fullHeavyName].Add(heavyPosFragmentKey);
                        if (heavyNegFragmentKey.Length > 0) lipid.negativeFragments[fullHeavyName].Add(heavyNegFragmentKey);
                        
                        
                        ElementDictionary totalHeavy = MS2Fragment.createEmptyElementDict();
                        foreach (ElementDictionary ed in heavyElementCountList)
                        {
                            MS2Fragment.addCounts(totalHeavy, ed);
                        }
                        MS2Fragment.addCounts(totalHeavy, heavyFragment, true);
                        
                        if (!MS2Fragment.validElementDict(totalHeavy))
                        {
                            lipid = null;
                        }
                    }
                }
                
            }
            
            Console.WriteLine("hg: " + lipid.headGroupNames[0]);
        }
        
        
        
        public void unsupportedEvent(Parser.TreeNode node)
        {
            lipid = null;
            makeUnsupported = true;
        }
        
        
        
        
        public void sortedFASeparatorPreEvent(Parser.TreeNode node)
        {
            sortedSeparator = true;
        }
        
        
        
        
        public void GLPreEvent(Parser.TreeNode node)
        {
            lipid = new Glycerolipid(lipidCreator);
            fagEnum = new FattyAcidGroupEnumerator((Glycerolipid)lipid);
        }
        
        
        
        
        public void PLPreEvent(Parser.TreeNode node)
        {
            lipid = new Phospholipid(lipidCreator);
            fagEnum = new FattyAcidGroupEnumerator((Phospholipid)lipid);
        }
        
        
        
        
        public void DPLPostEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                if (lipid.headGroupNames.Count != 0)
                {
                    Phospholipid gpl = (Phospholipid)lipid;
                    if (!sortedSeparator && (gpl.fag2.faTypes["FAp"] || gpl.fag2.faTypes["FAa"]))
                    {
                        FattyAcidGroup swapFAG = gpl.fag1;
                        gpl.fag1 = gpl.fag2;
                        gpl.fag2 = swapFAG;
                    }
                    if (gpl.fag2.faTypes["FAp"] || gpl.fag2.faTypes["FAa"])
                    {
                        lipid = null;
                        makeUnsupported = true;
                    }
                }
                else
                {
                    lipid = null;
                }
            }
        }
        
        
        
        
        public void SLPreEvent(Parser.TreeNode node)
        {
            lipid = new Sphingolipid(lipidCreator);
            ((Sphingolipid)lipid).lcb.hydroxylCounts.Clear();
            ((Sphingolipid)lipid).fag.hydroxylCounts.Clear();
            fagEnum = new FattyAcidGroupEnumerator((Sphingolipid)lipid);
        }
        
        
        
        
        public void CholesterolPreEvent(Parser.TreeNode node)
        {
            lipid = new Cholesterol(lipidCreator);
            fagEnum = new FattyAcidGroupEnumerator((Cholesterol)lipid);
        }
        
        
        
        
        public void MediatorPreEvent(Parser.TreeNode node)
        {
            lipid = new Mediator(lipidCreator);
            string headgroup = node.getText();
            lipid.headGroupNames.Add(headgroup);
        }
        
        
        
        
        public void LCBPreEvent(Parser.TreeNode node)
        {
            fag = ((Sphingolipid)lipid).lcb;
            heavyElementCounts = null;
        }
        
        
        
        
        
        public void LCBPostEvent(Parser.TreeNode node)
        {
            FALCBvalidationCheck();
            if (heavyElementCounts != null)
            {
                heavyElementCountList.Add(heavyElementCounts);
            }
            else
            {
                heavyElementCountList.Add(MS2Fragment.createEmptyElementDict());
            }
        }
        
        
        
        
        public void FAPreEvent(Parser.TreeNode node)
        {
            fag = (fagEnum != null && fagEnum.MoveNext()) ? fagEnum.Current : null;
            heavyElementCounts = null;
        }
        
        
        
        
        public void FAPostEvent(Parser.TreeNode node)
        {
            FALCBvalidationCheck();
            if (heavyElementCounts != null)
            {
                heavyElementCountList.Add(heavyElementCounts);
            }
            else
            {
                heavyElementCountList.Add(MS2Fragment.createEmptyElementDict());
            }
        }
        
        
        
        
        public void FALCBvalidationCheck()
        {
            // check if created fatty acid is valid
            if (fag != null)
            {
                if (fag.hydroxylCounts.Count == 0)
                {
                    fag.hydroxylCounts.Add(0);
                }
            
                if (fag.carbonCounts.Count == 0 || fag.doubleBondCounts.Count == 0)
                {
                    lipid = null;
                }
                else if (fag.carbonCounts.Count == 1 && fag.doubleBondCounts.Count == 1)
                {
                    int carbonLength = (new List<int>(fag.carbonCounts))[0];
                    int doubleBondCount = (new List<int>(fag.doubleBondCounts))[0];
                    
                    int maxDoubleBond = (carbonLength - 1) >> 1;
                    if (doubleBondCount > maxDoubleBond) lipid = null;
                    else if (fag.hydroxylCounts.Count == 1)
                    {
                        int hydroxylCount = (new List<int>(fag.hydroxylCounts))[0];
                        if (carbonLength < hydroxylCount) lipid = null;
                    }
                }
                else 
                {
                    lipid = null;
                }
                
                // check if at least one fatty acid type is enabled
                int enablesFATypes = 0;
                foreach(KeyValuePair<string, bool> kvp in fag.faTypes) enablesFATypes += kvp.Value ? 1 : 0;                
                if (enablesFATypes == 0) lipid = null;
            }
            else 
            {
                lipid = null;
            }
            
            if (lipid != null && fag != null)
            {
                // fatty acids with plasmalogens must not contain zero double bonds
                // it's not a bug, it's a feature ;-)
                if (fag.faTypes["FAp"] && (new List<int>(fag.doubleBondCounts))[0] == 0)
                {
                    lipid = null;
                    makeUnsupported = true;
                }
            }
            
            if (lipid != null && fag != null)
            {
                foreach(int l in fag.carbonCounts) fag.lengthInfo = Convert.ToString(l);
                foreach(int db in fag.doubleBondCounts) fag.dbInfo = Convert.ToString(db);
                foreach(int h in fag.hydroxylCounts) fag.hydroxylInfo = Convert.ToString(h);
            }
        }
        
        public void CarbonPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string carbonCount = node.getText();
                int carbonCountInt = Convert.ToInt32(carbonCount);
                if (LipidCreator.MIN_CARBON_LENGTH <= carbonCountInt && carbonCountInt <= LipidCreator.MAX_CARBON_LENGTH) fag.carbonCounts.Add(carbonCountInt);
                else fag = null;
            }
        }
        
        public void DB_countPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string doubleBondCount = node.getText();
                int doubleBondCountInt = Convert.ToInt32(doubleBondCount);
                if (LipidCreator.MIN_DB_LENGTH <= doubleBondCountInt && doubleBondCountInt <= LipidCreator.MAX_DB_LENGTH) fag.doubleBondCounts.Add(doubleBondCountInt);
                else fag = null;
            }
        }
        
        public void OldHydroxylPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string hydroxylCount = node.getText();
                if (hydroxylCount == "d") fag.hydroxylCounts.Add(2);
                else if (hydroxylCount == "t") fag.hydroxylCounts.Add(3);
                else fag = null;
            }
        }
        
        public void HydroxylPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string hydroxylCount = node.getText();
                int hydroxylCountInt = Convert.ToInt32(hydroxylCount);
                if (fag.isLCB && LipidCreator.MIN_LCB_HYDROXY_LENGTH <= hydroxylCountInt && hydroxylCountInt <= LipidCreator.MAX_LCB_HYDROXY_LENGTH) fag.hydroxylCounts.Add(hydroxylCountInt);
                else if ((lipid is Sphingolipid) && !fag.isLCB && LipidCreator.MIN_SPHINGO_FA_HYDROXY_LENGTH <= hydroxylCountInt && hydroxylCountInt <= LipidCreator.MAX_SPHINGO_FA_HYDROXY_LENGTH) fag.hydroxylCounts.Add(hydroxylCountInt);
                else if (!(lipid is Sphingolipid) && LipidCreator.MIN_HYDROXY_LENGTH <= hydroxylCountInt && hydroxylCountInt <= LipidCreator.MAX_HYDROXY_LENGTH) fag.hydroxylCounts.Add(hydroxylCountInt);
                else fag = null;
            }
        }
        
        public void EtherPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                List<string> keys = new List<string>(fag.faTypes.Keys);
                foreach(string faTypeKey in keys) fag.faTypes[faTypeKey] = false;
            
                string faType = node.getText();
                fag.faTypes["FA" + faType] = true;
                ++ethers;
                /*
                if ((new HashSet<string>{"LPC O-", "LPE O-", "PC O-", "PE O-"}).Contains(lipid.headGroupNames[0]))
                {
                    lipid.headGroupNames[0] += faType;
                }
                */
            }
        }
        
        public void HG_MGLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                List<string> keys = new List<string>(((Glycerolipid)lipid).fag2.faTypes.Keys);
                foreach(string faTypeKey in keys) ((Glycerolipid)lipid).fag2.faTypes[faTypeKey] = false;
                ((Glycerolipid)lipid).fag2.faTypes["FAx"] = true;
                keys = new List<string>(((Glycerolipid)lipid).fag3.faTypes.Keys);
                foreach(string faTypeKey in keys) ((Glycerolipid)lipid).fag3.faTypes[faTypeKey] = false;
                ((Glycerolipid)lipid).fag3.faTypes["FAx"] = true;
            }
        }
        
        public void HG_DGLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                List<string> keys = new List<string>(((Glycerolipid)lipid).fag3.faTypes.Keys);
                foreach(string faTypeKey in keys) ((Glycerolipid)lipid).fag3.faTypes[faTypeKey] = false;
                ((Glycerolipid)lipid).fag3.faTypes["FAx"] = true;
            }
        }
        
        public void HG_SGLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                List<string> keys = new List<string>(((Glycerolipid)lipid).fag3.faTypes.Keys);
                foreach(string faTypeKey in keys) ((Glycerolipid)lipid).fag3.faTypes[faTypeKey] = false;
                ((Glycerolipid)lipid).fag3.faTypes["FAx"] = true;
                ((Glycerolipid)lipid).containsSugar = true;
            }
        }
        
        public void HG_TGLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
            }
        }
        
        public void HG_CLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                ((Phospholipid)lipid).isCL = true;
            }
        }
        
        public void HG_MLCLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                ((Phospholipid)lipid).isCL = true;
                List<string> keys = new List<string>(((Phospholipid)lipid).fag4.faTypes.Keys);
                foreach(string faTypeKey in keys) ((Phospholipid)lipid).fag4.faTypes[faTypeKey] = false;
                ((Phospholipid)lipid).fag4.faTypes["FAx"] = true;
            }
        }
        
        public void HG_PLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
            }
        }
        
        public void HG_LPLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                ((Phospholipid)lipid).isLyso = true;
            }
        }
        
        public void HG_PL_OPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                if (headgroup == "PE O")
                {
                    headgroup = "PE";
                    expectsEther = true;
                }
                else if (headgroup == "PC O")
                {
                    headgroup = "PC";
                    expectsEther = true;
                }
                lipid.headGroupNames.Add(headgroup);
            }
        }
        
        public void HG_LPL_OPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                if (headgroup == "LPE O")
                {
                    headgroup = "LPE";
                    expectsEther = true;
                }
                else if (headgroup == "LPC O")
                {
                    headgroup = "LPC";
                    expectsEther = true;
                }
                lipid.headGroupNames.Add(headgroup);
                ((Phospholipid)lipid).isLyso = true;
            }
        }
        
        public void HG_LSLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                ((Sphingolipid)lipid).isLyso = true;
            }
        }
        
        public void HG_DSLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                if (headgroup == "GB3") headgroup = "Hex3Cer";
                lipid.headGroupNames.Add(headgroup);
            }
        }
        
        public void ChPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                List<string> keys = new List<string>(((Cholesterol)lipid).fag.faTypes.Keys);
                foreach(string faTypeKey in keys) ((Cholesterol)lipid).fag.faTypes[faTypeKey] = false;
                ((Cholesterol)lipid).fag.faTypes["FAx"] = true;
            }
        }
        
        
        
        public void HG_ChEPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                ((Cholesterol)lipid).containsEster = true;
            }
        }
        
        
        
        
            
        public void SLPostEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                if (lipid.headGroupNames.Count == 0)
                {
                    lipid = null;
                }
            }
        }
            
        public void MediatorPostEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                if (lipid.headGroupNames.Count == 0)
                {
                    lipid = null;
                }
            }
        }
        
        public void adductPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                adduct = node.getText();
            
            }
        }
        
        public void chargePreEvent(Parser.TreeNode node)
        {
            charge = Convert.ToInt32(node.getText());
        }
        
        public void charge_signPreEvent(Parser.TreeNode node)
        {
            charge *= node.getText() == "-" ? -1 : 1;
        }
    }    
}