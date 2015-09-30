-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchType_Update]
	@ID INT,
	@Name VARCHAR(50)
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM PSTN_BE.SwitchType WHERE Name = @Name AND ID != @ID)
	BEGIN
		UPDATE PSTN_BE.SwitchType
		SET Name = @Name
		WHERE ID = @ID
	END
END