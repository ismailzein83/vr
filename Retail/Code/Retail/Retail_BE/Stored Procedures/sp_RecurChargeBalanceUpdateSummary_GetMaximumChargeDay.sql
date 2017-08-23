
CREATE PROCEDURE [Retail_BE].[sp_RecurChargeBalanceUpdateSummary_GetMaximumChargeDay]
AS
BEGIN
	
	select MAX([ChargeDay])
	FROM [Retail_BE].[AccountPackageRecurCharge] with(nolock)
END