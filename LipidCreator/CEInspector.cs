using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LipidCreator
{
    public partial class CEInspector : Form
    {
        public CreatorGUI creatorGUI;
        public double[] xValCoords;
        public Dictionary<string, double[]> yValCoords;
        public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> instrumentParameters;
        public string selectedInstrument;
        public string selectedClass;
        public string selectedAdduct;
        public DataTable fragmentsList;
        public bool dataLoading = false;
        public bool initialCall = true;
        
        public CEInspector(CreatorGUI _creatorGUI)
        {
            creatorGUI = _creatorGUI;
            instrumentParameters = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>();
            
            fragmentsList = new DataTable("fragmentsList");
            fragmentsList.Columns.Add(new DataColumn("View"));
            fragmentsList.Columns[0].DataType = typeof(bool);
            fragmentsList.Columns.Add(new DataColumn("Fragment name"));
            fragmentsList.Columns[1].DataType = typeof(string);
            
            
            
            InitializeComponent();
            
            // foreach instrument
            foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> kvp1 in creatorGUI.lipidCreator.collisionEnergyHandler.instrumentParameters)
            {
                Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> p1 = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();
                instrumentParameters.Add(kvp1.Key, p1);
                
                // foreach class
                foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> kvp2 in kvp1.Value)
                {
                    Dictionary<string, Dictionary<string, Dictionary<string, string>>> p2 = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
                    p1.Add(kvp2.Key, p2);
                    
                    // foreach adduct
                    foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, string>>> kvp3 in kvp2.Value)
                    {
                        Dictionary<string, Dictionary<string, string>> p3 = new Dictionary<string, Dictionary<string, string>>();
                        p2.Add(kvp3.Key, p3);
                        
                        
                        // foreach fragment
                        foreach(KeyValuePair<string, Dictionary<string, string>> kvp4 in kvp3.Value)
                        {
                            Dictionary<string, string> p4 = new Dictionary<string, string>();
                            p3.Add(kvp4.Key, p4);
                            
                            // foreach parameter
                            foreach(KeyValuePair<string, string> p5 in kvp4.Value)
                            {
                                p4.Add(p5.Key, p5.Value);
                            }
                        }
                    }
                }
            }
            
            
            foreach (string instrumentName in instrumentParameters.Keys)
            {
                instrumentCombobox.Items.Add(instrumentName);
                instrumentCombobox.SelectedIndex = 0;
            }
            
        }
        
        private void fragmentsGridViewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (initialCall){
                fragmentsGridView.Columns[0].Width = 50;
                fragmentsGridView.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                fragmentsGridView.Columns[1].ReadOnly = true;
                initialCall = false;
                
            }
        }
        
        
        public void computeCurves()
        {
            cartesean.CEval = 0;
            xValCoords = new double[cartesean.innerWidthPx + 1];
            int n = fragmentsList.Rows.Count;
            cartesean.setFragmentColors();
            
            yValCoords = new Dictionary<string, double[]>();
            
            for (int i = 0; i <= cartesean.innerWidthPx; ++i)
            {
                xValCoords[i] = ((double)i) / cartesean.innerWidthPx * cartesean.maxXVal;
            }
            
            int k = 0;
            
            foreach(DataRow row in fragmentsList.Rows)
            {
                string fragmentName = (string)row["Fragment name"];
                           
                yValCoords[fragmentName] = new double[cartesean.innerWidthPx + 1];
                int j = 0;
                
                
                foreach (double valX in xValCoords)
                {
                    yValCoords[fragmentName][j] = 10000 * creatorGUI.lipidCreator.collisionEnergyHandler.getIntensity(selectedInstrument, selectedClass, selectedAdduct, fragmentName, valX);
                    ++j;
                }
                ++k;
            }
            cartesean.Refresh();
            fragmentOrderChanged();
        }
        
        
        
        
        
        
        
        public void fragmentOrderChanged()
        {
            int topRank = 100000;
            string topFragment = "";
            
            foreach(DataRow row in fragmentsList.Rows)
            {
                
                if (!(bool)row["View"]) continue;
                string fragmentName = (string)row["Fragment Name"];
            
                int rank = Convert.ToInt32(instrumentParameters[selectedInstrument][selectedClass][selectedAdduct][fragmentName]["rank"]);
                if (topRank > rank)
                {
                    topRank = rank;
                    topFragment = fragmentName;
                }
            }
            
            cartesean.CEval = creatorGUI.lipidCreator.collisionEnergyHandler.getCollisionEnergy(selectedInstrument, selectedClass, selectedAdduct, topFragment);
            cartesean.Refresh();
        }
        
        
        
        
        
        public void instrumentComboboxChanged(Object sender, EventArgs e)
        {
            selectedInstrument = (string)instrumentCombobox.Items[instrumentCombobox.SelectedIndex];
            classCombobox.Items.Clear();
            foreach(string lipidClass in instrumentParameters[selectedInstrument].Keys)
            {
                classCombobox.Items.Add(lipidClass);
            }
            classCombobox.SelectedIndex = 0;
        }
        
        
        
        
        
        public void classComboboxChanged(Object sender, EventArgs e)
        {
            selectedClass = (string)classCombobox.Items[classCombobox.SelectedIndex];
            adductCombobox.Items.Clear();
            foreach(string adduct in instrumentParameters[selectedInstrument][selectedClass].Keys)
            {
                adductCombobox.Items.Add(adduct);
            }
            adductCombobox.SelectedIndex = 0;
        }
        
        
        
        
        
        public void adductComboboxChanged(Object sender, EventArgs e)
        {
            selectedAdduct = (string)adductCombobox.Items[adductCombobox.SelectedIndex];
            dataLoading = true;
            fragmentsList.Rows.Clear();
            
            
            foreach(string fragmentName in instrumentParameters[selectedInstrument][selectedClass][selectedAdduct].Keys.OrderBy(fragmentName => Convert.ToInt32(instrumentParameters[selectedInstrument][selectedClass][selectedAdduct][fragmentName]["rank"])))
            {
                DataRow row = fragmentsList.NewRow();
                row["View"] = true;
                row["Fragment name"] = fragmentName;
                fragmentsList.Rows.Add(row);
            }
         
            fragmentsGridView.Update();
            fragmentsGridView.Refresh();
            computeCurves();
            dataLoading = false;
        }
        
        
        
        
        
        
        public void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        
        
        
        public void applyClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        
        
        
        
        public void mouseMove(object sender, MouseEventArgs e)
        {
            if (cartesean.marginLeft <= e.X && e.X <= cartesean.Width - cartesean.marginRight)
            {
                PointF vals = cartesean.pxToValue(e.X, e.Y);
                int pos = Math.Max(0, e.X - cartesean.marginLeft);
                pos = Math.Min(pos, cartesean.innerWidthPx);
                string highlightName = "";
            
                foreach(DataRow row in fragmentsList.Rows)
                {
                    if ((bool)row["View"])
                    {
                        string fragmentName = (string)row["Fragment name"];
                        if (vals.Y - cartesean.offset <= yValCoords[fragmentName][pos] && yValCoords[fragmentName][pos] <= vals.Y + cartesean.offset)
                        {
                            highlightName = fragmentName;
                            break;
                        }
                    }
                }
                if (cartesean.highlightName != highlightName)
                {
                    cartesean.highlightName = highlightName;
                    cartesean.Refresh();
                }
                if (highlightName.Length > 0)
                {
                    ToolTip1.SetToolTip(cartesean, highlightName);
                }
                else
                {
                    ToolTip1.SetToolTip(cartesean, null);
                    ToolTip1.Hide(cartesean);
                }
            }
        }
        
        
        // thank you for the code inspiration:
        // https://stackoverflow.com/questions/1620947/how-could-i-drag-and-drop-datagridview-rows-under-each-other
        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;

        private void fragmentsGridView_MouseMove(object sender, MouseEventArgs e)
        {
         
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {

                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // Proceed with the drag and drop, passing in the list item. 
                    fragmentsGridView.DoDragDrop(fragmentsGridView.Rows[rowIndexFromMouseDown], DragDropEffects.Move);
                }
            }
        }

        

        private void fragmentsGridView_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            rowIndexFromMouseDown = fragmentsGridView.HitTest(e.X, e.Y).RowIndex;
            if (rowIndexFromMouseDown != -1)
            {
                // Remember the point where the mouse down occurred.
                // The DragSize indicates the size that the mouse can move
                // before a drag event should be started.  
                Size dragSize = SystemInformation.DragSize;
                
                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width >> 1), e.Y - (dragSize.Height >> 1)), dragSize);
            }
            else
            {
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        

        private void fragmentsGridView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        
        
        private void fragmentsGridView_CellContentClick(object sender, EventArgs e)
        {
            fragmentsGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        
        
        
        private void fragmentsGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataLoading) return;
            cartesean.Refresh();
        }


        

        private void fragmentsGridView_DragDrop(object sender, DragEventArgs e)
        {
        
            // The mouse locations are relative to the screen, so they must be
            // converted to client coordinates.
            Point clientPoint = fragmentsGridView.PointToClient(new Point(e.X, e.Y));
            
            // Get the row index of the item the mouse is below.
            rowIndexOfItemUnderMouseToDrop = fragmentsGridView.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect== DragDropEffects.Move && rowIndexOfItemUnderMouseToDrop > -1)
            {
            
                /*
                
                DataGridViewRow rowToMove = e.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;
                fragmentsGridView.Rows.RemoveAt(rowIndexFromMouseDown);
                fragmentsGridView.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove);
                
                */
                
                
                bool view = (bool)fragmentsList.Rows[rowIndexFromMouseDown]["View"];
                string fragment = (string)fragmentsList.Rows[rowIndexFromMouseDown]["Fragment Name"];
                
                fragmentsList.Rows[rowIndexFromMouseDown]["View"] = fragmentsList.Rows[rowIndexOfItemUnderMouseToDrop]["View"];
                fragmentsList.Rows[rowIndexFromMouseDown]["Fragment Name"] = fragmentsList.Rows[rowIndexOfItemUnderMouseToDrop]["Fragment Name"];
                
                fragmentsList.Rows[rowIndexOfItemUnderMouseToDrop]["View"] = view;
                fragmentsList.Rows[rowIndexOfItemUnderMouseToDrop]["Fragment Name"] = fragment;
                
                /*
                DataRow rowToMove = fragmentsList.Rows[rowIndexFromMouseDown];
                fragmentsList.Rows.RemoveAt(rowIndexFromMouseDown);
                fragmentsList.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove);
                */
                
                
                
                int rank = 1;
                foreach(DataRow row in fragmentsList.Rows)
                {
                    instrumentParameters[selectedInstrument][selectedClass][selectedAdduct][(string)row["Fragment Name"]]["rank"] = Convert.ToString(rank);
                    rank++;
                }
                
                fragmentsGridView.Update();
                fragmentsGridView.Refresh();
                fragmentOrderChanged();
            }
        }  
      
    }
}
