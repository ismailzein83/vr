-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [VR_Invoice].[sp_PartnerInvoiceSetting_GetById]
	@Id uniqueidentifier
AS
BEGIN
	Select ID, PartnerId, InvoiceSettingID, Details FROM VR_Invoice.PartnerInvoiceSetting
	WHERE ID = @Id
END