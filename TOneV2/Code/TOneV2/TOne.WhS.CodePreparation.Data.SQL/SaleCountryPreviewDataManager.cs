using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class SaleCountryPreviewDataManager : BaseTOneDataManager, ISaleCountryPreviewDataManager
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
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_SPL_DBConnString"))
        {

        }

        public IEnumerable<CountryPreview> GetFilteredCountryPreview(SPLPreviewQuery query)
        {
            return GetItemsSP("[TOneWhS_CP].[sp_SaleCountry_Preview_GetFiltered]",CountryPreviewMapper, query.ProcessInstanceId, query.OnlyModified);
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
                DeletedCodes = (int)reader["DeletedCodes"],

            };
            return countryPreview;
        }


    }
}
