CREATE PROCEDURE [TOneWhS_Deal].[sp_Deal_GetAll]
AS
BEGIN
	SELECT	d.ID,d.Name,d.Settings, d.IsDeleted
	FROM	[TOneWhS_Deal].Deal d WITH(NOLOCK) 
END