﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_GetFiltered]
	@InvoiceTypeId uniqueidentifier,
	@PartnerIds nvarchar(MAX),
	@FromDate datetime,
	@ToDate datetime,
	@IssueDate datetime
AS
BEGIN
DECLARE @PartnerIdsTable TABLE (PartnerId nvarchar(50))
INSERT INTO @PartnerIdsTable (PartnerId)
select Convert(nvarchar(50), ParsedString) from [VR_Invoice].ParseStringList(@PartnerIds)
	SELECT	ID,InvoiceTypeID,PartnerID,SerialNumber,FromDate,ToDate,IssueDate,DueDate,Details,Paid
	FROM	VR_Invoice.Invoice with(nolock)
	where	(InvoiceTypeId = @InvoiceTypeId) AND  (@PartnerIds is Null or PartnerID IN (SELECT PartnerId FROM @PartnerIdsTable))
			AND FromDate >= @FromDate AND (@ToDate is null OR ToDate <= @ToDate)   AND (@IssueDate is null OR IssueDate =@IssueDate )
END