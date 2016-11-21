using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data.SQL
{
    public class SaleCountryPreviewDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISaleCountryPreviewDataManager
    {

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;

        public SaleCountryPreviewDataManager()
            : base(GetConnectionStringName("NumberingPlanDBConnStringKey", "NumberingPlanDBConnString"))
        {

        }

        public IEnumerable<CountryPreview> GetFilteredCountryPreview(SPLPreviewQuery query)
        {
            return GetItemsSP("[VR_NumberingPlan].[sp_SaleCountry_Preview_GetFiltered]", CountryPreviewMapper, query.ProcessInstanceId, query.OnlyModified);
        }

        private CountryPreview CountryPreviewMapper(IDataReader reader)
        {
            CountryPreview countryPreview = new CountryPreview
            {
                CountryId = (int)reader["CountryId"],
                NewZones = (int)reader["NewZones"],
                DeletedZones = (int)reader["DeletedZones"],
                RenamedZones = (int)reader["RenamedZones"],
                NewCodes = (int)reader["NewCodes"],
                MovedCodes = (int)reader["MovedCodes"],
                DeletedCodes = (int)reader["DeletedCodes"]
            };
            return countryPreview;
        }


    }
}
