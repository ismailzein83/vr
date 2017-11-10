-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CodeGroup_Update]
	@ID int,	
	@CountryID int,
	@Code varchar(20),
	@Name nvarchar(200)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.[CodeGroup] WHERE ID != @ID AND Code = @Code)
	BEGIN
		Update TOneWhS_BE.CodeGroup
	Set Code = @Code ,
		CountryID = @CountryID,
		Name = 	@Name
	Where ID = @ID
	END
END