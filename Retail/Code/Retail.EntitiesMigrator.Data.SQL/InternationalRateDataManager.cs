using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.EntitiesMigrator.Entities;
using Vanrise.Data.SQL;

namespace Retail.EntitiesMigrator.Data.SQL
{
    public class InternationalRateDataManager : BaseSQLDataManager, IInternationalRateDataManager
    {
        #region Constructor
        public InternationalRateDataManager()
            : base(GetConnectionStringName("RatesDBConnStringKey", "RatesDBConnString"))
        {

        }

        #endregion
        public IEnumerable<InternationalRate> GetInternationalRates()
        {
            return GetItemsText(query_getInetrnationalRates2, InternationalRateMapper2, null);
        }

        

        InternationalRate InternationalRateMapper2(IDataReader reader)
        {
            return new InternationalRate
            {
                InternationalRateDetail = new RateDetails
                {
                    //FractionUnit = (int)reader["Tariff"],
                    Rate = (decimal)reader["Rate"]
                },
                ZoneName = reader["Zone"] as string,
                BED = (DateTime)reader["BED"],
                ActivationDate = new DateTime(2010, 01, 01)
            };
        }

      
        const string query_getInetrnationalRates2 = @"SELECT    [Zone]
                                                                --,[Tariff]
                                                                ,[Rate]
                                                                ,BED
                                                      FROM      [MultiNet_International_Rates]";
    }
}
