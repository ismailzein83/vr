CREATE PROCEDURE [VR_Invoice].[sp_Invoice_Update]
	@ID bigint,
	@InvoiceTypeId  uniqueidentifier,
	@PartnerID nvarchar(50),
	@SerialNumber nvarchar(255),
	@FromDate datetime,
	@ToDate datetime,
	@IssueDate datetime,
	@DueDate datetime,
	@Details nvarchar(MAX),
	@PaidDate datetime,
	@LockDate datetime,
	@Notes nvarchar(MAX),
	@SourceID nvarchar(255)
AS
BEGIN
	
	UPDATE [VR_Invoice].[Invoice]
   SET [InvoiceTypeID] = @InvoiceTypeId
      ,[PartnerID] = @PartnerID
      ,[SerialNumber] = @SerialNumber
      ,[FromDate] = @FromDate
      ,[ToDate] = @ToDate
      ,[IssueDate] = @IssueDate
      ,[DueDate] = @DueDate
      ,[Details] = @Details
      ,[PaidDate] = @PaidDate
      ,[LockDate] = @LockDate
      ,[Notes] = @Notes
	  ,SourceId = @SourceID
 WHERE ID = @ID
END