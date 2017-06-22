using System;
using System.Collections.Generic;
using System.Data;
using Retail.EntitiesMigrator.Entities;
using Vanrise.Data.SQL;

namespace Retail.EntitiesMigrator.Data.SQL
{
    public class IncomingRatesDataManager : BaseSQLDataManager, IIncomingRatesDataManager
    {

        #region Constructor
        public IncomingRatesDataManager()
            : base(GetConnectionStringName("RatesDBConnStringKey", "RatesDBConnString"))
        {

        }

        #endregion

        public IEnumerable<IncomingRate> GetIncomingRates()
        {
            return GetItemsText(query_IncomingRates, IncomingRateMapper, null);
        }

        IncomingRate IncomingRateMapper(IDataReader reader)
        {
            return new IncomingRate
            {
                SubscriberId = (long)reader["SUB_SUBSCRIBERID"],
                LocalRate = ParseRateDetails(reader["SSICR_LOCALRATES"] as string),
                InternationalRate = ParseRateDetails(reader["SSICR_NWDRATES"] as string),
                ActivationDate = GetReaderValue<DateTime>(reader, "SSICR_ACTIVATIONDATE")

            };
        }

        private RateDetails ParseRateDetails(string rateString)
        {
            string[] rateStringDetails = rateString.Split('*');
            return new RateDetails
            {
                FractionUnit = int.Parse(rateStringDetails[1]),
                Rate = decimal.Parse(rateStringDetails[2])
            };
        }

        const string query_IncomingRates = @"SELECT    [SS_SUBSRVID]
                                                      ,[SSICR_LOCALRATES]
                                                      ,[SSICR_NWDRATES]
                                                      ,[SUB_SUBSCRIBERID]
                                                      ,[SSICR_ACTIVATIONDATE]
                                                  FROM [IncomingRates]";
    }
}
