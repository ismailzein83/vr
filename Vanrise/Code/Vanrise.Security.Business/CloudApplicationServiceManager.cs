using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class CloudApplicationServiceManager
    {
        public ConfigureAuthServerOutput ConfigureAuthServer(ConfigureAuthServerInput input)
        {
            ConfigureAuthServerOutput output = new ConfigureAuthServerOutput() { Result = ConfigureAuthServerResult.Failed };
            CloudAuthServer cloudAuthServer = new CloudAuthServer()
            {
                ApplicationIdentification = input.ApplicationIdentification,
                Settings = new CloudAuthServerSettings()
                {
                    AuthenticationCookieName = input.AuthenticationCookieName,
                    CurrentApplicationId = input.ApplicationId,
                    InternalURL = input.InternalURL,
                    OnlineURL = input.OnlineURL,
                    TokenDecryptionKey = input.TokenDecryptionKey
                }
            };
            CloudAuthServerManager cloudAuthServerManager = new CloudAuthServerManager();
            InsertOperationOutput<CloudAuthServer> insertOutput = cloudAuthServerManager.InsertCloudAuthServer(cloudAuthServer);

            output.Result = insertOutput.Result == InsertOperationResult.Succeeded ? ConfigureAuthServerResult.Succeeded : ConfigureAuthServerResult.Failed;
            return output;
        }
        public UpdateAuthServerOutput UpdateAuthServer(UpdateAuthServerInput input)
        {
            UpdateAuthServerOutput output = new UpdateAuthServerOutput() { Result = UpdateAuthServerResult.Failed };
            CloudAuthServerManager cloudAuthServerManager = new CloudAuthServerManager();
            
            var existingCloudAuthServer = cloudAuthServerManager.GetAuthServer();

            if (existingCloudAuthServer == null)
                return output;

            if ((string.Compare(existingCloudAuthServer.ApplicationIdentification.IdentificationKey, input.ApplicationIdentification.IdentificationKey) != 0)
                || existingCloudAuthServer.Settings.CurrentApplicationId != input.ApplicationId)
                return output;

            CloudAuthServer cloudAuthServer = new CloudAuthServer()
            {
                ApplicationIdentification = input.ApplicationIdentification,
                Settings = new CloudAuthServerSettings()
                {
                    AuthenticationCookieName = input.AuthenticationCookieName,
                    CurrentApplicationId = input.ApplicationId,
                    InternalURL = input.InternalURL,
                    OnlineURL = input.OnlineURL,
                    TokenDecryptionKey = input.TokenDecryptionKey
                }
            };

            UpdateOperationOutput<CloudAuthServer> updateOutput = cloudAuthServerManager.UpdateCloudAuthServer(cloudAuthServer);

            output.Result = updateOutput.Result == UpdateOperationResult.Succeeded ? UpdateAuthServerResult.Succeeded : UpdateAuthServerResult.Failed;
            return output;
        }

        public AssignUserFullControlOutput AssignUserFullControl(AssignUserFullControlInput input)
        {
            List<Vanrise.Security.Entities.Permission> permissions = new List<Vanrise.Security.Entities.Permission>();
            BusinessEntityModuleManager manager = new BusinessEntityModuleManager();
            BusinessEntityModule module = manager.GetBusinessEntityModules().First(itm => !itm.ParentId.HasValue || itm.ParentId.Value == Guid.Empty);

            foreach (int userId in input.UserIds)
            {
                var permission = new Vanrise.Security.Entities.Permission()
                {
                    EntityType = Vanrise.Security.Entities.EntityType.MODULE,
                    EntityId = module.ModuleId.ToString(),
                    HolderType = Vanrise.Security.Entities.HolderType.USER,
                    HolderId = userId.ToString(),
                    PermissionFlags = new List<Vanrise.Security.Entities.PermissionFlag>()
                };
                Vanrise.Security.Entities.PermissionFlag permissionFlag = new Vanrise.Security.Entities.PermissionFlag()
                {
                    Value = Vanrise.Security.Entities.Flag.ALLOW,
                    Name = "Full Control"
                };
                permission.PermissionFlags.Add(permissionFlag);
                permissions.Add(permission);
            }

            PermissionManager permissionManager = new PermissionManager();
            permissionManager.UpdatePermissions(permissions);

            return new AssignUserFullControlOutput();
        }
    }
}
