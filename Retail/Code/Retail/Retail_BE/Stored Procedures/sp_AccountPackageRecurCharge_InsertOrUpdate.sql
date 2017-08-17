
CREATE PROCEDURE [Retail_BE].[sp_AccountPackageRecurCharge_InsertOrUpdate]
@EffectiveDate Datetime,
@ProcessInstanceID BIGINT,
@AccountPackageRecurCharges [Retail_BE].[AccountPackageRecurChargeType] Readonly
AS
BEGIN
    SELECT t2.[AccountPackageID], t2.[ChargeableEntityID], t2.[BalanceAccountTypeID], t2.[BalanceAccountID], t2.[ChargeDay], t2.[ChargeAmount] as NewChargeAmount, t2.[CurrencyID] as NewCurrencyId,
	t2.[TransactionTypeID],  t1.ID, t1.ChargeAmount as OldChargeAmount, t1.CurrencyID as OldCurrencyId
	INTO #tempAccountPackageRecurCharge
	FROM  @AccountPackageRecurCharges t2
	left join [Retail_BE].[AccountPackageRecurCharge] t1 on t1.AccountPackageID = t2.AccountPackageID and t1.ChargeableEntityID = t2.ChargeableEntityID and t1.BalanceAccountTypeID = t2.BalanceAccountTypeID 
	and t1.BalanceAccountID = t2.BalanceAccountID and t1.ChargeDay = t2.ChargeDay and t1.TransactionTypeID = t2.TransactionTypeID

	Update accountPackageRecurCharge
	set IsSentToAccountBalance = 0, ProcessInstanceID = @ProcessInstanceID
	from [Retail_BE].[AccountPackageRecurCharge] accountPackageRecurCharge
    LEFT JOIN #tempAccountPackageRecurCharge tempStorage on accountPackageRecurCharge.ID = tempStorage.ID and tempStorage.ID is not null
	WHERE tempStorage.ID is null

	INSERT INTO [Retail_BE].[AccountPackageRecurCharge]([AccountPackageID],[ChargeableEntityID],[BalanceAccountTypeID],[BalanceAccountID],[ChargeDay],[ChargeAmount],[CurrencyId],[TransactionTypeID],[ProcessInstanceID])
	select [AccountPackageID],[ChargeableEntityID],[BalanceAccountTypeID],[BalanceAccountID],[ChargeDay],[NewChargeAmount],[NewCurrencyId],[TransactionTypeID],@ProcessInstanceID
	FROM #tempAccountPackageRecurCharge tempStorage
	where tempStorage.ID is null
	
	DELETE from #tempAccountPackageRecurCharge where Id is null or (NewChargeAmount = OldChargeAmount and NewCurrencyID  = OldCurrencyId)

	UPDATE accountPackageRecurCharge
    SET accountPackageRecurCharge.ChargeAmount = tempStorage.NewChargeAmount, accountPackageRecurCharge.CurrencyID  = tempStorage.NewCurrencyId, accountPackageRecurCharge.ProcessInstanceID= @ProcessInstanceID,
	accountPackageRecurCharge.IsSentToAccountBalance = 0
    FROM AccountPackageRecurCharge accountPackageRecurCharge
    JOIN #tempAccountPackageRecurCharge tempStorage
    ON accountPackageRecurCharge.ID = tempStorage.ID 
    
	DROP TABLE #tempAccountPackageRecurCharge
END