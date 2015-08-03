

CREATE  PROCEDURE [dbo].[bp_UpdateRoutesFromSupplierRouteBlock](
@CustomerID VARCHAR(10) = NULL,  
@SupplierZoneID  INT 
)
with Recompile
AS 

DECLARE  @Routes TABLE  (RouteID int)
INSERT INTO @Routes 
	SELECT r.RouteID FROM [Route] r
	WHERE  EXISTS (
           SELECT *
           FROM   RouteOption ro
           WHERE  ro.SupplierZoneID = @SupplierZoneID
                  AND ro.RouteID = r.RouteID
       )
       AND (@CustomerID IS NULL OR r.CustomerID = @CustomerID) 



UPDATE [Route] 
SET    Updated = GETDATE(),
       IsBlockAffected = 'Y'
FROM [Route]  WITH(NOLOCK)
WHERE  RouteID IN  (
           SELECT RTU.RouteID 
           FROM   @Routes RTU )
             
             
UPDATE RouteOption
SET    [State] = 0,
       Updated = GETDATE()
FROM RouteOption WITH(NOLOCK,INDEX(IDX_RouteOption_SupplierZoneID))
WHERE  RouteID IN  (
           SELECT RTU.RouteID 
           FROM   @Routes RTU )
AND SupplierZoneID = @SupplierZoneID

RETURN