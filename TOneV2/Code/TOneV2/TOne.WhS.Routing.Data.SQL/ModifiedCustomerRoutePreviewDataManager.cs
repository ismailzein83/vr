using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class ModifiedCustomerRoutePreviewDataManager : RoutingDataManager, IModifiedCustomerRoutePreviewDataManager
    {
        readonly string[] modifiedCustomerRoute_PreviewColumns = { "CustomerId", "Code", "SaleZoneId", "OrigIsBlocked", "IsBlocked", "OrigExecutedRuleId",
                                                                   "ExecutedRuleId","SupplierIds","OrigRouteOptions","RouteOptions", "IsApproved"};

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(ModifiedCustomerRoutePreviewData record, object dbApplyStream)
        {
            var streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord($"{record.CustomerId}^{record.Code}^{record.SaleZoneId}^{(record.OrigIsBlocked ? 1 : 0)}^{(record.IsBlocked ? 1 : 0)}^{record.OrigExecutedRuleId}^{record.ExecutedRuleId}^{record.SupplierIds}^{record.OrigRouteOptions}^{record.RouteOptions}^{(record.IsApproved ? 1 : 0)}");
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            var streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo()
            {
                TableName = "[dbo].[ModifiedCustomerRoute_Preview]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = modifiedCustomerRoute_PreviewColumns
            };
        }

        public void ApplyModifiedCustomerRoutesPreviewForDB(object preparedModifiedProductRoutePreview)
        {
            InsertBulkToTable(preparedModifiedProductRoutePreview as BaseBulkInsertInfo);
        }

        public void InitializeTable()
        {
            ExecuteScalarText(query_InitializeModifiedCustomerRoutePreviewTable, null);
        }

        public IEnumerable<Entities.ModifiedCustomerRoutesPreview> GetAllModifiedCustomerRoutesPreview(Vanrise.Entities.DataRetrievalInput<Entities.ModifiedCustomerRoutesPreviewQuery> input)
        {
            StringBuilder queryBuilder = new StringBuilder(query_GetAllModifiedCustomerRoutesPreview);

            IEnumerable<Entities.ModifiedCustomerRoutesPreview> modifiedCustomerRoutes = GetItemsText(queryBuilder.ToString(), ModifiedCustomerRoutesPreviewMapper, (cmd) => { });

            new CustomerRouteDataManager() { RoutingDatabase = RoutingDatabase }.CompleteSupplierData(modifiedCustomerRoutes);
            return modifiedCustomerRoutes;
        }

        #endregion

        #region Mappers

        private ModifiedCustomerRoutesPreview ModifiedCustomerRoutesPreviewMapper(IDataReader reader)
        {
            string saleZoneServiceIds = (reader["SaleZoneServiceIds"] as string);

            return new ModifiedCustomerRoutesPreview()
            {
                Id = (int)reader["Id"],
                CustomerId = (int)reader["CustomerID"],
                CustomerName = reader["CustomerName"] as string,
                Code = reader["Code"] as string,
                SaleZoneId = (long)reader["SaleZoneID"],
                SaleZoneName = reader["SaleZoneName"] as string,
                OrigIsBlocked = (bool)reader["OrigIsBlocked"],
                IsBlocked = (bool)reader["IsBlocked"],
                OrigExecutedRuleId = GetReaderValue<int?>(reader, "OrigExecutedRuleId"),
                ExecutedRuleId = GetReaderValue<int?>(reader, "ExecutedRuleId"),
                Rate = GetReaderValue<decimal?>(reader, "Rate"),
                SaleZoneServiceIds = !string.IsNullOrEmpty(saleZoneServiceIds) ? new HashSet<int>(saleZoneServiceIds.Split(',').Select(itm => int.Parse(itm))) : null,
                RouteOptions = reader["RouteOptions"] != DBNull.Value ? Helper.DeserializeOptions(reader["RouteOptions"] as string) : null,
                OrigRouteOptions = reader["OrigRouteOptions"] != DBNull.Value ? Helper.DeserializeOptions(reader["OrigRouteOptions"] as string) : null,
                SupplierIds = reader["SupplierIds"] as string
            };
        }

        #endregion

        #region Queries

        private string query_InitializeModifiedCustomerRoutePreviewTable = @"IF EXISTS(SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'[dbo].[ModifiedCustomerRoute_Preview]') AND s.type in (N'U')) 
                                                                                TRUNCATE TABLE [dbo].[ModifiedCustomerRoute_Preview] ;
			                                                        	    ELSE 
                                                                                CREATE TABLE [dbo].[ModifiedCustomerRoute_Preview](
                                                                                    [Id] [int] NOT NULL IDENTITY PRIMARY KEY,    
	                                                                                [CustomerId] [int] NOT NULL,
	                                                                                [Code] [varchar](20) NOT NULL,
	                                                                                [SaleZoneId] [bigint] NOT NULL,
	                                                                                [OrigIsBlocked] [bit] NOT NULL,
	                                                                                [IsBlocked] [bit] NOT NULL,
	                                                                                [OrigExecutedRuleId] [int] NULL,
	                                                                                [ExecutedRuleId] [int] NULL,
                                                                                    [SupplierIds] [varchar](max) NULL,
                                                                                    [OrigRouteOptions] [varchar](max) NULL,
	                                                                                [RouteOptions] [varchar](max) NULL,
	                                                                                [IsApproved] [bit] NOT NULL) ON [PRIMARY] ";

        private string query_GetAllModifiedCustomerRoutesPreview = @"SELECT 
                                                                    mcr.[Id]
                                                                   ,mcr.[CustomerId]
                                                                   ,ca.Name as CustomerName
                                                                   ,mcr.[Code]
                                                                   ,mcr.[SaleZoneId]
                                                                   ,sz.Name as SaleZoneName
                                                                   ,czd.EffectiveRateValue as Rate
                                                                   ,czd.SaleZoneServiceIds
                                                                   ,mcr.[OrigIsBlocked]
                                                                   ,mcr.[IsBlocked]
                                                                   ,mcr.[OrigExecutedRuleId]
                                                                   ,mcr.[ExecutedRuleId]
                                                                   ,mcr.[SupplierIds]
                                                                   ,mcr.[OrigRouteOptions]
                                                                   ,mcr.[RouteOptions]
                                                                   ,mcr.[IsApproved]
                                                                   FROM [dbo].[ModifiedCustomerRoute_Preview] mcr with(nolock)
                                                                   JOIN [dbo].[SaleZone] as sz ON mcr.SaleZoneId = sz.ID 
                                                                   JOIN [dbo].[CarrierAccount] as ca ON mcr.CustomerID = ca.ID
                                                                   JOIN [dbo].[CustomerZoneDetail] as czd ON czd.SaleZoneId = mcr.SaleZoneID and czd.CustomerId = mcr.CustomerID";

        #endregion
    }
}