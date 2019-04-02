-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPTask_Assign]
	@TaskID bigint,
	@UserId int
AS
BEGIN
	Update [bp].[BPTask] set [TakenBy] = @UserId,
							 [LastUpdatedTime] = GETDATE()
	where [ID] = @TaskID AND [TakenBy] IS NULL
END