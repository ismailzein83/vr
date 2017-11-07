﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_InvoiceSetting_GetAll]
AS
BEGIN
	SELECT	ID,Name,InvoiceTypeId,IsDefault,Details
	FROM	VR_Invoice.InvoiceSetting WITH(NOLOCK) 
	WHERE   ISNULL(IsDeleted, 0) = 0
	ORDER BY [Name]
END