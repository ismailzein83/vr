using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class CompanyManager
    {
        #region Public Methods

        public IDataRetrievalResult<CompanyDetail> GetFilteredCompanies(DataRetrievalInput<CompanyQuery> input)
        {
            var allCompanies = GetCachedCompanies(); // get all the companies
            Func<Company, bool> filterExpression = (company) =>
            {
                if (input.Query.Name != null && !company.Name.ToLower().Contains(input.Query.Name.ToLower())) // filter name does not match the company' name
                    return false;

                return true;
            }; // apply an if on each company
            return DataRetrievalManager.Instance.ProcessResult(input, allCompanies.ToBigResult(input, filterExpression, CompanyDetailMapper)); // apply the filter on all the companies and convert the output to CompanyDetail
        }

        public Company GetCompanyById(int companyId)
        {
            return GetCachedCompanies().GetRecord(companyId); // return the entity that has this id
        }

        public string GetCompanyName(int companyId)
        {
            var company = GetCompanyById(companyId);
            return company != null ? company.Name : null;
        }

        public IEnumerable<CompanyInfo> GetCompaniesInfo(CompanyInfoFilter companyInfoFilter)
        {
            var allCompanies = GetCachedCompanies();
            Func<Company, bool> filterFunc = (company) =>
            {
                if (companyInfoFilter != null)
                {
                    if (companyInfoFilter.Filters != null)
                    {
                        var context = new CompanyInfoFilterContext { CompanyId = company.CompanyId };  

                        foreach (var filter in companyInfoFilter.Filters)
                        {
                            if (!filter.IsMatch(context)) // if the context does not match the id return false
                                return false;
                        }
                    }
                }
                return true; 
            };
            return allCompanies.MapRecords(CompanyInfoMapper, filterFunc).OrderBy(company => company.Name); // apply the filter and convert each company to CompanyInfo then order them by name
        }

        public InsertOperationOutput<CompanyDetail> AddCompany(Company company)
        {
            InsertOperationOutput<CompanyDetail> insertOperationOutput = new InsertOperationOutput<CompanyDetail>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int companyId = -1;

            ICompanyDataManager companyDataManager = DemoModuleFactory.GetDataManager<ICompanyDataManager>();
            bool insertActionSuccess = companyDataManager.Insert(company, out companyId);
            if (insertActionSuccess)
            {
                company.CompanyId = companyId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = this.CompanyDetailMapper(company);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }

        public UpdateOperationOutput<CompanyDetail> UpdateCompany(Company company)
        {
            UpdateOperationOutput<CompanyDetail> updateOperationOutput = new UpdateOperationOutput<CompanyDetail>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ICompanyDataManager companyDataManager = DemoModuleFactory.GetDataManager<ICompanyDataManager>();
            bool updatedActionSuccess = companyDataManager.Update(company);
            if (updatedActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = this.CompanyDetailMapper(company);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        #endregion

        #region Private Methods

        private Dictionary<int, Company> GetCachedCompanies()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCompanies", () =>
            {
                ICompanyDataManager companyDataManager = DemoModuleFactory.GetDataManager<ICompanyDataManager>();
                List<Company> companies = companyDataManager.GetCompanies(); // go to the first class that implement this interface
                return companies.ToDictionary(company => company.CompanyId, company => company); // transform to dictionary
            });
        }

        #endregion

        #region Private Classes

        private class CacheManager: Vanrise.Caching.BaseCacheManager
        {
            ICompanyDataManager companyDataManager = DemoModuleFactory.GetDataManager<ICompanyDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return companyDataManager.AreCompaniesUpdated(ref _updateHandle);
            }

        }

        # endregion

        #region Mappers

        private CompanyDetail CompanyDetailMapper(Company company)
        {
            CompanyDetail companyDetail = new CompanyDetail();
            companyDetail.CompanyId = company.CompanyId;
            companyDetail.Name = company.Name;
            companyDetail.Settings = company.Settings;
            return companyDetail;
        }

        private CompanyInfo CompanyInfoMapper(Company company)
        {
            CompanyInfo companyInfo = new CompanyInfo();
            companyInfo.CompanyId = company.CompanyId;
            companyInfo.Name = company.Name;
            return companyInfo;
        }

        #endregion
    }
}
