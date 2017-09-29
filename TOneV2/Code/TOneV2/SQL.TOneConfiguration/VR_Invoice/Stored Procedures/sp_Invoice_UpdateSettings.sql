create PROCEDURE [VR_Invoice].sp_Invoice_UpdateSettings
	@InvoiceId bigint,
	@Settings nvarchar(MAX)

AS
BEGIN
	
	UPDATE [VR_Invoice].[Invoice]
    SET Settings =@Settings
    WHERE ID = @InvoiceId
END