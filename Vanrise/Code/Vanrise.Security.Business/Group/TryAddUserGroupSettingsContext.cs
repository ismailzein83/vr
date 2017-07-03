using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class TryAddUserGroupSettingsContext : ITryAddUserGroupSettingsContext
    {
        public int UserId { get; set; }
    }
}