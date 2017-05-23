-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE common.sp_CacheRefreshHandle_Update
	@CacheTypeName nvarchar(255)
AS
BEGIN
	IF NOT EXISTS (SELECT NULL FROM [common].[CacheRefreshHandle] WITH(NOLOCK) WHERE [CacheTypeName] = @CacheTypeName)
	BEGIN
		BEGIN TRY
			INSERT INTO [common].[CacheRefreshHandle] ([CacheTypeName])
			SELECT @CacheTypeName WHERE NOT EXISTS (SELECT NULL FROM [common].[CacheRefreshHandle] WHERE [CacheTypeName] = @CacheTypeName)
		END TRY
		BEGIN CATCH
			 
		END CATCH		
	END

    Update	[common].[CacheRefreshHandle] --the purpose of this procedure is to update the timestamp
	SET		[LastUpdateTime] = GETDATE()
	WHERE	[CacheTypeName] = @CacheTypeName

END