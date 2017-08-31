
CREATE PROCEDURE [Retail_BE].[sp_AccountPackageRecurCharge_GetByAccount]
@AcountBEDefinitionId uniqueIdentifier,
@AccountId bigint,
@IncludedFromDate DateTime,
@IncludedToDate DateTime

AS
BEGIN
	
	select	[ID],[AccountPackageID],[ChargeableEntityID],[BalanceAccountTypeID],[BalanceAccountID],[ChargeDay],[ChargeAmount],[CurrencyID],[TransactionTypeID],
			[CreatedTime], AccountID, AccountBEDefinitionId
	FROM	[Retail_BE].[AccountPackageRecurCharge] with(nolock)
	where	AccountID = @AccountId			 
			AND ChargeDay >= @IncludedFromDate AND ChargeDay<=@IncludedToDate
			AND AccountBEDefinitionId = @AcountBEDefinitionId
END