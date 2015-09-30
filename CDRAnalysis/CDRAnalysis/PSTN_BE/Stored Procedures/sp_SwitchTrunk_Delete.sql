-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchTrunk_Delete]
	@ID INT
AS
BEGIN
	--IF NOT EXISTS (SELECT 1 FROM PSTN_BE.SwitchTrunkLink WHERE Trunk1ID = @ID OR Trunk2ID = @ID)
	--BEGIN
		DELETE FROM PSTN_BE.SwitchTrunk WHERE ID = @ID
	--END
END