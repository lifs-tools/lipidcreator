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
    
    
        public ParserEventHandler(LipidCreator _lipidCreator) : base()
        {
            lipidCreator = _lipidCreator;
            resetLipidBuilder(null);
            
            registeredEvents.Add("lipid_pre_event", resetLipidBuilder);
            registeredEvents.Add("lipid_post_event", lipidPostEvent);
            
            registeredEvents.Add("FA_pre_event", FAPreEvent);
            registeredEvents.Add("FA_post_event", FAPostEvent);
            
            registeredEvents.Add("LCB_pre_event", LCBPreEvent);
            registeredEvents.Add("LCB_post_event", LCBPostEvent);
            registeredEvents.Add("Carbon_pre_event", CarbonPreEvent);
            registeredEvents.Add("DB_count_pre_event", DB_countPreEvent);
            registeredEvents.Add("Hydroxyl_pre_event", HydroxylPreEvent);
            registeredEvents.Add("Ether_pre_event", EtherPreEvent);
            
            registeredEvents.Add("GL_pre_event", GLPreEvent);
            registeredEvents.Add("PL_pre_event", PLPreEvent);
            registeredEvents.Add("PL-O_post_event", PL_OPostEvent);
            registeredEvents.Add("SL_pre_event", SLPreEvent);
            registeredEvents.Add("Cholesterol_pre_event", CholesterolPreEvent);
            registeredEvents.Add("Mediator_pre_event", MediatorPreEvent);
            
            registeredEvents.Add("HG_MGL_pre_event", HG_MGLPreEvent);
            registeredEvents.Add("HG_DGL_pre_event", HG_DGLPreEvent);
            registeredEvents.Add("HG_SGL_pre_event", HG_SGLPreEvent);
            registeredEvents.Add("HG_TGL_pre_event", HG_TGLPreEvent);
            
            registeredEvents.Add("HG_CL_pre_event", HG_CLPreEvent);
            registeredEvents.Add("HG_MLCL_pre_event", HG_MLCLPreEvent);
            registeredEvents.Add("HG_PL_pre_event", HG_PLPreEvent);
            registeredEvents.Add("HG_LPL_pre_event", HG_LPLPreEvent);
            registeredEvents.Add("HG_LPL-O_pre_event", HG_LPL_OPreEvent);
            registeredEvents.Add("HG_PL-O_pre_event", HG_PL_OPreEvent);
            
            registeredEvents.Add("HG_LSL_pre_event", HG_LSLPreEvent);
            registeredEvents.Add("HG_DSL_pre_event", HG_DSLPreEvent);
            
            registeredEvents.Add("Ch_pre_event", ChPreEvent);
            registeredEvents.Add("HG_ChE_pre_event", HG_ChEPreEvent);
            
            registeredEvents.Add("PL_post_event", PLPostEvent);
            registeredEvents.Add("SL_post_event", SLPostEvent);
            registeredEvents.Add("Mediator_post_event", MediatorPostEvent);
            
            registeredEvents.Add("adduct_pre_event", adductPreEvent);
            registeredEvents.Add("charge_pre_event", chargePreEvent);
            registeredEvents.Add("charge_sign_pre_event", charge_signPreEvent);
        }
        
        
        
        public void resetLipidBuilder(Parser.TreeNode node)
        {
            lipid = null;
            fagEnum = null;
            fag = null;
            charge = 0;
            adduct = "";
        }
        
        
        
        // handling all events
        public void lipidPostEvent(Parser.TreeNode node)
        {
            if (lipid != null && lipid.headGroupNames.Count > 0 && lipidCreator.headgroups.ContainsKey(lipid.headGroupNames[0]))
            {
            
                lipid.adducts["+H"] = false;
                lipid.adducts["+2H"] = false;
                lipid.adducts["+NH4"] = false;
                lipid.adducts["-H"] = false;
                lipid.adducts["-2H"] = false;
                lipid.adducts["+HCOO"] = false;
                lipid.adducts["+CH3COO"] = false;
                
                if (charge != 0)
                {
                    if (lipid.adducts.ContainsKey(adduct) && Lipid.adductToCharge[adduct] == charge && lipidCreator.headgroups[lipid.headGroupNames[0]].adductRestrictions[adduct])
                    {
                        lipid.adducts[adduct] = true;
                    }
                    else
                    {
                        lipid.adducts[lipidCreator.headgroups[lipid.headGroupNames[0]].defaultAdduct] = true;
                    }
                }
                else
                {
                    lipid.adducts[lipidCreator.headgroups[lipid.headGroupNames[0]].defaultAdduct] = true;
                }
                
            }
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
        
        public void PL_OPostEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                if (lipid.headGroupNames.Count != 0)
                {
                    string hg = lipid.headGroupNames[0];
                    if (hg[hg.Length - 1] == '-')
                    {
                        lipid = null;
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
                if (2 <= carbonCountInt && carbonCountInt <= 30) fag.carbonCounts.Add(carbonCountInt);
                else fag = null;
            }
        }
        
        public void DB_countPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string doubleBondCount = node.getText();
                int doubleBondCountInt = Convert.ToInt32(doubleBondCount);
                if (0 <= doubleBondCountInt && doubleBondCountInt <= 6) fag.doubleBondCounts.Add(doubleBondCountInt);
                else fag = null;
            }
        }
        
        public void HydroxylPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string hydroxylCount = node.getText();
                int hydroxylCountInt = Convert.ToInt32(hydroxylCount);
                if (fag.isLCB && 2 <= hydroxylCountInt && hydroxylCountInt <= 3) fag.hydroxylCounts.Add(hydroxylCountInt);
                else if ((lipid is Sphingolipid) && !fag.isLCB && 0 <= hydroxylCountInt && hydroxylCountInt <= 3) fag.hydroxylCounts.Add(hydroxylCountInt);
                else if (!(lipid is Sphingolipid) && 0 <= hydroxylCountInt && hydroxylCountInt <= 6) fag.hydroxylCounts.Add(hydroxylCountInt);
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
                if ((new HashSet<string>{"LPC O-", "LPE O-", "PC O-", "PE O-"}).Contains(lipid.headGroupNames[0]))
                {
                    lipid.headGroupNames[0] += faType;
                }
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
                lipid.headGroupNames.Add(headgroup + "-");
            }
        }
        
        public void HG_LPL_OPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup + "-");
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
        
        
            
        public void PLPostEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                if (lipid.headGroupNames.Count == 0)
                {
                    lipid = null;
                }
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