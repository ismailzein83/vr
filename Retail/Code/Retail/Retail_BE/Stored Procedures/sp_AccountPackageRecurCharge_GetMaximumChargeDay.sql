
CREATE PROCEDURE [Retail_BE].[sp_AccountPackageRecurCharge_GetMaximumChargeDay]
AS
BEGIN
	
	select MAX([ChargeDay])
	FROM [Retail_BE].[AccountPackageRecurCharge] with(nolock)
END