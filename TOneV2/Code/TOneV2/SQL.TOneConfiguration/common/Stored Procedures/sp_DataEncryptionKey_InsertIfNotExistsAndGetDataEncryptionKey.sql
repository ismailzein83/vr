create PROCEDURE [Common].[sp_DataEncryptionKey_InsertIfNotExistsAndGetDataEncryptionKey]
	@EncryptionKey nvarchar(255)
AS
BEGIN	
	
	--INSERT Encryption Key if not exists
	IF NOT EXISTS (SELECT TOP 1 NULL FROM [Common].DataEncryptionKey WITH(NOLOCK))
	BEGIN
		INSERT INTO [Common].DataEncryptionKey ([EncryptionKey])
		SELECT @EncryptionKey WHERE NOT EXISTS (SELECT TOP 1 NULL FROM [Common].DataEncryptionKey)
	END
	
	SELECT TOP 1 EncryptionKey FROM [Common].DataEncryptionKey
	ORDER BY ID
END