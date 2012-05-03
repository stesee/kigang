using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClassLibraryDatabase;
using System.Data.Entity;
using System.IO;

namespace DatabaseTool
{
    public partial class Form1 : Form
    {
        Therapie therapie = new Therapie();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Database.SetInitializer<KigangContext>(new DropCreateDatabaseIfModelChanges<KigangContext>());
            using (var db = new KigangContext())
            {
                var therapeut = new Therapeut();
                therapeut.Nachname = "test";
                therapeut.Vorname = DateTime.Now.ToString();
                propertyGrid1.SelectedObject = therapeut;
                db.Therapeuten.ToList();
                db.Therapeuten.Add(therapeut);
                db.SaveChanges();
                therapeutDataGridView.DataSource = db.Therapeuten.Local.ToBindingList();
            }

        }
        #region export 
        #region exporthelpers
        /// <summary> 
        /// Creates a semicolon delimeted string of all the objects property values. 
        /// </summary> 
        /// <param name="obj">object.</param> 
        /// <returns>string.</returns> 
        public static string ObjectNameToCsvData(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj", "Value can not be null or Nothing!");
            }
            StringBuilder sb = new StringBuilder();
            Type t = obj.GetType();
            System.Reflection.PropertyInfo[] pi = t.GetProperties();
            for (int index = 0; index < pi.Length; index++)
            {
                sb.Append(pi[index].Name);
                if (index < pi.Length - 1)
                {
                    sb.Append(";");
                }
            }
            return sb.ToString();
        }
        /// <summary> 
        /// Creates a semicolon delimeted string of all the objects values. 
        /// </summary> 
        /// <param name="obj">object.</param> 
        /// <returns>string.</returns> 
        public static string ObjectToCsvData(object obj)
        {
            StringBuilder sb = new StringBuilder();
            if (obj == null)
            {
                sb.Append("Keine Einträge");
            }
            else
            {
                Type t = obj.GetType();
                System.Reflection.PropertyInfo[] pi = t.GetProperties();

                for (int index = 0; index < pi.Length; index++)
                {
                    sb.Append(pi[index].GetValue(obj, null));

                    if (index < pi.Length - 1)
                    {
                        sb.Append(";");
                    }
                }
            }

            return sb.ToString();
        }
        #endregion
        private void buttonExport_Click(object sender, EventArgs e)
        {

            string strValue = string.Empty;
            //Replace this with your objects: 
            KigangContext kigangContext = new KigangContext();
            richTextBoxCommaDelimiterd.Text = ObjectNameToCsvData(kigangContext.Therapeuten.First());
            strValue += ObjectNameToCsvData(kigangContext.Therapeuten.First()) + "\n";
            foreach (var item in kigangContext.Therapeuten)
            {
                strValue += ObjectToCsvData(item) + "\n";
            }
            saveFileDialog1.ShowDialog();
            string strFile = saveFileDialog1.FileName;
            if (!string.IsNullOrEmpty(strValue))
            {
                File.WriteAllText(strFile, strValue);
            }
        }
        #endregion
    }
}