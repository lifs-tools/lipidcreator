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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using log4net;


namespace LipidCreator
{
    [Serializable]
    public partial class LipidsReview : Form
    {
        public DataTable transitionList;
        public DataTable transitionListUnique;
        public DataTable currentView;
        public ArrayList returnValues;
        public CreatorGUI creatorGUI;
        public string[] dataColumns = {};
        public bool pressedBackButton = false;
        public bool edited = false;
        public MoleculeFormulaParserEventHandler moleculeFormulaParserEventHandler;
        public Parser parser;
        private static readonly ILog log = LogManager.GetLogger(typeof(LipidsReview));
        
        

        public LipidsReview (CreatorGUI _creatorGUI, ArrayList _returnValues)
        {
            returnValues = _returnValues;
            if (returnValues != null) returnValues[0] = false;
            creatorGUI = _creatorGUI;
            transitionList = creatorGUI.lipidCreator.transitionList;
            currentView = this.transitionList;
            transitionListUnique = creatorGUI.lipidCreator.transitionListUnique;
            pressedBackButton = false;
            
            moleculeFormulaParserEventHandler = new MoleculeFormulaParserEventHandler(creatorGUI.lipidCreator);
            parser = new Parser(moleculeFormulaParserEventHandler, creatorGUI.lipidCreator.prefixPath + "data/molecule-formula.grammar", LipidCreator.QUOTE);
            
            
            InitializeComponent ();
            dataGridViewTransitions.DataSource = currentView;
            buttonSendToSkyline.Enabled = creatorGUI.lipidCreator.openedAsExternal;
            updateCountLabel();
            
            
            
            dataGridViewTransitions.Update ();
            dataGridViewTransitions.Refresh ();
            
            if (creatorGUI.lipidCreator.selectedInstrumentForCE.Length > 0)
            {
                InstrumentData instrumentData = creatorGUI.lipidCreator.msInstruments[creatorGUI.lipidCreator.selectedInstrumentForCE];
                buttonStoreSpectralLibrary.Enabled = instrumentData.minCE > 0 && instrumentData.maxCE > 0 && instrumentData.minCE < instrumentData.maxCE;
            }
            
            checkBoxCreateSpectralLibrary.Enabled = creatorGUI.lipidCreator.openedAsExternal && buttonStoreSpectralLibrary.Enabled;
            
        }
        
        
        
        private void gridviewDataRowRemoved(object sender, System.Windows.Forms.DataGridViewRowsRemovedEventArgs e)
        {
            updateCountLabel();
        }
        
        
        
        
        private void gridviewDataRowAdded(object sender, System.Windows.Forms.DataGridViewRowsAddedEventArgs e)
        {
            updateCountLabel();
        }
        
        
        
        
        
        public void updateCountLabel()
        {   
            dataGridViewTransitions.Update ();
            dataGridViewTransitions.Refresh ();
            labelNumberOfTransitions.Text = "Number of transitions: " + (dataGridViewTransitions.Rows.Count - (checkBoxEditMode.Checked ? 1 : 0));
        }
        
        
        
        private void gridviewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            gridviewDataColor();
            dataGridViewTransitions.Columns[0].Visible = false;
        }
        
        
        
        
        
        private void gridviewDataSorted(object sender, EventArgs e)
        {
            gridviewDataColor();
        }
        
        
        
        
        public void gridviewDataColor()
        {
            if (currentView == transitionList)
            {
                foreach (DataGridViewRow dataRow in dataGridViewTransitions.Rows)
                {
                    if((string)(dataRow.Cells[0].Value) == "False")
                    {
                        dataRow.DefaultCellStyle.BackColor = Color.Beige;
                    }
                }
            }
            else dataGridViewTransitions.DefaultCellStyle.BackColor = Color.Empty;
        }
        
        
        
        
        
        private void closingInteraction(Object sender, FormClosingEventArgs e)
        {
            if (returnValues != null) returnValues[0] = !pressedBackButton;
        }
        
        
        
        public Dictionary<int, int> parseMoleculeFormula(string moleculeFormula)
        {
            parser.parse(moleculeFormula);
            if (parser.wordInGrammer)
            {
                parser.raiseEvents();
                if (moleculeFormulaParserEventHandler.elements != null)
                {
                    return moleculeFormulaParserEventHandler.elements;
                }
                else 
                {
                    throw new Exception("mocecule formula invalid");
                }
            }
            throw new Exception("molecule formula invalid");
        }
        
        
        
        
        public string parseAdduct(string adduct)
        {
            string adductFormula = "";
            switch (adduct)
            {
                case ("[M+H]1+"): adductFormula = "+H"; break;
                case ("[M+2H]2+"): adductFormula = "+2H"; break;
                case ("[M+NH4]1+"): adductFormula = "+NH4"; break;
                case ("[M-H]1-"): adductFormula = "-H"; break;
                case ("[M-2H]2-"): adductFormula = "-2H"; break;
                case ("[M+HCOO]1-"): adductFormula = "+HCOO"; break;
                case ("[M+CH3COO]1-"): adductFormula = "+CH3COO"; break;
            
                default: throw new Exception("adduct formula invalid");
            }
            return adductFormula;
        }
        
        
        
        
        public int parseCharge(string charge)
        {
            try
            {
                return Convert.ToInt32(charge);
            }
            catch (Exception e)
            {
                throw new Exception("charge invalid");
            }
        }
        
        
        
        public double parseMass(string mass)
        {
            try
            {
                return Convert.ToDouble(mass.Replace(",", "."), CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new Exception("mass invalid");
            }
        }
        
        
        
        
        public bool editedCheck()
        {
            int rowLine = 0;
            foreach (DataRow row in currentView.Rows)
            {
                
                
                string validNames = "";
                // check if names are valid
                try 
                {
                    validNames = (string)row[LipidCreator.MOLECULE_LIST_NAME];
                    if (validNames.Length == 0) throw new Exception();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Invalid molecule list name in line :" + (rowLine + 1));
                    log.Error("Invalid molecule list name in line " + (rowLine + 1) + ": ", e);
                    return false;
                }
                try
                {
                    validNames = (string)row[LipidCreator.PRECURSOR_NAME];
                    if (validNames.Length == 0) throw new Exception();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Invalid precursor name in line :" + (rowLine + 1));
                    log.Error("Invalid precursor name in line " + (rowLine + 1) + ": ", e);
                    return false;
                }
                try
                {   
                    validNames = (string)row[LipidCreator.PRODUCT_NAME];
                    if (validNames.Length == 0) throw new Exception();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Invalid product name in line :" + (rowLine + 1));
                    log.Error("Invalid product name in line " + (rowLine + 1) + ": ", e);
                    return false;
                }
            
                // check precursor data
                string precursorMoluculeFormula = "", precursorIonFormula = "", precursorMass = "", precursorCharge = "";
                try
                {
                    precursorMoluculeFormula = (string)row[LipidCreator.PRECURSOR_NEUTRAL_FORMULA];
                }
                catch (Exception e) {}
                
                try
                {
                    precursorIonFormula = (string)row[LipidCreator.PRECURSOR_ADDUCT];
                }
                catch (Exception e) {}
                
                try
                {
                    precursorMass = (string)row[LipidCreator.PRECURSOR_MZ];
                }
                catch (Exception e) {}
                
                try
                {
                    precursorCharge = (string)row[LipidCreator.PRECURSOR_CHARGE];
                }
                catch (Exception e) {}
                
                
                int precursorState = (precursorMoluculeFormula.Length > 0 ? 1 : 0) | (precursorIonFormula.Length > 0 ? 2 : 0) | (precursorMass.Length > 0 ? 4 : 0) | (precursorCharge.Length > 0 ? 8 : 0);
                
                try {
                    Dictionary<int, int> precursorElements;
                    string precursorAdduct;
                    double precursorMassDB;
                    int precursorChargeInt;
                    int charge;
                    double mass;
                    switch (precursorState)
                    {
                            
                        case 3:
                            precursorElements = parseMoleculeFormula(precursorMoluculeFormula);
                            precursorAdduct = parseAdduct(precursorIonFormula);
                            charge = Lipid.getChargeAndAddAdduct(precursorElements, precursorAdduct);
                            mass = LipidCreator.computeMass(precursorElements, charge) / (double)(Math.Abs(charge));
                            row[LipidCreator.PRECURSOR_MZ] = string.Format("{0:N4}", mass);
                            row[LipidCreator.PRECURSOR_CHARGE] = Convert.ToString(charge);
                            break;
                            
                        case 7:
                            precursorElements = parseMoleculeFormula(precursorMoluculeFormula);
                            precursorAdduct = parseAdduct(precursorIonFormula);
                            precursorMassDB = parseMass(precursorMass);
                            charge = Lipid.getChargeAndAddAdduct(precursorElements, precursorAdduct);
                            mass = LipidCreator.computeMass(precursorElements, charge) / (double)(Math.Abs(charge));
                            if (Math.Abs(mass - precursorMassDB) > 0.01)
                            {
                                throw new Exception("mass invalid");
                            }
                            row[LipidCreator.PRECURSOR_CHARGE] = Convert.ToString(charge);
                            break;
                            
                        case 11:
                            precursorElements = parseMoleculeFormula(precursorMoluculeFormula);
                            precursorAdduct = parseAdduct(precursorIonFormula);
                            precursorChargeInt = parseCharge(precursorCharge);
                            charge = Lipid.getChargeAndAddAdduct(precursorElements, precursorAdduct);
                            mass = LipidCreator.computeMass(precursorElements, charge) / (double)(Math.Abs(charge));
                            row[LipidCreator.PRECURSOR_MZ] = string.Format("{0:N4}", mass);
                            if (charge != precursorChargeInt)
                            {
                                throw new Exception("charge invalid");
                            }
                            break;
                            
                        case 15:
                            precursorElements = parseMoleculeFormula(precursorMoluculeFormula);
                            precursorAdduct = parseAdduct(precursorIonFormula);
                            precursorMassDB = parseMass(precursorMass);
                            precursorChargeInt = parseCharge(precursorCharge);
                            charge = Lipid.getChargeAndAddAdduct(precursorElements, precursorAdduct);
                            mass = LipidCreator.computeMass(precursorElements, charge) / (double)(Math.Abs(charge));
                            if (Math.Abs(mass - precursorMassDB) > 0.01)
                            {
                                throw new Exception("mass invalid");
                            }
                            if (charge != precursorChargeInt)
                            {
                                throw new Exception("charge invalid");
                            }
                            break;
                            
                        case 12:
                            precursorMassDB = parseMass(precursorMass);
                            precursorChargeInt = parseCharge(precursorCharge);
                            if (precursorChargeInt == 0)
                            {
                                throw new Exception("charge invalid");
                            }
                            break;
                            
                        default:
                            throw new Exception("data missing");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error in line " + (rowLine + 1) + ": precursor " + e.Message);
                    log.Error("Error in line " + (rowLine + 1) + ": precursor " + e.Message);
                    return false;
                }
                
                
                
                
                
            
                // check product data
                string productMoluculeFormula = "", productIonFormula = "", productMass = "", productCharge = "";
                try
                {
                    productMoluculeFormula = (string)row[LipidCreator.PRODUCT_NEUTRAL_FORMULA];
                }
                catch (Exception e) {}
                
                try
                {
                    productIonFormula = (string)row[LipidCreator.PRODUCT_ADDUCT];
                }
                catch (Exception e) {}
                
                try
                {
                    productMass = (string)row[LipidCreator.PRODUCT_MZ];
                }
                catch (Exception e) {}
                
                try
                {
                    productCharge = (string)row[LipidCreator.PRODUCT_CHARGE];
                }
                catch (Exception e) {}
                
                
                int productState = (productMoluculeFormula.Length > 0 ? 1 : 0) | (productIonFormula.Length > 0 ? 2 : 0) | (productMass.Length > 0 ? 4 : 0) | (productCharge.Length > 0 ? 8 : 0);
                
                try {
                    Dictionary<int, int> productElements;
                    string productAdduct;
                    double productMassDB;
                    int productChargeInt;
                    int charge;
                    double mass;
                    switch (productState)
                    {
                            
                        case 3:
                            productElements = parseMoleculeFormula(productMoluculeFormula);
                            productAdduct = parseAdduct(productIonFormula);
                            charge = Lipid.getChargeAndAddAdduct(productElements, productAdduct);
                            mass = LipidCreator.computeMass(productElements, charge) / (double)(Math.Abs(charge));
                            row[LipidCreator.PRODUCT_MZ] = string.Format("{0:N4}", mass);
                            row[LipidCreator.PRODUCT_CHARGE] = Convert.ToString(charge);
                            break;
                            
                        case 7:
                            productElements = parseMoleculeFormula(productMoluculeFormula);
                            productAdduct = parseAdduct(productIonFormula);
                            productMassDB = parseMass(productMass);
                            charge = Lipid.getChargeAndAddAdduct(productElements, productAdduct);
                            mass = LipidCreator.computeMass(productElements, charge) / (double)(Math.Abs(charge));
                            if (Math.Abs(mass - productMassDB) > 0.01)
                            {
                                throw new Exception("mass invalid");
                            }
                            row[LipidCreator.PRODUCT_CHARGE] = Convert.ToString(charge);
                            break;
                            
                        case 11:
                            productElements = parseMoleculeFormula(productMoluculeFormula);
                            productAdduct = parseAdduct(productIonFormula);
                            productChargeInt = parseCharge(productCharge);
                            charge = Lipid.getChargeAndAddAdduct(productElements, productAdduct);
                            mass = LipidCreator.computeMass(productElements, charge) / (double)(Math.Abs(charge));
                            row[LipidCreator.PRODUCT_MZ] = string.Format("{0:N4}", mass);
                            if (charge != productChargeInt)
                            {
                                throw new Exception("charge invalid");
                            }
                            break;
                            
                        case 15:
                            productElements = parseMoleculeFormula(productMoluculeFormula);
                            productAdduct = parseAdduct(productIonFormula);
                            productMassDB = parseMass(productMass);
                            productChargeInt = parseCharge(productCharge);
                            charge = Lipid.getChargeAndAddAdduct(productElements, productAdduct);
                            mass = LipidCreator.computeMass(productElements, charge) / (double)(Math.Abs(charge));
                            if (Math.Abs(mass - productMassDB) > 0.01)
                            {
                                throw new Exception("mass invalid");
                            }
                            if (charge != productChargeInt)
                            {
                                throw new Exception("charge invalid");
                            }
                            break;
                            
                        case 14:
                        case 12:
                            productMassDB = parseMass(productMass);
                            productChargeInt = parseCharge(productCharge);
                            if (productChargeInt == 0)
                            {
                                throw new Exception("charge invalid");
                            }
                            break;
                            
                        default:
                            throw new Exception("data missing");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error in line " + (rowLine + 1) + ": product " + e.Message);
                    log.Error("Error in line " + (rowLine + 1) + ": product " + e.Message);
                    return false;
                }
                
                ++rowLine;
            }
            return true;
        }
        
        
        
        

        public void buttonSendToSkylineClick (Object sender, EventArgs e)
        {
            this.Enabled = false;
            
            if (edited && !editedCheck())
            {
                this.Enabled = true;
                return;
            }
            
            if (checkBoxCreateSpectralLibrary.Checked) {
                String[] specName = new String[]{""};
                SpectralName spectralName = new SpectralName (specName);
                spectralName.Owner = this;
                spectralName.ShowInTaskbar = false;
                spectralName.ShowDialog ();
                spectralName.Dispose ();
                if (specName [0].Length > 0) {
                    string blibPath = Application.StartupPath + "\\..\\Temp\\" + specName[0] + ".blib";
                    creatorGUI.lipidCreator.createBlib (blibPath);
                    creatorGUI.lipidCreator.sendToSkyline (currentView, specName[0], blibPath);
                    MessageBox.Show ("Sending transition list and spectral library to Skyline is complete.", "Sending complete");
                }
            } else {
                creatorGUI.lipidCreator.sendToSkyline (currentView, "", "");
                MessageBox.Show ("Sending transition list to Skyline is complete.", "Sending complete");
            }
            this.Enabled = true;
        }
        
        

        
        
        public void buttonCheckValuesClick (Object sender, EventArgs e)
        {
            if (editedCheck())
            {
                MessageBox.Show("All data are correct and valid.");
            }
        }
        
        
        private void checkBoxEditModeChanged (object sender, EventArgs e)
        {
            buttonSendToSkyline.Enabled = false;
            edited = true;
            
            if (((CheckBox)sender).Checked)
            {
                dataGridViewTransitions.ReadOnly = false;
                dataGridViewTransitions.AllowUserToAddRows = true;
                dataGridViewTransitions.AllowUserToDeleteRows = true;
                dataGridViewTransitions.RowHeadersVisible = true;
            }
            else
            {
                dataGridViewTransitions.ReadOnly = true;
                dataGridViewTransitions.AllowUserToAddRows = false;
                dataGridViewTransitions.AllowUserToDeleteRows = false;
                dataGridViewTransitions.RowHeadersVisible = false;
            }
            
            dataGridViewTransitions.Update();
            dataGridViewTransitions.Refresh();
        }
        
        

        
        
        
        private void checkBoxCheckedChanged (object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {            
                currentView = this.transitionListUnique;
            }
            else
            {
                currentView = this.transitionList;
            }
            
            updateCountLabel();
            dataGridViewTransitions.DataSource = currentView;
            dataGridViewTransitions.Update();
            dataGridViewTransitions.Refresh();
        }
        
        
        
        
        
        private void buttonBackClick (object sender, EventArgs e)
        {
            pressedBackButton = true;
            Close();
        }
        
        
        
        

        private void buttonStoreTransitionListClick (object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog ();
            
            saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv|tsv files (*.tsv)|*.tsv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog () == DialogResult.OK) {
                string mode = ".csv";
                string separator = ",";
                if(saveFileDialog1.FilterIndex==2) {
                    mode = ".tsv";
                    separator = "\t";
                }
                DialogResult mbr = MessageBox.Show ("Split polarities into two separate files?", "Storing mode", MessageBoxButtons.YesNo);
                
                this.Enabled = false;
                creatorGUI.lipidCreator.storeTransitionList(separator, (mbr == DialogResult.Yes), Path.GetFullPath (saveFileDialog1.FileName), currentView, mode);
                this.Enabled = true;
                MessageBox.Show ("Storing of transition list is complete.", "Storing complete");
            }
        }
        
        
        
        

        private void buttonStoreSpectralLibraryClick (object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog ();
            
            saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.Filter = "blib files (*.blib)|*.blib|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog () == DialogResult.OK) {
                this.Enabled = false;
                try
                {
                    creatorGUI.lipidCreator.createBlib(Path.GetFullPath(saveFileDialog1.FileName));
                    MessageBox.Show("Storing of spectral library is complete.", "Storing complete");
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Problem storing spectral library: " + exception.Message, "Problem storing spectral library");
                }
                this.Enabled = true;
            }
        }
    }
}
