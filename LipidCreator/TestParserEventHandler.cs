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
    public class TestParserEventHandler : BaseParserEventHandler
    {
        public string lipidname;
    
    
        public TestParserEventHandler() : base ()
        {
            resetLipidBuilder(null);
            
            registeredEvents.Add("lipid_pre_event", resetLipidBuilder);
            registeredEvents.Add("HG_MGL_pre_event", addContext);
            registeredEvents.Add("HG_DGL_pre_event", addContext);
            registeredEvents.Add("HG_SGL_pre_event", addContext);
            registeredEvents.Add("HG_TGL_pre_event", addContext);
            registeredEvents.Add("HG_CL_pre_event", addContext);
            registeredEvents.Add("HG_MLCL_pre_event", addContext);
            registeredEvents.Add("HG_PL_pre_event", addContext);
            registeredEvents.Add("HG_LPL_pre_event", addContext);
            registeredEvents.Add("HG_LPL-O_pre_event", addContext);
            registeredEvents.Add("HG_PL-O_pre_event", addContext);
            registeredEvents.Add("HG_LSL_pre_event", addContext);
            registeredEvents.Add("HG_DSL_pre_event", addContext);
            registeredEvents.Add("Ch_pre_event", addContext);
            registeredEvents.Add("HG_ChE_pre_event", addContext);
            registeredEvents.Add("Mediator_pre_event", addContext);
            registeredEvents.Add("FA_pre_event", addContext);
            registeredEvents.Add("LCB_pre_event", addContext);
            registeredEvents.Add("headgroup_separator_pre_event", addContext);
            registeredEvents.Add("sorted_FA_separator_pre_event", addContext);
            registeredEvents.Add("unsorted_FA_separator_pre_event", addContext);
            registeredEvents.Add("adductInfo_pre_event", addContext);
        }
        
        
        public void resetLipidBuilder(Parser.TreeNode node)
        {
            lipidname = "";
        }
        
        
        
        public void addContext(Parser.TreeNode node)
        {
            lipidname += node.getText();
        }
    }    
}