-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].sp_Invoice_CheckIfHasInvoices
		@InvoiceTypeId uniqueidentifier,
		@PartnerId varchar(50)
AS
BEGIN
	DECLARE @HasInvoices bit
	
	IF EXISTS(Select Top(1) null FROM VR_Invoice.Invoice  WHERE PartnerId = @PartnerId AND InvoiceTypeId = @InvoiceTypeId)
	BEGIN 
	    SET @HasInvoices = 1;
	END
	ELSE
	BEGIN
		SET @HasInvoices = 0;
	END
	SELECT @HasInvoices as HasInvoices;
END