using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using TABS.Addons.Utilities.ExtensionMethods;
using ICSharpCode.SharpZipLib.Zip;
using TABS.Components.CodeComparison;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Code Comparison", "Send code comparison email to defined recepients.")]
    public class CodeComparisonRunnable : RunnableBase
    {
         log4net.ILog log = log4net.LogManager.GetLogger(typeof(CodeComparisonRunnable));
        private byte[] GetCompressed(byte[] bytes, string entryName)
        {
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            using (ZipOutputStream s = new ZipOutputStream(memStream))
            {
                s.SetLevel(9);
                ZipEntry entry = new ZipEntry(entryName);
                s.PutNextEntry(entry);
                s.Write(bytes, 0, bytes.Length);
                s.Finish();
                s.Close();
            }
            return memStream.ToArray();
        }

        public override void Run()
        {
            this._LastRun = DateTime.Now;
            try
            {
                bool success = TABS.Components.Engine.BuildRoutesForCodeComparison(log);
                if (!success)
                    return;
                ComparisonOptions options = new ComparisonOptions(TABS.SystemParameter.CodeComparisonOptions.Value.ToString());
                this.status = "Comparing Codes";
                string[] accIds = options.DefaultSuppliers.Split(',');
                var suppliers = TABS.CarrierAccount.Suppliers.Where(x => x.CarrierAccountID.IsIn(accIds)).ToList();
                TABS.Components.CodeComparison.Comparison codeComparison = new TABS.Components.CodeComparison.Comparison("", suppliers, Math.Min(suppliers.Count / 2 + 1, suppliers.Count));

                this.status = "Generating Excel";
                byte[] buffer = GetExcelReportBytes(codeComparison, false, null, null);

                TABS.SpecialSystemParameters.SmtpInfo info = new TABS.SpecialSystemParameters.SmtpInfo();
                string mailTo = options.MailRecepients;
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new System.Net.Mail.MailAddress(info.Default_From);
                if (!string.IsNullOrEmpty(mailTo.Trim())) mailTo.Trim()
                                                                    .Split(',')
                                                                    .ToList().ForEach(t => mail.To.Add(t.Trim()));
                mail.Subject = string.Format("T.One - Code Comparison Report");
                mail.IsBodyHtml = true;
                mail.Body = "Code Comparison Report.";

                buffer = GetCompressed(buffer, "CodeComparisonReport.xls");
                System.IO.MemoryStream memStream = new System.IO.MemoryStream(buffer);
                mail.Attachments.Add(new System.Net.Mail.Attachment(memStream, "CodeComparisonReport.zip", "application/zip"));
                Exception ex;

                this.status = "Sending Mail";
                TABS.Components.EmailSender.Send(mail, out ex);

                if (ex != null)
                    throw ex;
            }
            catch (Exception ex)
            {
                if (ex != null)
                    throw ex;
            }
            finally
            {
                base._IsRunning = false;
                base._IsStopRequested = false;
                base._LastRunDuration = DateTime.Now.Subtract(this.LastRun.Value);
                this.status = string.Empty;
            }
        }

        string status = string.Empty;

        public override string Status
        {
            get { return status; }
        }

        public static byte[] GetExcelReportBytes(DataTable dt, bool actionOnly, TABS.CarrierAccount nameSupplier, string[] mobileKeywords)
        {
            if (actionOnly && nameSupplier == null)
                throw new ArgumentException("nameSupplier cannot be null");

            List<DataRow> rowsToRemove = new List<DataRow>();
            List<DataColumn> colsToRemove = new List<DataColumn>();
            if (actionOnly)
            {

                //filter out columns
                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.ColumnName.IsIn(new string[] { "Destination", "Code", "Action" }))
                        colsToRemove.Add(col);
                }
                foreach (DataColumn col in colsToRemove)
                {
                    dt.Columns.Remove(col);
                }
                //remove no action rows
                foreach (DataRow dr in dt.Rows)
                    if (dr["Action"].ToString() == string.Empty)
                        rowsToRemove.Add(dr);
                foreach (DataRow dr in rowsToRemove)
                    dt.Rows.Remove(dr);
                //reorder
                dt.Columns["Destination"].SetOrdinal(0);
                dt.Columns["Code"].SetOrdinal(1);
                dt.Columns["Action"].SetOrdinal(2);

                //Add M/F
                dt.Columns.Add("M/F");
                if (mobileKeywords != null && mobileKeywords.Length > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["M/F"] = "F";
                        foreach (var mobileKeyword in mobileKeywords)
                            if (mobileKeyword.Trim() != "" && dr["Destination"].ToString().ToLower().Contains(mobileKeyword.Trim().ToLower()))
                            {
                                dr["M/F"] = "M";
                                break;
                            }
                    }
                }
            }
            else
            {
                //filter out columns
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.IsIn(new string[] { "Destination" }))
                        colsToRemove.Add(col);
                }
                foreach (DataColumn col in colsToRemove)
                {
                    dt.Columns.Remove(col);
                }
            }

            if (dt.DataSet == null)
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
            }
            return TABS.Addons.Utilities.ExcelDataWriter.ExcelGenerator.GetExcelBytes(dt.DataSet, xlsgen.enumExcelTargetVersion.excelversion_2007);
        }

        public static byte[] GetExcelReportBytes
            (
            TABS.Components.CodeComparison.Comparison codeComparison,
            bool actionOnly,
            TABS.CarrierAccount nameSupplier,
            string[] mobileKeywords)
        {
            if (actionOnly && nameSupplier == null)
                throw new ArgumentException("nameSupplier cannot be null");
            DataTable dt = codeComparison.ToFlatDataTable(nameSupplier,true);
            List<DataRow> rowsToRemove = new List<DataRow>();
            List<DataColumn> colsToRemove = new List<DataColumn>();
            if (actionOnly)
            {

                //filter out columns
                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.ColumnName.IsIn(new string[] { "Destination", "Code", "Action" }))
                        colsToRemove.Add(col);
                }
                foreach (DataColumn col in colsToRemove)
                {
                    dt.Columns.Remove(col);
                }
                //remove no action rows
                foreach (DataRow dr in dt.Rows)
                    if (dr["Action"].ToString() == string.Empty)
                        rowsToRemove.Add(dr);
                foreach (DataRow dr in rowsToRemove)
                    dt.Rows.Remove(dr);
                //reorder
                dt.Columns["Destination"].SetOrdinal(0);
                dt.Columns["Code"].SetOrdinal(1);
                dt.Columns["Action"].SetOrdinal(2);

                //Add M/F
                dt.Columns.Add("M/F");
                if (mobileKeywords != null && mobileKeywords.Length > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["M/F"] = "F";
                        foreach (var mobileKeyword in mobileKeywords)
                            if (mobileKeyword.Trim() != "" && dr["Destination"].ToString().ToLower().Contains(mobileKeyword.Trim().ToLower()))
                            {
                                dr["M/F"] = "M";
                                break;
                            }
                    }
                }
            }
            else
            {
                //filter out columns
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.IsIn(new string[] { "Destination" }))
                        colsToRemove.Add(col);
                }
                foreach (DataColumn col in colsToRemove)
                {
                    dt.Columns.Remove(col);
                }
            }
            return TABS.Addons.Utilities.ExcelDataWriter.ExcelGenerator.GetExcelBytes(dt);
        }

    }
}
