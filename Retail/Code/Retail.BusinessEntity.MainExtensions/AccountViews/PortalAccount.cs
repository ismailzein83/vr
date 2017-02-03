using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountViews
{
    public class PortalAccount : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("DAB350C7-1451-42B2-9E04-215E252433E0"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-portalaccount-view";
            }
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public Guid ConnectionId { get; set; }
    }
} 
