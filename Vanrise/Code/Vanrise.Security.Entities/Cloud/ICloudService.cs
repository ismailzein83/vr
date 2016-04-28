using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Security.Entities
{
    public interface ICloudService
    {
        GetApplicationUsersOutput GetApplicationUsers(GetApplicationUsersInput input);

        CheckApplicationUsersUpdatedOuput CheckApplicationUsersUpdated(CheckApplicationUsersUpdatedInput input);

        AddUserToApplicationOutput AddUserToApplication(AddUserToApplicationInput input);
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

        public UserStatus Status { get; set; }

        public string Description { get; set; }
    }

    public class AddUserToApplicationOutput
    {
        public InsertOperationOutput<CloudApplicationUser> OperationOutput { get; set; }

    }
}
