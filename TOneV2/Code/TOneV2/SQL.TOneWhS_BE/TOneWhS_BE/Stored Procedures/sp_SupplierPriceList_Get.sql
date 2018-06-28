-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierPriceList_Get]
	@PriceListID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT	[ID],[SupplierID],[CurrencyID],[PricelistType],[FileID],[ProcessInstanceID],[SPLStateBackupID],[UserID]
    from	[TOneWhS_BE].SupplierPriceList WITH(NOLOCK) 
    where	ID = @PriceListID 
END