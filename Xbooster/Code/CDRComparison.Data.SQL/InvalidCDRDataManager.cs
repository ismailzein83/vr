using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace CDRComparison.Data.SQL
{
    public class InvalidCDRDataManager : BaseCDRDataManager, IInvalidCDRDataManager
    {
        private static string[] s_Columns = new string[]
        {
            "OriginalCDPN",
            "OriginalCGPN",
            "CDPN",
            "CGPN",
            "Time",
            "DurationInSec",
            "IsPartnerCDR"
        };

        protected override string TableNamePrefix
        {
            get { return "InvalidCDR"; }
        }

        public void CreateInvalidCDRTempTable()
        {
            var query = new StringBuilder();
            query.Append
            (@"
                create table #TEMP_TABLE_NAME#
                (
                    [ID] [int] identity(1, 1) not null,
                    [OriginalCDPN] [varchar](100) null,
                    [OriginalCGPN] [varchar](100) null,
                    [CDPN] [varchar](100) null,
                    [CGPN] [varchar](100) null,
                    [Time] [datetime] null,
                    [DurationInSec] [decimal](20, 10) null,
                    [IsPartnerCDR] [bit] null
                )"
            );
            query.Replace("#TEMP_TABLE_NAME#", base.TableName);
            ExecuteNonQueryText(query.ToString(), null);
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        public void WriteRecordToStream(InvalidCDR record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                record.OriginalCDPN,
                record.OriginalCGPN,
                record.CDPN,
                record.CGPN,
                GetDateTimeForBCP(record.Time),
                record.DurationInSec,
                record.IsPartnerCDR ? "1" : "0"
            );
        }
        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = this.TableName,
                Stream = streamForBulkInsert,
                ColumnNames = s_Columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^'
            };
        }

        public void ApplyInvalidCDRsToDB(object preparedInvalidCDRs)
        {
            InsertBulkToTable(preparedInvalidCDRs as BaseBulkInsertInfo);
        }

        public IEnumerable<InvalidCDR> GetAllInvalidCDRs(bool isPartnerCDRs)
        {
            var query = new StringBuilder();
            query.Append
            (@"
                select [ID], [OriginalCDPN], [OriginalCGPN], [CDPN], [CGPN], [Time], [DurationInSec], [IsPartnerCDR]
                from #TABLE_NAME#
                where IsPartnerCDR = @IsPartnerCDRs
            ");
            query.Replace("#TABLE_NAME#", base.TableName);

            return GetItemsText(query.ToString(), InvalidCDRMapper, cmd =>
            {
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "@IsPartnerCDRs",
                    SqlDbType = SqlDbType.Bit,
                    Value = isPartnerCDRs
                });
            });
        }
        public int GetInvalidCDRsCount(bool isPartnerCDRs)
        {
            var query = new StringBuilder();
            query.AppendFormat(@"select count(*) from {0} where IsPartnerCDR = @IsPartnerCDRs", base.TableName);

            object count = ExecuteScalarText(query.ToString(), cmd =>
            {
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "@IsPartnerCDRs",
                    SqlDbType = SqlDbType.Bit,
                    Value = isPartnerCDRs
                });
            });

            return (count != DBNull.Value) ? (int)count : 0;
        }
        public decimal GetInvalidCDRsDuration(bool isPartnerCDRs)
        {
            var query = new StringBuilder();
            query.AppendFormat(@"select sum(DurationInSec) from {0} where IsPartnerCDR = @IsPartnerCDRs", base.TableName);

            object duration = ExecuteScalarText(query.ToString(), cmd =>
            {
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
                {
                    ParameterName = "@IsPartnerCDRs",
                    SqlDbType = SqlDbType.Bit,
                    Value = isPartnerCDRs
                });
            });

            return (duration != DBNull.Value) ? (decimal)duration : 0;
        }

        private InvalidCDR InvalidCDRMapper(IDataReader reader)
        {
            return new InvalidCDR()
            {
                OriginalCDPN = reader["OriginalCDPN"] as string,
                OriginalCGPN = reader["OriginalCGPN"] as string,
                CDPN = reader["CDPN"] as string,
                CGPN = reader["CGPN"] as string,
                Time = GetReaderValue<DateTime>(reader, "Time"),
                DurationInSec = GetReaderValue<decimal>(reader, "DurationInSec"),
                IsPartnerCDR = GetReaderValue<bool>(reader, "IsPartnerCDR")
            };
        }
    }
}
