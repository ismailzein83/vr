using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace Demo.Module.Data
{
    public interface ICompanyDataManager : IDataManager
    {
        List<Company> GetCompanies();       
        bool Insert(Company company, out int insertedId);
        bool Update(Company company);
        bool AreCompaniesUpdated(ref object updateHandle);

    }
}
