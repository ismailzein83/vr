-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleZone_GetByNumberPlan] 
@SellingNumberPlanID int
AS
BEGIN
	
	SET NOCOUNT ON;
SELECT  [ID],[SellingNumberPlanID],[CountryID],[Name],[BED],[EED]
FROM	[TOneWhS_BE].[SaleZone] sz WITH(NOLOCK) 
Where	SellingNumberPlanID=@SellingNumberPlanID
END