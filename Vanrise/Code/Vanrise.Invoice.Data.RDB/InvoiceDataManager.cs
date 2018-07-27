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
            columns.Add("InvoiceSettingID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier});
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
                InvoiceId =reader.GetLong("ID"),
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
                    long insertedInvoiceId = new RDBQueryContext(dataProvider)
                                                    .Insert()
                                                    .IntoTable(TABLE_NAME)
                                                    .GenerateIdAndAssignToParameter("ID")
                                                    .ColumnValue("UserId", generateInvoiceInputToSave.Invoice.UserId)
                                                    .ColumnValue("PartnerID", generateInvoiceInputToSave.Invoice.PartnerId)
                                                    .ColumnValueDBNullIfDefaultValue("SplitInvoiceGroupId", splitInvoiceGroupId)
                                                    .ColumnValue("InvoiceSettingID", generateInvoiceInputToSave.Invoice.InvoiceSettingId)
                                                    .ColumnValue("SerialNumber", generateInvoiceInputToSave.Invoice.SerialNumber)
                                                    .ColumnValue("FromDate", generateInvoiceInputToSave.Invoice.FromDate)
                                                    .ColumnValue("ToDate", generateInvoiceInputToSave.Invoice.ToDate)
                                                    .ColumnValue("IssueDate", generateInvoiceInputToSave.Invoice.IssueDate)
                                                    .ColumnValue("DueDate", generateInvoiceInputToSave.Invoice.DueDate)
                                                    .ColumnValue("Details", serializedDetails)
                                                    .ColumnValue("Notes", generateInvoiceInputToSave.Invoice.Note)
                                                    .ColumnValue("SourceId", generateInvoiceInputToSave.Invoice.SourceId)
                                                    .ColumnValue("IsDraft", true)
                                                    .ColumnValue("IsAutomatic", generateInvoiceInputToSave.Invoice.IsAutomatic)
                                                    .ColumnValue("Settings", serializedSettings)
                                                    .ColumnValueDBNullIfDefaultValue("SentDate", generateInvoiceInputToSave.Invoice.SentDate)
                                                    .ColumnValueDBNullIfDefaultValue("NeedApproval", generateInvoiceInputToSave.Invoice.NeedApproval)
                                                    .EndInsert()
                                                    .ExecuteScalar()
                                                    .LongValue;

                   

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

                new RDBQueryContext(dataProvider)
                    .StartBatchQuery()
                    .Foreach(generateInvoicesInputToSave, 
                        (generateInvoiceInputToSave, ctx) =>
                            ctx.AddQueryIf(() => generateInvoiceInputToSave.InvoiceToSettleIds != null && generateInvoiceInputToSave.InvoiceToSettleIds.Count > 0,
                                ctx2 =>                            
                                ctx2.StartBatchQuery()
                                    .AddQuery().Update()
                                                .FromTable(TABLE_NAME)
                                                .Where().ListCondition("ID", RDBListConditionOperator.IN, generateInvoiceInputToSave.InvoiceToSettleIds)
                                                .ColumnValue("SettlementInvoiceId", generateInvoiceInputToSave.Invoice.InvoiceId)
                                                .EndUpdate()
                                    .AddQueryIf(() => generateInvoiceInputToSave.InvoiceIdToDelete.HasValue, 
                                                    ctx3 =>
                                                    ctx3.Update().FromTable(TABLE_NAME)
                                                    .Where().EqualsCondition("SettlementInvoiceId", generateInvoiceInputToSave.InvoiceIdToDelete.Value)
                                                    .ColumnValue("SettlementInvoiceId", new RDBNullExpression())
                                                    .EndUpdate()
                                                    )
                                    .EndBatchQuery()
                                            )
                               .AddQueryIf(() => generateInvoiceInputToSave.MappedTransactions != null && generateInvoiceInputToSave.MappedTransactions.Count() > 0,
                                             ctx2 =>
                                                 ctx2.StartBatchQuery()
                                                     .InsertInvoiceBillingTransactions(generateInvoiceInputToSave.MappedTransactions, generateInvoiceInputToSave.Invoice.InvoiceId)
                                                     .EndBatchQuery()
                                                    )
                               .AddQueryIf(() => generateInvoiceInputToSave.InvoiceIdToDelete.HasValue,
                                                ctx2 =>
                                                    ctx2.StartBatchQuery()
                                                        .AddQuery().Update().FromTable(TABLE_NAME).Where().EqualsCondition("ID", generateInvoiceInputToSave.InvoiceIdToDelete.Value).ColumnValue("IsDeleted", true).EndUpdate()
                                                        .SetInvoiceBillingTransactionsDeleted(generateInvoiceInputToSave.InvoiceIdToDelete.Value)
                                                        .EndBatchQuery()
                                          )
                                .AddQuery().Update().FromTable(TABLE_NAME).Where().EqualsCondition("ID", generateInvoiceInputToSave.Invoice.InvoiceId).ColumnValue("IsDraft", false).EndUpdate()                    
                            )
                    .EndBatchQuery()
                    .ExecuteNonQuery(true);
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
            BillingPeriodInfoDataManager billingPeriodInfoDataManager = new BillingPeriodInfoDataManager();
            var billingPeriodInfo = billingPeriodInfoDataManager.GetBillingPeriodInfoById(partnerId, invoiceTypeId);

            var nextPeriodStart = toDate.AddDays(1);
            new RDBQueryContext(GetDataProvider())
                .StartBatchQuery()
                .AddQuery().Update().FromTable(TABLE_NAME).Where().EqualsCondition("ID", invoiceId).ColumnValue("IsDeleted", true).EndUpdate()
                .AddQueryIf(() => billingPeriodInfo.NextPeriodStart.Date == nextPeriodStart.Date, ctx => ctx.InsertOrUpdateBillingPeriodInfo(new BillingPeriodInfo
                            {
                                InvoiceTypeId = invoiceTypeId,
                                NextPeriodStart = fromDate,
                                PartnerId = partnerId
                            }))
                .SetInvoiceBillingTransactionsDeleted(invoiceId)
                .EndBatchQuery()
                .ExecuteNonQuery(true);
            return true;
        }

        public void LoadInvoices(Guid invoiceTypeId, DateTime? from, DateTime? to, Vanrise.GenericData.Entities.RecordFilterGroup filterGroup, Vanrise.GenericData.Entities.OrderDirection? orderDirection, Func<bool> shouldStop, Action<Entities.Invoice> onInvoiceLoaded)
        {
            new RDBQueryContext(GetDataProvider())
                    .Select()
                    .From(TABLE_NAME, "inv")
                    .Where()
                        .And()
                            .EqualsCondition("InvoiceTypeID", invoiceTypeId)
                            .ConditionIfNotDefaultValue(from, (ctx) => ctx.CompareCondition("CreatedTime", RDBCompareConditionOperator.GEq, from.Value))
                            .ConditionIfNotDefaultValue(to, (ctx) => ctx.CompareCondition("CreatedTime", RDBCompareConditionOperator.LEq, to.Value))
                            .ConditionIf(() => filterGroup != null, ctx => ctx.RecordFilterGroupCondition(filterGroup, (fieldName) => new RDBColumnExpression { TableAlias = "inv", ColumnName = GetColumnNameFromFieldName(fieldName) }))
                        .EndAnd()
                    .SelectColumns().AllTableColumns("inv").EndColumns()
                    .Sort().AddSortIf(() => orderDirection.HasValue, ctx => ctx.ByColumn("ID", orderDirection.Value == OrderDirection.Ascending ? RDBSortDirection.ASC : RDBSortDirection.DESC))
                    .EndSort()
                    .EndSelect()
                    .ExecuteReader(
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

        public static T RecordFilterGroupCondition<T>(this RDBConditionContext<T> conditionContext, RecordFilterGroup filterGroup, Func<string, BaseRDBExpression> getExpressionFromFieldName)
        {
            if (filterGroup == null || filterGroup.Filters == null)
                return conditionContext.TrueCondition();

            RDBGroupConditionContext<T> groupConditionContext = conditionContext.ConditionGroup(filterGroup.LogicalOperator == RecordQueryLogicalOperator.And ? RDBGroupConditionType.And : RDBGroupConditionType.Or);

            foreach (var filter in filterGroup.Filters)
            {
                RecordFilterGroup childFilterGroup = filter as RecordFilterGroup;
                if (childFilterGroup != null)
                {
                    groupConditionContext.RecordFilterGroupCondition(childFilterGroup, getExpressionFromFieldName);
                    continue;
                }

                EmptyRecordFilter emptyFilter = filter as EmptyRecordFilter;
                if (emptyFilter != null)
                {
                    groupConditionContext.RecordFilterCondition(emptyFilter, getExpressionFromFieldName);
                    continue;
                }

                NonEmptyRecordFilter nonEmptyFilter = filter as NonEmptyRecordFilter;
                if (nonEmptyFilter != null)
                {
                    groupConditionContext.RecordFilterCondition(nonEmptyFilter, getExpressionFromFieldName);
                    continue;
                }

                StringRecordFilter stringFilter = filter as StringRecordFilter;
                if (stringFilter != null)
                {
                    groupConditionContext.RecordFilterCondition(stringFilter, getExpressionFromFieldName);
                    continue;
                }

                NumberRecordFilter numberFilter = filter as NumberRecordFilter;
                if (numberFilter != null)
                {
                    groupConditionContext.RecordFilterCondition(numberFilter, getExpressionFromFieldName);
                    continue;
                }

                DateTimeRecordFilter dateTimeFilter = filter as DateTimeRecordFilter;
                if (dateTimeFilter != null)
                {
                    groupConditionContext.RecordFilterCondition(dateTimeFilter, getExpressionFromFieldName);
                    continue;
                }

                BooleanRecordFilter booleanFilter = filter as BooleanRecordFilter;
                if (booleanFilter != null)
                {
                    groupConditionContext.RecordFilterCondition(booleanFilter, getExpressionFromFieldName);
                    continue;
                }

                NumberListRecordFilter numberListFilter = filter as NumberListRecordFilter;
                if (numberListFilter != null)
                {
                    groupConditionContext.RecordFilterCondition(numberListFilter, getExpressionFromFieldName);
                    continue;
                }

                StringListRecordFilter stringListRecordFilter = filter as StringListRecordFilter;
                if (stringListRecordFilter != null)
                {
                    groupConditionContext.RecordFilterCondition(stringListRecordFilter, getExpressionFromFieldName);
                    continue;
                }

                ObjectListRecordFilter objectListRecordFilter = filter as ObjectListRecordFilter;
                if (objectListRecordFilter != null)
                {
                    groupConditionContext.RecordFilterCondition(objectListRecordFilter, getExpressionFromFieldName);
                    continue;
                }

                AlwaysFalseRecordFilter alwaysFalseFilter = filter as AlwaysFalseRecordFilter;
                if (alwaysFalseFilter != null)
                {
                    groupConditionContext.RecordFilterCondition(alwaysFalseFilter, getExpressionFromFieldName);
                    continue;
                }

                AlwaysTrueRecordFilter alwaysTrueFilter = filter as AlwaysTrueRecordFilter;
                if (alwaysTrueFilter != null)
                {
                    groupConditionContext.RecordFilterCondition(alwaysTrueFilter, getExpressionFromFieldName);
                    continue;
                }
            }

            return groupConditionContext.EndConditionGroup();
        }

        private static RDBGroupConditionContext<T> RecordFilterCondition<T>(this RDBGroupConditionContext<T> groupConditionContext, EmptyRecordFilter emptyFilter, Func<string, BaseRDBExpression> getExpressionFromFieldName)
        {
            return groupConditionContext.NullCondition(getExpressionFromFieldName(emptyFilter.FieldName));
        }

        private static RDBGroupConditionContext<T> RecordFilterCondition<T>(this RDBGroupConditionContext<T> groupConditionContext, NonEmptyRecordFilter nonEmptyFilter, Func<string, BaseRDBExpression> getExpressionFromFieldName)
        {
            return groupConditionContext.NotNullCondition(getExpressionFromFieldName(nonEmptyFilter.FieldName));
        }

        private static RDBGroupConditionContext<T> RecordFilterCondition<T>(this RDBGroupConditionContext<T> groupConditionContext, StringRecordFilter stringFilter, Func<string, BaseRDBExpression> getExpressionFromFieldName)
        {
            RDBCompareConditionOperator compareOperator;
            switch (stringFilter.CompareOperator)
            {
                case StringRecordFilterOperator.Equals: compareOperator = RDBCompareConditionOperator.Eq; break;
                case StringRecordFilterOperator.NotEquals: compareOperator = RDBCompareConditionOperator.NEq; break;
                case StringRecordFilterOperator.Contains:compareOperator = RDBCompareConditionOperator.Contains; break;
                case StringRecordFilterOperator.NotContains:compareOperator = RDBCompareConditionOperator.NotContains; break;
                case StringRecordFilterOperator.StartsWith:compareOperator = RDBCompareConditionOperator.StartWith; break;
                case StringRecordFilterOperator.NotStartsWith:compareOperator = RDBCompareConditionOperator.NotStartWith; break;
                case StringRecordFilterOperator.EndsWith:compareOperator = RDBCompareConditionOperator.EndWith; break;
                case StringRecordFilterOperator.NotEndsWith:compareOperator = RDBCompareConditionOperator.NotEndWith; break;
                default: throw new NotSupportedException(string.Format("stringFilter.CompareOperator '{0}'", stringFilter.CompareOperator.ToString()));
            }
            return groupConditionContext.CompareCondition(getExpressionFromFieldName(stringFilter.FieldName), compareOperator, new RDBFixedTextExpression { Value = stringFilter.Value });
        }

        private static RDBGroupConditionContext<T> RecordFilterCondition<T>(this RDBGroupConditionContext<T> groupConditionContext, NumberRecordFilter numberFilter, Func<string, BaseRDBExpression> getExpressionFromFieldName)
        {
            RDBCompareConditionOperator compareOperator;
            switch (numberFilter.CompareOperator)
            {
                case NumberRecordFilterOperator.Equals: compareOperator = RDBCompareConditionOperator.Eq; break;
                case NumberRecordFilterOperator.NotEquals: compareOperator = RDBCompareConditionOperator.NEq; break;
                case NumberRecordFilterOperator.Greater: compareOperator = RDBCompareConditionOperator.G; break;
                case NumberRecordFilterOperator.GreaterOrEquals: compareOperator = RDBCompareConditionOperator.GEq; break;
                case NumberRecordFilterOperator.Less: compareOperator = RDBCompareConditionOperator.L; break;
                case NumberRecordFilterOperator.LessOrEquals: compareOperator = RDBCompareConditionOperator.LEq; break;
                default: throw new NotSupportedException(string.Format("numberFilter.CompareOperator '{0}'", numberFilter.CompareOperator.ToString()));
            }
            return groupConditionContext.CompareCondition(getExpressionFromFieldName(numberFilter.FieldName), compareOperator, new RDBFixedDecimalExpression { Value = numberFilter.Value });
        }

        #region Date Filter Commented

        private static RDBGroupConditionContext<T> RecordFilterCondition<T>(this RDBGroupConditionContext<T> groupConditionContext, DateTimeRecordFilter dateTimeFilter, Func<string, BaseRDBExpression> getExpressionFromFieldName)
        {
            BaseRDBExpression fieldDateExpression = getExpressionFromFieldName(dateTimeFilter.FieldName);
            BaseRDBExpression fieldDatePartExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, fieldDateExpression);

            DateTime firstDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value);            
            BaseRDBExpression startDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = firstDateTimeValue });

            switch (dateTimeFilter.ComparisonPart)
            {
                case DateTimeRecordFilterComparisonPart.DateTime:
                case DateTimeRecordFilterComparisonPart.DateOnly:
                case DateTimeRecordFilterComparisonPart.TimeOnly:
                    return groupConditionContext.GetDateTimeRecordFilterQuery(dateTimeFilter, fieldDatePartExpression, startDateExpression);
                case DateTimeRecordFilterComparisonPart.YearMonth:
                    return groupConditionContext.GetYearMonthRecordFilterQuery(dateTimeFilter, fieldDatePartExpression, startDateExpression, firstDateTimeValue);

                case DateTimeRecordFilterComparisonPart.YearWeek:
                    return groupConditionContext.GetYearWeekRecordFilterQuery(dateTimeFilter, fieldDatePartExpression, startDateExpression, firstDateTimeValue);

                case DateTimeRecordFilterComparisonPart.Hour:
                    return groupConditionContext.GetHourRecordFilterQuery(dateTimeFilter, fieldDatePartExpression, startDateExpression, firstDateTimeValue);

                default: throw new NotSupportedException(string.Format("dateTimeFilter.ComparisonPart '{0}'", dateTimeFilter.ComparisonPart));
            }
        }

        private static RDBGroupConditionContext<T> GetDateTimeRecordFilterQuery<T>(this RDBGroupConditionContext<T> groupConditionContext, DateTimeRecordFilter dateTimeFilter,
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
            if(compareOperator.HasValue)
                return groupConditionContext.CompareCondition(fieldDatePartExpression, compareOperator.Value, startDateExpression);

            dateTimeFilter.Value2.ThrowIfNull("dateTimeFilter.Value2", dateTimeFilter.FieldName);            
            DateTime secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
            BaseRDBExpression secondDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = secondDateTimeValue });
            if (dateTimeFilter.CompareOperator == DateTimeRecordFilterOperator.Between)
            {                
                    return groupConditionContext.And()
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression)
                                                    .CompareCondition(fieldDatePartExpression, dateTimeFilter.ExcludeValue2 ? RDBCompareConditionOperator.L : RDBCompareConditionOperator.LEq, secondDateExpression)
                                                .EndAnd();
            }

            if (dateTimeFilter.CompareOperator == DateTimeRecordFilterOperator.NotBetween)
            {
                return groupConditionContext.Or()
                                                .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression)
                                                .CompareCondition(fieldDatePartExpression, dateTimeFilter.ExcludeValue2 ? RDBCompareConditionOperator.G : RDBCompareConditionOperator.GEq, secondDateExpression)
                                            .EndOr();
            }

            throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator.ToString()));
        }

        private static RDBGroupConditionContext<T> GetYearMonthRecordFilterQuery<T>(this RDBGroupConditionContext<T> groupConditionContext, DateTimeRecordFilter dateTimeFilter,
            BaseRDBExpression fieldDatePartExpression, BaseRDBExpression startDateExpression, DateTime firstDateTimeValue)
        {

            DateTime secondDateTimeValue;
            
            DateTime endDate;
            BaseRDBExpression endDateExpression;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.And()
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression)
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression)
                                                .EndAnd();

                case DateTimeRecordFilterOperator.NotEquals:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.Or()
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression)
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression)
                                                .EndOr();

                case DateTimeRecordFilterOperator.Greater:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);

                case DateTimeRecordFilterOperator.GreaterOrEquals:
                    return groupConditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);

                case DateTimeRecordFilterOperator.Less:
                    return groupConditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);

                case DateTimeRecordFilterOperator.LessOrEquals:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);

                case DateTimeRecordFilterOperator.Between:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);                    
                    endDate = GetFirstDayOfNextMonth(secondDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.And()
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression)
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression)
                                                .EndAnd();

                case DateTimeRecordFilterOperator.NotBetween:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = GetFirstDayOfNextMonth(secondDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.Or()
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression)
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression)
                                                .EndOr();

                default: throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator));
            }
        }

        private static RDBGroupConditionContext<T> GetYearWeekRecordFilterQuery<T>(this RDBGroupConditionContext<T> groupConditionContext, DateTimeRecordFilter dateTimeFilter,
             BaseRDBExpression fieldDatePartExpression, BaseRDBExpression startDateExpression, DateTime firstDateTimeValue)
        {
            DateTime secondDateTimeValue;

            DateTime endDate;
            BaseRDBExpression endDateExpression;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.And()
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression)
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression)
                                                .EndAnd();

                case DateTimeRecordFilterOperator.NotEquals:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.Or()
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression)
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression)
                                                .EndOr();

                case DateTimeRecordFilterOperator.Greater:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);

                case DateTimeRecordFilterOperator.GreaterOrEquals:
                    return groupConditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);

                case DateTimeRecordFilterOperator.Less:
                    return groupConditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);

                case DateTimeRecordFilterOperator.LessOrEquals:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);

                case DateTimeRecordFilterOperator.Between:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.And()
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression)
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression)
                                                .EndAnd();

                case DateTimeRecordFilterOperator.NotBetween:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.Or()
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression)
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression)
                                                .EndOr();

                default: throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator));
            }
        }

        private static RDBGroupConditionContext<T> GetHourRecordFilterQuery<T>(this RDBGroupConditionContext<T> groupConditionContext, DateTimeRecordFilter dateTimeFilter,
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

                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.And()
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression)
                                                    .CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.LEq : RDBCompareConditionOperator.L, endDateExpression)
                                                .EndAnd();

                case DateTimeRecordFilterOperator.NotEquals:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.Or()
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression)
                                                    .CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.G : RDBCompareConditionOperator.GEq, endDateExpression)
                                                .EndOr();

                case DateTimeRecordFilterOperator.Greater:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.G : RDBCompareConditionOperator.GEq, endDateExpression);

                case DateTimeRecordFilterOperator.GreaterOrEquals:
                    return groupConditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);

                case DateTimeRecordFilterOperator.Less:
                    return groupConditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);

                case DateTimeRecordFilterOperator.LessOrEquals:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.LEq : RDBCompareConditionOperator.L, endDateExpression);

                case DateTimeRecordFilterOperator.Between:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(secondDateTimeValue.Year, secondDateTimeValue.Month, secondDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.And()
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression)
                                                    .CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.LEq : RDBCompareConditionOperator.L, endDateExpression)
                                                .EndAnd();

                case DateTimeRecordFilterOperator.NotBetween:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(secondDateTimeValue.Year, secondDateTimeValue.Month, secondDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(dateTimeFilter.ComparisonPart, new RDBFixedDateTimeExpression { Value = endDate });
                    return groupConditionContext.Or()
                                                    .CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression)
                                                    .CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.G : RDBCompareConditionOperator.GEq, endDateExpression)
                                                .EndOr();

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

        private static BaseRDBExpression GetDateExpressionBasedOnDatePart(DateTimeRecordFilterComparisonPart comparisonPart, BaseRDBExpression dateExpression)
        {
            if (comparisonPart == DateTimeRecordFilterComparisonPart.DateTime || comparisonPart == DateTimeRecordFilterComparisonPart.YearMonth)
            {
                return dateExpression;
            }
            else
            {
                RDBDateTimePartExpression dateTimePartExpression = new RDBDateTimePartExpression
                {
                    DateTimeExpression = dateExpression
                };
                switch (comparisonPart)
                {
                    case DateTimeRecordFilterComparisonPart.DateOnly:                     
                    case DateTimeRecordFilterComparisonPart.YearWeek: dateTimePartExpression.Part = RDBDateTimePart.DateOnly;break;
                    case DateTimeRecordFilterComparisonPart.TimeOnly: 
                    case DateTimeRecordFilterComparisonPart.Hour: dateTimePartExpression.Part = RDBDateTimePart.TimeOnly;break;
                    default: throw new NotSupportedException(string.Format("comparisonPart '{0}'", comparisonPart));
                }
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

        #endregion

        private static RDBGroupConditionContext<T> RecordFilterCondition<T>(this RDBGroupConditionContext<T> groupConditionContext, BooleanRecordFilter booleanFilter, Func<string, BaseRDBExpression> getExpressionFromFieldName)
        {
            return groupConditionContext.CompareCondition(getExpressionFromFieldName(booleanFilter.FieldName), RDBCompareConditionOperator.Eq, new RDBFixedBooleanExpression { Value = booleanFilter.IsTrue });
        }

        private static RDBGroupConditionContext<T> RecordFilterCondition<T>(this RDBGroupConditionContext<T> groupConditionContext, NumberListRecordFilter numberListFilter, Func<string, BaseRDBExpression> getExpressionFromFieldName)
        {
            return groupConditionContext.RecordFilterCondition(numberListFilter.FieldName, numberListFilter.CompareOperator, numberListFilter.Values.Select<Decimal, BaseRDBExpression>(value => new RDBFixedDecimalExpression { Value = value }).ToList(), getExpressionFromFieldName);
            
        }

        private static RDBGroupConditionContext<T> RecordFilterCondition<T>(this RDBGroupConditionContext<T> groupConditionContext, StringListRecordFilter stringListRecordFilter, Func<string, BaseRDBExpression> getExpressionFromFieldName)
        {
            return groupConditionContext.RecordFilterCondition(stringListRecordFilter.FieldName, stringListRecordFilter.CompareOperator, stringListRecordFilter.Values.Select<string, BaseRDBExpression>(value => new RDBFixedTextExpression { Value = value }).ToList(), getExpressionFromFieldName);
        }

        private static RDBGroupConditionContext<T> RecordFilterCondition<T>(this RDBGroupConditionContext<T> groupConditionContext, ObjectListRecordFilter stringListRecordFilter, Func<string, BaseRDBExpression> getExpressionFromFieldName)
        {
            return groupConditionContext.RecordFilterCondition(stringListRecordFilter.FieldName, stringListRecordFilter.CompareOperator, stringListRecordFilter.Values.Select(ObjectToExpression).ToList(), getExpressionFromFieldName);
            
        }

        static BaseRDBExpression ObjectToExpression(Object value)
        {
            if (value == null)
                return new RDBNullExpression();
            if (value is string)
                return new RDBFixedTextExpression { Value = value as string };
            if (value is int)
                return new RDBFixedIntExpression { Value = (int)value };
            if (value is long)
                return new RDBFixedLongExpression { Value = (long)value };
            if (value is decimal)
                return new RDBFixedDecimalExpression { Value = (decimal)value };
            if (value is float)
                return new RDBFixedFloatExpression { Value = (float)value };
            if (value is Guid)
                return new RDBFixedGuidExpression { Value = (Guid)value };
            if (value is DateTime)
                return new RDBFixedDateTimeExpression { Value = (DateTime)value };
            if (value is bool)
                return new RDBFixedBooleanExpression { Value = (bool)value };
            if (value is byte[])
                return new RDBFixedBytesExpression { Value = (byte[])value };
            throw new NotSupportedException(string.Format("value type '{0}'", value.GetType()));
        }

        private static RDBGroupConditionContext<T> RecordFilterCondition<T>(this RDBGroupConditionContext<T> groupConditionContext, string fieldName, ListRecordFilterOperator compareOperator, List<BaseRDBExpression> values, Func<string, BaseRDBExpression> getExpressionFromFieldName)
        {
            return groupConditionContext.ListCondition(getExpressionFromFieldName(fieldName), compareOperator == ListRecordFilterOperator.In ? RDBListConditionOperator.IN : RDBListConditionOperator.NotIN, values);
        }

        private static RDBGroupConditionContext<T> RecordFilterCondition<T>(this RDBGroupConditionContext<T> groupConditionContext, AlwaysFalseRecordFilter alwaysFalseFilter, Func<string, BaseRDBExpression> getExpressionFromFieldName)
        {
            return groupConditionContext.FalseCondition();
        }

        private static RDBGroupConditionContext<T> RecordFilterCondition<T>(this RDBGroupConditionContext<T> groupConditionContext, AlwaysTrueRecordFilter alwaysTrueFilter, Func<string, BaseRDBExpression> getExpressionFromFieldName)
        {
            return groupConditionContext.TrueCondition();
        }
    }
}
