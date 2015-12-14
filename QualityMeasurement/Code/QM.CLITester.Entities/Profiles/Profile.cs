using Vanrise.Entities.EntitySynchronization;

namespace QM.CLITester.Entities
{
    public class Profile : IItem
    {
        public int ProfileId { get; set; }

        public string Name { get; set; }

        public string SourceId { get; set; }

        public ProfileSettings Settings { get; set; }

        long IItem.ItemId
        {
            get
            {
                return ProfileId;
            }
            set
            {
                this.ProfileId = (int)value;
            }
        }

    }
}
