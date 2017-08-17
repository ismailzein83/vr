
CREATE PROCEDURE [Retail_BE].[sp_AccountPackageRecurCharge_GetNotSentData]
@EffectiveDate Datetime
AS
BEGIN
	;WITH groupingKeys AS (
	  SELECT BalanceAccountTypeID, BalanceAccountID, ChargeDay, TransactionTypeID
	  FROM [Retail_BE].[AccountPackageRecurCharge]
	  WHERE ChargeDay>=@EffectiveDate
	  GROUP BY BalanceAccountTypeID, BalanceAccountID, ChargeDay, TransactionTypeID
	),

	tempAccountPackageRecurCharge AS (
	  SELECT gk.BalanceAccountTypeID, gk.BalanceAccountID, gk.ChargeDay, gk.TransactionTypeID
	  FROM groupingKeys gk
	  JOIN [Retail_BE].[AccountPackageRecurCharge] recurrChar on gk.BalanceAccountID = recurrChar.BalanceAccountID AND gk.BalanceAccountTypeID = recurrChar.BalanceAccountTypeID 
	  AND gk.ChargeDay = recurrChar.ChargeDay AND gk.TransactionTypeID = recurrChar.TransactionTypeID
	  WHERE recurrChar.IsSentToAccountBalance is null OR recurrChar.IsSentToAccountBalance = 0
	)

	SELECT [ID],[AccountPackageID],[ChargeableEntityID],recurrChar.[BalanceAccountTypeID],recurrChar.[BalanceAccountID],recurrChar.[ChargeDay],[ChargeAmount],[CurrencyID],recurrChar.[TransactionTypeID],
		   [ProcessInstanceID],[IsSentToAccountBalance],[CreatedTime]
	FROM [Retail_BE].[AccountPackageRecurCharge] recurrChar
	JOIN tempAccountPackageRecurCharge tempRecurrChar on tempRecurrChar.BalanceAccountID = recurrChar.BalanceAccountID AND tempRecurrChar.BalanceAccountTypeID = recurrChar.BalanceAccountTypeID 
	AND tempRecurrChar.ChargeDay = recurrChar.ChargeDay AND tempRecurrChar.TransactionTypeID = recurrChar.TransactionTypeID
END