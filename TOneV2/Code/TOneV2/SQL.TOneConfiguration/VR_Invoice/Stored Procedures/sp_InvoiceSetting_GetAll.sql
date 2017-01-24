-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_InvoiceSetting_GetAll]
AS
BEGIN
	SELECT	ID,Name,InvoiceTypeId,IsDefault,Details
	FROM	VR_Invoice.InvoiceSetting WITH(NOLOCK) 
	ORDER BY [Name]
END