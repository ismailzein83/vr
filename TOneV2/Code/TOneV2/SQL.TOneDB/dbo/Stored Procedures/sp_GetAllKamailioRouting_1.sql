CREATE PROCEDURE [dbo].[sp_GetAllKamailioRouting]
(
@CustomerID varchar(4)
)
AS
BEGIN

    SET NOCOUNT ON;

    SELECT  r.RouteID, r.CustomerID, r.code , ro.percentage, ro.SupplierID, ro.Priority, ro.SupplierActiveRate
	FROM	[Route] r with (nolock) 
			LEFT JOIN RouteOption ro with (nolock) ON r.RouteID = ro.RouteID and ro.[State] = 1
			where r.CustomerID=@CustomerID
ORDER BY r.RouteID
    
end