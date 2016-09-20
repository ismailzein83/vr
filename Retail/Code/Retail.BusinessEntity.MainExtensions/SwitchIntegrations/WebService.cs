using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.SwitchIntegrations
{
    public enum WebServiceRequestType {  Get = 1, Post = 2}
    public class WebService : SwitchIntegration
    {
        public override Guid ConfigId { get { return new Guid("c6af019e-2caf-4fab-9889-74737b13bb0d"); } }
        public string URL { get; set; }

        public WebServiceRequestType RequestType { get; set; }

        public string CredentialLogic { get; set; }

        public string MappingLogic { get; set; }

 
    }
}
