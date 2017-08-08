-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].sp_InvoiceAccount_GetByPartnerIds
@InvoiceTypeId uniqueidentifier,
 @PartnerIds VARCHAR(MAX)
AS
BEGIN

   BEGIN
	    DECLARE @PartnerIdsTable TABLE (PartnerId varchar(50))
		INSERT INTO @PartnerIdsTable (PartnerId)
		select ParsedString from [VR_Invoice].[ParseStringList](@PartnerIds)


		SELECT	ID,InvoiceTypeId,PartnerId,BED,EED,[Status],IsDeleted
		FROM	VR_Invoice.InvoiceAccount ia WITH(NOLOCK) 
		WHERE (@PartnerIds  IS NULL OR ia.PartnerId IN (select PartnerId from @PartnerIdsTable))
		AND InvoiceTypeId = @InvoiceTypeId
	END
END