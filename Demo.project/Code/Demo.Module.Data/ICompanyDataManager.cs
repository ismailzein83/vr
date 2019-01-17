using Demo.Module.Entities;
using System.Collections.Generic;

namespace Demo.Module.Data
{
    public interface ICompanyDataManager:IDataManager
    {
        List<Company> GetCompanies();

        bool Insert(Company company, out int insertedId);

        bool Update(Company company);

        bool AreCompaniesUpdated(ref object updateHandle);
    }
}