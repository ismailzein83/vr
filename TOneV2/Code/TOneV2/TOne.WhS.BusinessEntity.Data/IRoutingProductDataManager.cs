using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IRoutingProductDataManager : IDataManager
    {
        bool Insert(RoutingProduct routingProduct, out int insertedId);

        bool Update(RoutingProduct routingProduct);
    }
}
