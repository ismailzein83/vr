-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_GetErrorData]
	@AccountTypeID uniqueidentifier,
	@TransactionTypeId uniqueidentifier,
	@CorrectionProcessId uniqueidentifier,
	@PeriodStart Datetime
AS
BEGIN
Select au.AccountID,
	   au.AccountTypeID,
	   au.CorrectionProcessID,
	   au.CurrencyId,
	   au.ID,
	   au.PeriodEnd,
	   au.PeriodStart,
	   au.TransactionTypeID,
	   au.UsageBalance
FROM [VR_AccountBalance].AccountUsage au
	WHERE AccountTypeID = @AccountTypeID AND 
	      TransactionTypeID = @TransactionTypeId AND 
		  PeriodStart = @PeriodStart AND 
		  (CorrectionProcessID IS NULL OR CorrectionProcessID != @CorrectionProcessId)
END