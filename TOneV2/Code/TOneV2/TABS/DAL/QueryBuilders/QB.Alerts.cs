
namespace TABS.DAL
{
	public partial class QueryBuilder
	{
        public static string Ex_Sp_GetAlertsQuery()
        {
            return @"EXEC	[dbo].[SP_GetAlerts]
		                                    @TopN = @P1,
		                                    @ShowHiddenAlerts = @P2,
		                                    @AlertLevel = @P3,
		                                    @TAG = @P4,
		                                    @Source = @P5,
		                                    @UserID = @P6";
        }
	}
}
