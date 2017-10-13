﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public sealed class SetImportSPLContext : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<int> CurrencyId { get; set; }

        #endregion
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            metadata.AddDefaultExtensionProvider<IImportSPLContext>(() => new ImportSPLContext());
            base.CacheMetadata(metadata);
        }

        protected override void Execute(CodeActivityContext context)
        {
            int supplierId = SupplierId.Get(context);
            int currencyId = CurrencyId.Get(context);
            ImportSPLContext importSPLContext = context.GetSPLParameterContext() as ImportSPLContext;
            importSPLContext.PriceListCurrencyId = currencyId;
            importSPLContext.SetSupplierEffectiveDateDayOffset(supplierId);
        }
    }

    internal static class ContextExtensionMethods
    {
        public static IImportSPLContext GetSPLParameterContext(this ActivityContext context)
        {
            var parameterContext = context.GetExtension<IImportSPLContext>();
            if (parameterContext == null)
                throw new NullReferenceException("parameterContext");
            return parameterContext;
        }
    }

    public class ImportSPLContext : IImportSPLContext
    {
        private object _obj = new object();

        private volatile bool _processHasChanges = false;

        private TimeSpan _codeCloseDateOffset;

        public const string CustomDataKey = "ImportSPLContext";

        private int _priceListCurrencyId;
        private int _systemCurrencyId;
        private Dictionary<int, decimal> _maximumRateConvertedByCurrency = new Dictionary<int, decimal>();
        public ImportSPLContext()
        {

            CurrencyManager currencyManager = new CurrencyManager();
            var systemCurrency = currencyManager.GetSystemCurrency();
            if (systemCurrency == null)
                throw new DataIntegrityValidationException("System Currency was not found");
            _systemCurrencyId = systemCurrency.CurrencyId;


            var tOneConfigManager = new TOne.WhS.BusinessEntity.Business.ConfigManager();
            MaximumRate = tOneConfigManager.GetPurchaseAreaMaximumRate();

            int retroactiveDayOffset = tOneConfigManager.GetPurchaseAreaRetroactiveDayOffset();
            RetroactiveDate = DateTime.Today.AddDays(-retroactiveDayOffset).Date;

            DateFormat = new Vanrise.Common.Business.GeneralSettingsManager().GetDateFormat();
        }
        public int PriceListCurrencyId
        {
            get
            {
                return _priceListCurrencyId;
            }
            set { _priceListCurrencyId = value; }
        }
        public bool ProcessHasChanges
        {
            get
            {
                return this._processHasChanges;
            }
        }
        public TimeSpan CodeCloseDateOffset
        {
            get
            {
                return _codeCloseDateOffset;
            }
            set { _codeCloseDateOffset = value; }
        }
        public DateTime CodeEffectiveDate { get; set; }
        public decimal MaximumRate { get; set; }

        public DateTime RetroactiveDate { get; set; }
        public string DateFormat { get; set; }

        #region public functions
        public void SetToTrueProcessHasChangesWithLock()
        {
            if (!_processHasChanges)
            {
                lock (_obj)
                {
                    this._processHasChanges = true;
                }
            }
        }

        public decimal GetMaximumRateConverted(int currencyId)
        {
            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
            decimal convertedRate;
            if (!_maximumRateConvertedByCurrency.TryGetValue(currencyId, out convertedRate))
            {
                decimal convertedmaximumRate = currencyExchangeRateManager.ConvertValueToCurrency(MaximumRate,
                _systemCurrencyId, currencyId, DateTime.Now);
                _maximumRateConvertedByCurrency.Add(currencyId, convertedmaximumRate);
                return convertedmaximumRate;
            }
            return convertedRate;
        }

        public int GetImportedRateCurrencyId(ImportedRate importedRate)
        {
            return _priceListCurrencyId;
        }

        public void SetSupplierEffectiveDateDayOffset(int supplierId)
        {
            int effectiveDateDayOffset = new TOne.WhS.BusinessEntity.Business.CarrierAccountManager().GetSupplierEffectiveDateDayOffset(supplierId);
            TimeSpan codeCloseDateOffset = new TimeSpan(effectiveDateDayOffset, 0, 0, 0);
            _codeCloseDateOffset = codeCloseDateOffset;
            CodeEffectiveDate = DateTime.Today.AddDays(effectiveDateDayOffset);
        }
        #endregion
    }
}
