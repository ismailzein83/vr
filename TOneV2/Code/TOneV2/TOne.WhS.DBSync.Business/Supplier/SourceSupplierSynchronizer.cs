using TOne.WhS.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business.EntitySynchronization;
using Vanrise.Entities.EntitySynchronization;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceSupplierSynchronizer : SourceItemSynchronizer<SourceSupplier, Supplier, SourceSupplierReader>
    {
        public SourceSupplierSynchronizer(SourceSupplierReader sourceItemReader)
            : base(sourceItemReader)
        {

        }

        public override void Synchronize()
        {
            base.Synchronize();
        }

        protected override void AddItems(List<Supplier> itemsToAdd)
        {
            SupplierManager supplierManager = new SupplierManager();
            foreach (var s in itemsToAdd)
            {
                supplierManager.AddSupplierFromSource(s);
            }
        }

        protected override Supplier BuildItemFromSource(SourceSupplier sourceItem)
        {
            return new Supplier
            {
                Name = sourceItem.Name,
                SourceId = sourceItem.SourceId
            };
        }

        protected override void ReserveIdRange(int nbOfIds, out long startingId)
        {
            SupplierManager.ReserveIDRange(nbOfIds, out startingId);
        }

        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceItemIds)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateItems(List<Supplier> itemsToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
