using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SupplierPriceListTemplateMigrator : Migrator<DefaultSourceItem, object>
    {
        public SupplierPriceListTemplateMigrator(MigrationContext context)
            : base(context)
        {
        }


        public override void FillTableInfo(bool useTempTables)
        {

        }


        public override void AddItems(List<object> itemsToAdd)
        {

        }

        public override IEnumerable<DefaultSourceItem> GetSourceItems()
        {
            return null;
        }

        public override object BuildItemFromSource(DefaultSourceItem sourceItem)
        {
            return null;
        }
    }
}
