-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE runtime.sp_RunningProcess_Insert
	@ProcessName nvarchar(1000),
	@MachineName nvarchar(1000)
AS
BEGIN
	INSERT INTO [runtime].[RunningProcess]
           ([ProcessName]
           ,[MachineName]
           ,[StartedTime]
           ,[LastHeartBeatTime])
     VALUES
           (@ProcessName
           ,@MachineName
           ,GETDATE()
           ,GETDATE())
           
     SELECT [ID]
      ,[ProcessName]
      ,[MachineName]
      ,[StartedTime]
      ,[LastHeartBeatTime]
	 FROM [runtime].[RunningProcess]
	 WHERE ID = @@identity
END