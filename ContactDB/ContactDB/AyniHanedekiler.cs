using ContactDB.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContactDB
{
    public partial class AyniHanedekiler : Form
    {
        string yol = Settings.Default["VERITABANI"].ToString();
        public AyniHanedekiler()
        {
            InitializeComponent();

        }

        private void AyniHanedekiler_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView1.GridLines = true;
            string AdresAd = Form1.gonderilecekveri;
            string sorgu = @"SELECT * FROM sahis WHERE adresid = " +AdresAd+ "";
            
            SQLiteConnection baglanti = new SQLiteConnection("Data Source = " + yol + "; Version = 3");
            baglanti.Open();
            SQLiteCommand komut = new SQLiteCommand(sorgu, baglanti);
            SQLiteDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                ListViewItem item = new ListViewItem(dr["tckn"].ToString());
                item.SubItems.Add(dr["ad"].ToString());
                item.SubItems.Add(dr["soyad"].ToString());
                item.SubItems.Add(dr["annead"].ToString());
                item.SubItems.Add(dr["babaad"].ToString());
                item.SubItems.Add(dr["cinsiyet"].ToString());
                item.SubItems.Add(dr["dogumyeri"].ToString());
                item.SubItems.Add(dr["dogumtr"].ToString());
                listView1.Items.Add(item);
            }
            baglanti.Close();
            baglanti.Dispose();
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
    }
}
