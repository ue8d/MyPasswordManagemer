using System;
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
            panel1.Parent = outerPanel;
            panel2.Parent = outerPanel;
            tabControl1.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.label1.Text = "MyPasswordManager - v0.01";
            this.label2.Text = "パスワードを入力してください";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            passwordBox.PasswordChar = '*';
            initializeListView();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            this.panel1.BringToFront();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(passwordBox.Text == "")
            {
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
                    "1以上、300以下の数字にのみ対応しています。",
                    "MyPasswordManager",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2
            );
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

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void initializeListView()
        {
            //詳細表示にする
            listView1.View = View.Details;
            //初期値の設定
            ListViewItem itemx = new ListViewItem();
            itemx.Text = "この機能は";
            itemx.ImageIndex = 0;
            itemx.SubItems.Add("まだ");
            itemx.SubItems.Add("実装されていません");
            //リストビューに項目を追加
            listView1.Items.Add(itemx);
        }
    }
}