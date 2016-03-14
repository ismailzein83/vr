-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPTask_Insert]
	@Title nvarchar(1000),
	@ProcessInstanceId bigint,
	@TypeId int,
	@TaskInformation nvarchar(max),
	@Status int,
	@AssignedUsers varchar(max),
	@ID bigint out
	
AS
BEGIN
	INSERT INTO [bp].[BPTask]
           ([Title]
           ,[ProcessInstanceID]
           ,[TypeID] 
           ,[TaskInformation] 
           ,[Status] 
           ,[AssignedUsers] 
           ,[LastUpdatedTime])
     VALUES
           (@Title
           ,@ProcessInstanceId
           ,@TypeId
           ,@TaskInformation
           ,@Status
           ,@AssignedUsers
           ,GETDATE())
     SET @ID = @@identity
END