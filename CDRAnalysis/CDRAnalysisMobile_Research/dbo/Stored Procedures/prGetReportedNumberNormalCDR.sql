-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[prGetReportedNumberNormalCDR]
	-- Add the parameters for the stored procedure here
	@ReportID int =NULL 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	

--	SELECT DISTINCT  0 AS Id, dbo.NormalCDR.A_Temp, dbo.NormalCDR.In_Trunk, dbo.NormalCDR.Switch, dbo.ReportDetails.ReportId AS ReportID
--FROM         dbo.NormalCDR INNER JOIN
--                      dbo.ReportDetails ON dbo.NormalCDR.A_Temp = dbo.ReportDetails.SubscriberNumber
--WHERE     (dbo.ReportDetails.ReportId = @ReportId)
--ORDER BY dbo.NormalCDR.A_Temp,  dbo.NormalCDR.Switch,dbo.NormalCDR.In_Trunk
     
	
END