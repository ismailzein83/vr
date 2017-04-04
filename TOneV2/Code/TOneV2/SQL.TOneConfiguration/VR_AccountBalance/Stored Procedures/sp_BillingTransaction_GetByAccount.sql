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
	SELECT	[ID],AccountID,AccountTypeID, TransactionTypeID,Amount,CurrencyId,TransactionTime,Notes,Reference,IsBalanceUpdated,ClosingPeriodId, SourceID
	FROM VR_AccountBalance.BillingTransaction with(nolock)
	where AccountTypeID = @AccountTypeID 
		  AND AccountId= @AccountId
END