-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [bp].[sp_BPTask_UpdateTask]
	@TaskID bigint,
	@TaskData nvarchar(max)
AS
BEGIN
Update [bp].[BPTask] set [TaskData] = @TaskData,
						 [LastUpdatedTime] = GETDATE()
where ID = @TaskID

END