using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class SupplierCodePreviewDataManager : BaseSQLDataManager, ISupplierCodePreviewDataManager
    {
        readonly string[] _columns = { "PriceListId", "Code", "ChangeType", "RecentZoneName", "ZoneName", "BED", "EED" };

        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        static SupplierCodePreviewDataManager()
        {
            _columnMapper.Add("ChangeTypeDecription", "ChangeType");
        }

        public SupplierCodePreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_SPL_DBConnStringKey", "TOneWhS_SPL_DBConnString"))
        {

        }

        public void Insert(int priceListId, IEnumerable<Entities.CodePreview> codePreviewList)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (CodePreview code in codePreviewList)
            {
                WriteRecordToStream(priceListId, code, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }

        public Vanrise.Entities.BigResult<Entities.CodePreview> GetCodePreviewFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SPLPreviewQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {

                ExecuteNonQuerySP("[TOneWhS_SPL].[sp_SupplierCode_Preview_CreateTempByFiltered]", tempTableName, input.Query.PriceListId);
            };

            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, createTempTableAction, CodePreviewMapper, _columnMapper);
        }

        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        private void WriteRecordToStream(int priceListId, CodePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                priceListId,
                record.Code,
                (int)record.ChangeType,
                record.RecentZoneName,
                record.ZoneName,
                record.BED,
                record.EED);
        }

        private object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_SPL.SupplierCode_Preview",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        private void ApplyForDB(object preparedObject)
        {
            InsertBulkToTable(preparedObject as BaseBulkInsertInfo);
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
