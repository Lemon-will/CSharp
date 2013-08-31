using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnimeListProgram
{
    public partial class Form1 : Form
    {
        readonly int columnNum = 4;   //リストにいるコラムの数。普通変わらないはずなのでreadonly
        readonly string defaultPath = "defaultPath.txt";    //デフォルトパスが記憶されているファイルのパス
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)//起動時のロード処理
        {
            if (!File.Exists(defaultPath))
            {
                MessageBox.Show("開くtxtを選んでください");
                selectProgram();
            }
            else
            {
                string s = File.ReadAllText(defaultPath);
                loadProgram(s);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void TodayButton_Click(object sender, EventArgs e)
        {

        }

        private void openButton_Click(object sender, EventArgs e)
        {
            selectProgram();
        }

        private void selectProgram()//番組表をファイルダイアログ起動して読み込んでリストに載せるメソッド
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;//OK以外がクリックされた場合は中止

            loadProgram(openFileDialog1.FileName);
        }

        private void loadProgram(string filename)   //番組表を読み込んで反映
        {
            using (var infile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read)))//StreamReaderを作成し、同時に処理を記述
            {
                var list = new List<string[]>();
                for (; ; )
                {
                    string[] str = new string[columnNum];
                    for (int i = 0; i < columnNum; i++)
                    {
                        str[i] = infile.ReadLine();
                    }
                    list.Add(str);
                    if (str[columnNum - 1] == null) break;

                }
                foreach (string[] data in list) { listView1.Items.Add(new ListViewItem(data)); }
                foreach (ColumnHeader ch in listView1.Columns) { ch.Width = -2; }//リストの幅を調整する。
                fileName.Text = filename;//ステータスバーに現在参照しているファイルを表示
            }
        }

        private void fileName_Click(object sender, EventArgs e)
        {

        }

        private void changeDefaultButton_Click(object sender, EventArgs e)
        {
            string def = fileName.Text;
            File.WriteAllText(defaultPath, def);//defaultPath.txtにデフォルトにするパスを記述し上書き保存する(場所はexeと同じディレクトリ)
        }
    }
}
