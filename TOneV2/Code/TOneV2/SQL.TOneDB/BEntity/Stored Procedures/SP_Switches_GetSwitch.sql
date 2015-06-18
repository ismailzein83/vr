
CREATE PROCEDURE [BEntity].[SP_Switches_GetSwitch]
	@switchID int=null
AS
BEGIN

	SET NOCOUNT ON;
	SELECT S.SwitchID, 
	S.Name,
	S.Symbol,
	S.Description,
	S.LastCDRImportTag,
	S.LastImport,
	S.LastAttempt,
	S.Enable_CDR_Import,
	S.Enable_Routing,
	S.LastRouteUpdate
	From Switch S where S.SwitchID=@switchID
END