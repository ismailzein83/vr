-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_UpdateAdapterState]
	@DataSourceID int,
	@AdapterState varchar(1000)
AS
BEGIN
	Update integration.DataSource 
	Set	AdapterState = @AdapterState
	Where ID = @DataSourceID
END