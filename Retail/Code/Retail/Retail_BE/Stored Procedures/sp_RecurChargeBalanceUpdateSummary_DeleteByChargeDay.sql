
CREATE PROCEDURE [Retail_BE].[sp_RecurChargeBalanceUpdateSummary_DeleteByChargeDay]
@ChargeDay DateTime
AS
BEGIN
	Delete
	FROM [Retail_BE].[RecurChargeBalanceUpdateSummary]
	where ChargeDay = @ChargeDay
END