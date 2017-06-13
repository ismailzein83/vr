-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetByAccount]
	@AccountTypeID uniqueidentifier,
	@AccountId nvarchar(255)

AS
BEGIN
	SELECT	[ID],AccountID,AccountTypeID, TransactionTypeID,Amount,CurrencyId,TransactionTime,Notes,Reference,IsBalanceUpdated,ClosingPeriodId, SourceID, Settings, IsDeleted, IsSubtractedFromBalance
	FROM VR_AccountBalance.BillingTransaction with(nolock)
	where isnull(IsDeleted, 0) = 0
		and AccountTypeID = @AccountTypeID 
		AND AccountId= @AccountId
		  
END