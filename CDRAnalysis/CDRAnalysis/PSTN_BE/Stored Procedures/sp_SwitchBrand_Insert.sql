
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchBrand_Insert]
	@Name VARCHAR(50),
	@ID INT OUT
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM PSTN_BE.SwitchBrand WHERE Name = @Name)
	BEGIN
		INSERT INTO PSTN_BE.SwitchBrand (Name) VALUES (@Name)
		
		SET @ID = @@IDENTITY
	END
END