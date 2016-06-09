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

        public Vanrise.Entities.BigResult<Entities.CountryPreview> GetCountryPreviewFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SPLPreviewQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {

                ExecuteNonQuerySP("[TOneWhS_SPL].[sp_SupplierCountry_Preview_CreateTempByFiltered]", tempTableName, input.Query.ProcessInstanceId, input.Query.OnlyModified);
            };
            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, createTempTableAction, CountryPreviewMapper);
        }


        private CountryPreview CountryPreviewMapper(IDataReader reader)
        {
            CountryPreview countryPreview = new CountryPreview
            {
                ProcessInstanceId = (long)reader["ProcessInstanceID"],
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
