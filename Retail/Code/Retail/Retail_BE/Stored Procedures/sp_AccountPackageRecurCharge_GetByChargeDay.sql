
CREATE PROCEDURE [Retail_BE].[sp_AccountPackageRecurCharge_GetByChargeDay]
@ChargeDay DateTime

AS
BEGIN
	select [ID],[AccountPackageID],[ChargeableEntityID],[BalanceAccountTypeID],[BalanceAccountID],[ChargeDay],[ChargeAmount],[CurrencyID],[TransactionTypeID],
		   [CreatedTime], AccountID, AccountBEDefinitionId
	FROM [Retail_BE].[AccountPackageRecurCharge] with(nolock)
	where ChargeDay = @ChargeDay
END