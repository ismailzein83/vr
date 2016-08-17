-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BalanceClosingPeriod_GetLastClosingPeriod]

AS
BEGIN
	SELECT  Top (1) ID,ClosingTime
	FROM	[VR_AccountBalance].BalanceClosingPeriod  with(nolock) order by ID desc
END