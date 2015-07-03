﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.BI.Entities;
using TOne.BI.Web.Models;

namespace TOne.BI.Web.ModelMappers
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
                    RequiredPermissions = val.Configuration.RequiredPermissions
                    
                });
            }

            return rslt;
        }
        public List<BIEntityModel> EntitiesMapper(List<BIConfiguration<BIConfigurationEntity>> managerData)
        {
            List<BIEntityModel> rslt = new List<BIEntityModel>();
            foreach (BIConfiguration<BIConfigurationEntity> val in managerData)
            {
                rslt.Add(new BIEntityModel
                {
                    Name = val.Name,
                    DisplayName = val.DisplayName,

                });
            }

            return rslt;
        }
    }
}