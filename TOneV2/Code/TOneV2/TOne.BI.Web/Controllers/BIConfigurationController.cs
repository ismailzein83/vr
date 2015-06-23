using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.BI.Entities;
using TOne.BI.Business;
using System.Net.Http;
using System.Web.Http;
using TOne.BI.Web.Models;
using TOne.BI.Web.ModelMappers;
namespace TOne.BI.Web.Controllers
{
    public class BIConfigurationController : ApiController
    {


        [HttpGet]
        public List<BIConfigurationModel> GetMeasures()
        {
            BIConfigurationManager manager = new BIConfigurationManager();
            List<BIConfiguration<BIConfigurationMeasure>> managerData = new List<BIConfiguration<BIConfigurationMeasure>>();
            managerData= manager.GetMeasures(); 
            Mappers mapper = new Mappers();
            return mapper.getSchemaConfiguration<BIConfigurationMeasure>(managerData); ;
        }
        [HttpGet]
        public List<BIConfigurationModel> GetEntities()
        {
            BIConfigurationManager manager = new BIConfigurationManager();
            List<BIConfiguration<BIConfigurationEntity>> managerData = new List<BIConfiguration<BIConfigurationEntity>>();
            managerData = manager.GetEntities();
            Mappers mapper = new Mappers();
            return mapper.getSchemaConfiguration<BIConfigurationEntity>(managerData); ;
        }
    
       
    }
}