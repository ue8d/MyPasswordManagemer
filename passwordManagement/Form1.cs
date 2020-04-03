using MySql.Data.MySqlClient;
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
        bool FIG = passwordManagement.Properties.Settings.Default.FIG;

        public Form1()
        {
            InitializeComponent();
            panel1.Parent = outerPanel;
            panel2.Parent = outerPanel;
            tabControl1.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //initializeDebug();//デバック用保存変数初期化
            initialize();
        }

        private void initialize()
        {
            this.label1.Text = "MyPasswordManager - v0.04";
            this.label6.Text = "";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.passwordBox.PasswordChar = '*';

            if (!FIG)//初回起動判定
            {
                this.label9.Visible = false;
                this.label10.Visible = false;
                this.databaseId.Visible = false;
                this.label11.Visible = false;
                this.databasePass.Visible = false;
                this.groupBox2.Visible = false;
            }
            else
            {
                firstLaunch();
            }

            if (!passwordManagement.Properties.Settings.Default.destination)
            {
                this.dataGridView1.Visible = false;
            }
        }

        private void firstLaunch()
        {
            label2.Text = "必要項目を入力してください。";
            button1.Text = "登録";
            label9.Text = "マスターパスワード";
            label10.Text = "DBログインID";
            label11.Text = "DBパスワード";
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            this.panel1.BringToFront();
        }

        //ログインボタン
        private void button1_Click(object sender, EventArgs e)
        {
            if (FIG)
            {
                if((radioButton4.Checked == false) && (radioButton5.Checked == false)){
                    DialogResult result = MessageBox.Show(
                        "保存先を選択してください。",
                        "エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }
                //データ保存先の選択
                if (radioButton5.Checked == true)//DB
                {
                    passwordManagement.Properties.Settings.Default.databaseId = databaseId.Text;
                    passwordManagement.Properties.Settings.Default.databasePassword = databasePass.Text;
                    passwordManagement.Properties.Settings.Default.destination = true;//保存先判定
                }
                else
                {
                    passwordManagement.Properties.Settings.Default.destination = false;
                }

                passwordManagement.Properties.Settings.Default.userMasterPassword = passwordBox.Text;
                passwordManagement.Properties.Settings.Default.FIG = false;
                passwordManagement.Properties.Settings.Default.Save();

                if (passwordManagement.Properties.Settings.Default.destination)
                {
                    initializeDataGrid();
                }
                else
                {
                    initializeListView();
                }

                this.panel1.Visible = false;
                this.panel2.Visible = true;
            }
            else
            {
                if (passwordBox.Text == passwordManagement.Properties.Settings.Default.userMasterPassword)
                {
                    if (passwordManagement.Properties.Settings.Default.destination)
                    {
                        initializeDataGrid();
                    }
                    else
                    {
                        initializeListView();
                    }
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
        }

        //パスワード生成ボタン
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
            //addListView(); //リストビューに追加

            if (passwordManagement.Properties.Settings.Default.destination)
            {
                addSQL();//DB
            }
            else
            {
                addFile(); //ローカルFile書き込み
            }


        }

        private void addSQL()
        {
            String sql = "insert into passwordmanager (site,siteId,password) values('" + textBox3.Text + "','siteid','" + textBox1.Text + "')";
            // MySQLへの接続
            try
            {
                MySqlConnection connection = new MySqlConnection(sLogin);
                connection.Open();

                DataTable dt = new DataTable();

                MySqlDataAdapter da = new MySqlDataAdapter(sql, connection);

                da.Fill(dt);
                dataGridView1.DataSource = dt;

                connection.Close();
                initializeDataGrid();
            }
            catch (MySqlException me)
            {
                MessageBox.Show("ERROR: " + me.Message);
            }
        }

        private void addFile()
        {
            string filePath = @"C:\ue8d\password.csv";
            if (File.Exists(filePath))
            {
                File.AppendAllText(filePath, EncryptString(textBox3.Text, masterPassword) + "," + EncryptString(textBox1.Text, masterPassword) + ",");
                addListView();
            }
            else
            {
                Directory.CreateDirectory(@"C:\ue8d");
                using (FileStream fs = File.Create(filePath))
                {
                    fs.Close();
                }
                addFile();
            }
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
            catch (Exception)
            {
                string filePath = @"C:\ue8d\password.csv";
                if (File.Exists(filePath))
                {
                    DialogResult result = MessageBox.Show(
                    "復元の失敗によりパスワードファイルの読み込みに失敗しました。" + Environment.NewLine + 
                    "お手数をおかけしますが、" + filePath + "を削除して再度お試しください。",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                }      
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

        private void initializeDataGrid()
        {
            // MySQLへの接続
            try
            {
                MySqlConnection connection = new MySqlConnection(sLogin);
                connection.Open();

                DataTable dt = new DataTable();

                MySqlDataAdapter da = new MySqlDataAdapter("select site,password from passwordManager", connection);

                da.Fill(dt);
                dataGridView1.DataSource = dt;

                connection.Close();
            }
            catch (MySqlException me)
            {
                MessageBox.Show("ERROR: " + me.Message);
            }
        }

        private void initializeDebug()
        {
            passwordManagement.Properties.Settings.Default.userMasterPassword = "";
            passwordManagement.Properties.Settings.Default.databaseId = "";
            passwordManagement.Properties.Settings.Default.databasePassword = "";
            passwordManagement.Properties.Settings.Default.FIG = true;
            passwordManagement.Properties.Settings.Default.Save();
        }
    }
}