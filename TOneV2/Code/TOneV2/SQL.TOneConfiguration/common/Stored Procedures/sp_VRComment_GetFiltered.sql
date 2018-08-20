
CREATE PROCEDURE [common].[sp_VRComment_GetFiltered]
@DefinitionId uniqueidentifier,
	@ObjectId VARCHAR(50)
AS
BEGIN
	SELECT [ID]
      ,[DefinitionId]
      ,[ObjectId]
      ,[Content]
      ,[CreatedBy]
      ,[CreatedTime]
      ,[LastModifiedBy]
      ,[LastModifiedTime]
  FROM [common].[VRComment]  WHERE (DefinitionId = @DefinitionId AND  ObjectId=@ObjectId) order by CreatedTime desc;
END