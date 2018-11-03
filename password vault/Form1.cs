using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Finisar.SQLite;
using System.IO;
using System.Security.Cryptography;
namespace password_vault
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // We use these three SQLite objects:

            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            // create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=user_info.db;Version=3;New=True;Compress=True;");
            // open the connection:
            sqlite_conn.Open();
            // create a new SQL command:
            sqlite_cmd = sqlite_conn.CreateCommand();
            // Let the SQLiteCommand object know our SQL-Query:
            sqlite_cmd.CommandText = "CREATE TABLE user (id integer primary key, url varchar(500), username varchar(100),  password varchar(100));";
            // Now lets execute the SQL ;D
            sqlite_cmd.ExecuteNonQuery();
            // Lets insert something into our new table:
            sqlite_cmd.CommandText = @"
                                       INSERT INTO user (id,url,username,password) VALUES (1,'http:\\www.google.com','George','123'); 
                                       INSERT INTO user (id,url,username,password) VALUES (2,'http:\\www.google.com','Lal','235');
                                     ";
            // And execute this again ;D
            sqlite_cmd.ExecuteNonQuery();
            // ...and inserting another line:
            //sqlite_cmd.CommandText = "";
            //// And execute this again ;D
            //sqlite_cmd.ExecuteNonQuery();
            // But how do we read something out of our table ?
            // First lets build a SQL-Query again:
            sqlite_cmd.CommandText = "SELECT * FROM user";
            // Now the SQLiteCommand object can give us a DataReader-Object:
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            // The SQLiteDataReader allows us to run through the result lines:
            while (sqlite_datareader.Read()) // Read() returns true if there is still a result line to read
            {

                dataGridView1.Rows.Add(new object[]{
                    sqlite_datareader.GetValue(sqlite_datareader.GetOrdinal("url")),
                    sqlite_datareader.GetValue(sqlite_datareader.GetOrdinal("username")),
                    sqlite_datareader.GetValue(sqlite_datareader.GetOrdinal("password"))
                });
                // Print out the content of the text field:
                System.Console.WriteLine(sqlite_datareader["username"]);
                string myreader = sqlite_datareader.GetString(0);
                //MessageBox.Show(myreader);
                /*  //show db to gridView
                  sqlite_cmd.CommandText = "SELECT * FROM user";
                  myreader.ExecuteReader();*/
            }
            // We are ready, now lets cleanup and close our connection:
            sqlite_conn.Close();

            textBox1.Hide();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            //exit application
            Application.Exit(); 
        }

  
        private string Encrypt(string clearText)
        {
            //Key
            string EncryptionKey = "MAKV2SPBNI99212"; 
 
            //Convert string clearText to bytes
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

            using (Aes encryptor = Aes.Create()) 
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                //Gets or sets the initialization vector (IV) for the symmetric algorithm and convert into bytes
                encryptor.IV = pdb.GetBytes(16);
                //allocate memory for storing string
                using (MemoryStream ms = new MemoryStream())
                {
                    //cryptostream for data transformation
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {

                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    //convert clearText to 64bit string
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        //encrypt option
        private void encryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s1 = dataGridView1.Rows[0].Cells[2].Value.ToString();
            textBox1.Text = Encrypt(s1);
            dataGridView1.Rows[0].Cells[2].Value = textBox1.Text;
            string s2 = dataGridView1.Rows[1].Cells[2].Value.ToString();
            textBox1.Text = Encrypt(s2);
            dataGridView1.Rows[1].Cells[2].Value = textBox1.Text;
           
           
        }

        //aes_decryption function
        private string Decrypt(string cipherText)
        {
            //Key
            string EncryptionKey = "MAKV2SPBNI99212";
            //Convert string clearText to bytes
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);

                //Gets or sets the initialization vector (IV) for the symmetric algorithm and convert into bytes
                encryptor.IV = pdb.GetBytes(16);

                //allocate memory for storing string
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }

                    //converting ciphertext to plain text
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());

                }
            }
            return cipherText;
        }
        //decrypt button
        private void decryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s1 = dataGridView1.Rows[0].Cells[2].Value.ToString();
            textBox1.Text = Decrypt(s1);
            dataGridView1.Rows[0].Cells[2].Value = textBox1.Text;
            string s2 = dataGridView1.Rows[1].Cells[2].Value.ToString();
            textBox1.Text = Decrypt(s2);
            dataGridView1.Rows[1].Cells[2].Value = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string btn = button2.Text = "Hidepass";
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
    }
}