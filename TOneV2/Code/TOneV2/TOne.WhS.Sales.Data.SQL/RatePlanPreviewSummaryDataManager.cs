using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class RatePlanPreviewSummaryDataManager : BaseSQLDataManager, IRatePlanPreviewSummaryDataManager
    {
        #region Fields / Properties

        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set { _processInstanceId = value; }
        }

        #endregion

        #region Constructors

        public RatePlanPreviewSummaryDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion
        
        #region Public Methods

        public RatePlanPreviewSummary GetCustomerRatePlanPreviewSummary(RatePlanPreviewQuery query)
        {
            string strcustomerIds = null;
            if (query.CustomerIds != null && query.CustomerIds.Any())
                strcustomerIds = string.Join(",", query.CustomerIds);
            return GetItemSP("TOneWhS_Sales.RP_RatePlanPreviewSummary_GetCustomerRatePlanPreviewSummary", CustomerRatePlanPreviewSummaryMapper, query.ProcessInstanceId, strcustomerIds);
        }
        public RatePlanPreviewSummary GetProductRatePlanPreviewSummary(RatePlanPreviewQuery query)
        {
            return GetItemSP("TOneWhS_Sales.RP_RatePlanPreviewSummary_GetProductRatePlanPreviewSummary", ProductRatePlanPreviewSummaryMapper, query.ProcessInstanceId);
        }
        public void ApplyRatePlanPreviewSummaryToDB(RatePlanPreviewSummary summary)
        {
            string newDefaultServices = summary.NewDefaultServices != null ? Vanrise.Common.Serializer.Serialize(summary.NewDefaultServices) : null;

            ExecuteNonQuerySP
            (
                "TOneWhS_Sales.sp_RatePlanPreviewSummary_Insert",
                _processInstanceId,
                summary.NumberOfNewRates,
                summary.NumberOfIncreasedRates,
                summary.NumberOfDecreasedRates,
                summary.NumberOfClosedRates,
                summary.NameOfNewDefaultRoutingProduct,
                summary.NameOfClosedDefaultRoutingProduct,
                summary.NumberOfNewSaleZoneRoutingProducts,
                summary.NumberOfClosedSaleZoneRoutingProducts,
                newDefaultServices,
                summary.ClosedDefaultServiceEffectiveOn,
                summary.NumberOfNewSaleZoneServices,
                summary.NumberOfClosedSaleZoneServices,
				summary.NumberOfChangedCountries,
                summary.NumberOfNewCountries,
                summary.NumberOfNewOtherRates,
                summary.NumberOfIncreasedOtherRates,
                summary.NumberOfDecreasedOtherRates
            );
        }
        
        #endregion

        #region Mappers

        private RatePlanPreviewSummary CustomerRatePlanPreviewSummaryMapper(IDataReader reader)
        {
            RatePlanPreviewSummary ratePlanPreviewSummary = new RatePlanPreviewSummary();
            string newDefaultServices = reader["NewDefaultServices"] as string;
            while (reader.Read())
            {
                ratePlanPreviewSummary.NewDefaultServices = newDefaultServices != null ? Vanrise.Common.Serializer.Deserialize<List<ZoneService>>(newDefaultServices) : null;
                ratePlanPreviewSummary.ClosedDefaultServiceEffectiveOn = GetReaderValue<DateTime?>(reader, "ClosedDefaultServiceEffectiveOn");
                ratePlanPreviewSummary.NumberOfNewSaleZoneServices = (int)reader["NumberOfNewSaleZoneServices"];
                ratePlanPreviewSummary.NumberOfClosedSaleZoneServices = (int)reader["NumberOfClosedSaleZoneServices"];
                ratePlanPreviewSummary.NumberOfChangedCountries = (int)reader["NumberOfChangedCountries"];
                ratePlanPreviewSummary.NumberOfNewCountries = GetReaderValue<int>(reader, "NumberOfNewCountries");
            }

            if (reader.NextResult())
                while (reader.Read())
                {
                    ratePlanPreviewSummary.NumberOfNewRates = (int)reader["NumberOfNewRates"];
                }
            if (reader.NextResult())
                while (reader.Read())
                {
                    ratePlanPreviewSummary.NumberOfIncreasedRates = (int)reader["NumberOfIncreasedRates"];
                }
            if (reader.NextResult())
                while (reader.Read())
                {
                    ratePlanPreviewSummary.NumberOfDecreasedRates = (int)reader["NumberOfDecreasedRates"];
                }
            if (reader.NextResult())
                while (reader.Read())
                {
                    ratePlanPreviewSummary.NumberOfNewSaleZoneRoutingProducts = (int)reader["NumberOfNewSaleZoneRoutingProducts"];
                }
            if (reader.NextResult())
                while (reader.Read())
                {
                    ratePlanPreviewSummary.NumberOfClosedSaleZoneRoutingProducts = (int)reader["NumberOfClosedSaleZoneRoutingProducts"];
                }
            if (reader.NextResult())
                while (reader.Read())
                {
                    ratePlanPreviewSummary.NumberOfNewOtherRates = (int)reader["NumberOfNewOtherRates"];
                }
            if (reader.NextResult())
                while (reader.Read())
                {
                    ratePlanPreviewSummary.NumberOfIncreasedOtherRates = (int)reader["NumberOfIncreasedOtherRates"];
                }
            if (reader.NextResult())
                while (reader.Read())
                {
                    ratePlanPreviewSummary.NumberOfDecreasedOtherRates = (int)reader["NumberOfDecreasedOtherRates"];
                }
            return ratePlanPreviewSummary;
        }


        private RatePlanPreviewSummary ProductRatePlanPreviewSummaryMapper(IDataReader reader)
        {
            string newDefaultServices = reader["NewDefaultServices"] as string;

            return new RatePlanPreviewSummary()
            {
                NumberOfNewRates = (int)reader["NumberOfNewRates"],
                NumberOfIncreasedRates = (int)reader["NumberOfIncreasedRates"],
                NumberOfDecreasedRates = (int)reader["NumberOfDecreasedRates"],
                NameOfNewDefaultRoutingProduct = reader["NameOfNewDefaultRoutingProduct"] as string,
                NameOfClosedDefaultRoutingProduct = reader["NameOfClosedDefaultRoutingProduct"] as string,
                NumberOfNewSaleZoneRoutingProducts = (int)reader["NumberOfNewSaleZoneRoutingProducts"],
                NumberOfClosedSaleZoneRoutingProducts = (int)reader["NumberOfClosedSaleZoneRoutingProducts"],
                NewDefaultServices = newDefaultServices != null ? Vanrise.Common.Serializer.Deserialize<List<ZoneService>>(newDefaultServices) : null,
                ClosedDefaultServiceEffectiveOn = GetReaderValue<DateTime?>(reader, "ClosedDefaultServiceEffectiveOn"),
                NumberOfNewSaleZoneServices = (int)reader["NumberOfNewSaleZoneServices"],
                NumberOfClosedSaleZoneServices = (int)reader["NumberOfClosedSaleZoneServices"],
                NumberOfChangedCountries = (int)reader["NumberOfChangedCountries"],
                NumberOfNewCountries = GetReaderValue<int>(reader, "NumberOfNewCountries"),
                NumberOfNewOtherRates = (int)reader["NumberOfNewOtherRates"],
                NumberOfIncreasedOtherRates = (int)reader["NumberOfIncreasedOtherRates"],
                NumberOfDecreasedOtherRates = (int)reader["NumberOfDecreasedOtherRates"]
            };
        }
        

        #endregion
    }
}
