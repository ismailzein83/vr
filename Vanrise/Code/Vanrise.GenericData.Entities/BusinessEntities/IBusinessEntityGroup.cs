using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities.BusinessEntities;

namespace Vanrise.GenericData.Entities
{
    public interface IBusinessEntityGroup
    {
        IEnumerable<Object> GetIds(IBusinessEntityGroupContext context);

        string GetDescription(IBusinessEntityGroupContext context);
    }
}
