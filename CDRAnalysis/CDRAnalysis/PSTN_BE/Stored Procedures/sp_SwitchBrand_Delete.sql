-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchBrand_Delete]
	@ID INT
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM PSTN_BE.Switch WHERE TypeID = @ID)
	BEGIN
		DELETE FROM PSTN_BE.SwitchBrand WHERE ID = @ID
	END
END