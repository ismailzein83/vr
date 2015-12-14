using QM.BusinessEntity.Business;
using QM.BusinessEntity.Entities;
using QM.CLITester.Business;
using QM.CLITester.Entities;
using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Business
{
    public class SourceProfileSynchronizer : SourceItemSynchronizer<SourceProfile, Profile, ISourceItemReader<SourceProfile>>
    {
        public SourceProfileSynchronizer(ISourceItemReader<SourceProfile> sourceItemReader)
            : base(sourceItemReader)
        {

        }

        protected override void AddItems(List<Profile> itemsToAdd)
        {
            ProfileManager ProfileManager = new ProfileManager();
            foreach (var s in itemsToAdd)
            {
                ProfileManager.InsertProfileSynchronize(s);
            }
        }

        protected override void UpdateItems(List<Profile> itemsToUpdate)
        {
            ProfileManager ProfileManager = new ProfileManager();
            foreach (var s in itemsToUpdate)
            {
                ProfileManager.UpdateProfileSynchronize(s);
            }
        }

        protected override Profile BuildItemFromSource(SourceProfile sourceItem)
        {
            return new Profile
            {
                Name = sourceItem.Name,
                SourceId = sourceItem.SourceId
            };
        }

        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceItemIds)
        {
            ProfileManager ProfileManager = new ProfileManager();
            return ProfileManager.GetExistingItemIds(sourceItemIds);
        }

        protected override void ReserveIdRange(int nbOfIds, out long startingId)
        {
            ProfileManager.ReserveIDRange(nbOfIds, out startingId);
        }
    }
}
