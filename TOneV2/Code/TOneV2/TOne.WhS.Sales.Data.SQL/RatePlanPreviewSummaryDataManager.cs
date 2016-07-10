using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public RatePlanPreviewSummary GetRatePlanPreviewSummary(RatePlanPreviewQuery query)
        {
            return GetItemSP("TOneWhS_Sales.RP_RatePlanPreviewSummary_Get", RatePlanPreviewSummaryMapper, query.ProcessInstanceId);
        }

        public void ApplyRatePlanPreviewSummaryToDB(RatePlanPreviewSummary summary)
        {
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
                summary.NumberOfClosedSaleZoneRoutingProducts
            );
        }
        
        #endregion

        #region Mappers

        private RatePlanPreviewSummary RatePlanPreviewSummaryMapper(IDataReader reader)
        {
            return new RatePlanPreviewSummary()
            {
                NumberOfNewRates = (int)reader["NumberOfNewRates"],
                NumberOfIncreasedRates = (int)reader["NumberOfIncreasedRates"],
                NumberOfDecreasedRates = (int)reader["NumberOfDecreasedRates"],
                NumberOfClosedRates = (int)reader["NumberOfClosedRates"],
                NameOfNewDefaultRoutingProduct = reader["NameOfNewDefaultRoutingProduct"] as string,
                NameOfClosedDefaultRoutingProduct = reader["NameOfClosedDefaultRoutingProduct"] as string,
                NumberOfNewSaleZoneRoutingProducts = (int)reader["NumberOfNewSaleZoneRoutingProducts"],
                NumberOfClosedSaleZoneRoutingProducts = (int)reader["NumberOfClosedSaleZoneRoutingProducts"]
            };
        }
        
        #endregion
    }
}
