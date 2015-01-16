using System;
using System.Text;
using System.Data;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Data Stats Monitor", "Monitors attempts, calls and minutes in the Billing Stats Cost and Sale according to CDR and Traffic Stats")]
    public class DataStatsMonitor : RunnableBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(DataStatsMonitor));

        protected void GetDataStats(DateTime date)
        {
            DataTable data;
            string tmpSql;
            StringBuilder resultNote = new StringBuilder("");
            double percentage = (double)TABS.SystemParameter.Daily_Stats_Monitor_PercentageDiff.NumericValue;
            tmpSql = @"EXEC bp_GetStatisticalDailySummary @date=@P1";
            data = TABS.DataHelper.GetDataTable(tmpSql, date.Date);
            if (data.Rows.Count > 0)
            {
                // Make sure that CDR and Traffic Stats Match 
                var cdrRow = data.Rows[0];
                var tsRow = data.Rows[1];
                resultNote.Append("CDR and Traffic Stats:<br/>");
                foreach (DataColumn column in data.Columns)
                {
                    // Compare values for non-name columns
                    if (column.ColumnName != "Source")
                    {
                        if (cdrRow[column.ColumnName] != DBNull.Value && tsRow[column.ColumnName] != DBNull.Value)
                        {
                            double cdrVal = double.Parse(cdrRow[column.ColumnName].ToString());
                            double tsVal = double.Parse(tsRow[column.ColumnName].ToString());
                            if (cdrVal != tsVal)
                                resultNote.AppendFormat(" Difference ({0}%) in number of {1} between the CDR and the Traffic Stats <br/> ", ((double)((cdrVal - tsVal) / cdrVal) * 100).ToString("#0.00"), column.ColumnName);
                            else resultNote.AppendFormat("Same Result in {0}.<br/>", column.ColumnName);
                        }
                        else
                        {
                            resultNote.AppendFormat(" A null value in {0} stop the comparision between the CDR and the Traffic Stats <br/> ", column.ColumnName);
                        }
                    }
                }

                var bsCostRow = data.Rows[2];
                resultNote.Append("CDR and Billing Stats Cost:<br/>");
                foreach (DataColumn column in data.Columns)
                {
                    // Compare values for non-name columns
                    if (column.ColumnName != "Source" && column.ColumnName != "Attempts")
                    {
                        if (cdrRow[column.ColumnName] != DBNull.Value && bsCostRow[column.ColumnName] != DBNull.Value)
                        {
                            double cdrVal = double.Parse(cdrRow[column.ColumnName].ToString());
                            double costVal = double.Parse(bsCostRow[column.ColumnName].ToString());
                            if (double.Parse(cdrRow[column.ColumnName].ToString()) != double.Parse(bsCostRow[column.ColumnName].ToString()))

                                if ((costVal < (cdrVal - (cdrVal * percentage / 100))) || (costVal > (cdrVal + cdrVal * percentage / 100)))
                                    resultNote.AppendFormat(" Difference ({0}%) in number of {1} between the CDR and the Billing Stats Cost <br/> ", ((double)((cdrVal - costVal) / cdrVal) * 100).ToString("#0.00"), column.ColumnName);
                                else resultNote.AppendFormat("succefully result in {0}, difference({1}%).<br/>", column.ColumnName, ((double)((cdrVal - costVal) / cdrVal) * 100).ToString("#0.00"));
                        }
                        else
                        {
                            resultNote.AppendFormat(" A null value in {0} stop the comparision between the CDR and the Billing Stats Cost<br/> ", column.ColumnName);
                        }
                    }
                }

                var bsSaleRow = data.Rows[3];
                resultNote.Append("CDR and Billing Stats Sale:<br/>");
                foreach (DataColumn column in data.Columns)
                {
                    // Compare values for non-name columns
                    if (column.ColumnName != "Source" && column.ColumnName != "Attempts")
                    {
                        if (cdrRow[column.ColumnName] != DBNull.Value && bsCostRow[column.ColumnName] != DBNull.Value)
                        {
                            double cdrVal = double.Parse(cdrRow[column.ColumnName].ToString());
                            double saleVal = double.Parse(bsCostRow[column.ColumnName].ToString());
                            if ((saleVal < (cdrVal - (cdrVal * percentage / 100))) || (saleVal > (cdrVal + (cdrVal * percentage / 100))))
                                resultNote.AppendFormat(" Difference ({0}%) in number of {1} between the CDR and the Billing Stats Sale <br/> ", ((double)((cdrVal - saleVal) / cdrVal) * 100).ToString("#0.00"), column.ColumnName);
                            else resultNote.AppendFormat("succefully result in {0}, difference({1}%).<br/>", column.ColumnName, ((double)((cdrVal - saleVal) / cdrVal) * 100).ToString("#0.00"));
                        }
                        else
                        {
                            resultNote.AppendFormat(" A null value in {0} stop the comparision between the CDR and the Billing Stats Sale<br/> ", column.ColumnName);
                        }
                    }
                }
            }
            else
            {
                // No data at all? Not possible
            }
        }

        protected string Compare(DataRow CDRRow, DataTable table, double percentage)
        {
            StringBuilder sb = new StringBuilder("");

            CDRRow = table.Rows[0];

            foreach (DataRow row in table.Rows)
            {
                if (row["Name"].ToString() != "CDR")
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        if (column.ColumnName != "Source")
                        {
                            if (CDRRow[column.ColumnName] == DBNull.Value || row[column.ColumnName] == DBNull.Value)
                                sb.AppendFormat("No Compare in {0} Since {1} {0} is NULL <br/> ", (CDRRow[column.ColumnName] == null) ? "CDR" : row["Source"].ToString());
                            else
                            {
                                double CDRAttempt = double.Parse(CDRRow[column.ColumnName].ToString());
                                double rowAttempt = double.Parse(row[column.ColumnName].ToString());
                                if ((rowAttempt < (CDRAttempt - CDRAttempt * percentage)) || (rowAttempt > (CDRAttempt + CDRAttempt * percentage)))
                                    sb.AppendFormat(" Differnce in {0} between CDR({1}) and {2}({3}) <br/> ", column.ColumnName, CDRAttempt, row["Source"].ToString(), rowAttempt);
                            }
                        }
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Run the DataStatsMonitor
        /// </summary>
        public override void Run()
        {
            this.IsRunning = true;
            this.IsLastRunSuccessful = true;

            try
            {
                GetDataStats(DateTime.Today.AddMonths(-2));
                //GetDataStats(DateTime.Today.AddDays(-1));
            }
            catch (Exception ex)
            {
                log.Error("Error Monitoring Data Stats", ex);
                this.IsLastRunSuccessful = false;
                this.Exception = ex;
            }

            this.IsRunning = false;
        }

        /// <summary>
        /// Request a stop for the operation
        /// </summary>
        /// <returns></returns>
        public override bool Stop()
        {
            bool result = false;
            if (this.IsRunning)
            {
                result = base.Stop();
            }
            return result;
        }

        public override string Status { get { return string.Empty; } }
    }
}
