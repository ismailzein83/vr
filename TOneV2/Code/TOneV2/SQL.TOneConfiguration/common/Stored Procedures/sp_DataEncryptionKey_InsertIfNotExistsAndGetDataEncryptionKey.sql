CREATE PROCEDURE [common].[sp_DataEncryptionKey_InsertIfNotExistsAndGetDataEncryptionKey]
	@EncryptionKey nvarchar(255)
AS
BEGIN	
	
	--INSERT Encryption Key if not exists
	IF NOT EXISTS (SELECT TOP 1 NULL FROM [common].[DataEncryptionKey] WITH(NOLOCK))
	BEGIN
		INSERT INTO [common].[DataEncryptionKey] ([EncryptionKey])
		SELECT @EncryptionKey WHERE NOT EXISTS (SELECT TOP 1 NULL FROM [common].[DataEncryptionKey])
	END
	
	SELECT TOP 1 EncryptionKey FROM [common].[DataEncryptionKey]
	ORDER BY ID
END