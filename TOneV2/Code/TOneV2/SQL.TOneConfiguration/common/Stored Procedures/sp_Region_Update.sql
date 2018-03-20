CREATE PROCEDURE [common].[sp_Region_Update]
	@ID int,
	@Name nvarchar(255), 
	@CountryID int,
	@Settings nvarchar(max),
	@LastModifiedBy int
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM common.[Region] WHERE ID != @ID AND Name = @Name and CountryID=@CountryID)
	BEGIN
		Update common.[Region]
	Set Name = @Name, CountryID = @CountryID , Settings = @Settings, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
	Where ID = @ID
	END
END