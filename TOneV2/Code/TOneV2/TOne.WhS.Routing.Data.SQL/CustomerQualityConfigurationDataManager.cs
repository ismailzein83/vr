using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CustomerQualityConfigurationDataManager : RoutingDataManager, ICustomerQualityConfigurationDataManager
    {
        string[] columns = { "QualityConfigurationId", "SupplierZoneId", "Quality" };

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(CustomerRouteQualityConfigurationData record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}", record.QualityConfigurationId, record.SupplierZoneId, decimal.Round(record.QualityData, 8));
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CustomerQualityConfigurationData]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        public void ApplyQualityConfigurationsToDB(object preparedQualityConfigurations)
        {
            InsertBulkToTable(preparedQualityConfigurations as BaseBulkInsertInfo);
        }

        public IEnumerable<CustomerRouteQualityConfigurationData> GetCustomerRouteQualityConfigurationsData()
        {
            string query = query_GetCustomerRouteQualityConfigurationData.Replace("#FILTER#", string.Empty);
            return GetItemsText(query, CustomerRouteQualityConfigurationDataMapper, null);
        }

        #endregion

        #region Mappers

        CustomerRouteQualityConfigurationData CustomerRouteQualityConfigurationDataMapper(IDataReader reader)
        {
            return new CustomerRouteQualityConfigurationData()
            {
                QualityConfigurationId = GetReaderValue<Guid>(reader, "QualityConfigurationId"),
                SupplierZoneId = GetReaderValue<long>(reader, "SupplierZoneId"),
                QualityData = GetReaderValue<decimal>(reader, "Quality")
            };
        }

        #endregion

        #region Queries

        const string query_GetCustomerRouteQualityConfigurationData = @"                                                       
                                                SELECT  [QualityConfigurationId], [SupplierZoneId], [Quality]
                                                  FROM [dbo].[CustomerQualityConfigurationData] with(nolock)
                                                  #FILTER#";

        #endregion
    }
}