using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace LipidCreator
{
    public class fatty_acid : IComparable<fatty_acid>
    {
        public int length;
        public int db;

        public fatty_acid(int l, int _db)
        {
            length = l;
            db = _db;
        }
        
        public fatty_acid(fatty_acid copy)
        {
            length = copy.length;
            db = copy.length;
        }

        public int CompareTo(fatty_acid other)
        {
            if (length != other.length)
            {
                return length - other.length;
            }
            else if (db != other.db)
            {
                return db - other.db;
            }
            return 0;
        }
    }

    public class fattyAcidGroup
    {
        public int chainType; // 0 = no restriction, 1 = odd carbon number, 2 = even carbon number
        public String lengthInfo;
        public String dbInfo;
        public Dictionary<String, bool> faTypes;
    
        public fattyAcidGroup()
        {
            chainType = 0;
            lengthInfo = "2-5";
            dbInfo = "0-1";
            faTypes = new Dictionary<String, bool>();
            faTypes.Add("FA", true);
            faTypes.Add("FAp", false);
            faTypes.Add("FAe", false);
            faTypes.Add("FAh", false);
        }
        
        public fattyAcidGroup(fattyAcidGroup copy)
        {
            chainType = copy.chainType;
            lengthInfo = copy.lengthInfo;
            dbInfo = copy.dbInfo;
            faTypes = new Dictionary<String, bool>();
            faTypes.Add("FA", copy.faTypes["FA"]);
            faTypes.Add("FAp", copy.faTypes["FAp"]);
            faTypes.Add("FAe", copy.faTypes["FAe"]);
            faTypes.Add("FAh", copy.faTypes["FAh"]);
        }
    }

    public class MS2Fragment
    {
        public String fragmentName;
        public int fragmentCharge;
        public String fragmentFile;
        public bool fragmentSelected;
        public DataTable fragmentElements;
    
    
        public static DataTable createEmptyElementTable()
        {
            DataTable elements = new DataTable();
            elements.Clear();
            String count = "Count";
            String shortcut = "Shortcut";
            String element = "Element";
            String monoMass = "mass";

            DataColumn columnCount = elements.Columns.Add(count);
            DataColumn columnShortcut = elements.Columns.Add(shortcut);
            DataColumn columnElement = elements.Columns.Add(element);
            DataColumn columnMass = elements.Columns.Add(monoMass);

            columnCount.DataType = System.Type.GetType("System.Int32");
            columnShortcut.ReadOnly = true;
            columnElement.ReadOnly = true;

            DataRow carbon = elements.NewRow();
            carbon[count] = "0";
            carbon[shortcut] = "C";
            carbon[element] = "carbon";
            carbon[monoMass] = 12;
            elements.Rows.Add(carbon);

            DataRow hydrogen = elements.NewRow();
            hydrogen[count] = "0";
            hydrogen[shortcut] = "H";
            hydrogen[element] = "hydrogen";
            hydrogen[monoMass] = 1.007276;
            elements.Rows.Add(hydrogen);

            DataRow oxygen = elements.NewRow();
            oxygen[count] = "0";
            oxygen[shortcut] = "O";
            oxygen[element] = "oxygen";
            oxygen[monoMass] = 15.994915;
            elements.Rows.Add(oxygen);

            DataRow nitrogen = elements.NewRow();
            nitrogen[count] = "0";
            nitrogen[shortcut] = "N";
            nitrogen[element] = "nitrogen";
            nitrogen[monoMass] = 14.003074;
            elements.Rows.Add(nitrogen);

            DataRow phosphor = elements.NewRow();
            phosphor[count] = "0";
            phosphor[shortcut] = "P";
            phosphor[element] = "phosphor";
            phosphor[monoMass] = 30.973763;
            elements.Rows.Add(phosphor);

            DataRow sulfur = elements.NewRow();
            sulfur[count] = "0";
            sulfur[shortcut] = "S";
            sulfur[element] = "sulfur";
            sulfur[monoMass] = 31.972072;
            elements.Rows.Add(sulfur);

            DataRow sodium = elements.NewRow();
            sodium[count] = "0";
            sodium[shortcut] = "Na";
            sodium[element] = "sodium";
            sodium[monoMass] = 22.989770;
            elements.Rows.Add(sodium);
            return elements;
        }
        
        public static DataTable createEmptyElementTable(DataTable copy)
        {
            DataTable elements = new DataTable();
            elements.Clear();
            String count = "Count";
            String shortcut = "Shortcut";
            String element = "Element";
            String monoMass = "mass";

            DataColumn columnCount = elements.Columns.Add(count);
            DataColumn columnShortcut = elements.Columns.Add(shortcut);
            DataColumn columnElement = elements.Columns.Add(element);
            DataColumn columnMass = elements.Columns.Add(monoMass);

            columnCount.DataType = System.Type.GetType("System.Int32");
            columnShortcut.ReadOnly = true;
            columnElement.ReadOnly = true;

            DataRow carbon = elements.NewRow();
            carbon[count] = copy.Rows[0][count];
            carbon[shortcut] = "C";
            carbon[element] = "carbon";
            carbon[monoMass] = 12;
            elements.Rows.Add(carbon);

            DataRow hydrogen = elements.NewRow();
            hydrogen[count] = copy.Rows[1][count];
            hydrogen[shortcut] = "H";
            hydrogen[element] = "hydrogen";
            hydrogen[monoMass] = 1.007276;
            elements.Rows.Add(hydrogen);

            DataRow oxygen = elements.NewRow();
            oxygen[count] = copy.Rows[2][count];
            oxygen[shortcut] = "O";
            oxygen[element] = "oxygen";
            oxygen[monoMass] = 15.994915;
            elements.Rows.Add(oxygen);

            DataRow nitrogen = elements.NewRow();
            nitrogen[count] = copy.Rows[3][count];
            nitrogen[shortcut] = "N";
            nitrogen[element] = "nitrogen";
            nitrogen[monoMass] = 14.003074;
            elements.Rows.Add(nitrogen);

            DataRow phosphor = elements.NewRow();
            phosphor[count] = copy.Rows[4][count];
            phosphor[shortcut] = "P";
            phosphor[element] = "phosphor";
            phosphor[monoMass] = 30.973763;
            elements.Rows.Add(phosphor);

            DataRow sulfur = elements.NewRow();
            sulfur[count] = copy.Rows[5][count];
            sulfur[shortcut] = "S";
            sulfur[element] = "sulfur";
            sulfur[monoMass] = 31.972072;
            elements.Rows.Add(sulfur);

            DataRow sodium = elements.NewRow();
            sodium[count] = copy.Rows[6][count];
            sodium[shortcut] = "Na";
            sodium[element] = "sodium";
            sodium[monoMass] = 22.989770;
            elements.Rows.Add(sodium);
            return elements;
        }
    

        public MS2Fragment(String name, String fileName)
        {
            fragmentName = name;
            fragmentCharge = -1;
            fragmentFile = fileName;
            fragmentSelected = true;
            fragmentElements = createEmptyElementTable();
        }
    

        public MS2Fragment(String name, String fileName, int charge)
        {
            fragmentName = name;
            fragmentCharge = charge;
            fragmentFile = fileName;
            fragmentSelected = true;
            fragmentElements = createEmptyElementTable();
        }

        public MS2Fragment(String name, int charge, String fileName, bool selected, DataTable dataElements)
        {
            fragmentName = name;
            fragmentCharge = charge;
            fragmentFile = fileName;
            fragmentSelected = selected;
            fragmentElements = dataElements;
        }

        public MS2Fragment(MS2Fragment copy)
        {
            fragmentName = copy.fragmentName;
            fragmentCharge = copy.fragmentCharge;
            fragmentFile = copy.fragmentFile;
            fragmentSelected = copy.fragmentSelected;
            fragmentElements = createEmptyElementTable(copy.fragmentElements);
        }
    }


    public class lipid
    {
        public string className;
        public TabPage text_page;
        public Dictionary<String, ArrayList> MS2Fragments;
        public Dictionary<String, String> paths_to_full_image;
        public String completeName;
        public Dictionary<String, bool> adducts;
    
        public lipid(){
            adducts = new Dictionary<String, bool>();
            adducts.Add("+H", true);
            adducts.Add("+2H", false);
            adducts.Add("+NH4", false);
            adducts.Add("+Na", false);
            adducts.Add("-H", false);
            adducts.Add("-2H", false);
            adducts.Add("+HCOO", false);
            adducts.Add("+CH3COO", false);
            MS2Fragments = new Dictionary<String, ArrayList>();
            paths_to_full_image = new Dictionary<String, String>();
        }
        
        public lipid(lipid copy)
        {
            adducts = new Dictionary<String, bool>();
            adducts.Add("+H", copy.adducts["+H"]);
            adducts.Add("+2H", copy.adducts["+2H"]);
            adducts.Add("+NH4", copy.adducts["+NH4"]);
            adducts.Add("+Na", copy.adducts["+Na"]);
            adducts.Add("-H", copy.adducts["-H"]);
            adducts.Add("-2H", copy.adducts["-2H"]);
            adducts.Add("+HCOO", copy.adducts["+HCOO"]);
            adducts.Add("+CH3COO", copy.adducts["+CH3COO"]);
            className = copy.className;
            MS2Fragments = new Dictionary<String, ArrayList>();
            paths_to_full_image = new Dictionary<String, String>();
            foreach (KeyValuePair<String, ArrayList> item in copy.MS2Fragments)
            {
                paths_to_full_image.Add(item.Key, copy.paths_to_full_image[item.Key]);
                foreach (MS2Fragment fragment in item.Value)
                {
                    MS2Fragments[item.Key].Add(new MS2Fragment(fragment));
                }
            }
        }
    }

    public class cl_lipid : lipid
    {
        public string[] fa_db_texts;
        public fattyAcidGroup fag1;
        public fattyAcidGroup fag2;
        public fattyAcidGroup fag3;
        public fattyAcidGroup fag4;
        public HashSet<int>[] fa_db_values;
    
        public cl_lipid(Dictionary<String, String> all_paths, Dictionary<String, ArrayList> all_fragments)
        {
            fag1 = new fattyAcidGroup();
            fag2 = new fattyAcidGroup();
            fag3 = new fattyAcidGroup();
            fag4 = new fattyAcidGroup();
            MS2Fragments.Add("CL", new ArrayList());
            
            foreach(KeyValuePair<String, ArrayList> kvp in MS2Fragments)
            {
                if (all_paths.ContainsKey(kvp.Key)) paths_to_full_image.Add(kvp.Key, all_paths[kvp.Key]);
                if (all_fragments.ContainsKey(kvp.Key))
                {
                    foreach (MS2Fragment fragment in all_fragments[kvp.Key])
                    {
                        MS2Fragments[kvp.Key].Add(new MS2Fragment(fragment));
                    }
                }
            }
        }
        
        public cl_lipid(cl_lipid copy) : base((lipid)copy)
        {
            fag1 = new fattyAcidGroup(copy.fag1);
            fag2 = new fattyAcidGroup(copy.fag2);
            fag3 = new fattyAcidGroup(copy.fag3);
            fag4 = new fattyAcidGroup(copy.fag4);
        }
    }

    public class gl_lipid : lipid
    {
        public string[] fa_db_texts;
        public fattyAcidGroup fag1;
        public fattyAcidGroup fag2;
        public fattyAcidGroup fag3;
        public HashSet<int>[] fa_db_values;
    
        public gl_lipid(Dictionary<String, String> all_paths, Dictionary<String, ArrayList> all_fragments)
        {
            fag1 = new fattyAcidGroup();
            fag2 = new fattyAcidGroup();
            fag3 = new fattyAcidGroup();
            MS2Fragments.Add("MG", new ArrayList());
            MS2Fragments.Add("DG", new ArrayList());
            MS2Fragments.Add("TG", new ArrayList());
            
            foreach(KeyValuePair<String, ArrayList> kvp in MS2Fragments)
            {
                if (all_paths.ContainsKey(kvp.Key)) paths_to_full_image.Add(kvp.Key, all_paths[kvp.Key]);
                if (all_fragments.ContainsKey(kvp.Key))
                {
                    foreach (MS2Fragment fragment in all_fragments[kvp.Key])
                    {
                        MS2Fragments[kvp.Key].Add(new MS2Fragment(fragment));
                    }
                }
            }
        }
    
        public gl_lipid(gl_lipid copy) : base((lipid)copy) 
        {
            fag1 = new fattyAcidGroup(copy.fag1);
            fag2 = new fattyAcidGroup(copy.fag1);
            fag3 = new fattyAcidGroup(copy.fag1);
        }
    }

    public class pl_lipid : lipid
    {
        public string[] fa_db_texts;
        public fattyAcidGroup fag1;
        public fattyAcidGroup fag2;
        public string hg;
        public int hgValue;
        public HashSet<int>[] fa_db_values;
    
        public pl_lipid(Dictionary<String, String> all_paths, Dictionary<String, ArrayList> all_fragments)
        {
            fag1 = new fattyAcidGroup();
            fag2 = new fattyAcidGroup();
            hgValue = 0;
            MS2Fragments.Add("PA", new ArrayList());
            MS2Fragments.Add("PC", new ArrayList());
            MS2Fragments.Add("PE", new ArrayList());
            MS2Fragments.Add("PG", new ArrayList());
            MS2Fragments.Add("PI", new ArrayList());
            MS2Fragments.Add("PIP", new ArrayList());
            MS2Fragments.Add("PIP2", new ArrayList());
            MS2Fragments.Add("PIP3", new ArrayList());
            MS2Fragments.Add("PS", new ArrayList());
            
            foreach(KeyValuePair<String, ArrayList> kvp in MS2Fragments)
            {
                if (all_paths.ContainsKey(kvp.Key)) paths_to_full_image.Add(kvp.Key, all_paths[kvp.Key]);
                if (all_fragments.ContainsKey(kvp.Key))
                {
                    foreach (MS2Fragment fragment in all_fragments[kvp.Key])
                    {
                        MS2Fragments[kvp.Key].Add(new MS2Fragment(fragment));
                    }
                }
            }
        }
    
        public pl_lipid(pl_lipid copy) : base((lipid)copy)
        {
            fag1 = new fattyAcidGroup(copy.fag1);
            fag2 = new fattyAcidGroup(copy.fag2);
            hgValue = copy.hgValue;
        }
    }

    public class sl_lipid : lipid
    {
        public string[] lcb_fa_db_texts;
        public string hg;
        public int hgValue;
        public fattyAcidGroup fag;
        public string lcb;
        public int lcbType;
        public string lcb_db;
        public string hydroxy;
        public int hydroxyValue;
        public HashSet<int>[] lcb_fa_db_values;
    
        public sl_lipid(Dictionary<String, String> all_paths, Dictionary<String, ArrayList> all_fragments)
        {
            fag = new fattyAcidGroup();
            hgValue = 0;
            lcbType = 0;
            hydroxyValue = 0;
            lcb = "5,7-8,10";
            lcb_db = "0-2";
            MS2Fragments.Add("Cer", new ArrayList());
            MS2Fragments.Add("CerP", new ArrayList());
            MS2Fragments.Add("GB3Cer", new ArrayList());
            MS2Fragments.Add("GM3Cer", new ArrayList());
            MS2Fragments.Add("GM4Cer", new ArrayList());
            MS2Fragments.Add("HexCer", new ArrayList());
            MS2Fragments.Add("LacCer", new ArrayList());
            MS2Fragments.Add("Lc3Cer", new ArrayList());
            MS2Fragments.Add("MIPCer", new ArrayList());
            MS2Fragments.Add("MIP2Cer", new ArrayList());
            MS2Fragments.Add("PECer", new ArrayList());
            MS2Fragments.Add("PICer", new ArrayList());
            MS2Fragments.Add("SM", new ArrayList());
            
            
            foreach(KeyValuePair<String, ArrayList> kvp in MS2Fragments)
            {
                if (all_paths.ContainsKey(kvp.Key)) paths_to_full_image.Add(kvp.Key, all_paths[kvp.Key]);
                if (all_fragments.ContainsKey(kvp.Key))
                {
                    foreach (MS2Fragment fragment in all_fragments[kvp.Key])
                    {
                        MS2Fragments[kvp.Key].Add(new MS2Fragment(fragment));
                    }
                }
            }
        }
    
        public sl_lipid(sl_lipid copy) : base((lipid)copy)
        {
            fag = new fattyAcidGroup(copy.fag);
            hgValue = copy.hgValue;
            lcbType = copy.lcbType;
            hydroxyValue = copy.hydroxyValue;
            lcb = copy.lcb;
            lcb_db = copy.lcb_db;
        }
    }



    public class LipidCreatorForm
    {

        public ArrayList lipidTabList;
        public ArrayList registered_lipids;
        //public ArrayList all_lipids;
        public CreatorGUI creatorGUI;
        public Dictionary<String, ArrayList> all_fragments;
        public Dictionary<String, String> all_paths_to_precursor_images;

        public LipidCreatorForm()
        {

            registered_lipids = new ArrayList();
            all_paths_to_precursor_images = new Dictionary<String, String>();
            all_fragments = new Dictionary<String, ArrayList>();
            //all_lipids = new ArrayList();
            
            try
            {
                using (StreamReader sr = new StreamReader("data/precursors.csv"))
                {
                    // omit titles
                    String line = sr.ReadLine();
                    while((line = sr.ReadLine()) != null)
                    {
                        String[] tokens = line.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                        all_paths_to_precursor_images.Add(tokens[0], tokens[1]);
                        Console.WriteLine(tokens[0] + " " + tokens[1]);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file 'data/precursors.csv' could not be read:");
                Console.WriteLine(e.Message);
            }
            
            try
            {
                using (StreamReader sr = new StreamReader("data/ms2fragments.csv"))
                {
                    // omit titles
                    String line = sr.ReadLine();
                    while((line = sr.ReadLine()) != null)
                    {
                        if (line.Length < 2) continue;
                        String[] tokens = line.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                        if (!all_fragments.ContainsKey(tokens[0]))
                        {
                            all_fragments.Add(tokens[0], new ArrayList());
                        }
                        all_fragments[tokens[0]].Add(new MS2Fragment(tokens[1], tokens[2], Convert.ToInt32(tokens[3])));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file 'data/ms2fragments.csv' could not be read:");
                Console.WriteLine(e.Message);
            }
            
            lipidTabList = new ArrayList(new lipid[] {new cl_lipid(all_paths_to_precursor_images, all_fragments),
                                                      new gl_lipid(all_paths_to_precursor_images, all_fragments),
                                                      new pl_lipid(all_paths_to_precursor_images, all_fragments),
                                                      new sl_lipid(all_paths_to_precursor_images, all_fragments) } );
                                                      
                                                      
            creatorGUI = new CreatorGUI(this);
            creatorGUI.changeTab(0);
        }







        /*
        private void add_cl_lipid(object obj, System.EventArgs ea)
        {



            TextBox[] text_boxes = { cl_fa_1_textbox, cl_fa_2_textbox, cl_fa_3_textbox, cl_fa_4_textbox, cl_db_1_textbox, cl_db_2_textbox, cl_db_3_textbox, cl_db_4_textbox };
            HashSet<int>[] fatty_acids = { new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>() };
            string[] fa_names = { "fatty acid 1", "fatty acid 2", "fatty acid 3", "fatty acid 4", "double bond 1", "double bond 2", "double bond 3", "double bond 4" };

            for (int tb = 0; tb < 8; ++tb)
            {
                int lower = 2 * (tb >= 4 ? 0 : 1);
                int higher = 30 - 24 * (tb >= 4 ? 1 : 0);
                string text_string = text_boxes[tb].Text.Replace(" ", "");
                foreach (char c in text_string)
                {
                    int ic = (int)c;
                    if (!((ic == (int)',') || (ic == (int)'-') || (ic == (int)' ') || (48 <= ic && ic < 58)))
                    {
                        MessageBox.Show(String.Format("The {1} contains an invalid character: '{0}'", c.ToString(), fa_names[tb]));
                        return;
                    }
                }
                string[] delimitors = new string[] { "," };
                string[] delimitors_range = new string[] { "-" };
                string[] tokens = text_string.Split(delimitors, StringSplitOptions.None);
                foreach (string acid in tokens)
                {
                    if (acid.Length == 0)
                    {
                        MessageBox.Show(String.Format("Wrong format in {0}, one entry is empty", fa_names[tb]));
                        return;
                    }


                    string[] interval = acid.Split(delimitors_range, StringSplitOptions.None);
                    if (interval.Length > 2)
                    {
                        MessageBox.Show(String.Format("Wrong format in {1}, too much '-'characters: {0}", acid, fa_names[tb]));
                        return;
                    }
                    else if (interval.Length == 2)
                    {
                        if (interval[0].Length == 0 || interval[1].Length == 0)
                        {
                            MessageBox.Show(String.Format("Wrong format in {0}, '{1}' notation is invalid", fa_names[tb], acid));
                            return;
                        }
                        else if (Convert.ToInt32(interval[0]) > Convert.ToInt32(interval[1]))
                        {
                            MessageBox.Show(String.Format("Attention in {0}, '{1}' is may be confused", fa_names[tb], acid));
                            return;
                        }
                        for (int i = Convert.ToInt32(interval[0]); i <= Convert.ToInt32(interval[1]); ++i)
                        {
                            if (!(lower <= i && i <= higher))
                            {
                                MessageBox.Show(String.Format("Wrong length in {1}: {0}", i.ToString(), fa_names[tb]));
                                return;
                            }
                            fatty_acids[tb].Add(i);
                        }
                    }
                    else
                    {
                        int i = Convert.ToInt32(acid);
                        if (!(lower <= i && i <= higher))
                        {
                            MessageBox.Show(String.Format("Wrong length in {1}: {0}", acid, fa_names[tb]));
                            return;
                        }
                        fatty_acids[tb].Add(i);
                    }
                }
            }
            int total_lipids = 1;
            for (int i = 0; i < 8; ++i)
            {
                int c = 0;
                foreach (int len in fatty_acids[i]) c++;
                total_lipids *= c;
            }

            bool proceed = true;
            if (total_lipids > 500)
            {
                string total_lipids_str = total_lipids.ToString();
                for (int i = 3; i < total_lipids_str.Length; i += 4)
                {
                    total_lipids_str = total_lipids_str.Insert(total_lipids_str.Length - i, ".");
                }
                string caution_text = "The current configuration results in up to " + total_lipids_str.ToString() + " combinations to check. Do you want to proceed?";
                DialogResult result = MessageBox.Show(caution_text, "Caution", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                proceed = (result == DialogResult.Yes ? true : false);
            }
            if (proceed)
            {

                DataRow row = dt.NewRow();
                row["Class"] = tab_control.SelectedTab.Text;
                row["Building Block 1"] = "Fatty acid 1: " + cl_fa_1_textbox.Text + "\nDouble Bond: " + cl_db_1_textbox.Text;
                row["Building Block 2"] = "Fatty acid 2: " + cl_fa_2_textbox.Text + "\nDouble Bond: " + cl_db_2_textbox.Text;
                row["Building Block 3"] = "Fatty acid 3: " + cl_fa_3_textbox.Text + "\nDouble Bond: " + cl_db_3_textbox.Text;
                row["Building Block 4"] = "Fatty acid 4: " + cl_fa_4_textbox.Text + "\nDouble Bond: " + cl_db_4_textbox.Text;
                cl_lipid lp = new cl_lipid();
                lp.fa_db_texts = new string[8];
                lp.fa_db_texts[0] = cl_fa_1_textbox.Text;
                lp.fa_db_texts[1] = cl_fa_2_textbox.Text;
                lp.fa_db_texts[2] = cl_fa_3_textbox.Text;
                lp.fa_db_texts[3] = cl_fa_4_textbox.Text;
                lp.fa_db_texts[4] = cl_db_1_textbox.Text;
                lp.fa_db_texts[5] = cl_db_2_textbox.Text;
                lp.fa_db_texts[6] = cl_db_3_textbox.Text;
                lp.fa_db_texts[7] = cl_db_4_textbox.Text;
                lp.class_name = tab_control.SelectedTab.Text;
                lp.text_page = tab_control.SelectedTab;
                lp.fa_db_values = fatty_acids;
                all_lipids.Add(lp);
            }
        }

        private void add_gl_lipid(object obj, System.EventArgs ea)
        {
            TextBox[] text_boxes = { gl_fa_1_textbox, gl_fa_2_textbox, gl_fa_3_textbox, gl_db_1_textbox, gl_db_2_textbox, gl_db_3_textbox };
            HashSet<int>[] fatty_acids = { new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>() };
            string[] fa_names = { "fatty acid 1", "fatty acid 2", "fatty acid 3", "double bond 1", "double bond 2", "double bond 3" };

            for (int tb = 0; tb < 6; ++tb)
            {
                int lower = 2 * (tb == 2 ? 1 : 0);
                int higher = 30 - 24 * (tb >= 3 ? 1 : 0);
                string text_string = text_boxes[tb].Text.Replace(" ", "");
                foreach (char c in text_string)
                {
                    int ic = (int)c;
                    if (!((ic == (int)',') || (ic == (int)'-') || (ic == (int)' ') || (48 <= ic && ic < 58)))
                    {
                        MessageBox.Show(String.Format("The {1} contains an invalid character: '{0}'", c.ToString(), fa_names[tb]));
                        return;
                    }
                }
                string[] delimitors = new string[] { "," };
                string[] delimitors_range = new string[] { "-" };
                string[] tokens = text_string.Split(delimitors, StringSplitOptions.None);
                foreach (string acid in tokens)
                {
                    if (acid.Length == 0)
                    {
                        MessageBox.Show(String.Format("Wrong format in {0}, one entry is empty", fa_names[tb]));
                        return;
                    }


                    string[] interval = acid.Split(delimitors_range, StringSplitOptions.None);
                    if (interval.Length > 2)
                    {
                        MessageBox.Show(String.Format("Wrong format in {1}, too much '-'characters: {0}", acid, fa_names[tb]));
                        return;
                    }
                    else if (interval.Length == 2)
                    {
                        if (interval[0].Length == 0 || interval[1].Length == 0)
                        {
                            MessageBox.Show(String.Format("Wrong format in {0}, '{1}' notation is invalid", fa_names[tb], acid));
                            return;
                        }
                        else if (Convert.ToInt32(interval[0]) > Convert.ToInt32(interval[1]))
                        {
                            MessageBox.Show(String.Format("Attention in {0}, '{1}' is may be confused", fa_names[tb], acid));
                            return;
                        }
                        for (int i = Convert.ToInt32(interval[0]); i <= Convert.ToInt32(interval[1]); ++i)
                        {
                            if (!(lower <= i && i <= higher))
                            {
                                MessageBox.Show(String.Format("Wrong length in {1}: {0}", i.ToString(), fa_names[tb]));
                                return;
                            }
                            fatty_acids[tb].Add(i);
                        }
                    }
                    else
                    {
                        int i = Convert.ToInt32(acid);
                        if (!(lower <= i && i <= higher))
                        {
                            MessageBox.Show(String.Format("Wrong length in {1}: {0}", acid, fa_names[tb]));
                            return;
                        }
                        fatty_acids[tb].Add(i);
                    }
                }
            }
            int total_lipids = 1;
            for (int i = 0; i < 6; ++i)
            {
                int c = 0;
                foreach (int len in fatty_acids[i]) c++;
                total_lipids *= c;
            }

            bool proceed = true;
            if (total_lipids > 500)
            {
                string total_lipids_str = total_lipids.ToString();
                for (int i = 3; i < total_lipids_str.Length; i += 4)
                {
                    total_lipids_str = total_lipids_str.Insert(total_lipids_str.Length - i, ".");
                }
                string caution_text = "The current configuration results in up to " + total_lipids_str.ToString() + " combinations to check. Do you want to proceed?";
                DialogResult result = MessageBox.Show(caution_text, "Caution", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                proceed = (result == DialogResult.Yes ? true : false);
            }
            if (proceed)
            {

                DataRow row = dt.NewRow();
                row["Class"] = tab_control.SelectedTab.Text;
                row["Building Block 1"] = "Fatty acid 1: " + gl_fa_1_textbox.Text + "\nDouble Bond: " + gl_db_1_textbox.Text;
                row["Building Block 2"] = "Fatty acid 2: " + gl_fa_2_textbox.Text + "\nDouble Bond: " + gl_db_2_textbox.Text;
                row["Building Block 3"] = "Fatty acid 3: " + gl_fa_3_textbox.Text + "\nDouble Bond: " + gl_db_3_textbox.Text;
                dt.Rows.Add(row);
                gl_lipid lp = new gl_lipid();
                lp.fa_db_texts = new string[6];
                lp.fa_db_texts[0] = gl_fa_1_textbox.Text;
                lp.fa_db_texts[1] = gl_fa_2_textbox.Text;
                lp.fa_db_texts[2] = gl_fa_3_textbox.Text;
                lp.fa_db_texts[3] = gl_db_1_textbox.Text;
                lp.fa_db_texts[4] = gl_db_2_textbox.Text;
                lp.fa_db_texts[5] = gl_db_3_textbox.Text;
                lp.class_name = tab_control.SelectedTab.Text;
                lp.text_page = tab_control.SelectedTab;
                lp.fa_db_values = fatty_acids;
                all_lipids.Add(lp);
            }
        }


        private void add_pl_lipid(object obj, System.EventArgs ea)
        {
            TextBox[] text_boxes = { pl_fa_1_textbox, pl_fa_2_textbox, pl_db_1_textbox, pl_db_2_textbox };
            HashSet<int>[] fatty_acids = { new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>() };
            string[] fa_names = { "fatty acid 1", "fatty acid 2", "double bond 1", "double bond 2" };

            for (int tb = 0; tb < 4; ++tb)
            {
                int lower = 2 * (tb == 1 ? 1 : 0);
                int higher = 30 - 24 * (tb >= 2 ? 1 : 0);
                string text_string = text_boxes[tb].Text.Replace(" ", "");
                foreach (char c in text_string)
                {
                    int ic = (int)c;
                    if (!((ic == (int)',') || (ic == (int)'-') || (ic == (int)' ') || (48 <= ic && ic < 58)))
                    {
                        MessageBox.Show(String.Format("The {1} contains an invalid character: '{0}'", c.ToString(), fa_names[tb]));
                        return;
                    }
                }
                string[] delimitors = new string[] { "," };
                string[] delimitors_range = new string[] { "-" };
                string[] tokens = text_string.Split(delimitors, StringSplitOptions.None);
                foreach (string acid in tokens)
                {
                    if (acid.Length == 0)
                    {
                        MessageBox.Show(String.Format("Wrong format in {0}, one entry is empty", fa_names[tb]));
                        return;
                    }


                    string[] interval = acid.Split(delimitors_range, StringSplitOptions.None);
                    if (interval.Length > 2)
                    {
                        MessageBox.Show(String.Format("Wrong format in {1}, too much '-'characters: {0}", acid, fa_names[tb]));
                        return;
                    }
                    else if (interval.Length == 2)
                    {
                        if (interval[0].Length == 0 || interval[1].Length == 0)
                        {
                            MessageBox.Show(String.Format("Wrong format in {0}, '{1}' notation is invalid", fa_names[tb], acid));
                            return;
                        }
                        else if (Convert.ToInt32(interval[0]) > Convert.ToInt32(interval[1]))
                        {
                            MessageBox.Show(String.Format("Attention in {0}, '{1}' is may be confused", fa_names[tb], acid));
                            return;
                        }
                        for (int i = Convert.ToInt32(interval[0]); i <= Convert.ToInt32(interval[1]); ++i)
                        {
                            if (!(lower <= i && i <= higher))
                            {
                                MessageBox.Show(String.Format("Wrong length in {1}: {0}", i.ToString(), fa_names[tb]));
                                return;
                            }
                            fatty_acids[tb].Add(i);
                        }
                    }
                    else
                    {
                        int i = Convert.ToInt32(acid);
                        if (!(lower <= i && i <= higher))
                        {
                            MessageBox.Show(String.Format("Wrong length in {1}: {0}", acid, fa_names[tb]));
                            return;
                        }
                        fatty_acids[tb].Add(i);
                    }
                }
            }
            int total_lipids = 1;
            for (int i = 0; i < 4; ++i)
            {
                int c = 0;
                foreach (int len in fatty_acids[i]) c++;
                total_lipids *= c;
            }

            bool proceed = true;
            if (total_lipids > 500)
            {
                string total_lipids_str = total_lipids.ToString();
                for (int i = 3; i < total_lipids_str.Length; i += 4)
                {
                    total_lipids_str = total_lipids_str.Insert(total_lipids_str.Length - i, ".");
                }
                string caution_text = "The current configuration results in up to " + total_lipids_str.ToString() + " combinations to check. Do you want to proceed?";
                DialogResult result = MessageBox.Show(caution_text, "Caution", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                proceed = (result == DialogResult.Yes ? true : false);
            }
            if (proceed)
            {

                DataRow row = dt.NewRow();
                row["Class"] = tab_control.SelectedTab.Text;
                row["Building Block 1"] = "Head group: " + (string)pl_hg_combobox.SelectedItem + "\n ";
                row["Building Block 2"] = "Fatty acid 1: " + pl_fa_1_textbox.Text + "\nDouble Bond: " + pl_db_1_textbox.Text;
                row["Building Block 3"] = "Fatty acid 2: " + pl_fa_2_textbox.Text + "\nDouble Bond: " + pl_db_2_textbox.Text;
                dt.Rows.Add(row);
                pl_lipid lp = new pl_lipid();
                lp.fa_db_texts = new string[6];
                lp.fa_db_texts[0] = pl_fa_1_textbox.Text;
                lp.fa_db_texts[1] = pl_fa_2_textbox.Text;
                lp.fa_db_texts[2] = pl_db_1_textbox.Text;
                lp.fa_db_texts[3] = pl_db_2_textbox.Text;
                lp.hg = (string)pl_hg_combobox.SelectedItem;
                lp.class_name = tab_control.SelectedTab.Text;
                lp.text_page = tab_control.SelectedTab;
                lp.fa_db_values = fatty_acids;
                all_lipids.Add(lp);
            }
        }

        private void add_sl_lipid(object obj, System.EventArgs ea)
        {
            TextBox[] text_boxes = { sl_fa_textbox, sl_lcb_textbox, sl_db_1_textbox, sl_db_2_textbox };
            int[] highers = { 30, 22, 6, 6 };
            HashSet<int>[] fatty_acids = { new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>() };
            string[] fa_names = { "fatty acid", "long chain base", "double bond (fa)", "double bond (lcb)" };

            for (int tb = 0; tb < 4; ++tb)
            {
                int lower = 14 * (tb == 1 ? 1 : 0);
                int higher = highers[tb];
                string text_string = text_boxes[tb].Text.Replace(" ", "");
                foreach (char c in text_string)
                {
                    int ic = (int)c;
                    if (!((ic == (int)',') || (ic == (int)'-') || (ic == (int)' ') || (48 <= ic && ic < 58)))
                    {
                        MessageBox.Show(String.Format("The {1} contains an invalid character: '{0}'", c.ToString(), fa_names[tb]));
                        return;
                    }
                }
                string[] delimitors = new string[] { "," };
                string[] delimitors_range = new string[] { "-" };
                string[] tokens = text_string.Split(delimitors, StringSplitOptions.None);
                foreach (string acid in tokens)
                {
                    if (acid.Length == 0)
                    {
                        MessageBox.Show(String.Format("Wrong format in {0}, one entry is empty", fa_names[tb]));
                        return;
                    }


                    string[] interval = acid.Split(delimitors_range, StringSplitOptions.None);
                    if (interval.Length > 2)
                    {
                        MessageBox.Show(String.Format("Wrong format in {1}, too much '-'characters: {0}", acid, fa_names[tb]));
                        return;
                    }
                    else if (interval.Length == 2)
                    {
                        if (interval[0].Length == 0 || interval[1].Length == 0)
                        {
                            MessageBox.Show(String.Format("Wrong format in {0}, '{1}' notation is invalid", fa_names[tb], acid));
                            return;
                        }
                        else if (Convert.ToInt32(interval[0]) > Convert.ToInt32(interval[1]))
                        {
                            MessageBox.Show(String.Format("Attention in {0}, '{1}' is may be confused", fa_names[tb], acid));
                            return;
                        }
                        for (int i = Convert.ToInt32(interval[0]); i <= Convert.ToInt32(interval[1]); ++i)
                        {
                            if (!(lower <= i && i <= higher))
                            {
                                MessageBox.Show(String.Format("Wrong length in {1}: {0}", i.ToString(), fa_names[tb]));
                                return;
                            }
                            fatty_acids[tb].Add(i);
                        }
                    }
                    else
                    {
                        int i = Convert.ToInt32(acid);
                        if (!(lower <= i && i <= higher))
                        {
                            MessageBox.Show(String.Format("Wrong length in {1}: {0}", acid, fa_names[tb]));
                            return;
                        }
                        fatty_acids[tb].Add(i);
                    }
                }
            }
            int total_lipids = 1;
            for (int i = 0; i < 4; ++i)
            {
                int c = 0;
                foreach (int len in fatty_acids[i]) c++;
                total_lipids *= c;
            }

            bool proceed = true;
            if (total_lipids > 500)
            {
                string total_lipids_str = total_lipids.ToString();
                for (int i = 3; i < total_lipids_str.Length; i += 4)
                {
                    total_lipids_str = total_lipids_str.Insert(total_lipids_str.Length - i, ".");
                }
                string caution_text = "The current configuration results in up to " + total_lipids_str.ToString() + " combinations to check. Do you want to proceed?";
                DialogResult result = MessageBox.Show(caution_text, "Caution", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                proceed = (result == DialogResult.Yes ? true : false);
            }
            if (proceed)
            {

                DataRow row = dt.NewRow();
                row["Class"] = tab_control.SelectedTab.Text;
                row["Building Block 1"] = "Head group: " + (string)sl_hg_combobox.SelectedItem + "\n ";
                row["Building Block 2"] = "Fatty acid: " + sl_fa_textbox.Text + "\nDouble Bond: " + sl_db_1_textbox.Text;
                row["Building Block 3"] = "Long chain base: " + ((string)sl_hydroxy_combobox.SelectedItem) + sl_lcb_textbox.Text + "\nDouble Bond: " + sl_db_2_textbox.Text;
                dt.Rows.Add(row);
                sl_lipid lp = new sl_lipid();
                lp.lcb_fa_db_texts = new string[6];
                lp.lcb_fa_db_texts[0] = sl_fa_textbox.Text;
                lp.lcb_fa_db_texts[1] = sl_lcb_textbox.Text;
                lp.lcb_fa_db_texts[2] = pl_db_1_textbox.Text;
                lp.lcb_fa_db_texts[3] = pl_db_2_textbox.Text;
                lp.hg = (string)sl_hg_combobox.SelectedItem;
                lp.hydroxy = (string)sl_hydroxy_combobox.SelectedItem;
                lp.class_name = tab_control.SelectedTab.Text;
                lp.text_page = tab_control.SelectedTab;
                lp.lcb_fa_db_values = fatty_acids;
                all_lipids.Add(lp);
            }
        }

        private void lipids_gridview_double_click(Object sender, EventArgs e)
        {
            int index = ((DataGridView)sender).CurrentCell.RowIndex;
            if (index >= 0)
            {
                tab_control.SelectTab(((lipid)all_lipids[index]).text_page);
                switch (((lipid)all_lipids[index]).class_name)
                {
                    case "Cardiolipins":
                        cl_fa_1_textbox.Text = ((cl_lipid)all_lipids[index]).fa_db_texts[0];
                        cl_fa_2_textbox.Text = ((cl_lipid)all_lipids[index]).fa_db_texts[1];
                        cl_fa_3_textbox.Text = ((cl_lipid)all_lipids[index]).fa_db_texts[2];
                        cl_fa_4_textbox.Text = ((cl_lipid)all_lipids[index]).fa_db_texts[3];
                        cl_db_1_textbox.Text = ((cl_lipid)all_lipids[index]).fa_db_texts[4];
                        cl_db_2_textbox.Text = ((cl_lipid)all_lipids[index]).fa_db_texts[5];
                        cl_db_3_textbox.Text = ((cl_lipid)all_lipids[index]).fa_db_texts[6];
                        cl_db_4_textbox.Text = ((cl_lipid)all_lipids[index]).fa_db_texts[7];
                        break;
                    case "Glycerolipids":
                        gl_fa_1_textbox.Text = ((gl_lipid)all_lipids[index]).fa_db_texts[0];
                        gl_fa_2_textbox.Text = ((gl_lipid)all_lipids[index]).fa_db_texts[1];
                        gl_fa_3_textbox.Text = ((gl_lipid)all_lipids[index]).fa_db_texts[2];
                        gl_db_1_textbox.Text = ((gl_lipid)all_lipids[index]).fa_db_texts[3];
                        gl_db_2_textbox.Text = ((gl_lipid)all_lipids[index]).fa_db_texts[4];
                        gl_db_3_textbox.Text = ((gl_lipid)all_lipids[index]).fa_db_texts[5];
                        break;
                    case "Phospholipids":
                        pl_fa_1_textbox.Text = ((pl_lipid)all_lipids[index]).fa_db_texts[0];
                        pl_fa_2_textbox.Text = ((pl_lipid)all_lipids[index]).fa_db_texts[1];
                        pl_db_1_textbox.Text = ((pl_lipid)all_lipids[index]).fa_db_texts[2];
                        pl_db_2_textbox.Text = ((pl_lipid)all_lipids[index]).fa_db_texts[3];
                        pl_hg_combobox.SelectedItem = ((pl_lipid)all_lipids[index]).hg;
                        break;
                    case "Sphingolipids":
                        sl_fa_textbox.Text = ((sl_lipid)all_lipids[index]).lcb_fa_db_texts[0];
                        sl_lcb_textbox.Text = ((sl_lipid)all_lipids[index]).lcb_fa_db_texts[1];
                        sl_db_1_textbox.Text = ((sl_lipid)all_lipids[index]).lcb_fa_db_texts[2];
                        sl_db_2_textbox.Text = ((sl_lipid)all_lipids[index]).lcb_fa_db_texts[3];
                        sl_hg_combobox.SelectedItem = ((sl_lipid)all_lipids[index]).hg;
                        sl_hydroxy_combobox.SelectedItem = ((sl_lipid)all_lipids[index]).hydroxy;
                        break;
                }
            }
        }

        private void lipids_gridview_delete_row(Object sender, DataGridViewRowCancelEventArgs e)
        {
            int index = ((DataGridView)sender).CurrentCell.RowIndex;
            if (index >= 0)
            {
                all_lipids.RemoveAt(index);
            }
        }

        public int cmp(int x, int y)
        {
            return x - y;
        }

        private void send_to_Skyline(Object sender, EventArgs e)
        {
            HashSet<long> cl_hash = new HashSet<long>();
            HashSet<long> gl_hash = new HashSet<long>();
            Dictionary<long, HashSet<string>> pl_hash = new Dictionary<long, HashSet<string>>();
            Dictionary<long, HashSet<string>> sl_hash = new Dictionary<long, HashSet<string>>();

            for (int i = 0; i < all_lipids.Count; ++i)
            {
                if (((lipid)all_lipids[i]).class_name == "Cardiolipins")
                {
                    cl_lipid lp = (cl_lipid)all_lipids[i];
                    foreach (int fa1 in lp.fa_db_values[0])
                    {
                        foreach (int db1 in lp.fa_db_values[4])
                        {
                            if (db1 <= Math.Ceiling(((double)fa1 - 2) / 2))
                            {
                                fatty_acid fat_a1 = new fatty_acid(fa1, db1);
                                foreach (int fa2 in lp.fa_db_values[1])
                                {
                                    foreach (int db2 in lp.fa_db_values[5])
                                    {
                                        if (db2 <= Math.Ceiling(((double)fa2 - 2) / 2))
                                        {
                                            fatty_acid fat_a2 = new fatty_acid(fa2, db2);
                                            foreach (int fa3 in lp.fa_db_values[2])
                                            {
                                                foreach (int db3 in lp.fa_db_values[6])
                                                {
                                                    if (db3 <= Math.Ceiling(((double)fa3 - 2) / 2))
                                                    {
                                                        fatty_acid fat_a3 = new fatty_acid(fa3, db3);
                                                        foreach (int fa4 in lp.fa_db_values[3])
                                                        {
                                                            foreach (int db4 in lp.fa_db_values[7])
                                                            {
                                                                if (db4 <= Math.Ceiling(((double)fa4 - 2) / 2))
                                                                {
                                                                    List<fatty_acid> l = new List<fatty_acid>();
                                                                    l.Add(fat_a1);
                                                                    l.Add(fat_a2);
                                                                    l.Add(fat_a3);
                                                                    l.Add(new fatty_acid(fa4, db4));
                                                                    l.Sort();

                                                                    long hash_value = (long)l[0].length;
                                                                    hash_value |= ((long)l[1].length) << 5;
                                                                    hash_value |= ((long)l[2].length) << 10;
                                                                    hash_value |= ((long)l[3].length) << 15;
                                                                    hash_value |= ((long)l[0].db) << 20;
                                                                    hash_value |= ((long)l[1].db) << 23;
                                                                    hash_value |= ((long)l[2].db) << 26;
                                                                    hash_value |= ((long)l[3].db) << 29;

                                                                    cl_hash.Add(hash_value);
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                            }

                                        }
                                    }
                                }

                            }
                        }
                    }
                }



                else if (((lipid)all_lipids[i]).class_name == "Glycerolipids")
                {
                    gl_lipid lp = (gl_lipid)all_lipids[i];
                    foreach (int fa1 in lp.fa_db_values[0])
                    {
                        foreach (int db1 in lp.fa_db_values[3])
                        {
                            if (db1 <= Math.Ceiling(((double)fa1 - 2) / 2) || (fa1 == 0 && db1 == 0))
                            {
                                fatty_acid fat_a1 = new fatty_acid(fa1, db1);
                                foreach (int fa2 in lp.fa_db_values[1])
                                {
                                    foreach (int db2 in lp.fa_db_values[4])
                                    {
                                        if (db2 <= Math.Ceiling(((double)fa2 - 2) / 2) || (fa2 == 0 && db2 == 0))
                                        {
                                            fatty_acid fat_a2 = new fatty_acid(fa2, db2);
                                            foreach (int fa3 in lp.fa_db_values[2])
                                            {
                                                foreach (int db3 in lp.fa_db_values[5])
                                                {
                                                    if (db3 <= Math.Ceiling(((double)fa3 - 2) / 2))
                                                    {
                                                        List<fatty_acid> l = new List<fatty_acid>();
                                                        l.Add(fat_a1);
                                                        l.Add(fat_a2);
                                                        l.Add(new fatty_acid(fa3, db3));
                                                        l.Sort();



                                                        long hash_value = (long)l[0].length;
                                                        hash_value |= ((long)l[1].length) << 5;
                                                        hash_value |= ((long)l[2].length) << 10;
                                                        hash_value |= ((long)l[0].db) << 20;
                                                        hash_value |= ((long)l[1].db) << 23;
                                                        hash_value |= ((long)l[2].db) << 26;

                                                        gl_hash.Add(hash_value);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                //int rrr = 0;
                else if (((lipid)all_lipids[i]).class_name == "Phospholipids")
                {
                    pl_lipid lp = (pl_lipid)all_lipids[i];
                    foreach (int fa1 in lp.fa_db_values[0])
                    {
                        foreach (int db1 in lp.fa_db_values[2])
                        {
                            if (db1 <= Math.Ceiling(((double)fa1 - 2) / 2) || (fa1 == 0 && db1 == 0))
                            {
                                fatty_acid fat_a1 = new fatty_acid(fa1, db1);
                                foreach (int fa2 in lp.fa_db_values[1])
                                {
                                    foreach (int db2 in lp.fa_db_values[3])
                                    {
                                        if (db2 <= Math.Ceiling(((double)fa2 - 2) / 2))
                                        {

                                            List<fatty_acid> l = new List<fatty_acid>();
                                            l.Add(fat_a1);
                                            l.Add(new fatty_acid(fa2, db2));
                                            l.Sort();


                                            long hash_value = (long)l[0].length;
                                            hash_value |= ((long)l[1].length) << 5;
                                            hash_value |= ((long)l[0].db) << 20;
                                            hash_value |= ((long)l[1].db) << 23;

                                            if (!pl_hash.ContainsKey(hash_value)) pl_hash.Add(hash_value, new HashSet<string>());
                                            if (!pl_hash[hash_value].Contains(lp.hg))
                                            {
                                                //Console.WriteLine("{0}:{1}/{2}:{3}", l[0].length, l[0].db, l[1].length, l[1].db);
                                                //rrr += 1;
                                                pl_hash[hash_value].Add(lp.hg);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                else if (((lipid)all_lipids[i]).class_name == "Sphingolipids")
                {
                    sl_lipid lp = (sl_lipid)all_lipids[i];
                    foreach (int fa in lp.lcb_fa_db_values[0])
                    {
                        foreach (int db1 in lp.lcb_fa_db_values[2])
                        {
                            if (db1 <= Math.Ceiling(((double)fa - 2) / 2) || (fa == 0 && db1 == 0))
                            {
                                foreach (int lcb in lp.lcb_fa_db_values[1])
                                {
                                    foreach (int db2 in lp.lcb_fa_db_values[3])
                                    {
                                        if (db2 <= Math.Ceiling(((double)lcb - 2) / 2))
                                        {

                                            long hash_value = (long)fa;
                                            hash_value |= ((long)lcb) << 5;
                                            hash_value |= ((long)(lp.hydroxy == "d" ? 1 : 2)) << 12;
                                            hash_value |= ((long)db1) << 20;
                                            hash_value |= ((long)db2) << 23;

                                            if (!sl_hash.ContainsKey(hash_value)) sl_hash.Add(hash_value, new HashSet<string>());
                                            if (!sl_hash[hash_value].Contains(lp.hg))
                                            {
                                                sl_hash[hash_value].Add(lp.hg);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                //Console.WriteLine("cnt: {0}", rrr);
            }

            // create csv
            List<string> csv_lines = new List<string>();
            foreach (long key in cl_hash)
            {
                long fa_bits = key;
                long db_bits = key >> 20;
                long fa_mask = (long)31;
                long db_mask = (long)7;
                string fa1 = (fa_bits & fa_mask).ToString();
                string fa2 = ((fa_bits >> 5) & fa_mask).ToString();
                string fa3 = ((fa_bits >> 10) & fa_mask).ToString();
                string fa4 = ((fa_bits >> 15) & fa_mask).ToString();
                string db1 = (db_bits & db_mask).ToString();
                string db2 = ((db_bits >> 3) & db_mask).ToString();
                string db3 = ((db_bits >> 6) & db_mask).ToString();
                string db4 = ((db_bits >> 9) & db_mask).ToString();

                string molecule_name = string.Format("CL{0}:{1}/{2}:{3}/{4}:{5}/{6}:{7}", fa1, db1, fa2, db2, fa3, db3, fa4, db4);

                csv_lines.Add(molecule_name);

            }

            string[] gl_prefixes = { "-", "MG", "DG", "TG" };

            foreach (long key in gl_hash)
            {
                long fa_bits = key;
                long db_bits = key >> 20;
                long fa_mask = (long)31;
                long db_mask = (long)7;
                string fa1 = (fa_bits & fa_mask).ToString();
                string fa2 = ((fa_bits >> 5) & fa_mask).ToString();
                string fa3 = ((fa_bits >> 10) & fa_mask).ToString();
                string db1 = (db_bits & db_mask).ToString();
                string db2 = ((db_bits >> 3) & db_mask).ToString();
                string db3 = ((db_bits >> 6) & db_mask).ToString();

                int mono = 0;
                if (fa1 != "0") ++mono;
                if (fa2 != "0") ++mono;
                if (fa3 != "0") ++mono;

                string molecule_name = string.Format("{6}{0}:{1}/{2}:{3}/{4}:{5}", fa1, db1, fa2, db2, fa3, db3, gl_prefixes[mono]);

                csv_lines.Add(molecule_name);

            }


            foreach (long key in pl_hash.Keys)
            {
                long fa_bits = key;
                long db_bits = key >> 20;
                long fa_mask = (long)31;
                long db_mask = (long)7;
                string fa1 = (fa_bits & fa_mask).ToString();
                string fa2 = ((fa_bits >> 5) & fa_mask).ToString();
                string db1 = (db_bits & db_mask).ToString();
                string db2 = ((db_bits >> 3) & db_mask).ToString();

                foreach (string hg_name in pl_hash[key])
                {
                    string molecule_name = string.Format("{4}{0}:{1}/{2}:{3}", fa1, db1, fa2, db2, hg_name);
                    csv_lines.Add(molecule_name);
                }

            }


            foreach (long key in sl_hash.Keys)
            {
                long fa_bits = key;
                long db_bits = key >> 20;
                long fa_mask = (long)31;
                long db_mask = (long)7;
                string fa = (fa_bits & fa_mask).ToString();
                string lcb = ((fa_bits >> 5) & fa_mask).ToString();
                string hydroxy = (((key >> 12) & 3) == 1) ? "d" : "t";
                string db1 = (db_bits & db_mask).ToString();
                string db2 = ((db_bits >> 3) & db_mask).ToString();

                foreach (string hg_name in sl_hash[key])
                {
                    string molecule_name = string.Format("{4} {5}{0}:{1}/{2}:{3}", lcb, db2, fa, db1, hg_name, hydroxy);
                    csv_lines.Add(molecule_name);
                }
            }

            System.IO.File.WriteAllLines((String)AppDomain.CurrentDomain.BaseDirectory + @"\lipids.csv", csv_lines);
            MessageBox.Show("finished");
        }
        */




        [STAThread]
        public static void Main()
        {
            LipidCreatorForm lcf = new LipidCreatorForm();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(lcf.creatorGUI);
        }
    }

}


