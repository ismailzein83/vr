﻿create PROCEDURE [TOneWhS_BE].[sp_FinancialAccount_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT  fa.ID,
			fa.CarrierAccountId,
			fa.CarrierProfileID,
			fa.FinancialAccountDefinitionId,
			fa.EED,
			fa.BED,
			fa.FinancialAccountSettings
	FROM    [TOneWhS_BE].FinancialAccount fa
	SET NOCOUNT OFF
END