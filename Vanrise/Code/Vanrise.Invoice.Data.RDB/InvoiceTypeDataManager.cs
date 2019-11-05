﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Invoice.Entities;
using Vanrise.Entities;
namespace Vanrise.Invoice.Data.RDB
{
    public class InvoiceTypeDataManager : IInvoiceTypeDataManager
    {
        static string TABLE_NAME = "VR_Invoice_InvoiceType";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_DevProjectID = "DevProjectID";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static InvoiceTypeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_DevProjectID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceType",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Invoice", "InvoiceDBConnStringKey", "InvoiceDBConnString");
        }

        public InvoiceType InvoiceTypeMapper(IRDBDataReader reader)
        {
            InvoiceType invoiceType = new InvoiceType
            {
                InvoiceTypeId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                DevProjectId = reader.GetNullableGuid(COL_DevProjectID),
                Settings = Vanrise.Common.Serializer.Deserialize<InvoiceTypeSettings>(reader.GetString(COL_Settings))
            };
            return invoiceType;
        }

        #endregion

        #region IInvoiceTypeDataManager

        public List<Entities.InvoiceType> GetInvoiceTypes()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "invType", null, true);
            selectQuery.SelectColumns().AllTableColumns("invType");
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);

            return queryContext.GetItems(InvoiceTypeMapper);
        }

        public bool AreInvoiceTypesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool InsertInvoiceType(Entities.InvoiceType invoiceType)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.IfNotExists("invType").EqualsCondition(COL_Name).Value(invoiceType.Name);

            insertQuery.Column(COL_ID).Value(invoiceType.InvoiceTypeId);
            insertQuery.Column(COL_Name).Value(invoiceType.Name);

            if (invoiceType.DevProjectId.HasValue)
                insertQuery.Column(COL_DevProjectID).Value(invoiceType.DevProjectId.Value);

            if (invoiceType.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(invoiceType.Settings));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateInvoiceType(Entities.InvoiceType invoiceType)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists("invType");
            notExistsCondition.NotEqualsCondition(COL_ID).Value(invoiceType.InvoiceTypeId);
            notExistsCondition.EqualsCondition(COL_Name).Value(invoiceType.Name);

            updateQuery.Column(COL_Name).Value(invoiceType.Name);

            if (invoiceType.DevProjectId.HasValue)
                updateQuery.Column(COL_DevProjectID).Value(invoiceType.DevProjectId.Value);
            else
                updateQuery.Column(COL_DevProjectID).Null();

            if (invoiceType.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(invoiceType.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            updateQuery.Where().EqualsCondition(COL_ID).Value(invoiceType.InvoiceTypeId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        

        #endregion
    }
}
