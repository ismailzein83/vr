-- =============================================
-- Author:		Rabih Fashwal
-- Create date: 2014-07-17
-- Description:	Partial Routing: Fill filters tables 
-- =============================================
CREATE PROCEDURE [dbo].[bp_RT_Part_FillFiltrationTables]
	-- Add the parameters for the stored procedure here
	 @Targets VARCHAR(MAX) = ''
	,@TargetsType VARCHAR(2) = 'CW'
	,@TargetCustomers VARCHAR(MAX) = '' 
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint

	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	
	CREATE TABLE #TempFilters (Code VARCHAR(20), ZoneID INT,CodeGroup VARCHAR(15), CarrierID VARCHAR(5), IsValid BIT)
----------------------------------------------------------------------------------------
--The process done is to prepare all data in one Table #TempFilters and then split then in the last process into the three tables 
--CodeFilter, SaleZoneFilter, CostZoneFilter
--Then fill the CustomerFilter in the last step

-----------------------------------------------------------------------------------------
--First Scenario: Code without Sub Codes
--Get distinct codes for every supplier and their ZoneIDs with code group from code match where splited codes = CodeMatch.Code
IF(@TargetsType = 'CW')
	BEGIN
		INSERT INTO #TempFilters
		SELECT DISTINCT(isnull(cm.code,pa.value)),cm.SupplierZoneID,z.CodeGroup,cm.SupplierID, CASE WHEN cm.Code IS NULL THEN 0 ELSE 1 END
		  FROM dbo.ParseArray(@Targets,',') pa
		LEFT JOIN CodeMatch cm ON cm.Code = pa.[value]
		LEFT JOIN #ActiveSuppliers ac ON ac.CarrierAccountID COLLATE DATABASE_Default = cm.SupplierID
		LEFT JOIN Zone z ON z.ZoneID = cm.SupplierZoneID
						
	END	

--Second and Fifth Scenario: Code with Sub Codes or Code Group
--Get distinct codes for every supplier and their ZoneIDs with code group from code match where CodeMatch.Code like splited codes. 
--Code Group is a set of codes and treated as Code with sub codes the difference is that it is the master(mother code).
IF(@TargetsType = 'CS' OR @TargetsType = 'CG')
	BEGIN
		INSERT INTO #TempFilters
		SELECT DISTINCT(isnull(cm.code,pa.value)),cm.SupplierZoneID,z.CodeGroup,cm.SupplierID, CASE WHEN cm.Code IS NULL THEN 0 ELSE 1 END
		FROM dbo.ParseArray(@Targets,',') pa
		LEFT JOIN CodeMatch cm ON cm.Code LIKE pa.[value] + '%'
		LEFT JOIN #ActiveSuppliers ac ON ac.CarrierAccountID COLLATE DATABASE_Default = cm.SupplierID
		LEFT JOIN Zone z ON z.ZoneID = cm.SupplierZoneID
	END	

--Third and Fourth Scenario: Cost Zone or Sale Zone
--These to steps are done together as they are similar. For cost zones, we will need the other suppliers to manage the Routes and
--their options if affected.
IF(@TargetsType = 'CZ' OR @TargetsType = 'SZ')
	BEGIN
		WITH DistinctCodes AS 
		(
		SELECT DISTINCT(cm.code),z.CodeGroup
		FROM dbo.ParseIntArray(@Targets,',') pa
		LEFT JOIN CodeMatch cm ON cm.SupplierZoneID = pa.Value	
		LEFT JOIN Zone z ON z.ZoneID = cm.SupplierZoneID
		)
		
		INSERT INTO  #TempFilters
		SELECT dc.Code,cm.SupplierZoneID,dc.CodeGroup,cm.SupplierID,CASE WHEN cm.Code IS NULL THEN 0 ELSE 1 END
		FROM DistinctCodes dc
		INNER JOIN CodeMatch cm ON cm.Code = dc.Code
		LEFT JOIN #ActiveSuppliers ac ON ac.CarrierAccountID COLLATE DATABASE_Default = cm.SupplierID
				
	END	

--Fifth Scenario: Code Group
--Code Group is a set of Zones, thus the codes are loaded with it sub codes	
--IF(@TargetsType = 'CG')
--	BEGIN
--		INSERT INTO #TempFilters
--		SELECT cm.code,cm.SupplierZoneID,z.CodeGroup,cm.SupplierID, CASE WHEN cm.Code IS NULL THEN 0 ELSE 1 END
--		FROM dbo.ParseArray(@Targets,',') pa
--		LEFT JOIN CodeMatch cm ON cm.Code LIKE pa.[value] + '%'	
--		LEFT JOIN #ActiveSuppliers ac ON ac.CarrierAccountID = cm.SupplierID
--		LEFT JOIN Zone z ON z.ZoneID = cm.SupplierZoneID
		
--	END
	
		INSERT INTO #CodeFilter
		SELECT DISTINCT(tf.Code),tf.IsValid FROM #TempFilters tf
		
		INSERT INTO #SaleZoneFilter
		SELECT DISTINCT(tf.ZoneID),tf.CodeGroup,tf.IsValid,tf.CarrierID FROM #TempFilters tf
		WHERE tf.CarrierID = 'SYS'
		
		INSERT INTO #CostZoneFilter
		SELECT DISTINCT(tf.ZoneID),tf.CodeGroup,tf.IsValid, tf.CarrierID FROM #TempFilters tf
		WHERE tf.CarrierID <> 'SYS'
		
IF(@TargetCustomers = '' OR @TargetCustomers IS NULL)
		BEGIN
			INSERT INTO #CustomerFilter
			SELECT ac.CarrierAccountID,ac.ProfileID,1 FROM #ActiveCustomers ac	
		END	
ELSE
		BEGIN
			INSERT INTO #CustomerFilter
			SELECT ISNULL(ca.CarrierAccountID,pa.VALUE),
			ca.ProfileID, 
			CASE WHEN ca.CarrierAccountID IS NULL THEN 0 ELSE 1 END
			  FROM dbo.ParseArray(@TargetCustomers,',') pa
			LEFT JOIN CarrierAccount ca ON pa.[value] = ca.CarrierAccountID
			WHERE ca.ActivationStatus <> @Account_Inactive
		    And ca.RoutingStatus <> @Account_BlockedInbound 
		    AND ca.RoutingStatus <> @Account_Blocked 
		    AND ca.IsDeleted='N'
		    
		    INSERT INTO #CustomerFilter
		    SELECT ca.CarrierAccountID, ca.ProfileID, 1
		    FROM CarrierAccount ca WHERE ca.CarrierAccountID = 'SYS'
		  
		END

---For Testing-------
SELECT * FROM #TempFilters
SELECT * FROM #CustomerFilter
SELECT * FROM #CodeFilter
SELECT * FROM #SaleZoneFilter
SELECT * FROM #CostZoneFilter	
		
END