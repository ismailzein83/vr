CREATE PROCEDURE [common].[sp_City_Insert]
	@Name nvarchar(255),
	@CountryID int, 
	@id INT OUT
AS
BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM common.[City] WHERE Name = @Name)
	BEGIN
		INSERT INTO common.[City](Name, CountryID)
		VALUES (@Name, @CountryID)

		SET @id = @@IDENTITY
	END
END