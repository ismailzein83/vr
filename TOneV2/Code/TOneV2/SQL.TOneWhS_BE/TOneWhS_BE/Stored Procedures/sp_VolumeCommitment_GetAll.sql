create PROCEDURE [TOneWhS_BE].[sp_VolumeCommitment_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	vc.ID,vc.Settings
	FROM	[TOneWhS_BE].VolumeCommitment vc WITH(NOLOCK) 
	SET NOCOUNT OFF
END