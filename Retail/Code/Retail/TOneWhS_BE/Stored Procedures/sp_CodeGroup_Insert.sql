-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CodeGroup_Insert]
	@Code varchar(20),
	@CountryID int,
	@id INT OUT
AS
BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.[CodeGroup] WHERE Code = @Code)
	BEGIN
		INSERT INTO TOneWhS_BE.CodeGroup(Code,CountryID)
		VALUES (@Code,@CountryID)
		
		SET @id = SCOPE_IDENTITY()
	END
END