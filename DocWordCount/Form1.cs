using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace DocWordCount
{
    public partial class Form1 : Form
    {
        List<string> files;
        Dictionary<string,int> dict=null;
        Dictionary<int, string> dictindex = null;
        string rootpath="";
        string savepath="";
        bool isRunning = false;
        DateTime startTime;
        int count;
        string openpath="";
        
        public Form1()
        {
            InitializeComponent();
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isRunning) return;
            if (!this.folderBrowserDialog1.ShowDialog().Equals(DialogResult.OK)) return;
            files = new List<string>();
            rootpath = this.folderBrowserDialog1.SelectedPath;
            LoadFolderFiles();
             
        }
        private void LoadFolderFiles()
        {
            this.OpenFolder(rootpath);
            this.listBox1.Items.Clear();
            this.listBox1.Items.AddRange(files.ToArray());
            this.button1.Visible = true;
            this.button1.Enabled = true;
            this.button2.Enabled = true;
            this.button3.Enabled = true;
            this.toolStripStatusLabel1.Text = "Ready";
            this.toolStripProgressBar1.Value = 0;
        }
        private void OpenFolder(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] fs = dir.GetFiles("*.txt");
                foreach (FileInfo f in fs) files.Add(f.FullName.Substring(rootpath.Length));
                DirectoryInfo[] subdirs = dir.GetDirectories();
                foreach (DirectoryInfo d in subdirs) OpenFolder(d.FullName);
            }
            catch {
                MessageBox.Show("Path not found: " + path);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Filter = "*.txt";
            if (!this.saveFileDialog1.ShowDialog().Equals(DialogResult.OK)) return;
            savepath = this.saveFileDialog1.FileName;
            this.button1.Enabled = false;
            this.toolStripProgressBar1.Visible = true;
            this.toolStripStatusLabel1.Text = "Counting...";
            this.timer1.Start();
            startTime = DateTime.Now;
            ThreadStart start = new ThreadStart(Run);
            Thread thread = new Thread(start);
            thread.Start();
          
        }
        private void Run()
        {
            isRunning = true;
            int count1 = 0;
            LoadDict(); 
               
                 
                foreach (string f in files)
                {
                    encode(f);
                    count1++;
                    count = count1 * 100 / files.Count;
                }
                isRunning = false;
   
        }
        private void Run1()
        {
            isRunning = true;
            int count1 = 0;
            LoadDict();


            foreach (string f in files)
            {
                encode1(f);
                count1++;
                count = count1 * 100 / files.Count;
            }
            isRunning = false;

        }
        private void UpdateStatus()
        {
                this.toolStripProgressBar1.Value = count;
            DateTime nowTime = DateTime.Now;
            TimeSpan span = nowTime - startTime;
            this.toolStripStatusLabel1.Text = "Time spent: " + span.ToString();
        }
        
        private void End()
        {
           // this.menuStrip1.Enabled = true;
           // this.button1.Enabled = true;
            this.toolStripStatusLabel1.Text += " Done!";
        }
        private  void LoadDict()
        {
            if (this.dict != null) return;
            DirectoryInfo dir = new DirectoryInfo(Application.ExecutablePath);
            string dictpath = dir.Parent.Parent.FullName + "\\data\\dict.dat";
            this.dict = new Dictionary<string, int>();
            this.dictindex = new Dictionary<int, string>();
            using (StreamReader sr = new StreamReader(dictpath))
            { 
                string line="";
                while((line=sr.ReadLine())!=null)
                {
                    string[]st=line.Trim().Split('\t');
                    if(st.Length<2)continue;
                    string word = st[0].Trim();
                    int index = int.Parse(st[1].Trim());
                    if (word == "") continue;
                    if (!dict.ContainsKey(word)) dict.Add(word, index);
                    if (!dictindex.ContainsKey(index)) dictindex.Add(index, word);
                }              
            }
        }
        private void encode(string path)
        {
            string text = "";
            using (StreamReader sr = new StreamReader(rootpath + path,true))
            {
                 text = sr.ReadToEnd().Trim().ToLower();        
            }
            Dictionary<string, int> data = new Dictionary<string, int>();
            string[] words = text.Split(",.?/:;'\"[]{}\\|1234567890_=!@#$%^&*()_+ \t\r\n¿¡-‑—―-~‘’»“”«·…§".ToCharArray());
            foreach (string w in words)
            {
                string wt = w.Trim();
                if (wt == "") continue;
                if (data.ContainsKey(wt)) data[wt]++;
                else data.Add(wt, 1);
            }
            FileStream fs = new FileStream(savepath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
           
                sw.Write(path);
                foreach (string wt in data.Keys)
                {
                    if (!dict.ContainsKey(wt)) dict.Add(wt, dict.Count + 100);
                    sw.Write(";"+dict[wt]+","+data[wt]);
                }
                sw.WriteLine();
                sw.Close();
                fs.Close();
        }
        private void encode1(string path)
        {
            string text = "";
            using (StreamReader sr = new StreamReader(rootpath + path,true))
            {
                
                text = sr.ReadToEnd().Trim().ToLower();
                          }
            Dictionary<string, int> data = new Dictionary<string, int>();
            string[] words = text.Split(",.?/:;'\"[]{}\\|1234567890_=!@#$%^&*()_+ \t\r\n¿¡-‑—―-~‘’»“”«·…§€?".ToCharArray());
            StringBuilder sb = new StringBuilder();
            sb.Append(path.Replace(".txt", "") + ",");
            foreach (string w in words) sb.Append(cleanWord(w) + " ");
            FileStream fs = new FileStream(savepath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs); 
            sw.WriteLine(sb.ToString().Trim());
            sw.Close();
            fs.Close();
        }
        private string cleanWord(string word)
        {
            StringBuilder sb = new StringBuilder();
            string test = ",./<>?::'\"[]{}|\\`~!@#$%^&*()-_=+";
            for (int i = 0; i < word.Length; i++)
            {
                char c = word[i];
                bool yes=false;
                if(c>='a'&&c<='z')yes=true;
                if(c>='A'&&c<='Z')yes=true;
                if(c>='0'&&c<='9')yes=true;
                if (test.Contains(c)) yes = true;
                if (yes) sb.Append(c);
            }
            return sb.ToString();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isRunning) UpdateStatus();
            else { this.timer1.Stop(); End(); this.toolStripProgressBar1.Value = 100; LoadFolderFiles(); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.rootpath == "")
            {
                MessageBox.Show("Open an empty folder to save the decoded files.");
                return;
            }
            if (this.files.Count > 0)
            {
                MessageBox.Show("Error: The openned folder already has files. You need to open an empty folder for decoding.");
                return;
            }
            if (!this.openFileDialog1.ShowDialog().Equals(DialogResult.OK)) return;
            this.openpath = this.openFileDialog1.FileName;
            isRunning = true;
          
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.toolStripProgressBar1.Visible = true;
            this.toolStripStatusLabel1.Text = "Counting...";
            this.timer1.Start();
            startTime = DateTime.Now;
            ThreadStart start = new ThreadStart(decode);
            Thread thread = new Thread(start);
            thread.Start();

        }
        private void decode()
        {
            LoadDict();
            FileStream fs = new FileStream(openpath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string line = "";
            while (sr.Peek() > -1)
            {
                line = sr.ReadLine().Trim();
                if(line=="")continue;
                string[] st = line.Split(';');
                string filepath = st[0];
                string folderpath =rootpath+ filepath.Substring(0, filepath.LastIndexOf('\\'));
                if (!Directory.Exists(folderpath)) Directory.CreateDirectory(folderpath);
                try
                {
                    FileStream fs1 = new FileStream(rootpath + filepath, FileMode.CreateNew, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs1);
                    for (int i = 1; i < st.Length; i++)
                    {
                        string[] st1 = st[i].Trim().Split(',');
                        if (st1.Length != 2) continue;
                        try
                        {
                            int index = int.Parse(st1[0].Trim());
                            int freq = int.Parse(st1[1].Trim());
                            string word="";
                            if (dictindex.ContainsKey(index))
                            {
                                word = dictindex[index]; 
                            }
                            else
                            {
                                word = RandomStr();
                                dictindex.Add(index, word);
                            }
                            StringBuilder sb=new StringBuilder();
                            for (int k = 0; k < freq; k++) sb.Append(word + " ");
                            sw.WriteLine(sb.ToString().Trim());
                        }
                        catch { };
                    }
                        sw.Close();
                    fs1.Close();
                    count = (int)(fs.Position * 100 / fs.Length);
                }
                catch (Exception exception){
                    MessageBox.Show(exception.Message);
                    isRunning = false;
                    return;
                }

            }
            sr.Close();
            fs.Close();
            isRunning = false;
        }
        private string RandomStr()
        {
            while (true)
            {
                string rStr = Path.GetRandomFileName();
                rStr = rStr.Replace(".", ""); // For Removing the .
                if (!dict.ContainsKey(rStr)) return rStr;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count == 0) return;
            if (this.listBox1.SelectedIndex < 0) return;
            string path = this.listBox1.Items[this.listBox1.SelectedIndex].ToString();
            FileStream fs = new FileStream(rootpath + path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, true);
            this.richTextBox1.Text = sr.ReadToEnd();
           // this.richTextBox1.LoadFile(rootpath + path, RichTextBoxStreamType.UnicodePlainText);
            sr.Close();
            fs.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            this.saveFileDialog1.Filter = "CSV Files|*.csv";
            if (!this.saveFileDialog1.ShowDialog().Equals(DialogResult.OK)) return;
            savepath = this.saveFileDialog1.FileName;
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.toolStripProgressBar1.Visible = true;
            this.toolStripStatusLabel1.Text = "Counting...";
            this.timer1.Start();
            startTime = DateTime.Now;
            ThreadStart start = new ThreadStart(Run1);
            Thread thread = new Thread(start);
            thread.Start();
        }
    }
}
