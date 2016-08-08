using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class ViewUserAccessContext : IViewUserAccessContext
    {
        public int UserId
        {
            get;
            set;
        }
    }
}
