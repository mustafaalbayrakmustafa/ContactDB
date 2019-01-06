using ContactDB.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication1;
using PrintableListView;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Diagnostics;
using Microsoft.Win32;

namespace ContactDB
{

    public partial class Form1 : Form
    {
        string yol = Settings.Default["VERITABANI"].ToString();
        public static string gonderilecekveri;
        public Form1()
        {
            InitializeComponent();
            int genislik, yukseklik, bitsayisi;
            genislik = Screen.PrimaryScreen.Bounds.Width;
            yukseklik = Screen.PrimaryScreen.Bounds.Height;
            bitsayisi = Screen.PrimaryScreen.BitsPerPixel;
            this.Size = new Size((genislik + genislik / 2) / 3, (yukseklik + yukseklik / 2) / 3);
            listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("tr-TR");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
        }

        public static string UrlEncodeExtended(string value)
        {
            char[] chars = value.ToCharArray();
            StringBuilder encodedValue = new StringBuilder();
            foreach (char c in chars)
            {
                encodedValue.Append("%" + ((int)c).ToString("X2"));
            }
            return encodedValue.ToString();
        }


        public void export(DataGridView dgw, string filename)
        {
            try
            {
                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, BaseFont.EMBEDDED);
                PdfPTable pdftable = new PdfPTable(dgw.Columns.Count);
                pdftable.DefaultCell.Padding = 3;
                pdftable.WidthPercentage = 100;
                pdftable.HorizontalAlignment = Element.ALIGN_LEFT;
                pdftable.DefaultCell.BorderWidth = 1;



                iTextSharp.text.pdf.BaseFont STF_Helvetica_Turkish = iTextSharp.text.pdf.BaseFont.CreateFont("Helvetica", "CP1254", iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font text = new iTextSharp.text.Font(STF_Helvetica_Turkish, 7, iTextSharp.text.Font.NORMAL);

                //iTextSharp.text.Font text = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);

                foreach (DataGridViewColumn column in dgw.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, text));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240);
                    pdftable.AddCell(cell);
                }

                foreach (DataGridViewRow row in dgw.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        pdftable.AddCell(new Phrase(cell.Value.ToString(), text));
                    }
                }

                var savefiledialog = new SaveFileDialog();
                savefiledialog.FileName = filename;
                savefiledialog.DefaultExt = ".pdf";
                //if (savefiledialog.ShowDialog() == DialogResult.OK)
                //{
                    using (FileStream stream = new FileStream(savefiledialog.FileName, FileMode.Create))
                    {
                        Document pdfdoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                        pdfdoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                        PdfWriter.GetInstance(pdfdoc, stream);
                        pdfdoc.Open();
                        pdfdoc.Add(pdftable);
                        pdfdoc.Close();
                        
                        stream.Close();

                    //}
                    string name = UrlEncodeExtended(stream.Name);
                    Process.Start("chrome.exe",name);
                }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Uyarı!",MessageBoxButtons.OK,MessageBoxIcon.Hand);
            }
            

        }


        private void button1_Click(object sender, EventArgs e)
        {

            listView2.Items.Clear();
            try
            {
                bool flag = this.txtTCK.TextLength == 11;
                if (flag)
                {
                    string sorgu = ("SELECT sahis.tckn, sahis.ad, sahis.soyad, sahis.annead, sahis.babaad, sahis.cinsiyet, sahis.dogumyeri,sahis.dogumtr, mahalleler.mahallead, adres.caddesokak, adres.kapino, adres.daireno, ilceler.ilcead, iller.sehir,sahis.adresid FROM sahis INNER JOIN adres ON sahis.adresid = adres.id AND sahis.adresid = adres.id INNER JOIN ilceler ON sahis.nufusilceid = ilceler.id INNER JOIN iller ON sahis.nufusilid = iller.id INNER JOIN mahalleler ON adres.muhtarlikid = mahalleler.id WHERE sahis.tckn = " + this.txtTCK.Text) ?? "";

                    SQLiteConnection baglanti = new SQLiteConnection("Data Source = " + yol + "; Version = 3");

                    listView2.View = View.Details;
                    listView2.GridLines = true;

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
                        item.SubItems.Add(dr["mahallead"].ToString());
                        item.SubItems.Add(dr["caddesokak"].ToString());
                        item.SubItems.Add(dr["kapino"].ToString());
                        item.SubItems.Add(dr["daireno"].ToString());
                        item.SubItems.Add(dr["ilcead"].ToString());
                        item.SubItems.Add(dr["sehir"].ToString());
                        item.SubItems.Add(dr["adresid"].ToString());
                        listView2.Items.Add(item);
                    }
                    SQLiteCommand cmd = new SQLiteCommand(sorgu, baglanti);
                    SQLiteDataReader dataReader = cmd.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    dataGridView1.DataSource = dataTable;
                    baglanti.Close();
                    baglanti.Dispose();

                }
                bool flag3 = this.txtTCK.TextLength < 11;
                if (flag3)
                {
                    string sorgu2 = ("SELECT sahis.tckn, sahis.ad, sahis.soyad, sahis.annead, sahis.babaad, sahis.cinsiyet, sahis.dogumyeri,sahis.dogumtr, mahalleler.mahallead, adres.caddesokak, adres.kapino, adres.daireno, ilceler.ilcead, iller.sehir, sahis.adresid FROM sahis INNER JOIN adres ON sahis.adresid = adres.id AND sahis.adresid = adres.id INNER JOIN ilceler ON sahis.nufusilceid = ilceler.id INNER JOIN iller ON sahis.nufusilid = iller.id INNER JOIN mahalleler ON adres.muhtarlikid = mahalleler.id WHERE sahis.tckn LIKE '%" + this.txtTCK.Text + "%'") ?? "";
                    SQLiteConnection baglanti2 = new SQLiteConnection("Data Source = " + yol + "; Version = 3");
                    baglanti2.Open();
                    SQLiteCommand komut = new SQLiteCommand(sorgu2, baglanti2);
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
                        item.SubItems.Add(dr["mahallead"].ToString());
                        item.SubItems.Add(dr["caddesokak"].ToString());
                        item.SubItems.Add(dr["kapino"].ToString());
                        item.SubItems.Add(dr["daireno"].ToString());
                        item.SubItems.Add(dr["ilcead"].ToString());
                        item.SubItems.Add(dr["sehir"].ToString());
                        item.SubItems.Add(dr["adresid"].ToString());
                        listView2.Items.Add(item);
                    }
                    SQLiteCommand cmd = new SQLiteCommand(sorgu2, baglanti2);
                    SQLiteDataReader dataReader = cmd.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    dataGridView1.DataSource = dataTable;
                    baglanti2.Close();
                    baglanti2.Dispose();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //this.cmbSehirler.Text = null;
            }

            listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }







        private void button2_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            try
            {
                string sorgu = string.Concat(new string[]
                {
            "SELECT Sahis.TCKN,Sahis.Ad,Sahis.Soyad, Sahis.AnneAd, Sahis.BabaAd, Sahis.Cinsiyet, Sahis.DogumYeri, Sahis.DogumTr, Mahalleler.MahalleAd, Adres.CaddeSokak, Adres.KapiNo, Adres.DaireNo, ilceler.ilceAd, iller.Sehir, Sahis.AdresId " +
            "FROM " +
            "iller " +
            "INNER JOIN Sahis ON iller.Id = Sahis.NufusIlId INNER JOIN ilceler ON Sahis.NufusIlceId = ilceler.Id INNER JOIN Adres ON ilceler.ilKodu = Adres.ilKodu AND Sahis.AdresId = Adres.Id INNER JOIN Mahalleler ON Adres.MuhtarlikId = Mahalleler.Id " +
            "WHERE " +
            "(sahis.ad LIKE "+"'"+"%"+""+txtAD.Text+"%"+"'"+") " +
            "AND (sahis.soyad LIKE "+"'"+"%"+""+txtSOYAD.Text+"%"+"'"+") " +
            "AND (iller.sehir LIKE "+"'"+"%"+""+cmbSehirler.Text+"%"+"'"+")"});

                string sorgu2 = string.Concat(new string[]
                {
            "SELECT sahis.tckn, sahis.ad, sahis.soyad, sahis.annead, sahis.babaad, sahis.cinsiyet, sahis.dogumyeri,sahis.dogumtr, mahalleler.mahallead, adres.caddesokak, adres.kapino, adres.daireno, ilceler.ilcead, iller.sehir, sahis.adresid FROM sahis INNER JOIN adres ON sahis.adresid = adres.id AND sahis.adresid = adres.id INNER JOIN ilceler ON sahis.nufusilceid = ilceler.id INNER JOIN iller ON sahis.nufusilid = iller.id INNER JOIN mahalleler ON adres.muhtarlikid = mahalleler.id WHERE sahis.ad = '",
            this.txtAD.Text,
            "' AND sahis.soyad= '",
            this.txtSOYAD.Text,
            "'"
                });

                SQLiteConnection baglanti = new SQLiteConnection("Data Source = " + yol + "; Version = 3");
                //bool flag = this.cmbSehirler.Text == null;
                if (cmbSehirler.SelectedIndex.ToString() != null)
                {
                    listView2.View = View.Details;
                    listView2.GridLines = true;

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
                        item.SubItems.Add(dr["mahallead"].ToString());
                        item.SubItems.Add(dr["caddesokak"].ToString());
                        item.SubItems.Add(dr["kapino"].ToString());
                        item.SubItems.Add(dr["daireno"].ToString());
                        item.SubItems.Add(dr["ilcead"].ToString());
                        item.SubItems.Add(dr["sehir"].ToString());
                        item.SubItems.Add(dr["adresid"].ToString());
                        listView2.Items.Add(item);
                    }


                    SQLiteCommand cmd = new SQLiteCommand(sorgu, baglanti);
                    SQLiteDataReader dataReader = cmd.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    dataGridView1.DataSource = dataTable;



                    baglanti.Close();
                    baglanti.Dispose();
                }
                if (cmbSehirler.SelectedIndex.ToString() == null | cmbSehirler.SelectedIndex.ToString() == "")
                {
                    listView2.View = View.Details;
                    listView2.GridLines = true;

                    baglanti.Open();
                    SQLiteCommand komut2 = new SQLiteCommand(sorgu2, baglanti);
                    SQLiteDataReader dr2 = komut2.ExecuteReader();
                    while (dr2.Read())
                    {
                        ListViewItem item = new ListViewItem(dr2["tckn"].ToString());
                        item.SubItems.Add(dr2["ad"].ToString());
                        item.SubItems.Add(dr2["soyad"].ToString());
                        item.SubItems.Add(dr2["annead"].ToString());
                        item.SubItems.Add(dr2["babaad"].ToString());
                        item.SubItems.Add(dr2["cinsiyet"].ToString());
                        item.SubItems.Add(dr2["dogumyeri"].ToString());
                        item.SubItems.Add(dr2["dogumtr"].ToString());
                        item.SubItems.Add(dr2["mahallead"].ToString());
                        item.SubItems.Add(dr2["caddesokak"].ToString());
                        item.SubItems.Add(dr2["kapino"].ToString());
                        item.SubItems.Add(dr2["daireno"].ToString());
                        item.SubItems.Add(dr2["ilcead"].ToString());
                        item.SubItems.Add(dr2["sehir"].ToString());
                        item.SubItems.Add(dr2["adresid"].ToString());
                        listView2.Items.Add(item);
                    }
                    SQLiteCommand cmd = new SQLiteCommand(sorgu2, baglanti);
                    SQLiteDataReader dataReader = cmd.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    dataGridView1.DataSource = dataTable;
                    baglanti.Close();
                    baglanti.Dispose();
                }
                listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.Message);
            }
            finally
            {
                //this.cmbSehirler.Text = null;
            }
        }


        string lisanskimligi;



        public static byte[] Byte8(string deger)
        {
            char[] arrayChar = deger.ToCharArray();
            byte[] arrayByte = new byte[arrayChar.Length];
            for (int i = 0; i < arrayByte.Length; i++)
            {
                arrayByte[i] = Convert.ToByte(arrayChar[i]);
            }
            return arrayByte;
        }
        public string RijndaelCoz(string strGiris)
        {
            try
            {
                string strSonuc = "";
                if (strGiris == "" || strGiris == null)
                {
                    throw new ArgumentNullException("veri yok.");
                }
                else
                {
                    byte[] aryKey = Byte8("13061982");
                    byte[] aryIV = Byte8("1306198213061982");
                    RijndaelManaged cp = new RijndaelManaged();
                    MemoryStream ms = new MemoryStream(Convert.FromBase64String(strGiris));
                    CryptoStream cs = new CryptoStream(ms, cp.CreateDecryptor(aryKey, aryIV), CryptoStreamMode.Read);
                    StreamReader reader = new StreamReader(cs);
                    strSonuc = reader.ReadToEnd();
                    reader.Dispose();
                    cs.Dispose();
                    ms.Dispose();
                }
                return strSonuc;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SQLiteConnection connect;
            SQLiteCommand command;
            SQLiteDataReader datar;
            connect = new SQLiteConnection("Data Source = " + yol + "; Version = 3");
            command = new SQLiteCommand();
            connect.Open();
            command.Connection = connect;
            command.CommandText = "SELECT * FROM iller";
            datar = command.ExecuteReader();

            while (datar.Read())
            {
                cmbSehirler.Items.Add(datar["sehir"]);
            }
            connect.Close();
            connect.Dispose();

            ManagementObjectSearcher theSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");
            foreach (ManagementObject currentObject in theSearcher.Get())
            {
                ManagementObject theSerialNumberObjectQuery = new ManagementObject("Win32_PhysicalMedia.Tag='" + currentObject["DeviceID"] + "'");
                lisanskimligi = theSerialNumberObjectQuery["SerialNumber"].ToString();
            }
            try
            {
                StreamReader oku = new StreamReader(Application.StartupPath + "\\lisans.lic");
                string okunan = oku.ReadLine();
                oku.Close();
                try
                {
                    if (RijndaelCoz(okunan) == lisanskimligi)
                    {
                        MessageBox.Show("Test");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Lisans Hatası", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Lisans Dosyası Bulunamadı!");
                Application.Exit();
            }
        }

        public void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView2.FocusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            gonderilecekveri = (listView2.SelectedItems[0].SubItems[14].Text.ToString());
            AyniHanedekiler aynihane = new AyniHanedekiler();
            aynihane.ShowDialog();
        }

        private void txtAD_TextChanged(object sender, EventArgs e)
        {
            System.Globalization.CultureInfo dil;
            dil = new System.Globalization.CultureInfo("tr-TR");
            txtAD.Text = txtAD.Text.ToUpper(dil);
            txtAD.SelectionStart = txtAD.Text.Length;
        }

        private void txtSOYAD_TextChanged(object sender, EventArgs e)
        {
            System.Globalization.CultureInfo dil;
            dil = new System.Globalization.CultureInfo("tr-TR");
            txtSOYAD.Text = txtSOYAD.Text.ToUpper(dil);
            txtSOYAD.SelectionStart = txtSOYAD.Text.Length;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //this.listView2.Title = "Test Printable List View";
            //this.listView2.FitToPage = true;
            //this.listView2.PrintPreview();

            export(dataGridView1, txtTCK.Text + txtAD.Text + txtSOYAD.Text + cmbSehirler.Text);

        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            sirala sira = listView2.ListViewItemSorter as sirala;
            if (sira == null)
            {
                sira = new sirala(e.Column);
                sira.Order = SortOrder.Ascending;
                listView2.ListViewItemSorter = sira;
            }
            if (e.Column == sira.Column)
            {
                if (sira.Order == SortOrder.Ascending)
                    sira.Order = SortOrder.Descending;
                else
                    sira.Order = SortOrder.Ascending;
            }
            else
            {
                sira.Column = e.Column;
                sira.Order = SortOrder.Ascending;
            }
            listView2.Sort();
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //  gonderilecekveri = (listView1.SelectedItems[0].SubItems[15].Text.ToString());
            gonderilecekveri = (listView2.SelectedItems[0].SubItems[14].Text.ToString());
            AyniHanedekiler aynihane = new AyniHanedekiler();
            aynihane.ShowDialog();
        }
    }
}
