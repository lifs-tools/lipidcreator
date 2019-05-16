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
    
    
        public ParserEventHandler(LipidCreator _lipidCreator) : base()
        {
            lipidCreator = _lipidCreator;
            resetLipidBuilder(null);
            
            registeredEvents.Add("lipid_pre_event", resetLipidBuilder);
            registeredEvents.Add("lipid_post_event", lipidPostEvent);
            
            registeredEvents.Add("fa_pre_event", FAPreEvent);
            registeredEvents.Add("fa_post_event", FAPostEvent);
            
            registeredEvents.Add("lcb_pre_event", LCBPreEvent);
            registeredEvents.Add("lcb_post_event", LCBPostEvent);
            registeredEvents.Add("carbon_pre_event", CarbonPreEvent);
            registeredEvents.Add("db_count_pre_event", DB_countPreEvent);
            registeredEvents.Add("hydroxyl_pre_event", HydroxylPreEvent);
            registeredEvents.Add("old_Hydroxyl_pre_event", OldHydroxylPreEvent);
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
            
            registeredEvents.Add("heavy_pre_event", unsupportedEvent);
            registeredEvents.Add("gl_species_pre_event", unsupportedEvent);
            registeredEvents.Add("pl_species_pre_event", unsupportedEvent);
            registeredEvents.Add("sl_species_pre_event", unsupportedEvent);
            
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
        }
        
        
        public void LCBPostEvent(Parser.TreeNode node)
        {
            FALCBvalidationCheck();
        }
        
        public void FAPreEvent(Parser.TreeNode node)
        {
            fag = (fagEnum != null && fagEnum.MoveNext()) ? fagEnum.Current : null;
        }
        
        public void FAPostEvent(Parser.TreeNode node)
        {
            FALCBvalidationCheck();
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
                //lipid.headGroupNames.Add(headgroup + "-");
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
                //lipid.headGroupNames.Add(headgroup + "-");
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