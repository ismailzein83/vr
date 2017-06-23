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
            return GetItemsText(query_getInetrnationalRates, InternationalRateMapper, null);
        }

        InternationalRate InternationalRateMapper(IDataReader reader)
        {
            return new InternationalRate
            {
                InternationalRateDetail = DataHelper.ParseRateDetails(reader["DR_RATECONF"] as string),
                ZoneName = reader["DES_DESTINATIONNAME"] as string,
                ActivationDate = new DateTime(2010, 01, 01)
            };
        }

        const string query_getInetrnationalRates = @"SELECT     distinct [DES_DESTINATIONNAME]
                                                                    ,[DR_RATECONF]
                                                    FROM        [Multinet_Intl_Rates]";
    }
}
