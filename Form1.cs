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
        //    readonly int columnNum = 4;   //リストにいるコラムの数。普通変わらないはずなのでreadonly
        readonly string defaultPath = "defaultPath.txt";    //デフォルトパスが記憶されているファイルのパス
        readonly DateTime todayDay;
        readonly int todayDate;

        List<string[]> currentList; //現在処理中のリスト(フィルタは無視する、つまりファイルの生データ)


        public Form1()
        {
            InitializeComponent();
            todayDay = DateTime.Today;
            todayDate = (int)todayDay.DayOfWeek;
        }

        private void Form1_Load(object sender, EventArgs e)//起動時のロード処理
        {
            listView1.ColumnClick += new ColumnClickEventHandler(listView1_ColumnClick);    //listView1.ColumnClickにコラムイベントハンドラを追加
            today.Text = todayDay.ToString("yyyy/MM/dd") + "(" + dateNumToString(todayDate) + ")";
            //デフォルトパスが書いてあるtxtをカレントディレクトリから探して読み込み、ない場合は開くcsvを選ばせる
            if (!File.Exists(defaultPath))
            {
                MessageBox.Show("開くcsvを選んでください");
                selectProgram();
            }
            else
            {
                string s = File.ReadAllText(defaultPath);
                loadProgram(s);
            }
        }


        /*  以下GUIの挙動を規定するメソッド(消去しないこと)  */

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {

        }

        private void todayButton_Click(object sender, EventArgs e)
        {
            filterDate(todayDate);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            selectProgram();
        }

        private void fileName_Click(object sender, EventArgs e)
        {

        }

        private void changeDefaultButton_Click(object sender, EventArgs e)
        {
            string def = fileName.Text;
            File.WriteAllText(defaultPath, def);//defaultPath.txtにデフォルトにするパスを記述し上書き保存する(場所はexeと同じディレクトリ)
            MessageBox.Show("現在のリストをデフォルトにしました");
        }

        private void SunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterDate(dateStringToNum("日"));
        }

        private void MonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterDate(dateStringToNum("月"));
        }

        private void TueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterDate(dateStringToNum("火"));
        }

        private void WedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterDate(dateStringToNum("水"));
        }

        private void ThuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterDate(dateStringToNum("木"));
        }

        private void FriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterDate(dateStringToNum("金"));
        }

        private void SatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterDate(dateStringToNum("土"));
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            foreach (string[] data in currentList) { listView1.Items.Add(new ListViewItem(data)); }
            foreach (ColumnHeader ch in listView1.Columns) { ch.Width = -2; }//リストの幅を調整する。

        }


        /*  以下ファイル処理などの処理系メソッド  */

        /// <summary>
        /// 番組表を読み込んで反映
        /// </summary>
        /// <param name="filename">読み込むファイル名</param>
        private void loadProgram(string filename)
        {
            using (var infile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Default))//StreamReaderを作成し、同時に処理を記述
            {
                var list = new List<string[]>();
                while (!infile.EndOfStream)
                {
                    string line = infile.ReadLine();
                    string[] str = line.Split(',');
                    list.Add(str);
                }
                foreach (string[] data in list) { listView1.Items.Add(new ListViewItem(data)); }
                foreach (ColumnHeader ch in listView1.Columns) { ch.Width = -2; }//リストの幅を調整する。
                fileName.Text = filename;//ステータスバーに現在参照しているファイルを表示
                currentList = list;     //現在のリストを更新
            }
        }

        /// <summary>
        /// 番組表をファイルダイアログ起動して読み込んでリストに載せる
        /// </summary>
        private void selectProgram()
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;//OK以外がクリックされた場合は中止

            loadProgram(openFileDialog1.FileName);
        }

        /// <summary>
        /// int型の曜日を日本語の曜日のstringにする
        /// </summary>
        /// <param name="date">変換する曜日を表すint</param>
        /// <returns>日本語の曜日のstring</returns>
        private string dateNumToString(int date)
        {
            return ("日月火水木金土").Substring(date, 1);
        }

        /// <summary>
        /// string型の日本語表記の曜日を日を0としたint型にする
        /// 曜日以外のstringを受け取った場合はエラーのメッセージを出現させ、-1を返す。
        /// </summary>
        /// <param name="date">string型の曜日</param>
        /// <returns>日=0、月=1…としたint。曜日以外のstringの場合は-1を返す。</returns>
        private int dateStringToNum(string date)
        {
            int num = 0;
            if (date == "日") { num = 0; }
            else if (date == "月") { num = 1; }
            else if (date == "火") { num = 2; }
            else if (date == "水") { num = 3; }
            else if (date == "木") { num = 4; }
            else if (date == "金") { num = 5; }
            else if (date == "土") { num = 6; }
            else
            {
                num = -1;
            }
            return num;
        }
        /// <summary>
        /// 引数の曜日のリストを返す
        /// </summary>
        /// <param name="date">曜日を表すint</param>
        /// <returns>フィルターがかけられたリスト</returns>
        private List<string[]> filterDate(int date)
        {
            List<string[]> list = new List<string[]>();
            foreach (string[] s in currentList)
            {
                int d = dateStringToNum(s[2]);
                if (d == date)
                {
                    list.Add(s);
                }
            }
            listView1.Items.Clear();
            foreach (string[] data in list) { listView1.Items.Add(new ListViewItem(data)); }
            foreach (ColumnHeader ch in listView1.Columns) { ch.Width = -2; }//リストの幅を調整する。
            dateSplitButton.Text = dateNumToString(date);

            return list;
        }


    }
}
