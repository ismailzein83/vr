﻿

CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierZoneRate_Preview_GetNumberOfRatesWithChangeType]
	@ProcessInstanceID_IN INT,
	@RateChangeType_IN INT
AS
BEGIN

	SELECT	COUNT(RateChangeType)
	FROM	[TOneWhS_SPL].[SupplierZoneRate_Preview] WITH(NOLOCK)
	WHERE	ProcessInstanceID = @ProcessInstanceID_IN and RateChangeType=@RateChangeType_IN
	
END