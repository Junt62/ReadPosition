using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadPosition {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        public void Form1_Shown(object sender, EventArgs e) {
            string path = Program.CommandLineArgs;
            path = "C:\\Users\\Administrator\\Downloads\\新建文本文档.txt";
            string pathDirectory = Path.GetDirectoryName(path);

            if (path != null) {
                string pathExtension = Path.GetExtension(path).ToLower();
                if (!Array.Exists(new string[] { ".txt", ".xml" }, ext => ext == pathExtension)) {
                    MessageBox.Show("此程序只接受 txt xml 格式的文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                    return;
                }
            }

            string pattern = "<Entry Path=\"([^\"]*)\"[^>]*ofsx=\"([^\"]*)\"[^>]*ofsy=\"([^\"]*)\"[^>]*>";
            List<EntryData> extractedData = ExtractDataFromFile(path, pattern);
            if (extractedData != null) {
                if (extractedData.Count() > 0) {
                    foreach (EntryData data in extractedData) {

                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(data.Path);

                        string fileName = Path.Combine("Placements", fileNameWithoutExt + ".txt");

                        string fileDirectory = Path.GetDirectoryName(data.Path);

                        if (Regex.IsMatch(Path.GetDirectoryName(data.Path), "^[A-Za-z]:.*$"))
                            fileDirectory = Path.GetDirectoryName(data.Path).Substring(2);
                        else if (!Regex.IsMatch(Path.GetDirectoryName(data.Path), "^\\.*$"))
                            fileDirectory = "\\" + Path.GetDirectoryName(data.Path);

                        string filePath = pathDirectory + Path.Combine(fileDirectory, fileName);

                        if (!Directory.Exists(Path.GetDirectoryName(filePath))) {
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        }

                        using (StreamWriter writer = new StreamWriter(filePath)) {
                            writer.WriteLine(data.Ofsx);
                            writer.WriteLine(data.Ofsy);
                        }

                    }
                    MessageBox.Show("已在文件路径生成坐标文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }
                else {
                    MessageBox.Show("未找到任何可供生成的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                }
            }
            else {
                MessageBox.Show("未找到任何可供生成的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Exit();
            }

        }

        static string[] FindLinesWithPattern(string path, string pattern) {
            try {
                string fileContent = File.ReadAllText(path);

                MatchCollection matches = Regex.Matches(fileContent, pattern);

                if (matches.Count > 0) {
                    string[] matchedLines = new string[matches.Count];
                    for (int i = 0; i < matches.Count; i++) {
                        matchedLines[i] = matches[i].Value;
                    }
                    return matchedLines;
                }
            }
            catch (ArgumentNullException) {
                MessageBox.Show("请拖动 txt xml 格式的文件至本程序图标打开" +
                    "\n" +
                    "\n将在文件路径下生成 Placements 坐标文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (FileNotFoundException) {
                MessageBox.Show("文件不存在，可能被删除或移动", "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            return null;
        }

        static List<EntryData> ExtractDataFromFile(string filePath, string pattern) {

            List<EntryData> extract = new List<EntryData>();

            try {
                string content = File.ReadAllText(filePath);

                MatchCollection matches = Regex.Matches(content, pattern);

                foreach (Match match in matches) {
                    string path = match.Groups[1].Value;
                    string ofsx = match.Groups[2].Value;
                    string ofsy = match.Groups[3].Value;

                    EntryData data = new EntryData {
                        Path = path,
                        Ofsx = ofsx,
                        Ofsy = ofsy
                    };

                    extract.Add(data);
                }
            }
            catch (ArgumentNullException) {
                MessageBox.Show("请拖动 txt xml 格式的文件至本程序图标打开" +
                    "\n" +
                    "\n将在文件路径下生成 Placements 坐标文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
            catch (FileNotFoundException) {
                MessageBox.Show("文件不存在，可能被删除或移动", "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return null;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return null;
            }

            return extract;
        }
    }

}

class EntryData {
    public string Path { get; set; }
    public string Ofsx { get; set; }
    public string Ofsy { get; set; }

    public EntryData() { }
}

