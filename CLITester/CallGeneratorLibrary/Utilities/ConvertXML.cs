using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CallGeneratorLibrary.Utilities
{
    public class ConvertXML
    {
        public static string ConvertToXML(string fileName)
        {
            try
            {
                StringBuilder xml = new StringBuilder();
                xml.Append("<?xml version='1.0' encoding='UTF-8'?>");
                xml.Append("<items>");

                string[] lines = System.IO.File.ReadAllLines(fileName);

                List<int> rows = new List<int>();
                List<int> cols = new List<int>();

                //read the columns from the first line in the file (skip cell (0,0)
                string[] strCols = lines[0].Split(new char[] { ',' });
                for (int i = 1; i < strCols.Length; i++)
                {
                    if (!String.IsNullOrEmpty(strCols[i]))
                        cols.Add(int.Parse(strCols[i]));
                }

                //read the row headers - first item in every line except the first line
                for (int i = 1; i < lines.Length; i++)
                {
                    string rowIndexStr = lines[i].Substring(0, lines[i].IndexOf(","));
                    rows.Add(int.Parse(rowIndexStr));
                }

                //build a matrix
                string[,] matrixString = new string[5000, 5000];

                for (int i = 1; i < lines.Length; i++)
                {
                    string[] items = lines[i].Split(new char[] { ',' });

                    for (int j = 1; j < items.Length; j++)
                    {
                        matrixString[j - 1, i - 1] = items[j];
                    }
                }

                for (int i = 0; i < cols.Count; i++)
                {
                    for (int j = 0; j < rows.Count; j++)
                    {
                        xml.Append("<item col='" + cols[i] + "' row='" + rows[j] + "'>" + matrixString[i, j] + "</item>");
                    }
                }

                xml.Append("</items>");

                return xml.ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}
