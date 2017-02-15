CREATE PROCEDURE [VR_Invoice].[sp_Invoice_Save]
	@UserId int,
	@InvoiceTypeId  uniqueidentifier,
	@PartnerID nvarchar(50),
	@SerialNumber nvarchar(255),
	@FromDate datetime,
	@ToDate datetime,
	@TimeZoneId int,
	@IssueDate datetime,
	@DueDate datetime,
	@Details nvarchar(MAX),
	@Notes nvarchar(MAX),
	@InvoiceIdToDelete bigint = Null,
	@ID bigint out
AS
BEGIN
	
	If(@InvoiceIdToDelete IS NOT Null)
	BEGIN
		Update VR_Invoice.Invoice Set IsDeleted = 1
		Where ID =@InvoiceIdToDelete;
  	END

	Insert INTO VR_Invoice.Invoice (UserId,InvoiceTypeId,PartnerID,SerialNumber,FromDate,ToDate,TimeZoneId,IssueDate,DueDate,Details,Notes)
	VALUES (@UserId, @InvoiceTypeId,@PartnerID,@SerialNumber,@FromDate,@ToDate,@TimeZoneId,@IssueDate,@DueDate,@Details,@Notes)
	SET @ID = @@Identity
END