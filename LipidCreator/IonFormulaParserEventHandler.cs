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
    public class IonFormulaParserEventHandler : BaseParserEventHandler
    {
        public string adduct;
        public string charge;
        public string chargeOutput;
        public bool validIon;
        public Dictionary<int, int> elements;
        public int count;
        public string heavyElement;
    
    
        public IonFormulaParserEventHandler() : base()
        {
            adduct = "";
            charge = "";
            validIon = false;
            elements = MS2Fragment.createEmptyElementDict();
            count = 0;
            chargeOutput = "";
            heavyElement = "";
            
            registeredEvents.Add("Ion_Formula_pre_event", resetParser);
            registeredEvents.Add("Ion_Formula_post_event", checkValidIon);
            registeredEvents.Add("Adduct_pre_event", AdductPreEvent);
            registeredEvents.Add("Charge_pre_event", ChargePreEvent);
            registeredEvents.Add("Count_pre_event", CountPreEvent);
            registeredEvents.Add("Heavy_Molecule_IUPAC_pre_event", HeavyMoleculeIUPACPreEvent);
            registeredEvents.Add("Molecule_Group_post_event", MoleculeGroupPostEvent);
        }
        
        
        public void resetParser(Parser.TreeNode node)
        {
            adduct = "";
            charge = "";
            validIon = false;
            elements = MS2Fragment.createEmptyElementDict();
            count = 0;
            chargeOutput = "";
            heavyElement = "";
        }
        
        
        public void checkValidIon(Parser.TreeNode node)
        {
            if (adduct.Length > 0 && charge.Length > 0 && elements != null)
            {
                switch (adduct)
                {
                    case "+H":
                    case "+NH4":
                        validIon = (charge == "1+");
                        chargeOutput = "+1";
                        break;
                        
                    case "+2H":
                        validIon = (charge == "2+");
                        chargeOutput = "+2";
                        break;
                        
                    case "-H":
                    case "+HCOO":
                    case "+CH3COO":
                        validIon = (charge == "1-");
                        chargeOutput = "-1";
                        break;
                        
                    case "-2H":
                        validIon = (charge == "2-");
                        chargeOutput = "-2";
                        break;
                        
                    default:
                        break;
                }
            }
        }
        
        public void AdductPreEvent(Parser.TreeNode node)
        {
            adduct = node.getText();
        }
        
        public void ChargePreEvent(Parser.TreeNode node)
        {
            charge = node.getText();
        }
        
        public void CountPreEvent(Parser.TreeNode node)
        {
            count = Convert.ToInt32(node.getText());
        }
        
        public void HeavyMoleculeIUPACPreEvent(Parser.TreeNode node)
        {
            heavyElement = node.getText();
        }
        
        public void MoleculeGroupPostEvent(Parser.TreeNode node)
        {
            if (elements != null && MS2Fragment.HEAVY_POSITIONS_IUPAC.ContainsKey(heavyElement))
            {
                int pos = MS2Fragment.HEAVY_POSITIONS_IUPAC[heavyElement];
                int lightPos = MS2Fragment.LIGHT_ORIGIN[pos];
                elements[pos] += count;
                elements[lightPos] -= count;
            }
            else
            {
                elements = null;
            }
        }
    }    
}