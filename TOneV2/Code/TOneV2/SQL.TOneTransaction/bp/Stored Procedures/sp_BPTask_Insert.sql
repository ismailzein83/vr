-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPTask_Insert]
	@Title nvarchar(1000),
	@ProcessInstanceId bigint,
	@TypeId uniqueidentifier,
	@TaskData nvarchar(max),
	@Status int,
	@AssignedUsers varchar(max),
	@AssignedUsersDescription nvarchar(max),
	@ID bigint out
	
AS
BEGIN
	INSERT INTO [bp].[BPTask]
           ([Title]
           ,[ProcessInstanceID]
           ,[TypeID] 
           ,[TaskData] 
           ,[Status] 
           ,[AssignedUsers]
           ,[AssignedUsersDescription] 
           ,[LastUpdatedTime])
     VALUES
           (@Title
           ,@ProcessInstanceId
           ,@TypeId
           ,@TaskData
           ,@Status
           ,@AssignedUsers
           ,@AssignedUsersDescription
           ,GETDATE())
     SET @ID = SCOPE_IDENTITY()
END