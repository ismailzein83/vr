using Aspose.Cells;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.Bypass
{
    public class ClientVariables
    {
        static string profile_Name = "FMS_Profile";
        static string profile_Madar_Name = "FMS_Madar_Profile";
        static string profile_Syria_Name = "FMS_Syria_Profile";
        public static string GetProfileName(int clientId)
        {
            switch(clientId)
            {
                case (int)Enums.Clients.Madar: return profile_Madar_Name;
                case (int)Enums.Clients.ST: return profile_Syria_Name;
                default: return profile_Name;
            }
        }
        public static string GetReportName(int clientId, int differenceInGMT)
        {
            switch (clientId)
            {
                case (int)Enums.Clients.Madar: return System.DateTime.Now.AddHours(differenceInGMT).Ticks.ToString() + ".csv";
                case (int)Enums.Clients.ST: return System.DateTime.Now.AddHours(differenceInGMT).Ticks.ToString() + ".xls";
                default: return  System.DateTime.Now.AddHours(differenceInGMT).Ticks.ToString() + ".pdf";;
            }
        }

        public static string ExportReportToPDF(string reportName, ReportViewer rdlcReport)
        {
            return ExportReport(reportName, rdlcReport, "PDF");
        }

        public static string ExportReportToExcel(string reportName, ReportViewer rdlcReport)
        {
            return ExportReport(reportName, rdlcReport, "Excel");
        }
        public static string ExportReportToCSV(string reportName, ReportViewer rdlcReport)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;
            byte[] bytes = rdlcReport.LocalReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            string filename = Path.Combine(ConfigurationManager.AppSettings["ReportsPath"], reportName);
            MemoryStream ms = new MemoryStream();
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;
            Workbook workbook = new Workbook(ms);
            Aspose.Cells.License license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Cells.lic");
            workbook.Save(filename, SaveFormat.CSV);
            return filename;
        }

        private static string ExportReport(string reportName, ReportViewer rdlcReport, string reportFormat)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;
            byte[] bytes = rdlcReport.LocalReport.Render(reportFormat, null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            string filename = Path.Combine(ConfigurationManager.AppSettings["ReportsPath"], reportName);
            SaveClientReport(filename, bytes);
            return filename;
        }
        private static void SaveClientReport(string filename, byte[] bytes)
        {
            using (var fs = new FileStream(filename, FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
        }
        public static string SaveCSVClientReport(string reportName, StringBuilder stringBuilder)
        {
            string filename = Path.Combine(ConfigurationManager.AppSettings["ReportsPath"], reportName);
            File.WriteAllText(filename, stringBuilder.ToString());
            return filename;
        }
    }
}
