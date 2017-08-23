
CREATE PROCEDURE [Retail_BE].[sp_RecurChargeBalanceUpdateSummary_GetByChargeDay]
@ChargeDay DateTime
AS
BEGIN
	select [ChargeDay],[Data]
	FROM [Retail_BE].[RecurChargeBalanceUpdateSummary] with(nolock)
	where ChargeDay = @ChargeDay
END