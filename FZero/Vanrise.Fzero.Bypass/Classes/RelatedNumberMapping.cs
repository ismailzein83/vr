using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class RelatedNumberMapping
    {
        public static List<RelatedNumberMapping> GetRelatedNumberMappings(int MobileOperatorID)
           
        {
            List<RelatedNumberMapping> RelatedNumberMappingsList = new List<RelatedNumberMapping>();

            try
            {
                using (Entities context = new Entities())
                {
                    RelatedNumberMappingsList = context.RelatedNumberMappings.Where(x => x.MobileOperatorID == MobileOperatorID).Include(u => u.PredefinedColumnsforRelatedNumber)
                                            .OrderByDescending(u => u.ID)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.RelatedNumberMapping.GetRelatedNumberMappings()", err);
            }

            return RelatedNumberMappingsList;
        }

        public static RelatedNumberMapping Load(int ID)
        {
            RelatedNumberMapping RelatedNumberMapping = new RelatedNumberMapping();
            try
            {
                using (Entities context = new Entities())
                {
                    RelatedNumberMapping = context.RelatedNumberMappings
                     .Where(u => u.ID == ID)
                     .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.RelatedNumberMapping.Load(" + ID.ToString() + ")", err);
            }
            return RelatedNumberMapping;
        }

        public static bool Delete(int ID)
        {

            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    RelatedNumberMapping RelatedNumberMapping = RelatedNumberMapping.Load(ID);
                    context.Entry(RelatedNumberMapping).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.RelatedNumberMapping.Delete(" + ID.ToString() + ")", err);
            }
            return success;




        }

        public static RelatedNumberMapping Save(RelatedNumberMapping RelatedNumberMapping)
        {
            RelatedNumberMapping CurrentRelatedNumberMapping = new RelatedNumberMapping();
            try
            {
                using (Entities context = new Entities())
                {
                    if (RelatedNumberMapping.ID == 0)
                    {
                        context.RelatedNumberMappings.Add(RelatedNumberMapping);
                    }
                    else
                    {
                        context.RelatedNumberMappings.Attach(RelatedNumberMapping);
                        context.Entry(RelatedNumberMapping).State = System.Data.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.RelatedNumberMapping.Save(" + RelatedNumberMapping.ID.ToString() + ")", err);
            }
            return CurrentRelatedNumberMapping;
        }

        public static bool CheckIfExists(RelatedNumberMapping RelatedNumberMapping)
        {
            try
            {
                using (Entities context = new Entities())
                {
                    int Count;
                    if (RelatedNumberMapping.MobileOperatorID == 0)
                    {
                        Count = context.RelatedNumberMappings
                           .Where(u => u.MobileOperatorID == RelatedNumberMapping.MobileOperatorID && (u.ColumnName == RelatedNumberMapping.ColumnName 
                               || u.MappedtoColumnNumber == RelatedNumberMapping.MappedtoColumnNumber))
                           .Count();
                        if (Count == 0)
                            return false;
                        else
                            return true;
                    }
                    else
                    {
                        Count = context.RelatedNumberMappings
                           .Where(u => u.MobileOperatorID == RelatedNumberMapping.MobileOperatorID && u.ID != RelatedNumberMapping.ID && (u.ColumnName == RelatedNumberMapping.ColumnName 
                               || u.MappedtoColumnNumber == RelatedNumberMapping.MappedtoColumnNumber))
                           .Count();
                        if (Count == 0)
                            return false;
                        else
                            return true;
                    }

                  
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.RelatedNumberMapping.CheckIfExists(" + RelatedNumberMapping.MobileOperatorID.ToString() + ", " + RelatedNumberMapping.ColumnName.ToString() + ", " + RelatedNumberMapping.MappedtoColumnNumber.ToString() + ")", err);
            }
            return true;
        }

        public static DataTable GetDataFromExcel(string filePath, int MobileOperatorId)
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
            List<RelatedNumberMapping> listRelatedNumberMappingxls = RelatedNumberMapping.GetRelatedNumberMappings(MobileOperatorId);

            int colNumberxls = 0;

            foreach (DataColumn dc in dtxls.Columns)
            {
                RelatedNumberMapping relatedNumberMapping = listRelatedNumberMappingxls.Where(x => x.ColumnName == dtxls.Columns[colNumberxls].ColumnName).FirstOrDefault();
                if (relatedNumberMapping != null)
                {
                    dc.ColumnName = relatedNumberMapping.PredefinedColumnsforRelatedNumber.Name;
                }
                else
                {
                    dc.ColumnName = dc + " : UnMapped";
                }

                colNumberxls++;
            }
            return dtxls;

        }

        public static DataTable GetDataFromXml(string filePath, int MobileOperatorId)
        {

            try
            {
                DataSet ds = new DataSet("GeneratedCalls");
                ds.ReadXml(filePath);

                DataTable dtXml = ds.Tables[0];

                List<RelatedNumberMapping> listRelatedNumberMappingxls = RelatedNumberMapping.GetRelatedNumberMappings(MobileOperatorId);

                int colNumberXml = 0;

                foreach (DataColumn DC in dtXml.Columns)
                {
                    RelatedNumberMapping relatedNumberMapping = listRelatedNumberMappingxls.Where(x => x.ColumnName == dtXml.Columns[colNumberXml].ColumnName).FirstOrDefault();

                    if (relatedNumberMapping != null)
                    {
                        DC.ColumnName = relatedNumberMapping.PredefinedColumnsforRelatedNumber.Name;
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
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCalls.GetDataFromXml(" + filePath.ToString() + ")", err);
                return null;
            }


        }

        public static bool SaveBulk(string tableName, List<RelatedNumber> listRelatedNumbers)
        {
            bool success = false;
            try
            {
                Manager.InsertData(listRelatedNumbers.ToList(), tableName, "FMSConnectionString");
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.RelatedNumber.SaveBulk(" + listRelatedNumbers.Count.ToString() + ")", err);
            }
            return success;
        }

        public static void Confirm(DataTable dt, int? ImportedBy, int ReportID)
        {
                    List<RelatedNumber> listRelatedNumbers = new List<RelatedNumber>();
                    foreach (DataRow i in dt.Rows)
                    {
                        RelatedNumber rn = new RelatedNumber();
                        string RelatedNumber1 = string.Empty;
                        if (i.Table.Columns.Contains("Related Number"))
                        {
                            RelatedNumber1 = i["Related Number"].ToString();
                        }
                       


                        // Add Area Code to be Able to Know the Mobile Operator Prefix
                        string NumberWithoutAreaCodeCLI = RelatedNumber1;

                        if (RelatedNumber1.StartsWith("+"))
                        {
                            NumberWithoutAreaCodeCLI = RelatedNumber1.Substring(1);
                        }

                        if (RelatedNumber1.StartsWith("00"))
                        {
                            NumberWithoutAreaCodeCLI = RelatedNumber1.Substring(2);
                        }

                        if (RelatedNumber1.StartsWith("0"))
                        {
                            NumberWithoutAreaCodeCLI = RelatedNumber1.Substring(1);
                        }

                        if (RelatedNumber1.StartsWith("964"))
                        {
                            NumberWithoutAreaCodeCLI = RelatedNumber1.Substring("964".Count());
                        }

                        if (RelatedNumber1.StartsWith("+" + "964"))
                        {
                            NumberWithoutAreaCodeCLI = RelatedNumber1.Substring("964".Count() + 1);
                        }

                        if (RelatedNumber1.StartsWith("00" + "964"))
                        {
                            NumberWithoutAreaCodeCLI = RelatedNumber1.Substring("964".Count() + 2);
                        }


                        if (RelatedNumber1.StartsWith("240"))
                        {
                            NumberWithoutAreaCodeCLI = RelatedNumber1.Substring("240".Count());
                        }

                        if (RelatedNumber1.StartsWith("+" + "240"))
                        {
                            NumberWithoutAreaCodeCLI = RelatedNumber1.Substring("240".Count() + 1);
                        }

                        if (RelatedNumber1.StartsWith("00" + "240"))
                        {
                            NumberWithoutAreaCodeCLI = RelatedNumber1.Substring("240".Count() + 2);
                        }


                        if (RelatedNumber1.StartsWith("963"))
                        {
                            NumberWithoutAreaCodeCLI = RelatedNumber1.Substring("963".Count());
                        }

                        if (RelatedNumber1.StartsWith("+" + "963"))
                        {
                            NumberWithoutAreaCodeCLI = RelatedNumber1.Substring("963".Count() + 1);
                        }

                        if (RelatedNumber1.StartsWith("00" + "963"))
                        {
                            NumberWithoutAreaCodeCLI = RelatedNumber1.Substring("963".Count() + 2);
                        }

                        rn.RelatedNumber1 = "0" + RelatedNumber1;
                        rn.ReportID = ReportID;
                        rn.RegisteredOn = DateTime.Now;
                        listRelatedNumbers.Add(rn);
                    }
                    SaveBulk("RelatedNumbers", listRelatedNumbers);

        }

    }
}
