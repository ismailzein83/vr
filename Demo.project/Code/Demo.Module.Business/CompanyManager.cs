using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;
using Demo.Module.Data;
using Vanrise.Entities;
using Vanrise.Common;
using Demo.Module.Entities.Company;
using Vanrise.Common.Business;


namespace Demo.Module.Business
{
    public class CompanyManager
    {
        #region Public Methods
        public InsertOperationOutput<CompanyDetails> AddCompany(Company company)
        {
            ICompanyDataManager companyDataManager = DemoModuleFactory.GetDataManager<ICompanyDataManager>();
            InsertOperationOutput<CompanyDetails> insertOperationOutput = new InsertOperationOutput<CompanyDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int companyId = -1;

            bool insertActionSuccess = companyDataManager.Insert(company, out companyId);
            if (insertActionSuccess)
            {
                company.CompanyId = companyId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = CompanyDetailMapper(company);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }



        public IDataRetrievalResult<CompanyDetails> GetFilteredCompanies(DataRetrievalInput<CompanyQuery> input)
        {
            var allCompanies = GetAllCompanies();
            Func<Company, bool> filterExpression = (company) =>
                {
                    if (input.Query.Name != null && !company.Name.ToLower().Contains(input.Query.Name.ToLower()))
                        return false;
                    return true;
                };
            return DataRetrievalManager.Instance.ProcessResult(input, allCompanies.ToBigResult(input, filterExpression, CompanyDetailMapper));

        }

        public Company GetCompanyById(int companyId)
        {
            var allCompanies = GetAllCompanies();
            return allCompanies.GetRecord(companyId);

        }

        public UpdateOperationOutput<CompanyDetails> UpdateCompany(Company company)
        {
            ICompanyDataManager companyDataManager = DemoModuleFactory.GetDataManager<ICompanyDataManager>();
            UpdateOperationOutput<CompanyDetails> updateOperationOutput = new UpdateOperationOutput<CompanyDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = companyDataManager.Update(company);
            if (updateActionSuccess)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CompanyDetailMapper(company);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public IEnumerable<CompanyDetails> GetCompaniesInfo()
        {
            var allCompanies = GetAllCompanies();
            Func<Company, bool> filterFunc = (company) =>
            {
                return true;
            };
            IEnumerable<Company> filteredCompanies = (filterFunc != null) ? allCompanies.FindAllRecords(filterFunc) : allCompanies.MapRecords(u => u.Value);
            return filteredCompanies.MapRecords(CompanyDetailMapper).OrderBy(company => company.Name);
        }
        #endregion

        #region Private Methods
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICompanyDataManager companyDataManager = DemoModuleFactory.GetDataManager<ICompanyDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return companyDataManager.AreCompaniesUpdated(ref _updateHandle);
            }
        }
        private Dictionary<int, Company> GetAllCompanies()
        {
             return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
                .GetOrCreateObject("GetCacheCompanies", () =>
                {
                    ICompanyDataManager companyDataManager = DemoModuleFactory.GetDataManager<ICompanyDataManager>();
                    List<Company> companies = companyDataManager.GetCompanies();
                    return companies.ToDictionary(company => company.CompanyId, company => company);
                });
        }
        #endregion

        #region Mappers
        public CompanyDetails CompanyDetailMapper(Company company)
        {
            CompanyDetails companyDetails = new CompanyDetails();
            companyDetails.CompanyId = company.CompanyId;
            companyDetails.Name = company.Name;
            return companyDetails;
        }
        #endregion 
    }
}

