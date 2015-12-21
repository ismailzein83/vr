CREATE PROCEDURE [TOneWhS_BE].[sp_RateType_Insert]
	@Name nvarchar(255),
	@id INT OUT
AS

BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.[RateType] WHERE Name = @Name)
	BEGIN
		INSERT INTO TOneWhS_BE.RateType(Name)
		VALUES (@Name)

		SET @id = @@IDENTITY
	END
END