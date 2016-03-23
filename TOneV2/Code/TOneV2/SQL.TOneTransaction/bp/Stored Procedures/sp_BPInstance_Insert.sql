-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_Insert]
	@Title nvarchar(1000),
	@ParentID bigint,
	@DefinitionID int,
	@InputArguments nvarchar(max),
	@ExecutionStatus int,
	@InitiatorUserId int,
	@ID bigint out
	
AS
BEGIN
	INSERT INTO [bp].[BPInstance]
           ([Title]
           ,[ParentID]
           ,[DefinitionID]
           ,[InputArgument]
           ,[ExecutionStatus]
           ,[StatusUpdatedTime]
           ,[InitiatorUserId])
     VALUES
           (@Title
           ,@ParentID
           ,@DefinitionID
           ,@InputArguments
           ,@ExecutionStatus
           ,GETDATE()
           ,@InitiatorUserId)
     SET @ID = @@identity
END