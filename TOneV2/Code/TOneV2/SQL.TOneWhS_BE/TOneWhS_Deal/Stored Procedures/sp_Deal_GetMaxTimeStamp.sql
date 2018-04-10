CREATE PROCEDURE [TOneWhS_Deal].[sp_Deal_GetMaxTimeStamp]
AS
BEGIN
	SELECT max(timestamp) FROM [TOneWhS_Deal].[Deal] with(nolock)
END