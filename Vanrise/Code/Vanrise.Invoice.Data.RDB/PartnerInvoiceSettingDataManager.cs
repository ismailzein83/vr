using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Invoice.Entities;
using Vanrise.Entities;
namespace Vanrise.Invoice.Data.RDB
{
    public class PartnerInvoiceSettingDataManager : IPartnerInvoiceSettingDataManager
    {
        static string TABLE_NAME = "VR_Invoice_PartnerInvoiceSetting";
        const string COL_ID = "ID";
        const string COL_PartnerID = "PartnerID";
        const string COL_InvoiceSettingID = "InvoiceSettingID";
        const string COL_Details = "Details";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static PartnerInvoiceSettingDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_PartnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_InvoiceSettingID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Details, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "PartnerInvoiceSetting",
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

        public PartnerInvoiceSetting PartnerInvoiceSettingMapper(IRDBDataReader reader)
        {
            PartnerInvoiceSetting partnerInvoiceSetting = new PartnerInvoiceSetting
            {
                PartnerInvoiceSettingId = reader.GetGuid(COL_ID),
                PartnerId = reader.GetString(COL_PartnerID),
                InvoiceSettingID = reader.GetGuid(COL_InvoiceSettingID),
                Details = Vanrise.Common.Serializer.Deserialize<PartnerInvoiceSettingDetails>(reader.GetString(COL_Details))
            };
            return partnerInvoiceSetting;
        }

        #endregion

        #region IPartnerInvoiceSettingDataManager

        public List<PartnerInvoiceSetting> GetPartnerInvoiceSettings()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "partnerSett", null, true);
            selectQuery.SelectColumns().AllTableColumns("partnerSett");

            return queryContext.GetItems(PartnerInvoiceSettingMapper);
        }

        public bool ArePartnerInvoiceSettingsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool InsertPartnerInvoiceSetting(Guid invoicePartnerSettingId, Guid invoiceSettingId, string partnerId, Entities.PartnerInvoiceSettingDetails partnerInvoiceSettingDetails)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var notExistsCondition = insertQuery.IfNotExists("partnerSett");
            notExistsCondition.EqualsCondition(COL_InvoiceSettingID).Value(invoiceSettingId);
            notExistsCondition.EqualsCondition(COL_PartnerID).Value(partnerId);

            insertQuery.Column(COL_ID).Value(invoicePartnerSettingId);
            insertQuery.Column(COL_InvoiceSettingID).Value(invoiceSettingId);
            insertQuery.Column(COL_PartnerID).Value(partnerId);
            if (partnerInvoiceSettingDetails != null)
                insertQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(partnerInvoiceSettingDetails));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdatePartnerInvoiceSetting(Guid partnerInvoiceSettingId, Entities.PartnerInvoiceSettingDetails partnerInvoiceSettingDetails)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            if (partnerInvoiceSettingDetails != null)
                updateQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(partnerInvoiceSettingDetails));
            else
                updateQuery.Column(COL_Details).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(partnerInvoiceSettingId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool DeletePartnerInvoiceSetting(Guid partnerInvoiceSettingId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ID).Value(partnerInvoiceSettingId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool InsertOrUpdateInvoiceSetting(Guid partnerInvoiceSettingId, string partnerId, Guid invoiceSettingId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_PartnerID).Value(partnerId);
            updateQuery.Column(COL_InvoiceSettingID).Value(invoiceSettingId);
            updateQuery.Where().EqualsCondition(COL_ID).Value(partnerInvoiceSettingId);

            if(queryContext.ExecuteNonQuery() <=0)
            {
                queryContext = new RDBQueryContext(GetDataProvider());
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);

                insertQuery.Column(COL_ID).Value(partnerInvoiceSettingId);
                insertQuery.Column(COL_InvoiceSettingID).Value(invoiceSettingId);
                insertQuery.Column(COL_PartnerID).Value(partnerId);

                queryContext.ExecuteNonQuery();
            }
            return true;
        }

        public PartnerInvoiceSetting GetPartnerInvoiceSetting(Guid partnerInvoiceSettingId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "partnerSett");
            selectQuery.SelectColumns().AllTableColumns("partnerSett");
            selectQuery.Where().EqualsCondition(COL_ID).Value(partnerInvoiceSettingId);
            return queryContext.GetItem(PartnerInvoiceSettingMapper);
        }

        #endregion
    }
}
