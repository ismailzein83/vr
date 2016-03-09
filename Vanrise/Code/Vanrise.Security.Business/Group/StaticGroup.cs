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
