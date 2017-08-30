
CREATE PROCEDURE [Retail_BE].[sp_AccountPackageRecurCharge_GetByAccountPackages]
@AccountPackageRecurChargePeriods [Retail_BE].[AccountPackageRecurChargePeriodType] Readonly
AS
BEGIN

	select [ID],accountPackageRecurCharge.[AccountPackageID],[ChargeableEntityID],[BalanceAccountTypeID],[BalanceAccountID],[ChargeDay],[ChargeAmount],[CurrencyID],[TransactionTypeID],
		   [CreatedTime], AccountID, AccountBEDefinitionId
	FROM [Retail_BE].[AccountPackageRecurCharge] accountPackageRecurCharge with(nolock)
	Join @AccountPackageRecurChargePeriods tempStorage on tempStorage.AccountPackageId = accountPackageRecurCharge.AccountPackageID 
	And accountPackageRecurCharge.ChargeDay >= tempStorage.FromDate 
	And accountPackageRecurCharge.ChargeDay < tempStorage.ToDate
END