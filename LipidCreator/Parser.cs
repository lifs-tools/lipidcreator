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
using System.IO;

namespace LipidCreator
{    
    
    [Serializable]
    public class Parser
    {
        public class TreeNode
        {
            public int rule;
            public TreeNode left;
            public TreeNode right;
            public char terminal;
            public ArrayList preEvents;
            public ArrayList postEvents;
            
            
            public TreeNode(int _rule)
            {
                rule = _rule;
                left = null;
                right = null;
                terminal = '\0';
                preEvents = new ArrayList();
                postEvents = new ArrayList();
            }
            
            public string getTextRecursive(TreeNode node)
            {
                string text = "";
                if (node.terminal == '\0')
                {
                    text = node.getTextRecursive(node.left);
                    text += node.getTextRecursive(node.right);
                }
                else
                {
                    text += node.terminal;
                }
                return text;
            }
            
                
            public string getText()
            {
                return getTextRecursive(this);
            }
        }
        
        
        
        public int freeNumber;
        public Dictionary<string, int> ruleToNT;
        public Dictionary<char, ArrayList> TtoNT;
        public Dictionary<int, ArrayList> NTtoNT;
        public char quote;
        public TreeNode parseTree;
        public bool wordInGrammer;
        public Dictionary<int, string> NTtoRule;
        public Dictionary<string, Action<Parser.TreeNode>> events;
        public LipidCreator lipidCreator;
    
        // lipid specific variables
        public Lipid lipid;
        public FattyAcidGroupEnumerator fagEnum;
        public FattyAcidGroup fag;
    
    
        public Parser(LipidCreator _lipidCreator, string grammerFilename, char _quote = '"')
        {
            freeNumber = 0;
            ruleToNT = new Dictionary<string, int>();
            TtoNT = new Dictionary<char, ArrayList>();
            NTtoNT = new Dictionary<int, ArrayList>();
            quote = _quote;
            parseTree = null;
            wordInGrammer = false;
            NTtoRule = new Dictionary<int, string>();
            lipidCreator = _lipidCreator;
            
            events = new Dictionary<string, Action<TreeNode>>();
            events.Add("FA_pre_event", FAPreEvent);
            events.Add("FA_post_event", FAPostEvent);
            
            events.Add("LCB_pre_event", LCBPreEvent);
            events.Add("Carbon_pre_event", CarbonPreEvent);
            events.Add("DB_pre_event", DBPreEvent);
            events.Add("Hydroxyl_pre_event", HydroxylPreEvent);
            events.Add("Hydroxyl_LCB_pre_event", Hydroxyl_LCBPreEvent);
            events.Add("Ether_pre_event", EtherPreEvent);
            
            events.Add("GL_pre_event", GLPreEvent);
            events.Add("PL_pre_event", PLPreEvent);
            events.Add("SL_pre_event", SLPreEvent);
            events.Add("Cholesterol_pre_event", CholesterolPreEvent);
            events.Add("Mediator_pre_event", MediatorPreEvent);
            
            events.Add("HG_MGL_pre_event", HG_MGLPreEvent);
            events.Add("HG_DGL_pre_event", HG_DGLPreEvent);
            events.Add("HG_SGL_pre_event", HG_SGLPreEvent);
            events.Add("HG_TGL_pre_event", HG_TGLPreEvent);
            
            
            
            
            int lineCounter = 0;
            if (File.Exists(grammerFilename))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(grammerFilename))
                    {
                        string line;
                        while((line = sr.ReadLine()) != null)
                        {
                            lineCounter++;
                            // skip empty lines and comments
                            if (line.Length < 1) continue;
                            if (line.IndexOf("#") > -1) line = line.Substring(0, line.IndexOf("#"));
                            if (line.Length < 1) continue;
                            line = strip(line, ' ');
                            if (line.Length < 2) continue;
                            
                            ArrayList tokens_level_1 = new ArrayList();
                            foreach (string t in splitString(line, '=', quote))
                            {
                                tokens_level_1.Add(strip(t, ' '));
                            }
                            if (tokens_level_1.Count != 2) throw new Exception("Error: corrupted token in grammer");


                            string rule = (string)tokens_level_1[0];
                            
                            ArrayList products = new ArrayList();
                            foreach (string p in splitString((string)tokens_level_1[1], '|', quote))
                            {
                                products.Add(strip(p, ' '));
                            }
                            

                            if (!ruleToNT.ContainsKey(rule))
                            {
                                ruleToNT.Add(rule, freeNumber);
                                freeNumber += 1;
                            }
                            int ruleNT = ruleToNT[rule];
                            NTtoRule.Add(ruleNT, rule);
                            
                            
                            foreach (string product in products)
                            {
                                ArrayList singleNTs = new ArrayList();
                                foreach (string NT in splitString(product, ' ', quote))
                                {
                                    singleNTs.Add(strip(NT, ' '));
                                }
                                
                                
                                // changing all (non)terminals into rule numbers
                                for (int i = 0; i < singleNTs.Count; ++i)
                                {
                                
                                    if (isTerminal((string)singleNTs[i]))
                                    {
                                        singleNTs[i] = addTerminal((string)singleNTs[i]);
                                    }
                                    else
                                    {
                                        if (!ruleToNT.ContainsKey((string)singleNTs[i]))
                                        {
                                            ruleToNT[(string)singleNTs[i]] = freeNumber;
                                            freeNumber += 1;
                                        }
                                        singleNTs[i] = ruleToNT[(string)singleNTs[i]];
                                    }
                                }
                                
                                
                                // more than two rules
                                while (singleNTs.Count > 2)
                                {
                                    int p2NF = (int)singleNTs[singleNTs.Count - 1];
                                    singleNTs.RemoveAt(singleNTs.Count - 1);
                                    int p1NF = (int)singleNTs[singleNTs.Count - 1];
                                    singleNTs.RemoveAt(singleNTs.Count - 1);
                                    
                                    int n = freeNumber;
                                    freeNumber += 1;
                                    
                                    int key = (p1NF << 16) | p2NF;
                                    if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new ArrayList());
                                    NTtoNT[key].Add(n);
                                    
                                    singleNTs.Add(n);
                                }    
                                
                                    
                                // two product rules
                                if (singleNTs.Count == 2)
                                {
                                    int p1NF = (int)singleNTs[0];
                                    int p2NF = (int)singleNTs[1];
                                    int key = (p1NF << 16) | p2NF;
                                    if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new ArrayList());
                                    NTtoNT[key].Add(ruleNT);
                                }
                                
                                
                                // only one product rule
                                else if (singleNTs.Count == 1)
                                {
                                    int p1NF = (int)singleNTs[0];
                                    if (!NTtoNT.ContainsKey(p1NF)) NTtoNT.Add(p1NF, new ArrayList());
                                    NTtoNT[p1NF].Add(ruleNT);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file '" + grammerFilename + "' in line '" + lineCounter + "' could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("Error: file '" + grammerFilename + "' does not exist or can not be opened.");
            }
            
            
        }
        
        
        
        
        
        public ArrayList splitString(string text, char separator, char quote)
        {
            bool inQuote = false;
            ArrayList tokens = new ArrayList();
            string token = "";
            
            foreach (char c in text)
            {
                if (!inQuote)
                {
                    if (c == separator)
                    {
                        if (token.Length > 0) tokens.Add(token);
                        token = "";
                    }
                    else
                    {
                        if (c == quote) inQuote = !inQuote;
                        token += c;
                    }
                }
                else
                {
                    if (c == quote) inQuote = !inQuote;
                    token += c;
                }
            }
                    
            if (token.Length > 0) tokens.Add(token);
            
            return (inQuote ? null : tokens);
        }
        
        
        public string strip(string text, char stripChar)
        {
            while (text.Length > 1 && text[0] == stripChar) text = text.Substring(1, text.Length - 1);
            while (text.Length > 1 && text[text.Length - 1] == stripChar) text = text.Substring(0, text.Length - 1);
            return text;
        }
        
        
        public bool isTerminal(string token)
        {
            string[] tks = token.Split(quote);
            if (tks.Length != 1 && tks.Length != 3) throw new Exception("Error: corrupted token in grammer");
            
            if (tks.Length == 1) return false;
        
            if (token[0] == quote && token[token.Length - 1] == quote) return true;

            throw new Exception("Error: corrupted token in grammer");
        }
        
        
        public int addTerminal(string text)
        {
            text = strip(text, quote);
            ArrayList tRules = new ArrayList();
            foreach (char c in text)
            {
                if (!TtoNT.ContainsKey(c)) TtoNT.Add(c, new ArrayList());
                TtoNT[c].Add(freeNumber);
                tRules.Add(freeNumber);
                freeNumber += 1;
            }
            while (tRules.Count > 1)
            {
                int p2NF = (int)tRules[tRules.Count - 1];
                tRules.RemoveAt(tRules.Count - 1);
                int p1NF = (int)tRules[tRules.Count - 1];
                tRules.RemoveAt(tRules.Count - 1);
                
                int n = freeNumber;
                freeNumber += 1;
                
                int key = (p1NF << 16) | p2NF;
                if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new ArrayList());
                NTtoNT[key].Add(n);
                
                tRules.Add(n);
            }
            return (int)tRules[0];
        }
        
        
        
        // adding singleton rules, e.g. S -> A, A -> B, B -> C
        public ArrayList collectBackward(int r1)
        {
            ArrayList collection = new ArrayList();
            collection.Add(r1);
            int i = 0;
            while (i < collection.Count)
            {
                int r = (int)collection[i];
                if (NTtoNT.ContainsKey(r))
                {
                    foreach (int rf in NTtoNT[r]) collection.Add(rf);
                }
                i += 1;
            }
            return collection;
        }
            
            
            
        
        public void raiseEventsRecursive(TreeNode node)
        {
            foreach (Action<TreeNode> action in node.preEvents) action(node);
            
            if (node.terminal == '\0') // node.terminal is != null when node is leaf
            {
                raiseEventsRecursive(node.left);
                raiseEventsRecursive(node.right);
            }
                
            foreach (Action<TreeNode> action in node.postEvents) action(node);
        }
        
        
                    
        
        public void raiseEvents()
        {
            if (parseTree != null) raiseEventsRecursive(parseTree);
        }
        
        
        
        
    
    
        public void fillTree(TreeNode node, ArrayList dp, int i, int j)
        {
            ArrayList dpCell = ((Dictionary<int, ArrayList>)((ArrayList)dp[i])[j])[node.rule];
            
            if (i > 0) // 0 => leaf
            {
            
            
                // filling the syntax tree including lexers and events
                int key = ((int)dpCell[0] << 16) | (int)dpCell[1];
                ArrayList mergedRules = collectBackward(key);
                
                // it comes in reversed order, so it makes sence to start with post events
                foreach(int r in mergedRules)
                {
                    if (NTtoRule.ContainsKey(r))
                    {
                        string postEventName = NTtoRule[r] + "_post_event";
                        if (events.ContainsKey(postEventName)) node.postEvents.Add(events[postEventName]);
                    }
                }
                
                mergedRules.Reverse();
                foreach(int r in mergedRules)
                {
                    if (NTtoRule.ContainsKey(r))
                    {
                        string preEventName = NTtoRule[r] + "_pre_event";
                        if (events.ContainsKey(preEventName)) node.preEvents.Add(events[preEventName]);
                    }
                }
            
                node.left = new TreeNode((int)dpCell[0]);
                node.right = new TreeNode((int)dpCell[1]);
                int ii = (int)((ArrayList)dpCell[2])[0];
                int jj = (int)((ArrayList)dpCell[2])[1];
                fillTree(node.left, dp, ii, jj);
                ii = (int)((ArrayList)dpCell[3])[0];
                jj = (int)((ArrayList)dpCell[3])[1];
                fillTree(node.right, dp, ii, jj);
            }
            else
            {
                ArrayList mergedRules = collectBackward(node.rule);
                
                // it comes in reversed order, so it makes sence to start with post events
                foreach(int r in mergedRules)
                {
                    if (NTtoRule.ContainsKey(r))
                    {
                        string postEventName = NTtoRule[r] + "_post_event";
                        if (events.ContainsKey(postEventName)) node.postEvents.Add(events[postEventName]);
                    }
                }
                
                mergedRules.Reverse();
                foreach(int r in mergedRules)
                {
                    if (NTtoRule.ContainsKey(r))
                    {
                        string preEventName = NTtoRule[r] + "_pre_event";
                        if (events.ContainsKey(preEventName)) node.preEvents.Add(events[preEventName]);
                    }
                }
            
                node.terminal = (char)dpCell[0];
            }
        }
        
        
        
        
        
        // re-implementation of Cocke-Younger-Kasami algorithm
        public void parse(string textToParse)
        {
            lipid = null;
            fagEnum = null;
            fag = null;
        
            wordInGrammer = false;
            int n = textToParse.Length;
            ArrayList dp = new ArrayList(); // dp stands for dynamic programming
            for (int i = 0; i < n; ++i)
            {
                ArrayList row = new ArrayList();
                for (int j = 0; j < n; ++j)
                {
                    Dictionary<int, ArrayList> d = new Dictionary<int, ArrayList>();
                    row.Add(d);
                }
                dp.Add(row);
            }
            
            for (int i = 0; i < n; ++i)
            {
                char c = textToParse[i];
                if (!TtoNT.ContainsKey(c)) return;
                foreach (int r in TtoNT[c])
                {
                    foreach (int rf in collectBackward(r))
                    {
                        ArrayList al = new ArrayList();
                        al.Add(c);
                        ((Dictionary<int, ArrayList>)((ArrayList)dp[0])[i]).Add(rf, al);
                    }
                }
            }        
            
            
            for (int i = 1 ; i < n; ++i)
            {
                for (int j = 0; j < n - i; ++j)
                {
                    for (int k = 0; k < i; ++k)
                    {
                        foreach (int r1 in ((Dictionary<int, ArrayList>)((ArrayList)dp[k])[j]).Keys)
                        {
                            foreach (int r2 in ((Dictionary<int, ArrayList>)((ArrayList)dp[i - k - 1])[j + k + 1]).Keys)
                            {
                                int key = (r1 << 16) | r2;
                                if (NTtoNT.ContainsKey(key))
                                {
                                    foreach (int r in NTtoNT[key])
                                    {
                                        foreach (int rf in collectBackward(r))
                                        {
                                            ArrayList content = new ArrayList();
                                            content.Add(r1);
                                            content.Add(r2);
                                            content.Add(new ArrayList{k, j});
                                            content.Add(new ArrayList{i - k - 1, j + k + 1});
                                            
                                            ((Dictionary<int, ArrayList>)((ArrayList)dp[i])[j]).Add(rf, content);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            wordInGrammer = ((Dictionary<int, ArrayList>)((ArrayList)dp[n - 1])[0]).ContainsKey(0);
            
            if (wordInGrammer)
            {
                parseTree = new TreeNode(0);
                fillTree(parseTree, dp, n - 1, 0);
            }
            else
            {
                parseTree = null;
            }
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
        
        public void SLPreEvent(Parser.TreeNode node)
        {
            lipid = new SLLipid(lipidCreator);
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
            fagEnum = new FattyAcidGroupEnumerator((Mediator)lipid);
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
            if (fag != null)
            {
                if (fag.carbonCounts.Count == 1 && fag.doubleBondCounts.Count == 1)
                {
                    int carbonLength = (new List<int>(fag.carbonCounts))[0];
                    int doubleBondCount = (new List<int>(fag.doubleBondCounts))[0];
                    
                    int maxDoubleBond = (carbonLength - 1) >> 1;
                    if (doubleBondCount > maxDoubleBond)
                    {
                        Console.WriteLine("double bond error");
                        lipid = null;
                    }
                    else if (fag.hydroxylCounts.Count == 1)
                    {
                        int hydroxylCount = (new List<int>(fag.hydroxylCounts))[0];
                        
                        if (carbonLength < hydroxylCount)
                        {
                            Console.WriteLine("hydroxyl number error");
                            lipid = null;
                        }
                    }
                }
                else 
                {
                    lipid = null;
                }
            }
        }
        
        public void CarbonPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string carbonCount = node.getText();
                fag.carbonCounts.Add(Convert.ToInt32(carbonCount));
                Console.WriteLine("l: " + fag.lengthInfo + " " + carbonCount);
            }
        }
        
        public void DBPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string doubleBondCount = node.getText();
                fag.doubleBondCounts.Add(Convert.ToInt32(doubleBondCount));
                Console.WriteLine("db: " + fag.dbInfo + " " + doubleBondCount);
            }
        }
        
        public void HydroxylPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string hydroxylCount = node.getText();
                fag.hydroxylCounts.Add(Convert.ToInt32(hydroxylCount));
                Console.WriteLine("hy: " + fag.dbInfo + " " + hydroxylCount);
            }
        }
        
        public void Hydroxyl_LCBPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
                string hydroxylCount = node.getText();
                fag.hydroxylCounts.Add(Convert.ToInt32(hydroxylCount));
                Console.WriteLine("hy: " + fag.hydroxylInfo + " " + hydroxylCount);
            }
        }
        
        public void EtherPreEvent(Parser.TreeNode node)
        {
            if (fag != null)
            {
            
                List<string> keys = new List<string>(fag.faTypes.Keys);
                foreach(string faTypeKey in keys) fag.faTypes[faTypeKey] = false;
            
                string faType = node.getText();
                fag.faTypes["FA" + faType] = false;
                Console.WriteLine("type: " + faType);
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
                Console.WriteLine("headgroup: " + headgroup);
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
                Console.WriteLine("headgroup: " + headgroup);
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
                Console.WriteLine("headgroup: " + headgroup);
            }
        }
        
        public void HG_TGLPreEvent(Parser.TreeNode node)
        {
            if (lipid != null)
            {
                string headgroup = node.getText();
                lipid.headGroupNames.Add(headgroup);
                Console.WriteLine("headgroup: " + headgroup);
            }
        }
    }    
}