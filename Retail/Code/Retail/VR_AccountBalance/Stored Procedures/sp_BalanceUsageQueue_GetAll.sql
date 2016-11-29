-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BalanceUsageQueue_GetAll]
	@AccountTypeID uniqueidentifier
AS
BEGIN
	SELECT ID, AccountTypeID , UsageDetails
	FROM VR_AccountBalance.BalanceUsageQueue with(nolock)
	where AccountTypeID = @AccountTypeID 
END