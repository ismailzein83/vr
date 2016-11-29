-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BalanceClosingPeriod_Insert] 
	@ClosingTime datetime,
	@AccountTypeID uniqueidentifier,
	@ClosingPeriodID bigint out
AS
BEGIN
	INSERT INTO [VR_AccountBalance].[BalanceClosingPeriod] ([ClosingTime], [AccountTypeID]) VALUES (@ClosingTime, @AccountTypeID)
	SET @ClosingPeriodID = SCOPE_IDENTITY()
END