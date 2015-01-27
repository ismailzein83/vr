CREATE PROCEDURE [dbo].[bp_GetCodeLCR]
(
	@ZoneNameFilter VARCHAR(100) = NULL,
	@CodeFilter VARCHAR(50) = NULL,
	@ServicesFlagFilter smallint = 0,
	@SupplierIDs VARCHAR(MAX) = NULL
)
AS
BEGIN

	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	DECLARE @SupplierIDsTable AS TABLE (ID VARCHAR(100))
	INSERT INTO @SupplierIDsTable SELECT * FROM [dbo].ParseArray(@SupplierIDs,',')
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	

	DECLARE @ValidCustomers TABLE (CarrierAccountID VARCHAR(10))
	INSERT INTO @ValidCustomers SELECT CarrierAccountID FROM CarrierAccount ca WHERE ca.RoutingStatus NOT IN (@Account_Blocked, @Account_BlockedInbound) AND ca.ActivationStatus <> @Account_Inactive 	

	IF @CodeFilter IS NULL
		SELECT 
			cm.Code, 
			cm.SupplierZoneID AS OurZoneID,
			oz.[Name] OurZoneName,
			(SELECT MIN(zr.NormalRate) FROM ZoneRate zr WHERE zr.ZoneID = cm.SupplierZoneID AND zr.NormalRate > 0 AND zr.CustomerID IN (SELECT * FROM @ValidCustomers) AND zr.ServicesFlag & @ServicesFlagFilter = @ServicesFlagFilter) AS MinSaleRate,
			cs.SupplierID, 
			cs.SupplierZoneID,
			sz.[Name] AS SupplierZoneName,
			cs.SupplierNormalRate,
			cs.SupplierServicesFlag 
		FROM 
			CodeSupply cs, CodeMatch cm, Zone oz, Zone sz   
		WHERE 
				cm.Code = cs.Code 
			AND cm.SupplierID = 'SYS' 
			AND oz.[Name] LIKE @ZoneNameFilter
			AND oz.ZoneID = cm.SupplierZoneID
			AND sz.ZoneID = cs.SupplierZoneID
			AND cs.SupplierServicesFlag & @ServicesFlagFilter = @ServicesFlagFilter
			AND (@SupplierIDs Is Null Or (sz.SupplierID IN (SELECT ID FROM @SupplierIDsTable)))
		ORDER BY cs.Code, cs.SupplierNormalRate
	ELSE
		SELECT 
			cm.Code, 
			cm.SupplierZoneID AS OurZoneID,
			oz.[Name] OurZoneName,
			(SELECT MIN(zr.NormalRate) FROM ZoneRate zr WHERE zr.ZoneID = cm.SupplierZoneID AND zr.NormalRate > 0 AND zr.CustomerID IN (SELECT * FROM @ValidCustomers) AND zr.ServicesFlag & @ServicesFlagFilter = @ServicesFlagFilter) AS MinSaleRate,
			cs.SupplierID, 
			cs.SupplierZoneID,
			sz.[Name] AS SupplierZoneName,
			cs.SupplierNormalRate,
			cs.SupplierServicesFlag 
		FROM 
			CodeSupply cs, CodeMatch cm, Zone oz, Zone sz   
		WHERE 
				cm.Code = cs.Code 
			AND oz.SupplierID = 'SYS' 
			AND cm.Code LIKE @CodeFilter
			AND oz.ZoneID = cm.SupplierZoneID
			AND sz.ZoneID = cs.SupplierZoneID
			AND cs.SupplierServicesFlag & @ServicesFlagFilter = @ServicesFlagFilter
			AND (@SupplierIDs Is Null Or (sz.SupplierID IN (SELECT ID FROM @SupplierIDsTable)))
		ORDER BY cs.Code, cs.SupplierNormalRate
		
END