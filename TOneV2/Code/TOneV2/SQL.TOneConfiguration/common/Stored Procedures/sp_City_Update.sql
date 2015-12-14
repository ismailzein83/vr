CREATE PROCEDURE [common].[sp_City_Update]
	@ID int,
	@Name nvarchar(255), 
	@CountryID int
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM common.[City] WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update common.[City]
	Set Name = @Name, CountryID = @CountryID
	Where ID = @ID
	END
END