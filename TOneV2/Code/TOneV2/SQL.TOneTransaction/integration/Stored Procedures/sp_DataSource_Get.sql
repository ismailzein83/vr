-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_Get] 
@DataSourceID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [ID]
      ,[Name]
      ,[AdapterID]
      ,[TaskId]
      ,[Settings]
       from integration.DataSource
       where [ID] = @DataSourceID
END