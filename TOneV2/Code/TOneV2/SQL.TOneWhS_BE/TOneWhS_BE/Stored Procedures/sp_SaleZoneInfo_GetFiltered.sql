-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleZoneInfo_GetFiltered] 
@SellingNumberPlanID int,
@Filter nvarchar(255)
AS
BEGIN
	
	SET NOCOUNT ON;
SELECT  [ID],[Name],[SellingNumberPlanID]
FROM	[TOneWhS_BE].[SaleZone] WITH(NOLOCK) 
Where	SellingNumberPlanID=@SellingNumberPlanID
		and Name like('%' + @Filter + '%')
END