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
SOFTWARE. IF YOU VIOLATE THE COPYRIGHT, SMALL YELLOW CAPSULE-SHAPED CREATURES
WITH BLUE BIBS WILL COME AND SLAP YOU WITH BANANAS.
*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using log4net;
using log4net.Config;

namespace LipidCreator
{    
    
    public enum Context {NoContext, InLineComment, InLongComment, InQuote};
    public enum MatchWords {NoMatch, LineCommentStart, LineCommentEnd, LongCommentStart, LongCommentEnd, Quote};
    
    [Serializable]
    public class Parser
    {
        [Serializable]
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
            public long ruleIndex1;
            public long ruleIndex2;
            public DPNode left;
            public DPNode right;
            
            public DPNode(long _rule1, long _rule2, DPNode _left, DPNode _right)
            {
                ruleIndex1 = _rule1;
                ruleIndex2 = _rule2;
                left = _left;
                right = _right;
            }
        }
        
        
        
        
        // this class is dedicated to have an efficient sorted set class storing
        // values within 0..n-1 and fast sequencial iterator
        public class Bitfield
        {
            public ulong[] field;
            public ulong[] superfield;
            static readonly ulong multiplicator = 0x022fdd63cc95386dUL;
            static readonly public int[] positions = new int[64] // from http://chessprogramming.wikispaces.com/De+Bruijn+Sequence+Generator
            { 
                0, 1,  2, 53,  3,  7, 54, 27, 4, 38, 41,  8, 34, 55, 48, 28,
                62,  5, 39, 46, 44, 42, 22,  9, 24, 35, 59, 56, 49, 18, 29, 11,
                63, 52,  6, 26, 37, 40, 33, 47, 61, 45, 43, 21, 23, 58, 17, 10,
                51, 25, 36, 32, 60, 20, 57, 16, 50, 31, 19, 15, 30, 14, 13, 12
            };
            
            public Bitfield(int length)
            {
                int l = 1 + ((length + 1) >> 6);
                int s = 1 + ((l + 1) >> 6);
                field = new ulong[l];
                superfield = new ulong[s];
                for (int i = 0; i < l; ++i) field[i] = 0;
                for (int i = 0; i < s; ++i) superfield[i] = 0;
            }
            
            public void set(int pos)
            {
                field[pos >> 6] |= (ulong)(1UL << (pos & 63));
                superfield[pos >> 12] |= (ulong)(1UL << ((pos >> 6) & 63));
            }
            
            
            public bool isSet(int pos)
            {
                return ((field[pos >> 6] >> (pos & 63)) & 1UL) == 1UL;
            }
            
            
            public bool isNotSet(int pos)
            {
                return ((field[pos >> 6] >> (pos & 63)) & 1UL) == 0UL;
            }
            
            
            public System.Collections.Generic.IEnumerable<int> getBitPositions()
            {
                int spre = 0;
                foreach (ulong cell in superfield)
                {
                    ulong sv = cell;
                    while (sv != 0)
                    {
                        // algorithm for getting least significant bit position
                        ulong sv1 = (ulong)((long)sv & -(long)sv);
                        int pos = spre + positions[(ulong)(sv1 * multiplicator) >> 58];
                        
                        ulong v = field[pos];
                        while (v != 0)
                        {
                            // algorithm for getting least significant bit position
                            ulong v1 = (ulong)((long)v & -(long)v);
                            yield return unchecked(pos << 6) + positions[unchecked((v1 * multiplicator) >> 58)];
                            v &= v - 1;
                        }
                        
                        sv &= sv - 1;
                    }
                    spre += 64;
                }
            }
            
            
            public static System.Collections.Generic.IEnumerable<int> getPositions(long x)
            {
                while (x != 0)
                {
                    // algorithm for getting least significant bit position
                    ulong v1 = (ulong)((long)x & -(long)x);
                    yield return positions[unchecked((v1 * multiplicator) >> 58)];
                    x &= x - 1;
                }
            }
        }
        
        
        
        
    
        public class TreeNode
        {
            public long ruleIndex;
            public TreeNode left;
            public TreeNode right;
            public char terminal;
            public bool fireEvent;
            
            public TreeNode(long _rule, bool _fireEvent)
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
        
        
        
        public long nextFreeRuleIndex;
        public Dictionary<char, HashSet<long>> TtoNT;
        public Dictionary<long, HashSet<long>> NTtoNT;
        public char quote;
        [NonSerialized]
        public TreeNode parseTree;
        public bool wordInGrammar;
        public Dictionary<long, string> NTtoRule;
        public BaseParserEventHandler parserEventHandler;
        public const int SHIFT = 32;
        public const long MASK = (1L << SHIFT) - 1;
        public const char RULE_ASSIGNMENT = ':';
        public const char RULE_SEPARATOR = '|';
        public const char RULE_TERMINAL = ';';
        public const string EOF_RULE_NAME = "EOF";
        public const char EOF_SIGN = '\0';
        public const long EOF_RULE = 1;
        public const long START_RULE = 2;
        public bool usedEOF = false;
        public string grammarName = "";
        private static readonly ILog log = LogManager.GetLogger(typeof(Parser));
    
    
        public Parser(BaseParserEventHandler _parserEventHandler, string grammarFilename, char _quote = '"')
        {
            nextFreeRuleIndex = START_RULE;
            TtoNT = new Dictionary<char, HashSet<long>>();
            NTtoNT = new Dictionary<long, HashSet<long>>();
            NTtoRule = new Dictionary<long, string>();
            quote = _quote;
            parserEventHandler = _parserEventHandler;
            parseTree = null;
            wordInGrammar = false;
            
            
            
            if (File.Exists(grammarFilename))
            {
                // interpret the rules and create the structure for parsing
                ArrayList rules = Parser.extractTextBasedRules(grammarFilename, quote);
                grammarName = (string)splitString((string)rules[0], ' ', quote)[1];
                rules.RemoveAt(0);
                Dictionary<string, long> ruleToNT = new Dictionary<string, long>();
                ruleToNT.Add(EOF_RULE_NAME, EOF_RULE);
                TtoNT.Add(EOF_SIGN, new HashSet<long>(){EOF_RULE});
                foreach (string ruleLine in rules)
                {
                
                    ArrayList tokens_level_1 = new ArrayList();
                    foreach (string t in splitString(ruleLine, RULE_ASSIGNMENT, quote)) tokens_level_1.Add(strip(t, ' '));
                    if (tokens_level_1.Count != 2) throw new Exception("Error: corrupted token in grammar rule: '" + ruleLine + "'");
                    
                    if (splitString((string)tokens_level_1[0], ' ', quote).Count > 1)
                    {
                        log.Error("Error: several rule names on left hand side in grammar rule: '" + ruleLine + "'");
                        throw new Exception("Error: several rule names on left hand side in grammar rule: '" + ruleLine + "'");
                    }

                    string rule = (string)tokens_level_1[0];
                    
                    if (rule.Equals(EOF_RULE_NAME))
                    {
                        log.Error("Error: rule name is not allowed to be called EOF");
                        throw new Exception("Error: rule name is not allowed to be called EOF");
                    }
                    
                    ArrayList products = new ArrayList();
                    foreach (string p in splitString((string)tokens_level_1[1], RULE_SEPARATOR, quote)) products.Add(strip(p, ' '));
                    
                    if (!ruleToNT.ContainsKey(rule)) ruleToNT.Add(rule, getNextFreeRuleIndex());
                    long newRuleIndex = ruleToNT[rule];
                    
                    if (!NTtoRule.ContainsKey(newRuleIndex)) NTtoRule.Add(newRuleIndex, rule);
                    
                    
                    foreach (string product in products)
                    {
                        LinkedList<string> nonTerminals = new LinkedList<string>();
                        ExtendedLinkedList<long> nonTerminalRules = new ExtendedLinkedList<long>();
                        foreach (string NT in splitString(product, ' ', quote))
                        {
                            string stripedNT = strip(NT, ' ');
                            if (isTerminal(stripedNT, quote)) stripedNT = deEscape(stripedNT, quote);
                            nonTerminals.AddLast(stripedNT);
                            usedEOF |= stripedNT.Equals(EOF_RULE_NAME);
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
                            if (!TtoNT.ContainsKey(c)) TtoNT[c] = new HashSet<long>();
                            TtoNT[c].Add(newRuleIndex);
                        }
                        
                        
                        // more than two rules, insert intermediate rule indexes
                        while (nonTerminalRules.Count > 2)
                        {
                            long ruleIndex2 = nonTerminalRules.PopLast();
                            long ruleIndex1 = nonTerminalRules.PopLast();
                            
                            long key = computeRuleKey(ruleIndex1, ruleIndex2);
                            long nextIndex = getNextFreeRuleIndex();
                            if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new HashSet<long>());
                            NTtoNT[key].Add(nextIndex);
                            nonTerminalRules.AddLast(nextIndex);
                        }    
                        
                            
                        // two product rules
                        if (nonTerminalRules.Count == 2)
                        {
                            long ruleIndex1 = nonTerminalRules.PopFirst();
                            long ruleIndex2 = nonTerminalRules.PopFirst();
                            long key = computeRuleKey(ruleIndex1, ruleIndex2);
                            if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new HashSet<long>());
                            NTtoNT[key].Add(newRuleIndex);
                        }
                        // only one product rule
                        else if (nonTerminalRules.Count == 1)
                        {
                            long ruleIndex1 = nonTerminalRules.First.Value;
                            if (ruleIndex1 == newRuleIndex)
                            {
                                log.Error("Error: corrupted token in grammar: rule '" + rule + "' is not allowed to refer soleley to itself.");
                                throw new Exception("Error: corrupted token in grammar: rule '" + rule + "' is not allowed to refer soleley to itself.");
                            }
                            
                            if (!NTtoNT.ContainsKey(ruleIndex1)) NTtoNT.Add(ruleIndex1, new HashSet<long>());
                            NTtoNT[ruleIndex1].Add(newRuleIndex);
                        }
                    }
                }
                
                // adding all rule names into the event handler
                foreach (string ruleName in ruleToNT.Keys)
                {
                    parserEventHandler.ruleNames.Add(ruleName);
                }
                parserEventHandler.parser = this;
                parserEventHandler.sanityCheck();
            }
            else
            {
                log.Error("Error: file '" + grammarFilename + "' does not exist or can not be opened.");
                throw new Exception("Error: file '" + grammarFilename + "' does not exist or can not be opened.");
            }
            
            
            
            HashSet<char> keys = new HashSet<char>(TtoNT.Keys);
            foreach(char c in keys)
            {
                HashSet<long> rules = new HashSet<long>(TtoNT[c]);
                TtoNT[c].Clear();
                foreach(long rule in rules)
                {
                    foreach (long p in collectBackwards(rule))
                    {
                        long key = computeRuleKey(p, rule);
                        TtoNT[c].Add(key);
                    }
                }
            }
            
            
            HashSet<long> keysNT = new HashSet<long>(NTtoNT.Keys);
            foreach(long r in keysNT)
            {
                HashSet<long> rules = new HashSet<long>(NTtoNT[r]);
                foreach(long rule in rules)
                {
                    foreach (long p in collectBackwards(rule)) NTtoNT[r].Add(p);
                }
            }
        }
        
        
        
        
        
        public static ArrayList extractTextBasedRules(string grammarFilename, char quote)
        {
            string grammar = File.ReadAllText(grammarFilename) + "\n";
            int grammarLength = grammar.Length;
            
            // deleting comments to prepare for splitting the grammar in rules.
            // Therefore, we have to consider three different contexts, namely
            // within a quote, within a line comment, within a long comment.
            // As long as we are in one context, key words for starting / ending
            // the other contexts have to be ignored.
            StringBuilder sb = new StringBuilder();
            Context currentContext = Context.NoContext;
            int currentPosition = 0;
            int lastEscapedBackslash = -1;
            for (int i = 0; i < grammarLength - 1; ++i)
            {
                MatchWords match = MatchWords.NoMatch;
                
                if (i > 0 && grammar[i] == '\\' && grammar[i - 1] == '\\' && lastEscapedBackslash != i - 1)
                {
                    lastEscapedBackslash = i;
                    continue;
                }
                if (grammar[i] == '/' && grammar[i + 1] == '/') match = MatchWords.LineCommentStart;
                else if (grammar[i] == '\n') match = MatchWords.LineCommentEnd;
                else if (grammar[i] == '/' && grammar[i + 1] == '*') match = MatchWords.LongCommentStart;
                else if (grammar[i] == '*' && grammar[i + 1] == '/') match = MatchWords.LongCommentEnd;
                else if (grammar[i] == quote && !(i >= 1 && grammar[i - 1] == '\\' && i - 1 != lastEscapedBackslash)) match = MatchWords.Quote;
                
                if (match != MatchWords.NoMatch)
                {
                    switch (currentContext)
                    {
                        case Context.NoContext:
                            switch (match)
                            {
                                case MatchWords.LongCommentStart:
                                    sb.Append(grammar.Substring(currentPosition, i - currentPosition));
                                    currentContext = Context.InLongComment;
                                    break;
                                    
                                case MatchWords.LineCommentStart:
                                    sb.Append(grammar.Substring(currentPosition, i - currentPosition));
                                    currentContext = Context.InLineComment;
                                    break;
                                    
                                case MatchWords.Quote:
                                    currentContext = Context.InQuote;
                                    break;
                                    
                                default:
                                    break;
                            } 
                            break;
                    
                            
                        case Context.InQuote:
                            if (match == MatchWords.Quote)
                            {
                                currentContext = Context.NoContext;
                            }
                            break;
                            
                            
                        case Context.InLineComment:
                            if (match == MatchWords.LineCommentEnd)
                            {
                                currentContext = Context.NoContext;
                                currentPosition = i + 1;
                            }
                            break;
                            
                        case Context.InLongComment:
                            if (match == MatchWords.LongCommentEnd)
                            {
                                currentContext = Context.NoContext;
                                currentPosition = i + 2;
                            }
                            break;
                    }
                }
            }
            if (currentContext == Context.NoContext)
            {
                sb.Append(grammar.Substring(currentPosition, grammarLength - currentPosition));
            }
            else
            {
                log.Error("Error: corrupted grammar '" + grammarFilename + "', ends either in comment or quote");
                throw new Exception("Error: corrupted grammar '" + grammarFilename + "', ends either in comment or quote");
            }
            grammar = strip(sb.ToString().Replace("\r\n", "").Replace("\n", "").Replace("\r", ""), ' ');
            if (grammar[grammar.Length - 1] != RULE_TERMINAL)
            {
                log.Error("Error: corrupted grammar'" + grammarFilename + "', last rule has no termininating sign, was: '" + grammar[grammar.Length-1] + "'");
                throw new Exception("Error: corrupted grammar'" + grammarFilename + "', last rule has no termininating sign, was: '" + grammar[grammar.Length-1] + "'");
            }
            ArrayList rules = splitString(grammar, RULE_TERMINAL, quote);
            
            if (rules.Count < 1)
            {
                log.Error("Error: corrupted grammar'" + grammarFilename + "', grammar is empty");
                throw new Exception("Error: corrupted grammar'" + grammarFilename + "', grammar is empty");
            }
            
            ArrayList grammarNameRule = splitString((string)rules[0], ' ', quote);
            if (!grammarNameRule[0].Equals("grammar"))
            {
                log.Error("Error: first rule must start with the keyword 'grammar'");
                throw new Exception("Error: first rule must start with the keyword 'grammar'");
            }
            else if (grammarNameRule.Count != 2)
            {
                log.Error("Error: incorrect first rule");
                throw new Exception("Error: incorrect first rule");
            }
            
            return rules;
        }
        
        
        
        
        
        public long getNextFreeRuleIndex()
        {
            if (nextFreeRuleIndex <= MASK) return nextFreeRuleIndex++;
            throw new Exception("Error: grammar is too big.");
        }
        
        
        
        
        public long computeRuleKey(long ruleIndex1, long ruleIndex2)
        {
            return (ruleIndex1 << SHIFT) | ruleIndex2;
        }
        
        
        
        
        
        
        public static ArrayList splitString(string text, char separator, char quote)
        {
            bool inQuote = false;
            ArrayList tokens = new ArrayList();
            StringBuilder sb = new StringBuilder();
            int lastChar = '\0';
            bool lastEscapedBackslash = false;
            
            foreach (char c in text)
            {
                bool escapedBackslash = false;
                if (!inQuote)
                {
                    if (c == separator)
                    {
                        if (sb.Length > 0)
                        {
                            tokens.Add(sb.ToString());
                        }
                        sb.Clear();
                    }
                    else
                    {
                        if (c == quote) inQuote = !inQuote;
                        sb.Append(c);
                    }
                }
                else
                {
                    if (c == '\\' && lastChar == '\\' && !lastEscapedBackslash)
                    {
                        escapedBackslash = true;
                    }
                    else if (c == quote && !(lastChar == '\\' && !lastEscapedBackslash))
                    {
                        inQuote = !inQuote;
                    }
                    sb.Append(c);
                }
                lastEscapedBackslash = escapedBackslash;
                lastChar = c;
            }
                    
            if (sb.Length > 0)
            {
                tokens.Add(sb.ToString());
            }
            if (inQuote) throw new Exception("Error: corrupted token in grammar");
            
            return tokens;
        }
        
        
        
        
        public static string strip(string text, char stripChar)
        {
            if (text.Length > 0)
            {
                int st = 0;
                while (st < text.Length - 1 && text[st] == stripChar) ++st;
                text = text.Substring(st, text.Length - st);
            }
            
            if (text.Length > 0)
            {
                int en = 0;
                while (en < text.Length - 1 && text[text.Length - 1 - en] == stripChar) ++en;
                text = text.Substring(0, text.Length - en);
            }
            return text;
        }
        
        
        
        
        // checking if string is terminal
        public static bool isTerminal(string productToken, char quote)
        {
            return (productToken[0] == quote && productToken[productToken.Length - 1] == quote && productToken.Length > 2);
        }
        
        
        
        
        public static string deEscape(string text, char quote)
        {
            // remove the escape chars
            StringBuilder sb = new StringBuilder();
            bool lastEscapeChar = false;
            foreach (char c in text)
            {
                bool escapeChar = false;
                
                if (c != '\\')
                {
                    sb.Append(c);
                }
                else
                {
                    if (!lastEscapeChar) escapeChar = true;
                    else sb.Append(c);
                } 
                
                lastEscapeChar = escapeChar;
            }
            return sb.ToString();
        }
        
        
        
        // splitting the whole terminal in a tree structure where characters of terminal are the leafs and the inner nodes are added non terminal rules
        public long addTerminal(string text)
        {
            ExtendedLinkedList<long> terminalRules = new ExtendedLinkedList<long>();
            for (int i = 1; i < text.Length - 1; ++i)
            {
                char c = text[i];
                if (!TtoNT.ContainsKey(c)) TtoNT.Add(c, new HashSet<long>());
                long nextIndex = getNextFreeRuleIndex();
                TtoNT[c].Add(nextIndex);
                terminalRules.AddLast(nextIndex);
            }
            while (terminalRules.Count > 1)
            {
                long ruleIndex2 = terminalRules.PopLast();
                long ruleIndex1 = terminalRules.PopLast();
                
                long nextIndex = getNextFreeRuleIndex();
                
                long key = computeRuleKey(ruleIndex1, ruleIndex2);
                if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new HashSet<long>());
                NTtoNT[key].Add(nextIndex);
                terminalRules.AddLast(nextIndex);
            }
            return terminalRules.First.Value;
        }
        
        
        
        // expanding singleton rules, e.g. S -> A, A -> B, B -> C
        public LinkedList<long> collectBackwards(long ruleIndex)
        {
            LinkedList<long> collection = new LinkedList<long>();
            collection.AddLast(ruleIndex);
            LinkedListNode<long> current = collection.First;
            while (current != null)
            {
                long currentIndex = current.Value;
                if (NTtoNT.ContainsKey(currentIndex))
                {
                    foreach (long previousIndex in NTtoNT[currentIndex]) collection.AddLast(previousIndex);
                }
                current = current.Next;
            }
            return collection;
        }
        
        
        
        
        
        public LinkedList<long> collectBackwards(long childRuleIndex, long parentRuleIndex)
        {
            if (!NTtoNT.ContainsKey(childRuleIndex)) return null;
            LinkedList<long> collection;
            
            foreach (long previousIndex in NTtoNT[childRuleIndex])
            {
                if (previousIndex == parentRuleIndex)
                {
                    collection = new LinkedList<long>();
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
            
            
            
            
        
        public void raiseEvents(TreeNode node = null)
        {
            if (node != null)
            {
                string nodeRuleName = node.fireEvent ? NTtoRule[node.ruleIndex] : "";
                if (node.fireEvent) parserEventHandler.handleEvent(nodeRuleName + "_pre_event", node);
                
                if (node.left != null) // node.terminal is != null when node is leaf
                {
                    raiseEvents(node.left);
                    if (node.right != null) raiseEvents(node.right);
                }
                    
                if (node.fireEvent) parserEventHandler.handleEvent(nodeRuleName + "_post_event", node);
            }
            else
            {
                if (parseTree != null) raiseEvents(parseTree);
            }
        }
        
        
    
        // filling the syntax tree including events
        public void fillTree(TreeNode node, DPNode dpNode)
        {
           
            // checking and extending nodes for single rule chains
            long key = (dpNode.left != null) ? computeRuleKey(dpNode.ruleIndex1, dpNode.ruleIndex2) : dpNode.ruleIndex2;
            LinkedList<long> mergedRules = collectBackwards(key, node.ruleIndex);
            if (mergedRules != null)
            {
                foreach (long ruleIndex in mergedRules)
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
            if (usedEOF) textToParse += EOF_SIGN;
            
            if (textToParse.Length < 64) parse64(textToParse);
            else parseRegular(textToParse);
        }
            
            
            
            
            
            
            
        public void parse64(string textToParse)
        {
            wordInGrammar = false;
            parseTree = null;
            int n = textToParse.Length;
            // dp stands for dynamic programming, nothing else
            Dictionary<long, DPNode>[][] dpTable = new Dictionary<long, DPNode>[n][];
            // V (vertical) and Diag (diagonal) are lookups, which fields in the dpTable are filled
            long[] V = new long[n];
            long[] Diag = new long[n];
            
            
            
            for (int i = 0; i < n; ++i)
            {
                dpTable[i] = new Dictionary<long, DPNode>[n - i];
                for (int j = 0; j < n - i; ++j) dpTable[i][j] = new Dictionary<long, DPNode>();
                V[i] = 0;
                Diag[i] = 0;
            }
            
            for (int i = 0; i < n; ++i)
            {
                char c = textToParse[i];
                if (!TtoNT.ContainsKey(c)) return;
                
                foreach (long ruleIndex in TtoNT[c])
                {
                    long newKey = ruleIndex >> SHIFT;
                    long oldKey = ruleIndex & MASK;
                    DPNode dpNode = new DPNode((long)c, oldKey, null, null);
                    dpTable[i][0][newKey] =  dpNode;
                    V[i] = 1;
                    Diag[i] = 1L << i;
                }
            }
            
            for (int i = 1; i < n; ++i)
            {
                int im1 = i - 1;
                for (int j = 0; j < n - i; ++j)
                {
                    Dictionary<long, DPNode>[] D = dpTable[j];
                    Dictionary<long, DPNode> Di = D[i];
                    int jp1 = j + 1;
                    
                    // checking if at least one k is valid
                    long intersect = (V[j] & unchecked((1L << i) - 1L)) & unchecked(Diag[i + j] >> jp1);
                    if (intersect == 0) continue;
                    
                    foreach(int k in Bitfield.getPositions(intersect))
                    {
                        foreach (KeyValuePair<long, DPNode> indexPair1 in D[k])
                        {
                            foreach (KeyValuePair<long, DPNode> indexPair2 in dpTable[jp1 + k][im1 - k])
                            {
                                long key = computeRuleKey(indexPair1.Key, indexPair2.Key);
                                if (!NTtoNT.ContainsKey(key)) continue;
                                
                                DPNode content = new DPNode(indexPair1.Key, indexPair2.Key, indexPair1.Value, indexPair2.Value);
                                V[j] |= unchecked(1L << i);
                                Diag[i + j] |= unchecked(1L << j);
                                foreach (long ruleIndex in NTtoNT[key])
                                {
                                    Di[ruleIndex] = content;
                                }
                            }
                        }
                    }
                }
            }
            
            for (int i = n - 1; i > 0; --i){
                if (dpTable[0][i].ContainsKey(START_RULE))
                {
                    wordInGrammar = true;
                    parseTree = new TreeNode(START_RULE, NTtoRule.ContainsKey(START_RULE));
                    fillTree(parseTree, dpTable[0][i][START_RULE]);
                    break;
                }
            }
        }
            
            
            
            
            
        public void parseRegular(string textToParse)
        {
        
        
            wordInGrammar = false;
            parseTree = null;
            int n = textToParse.Length;
            // dp stands for dynamic programming, nothing else
            Dictionary<long, DPNode>[][] dpTable = new Dictionary<long, DPNode>[n][];
            // Ks is a lookup, which fields in the dpTable are filled
            Bitfield[] Ks = new Bitfield[n];            
            
            for (int i = 0; i < n; ++i)
            {
                dpTable[i] = new Dictionary<long, DPNode>[n - i];
                Ks[i] = new Bitfield(n - 1);
                for (int j = 0; j < n - i; ++j) dpTable[i][j] = new Dictionary<long, DPNode>();
            }
            
            for (int i = 0; i < n; ++i)
            {
                char c = textToParse[i];
                if (!TtoNT.ContainsKey(c)) return;
                
                foreach (long ruleIndex in TtoNT[c])
                {
                    long newKey = ruleIndex >> SHIFT;
                    long oldKey = ruleIndex & MASK;
                    DPNode dpNode = new DPNode((long)c, oldKey, null, null);
                    dpTable[i][0][newKey] =  dpNode;
                    Ks[i].set(0);
                }
            }
            
            for (int i = 1; i < n; ++i)
            {
                int im1 = i - 1;
                for (int j = 0; j < n - i; ++j)
                {
                    Dictionary<long, DPNode>[] D = dpTable[j];
                    Dictionary<long, DPNode> Di = D[i];
                    int jp1 = j + 1;
                    
                    foreach(int k in Ks[j].getBitPositions())
                    {
                        if (k >= i) break;
                        if (Ks[jp1 + k].isNotSet(im1 - k)) continue;
                        
                        foreach (KeyValuePair<long, DPNode> indexPair1 in D[k])
                        {
                            foreach (KeyValuePair<long, DPNode> indexPair2 in dpTable[jp1 + k][im1 - k])
                            {
                                long key = computeRuleKey(indexPair1.Key, indexPair2.Key);
                                if (!NTtoNT.ContainsKey(key)) continue;
                                
                                DPNode content = new DPNode(indexPair1.Key, indexPair2.Key, indexPair1.Value, indexPair2.Value);
                                Ks[j].set(i);
                                foreach (long ruleIndex in NTtoNT[key])
                                {
                                    Di[ruleIndex] = content;
                                }
                            }
                        }
                    }
                }
            }
            
            for (int i = n - 1; i > 0; --i){
                if (dpTable[0][i].ContainsKey(START_RULE))
                {
                    wordInGrammar = true;
                    parseTree = new TreeNode(START_RULE, NTtoRule.ContainsKey(START_RULE));
                    fillTree(parseTree, dpTable[0][i][START_RULE]);
                    break;
                }
            }
        }
    }    
}
