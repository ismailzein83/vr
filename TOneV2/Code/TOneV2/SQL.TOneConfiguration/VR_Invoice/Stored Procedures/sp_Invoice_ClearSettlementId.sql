-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_Invoice_ClearSettlementId
	@SettlementInvoiceId bigint

AS
BEGIN
	Update VR_Invoice.Invoice
	Set  SettlementInvoiceId = null
	Where SettlementInvoiceId = @SettlementInvoiceId
END