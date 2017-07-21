-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_Invoice_GetPopulatedPeriod
		@InvoiceTypeId uniqueidentifier,
		@PartnerId varchar(50)
AS
BEGIN
	SELECT Min(FromDate) as FromDate,Max(ToDate) as ToDate
	FROM	VR_Invoice.Invoice with(nolock)
	where	InvoiceTypeID = @InvoiceTypeId  AND  PartnerID = @PartnerId  AND ISNULL( IsDraft,0) = 0 AND ISNULL( IsDeleted,0) = 0
END