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

namespace LipidCreator
{    
    
    [Serializable]
    public class ParserEventHandler
    {
        public Dictionary<string, Action<Parser.TreeNode>> registeredEvents;
        public LipidCreator lipidCreator;
        public Lipid lipid;
        public FattyAcidGroupEnumerator fagEnum;
        public FattyAcidGroup fag;
    
    
        public ParserEventHandler(LipidCreator _lipidCreator)
        {
            lipidCreator = _lipidCreator;
            resetLipidBuilder();
            
            registeredEvents = new Dictionary<string, Action<Parser.TreeNode>>();
            registeredEvents.Add("FA_pre_event", FAPreEvent);
            registeredEvents.Add("FA_post_event", FAPostEvent);
            
            registeredEvents.Add("LCB_pre_event", LCBPreEvent);
            registeredEvents.Add("Carbon_pre_event", CarbonPreEvent);
            registeredEvents.Add("DB_pre_event", DBPreEvent);
            registeredEvents.Add("Hydroxyl_pre_event", HydroxylPreEvent);
            registeredEvents.Add("Hydroxyl_LCB_pre_event", Hydroxyl_LCBPreEvent);
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
            
            registeredEvents.Add("HG_CH_pre_event", HG_CHPreEvent);
            
            registeredEvents.Add("PL_post_event", PLPostEvent);
            registeredEvents.Add("SL_post_event", SLPostEvent);
            registeredEvents.Add("Mediator_post_event", MediatorPostEvent);
            
        }
        
        
        public void handleEvent(string eventName, Parser.TreeNode node)
        {
            if (registeredEvents.ContainsKey(eventName))
            {
                registeredEvents[eventName](node);
            }
        }
        
        
        
        
        public void resetLipidBuilder()
        {
            lipid = null;
            fagEnum = null;
            fag = null;
        }
        
        
        
        
        // handling all events
        public void GLPreEvent(Parser.TreeNode node)
        {
            lipid = new GLLipid(lipidCreator);
            fagEnum = new FattyAcidGroupEnumerator((GLLipid)lipid);
        }
        
        public void PLPreEvent(Parser.TreeNode node)
        {
            lipid = new PLLipid(lipidCreator);
            fagEnum = new FattyAcidGroupEnumerator((PLLipid)lipid);
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
            lipid = new SLLipid(lipidCreator);
            ((SLLipid)lipid).lcb.hydroxylCounts.Clear();
            ((SLLipid)lipid).fag.hydroxylCounts.Clear();
            fagEnum = new FattyAcidGroupEnumerator((SLLipid)lipid);
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
            fag = ((SLLipid)lipid).lcb;
        }
        
        public void FAPreEvent(Parser.TreeNode node)
        {
            fag = (fagEnum != null && fagEnum.MoveNext()) ? fagEnum.Current : null;
        }
        
        public void FAPostEvent(Parser.TreeNode node)
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
                    if (doubleBondCount > maxDoubleBond)
                    {
                        lipid = null;
                    }
                    else if (fag.hydroxylCounts.Count == 1)
                    {
                        int hydroxylCount = (new List<int>(fag.hydroxylCounts))[0];
                        
                        if (carbonLength < hydroxylCount)
                        {
                            lipid = null;
                        }
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
        }
        
        public void CarbonPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string carbonCount = node.getText();
                fag.carbonCounts.Add(Convert.ToInt32(carbonCount));
            }
        }
        
        public void DBPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string doubleBondCount = node.getText();
                fag.doubleBondCounts.Add(Convert.ToInt32(doubleBondCount));
            }
        }
        
        public void HydroxylPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string hydroxylCount = node.getText();
                fag.hydroxylCounts.Add(Convert.ToInt32(hydroxylCount));
            }
        }
        
        public void Hydroxyl_LCBPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string hydroxylCount = node.getText();
                fag.hydroxylCounts.Add(Convert.ToInt32(hydroxylCount));
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
                List<string> keys = new List<string>(((GLLipid)lipid).fag2.faTypes.Keys);
                foreach(string faTypeKey in keys) ((GLLipid)lipid).fag2.faTypes[faTypeKey] = false;
                ((GLLipid)lipid).fag2.faTypes["FAx"] = true;
                keys = new List<string>(((GLLipid)lipid).fag3.faTypes.Keys);
                foreach(string faTypeKey in keys) ((GLLipid)lipid).fag3.faTypes[faTypeKey] = false;
                ((GLLipid)lipid).fag3.faTypes["FAx"] = true;
            }
        }
        
        public void HG_DGLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                List<string> keys = new List<string>(((GLLipid)lipid).fag3.faTypes.Keys);
                foreach(string faTypeKey in keys) ((GLLipid)lipid).fag3.faTypes[faTypeKey] = false;
                ((GLLipid)lipid).fag3.faTypes["FAx"] = true;
            }
        }
        
        public void HG_SGLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                List<string> keys = new List<string>(((GLLipid)lipid).fag3.faTypes.Keys);
                foreach(string faTypeKey in keys) ((GLLipid)lipid).fag3.faTypes[faTypeKey] = false;
                ((GLLipid)lipid).fag3.faTypes["FAx"] = true;
                ((GLLipid)lipid).containsSugar = true;
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
                ((PLLipid)lipid).isCL = true;
            }
        }
        
        public void HG_MLCLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                ((PLLipid)lipid).isCL = true;
                List<string> keys = new List<string>(((PLLipid)lipid).fag4.faTypes.Keys);
                foreach(string faTypeKey in keys) ((PLLipid)lipid).fag4.faTypes[faTypeKey] = false;
                ((PLLipid)lipid).fag4.faTypes["FAx"] = true;
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
                ((PLLipid)lipid).isLyso = true;
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
                ((PLLipid)lipid).isLyso = true;
            }
        }
        
        public void HG_LSLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                ((SLLipid)lipid).isLyso = true;
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
        
        public void HG_CHPreEvent(Parser.TreeNode node)
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
    }    
}