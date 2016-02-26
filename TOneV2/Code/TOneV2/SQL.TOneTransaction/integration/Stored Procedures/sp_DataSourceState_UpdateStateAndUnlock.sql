-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE integration.sp_DataSourceState_UpdateStateAndUnlock 
	@DataSourceID int,
	@State nvarchar(max)
AS
BEGIN
	
	Update integration.DataSourceState
	SET [State] = @State,
		LockedByProcessID = NULL
	WHERE DataSourceID = @DataSourceID
	
END