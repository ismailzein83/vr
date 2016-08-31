-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_RunningProcess_Insert]
	@ProcessName nvarchar(1000),
	@MachineName nvarchar(1000),
	@AdditionalInfo nvarchar(max)
AS
BEGIN
	INSERT INTO [runtime].[RunningProcess]
           ([ProcessName]
           ,[MachineName]
           ,[StartedTime]
           ,AdditionalInfo)
     VALUES
           (@ProcessName
           ,@MachineName
           ,GETDATE()
           ,@AdditionalInfo)
           
     SELECT [ID]
      ,[ProcessName]
      ,[MachineName]
      ,[StartedTime]
      ,AdditionalInfo
	 FROM [runtime].[RunningProcess]
	 WHERE ID = SCOPE_IDENTITY()
END