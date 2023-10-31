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
        public FattyAcidType fattyAcidType;
        public ElementDictionary atomsCount;
        public bool isLCB;
        public Dictionary<FunctionalGroupType, int> functionalGroups = new Dictionary<FunctionalGroupType, int>();
        
        public override string ToString()
        {
            string faLCB = isLCB ? "Long chain base" : "Fatty acyl";
            return String.Format("{0} with length {1}, double bond(s) {2}, hydroxylations {3}, and fatty acid type {4}.", faLCB, length, db, hydroxyl, fattyAcidType);
        }
        
        public FattyAcid(int l, int db, int hydro, FattyAcidType fattyAcidType = FattyAcidType.Ester, bool _isLCB = false, Dictionary<FunctionalGroupType, int> funcGroups = null)
        {
            length = l;
            this.db = db;
            hydroxyl = hydro;
            isLCB = _isLCB;
            atomsCount = MS2Fragment.createEmptyElementDict();
            this.fattyAcidType = fattyAcidType;
            if (funcGroups != null)
            {
                foreach (KeyValuePair<FunctionalGroupType, int> kvp in funcGroups) functionalGroups.Add(kvp.Key, kvp.Value);
            }
            if (!isLCB)
            {
                if (length > 0 || db > 0)
                {
                    atomsCount[(int)Molecule.C] = length; // C
                    switch(fattyAcidType)
                    {
                        case FattyAcidType.Ester:
                            atomsCount[(int)Molecule.H] = 2 * length - 1 - 2 * db; // H
                            atomsCount[(int)Molecule.O] = 1 + hydroxyl; // O
                            break;
                            
                        case FattyAcidType.Plasmenyl:
                            atomsCount[(int)Molecule.H] = 2 * length - 1 - 2 * (db + 1) + 2; // H
                            atomsCount[(int)Molecule.O] = hydroxyl; // O
                            break;
                            
                        case FattyAcidType.Plasmanyl:
                            atomsCount[(int)Molecule.H] = (length + 1) * 2 - 1 - 2 * db; // H
                            atomsCount[(int)Molecule.O] = hydroxyl; // O
                            break;
                            
                        default:
                            break;
                    }
                }
            }
            else 
            {
                // long chain base
                atomsCount[(int)Molecule.C] = length; // C
                atomsCount[(int)Molecule.H] = (2 * (length - db) + 2); // H
                atomsCount[(int)Molecule.O] = hydroxyl; // O
                atomsCount[(int)Molecule.N] = 1; // N
            }
            foreach (KeyValuePair<FunctionalGroupType, int> kvp in functionalGroups)
            {
                if (kvp.Value <= 0) continue;
                MS2Fragment.addCounts(atomsCount, Lipid.ALL_FUNCTIONAL_GROUPS[kvp.Key].elements);
            }
        }
        
        public FattyAcid(FattyAcid copy)
        {
            length = copy.length;
            db = copy.db;
            isLCB = copy.isLCB;
            hydroxyl = copy.hydroxyl;
            fattyAcidType = copy.fattyAcidType;
            atomsCount = MS2Fragment.createEmptyElementDict();
            for (int m = 0; m < copy.atomsCount.Count; ++m) atomsCount[m] += copy.atomsCount[m];
        }
        
        
        
        
        public void merge(FattyAcid copy)
        {
            if (fattyAcidType == FattyAcidType.NoType) return;
            
            length += copy.length;
            db += copy.db;
            isLCB |= copy.isLCB;
            hydroxyl += copy.hydroxyl;
            fattyAcidType = copy.fattyAcidType;
            for (int m = 0; m < copy.atomsCount.Count; ++m) atomsCount[m] += copy.atomsCount[m];
        }
        
        
        public string ToString(bool fullFormat = true)
        {
            string key = Lipid.FAPrefix[fattyAcidType] + Convert.ToString(length) + ":" + Convert.ToString(db);
            if (isLCB)
            {
                key += ";O" + Convert.ToString(hydroxyl);
            }
            else
            {
                if (fullFormat)
                {
                    if (hydroxyl > 0) key += ";O" + Convert.ToString(hydroxyl);
                }
            }
            foreach (KeyValuePair<FunctionalGroupType, int> kvp in functionalGroups) key += ";(" + Lipid.ALL_FUNCTIONAL_GROUPS[kvp.Key].abbreviation + ")" + Convert.ToString(kvp.Value);
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
            if (other.isLCB) return -1;
            if (isLCB) return 1;
            if ((int)fattyAcidType != (int)other.fattyAcidType)
            {
                return (int)fattyAcidType - (int)other.fattyAcidType;
            }
            if (length != other.length)
            {
                return length - other.length;
            }
            else if (db != other.db)
            {
                return db - other.db;
            }
            else if (hydroxyl != other.hydroxyl)
            {
                return hydroxyl - other.hydroxyl;
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
            return (obj.length == obj2.length) && (obj.db == obj2.db) && (obj.fattyAcidType == obj2.fattyAcidType) && (obj.hydroxyl == obj2.hydroxyl);
        }
    }
}
