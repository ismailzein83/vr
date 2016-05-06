
namespace Vanrise.Security.Entities
{
    public interface ICloudApplicationService
    {
        ConfigureAuthServerOutput ConfigureAuthServer(ConfigureAuthServerInput input);

        UpdateAuthServerOutput UpdateAuthServer(UpdateAuthServerInput input);

        AssignUserFullControlOutput AssignUserFullControl(AssignUserFullControlInput input);
    }

    public class BaseAuthServerInput
    {
        public CloudApplicationIdentification ApplicationIdentification { get; set; }

        public int ApplicationId { get; set; }

        public string OnlineURL { get; set; }

        public string InternalURL { get; set; }

        public string AuthenticationCookieName { get; set; }

        public string TokenDecryptionKey { get; set; }
    }
    public class ConfigureAuthServerInput : BaseAuthServerInput
    {

    }

    public enum ConfigureAuthServerResult {  Succeeded = 0, Failed = 1}

    public class ConfigureAuthServerOutput
    {
        public ConfigureAuthServerResult Result { get; set; }
    }

    public class UpdateAuthServerInput : BaseAuthServerInput
    {

    }

    public enum UpdateAuthServerResult { Succeeded = 0, Failed = 1 }

    public class UpdateAuthServerOutput
    {
        public UpdateAuthServerResult Result { get; set; }
    }

    public class AssignUserFullControlInput
    {
        public int UserId { get; set; }
    }

    public class AssignUserFullControlOutput
    {

    }
}