using Demo.Module.Business;
using Demo.Module.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_Company")]
    [JSONWithTypeAttribute]
    public class Demo_Module_CompanyController : BaseAPIController
    {
        CompanyManager _companyManager = new CompanyManager();

        [HttpPost]
        [Route("GetFilteredCompanies")]
        public object GetFilteredCompanies(DataRetrievalInput<CompanyQuery> input)
        {
            return GetWebResponse(input, _companyManager.GetFilteredCompanies(input));
        }

        [HttpGet]
        [Route("GetCompanyById")]
        public Company GetCompanyById(int companyId)
        {
            return _companyManager.GetCompanyById(companyId);
        }

        [HttpGet]
        [Route("GetCompaniesInfo")]
        public IEnumerable<CompanyInfo> GetCompaniesInfo(string filter = null)
        {
            CompanyInfoFilter companyInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<CompanyInfoFilter>(filter) : null;
            return _companyManager.GetCompaniesInfo(companyInfoFilter);
        }

        [HttpPost]
        [Route("AddCompany")]
        public InsertOperationOutput<CompanyDetail> AddCompany(Company company)
        {
            return _companyManager.AddCompany(company);
        }

        [HttpPost]
        [Route("UpdateCompany")]
        public UpdateOperationOutput<CompanyDetail> UpdateCompany(Company company)
        {
            return _companyManager.UpdateCompany(company);
        }

    }
}