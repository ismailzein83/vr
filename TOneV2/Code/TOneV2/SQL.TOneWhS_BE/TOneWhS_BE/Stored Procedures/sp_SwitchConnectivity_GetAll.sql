﻿create PROCEDURE [TOneWhS_BE].[sp_SwitchConnectivity_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT sc.ID,
		sc.Name,
		sc.CarrierAccountID,
		sc.SwitchID,
		sc.Settings,
		sc.BED,
		sc.EED
	FROM TOneWhS_BE.SwitchConnectivity sc
	SET NOCOUNT OFF
END