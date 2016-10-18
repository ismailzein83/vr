-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPDefinitionState_Insert]
	@DefinitionID uniqueidentifier,
	@ObjectKey varchar(255),
	@ObjectValue nvarchar(max)
AS
BEGIN
	INSERT INTO [bp].[BPDefinitionState]
           ([DefinitionID]
           ,[ObjectKey]
           ,[ObjectValue])
     VALUES
           (@DefinitionID
           ,@ObjectKey
           ,@ObjectValue)
END