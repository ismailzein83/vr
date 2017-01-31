using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class SQLConnection : VRConnectionSettings
    {
        public override Guid ConfigId { get { return new Guid("8224B27C-C128-4150-A4E4-5E2034BB3A36"); } }
        
        public string ConnectionString { get; set; }

        
    }

   
}
