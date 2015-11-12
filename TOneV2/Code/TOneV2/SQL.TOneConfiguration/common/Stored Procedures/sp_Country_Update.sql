-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_Country_Update]
	@ID int,
	@Name nvarchar(255)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM common.[Country] WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update common.[Country]
	Set Name = @Name
	Where ID = @ID
	END
END