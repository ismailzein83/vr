using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CustomerQualityConfigurationDataManager : RoutingDataManager, ICustomerQualityConfigurationDataManager
    {
        string[] columns = { "QualityConfigurationId", "SupplierZoneId", "Quality", "VersionNumber" };

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(CustomerRouteQualityConfigurationData record, object dbApplyStream)
        {
            decimal qualityData = record.QualityData >= 0 ? decimal.Round(record.QualityData, 8) : 0;
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}", record.QualityConfigurationId, record.SupplierZoneId, qualityData, record.VersionNumber);
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

        public List<CustomerRouteQualityConfigurationData> GetCustomerRouteQualityConfigurationsDataAfterVersionNumber(int versionNumber)
        {
            string query = query_GetCustomerRouteQualityConfigurationData.Replace("#FILTER#", string.Format("WHERE VersionNumber > {0}", versionNumber));
            return GetItemsText(query, CustomerRouteQualityConfigurationDataMapper, null);
        }

        public void UpdateCustomerRouteQualityConfigurationsData(List<CustomerRouteQualityConfigurationData> customerRouteQualityConfigurationData)
        {
            DataTable dtCustomerQualityConfigurationData = BuildCustomerRouteQualityConfigurationsDataTable(customerRouteQualityConfigurationData);
            ExecuteNonQueryText(query_UpdateCustomerRouteQualityConfigurationData.ToString(), (cmd) =>
            {
                var dtPrm = new SqlParameter("@CustomerQualityConfigurationData", SqlDbType.Structured);
                dtPrm.TypeName = "CustomerQualityConfigurationDataType";
                dtPrm.Value = dtCustomerQualityConfigurationData;
                cmd.Parameters.Add(dtPrm);
            });
        }

        private DataTable BuildCustomerRouteQualityConfigurationsDataTable(List<CustomerRouteQualityConfigurationData> customerRouteQualityConfigurationData)
        {
            DataTable dtCustomerRouteQualityConfigurationsDataInfo = new DataTable();
            dtCustomerRouteQualityConfigurationsDataInfo.Columns.Add("QualityConfigurationId", typeof(Guid));
            dtCustomerRouteQualityConfigurationsDataInfo.Columns.Add("SupplierZoneId", typeof(Int64));
            dtCustomerRouteQualityConfigurationsDataInfo.Columns.Add("Quality", typeof(Decimal));
            dtCustomerRouteQualityConfigurationsDataInfo.Columns.Add("VersionNumber", typeof(Int32));
            dtCustomerRouteQualityConfigurationsDataInfo.BeginLoadData();

            foreach (var customerRouteQualityConfiguration in customerRouteQualityConfigurationData)
            {
                DataRow dr = dtCustomerRouteQualityConfigurationsDataInfo.NewRow();
                dr["QualityConfigurationId"] = customerRouteQualityConfiguration.QualityConfigurationId;
                dr["SupplierZoneId"] = customerRouteQualityConfiguration.SupplierZoneId;
                dr["Quality"] = customerRouteQualityConfiguration.QualityData;
                dr["VersionNumber"] = customerRouteQualityConfiguration.VersionNumber;

                dtCustomerRouteQualityConfigurationsDataInfo.Rows.Add(dr);
            }
            dtCustomerRouteQualityConfigurationsDataInfo.EndLoadData();
            return dtCustomerRouteQualityConfigurationsDataInfo;
        }

        #endregion

        #region Mappers

        CustomerRouteQualityConfigurationData CustomerRouteQualityConfigurationDataMapper(IDataReader reader)
        {
            return new CustomerRouteQualityConfigurationData()
            {
                QualityConfigurationId = GetReaderValue<Guid>(reader, "QualityConfigurationId"),
                SupplierZoneId = GetReaderValue<long>(reader, "SupplierZoneId"),
                QualityData = GetReaderValue<decimal>(reader, "Quality"),
                VersionNumber = (int)reader["VersionNumber"]
            };
        }

        #endregion

        #region Queries

        const string query_GetCustomerRouteQualityConfigurationData = @"                                                       
                                                SELECT  [QualityConfigurationId], [SupplierZoneId], [Quality], [VersionNumber]
                                                  FROM [dbo].[CustomerQualityConfigurationData] with(nolock)
                                                  #FILTER#";

        const string query_UpdateCustomerRouteQualityConfigurationData = @"
                                            Update  customerRouteQualityConfigurationData set 
                                                    customerRouteQualityConfigurationData.Quality = crqcd.Quality,
                                                    customerRouteQualityConfigurationData.VersionNumber = crqcd.VersionNumber
                                            FROM [dbo].[CustomerQualityConfigurationData] customerRouteQualityConfigurationData
                                            JOIN @CustomerQualityConfigurationData crqcd on crqcd.QualityConfigurationId = customerRouteQualityConfigurationData.QualityConfigurationId
                                                and crqcd.SupplierZoneId = customerRouteQualityConfigurationData.SupplierZoneId";

        #endregion
    }
}