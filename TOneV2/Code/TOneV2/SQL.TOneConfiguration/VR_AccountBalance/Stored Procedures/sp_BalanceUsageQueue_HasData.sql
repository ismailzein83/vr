-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BalanceUsageQueue_HasData]
	@AccountTypeID uniqueidentifier
AS
BEGIN
	IF EXISTS(Select TOP 1 ID from [VR_AccountBalance].[BalanceUsageQueue] WITH(NOLOCK) where AccountTypeID = @AccountTypeID)
	BEGIN
		SELECT 1
	END

	ELSE
	BEGIN
		SELECT 0
	END
END