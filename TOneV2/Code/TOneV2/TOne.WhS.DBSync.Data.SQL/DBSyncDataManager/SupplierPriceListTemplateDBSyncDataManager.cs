using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SupplierPriceListTemplateDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SupplierPriceListTemplate);
        string _Schema = "TOneWhS_SPL";
        bool _UseTempTables;
        public SupplierPriceListTemplateDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void  ClearSupplierPriceListTemplateTable()
        {
            ExecuteNonQueryText("delete from [TOneWhS_SPL].[SupplierPriceListTemplate]", null);
        }
       
        public string GetConnection()
        {
            return base.GetConnectionString();
        }

        public string GetTableName()
        {
            return _TableName;
        }

        public string GetSchema()
        {
            return _Schema;
        }
    }
}
