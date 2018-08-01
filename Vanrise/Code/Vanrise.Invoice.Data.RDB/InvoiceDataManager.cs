using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.AccountBalance.Data.RDB;
using Vanrise.GenericData.Entities;

namespace Vanrise.Invoice.Data.RDB
{
    public class InvoiceDataManager : IInvoiceDataManager
    {
        public static string TABLE_NAME = "VR_Invoice_Invoice";

        static InvoiceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("UserId", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add("InvoiceTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("PartnerID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add("SettlementInvoiceId", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("SplitInvoiceGroupId", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("InvoiceSettingID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("SerialNumber", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add("FromDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("ToDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("IssueDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("DueDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("Details", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("PaidDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("LockDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("SentDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("IsDeleted", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("Notes", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("Settings", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("SourceId", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add("IsDraft", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("IsAutomatic", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("NeedApproval", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("ApprovedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("ApprovedBy", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "Invoice",
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

        string GetColumnNameFromFieldName(string fieldName)
        {
            switch (fieldName)
            {
                case "Partner": return "PartnerID";
                case "User": return "UserId";
                default: return fieldName;
            }
        }

        public Entities.Invoice InvoiceMapper(IRDBDataReader reader)
        {
            Entities.Invoice invoice = new Entities.Invoice
            {
                Details = Vanrise.Common.Serializer.Deserialize(reader.GetString("Details")),
                FromDate = reader.GetDateTimeWithNullHandling("FromDate"),
                InvoiceId = reader.GetLong("ID"),
                InvoiceTypeId = reader.GetGuid("InvoiceTypeId"),
                IssueDate = reader.GetDateTimeWithNullHandling("IssueDate"),
                PartnerId = reader.GetString("PartnerId"),
                SerialNumber = reader.GetString("SerialNumber"),
                ToDate = reader.GetDateTimeWithNullHandling("ToDate"),
                PaidDate = reader.GetNullableDateTime("PaidDate"),
                DueDate = reader.GetDateTimeWithNullHandling("DueDate"),
                UserId = reader.GetIntWithNullHandling("UserId"),
                CreatedTime = reader.GetDateTimeWithNullHandling("CreatedTime"),
                LockDate = reader.GetNullableDateTime("LockDate"),
                Note = reader.GetString("Notes"),
                SourceId = reader.GetString("SourceID"),
                IsAutomatic = reader.GetBooleanWithNullHandling("IsAutomatic"),
                Settings = Vanrise.Common.Serializer.Deserialize<InvoiceSettings>(reader.GetString("Settings")),
                InvoiceSettingId = reader.GetGuidWithNullHandling("InvoiceSettingId"),
                SentDate = reader.GetNullableDateTime("SentDate"),
                SettlementInvoiceId = reader.GetNullableLong("SettlementInvoiceId"),
                SplitInvoiceGroupId = reader.GetNullableGuid("SplitInvoiceGroupId"),
                ApprovedBy = reader.GetNullableInt("ApprovedBy"),
                ApprovedTime = reader.GetNullableDateTime("ApprovedTime"),
                NeedApproval = reader.GetNullableBoolean("NeedApproval"),
            };
            return invoice;
        }

        public InvoiceByPartnerInfo InvoiceByPartnerInfoMapper(IRDBDataReader reader)
        {
            return new InvoiceByPartnerInfo
            {
                InvoiceTypeId = reader.GetGuidWithNullHandling("InvoiceTypeId"),
                IssueDate = reader.GetDateTimeWithNullHandling("IssueDate"),
                PartnerId = reader.GetString("PartnerId"),
                ToDate = reader.GetDateTimeWithNullHandling("ToDate"),
                DueDate = reader.GetDateTimeWithNullHandling("DueDate"),
            };
        }
        public VRPopulatedPeriod VRPopulatedPeriodMapper(IRDBDataReader reader)
        {
            return new VRPopulatedPeriod
            {
                FromDate = reader.GetNullableDateTime("FromDate"),
                ToDate = reader.GetNullableDateTime("ToDate"),
            };
        }

        #endregion

        #region IInvoiceDataManager

        public List<Entities.Invoice> GetPartnerInvoicesByDate(Guid invoiceTypeId, IEnumerable<string> partnerIds, DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entities.Invoice> GetFilteredInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceQuery> input)
        {
            throw new NotImplementedException();
        }

        public int GetInvoiceCount(Guid InvoiceTypeId, string partnerId, DateTime? fromDate, DateTime? toDate)
        {
            throw new NotImplementedException();
        }

        public bool SaveInvoices(List<GenerateInvoiceInputToSave> generateInvoicesInputToSave, out List<long> insertedInvoiceIds)
        {
            BillingTransactionDataManager billingTransactionDataManager = new BillingTransactionDataManager();
            insertedInvoiceIds = null;
            if (generateInvoicesInputToSave != null)
            {
                var dataProvider = GetDataProvider();
                insertedInvoiceIds = new List<long>();

                Guid? splitInvoiceGroupId = Guid.NewGuid();

                for (var i = 0; i < generateInvoicesInputToSave.Count; i++)
                {
                    var generateInvoiceInputToSave = generateInvoicesInputToSave[i];

                    if (generateInvoiceInputToSave.InvoiceIdToDelete.HasValue && generateInvoiceInputToSave.SplitInvoiceGroupId.HasValue)
                    {
                        splitInvoiceGroupId = generateInvoiceInputToSave.SplitInvoiceGroupId.Value;
                    }

                    string serializedSettings = null;
                    if (generateInvoiceInputToSave.Invoice.Settings != null)
                    {
                        serializedSettings = Vanrise.Common.Serializer.Serialize(generateInvoiceInputToSave.Invoice.Settings);
                    }
                    string serializedDetails = Vanrise.Common.Serializer.Serialize(generateInvoiceInputToSave.Invoice.Details);
                    var queryContext = new RDBQueryContext(dataProvider);
                    var insertQuery = queryContext.AddInsertQuery();
                    insertQuery.IntoTable(TABLE_NAME);
                    insertQuery.GenerateIdAndAssignToParameter("ID");
                    insertQuery.Column("UserId").Value(generateInvoiceInputToSave.Invoice.UserId);
                    insertQuery.Column("PartnerID").Value(generateInvoiceInputToSave.Invoice.PartnerId);
                    if (splitInvoiceGroupId.HasValue)
                        insertQuery.Column("SplitInvoiceGroupId").Value(splitInvoiceGroupId.Value);
                    insertQuery.Column("InvoiceSettingID").Value(generateInvoiceInputToSave.Invoice.InvoiceSettingId);
                    insertQuery.Column("SerialNumber").Value(generateInvoiceInputToSave.Invoice.SerialNumber);
                    insertQuery.Column("FromDate").Value(generateInvoiceInputToSave.Invoice.FromDate);
                    insertQuery.Column("ToDate").Value(generateInvoiceInputToSave.Invoice.ToDate);
                    insertQuery.Column("IssueDate").Value(generateInvoiceInputToSave.Invoice.IssueDate);
                    insertQuery.Column("DueDate").Value(generateInvoiceInputToSave.Invoice.DueDate);
                    insertQuery.Column("Details").Value(serializedDetails);
                    insertQuery.Column("Notes").Value(generateInvoiceInputToSave.Invoice.Note);
                    insertQuery.Column("SourceId").Value(generateInvoiceInputToSave.Invoice.SourceId);
                    insertQuery.Column("IsDraft").Value(true);
                    insertQuery.Column("IsAutomatic").Value(generateInvoiceInputToSave.Invoice.IsAutomatic);
                    insertQuery.Column("Settings").Value(serializedSettings);
                    if (generateInvoiceInputToSave.Invoice.SentDate.HasValue)
                        insertQuery.Column("SentDate").Value(generateInvoiceInputToSave.Invoice.SentDate.Value);
                    if (generateInvoiceInputToSave.Invoice.NeedApproval.HasValue)
                        insertQuery.Column("NeedApproval").Value(generateInvoiceInputToSave.Invoice.NeedApproval.Value);
                    long insertedInvoiceId = queryContext.ExecuteScalar().LongValue;

                    insertedInvoiceIds.Add(insertedInvoiceId);

                    if (generateInvoiceInputToSave.ItemSetNameStorageDic != null && generateInvoiceInputToSave.ItemSetNameStorageDic.Count > 0)
                    {
                        var remainingInvoiceItemSets = generateInvoiceInputToSave.InvoiceItemSets.FindAllRecords(x => !generateInvoiceInputToSave.ItemSetNameStorageDic.Values.Any(y => y.Contains(x.SetName)));
                        if (remainingInvoiceItemSets != null)
                        {
                            InvoiceItemDataManager dataManager = new InvoiceItemDataManager();
                            dataManager.SaveInvoiceItems(insertedInvoiceId, remainingInvoiceItemSets);
                        }
                        foreach (var item in generateInvoiceInputToSave.ItemSetNameStorageDic)
                        {
                            InvoiceItemDataManager dataManager = new InvoiceItemDataManager();
                            dataManager.StorageConnectionStringKey = item.Key;
                            var invoiceItemSetsToSave = generateInvoiceInputToSave.InvoiceItemSets.FindAllRecords(x => item.Value.Contains(x.SetName));
                            dataManager.SaveInvoiceItems(insertedInvoiceId, invoiceItemSetsToSave);
                        }
                    }
                    else
                    {
                        InvoiceItemDataManager dataManager = new InvoiceItemDataManager();
                        dataManager.SaveInvoiceItems(insertedInvoiceId, generateInvoiceInputToSave.InvoiceItemSets);
                    }
                    generateInvoiceInputToSave.Invoice.InvoiceId = insertedInvoiceId;

                    if (generateInvoiceInputToSave.ActionBeforeGenerateInvoice != null)
                    {
                        generateInvoiceInputToSave.ActionBeforeGenerateInvoice(generateInvoiceInputToSave.Invoice);
                    }

                    if (generateInvoiceInputToSave.ActionAfterGenerateInvoice != null)
                    {
                        generateInvoiceInputToSave.ActionAfterGenerateInvoice(generateInvoiceInputToSave.Invoice);
                    }
                }

                var queryContext2 = new RDBQueryContext(dataProvider);

                foreach (var generateInvoiceInputToSave in generateInvoicesInputToSave)
                {
                    if (generateInvoiceInputToSave.InvoiceToSettleIds != null && generateInvoiceInputToSave.InvoiceToSettleIds.Count > 0)
                    {
                        var updateQuery = queryContext2.AddUpdateQuery();
                        updateQuery.FromTable(TABLE_NAME);
                        updateQuery.Column("SettlementInvoiceId").Value(generateInvoiceInputToSave.Invoice.InvoiceId);
                        updateQuery.Where().ListCondition("ID", RDBListConditionOperator.IN, generateInvoiceInputToSave.InvoiceToSettleIds);

                        if (generateInvoiceInputToSave.InvoiceIdToDelete.HasValue)
                        {
                            var updateQuery2 = queryContext2.AddUpdateQuery();
                            updateQuery2.FromTable(TABLE_NAME);
                            updateQuery2.Column("SettlementInvoiceId").Null();
                            updateQuery2.Where().EqualsCondition("SettlementInvoiceId").Value(generateInvoiceInputToSave.InvoiceIdToDelete.Value);
                        }
                    }

                    if (generateInvoiceInputToSave.MappedTransactions != null && generateInvoiceInputToSave.MappedTransactions.Count() > 0)
                    {
                        foreach (var billingTransaction in generateInvoiceInputToSave.MappedTransactions)
                        {
                            billingTransactionDataManager.AddQueryInsertBillingTransaction(queryContext2, billingTransaction, generateInvoiceInputToSave.Invoice.InvoiceId, false);
                        }
                    }

                    if (generateInvoiceInputToSave.InvoiceIdToDelete.HasValue)
                    {
                        var updateQuery = queryContext2.AddUpdateQuery();
                        updateQuery.FromTable(TABLE_NAME);
                        updateQuery.Column("IsDeleted").Value(true);
                        updateQuery.Where().EqualsCondition("ID").Value(generateInvoiceInputToSave.InvoiceIdToDelete.Value);

                        billingTransactionDataManager.AddQuerySetInvoiceBillingTransactionsDeleted(queryContext2, generateInvoiceInputToSave.InvoiceIdToDelete.Value);
                    }

                    var updateInvoiceQuery = queryContext2.AddUpdateQuery();
                    updateInvoiceQuery.FromTable(TABLE_NAME);
                    updateInvoiceQuery.Column("IsDraft").Value(false);
                    updateInvoiceQuery.Where().EqualsCondition("ID").Value(generateInvoiceInputToSave.Invoice.InvoiceId);
                }

                queryContext2.ExecuteNonQuery(true);
            }
            return true;
        }

        public List<Entities.Invoice> GetInvoices(List<long> invoiceIds)
        {
            throw new NotImplementedException();
        }

        public bool CheckInvoiceOverlaping(Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate, long? invoiceId)
        {
            throw new NotImplementedException();
        }

        public bool SetInvoicePaid(long invoiceId, DateTime? paidDate)
        {
            throw new NotImplementedException();
        }

        public bool SetInvoiceLocked(long invoiceId, DateTime? lockedDate)
        {
            throw new NotImplementedException();
        }

        public bool UpdateInvoiceNote(long invoiceId, string invoiceNote)
        {
            throw new NotImplementedException();
        }

        public void LoadInvoicesAfterImportedId(Guid invoiceTypeId, long lastImportedId, Action<Entities.Invoice> onInvoiceReady)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entities.Invoice> GetUnPaidPartnerInvoices(IEnumerable<PartnerInvoiceType> partnerInvoiceTypes)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<InvoiceByPartnerInfo> GetLastInvoicesByPartners(IEnumerable<PartnerInvoiceType> partnerInvoiceTypes)
        {
            throw new NotImplementedException();
        }

        public bool Update(Entities.Invoice invoice)
        {
            throw new NotImplementedException();
        }

        public Entities.Invoice GetInvoiceBySourceId(Guid invoiceTypeId, string sourceId)
        {
            throw new NotImplementedException();
        }

        public Entities.Invoice GetLastInvoice(Guid invoiceTypeId, string partnerId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateInvoicePaidDateBySourceId(Guid invoiceTypeId, string sourceId, DateTime paidDate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entities.Invoice> GetLasInvoices(Guid invoiceTypeId, string partnerId, DateTime? beforeDate, int lastInvoices)
        {
            throw new NotImplementedException();
        }

        public VRPopulatedPeriod GetInvoicesPopulatedPeriod(Guid invoiceTypeId, string partnerId)
        {
            throw new NotImplementedException();
        }

        public bool CheckPartnerIfHasInvoices(Guid invoiceTypeId, string partnerId)
        {
            throw new NotImplementedException();
        }

        public List<Entities.Invoice> GetInvoicesBySerialNumbers(Guid invoiceTypeId, IEnumerable<string> serialNumbers)
        {
            throw new NotImplementedException();
        }

        public bool UpdateInvoicePaidDateById(Guid invoiceTypeId, long invoiceId, DateTime paidDate)
        {
            throw new NotImplementedException();
        }

        public bool UpdateInvoiceSettings(long invoiceId, InvoiceSettings invoiceSettings)
        {
            throw new NotImplementedException();
        }

        public bool SetInvoiceSentDate(long invoiceId, DateTime? sentDate)
        {
            throw new NotImplementedException();
        }

        public bool DeleteGeneratedInvoice(long invoiceId, Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate)
        {
            BillingTransactionDataManager billingTransactionDataManager = new BillingTransactionDataManager();
            BillingPeriodInfoDataManager billingPeriodInfoDataManager = new BillingPeriodInfoDataManager();
            var billingPeriodInfo = billingPeriodInfoDataManager.GetBillingPeriodInfoById(partnerId, invoiceTypeId);
            var nextPeriodStart = toDate.AddDays(1);

            var queryContext = new RDBQueryContext(GetDataProvider());


            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column("IsDeleted").Value(true);
            updateQuery.Where().EqualsCondition("ID").Value(invoiceId);

            if (billingPeriodInfo.NextPeriodStart.Date == nextPeriodStart.Date)
            {
                billingPeriodInfoDataManager.AddInsertOrUpdateBillingPeriodInfo(queryContext, new BillingPeriodInfo
                            {
                                InvoiceTypeId = invoiceTypeId,
                                NextPeriodStart = fromDate,
                                PartnerId = partnerId
                            });
            }

            billingTransactionDataManager.AddQuerySetInvoiceBillingTransactionsDeleted(queryContext, invoiceId);

            queryContext.ExecuteNonQuery(true);
            return true;
        }

        public void LoadInvoices(Guid invoiceTypeId, DateTime? from, DateTime? to, Vanrise.GenericData.Entities.RecordFilterGroup filterGroup, Vanrise.GenericData.Entities.OrderDirection? orderDirection, Func<bool> shouldStop, Action<Entities.Invoice> onInvoiceLoaded)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().AllTableColumns("inv");

            var where = selectQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            if (from.HasValue)
                where.CompareCondition("CreatedTime", RDBCompareConditionOperator.GEq).Value(from.Value);
            if (to.HasValue)
                where.CompareCondition("CreatedTime", RDBCompareConditionOperator.LEq).Value(to.Value);
            if (filterGroup != null)
                RecordFilterRDBBuilder.RecordFilterGroupCondition(where, filterGroup, (fieldName, expressionContext) => expressionContext.Column("inv", GetColumnNameFromFieldName(fieldName)));

            if (orderDirection.HasValue)
                selectQuery.Sort().ByColumn("ID", orderDirection.Value == OrderDirection.Ascending ? RDBSortDirection.ASC : RDBSortDirection.DESC);

            queryContext.ExecuteReader(
                        (reader) =>
                        {
                            while (reader.Read())
                            {
                                onInvoiceLoaded(InvoiceMapper(reader));
                                if (shouldStop())
                                    break;
                            }
                        }
                    );
        }

        #endregion
    }

    public static class RecordFilterRDBBuilder
    {

        public static void RecordFilterGroupCondition(RDBConditionContext conditionContext, RecordFilterGroup filterGroup, Action<string, RDBExpressionContext> setExpressionFromField)
        {
            if (filterGroup == null || filterGroup.Filters == null)
                return;

            var groupConditionContext = conditionContext.ChildConditionGroup(filterGroup.LogicalOperator == RecordQueryLogicalOperator.And ? RDBConditionGroupOperator.AND : RDBConditionGroupOperator.OR);

            foreach (var filter in filterGroup.Filters)
            {
                RecordFilterGroup childFilterGroup = filter as RecordFilterGroup;
                if (childFilterGroup != null)
                {
                    RecordFilterGroupCondition(groupConditionContext, childFilterGroup, setExpressionFromField);
                    continue;
                }

                EmptyRecordFilter emptyFilter = filter as EmptyRecordFilter;
                if (emptyFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, emptyFilter, setExpressionFromField);
                    continue;
                }

                NonEmptyRecordFilter nonEmptyFilter = filter as NonEmptyRecordFilter;
                if (nonEmptyFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, nonEmptyFilter, setExpressionFromField);
                    continue;
                }

                StringRecordFilter stringFilter = filter as StringRecordFilter;
                if (stringFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, stringFilter, setExpressionFromField);
                    continue;
                }

                NumberRecordFilter numberFilter = filter as NumberRecordFilter;
                if (numberFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, numberFilter, setExpressionFromField);
                    continue;
                }

                DateTimeRecordFilter dateTimeFilter = filter as DateTimeRecordFilter;
                if (dateTimeFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, dateTimeFilter, setExpressionFromField);
                    continue;
                }

                BooleanRecordFilter booleanFilter = filter as BooleanRecordFilter;
                if (booleanFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, booleanFilter, setExpressionFromField);
                    continue;
                }

                NumberListRecordFilter numberListFilter = filter as NumberListRecordFilter;
                if (numberListFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, numberListFilter, setExpressionFromField);
                    continue;
                }

                StringListRecordFilter stringListRecordFilter = filter as StringListRecordFilter;
                if (stringListRecordFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, stringListRecordFilter, setExpressionFromField);
                    continue;
                }

                ObjectListRecordFilter objectListRecordFilter = filter as ObjectListRecordFilter;
                if (objectListRecordFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, objectListRecordFilter, setExpressionFromField);
                    continue;
                }

                AlwaysFalseRecordFilter alwaysFalseFilter = filter as AlwaysFalseRecordFilter;
                if (alwaysFalseFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, alwaysFalseFilter, setExpressionFromField);
                    continue;
                }

                AlwaysTrueRecordFilter alwaysTrueFilter = filter as AlwaysTrueRecordFilter;
                if (alwaysTrueFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, alwaysTrueFilter, setExpressionFromField);
                    continue;
                }
            }
        }

        private static void RecordFilterCondition(RDBConditionContext conditionContext, EmptyRecordFilter emptyFilter, Action<string, RDBExpressionContext> setExpressionFromField)
        {
            setExpressionFromField(emptyFilter.FieldName, conditionContext.NullCondition());
        }

        private static void RecordFilterCondition(RDBConditionContext conditionContext, NonEmptyRecordFilter nonEmptyFilter, Action<string, RDBExpressionContext> setExpressionFromField)
        {
            setExpressionFromField(nonEmptyFilter.FieldName, conditionContext.NotNullCondition());
        }

        private static void RecordFilterCondition(RDBConditionContext conditionContext, StringRecordFilter filter, Action<string, RDBExpressionContext> setExpressionFromField)
        {
            RDBCompareConditionOperator compareOperator;
            switch (filter.CompareOperator)
            {
                case StringRecordFilterOperator.Equals: compareOperator = RDBCompareConditionOperator.Eq; break;
                case StringRecordFilterOperator.NotEquals: compareOperator = RDBCompareConditionOperator.NEq; break;
                case StringRecordFilterOperator.Contains:compareOperator = RDBCompareConditionOperator.Contains; break;
                case StringRecordFilterOperator.NotContains:compareOperator = RDBCompareConditionOperator.NotContains; break;
                case StringRecordFilterOperator.StartsWith:compareOperator = RDBCompareConditionOperator.StartWith; break;
                case StringRecordFilterOperator.NotStartsWith:compareOperator = RDBCompareConditionOperator.NotStartWith; break;
                case StringRecordFilterOperator.EndsWith:compareOperator = RDBCompareConditionOperator.EndWith; break;
                case StringRecordFilterOperator.NotEndsWith:compareOperator = RDBCompareConditionOperator.NotEndWith; break;
                default: throw new NotSupportedException(string.Format("stringFilter.CompareOperator '{0}'", filter.CompareOperator.ToString()));
            }
            var compareConditionContext = conditionContext.CompareCondition(compareOperator);
            setExpressionFromField(filter.FieldName, compareConditionContext.Expression1());
            compareConditionContext.Expression2().Value(filter.Value);
        }

        private static void RecordFilterCondition(RDBConditionContext conditionContext, NumberRecordFilter filter, Action<string, RDBExpressionContext> setExpressionFromField)
        {
            RDBCompareConditionOperator compareOperator;
            switch (filter.CompareOperator)
            {
                case NumberRecordFilterOperator.Equals: compareOperator = RDBCompareConditionOperator.Eq; break;
                case NumberRecordFilterOperator.NotEquals: compareOperator = RDBCompareConditionOperator.NEq; break;
                case NumberRecordFilterOperator.Greater: compareOperator = RDBCompareConditionOperator.G; break;
                case NumberRecordFilterOperator.GreaterOrEquals: compareOperator = RDBCompareConditionOperator.GEq; break;
                case NumberRecordFilterOperator.Less: compareOperator = RDBCompareConditionOperator.L; break;
                case NumberRecordFilterOperator.LessOrEquals: compareOperator = RDBCompareConditionOperator.LEq; break;
                default: throw new NotSupportedException(string.Format("numberFilter.CompareOperator '{0}'", filter.CompareOperator.ToString()));
            }
            var compareConditionContext = conditionContext.CompareCondition(compareOperator);
            setExpressionFromField(filter.FieldName, compareConditionContext.Expression1());
            compareConditionContext.Expression2().Value(filter.Value);
        }

        private static void RecordFilterCondition(RDBConditionContext conditionContext, DateTimeRecordFilter dateTimeFilter, Action<string, RDBExpressionContext> setExpressionFromField)
        {

            BaseRDBExpression fieldDateExpression = null;
            setExpressionFromField(dateTimeFilter.FieldName, conditionContext.CreateExpressionContext((expression) => fieldDateExpression = expression));

            BaseRDBExpression fieldDatePartExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, fieldDateExpression);

            DateTime firstDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value);
            BaseRDBExpression firstDateTimeValueExpression = BuildExpressionFromDateTime(conditionContext, firstDateTimeValue);
            BaseRDBExpression startDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, firstDateTimeValueExpression);

            switch (dateTimeFilter.ComparisonPart)
            {
                case DateTimeRecordFilterComparisonPart.DateTime:
                case DateTimeRecordFilterComparisonPart.DateOnly:
                case DateTimeRecordFilterComparisonPart.TimeOnly:
                    GetDateTimeRecordFilterQuery(conditionContext, dateTimeFilter, fieldDatePartExpression, startDateExpression);
                    break;
                case DateTimeRecordFilterComparisonPart.YearMonth:
                    GetYearMonthRecordFilterQuery(conditionContext, dateTimeFilter, fieldDatePartExpression, startDateExpression, firstDateTimeValue);
                    break;
                case DateTimeRecordFilterComparisonPart.YearWeek:
                    GetYearWeekRecordFilterQuery(conditionContext, dateTimeFilter, fieldDatePartExpression, startDateExpression, firstDateTimeValue);
                    break;
                case DateTimeRecordFilterComparisonPart.Hour:
                    GetHourRecordFilterQuery(conditionContext, dateTimeFilter, fieldDatePartExpression, startDateExpression, firstDateTimeValue);
                    break;
                default: throw new NotSupportedException(string.Format("dateTimeFilter.ComparisonPart '{0}'", dateTimeFilter.ComparisonPart));
            }
        }

        private static BaseRDBExpression BuildExpressionFromDateTime(RDBConditionContext conditionContext, DateTime value)
        {
            BaseRDBExpression rdbExpression = null;
            conditionContext.CreateExpressionContext((expression) => rdbExpression = expression).Value(value);
            return rdbExpression;
        }

        private static void GetDateTimeRecordFilterQuery(RDBConditionContext conditionContext, DateTimeRecordFilter dateTimeFilter,
            BaseRDBExpression fieldDatePartExpression, BaseRDBExpression startDateExpression)
        {
            RDBCompareConditionOperator? compareOperator = null;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals: compareOperator = RDBCompareConditionOperator.Eq; break;
                case DateTimeRecordFilterOperator.NotEquals: compareOperator = RDBCompareConditionOperator.NEq; break;
                case DateTimeRecordFilterOperator.Greater: compareOperator = RDBCompareConditionOperator.G; break;
                case DateTimeRecordFilterOperator.GreaterOrEquals: compareOperator = RDBCompareConditionOperator.GEq; break;
                case DateTimeRecordFilterOperator.Less: compareOperator = RDBCompareConditionOperator.L; break;
                case DateTimeRecordFilterOperator.LessOrEquals: compareOperator = RDBCompareConditionOperator.LEq; break;
            }
            if (compareOperator.HasValue)
            {
                conditionContext.CompareCondition(fieldDatePartExpression, compareOperator.Value, startDateExpression);
                return;
            }
            dateTimeFilter.Value2.ThrowIfNull("dateTimeFilter.Value2", dateTimeFilter.FieldName);
            DateTime secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
            BaseRDBExpression secondDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, secondDateTimeValue));
            if (dateTimeFilter.CompareOperator == DateTimeRecordFilterOperator.Between)
            {
                var andCondition = conditionContext.ChildConditionGroup();                
                andCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                andCondition.CompareCondition(fieldDatePartExpression, dateTimeFilter.ExcludeValue2 ? RDBCompareConditionOperator.L : RDBCompareConditionOperator.LEq, secondDateExpression);

                return;
            }

            if (dateTimeFilter.CompareOperator == DateTimeRecordFilterOperator.NotBetween)
            {
                var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                orCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                orCondition.CompareCondition(fieldDatePartExpression, dateTimeFilter.ExcludeValue2 ? RDBCompareConditionOperator.G : RDBCompareConditionOperator.GEq, secondDateExpression);
                return;
            }

            throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator.ToString()));
        }

        private static void GetYearMonthRecordFilterQuery(RDBConditionContext conditionContext, DateTimeRecordFilter dateTimeFilter,
            BaseRDBExpression fieldDatePartExpression, BaseRDBExpression startDateExpression, DateTime firstDateTimeValue)
        {

            DateTime secondDateTimeValue;
            
            DateTime endDate;
            BaseRDBExpression endDateExpression;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var andCondition = conditionContext.ChildConditionGroup();
                    andCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                    andCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.NotEquals:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                    orCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    orCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.Greater:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate ));
                     conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.GreaterOrEquals:
                     conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                     return;
                case DateTimeRecordFilterOperator.Less:
                     conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                     return;
                case DateTimeRecordFilterOperator.LessOrEquals:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate ));
                     conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.Between:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);                    
                    endDate = GetFirstDayOfNextMonth(secondDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate ));
                    var andCondition2 = conditionContext.ChildConditionGroup();
                    andCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                    andCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.NotBetween:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = GetFirstDayOfNextMonth(secondDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate ));
                    var orCondition2 = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                   orCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                   orCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);
                     return;
                default: throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator));
            }
        }

        private static void GetYearWeekRecordFilterQuery(RDBConditionContext conditionContext, DateTimeRecordFilter dateTimeFilter,
             BaseRDBExpression fieldDatePartExpression, BaseRDBExpression startDateExpression, DateTime firstDateTimeValue)
        {
            DateTime secondDateTimeValue;

            DateTime endDate;
            BaseRDBExpression endDateExpression;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var andCondition = conditionContext.ChildConditionGroup();
                    andCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                    andCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.NotEquals:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate ));
                    var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                    orCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    orCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.Greater:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate ));
                     conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.GreaterOrEquals:
                     conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                     return;
                case DateTimeRecordFilterOperator.Less:
                     conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                     return;
                case DateTimeRecordFilterOperator.LessOrEquals:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                     conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.Between:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var andCondition2 = conditionContext.ChildConditionGroup();
                   andCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                   andCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.NotBetween:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var orCondition2 = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                    orCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    orCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);
                     return;
                default: throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator));
            }
        }

        private static void GetHourRecordFilterQuery(RDBConditionContext conditionContext, DateTimeRecordFilter dateTimeFilter,
             BaseRDBExpression fieldDatePartExpression, BaseRDBExpression startDateExpression, DateTime firstDateTimeValue)
        {
            DateTime secondDateTimeValue;

            DateTime endDate;
            BaseRDBExpression endDateExpression;

            bool isMidnight;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var andCondition = conditionContext.ChildConditionGroup();
                   andCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                   andCondition.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.LEq : RDBCompareConditionOperator.L, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.NotEquals:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate ));
                    var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                    orCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    orCondition.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.G : RDBCompareConditionOperator.GEq, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.Greater:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                     conditionContext.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.G : RDBCompareConditionOperator.GEq, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.GreaterOrEquals:
                     conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                     return;
                case DateTimeRecordFilterOperator.Less:
                     conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                     return;
                case DateTimeRecordFilterOperator.LessOrEquals:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                     conditionContext.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.LEq : RDBCompareConditionOperator.L, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.Between:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(secondDateTimeValue.Year, secondDateTimeValue.Month, secondDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var andCondition2 = conditionContext.ChildConditionGroup();
                    andCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                    andCondition2.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.LEq : RDBCompareConditionOperator.L, endDateExpression);
                     return;
                case DateTimeRecordFilterOperator.NotBetween:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(secondDateTimeValue.Year, secondDateTimeValue.Month, secondDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var orCondition2 = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                    orCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    orCondition2.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.G : RDBCompareConditionOperator.GEq, endDateExpression);
                     return;
                default: throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator));
            }
        }

        private static DateTime GetDateTimeValue(DateTimeRecordFilterComparisonPart comparisonPart, object value)
        {
            switch (comparisonPart)
            {
                case DateTimeRecordFilterComparisonPart.DateTime:
                case DateTimeRecordFilterComparisonPart.DateOnly:
                case DateTimeRecordFilterComparisonPart.YearMonth:
                    return (DateTime)value;

                case DateTimeRecordFilterComparisonPart.TimeOnly:
                    return Vanrise.Common.Utilities.AppendTimeToDateTime((Vanrise.Entities.Time)value, DateTime.Now);

                case DateTimeRecordFilterComparisonPart.Hour:
                    return Vanrise.Common.Utilities.AppendTimeToDateTime((Vanrise.Entities.Time)value, DateTime.Now);

                case DateTimeRecordFilterComparisonPart.YearWeek:
                    return Vanrise.Common.Utilities.GetMonday((DateTime)value);

                default: throw new NotSupportedException(string.Format("ComparisonPart '{0}'", comparisonPart));
            }
        }

        private static BaseRDBExpression GetDateExpressionBasedOnDatePart(RDBConditionContext conditionContext, DateTimeRecordFilterComparisonPart comparisonPart, BaseRDBExpression dateExpression)
        {
            if (comparisonPart == DateTimeRecordFilterComparisonPart.DateTime || comparisonPart == DateTimeRecordFilterComparisonPart.YearMonth)
            {
                return dateExpression;
            }
            else
            {                
                RDBDateTimePart part;
                switch (comparisonPart)
                {
                    case DateTimeRecordFilterComparisonPart.DateOnly:
                    case DateTimeRecordFilterComparisonPart.YearWeek: part = RDBDateTimePart.DateOnly; break;
                    case DateTimeRecordFilterComparisonPart.TimeOnly:
                    case DateTimeRecordFilterComparisonPart.Hour: part = RDBDateTimePart.TimeOnly; break;
                    default: throw new NotSupportedException(string.Format("comparisonPart '{0}'", comparisonPart));
                }
                BaseRDBExpression dateTimePartExpression = null;
                conditionContext.CreateExpressionContext((expression) => dateTimePartExpression = expression).DateTimePart(part).Expression(dateExpression);
                return dateTimePartExpression;
            }
        }

        private static DateTime GetFirstDayOfNextMonth(DateTime dateTime)
        {
            DateTime dateTimeNextMonth = dateTime.AddMonths(1);
            return new DateTime(dateTimeNextMonth.Year, dateTimeNextMonth.Month, 1);
        }

        private static bool CheckMidnight(DateTime dateTime)
        {
            if (dateTime.Hour == 0 && dateTime.Minute == 0 && dateTime.Second == 0 && dateTime.Millisecond == 0)
                return true;
            return false;
        }

       

        private static void RecordFilterCondition(RDBConditionContext conditionContext, BooleanRecordFilter booleanFilter, Action<string, RDBExpressionContext> setExpressionFromField)
        {
            BaseRDBExpression fieldExpression = null;
            setExpressionFromField(booleanFilter.FieldName, conditionContext.CreateExpressionContext((expression) => fieldExpression = expression));
            BaseRDBExpression valueExpression = null;
            conditionContext.CreateExpressionContext((exp) => valueExpression = exp).Value(booleanFilter.IsTrue);
            conditionContext.CompareCondition(fieldExpression, RDBCompareConditionOperator.Eq, valueExpression);
        }

        private static void RecordFilterCondition(RDBConditionContext conditionContext, NumberListRecordFilter numberListFilter, Action<string, RDBExpressionContext> setExpressionFromField)
        {
            List<BaseRDBExpression> valueExpressions = null;
            if (numberListFilter.Values != null)
            {
                valueExpressions = new List<BaseRDBExpression>();
                foreach (var value in numberListFilter.Values)
                {
                    conditionContext.CreateExpressionContext((expression) => valueExpressions.Add(expression)).Value(value);
                }
            }
            RecordFilterCondition(conditionContext, numberListFilter.FieldName, numberListFilter.CompareOperator, valueExpressions, setExpressionFromField);
            
        }

        private static void RecordFilterCondition(RDBConditionContext conditionContext, StringListRecordFilter stringListRecordFilter, Action<string, RDBExpressionContext> setExpressionFromField)
        {
            List<BaseRDBExpression> valueExpressions = null;
            if (stringListRecordFilter.Values != null)
            {
                valueExpressions = new List<BaseRDBExpression>();
                foreach (var value in stringListRecordFilter.Values)
                {
                    conditionContext.CreateExpressionContext((expression) => valueExpressions.Add(expression)).Value(value);
                }
            }
            RecordFilterCondition(conditionContext, stringListRecordFilter.FieldName, stringListRecordFilter.CompareOperator, valueExpressions, setExpressionFromField);
        }

        private static void RecordFilterCondition(RDBConditionContext conditionContext, ObjectListRecordFilter stringListRecordFilter, Action<string, RDBExpressionContext> setExpressionFromField)
        {
            List<BaseRDBExpression> valueExpressions = null;
            if (stringListRecordFilter.Values != null)
            {
                valueExpressions = new List<BaseRDBExpression>();
                foreach (var value in stringListRecordFilter.Values)
                {
                    conditionContext.CreateExpressionContext((expression) => valueExpressions.Add(expression)).ObjectValue(value);
                }
            }
            RecordFilterCondition(conditionContext, stringListRecordFilter.FieldName, stringListRecordFilter.CompareOperator, valueExpressions, setExpressionFromField);
            
        }

        private static void RecordFilterCondition(RDBConditionContext conditionContext, string fieldName, ListRecordFilterOperator compareOperator, List<BaseRDBExpression> values, Action<string, RDBExpressionContext> setExpressionFromField)
        {
            BaseRDBExpression fieldExpression = null;
            setExpressionFromField(fieldName, conditionContext.CreateExpressionContext((expression) => fieldExpression = expression));
            conditionContext.ListCondition(fieldExpression, compareOperator == ListRecordFilterOperator.In ? RDBListConditionOperator.IN : RDBListConditionOperator.NotIN, values);
        }

        private static void RecordFilterCondition(RDBConditionContext conditionContext, AlwaysFalseRecordFilter alwaysFalseFilter, Action<string, RDBExpressionContext> setExpressionFromField)
        {
            conditionContext.FalseCondition();
        }

        private static void RecordFilterCondition(RDBConditionContext conditionContext, AlwaysTrueRecordFilter alwaysTrueFilter, Action<string, RDBExpressionContext> setExpressionFromField)
        {
            conditionContext.TrueCondition();
        }
    }
}
