CREATE PROCEDURE [integration].[sp_DataSourceState_Update] 
	@DataSourceID uniqueidentifier,
	@DataSourceState nvarchar(max)
AS
BEGIN
	
	Update integration.DataSourceState
	SET [State] = @DataSourceState
    WHERE DataSourceID = @DataSourceID
	
END