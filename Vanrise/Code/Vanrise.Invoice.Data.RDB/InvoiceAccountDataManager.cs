using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.RDB
{
    public class InvoiceAccountDataManager : IInvoiceAccountDataManager
    {
        static string TABLE_NAME = "VR_Invoice_InvoiceAccount";

        const string COL_ID = "ID";
        const string COL_InvoiceTypeID = "InvoiceTypeID";
        const string COL_PartnerID = "PartnerID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_Status = "Status";
        const string COL_IsDeleted = "IsDeleted";
        const string COL_CreatedTime = "CreatedTime";

        static InvoiceAccountDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_InvoiceTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_PartnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_Status, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_IsDeleted, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceAccount",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Invoice", "InvoiceDBConnStringKey", "InvoiceDBConnString");
        }

        VRInvoiceAccount VRInvoiceAccountMapper(IRDBDataReader reader)
        {
            return new VRInvoiceAccount
            {
                Status = (VRAccountStatus)reader.GetInt(COL_Status),
                BED = reader.GetNullableDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                InvoiceAccountId = reader.GetLong(COL_ID),
                InvoiceTypeId = reader.GetGuid(COL_InvoiceTypeID),
                IsDeleted = reader.GetBooleanWithNullHandling(COL_IsDeleted),
                PartnerId = reader.GetString(COL_PartnerID)
            };
        }

        #endregion

        #region IInvoiceAccountDataManager

        public bool InsertInvoiceAccount(Entities.VRInvoiceAccount invoiceAccount, out long insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var notExistsCondition = insertQuery.IfNotExists("invAcc");
            notExistsCondition.EqualsCondition(COL_InvoiceTypeID).Value(invoiceAccount.InvoiceTypeId);
            notExistsCondition.EqualsCondition(COL_PartnerID).Value(invoiceAccount.PartnerId);

            insertQuery.Column(COL_InvoiceTypeID).Value(invoiceAccount.InvoiceTypeId);
            insertQuery.Column(COL_PartnerID).Value(invoiceAccount.PartnerId);
            if (invoiceAccount.BED.HasValue)
                insertQuery.Column(COL_BED).Value(invoiceAccount.BED.Value);
            if (invoiceAccount.EED.HasValue)
                insertQuery.Column(COL_EED).Value(invoiceAccount.EED.Value);
            insertQuery.Column(COL_Status).Value((int)invoiceAccount.Status);
            insertQuery.Column(COL_IsDeleted).Value(invoiceAccount.IsDeleted);

            insertQuery.AddSelectGeneratedId();

            long? insertedIdObj = queryContext.ExecuteScalar().NullableLongValue;
            if(insertedIdObj.HasValue)
            {
                insertedId = insertedIdObj.Value;
                return true;
            }
            else
            {
                insertedId = -1;
                return false;
            }
        }

        public bool TryUpdateInvoiceAccountStatus(Guid invoiceTypeId, string partnerId, VRAccountStatus status, bool isDeleted)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_Status).Value((int)status);
            updateQuery.Column(COL_IsDeleted).Value(isDeleted);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            where.EqualsCondition(COL_PartnerID).Value(partnerId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool TryUpdateInvoiceAccountEffectiveDate(Guid invoiceTypeId, string partnerId, DateTime? bed, DateTime? eed)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            if (bed.HasValue)
                updateQuery.Column(COL_BED).Value(bed.Value);
            else
                updateQuery.Column(COL_BED).Null();
            if (bed.HasValue)
                updateQuery.Column(COL_EED).Value(eed.Value);
            else
                updateQuery.Column(COL_EED).Null();

            var where = updateQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            where.EqualsCondition(COL_PartnerID).Value(partnerId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public List<Entities.VRInvoiceAccount> GetInvoiceAccountsByPartnerIds(Guid invoiceTypeId, IEnumerable<string> partnerIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "invAcc");
            selectQuery.SelectColumns().AllTableColumns("invAcc");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            if (partnerIds != null && partnerIds.Count() > 0)
                where.ListCondition(COL_PartnerID, RDBListConditionOperator.IN, partnerIds);

            return queryContext.GetItems(VRInvoiceAccountMapper);
        }

        #endregion

        internal void AddJoinInvoiceToInvoiceAccount(RDBJoinContext joinContext, string invoiceTableAlias, string invoiceAccountTableAlias, VRAccountStatus? accountStatus, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            var joinCondition = joinContext.Join(TABLE_NAME, invoiceAccountTableAlias).On();
            joinCondition.EqualsCondition(invoiceTableAlias, InvoiceDataManager.COL_InvoiceTypeID).Column(invoiceAccountTableAlias, COL_InvoiceTypeID);
            joinCondition.EqualsCondition(invoiceTableAlias, InvoiceDataManager.COL_PartnerID).Column(invoiceAccountTableAlias, COL_PartnerID);

            joinCondition.ConditionIfColumnNotNull(invoiceAccountTableAlias, COL_IsDeleted).EqualsCondition(invoiceAccountTableAlias, COL_IsDeleted).Value(false);

            if (accountStatus.HasValue)
                joinCondition.EqualsCondition(invoiceAccountTableAlias, COL_Status).Value((int)accountStatus.Value);

            if (effectiveDate.HasValue)
            {
                joinCondition.ConditionIfColumnNotNull(invoiceAccountTableAlias, COL_BED).LessOrEqualCondition(invoiceAccountTableAlias, COL_BED).Value(effectiveDate.Value);
                joinCondition.ConditionIfColumnNotNull(invoiceAccountTableAlias, COL_EED).GreaterThanCondition(invoiceAccountTableAlias, COL_EED).Value(effectiveDate.Value);
            }

            if (isEffectiveInFuture.HasValue)
            {
                if (isEffectiveInFuture.Value)
                {
                    joinCondition.ConditionIfColumnNotNull(invoiceAccountTableAlias, COL_EED).GreaterOrEqualCondition(invoiceAccountTableAlias, COL_EED).DateNow();
                }
                else
                {
                    var andCondition = joinCondition.ChildConditionGroup();
                    andCondition.NotNullCondition(invoiceAccountTableAlias, COL_EED);
                    andCondition.LessThanCondition(invoiceAccountTableAlias, COL_EED).DateNow();
                }
            }
        }
    }
}
