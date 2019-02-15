using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.MobileNetwork.Business;
using Vanrise.MobileNetwork.Entities;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class SupplierSMSRateManager
    {
        ISupplierSMSRateDataManager _supplierSMSRateDataManager = SMSBEDataFactory.GetDataManager<ISupplierSMSRateDataManager>();

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<SupplierSMSRateDetail> GetFilteredSupplierSMSRate(DataRetrievalInput<SupplierSMSRateQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierSMSRateRequestHandler());
        }

        public Dictionary<int, SupplierSMSRateItem> GetEffectiveMobileNetworkRates(int supplierID, DateTime dateTime)
        {
            var supplierSMSRatesByMobileNetworkIDs = GetSupplierSMSRatesByMobileNetworkID(supplierID, dateTime);

            if (supplierSMSRatesByMobileNetworkIDs == null)
                return null;

            Dictionary<int, SupplierSMSRateItem> supplierSMSRates = new Dictionary<int, SupplierSMSRateItem>();

            foreach (var mobileNetworkKvp in supplierSMSRatesByMobileNetworkIDs)
            {
                int mobileNetworkId = mobileNetworkKvp.Key;
                List<SupplierSMSRate> supplierSMSRatesByMobileNetworkID = mobileNetworkKvp.Value;

                supplierSMSRates.Add(mobileNetworkId, BuildSupplierSMSRateItem(supplierSMSRatesByMobileNetworkID, dateTime));
            }
            return supplierSMSRates;
        }

        #endregion

        #region Private Methods 

        private Dictionary<int, List<SupplierSMSRate>> GetSupplierSMSRatesByMobileNetworkID(int supplierID, DateTime effectiveDate)
        {
            List<SupplierSMSRate> supplierSMSRates = _supplierSMSRateDataManager.GetSupplierSMSRatesEffectiveAfter(supplierID, effectiveDate);

            if (supplierSMSRates == null)
                return null;

            Dictionary<int, List<SupplierSMSRate>> supplierSMSRatesByMobileNetworkID = new Dictionary<int, List<SupplierSMSRate>>();

            foreach (var supplierSMSRate in supplierSMSRates)
            {
                var mobileNetworkList = supplierSMSRatesByMobileNetworkID.GetOrCreateItem(supplierSMSRate.MobileNetworkID);
                mobileNetworkList.Add(supplierSMSRate);
            }

            return supplierSMSRatesByMobileNetworkID;
        }

        private SupplierSMSRateItem BuildSupplierSMSRateItem(List<SupplierSMSRate> supplierSMSRates, DateTime dateTime)
        {
            if (supplierSMSRates == null || supplierSMSRates.Count == 0)
                return null;

            SupplierSMSRate supplierSMSRate = supplierSMSRates.First();

            if (!supplierSMSRate.IsEffective(dateTime))
                return new SupplierSMSRateItem() { FutureRate = supplierSMSRate };

            if (supplierSMSRates.Count > 1)
                return new SupplierSMSRateItem() { CurrentRate = supplierSMSRate, FutureRate = supplierSMSRates[1] };

            return new SupplierSMSRateItem() { CurrentRate = supplierSMSRate };
        }

        #endregion

        #region Private Classes

        private class SupplierSMSRateRequestHandler : BigDataRequestHandler<SupplierSMSRateQuery, SupplierSMSRateItem, SupplierSMSRateDetail>
        {
            SupplierSMSRateManager _manager = new SupplierSMSRateManager();
            MobileNetworkManager _mobileNetworkManager = new MobileNetworkManager();

            public override SupplierSMSRateDetail EntityDetailMapper(SupplierSMSRateItem entity)
            {
                return SupplierSMSRateDetailMapper(entity);
            }

            public override IEnumerable<SupplierSMSRateItem> RetrieveAllData(DataRetrievalInput<SupplierSMSRateQuery> input)
            {
                if (input != null && input.Query != null)
                {
                    List<SupplierSMSRateItem> _supplierSMSRates = _manager.GetEffectiveMobileNetworkRates(input.Query.SupplierID, input.Query.EffectiveDate).Values.ToList();
                    return _supplierSMSRates.Where(item => FilterSupplierSMSRates(item, input.Query));
                }

                return null;
            }

            private SupplierSMSRateDetail SupplierSMSRateDetailMapper(SupplierSMSRateItem supplierSMSRate)
            {
                if (supplierSMSRate == null)
                    return null;

                var currentRate = supplierSMSRate.CurrentRate;
                var futureRate = supplierSMSRate.FutureRate;

                int mobileNetworkID = 0;
                if (currentRate != null)
                {

                    mobileNetworkID = currentRate.MobileNetworkID;
                }
                else if (futureRate != null)
                    mobileNetworkID = futureRate.MobileNetworkID;

                MobileNetwork mobileNetwork = _mobileNetworkManager.GetMobileNetworkById(mobileNetworkID);
                mobileNetwork.ThrowIfNull("mobileNetwork", mobileNetworkID);

                return new SupplierSMSRateDetail()
                {
                    ID = currentRate.ID,
                    MobileCountryName = new MobileCountryManager().GetMobileCountryName(mobileNetwork.MobileCountryId),
                    MobileNetworkName = mobileNetwork.NetworkName,
                    Rate = currentRate.Rate,
                    MobileNetworkID = mobileNetworkID,
                    BED = currentRate.BED,
                    EED = currentRate.EED,
                    FutureRate = futureRate != null ? new SMSFutureRate() { Rate = futureRate.Rate, BED = futureRate.BED, EED = futureRate.EED } : null
                };
            }

            private bool FilterSupplierSMSRates(SupplierSMSRateItem supplierSMSRate, SupplierSMSRateQuery queryInput)
            {
                if (supplierSMSRate.CurrentRate == null)
                    return false;

                int mobileNetworkID = supplierSMSRate.CurrentRate.MobileNetworkID;

                if (queryInput.MobileNetworkIds != null && queryInput.MobileNetworkIds.Count != 0 && !queryInput.MobileNetworkIds.Contains(mobileNetworkID))
                    return false;

                if (queryInput.MobileCountryIds != null && queryInput.MobileCountryIds.Count != 0)
                {
                    MobileNetwork mobileNetwork = _mobileNetworkManager.GetMobileNetworkById(mobileNetworkID);
                    mobileNetwork.ThrowIfNull("mobileNetwork", mobileNetworkID);
                    int mobileCountryId = mobileNetwork.MobileCountryId;

                    if (!queryInput.MobileCountryIds.Contains(mobileCountryId))
                        return false;
                }

                return true;
            }

            protected override ResultProcessingHandler<SupplierSMSRateDetail> GetResultProcessingHandler(DataRetrievalInput<SupplierSMSRateQuery> input, BigResult<SupplierSMSRateDetail> bigResult)
            {
                var resultProcessingHandler = new ResultProcessingHandler<SupplierSMSRateDetail>() { ExportExcelHandler = new SupplierSMSRateExcelExportHandler() };
                return resultProcessingHandler;
            }
        }

        private class SupplierSMSRateExcelExportHandler : ExcelExportHandler<SupplierSMSRateDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SupplierSMSRateDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Supplier SMS Rates",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() },
                    AutoFitColumns = true
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID", CellType = ExcelCellType.Number, NumberType = NumberType.BigInt });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Country" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Mobile Network", Width = 45 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", Width = 45, CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", Width = 45, CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null && context.BigResult.Data.Count() > 0)
                {

                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        row.Cells.Add(new ExportExcelCell { Value = record.ID });
                        row.Cells.Add(new ExportExcelCell { Value = record.MobileCountryName });
                        row.Cells.Add(new ExportExcelCell { Value = record.MobileNetworkName });
                        row.Cells.Add(new ExportExcelCell { Value = record.Rate.ToString() });
                        row.Cells.Add(new ExportExcelCell { Value = record.BED });
                        row.Cells.Add(new ExportExcelCell { Value = record.EED });

                        sheet.Rows.Add(row);
                    }
                }

                context.MainSheet = sheet;
            }
        }
    }

    #endregion
}
