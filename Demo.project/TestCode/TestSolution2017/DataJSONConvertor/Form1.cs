using Newtonsoft.Json;
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

namespace DataJSONConvertor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFileDialog1.FileName;
            }
        }

        private void btnConvertJSON_Click(object sender, EventArgs e)
        {
            string inputFilePath = txtFilePath.Text;
            if (string.IsNullOrEmpty(inputFilePath))
                return;
            string directory = new FileInfo(inputFilePath).DirectoryName;
            string outputDirectory = Path.Combine(directory, "Output");
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);
            string outputFilePath = Path.Combine(outputDirectory, $"{DateTime.Now.ToString("yyyy-MM-dd HHmmss")}.json");
            int linesConverted = 0;
            DateTime startTime = DateTime.Now;
            using (var reader = new StreamReader(inputFilePath))
            {
                using (var writer = new StreamWriter(outputFilePath))
                {
                    string firstLine = reader.ReadLine();
                    string[] fieldNames = firstLine.Split(',');
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        string[] parts = line.Split(',');
                        Dictionary<string, string> itm = new Dictionary<string, string>();
                        for (int i=0;i<parts.Length;i++)
                        {
                            itm.Add(fieldNames[i], parts[i]);
                        }
                        writer.WriteLine(@"{ ""index"" : {} }");
                        writer.WriteLine(JsonConvert.SerializeObject(itm));
                        linesConverted++;
                        if(linesConverted % 100000 == 0)
                            Console.WriteLine($"{DateTime.Now}: {linesConverted} records converted. {DateTime.Now - startTime} elapsed");
                        line = reader.ReadLine();
                    }
                }
                Console.WriteLine($"{DateTime.Now}: {linesConverted} records converted. {DateTime.Now - startTime} elapsed");
                MessageBox.Show($"Conversion of {linesConverted} records is done in {DateTime.Now - startTime}");
            }
        }
    }
}
