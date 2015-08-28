using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.BI.Entities;
using Vanrise.BI.Web.Models;

namespace Vanrise.BI.Web.ModelMappers
{
    public class Mappers
    {
        public List<BIMeasureModel> MeasuresMapper(List<BIConfiguration<BIConfigurationMeasure>> managerData)
        {
            List<BIMeasureModel> rslt = new List<BIMeasureModel>();
            foreach (BIConfiguration<BIConfigurationMeasure> val in managerData)
            {
                rslt.Add(new BIMeasureModel
                {
                    Name = val.Name,
                    DisplayName = val.DisplayName,
                    RequiredPermissions = val.Configuration.RequiredPermissions,
                    Unit=val.Configuration.Unit
                });
            }

            return rslt;
        }
        public List<BIEntityModel<BIConfigurationEntity>> EntitiesMapper(List<BIConfiguration<BIConfigurationEntity>> managerData)
        {
            List<BIEntityModel<BIConfigurationEntity>> rslt = new List<BIEntityModel<BIConfigurationEntity>>();
            foreach (BIConfiguration<BIConfigurationEntity> val in managerData)
            {
                rslt.Add(new BIEntityModel<BIConfigurationEntity>
                {
                    Name = val.Name,
                    DisplayName = val.DisplayName,
                    Configuration = val.Configuration
                });
            }

            return rslt;
        }
    }
}