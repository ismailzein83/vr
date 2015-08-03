
-- =============================================
-- Create date: <2015-01-22>
-- Description:    <Get the Routing for MVTS Radius>
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetAllRadiusRouting]
AS
BEGIN

    SET NOCOUNT ON;

    SELECT  r.RouteID, r.CustomerID, r.code , ro.percentage, ro.SupplierID, ro.Priority, ro.SupplierActiveRate
	FROM	[Route] r with (nolock) 
			LEFT JOIN RouteOption ro with (nolock) ON r.RouteID = ro.RouteID and ro.[State] = 1
ORDER BY r.RouteID
    
end