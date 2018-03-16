CREATE PROCEDURE [VR_Invoice].[sp_InvoiceGenerationDraft_Insert]
	@InvoiceGenerationIdentifier uniqueidentifier,
	@InvoiceTypeId uniqueidentifier,
	@PartnerId varchar(50),
	@PartnerName varchar(max), 	
	@From datetime,
	@To datetime,
	@CustomPayload nvarchar(max),
	@id BIGINT OUT

AS
BEGIN

	INSERT INTO [VR_Invoice].[InvoiceGenerationDraft](InvoiceGenerationIdentifier,InvoiceTypeId, PartnerID,PartnerName, FromDate, ToDate, CustomPayload)
	VALUES (@InvoiceGenerationIdentifier,@InvoiceTypeId, @PartnerId,@PartnerName, @From, @To, @CustomPayload)

	SET @id = SCOPE_IDENTITY()

END