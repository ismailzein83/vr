using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class BEActionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public BEActionType ActionType { get; set; }
    }

    public abstract class BEActionType
    {
        public abstract Guid ConfigId { get; }
    }
}
