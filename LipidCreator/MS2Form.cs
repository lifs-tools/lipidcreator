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
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Linq;
using System.Drawing.Drawing2D;

namespace LipidCreator
{
    public class StructureNode
    {
        public PointF position;
        public RectangleF boundingBox = new RectangleF();
        public string text = "";
        public SolidBrush solidBrush = null;
        public Font font = null;
        
        public StructureNode(PointF p)
        {
            position = p;
        }    
        
        public StructureNode(PointF p, RectangleF b, string t, SolidBrush s, Font f)
        {
            position = p;
            boundingBox = b;
            text = t;
            solidBrush = s;
            font = f;
        }
    }
    
    public class StructureEdge
    {
        public PointF start;
        public PointF end;
        public Pen pen;
        
        public StructureEdge(Pen p, PointF s, PointF e)
        {
            start = s;
            end = e;
            pen = p;
        }
    }
    
    public class LipidStructure
    {
        
        public List<StructureNode> nodes = new List<StructureNode>();
        public List<StructureEdge> edges = new List<StructureEdge>();
        public Dictionary<string, StructureNode> idToNode = new Dictionary<string, StructureNode>();
        public List<RectangleF> clippingAreas = new List<RectangleF>();
        public float offsetX = 0;
        public float offsetY = 0;
        public float factor = 1.5f;
        public float dbSpace = 1.5f;
            
        public List<Color> colorList = new List<Color>();
        public Dictionary<string, string> fonts = new Dictionary<string, string>();
        
        
        
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
        
        
        
        public LipidStructure(string file_name, PaintEventArgs e, CustomPictureBox pictureBoxFragments)
        {
            XDocument doc = XDocument.Load(file_name);
            
            List<Color> colorList = new List<Color>();
            Dictionary<string, string> fonts = new Dictionary<string, string>();
            
            foreach (var colors in doc.Element("CDXML").Elements().Where(el => el.Name.LocalName.Equals("colortable")))
            {
                foreach(var color in colors.Elements().Where(el => el.Name.LocalName.Equals("color")))
                {
                    if (((string)color.Attribute("r") == null) || ((string)color.Attribute("g") == null) || ((string)color.Attribute("b") == null)) continue;
                        
                    int r = (int)(Convert.ToDouble(color.Attribute("r").Value.ToString()) * 255.0);
                    int g = (int)(Convert.ToDouble(color.Attribute("g").Value.ToString()) * 255.0);
                    int b = (int)(Convert.ToDouble(color.Attribute("b").Value.ToString()) * 255.0);
                    colorList.Add(Color.FromArgb(r, g, b));
                }
            }
            
                
            foreach (var fontlist in doc.Element("CDXML").Elements().Where(el => el.Name.LocalName.Equals("fonttable")))
            {
                foreach(var font in fontlist.Elements().Where(el => el.Name.LocalName.Equals("font")))
                {
                    if (((string)font.Attribute("id") == null) || ((string)font.Attribute("name") == null)) continue;
                    
                    string fontId = font.Attribute("id").Value.ToString();
                    try
                    {
                        string fontName = font.Attribute("name").Value.ToString();                        
                        fonts.Add(fontId, fontName);
                    }
                    catch(Exception)
                    {
                        fonts.Add(fontId, "Arial");
                    }
                }
            }
            
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
                    string nodeId = (string)node.Attribute("id").Value;
                    
                    if (node.Elements().Count() == 0 || node.Elements().First().Elements().Count() == 0)
                    {
                        StructureNode sn = new StructureNode(new PointF(x, y));
                        nodes.Add(sn);
                        idToNode.Add(nodeId, sn);
                        continue;
                    }
                    
                    var t = node.Elements().First();
                    var s = t.Elements().First();
                    string text = s.Value.ToString();
                        
                    float fontSize = 10.0f;

                    // define the color
                    int colorCode = ((string)node.Attribute("color") != null) ? Convert.ToInt32(node.Attribute("color").Value.ToString()) - 2 : -1;
                    
                    SolidBrush mySolidBrush = new SolidBrush((0 <= colorCode && colorCode < colorList.Count) ? colorList[colorCode] : Color.Black);
                    
                    string fontCode = ((string)s.Attribute("font") != null) ? s.Attribute("font").Value.ToString() : "";
                    Font drawFont = (!fontCode.Equals("") && fonts.ContainsKey(fontCode)) ? new Font(fonts[fontCode], fontSize) : new Font("Arial", fontSize);
                    
                    RectangleF drawRect = new RectangleF(x, y, 0, 0);
                
                    if ((new HashSet<string>(){"R1", "R2", "R3", "R4"}).Contains(text))
                    {
                        StructureNode sn = new StructureNode(new PointF(x, y), drawRect, "R", mySolidBrush, drawFont);
                        x -= (float)Math.Abs(x * 0.02);
                        nodes.Add(sn);
                        idToNode.Add(nodeId, sn);
                        
                        Size size = TextRenderer.MeasureText(e.Graphics, "R", drawFont);
                        x += size.Width * 0.75f;
                        y += size.Height * 0.25f;
                        fontSize *= 0.5f;
                        string subIndex = text.Substring(1, 1);
                        
                        RectangleF subscript = new RectangleF(x, y, 0, 0);
                        Font subFont = (!fontCode.Equals("") && fonts.ContainsKey(fontCode)) ? new Font(fonts[fontCode], fontSize) : new Font("Arial", fontSize);
                        nodes.Add(new StructureNode(new PointF(x, y), subscript, subIndex, mySolidBrush, subFont));
                        
                    }
                    else 
                    {
                        StructureNode sn = new StructureNode(new PointF(x, y), drawRect, text, mySolidBrush, drawFont);
                        nodes.Add(sn);
                        idToNode.Add(nodeId, sn);
                    }
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
                    string nodeId = graphic.Elements().First().Attribute("object").Value.ToString();
                    
                    if (!idToNode.ContainsKey(nodeId)) continue;
                    
                    
                    string[] tokensBB = graphic.Attribute("BoundingBox").Value.Split(new char[]{' '});
                    float bx = (float)Convert.ToDouble(tokensBB[0]) - 1;
                    float by = (float)Convert.ToDouble(tokensBB[1]) - 1 - (symbol.Equals("+") ? 2 : 0);
                    float bw = Math.Abs((float)Convert.ToDouble(tokensBB[2]) - bx) + 5;
                    float bh = Math.Abs((float)Convert.ToDouble(tokensBB[3]) - by) + 3 + (symbol.Equals("+") ? 4 : 0);
                    RectangleF drawRect = new RectangleF(bx, by, 0, 0);
                    nodes.Add(new StructureNode(new PointF(bx, by), drawRect, symbol, idToNode[nodeId].solidBrush, idToNode[nodeId].font));                    
                }
                catch (Exception){}
            }
            
            
            
            // draw single and double bonds
            foreach (var edge in xedges.Where(el => ((string)el.Attribute("id") != null) && ((string)el.Attribute("B") != null) && ((string)el.Attribute("E") != null)))
            {
                string idB = edge.Attribute("B").Value;
                string idE = edge.Attribute("E").Value;
                if (!idToNode.ContainsKey(idB) || !idToNode.ContainsKey(idE)) continue;
                
                StructureNode nodeB = idToNode[idB];
                StructureNode nodeE = idToNode[idE];
                
                try
                {
                    float xB = nodeB.position.X;
                    float yB = nodeB.position.Y;
                    float xE = nodeE.position.X;
                    float yE = nodeE.position.Y;
                            
                    
                    // get the edge color
                    int colorCode = ((string)edge.Attribute("color") != null) ? Convert.ToInt32(edge.Attribute("color").Value.ToString()) - 2 : -1;
                    Pen colorPen = new Pen((0 <= colorCode && colorCode < colorList.Count) ? colorList[colorCode] : Color.Black, 1.0f * factor);
                    
                    bool isDoubleBond = ((string)edge.Attribute("Order") != null) && edge.Attribute("Order").Value.ToString().Equals("2");
                    
                    bool bondTypeNormal = ((string)edge.Attribute("BS") != null) && edge.Attribute("BS").Value.ToString().Equals("N");
                    
                    // Draw line to screen.
                    if (isDoubleBond)
                    {
                        float deltaX = yE - yB;
                        float deltaY = xB - xE;
                        float norming = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                        deltaX /= norming;
                        deltaY /= norming;
                        
                        if (bondTypeNormal)
                        {
                        
                            float x1B = xB - deltaX * dbSpace;
                            float y1B = yB - deltaY * dbSpace;
                            float x1E = xE - deltaX * dbSpace;
                            float y1E = yE - deltaY * dbSpace;
                            edges.Add(new StructureEdge(colorPen, new PointF(x1B, y1B), new PointF(x1E, y1E)));
                            
                            
                            float x2B = xB + deltaX * dbSpace;
                            float y2B = yB + deltaY * dbSpace;
                            float x2E = xE + deltaX * dbSpace;
                            float y2E = yE + deltaY * dbSpace;
                            edges.Add(new StructureEdge(colorPen, new PointF(x2B, y2B), new PointF(x2E, y2E)));
                        }
                        else
                        {
                        
                            float x1B = xB - deltaX * (2 * dbSpace);
                            float y1B = yB - deltaY * (2 * dbSpace);
                            float x1E = xE - deltaX * (2 * dbSpace);
                            float y1E = yE - deltaY * (2 * dbSpace);
                            edges.Add(new StructureEdge(colorPen, new PointF(x1B, y1B), new PointF(x1E, y1E)));
                            edges.Add(new StructureEdge(colorPen, new PointF(xB, yB), new PointF(xE, yE)));
                        }
                    }
                    else
                    {
                        edges.Add(new StructureEdge(colorPen, new PointF(xB, yB), new PointF(xE, yE)));
                    }
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
            offsetX = (float)pictureBoxFragments.Size.Width / 2.0f - midX * factor;
            offsetY = (float)pictureBoxFragments.Size.Height / 2.0f - midY * factor;
            
            foreach (var node in nodes)
            {
                float x = node.position.X * factor + offsetX;
                float y = node.position.Y * factor + offsetY;
                node.position = new PointF(x, y);
                
                if (node.font == null) continue;
                
                node.font = new Font(node.font.FontFamily, node.font.Size * factor);
                
                x = node.boundingBox.X * factor + offsetX;
                y = node.boundingBox.Y * factor + offsetY;
                
                Size size = TextRenderer.MeasureText(e.Graphics, node.text, node.font);
                float w = size.Width * 0.9f;
                float h = size.Height * 0.9f;
                node.boundingBox = new RectangleF(x - w * 0.5f, y - h * 0.5f, w, h);
                clippingAreas.Add(node.boundingBox);
            }
            
            
            
            foreach (var edge in edges)
            {
                float xs = edge.start.X;
                float ys = edge.start.Y;
                edge.start = new PointF(xs * factor + offsetX, ys * factor + offsetY);
                
                float xe = edge.end.X;
                float ye = edge.end.Y;
                edge.end = new PointF(xe * factor + offsetX, ye * factor + offsetY);
            }
        }
    }
    
    
    
    [Serializable]
    public partial class MS2Form : Form
    {
        
        public Image fragmentComplete = null;
        public Lipid currentLipid;
        public bool senderInterupt;
        public bool loading;
        public NewFragment newFragment;
        public CreatorGUI creatorGUI;
        [NonSerialized]
        public CheckedListBox editDeletePositive;
        public int editDeleteIndex;
        public int hoveredIndex;
        public LipidException lipidException;
        public Dictionary<string, LipidStructure> lipidStructures = new Dictionary<string, LipidStructure>();
        
        public string currentHeadgroup = "";
        public string currentFragment = "";
        public string currentPolarity = "";
        
        public Dictionary<string, string> f = new Dictionary<string, string>(){{"-(CH3[adduct])", "PC genNeg1"}, {"FA1(+O)", "PC genNeg2"}, {"FA2(+O)", "PC genNeg3"}, {"HG(PC,168)", "PC genNeg168"}, {"-FA1(+HO)-(CH3[adduct])", "PC genNeg-FA11"}, {"-FA2(+HO)-(CH3[adduct])", "PC genNeg-FA22"}, {"-FA1(-H)-(CH3[adduct])", "PC genNeg-FA1"}, {"-FA2(-H)-(CH3[adduct])", "PC genNeg-FA2"}};
        
        public MS2Form(CreatorGUI creatorGUI, LipidException _lipidException = null)
        {
            lipidException = _lipidException;
            senderInterupt = false;
            loading = false;
            hoveredIndex = -1;
            this.creatorGUI = creatorGUI;
            Lipid currentLipidTmp = creatorGUI.currentLipid;
            
            
            if (currentLipidTmp is Glycerolipid){
                this.currentLipid = new Glycerolipid((Glycerolipid)currentLipidTmp);
            }
            else if (currentLipidTmp is Phospholipid)
            {
                this.currentLipid = new Phospholipid((Phospholipid)currentLipidTmp);
            }
            else if (currentLipidTmp is Sphingolipid)
            {
                this.currentLipid = new Sphingolipid((Sphingolipid)currentLipidTmp);
            }
            else if (currentLipidTmp is Sterol)
            {
                this.currentLipid = new Sterol((Sterol)currentLipidTmp);
            }
            
            InitializeComponent();
            
            foreach (string lipidClass in creatorGUI.lipidCreator.categoryToClass[(int)creatorGUI.currentIndex])
            {
                if (!creatorGUI.lipidCreator.headgroups[lipidClass].attributes.Contains("heavy"))
                {
                    TabPage tp = new TabPage();
                    tp.Location = new System.Drawing.Point(4, 22);
                    tp.Name = lipidClass;
                    tp.Padding = new System.Windows.Forms.Padding(3);
                    tp.Size = new System.Drawing.Size(766, 392);
                    tp.TabIndex = 0;
                    tp.Text = lipidClass;
                    tp.UseVisualStyleBackColor = true;
                    this.tabControlFragments.Controls.Add(tp);
                    this.tabPages.Add(tp);
                }
            }
            if (tabPages.Count > 16) {
                tabControlFragments.Multiline = true;
                tabControlFragments.ItemSize = new Size(tabControlFragments.Width / 16 - 1, 20);
                tabControlFragments.SizeMode = TabSizeMode.Fixed;
            }
            
            tabChange(0);
        }
        
        
        
        public string getHeadgroup()
        {
            if (isotopeList.SelectedIndex == 0) return ((TabPage)tabPages[tabControlFragments.SelectedIndex]).Name;
            return ((string)isotopeList.Items[isotopeList.SelectedIndex]);
        }
        
        

        void checkedListBoxMouseLeave(object sender, EventArgs e)
        {
            if (currentHeadgroup.Equals("PC"))
            {
                pictureBoxFragments.Image = null;
            }
            else
            {
                pictureBoxFragments.Image = fragmentComplete;
            }
            hoveredIndex = -1;
            currentFragment = "";
            currentPolarity = "";
            pictureBoxFragments.Update();
            pictureBoxFragments.Refresh();
        }
        
        
        
        void checkedListBoxPositiveSelectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            currentLipid.positiveFragments[getHeadgroup()].Clear();
            for (int i = 0; i < checkedListBoxPositiveFragments.Items.Count; ++i)
            {
                currentLipid.positiveFragments[getHeadgroup()].Add((string)checkedListBoxPositiveFragments.Items[i]);
                checkedListBoxPositiveFragments.SetItemChecked(i, true);
            }
            senderInterupt = false;
        }
        
        
        
        void checkedListBoxNegativeSelectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            currentLipid.negativeFragments[getHeadgroup()].Clear();
            for (int i = 0; i < checkedListBoxNegativeFragments.Items.Count; ++i)
            {
                currentLipid.negativeFragments[getHeadgroup()].Add((string)checkedListBoxNegativeFragments.Items[i]);
                checkedListBoxNegativeFragments.SetItemChecked(i, true);
            }
            senderInterupt = false;
        }
        
        
        
        void checkedListBoxPositiveDeselectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            currentLipid.positiveFragments[getHeadgroup()].Clear();
            for (int i = 0; i < checkedListBoxPositiveFragments.Items.Count; ++i)
            {
                checkedListBoxPositiveFragments.SetItemChecked(i, false);
            }
            senderInterupt = false;
        }
        
        
        
        void checkedListBoxNegativeDeselectAll(object sender, EventArgs e)
        {
            senderInterupt = true;
            currentLipid.negativeFragments[getHeadgroup()].Clear();
            for (int i = 0; i < checkedListBoxNegativeFragments.Items.Count; ++i)
            {
                checkedListBoxNegativeFragments.SetItemChecked(i, false);
            }
            senderInterupt = false;
        }
        
        
        

        private void checkedListBoxPositiveMouseHover(object sender, MouseEventArgs e)
        {

            
            toolTip1.SetToolTip(this.checkedListBoxNegativeFragments, "");
            Point point = checkedListBoxPositiveFragments.PointToClient(Cursor.Position);
            int hIndex = checkedListBoxPositiveFragments.IndexFromPoint(point);

            if (hIndex != -1)
            {
                this.checkedListBoxPositiveFragments.ContextMenu = this.contextMenuFragment;
                string lipidClass = getHeadgroup();
                string fragmentName = (string)checkedListBoxPositiveFragments.Items[hIndex];
                currentFragment = fragmentName;
                currentPolarity = "+";
                MS2Fragment fragment = creatorGUI.lipidCreator.allFragments[lipidClass][true][fragmentName];
                menuFragmentItem1.Enabled = fragment.userDefined;
                menuFragmentItem2.Enabled = fragment.userDefined;
                if (fragment.fragmentFile != null && fragment.fragmentFile.Length > 0 && hoveredIndex != hIndex) pictureBoxFragments.Image = Image.FromFile(fragment.fragmentFile);
                hoveredIndex = hIndex;
                
                // create tool tip           
                string chemFormP = "";       
                string chemFormN = "";
                string baseName = "";
                string lBracket = "";
                string rBracket = "";
                
                if (fragment.fragmentBase.Count > 0)
                {
                    foreach (string bs in fragment.fragmentBase)
                    {
                        if (baseName.Length > 0) baseName += " + ";
                        baseName += bs;
                    }
                }
                
                
                for (int m = 0; m < fragment.fragmentElements.Count; ++m)
                {
                    int value = fragment.fragmentElements[m];
                    if (value > 0) chemFormP += MS2Fragment.ALL_ELEMENTS[(Molecule)m].shortcut + Convert.ToString(Math.Abs(value));
                    else if (value < 0) chemFormN += MS2Fragment.ALL_ELEMENTS[(Molecule)m].shortcut + Convert.ToString(Math.Abs(value));
                }
                string combinedChemForm = "";
                if (baseName.Length > 0 && (chemFormP.Length > 0 || chemFormN.Length > 0))
                {
                    if (chemFormP.Length > 0) combinedChemForm += " + " + chemFormP;
                    if (chemFormN.Length > 0) combinedChemForm += " - " + chemFormN;
                    lBracket = "(";
                    rBracket = ")";
                }
                string toolTipText = lBracket + baseName + combinedChemForm + rBracket + "+";
                toolTip1.SetToolTip(this.checkedListBoxPositiveFragments, toolTipText);
            }
            else
            {
                toolTip1.Hide(this.checkedListBoxPositiveFragments);
                this.checkedListBoxPositiveFragments.ContextMenu = null;
                
                if (currentHeadgroup.Equals("PC"))
                {
                    pictureBoxFragments.Image = null;
                }
                else
                {
                    pictureBoxFragments.Image = fragmentComplete;
                }
                hoveredIndex = -1;
                currentFragment = "";
                currentPolarity = "";
            }
        }
        
        
        

        private void checkedListBoxNegativeMouseHover(object sender, MouseEventArgs e)
        {

            toolTip1.SetToolTip(this.checkedListBoxPositiveFragments, "");
            Point point = checkedListBoxNegativeFragments.PointToClient(Cursor.Position);
            int hIndex = checkedListBoxNegativeFragments.IndexFromPoint(point);
            bool updateing = true;

            if (hIndex != -1)
            {
                this.checkedListBoxNegativeFragments.ContextMenu = this.contextMenuFragment;
                String lipidClass = getHeadgroup();
                string fragmentName = (string)checkedListBoxNegativeFragments.Items[hIndex];
                currentFragment = fragmentName;
                currentPolarity = "-";
                MS2Fragment fragment = creatorGUI.lipidCreator.allFragments[lipidClass][false][fragmentName];
                menuFragmentItem1.Enabled = fragment.userDefined;
                menuFragmentItem2.Enabled = fragment.userDefined;
                if (fragment.fragmentFile != null && fragment.fragmentFile.Length > 0 && hoveredIndex != hIndex)
                {
                    if (f.ContainsKey(currentFragment))
                    {
                        pictureBoxFragments.Image = null;
                    }
                    else
                    {
                        pictureBoxFragments.Image = Image.FromFile(fragment.fragmentFile);
                    }
                }
                hoveredIndex = hIndex;
                
                // create tool tip           
                string chemFormP = "";       
                string chemFormN = "";
                string baseName = "";
                string lBracket = "";
                string rBracket = "";
                
                if (fragment.fragmentBase.Count > 0)
                {
                    foreach (string bs in fragment.fragmentBase)
                    {
                        if (baseName.Length > 0) baseName += " + ";
                        baseName += bs;
                    }
                }
                
                for (int m = 0; m < fragment.fragmentElements.Count; ++m)
                {
                    int value = fragment.fragmentElements[m];
                    if (value > 0) chemFormP += MS2Fragment.ALL_ELEMENTS[(Molecule)m].shortcut + Convert.ToString(Math.Abs(value));
                    else if (value < 0) chemFormN += MS2Fragment.ALL_ELEMENTS[(Molecule)m].shortcut + Convert.ToString(Math.Abs(value));
                }
                string combinedChemForm = "";
                if (baseName.Length > 0 && (chemFormP.Length > 0 || chemFormN.Length > 0))
                {
                    if (chemFormP.Length > 0) combinedChemForm += " + " + chemFormP;
                    if (chemFormN.Length > 0) combinedChemForm += " - " + chemFormN;
                    lBracket = "(";
                    rBracket = ")";
                }
                string toolTipText = lBracket + baseName + combinedChemForm + rBracket + "-";
                toolTip1.SetToolTip(this.checkedListBoxNegativeFragments, toolTipText);
            }
            else
            {
                toolTip1.Hide(this.checkedListBoxNegativeFragments);
                this.checkedListBoxNegativeFragments.ContextMenu = null;
                if (currentHeadgroup.Equals("PC"))
                {
                    pictureBoxFragments.Image = null;
                }
                else
                {
                    pictureBoxFragments.Image = fragmentComplete;
                }
                hoveredIndex = -1;
                currentFragment = "";
                currentPolarity = "";
            }
            pictureBoxFragments.Update();
            pictureBoxFragments.Refresh();
        }
        
        
        
        public void contextMenuFragmentPopup(Object sender, EventArgs e)
        {
            editDeletePositive = (CheckedListBox)((ContextMenu)sender).SourceControl;
            Point point = editDeletePositive.PointToClient(Cursor.Position);
            editDeleteIndex = editDeletePositive.IndexFromPoint(point);
        }
        
        
        
        public void editFragment(Object sender, EventArgs e)
        {
            newFragment = new NewFragment(this, true);
            newFragment.Owner = this;
            newFragment.ShowInTaskbar = false;
            if (creatorGUI.tutorial.tutorial == Tutorials.NoTutorial)
            {
                newFragment.ShowDialog();
                newFragment.Dispose();
            }
            else
            {
                newFragment.Show();
            }
            
        }
        
        
        
        public void deleteFragment(Object sender, EventArgs e)
        {
            string lipidClass = getHeadgroup();
            string fragmentName = (string)editDeletePositive.Items[editDeleteIndex];
            bool isPositive = editDeletePositive.Name.Equals("checkedListBoxPositive");
            bool userDefined = creatorGUI.lipidCreator.allFragments[lipidClass][isPositive][fragmentName].userDefined;
            if (userDefined){
                if (isPositive){
                    currentLipid.positiveFragments[lipidClass].Remove(fragmentName);
                    checkedListBoxPositiveFragments.Items.RemoveAt(editDeleteIndex);
                }
                else {
                    currentLipid.negativeFragments[lipidClass].Remove(fragmentName);
                    checkedListBoxNegativeFragments.Items.RemoveAt(editDeleteIndex);
                }
                creatorGUI.lipidCreator.allFragments[lipidClass][isPositive].Remove(fragmentName);
            }
        }
        
        

        public void tabIndexChanged(Object sender, EventArgs e)
        {
            tabChange(((TabControl)sender).SelectedIndex);
        }
        
        
        
        
        public void isotopeListComboBoxValueChanged(object sender, EventArgs e)
        {
            if (loading) return;
            String lipidClass = getHeadgroup();
            
            checkedListBoxPositiveFragments.Items.Clear();
            checkedListBoxNegativeFragments.Items.Clear();
            
            foreach (KeyValuePair<string, MS2Fragment> currentFragment in creatorGUI.lipidCreator.allFragments[lipidClass][true])
            {
                checkedListBoxPositiveFragments.Items.Add(currentFragment.Value.fragmentName);
                checkedListBoxPositiveFragments.SetItemChecked(checkedListBoxPositiveFragments.Items.Count - 1, currentLipid.positiveFragments[lipidClass].Contains(currentFragment.Value.fragmentName));
            }
            foreach (KeyValuePair<string, MS2Fragment> currentFragment in creatorGUI.lipidCreator.allFragments[lipidClass][false])
            {
                checkedListBoxNegativeFragments.Items.Add(currentFragment.Value.fragmentName);
                checkedListBoxNegativeFragments.SetItemChecked(checkedListBoxNegativeFragments.Items.Count - 1, currentLipid.negativeFragments[lipidClass].Contains(currentFragment.Value.fragmentName));
            }
            
            if (creatorGUI.lipidCreator.headgroups.ContainsKey(lipidClass) && creatorGUI.lipidCreator.headgroups[lipidClass].pathToImage.Length > 0)
            {
                fragmentComplete = Image.FromFile(creatorGUI.lipidCreator.headgroups[lipidClass].pathToImage);
                if (currentHeadgroup.Equals("PC"))
                {
                    pictureBoxFragments.Image = null;
                }
                else
                {
                    pictureBoxFragments.Image = fragmentComplete;
                }
            }
            else
            {
                if (fragmentComplete != null)
                {
                    fragmentComplete = null;
                }
                
                if (pictureBoxFragments.Image != null)
                {
                    pictureBoxFragments.Image.Dispose();
                    pictureBoxFragments.Image = null;
                }
            }
        }
        
        
        
        
        private void Form_Shown(Object sender, EventArgs e)
        {
            if (lipidException == null) return;
            
            // search for precursor
            for (int i = 0; i < tabControlFragments.TabCount; ++i)
            {
                if (tabControlFragments.TabPages[i].Text == lipidException.precursorData.moleculeListName)
                {
                    tabControlFragments.SelectedIndex = i;
                    // search for isotope
                    for (int j = 1; j < isotopeList.Items.Count; ++j)
                    {
                        string isotopeName = LipidCreator.precursorNameSplit((string)isotopeList.Items[j])[1];
                        if (isotopeName.Equals(lipidException.heavyIsotope))
                        {
                            isotopeList.SelectedIndex = j;
                            
                            // search for fragment
                            string fragmentName = lipidException.fragment.fragmentName;
                            CheckedListBox clb = null;
                            if (lipidException.fragment.fragmentAdduct.charge > 0)
                            {
                                clb = checkedListBoxPositiveFragments;
                            }
                            else
                            {
                                clb = checkedListBoxNegativeFragments;
                            }
                            editDeletePositive = clb;
                            
                            for (int k = 0; k < clb.Items.Count; ++k)
                            {
                                if (fragmentName.Equals((string)clb.Items[k]))
                                {
                                    clb.SelectedIndex = k;
                                    editDeleteIndex = k;
                                    newFragment = new NewFragment(this, true, lipidException);
                                    newFragment.Owner = this;
                                    newFragment.ShowInTaskbar = false;
                                    newFragment.ShowDialog();
                                    newFragment.Dispose();
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }

        }

        
        
        
        
        

        public void tabChange(int index)
        {
            loading = true;
            isotopeList.Items.Clear();
            ((TabPage)tabPages[index]).Controls.Add(checkedListBoxNegativeFragments);
            ((TabPage)tabPages[index]).Controls.Add(labelPositiveFragments);
            ((TabPage)tabPages[index]).Controls.Add(labelNegativeFragments);
            ((TabPage)tabPages[index]).Controls.Add(labelFragmentDescriptionBlack);
            ((TabPage)tabPages[index]).Controls.Add(labelFragmentDescriptionRed);
            ((TabPage)tabPages[index]).Controls.Add(labelFragmentDescriptionBlue);
            ((TabPage)tabPages[index]).Controls.Add(labelPositiveSelectAll);
            ((TabPage)tabPages[index]).Controls.Add(labelPositiveDeselectAll);
            ((TabPage)tabPages[index]).Controls.Add(labelNegativeSelectAll);
            ((TabPage)tabPages[index]).Controls.Add(labelNegativeDeselectAll);
            ((TabPage)tabPages[index]).Controls.Add(labelSlashPositive);
            ((TabPage)tabPages[index]).Controls.Add(labelSlashNegative);
            ((TabPage)tabPages[index]).Controls.Add(checkedListBoxPositiveFragments);
            ((TabPage)tabPages[index]).Controls.Add(pictureBoxFragments);
            ((TabPage)tabPages[index]).Controls.Add(isotopeList);
            
            isotopeList.Items.Add("Monoisotopic");
            String lipidClass = ((TabPage)tabPages[tabControlFragments.SelectedIndex]).Name;
            foreach(Precursor heavyPrecursor in creatorGUI.lipidCreator.headgroups[lipidClass].heavyLabeledPrecursors)
            {
                isotopeList.Items.Add(heavyPrecursor.name);
            }
            
            loading = false;
            isotopeList.SelectedIndex = 0;
            currentHeadgroup = getHeadgroup();
            currentFragment = "";
            currentPolarity = "";
        }


        public void CheckedListBoxPositiveItemCheck(Object sender, ItemCheckEventArgs e)
        {
            if (senderInterupt) return;
            if (e.NewValue == CheckState.Checked)
            {
                currentLipid.positiveFragments[getHeadgroup()].Add((string)checkedListBoxPositiveFragments.Items[e.Index]);
            }
            else
            {
                currentLipid.positiveFragments[getHeadgroup()].Remove((string)checkedListBoxPositiveFragments.Items[e.Index]);
            }
        }
        
        
        public void CheckedListBoxNegativeItemCheck(Object sender, ItemCheckEventArgs e)
        {
            if (senderInterupt) return;
            if (e.NewValue == CheckState.Checked)
            {
                currentLipid.negativeFragments[getHeadgroup()].Add((string)checkedListBoxNegativeFragments.Items[e.Index]);
            }
            else
            {
                currentLipid.negativeFragments[getHeadgroup()].Remove((string)checkedListBoxNegativeFragments.Items[e.Index]);
            }
        }
        
        
        
        
        
        
        private void cancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
        
        
        
        
        
        
        private void okClick(object sender, EventArgs e)
        {
            if (currentLipid is Glycerolipid)
            {
                creatorGUI.lipidTabList[(int)LipidCategory.Glycerolipid] = new Glycerolipid((Glycerolipid)currentLipid);
                creatorGUI.currentLipid = (Lipid)creatorGUI.lipidTabList[(int)LipidCategory.Glycerolipid];
            }
            else if (currentLipid is Phospholipid)
            {
                creatorGUI.lipidTabList[(int)LipidCategory.Glycerophospholipid] = new Phospholipid((Phospholipid)currentLipid);
                creatorGUI.currentLipid = (Lipid)creatorGUI.lipidTabList[(int)LipidCategory.Glycerophospholipid];
            }
            else if (currentLipid is Sphingolipid)
            {
                creatorGUI.lipidTabList[(int)LipidCategory.Sphingolipid] = new Sphingolipid((Sphingolipid)currentLipid);
                creatorGUI.currentLipid = (Lipid)creatorGUI.lipidTabList[(int)LipidCategory.Sphingolipid];
            }
            else if (currentLipid is Sterol)
            {
                creatorGUI.lipidTabList[(int)LipidCategory.Sterollipid] = new Sterol((Sterol)currentLipid);
                creatorGUI.currentLipid = (Lipid)creatorGUI.lipidTabList[(int)LipidCategory.Sterollipid];
            }
            this.Close();
        }
        
        
        
        
        
        
        private void addFragmentClick(object sender, EventArgs e)
        {
            newFragment = new NewFragment(this);
            newFragment.Owner = this;
            newFragment.ShowInTaskbar = false;
            
            if (creatorGUI.tutorial.tutorial == Tutorials.NoTutorial)
            {
                newFragment.ShowDialog();
                newFragment.Dispose();
            }
            else
            {
                newFragment.Show();
            }
            tabChange(tabControlFragments.SelectedIndex);
        }
        
        
        
        
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
        
        
        
        
        public void test_Paint(object sender, PaintEventArgs e)
        {
            
            
            
            if (!currentHeadgroup.Equals("PC") || (!f.ContainsKey(currentFragment) && !currentFragment.Equals(""))) return;
            
            
            string structureId = currentHeadgroup + (!currentFragment.Equals("") ? "/" + currentFragment + currentPolarity : "");
            LipidStructure lipidStructure = null;
            if (!lipidStructures.ContainsKey(structureId))
            {
                
                string root = "/home/dominik/workspace/src/LipidCreator/LipidCreator/data/structures/";
                
                string file_name = "PC genNeg";
                if (!currentFragment.Equals(""))
                {
                    file_name = f[currentFragment];
                }
                file_name = root + file_name + ".cdxml";
                lipidStructure = new LipidStructure(file_name, e, pictureBoxFragments);
                lipidStructures.Add(structureId, lipidStructure);
            }
            else
            {
                lipidStructure = lipidStructures[structureId];
            }
            
            
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;
            drawFormat.LineAlignment = StringAlignment.Center;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            
            foreach (var node in lipidStructure.nodes)
            {
                if (node.text.Equals("")) continue;
                e.Graphics.DrawString(node.text, node.font, node.solidBrush, node.boundingBox, drawFormat);
            }
            
            // set up all clipping
            Graphics clippingGraphics = this.CreateGraphics();
            foreach (var clippingArea in lipidStructure.clippingAreas)
            {
                clippingGraphics.SetClip(clippingArea);
                e.Graphics.SetClip(clippingGraphics, CombineMode.Exclude);
            }
            
            foreach (var edge in lipidStructure.edges)
            {
                e.Graphics.DrawLine(edge.pen, edge.start, edge.end);
            }
            
            clippingGraphics.Dispose();
            
        }
    }
}
