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

        static InvoiceAccountDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("InvoiceTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("PartnerID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add("BED", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("EED", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("Status", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add("IsDeleted", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceAccount",
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

        VRInvoiceAccount VRInvoiceAccountMapper(IRDBDataReader reader)
        {
            return new VRInvoiceAccount
            {
                Status = (VRAccountStatus)reader.GetInt("Status"),
                BED = reader.GetNullableDateTime("BED"),
                EED = reader.GetNullableDateTime("EED"),
                InvoiceAccountId = reader.GetLong("ID"),
                InvoiceTypeId = reader.GetGuid("InvoiceTypeId"),
                IsDeleted = reader.GetBooleanWithNullHandling("IsDeleted"),
                PartnerId = reader.GetString("PartnerId")
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
            notExistsCondition.EqualsCondition("InvoiceTypeID").Value(invoiceAccount.InvoiceTypeId);
            notExistsCondition.EqualsCondition("PartnerID").Value(invoiceAccount.PartnerId);

            insertQuery.Column("InvoiceTypeID").Value(invoiceAccount.InvoiceTypeId);
            insertQuery.Column("PartnerID").Value(invoiceAccount.PartnerId);
            if (invoiceAccount.BED.HasValue)
                insertQuery.Column("BED").Value(invoiceAccount.BED.Value);
            if (invoiceAccount.EED.HasValue)
                insertQuery.Column("EED").Value(invoiceAccount.EED.Value);
            insertQuery.Column("Status").Value((int)invoiceAccount.Status);
            insertQuery.Column("IsDeleted").Value(invoiceAccount.IsDeleted);

            insertQuery.GenerateIdAndAssignToParameter("Id");

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

            updateQuery.Column("Status").Value((int)status);
            updateQuery.Column("IsDeleted").Value(isDeleted);

            var where = updateQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.EqualsCondition("PartnerID").Value(partnerId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool TryUpdateInvoiceAccountEffectiveDate(Guid invoiceTypeId, string partnerId, DateTime? bed, DateTime? eed)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            if (bed.HasValue)
                updateQuery.Column("BED").Value(bed.Value);
            else
                updateQuery.Column("BED").Null();
            if (bed.HasValue)
                updateQuery.Column("EED").Value(eed.Value);
            else
                updateQuery.Column("EED").Null();

            var where = updateQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.EqualsCondition("PartnerID").Value(partnerId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public List<Entities.VRInvoiceAccount> GetInvoiceAccountsByPartnerIds(Guid invoiceTypeId, IEnumerable<string> partnerIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "invAcc");
            selectQuery.SelectColumns().AllTableColumns("invAcc");

            var where = selectQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.ListCondition("PartnerID", RDBListConditionOperator.IN, partnerIds);

            return queryContext.GetItems(VRInvoiceAccountMapper);
        }

        #endregion

        internal void AddJoinInvoiceToInvoiceAccount(RDBJoinContext joinContext, string invoiceTableAlias, string invoiceAccountTableAlias, VRAccountStatus? accountStatus, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            var joinCondition = joinContext.Join(TABLE_NAME, invoiceAccountTableAlias).On();
            joinCondition.EqualsCondition(invoiceTableAlias, "InvoiceTypeID").Column(invoiceAccountTableAlias, "InvoiceTypeID");
            joinCondition.EqualsCondition(invoiceTableAlias, "PartnerID").Column(invoiceAccountTableAlias, "PartnerID");

            joinCondition.ConditionIfColumnNotNull(invoiceAccountTableAlias, "IsDeleted").EqualsCondition(invoiceAccountTableAlias, "IsDeleted").Value(false);

            if (accountStatus.HasValue)
                joinCondition.EqualsCondition(invoiceAccountTableAlias, "Status").Value((int)accountStatus.Value);

            if (effectiveDate.HasValue)
            {
                joinCondition.ConditionIfColumnNotNull(invoiceAccountTableAlias, "BED").LessOrEqualCondition(invoiceAccountTableAlias, "BED").Value(effectiveDate.Value);
                joinCondition.ConditionIfColumnNotNull(invoiceAccountTableAlias, "EED").GreaterThanCondition(invoiceAccountTableAlias, "EED").Value(effectiveDate.Value);
            }

            if (isEffectiveInFuture.HasValue)
            {
                if (isEffectiveInFuture.Value)
                {
                    joinCondition.ConditionIfColumnNotNull(invoiceAccountTableAlias, "EED").GreaterOrEqualCondition(invoiceAccountTableAlias, "EED").DateNow();
                }
                else
                {
                    var andCondition = joinCondition.ChildConditionGroup();
                    andCondition.NotNullCondition(invoiceAccountTableAlias, "EED");
                    andCondition.LessThanCondition(invoiceAccountTableAlias, "EED").DateNow();
                }
            }
        }
    }
}
