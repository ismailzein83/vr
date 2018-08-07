using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.RDB
{
    public class PartnerInvoiceSettingDataManager : IPartnerInvoiceSettingDataManager
    {
        static string TABLE_NAME = "VR_Invoice_PartnerInvoiceSetting";

        static PartnerInvoiceSettingDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("PartnerID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add("InvoiceSettingID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("Details", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "PartnerInvoiceSetting",
                Columns = columns,
                IdColumnName = "ID",
                CreatedTimeColumnName = "CreatedTime"
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
                PartnerInvoiceSettingId = reader.GetGuid("ID"),
                PartnerId = reader.GetString("PartnerID"),
                InvoiceSettingID = reader.GetGuid("InvoiceSettingID"),
                Details = Vanrise.Common.Serializer.Deserialize<PartnerInvoiceSettingDetails>(reader.GetString("Details"))
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
            throw new NotImplementedException();
        }

        public bool InsertPartnerInvoiceSetting(Guid invoicePartnerSettingId, Guid invoiceSettingId, string partnerId, Entities.PartnerInvoiceSettingDetails partnerInvoiceSettingDetails)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var notExistsCondition = insertQuery.IfNotExists("partnerSett");
            notExistsCondition.EqualsCondition("InvoiceSettingID").Value(invoiceSettingId);
            notExistsCondition.EqualsCondition("PartnerID").Value(partnerId);

            insertQuery.Column("ID").Value(invoicePartnerSettingId);
            insertQuery.Column("invoiceSettingId").Value(invoiceSettingId);
            insertQuery.Column("PartnerID").Value(partnerId);
            if (partnerInvoiceSettingDetails != null)
                insertQuery.Column("Details").Value(Vanrise.Common.Serializer.Serialize(partnerInvoiceSettingDetails));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdatePartnerInvoiceSetting(Guid partnerInvoiceSettingId, Entities.PartnerInvoiceSettingDetails partnerInvoiceSettingDetails)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column("Details").Value(Vanrise.Common.Serializer.Serialize(partnerInvoiceSettingDetails));
            updateQuery.Where().EqualsCondition("ID").Value(partnerInvoiceSettingId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool DeletePartnerInvoiceSetting(Guid partnerInvoiceSettingId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition("ID").Value(partnerInvoiceSettingId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool InsertOrUpdateInvoiceSetting(Guid partnerInvoiceSettingId, string partnerId, Guid invoiceSettingId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column("PartnerID").Value(partnerId);
            updateQuery.Column("InvoiceSettingID").Value(invoiceSettingId);
            updateQuery.Where().EqualsCondition("ID").Value(partnerInvoiceSettingId);

            if(queryContext.ExecuteNonQuery() <=0)
            {
                queryContext = new RDBQueryContext(GetDataProvider());
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);

                insertQuery.Column("ID").Value(partnerInvoiceSettingId);
                insertQuery.Column("invoiceSettingId").Value(invoiceSettingId);
                insertQuery.Column("PartnerID").Value(partnerId);

                queryContext.ExecuteNonQuery();
            }
            return true;
        }

        #endregion
    }
}
