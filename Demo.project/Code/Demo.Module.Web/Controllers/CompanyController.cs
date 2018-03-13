using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Entities.Company;
using Demo.Module.Business;
using Vanrise.Entities;


namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Company")]
    [JSONWithTypeAttribute]
    public class Demo_Module_CompanyController : BaseAPIController
    {
       
        CompanyManager companyManager = new CompanyManager();

        [HttpPost]
        [Route("GetFilteredCompanies")]
        public object GetFilteredCompanies(DataRetrievalInput<CompanyQuery> input)
        {
            return GetWebResponse(input, companyManager.GetFilteredCompanies(input));
        }

        [HttpPost]
        [Route("AddCompany")]
        public InsertOperationOutput<CompanyDetails> AddCompany(Company company)
        {
            return companyManager.AddCompany(company);
        }

        [HttpGet]
        [Route("GetCompanyById")]
        public Company GetCompanyById(int companyId)
        {
            return companyManager.GetCompanyById(companyId);
        }

        [HttpPost]
        [Route("UpdateCompany")]
        public UpdateOperationOutput<CompanyDetails> UpdateCompany(Company company)
        {
            return companyManager.UpdateCompany(company);
        }

        [HttpGet]
        [Route("GetCompaniesInfo")]
        public IEnumerable<CompanyDetails> GetCompaniesInfo(string filter = null)
        {        
            return companyManager.GetCompaniesInfo();
        }
    }
 

}
