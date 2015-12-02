using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Entities
{
    public interface ISourceItem
    {
        string SourceId { get; }
    }

    public interface IItem
    {
        long ItemId { get; set; }
    }

    public class Zone : IItem
    {
        public long ZoneId { get; set; }

        public int CountryId { get; set; }

        public string Name { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public long ItemId
        {
            get
            {
                return this.ZoneId;
            }
            set
            {
                this.ZoneId = value;
            }
        }
    }
}
