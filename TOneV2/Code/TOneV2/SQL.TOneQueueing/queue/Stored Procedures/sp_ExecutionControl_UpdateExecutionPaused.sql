-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].sp_ExecutionControl_UpdateExecutionPaused
	@IsPaused bit	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT top 1 null from [queue].[ExecutionControl] WHERE ID = 1)
		INSERT INTO [queue].[ExecutionControl]
		(ID, IsPaused)
		VALUES
		(1, @IsPaused)
	ELSE
		UPDATE [queue].[ExecutionControl]
		SET IsPaused = @IsPaused
		WHERE ID = 1    
END