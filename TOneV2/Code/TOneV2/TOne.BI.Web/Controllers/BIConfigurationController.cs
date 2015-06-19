using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.BI.Entities;
using TOne.BI.Business;
using System.Net.Http;
using System.Web.Http;
using TOne.BI.Web.Models;
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
            List<BIConfigurationModel> rslt = new List<BIConfigurationModel>();
            foreach (BIConfiguration<BIConfigurationMeasure> val in managerData)
                {
                    rslt.Add(new BIConfigurationModel
                    {
                        Id = val.Id,
                        Name = val.Name
                    });
                }

            return rslt;
        }
        [HttpGet]
        public List<BIConfigurationModel> GetEntities()
        {
            BIConfigurationManager manager = new BIConfigurationManager();
            List<BIConfiguration<BIConfigurationEntity>> managerData = new List<BIConfiguration<BIConfigurationEntity>>();
            managerData = manager.GetEntities();
            List<BIConfigurationModel> rslt = new List<BIConfigurationModel>();
            foreach (BIConfiguration<BIConfigurationEntity> val in managerData)
            {
                rslt.Add(new BIConfigurationModel
                {
                    Id = val.Id,
                    Name = val.Name
                });
            }

            return rslt;
        }
         [HttpGet]
        public IEnumerable<TimeValuesRecord> GetMeasureValues1(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, [FromUri] int[] measureTypesIDs)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetMeasureValues1(timeDimensionType, fromDate, toDate, measureTypesIDs);
        }
         [HttpGet]
        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues1(int entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, [FromUri] int[] measureTypesIDs)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetEntityMeasuresValues1(entityType, entityId, timeDimensionType, fromDate, toDate, measureTypesIDs);
        }
         [HttpGet]
         public IEnumerable<EntityRecord> GetTopEntities1(int entityTypeID, int topByMeasureTypeID, DateTime fromDate, DateTime toDate, int topCount, [FromUri] int[] measureTypesIDs)
         {
             GenericEntityManager manager = new GenericEntityManager();
             return manager.GetTopEntities1(entityTypeID, topByMeasureTypeID, fromDate, toDate, topCount, measureTypesIDs);
         }
    }
}