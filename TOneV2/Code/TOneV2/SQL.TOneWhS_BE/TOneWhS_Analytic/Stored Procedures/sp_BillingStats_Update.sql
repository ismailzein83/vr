-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Analytic].[sp_BillingStats_Update]
	@BillingStats [TOneWhS_Analytic].BillingStatsType  READONLY
AS
BEGIN

	
	UPDATE [TOneWhS_Analytic].BillingStats
	SET 
		BillingStats.AvgDuration = BillingStats.AvgDuration + bStat.AvgDuration,
		BillingStats.MinDuration = CASE WHEN BillingStats.MinDuration < bStat.MinDuration THEN BillingStats.MinDuration ELSE bStat.MinDuration END ,
		BillingStats.MaxDuration = CASE WHEN BillingStats.MaxDuration > bStat.MaxDuration THEN BillingStats.MaxDuration ELSE bStat.MaxDuration END ,
		
		BillingStats.FirstCallTime = CASE WHEN BillingStats.FirstCallTime < bStat.FirstCallTime THEN BillingStats.FirstCallTime ELSE bStat.FirstCallTime END ,
		BillingStats.LastCallTime = CASE WHEN BillingStats.LastCallTime > bStat.LastCallTime THEN BillingStats.LastCallTime ELSE bStat.LastCallTime END ,
		
		BillingStats.CostExtraCharges = BillingStats.CostExtraCharges + bStat.CostExtraCharges,
		BillingStats.CostDuration = BillingStats.CostNets + bStat.CostDuration,
		BillingStats.CostNets = BillingStats.CostNets + bStat.CostNets,
		BillingStats.SaleExtraCharges  = BillingStats.SaleExtraCharges + bStat.SaleExtraCharges,
		BillingStats.SaleDuration = BillingStats.SaleDuration + bStat.SaleDuration,
		BillingStats.NumberOfCalls = BillingStats.NumberOfCalls +  bStat.NumberOfCalls,
		BillingStats.SaleNets = BillingStats.SaleNets + bStat.SaleNets
		
		
	FROM [TOneWhS_Analytic].BillingStats  inner join @BillingStats as bStat ON  BillingStats.ID = bStat.ID
	
END