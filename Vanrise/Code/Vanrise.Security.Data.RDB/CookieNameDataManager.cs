﻿using System;
using System.Collections.Generic;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace Vanrise.Security.Data.RDB
{
    public class CookieNameDataManager : ICookieNameDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_CookieName";
        static string TABLE_ALIAS = "cookie";
        const string COL_ID = "ID";
        const string COL_CookieName = "CookieName";


        static CookieNameDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CookieName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "CookieName",
                Columns = columns,
                IdColumnName = COL_ID
            });
        }

        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Sec", "SecurityDBConnStringKey", "SecurityDBConnString");
        }
        #endregion

        #region ICookieNameDataManager
        public string InsertIfNotExistsAndGetCookieName(string nameToInsertIfNotExists)
        {
            var insertQueryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = insertQueryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.IfNotExists(TABLE_ALIAS);
            insertQuery.Column(COL_CookieName).Value(nameToInsertIfNotExists);
            if (insertQueryContext.ExecuteNonQuery() > 0)
                return nameToInsertIfNotExists;
            var selectQueryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = selectQueryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery.SelectColumns().Column(COL_CookieName);
            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.ASC);
            return selectQueryContext.ExecuteScalar().StringValue;
        }
        #endregion
    }
}
