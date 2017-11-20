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
    public class FattyAcid : IComparable<FattyAcid>
    {
        public int length;
        public int db;
        public int hydroxyl;
        public String suffix;
        public DataTable atomsCount;
        public FattyAcid(int l, int db, int hydro){
        
        }
        

        public FattyAcid(int l, int db, int hydro, bool isLCB = false)
        {
            length = l;
            this.db = db;
            hydroxyl = hydro;
            suffix = "";
            atomsCount = MS2Fragment.createEmptyElementTable();
            if (l > 0 || db > 0){
                if (!isLCB){
                    atomsCount.Rows[0]["Count"] = l; // C
                    atomsCount.Rows[1]["Count"] = 2 * l - 1 - 2 * db; // H
                    atomsCount.Rows[2]["Count"] = 1 + hydro; // O
                }
                else {
                    // long chain base
                    atomsCount.Rows[0]["Count"] = l; // C
                    atomsCount.Rows[1]["Count"] = (2 * (l - db) + 1); // H
                    atomsCount.Rows[2]["Count"] = hydro; // O
                    atomsCount.Rows[3]["Count"] = 1; // N
                }
            }
        }

        
        public FattyAcid(int l, int db, int hydro, String suffix)
        {
            length = l;
            this.db = db;
            hydroxyl = hydro;
            this.suffix = (suffix.Length > 2) ? suffix.Substring(2, 1) : "";
            atomsCount = MS2Fragment.createEmptyElementTable();
            if (l > 0 || db > 0){
                atomsCount.Rows[0]["Count"] = l; // C
                switch(this.suffix)
                {
                    case "":
                        atomsCount.Rows[1]["Count"] = 2 * l - 1 - 2 * db; // H
                        atomsCount.Rows[2]["Count"] = 1 + hydro; // O
                        break;
                    case "p":
                        atomsCount.Rows[1]["Count"] = 2 * l - 1 - 2 * db + 2; // H
                        atomsCount.Rows[2]["Count"] = hydro; // O
                        break;
                    case "e":
                        atomsCount.Rows[1]["Count"] = (l + 1) * 2 - 1 - 2 * db; // H
                        atomsCount.Rows[2]["Count"] = hydro; // O
                        break;
                }
            }
        }
        
        public FattyAcid(FattyAcid copy)
        {
            length = copy.length;
            db = copy.length;
            hydroxyl = copy.hydroxyl;
            suffix = copy.suffix;
            atomsCount = MS2Fragment.createEmptyElementTable(copy.atomsCount);
        }

        public int CompareTo(FattyAcid other)
        {
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
            return obj.length * 31 * 31 * 6 + obj.db * 31 + obj.hydroxyl;
        }
        
        public override bool Equals(FattyAcid obj, FattyAcid obj2)
        { 
            return (obj.length == obj2.length) && (obj.db == obj2.db) && (obj.suffix == obj2.suffix) && (obj.hydroxyl == obj2.hydroxyl);
        }
    }
}