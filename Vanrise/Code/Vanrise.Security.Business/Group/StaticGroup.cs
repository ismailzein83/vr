using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class StaticGroup : GroupSettings
    {
        public override Guid ConfigId { get { return new Guid("be6619ae-687f-45e3-bd7b-90d1db4626b6"); } }
        public List<int> MemberIds { get; set; }

        public override bool IsMember(IGroupSettingsContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if(this.MemberIds != null)
            {
                return this.MemberIds.Contains(context.UserId);
            }

            return false;
        }
    }
}
