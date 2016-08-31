-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_BigDataService_GetAll]
AS
BEGIN
	SELECT [ID]
      ,[ServiceURL]
      ,[RuntimeProcessID]
      ,[TotalCachedRecordsCount]
      ,[CachedObjectIds]
   FROM [common].[BigDataService] WITH(NOLOCK) 
END