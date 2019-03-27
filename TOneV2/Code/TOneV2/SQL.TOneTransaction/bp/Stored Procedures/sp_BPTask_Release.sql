-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [bp].[sp_BPTask_Release]
	@TaskID bigint
AS
BEGIN
Update [bp].[BPTask] set [TakenBy] = NULL,
						 [LastUpdatedTime] = GETDATE()
where ID = @TaskID

END