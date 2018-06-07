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
        public class ExtendedLinkedList<T> : LinkedList<T>
        {
            public T PopFirst()
            {
                T current = First.Value;
                RemoveFirst();
                return current;
            }
            
            public T PopLast()
            {
                T current = Last.Value;
                RemoveLast();
                return current;
            }
        }
        
        
        // DP stands for dynamic programming
        public class DPNode
        {
            public int ruleIndex1;
            public int ruleIndex2;
            public DPNode left;
            public DPNode right;
            
            public DPNode(int _rule1, int _rule2, DPNode _left, DPNode _right)
            {
                ruleIndex1 = _rule1;
                ruleIndex2 = _rule2;
                left = _left;
                right = _right;
            }
        }
        
        
    
        public class TreeNode
        {
            public int ruleIndex;
            public TreeNode left;
            public TreeNode right;
            public char terminal;
            public bool fireEvent;
            
            public TreeNode(int _rule, bool _fireEvent)
            {
                ruleIndex = _rule;
                left = null;
                right = null;
                terminal = '\0';
                fireEvent = _fireEvent;
            }
            
            public string getText(TreeNode node = null)
            {
                if (node == null) node = this;
                if (node.terminal == '\0') return getText(node.left) + ((node.right != null) ? getText(node.right) : "");
                    
                return Convert.ToString(node.terminal);
            }
        }
        
        
        
        public int nextFreeRuleIndex;
        public Dictionary<char, ArrayList> TtoNT;
        public Dictionary<int, ArrayList> NTtoNT;
        public char quote;
        public TreeNode parseTree;
        public bool wordInGrammer;
        public Dictionary<int, string> NTtoRule;
        public ParserEventHandler parserEventHandler;
    
    
        public Parser(ParserEventHandler _parserEventHandler, string grammerFilename, char _quote = '"')
        {
            nextFreeRuleIndex = 0;
            TtoNT = new Dictionary<char, ArrayList>();
            NTtoNT = new Dictionary<int, ArrayList>();
            quote = _quote;
            parseTree = null;
            wordInGrammer = false;
            NTtoRule = new Dictionary<int, string>();
            parserEventHandler = _parserEventHandler;
            
            
            if (File.Exists(grammerFilename))
            {
                int lineCounter = 0;
                try
                {
                    Dictionary<string, int> ruleToNT = new Dictionary<string, int>();
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
                            foreach (string t in splitString(line, '=', quote)) tokens_level_1.Add(strip(t, ' '));
                            if (tokens_level_1.Count != 2) throw new Exception("Error: corrupted token in grammer");

                            string rule = (string)tokens_level_1[0];
                            
                            ArrayList products = new ArrayList();
                            foreach (string p in splitString((string)tokens_level_1[1], '|', quote)) products.Add(strip(p, ' '));
                            
                            if (!ruleToNT.ContainsKey(rule)) ruleToNT.Add(rule, getNextFreeRuleIndex());
                            int newRuleIndex = ruleToNT[rule];
                            
                            if (NTtoRule.ContainsKey(newRuleIndex)) throw new Exception("Error: corrupted token in grammer: rule '" + rule + "' must not be declared two times.");
                            NTtoRule.Add(newRuleIndex, rule);
                            
                            
                            foreach (string product in products)
                            {
                                LinkedList<string> nonTerminals = new LinkedList<string>();
                                ExtendedLinkedList<int> nonTerminalRules = new ExtendedLinkedList<int>();
                                foreach (string NT in splitString(product, ' ', quote)) nonTerminals.AddLast(strip(NT, ' '));
                                
                                
                                // changing all (non)terminals into rule numbers
                                foreach (string nonTerminal in nonTerminals)
                                {
                                    if (isTerminal(nonTerminal))
                                    {
                                        nonTerminalRules.AddLast(addTerminal(nonTerminal));
                                    }
                                    else
                                    {
                                        if (!ruleToNT.ContainsKey(nonTerminal))
                                        {
                                            ruleToNT[nonTerminal] = getNextFreeRuleIndex();
                                        }
                                        nonTerminalRules.AddLast(ruleToNT[nonTerminal]);
                                    }
                                }
                                
                                
                                // more than two rules
                                while (nonTerminalRules.Count > 2)
                                {
                                    int ruleIndex2 = nonTerminalRules.PopLast();
                                    int ruleIndex1 = nonTerminalRules.PopLast();
                                    
                                    int n = getNextFreeRuleIndex();
                                    
                                    int key = (ruleIndex1 << 16) | ruleIndex2;
                                    if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new ArrayList());
                                    NTtoNT[key].Add(n);
                                    
                                    nonTerminalRules.AddLast(n);
                                }    
                                
                                    
                                // two product rules
                                if (nonTerminalRules.Count == 2)
                                {
                                    int ruleIndex1 = nonTerminalRules.PopFirst();
                                    int ruleIndex2 = nonTerminalRules.PopFirst();
                                    int key = (ruleIndex1 << 16) | ruleIndex2;
                                    if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new ArrayList());
                                    NTtoNT[key].Add(newRuleIndex);
                                }
                                // only one product rule
                                else if (nonTerminalRules.Count == 1)
                                {
                                
                                    int ruleIndex1 = nonTerminalRules.First.Value;
                                    if (ruleIndex1 == newRuleIndex) throw new Exception("Error: corrupted token in grammer: rule '" + rule + "' is not allowed to refer soleley to itself.");
                                    
                                    if (!NTtoNT.ContainsKey(ruleIndex1)) NTtoNT.Add(ruleIndex1, new ArrayList());
                                    NTtoNT[ruleIndex1].Add(newRuleIndex);
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
        
        
        
        public int getNextFreeRuleIndex()
        {
            if (nextFreeRuleIndex < 65536) return nextFreeRuleIndex++;
            throw new Exception("Error: grammer is too big.");
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
            if (inQuote) throw new Exception("Error: corrupted token in grammer");
            
            return tokens;
        }
        
        
        public string strip(string text, char stripChar)
        {
            while (text.Length > 0 && text[0] == stripChar) text = text.Substring(1, text.Length - 1);
            while (text.Length > 0 && text[text.Length - 1] == stripChar) text = text.Substring(0, text.Length - 1);
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
            ExtendedLinkedList<int> terminalRules = new ExtendedLinkedList<int>();
            foreach (char c in text)
            {
                if (!TtoNT.ContainsKey(c)) TtoNT.Add(c, new ArrayList());
                int nextIndex = getNextFreeRuleIndex();
                TtoNT[c].Add(nextIndex);
                terminalRules.AddLast(nextIndex);
            }
            while (terminalRules.Count > 1)
            {
                int ruleIndex2 = terminalRules.PopLast();
                int ruleIndex1 = terminalRules.PopLast();
                
                int nextIndex = getNextFreeRuleIndex();
                
                int key = (ruleIndex1 << 16) | ruleIndex2;
                if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new ArrayList());
                NTtoNT[key].Add(nextIndex);
                terminalRules.AddLast(nextIndex);
            }
            return terminalRules.First.Value;
        }
        
        
        
        // expanding singleton rules, e.g. S -> A, A -> B, B -> C
        public LinkedList<int> collectBackwards(int ruleIndex)
        {
            LinkedList<int> collection = new LinkedList<int>();
            collection.AddLast(ruleIndex);
            LinkedListNode<int> current = collection.First;
            while (current != null)
            {
                int currentIndex = current.Value;
                if (NTtoNT.ContainsKey(currentIndex))
                {
                    foreach (int previousIndex in NTtoNT[currentIndex]) collection.AddLast(previousIndex);
                }
                current = current.Next;
            }
            return collection;
        }
            
            
            
        
        public void raiseEventsRecursive(TreeNode node)
        {
            if (node.fireEvent) parserEventHandler.handleEvent(NTtoRule[node.ruleIndex] + "_pre_event", node);
            
            if (node.terminal == '\0') // node.terminal is != null when node is leaf
            {
                raiseEventsRecursive(node.left);
                if (node.right != null) raiseEventsRecursive(node.right);
            }
                
            if (node.fireEvent) parserEventHandler.handleEvent(NTtoRule[node.ruleIndex] + "_post_event", node);
        }
        
        
                    
        
        public void raiseEvents()
        {
            parserEventHandler.resetLipidBuilder();
            if (parseTree != null) raiseEventsRecursive(parseTree);
        }
        
        
    
        // filling the syntax tree including events
        public void fillTree(TreeNode node, DPNode dpNode)
        {
                    
            if (dpNode.left != null) // null => leaf
            {
                // checking and extending nodes for single rule chains
                int key = (dpNode.ruleIndex1 << 16) | dpNode.ruleIndex2;
                LinkedList<int> mergedRules = collectBackwards(key);
                while (mergedRules.Last.Value != node.ruleIndex) mergedRules.RemoveLast();
                if (mergedRules.Count > 2)
                {
                    mergedRules.RemoveFirst();
                    mergedRules.RemoveLast();
                    
                    LinkedListNode<int> current = mergedRules.Last;
                    while(current != null)
                    {
                        int r = current.Value;
                        current = current.Previous;
                        node.left = new TreeNode(r, NTtoRule.ContainsKey(r));
                        node = node.left;
                    }
                }
            
                node.left = new TreeNode(dpNode.ruleIndex1, NTtoRule.ContainsKey(dpNode.ruleIndex1));
                node.right = new TreeNode(dpNode.ruleIndex2, NTtoRule.ContainsKey(dpNode.ruleIndex2));
                fillTree(node.left, dpNode.left);
                fillTree(node.right, dpNode.right);
            }
            else
            {
                // I know, it is not 100% clean to store the character in an integer
                // especially when it is not the dedicated attribute for, but the heck with it!
                node.terminal = (char)dpNode.ruleIndex1;
            }
        }
        
        
        
        
        // re-implementation of Cocke-Younger-Kasami algorithm
        public void parse(string textToParse)
        {
            wordInGrammer = false;
            parseTree = null;
            int n = textToParse.Length;
            Dictionary<int, DPNode>[][] dpTable = new Dictionary<int, DPNode>[n][]; // dp stands for dynamic programming, nothing else
            for (int i = 0; i < n; ++i)
            {
                dpTable[i] = new Dictionary<int, DPNode>[n - i];
                for (int j = 0; j < n - i; ++j)
                {
                    dpTable[i][j] = new Dictionary<int, DPNode>();
                }
            }
            
            for (int i = 0; i < n; ++i)
            {
                char c = textToParse[i];
                if (!TtoNT.ContainsKey(c)) return;
                foreach (int ruleIndex in TtoNT[c])
                {
                    DPNode dpNode = new DPNode((int)c, 0, null, null);
                    foreach (int previousIndex in collectBackwards(ruleIndex))
                    {
                        dpTable[0][i].Add(previousIndex, dpNode);
                    }
                }
            }        
            
            
            for (int i = 1 ; i < n; ++i)
            {
                for (int j = 0; j < n - i; ++j)
                {
                    for (int k = 0; k < i; ++k)
                    {
                        int k1 = k + 1;
                        foreach (KeyValuePair<int, DPNode> indexPair1 in dpTable[k][j])
                        {
                            foreach (KeyValuePair<int, DPNode> indexPair2 in dpTable[i - k1][j + k1])
                            {
                                int key = (indexPair1.Key << 16) | indexPair2.Key;
                                if (!NTtoNT.ContainsKey(key)) continue;
                                
                                DPNode content = new DPNode(indexPair1.Key, indexPair2.Key, indexPair1.Value, indexPair2.Value);
                                
                                foreach (int ruleIndex in NTtoNT[key])
                                {
                                    foreach (int previousIndex in collectBackwards(ruleIndex))
                                    {   
                                        dpTable[i][j].Add(previousIndex, content);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            if (dpTable[n - 1][0].ContainsKey(0)) // 0 => start rule
            {
                wordInGrammer = true;
                parseTree = new TreeNode(0, NTtoRule.ContainsKey(0));
                fillTree(parseTree, dpTable[n - 1][0][0]);
            }
        }
    }    
}