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
    public class ListingParserEventHandler : BaseParserEventHandler
    {
        public HashSet<int> counts;
        public int oddEven;
        public int lowerLimit;
        public int upperLimit;
        public int min;
        public int max;
    
        public ListingParserEventHandler() : base()
        {
            oddEven = 0;
            registeredEvents.Add("Listing_pre_event", resetParser);
            registeredEvents.Add("SingleValue_pre_event", SingleValuePreEvent);
            registeredEvents.Add("Range_post_event", RangePostEvent);
            registeredEvents.Add("LowerValue_pre_event", LowerValuePreEvent);
            registeredEvents.Add("UpperValue_pre_event", UpperValuePreEvent);
        }
        
        public void changeOddEvenFlag(int _oddElen)
        {
            oddEven = _oddElen;
        }
        
        
        public void resetParser(Parser.TreeNode node)
        {
            counts = new HashSet<int>();
            min = 1 << 22;
            max = -(1 << 22);
        }
        
        
        public void SingleValuePreEvent(Parser.TreeNode node)
        {
            Add(Convert.ToInt32(node.getText()));
        }
        
        
        public void LowerValuePreEvent(Parser.TreeNode node)
        {
            lowerLimit = Convert.ToInt32(node.getText());
        }
        
        
        public void UpperValuePreEvent(Parser.TreeNode node)
        {
            upperLimit = Convert.ToInt32(node.getText());
        }
        
        
        public void RangePostEvent(Parser.TreeNode node)
        {
            for (int count = lowerLimit; count <= upperLimit; ++count)
            {
                Add(count);
            }
        }
        
        public void Add(int count)
        {
            if (oddEven == 0 || (oddEven == 1 && (count % 2 == 1)) || (oddEven == 2 && (count % 2 == 0)))
            {
                counts.Add(count);
                min = Math.Min(min, count);
                max = Math.Max(max, count);
            }
        }
    }    
}