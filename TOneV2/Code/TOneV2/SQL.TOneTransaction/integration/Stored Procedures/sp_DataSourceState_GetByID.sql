CREATE PROCEDURE [integration].[sp_DataSourceState_GetByID] 
	@DataSourceID uniqueidentifier
AS
BEGIN
	
	SELECT [State]
    FROM integration.DataSourceState WITH(NOLOCK)
    WHERE DataSourceID = @DataSourceID
	
END