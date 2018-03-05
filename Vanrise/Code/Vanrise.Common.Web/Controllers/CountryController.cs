using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Data;

namespace Vanrise.Common.Web.Controllers
{
	[RoutePrefix(Constants.ROUTE_PREFIX + "Country")]
	public class VRCommon_CountryController : BaseAPIController
	{
		[HttpPost]
		[Route("GetFilteredCountries")]
		public object GetFilteredCountries(Vanrise.Entities.DataRetrievalInput<CountryQuery> input)
		{
			CountryManager manager = new CountryManager();
			return GetWebResponse(input, manager.GetFilteredCountries(input));
		}
        [HttpGet]
        [Route("GetCountryHistoryDetailbyHistoryId")]
        public Country GetCountryHistoryDetailbyHistoryId(int countryHistoryId)
        {
            CountryManager manager = new CountryManager();
            return manager.GetCountryHistoryDetailbyHistoryId(countryHistoryId);
        }
		[HttpGet]
		[Route("GetCountriesInfo")]
		public IEnumerable<CountryInfo> GetCountriesInfo(string filter = null)
		{
			var manager = new CountryManager();
			CountryFilter countryFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<CountryFilter>(filter) : null;
			return manager.GeCountriesInfo(countryFilter);
		}

		[HttpGet]
		[Route("GetCountry")]
		public Country GetCountry(int countryId)
		{
			CountryManager manager = new CountryManager();
			return manager.GetCountry(countryId,true);
		}

		[HttpPost]
		[Route("AddCountry")]
		public Vanrise.Entities.InsertOperationOutput<CountryDetail> AddCountry(Country country)
		{
			CountryManager manager = new CountryManager();
			return manager.AddCountry(country);
		}

		[HttpPost]
		[Route("UpdateCountry")]
		public Vanrise.Entities.UpdateOperationOutput<CountryDetail> UpdateCountry(Country country)
		{
			CountryManager manager = new CountryManager();
			return manager.UpdateCountry(country);
		}

		[HttpGet]
		[Route("GetCountrySourceTemplates")]
		public IEnumerable<SourceCountryReaderConfig> GetCountrySourceTemplates()
		{
			CountryManager manager = new CountryManager();
			return manager.GetCountrySourceTemplates();
		}

		[HttpGet]
		[Route("DownloadCountriesTemplate")]
		public object DownloadCountriesTemplate()
		{
			var template = "~/Client/Modules/Common/Template/Country Add sample.xls";
			string physicalPath = HttpContext.Current.Server.MapPath(template);
			byte[] bytes = File.ReadAllBytes(physicalPath);

			MemoryStream memStreamRate = new System.IO.MemoryStream();
			memStreamRate.Write(bytes, 0, bytes.Length);
			memStreamRate.Seek(0, System.IO.SeekOrigin.Begin);
			return GetExcelResponse(memStreamRate, "Country Template.xls");
		}

		[HttpGet]
		[Route("UploadCountries")]
		public UploadCountryLog UploadCountries(int fileID)
		{
			CountryManager manager = new CountryManager();
			return manager.AddCountries(fileID);
		}

		[HttpGet]
		[Route("DownloadCountryLog")]
		public object DownloadCountryLog(long fileID)
		{
			CountryManager manager = new CountryManager();
			byte[] bytes = manager.DownloadCountryLog(fileID);
			return GetExcelResponse(bytes, "ImportedCountriesResults.xls");
		}

        [HttpPost]
        [Route("GetCountriesByCountryIds")]
        public IEnumerable<Country> GetCountriesByCountryIds(IEnumerable<int> countryIds)
        {
            CountryManager manager = new CountryManager();
           return  manager.GetCountriesByCountryIds(countryIds);
        }

        [HttpGet]
        [Route("GetCountryCriteriaGroupTemplates")]
        public IEnumerable<CountryCriteriaGroupConfig> GetCountryCriteriaGroupTemplates()
        {
            CountryManager manager = new CountryManager();
            return manager.GetCountryCriteriaGroupTemplates();
        }
	}
}