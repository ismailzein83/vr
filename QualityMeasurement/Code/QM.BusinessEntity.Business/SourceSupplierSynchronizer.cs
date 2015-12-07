using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Business
{
    public class SourceSupplierSynchronizer : SourceItemSynchronizer<SourceSupplier, Supplier, ISourceItemReader<SourceSupplier>>
    {
        protected override void AddItems(List<Supplier> itemsToAdd)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateItems(List<Supplier> itemsToUpdate)
        {
            throw new NotImplementedException();
        }

        protected override Supplier BuildItemFromSource(SourceSupplier sourceZone)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceZoneIds)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<long> itemIds)
        {
            throw new NotImplementedException();
        }

        protected override void ReserveIdRange(int nbOfIds, out long startingId)
        {
            throw new NotImplementedException();
        }
    }
}
