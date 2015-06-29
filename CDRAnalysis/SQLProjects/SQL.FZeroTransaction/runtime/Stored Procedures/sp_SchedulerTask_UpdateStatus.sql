-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].sp_SchedulerTask_UpdateStatus
	@ID int,
	@Status int
AS
BEGIN
	UPDATE runtime.ScheduleTask
	SET [Status] = @Status
	WHERE ID = @ID
END