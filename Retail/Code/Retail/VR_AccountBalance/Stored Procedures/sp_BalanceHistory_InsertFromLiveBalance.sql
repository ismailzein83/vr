-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BalanceHistory_InsertFromLiveBalance] 
	@ClosingPeriodID bigint,
	@AccountTypeId uniqueidentifier
AS
BEGIN
		INSERT INTO [VR_AccountBalance].[BalanceHistory]
				   ([ClosingPeriodID]
				   ,[AccountID]
				   ,[ClosingBalance])
		SELECT  @ClosingPeriodID
			   ,[AccountID]
			   ,CurrentBalance
		FROM [VR_AccountBalance].[LiveBalance]
		where AccountTypeID = @AccountTypeId
		
END