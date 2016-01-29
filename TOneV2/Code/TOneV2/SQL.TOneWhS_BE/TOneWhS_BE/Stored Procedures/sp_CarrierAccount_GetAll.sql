﻿CREATE PROCEDURE [TOneWhS_BE].[sp_CarrierAccount_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT ca.ID,
		ca.AccountType,
		ca.CarrierProfileID,
		ca.NameSuffix,
		ca.CustomerSettings,
		ca.SupplierSettings,
		ca.SellingNumberPlanID,
		ca.CarrierAccountSettings
	FROM TOneWhS_BE.CarrierAccount ca
	SET NOCOUNT OFF
END