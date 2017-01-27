-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_PartnerInvoiceSetting_GetAll
AS
BEGIN
	SELECT	ID,PartnerID,InvoiceSettingID,Details
	FROM	VR_Invoice.PartnerInvoiceSetting WITH(NOLOCK) 

END