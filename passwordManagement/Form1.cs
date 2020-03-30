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

        public Form1()
        {
            InitializeComponent();
            //初回起動判定
            if (passwordManagement.Properties.Settings.Default.FIG)
            {
                //本来はここでマスターパスワードを入力させる
                DialogResult result = MessageBox.Show(
                    "初回起動",
                    "お知らせ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                passwordManagement.Properties.Settings.Default.FIG = false;
                passwordManagement.Properties.Settings.Default.Save();
            }
            panel1.Parent = outerPanel;
            panel2.Parent = outerPanel;
            tabControl1.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.label1.Text = "MyPasswordManager - v0.04";
            this.label6.Text = "";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            passwordBox.PasswordChar = '*';
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            this.panel1.BringToFront();
        }

        //ログインボタン
        private void button1_Click(object sender, EventArgs e)
        {
            if(passwordBox.Text == "")
            {
                initializeListView();
                this.panel1.Visible = false;
                this.panel2.Visible = true;
            }
            else
            {
                DialogResult result = MessageBox.Show(
                    "マスターパスワードが違います。",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                passwordBox.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if ((int.Parse(textBox2.Text) >= 1) && (int.Parse(textBox2.Text) <= 300))
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
            catch(Exception)
            {
                DialogResult result = MessageBox.Show(
                        "数字を入力してください。",
                        "エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                textBox2.Text = "32";
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
            File.AppendAllText(@"C:\ue8d\password.csv", EncryptString(textBox3.Text,masterPassword) + "," + EncryptString(textBox1.Text,masterPassword) + ",");
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
            try
            {
                var line = File.ReadAllText(@"C:\ue8d\password.csv");
                var l = line.Split(',');
                for (var i = 0; i < l.Length - 1; i++)
                {
                    lvi = listView1.Items.Add(DecryptString(l[i], masterPassword));
                    lvi.SubItems.Add(DecryptString(l[i + 1], masterPassword));
                    i++;
                }
            }
            catch (Exception e)
            {
                DialogResult result = MessageBox.Show(
                    "パスワードファイルの読み込みに失敗しました。",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
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

        //これは要らない子（後で消す）
        private void Form1_Shown(object sender, EventArgs e)
        {
            passwordManagement.Properties.Settings.Default.userMasterPassword = "test";
            passwordManagement.Properties.Settings.Default.Save();
        }
    }
}