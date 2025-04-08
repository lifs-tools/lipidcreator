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
using System.Xml.Linq;
using System.Linq;


namespace LipidCreator
{
    public enum Molecule {C = 0, C13 = 1, H = 2, H2 = 3, N = 4, N15 = 5, O = 6, O17 = 7, O18 = 8, P = 9, P32 = 10, S = 11, S34 = 12, S33 = 13, B = 14, F = 15, Cl = 16, As = 17, Br = 18, I = 19};
    //public static List<Molecule> ElementOrder = new List<Molecule>(){Molecule.C, MoleculeC13, Molecule.H, Molecule.H2, Molecule.N, Molecule.N15, Molecule.O, Molecule.O17, Molecule.O18, Molecule.P, Molecule.P32, Molecule.S, Molecule.S34, Molecule.S33};
    
    public class ElementDictionary : List<int> {
        public const int ElementCount = 20;
        public ElementDictionary(ElementDictionary elementDictionary)
        {
            for (int i = 0; i < ElementCount; ++i) 
            {
                this.Add(elementDictionary[i]);
            }
            while (this.Count < ElementCount) this.Add(0);
        }
        
        public ElementDictionary()
        {
            for (int i = 0; i < ElementCount; ++i) 
            {
                this.Add(0);
            }
        }
        
        public bool hasHeavy()
        {
            foreach (KeyValuePair<Molecule, Element> kvp in MS2Fragment.ALL_ELEMENTS.Where(x => x.Value.isHeavy)) 
            {
                if (this[(int)kvp.Key] > 0) return true;
            }
            return false;
        }
        
        public void print()
        {
            for (int m = 0; m < ElementCount; ++m) 
            {
                Console.WriteLine(MS2Fragment.ALL_ELEMENTS[(Molecule)m].shortcut + ": " + this[m]);
            }
            Console.WriteLine("----------------");
        }
        
        public ulong getHashCode()
        {
            unchecked
            {
                ulong hashCode = 0;
                for (int m = 0; m < ElementCount; ++m) 
                {
                    hashCode += LipidCreator.HashCode(MS2Fragment.ALL_ELEMENTS[(Molecule)m].shortcut) * (ulong)(this[m] + 7);
                }
                return hashCode;
            }
        }
    }


    [Serializable]
    public partial class Element
    {
        public string shortcut;
        public string shortcutNumber;
        public string shortcutIUPAC;
        public string shortcutNomenclature;
        public int position;
        public double mass;
        public bool isHeavy;
        public Molecule[] derivatives;
        public Molecule lightOrigin;
        
        public Element(string _shortcut, string _shortcutNumber, string _shortcutIUPAC, string _shortcutNomenclature, int _position, double _mass, bool _isHeavy, Molecule[] _derivatives, Molecule _lightOrigin)
        {
            shortcut = _shortcut;
            shortcutNumber = _shortcutNumber;
            shortcutIUPAC = _shortcutIUPAC;
            shortcutNomenclature = _shortcutNomenclature;
            position = _position;
            mass = _mass;
            isHeavy = _isHeavy;
            derivatives = _derivatives;
            lightOrigin = _lightOrigin;
        }
    }
}
