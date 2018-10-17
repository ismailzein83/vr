-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_CacheRefreshHandle_GetByCacheTypeName]
	@CacheTypeName nvarchar(255)
AS
BEGIN
	SET NOCOUNT ON;

    SELECT [CacheTypeName]
		  ,[CreatedTime]
		  ,[LastUpdateTime]
		  ,[timestamp]
	FROM [common].[CacheRefreshHandle]
	WHERE	[CacheTypeName] = @CacheTypeName
END