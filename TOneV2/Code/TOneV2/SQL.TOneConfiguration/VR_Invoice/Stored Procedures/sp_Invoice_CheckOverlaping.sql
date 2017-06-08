﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_CheckOverlaping]
		@InvoiceTypeId uniqueidentifier,
		@PartnerId varchar(50),
		@FromDate Datetime,
		@ToDate Datetime,
		@InvoiceId bigint = NULL
AS
BEGIN
	SELECT	Count(*) as CountNb
	FROM	VR_Invoice.Invoice with(nolock)
	where	PartnerId = @PartnerId AND InvoiceTypeId = @InvoiceTypeId AND
	(ToDate >= @FromDate AND  @ToDate>=FromDate) AND ISNULL(IsDeleted,0) = 0 AND ISNULL(IsDraft, 0) = 0  AND (@InvoiceId IS NULL OR ID != @InvoiceId)
END