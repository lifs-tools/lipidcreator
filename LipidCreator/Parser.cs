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
using System.Diagnostics;
using System.Threading.Tasks;

namespace LipidCreator
{    
    
    [Serializable]
    public class Parser
    {
        public class ExtendedLinkedList<T> : LinkedList<T>
        {
            public T PopFirst()
            {
                if (First == null) return default(T);
                T current = First.Value;
                RemoveFirst();
                return current;
            }
            
            public T PopLast()
            {
                if (Last == null) return default(T);
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
        public Dictionary<char, HashSet<int>> TtoNT;
        public Dictionary<int, HashSet<int>> NTtoNT;
        public char quote;
        public TreeNode parseTree;
        public bool wordInGrammer;
        public Dictionary<int, string> NTtoRule;
        public BaseParserEventHandler parserEventHandler;
        public const int SHIFT = 16;
    
    
        public Parser(BaseParserEventHandler _parserEventHandler, string grammerFilename, char _quote = '"')
        {
            nextFreeRuleIndex = 1;
            TtoNT = new Dictionary<char, HashSet<int>>();
            NTtoNT = new Dictionary<int, HashSet<int>>();
            NTtoRule = new Dictionary<int, string>();
            quote = _quote;
            parserEventHandler = _parserEventHandler;
            parseTree = null;
            wordInGrammer = false;
            
            
            if (File.Exists(grammerFilename))
            {
                int lineCounter = 0;
                
                Dictionary<string, int> ruleToNT = new Dictionary<string, int>();
                using (StreamReader sr = new StreamReader(grammerFilename))
                {
                    string line;
                    while((line = sr.ReadLine()) != null)
                    {
                        lineCounter++;
                        // skip empty lines and comments
                        if (line.Length < 1) continue;
                        line = strip(line, ' ');
                        if (line[0] == '#') continue;
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
                        
                        if (!NTtoRule.ContainsKey(newRuleIndex)) NTtoRule.Add(newRuleIndex, rule);
                        
                        
                        foreach (string product in products)
                        {
                            LinkedList<string> nonTerminals = new LinkedList<string>();
                            ExtendedLinkedList<int> nonTerminalRules = new ExtendedLinkedList<int>();
                            foreach (string NT in splitString(product, ' ', quote))
                            {
                                string stripedNT = strip(NT, ' ');
                                if (stripedNT[0] == '#') break;
                                nonTerminals.AddLast(stripedNT);
                            }
                            
                            string NTFirst = nonTerminals.First.Value;
                            if (nonTerminals.Count > 1 || !isTerminal(NTFirst, quote) || NTFirst.Length != 3)
                            {
                            
                                foreach (string nonTerminal in nonTerminals)
                                {
                                    if (isTerminal(nonTerminal, quote))
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
                            }
                            else
                            {
                                char c = NTFirst[1];
                                if (!TtoNT.ContainsKey(c)) TtoNT[c] = new HashSet<int>();
                                TtoNT[c].Add(newRuleIndex);
                            }
                            
                            
                            // more than two rules, insert intermediate rule indexes
                            while (nonTerminalRules.Count > 2)
                            {
                                int ruleIndex2 = nonTerminalRules.PopLast();
                                int ruleIndex1 = nonTerminalRules.PopLast();
                                
                                int key = computeRuleKey(ruleIndex1, ruleIndex2);
                                int nextIndex = getNextFreeRuleIndex();
                                if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new HashSet<int>());
                                NTtoNT[key].Add(nextIndex);
                                nonTerminalRules.AddLast(nextIndex);
                            }    
                            
                                
                            // two product rules
                            if (nonTerminalRules.Count == 2)
                            {
                                int ruleIndex1 = nonTerminalRules.PopFirst();
                                int ruleIndex2 = nonTerminalRules.PopFirst();
                                int key = computeRuleKey(ruleIndex1, ruleIndex2);
                                if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new HashSet<int>());
                                NTtoNT[key].Add(newRuleIndex);
                                
                            }
                            // only one product rule
                            else if (nonTerminalRules.Count == 1)
                            {
                                int ruleIndex1 = nonTerminalRules.First.Value;
                                if (ruleIndex1 == newRuleIndex) throw new Exception("Error: corrupted token in grammer: rule '" + rule + "' is not allowed to refer soleley to itself.");
                                
                                if (!NTtoNT.ContainsKey(ruleIndex1)) NTtoNT.Add(ruleIndex1, new HashSet<int>());
                                NTtoNT[ruleIndex1].Add(newRuleIndex);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Error: file '" + grammerFilename + "' does not exist or can not be opened.");
            }
            
            
            HashSet<char> keys = new HashSet<char>(TtoNT.Keys);
            foreach(char c in keys)
            {
                HashSet<int> rules = new HashSet<int>(TtoNT[c]);
                TtoNT[c].Clear();
                foreach(int rule in rules)
                {
                    foreach (int p in collectBackwards(rule))
                    {
                        int key = computeRuleKey(p, rule);
                        TtoNT[c].Add(key);
                    }
                }
            }
            
            
            HashSet<int> keysNT = new HashSet<int>(NTtoNT.Keys);
            foreach(int r in keysNT)
            {
                HashSet<int> rules = new HashSet<int>(NTtoNT[r]);
                foreach(int rule in rules)
                {
                    foreach (int p in collectBackwards(rule)) NTtoNT[r].Add(p);
                }
            }
        }
        
        
        public int getNextFreeRuleIndex()
        {
            if (nextFreeRuleIndex < 65536) return nextFreeRuleIndex++;
            throw new Exception("Error: grammer is too big.");
        }
        
        
        
        public int computeRuleKey(int ruleIndex1, int ruleIndex2)
        {
            return (ruleIndex1 << SHIFT) | ruleIndex2;
        }
        
        
        public static ArrayList splitString(string text, char separator, char quote)
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
        
        
        public static string strip(string text, char stripChar)
        {
            while (text.Length > 0 && text[0] == stripChar) text = text.Substring(1, text.Length - 1);
            while (text.Length > 0 && text[text.Length - 1] == stripChar) text = text.Substring(0, text.Length - 1);
            return text;
        }
        
        
        public static bool isTerminal(string productToken, char quote)
        {
            string[] tks = productToken.Split(quote);
            if (tks.Length != 1 && tks.Length != 3) throw new Exception("Error: corrupted token in grammer");
            
            if (tks.Length == 1) return false;
        
            if (productToken[0] == quote && productToken[productToken.Length - 1] == quote && productToken.Length > 2) return true;

            throw new Exception("Error: corrupted token in grammer");
        }
        
        
        public int addTerminal(string text)
        {
            text = strip(text, quote);
            ExtendedLinkedList<int> terminalRules = new ExtendedLinkedList<int>();
            foreach (char c in text)
            {
                if (!TtoNT.ContainsKey(c)) TtoNT.Add(c, new HashSet<int>());
                int nextIndex = getNextFreeRuleIndex();
                TtoNT[c].Add(nextIndex);
                terminalRules.AddLast(nextIndex);
            }
            while (terminalRules.Count > 1)
            {
                int ruleIndex2 = terminalRules.PopLast();
                int ruleIndex1 = terminalRules.PopLast();
                
                int nextIndex = getNextFreeRuleIndex();
                
                int key = computeRuleKey(ruleIndex1, ruleIndex2);
                if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new HashSet<int>());
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
        
        
        
        
        
        
        public LinkedList<int> collectBackwards(int childRuleIndex, int parentRuleIndex)
        {
            if (!NTtoNT.ContainsKey(childRuleIndex)) return null;
            LinkedList<int> collection;
            
            foreach (int previousIndex in NTtoNT[childRuleIndex])
            {
                if (previousIndex == parentRuleIndex)
                {
                    collection = new LinkedList<int>();
                    return collection;
                }
                else if (NTtoNT.ContainsKey(previousIndex))
                {
                    collection = collectBackwards(previousIndex, parentRuleIndex);
                    if (collection != null)
                    {
                        collection.AddLast(previousIndex);
                        return collection;
                    }
                }
            }
            return null;
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
            if (parseTree != null) raiseEventsRecursive(parseTree);
        }
        
        
    
        // filling the syntax tree including events
        public void fillTree(TreeNode node, DPNode dpNode)
        {
           
            // checking and extending nodes for single rule chains
            int key = (dpNode.left != null) ? computeRuleKey(dpNode.ruleIndex1, dpNode.ruleIndex2) : dpNode.ruleIndex2;
            LinkedList<int> mergedRules = collectBackwards(key, node.ruleIndex);
            if (mergedRules != null)
            {
                foreach (int ruleIndex in mergedRules)
                {
                    node.left = new TreeNode(ruleIndex, NTtoRule.ContainsKey(ruleIndex));
                    node = node.left;
                }
            }
            
            
            if (dpNode.left != null) // null => leaf
            {
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
            // dp stands for dynamic programming, nothing else
            Dictionary<int, DPNode>[][] dpTable = new Dictionary<int, DPNode>[n][];
            
            
            long[] lookupLeftKey = new long[1 + ((nextFreeRuleIndex + 1) >> 7)];
            for (int i = 0; i < lookupLeftKey.Length; ++i) lookupLeftKey[i] = 0L;
            foreach(int rule in NTtoNT.Keys)
            {
                int key1 = rule >> SHIFT;
                lookupLeftKey[key1 >> 7] |= 1L << (key1 & 63);
            }
            
            
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();
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
                    int newKey = ruleIndex >> SHIFT;
                    int oldKey = ruleIndex & 65535;
                    DPNode dpNode = new DPNode((int)c, oldKey, null, null);
                    dpTable[i][0][newKey] =  dpNode;
                }
            }
            
            for (int i = 1 ; i < n; ++i)
            {
                
                for (int j = 0; j < n - i; ++j)
                {
                    Dictionary<int, DPNode>[] D = dpTable[j];
                    Dictionary<int, DPNode> Di = D[i];
                    int jp1 = j + 1;
                    int im1 = i - 1;
                    for (int k = 0; k < i; ++k)
                    {   
                        if (D[k].Count == 0 || dpTable[jp1 + k][im1 - k].Count == 0) continue;
                        
                        foreach (KeyValuePair<int, DPNode> indexPair1 in D[k])
                        {
                            if (((lookupLeftKey[indexPair1.Key >> 7] >> (indexPair1.Key & 63)) & 1) != 1) continue;
                            
                            foreach (KeyValuePair<int, DPNode> indexPair2 in dpTable[jp1 + k][im1 - k])
                            {
                                int key = computeRuleKey(indexPair1.Key, indexPair2.Key);
                                if (!NTtoNT.ContainsKey(key)) continue;
                                
                                DPNode content = new DPNode(indexPair1.Key, indexPair2.Key, indexPair1.Value, indexPair2.Value);
                                foreach (int ruleIndex in NTtoNT[key])
                                {
                                    Di[ruleIndex] = content;
                                }
                            }
                        }
                    }
                }
            }
            //stopWatch.Stop();
            //Console.WriteLine(stopWatch.Elapsed);
            
            
            
            
            
            
            if (dpTable[0][n - 1].ContainsKey(1)) // 0 => start rule
            {
                wordInGrammer = true;
                parseTree = new TreeNode(1, NTtoRule.ContainsKey(1));
                fillTree(parseTree, dpTable[0][n - 1][1]);
            }
            
        }
    }    
}