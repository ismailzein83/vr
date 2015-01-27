
CREATE PROCEDURE [dbo].[SP_UpdateEED]	
AS

BEGIN
	;WITH Zones AS
	
	( SELECT
	  	ZoneID,
	  	BeginEffectiveDate,
	  	EndEffectiveDate
	  	
	  FROM Zone 
	  WHERE Zone.SupplierID = 'SYS'
	 
	)
	SELECT T.ZoneID,T.EndEffectiveDate,T.BeginEffectiveDate
	FROM Tariff T 
	INNER JOIN Zones zs
	ON T.ZoneID = zs.ZoneID 
	
	
	
	   
END