-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_OrgChartLinkedEntity_GetLinkedOrgChartId]
	@LinkedEntityIdentifier varchar(850)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [OrgChartID]
       from sec.OrgChartLinkedEntity where LinkedEntityIdentifier = @LinkedEntityIdentifier
END