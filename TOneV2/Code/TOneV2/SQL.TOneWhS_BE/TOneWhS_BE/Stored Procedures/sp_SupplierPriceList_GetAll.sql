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

    SELECT	[ID],[CreatedTime],[SupplierID],[PricelistType],[CurrencyID],[FileID],[ProcessInstanceID],[SPLStateBackupID],[UserID],[EffectiveOn]
    FROM	[TOneWhS_BE].SupplierPriceList WITH(NOLOCK) 
	

END