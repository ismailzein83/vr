using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class ViewSettings
    {
        public virtual string GetURL(View view)
        {
            return view.Url;
        }

        public virtual bool DoesUserHaveAccess(IViewUserAccessContext context)
        {
            return true;
        }
    }

    public interface IViewUserAccessContext
    {
        int UserId { get; }
    }
}
