-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_AccountBalance].sp_BalanceUsageQueue_GetAll
AS
BEGIN
	SELECT ID, UsageDetails
	FROM VR_AccountBalance.BalanceUsageQueue
END