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
    
    public enum FunctionalGroupType {Alkoxy, Amino, Carboxyl, Cyano, Epoxy, Ethyl, Hydroperoxy, Hydroxy, Methoxy, Methyl, Nitro, Oxo, Peroxy, Phosphate, Sulfanyl, Sulfate};

    [Serializable]
    public partial class FunctionalGroup
    {
        public FunctionalGroupType type;
        public string name;
        public string abbreviation;
        public ElementDictionary elements;
        
        public FunctionalGroup(FunctionalGroupType _type, string _name, string abbrev, ElementDictionary _elements)
        {
            type = _type;
            name = _name;
            abbreviation = abbrev;
            elements = _elements;
        }
    }
}
