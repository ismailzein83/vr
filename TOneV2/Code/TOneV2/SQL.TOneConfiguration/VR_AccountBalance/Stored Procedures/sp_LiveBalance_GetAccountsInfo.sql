-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_GetAccountsInfo]
	@AccountTypeID uniqueidentifier
AS
BEGIN
	SELECT ID,AccountID, CurrencyID,[Status],BED,EED
	FROM VR_AccountBalance.LiveBalance  with(nolock)
	where AccountTypeID = @AccountTypeID AND ISNULL(IsDeleted,0) = 0
END