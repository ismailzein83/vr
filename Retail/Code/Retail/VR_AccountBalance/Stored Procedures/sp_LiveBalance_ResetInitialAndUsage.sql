-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_ResetInitialAndUsage] 
	@AccountTypeID uniqueidentifier
AS
BEGIN
		UPDATE [VR_AccountBalance].[LiveBalance]
		SET InitialBalance = CurrentBalance,
			UsageBalance = 0
		WHERe AccountTypeID = @AccountTypeID 
END