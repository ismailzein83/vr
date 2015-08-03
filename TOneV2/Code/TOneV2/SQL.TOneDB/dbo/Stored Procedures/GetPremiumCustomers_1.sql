CREATE PROCEDURE [dbo].[GetPremiumCustomers]
    -- Add the parameters for the stored procedure here
@SwitchID INT 
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

SELECT pn.SaleZoneID, scm.Identifier FROM PremiumSaleZone pn
                                            INNER JOIN Rate r ON pn.SaleZoneID = r.ZoneID --AND r.Rate = pn.Rate
                                            INNER JOIN PriceList pl ON r.PriceListID = pl.PriceListID
                                            INNER JOIN SwitchCarrierMapping scm ON scm.CarrierAccountID = pl.CustomerID
                                            WHERE pl.SupplierID = 'SYS' 
                                            AND r.IsEffective = 'Y' 
                                             AND scm.inroute='Y'
                                            AND scm.SwitchID = @SwitchID

END