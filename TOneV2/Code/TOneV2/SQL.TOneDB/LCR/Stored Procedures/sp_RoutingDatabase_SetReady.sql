-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,
-- Description:	<Description,,
-- =============================================
CREATE PROCEDURE [LCR].[sp_RoutingDatabase_SetReady] 
   @ID int
AS
BEGIN
	UPDATE [LCR].[RoutingDatabase]
    SET IsReady = 1,
		ReadyTime = GETDATE()
	WHERE ID = @ID
END