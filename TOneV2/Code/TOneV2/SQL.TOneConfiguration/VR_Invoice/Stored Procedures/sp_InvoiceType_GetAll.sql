-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_InvoiceType_GetAll]
AS
BEGIN
	SELECT	ID,Name,Settings
	FROM	VR_Invoice.InvoiceType WITH(NOLOCK) 
	ORDER BY [Name]
END