using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using System.Data;
using Demo.Module.Entities;
using Newtonsoft.Json;
using Demo.Module.Entities.Company;

namespace Demo.Module.Data.SQL
{
    public class CompanyDataManager : BaseSQLDataManager, ICompanyDataManager
    {
        #region Constructors
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
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Company_Insert]", out id, company.Name);
            insertedId = Convert.ToInt32(id); 
            return (nbOfRecordsAffected > 0);
        }

        public bool Update(Company company)
        {

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Company_Update]", company.CompanyId, company.Name);
            return (nbOfRecordsAffected > 0);
        }
        public bool AreCompaniesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Company]", ref updateHandle);
        }
        #endregion

        #region Mappers
        Company CompanyMapper(IDataReader reader)
        {
            Company company = new Company();
            company.CompanyId = GetReaderValue<int>(reader, "ID");
            company.Name = GetReaderValue<string>(reader, "Name");
            return company;
        }
        #endregion
    }
}
