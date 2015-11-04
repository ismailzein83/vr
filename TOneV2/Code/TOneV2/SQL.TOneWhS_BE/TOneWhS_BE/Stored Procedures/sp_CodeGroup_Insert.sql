-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CodeGroup_Insert]
	@Code varchar(20),
	@CountryID varchar(20),
	@id INT OUT
AS
BEGIN
	INSERT INTO TOneWhS_BE.CodeGroup(Code,CountryID)
	VALUES (@Code,@CountryID)
	
	SET @id = @@IDENTITY
END