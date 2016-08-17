-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--- =============================================
create PROCEDURE [VR_AccountBalance].[sp_LiveBalance_UpdateBalance] 
AS
BEGIN
		UPDATE [VR_AccountBalance].[LiveBalance]
		SET InitialBalance = CurrentBalance,
			UsageBalance = 0
END