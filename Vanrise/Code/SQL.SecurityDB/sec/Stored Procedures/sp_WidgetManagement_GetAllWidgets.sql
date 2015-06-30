-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_WidgetManagement_GetAllWidgets] 
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
select wm.Id,wm.WidgetDefinitionId,wm.Name as WidgetName,wm.Setting,wd.Name as WidgetDefinitionName,wd.DirectiveName as DirectiveName from sec.WidgetManagement wm LEFT JOIN sec.WidgetDefinition wd ON wm.WidgetDefinitionId=wd.ID
END