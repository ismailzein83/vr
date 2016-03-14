-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPTask_GetByID]
	@TaskID bigint
	
AS
BEGIN
Select [ID], [ProcessInstanceID], [TypeID] , [Title] , [AssignedUsers] , [ExecutedBy] , [Status] , 
	   [TaskInformation] ,[TaskExecutionInformation] , [CreatedTime] , [LastUpdatedTime]
from [bp].[BPTask]
where ID = @TaskID 
END