

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Analytic].[sp_AnalyticReport_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT rtr.[ID],rtr.[Name],rtr.[UserID],rtr.AccessType,rtr.[Settings]
    FROM [Analytic].AnalyticReport rtr WITH(NOLOCK) 
END