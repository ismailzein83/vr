using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class NewSupplierCodeDataManager : BaseSQLDataManager, INewSupplierCodeDataManager
    {
        public NewSupplierCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;

        public void ApplyNewCodesToDB(object preparedCodes)
        {
            InsertBulkToTable(preparedCodes as BaseBulkInsertInfo);
        }

        object Vanrise.Data.IBulkApplyDataManager<NewCode>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_BE.SPL_SupplierCode_New",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        object Vanrise.Data.IBulkApplyDataManager<NewCode>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(NewCode record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                       record.CodeId,
                       _processInstanceID,
                       record.Code,
                       record.Zone.ZoneId,
                       record.CodeGroupId,
                       GetDateTimeForBCP(record.BED),
                       GetDateTimeForBCP(record.EED),
                        (record.IsExcluded) ? 1 : 0);
        }
    }
}
