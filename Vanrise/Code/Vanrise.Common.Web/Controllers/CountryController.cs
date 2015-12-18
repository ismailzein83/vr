using Aspose.Cells;
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

namespace Vanrise.Web.Controllers
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
        [Route("GetCountriesInfo")]
        public IEnumerable<CountryInfo> GetCountriesInfo()
        {
            CountryManager manager = new CountryManager();
            return manager.GeCountriesInfo();
        }
        [HttpGet]
        [Route("GetCountry")]
        public Country GetCountry(int countryId)
        {
            CountryManager manager = new CountryManager();
            return manager.GetCountry(countryId);
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
        public List<TemplateConfig> GetCountrySourceTemplates()
        {
            CountryManager manager = new CountryManager();
            return manager.GetCountrySourceTemplates();
        }

        [HttpGet]
        [Route("DownloadCountriesTemplate")]
        public HttpResponseMessage DownloadCountriesTemplate(int type)
        {

            var template = type == 1 ? "~/Client/Modules/Common/Template/Country Add sample.xls" : "~/Client/Modules/Common/Template/Country Update sample.xls";
            string obj = HttpContext.Current.Server.MapPath(template);
            Workbook workbook = new Workbook(obj);
            Aspose.Cells.License license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Cells.lic");
            MemoryStream memoryStream = new MemoryStream();
            memoryStream = workbook.SaveToStream();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            memoryStream.Position = 0;
            response.Content = new StreamContent(memoryStream);

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format("CountiesTemplate.xls")
            };
            return response;
        }


        [HttpPost]
        [Route("DownloadCountriesTemplate")]
        public string UploadCountries(CountryFile file)
        {
            //DataSet data = new DataSet();
            DataTable countryDataTable = new DataTable();
            VRFileManager fileManager = new VRFileManager();
            CountryManager manager = new CountryManager();
            byte[] bytes = fileManager.GetFile(file.FileId).Content;

            var fileStream = new System.IO.MemoryStream(bytes);
            ExportTableOptions options = new ExportTableOptions();
            options.CheckMixedValueType = true;
            Workbook wbk = new Workbook(fileStream);
            wbk.CalculateFormula();
            string message = "";
           
            if (wbk.Worksheets[0].Cells.MaxDataRow > -1 && wbk.Worksheets[0].Cells.MaxDataColumn > -1)
                countryDataTable = wbk.Worksheets[0].Cells.ExportDataTableAsString(0, 0, wbk.Worksheets[0].Cells.MaxDataRow + 1, wbk.Worksheets[0].Cells.MaxDataColumn + 1);
            if (file.Type == 1)
            {
                List<string> countriesToAdd = new List<string>();
                for (int i = 1; i < countryDataTable.Rows.Count; i++)
                {
                    countriesToAdd.Add(countryDataTable.Rows[i][0].ToString());
                }
                message = manager.AddCountries(countriesToAdd);
            }
            else if (file.Type == 2)
            {
                Dictionary<string, string> countriesToUpdate = new Dictionary<string, string>();

                for (int i = 1; i < countryDataTable.Rows.Count; i++)
                {
                    
                    countriesToUpdate.Add(countryDataTable.Rows[i][0].ToString(), countryDataTable.Rows[i][1].ToString());
                }

                message = manager.UpdateCountires(countriesToUpdate);
            }
            return message;
        }
    }
}