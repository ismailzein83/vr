CREATE PROCEDURE [TOneWhS_BE].[sp_ANumberSaleCode_GetEffectiveAfterBySellingNumberPlanId]	
	@SellingNumberPlanId int ,
	@EfectiveOn DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	    BEGIN

		SELECT  salecode.ID,
				salecode.ANumberGroupID,
				salecode.SellingNumberPlanID,
				salecode.Code,
				salecode.BED,
				salecode.EED
		FROM [TOneWhS_BE].ANumberSaleCode salecode WITH(NOLOCK) 
		WHERE	salecode.SellingNumberPlanID = @SellingNumberPlanId
		and (salecode.EED is null or (salecode.EED<>salecode.BED and salecode.EED > @EfectiveOn))
        
		END
	SET NOCOUNT OFF
END