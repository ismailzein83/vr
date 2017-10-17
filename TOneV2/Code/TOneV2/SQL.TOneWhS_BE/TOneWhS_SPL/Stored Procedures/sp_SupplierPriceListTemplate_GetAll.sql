-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_SPL].[sp_SupplierPriceListTemplate_GetAll]
AS
BEGIN
	SELECT ID,
		SupplierID,
		ConfigDetails,
		Draft
	FROM [TOneWhS_SPL].SupplierPriceListTemplate WITH(NOLOCK)
END