Create PROCEDURE [sec].[sp_CookieName_InsertIfNotExistsAndGetCookieName]
	@CookieName nvarchar(255)
AS
BEGIN	
	
	--INSERT Encryption Key if not exists
	IF NOT EXISTS (SELECT TOP 1 NULL FROM sec.[CookieName] WITH(NOLOCK))
	BEGIN
		INSERT INTO sec.[CookieName] ([CookieName])
		SELECT @CookieName WHERE NOT EXISTS (SELECT TOP 1 NULL FROM sec.[CookieName])
	END
	
	SELECT TOP 1 CookieName FROM sec.[CookieName]
	ORDER BY ID
END