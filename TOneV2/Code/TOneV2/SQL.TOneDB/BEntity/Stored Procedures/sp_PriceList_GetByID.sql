
CREATE PROCEDURE [BEntity].[sp_PriceList_GetByID]
	 @PriceListId int
AS
BEGIN
	SET NOCOUNT ON;

SELECT   p.PriceListID
		,P.Description
		,P.BeginEffectiveDate
		,p.EndEffectiveDate
		,p.CurrencyId
FROM     PriceList p		
WHERE	p.PriceListID = @PriceListId

END