using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class Switch : Vanrise.Entities.EntitySynchronization.IItem
    {
        public int SwitchId { get; set; }

        public string Name { get; set; }

        public string SourceId { get; set; }
        long IItem.ItemId
        {
            get
            {
                return SwitchId;
            }
            set
            {
                this.SwitchId = (int)value;
            }
        }
    }
}
