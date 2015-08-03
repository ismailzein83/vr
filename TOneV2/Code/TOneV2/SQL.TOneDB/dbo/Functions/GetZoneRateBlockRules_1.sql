
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[GetZoneRateBlockRules]
(	
	-- Add the parameters for the function here
@Level INT,
@ZoneId INT,
@CodeGroup VARCHAR(250)
)
RETURNS @ZoneRateBlockRules TABLE 
(
	CustomerID VARCHAR(5),
	SupplierID VARCHAR(5),
	Code VARCHAR(20),
	ZoneID INT,
	UpdateDate SMALLDATETIME,
	IncludeSubCodes CHAR(1),
	ExcludedCodes VARCHAR(250)
	)
AS

BEGIN
	
	IF(@Level = 1)
	BEGIN
		INSERT INTO @ZoneRateBlockRules
		SELECT 	
								rb.CustomerID,
								rb.SupplierID,
								rb.Code,
								rb.ZoneID,
								rb.UpdateDate,
								rb.IncludeSubCodes,
								rb.ExcludedCodes
								
								
							FROM   RouteBlock rb WITH (NOLOCK) 
							WHERE 
								rb.IsEffective = 'Y' 
								AND rb.CustomerID IS NOT NULL 
								AND rb.SupplierID IS NULL 
								AND rb.ZoneID IS NULL 
								AND rb.Code IS NOT NULL 
								AND rb.IncludeSubCodes = 'N'
								
								
	END
	
	ELSE
		IF(@Level = 2)
		BEGIN
			INSERT INTO @ZoneRateBlockRules
					SELECT 	
								rb.CustomerID,
								rb.SupplierID,
								rb.Code,
								rb.ZoneID,
								rb.UpdateDate,
								rb.IncludeSubCodes,
								rb.ExcludedCodes
								
								
							FROM   RouteBlock rb WITH (NOLOCK) 
							WHERE 
								rb.IsEffective = 'Y' 
								AND rb.CustomerID IS NOT NULL 
								AND rb.SupplierID IS NULL 
								AND rb.ZoneID IS NULL 
								AND rb.Code IS NOT NULL 
								AND rb.IncludeSubCodes = 'Y'
		END
	ELSE 
		IF(@Level = 3)
		BEGIN
				INSERT INTO @ZoneRateBlockRules
			SELECT 	
								rb.CustomerID,
								rb.SupplierID,
								rb.Code,
								rb.ZoneID,
								rb.UpdateDate,
								rb.IncludeSubCodes,
								rb.ExcludedCodes
								
								
							FROM   RouteBlock rb WITH (NOLOCK) 
							WHERE 
								rb.IsEffective = 'Y' 
								AND rb.ZoneID IS NOT NULL 
								AND rb.CustomerID IS NULL
		END
	RETURN
END