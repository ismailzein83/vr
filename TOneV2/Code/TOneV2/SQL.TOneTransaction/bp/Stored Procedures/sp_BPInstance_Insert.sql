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
	@EntityId varchar(50),
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
		   ,EntityId)
     VALUES
           (@Title
           ,@ParentID
           ,@DefinitionID
           ,@InputArguments
		   ,@CompletionNotifier
           ,@ExecutionStatus
           ,GETDATE()
           ,@InitiatorUserId
		   ,@EntityId)
     SET @ID = SCOPE_IDENTITY()
END