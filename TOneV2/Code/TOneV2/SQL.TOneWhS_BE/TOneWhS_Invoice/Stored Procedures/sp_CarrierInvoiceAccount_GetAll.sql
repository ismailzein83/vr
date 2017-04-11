create PROCEDURE [TOneWhS_Invoice].[sp_CarrierInvoiceAccount_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT cia.ID,
		cia.CarrierAccountId,
		cia.CarrierProfileID,
		cia.EED,
		cia.BED,
		cia.InvoiceAccountSettings
	FROM [TOneWhS_Invoice].CarrierInvoiceAccount cia
	SET NOCOUNT OFF
END