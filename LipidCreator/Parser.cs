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
        
        public int free_number;
        public Dictionary<string, int> rule_to_NT;
        public Dictionary<int, int> T_to_NT;
        public Dictionary<int, int> NT_to_NT;
        public char quote;
        public TreeNode parseTree;
        public bool wordInGrammer;
        public Dictionary<int, string> NT_to_rule;
        //self.events = _events
    
        public Parser(string filename, char _quote)
        {
            free_number = 0;
            rule_to_NT = new Dictionary<string, int>();
            T_to_NT = new Dictionary<int, int>();
            NT_to_NT = new Dictionary<int, int>();
            quote = _quote;
            parseTree = null;
            wordInGrammer = false;
            NT_to_rule = new Dictionary<int, string>();
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
        
        public ArrayList addTerminal(stirng text)
        {
            /*
            text = text.strip(self.quote)
            t_rules = []
            for c in text:
                if c not in self.T_to_NT: self.T_to_NT[c] = []
                self.T_to_NT[c].append(self.free_number)
                t_rules.append(self.free_number)
                self.free_number += 1
            
            while len(t_rules) > 1:
                p2_NF = t_rules.pop()
                p1_NF = t_rules.pop()
                
                n = self.free_number
                self.free_number += 1
                
                if (p1_NF, p2_NF) not in self.NT_to_NT: self.NT_to_NT[(p1_NF, p2_NF)] = []
                self.NT_to_NT[(p1_NF, p2_NF)].append(n)
                
                t_rules.append(n)
            return t_rules[0]
            */
        }
        
        public void parse(string textToParse)
        {
        
        }
    }    
}