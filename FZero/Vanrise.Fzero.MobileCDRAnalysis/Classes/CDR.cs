using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class CDR
    {

        public static void Confirm(int SourceID, DataTable dt, int? ImportedBy)
        {

            List<int> listIDs = new List<int>();
            Import import = new Import();
            import.ImportDate = DateTime.Now;
            import.ImportedBy = ImportedBy;
            import.ImportTypeId = (int)Enums.ImportTypes.CDRs;
            Import.Save(import);





            int CounterGeneratesOnly = 1;
            List<CDR> listCDRs = new List<CDR>();
            foreach (DataRow i in dt.Rows)
            {
                CDR gc = new CDR();
               




                string MSISDN=string.Empty;
                if (i.Table.Columns.Contains("MSISDN"))
                {
                    MSISDN = i["MSISDN"].ToString();
                }
                gc.MSISDN = MSISDN;


                string IMSI = string.Empty;
                if (i.Table.Columns.Contains("IMSI"))
                {
                    IMSI = i["IMSI"].ToString();
                }
                gc.IMSI = IMSI;




                string Destination = string.Empty;
                if (i.Table.Columns.Contains("Destination"))
                {
                    Destination = i["Destination"].ToString();
                }
                gc.Destination = Destination;




               




                decimal? DurationInSeconds = null;
                decimal Duration = 0;

                if (i.Table.Columns.Contains("DurationInSeconds"))
                {
                    if (i["DurationInSeconds"].ToString() != string.Empty)
                    {
                        decimal.TryParse(i["DurationInSeconds"].ToString(), out Duration);
                        DurationInSeconds = Duration;
                    }
                }

                gc.DurationInSeconds =DurationInSeconds;


                string Reference = string.Empty;
                if (i.Table.Columns.Contains("Reference"))
                {
                    Reference = i["Reference"].ToString();
                }
                gc.Reference = Reference;


                DateTime Account_Age = new DateTime();
                if (i.Table.Columns.Contains("Account_Age"))
                {
                    DateTime.TryParse(i["Account_Age"].ToString(), out Account_Age);
                }
                gc.Account_Age = Account_Age;

                DateTime ConnectDateTime = new DateTime();
                if (i.Table.Columns.Contains("ConnectDateTime"))
                {
                    DateTime.TryParse(i["ConnectDateTime"].ToString(), out ConnectDateTime);
                }
                gc.ConnectDateTime = ConnectDateTime;

                DateTime DisconnectDateTime = new DateTime();
                if (i.Table.Columns.Contains("DisconnectDateTime"))
                {
                    DateTime.TryParse(i["DisconnectDateTime"].ToString(), out DisconnectDateTime);
                }
                gc.DisconnectDateTime = DisconnectDateTime;


                string Call_Class = string.Empty;
                if (i.Table.Columns.Contains("Call_Class"))
                {
                    Call_Class = i["Call_Class"].ToString();
                }
                gc.Call_Class = Call_Class;


                int? Call_Type = null;
                if (i.Table.Columns.Contains("Call_Type"))
                {
                    Call_Type = i["Call_Type"].ToString().ToInt();
                }
                gc.Call_Type = Call_Type;



                string Sub_Type = string.Empty;
                if (i.Table.Columns.Contains("Sub_Type"))
                {
                    Sub_Type = i["Sub_Type"].ToString();
                }
                gc.Sub_Type = Sub_Type;


                string IMEI = string.Empty;
                if (i.Table.Columns.Contains("IMEI"))
                {
                    IMEI = i["IMEI"].ToString();
                }
                gc.IMEI = IMEI;


                int? BTS_Id = null;
                if (i.Table.Columns.Contains("BTS_Id"))
                {
                    BTS_Id = i["BTS_Id"].ToString().ToInt();
                }
                gc.BTS_Id = BTS_Id;


                string LAC = string.Empty;
                if (i.Table.Columns.Contains("LAC"))
                {
                    LAC = i["LAC"].ToString();
                }
                gc.LAC = LAC;


                string Cell_Id = string.Empty;
                if (i.Table.Columns.Contains("Cell_Id"))
                {
                    Cell_Id = i["Cell_Id"].ToString();
                }
                gc.Cell_Id = Cell_Id;

                string Origin_Zone_Code = string.Empty;
                if (i.Table.Columns.Contains("Origin_Zone_Code"))
                {
                    Origin_Zone_Code = i["Origin_Zone_Code"].ToString();
                }
                gc.Origin_Zone_Code = Origin_Zone_Code;


                string Termin_Zone_Code = string.Empty;
                if (i.Table.Columns.Contains("Termin_Zone_Code"))
                {
                    Termin_Zone_Code = i["Termin_Zone_Code"].ToString();
                }
                gc.Termin_Zone_Code = Termin_Zone_Code;





                gc.SourceID = SourceID;
                gc.ImportID = import.ID;
                

               
                CounterGeneratesOnly++;

                if (ConnectDateTime != DateTime.Parse("1/1/0001 12:00:00 AM") && DisconnectDateTime != DateTime.Parse("1/1/0001 12:00:00 AM") && Account_Age != DateTime.Parse("1/1/0001 12:00:00 AM"))
                {
                    listCDRs.Add(gc);

                }


            }
            CDR.SaveBulk("CDR", listCDRs);
           
        }

        public static DataTable GetDataFromExcel(string filePath, int SwitchId)
        {
            string strConn;
            if (filePath.Substring(filePath.LastIndexOf('.')).ToLower() == ".xlsx")
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=0\"";
            else
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=0\"";

            DataSet ds = new DataSet();

            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                conn.Open();

                DataTable schemaTable = conn.GetOleDbSchemaTable(
                    OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                foreach (DataRow schemaRow in schemaTable.Rows)
                {
                    string sheet = schemaRow["TABLE_NAME"].ToString();

                    if (!sheet.EndsWith("_"))
                    {
                        try
                        {
                            OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet + "]", conn);
                            cmd.CommandType = CommandType.Text;

                            DataTable outputTable = new DataTable(sheet);
                            ds.Tables.Add(outputTable);
                            new OleDbDataAdapter(cmd).Fill(outputTable);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message + string.Format("Sheet:{0}.File:F{1}", sheet, filePath), ex);
                        }
                    }
                }
            }

            DataTable dtxls = ds.Tables[0];
            List<SourceMapping> listSourceMappingxls = SourceMapping.GetSwitchMappings(SwitchId);

            int colNumberxls = 0;

            foreach (DataColumn dc in dtxls.Columns)
            {
                SourceMapping sourceMapping = listSourceMappingxls.Where(x => x.ColumnName == dtxls.Columns[colNumberxls].ColumnName).FirstOrDefault();
                if (sourceMapping != null)
                {
                    dc.ColumnName = sourceMapping.PredefinedColumn.Name;
                }
                else
                {
                    dc.ColumnName = dc + " : UnMapped";
                }

                colNumberxls++;
            }
            return dtxls;

        }

        public static DataTable GetDataFromDat(string filePath, int SwitchId)
        {
            string strConn;
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties=\"text;HDR=NO;FMT=FixedLength\"";

            DataSet ds = new DataSet();

            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                conn.Open();

                DataTable schemaTable = conn.GetOleDbSchemaTable(
                    OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                foreach (DataRow schemaRow in schemaTable.Rows)
                {
                    string sheet = schemaRow["TABLE_NAME"].ToString();

                    if (!sheet.EndsWith("_"))
                    {
                        try
                        {
                            OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet + "]", conn);
                            cmd.CommandType = CommandType.Text;

                            DataTable outputTable = new DataTable(sheet);
                            ds.Tables.Add(outputTable);
                            new OleDbDataAdapter(cmd).Fill(outputTable);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message + string.Format("Sheet:{0}.File:F{1}", sheet, filePath), ex);
                        }
                    }
                }
            }

            DataTable dtxls = ds.Tables[0];
            List<SourceMapping> listSourceMappingxls = SourceMapping.GetSwitchMappings(SwitchId);

            int colNumberxls = 0;

            foreach (DataColumn dc in dtxls.Columns)
            {
                SourceMapping sourceMapping = listSourceMappingxls.Where(x => x.ColumnName == dtxls.Columns[colNumberxls].ColumnName).FirstOrDefault();
                if (sourceMapping != null)
                {
                    dc.ColumnName = sourceMapping.PredefinedColumn.Name;
                }
                else
                {
                    dc.ColumnName = dc + " : UnMapped";
                }

                colNumberxls++;
            }
            return dtxls;

        }

        public static DataTable GetDataFromXml(string filePath, int SwitchId)
        {

            try
            {
                DataSet ds = new DataSet("CDRs");
                ds.ReadXml(filePath);

                DataTable dtXml = ds.Tables[0];

                List<SourceMapping> listSourceMappingXml = SourceMapping.GetSwitchMappings(SwitchId);

                int colNumberXml = 0;

                foreach (DataColumn DC in dtXml.Columns)
                {
                    SourceMapping sourceMapping = listSourceMappingXml.Where(x => x.ColumnName == dtXml.Columns[colNumberXml].ColumnName).FirstOrDefault();

                    if (sourceMapping != null)
                    {
                        DC.ColumnName = sourceMapping.PredefinedColumn.Name;
                    }
                    else
                    {
                        DC.ColumnName = DC.ColumnName + " : UnMapped";
                    }

                    colNumberXml++;
                }


                return dtXml;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.CDRs.GetDataFromXml(" + filePath.ToString() + ")", err);
                return null;
            }

           
        }

        public static bool SaveBulk(string tableName, List<CDR> listCDRs)
        {
            bool success = false;
            try
            {
                Manager.InsertData(listCDRs.ToList(), tableName, "CDRAnalysisConnectionString");
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.CDR.SaveBulk(" + listCDRs.Count.ToString() + ")", err);
            }
            return success;
        }


 
    }
}
