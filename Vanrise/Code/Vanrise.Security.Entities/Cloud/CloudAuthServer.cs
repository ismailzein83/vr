using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{    
    public class CloudAuthServer
    {
        public const string CLOUDSERVICE_HTTPHEADERNAME = "Vanrise_CloudApplicationIdentification";        

        public CloudAuthServerSettings Settings { get; set; }

        public CloudApplicationIdentification ApplicationIdentification { get; set; }
    }

    public class CloudAuthServerSettings
    {
        public string OnlineURL { get; set; }

        public string InternalURL { get; set; }

        public string AuthenticationCookieName { get; set; }

        public string TokenDecryptionKey { get; set; }

        public int CurrentApplicationId { get; set; }
    }
}
