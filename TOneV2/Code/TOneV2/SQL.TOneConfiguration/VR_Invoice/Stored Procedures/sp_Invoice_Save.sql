﻿CREATE PROCEDURE [VR_Invoice].[sp_Invoice_Save]
	@UserId int,
	@InvoiceTypeId  uniqueidentifier,
	@PartnerID nvarchar(50),
	@SerialNumber nvarchar(255),
	@FromDate datetime,
	@ToDate datetime,
	@IssueDate datetime,
	@DueDate datetime,
	@Details nvarchar(MAX),
	@Notes nvarchar(MAX),
	@SourceID nvarchar(255),
	@IsDraft bit,
	@IsAutomatic bit,
	@Settings nvarchar(MAX),
	@InvoiceSettingId uniqueidentifier,
	@ID bigint out
AS
BEGIN
	Insert INTO VR_Invoice.Invoice (UserId,InvoiceTypeId,PartnerID,SerialNumber,FromDate,ToDate,IssueDate,DueDate,Details,Notes, SourceId,IsDraft,IsAutomatic,Settings,InvoiceSettingID)
	VALUES (@UserId, @InvoiceTypeId,@PartnerID,@SerialNumber,@FromDate,@ToDate,@IssueDate,@DueDate,@Details,@Notes, @SourceID,@IsDraft,@IsAutomatic,@Settings,@InvoiceSettingId)
	SET @ID = @@Identity
END