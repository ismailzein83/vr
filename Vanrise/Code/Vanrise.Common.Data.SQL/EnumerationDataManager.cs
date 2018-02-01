using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class EnumerationDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IEnumerationDataManager
    {
        public EnumerationDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        private readonly string[] _enumerationsColumns =
        {
            "ID", "NameSpace", "Name", "Values"
        };


        public void ClearEnumerations()
        {
            ExecuteNonQuerySP("common.sp_Enumerations_Delete");
        }

        public void SaveEnumerationsToDb(IEnumerable<Enumeration> enumerations)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (Enumeration enumeration in enumerations)
                WriteRecordToStream(enumeration, dbApplyStream);
            Object preparedEnumerations = FinishDBApplyStream(dbApplyStream, "[common].[Enumerations]");
            ApplyEnumerationsToDB(preparedEnumerations);
        }

        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        private void WriteRecordToStream(Enumeration record, object dbApplyStream)
        {
            Vanrise.Data.SQL.StreamForBulkInsert streamForBulkInsert = dbApplyStream as Vanrise.Data.SQL.StreamForBulkInsert;
            if (streamForBulkInsert != null)
                streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}",
                    record.ID,
                    record.NameSpace,
                    record.Name,
                    record.Values);
        }

        private object FinishDBApplyStream(object dbApplyStream, string tableName)
        {
            Vanrise.Data.SQL.StreamForBulkInsert streamForBulkInsert = dbApplyStream as Vanrise.Data.SQL.StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new Vanrise.Data.SQL.StreamBulkInsertInfo
            {
                TableName = tableName,
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = _enumerationsColumns
            };
        }

        private void ApplyEnumerationsToDB(object preparedEnumerations)
        {
            InsertBulkToTable(preparedEnumerations as Vanrise.Data.SQL.BaseBulkInsertInfo);
        }
    }
}
