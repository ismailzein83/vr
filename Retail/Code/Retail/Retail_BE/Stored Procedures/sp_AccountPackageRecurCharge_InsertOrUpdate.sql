
CREATE PROCEDURE [Retail_BE].[sp_AccountPackageRecurCharge_InsertOrUpdate]
@ChargeDay Datetime,
@AccountPackageRecurCharges [Retail_BE].[AccountPackageRecurChargeType] Readonly
AS
BEGIN

    SELECT t2.[AccountPackageID], t2.[ChargeableEntityID], t2.[BalanceAccountTypeID], t2.[BalanceAccountID], t2.[ChargeDay], t2.[ChargeAmount] as NewChargeAmount, t2.[CurrencyID] as NewCurrencyId,
	t2.[TransactionTypeID],  t1.ID, t1.ChargeAmount as OldChargeAmount, t1.CurrencyID as OldCurrencyId, t2.AccountID, t2.AccountBEDefinitionId
	INTO #tempAccountPackageRecurCharge
	FROM  @AccountPackageRecurCharges t2
	left join [Retail_BE].[AccountPackageRecurCharge] t1 on t1.AccountPackageID = t2.AccountPackageID and t1.ChargeableEntityID = t2.ChargeableEntityID 
	and ISNULL(t1.BalanceAccountTypeID,'00000000-0000-0000-0000-000000000000') = ISNULL(t2.BalanceAccountTypeID,'00000000-0000-0000-0000-000000000000')
	and ISNULL(t1.BalanceAccountID,'') = ISNULL(t2.BalanceAccountID,'') and t1.ChargeDay = t2.ChargeDay 
	and ISNULL(t1.TransactionTypeID,'00000000-0000-0000-0000-000000000000') = ISNULL(t2.TransactionTypeID,'00000000-0000-0000-0000-000000000000')
	and t1.AccountID = t2.AccountID and t1.AccountBEDefinitionId = t2.AccountBEDefinitionId

	DELETE FROM [Retail_BE].[AccountPackageRecurCharge] where Id not in (select ID from #tempAccountPackageRecurCharge where ID is not null) and ChargeDay = @ChargeDay

	UPDATE accountPackageRecurCharge
    SET accountPackageRecurCharge.ChargeAmount = tempStorage.NewChargeAmount, accountPackageRecurCharge.CurrencyID  = tempStorage.NewCurrencyId
    FROM AccountPackageRecurCharge accountPackageRecurCharge
    JOIN #tempAccountPackageRecurCharge tempStorage
    ON accountPackageRecurCharge.ID = tempStorage.ID 
    where accountPackageRecurCharge.ChargeDay = @ChargeDay

	INSERT INTO [Retail_BE].[AccountPackageRecurCharge]([AccountPackageID],[ChargeableEntityID],[BalanceAccountTypeID],[BalanceAccountID],[ChargeDay],[ChargeAmount],[CurrencyId],[TransactionTypeID],AccountID,AccountBEDefinitionId)
	select [AccountPackageID],[ChargeableEntityID],[BalanceAccountTypeID],[BalanceAccountID],[ChargeDay],[NewChargeAmount],[NewCurrencyId],[TransactionTypeID],AccountID,AccountBEDefinitionId
	FROM #tempAccountPackageRecurCharge tempStorage
	where tempStorage.ID is null
	
	DROP TABLE #tempAccountPackageRecurCharge
END