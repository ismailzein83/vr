using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class NewSupplierZonesServicesDataManager : BaseSQLDataManager, INewSupplierZonesServicesDataManager
    {
        readonly string[] _columns = { "ID", "SupplierID", "ProcessInstanceID", "ZoneID", "ZoneServices", "BED", "EED","IsExcluded" };
        public NewSupplierZonesServicesDataManager()
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

        public void ApplyNewZonesServicesToDB(object preparedZonesServices)
        {
            InsertBulkToTable(preparedZonesServices as BaseBulkInsertInfo);
        }

        object Vanrise.Data.IBulkApplyDataManager<NewZoneService>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_BE.SPL_SupplierZoneService_New",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        object Vanrise.Data.IBulkApplyDataManager<NewZoneService>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(NewZoneService record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                       record.ZoneServiceId,
                       record.SupplierId,
                       _processInstanceID,
                       record.Zone.ZoneId,
                       Vanrise.Common.Serializer.Serialize(record.ZoneServices, true),
                       GetDateTimeForBCP(record.BED),
                       GetDateTimeForBCP(record.EED),
                        (record.IsExcluded) ? 1 : 0);
        }
    }
}
