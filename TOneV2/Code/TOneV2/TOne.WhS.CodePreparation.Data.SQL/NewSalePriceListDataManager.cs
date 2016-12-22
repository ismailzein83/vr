using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Data;
using Vanrise.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class NewSalePriceListDataManager : BaseTOneDataManager, INewSalePriceListDataManager
    {
        public NewSalePriceListDataManager()
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

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_BE.CP_SalePriceList_New",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(PriceListToAdd record, object dbApplyStream)
        {

            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}",
                                record.PriceListId,
                               this._processInstanceID,
                               (int)record.OwnerType,
                               record.OwnerId,
                               record.CurrencyId,
                              GetDateTimeForBCP(record.EffectiveOn));
        }

        public void ApplySalePriceListsToDB(object preparedSalePriceLists)
        {
            InsertBulkToTable(preparedSalePriceLists as BaseBulkInsertInfo);
        }

        public void SaveSalePriceListsToDB(IEnumerable<PriceListToAdd> salePriceLists)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (PriceListToAdd salePriceList in salePriceLists)
                WriteRecordToStream(salePriceList, dbApplyStream);

            Object preparedSalePriceLists = FinishDBApplyStream(dbApplyStream);
            ApplySalePriceListsToDB(preparedSalePriceLists);
        }

    }
}
