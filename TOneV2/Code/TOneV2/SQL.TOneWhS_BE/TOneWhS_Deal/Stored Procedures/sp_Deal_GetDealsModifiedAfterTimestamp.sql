CREATE PROCEDURE [TOneWhS_Deal].[sp_Deal_GetDealsModifiedAfterTimestamp]
	@LastTimeStamp timestamp
AS
BEGIN
SET NOCOUNT ON;
	Select d.ID,d.Name,d.Settings
	From [TOneV2_Dev].[TOneWhS_Deal].[Deal] d WITH(NOLOCK) 
	Where @LastTimeStamp is null or [timestamp] > @LastTimeStamp
	SET NOCOUNT OFF
END