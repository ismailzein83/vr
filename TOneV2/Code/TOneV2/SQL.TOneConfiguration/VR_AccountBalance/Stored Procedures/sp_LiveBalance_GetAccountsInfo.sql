-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_GetAccountsInfo]
	@AccountTypeID uniqueidentifier
AS
BEGIN
	SELECT ID,AccountID, CurrencyID
	FROM VR_AccountBalance.LiveBalance  with(nolock)
	where AccountTypeID = @AccountTypeID 
END