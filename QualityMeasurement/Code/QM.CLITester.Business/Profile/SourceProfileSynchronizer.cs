using QM.CLITester.Entities;
using System.Collections.Generic;
using Vanrise.Common.Business.EntitySynchronization;
using Vanrise.Entities.EntitySynchronization;

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
                SourceId = sourceItem.SourceId, 
                Settings = sourceItem.Settings
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
