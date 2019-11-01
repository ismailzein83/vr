using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class NodeInfoFilter: IGenericBusinessEntityFilter
    {
        public long AreaId { get; set; }
        public long SiteId { get; set; }

        public bool IsMatch(IGenericBusinessEntityFilterContext context)
        {
            if (context != null)
            {
                if (context.GenericBusinessEntity != null && context.GenericBusinessEntity.FieldValues != null && context.GenericBusinessEntity.FieldValues.Count > 0)
                {
                    long areaId = (long)context.GenericBusinessEntity.FieldValues.GetRecord("Area");
                    long siteId = (long)context.GenericBusinessEntity.FieldValues.GetRecord("Site");

                    if (!AreaId.Equals(areaId))
                    {
                        return false;
                    }
                    if (!SiteId.Equals(siteId))
                    {
                        return false;
                    }

                }
            }
            return true;
        }
    }
}
