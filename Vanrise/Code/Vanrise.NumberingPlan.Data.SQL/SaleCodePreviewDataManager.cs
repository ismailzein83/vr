using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data.SQL
{
    public class SaleCodePreviewDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISaleCodePreviewDataManager
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
            : base(GetConnectionStringName("NumberingPlanDBConnStringKey", "NumberingPlanDBConnString"))
        {

        }

        public void ApplyPreviewCodesToDB(object preparedZones)
        {
            InsertBulkToTable(preparedZones as BaseBulkInsertInfo);
        }

        public IEnumerable<CodePreview> GetFilteredCodePreview(SPLPreviewQuery query)
        {
            return GetItemsSP("[VR_NumberingPlan].[sp_SaleCode_Preview_GetFiltered]", CodePreviewMapper, query.ProcessInstanceId, query.ZoneName, query.OnlyModified);
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
                record.BED,
                record.EED);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "VR_NumberingPlan.SaleCode_Preview",
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
