-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE VR_Invoice.sp_PartnerInvoiceSetting_Delete
	@Id uniqueidentifier
AS
BEGIN
	DELETE FROM VR_Invoice.PartnerInvoiceSetting
	WHERE ID = @Id
END