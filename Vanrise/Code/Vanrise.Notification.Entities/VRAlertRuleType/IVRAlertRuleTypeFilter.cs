
namespace Vanrise.Notification.Entities
{
    public interface IVRAlertRuleTypeFilter
    {
        bool IsMatch(VRAlertRuleType alertRuleType);
    }
}