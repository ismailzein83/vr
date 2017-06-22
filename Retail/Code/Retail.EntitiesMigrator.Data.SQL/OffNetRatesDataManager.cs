using Retail.EntitiesMigrator.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.EntitiesMigrator.Data.SQL
{
    public class OffNetRatesDataManager : BaseSQLDataManager
    { 
        #region Constructor
        public OffNetRatesDataManager()
            : base(GetConnectionStringName("RatesDBConnStringKey", "RatesDBConnString"))
        {

        }
        public List<OffNetRate> GetOffNetRates()
        {
            return GetItemsText(@"SELECT distinct [AC_ACCOUNTNO]
      ,[DES_DESTINATIONNAME]
      ,[DR_RATECONF]
  FROM [dbo].[vw_Multinet_OffNet_Rates]", (reader) =>
            {
                return new OffNetRate
                {
                    SourceBranchId = reader["AC_ACCOUNTNO"].ToString(),
                    OperatorName = reader["DES_DESTINATIONNAME"] as string,
                    RateDetail = DataHelper.ParseRateDetails(reader["DR_RATECONF"] as string)
                };
            }, null);
        }
        #endregion
    }

    internal class DataHelper
    {
        public static RateDetails ParseRateDetails(string rateString)
        {
            string[] rateStringDetails = rateString.Split('*');
            return new RateDetails
            {
                FractionUnit = int.Parse(rateStringDetails[1]),
                Rate = decimal.Parse(rateStringDetails[2])
            };
        }
    }

    
}