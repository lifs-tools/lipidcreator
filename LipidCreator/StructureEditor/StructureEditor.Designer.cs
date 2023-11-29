using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;


namespace LipidCreatorStructureEditor
{
    
    public class InputBox : Form
    {
        TextBox inputBox = new TextBox();
        Label message = new Label();
        Button okButton = new Button();
        Button cancelButton = new Button();
        
        public InputBox(string messageText = "", string title = "", string preText = "")
        {
            this.Controls.Add(inputBox);
            this.Controls.Add(message);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
            
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.ClientSize = new System.Drawing.Size(300, 160);
            this.Text = title;
            
            message.Location = new Point(10, 10);
            message.Size = new Size(230, 60);
            message.Text = messageText;
            
            inputBox.Location = new Point(10, 80);
            inputBox.Size = new Size(200, 25);
            inputBox.Text = preText;
            
            cancelButton.Size = new Size(100, 30);
            cancelButton.Location = new Point(190, 120);
            cancelButton.Text = "&Cancel";
            cancelButton.Click += cancelClicked;
            
            okButton.Size = new Size(100, 30);
            okButton.Location = new Point(80, 120);
            okButton.Text = "&Ok";
            okButton.Click += okClicked;
            
            ShowDialog();
        }
        
        public void okClicked(Object sender, EventArgs e)
        {
            Close();
        }
        
        public void cancelClicked(Object sender, EventArgs e)
        {
            inputBox.Text = "";
            Close();
        }
        
        public static string Show(string messageText = "", string title = "", string preText = "")
        {
            InputBox inputBox = new InputBox(messageText, title, preText);
            return inputBox.inputBox.Text;
        }
    }

    partial class LipidCreatorStructureEditor
    {

        public ListBox positiveFragmentsListBox = new ListBox();
        public ListBox negativeFragmentsListBox = new ListBox();
        
        public Button actionChangeAtomState = new Button();
        public Button actionChangeAtom = new Button();
        public Button actionChangeBondState = new Button();
        public Button actionChangeGlobalCharge = new Button();
        public Button actionDrawAtom = new Button();
        public Button actionDrawBond = new Button();
        public Button actionRemoveAtom = new Button();
        public Button actionRemoveBond = new Button();
        public Button actionFinalView = new Button();
        public Button actionMoveAtom = new Button();
        
        public List<Button> actionButtons = new List<Button>();
        
        public TextBox bb1Carbon = new TextBox();
        public TextBox bb1DB = new TextBox();
        public TextBox bb1Hydro = new TextBox();
        
        public TextBox bb2Carbon = new TextBox();
        public TextBox bb2DB = new TextBox();
        public TextBox bb2Hydro = new TextBox();
        
        public TextBox bb3Carbon = new TextBox();
        public TextBox bb3DB = new TextBox();
        public TextBox bb3Hydro = new TextBox();
        
        public TextBox bb4Carbon = new TextBox();
        public TextBox bb4DB = new TextBox();
        public TextBox bb4Hydro = new TextBox();
        
        public Label massLabel = new Label();
        public Label sumFormulaLabel = new Label();
        
        public ComboBox adductComboBox = new ComboBox();
        
        public Button addPositiveFragmentButton = new Button();
        public Button removePositiveFragmentButton = new Button();
        public Button addNegativeFragmentButton = new Button();
        public Button removeNegativeFragmentButton = new Button();
        
        private void InitializeComponent()
        {
            this.Text = "LipidCreator Structure Editor";
            this.Width = 1400;
            this.Height = 900;
            
            this.BackColor = Color.White;
            this.MouseMove += mouseMove;
            this.MouseUp += mouseUp;
            this.MouseDown += mouseDown;
            
            
            this.Controls.Add(actionChangeAtomState);
            actionChangeAtomState.Size = new Size(120, 25);
            actionChangeAtomState.Text = "Change atom state";
            actionChangeAtomState.Location = new Point(10, 10);
            actionChangeAtomState.Click += actionChangeAtomStateClicked;
            
            this.Controls.Add(actionChangeAtom);
            actionChangeAtom.Size = new Size(120, 25);
            actionChangeAtom.Text = "Change atom";
            actionChangeAtom.Location = new Point(10, actionChangeAtomState.Top + 35);
            actionChangeAtom.Click += actionChangeAtomClicked;
            
            
            this.Controls.Add(actionChangeBondState);
            actionChangeBondState.Size = new Size(120, 25);
            actionChangeBondState.Text = "Change bond state";
            actionChangeBondState.Location = new Point(10, actionChangeAtom.Top + 35);
            actionChangeBondState.Click += actionChangeBondStateClicked;
            
            
            this.Controls.Add(actionChangeGlobalCharge);
            actionChangeGlobalCharge.Size = new Size(120, 25);
            actionChangeGlobalCharge.Text = "Change global charge";
            actionChangeGlobalCharge.Location = new Point(10, actionChangeBondState.Top + 35);
            actionChangeGlobalCharge.Click += actionChangeGlobalChargeClicked;
            
            this.Controls.Add(actionDrawAtom);
            actionDrawAtom.Size = new Size(120, 25);
            actionDrawAtom.Text = "Draw atom";
            actionDrawAtom.Location = new Point(10, actionChangeGlobalCharge.Top + 35);
            actionDrawAtom.Click += actionDrawAtomClicked;
            
            this.Controls.Add(actionDrawBond);
            actionDrawBond.Size = new Size(120, 25);
            actionDrawBond.Text = "Draw bond";
            actionDrawBond.Location = new Point(10, actionDrawAtom.Top + 35);
            actionDrawBond.Click += actionDrawBondClicked;
            
            this.Controls.Add(actionRemoveAtom);
            actionRemoveAtom.Size = new Size(120, 25);
            actionRemoveAtom.Text = "Remove atom";
            actionRemoveAtom.Location = new Point(10, actionDrawBond.Top + 35);
            actionRemoveAtom.Click += actionRemoveAtomClicked;
            
            this.Controls.Add(actionRemoveBond);
            actionRemoveBond.Size = new Size(120, 25);
            actionRemoveBond.Text = "Remove bond";
            actionRemoveBond.Location = new Point(10, actionRemoveAtom.Top + 35);
            actionRemoveBond.Click += actionRemoveBondClicked;
            
            this.Controls.Add(actionMoveAtom);
            actionMoveAtom.Size = new Size(120, 25);
            actionMoveAtom.Text = "Move atom";
            actionMoveAtom.Location = new Point(10, actionRemoveBond.Top + 35);
            actionMoveAtom.Click += actionMoveAtomClicked;
            
            this.Controls.Add(actionFinalView);
            actionFinalView.Size = new Size(120, 25);
            actionFinalView.Text = "Final View";
            actionFinalView.Location = new Point(10, actionMoveAtom.Top + 55);
            actionFinalView.Click += actionFinalViewClicked;
            
            
            actionButtons.Add(actionChangeAtomState);
            actionButtons.Add(actionChangeAtom);
            actionButtons.Add(actionChangeBondState);
            actionButtons.Add(actionDrawAtom);
            actionButtons.Add(actionDrawBond);
            actionButtons.Add(actionRemoveAtom);
            actionButtons.Add(actionRemoveBond);
            actionButtons.Add(actionMoveAtom);
            
            
            this.Controls.Add(positiveFragmentsListBox);
            positiveFragmentsListBox.Width = 130;
            positiveFragmentsListBox.Height = 300;
            positiveFragmentsListBox.Location = new Point(10, actionFinalView.Top + 35);
            positiveFragmentsListBox.KeyUp += fragmentKeyPressed;
            positiveFragmentsListBox.SelectedIndexChanged += fragmentClicked;
            positiveFragmentsListBox.DoubleClick += positiveFragmentDoubleClicked;
            
            this.Controls.Add(negativeFragmentsListBox);
            negativeFragmentsListBox.Width = 130;
            negativeFragmentsListBox.Height = 300;
            negativeFragmentsListBox.Location = new Point(150, actionFinalView.Top + 35);
            negativeFragmentsListBox.KeyUp += fragmentKeyPressed;
            negativeFragmentsListBox.SelectedIndexChanged += fragmentClicked;
            negativeFragmentsListBox.DoubleClick += negativeFragmentDoubleClicked;
            
            this.Controls.Add(removePositiveFragmentButton);
            removePositiveFragmentButton.Location = new Point(positiveFragmentsListBox.Left + positiveFragmentsListBox.Width - 25, positiveFragmentsListBox.Top + positiveFragmentsListBox.Height);
            removePositiveFragmentButton.Size = new Size(25, 25);
            removePositiveFragmentButton.Text = "-";
            removePositiveFragmentButton.Click += removePositiveFragment;
            
            
            this.Controls.Add(addPositiveFragmentButton);
            addPositiveFragmentButton.Location = new Point(removePositiveFragmentButton.Left - removePositiveFragmentButton.Width, removePositiveFragmentButton.Top);
            addPositiveFragmentButton.Size = new Size(25, 25);
            addPositiveFragmentButton.Text = "+";
            addPositiveFragmentButton.Click += addPositiveFragment;
            
            this.Controls.Add(removeNegativeFragmentButton);
            removeNegativeFragmentButton.Location = new Point(negativeFragmentsListBox.Left + negativeFragmentsListBox.Width - 25, negativeFragmentsListBox.Top + negativeFragmentsListBox.Height);
            removeNegativeFragmentButton.Size = new Size(25, 25);
            removeNegativeFragmentButton.Text = "-";
            removeNegativeFragmentButton.Click += removeNegativeFragment;
            
            this.Controls.Add(addNegativeFragmentButton);
            addNegativeFragmentButton.Location = new Point(removeNegativeFragmentButton.Left - removeNegativeFragmentButton.Width, removeNegativeFragmentButton.Top);
            addNegativeFragmentButton.Size = new Size(25, 25);
            addNegativeFragmentButton.Text = "+";
            addNegativeFragmentButton.Click += addNegativeFragment;
            
            
            
            
            this.Controls.Add(bb1Carbon);
            bb1Carbon.Location = new Point(200, 10);
            bb1Carbon.Size = new Size(120, 25);
            bb1Carbon.Text = "18";
            bb1Carbon.TextChanged += computeFragmentMass;
            
            this.Controls.Add(bb1DB);
            bb1DB.Location = new Point(200, 40);
            bb1DB.Size = new Size(120, 25);
            bb1DB.Text = "0";
            bb1DB.TextChanged += computeFragmentMass;
            
            this.Controls.Add(bb1Hydro);
            bb1Hydro.Location = new Point(200, 70);
            bb1Hydro.Size = new Size(120, 25);
            bb1Hydro.Text = "0";
            bb1Hydro.TextChanged += computeFragmentMass;
            
            this.Controls.Add(massLabel);
            massLabel.Location = new Point(200, 100);
            massLabel.Size = new Size(220, 25);
            
            this.Controls.Add(sumFormulaLabel);
            sumFormulaLabel.Location = new Point(200, 120);
            sumFormulaLabel.Size = new Size(220, 25);
            sumFormulaLabel.BringToFront();
            
            
            
            this.Controls.Add(bb2Carbon);
            bb2Carbon.Location = new Point(350, 10);
            bb2Carbon.Size = new Size(120, 25);
            bb2Carbon.Text = "18";
            bb2Carbon.TextChanged += computeFragmentMass;
            
            this.Controls.Add(bb2DB);
            bb2DB.Location = new Point(350, 40);
            bb2DB.Size = new Size(120, 25);
            bb2DB.Text = "0";
            bb2DB.TextChanged += computeFragmentMass;
            
            this.Controls.Add(bb2Hydro);
            bb2Hydro.Location = new Point(350, 70);
            bb2Hydro.Size = new Size(120, 25);
            bb2Hydro.Text = "0";
            bb2Hydro.TextChanged += computeFragmentMass;
            
            
            
            this.Controls.Add(bb3Carbon);
            bb3Carbon.Location = new Point(500, 10);
            bb3Carbon.Size = new Size(120, 25);
            bb3Carbon.Text = "18";
            bb3Carbon.TextChanged += computeFragmentMass;
            
            this.Controls.Add(bb3DB);
            bb3DB.Location = new Point(500, 40);
            bb3DB.Size = new Size(120, 25);
            bb3DB.Text = "0";
            bb3DB.TextChanged += computeFragmentMass;
            
            this.Controls.Add(bb3Hydro);
            bb3Hydro.Location = new Point(500, 70);
            bb3Hydro.Size = new Size(120, 25);
            bb3Hydro.Text = "0";
            bb3Hydro.TextChanged += computeFragmentMass;
            
            
            
            this.Controls.Add(bb4Carbon);
            bb4Carbon.Location = new Point(650, 10);
            bb4Carbon.Size = new Size(120, 25);
            bb4Carbon.Text = "18";
            bb4Carbon.TextChanged += computeFragmentMass;
            
            this.Controls.Add(bb4DB);
            bb4DB.Location = new Point(650, 40);
            bb4DB.Size = new Size(120, 25);
            bb4DB.Text = "0";
            bb4DB.TextChanged += computeFragmentMass;
            
            this.Controls.Add(bb4Hydro);
            bb4Hydro.Location = new Point(650, 70);
            bb4Hydro.Size = new Size(120, 25);
            bb4Hydro.Text = "0";
            bb4Hydro.TextChanged += computeFragmentMass;
            
            
            this.Controls.Add(adductComboBox);
            adductComboBox.Location = new Point(800, 10);
            adductComboBox.Size = new Size(120, 25);
            adductComboBox.Items.Add("+H+");
            adductComboBox.Items.Add("+2H+");
            adductComboBox.Items.Add("+NH4+");
            adductComboBox.Items.Add("-H-");
            adductComboBox.Items.Add("-2H-");
            adductComboBox.Items.Add("+HC00-");
            adductComboBox.Items.Add("+CH3C00-");
            adductComboBox.SelectedIndex = 0;
            adductComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            adductComboBox.SelectedIndexChanged += computeFragmentMass;
        }
    }
}
