using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyPasswordManager
{
    public partial class Form1 : Form
    {
        private static readonly string passwordChars1 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string passwordChars2 = "0123456789abcdefghijklmnopqrstuvwxyz";
        private static readonly string passwordChars3 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_-+=[{]};:<>|./?";
        String passwordChars;
        String masterPassword = "test";

        public Form1()
        {
            InitializeComponent();
            panel1.Parent = outerPanel;
            panel2.Parent = outerPanel;
            tabControl1.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.label1.Text = "MyPasswordManager - v0.03";
            this.label6.Text = "";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            passwordBox.PasswordChar = '*';
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            this.panel1.BringToFront();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(passwordBox.Text == "")
            {
                initializeListView();
                this.panel1.Visible = false;
                this.panel2.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if((int.Parse(textBox2.Text) >= 1) && (int.Parse(textBox2.Text) <= 300))
            {
                //使用文字列の選定
                if (radioButton1.Checked == true)
                {
                    passwordChars = passwordChars1;
                }
                else if (radioButton2.Checked == true)
                {
                    passwordChars = passwordChars2;
                }
                else
                {
                    passwordChars = passwordChars3;
                }
                //パスワードの作成
                textBox1.Text = generatePassWord(int.Parse(textBox2.Text));
            }
            else
            {
                DialogResult result = MessageBox.Show(
                    "1以上、300以下の数字を入力してください。",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        private string generatePassWord(int length)
        {
            StringBuilder sb = new StringBuilder(length);
            Random r = new Random();

            for (int i = 0; i < length; i++)
            {
                //文字の位置をランダムに選択
                int pos = r.Next(passwordChars.Length);
                //選択された位置の文字を取得
                char c = passwordChars[pos];
                //パスワードに追加
                sb.Append(c);
            }

            return sb.ToString();
        }

        //保存ボタン
        private void button3_Click(object sender, EventArgs e)
        {
            addListView();//リストビューに追加
            addFile();//File書き込み
        }

        private void addFile()
        {
            File.AppendAllText(@"C:\ue8d\test.txt", EncryptString(textBox3.Text,masterPassword) + "," + EncryptString(textBox1.Text,masterPassword) + ",");
        }

        private void addListView()
        {
            ListViewItem lvi;
            lvi = listView1.Items.Add(textBox3.Text);
            lvi.SubItems.Add(textBox1.Text);
            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void initializeListView()
        {
            //詳細表示にする
            listView1.View = View.Details;
            //初期値の設定
            listView1.Columns.Add("サイト名");
            listView1.Columns.Add("パスワード");
            ListViewItem lvi;
            //本来はデータベースから値を取ってくる
            var line = File.ReadAllText(@"C:\ue8d\test.txt");
            var l = line.Split(',');
            for (var i = 0; i < l.Length - 1; i++)
            {
                lvi = listView1.Items.Add(DecryptString(l[i],masterPassword));
                lvi.SubItems.Add(DecryptString(l[i + 1],masterPassword));
                i++;
            }
            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                int idx = 0;
                idx = listView1.SelectedItems[0].Index;
                Clipboard.SetText(listView1.Items[idx].SubItems[1].Text);
                this.label6.Text = "コピーしました";
            }
        }


        /// <summary>
        /// 文字列を暗号化する
        /// </summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="password">暗号化に使用するパスワード</param>
        /// <returns>暗号化された文字列</returns>
        private static string EncryptString(string sourceString, string password)
        {
            //RijndaelManagedオブジェクトを作成
            System.Security.Cryptography.RijndaelManaged rijndael =
                new System.Security.Cryptography.RijndaelManaged();

            //パスワードから共有キーと初期化ベクタを作成
            byte[] key, iv;
            GenerateKeyFromPassword(
                password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
            rijndael.Key = key;
            rijndael.IV = iv;

            //文字列をバイト型配列に変換する
            byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(sourceString);

            //対称暗号化オブジェクトの作成
            System.Security.Cryptography.ICryptoTransform encryptor =
                rijndael.CreateEncryptor();
            //バイト型配列を暗号化する
            byte[] encBytes = encryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);
            //閉じる
            encryptor.Dispose();

            //バイト型配列を文字列に変換して返す
            return System.Convert.ToBase64String(encBytes);
        }

        /// <summary>
        /// 暗号化された文字列を復号化する
        /// </summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="password">暗号化に使用したパスワード</param>
        /// <returns>復号化された文字列</returns>
        private static string DecryptString(string sourceString, string password)
        {
            //RijndaelManagedオブジェクトを作成
            System.Security.Cryptography.RijndaelManaged rijndael =
                new System.Security.Cryptography.RijndaelManaged();

            //パスワードから共有キーと初期化ベクタを作成
            byte[] key, iv;
            GenerateKeyFromPassword(
                password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
            rijndael.Key = key;
            rijndael.IV = iv;

            //文字列をバイト型配列に戻す
            byte[] strBytes = System.Convert.FromBase64String(sourceString);

            //対称暗号化オブジェクトの作成
            System.Security.Cryptography.ICryptoTransform decryptor =
                rijndael.CreateDecryptor();
            //バイト型配列を復号化する
            //復号化に失敗すると例外CryptographicExceptionが発生
            byte[] decBytes = decryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);
            //閉じる
            decryptor.Dispose();

            //バイト型配列を文字列に戻して返す
            return System.Text.Encoding.UTF8.GetString(decBytes);
        }

        /// <summary>
        /// パスワードから共有キーと初期化ベクタを生成する
        /// </summary>
        /// <param name="password">基になるパスワード</param>
        /// <param name="keySize">共有キーのサイズ（ビット）</param>
        /// <param name="key">作成された共有キー</param>
        /// <param name="blockSize">初期化ベクタのサイズ（ビット）</param>
        /// <param name="iv">作成された初期化ベクタ</param>
        private static void GenerateKeyFromPassword(string password,
            int keySize, out byte[] key, int blockSize, out byte[] iv)
        {
            //パスワードから共有キーと初期化ベクタを作成する
            //saltを決める
            byte[] salt = System.Text.Encoding.UTF8.GetBytes("saltは必ず8バイト以上");
            //Rfc2898DeriveBytesオブジェクトを作成する
            System.Security.Cryptography.Rfc2898DeriveBytes deriveBytes =
                new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt);
            //反復処理回数を指定する デフォルトで1000回
            deriveBytes.IterationCount = 1000;

            //共有キーと初期化ベクタを生成する
            key = deriveBytes.GetBytes(keySize / 8);
            iv = deriveBytes.GetBytes(blockSize / 8);
        }
    }
}