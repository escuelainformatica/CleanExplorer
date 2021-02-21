using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace CleanExplorer
{
    public partial class Form1 : Form
    {
        private bool ignoreCheck = false;
        public static List<Element> result=new List<Element>();
        public Form1()
        {
            InitializeComponent();
            checkedListBox1.DisplayMember = "Text";
            checkedListBox1.ValueMember = "Value";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var rk = Registry.ClassesRoot.OpenSubKey("CLSID");
            result = new List<Element>();
            ignoreCheck = true;
            SearchSubKeys(rk, "System.IsPinnedToNamespaceTree");
            rk.Close();
            int contador = 0;
            
            foreach (var item in result)
            {
                
                checkedListBox1.Items.Insert(contador, new {Text=item.Name +" - ("+item.Key+")",Value= contador });
                checkedListBox1.SetItemChecked(contador,item.Value==1);
                contador++;
            }

            ignoreCheck = false;
        }
        private static void SearchSubKeys(RegistryKey root, string searchKey)
        {
            searchKey = searchKey.ToUpper();
            if (root == null)
            {
                return;
            }
            
            foreach (string keyname in root.GetSubKeyNames())
            {
                try
                {
                    using (RegistryKey key = root.OpenSubKey(keyname))
                    {

                        

                        if (key == null)
                        {
                            return;
                        }

      

                        var skn = key.GetSubKeyNames();
                        var vn = key.GetValueNames();

                        var findme = skn.FirstOrDefault(k=>k.ToUpper()== searchKey);
                        if (findme != null)
                        {
                            string name = (string)key.GetValue("");
                            if (name != null)
                            {
                                int value = (int) key.GetValue(findme);
                                var elm = new Element(key.Name, findme, name, value);
                                result.Add(elm);
                            }
                        }
                        var findme2 = vn.FirstOrDefault(k => k.ToUpper() == searchKey);
                        if (findme2 != null)
                        {
                            string name = (string) key.GetValue("");
                            if (name != null)
                            {
                                string keyname2 = key.Name.Replace("HKEY_CLASSES_ROOT\\CLSID\\", "");
                                int value = (int) key.GetValue(findme2);
                                var elm = new Element(keyname2, findme2, name, value);
                                result.Add(elm);
                            }


                        }
                
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (ignoreCheck)
            {
                return;
            }
            
            var check=e.NewValue == CheckState.Checked;
            var elem = result[e.Index];
            elem.Value =  check ? 1 : 0;
            // RegistryKeyPermissionCheck.ReadWriteSubTree,RegistryRights.SetValue
            using (var sk=Registry.ClassesRoot.OpenSubKey("CLSID")
                .OpenSubKey(elem.Key, RegistryKeyPermissionCheck.ReadWriteSubTree)) { 
                sk.SetValue(elem.SubKey,(object)elem.Value,RegistryValueKind.DWord);

            }
            
        }
    }
}
