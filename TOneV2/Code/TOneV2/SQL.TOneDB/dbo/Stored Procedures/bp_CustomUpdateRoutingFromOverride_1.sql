-- =================================================================
-- Author:		Fadi Chamieh
-- Create date: 2009-04-30
-- Description:	Updates Routes / Route Options from Route Overrides
-- =================================================================
CREATE  PROCEDURE [dbo].[bp_CustomUpdateRoutingFromOverride] 
(
@CustomerID VARCHAR(10) = NULL,
@Code VARCHAR(20) = NULL,
@OurZoneID INT = NULL,
@RouteOptions VARCHAR(100),
@blockedSuppliers VARCHAR(100)  
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @State_Blocked tinyint
	DECLARE @State_Enabled tinyint
	SET @State_Blocked = 0
	SET @State_Enabled = 1

	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	


		/*************************************
		* Process Blocks 
		*************************************/
		IF(@BlockedSuppliers IS NOT NULL)
	    UPDATE RouteOption SET [State] = @State_Blocked, Updated = GETDATE()
			FROM [Route] r, RouteOption WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
			WHERE    
					r.CustomerID = @CustomerID
					AND r.Code = @Code					 
					AND RouteOption.RouteID = r.RouteID
					AND RouteOption.SupplierID IN (SELECT pa.[value] FROM dbo.ParseArray(@BlockedSuppliers, '|') pa)

		IF(@BlockedSuppliers IS NOT NULL)	
	    UPDATE RouteOption SET [State] = @State_Blocked, Updated = GETDATE()
			FROM [Route] r, RouteOption WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
			WHERE      
					r.CustomerID = @CustomerID
					AND r.OurZoneID = @OurZoneID 
					AND RouteOption.RouteID = r.RouteID
					AND RouteOption.SupplierID IN (SELECT pa.[value] FROM dbo.ParseArray(@BlockedSuppliers, '|') pa)



		/*************************************
		* Delete Overrides / Options 
		*************************************/
	
		DELETE RouteOption 
			FROM [Route] r, RouteOverride rov, RouteOption WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
			WHERE 
				    r.CustomerID = @CustomerID
				AND r.Code = @Code
				AND RouteOption.RouteID = r.RouteID

		DELETE RouteOption 
			FROM [Route] r,RouteOption WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
			WHERE 
				   r.CustomerID = @CustomerID
				AND r.OurZoneID = @OurZoneID
				AND RouteOption.RouteID = r.RouteID
		
		
		/*************************************
		* Insert Overrides / Options 
		*************************************/
		
		-- Insert Override Options (Code based)
		INSERT INTO RouteOption (RouteID, SupplierID, SupplierZoneID, SupplierActiveRate, SupplierNormalRate, SupplierServicesFlag, Priority, [State], NumberOfTries)
			SELECT r.RouteID, ca.CarrierAccountID, NULL, -1, -1, 0, ROW_NUMBER() OVER (PARTITION BY r.RouteID ORDER BY PATINDEX('%'+ca.CarrierAccountID+'%', @RouteOptions) DESC), 1, 1
			  FROM [Route] r, CarrierAccount ca  
			WHERE
			        r.CustomerID = @CustomerID
				AND r.Code = @Code
				AND ca.CarrierAccountID IN (SELECT pa.[value] FROM dbo.ParseArray(@RouteOptions, '|') pa)
				AND ca.ActivationStatus <> @Account_Inactive
				AND ca.RoutingStatus <> @Account_Blocked
				AND ca.RoutingStatus <> @Account_BlockedOutbound				

		-- Insert Override Options (Zone based)
		INSERT INTO RouteOption (RouteID, SupplierID, SupplierZoneID, SupplierActiveRate, SupplierNormalRate, SupplierServicesFlag, Priority, [State], NumberOfTries)
			SELECT r.RouteID, ca.CarrierAccountID, NULL, -1, -1, 0, ROW_NUMBER() OVER (PARTITION BY r.RouteID ORDER BY PATINDEX('%'+ca.CarrierAccountID+'%', @RouteOptions) DESC), 1, 1
			  FROM [Route] r,  CarrierAccount ca  
			WHERE 
					r.CustomerID = @CustomerID
				AND r.OurZoneID = @OurZoneID
				AND ca.CarrierAccountID IN (SELECT pa.[value] FROM dbo.ParseArray(@RouteOptions, '|') pa)
				AND ca.ActivationStatus <> @Account_Inactive
				AND ca.RoutingStatus <> @Account_Blocked
				AND ca.RoutingStatus <> @Account_BlockedOutbound				

		
END