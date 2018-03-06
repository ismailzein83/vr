CREATE PROCEDURE [sec].[sp_EncryptionKey_InsertIfNotExistsAndGetEncryptionKey]
	@EncryptionKey nvarchar(255)
AS
BEGIN	
	
	--INSERT Encryption Key if not exists
	IF NOT EXISTS (SELECT TOP 1 NULL FROM sec.[EncryptionKey] WITH(NOLOCK))
	BEGIN
		INSERT INTO sec.[EncryptionKey] ([EncryptionKey])
		SELECT @EncryptionKey WHERE NOT EXISTS (SELECT TOP 1 NULL FROM sec.[EncryptionKey])
	END
	
	SELECT TOP 1 EncryptionKey FROM sec.[EncryptionKey]
	ORDER BY ID
END