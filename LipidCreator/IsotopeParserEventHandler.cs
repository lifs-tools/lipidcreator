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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LipidCreator
{    
    
    [Serializable]
    public class IsotopeParserEventHandler : BaseParserEventHandler
    {
        public int heavyIsotope = 0;
        public string heavyElement = "";
        public int heavyCount = 1;
        public ElementDictionary heavyElementCounts = null;
        public ArrayList heavyElementCountList = new ArrayList();
        public string heavyIsotopeLabel = "";
    
    
        public IsotopeParserEventHandler() : base()
        {
            reset(null);
            
            
            registeredEvents.Add("formula_pre_event", reset);
            registeredEvents.Add("isotope_pre_event", resetHeavyIsotope);
            registeredEvents.Add("isotope_post_event", addHeavyIsotope);
            registeredEvents.Add("isotope_number_pre_event", addIsotopeNumber);
            registeredEvents.Add("isotope_element_pre_event", addIsotopeElement);
            registeredEvents.Add("isotope_count_pre_event", addIsotopeCount);
        }
        
        
        
        public void reset(Parser.TreeNode node)
        {
            heavyIsotope = 0;
            heavyElement = "";
            heavyCount = 1;
            if (node != null) heavyIsotopeLabel = node.getText();
            heavyElementCounts = MS2Fragment.createEmptyElementDict();
        }
        
        
        
        
        
        
        
        public void resetHeavyIsotope(Parser.TreeNode node)
        {
            heavyIsotope = 0;
            heavyElement = "";
            heavyCount = 1;
        }
        
        
        
        
        public void addIsotopeNumber(Parser.TreeNode node)
        {
            heavyIsotope = Convert.ToInt32(node.getText());
        }
        
        
        
        
        public void addIsotopeElement(Parser.TreeNode node)
        {
            heavyElement = node.getText();
        }
        
        
        
        
        
        public void addIsotopeCount(Parser.TreeNode node)
        {
            heavyCount = Convert.ToInt32(node.getText());
        }
        
        
        
        
        public void addHeavyIsotope(Parser.TreeNode node)
        {
            if (heavyElementCounts == null || heavyCount < 1)
            {
                heavyElementCounts = null;
                return;
            }
            
            string key = heavyElement + heavyIsotope.ToString();
            if (MS2Fragment.ELEMENT_POSITIONS.ContainsKey(key))
            {
                Molecule m = MS2Fragment.ELEMENT_POSITIONS[key];
                if (heavyElementCounts.ContainsKey(m))
                {
                    heavyElementCounts[m] = heavyCount;
                }
                else
                {
                    heavyElementCounts.Add(m, heavyCount);
                }
            }
            else
            {
                heavyElementCounts = null;
            }
        }
        
    }
}