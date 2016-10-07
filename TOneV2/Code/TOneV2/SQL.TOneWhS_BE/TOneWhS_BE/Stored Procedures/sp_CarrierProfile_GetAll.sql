﻿CREATE PROCEDURE [TOneWhS_BE].[sp_CarrierProfile_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	cp.ID,
			cp.Name,
			cp.Settings,
			cp.SourceID,
			cp.IsDeleted
	FROM	[TOneWhS_BE].CarrierProfile  as cp WITH(NOLOCK) 
	WHERE ISNULL(cp.IsDeleted, 0) = 0
END