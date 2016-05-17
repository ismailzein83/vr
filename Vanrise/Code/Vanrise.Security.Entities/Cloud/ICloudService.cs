using System.Collections.Generic;
namespace Vanrise.Security.Entities
{
    public interface ICloudService
    {
        GetApplicationUsersOutput GetApplicationUsers(GetApplicationUsersInput input);

        CheckApplicationUsersUpdatedOuput CheckApplicationUsersUpdated(CheckApplicationUsersUpdatedInput input);

        AddUserToApplicationOutput AddUserToApplication(AddUserToApplicationInput input);

        UpdateUserToApplicationOutput UpdateUserToApplication(UpdateUserToApplicationInput input);

        GetApplicationTenantsOutput GetApplicationTenants(GetApplicationTenantsInput input);

        CheckApplicationTenantsUpdatedOuput CheckApplicationTenantsUpdated(CheckApplicationTenantsUpdatedInput input);

        AddTenantToApplicationOutput AddTenantToApplication(AddTenantToApplicationInput input);

        UpdateTenantToApplicationOutput UpdateTenantToApplication(UpdateTenantToApplicationInput input);
        
        List<CloudTenantOutput> GetCloudTenants(CloudTenantInput input);

        ResetUserPasswordApplicationOutput ResetUserPasswordApplication(ResetUserPasswordApplicationInput input);

        ForgotUserPasswordApplicationOutput ForgotUserPasswordApplication(ForgotUserPasswordApplicationInput input);
    }
}
