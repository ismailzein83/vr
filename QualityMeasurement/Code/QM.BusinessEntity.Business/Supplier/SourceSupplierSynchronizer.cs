using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business.EntitySynchronization;
using Vanrise.Entities.EntitySynchronization;

namespace QM.BusinessEntity.Business
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

        protected override void UpdateItems(List<Supplier> itemsToUpdate)
        {
            SupplierManager supplierManager = new SupplierManager();
            foreach (var s in itemsToUpdate)
            {
                supplierManager.UpdateSupplierFromSource(s);
            }
        }

        protected override void SetItemsDeleted(List<long> itemIds)
        {
            foreach (var s in itemIds)
            {
                SupplierManager supplierManager = new SupplierManager();
                Supplier supplier = supplierManager.GetSupplier((int)s);
                if (supplier != null)
                {
                    supplier.IsDeleted = true;
                    supplierManager.UpdateSupplier(supplier);                    
                }
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

        protected override Dictionary<string, long> GetExistingItemIds()
        {
            SupplierManager supplierManager = new SupplierManager();
            return supplierManager.GetExistingItemIds();
        }

        protected override void ReserveIdRange(int nbOfIds, out long startingId)
        {
            SupplierManager.ReserveIDRange(nbOfIds, out startingId);
        }
    }
}
