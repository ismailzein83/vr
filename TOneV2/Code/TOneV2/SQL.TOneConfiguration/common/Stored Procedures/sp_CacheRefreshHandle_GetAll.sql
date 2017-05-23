-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_CacheRefreshHandle_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [CacheTypeName]
      ,[CreatedTime]
      ,[timestamp]
	FROM [common].[CacheRefreshHandle] WITH (NOLOCK)

END