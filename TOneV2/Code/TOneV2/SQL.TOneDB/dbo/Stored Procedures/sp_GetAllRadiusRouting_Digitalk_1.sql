

-- =============================================
-- Create date: <2015-01-22>
-- Description:    <Get the Routing for MVTS Radius>
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetAllRadiusRouting_Digitalk]
@SwitchID INT 
AS
BEGIN

    SET NOCOUNT ON;

    SELECT  r.RouteID, r.CustomerID,ISNULL(pc.[Name] + cu.NameSuffix, pc.NAME) CustomerName, r.code, z.[Name] , ro.percentage, ro.SupplierID,ISNULL(ps.[Name] + su.NameSuffix, ps.NAME) SupplierName, ro.Priority, ro.SupplierActiveRate
    FROM Route r 
    LEFT JOIN RouteOption ro ON r.RouteID = ro.RouteID
    JOIN Zone z ON z.ZoneID = r.OurZoneID
    JOIN SwitchCarrierMapping scm ON scm.CarrierAccountID = r.CustomerID
    LEFT JOIN CarrierAccount cu ON cu.CarrierAccountID = r.CustomerID
    LEFT JOIN CarrierProfile pc ON pc.ProfileID = cu.ProfileID AND cu.IsDeleted = 'N'
	LEFT JOIN CarrierAccount su ON su.CarrierAccountID = ro.SupplierID 
    LEFT JOIN CarrierProfile ps ON ps.ProfileID = su.ProfileID AND su.IsDeleted = 'N'
    WHERE scm.InCDR = 'Y' AND scm.SwitchID = @SwitchID
    ORDER BY r.CustomerID,r.Code
    
end