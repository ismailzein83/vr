
CREATE PROCEDURE [BEntity].sp_PriceList_GetByCarrierAccountID
	 --(@CarrierAccountId VARCHAR(30) =  NULL)
AS
BEGIN
	SET NOCOUNT ON;

SELECT   p.PriceListID
		,P.[Description]
		,P.BeginEffectiveDate
		,u.Name
FROM     PriceList p		
	JOIN [User] u on u.ID= p.UserID
	    -- WHERE	p.CustomerID = @CarrierAccountId

END