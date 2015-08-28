using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.BI.Entities;
using Vanrise.BI.Business;
using System.Net.Http;
using System.Web.Http;
using Vanrise.BI.Web.Models;
using Vanrise.BI.Web.ModelMappers;
namespace Vanrise.BI.Web.Controllers
{
    public class BIConfigurationController : Vanrise.Web.Base.BaseAPIController
    {


        [HttpGet]
        public List<BIMeasureModel> GetMeasures()
        {
            BIConfigurationManager manager = new BIConfigurationManager();
            List<BIConfiguration<BIConfigurationMeasure>> managerData = new List<BIConfiguration<BIConfigurationMeasure>>();
            managerData= manager.GetMeasures(); 
            Mappers mapper = new Mappers();
            return mapper.MeasuresMapper(managerData); ;
        }
        [HttpGet]
        public List<BIEntityModel<BIConfigurationEntity>> GetEntities()
        {
            BIConfigurationManager manager = new BIConfigurationManager();
            List<BIConfiguration<BIConfigurationEntity>> managerData = new List<BIConfiguration<BIConfigurationEntity>>();
            managerData = manager.GetEntities();
            Mappers mapper = new Mappers();
            return mapper.EntitiesMapper(managerData); ;
        }

        [HttpGet]
        public string GetSystemCurrency()
        {
            return System.Configuration.ConfigurationManager.AppSettings["Currency"];
        } 
    }
}