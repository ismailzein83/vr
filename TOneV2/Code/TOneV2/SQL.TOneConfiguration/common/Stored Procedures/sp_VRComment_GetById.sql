
CREATE PROCEDURE [common].[sp_VRComment_GetById]
@VRCommentId bigint
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
  FROM [common].[VRComment]  WHERE (ID = @VRCommentId );
END