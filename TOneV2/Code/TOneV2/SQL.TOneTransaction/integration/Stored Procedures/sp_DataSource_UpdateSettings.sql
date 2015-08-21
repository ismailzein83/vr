-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_UpdateSettings]
	@DataSourceID int,
	@Settings varchar(max)
AS
BEGIN
	Update integration.DataSource 
	Set	Settings = @Settings
	Where ID = @DataSourceID
END