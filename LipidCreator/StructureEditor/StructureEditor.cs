using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Drawing.Drawing2D;

namespace LipidCreatorStructureEditor
{
    public enum MouseState {MouseDefault, MouseDown, MouseDownMove};
    public enum Action {Idle, ChangeAtom, ChangeAtomSelect, ChangeBond, DrawBond, MoveAtom, MoveAtomSelect};
        
    public partial class LipidCreatorStructureEditor : Form
    {
        
        public static int cursorCircleRadius = 15;
        public LipidStructure lipidStructure;
        public StructureNode currentNode = null;
        public Bond currentBond = null;
        public MouseState mouseState = MouseState.MouseDefault;
        public StructureNode drawStart = null;
        public Action action = Action.Idle;
        public StructureNode moveNode = null;
        public int selectedIndexPositive = -1;
        public int selectedIndexNegative = -1;
        public Point startSelect;
        public PointF previousMousePosition;
        public List<StructureNode> moveNodes = new List<StructureNode>();
        public static double ELECTRON_REST_MASS = 0.00054857990946;
        
        
        
        public LipidCreatorStructureEditor()
        {            
            
            InitializeComponent();
            this.DoubleBuffered = true;
            
            string file = "/home/dominik/workspace/src/LipidCreator/LipidCreator/data/structures/PC genNeg.cdxml";
            lipidStructure = new LipidStructure(file, this);
            
            positiveFragmentsListBox.Items.Add("HG 164");
            lipidStructure.addPositiveFragment("HG 164");
            
            positiveFragmentsListBox.Items.Add("HG 181");
            lipidStructure.addPositiveFragment("HG 181");
            
            negativeFragmentsListBox.Items.Add("HG -OH");
            lipidStructure.addNegativeFragment("HG -OH");
            
            computeFragmentMass(null, null);
        }
        
        
        protected override void OnPaint(PaintEventArgs e)
        {
            DrawScene(e.Graphics);
            base.OnPaint(e);
        }

        
        
        private void DrawScene(Graphics g)
        {
            
            // Handle mouse 
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            var mouse = PointToClient(Cursor.Position);
            
            if (((action == Action.ChangeAtom || action == Action.MoveAtom) && mouseState != MouseState.MouseDownMove) || action == Action.DrawBond)
            {
                if (currentNode != null)
                {
                    NodeProjection nodeProjection = currentNode.nodeProjections[lipidStructure.currentProjection];
                    Pen pen = new Pen(Color.DeepSkyBlue, 2);
                    g.DrawEllipse(pen, currentNode.position.X + nodeProjection.shift.X - cursorCircleRadius, currentNode.position.Y + nodeProjection.shift.Y - cursorCircleRadius, cursorCircleRadius * 2, cursorCircleRadius * 2);
                }
            }
            else if (action == Action.ChangeBond)
            {
                if (currentBond != null)
                {
                    Pen pen = new Pen(Color.DeepSkyBlue, 2);
                    int xb = (int)((currentBond.edgeSingle.start.X + currentBond.edgeSingle.end.X) * 0.5);
                    int yb = (int)((currentBond.edgeSingle.start.Y + currentBond.edgeSingle.end.Y) * 0.5);
                    g.DrawEllipse(pen, xb - cursorCircleRadius, yb - cursorCircleRadius, cursorCircleRadius * 2, cursorCircleRadius * 2);
                }
            }
            
            if (moveNodes.Count > 0)
            {
                foreach (var node in moveNodes)
                {
                    NodeProjection nodeProjection = node.nodeProjections[lipidStructure.currentProjection];
                    Pen pen = new Pen(Color.DeepSkyBlue, 2);
                    g.DrawEllipse(pen, node.position.X + nodeProjection.shift.X - cursorCircleRadius, node.position.Y + nodeProjection.shift.Y - cursorCircleRadius, cursorCircleRadius * 2, cursorCircleRadius * 2);
                }
            }
            
            lipidStructure.drawEdges(g, this);
            if (mouseState == MouseState.MouseDownMove && action == Action.DrawBond && drawStart != null)
            {
                NodeProjection nodeProjection = drawStart.nodeProjections[lipidStructure.currentProjection];
                Graphics clippingGraphics = lipidStructure.setClipping(g, this);
                PointF start = new PointF(drawStart.position.X + nodeProjection.shift.X, drawStart.position.Y + nodeProjection.shift.Y);
                g.DrawLine(lipidStructure.penEnabled, start, mouse);
                lipidStructure.releaseClipping(g, clippingGraphics);
            }
            lipidStructure.drawNodes(g);
            
            
            if (mouseState == MouseState.MouseDownMove && (action == Action.ChangeAtomSelect || action == Action.MoveAtomSelect))
            {
                float[] dashValues = {5, 5};
                Pen blackPen = new Pen(Color.Black, 1);
                blackPen.DashPattern = dashValues;
                Point lower = new Point(Math.Min(startSelect.X, mouse.X), Math.Min(startSelect.Y, mouse.Y));
                Point upper = new Point(Math.Max(startSelect.X, mouse.X), Math.Max(startSelect.Y, mouse.Y));
                g.DrawLine(blackPen, lower, new Point(upper.X, lower.Y));
                g.DrawLine(blackPen, lower, new Point(lower.X, upper.Y));
                g.DrawLine(blackPen, new Point(upper.X, lower.Y), upper);
                g.DrawLine(blackPen, new Point(lower.X, upper.Y), upper);
                
                // mark all atoms within the selection box
                foreach (var node in lipidStructure.nodes)
                {
                    NodeProjection nodeProjection = node.nodeProjections[lipidStructure.currentProjection];
                    double xm = node.position.X + nodeProjection.shift.X;
                    double ym = node.position.Y + nodeProjection.shift.Y;
                    
                    if (lower.X <= xm && xm <= upper.X && lower.Y <= ym && ym <= upper.Y)
                    {
                        Pen pen = new Pen(Color.DeepSkyBlue, 2);
                        g.DrawEllipse(pen, node.position.X + nodeProjection.shift.X - cursorCircleRadius, node.position.Y + nodeProjection.shift.Y - cursorCircleRadius, cursorCircleRadius * 2, cursorCircleRadius * 2);
                    }
                }
                
            }
        } 
        
        
        
        
        
        
        private void mouseUp(Object sender, MouseEventArgs e)
        {
            switch (mouseState)
            {
                case MouseState.MouseDown:
                    if (action == Action.ChangeAtom && currentNode != null)
                    {
                        currentNode.toggleState();
                        updateStructure();
                    }
                    else if (action == Action.ChangeBond && currentBond != null)
                    {
                        currentBond.toggleState(); 
                        lipidStructure.computeBonds();
                        updateStructure();
                    }
                    else if (action == Action.MoveAtomSelect)
                    {
                        action = Action.MoveAtom;
                    }
                    else if (action == Action.ChangeAtomSelect)
                    {
                        action = Action.ChangeAtom;
                    }
                    break;
                    
                case MouseState.MouseDownMove:
                    if (action == Action.ChangeAtom && currentNode != null)
                    {
                        currentNode.toggleState();
                        updateStructure();
                    }
                    else if (action == Action.ChangeBond && currentBond != null)
                    {
                        currentBond.toggleState(); 
                        lipidStructure.computeBonds();
                        updateStructure();
                    }
                    else if (action == Action.DrawBond)
                    {
                        if (currentNode != null)
                        {
                            if (drawStart != currentNode)
                            {
                                lipidStructure.addBond(drawStart, currentNode);
                                updateStructure();
                            }
                        }
                        drawStart = null;
                    }
                    else if (action == Action.MoveAtom)
                    {
                        moveNode = null;
                    }
                    else if (action == Action.ChangeAtomSelect)
                    {
                        action = Action.ChangeAtom;
                        var mouse = PointToClient(Cursor.Position);
                        Point lower = new Point(Math.Min(startSelect.X, mouse.X), Math.Min(startSelect.Y, mouse.Y));
                        Point upper = new Point(Math.Max(startSelect.X, mouse.X), Math.Max(startSelect.Y, mouse.Y));
                        // mark all atoms within the selection box
                        foreach (var node in lipidStructure.nodes)
                        {
                            NodeProjection nodeProjection = node.nodeProjections[lipidStructure.currentProjection];
                            double xm = node.position.X + nodeProjection.shift.X;
                            double ym = node.position.Y + nodeProjection.shift.Y;
                            
                            if (lower.X <= xm && xm <= upper.X && lower.Y <= ym && ym <= upper.Y)
                            {
                                node.toggleState();
                            }
                        }
                    }
                    else if (action == Action.MoveAtomSelect)
                    {
                        action = Action.MoveAtom;
                        var mouse = PointToClient(Cursor.Position);
                        Point lower = new Point(Math.Min(startSelect.X, mouse.X), Math.Min(startSelect.Y, mouse.Y));
                        Point upper = new Point(Math.Max(startSelect.X, mouse.X), Math.Max(startSelect.Y, mouse.Y));
                        // mark all atoms within the selection box
                        foreach (var node in lipidStructure.nodes)
                        {
                            NodeProjection nodeProjection = node.nodeProjections[lipidStructure.currentProjection];
                            double xm = node.position.X + nodeProjection.shift.X;
                            double ym = node.position.Y + nodeProjection.shift.Y;
                            
                            if (lower.X <= xm && xm <= upper.X && lower.Y <= ym && ym <= upper.Y)
                            {
                                moveNodes.Add(node);
                            }
                        }
                    }
                    break;
                    
                default:
                    break;
            }
            
            mouseState = MouseState.MouseDefault;
            updateStructure();
        }
        
        
        
        
        
        
        
        private void mouseDown(Object sender, MouseEventArgs e)
        {
            if (mouseState == MouseState.MouseDefault)
            {
                mouseState = MouseState.MouseDown;
                if (currentNode == null)
                {
                    if (action == Action.ChangeAtom)
                    {
                        startSelect = PointToClient(Cursor.Position);
                        action = Action.ChangeAtomSelect;
                    }
                    if (action == Action.MoveAtom)
                    {
                        startSelect = PointToClient(Cursor.Position);
                        action = Action.MoveAtomSelect;
                        moveNodes.Clear();
                    }
                }
                
                else if (action == Action.MoveAtom)
                {
                    previousMousePosition = PointToClient(Cursor.Position);
                    currentNode.prepareMove();
                    foreach (var node in moveNodes) node.prepareMove();
                }
                   
                else if (action == Action.DrawBond)
                {
                    drawStart = currentNode;
                }
            }
        }
        
        
        
        
        
        
        
        private void mouseMove(Object sender, MouseEventArgs e)
        {
            StructureNode previousNode = currentNode;
            currentNode = null;
            
            Bond previousBond = currentBond;
            currentBond = null;
            
            PointF mouse = PointToClient(Cursor.Position);
            
            
            if (action == Action.ChangeAtom || action == Action.MoveAtom || action == Action.DrawBond)
            {
                double minDist = 1e10;
                foreach (var node in lipidStructure.nodes)
                {
                    NodeProjection nodeProjection = node.nodeProjections[lipidStructure.currentProjection];
                    double dist = Math.Pow(node.position.X + nodeProjection.shift.X - mouse.X, 2) + Math.Pow(node.position.Y + nodeProjection.shift.Y - mouse.Y, 2);
                    if (dist <= Math.Pow(10 + cursorCircleRadius, 2) && dist < minDist)
                    {
                        minDist = dist;
                        currentNode = node;
                    }
                    
                    if (action != Action.DrawBond && node.decorators.Count > 0 && !node.text.Equals("R"))
                    {
                        foreach (var decorator in node.decorators)
                        {
                            NodeProjection decoratorProjection = decorator.nodeProjections[lipidStructure.currentProjection];
                            dist = Math.Pow(decorator.position.X + decoratorProjection.shift.X - mouse.X, 2) + Math.Pow(decorator.position.Y + decoratorProjection.shift.Y - mouse.Y, 2);
                            if (dist <= Math.Pow(10 + cursorCircleRadius, 2) && dist < minDist)
                            {
                                minDist = dist;
                                currentNode = decorator;
                            }
                        }
                    }
                }
                
                if (previousNode != currentNode || action == Action.DrawBond)
                {
                    updateStructure();
                }
            }
            
            else if (action == Action.ChangeBond)
            {
                double minDistC = 1e10;
                foreach (var bond in lipidStructure.bonds)
                {
                    double xb = (bond.edgeSingle.start.X + bond.edgeSingle.end.X) * 0.5;
                    double yb = (bond.edgeSingle.start.Y + bond.edgeSingle.end.Y) * 0.5;
                    double dist  = Math.Pow(xb - mouse.X, 2) + Math.Pow(yb - mouse.Y, 2);
                    if (dist <= Math.Pow(10 + cursorCircleRadius, 2) && dist < minDistC)
                    {
                        minDistC = dist;
                        currentBond = bond;
                    }
                }
                foreach (var bond in lipidStructure.additionalBonds)
                {
                    if (!bond.bondProjections.ContainsKey(lipidStructure.currentProjection)) continue;
                    double xb = (bond.edgeSingle.start.X + bond.edgeSingle.end.X) * 0.5;
                    double yb = (bond.edgeSingle.start.Y + bond.edgeSingle.end.Y) * 0.5;
                    double dist  = Math.Pow(xb - mouse.X, 2) + Math.Pow(yb - mouse.Y, 2);
                    if (dist <= Math.Pow(10 + cursorCircleRadius, 2) && dist < minDistC)
                    {
                        minDistC = dist;
                        currentBond = bond;
                    }
                }
                
                if (previousBond != currentBond)
                {
                    updateStructure();
                }
            }
            else if (action == Action.ChangeAtomSelect || action == Action.MoveAtomSelect)
            {
                updateStructure();
            }
            
            
            if (mouseState == MouseState.MouseDown)
            {
                mouseState = MouseState.MouseDownMove;
                
                if (currentNode != null)
                {
                    if (action == Action.MoveAtom)
                    {
                        moveNode = currentNode;
                    }
                }
            }
            
            else if (mouseState == MouseState.MouseDownMove)
            {
                if (moveNode != null && action == Action.MoveAtom)
                {
                    PointF shift = new PointF(mouse.X - previousMousePosition.X, mouse.Y - previousMousePosition.Y);
                    if (moveNodes.Count > 0 && moveNodes.Contains(moveNode))
                    {
                        foreach (var m in moveNodes) m.move(shift);
                    }
                    else
                    {
                        moveNodes.Clear();
                        moveNode.move(shift);
                    }
                    lipidStructure.computeBonds();
                    updateStructure();
                }
            }
        }
    
    
    
            
        private void actionChangeAtomStateClicked(Object sender, EventArgs e)
        {
            moveNodes.Clear();
            foreach (var button in actionButtons) button.BackColor = Color.FromArgb(255, 255, 255);
            if (action == Action.ChangeAtom)
            {
                actionChangeAtomState.BackColor = Color.FromArgb(255, 255, 255);
                action = Action.Idle;
            }
            else 
            {
                actionChangeAtomState.BackColor = Color.FromArgb(210, 210, 210);
                action = Action.ChangeAtom;
            }
            updateStructure();
        }
        
        
        
            
        private void actionChangeBondStateClicked(Object sender, EventArgs e)
        {
            moveNodes.Clear();
            foreach (var button in actionButtons) button.BackColor = Color.FromArgb(255, 255, 255);
            if (action == Action.ChangeBond)
            {
                actionChangeBondState.BackColor = Color.FromArgb(255, 255, 255);
                action = Action.Idle;
            }
            else
            {
                actionChangeBondState.BackColor = Color.FromArgb(210, 210, 210);
                action = Action.ChangeBond;
            }
            updateStructure();
        }
        
        
        
            
        private void actionDrawBondClicked(Object sender, EventArgs e)
        {
            foreach (var button in actionButtons) button.BackColor = Color.FromArgb(255, 255, 255);
            moveNodes.Clear();
            if (action == Action.DrawBond)
            {
                actionDrawBond.BackColor = Color.FromArgb(255, 255, 255);
                action = Action.Idle;
            }
            else
            {
                actionDrawBond.BackColor = Color.FromArgb(210, 210, 210);
                action = Action.DrawBond;
            }
            updateStructure();
        }
        
        
        
            
        private void actionMoveAtomClicked(Object sender, EventArgs e)
        {
            foreach (var button in actionButtons) button.BackColor = Color.FromArgb(255, 255, 255);
            moveNodes.Clear();
            if (action == Action.MoveAtom)
            {
                actionMoveAtom.BackColor = Color.FromArgb(255, 255, 255);
                action = Action.Idle;
            }
            else
            {
                actionMoveAtom.BackColor = Color.FromArgb(210, 210, 210);
                action = Action.MoveAtom;
            }
            updateStructure();
        }
        
        
        
        
            
        private void actionFinalViewClicked(Object sender, EventArgs e)
        {
            if (lipidStructure.developmentView)
            {
                actionFinalView.BackColor = Color.FromArgb(210, 210, 210);
            }
            else
            {
                actionFinalView.BackColor = Color.FromArgb(255, 255, 255);
            }
            lipidStructure.developmentView = !lipidStructure.developmentView;
            updateStructure();
        }
        
        
        
        
        private void positiveFragmentDoubleClicked(Object sender, EventArgs e)
        {
            int selectedIndex = positiveFragmentsListBox.SelectedIndex;
            if (selectedIndex < 0) return;
            
            string currentFragmentName = (string)positiveFragmentsListBox.Items[selectedIndex];
            string newFragmentName = InputBox.Show("Please write a new fragment name:", "New fragment name", currentFragmentName);
            if (newFragmentName.Length == 0 || newFragmentName.Equals(currentFragmentName)) return;
            
            lipidStructure.positiveFragments.Add(newFragmentName, lipidStructure.positiveFragments[currentFragmentName]);
            lipidStructure.positiveFragments.Remove(currentFragmentName);
            positiveFragmentsListBox.Items[selectedIndex] = newFragmentName;
        }
        
        
        private void positiveFragmentClicked(Object sender, EventArgs e)
        {
            moveNodes.Clear();
            int selectedIndex = positiveFragmentsListBox.SelectedIndex;
            if (selectedIndex == selectedIndexPositive)
            {
                positiveFragmentsListBox.SelectedIndex = -1;
                selectedIndexPositive = -1;
                lipidStructure.changeFragment();
            }
            else
            {
                selectedIndexPositive = selectedIndex;
                lipidStructure.changeFragment((string)positiveFragmentsListBox.Items[selectedIndex], true);
            }
            negativeFragmentsListBox.SelectedIndex = -1;
            selectedIndexNegative = -1;
            lipidStructure.computeBonds();
            updateStructure();
        }
        
        
        private void negativeFragmentDoubleClicked(Object sender, EventArgs e)
        {
            int selectedIndex = negativeFragmentsListBox.SelectedIndex;
            if (selectedIndex < 0) return;
            
            string currentFragmentName = (string)negativeFragmentsListBox.Items[selectedIndex];
            string newFragmentName = InputBox.Show("Please write a new fragment name:", "New fragment name", currentFragmentName);
            if (newFragmentName.Length == 0 || newFragmentName.Equals(currentFragmentName)) return;
            
            lipidStructure.negativeFragments.Add(newFragmentName, lipidStructure.negativeFragments[currentFragmentName]);
            lipidStructure.negativeFragments.Remove(currentFragmentName);
            negativeFragmentsListBox.Items[selectedIndex] = newFragmentName;
        }
        
        
        private void negativeFragmentClicked(Object sender, EventArgs e)
        {
            moveNodes.Clear();
            int selectedIndex = negativeFragmentsListBox.SelectedIndex;
            if (selectedIndex == selectedIndexNegative)
            {
                negativeFragmentsListBox.SelectedIndex = -1;
                selectedIndexNegative = -1;
                lipidStructure.changeFragment();
            }
            else
            {
                selectedIndexNegative = selectedIndex;
                lipidStructure.changeFragment((string)negativeFragmentsListBox.Items[selectedIndex], false);
                
            }
            positiveFragmentsListBox.SelectedIndex = -1;
            selectedIndexPositive = -1;
            lipidStructure.computeBonds();
            updateStructure();
        }
        
        
        
        
        public void updateStructure()
        {
            Invalidate();
            computeFragmentMass(null, null);
        }
        
        
        
        
        private void computeFragmentMass(Object sender, EventArgs e)
        {
            int C = 0;
            int H = 0;
            int N = 0;
            int O = 0;
            int P = 0;
            int S = 0;
            
            double mass = 0;
            Dictionary<string, int[]> residues = new Dictionary<string, int[]>();
            
            try {
                int numC = Convert.ToInt32(bb1Carbon.Text);
                int numDB = Convert.ToInt32(bb1DB.Text);
                int numOH = Convert.ToInt32(bb1Hydro.Text);
                residues.Add("1", new int[]{numC, numDB, numOH});
            }
            catch(Exception){}
            
            try {
                int numC = Convert.ToInt32(bb2Carbon.Text);
                int numDB = Convert.ToInt32(bb2DB.Text);
                int numOH = Convert.ToInt32(bb2Hydro.Text);
                residues.Add("2", new int[]{numC, numDB, numOH});
            }
            catch(Exception){}
            
            int adductCharge = 0;
            bool useAdduct = false;
            foreach (var node in lipidStructure.nodes)
            {
                NodeProjection nodeProjection = node.nodeProjections[lipidStructure.currentProjection];
                if (nodeProjection.nodeState != NodeState.Enabled) continue;
                
                if (node.text.Equals("X"))
                {
                    useAdduct = true;
                    continue;
                }
                
                int charges = 0;
                foreach (var decorator in node.decorators)
                {   
                    NodeProjection decoratorProjection = decorator.nodeProjections[lipidStructure.currentProjection];
                    if (decorator.text.Equals("+") && decoratorProjection.nodeState == NodeState.Enabled) charges += 1;
                    else if (decorator.text.Equals("-") && decoratorProjection.nodeState == NodeState.Enabled) charges -= 1;
                }
                adductCharge += charges;
                
                if (lipidStructure.freeElectrons.ContainsKey(node.text))
                {
                    H += Math.Max(0, charges + lipidStructure.freeElectrons[node.text] - lipidStructure.nodeConnectionsCount[node.id]);
                }
                
                switch (node.text)
                {
                    case "C": C += 1; break;
                    case "H": H += 1; break;
                    case "N": N += 1; break;
                    case "O": O += 1; break;
                    case "P": P += 1; break;
                    case "S": S += 1; break;
                    case "R":
                        string residue = node.decorators[0].text;
                        if (!residues.ContainsKey(residue)) break;
                        int[] nums = residues[residue];
                        C += nums[0] - 1;
                        H += 1 + 2 * (nums[0] - 1) - (2 * nums[1]) - nums[2];
                        O += nums[2];
                        break;
                }
            }
            
            if (useAdduct)
            {
                switch(adductComboBox.SelectedIndex)
                {
                    case 0:
                        H += 1;
                        adductCharge += 1;
                        break;
                        
                    case 1: 
                        H += 2;
                        adductCharge += 2;
                        break;
                        
                    case 2: 
                        C += 1; H += 4;
                        adductCharge += 1;
                        break;
                        
                    case 3: 
                        H -= 1;
                        adductCharge -= 1;
                        break;
                        
                    case 4: 
                        H -= 2;
                        adductCharge -= 2;
                        break;
                        
                    case 5: 
                        C += 1; H += 1; O += 2;
                        adductCharge -= 1;
                        break;
                        
                    case 6: 
                        C += 2; H += 3; O += 2;
                        adductCharge -= 1;
                        break;
                }
            }
            
            mass += C * 12.0 + H * 1.007825035 + N * 14.0030740 + O * 15.99491463 + P * 30.973762 + S * 31.9720707;
            if (adductCharge != 0) mass = (mass - adductCharge * ELECTRON_REST_MASS) / Math.Abs(adductCharge);
            massLabel.Text = string.Format("m/z: {0:N6} Da", mass);
        }
    }


    public class StructureEditor
    {
        public static void Main(string[] args)
        {
            Application.Run(new LipidCreatorStructureEditor());
        }
    }
}
