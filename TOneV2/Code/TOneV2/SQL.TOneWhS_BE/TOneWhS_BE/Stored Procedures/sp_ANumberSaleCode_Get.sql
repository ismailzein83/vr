-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_ANumberSaleCode_Get]
	@ANumberSaleCodeID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   	SELECT  salecode.ID,
				salecode.ANumberGroupID,
				salecode.SellingNumberPlanID,
				salecode.Code,
				salecode.BED,
				salecode.EED
		FROM [TOneWhS_BE].ANumberSaleCode salecode WITH(NOLOCK) 
        WHERE  salecode.ID = @ANumberSaleCodeID       
END