-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_Country_Insert]
	@Name nvarchar(255),
	@id INT OUT
AS
BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM common.[Country] WHERE Name = @Name)
	BEGIN
		INSERT INTO common.[Country](Name)
		VALUES (@Name)

		SET @id = @@IDENTITY
	END
END