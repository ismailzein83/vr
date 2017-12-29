CREATE PROCEDURE [integration].[sp_DataSourceState_Insert] 
	@DataSourceID uniqueidentifier,
	@DataSourceState nvarchar(max)
AS
BEGIN
	
	INSERT INTO integration.DataSourceState
	(DataSourceID, [State])
	VALUES
	(@DataSourceID, @DataSourceState)
	
END