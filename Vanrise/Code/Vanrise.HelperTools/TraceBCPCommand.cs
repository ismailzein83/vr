using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanrise.HelperTools.Data;

namespace Vanrise.HelperTools
{
    public partial class TraceBCPCommand : Form
    {
        List<string> rows;

        string data;
        public TraceBCPCommand()
        {

            InitializeComponent();
        }

        private void btn_browse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                btn_trace.Enabled = false;
                rows = new List<string>();
                System.IO.StreamReader sr = new
                   System.IO.StreamReader(openFileDialog1.FileName);
                string row = string.Empty;
                while (!sr.EndOfStream)
                {
                    rows.Add(sr.ReadLine());
                }
                sr.Close();
                btn_trace.Enabled = true;
            }
        }

        private void btn_trace_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_connectionString.Text))
            {
                MessageBox.Show("Please fill Connection String");
                return;
            }

            if (string.IsNullOrEmpty(txt_tableName.Text))
            {
                MessageBox.Show("Please fill Table Name");
                return;
            }

            if (string.IsNullOrEmpty(txt_columns.Text))
            {
                MessageBox.Show("Please fill columns with comma separated");
                return;
            }

            if (rows == null || rows.Count == 0)
            {
                MessageBox.Show("No File selected or Empty File selected");
                return;
            }

            int intervalCount = 1000;
            List<string> columns = txt_columns.Text.Split(',').ToList();

            btn_trace.Enabled = false;
            object obj = new object();

            //First Time invalid data contains all data in the file
            List<string> invalidData = rows;

            //Used to merge data from invalidData List based on intervalCount
            List<string> modifiedData;

            //Used to group data from invalidData List based on intervalCount without merging
            List<List<string>> data;
            
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                while (intervalCount >= 1)
                {
                    modifiedData = PrepareData(invalidData, intervalCount, out data);
                    invalidData = new List<string>();
                    int counter = 0;

                    foreach (string row in modifiedData)
                    {
                        try
                        {
                            HelperToolDataManager dataManager = new HelperToolDataManager(txt_connectionString.Text);
                            dataManager.BulkInsert(txt_tableName.Text, row, txt_columns.Text);
                        }
                        catch (Exception ex)
                        {
                            if (intervalCount == 1)
                            {
                                strBuilder.AppendLine(ex.Message);
                                List<string> modifiedRow = row.Split('^').ToList();
                                StringBuilder errorString = new StringBuilder();
                                int itemCount = 0;
                                foreach (string column in columns)
                                {
                                    errorString.AppendLine(string.Format("{0}: {1}", column, modifiedRow[itemCount]));
                                    itemCount++;
                                }
                                strBuilder.AppendLine(errorString.ToString());
                            }
                            else
                                invalidData.AddRange(data[counter]);
                        }
                        finally
                        {
                            counter++;
                        }
                    }
                    if (invalidData.Count == 0)
                        break;

                    intervalCount = intervalCount / 10;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            if (strBuilder.Length > 0)
            {
                string errorFile = string.Concat("Error_", Guid.NewGuid(), ".error");
                File.WriteAllText(string.Format(@"{0}\{1}", ConfigurationManager.AppSettings["BCPTempFilesDirectory"], errorFile), strBuilder.ToString());
            }

            btn_trace.Enabled = true;

        }

        private List<string> PrepareData(List<string> invalidData, int intervalCount, out List<List<string>> data)
        {
            data = null;
            List<string> result = new List<string>();
            if (invalidData != null && invalidData.Count > 0)
            {
                data = Split<string>(invalidData, intervalCount);
                foreach (List<string> item in data)
                {
                    result.Add(string.Join<string>(Environment.NewLine, item));
                }
            }
            return result;
        }

        private List<List<T>> Split<T>(List<T> source, int intervalCount)
        {
            return source

                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / intervalCount)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
