-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BalanceClosingPeriod_GetLastClosingPeriod]
	@AccountTypeID uniqueidentifier
AS
BEGIN
	SELECT  Top (1) ID, AccountTypeID, ClosingTime
	FROM	[VR_AccountBalance].BalanceClosingPeriod  with(nolock)
	where AccountTypeID = @AccountTypeID 
	 order by ID desc
END