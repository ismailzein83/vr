-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_GetAll] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DS.[ID]
      ,DS.[Name]
      ,DS.[AdapterID]
      ,DS.[AdapterState]
      ,AD.[Info]
      ,DS.[TaskId]
      ,DS.[Settings]
       from integration.DataSource as DS
       Join AdapterType as AD on DS.AdapterID = AD.ID
END