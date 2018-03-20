-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_Country_Insert]	
	@ID INT OUT,
	@Name nvarchar(255),
	@CreatedBy int,
	@LastModifiedBy int
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.[Country] WHERE Name = @Name)
	BEGIN
		INSERT INTO common.[Country](ID,Name, CreatedBy, LastModifiedBy, LastModifiedTime)
		VALUES (@ID,@Name, @CreatedBy, @LastModifiedBy, GETDATE())

	END
END