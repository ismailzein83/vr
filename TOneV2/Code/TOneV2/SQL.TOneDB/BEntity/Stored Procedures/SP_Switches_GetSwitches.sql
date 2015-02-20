
CREATE PROCEDURE BEntity.SP_Switches_GetSwitches
	
AS
BEGIN

	SET NOCOUNT ON;
	SELECT S.SwitchID, S.Name
	From Switch S
END