using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Business.EntityMigrator;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceSwitchMigrator : SourceItemMigrator<SourceSwitch, Switch, SourceSwitchReader>
    {
        public SourceSwitchMigrator(SourceSwitchReader sourceItemReader)
            : base(sourceItemReader)
        {

        }

        public override void Migrate()
        {
            base.Migrate();
        }

        protected override void AddItems(List<Switch> itemsToAdd)
        {
            SwitchManager switchManager = new SwitchManager();
            switchManager.AddSwitchesFromSource(itemsToAdd);
        }

        protected override Switch BuildItemFromSource(SourceSwitch sourceItem)
        {
            return new Switch
            {
                Name = sourceItem.Name
            };
        }
    }
}
