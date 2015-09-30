-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchType_Insert]
	@Name VARCHAR(50),
	@ID INT OUT
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM PSTN_BE.SwitchType WHERE Name = @Name)
	BEGIN
		INSERT INTO PSTN_BE.SwitchType (Name) VALUES (@Name)
		
		SET @ID = @@IDENTITY
	END
END