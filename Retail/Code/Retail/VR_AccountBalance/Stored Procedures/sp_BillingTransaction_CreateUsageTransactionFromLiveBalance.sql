-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_CreateUsageTransactionFromLiveBalance] 
	@ClosingTime datetime,
	@AccountTypeID uniqueidentifier,
	@UsageTransactionTypeId uniqueidentifier,
	@ClosingPeriodID bigint
AS
BEGIN
	INSERT INTO [VR_AccountBalance].[BillingTransaction]
				   ([AccountID]
				   ,[AccountTypeID]
				   ,[TransactionTypeID]
				   ,[Amount]
				   ,[CurrencyId]
				   ,[TransactionTime]
				   ,[IsBalanceUpdated]
				   ,[ClosingPeriodId])
		SELECT [AccountID]
			   ,@AccountTypeID
			   ,@UsageTransactionTypeId
			   ,[UsageBalance]
			   ,[CurrencyID]
			   ,@ClosingTime
			   ,1
			   ,@ClosingPeriodID
		FROM [VR_AccountBalance].[LiveBalance]
		WHERE [UsageBalance] > 0

END