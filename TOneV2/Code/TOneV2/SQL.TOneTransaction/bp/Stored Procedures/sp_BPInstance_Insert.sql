-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_Insert]
	@Title nvarchar(1000),
	@ParentID bigint,
	@DefinitionID uniqueidentifier,
	@InputArguments nvarchar(max),
	@CompletionNotifier nvarchar(max),
	@ExecutionStatus int,
	@InitiatorUserId int,
	@EntityId varchar(255),
	@ViewRequiredPermissionSetId int,
	@TaskId uniqueidentifier,
	@ID bigint out
	
AS
BEGIN
	INSERT INTO [bp].[BPInstance]
           ([Title]
           ,[ParentID]
           ,[DefinitionID]
           ,[InputArgument]
		   ,[CompletionNotifier]
           ,[ExecutionStatus]
           ,[StatusUpdatedTime]
           ,[InitiatorUserId]
		   ,EntityId
		   ,ViewRequiredPermissionSetId
		   ,TaskId)
     VALUES
           (@Title
           ,@ParentID
           ,@DefinitionID
           ,@InputArguments
		   ,@CompletionNotifier
           ,@ExecutionStatus
           ,GETDATE()
           ,@InitiatorUserId
		   ,@EntityId
		   ,@ViewRequiredPermissionSetId
		   ,@TaskId)
     SET @ID = SCOPE_IDENTITY()
END