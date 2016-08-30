CREATE PROCEDURE [TOneWhS_Routing].[sp_RoutingDatabase_GetNotDeleted] 
AS
BEGIN
	SELECT [ID]
      ,[Title]
      ,[Type]
      ,[ProcessType]
      ,[EffectiveTime]
      ,[IsReady]
      ,[CreatedTime]
      ,[ReadyTime]
      ,[Information]
      ,[Settings]
  FROM TOneWhS_Routing.[RoutingDatabase] WITH(NOLOCK) 
  WHERE ISNULL(IsDeleted, 0) = 0
END


SET ANSI_NULLS ON