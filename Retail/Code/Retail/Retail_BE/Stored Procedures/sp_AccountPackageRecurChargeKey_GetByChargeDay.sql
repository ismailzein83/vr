
CREATE PROCEDURE [Retail_BE].[sp_AccountPackageRecurChargeKey_GetByChargeDay]
@ChargeDay DateTime
AS
BEGIN
	select [BalanceAccountTypeID],[ChargeDay],[TransactionTypeID]
	FROM [Retail_BE].[AccountPackageRecurCharge] with(nolock)
	where ChargeDay = @ChargeDay
END