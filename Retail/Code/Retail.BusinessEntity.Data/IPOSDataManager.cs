using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Data
{
    public interface IPOSDataManager : IDataManager
    {
        IEnumerable<PointOfSale> GetPointOfSales();

        bool Insert(PointOfSale pos, out long insertedId);

        bool Update(PointOfSale pos);

        bool ArePOSsUpdated(ref object updateHandle);
    }
}
