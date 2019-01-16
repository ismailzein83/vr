using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.Data.RDB
{
    public class SaleCountryPreviewDataManager : ISaleCountryPreviewDataManager
    {
        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("WhS_CodePrep", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion

        #region Mappers
        private CountryPreview CountryPreviewMapper(IRDBDataReader reader)
        {
            CountryPreview countryPreview = new CountryPreview
            {
                CountryId = reader.GetInt("CountryId"),
                NewZones = reader.GetInt("NewZones"),
                DeletedZones = reader.GetInt("DeletedZones"),
                RenamedZones = reader.GetInt("RenamedZones"),
                NewCodes = reader.GetInt("NewCodes"),
                MovedCodes = reader.GetInt("MovedCodes"),
                DeletedCodes = reader.GetInt("DeletedCodes")
            };
            return countryPreview;
        }
        #endregion

        #region ISaleCountryPreviewDataManager
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;
        public IEnumerable<CountryPreview> GetFilteredCountryPreview(SPLPreviewQuery query)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var saleZonePreviewDataManager = new SaleZonePreviewDataManager();
            saleZonePreviewDataManager.BuildSelectQuery(queryContext, query);
            return queryContext.GetItems(CountryPreviewMapper);
        }
        #endregion

    }
}
