using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.CDRAnalysis
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
                        float DurationInSeconds = 0;
                        if (i.Table.Columns.Contains("DurationInSeconds"))
                        {
                            if (i["DurationInSeconds"].ToString() != string.Empty)
                            {
                                float.TryParse(i["DurationInSeconds"].ToString(), out DurationInSeconds);
                            }
                        }

                        gc.DurationInSeconds = Convert.ToInt32(DurationInSeconds);




                        string IN_TRUNK = string.Empty;
                        if (i.Table.Columns.Contains("IN_TRUNK"))
                        {
                            IN_TRUNK = i["IN_TRUNK"].ToString();
                        }
                        gc.IN_TRUNK = IN_TRUNK;


                        string OUT_TRUNK = string.Empty;
                        if (i.Table.Columns.Contains("OUT_TRUNK"))
                        {
                            OUT_TRUNK = i["OUT_TRUNK"].ToString();
                        }
                        gc.OUT_TRUNK = OUT_TRUNK;


                        string CGPN = string.Empty;
                        if (i.Table.Columns.Contains("CGPN"))
                        {
                            CGPN = i["CGPN"].ToString();
                        }
                        gc.CGPN = CGPN;


                        string CDPN = string.Empty;
                        if (i.Table.Columns.Contains("CDPN"))
                        {
                            CDPN = i["CDPN"].ToString();
                        }
                        gc.CDPN = CDPN;




                        string Reference = string.Empty;
                        if (i.Table.Columns.Contains("Reference"))
                        {
                            Reference = i["Reference"].ToString();
                        }
                        gc.Reference = Reference;






                        DateTime ConnectDateTime = new DateTime();
                        if (i.Table.Columns.Contains("ConnectDateTime"))
                        {
                            if (i["ConnectDateTime"].ToString() != string.Empty)
                            {
                                DateTime.TryParse(i["ConnectDateTime"].ToString(), out ConnectDateTime);
                            }
                        }
                        gc.ConnectDateTime = ConnectDateTime;



                        DateTime DisconnectDateTime = new DateTime();
                        if (i.Table.Columns.Contains("DisconnectDateTime"))
                        {
                            if (i["DisconnectDateTime"].ToString() != string.Empty)
                            {
                                DateTime.TryParse(i["DisconnectDateTime"].ToString(), out DisconnectDateTime);
                            }
                        }
                        gc.DisconnectDateTime = DisconnectDateTime;




                        gc.SourceID = SourceID;
                        CounterGeneratesOnly++;

                        if (ConnectDateTime != DateTime.Parse("1/1/0001 12:00:00 AM") && DisconnectDateTime != DateTime.Parse("1/1/0001 12:00:00 AM") )
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
