using System;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.MainExtensions.DARecordAggregates
{
    public class SumAggregate : DARecordAggregate
    {
        public override Guid ConfigId { get { return new Guid("DC962A83-2FDA-456F-9940-15E9BE787D89"); } }

        public override DataRecordFieldType FieldType
        {
            get
            {
                return new GenericData.MainExtensions.DataRecordFields.FieldNumberType
                {
                    DataType = GenericData.MainExtensions.DataRecordFields.FieldNumberDataType.Decimal
                };
            }
            set
            {
                base.FieldType = value;
            }
        }

        public string SumFieldName { get; set; }

        public string CurrencySQLColumnName { get; set; }


        public override DARecordAggregateState CreateState(IDARecordAggregateCreateStateContext context)
        {
            return new SumAggregateState();
        }

        public override void Evaluate(IDARecordAggregateEvaluationContext context)
        {
            if (context.Record == null)
                throw new ArgumentNullException("context.Record");

            dynamic sumField = context.Record.GetFieldValue(this.SumFieldName);

            if (!string.IsNullOrEmpty(this.CurrencySQLColumnName))
            {
                int? currencyId = context.Record.GetFieldValue(this.CurrencySQLColumnName);

                if (currencyId.HasValue && sumField != null)
                {
                    int systemCurrencyId = new ConfigManager().GetSystemCurrencyId();

                    DataRecordType dataRecordType = new DataRecordTypeManager().GetDataRecordType(context.DataRecordTypeId);
                    dataRecordType.ThrowIfNull("dataRecordType of Id:", context.DataRecordTypeId);
                    dataRecordType.Settings.ThrowIfNull(string.Format("dataRecordType.Settings of DataRecordType Id: {0}", context.DataRecordTypeId));
                    DateTime dateTimeField = context.Record.GetFieldValue(dataRecordType.Settings.DateTimeField);
                 
                    CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
                    decimal amountInSystemCurrency = currencyExchangeRateManager.ConvertValueToCurrency(sumField, currencyId.Value, systemCurrencyId, dateTimeField);
                    (context.State as SumAggregateState).Sum += amountInSystemCurrency;
                }
            }
            else
            {
                (context.State as SumAggregateState).Sum += sumField != null ? sumField : 0;
            }
        }

        public override void UpdateExistingFromNew(IDARecordAggregateUpdateExistingFromNewContext context)
        {
            (context.ExistingState as SumAggregateState).Sum += (context.NewState as SumAggregateState).Sum;
        }

        public override dynamic GetResult(IDARecordAggregateGetResultContext context)
        {
            return (context.State as SumAggregateState).Sum;
        }
    }

    public class SumAggregateState : DARecordAggregateState
    {
        public Decimal Sum { get; set; }
    }
}
