
create PROCEDURE [TOneWhS_BE].[sp_AccountManager_GetAll]

AS
BEGIN
SET NOCOUNT ON 
	SELECT 
		am.CarrierAccountId,
		am.UserId,
		am.RelationType
	FROM [TOneWhS_BE].AccountManager am
SET NOCOUNT OFF
END