-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BalanceClosingPeriod_Insert] 
	@ClosingTime datetime,
	@ClosingPeriodID bigint out
AS
BEGIN
	INSERT INTO [VR_AccountBalance].[BalanceClosingPeriod] ([ClosingTime]) VALUES (@ClosingTime)
	SET @ClosingPeriodID = SCOPE_IDENTITY()
END