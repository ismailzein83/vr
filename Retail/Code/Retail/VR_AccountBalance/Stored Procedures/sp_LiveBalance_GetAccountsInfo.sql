-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_AccountBalance].[sp_LiveBalance_GetAccountsInfo]
AS
BEGIN
	SELECT AccountID, CurrencyID
	FROM VR_AccountBalance.LiveBalance
END