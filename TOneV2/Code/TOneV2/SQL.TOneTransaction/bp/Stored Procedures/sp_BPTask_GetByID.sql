-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPTask_GetByID]
	@TaskID bigint
	
AS
BEGIN
Select	[ID], [ProcessInstanceID], [TypeID] , [Title] , [AssignedUsers], [AssignedUsersDescription], [ExecutedBy] , [Status] , 
		[TaskData] ,[TaskExecutionInformation] , [CreatedTime] , [LastUpdatedTime], [Notes], [Decision]
from	[bp].[BPTask] WITH(NOLOCK) 
where	ID = @TaskID 
END