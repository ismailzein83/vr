create PROCEDURE [VR_Invoice].[sp_Invoice_Save]
	@InvoiceTypeId  uniqueidentifier,
	@PartnerID nvarchar(50),
	@SerialNumber nvarchar(255),
	@FromDate datetime,
	@ToDate datetime,
	@IssueDate datetime,
	@DueDate datetime,
	@Details nvarchar(MAX),
	@ID bigint out
AS
BEGIN
	Insert INTO VR_Invoice.Invoice (InvoiceTypeId,PartnerID,SerialNumber,FromDate,ToDate,IssueDate,DueDate,Details)
	VALUES ( @InvoiceTypeId,@PartnerID,@SerialNumber,@FromDate,@ToDate,@IssueDate,@DueDate,@Details)
	SET @ID = @@Identity
END