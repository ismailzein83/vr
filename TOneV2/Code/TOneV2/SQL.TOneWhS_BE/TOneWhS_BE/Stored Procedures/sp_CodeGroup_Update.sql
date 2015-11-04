-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CodeGroup_Update]
	@ID int,	
	@CountryID int,
	@Code varchar(20)
AS
BEGIN

	Update TOneWhS_BE.CodeGroup
	Set Code = @Code ,
		CountryID = @CountryID
	Where ID = @ID
END