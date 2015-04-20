using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Data.SQL
{
    public class CDRDataManager : BaseSQLDataManager, ICDRDataManager
    {
        public CDRDataManager()
            : base("CDRDBConnectionString")
        {

        }


        public void ApplyMainCDRsToDB(Object preparedMainCDRs)
        {
            List<BulkInsertInfo> listPreparedCDRs = (List<BulkInsertInfo>)preparedMainCDRs;

            Parallel.ForEach(listPreparedCDRs, item =>
            {
                InsertBulkToTable(item);
            });

        }

        private BulkInsertInfo PrepareCDRsForDBApply(List<CDR> cdrs)
        {
            string filePath = GetFilePathForBulkInsert();

            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (CDR cdr in cdrs)
                {
                    wr.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                                cdr.Call_Type,
                                cdr.Entity,
                                cdr.IMEI,
                                cdr.IMEI14,
                                cdr.Record_Type,
                                cdr.Source_File,
                                cdr.Source_Name,
                                cdr.Source_Type));
                }
                wr.Close();
                wr.Dispose();
            }

            return new BulkInsertInfo
            {
                TableName = "[dbo].[NormalCDR]",
                DataFilePath = filePath,
                TabLock = false,
                FieldSeparator = '^'
            };
        }

    }
}
