using Demo.Module.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class CompanyDataManager : BaseSQLDataManager, ICompanyDataManager
    {
        #region Properties/Ctor

        public CompanyDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        #endregion

        #region Public Methods

        public List<Company> GetCompanies()
        {
            return GetItemsSP("[dbo].[sp_Company_GetAll]", CompanyMapper);
        }

        public bool Insert(Company company, out int insertedId)
        {
            string serializedCompanySettings = company.Settings != null ? Vanrise.Common.Serializer.Serialize(company.Settings) : null;

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Company_Insert]", out object id, company.Name, serializedCompanySettings);

            bool result = nbOfRecordsAffected > 0;
            if (result)
                insertedId = (int)id;
            else
                insertedId = 0;

            return result;
        }

        public bool Update(Company company)
        {
            string serializedCountrySettings = company.Settings != null ? Vanrise.Common.Serializer.Serialize(company.Settings) : null;

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Company_Update]", company.CompanyId, company.Name, serializedCountrySettings);
            return nbOfRecordsAffected > 0;
        }

        public bool AreCompaniesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Company]", ref updateHandle);
        }

        #endregion

        #region Mappers

        private Company CompanyMapper(IDataReader reader)
        {
            return new Company
            {
                CompanyId = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                Settings = Vanrise.Common.Serializer.Deserialize<CompanySettings>(reader["Settings"] as string)
            };
        }

        #endregion
    }
}