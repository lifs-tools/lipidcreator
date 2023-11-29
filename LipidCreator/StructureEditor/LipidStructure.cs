using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Drawing.Drawing2D;

namespace LipidCreatorStructureEditor
{
    public enum NodeState {Enabled, Disabled, Hidden};
    
    
    public class S {
        public static string space(int n)
        {
            return new string(' ', 2 * n);
        }
    }
    
    public class NodeProjection 
    {
        public PointF shift = new PointF(0, 0);
        public NodeState nodeState = NodeState.Enabled;
        public PointF previousShift;
        
        public NodeProjection(NodeState state = NodeState.Enabled)
        {
            previousShift = new PointF(shift.X, shift.Y);
            nodeState = state;
        }
        
        public NodeProjection(NodeProjection copy)
        {
            shift = new PointF(copy.shift.X, copy.shift.Y);
            nodeState = copy.nodeState;
            previousShift = new PointF(shift.X, shift.Y);
        }
        
        public void toggleState(){
            switch(nodeState)
            {
                case NodeState.Enabled: nodeState = NodeState.Disabled; break;
                case NodeState.Disabled: nodeState = NodeState.Hidden; break;
                case NodeState.Hidden: nodeState = NodeState.Enabled; break;
            }
        }
        
        public void prepareMove()
        {
            previousShift = new PointF(shift.X, shift.Y);
        }
        
        
        public void serialize(StringBuilder sb, int id, int tabs = 0)
        {
            sb.Append(String.Format(S.space(tabs) + "<NodeProjection id={0} x={1} y={2} state=\"{3}\" />\n", id, shift.X, shift.Y, nodeState));
        }
    }
    
    public class StructureNode
    {
        public int id;
        public PointF position;
        public RectangleF boundingBox = new RectangleF();
        public string text = "";
        public List<StructureNode> decorators = new List<StructureNode>();
        public bool isCarbon = false;
        public Dictionary<int, NodeProjection> nodeProjections = new Dictionary<int, NodeProjection>();
        public LipidStructure lipidStructure;
        
        public StructureNode(LipidStructure _lipidStructure, int _id, PointF p)
        {
            lipidStructure = _lipidStructure;
            id = _id;
            position = p;
            isCarbon = true;
        }
        
        public StructureNode(LipidStructure _lipidStructure, int _id, PointF p, RectangleF b, string t)
        {
            lipidStructure = _lipidStructure;
            id = _id;
            position = p;
            boundingBox = b;
            text = t;
            isCarbon = text.Equals("C");
        }
        
        
        public void prepareMove()
        {
            if (lipidStructure.currentProjection == 0) return;
            
            nodeProjections[lipidStructure.currentProjection].prepareMove();
            foreach (var decorator in decorators)
            {
                decorator.nodeProjections[lipidStructure.currentProjection].prepareMove();
            }
        }
        
        
        public void move(PointF shift)
        {
            if (lipidStructure.currentProjection == 0) return;
            nodeProjections[lipidStructure.currentProjection].shift.X = nodeProjections[lipidStructure.currentProjection].previousShift.X + shift.X;
            nodeProjections[lipidStructure.currentProjection].shift.Y = nodeProjections[lipidStructure.currentProjection].previousShift.Y + shift.Y;
            
            foreach (var decorator in decorators)
            {
                NodeProjection decoratorProjection = decorator.nodeProjections[lipidStructure.currentProjection];
                decoratorProjection.shift.X = decoratorProjection.previousShift.X + shift.X;
                decoratorProjection.shift.Y = decoratorProjection.previousShift.Y + shift.Y;
            }
            lipidStructure.countNodeConnections();
        }
        
        
        public void toggleState()
        {
            if (lipidStructure.currentProjection == 0) return;
            
            nodeProjections[lipidStructure.currentProjection].toggleState();
            
            foreach (var decorator in decorators)
            {
                NodeProjection decoratorProjection = decorator.nodeProjections[lipidStructure.currentProjection];
                decoratorProjection.nodeState = nodeProjections[lipidStructure.currentProjection].nodeState;
            }
            lipidStructure.computeBonds();
        }
        
        public void toggleAtom()
        {
            if (lipidStructure.currentProjection == 0 || !lipidStructure.additionalNodes.Contains(this)) return;
            
            switch(text)
            {
                case "C": text = "H"; break;
                case "H": text = "N"; break;
                case "N": text = "O"; break;
                case "O": text = "P"; break;
                case "P": text = "S"; break;
                case "S": text = "C"; break;
                default: break;
            }
            isCarbon = text.Equals("C");
            lipidStructure.computeBonds();
        }
        
        
        public void serialize(StringBuilder sb, int tabs = 0)
        {
            sb.Append(String.Format(S.space(tabs) + "<StructureNode id={0} x={1} y={2} ", id, position.X, position.Y));
            sb.Append(String.Format("bx={0} by={1} bw={2} bh={3} ", boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height));
            sb.Append(String.Format("text=\"{0}\">\n", text));
            if (decorators.Count > 0)
            {
                sb.Append(S.space(tabs + 1) + "<Decorators>\n");
                foreach (var decorator in decorators) decorator.serialize(sb, tabs + 2);
                sb.Append(S.space(tabs + 1) + "</Decorators>\n");
            }
            if (nodeProjections.Count > 0)
            {
                sb.Append(S.space(tabs + 1) + "<NodeProjections>\n");
                foreach (var kvp in nodeProjections) kvp.Value.serialize(sb, kvp.Key, tabs + 2);
                sb.Append(S.space(tabs + 1) + "</NodeProjections>\n");
            }
            sb.Append(S.space(tabs) + "</StructureNode>\n");
        }
    }
    
        
    
    
    public class StructureEdge
    {
        public PointF start;
        public PointF end;
        
        public StructureEdge(PointF s, PointF e)
        {
            start = s;
            end = e;
        }
        
        public StructureEdge(StructureEdge copy)
        {
            start = new PointF(copy.start.X, copy.start.Y);
            end = new PointF(copy.end.X, copy.end.Y);
        }
        
        public void serialize(StringBuilder sb, int tabs = 0)
        {
            sb.Append(S.space(tabs) + String.Format("<StructureEdge sx={0} sy={1} ex={2} ey={3} />\n", start.X, start.Y, end.X, end.Y));
        }
    }
    
    
    public class Bond
    {
        public int id;
        public int startId;
        public int endId;
        public bool isDoubleBond = false;
        public StructureEdge edgeSingle;
        public List<StructureEdge> edgesDouble = new List<StructureEdge>();
        public Dictionary<int, bool> bondProjections = new Dictionary<int, bool>();
        public LipidStructure lipidStructure;
        
        public Bond(LipidStructure _lipidStructure, int _id, int start, int end, bool doubleBond)
        {
            id = _id;
            lipidStructure = _lipidStructure;
            startId = start;
            endId = end;
            isDoubleBond = doubleBond;
        }
        
        public Bond(Bond copy)
        {
            id = copy.id;
            startId = copy.startId;
            endId = copy.endId;
            isDoubleBond = copy.isDoubleBond;
            if (copy.edgeSingle != null) edgeSingle = new StructureEdge(copy.edgeSingle);
            foreach (var edge in copy.edgesDouble)
            {
                edgesDouble.Add(new StructureEdge(edge));
            }
            foreach (var kvp in copy.bondProjections)
            {
                bondProjections.Add(kvp.Key, kvp.Value);
            }
            lipidStructure = copy.lipidStructure;
        }
        
        
        public void toggleState()
        {
            if (lipidStructure.currentProjection == 0) return;
            
            bondProjections[lipidStructure.currentProjection] = !bondProjections[lipidStructure.currentProjection];
            lipidStructure.countNodeConnections();
        }
        
        public void serialize(StringBuilder sb, int tabs = 0)
        {
            
        }
    }
    
    
    
    
    public class LipidStructureFragment {
        public int id;
        public string fragmentName;
        public int charge;
        public const int minCharge = -1;
        public const int maxCharge = 1;
        
        public LipidStructureFragment(int i, string f, int c = 0)
        {
            id = i;
            fragmentName = f;
            charge = c;
        }
        
        public void toggleCharge()
        {
            charge = (charge >= maxCharge) ? minCharge : charge + 1;
        }
        
        public void serialize(StringBuilder sb, int tabs = 0)
        {
            
        }
    }
    
    
    
    public class LipidStructure
    {
        public List<StructureNode> nodes = new List<StructureNode>();
        public HashSet<StructureNode> additionalNodes = new HashSet<StructureNode>();
        public List<Bond> bonds = new List<Bond>();
        public HashSet<Bond> additionalBonds = new HashSet<Bond>();
        public Dictionary<string, LipidStructureFragment> positiveFragments = new Dictionary<string, LipidStructureFragment>();
        public Dictionary<string, LipidStructureFragment> negativeFragments = new Dictionary<string, LipidStructureFragment>();
        
        public Dictionary<int, StructureNode> idToNode = new Dictionary<int, StructureNode>();
        public float factor = 2.5f;
        public float dbSpace = 1.5f;
        public float fontSize = 10.0f;
        public SolidBrush solidBrushEnabled = new SolidBrush(Color.Black);
        public SolidBrush solidBrushDisabled = new SolidBrush(Color.FromArgb(180, 180, 180));
        public SolidBrush solidBrushHidden = new SolidBrush(Color.FromArgb(220, 220, 220));
        public Pen penEnabled;
        public Pen penDisabled;
        public Pen penHidden;
        public int currentId = 0;
        public bool developmentView = true;
        public Font nodeFont;
        public Font decoratorFont;
        public Graphics graphics;
        public int currentProjection;
        public LipidStructureFragment currentFragment = null;
        public Dictionary<int, int> nodeConnectionsCount = new Dictionary<int, int>();
        public Dictionary<int, int> nodeConnectionsHiddenCount = new Dictionary<int, int>();
        public HashSet<string> specialAtoms = new HashSet<string>(){"N", "O", "P", "S"};
        public Dictionary<string, int> freeElectrons = new Dictionary<string, int>(){{"C", 4}, {"N", 3}, {"O", 2}, {"P", 3}, {"S", 2}};
        public int projectionIds = 1;
        public int currentNodeId = 0;
        public int bondIDs = 0;
        
        
        public void parseXML(XElement element, List<XElement> nodes, List<XElement> edges)
        {
            foreach (var fragment in element.Elements().Where(el => el.Name.LocalName.Equals("fragment")))
            {
                foreach (var node in fragment.Elements().Where(el => el.Name.LocalName.Equals("n")))
                {
                    nodes.Add(node);
                    parseXML(node, nodes, edges);
                }
                foreach (var node in fragment.Elements().Where(el => el.Name.LocalName.Equals("b")))
                {
                    edges.Add(node);
                }
                foreach (var node in fragment.Elements().Where(el => el.Name.LocalName.Equals("graphic")))
                {
                    nodes.Add(node);
                }
            }
        }
        
        
        
        public LipidStructure(string file_name, Form form)
        {
            nodeFont = new Font("Arial", fontSize);
            decoratorFont = new Font("Arial", (float)(fontSize * 0.5));
            
            
            XDocument doc = XDocument.Load(file_name);
            graphics = form.CreateGraphics();
            
            List<XElement> xnodes = new List<XElement>();
            List<XElement> xedges = new List<XElement>();
            foreach (var pages in doc.Element("CDXML").Elements().Where(el => el.Name.LocalName.Equals("page")))
            {
                parseXML(pages, xnodes, xedges);
            }
            
            
            // draw atoms and any other label
            foreach (var node in xnodes.Where(el => (el.Name.LocalName.Equals("n")) && ((string)el.Attribute("p") != null) && ((string)el.Attribute("id") != null)))
            {
                string[] tokens = node.Attribute("p").Value.Split(new char[]{' '});
                if (tokens.Length != 2) continue;
                
                try
                {
                    float x = (float)Convert.ToDouble(tokens[0]);
                    float y = (float)Convert.ToDouble(tokens[1]);
                    int nodeId = Convert.ToInt32((string)node.Attribute("id").Value);
                    currentId = Math.Max(currentId, nodeId);
                    
                    
                    string text = "";
                    if (node.Elements().Count() == 0 || node.Elements().First().Elements().Count() == 0)
                    {
                        text = "C";
                    }
                    else 
                    {
                        var t = node.Elements().First();
                        var s = t.Elements().First();
                        text = s.Value.ToString();
                    }
                    
                    RectangleF drawRect = new RectangleF(x, y, 0, 0);
                
                    if ((new HashSet<string>(){"R1", "R2", "R3", "R4"}).Contains(text))
                    {
                        StructureNode sn = new StructureNode(this, nodeId, new PointF(x, y), drawRect, "R");
                        x -= (float)Math.Abs(x * 0.02);
                        nodes.Add(sn);
                        idToNode.Add(nodeId, sn);
                        
                        Size size = TextRenderer.MeasureText(graphics, "R", nodeFont);
                        x += size.Width * 0.75f;
                        y += size.Height * 0.25f;
                        string subIndex = text.Substring(1, 1);
                        
                        RectangleF subscript = new RectangleF(x, y, 0, 0);
                        sn.decorators.Add(new StructureNode(this, -1, new PointF(x, y), subscript, subIndex));
                        
                    }
                    else 
                    {
                        StructureNode sn = new StructureNode(this, nodeId, new PointF(x, y), drawRect, text);
                        nodes.Add(sn);
                        idToNode.Add(nodeId, sn);
                    }
                    
                    currentNodeId = Math.Max(currentNodeId, nodeId);
                }
                catch(Exception ex){
                    Console.WriteLine(ex);
                }
            }
            
            
            
            foreach (var graphic in xnodes.Where(el => (el.Name.LocalName.Equals("graphic")) && ((string)el.Attribute("SymbolType") != null) && ((string)el.Attribute("BoundingBox") != null)))
            {
                // get the symbol
                try
                {
                    string symbolType = graphic.Attribute("SymbolType").Value.ToString();
                    if (!symbolType.Equals("Plus") && !symbolType.Equals("Minus")) continue;
                    string symbol = symbolType.Equals("Plus") ? "+" : "-";
                    
                    if ((string)graphic.Elements().First().Attribute("object") == null) continue;
                    int parentId = Convert.ToInt32(graphic.Elements().First().Attribute("object").Value.ToString());
                    
                    if (!idToNode.ContainsKey(parentId)) continue;
                    
                    int nodeIdG = Convert.ToInt32((string)graphic.Attribute("id").Value);
                    currentId = Math.Max(currentId, nodeIdG);
                    string[] tokensBB = graphic.Attribute("BoundingBox").Value.Split(new char[]{' '});
                    float bx = (float)Convert.ToDouble(tokensBB[0]) - 1;
                    float by = (float)Convert.ToDouble(tokensBB[1]) - 1 - (symbol.Equals("+") ? 2 : 0);
                    RectangleF drawRect = new RectangleF(bx, by, 0, 0);
                    idToNode[parentId].decorators.Add(new StructureNode(this, nodeIdG, new PointF(bx, by), drawRect, symbol));
                }
                catch (Exception){}
            }
            
            
            
            // draw single and double bonds
            foreach (var edge in xedges.Where(el => ((string)el.Attribute("id") != null) && ((string)el.Attribute("B") != null) && ((string)el.Attribute("E") != null)))
            {
                int idB = Convert.ToInt32(edge.Attribute("B").Value);
                int idE = Convert.ToInt32(edge.Attribute("E").Value);
                if (!idToNode.ContainsKey(idB) || !idToNode.ContainsKey(idE)) continue;
                
                StructureNode nodeB = idToNode[idB];
                StructureNode nodeE = idToNode[idE];
                
                Bond bond = null;
                try
                {
                    float xB = nodeB.position.X;
                    float yB = nodeB.position.Y;
                    float xE = nodeE.position.X;
                    float yE = nodeE.position.Y;
                    
                    // get the edge color
                    bool isDoubleBond = ((string)edge.Attribute("Order") != null) && edge.Attribute("Order").Value.ToString().Equals("2");
                    
                    bond = new Bond(this, bondIDs++, idB, idE, isDoubleBond);
                    bonds.Add(bond);
                }
                catch(Exception){}
            }
            
            
            
            float minX = 100000.0f;
            float maxX = -100000.0f;
            float minY = 100000.0f;
            float maxY = -100000.0f;
            
            
            // draw atoms and any other label
            foreach (var node in nodes)
            {
                float x = node.position.X;
                float y = node.position.Y;
                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);
            }
            
            float midX = minX + (maxX - minX) / 2.0f;
            float midY = minY + (maxY - minY) / 2.0f;
            float offsetX = (float)form.Size.Width / 2.0f - midX * factor;
            float offsetY = (float)form.Size.Height / 2.0f - midY * factor;
            
            
            nodeFont = new Font("Arial", fontSize * factor);
            decoratorFont = new Font("Arial", (float)(fontSize * factor * 0.5));
            foreach (var node in nodes)
            {
                float x = node.position.X * factor + offsetX;
                float y = node.position.Y * factor + offsetY;
                node.position = new PointF(x, y);
                
                x = node.boundingBox.X * factor + offsetX;
                y = node.boundingBox.Y * factor + offsetY;
                
                Size size = TextRenderer.MeasureText(graphics, node.text, nodeFont);
                float w = size.Width * 0.9f;
                float h = size.Height * 0.9f;
                node.boundingBox = new RectangleF(x - w * 0.5f, y - h * 0.5f, w, h);
                
                foreach (var decorator in node.decorators)
                {
                    float xd = decorator.position.X * factor + offsetX;
                    float yd = decorator.position.Y * factor + offsetY;
                    decorator.position = new PointF(xd, yd);
                    
                    xd = decorator.boundingBox.X * factor + offsetX;
                    yd = decorator.boundingBox.Y * factor + offsetY;
                    
                    Size sizeD = TextRenderer.MeasureText(graphics, decorator.text, node.text.Equals("R") ? decoratorFont : nodeFont);
                    float wd = sizeD.Width * 0.9f;
                    float hd = sizeD.Height * 0.9f;
                    decorator.boundingBox = new RectangleF(xd - wd * 0.5f, yd - hd * 0.5f, wd, hd);
                }
            }
            
            penEnabled = new Pen(Color.Black, factor);
            penDisabled = new Pen(Color.FromArgb(180, 180, 180), factor);
            penHidden = new Pen(Color.FromArgb(220, 220, 220), factor);
            
            
            foreach (var node in nodes)
            {
                node.nodeProjections.Add(0, new NodeProjection());
                foreach (var decorator in node.decorators) decorator.nodeProjections.Add(0, new NodeProjection());
            }
            foreach (var bond in bonds) bond.bondProjections.Add(0, bond.isDoubleBond);
            currentProjection = 0;
            
            computeBonds();
            changeFragment();
            countNodeConnections();
            
            serialize("foo");
        }
        
        
        
        
        public void serialize(string file_name)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<LipidStructure>\n");
            
            // add node information
            sb.Append("  <Nodes>\n");
            foreach (var node in nodes) node.serialize(sb, 2);
            sb.Append("  </Nodes>\n");
            
            // add additional node information
            if (additionalNodes.Count > 0)
            {
                sb.Append("  <AdditionalNodes>\n");
                foreach (var node in additionalNodes) sb.Append(String.Format("    <NodeID id={0}>\n", node.id));
                sb.Append("  </AdditionalNodes>\n");
            }
            
            // add bond information
            sb.Append("  <Bonds>\n");
            foreach (var bond in bonds) bond.serialize(sb, 2);
            sb.Append("  </Bonds>\n");
            
            // add additional bond information
            if (additionalBonds.Count > 0)
            {
                sb.Append("  <AdditionalBonds>\n");
                foreach (var bond in additionalBonds) sb.Append(String.Format("    <BondsID id={0}>\n", bond.id));
                sb.Append("  </AdditionalBonds>\n");
            }
            
            // add positive fragments
            if (positiveFragments.Count > 0)
            {
                sb.Append("  <PositiveFragments>\n");
                foreach (var kvp in positiveFragments) kvp.Value.serialize(sb, 2);
                sb.Append("  </PositiveFragments>\n");
            }
            
            // add positive fragments
            if (negativeFragments.Count > 0)
            {
                sb.Append("  <NegativeFragments>\n");
                foreach (var kvp in negativeFragments) kvp.Value.serialize(sb, 2);
                sb.Append("  </NegativeFragments>\n");
            }

            
            
            sb.Append("</LipidStructure>\n");
            
            Console.WriteLine(sb.ToString());
        }
        
        
        
        
        public void addFragment(string fragmentName, bool isPositive)
        {
            int newProjectionId = projectionIds++;
            
            LipidStructureFragment newFragment = new LipidStructureFragment(newProjectionId, fragmentName, currentFragment != null ? currentFragment.charge : 0);
            
            if (isPositive) positiveFragments.Add(fragmentName, newFragment);
            else negativeFragments.Add(fragmentName, newFragment);
            
            foreach (var node in nodes)
            {
                node.nodeProjections.Add(newProjectionId, new NodeProjection(node.nodeProjections[currentProjection]));
                foreach (var decorator in node.decorators) decorator.nodeProjections.Add(newProjectionId, new NodeProjection(decorator.nodeProjections[currentProjection]));
            }
            
            foreach (var bond in bonds) bond.bondProjections.Add(newProjectionId, bond.bondProjections[currentProjection]);
            computeBonds();
        }
        
        
        
        
        public void removeFragment(string fragmentName, bool isPositive)
        {
            int projectionId = -1;
            if (isPositive)
            {
                if (!positiveFragments.ContainsKey(fragmentName)) return;
                projectionId = positiveFragments[fragmentName].id;
                positiveFragments.Remove(fragmentName);
            }
            else
            {
                if (!negativeFragments.ContainsKey(fragmentName)) return;
                projectionId = negativeFragments[fragmentName].id;
                negativeFragments.Remove(fragmentName);
            }
            
            foreach (var node in nodes)
            {
                node.nodeProjections.Remove(projectionId);
                foreach (var decorator in node.decorators) decorator.nodeProjections.Remove(projectionId);
            }
            foreach (var bond in bonds) bond.bondProjections.Remove(projectionId);
            
            if (currentProjection == projectionId)
            {
                currentProjection = 0;
                countNodeConnections();
            }
        }
        
        
        
        
        
        public void computeBonds()
        {
            List<Bond> deleteBonds = new List<Bond>();
            foreach (var bond in bonds)
            {
                if (!idToNode.ContainsKey(bond.startId) || !idToNode.ContainsKey(bond.endId))
                {
                    deleteBonds.Add(bond);
                    continue;
                }
                
                StructureNode nodeB = idToNode[bond.startId];
                StructureNode nodeE = idToNode[bond.endId];
                
                if (!nodeB.nodeProjections.ContainsKey(currentProjection) || !nodeE.nodeProjections.ContainsKey(currentProjection))
                {
                    //deleteBonds.Add(bond);
                    continue;
                }
            
                float xB = nodeB.position.X + nodeB.nodeProjections[currentProjection].shift.X;
                float yB = nodeB.position.Y + nodeB.nodeProjections[currentProjection].shift.Y;
                float xE = nodeE.position.X + nodeE.nodeProjections[currentProjection].shift.X;
                float yE = nodeE.position.Y + nodeE.nodeProjections[currentProjection].shift.Y;
                
                bond.edgesDouble.Clear();
                        
                float deltaX = yE - yB;
                float deltaY = xB - xE;
                float norming = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                deltaX /= norming;
                deltaY /= norming;
                
                
                
                if (!nodeB.isCarbon || !nodeE.isCarbon)
                {
                
                    float x1B = xB - deltaX * dbSpace * factor;
                    float y1B = yB - deltaY * dbSpace * factor;
                    float x1E = xE - deltaX * dbSpace * factor;
                    float y1E = yE - deltaY * dbSpace * factor;
                    bond.edgesDouble.Add(new StructureEdge(new PointF(x1B, y1B), new PointF(x1E, y1E)));
                    
                    
                    float x2B = xB + deltaX * dbSpace * factor;
                    float y2B = yB + deltaY * dbSpace * factor;
                    float x2E = xE + deltaX * dbSpace * factor;
                    float y2E = yE + deltaY * dbSpace * factor;
                    bond.edgesDouble.Add(new StructureEdge(new PointF(x2B, y2B), new PointF(x2E, y2E)));
                }
                else
                {
                
                    float x1B = xB - deltaX * (2 * dbSpace * factor);
                    float y1B = yB - deltaY * (2 * dbSpace * factor);
                    float x1E = xE - deltaX * (2 * dbSpace * factor);
                    float y1E = yE - deltaY * (2 * dbSpace * factor);
                    bond.edgesDouble.Add(new StructureEdge(new PointF(x1B, y1B), new PointF(x1E, y1E)));
                    bond.edgesDouble.Add(new StructureEdge(new PointF(xB, yB), new PointF(xE, yE)));
                }
                
                bond.edgeSingle = new StructureEdge(new PointF(xB, yB), new PointF(xE, yE));
            }
            
            foreach (var bond in deleteBonds)
            {
                bonds.Remove(bond);
                if (additionalBonds.Contains(bond)) additionalBonds.Remove(bond);
            }
            
            countNodeConnections();
        }
        
        
        
        public void addNode(int x, int y)
        {
            if (currentProjection == 0) return;
            
            nodeFont = new Font("Arial", fontSize * factor);
            Size size = TextRenderer.MeasureText(graphics, "C", nodeFont);
            float w = size.Width * 0.9f;
            float h = size.Height * 0.9f;
            RectangleF drawRect = new RectangleF(x - w * 0.5f, y - h * 0.5f, w, h);
            StructureNode sn = new StructureNode(this, ++currentNodeId, new PointF(x, y), drawRect, "C");
            
        
            int fragId = currentFragment.id;
            sn.nodeProjections.Add(fragId, new NodeProjection(fragId == currentProjection ? NodeState.Enabled : NodeState.Hidden));
            
            nodes.Add(sn);
            additionalNodes.Add(sn);
            idToNode.Add(currentNodeId, sn);
            
            computeBonds();
        }
        
        
        public StructureNode removeNode(StructureNode node)
        {
            if (currentProjection == 0 || !additionalNodes.Contains(node) || !node.nodeProjections.ContainsKey(currentProjection)) return node;
            
            node.nodeProjections.Remove(currentProjection);
            if (node.nodeProjections.Count == 0)
            {
                additionalNodes.Remove(node);
                nodes.Remove(node);
                idToNode.Remove(node.id);
            }
            
            computeBonds();
            
            return null;
        }
        
        
        
        public void addBond(StructureNode start, StructureNode end)
        {
            if (currentProjection == 0) return;
            Bond bond = new Bond(this, bondIDs++, start.id, end.id, false);
            bond.bondProjections.Add(currentProjection, false);
            bonds.Add(bond);
            additionalBonds.Add(bond);
            computeBonds();
        }
        
        
        
        public void removeBond(Bond bond)
        {
            if (currentProjection == 0|| !additionalBonds.Contains(bond) || !bond.bondProjections.ContainsKey(currentProjection)) return;
            bond.bondProjections.Remove(currentProjection);
            if (bond.bondProjections.Count == 0)
            {
                additionalBonds.Remove(bond);
                bonds.Remove(bond);
            }
            computeBonds();
        }
        
        
        public Graphics setClipping(Graphics g, Form form)
        {
            // set up all clipping
            Graphics clippingGraphics = form.CreateGraphics();
            foreach (var node in nodes)
            {
                if (!node.nodeProjections.ContainsKey(currentProjection)) continue;
                
                NodeProjection nodeProjection = node.nodeProjections[currentProjection];
                if (!developmentView && (node.nodeProjections[currentProjection].nodeState == NodeState.Hidden || node.isCarbon)) continue;
                
                RectangleF r = new RectangleF(node.boundingBox.X + nodeProjection.shift.X, node.boundingBox.Y + nodeProjection.shift.Y, node.boundingBox.Width, node.boundingBox.Height);
                
                clippingGraphics.SetClip(r);
                g.SetClip(clippingGraphics, CombineMode.Exclude);
                
                foreach (var decorator in node.decorators)
                {
                    NodeProjection decoratorProjection = decorator.nodeProjections[currentProjection];
                    if (!developmentView && decoratorProjection.nodeState == NodeState.Hidden) continue;
                    
                    RectangleF rd = new RectangleF(decorator.boundingBox.X + decoratorProjection.shift.X, decorator.boundingBox.Y + decoratorProjection.shift.Y, decorator.boundingBox.Width, decorator.boundingBox.Height);
                    clippingGraphics.SetClip(rd);
                    g.SetClip(clippingGraphics, CombineMode.Exclude);
                }
                    
                
            }
            return clippingGraphics;
        }
        
        
        
        public void countNodeConnections()
        {
            
            nodeConnectionsCount.Clear();
            foreach (var node in nodes) nodeConnectionsCount.Add(node.id, 0);
            
            nodeConnectionsHiddenCount.Clear();
            foreach (var node in nodes) nodeConnectionsHiddenCount.Add(node.id, 0);
            
            foreach (var bond in bonds)
            {
                if (!bond.bondProjections.ContainsKey(currentProjection)) continue;
                
                StructureNode nodeB = idToNode[bond.startId];
                StructureNode nodeE = idToNode[bond.endId];
                NodeProjection proStart = nodeB.nodeProjections[currentProjection];
                NodeProjection proEnd = nodeE.nodeProjections[currentProjection];
                
                if (proStart.nodeState == NodeState.Enabled && proEnd.nodeState == NodeState.Enabled)
                {
                    nodeConnectionsCount[bond.startId] += bond.bondProjections[currentProjection] ? 2 : 1;
                    nodeConnectionsCount[bond.endId] += bond.bondProjections[currentProjection] ? 2 : 1;
                }
                if (proStart.nodeState != NodeState.Hidden && proEnd.nodeState != NodeState.Hidden){
                    nodeConnectionsHiddenCount[bond.startId] += bond.bondProjections[currentProjection] ? 2 : 1;
                    nodeConnectionsHiddenCount[bond.endId] += bond.bondProjections[currentProjection] ? 2 : 1;
                }
            }
        }
        
        
        
        public void changeFragment(string fragmentName = "", bool charge = false)
        {
            currentFragment = (fragmentName.Length == 0) ? null : ((charge ? positiveFragments : negativeFragments)[fragmentName]);
            currentProjection = (fragmentName.Length == 0) ? 0 : ((charge ? positiveFragments : negativeFragments)[fragmentName].id);
            computeBonds();
        }
        
        
        
        
        public void changeGlobalCharge()
        {
            if (currentFragment == null) return;
            currentFragment.toggleCharge();
        }
        
        
        
        
        public void releaseClipping(Graphics g, Graphics clippingGraphics)
        {
            clippingGraphics.Dispose();
            g.ResetClip();
        }
        
        
        
        
        
        public void drawEdges(Graphics g, Form form)
        {
            Graphics clippingGraphics = setClipping(g, form);
            
            foreach (var bond in bonds)
            {
                if (!bond.bondProjections.ContainsKey(currentProjection)) continue;
                
                if (!idToNode[bond.startId].nodeProjections.ContainsKey(currentProjection) || !idToNode[bond.endId].nodeProjections.ContainsKey(currentProjection)) continue;
                
                NodeProjection proStart = idToNode[bond.startId].nodeProjections[currentProjection];
                NodeProjection proEnd = idToNode[bond.endId].nodeProjections[currentProjection];
                
                if (!developmentView && (proStart.nodeState == NodeState.Hidden || proEnd.nodeState == NodeState.Hidden)) continue;
                    
                Pen pen = penEnabled; 
                if (proStart.nodeState == NodeState.Hidden || proEnd.nodeState == NodeState.Hidden) pen = penHidden;
                else if (proStart.nodeState == NodeState.Disabled || proEnd.nodeState == NodeState.Disabled) pen = penDisabled;
                if (bond.bondProjections[currentProjection])
                {
                    foreach (var edge in bond.edgesDouble) g.DrawLine(pen, edge.start, edge.end);
                }
                else
                {
                    g.DrawLine(pen, bond.edgeSingle.start, bond.edgeSingle.end);
                }
            }
            
            releaseClipping(g, clippingGraphics);
        }
        
        
        
        
        
        public void drawNodes(Graphics g)
        {
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;
            drawFormat.LineAlignment = StringAlignment.Center;
            
            Dictionary<StructureNode, List<StructureNode>> adjacentNodes = new Dictionary<StructureNode, List<StructureNode>>();
            foreach (var node in nodes) adjacentNodes.Add(node, new List<StructureNode>());
                
            foreach (var bond in bonds)
            {
                if (!idToNode.ContainsKey(bond.startId) || !idToNode.ContainsKey(bond.endId)) continue;
                
                StructureNode nodeB = idToNode[bond.startId];
                StructureNode nodeE = idToNode[bond.endId];
                
                if (!nodeB.nodeProjections.ContainsKey(currentProjection) || !nodeE.nodeProjections.ContainsKey(currentProjection) || !adjacentNodes.ContainsKey(nodeB) || ! adjacentNodes.ContainsKey(nodeE)) continue;
                
                NodeProjection proStart = nodeB.nodeProjections[currentProjection];
                NodeProjection proEnd = nodeE.nodeProjections[currentProjection];
                if (proStart.nodeState != NodeState.Hidden && proEnd.nodeState != NodeState.Hidden)
                {
                    adjacentNodes[nodeB].Add(nodeE);
                    adjacentNodes[nodeE].Add(nodeB);
                }
            }
            
            
            int minX = (1 << 30);
            int maxX = -(1 << 30);
            int minY = (1 << 30);
            int maxY = -(1 << 30);
            
            foreach (var node in nodes)
            {
                if (!node.nodeProjections.ContainsKey(currentProjection)) continue;
                
                NodeProjection nodeProjection = node.nodeProjections[currentProjection];
                PointF nodePoint = new PointF(node.boundingBox.X + nodeProjection.shift.X, node.boundingBox.Y + nodeProjection.shift.Y);
                if (!developmentView && (nodeProjection.nodeState == NodeState.Hidden)) continue;
                
                SolidBrush brush = solidBrushEnabled;
                if (nodeProjection.nodeState == NodeState.Hidden) brush = solidBrushHidden;
                else if (nodeProjection.nodeState == NodeState.Disabled) brush = solidBrushDisabled;
                
                RectangleF r = new RectangleF(nodePoint.X, nodePoint.Y, node.boundingBox.Width, node.boundingBox.Height);
                if (!node.isCarbon || developmentView) g.DrawString(node.text, nodeFont, brush, r, drawFormat);
                if (nodeProjection.nodeState == NodeState.Enabled)
                {
                    minX = Math.Min(minX, (int)nodePoint.X);
                    maxX = Math.Max(maxX, (int)(nodePoint.X + node.boundingBox.Width));
                    minY = Math.Min(minY, (int)nodePoint.Y);
                    maxY = Math.Max(maxY, (int)(nodePoint.Y + node.boundingBox.Height));
                }
                
                if (node.isCarbon) continue;
                
                foreach (var decorator in node.decorators)
                {
                    NodeProjection decoratorProjection = decorator.nodeProjections[currentProjection];
                    SolidBrush decoBrush = solidBrushEnabled;
                    
                    if (!developmentView && (decoratorProjection.nodeState == NodeState.Hidden)) continue;
                    
                    if (decoratorProjection.nodeState == NodeState.Hidden) decoBrush = solidBrushHidden;
                    else if (decoratorProjection.nodeState == NodeState.Disabled) decoBrush = solidBrushDisabled;
                    
                    RectangleF rd = new RectangleF(decorator.boundingBox.X + decoratorProjection.shift.X, decorator.boundingBox.Y + decoratorProjection.shift.Y, decorator.boundingBox.Width, decorator.boundingBox.Height);
                    g.DrawString(decorator.text, node.text.Equals("R") ? decoratorFont : nodeFont, decoBrush, rd, drawFormat);
                }
                
                
                
            
                // if we are in user view mode and {O, P, N, S} atoms are not connected
                // with enough bonds, we will draw additional H atoms to the left or right
                if (developmentView || !specialAtoms.Contains(node.text) || nodeProjection.nodeState != NodeState.Enabled) continue;
                
                
                int charges = 0;
                foreach (var decorator in node.decorators)
                {   
                    NodeProjection decoratorProjection = decorator.nodeProjections[currentProjection];
                    if (decorator.text.Equals("+") && decoratorProjection.nodeState != NodeState.Hidden) charges += 1;
                    else if (decorator.text.Equals("-") && decoratorProjection.nodeState != NodeState.Hidden) charges -= 1;
                }
                
                int unusedElectrons = Math.Max(0, charges + freeElectrons[node.text] - nodeConnectionsHiddenCount[node.id]);
                if (unusedElectrons == 0) continue;
                
                // check if right side of atom has space
                bool leftfree = true;
                bool rightfree = true;
                
                foreach (var targetNode in adjacentNodes[node])
                {
                    NodeProjection targetProjection = targetNode.nodeProjections[currentProjection];
                    PointF targetPoint = new PointF(targetNode.boundingBox.X + targetProjection.shift.X, targetNode.boundingBox.Y + targetProjection.shift.Y);
                    
                    double angle = Math.Atan((targetPoint.Y - nodePoint.Y) / (targetPoint.X - nodePoint.X)) / Math.PI * 180.0;
                    if (nodePoint.X <= targetPoint.X && -45 <= angle && angle <= 45) rightfree = false;
                    else if (nodePoint.X > targetPoint.X && -45 <= angle && angle <= 45) leftfree = false;
                    
                }
                
                if (!rightfree && leftfree) r.X -= r.Width * 0.8f;
                else r.X += r.Width * 0.8f;
                
                Size sizeAdditionalText = new Size();
                if (unusedElectrons > 1)
                {
                    sizeAdditionalText = TextRenderer.MeasureText(graphics, Convert.ToString(unusedElectrons), decoratorFont);
                    if (!rightfree && leftfree) r.X -= sizeAdditionalText.Width * 0.5f;
                }
                g.DrawString("H", nodeFont, brush, r, drawFormat);
                if (unusedElectrons > 1)
                {
                    RectangleF rd = new RectangleF(r.X + sizeAdditionalText.Width * 1.25f, r.Y + sizeAdditionalText.Height * 0.75f, sizeAdditionalText.Width, sizeAdditionalText.Height);
                    g.DrawString(Convert.ToString(unusedElectrons), decoratorFont, brush, rd, drawFormat);
                }
            }
            
            if (currentFragment == null || currentFragment.charge == 0) return;
            
            g.DrawLine(penEnabled, new Point(minX, minY), new Point(minX, maxY));
            g.DrawLine(penEnabled, new Point(maxX, minY), new Point(maxX, maxY));
            
            int h = (int)((maxY - minY) * 0.06);
            g.DrawLine(penEnabled, new Point(minX, minY), new Point(minX + h, minY));
            g.DrawLine(penEnabled, new Point(maxX, minY), new Point(maxX - h, minY));
            
            g.DrawLine(penEnabled, new Point(minX, maxY), new Point(minX + h, maxY));
            g.DrawLine(penEnabled, new Point(maxX, maxY), new Point(maxX - h, maxY));
            
            string chargeSign = currentFragment.charge > 0 ? "+" : "-";
            Size size = TextRenderer.MeasureText(graphics, chargeSign, nodeFont);
            g.DrawString(chargeSign, nodeFont, solidBrushEnabled, new RectangleF(maxX, minY - (size.Height >> 1), size.Width, size.Height), drawFormat);
        }
    }
}
