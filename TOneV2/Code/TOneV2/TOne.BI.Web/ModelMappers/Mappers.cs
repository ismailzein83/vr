using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.BI.Entities;
using TOne.BI.Web.Models;

namespace TOne.BI.Web.ModelMappers
{
    public class Mappers
    {
        public List<BIConfigurationModel> getSchemaConfiguration<T>(List<BIConfiguration<T>> managerData)
        {
            List<BIConfigurationModel> rslt = new List<BIConfigurationModel>();
            foreach (BIConfiguration<T> val in managerData)
            {
                rslt.Add(new BIConfigurationModel
                {
                    Name = val.Name,
                    DisplayName = val.DisplayName
                });
            }

            return rslt;
        }
    }
}