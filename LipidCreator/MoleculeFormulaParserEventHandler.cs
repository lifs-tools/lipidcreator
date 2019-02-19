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
    public class MoleculeFormulaParserEventHandler : BaseParserEventHandler
    {
        public Dictionary<int, int> elements;
        public int molecule;
        public int count;
    
    
        public MoleculeFormulaParserEventHandler() : base()
        {
            molecule = -1;
            elements = MS2Fragment.createEmptyElementDict();
            
            registeredEvents.Add("Molecule_pre_event", resetParser);
            registeredEvents.Add("Element_Group_post_event", elementGroupPostEvent);
            registeredEvents.Add("Element_pre_event", elementPreEvent);
            registeredEvents.Add("Single_Element_pre_event", singleElementGroupPreEvent);
            registeredEvents.Add("Count_pre_event", countPreEvent);
            
        }
        
        
        public void resetParser(Parser.TreeNode node)
        {
            elements = MS2Fragment.createEmptyElementDict();
        }
        
        
        public void elementGroupPostEvent(Parser.TreeNode node)
        {
            if (elements == null) return;
            elements[molecule] += count;
        }
        
        
        public void elementPreEvent(Parser.TreeNode node)
        {
            
            if (elements == null) return;
            string element = node.getText();
            if (MS2Fragment.ELEMENT_POSITIONS.ContainsKey(element))
            {
                molecule = MS2Fragment.ELEMENT_POSITIONS[element];
            }
            else
            {
                elements = null;
            }
        }
        
        
        
        
        public void singleElementGroupPreEvent(Parser.TreeNode node)
        {
            if (elements == null) return;
            string element = node.getText();
            if (MS2Fragment.ELEMENT_POSITIONS.ContainsKey(element))
            {
                molecule = MS2Fragment.ELEMENT_POSITIONS[element];
                elements[molecule] += 1;
            }
            else
            {
                element = null;
            }
        }
        
        
        
        
        public void countPreEvent(Parser.TreeNode node)
        {
            if (elements == null) return;
            count = Convert.ToInt32(node.getText());
        }
    }    
}