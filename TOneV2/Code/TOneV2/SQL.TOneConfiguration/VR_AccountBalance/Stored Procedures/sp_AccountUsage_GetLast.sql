-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_AccountBalance].[sp_AccountUsage_GetLast]
	@AccountTypeID uniqueidentifier,
	@AccountId nvarchar(255)
AS
BEGIN
	SELECT TOP(1) au.ID, au.AccountTypeID,TransactionTypeID, au.AccountID,au.CurrencyId,PeriodStart,PeriodEnd,UsageBalance, IsOverridden, OverriddenAmount, CorrectionProcessID
	FROM VR_AccountBalance.AccountUsage au WITH(NOLOCK)
	WHERE  ISNULL(IsOverridden, 0) = 0
		   AND au.AccountTypeID = @AccountTypeID 
		   AND au.AccountId= @AccountId
	ORDER BY PeriodEnd DESC
END