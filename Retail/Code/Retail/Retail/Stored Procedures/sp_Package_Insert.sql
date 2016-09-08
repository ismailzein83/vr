CREATE PROCEDURE [Retail].[sp_Package_Insert]
	@Name nvarchar(255),
	@Description nvarchar(1000),
	@Settings nvarchar(MAX), 
	@id INT OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Retail.Package WHERE Name = @Name)
	BEGIN
		INSERT INTO Retail.Package(Name,[Description],Settings)
		VALUES (@Name,@Description,@Settings)

		SET @id = SCOPE_IDENTITY()
	END
END