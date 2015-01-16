-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_Insert]
	@ID uniqueidentifier,
	@Title nvarchar(1000),
	@ParentID uniqueidentifier,
	@DefinitionID int,
	@InputArguments nvarchar(max),
	@ExecutionStatus int
	
AS
BEGIN
	INSERT INTO [bp].[BPInstance]
           ([ID]
           ,[Title]
           ,[ParentID]
           ,[DefinitionID]
           ,[InputArgument]
           ,[ExecutionStatus]
           ,[StatusUpdatedTime])
     VALUES
           (@ID
           ,@Title
           ,@ParentID
           ,@DefinitionID
           ,@InputArguments
           ,@ExecutionStatus
           ,GETDATE())
END