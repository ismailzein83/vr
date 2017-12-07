CREATE PROCEDURE [bp].[sp_BPTask_CancelNotCompleted]
       @ProcessInstanceId bigint
AS
BEGIN
       -- Status: 50 = Completed, 60 = Cancelled
       UPDATE	[bp].[BPTask] 
	   SET		[Status] = 60 
	   WHERE	ProcessInstanceID = @ProcessInstanceId and [Status] != 50
END