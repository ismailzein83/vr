-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].sp_SaleCode_GetParentsByPlan
@sellingNumberPlanId int,
@code varchar(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT  sc.[ID],sc.[Code],sc.[ZoneID],sc.[CodeGroupID],sc.[BED],sc.[EED],sc.[SourceID]
    from	[TOneWhS_BE].SaleCode sc WITH(NOLOCK) 
	Inner Join [TOneWhS_BE].SaleZone sz on sz.ID = sc.ZoneID
	WHERE   @code like Code  + '%'
		AND sz.SellingNumberPlanID = @sellingNumberPlanId
	  AND   sc.BED <= getdate()
	  AND   (sc.EED IS NULL OR sc.EED > getDate())
	ORDER BY Code 
END