CREATE PROCEDURE [genericdata].[sp_DataRecordFieldChoice_Insert]
	@Name nvarchar(255),
	@Settings nvarchar(MAX), 
	@id INT OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.DataRecordFieldChoice WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.DataRecordFieldChoice(Name, Settings)
		VALUES (@Name, @Settings)

		SET @id = SCOPE_IDENTITY()
	END
END