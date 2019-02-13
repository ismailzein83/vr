﻿using System;
using System.Collections.Generic;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.SMSBusinessEntity.Data.RDB
{
    public class CustomerSMSPriceListDataManager : ICustomerSMSPriceListDataManager
    {
        static string TABLE_NAME = "TOneWhS_SMSBE_SalePriceList";
        static string TABLE_ALIAS = "SMSSalePriceList";
        const string COL_ID = "ID";
        internal const string COL_CustomerID = "CustomerID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_EffectiveOn = "EffectiveOn";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_UserID = "UserID";

        static CustomerSMSPriceListDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CustomerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_EffectiveOn, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_UserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_SMSBE",
                DBTableName = "SalePriceList",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_SMSBuisenessEntity", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        public void JoinRateTableWithPriceListTable(RDBJoinContext joinContext, string salePriceListTableAlias, string otherTableAlias, string otherTableColumn)
        {
            joinContext.JoinOnEqualOtherTableColumn(TABLE_NAME, salePriceListTableAlias, COL_ID, otherTableAlias, otherTableColumn);
        }

        public void AddInsertPriceListQueryContext(RDBQueryContext queryContext, CustomerSMSPriceList customerSMSPriceList)
        {
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            
            insertQuery.Column(COL_ID).Value(customerSMSPriceList.ID);
            insertQuery.Column(COL_CustomerID).Value(customerSMSPriceList.CustomerID);
            insertQuery.Column(COL_CurrencyID).Value(customerSMSPriceList.CurrencyID);
            insertQuery.Column(COL_EffectiveOn).Value(customerSMSPriceList.EffectiveOn);
            insertQuery.Column(COL_ProcessInstanceID).Value(customerSMSPriceList.ProcessInstanceID);
            insertQuery.Column(COL_UserID).Value(customerSMSPriceList.UserID);
        }
    }
}
