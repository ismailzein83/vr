-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--- =============================================
create PROCEDURE [VR_AccountBalance].[sp_BalanceClosingPeriod_Insert] 
	@ClosingTime datetime,
	@ClosingPeriodID bigint out
AS
BEGIN
	INSERT INTO [VR_AccountBalance].[BalanceClosingPeriod] ([ClosingTime]) VALUES (@ClosingTime)
	SET @ClosingPeriodID = @@identity
END