﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Data.SQL
{
    public class BillingReportDataManager : BaseSQLDataManager, IBillingReportDataManager
    {
                
        #region Constructors

        public BillingReportDataManager()
            : base(GetConnectionStringName("TOneWhS_Analytics_DBConnStringKey", "TOneAnalyticsDBConnString"))
        {

        }
        
        #endregion

        public List<Entities.BillingReport.BusinessCaseStatus> GetBusinessCaseStatus(DateTime fromDate, DateTime toDate, int carrierAccountId, int topDestination, bool isSale, bool isAmount, int currencyId)
        {
            string DurationField = "BS.SaleDurationInSeconds ";
            string amountField = isSale ? "BS.SaleNet / ISNULL(ERS.Rate, 1) " : "BS.CostNet / ISNULL(ERC.Rate, 1) ";
            string amountDuration = isAmount ? amountField : DurationField;
            string carrierId = isSale ? "BS.CustomerId" : "BS.SupplierId";


            string query = String.Format(@"{2}
                SELECT TOP (@TopDestination)  BS.SaleZoneId AS ZoneId ,Year(BS.BatchStart) AS YearDuration, 
                MONTH(BS.BatchStart) AS MonthDuration, 
                cast( (SUM({0} )/60 ) as decimal(13,4) ) AS SaleDuration 
                From [TOneWhS_Analytics].[BillingStatsDaily] BS WITH(NOLOCK,INDEX(IX_BillingStatsDaily_BatchStart,IX_BillingStatsDaily_Id)),
                    @ConvertedExchangeRates as ERC, @ConvertedExchangeRates as ERS
                WHERE BS.BatchStart BETWEEN @FromDate AND @ToDate AND {1} = @CustomerId 
                And ERC.CurrencyID = BS.CostCurrencyId AND BS.BatchStart >= ERC.BED AND (ERC.EED IS NULL OR BS.BatchStart < ERC.EED) 
                And ERS.CurrencyID = BS.SaleCurrencyId AND BS.BatchStart >= ERS.BED AND (ERS.EED IS NULL OR BS.BatchStart < ERS.EED)
                GROUP BY BS.SaleZoneId , Year(BS.BatchStart), MONTH(BS.BatchStart)",
                amountDuration,
                carrierId,
                GetExchangeRatesTable(currencyId)
                );

            return GetItemsText(query, BusinessCaseStatusMapper,
            (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@TopDestination", topDestination));
                cmd.Parameters.Add(new SqlParameter("@FromDate", new DateTime(fromDate.Year, fromDate.Month, fromDate.Day)));
                cmd.Parameters.Add(new SqlParameter("@ToDate", new DateTime(toDate.Year, toDate.Month, toDate.Day)));
                cmd.Parameters.Add(new SqlParameter("@CustomerId", carrierAccountId));
            });
        }

        string GetExchangeRatesTable(int? currencyId)
        {
            if (currencyId.HasValue)
                return String.Format
                (
                    @"DECLARE @ConvertedExchangeRates TABLE
                    (
	                    CurrencyID INT NOT NULL,
	                    Rate DECIMAL(18, 6) NOT NULL,
	                    BED DATETIME NOT NULL,
	                    EED DATETIME,
	                    PRIMARY KEY(CurrencyID, BED)
                    )

                    INSERT INTO @ConvertedExchangeRates
                    SELECT * FROM Common.getExchangeRatesConvertedToCurrency({0}, @FromDate, @ToDate)",
                    currencyId
                );
            return null;
        }

        
        #region PrivatMethods
        private BusinessCaseStatus BusinessCaseStatusMapper(IDataReader reader)
        {
            BusinessCaseStatus instance = new BusinessCaseStatus
            {
                ZoneId = GetReaderValue<long>(reader, "ZoneId"),
                Month = GetReaderValue<int>(reader, "MonthDuration"),
                Year = GetReaderValue<int>(reader, "YearDuration"),
                Durations = GetReaderValue<decimal>(reader, "SaleDuration")
            };
            return instance;
        }
        #endregion
    }
}
