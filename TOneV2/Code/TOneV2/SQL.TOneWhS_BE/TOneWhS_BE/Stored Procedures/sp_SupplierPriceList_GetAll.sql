-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierPriceList_GetAll]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT	[ID],[CreatedTime],[SupplierID],[CurrencyID],[FileID]
    FROM	[TOneWhS_BE].SupplierPriceList WITH(NOLOCK) 
	ORDER BY ID DESC

END