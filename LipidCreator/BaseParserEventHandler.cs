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
    public abstract class BaseParserEventHandler
    {
        public Dictionary<string, Action<Parser.TreeNode>> registeredEvents = new Dictionary<string, Action<Parser.TreeNode>>();
        public HashSet<string> ruleNames = new HashSet<string>();
        public Parser parser = null;
    
        public BaseParserEventHandler()
        {
            registeredEvents = new Dictionary<string, Action<Parser.TreeNode>>();
            ruleNames = new HashSet<string>();
        }
        
        
        
        // checking if all registered events are reasonable and orrur as rules in the grammar
        public void sanityCheck()
        {
            foreach (string eventName in registeredEvents.Keys)
            {
                if (!eventName.EndsWith("_pre_event") && !eventName.EndsWith("_post_event"))
                {
                    throw new Exception("Parser event handler error: event '" + eventName + "' does not contain the suffix '_pre_event' or '_post_event'");
                }
                string ruleName = eventName.Replace("_pre_event", "").Replace("_post_event", "");
                if (!ruleNames.Contains(ruleName))
                {
                    throw new Exception("Parser event handler error: rule '" + ruleName + "' in event '" + eventName + "' is not present in the grammar" + (parser != null ? " '" + parser.grammarName + "'" : ""));
                }
            }
        }
        
        
        public void handleEvent(string eventName, Parser.TreeNode node)
        {
            if (registeredEvents.ContainsKey(eventName))
            {
                registeredEvents[eventName](node);
            }
        }
    }    
}
