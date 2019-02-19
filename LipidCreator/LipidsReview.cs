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
        public Parser moleculeFormulaParser;
        public IonFormulaParserEventHandler ionFormulaParserEventHandler;
        public Parser ionFormulaParser;
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
            
            moleculeFormulaParserEventHandler = new MoleculeFormulaParserEventHandler();
            moleculeFormulaParser = new Parser(moleculeFormulaParserEventHandler, creatorGUI.lipidCreator.prefixPath + "data/molecule-formula.grammar", LipidCreator.QUOTE);
            ionFormulaParserEventHandler = new IonFormulaParserEventHandler();
            ionFormulaParser = new Parser(ionFormulaParserEventHandler, creatorGUI.lipidCreator.prefixPath + "data/ion-formula.grammar", LipidCreator.QUOTE);
            
            
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
            updateCountLabel();
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
        
        
        
        public Dictionary<int, int> parseMoleculeFormula(string moleculeFormula, string colName)
        {
            moleculeFormulaParser.parse(moleculeFormula);
            if (moleculeFormulaParser.wordInGrammer)
            {
                moleculeFormulaParser.raiseEvents();
                if (moleculeFormulaParserEventHandler.elements != null)
                {
                    return moleculeFormulaParserEventHandler.elements;
                }
                else 
                {
                    throw new WrongFormatException("molecule formula invalid", colName);
                }
            }
            throw new WrongFormatException("molecule formula invalid", colName);
        }
        
        
        
        
        public ArrayList parseAdduct(string adduct, string colName)
        {
            ionFormulaParser.parse(adduct);
            if (ionFormulaParser.wordInGrammer)
            {
                ionFormulaParser.raiseEvents();
                if (ionFormulaParserEventHandler.validIon)
                {
                    return new ArrayList(){ionFormulaParserEventHandler.adduct, ionFormulaParserEventHandler.elements};
                }
                else 
                {
                    throw new WrongFormatException("adduct formula invalid", colName);
                }
            }
            throw new WrongFormatException("addut formula invalid", colName);
        }
        
        
        
        
        public int parseCharge(string charge, string colName)
        {
            try
            {
                return Convert.ToInt32(charge);
            }
            catch (Exception e)
            {
                throw new WrongFormatException("charge invalid", colName);
            }
        }
        
        
        
        public double parseMass(string mass, string colName)
        {
            try
            {
                return Convert.ToDouble(mass.Replace(",", "."), CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new WrongFormatException("mass invalid", colName);
            }
        }
        
        
        public void selectCell(int r, string colName)
        {
            int c = 0;
            foreach (DataColumn col in currentView.Columns)
            {
                if (col.ColumnName == colName) break;
                c++;
            }
            if (c < dataGridViewTransitions.Rows[r].Cells.Count)
            {
                dataGridViewTransitions.CurrentCell = dataGridViewTransitions.Rows[r].Cells[c];
                dataGridViewTransitions.CurrentCell.Selected = true;
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
                    if (validNames.Length == 0) throw new WrongFormatException("molecule list name invalid", LipidCreator.MOLECULE_LIST_NAME);
                }
                catch (WrongFormatException e)
                {
                    MessageBox.Show("Error in line " + (rowLine + 1) + ": " + e.Message);
                    selectCell(rowLine, e.columnName);
                    return false;
                }
                try
                {
                    validNames = (string)row[LipidCreator.PRECURSOR_NAME];
                    if (validNames.Length == 0) throw new WrongFormatException("precursor name invalid", LipidCreator.PRECURSOR_NAME);
                }
                catch (WrongFormatException e)
                {
                    MessageBox.Show("Error in line " + (rowLine + 1) + ": " + e.Message);
                    selectCell(rowLine, e.columnName);
                    return false;
                }
                try
                {   
                    validNames = (string)row[LipidCreator.PRODUCT_NAME];
                    if (validNames.Length == 0) throw new WrongFormatException("product name invalid", LipidCreator.PRODUCT_NAME);
                }
                catch (WrongFormatException e)
                {
                    MessageBox.Show("Error in line " + (rowLine + 1) + ": " + e.Message);
                    selectCell(rowLine, e.columnName);
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
                    Dictionary<int, int> precursorHeavyElements;
                    string precursorAdduct;
                    ArrayList precursorAdductData;
                    double precursorMassDB;
                    int precursorChargeInt;
                    int charge;
                    double mass;
                    switch (precursorState)
                    {
                            
                        case 3:
                            precursorElements = parseMoleculeFormula(precursorMoluculeFormula, LipidCreator.PRECURSOR_NEUTRAL_FORMULA);
                            precursorAdductData = parseAdduct(precursorIonFormula, LipidCreator.PRECURSOR_ADDUCT);
                            precursorAdduct = (string)precursorAdductData[0];
                            precursorHeavyElements = (Dictionary<int, int>)precursorAdductData[1];
                            MS2Fragment.addCounts(precursorElements, precursorHeavyElements);
                            if (!MS2Fragment.validElementDict(precursorElements)) throw new WrongFormatException("mass invalid", LipidCreator.PRECURSOR_MZ);
                            
                            charge = Lipid.getChargeAndAddAdduct(precursorElements, precursorAdduct);
                            mass = LipidCreator.computeMass(precursorElements, charge) / (double)(Math.Abs(charge));
                            row[LipidCreator.PRECURSOR_MZ] = string.Format("{0:N4}", mass);
                            row[LipidCreator.PRECURSOR_CHARGE] = Convert.ToString(charge);
                            break;
                            
                        case 7:
                            precursorElements = parseMoleculeFormula(precursorMoluculeFormula, LipidCreator.PRECURSOR_NEUTRAL_FORMULA);
                            precursorAdductData = parseAdduct(precursorIonFormula, LipidCreator.PRECURSOR_ADDUCT);
                            precursorAdduct = (string)precursorAdductData[0];
                            precursorHeavyElements = (Dictionary<int, int>)precursorAdductData[1];
                            MS2Fragment.addCounts(precursorElements, precursorHeavyElements);
                            if (!MS2Fragment.validElementDict(precursorElements)) throw new WrongFormatException("mass invalid", LipidCreator.PRECURSOR_MZ);
                            
                            precursorMassDB = parseMass(precursorMass, LipidCreator.PRECURSOR_MZ);
                            charge = Lipid.getChargeAndAddAdduct(precursorElements, precursorAdduct);
                            mass = LipidCreator.computeMass(precursorElements, charge) / (double)(Math.Abs(charge));
                            if (Math.Abs(mass - precursorMassDB) > 0.01)
                            {
                                throw new WrongFormatException("mass invalid", LipidCreator.PRECURSOR_MZ);
                            }
                            row[LipidCreator.PRECURSOR_CHARGE] = Convert.ToString(charge);
                            break;
                            
                        case 11:
                            precursorElements = parseMoleculeFormula(precursorMoluculeFormula, LipidCreator.PRECURSOR_NEUTRAL_FORMULA);
                            precursorAdductData = parseAdduct(precursorIonFormula, LipidCreator.PRECURSOR_ADDUCT);
                            precursorAdduct = (string)precursorAdductData[0];
                            precursorHeavyElements = (Dictionary<int, int>)precursorAdductData[1];
                            MS2Fragment.addCounts(precursorElements, precursorHeavyElements);
                            if (!MS2Fragment.validElementDict(precursorElements)) throw new WrongFormatException("mass invalid", LipidCreator.PRECURSOR_MZ);
                            
                            precursorChargeInt = parseCharge(precursorCharge, LipidCreator.PRECURSOR_CHARGE);
                            charge = Lipid.getChargeAndAddAdduct(precursorElements, precursorAdduct);
                            mass = LipidCreator.computeMass(precursorElements, charge) / (double)(Math.Abs(charge));
                            row[LipidCreator.PRECURSOR_MZ] = string.Format("{0:N4}", mass);
                            if (charge != precursorChargeInt)
                            {
                                throw new WrongFormatException("charge invalid", LipidCreator.PRECURSOR_CHARGE);
                            }
                            break;
                            
                        case 15:
                            precursorElements = parseMoleculeFormula(precursorMoluculeFormula, LipidCreator.PRECURSOR_NEUTRAL_FORMULA);
                            precursorAdductData = parseAdduct(precursorIonFormula, LipidCreator.PRECURSOR_ADDUCT);
                            precursorAdduct = (string)precursorAdductData[0];
                            precursorHeavyElements = (Dictionary<int, int>)precursorAdductData[1];
                            MS2Fragment.addCounts(precursorElements, precursorHeavyElements);
                            if (!MS2Fragment.validElementDict(precursorElements)) throw new WrongFormatException("mass invalid", LipidCreator.PRECURSOR_MZ);
                            
                            precursorMassDB = parseMass(precursorMass, LipidCreator.PRECURSOR_MZ);
                            precursorChargeInt = parseCharge(precursorCharge, LipidCreator.PRECURSOR_CHARGE);
                            charge = Lipid.getChargeAndAddAdduct(precursorElements, precursorAdduct);
                            mass = LipidCreator.computeMass(precursorElements, charge) / (double)(Math.Abs(charge));
                            if (Math.Abs(mass - precursorMassDB) > 0.01)
                            {
                                throw new WrongFormatException("mass invalid", LipidCreator.PRECURSOR_MZ);
                            }
                            if (charge != precursorChargeInt)
                            {
                                throw new WrongFormatException("charge invalid", LipidCreator.PRECURSOR_CHARGE);
                            }
                            break;
                            
                        case 12:
                            precursorMassDB = parseMass(precursorMass, LipidCreator.PRECURSOR_MZ);
                            precursorChargeInt = parseCharge(precursorCharge, LipidCreator.PRECURSOR_CHARGE);
                            if (precursorChargeInt == 0)
                            {
                                throw new WrongFormatException("charge invalid", LipidCreator.PRECURSOR_CHARGE);
                            }
                            break;
                            
                        default:
                            throw new WrongFormatException("data missing", LipidCreator.PRECURSOR_MZ);
                    }
                }
                catch (WrongFormatException e)
                {
                    selectCell(rowLine, e.columnName);
                    MessageBox.Show("Error in line " + (rowLine + 1) + ": precursor " + e.Message);
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
                    Dictionary<int, int> productHeavyElements;
                    string productAdduct;
                    ArrayList productAdductData;
                    double productMassDB;
                    int productChargeInt;
                    int charge;
                    double mass;
                    switch (productState)
                    {
                            
                        case 3:
                            productElements = parseMoleculeFormula(productMoluculeFormula, LipidCreator.PRODUCT_NEUTRAL_FORMULA);
                            productAdductData = parseAdduct(productIonFormula, LipidCreator.PRODUCT_ADDUCT);
                            productAdduct = (string)productAdductData[0];
                            productHeavyElements = (Dictionary<int, int>)productAdductData[1];
                            MS2Fragment.addCounts(productElements, productHeavyElements);
                            if (!MS2Fragment.validElementDict(productElements)) throw new WrongFormatException("mass invalid", LipidCreator.PRODUCT_MZ);
                            
                            charge = Lipid.getChargeAndAddAdduct(productElements, productAdduct);
                            mass = LipidCreator.computeMass(productElements, charge) / (double)(Math.Abs(charge));
                            row[LipidCreator.PRODUCT_MZ] = string.Format("{0:N4}", mass);
                            row[LipidCreator.PRODUCT_CHARGE] = Convert.ToString(charge);
                            break;
                            
                        case 7:
                            productElements = parseMoleculeFormula(productMoluculeFormula, LipidCreator.PRODUCT_NEUTRAL_FORMULA);
                            productAdductData = parseAdduct(productIonFormula, LipidCreator.PRODUCT_ADDUCT);
                            productAdduct = (string)productAdductData[0];
                            productHeavyElements = (Dictionary<int, int>)productAdductData[1];
                            MS2Fragment.addCounts(productElements, productHeavyElements);
                            if (!MS2Fragment.validElementDict(productElements)) throw new WrongFormatException("mass invalid", LipidCreator.PRODUCT_MZ);
                            
                            productMassDB = parseMass(productMass, LipidCreator.PRODUCT_MZ);
                            charge = Lipid.getChargeAndAddAdduct(productElements, productAdduct);
                            mass = LipidCreator.computeMass(productElements, charge) / (double)(Math.Abs(charge));
                            if (Math.Abs(mass - productMassDB) > 0.01)
                            {
                                throw new WrongFormatException("mass invalid", LipidCreator.PRODUCT_MZ);
                            }
                            row[LipidCreator.PRODUCT_CHARGE] = Convert.ToString(charge);
                            break;
                            
                        case 11:
                            productElements = parseMoleculeFormula(productMoluculeFormula, LipidCreator.PRODUCT_NEUTRAL_FORMULA);
                            productAdductData = parseAdduct(productIonFormula, LipidCreator.PRODUCT_ADDUCT);
                            productAdduct = (string)productAdductData[0];
                            productHeavyElements = (Dictionary<int, int>)productAdductData[1];
                            MS2Fragment.addCounts(productElements, productHeavyElements);
                            if (!MS2Fragment.validElementDict(productElements)) throw new WrongFormatException("mass invalid", LipidCreator.PRODUCT_MZ);
                            
                            productChargeInt = parseCharge(productCharge, LipidCreator.PRODUCT_CHARGE);
                            charge = Lipid.getChargeAndAddAdduct(productElements, productAdduct);
                            mass = LipidCreator.computeMass(productElements, charge) / (double)(Math.Abs(charge));
                            row[LipidCreator.PRODUCT_MZ] = string.Format("{0:N4}", mass);
                            if (charge != productChargeInt)
                            {
                                throw new WrongFormatException("charge invalid", LipidCreator.PRODUCT_CHARGE);
                            }
                            break;
                            
                        case 15:
                            productElements = parseMoleculeFormula(productMoluculeFormula, LipidCreator.PRODUCT_NEUTRAL_FORMULA);
                            productAdductData = parseAdduct(productIonFormula, LipidCreator.PRODUCT_ADDUCT);
                            productAdduct = (string)productAdductData[0];
                            productHeavyElements = (Dictionary<int, int>)productAdductData[1];
                            MS2Fragment.addCounts(productElements, productHeavyElements);
                            if (!MS2Fragment.validElementDict(productElements)) throw new WrongFormatException("mass invalid", LipidCreator.PRODUCT_MZ);
                            
                            productMassDB = parseMass(productMass, LipidCreator.PRODUCT_MZ);
                            productChargeInt = parseCharge(productCharge, LipidCreator.PRODUCT_CHARGE);
                            charge = Lipid.getChargeAndAddAdduct(productElements, productAdduct);
                            mass = LipidCreator.computeMass(productElements, charge) / (double)(Math.Abs(charge));
                            if (Math.Abs(mass - productMassDB) > 0.01)
                            {
                                throw new WrongFormatException("mass invalid", LipidCreator.PRODUCT_MZ);
                            }
                            if (charge != productChargeInt)
                            {
                                throw new WrongFormatException("charge invalid", LipidCreator.PRODUCT_CHARGE);
                            }
                            break;
                            
                        case 14:
                        case 12:
                            productMassDB = parseMass(productMass, LipidCreator.PRODUCT_MZ);
                            productChargeInt = parseCharge(productCharge, LipidCreator.PRODUCT_CHARGE);
                            if (productChargeInt == 0)
                            {
                                throw new WrongFormatException("charge invalid", LipidCreator.PRODUCT_CHARGE);
                            }
                            break;
                            
                        default:
                            throw new WrongFormatException("data missing", LipidCreator.PRODUCT_MZ);
                    }
                }
                catch (WrongFormatException e)
                {
                    selectCell(rowLine, e.columnName);
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
    
    public class WrongFormatException : Exception{
        public string columnName;
        public WrongFormatException(string error, string _columnName) : base(error)
        {
            columnName = _columnName;
        }
    }
}
