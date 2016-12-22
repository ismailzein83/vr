using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class SaleCodePreviewDataManager : BaseTOneDataManager, ISaleCodePreviewDataManager
    {
        readonly string[] _columns = { "ProcessInstanceID", "Code", "ChangeType", "RecentZoneName", "ZoneName", "BED", "EED" };

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;

        public SaleCodePreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public void ApplyPreviewCodesToDB(object preparedZones)
        {
            InsertBulkToTable(preparedZones as BaseBulkInsertInfo);
        }

        public IEnumerable<CodePreview> GetFilteredCodePreview(SPLPreviewQuery query)
        {
            return GetItemsSP("[TOneWhS_CP].[sp_SaleCode_Preview_GetFiltered]", CodePreviewMapper, query.ProcessInstanceId, query.ZoneName, query.OnlyModified);
        }
 

        object Vanrise.Data.IBulkApplyDataManager<CodePreview>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(CodePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                _processInstanceID,
                record.Code,
                (int)record.ChangeType,
                record.RecentZoneName,
                record.ZoneName,
               GetDateTimeForBCP(record.BED),
               GetDateTimeForBCP(record.EED));
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_CP.SaleCode_Preview",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

 
        private CodePreview CodePreviewMapper(IDataReader reader)
        {
            CodePreview codePreview = new CodePreview
            {
                Code =  reader["Code"] as string,
                ChangeType = (CodeChangeType)GetReaderValue<int>(reader, "ChangeType"),
                RecentZoneName = reader["RecentZoneName"] as string,
                ZoneName = reader["ZoneName"] as string,
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
            return codePreview;
        }





        
    }
}
