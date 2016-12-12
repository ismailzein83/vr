using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Security.Entities
{
    public class CloudApplicationUser
    {
        public User User { get; set; }

        public DateTime? EnabledTill { get; set; }

        public string Description { get; set; }
    }

    public class GetApplicationUsersInput
    {
    }

    public class GetApplicationUsersOutput
    {
        public List<CloudApplicationUser> Users { get; set; }
    }

    public class CheckApplicationUsersUpdatedInput
    {
        public object LastReceivedUpdateInfo { get; set; }
    }

    public class CheckApplicationUsersUpdatedOuput
    {
        public bool Updated { get; set; }

        public object LastUpdateInfo { get; set; }
    }

    public class AddUserToApplicationInput
    {
        public string Email { get; set; }

        public DateTime? EnabledTill { get; set; }

        public string Description { get; set; }

        public int TenantId { get; set; }
    }

    public class AddUserToApplicationOutput
    {
        public InsertOperationOutput<CloudApplicationUser> OperationOutput { get; set; }
    }

    public class UpdateUserToApplicationInput
    {
        public int UserId { get; set; }

        public DateTime? EnabledTill { get; set; }

        public string Description { get; set; }

        //public int TenantId { get; set; }
    }

    public class UpdateUserToApplicationOutput
    {
        public UpdateOperationOutput<CloudApplicationUser> OperationOutput { get; set; }
    }

    public class ResetUserPasswordApplicationInput
    {
        public int UserId { get; set; }
    }

    public class ResetUserPasswordApplicationOutput
    {
        public UpdateOperationOutput<object> OperationOutput { get; set; }
    }

    public class ForgotUserPasswordApplicationInput 
    {
        public string Email { get; set; }
    }

    public class ForgotUserPasswordApplicationOutput
    {
        public UpdateOperationOutput<object> OperationOutput { get; set; }
    }

}
