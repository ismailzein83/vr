-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_Country_Update]
	@ID int,
	@Name nvarchar(255)
AS
BEGIN

	Update TOneWhS_BE.Country
	Set Name = @Name
	Where ID = @ID
END