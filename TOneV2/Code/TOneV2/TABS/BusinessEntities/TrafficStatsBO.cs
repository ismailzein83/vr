
namespace TABS.BusinessEntities
{
    public class TrafficStatsBO
    {
        public static object FindIdInTrafficStats(long id)
        {
            return TABS.DataHelper.ExecuteScalar("SELECT ID FROM TrafficStats WITH(NOLOCK) WHERE ID = @P1", id);
        }
    }
}
