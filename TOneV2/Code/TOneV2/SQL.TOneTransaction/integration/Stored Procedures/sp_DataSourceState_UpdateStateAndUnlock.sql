-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSourceState_UpdateStateAndUnlock] 
	@DataSourceID uniqueidentifier,
	@State nvarchar(max)
AS
BEGIN
	
	Update integration.DataSourceState
	SET [State] = @State,
		LockedByProcessID = NULL
	WHERE DataSourceID = @DataSourceID
	
END