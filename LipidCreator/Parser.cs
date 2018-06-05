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
    public class Parser
    {
        public class TreeNode
        {
            public int rule;
            public TreeNode left;
            public TreeNode right;
            public char terminal;
            //self.pre_event = None
            //self.post_event = None
            
            
            public TreeNode(int _rule)
            {
                rule = _rule;
                left = null;
                right = null;
                terminal = '\0';
                //self.pre_event = None
                //self.post_event = None
            }
            
            public string getTextRecursive(TreeNode node)
            {
                string text = "";
                if (node.terminal != '\0')
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
        //self.events = _events
    
        public Parser(string filename, char _quote)
        {
            freeNumber = 0;
            ruleToNT = new Dictionary<string, int>();
            TtoNT = new Dictionary<char, ArrayList>();
            NTtoNT = new Dictionary<int, ArrayList>();
            quote = _quote;
            parseTree = null;
            wordInGrammer = false;
            NTtoRule = new Dictionary<int, string>();
            //events = _events
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
            while (text[0] == stripChar) text = text.Substring(1, text.Length - 1);
            while (text[text.Length - 1] == stripChar) text = text.Substring(0, text.Length - 1);
            return text;
        }
        
        
        public bool isTerminal(string token)
        {
            string[] tks = token.Split(quote);
            if (tks.Length != 1 || tks.Length != 3) throw new Exception("Error: corrupted token in grammer");
            
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
            //if node.pre_event != None: node.pre_event(node)
            
            if (node.terminal != '\0')
            {
                raiseEventsRecursive(node.left);
                raiseEventsRecursive(node.right);
            }
                
            //if node.post_event != None: node.post_event(node)
        }
        
        
                    
        
        public void raiseEvents()
        {
            if (parseTree != null) raiseEventsRecursive(parseTree);
        }
        
        
        
        
    
    
        // filling the syntax tree including lexers and events
        public void fillTree(TreeNode node, ArrayList dp, int i, int j)
        {
            ArrayList dpCell = ((Dictionary<int, ArrayList>)((ArrayList)dp[i])[j])[node.rule];
        
            if (NTtoRule.ContainsKey(node.rule))
            {
                /*
                string preEventName = NTtoRule[node.rule] + "_pre_event";
                string postEventName = NTtoRule[node.rule] + "_post_event";
                
                if pre_event_name in self.events:
                    node.pre_event = self.events[pre_event_name]
                if post_event_name in self.events:
                    node.post_event = self.events[post_event_name]
                */
            }
            if (i == 0)
            {
                node.terminal = (char)dpCell[0];
            }
            else
            {
                node.left = new TreeNode((int)dpCell[0]);
                node.right = new TreeNode((int)dpCell[1]);
                int l = (int)((ArrayList)dpCell[2])[0];
                int r = (int)((ArrayList)dpCell[2])[1];
                fillTree(node.left, dp, l, r);
                l = (int)((ArrayList)dpCell[3])[0];
                r = (int)((ArrayList)dpCell[3])[1];
                fillTree(node.right, dp, l, r);
            }
        }
        
        
        
        // re-implementation of Cocke-Younger-Kasami algorithm
        public void parse(string textToParse)
        {
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
    }    
}