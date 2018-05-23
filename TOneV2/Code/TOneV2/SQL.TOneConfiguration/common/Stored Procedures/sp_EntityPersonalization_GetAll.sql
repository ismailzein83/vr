CREATE PROCEDURE [common].[sp_EntityPersonalization_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT  
	   [ID]
      ,[UserID]
      ,[EntityUniqueName]
      ,[Details]
      ,[CreatedTime]
      ,[CreatedBy]
      ,[LastModifiedTime]
      ,[LastModifiedBy]
  FROM [common].[EntityPersonalization] WITH(NOLOCK) 
END