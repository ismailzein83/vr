CREATE PROCEDURE [LCR].[sp_RoutingDatabase_GetNotDeleted] 
AS
BEGIN
	SELECT [ID]
      ,[Title]
      ,[Type]
      ,[EffectiveTime]
      ,[IsReady]
      ,[CreatedTime]
      ,[ReadyTime]
	  ,[IsLcrOnly]
  FROM [LCR].[RoutingDatabase]
  WHERE ISNULL(IsDeleted, 0) = 0
END