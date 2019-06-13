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
    public class LipidMapsParserEventHandler : BaseParserEventHandler
    {
        public LipidCreator lipidCreator;
        public Lipid lipid;
        public FattyAcidGroupEnumerator fagEnum;
        public FattyAcidGroup fag;
        public string mediatorName;
        public bool makeUnsupported = false;
    
    
        public LipidMapsParserEventHandler(LipidCreator _lipidCreator) : base()
        {
            lipidCreator = _lipidCreator;
            resetLipidBuilder(null);
            
            registeredEvents.Add("lipid_pre_event", resetLipidBuilder);
            registeredEvents.Add("lipid_post_event", lipidPostEvent);
            
            registeredEvents.Add("pure_fa_pre_event", PureFAPreEvent);
            
            registeredEvents.Add("fa_pre_event", FAPreEvent);
            registeredEvents.Add("fa_post_event", FAPostEvent);
            registeredEvents.Add("lcb_pre_event", LCBPreEvent);
            registeredEvents.Add("lcb_post_event", LCBPostEvent);
            
            registeredEvents.Add("carbon_pre_event", CarbonPreEvent);
            registeredEvents.Add("db_count_pre_event", DB_countPreEvent);
            registeredEvents.Add("hydroxyl_pre_event", HydroxylPreEvent);
            registeredEvents.Add("hydroxyl_lcb_pre_event", HydroxylLCBPreEvent);
            registeredEvents.Add("ether_pre_event", EtherPreEvent);
            registeredEvents.Add("mod_text_pre_event", mod_textPreEvent);
            
            registeredEvents.Add("gl_pre_event", GLPreEvent);
            registeredEvents.Add("pl_pre_event", PLPreEvent);
            registeredEvents.Add("pl_post_event", PLPostEvent);
            registeredEvents.Add("sl_pre_event", SLPreEvent);
            registeredEvents.Add("sl_post_event", SLPostEvent);
            registeredEvents.Add("cholesterol_pre_event", CholesterolPreEvent);
            registeredEvents.Add("mediator_pre_event", MediatorPreEvent);
            
            registeredEvents.Add("hg_sgl_pre_event", HG_SGLPreEvent);
            registeredEvents.Add("hg_gl_pre_event", HG_GLPreEvent);
            
            registeredEvents.Add("hg_cl_pre_event", HG_CLPreEvent);
            registeredEvents.Add("hg_dpl_pre_event", HG_DPLPreEvent);
            registeredEvents.Add("hg_lpl_pre_event", HG_LPLPreEvent);
            registeredEvents.Add("hg_fourpl_pre_event", HG_4PLPreEvent);
            
            registeredEvents.Add("hg_dsl_pre_event", HG_DSLPreEvent);
            registeredEvents.Add("hg_lsl_pre_event", HG_LSLPreEvent);
            registeredEvents.Add("sphingoxine_pre_event", SphingoXinePreEvent);
            registeredEvents.Add("sphingoxine_post_event", SphingoXinePostEvent);
            registeredEvents.Add("sphingoxine_pure_pre_event", SphingoXine_purePreEvent);
            registeredEvents.Add("sphingosine_name_pre_event", Sphingosine_namePreEvent);
            registeredEvents.Add("sphinganine_name_pre_event", Sphinganine_namePreEvent);
            registeredEvents.Add("ctype_pre_event", CTypePreEvent);
            
            registeredEvents.Add("ch_pre_event", ChPreEvent);
            registeredEvents.Add("hg_che_pre_event", HG_ChEPreEvent);
            
            registeredEvents.Add("mediator_post_event", MediatorPostEvent);
            registeredEvents.Add("mediator_number_pure_pre_event", MediatorAssemble);
            registeredEvents.Add("mediator_oxo_pre_event", Mediator_OxoPreEvent);
            registeredEvents.Add("mediator_name_separator_pre_event", MediatorAssemble);
            registeredEvents.Add("mediator_separator_pre_event", MediatorAssemble);
            registeredEvents.Add("mediator_var_name_pre_event", MediatorAssemble);
            registeredEvents.Add("mediator_const_pre_event", MediatorAssemble);
            
            
            registeredEvents.Add("dpl_species_pre_event", Unsupported);
            registeredEvents.Add("cl_species_pre_event", Unsupported);
            registeredEvents.Add("tgl_species_pre_event", Unsupported);
            registeredEvents.Add("sgl_species_pre_event", Unsupported);
            registeredEvents.Add("dsl_species_pre_event", Unsupported);
            
            
        }
        
        
        public void resetLipidBuilder(Parser.TreeNode node)
        {
            lipid = null;
            fagEnum = null;
            fag = null;
            mediatorName = "";
            makeUnsupported = false;
        }
        
        
        public void Unsupported(Parser.TreeNode node)
        {
            lipid = null;
            makeUnsupported = true;
        }
        
        
        
        public void PureFAPreEvent(Parser.TreeNode node)
        {
            lipid = new UnsupportedLipid(lipidCreator);
        }
        
        
        
        
        public void MediatorAssemble(Parser.TreeNode node)
        {
            mediatorName += node.getText();
        }
        
        
        
        // handling all events
        public void lipidPostEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid) && lipid.headGroupNames.Count > 0 && lipidCreator.headgroups.ContainsKey(lipid.headGroupNames[0]))
            {
                lipid.adducts["+H"] = false;
                lipid.adducts["+2H"] = false;
                lipid.adducts["+NH4"] = false;
                lipid.adducts["-H"] = false;
                lipid.adducts["-2H"] = false;
                lipid.adducts["+HCOO"] = false;
                lipid.adducts["+CH3COO"] = false;
                
                lipid.adducts[lipidCreator.headgroups[lipid.headGroupNames[0]].defaultAdduct] = true;
            }
            
            if (lipid == null && makeUnsupported)
            {
                lipid = new UnsupportedLipid(lipidCreator);
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
        
        
        
        public void PLPostEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                if (lipid.headGroupNames.Count != 0)
                {
                    int FAset = 0;
                    if (!((Phospholipid)lipid).fag1.faTypes["FAx"]) FAset += 1;
                    if (!((Phospholipid)lipid).fag2.faTypes["FAx"]) FAset += 1;
                    if (!((Phospholipid)lipid).fag3.faTypes["FAx"]) FAset += 1;
                    if (!((Phospholipid)lipid).fag4.faTypes["FAx"]) FAset += 1;
                
                    if (((Phospholipid)lipid).fag1.faTypes["FAx"])
                    {
                        FattyAcidGroup fag = ((Phospholipid)lipid).fag1;
                        ((Phospholipid)lipid).fag1 = ((Phospholipid)lipid).fag2;
                        ((Phospholipid)lipid).fag2 = fag;
                    }
                    
                    if (((Phospholipid)lipid).isCL)
                    {
                        if (FAset < 3)
                        {
                            lipid = null;
                            return;
                        }
                        
                    }
                    else if (((Phospholipid)lipid).isLyso)
                    {
                        if (FAset < 1)
                        {
                            lipid = null;
                            return;
                        }
                    
                    }
                    else
                    {
                        if (FAset < 2)
                        {
                            lipid = null;
                            return;
                        }
                    }
                
                    string hg = lipid.headGroupNames[0];
                    if (((Phospholipid)lipid).fag2.faTypes["FAx"] && (new HashSet<string>{"PA", "PC", "PE", "PI", "PS"}).Contains(hg))
                    {
                        lipid.headGroupNames[0] = "L" + lipid.headGroupNames[0];
                    }
                    
                    if ((new HashSet<string>{"LPC", "PC", "LPE", "PE"}).Contains(hg))
                    {
                        if (((Phospholipid)lipid).fag1.faTypes["FAa"] || ((Phospholipid)lipid).fag2.faTypes["FAa"]) lipid.headGroupNames[0] += " O-a";
                        else if (((Phospholipid)lipid).fag1.faTypes["FAp"] || ((Phospholipid)lipid).fag2.faTypes["FAp"]) lipid.headGroupNames[0] += " O-p";
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
        }
        
        
        
        
        public void LCBPreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                fag = ((Sphingolipid)lipid).lcb;
            }
        }
        
        
        
        
        public void mod_textPreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid) && fag != null)
            {
                if (node.getText().Equals("OH"))
                {
                    if (fag.hydroxylCounts.Count == 0)
                    {
                        fag.hydroxylCounts.Add(1);
                    }
                    else
                    {
                        int hydCnt = (new List<int>(fag.hydroxylCounts))[0];
                        fag.hydroxylCounts.Clear();
                        fag.hydroxylCounts.Add(hydCnt + 1);
                    }
                }
            }
        }
        
        
        
        public void LCBPostEvent(Parser.TreeNode node)
        {
            FALCBvalidationCheck();
        }
        
        
        
        
        public void FAPreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                fag = (fagEnum != null && fagEnum.MoveNext()) ? fagEnum.Current : null;
            }
        }
        
        
        
        
        public void FAPostEvent(Parser.TreeNode node)
        {
            if ((fag != null) && fag.faTypes["FAp"])
            {
                int dbCnt = (new List<int>(fag.doubleBondCounts))[0];
                fag.doubleBondCounts.Clear();
                fag.doubleBondCounts.Add(dbCnt + 1);
            }
            FALCBvalidationCheck();
        }
        
        
        
        
        public void FALCBvalidationCheck()
        {            
            // check if created fatty acid is valid
            if (lipid != null && !(lipid is UnsupportedLipid) && fag != null)
            {
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
                        
                        int maxDoubleBond = Math.Max((carbonLength - 1) >> 1, 0);
                        
                        if (doubleBondCount > maxDoubleBond)
                        {
                            lipid = null;
                        }
                        else if (fag.hydroxylCounts.Count == 1)
                        {
                            int hydroxylCount = (new List<int>(fag.hydroxylCounts))[0];
                            if (carbonLength < hydroxylCount) lipid = null;
                        }
                        
                        if (carbonLength == 0)
                        {
                            fag.faTypes["FA"] = false;
                            fag.faTypes["FAp"] = false;
                            fag.faTypes["FAa"] = false;
                            fag.faTypes["FAx"] = true;
                        }
                    }
                    else 
                    {
                        lipid = null;
                    }
                    
                    // check if at least one fatty acid type is enabled
                    int enablesFATypes = 0;
                    foreach(KeyValuePair<string, bool> kvp in fag.faTypes) enablesFATypes += kvp.Value ? 1 : 0;                
                    if (enablesFATypes == 0)
                    {
                        lipid = null;
                    }
                }
                else 
                {
                    lipid = null;
                }
            }
            
            if (lipid != null && !(lipid is UnsupportedLipid) && fag != null)
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
                // 0 is allowed here, because for instance TG 12:0/0:0/0:0 is also allowed
                if (0 <= carbonCountInt && carbonCountInt <= LipidCreator.MAX_CARBON_LENGTH) fag.carbonCounts.Add(carbonCountInt);
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
        
        
        
        
        
        public void HydroxylLCBPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string hydroxylCount = node.getText();
                int hydroxylCountInt = 0; 
                
                if (hydroxylCount == "m"){
                    lipid = new UnsupportedLipid(lipidCreator);
                }
                else {
                    if (hydroxylCount == "d") hydroxylCountInt = LipidCreator.MIN_LCB_HYDROXY_LENGTH;
                    else if (hydroxylCount == "t") hydroxylCountInt = LipidCreator.MAX_LCB_HYDROXY_LENGTH;
                    if (fag.isLCB && LipidCreator.MIN_LCB_HYDROXY_LENGTH <= hydroxylCountInt && hydroxylCountInt <= LipidCreator.MAX_LCB_HYDROXY_LENGTH) fag.hydroxylCounts.Add(hydroxylCountInt);
                    else if ((lipid is Sphingolipid) && !fag.isLCB && LipidCreator.MIN_SPHINGO_FA_HYDROXY_LENGTH <= hydroxylCountInt && hydroxylCountInt <= LipidCreator.MAX_SPHINGO_FA_HYDROXY_LENGTH) fag.hydroxylCounts.Add(hydroxylCountInt);
                    else if (!(lipid is Sphingolipid) && LipidCreator.MIN_HYDROXY_LENGTH <= hydroxylCountInt && hydroxylCountInt <= LipidCreator.MAX_HYDROXY_LENGTH) fag.hydroxylCounts.Add(hydroxylCountInt);
                    else fag = null;
                }
            }
        }
        
        
        
        
        
        
        public void EtherPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                List<string> keys = new List<string>(fag.faTypes.Keys);
                foreach(string faTypeKey in keys) fag.faTypes[faTypeKey] = false;
            
                string faType = node.getText();
                if (faType == "O-") faType = "a";
                else if (faType == "P-") faType = "p";
                fag.faTypes["FA" + faType] = true;
            }
        }
        
        
        
        
        public void HG_SGLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                string headgroup = node.getText();
                if (headgroup != "SQMG"){
                    lipid.headGroupNames.Add(headgroup);
                    List<string> keys = new List<string>(((Glycerolipid)lipid).fag3.faTypes.Keys);
                    foreach(string faTypeKey in keys) ((Glycerolipid)lipid).fag3.faTypes[faTypeKey] = false;
                    ((Glycerolipid)lipid).fag3.faTypes["FAx"] = true;
                    if (headgroup != "DG") ((Glycerolipid)lipid).containsSugar = true;
                }
                else
                {
                    lipid = new UnsupportedLipid(lipidCreator);
                }
            }
        }
        
        
        
        
        public void SphingoXinePostEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                FALCBvalidationCheck();
            }
        }
        
        
        
        
        public void SphingoXinePreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                ((Sphingolipid)lipid).isLyso = true;
                fag = ((Sphingolipid)lipid).lcb;
                fag.hydroxylCounts.Add(2);
            }
        }
        
        
        
        
        public void SphingoXine_purePreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid) && fag != null)
            {
                fag.carbonCounts.Add(18);
            }
        }
        
        
        
        public void Sphingosine_namePreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid) && fag != null)
            {
                fag.doubleBondCounts.Add(1);
                string headgroup = node.getText();
                if (headgroup.Equals("Sphingosine")) lipid.headGroupNames.Add("LCB");
                else if (headgroup.Equals("So")) lipid.headGroupNames.Add("LCB");
                else if (headgroup.Equals("Sphingosine-1-phosphate")) lipid.headGroupNames.Add("LCBP");
                else lipid = null;
            }
        }
        
        
        
        
        public void Sphinganine_namePreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid) && fag != null)
            {
                fag.doubleBondCounts.Add(0);
                string headgroup = node.getText();
                if (headgroup.Equals("Sphinganine")) lipid.headGroupNames.Add("LCB");
                else if (headgroup.Equals("Sa")) lipid.headGroupNames.Add("LCB");
                else if (headgroup.Equals("Sphinganine-1-phosphate")) lipid.headGroupNames.Add("LCBP");
                else lipid = null;
            }
        }
        
        
        
        
        
        public void CTypePreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid) && fag != null)
            {
                string carbonCount = node.right.getText(); // omit the C e.g. for C16
                int carbonCountInt = Convert.ToInt32(carbonCount);
                if (0 <= carbonCountInt && carbonCountInt <= LipidCreator.MAX_CARBON_LENGTH) fag.carbonCounts.Add(carbonCountInt);
                else fag = null;
            }
        }
        
        
        
        
        
        public void HG_GLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
            }
        }
        
        
        
        
        public void HG_CLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                ((Phospholipid)lipid).isCL = true;
            }
        }
        
        
        
        
        
        public void HG_DPLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                string headgroup = node.getText();
                if ((new HashSet<string>{"PIM1", "PIM2", "PIM3", "PIM4", "PIM5", "PIM6", "Glc-DG", "PGP", "PE-NMe2", "AC2SGL", "DAT", "PE-NMe", "PT", "Glc-GP", "NAPE"}).Contains(headgroup))
                {
                    lipid = new UnsupportedLipid(lipidCreator);
                }
                else {
                    if ("CDP-DG".Equals(headgroup)) headgroup = "CDPDAG";
                    else if ("LBPA".Equals(headgroup)) headgroup = "BMP";
                    lipid.headGroupNames.Add(headgroup);
                }
            }
        }
        
        
        
        
        
        public void HG_LPLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                string headgroup = node.getText();
                if ((new HashSet<string>{"LPIM1", "LPIM2", "LPIM3", "LPIM4", "LPIM5", "LPIM6", "CPA"}).Contains(headgroup))
                {
                    lipid = new UnsupportedLipid(lipidCreator);
                }
                else
                {
                    if ("LysoPC".Equals(headgroup)) headgroup = "LPC";
                    else if ("LysoPE".Equals(headgroup)) headgroup = "LPE";
                    lipid.headGroupNames.Add(headgroup);
                    ((Phospholipid)lipid).isLyso = true;
                }
            }
        }
        
        
        
        
        public void HG_4PLPreEvent(Parser.TreeNode node)
        {
            lipid = new UnsupportedLipid(lipidCreator);
        }
        
        
        
        
        public void HG_LSLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                string headgroup = node.getText();
                ((Sphingolipid)lipid).isLyso = true;
                if (headgroup.Equals("SPH")) headgroup = "LCB";
                else if (headgroup.Equals("S1P")) headgroup = "LCBP";
                else if (headgroup.Equals("HexSph")) headgroup = "LHexCer";
                else if (headgroup.Equals("SPC")) headgroup = "LSM";
                else if (headgroup.Equals("SPH-P")) headgroup = "LCBP";
                lipid.headGroupNames.Add(headgroup);
            }
        }
        
        
        
        public void HG_DSLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                string headgroup = node.getText();
                if ((new HashSet<string>{"FMC-5", "FMC-6"}).Contains(headgroup))
                {
                    lipid = new UnsupportedLipid(lipidCreator);
                }
                else
                {
                    if (headgroup.Equals("PE-Cer")) headgroup = "EPC";
                    else if (headgroup.Equals("PI-Cer")) headgroup = "IPC";
                    else if (headgroup.Equals("LacCer")) headgroup = "Hex2Cer";
                    else if (headgroup.Equals("GalCer")) headgroup = "HexCer";
                    else if (headgroup.Equals("GlcCer")) headgroup = "HexCer";
                    else if (headgroup.Equals("(3'-sulfo)Galbeta-Cer")) headgroup = "SHexCer";
                    lipid.headGroupNames.Add(headgroup);
                }
            }
        }
        
        
        
        
        public void ChPreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                lipid.headGroupNames.Add("Ch");
                List<string> keys = new List<string>(((Cholesterol)lipid).fag.faTypes.Keys);
                foreach(string faTypeKey in keys) ((Cholesterol)lipid).fag.faTypes[faTypeKey] = false;
                ((Cholesterol)lipid).fag.faTypes["FAx"] = true;
            }
        }
        
        
        
        
        public void HG_ChEPreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                lipid.headGroupNames.Add("ChE");
                ((Cholesterol)lipid).containsEster = true;
            }
        }
        
        
        
        
        
        public void SLPostEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                if (lipid.headGroupNames.Count == 0)
                {
                    lipid = null;
                }
            }
        }
            
            
        public void Mediator_OxoPreEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                mediatorName += "Oxo";
            }
        }
            
            
        public void MediatorPostEvent(Parser.TreeNode node)
        {
            if (lipid != null && !(lipid is UnsupportedLipid))
            {
                if (mediatorName.Equals("Arachidonic acid")) mediatorName = "AA";
                else if (mediatorName.Equals("Arachidonic Acid")) mediatorName = "AA";
            
                if ((new HashSet<string>{"10-HDoHE", "11-HDoHE", "11-HETE", "11,12-DHET", "11(12)-EET", "12-HEPE", "12-HETE", "12-HHTrE", "12-OxoETE", "12(13)-EpOME", "13-HODE", "13-HOTrE", "14,15-DHET", "14(15)-EET", "14(15)-EpETE", "15-HEPE", "15-HETE", "15d-PGJ2", "16-HDoHE", "16-HETE", "18-HEPE", "5-HEPE", "5-HETE", "5-HpETE", "5-OxoETE", "5,12-DiHETE", "5,6-DiHETE", "5,6,15-LXA4", "5(6)-EET", "8-HDoHE", "8-HETE", "8,9-DHET", "8(9)-EET", "9-HEPE", "9-HETE", "9-HODE", "9-HOTrE", "9(10)-EpOME", "AA", "alpha-LA", "DHA", "EPA", "Linoleic acid", "LTB4", "LTC4", "LTD4", "Maresin 1", "Palmitic acid", "PGB2", "PGD2", "PGE2", "PGF2alpha", "PGI2", "Resolvin D1", "Resolvin D2", "Resolvin D3", "Resolvin D5", "tetranor-12-HETE", "TXB1", "TXB2", "TXB3"}).Contains(mediatorName))
                {
                    lipid.headGroupNames.Add(mediatorName);
                }
                else
                {
                    lipid = new UnsupportedLipid(lipidCreator);
                }
            }
        }
    }    
}