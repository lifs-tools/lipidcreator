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
using System.Linq;

namespace LipidCreator
{    
    [Serializable]
    public class FattyAcid : IComparable<FattyAcid>
    {
        public int length;
        public int db;
        public int hydroxyl;
        public string suffix;
        public ElementDictionary atomsCount;
        public bool isLCB;
        
        
        public override string ToString()
        {
            string faLCB = isLCB ? "Long chain base" : "Fatty acyl";
            return String.Format("{0} with length {1}, double bond(s) {2}, hydroxylations {3}, and suffix {4}.", faLCB, length, db, hydroxyl, suffix);
        }
        
        public FattyAcid(int l, int db, int hydro)
        {
        
        }
        
        public FattyAcid(int l, int db, int hydro, string suffix, bool _isLCB = false)
        {
            length = l;
            this.db = db;
            hydroxyl = hydro;
            isLCB = _isLCB;
            atomsCount = MS2Fragment.createEmptyElementDict();
            if (!isLCB)
            {
                this.suffix = (suffix.Length > 2) ? suffix.Substring(2, 1) : "";
                if (length > 0 || db > 0)
                {
                    atomsCount[(int)Molecule.C] = length; // C
                    switch(this.suffix)
                    {
                        case "":
                            atomsCount[(int)Molecule.H] = 2 * length - 1 - 2 * db; // H
                            atomsCount[(int)Molecule.O] = 1 + hydroxyl; // O
                            break;
                        case "p":
                            atomsCount[(int)Molecule.H] = 2 * length - 1 - 2 * (db + 1) + 2; // H
                            atomsCount[(int)Molecule.O] = hydroxyl; // O
                            break;
                        case "a":
                            atomsCount[(int)Molecule.H] = (length + 1) * 2 - 1 - 2 * db; // H
                            atomsCount[(int)Molecule.O] = hydroxyl; // O
                            break;
                    }
                }
            }
            else 
            {
                // long chain base
                this.suffix = "";
                atomsCount[(int)Molecule.C] = length; // C
                atomsCount[(int)Molecule.H] = (2 * (length - db) + 2); // H
                atomsCount[(int)Molecule.O] = hydroxyl; // O
                atomsCount[(int)Molecule.N] = 1; // N
            }
        }
        
        public FattyAcid(FattyAcid copy)
        {
            length = copy.length;
            db = copy.db;
            isLCB = copy.isLCB;
            hydroxyl = copy.hydroxyl;
            suffix = copy.suffix;
            atomsCount = MS2Fragment.createEmptyElementDict();
            for (int m = 0; m < copy.atomsCount.Count; ++m) atomsCount[m] += copy.atomsCount[m];
        }
        
        
        
        
        public void merge(FattyAcid copy)
        {
            if (copy.suffix == "x") return;
            
            length += copy.length;
            db += copy.db;
            isLCB |= copy.isLCB;
            hydroxyl += copy.hydroxyl;
            suffix += copy.suffix;
            for (int m = 0; m < copy.atomsCount.Count; ++m) atomsCount[m] += copy.atomsCount[m];
        }
        
        
        public string ToString(bool fullFormat = true)
        {
            string key = Convert.ToString(length) + ":" + Convert.ToString(db);
            if (isLCB)
            {
                key += ";" + Convert.ToString(hydroxyl);
            }
            else
            {
                if (fullFormat)
                {
                    if (hydroxyl > 0) key += ";" + Convert.ToString(hydroxyl);
                    key += suffix;
                }
            }
            key += LipidCreator.computeHeavyIsotopeLabel(atomsCount);
            return key;
        }
        
        
        
        // this function is different to the one from MS2Fragment class
        public void updateForHeavyLabeled(ElementDictionary heavyAtomsCount)
        {
            for (int m = 0; m < heavyAtomsCount.Count; ++m)
            {
                if (!MS2Fragment.ALL_ELEMENTS[(Molecule)m].isHeavy) continue;
                
                Molecule monoIsotopic = MS2Fragment.ALL_ELEMENTS[(Molecule)m].lightOrigin;
                //int updateValue = updateElements.;
                if (atomsCount[(int)monoIsotopic] >= heavyAtomsCount[m])
                {
                    atomsCount[(int)monoIsotopic] -= heavyAtomsCount[m];
                    atomsCount[m] += heavyAtomsCount[m];
                }
                else
                {
                    atomsCount[m] = atomsCount[(int)monoIsotopic];
                    atomsCount[(int)monoIsotopic] = 0;
                }
            }
        }
        

        public int CompareTo(FattyAcid other)
        {
            if (other.suffix.Length > 0 && other.suffix[0] == 'x') return -1;
            if (suffix.Length > 0 && suffix[0] == 'x') return 1;
            if (length != other.length)
            {
                return length - other.length;
            }
            else if (db != other.db)
            {
                return db - other.db;
            }
            else if (suffix.Length != other.suffix.Length)
            {
                return suffix.Length - other.suffix.Length;
            }
            else if (suffix.Length > 0 && suffix[0] != other.suffix[0])
            {
                return suffix[0] - other.suffix[0];
            }
            return 0;
        }
    }
    
    
    [Serializable]
    public class FattyAcidComparer : EqualityComparer<FattyAcid>
    {
        public override int GetHashCode(FattyAcid obj)
        {
            return obj.length * (LipidCreator.MAX_CARBON_LENGTH + 1) * (LipidCreator.MAX_CARBON_LENGTH + 1) * (LipidCreator.MAX_DB_LENGTH + 1) + obj.db * (LipidCreator.MAX_CARBON_LENGTH + 1) + obj.hydroxyl;
        }
        
        public override bool Equals(FattyAcid obj, FattyAcid obj2)
        { 
            return (obj.length == obj2.length) && (obj.db == obj2.db) && (obj.suffix == obj2.suffix) && (obj.hydroxyl == obj2.hydroxyl);
        }
    }
}
