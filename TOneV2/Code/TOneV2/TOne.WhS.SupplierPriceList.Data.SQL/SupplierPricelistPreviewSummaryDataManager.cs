using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class SupplierPricelistPreviewSummaryDataManager : BaseTOneDataManager, ISupplierPricelistPreviewSummaryDataManager
    {
        public SupplierPricelistPreviewSummaryDataManager()
            : base(GetConnectionStringName("TOneWhS_SPL_DBConnStringKey", "TOneWhS_SPL_DBConnString"))
        {

        }

        public PreviewSummary GetSupplierPricelistPreviewSummary(int processInstanceId)
        {
            PreviewSummary supplierPricelistPreviewSummary = new PreviewSummary();

            supplierPricelistPreviewSummary.NumberOfNewRates = Convert.ToInt32(ExecuteScalarSP("TOneWhS_SPL.sp_SupplierZoneRate_Preview_GetNumberOfRatesWithChangeType", processInstanceId, RateChangeType.New));
            supplierPricelistPreviewSummary.NumberOfIncreasedRates = Convert.ToInt32(ExecuteScalarSP("TOneWhS_SPL.sp_SupplierZoneRate_Preview_GetNumberOfRatesWithChangeType", processInstanceId, RateChangeType.Increase));
            supplierPricelistPreviewSummary.NumberOfDecreasedRates = Convert.ToInt32(ExecuteScalarSP("TOneWhS_SPL.sp_SupplierZoneRate_Preview_GetNumberOfRatesWithChangeType", processInstanceId, RateChangeType.Decrease));

            supplierPricelistPreviewSummary.NumberOfNewOtherRates = Convert.ToInt32(ExecuteScalarSP("TOneWhS_SPL.sp_SupplierZoneRate_Preview_GetNumberOfOtherRatesWithChangeType", processInstanceId, RateChangeType.New));
            supplierPricelistPreviewSummary.NumberOfIncreasedOtherRates = Convert.ToInt32(ExecuteScalarSP("TOneWhS_SPL.sp_SupplierZoneRate_Preview_GetNumberOfOtherRatesWithChangeType", processInstanceId, RateChangeType.Increase));
            supplierPricelistPreviewSummary.NumberOfDecreasedOtherRates = Convert.ToInt32(ExecuteScalarSP("TOneWhS_SPL.sp_SupplierZoneRate_Preview_GetNumberOfOtherRatesWithChangeType", processInstanceId, RateChangeType.Decrease));

            supplierPricelistPreviewSummary.NumberOfNewZones = Convert.ToInt32(ExecuteScalarSP("TOneWhS_SPL.sp_SupplierZoneRate_Preview_GetNumberOfZonesWithChangeType", processInstanceId, ZoneChangeType.New));
            supplierPricelistPreviewSummary.NumberOfClosedZones = Convert.ToInt32(ExecuteScalarSP("TOneWhS_SPL.sp_SupplierZoneRate_Preview_GetNumberOfZonesWithChangeType", processInstanceId, ZoneChangeType.Deleted));
            supplierPricelistPreviewSummary.NumberOfRenamedZones = Convert.ToInt32(ExecuteScalarSP("TOneWhS_SPL.sp_SupplierZoneRate_Preview_GetNumberOfZonesWithChangeType", processInstanceId, ZoneChangeType.Renamed));

            supplierPricelistPreviewSummary.NumberOfNewCodes = Convert.ToInt32(ExecuteScalarSP("TOneWhS_SPL.sp_SupplierCode_Preview_GetNumberOfCodesWithChangeType", processInstanceId,CodeChangeType.New ));
            supplierPricelistPreviewSummary.NumberOfClosedCodes = Convert.ToInt32(ExecuteScalarSP("TOneWhS_SPL.sp_SupplierCode_Preview_GetNumberOfCodesWithChangeType", processInstanceId,CodeChangeType.Deleted));
            supplierPricelistPreviewSummary.NumberOfMovedCodes = Convert.ToInt32(ExecuteScalarSP("TOneWhS_SPL.sp_SupplierCode_Preview_GetNumberOfCodesWithChangeType", processInstanceId,CodeChangeType.Moved));

            supplierPricelistPreviewSummary.NumberOfZonesWithChangedServices = Convert.ToInt32(ExecuteScalarSP("TOneWhS_SPL.sp_SupplierZoneRate_Preview_GetNumberOfZonesWithChangedSe", processInstanceId));

            supplierPricelistPreviewSummary.NumberOfExcludedCountries = Convert.ToInt32(ExecuteScalarSP("[TOneWhS_SPL].[sp_SupplierZoneRate_Preview_GetNumberOfExcludedCountries]"));

            return supplierPricelistPreviewSummary;

           
        }
    }
}
