﻿using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class RPQualityConfigurationDataManager : RoutingDataManager, IRPQualityConfigurationDataManager
    {
        string[] columns = { "QualityConfigurationId", "SaleZoneId", "SupplierId", "Quality" };

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(RPQualityConfigurationData record, object dbApplyStream)
        {
            decimal qualityData = record.QualityData >= 0 ? decimal.Round(record.QualityData, 8) : 0;
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}", record.QualityConfigurationId, record.SaleZoneId, record.SupplierId, qualityData);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[RPQualityConfigurationData]",
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

        public IEnumerable<RPQualityConfigurationData> GetRPQualityConfigurationData()
        {
            string query = query_GetRPQualityConfigurationData.Replace("#FILTER#", string.Empty);
            return GetItemsText(query, RPQualityConfigurationDataMapper, null);
        }

        #endregion

        #region Mappers

        RPQualityConfigurationData RPQualityConfigurationDataMapper(IDataReader reader)
        {
            return new RPQualityConfigurationData()
            {
                QualityConfigurationId = GetReaderValue<Guid>(reader, "QualityConfigurationId"),
                SaleZoneId = GetReaderValue<long>(reader, "SaleZoneId"),
                SupplierId = GetReaderValue<int>(reader, "SupplierId"),
                QualityData = GetReaderValue<decimal>(reader, "Quality")
            };
        }

        #endregion

        #region Queries

        const string query_GetRPQualityConfigurationData = @"                                                       
                                                SELECT  [QualityConfigurationId], [SaleZoneId], [SupplierId], [Quality]
                                                  FROM [dbo].[RPQualityConfigurationData] with(nolock)
                                                  #FILTER#";

        #endregion
    }
}