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
using System.Data;


namespace LipidCreator
{
    public enum AdductType {Hp, HHp, NHHHHp, Hm, HHm, HCOOm, CHHHCOOm};

    [Serializable]
    public partial class Adduct
    {
        public string name;
        public string visualization;
        public int charge;
        public ElementDictionary elements;
        
        public Adduct(string _name, string _visualization, int _charge, ElementDictionary _elements)
        {
            name = _name;
            visualization = _visualization;
            charge = _charge;
            elements = _elements;
        }
        
        public override string ToString()
        {
            return "[M" + name + "]" + Math.Abs(charge) + (charge > 0 ? "+" : "-");
        }
        
        
        public ulong getHashCode()
        {
            unchecked {
				ulong hashCode = LipidCreator.HashCode(name);
				hashCode += LipidCreator.HashCode(visualization);
				hashCode += 3896323UL << (charge + 5);
				return hashCode;
            }
        }
    }
}
