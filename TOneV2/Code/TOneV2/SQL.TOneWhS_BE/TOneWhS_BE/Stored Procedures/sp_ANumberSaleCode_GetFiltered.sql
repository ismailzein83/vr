CREATE PROCEDURE [TOneWhS_BE].[sp_ANumberSaleCode_GetFiltered]	
	@ANumberGroupId int ,
	@SellingNumberPlanIds varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	    BEGIN
	    DECLARE @SellingNumberPlanIdsTable TABLE (SellingNumberPlanId int)
		INSERT INTO @SellingNumberPlanIdsTable (SellingNumberPlanId)
		select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@SellingNumberPlanIds)

		SELECT  salecode.ID,
				salecode.ANumberGroupID,
				salecode.SellingNumberPlanID,
				salecode.Code,
				salecode.BED,
				salecode.EED
		FROM [TOneWhS_BE].ANumberSaleCode salecode WITH(NOLOCK) 
        WHERE  salecode.ANumberGroupID = @ANumberGroupId        
        and (@SellingNumberPlanIds  is null or salecode.SellingNumberPlanID in (select SellingNumberPlanId from @SellingNumberPlanIdsTable))            
        
		END
	SET NOCOUNT OFF
END