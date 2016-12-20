-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_AccountBalance].sp_AccountUsage_GetByPeriod
	@AccountTypeID uniqueidentifier,
	@PeriodStart Datetime
AS
BEGIN
	SELECT ID, AccountID
	FROM VR_AccountBalance.AccountUsage with(nolock)
	where AccountTypeID = @AccountTypeID AND PeriodStart = @PeriodStart
END