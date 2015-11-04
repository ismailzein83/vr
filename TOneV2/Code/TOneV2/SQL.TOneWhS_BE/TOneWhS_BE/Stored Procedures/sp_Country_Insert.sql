-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_Country_Insert]
	@Name nvarchar(255),
	@id INT OUT
AS
BEGIN
	INSERT INTO TOneWhS_BE.Country(Name)
	VALUES (@Name)
	
	SET @id = @@IDENTITY
END