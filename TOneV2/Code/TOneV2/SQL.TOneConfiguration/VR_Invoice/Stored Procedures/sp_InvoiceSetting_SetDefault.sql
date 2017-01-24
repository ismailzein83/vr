-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].[sp_InvoiceSetting_SetDefault]
	@InvoiceSettingId uniqueidentifier
AS
BEGIN
	UPDATE is1
	SET is1.IsDefault = CASE WHEN is1.IsDefault = 1 THEN 0 ELSE 1 END
	FROM VR_Invoice.InvoiceSetting is1
    JOIN VR_Invoice.InvoiceSetting is2 
	ON  is1.invoicetypeid = (select is3.invoicetypeid from VR_Invoice.InvoiceSetting is3 where is3.id = @InvoiceSettingId ) 
	  AND  is1.isDefault = 1  
	  OR is1.Id = @InvoiceSettingId
END