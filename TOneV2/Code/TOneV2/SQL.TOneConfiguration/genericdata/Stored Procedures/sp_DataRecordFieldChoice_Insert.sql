CREATE PROCEDURE [genericdata].[sp_DataRecordFieldChoice_Insert]
	@id uniqueidentifier ,
	@Name nvarchar(255),
	@DevProjectId uniqueidentifier,
	@Settings nvarchar(MAX)
	
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.DataRecordFieldChoice WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.DataRecordFieldChoice(ID,Name,DevProjectId, Settings)
		VALUES (@id,@Name,@DevProjectId, @Settings)
	END
END