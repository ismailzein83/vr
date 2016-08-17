-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--- =============================================
create PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_InsertFromLiveBalance] 
	@ClosingTime datetime,
	@UsageTransactionTypeId uniqueidentifier,
	@ClosingPeriodID bigint
AS
BEGIN
	INSERT INTO [VR_AccountBalance].[BillingTransaction]
				   ([AccountID]
				   ,[TransactionTypeID]
				   ,[Amount]
				   ,[CurrencyId]
				   ,[TransactionTime]
				   ,[IsBalanceUpdated]
				   ,[ClosingPeriodId])
		SELECT [AccountID]
			   ,@UsageTransactionTypeId
			   ,[UsageBalance]
			   ,[CurrencyID]
			   ,@ClosingTime
			   ,1
			   ,@ClosingPeriodID
		FROM [VR_AccountBalance].[LiveBalance]
		WHERE [UsageBalance] > 0

END