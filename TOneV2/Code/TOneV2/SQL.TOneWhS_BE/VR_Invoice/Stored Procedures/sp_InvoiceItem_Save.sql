CREATE PROCEDURE [VR_Invoice].[sp_InvoiceItem_Save]
	@InvoiceItemTable VR_Invoice.InvoiceItemType READONLY
AS
BEGIN
	Insert INTO VR_Invoice.InvoiceItem (InvoiceID,ItemSetName,Name,Details)
	SELECT InvoiceID,ItemSetName,Name,Details FROM @InvoiceItemTable
END