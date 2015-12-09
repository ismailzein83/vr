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
        public SourceSupplierSynchronizer(ISourceItemReader<SourceSupplier> sourceItemReader)
            : base(sourceItemReader)
        {

        }

        protected override void AddItems(List<Supplier> itemsToAdd)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateItems(List<Supplier> itemsToUpdate)
        {
            throw new NotImplementedException();
        }

        protected override Supplier BuildItemFromSource(SourceSupplier sourceItem)
        {
            return new Supplier
            {
                Name = sourceItem.Name,
                SourceId = sourceItem.SourceId
            };
        }

        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceItemIds)
        {
            throw new NotImplementedException();
        }

        protected override void ReserveIdRange(int nbOfIds, out long startingId)
        {
            SupplierManager.ReserveIDRange(nbOfIds, out startingId);
        }
    }
}
