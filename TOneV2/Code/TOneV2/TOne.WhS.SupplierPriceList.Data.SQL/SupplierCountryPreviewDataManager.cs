using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class SupplierCountryPreviewDataManager : BaseTOneDataManager, ISupplierCountryPreviewDataManager
    {

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;

        public SupplierCountryPreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_SPL_DBConnStringKey", "TOneWhS_SPL_DBConnString"))
        {

        }

        public IEnumerable<CountryPreview> GetFilteredCountryPreview(SPLPreviewQuery query)
        {
            return GetItemsSP("[TOneWhS_SPL].[sp_SupplierCountry_Preview_GetFiltered]", CountryPreviewMapper, query.ProcessInstanceId, query.OnlyModified,query.IsExcluded);
        }

        private CountryPreview CountryPreviewMapper(IDataReader reader)
        {
            CountryPreview countryPreview = new CountryPreview
            {
                CountryId = GetReaderValue<int>(reader, "CountryId"),
                NewZones = GetReaderValue<int>(reader, "NewZones"),
                DeletedZones = GetReaderValue<int>(reader, "DeletedZones"),
                RenamedZones = GetReaderValue<int>(reader, "RenamedZones"),
                NewCodes = GetReaderValue<int>(reader, "NewCodes"),
                MovedCodes = GetReaderValue<int>(reader, "MovedCodes"),
                DeletedCodes = GetReaderValue<int>(reader, "DeletedCodes"),

            };
            return countryPreview;
        }


    }
}
