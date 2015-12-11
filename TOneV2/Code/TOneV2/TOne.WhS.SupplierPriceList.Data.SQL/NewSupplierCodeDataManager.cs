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

        public void Insert(int priceListId, IEnumerable<Entities.SPL.NewCode> codesList)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (NewCode code in codesList)
            {
                WriteRecordToStream(priceListId, code, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }

        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        private void WriteRecordToStream(int priceListId, NewCode record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                       record.CodeId,
                       priceListId,
                       record.Code,
                       record.Zone.ZoneId,
                       record.CodeGroupId,
                       record.BED,
                       record.EED);
        }

        private object FinishDBApplyStream(object dbApplyStream)
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

        private void ApplyForDB(object preparedObject)
        {
            InsertBulkToTable(preparedObject as BaseBulkInsertInfo);
        }
    }
}
