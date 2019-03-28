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
	IF NOT EXISTS(SELECT 1 from [bp].[BPTask] WITH(NOLOCK) where ID = @TaskID AND [TakenBy] IS NOT NULL)
	BEGIN
	Update [bp].[BPTask] set [TakenBy] = @UserId,
							 [LastUpdatedTime] = GETDATE()
	where ID = @TaskID AND NOT EXISTS(SELECT 1 from [bp].[BPTask] WITH(NOLOCK) where ID = @TaskID AND [TakenBy] IS NOT NULL)
	END
END