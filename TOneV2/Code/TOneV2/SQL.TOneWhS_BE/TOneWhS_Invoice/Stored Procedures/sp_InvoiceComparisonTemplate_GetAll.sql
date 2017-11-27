-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Invoice].[sp_InvoiceComparisonTemplate_GetAll] 
	
AS
BEGIN
SELECT ID,InvoiceTypeId,PartnerId,Details
FROM InvoiceComparisonTemplate

END