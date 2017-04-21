CREATE PROCEDURE [genericdata].[sp_DataRecordFieldChoice_Insert]
	@id uniqueidentifier ,
	@Name nvarchar(255),
	@Settings nvarchar(MAX)
	
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.DataRecordFieldChoice WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.DataRecordFieldChoice(ID,Name, Settings)
		VALUES (@id,@Name, @Settings)
	END
END