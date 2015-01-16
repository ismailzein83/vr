using System;
using System.Data;


namespace ExcelDataReader
{    // Used to add extended features to the dataexcelreader class 
    public class Utility
    {

        public static readonly DateTime ReferenceDate = new DateTime(1899, 12, 30);

        public static DateTime GetDate(string embeddedNumber)
        {
            return ReferenceDate.AddDays(double.Parse(embeddedNumber));
        }

        public static TimeSpan GetTime(string embeddedNumber)
        {
            return DateTime.Today.AddDays(double.Parse(embeddedNumber)).TimeOfDay;
        }

        public static void FixDateTable(DataTable table, int dateColumn, int startingRow)
        {
            for (int i = startingRow; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                if (row[dateColumn] != DBNull.Value)
                    row[dateColumn] = GetDate(row[dateColumn].ToString());
            }
        }

        public static void FixTimeTable(DataTable table, int timeColumn, int startingRow)
        {
            for (int i = startingRow; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                if (row[timeColumn] != DBNull.Value)
                {
                    string value = row[timeColumn].ToString();
                    if (value.IndexOf(":") > 0)
                        row[timeColumn] = TimeSpan.Parse(value);
                    else
                        row[timeColumn] = GetTime(value);
                }
            }
        }
    }
}
