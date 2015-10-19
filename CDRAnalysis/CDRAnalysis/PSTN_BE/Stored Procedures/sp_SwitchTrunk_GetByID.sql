
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchTrunk_GetByID]
	@ID INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT t.ID,
		t.Name,
		t.Symbol,
		t.SwitchID,
		s.Name AS SwitchName,
		t.[Type],
		t.Direction,
		t.LinkedToTrunkID,
		linkedTrunks.Name AS LinkedToTrunkName
		
	FROM PSTN_BE.SwitchTrunk t
	INNER JOIN PSTN_BE.Switch s ON s.ID = t.SwitchID
	LEFT JOIN PSTN_BE.SwitchTrunk linkedTrunks ON linkedTrunks.ID = t.LinkedToTrunkID
	
	WHERE t.ID = @ID
	
	SET NOCOUNT	OFF;
END