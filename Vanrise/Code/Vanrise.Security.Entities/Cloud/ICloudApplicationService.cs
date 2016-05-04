using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public interface ICloudApplicationService
    {
        ConfigureAuthServerOutput ConfigureAuthServer(ConfigureAuthServerInput input);

        UpdateAuthServerOutput UpdateAuthServer(UpdateAuthServerInput input);

        AssignUserFullControlOutput AssignUserFullControl(AssignUserFullControlInput input);
    }

    public class ConfigureAuthServerInput
    {
        public CloudApplicationIdentification ApplicationIdentification { get; set; }

        public int ApplicationId { get; set; }

        public string OnlineURL { get; set; }

        public string InternalURL { get; set; }

        public string AuthenticationCookieName { get; set; }

        public string TokenDecryptionKey { get; set; }
    }

    public enum ConfigureAuthServerResult {  Succeeded = 0, Failed = 1}

    public class ConfigureAuthServerOutput
    {
        public ConfigureAuthServerResult Result { get; set; }
    }

    public class UpdateAuthServerInput
    {
        
    }

    public class UpdateAuthServerOutput
    {

    }

    public class AssignUserFullControlInput
    {
        public int UserId { get; set; }
    }

    public class AssignUserFullControlOutput
    {

    }
}
