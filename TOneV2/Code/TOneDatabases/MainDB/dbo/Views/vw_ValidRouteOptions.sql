CREATE VIEW [dbo].[vw_ValidRouteOptions]
AS
	SELECT    
		TOP (100) PERCENT R.RouteID, R.CustomerID, R.Code, O.SupplierID, O.Priority, O.SupplierActiveRate, O.NumberOfTries
		FROM         
			dbo.RouteOption AS O WITH (NOLOCK)  
			INNER JOIN dbo.Route AS R WITH (NOLOCK) ON R.RouteID = O.RouteID AND O.SupplierActiveRate < R.OurActiveRate
		WHERE     
				(R.State <> 0) 
			AND (O.State <> 0)