-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_GetByAccount]
	@AccountTypeID uniqueidentifier,
	@AccountId nvarchar(255)

AS
BEGIN
		SELECT ID, AccountTypeID,TransactionTypeID, AccountID,CurrencyId,PeriodStart,PeriodEnd,UsageBalance
	FROM VR_AccountBalance.AccountUsage with(nolock)
	where AccountTypeID = @AccountTypeID 
		  AND AccountId= @AccountId
END