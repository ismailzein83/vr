﻿CREATE PROCEDURE [common].[sp_Region_Insert]
	@Name nvarchar(255),
	@CountryID int, 	
	@Settings nvarchar(max),
	@id INT OUT
AS
BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM common.[Region] WHERE Name = @Name and CountryID=@CountryID)
	BEGIN
		INSERT INTO common.[Region](Name, CountryID,Settings)
		VALUES (@Name, @CountryID,@Settings)

		SET @id = SCOPE_IDENTITY()
	END
END