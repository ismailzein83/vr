
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CodeGroup_Insert]
	@Code varchar(20),
	@CountryID int,
	@id INT OUT
AS
BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM dbo.[CodeGroup] WHERE Code = @Code)
	BEGIN
		INSERT INTO dbo.CodeGroup(Code,CountryID)
		VALUES (@Code,@CountryID)
		
		SET @id = @@IDENTITY
	END
END