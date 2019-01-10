﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace Vanrise.Security.Data.RDB
{
    public class EncryptionKeyDataManager : IEncryptionKeyDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_EncryptionKey";
        static string TABLE_ALIAS = "key";
        const string COL_ID = "ID";
        const string COL_EncryptionKey = "EncryptionKey";


        static EncryptionKeyDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_EncryptionKey, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "EncryptionKey",
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

        #region IEncryptionKeyDataManager
        public string InsertIfNotExistsAndGetEncryptionKey(string keyToInsertIfNotExists)
        {
            var selectQueryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = selectQueryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery.SelectColumns().Column(COL_EncryptionKey);
            var encryptionKey = selectQueryContext.ExecuteScalar().StringValue;
            if (encryptionKey == null)
            {
                var insertQueryContext = new RDBQueryContext(GetDataProvider());
                var insertQuery = insertQueryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.Column(COL_EncryptionKey).Value(keyToInsertIfNotExists);
                insertQueryContext.ExecuteNonQuery();
            }
            var selectQueryContext2 = new RDBQueryContext(GetDataProvider());
            var selectQuery2 = selectQueryContext2.AddSelectQuery();
            selectQuery2.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery2.SelectColumns().Column(COL_EncryptionKey);
            selectQuery2.Sort().ByColumn(COL_ID, RDBSortDirection.ASC);
            return selectQueryContext2.ExecuteScalar().StringValue;
        }
        #endregion
    }
}
