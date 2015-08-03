-- ======================================================================
-- Author: Mohammad El-Shab
-- Description: 
-- ======================================================================
CREATE PROCEDURE [dbo].[SP_DeleteCarrierAccount]
	@SupplierID VARCHAR(5) = NULL,
	@CustomerID VARCHAR(5) = NULL,
    @Date DATETIME = NULL
AS
BEGIN
	-- TRY	
	BEGIN TRY
		BEGIN TRANSACTION
			DECLARE @ZoneRate TABLE(ZoneID bigint,RateID int)
			IF (@SupplierID IS NOT NULL)
				BEGIN
						;WITH PriceListCTE AS 
						(
						SELECT PriceListID
						FROM PriceList WITH(NOLOCK)
						WHERE --BeginEffectiveDate <= @Date AND
							  --CustomerID = 'SYS' AND 
							    SupplierID = @SupplierID AND 
							   (EndEffectiveDate IS NULL 
							 OR EndEffectiveDate > @Date) -- AND 
	 						    --IsEffective = 'Y'
	 					 )
						,RateCTE AS 
						(
						SELECT r.ZoneID,r.RateID
						FROM Rate r
						JOIN PriceListCTE pl ON r.PriceListID = pl.PriceListID
						WHERE  --r.BeginEffectiveDate <= @Date AND
						      (r.EndEffectiveDate IS NULL 
						    OR r.EndEffectiveDate > @Date)
						)
						INSERT INTO @ZoneRate(ZoneID,RateID)
						SELECT ZoneID,RateID
						FROM RateCTE
						
						-- Update Rates
						UPDATE Rate
						SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
						WHERE RateID IN (SELECT RateID FROM @ZoneRate)

						-- Update Codes	
						UPDATE Code
						SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
						WHERE ZoneID IN (SELECT DISTINCT ZoneID FROM @ZoneRate) AND
						    --BeginEffectiveDate <= @Date AND
							 (EndEffectiveDate IS NULL 
						   OR EndEffectiveDate > @Date)
								
						-- Update Zones
						UPDATE Zone
						SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
						WHERE ZoneID IN (SELECT DISTINCT ZoneID FROM @ZoneRate) AND
						    --BeginEffectiveDate <= @Date AND
						     (EndEffectiveDate IS NULL 
						   OR EndEffectiveDate > @Date)
					 					 
						-- Update PriceList
						UPDATE PriceList
						SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
						WHERE --BeginEffectiveDate <= @Date AND
							  --CustomerID = @CustomerID AND 
							   SupplierID = @SupplierID AND 
							  (EndEffectiveDate IS NULL 
							OR EndEffectiveDate > @Date) -- AND 
							-- IsEffective = 'Y'
						  
					   -- Update Tariff	  
					   UPDATE Tariff
   					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE SupplierID = @SupplierID AND
						   (EndEffectiveDate IS NULL 
						 OR EndEffectiveDate > @Date)
						  
					   -- Update TODConsideration
					   UPDATE ToDConsideration
					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE SupplierID = @SupplierID AND
					    	(EndEffectiveDate IS NULL 
						  OR EndEffectiveDate > @Date)
						  
					   -- Update Commission
					   UPDATE Commission
					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE SupplierID = @SupplierID AND
					        (EndEffectiveDate IS NULL 
						  OR EndEffectiveDate > @Date)

					   -- Update Route Block
 					   UPDATE RouteBlock
					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE SupplierID = @SupplierID AND
						 (EndEffectiveDate IS NULL 
					   OR EndEffectiveDate > @Date)

					   -- Update Special Request
					   UPDATE SpecialRequest
					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE SupplierID = @SupplierID AND
							(EndEffectiveDate IS NULL 
						  OR EndEffectiveDate > @Date)

					  -- Update Route Override
					  UPDATE RouteOverride
					  SET RouteOptions = dbo.ErasePatterns(RouteOptions,'|',@SupplierID) 
					  WHERE RouteOptions LIKE '%' + @SupplierID + '%'	
					  
					  -- Update Bilateral_Agreement
					  UPDATE Bilateral_Agreement
					  SET EndDate = (CASE WHEN BeginDate > @Date THEN BeginDate ELSE @Date END),
					      GraceDate = (CASE WHEN BeginDate > @Date THEN BeginDate ELSE @Date END)
					  WHERE CarrierID = @SupplierID AND
					       (EndDate IS NULL 
						 OR EndDate > @Date)
						  
					  -- Update BQR Criterion
					  UPDATE BQR_Threshold
					  SET EndEffectiveDate = @Date
					  WHERE CriterionID IN (SELECT ID
											FROM BQR_Criterion WITH(NOLOCK)
											WHERE SupplierID = @SupplierID AND
												 (EndEffectiveDate IS NULL 
											   OR EndEffectiveDate > @Date))
				      UPDATE BQR_Criterion
					  SET EndEffectiveDate = @Date --(CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					  WHERE SupplierID = @SupplierID AND
					       (EndEffectiveDate IS NULL 
						 OR EndEffectiveDate > @Date)
				
					  -- Update Carrier Account 
					  UPDATE CarrierAccount
					  SET ActivationStatus = 0,
					   	  RoutingStatus = 0,
						  IsDeleted = 'Y'
					  WHERE CarrierAccountID = @SupplierID
				END
			IF @CustomerID IS NOT NULL
				BEGIN
						;WITH PriceListCTE AS 
						(
						SELECT PriceListID
						FROM PriceList WITH(NOLOCK)
						WHERE --BeginEffectiveDate <= @Date AND
							    CustomerID = @CustomerID AND 
							    --SupplierID = 'SYS' AND 
							   (EndEffectiveDate IS NULL 
							 OR EndEffectiveDate > @Date) AND 
	 						    IsEffective = 'Y')
						,RateCTE AS 
						(
						SELECT r.ZoneID,r.RateID
						FROM Rate r
						JOIN PriceListCTE pl ON r.PriceListID = pl.PriceListID
						WHERE  --r.BeginEffectiveDate <= @Date AND
						      (r.EndEffectiveDate IS NULL 
						    OR r.EndEffectiveDate > @Date)
						)
						INSERT INTO @ZoneRate(ZoneID,RateID)
						SELECT ZoneID,RateID
						FROM RateCTE
						
						-- Update Rates
						UPDATE Rate
						SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
						WHERE RateID IN (SELECT RateID FROM @ZoneRate)
						
						-- Update PriceList
						UPDATE PriceList
						SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
						WHERE --BeginEffectiveDate <= @Date AND
						      CustomerID = @CustomerID AND 
							 (EndEffectiveDate IS NULL OR EndEffectiveDate > @Date) -- AND 
							  --SupplierID = @SupplierID AND 
							  --IsEffective = 'Y'
						  
					   -- Update Tariff	  
					   UPDATE Tariff
   					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE CustomerID = @CustomerID AND
						   (EndEffectiveDate IS NULL 
						 OR EndEffectiveDate > @Date)
						  
					   -- Update TODConsideration
					   UPDATE ToDConsideration
					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE CustomerID = @CustomerID AND
					    	(EndEffectiveDate IS NULL 
						  OR EndEffectiveDate > @Date)
						  
					   -- Update Commission
					   UPDATE Commission
					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE CustomerID = @CustomerID AND
					        (EndEffectiveDate IS NULL 
						  OR EndEffectiveDate > @Date)

					   -- Update Route Block
 					   UPDATE RouteBlock
					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE CustomerID = @CustomerID AND
						    (EndEffectiveDate IS NULL 
					      OR EndEffectiveDate > @Date)

					   -- Update Special Request
					   UPDATE SpecialRequest
					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE CustomerID = @CustomerID AND
							(EndEffectiveDate IS NULL 
						  OR EndEffectiveDate > @Date)

					  -- Update Route Override
					  UPDATE RouteOverride
					  SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					  WHERE CustomerID = @CustomerID AND
					       (EndEffectiveDate IS NULL 
						 OR EndEffectiveDate > @Date)
					
					  -- Update Bilateral_Agreement
					  UPDATE Bilateral_Agreement
					  SET EndDate = (CASE WHEN BeginDate > @Date THEN BeginDate ELSE @Date END),
					      GraceDate = (CASE WHEN BeginDate > @Date THEN BeginDate ELSE @Date END)
					  WHERE CarrierID = @CustomerID AND
					       (EndDate IS NULL 
						 OR EndDate > @Date)
						  
				      -- Update BQR Criterion
				      UPDATE BQR_Threshold
					  SET EndEffectiveDate = @Date
					  WHERE CriterionID IN (SELECT ID
											FROM BQR_Criterion WITH(NOLOCK)
											WHERE CustomerID = @CustomerID AND
												 (EndEffectiveDate IS NULL 
											   OR EndEffectiveDate > @Date))
				      UPDATE BQR_Criterion
					  SET EndEffectiveDate = @Date --(CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					  WHERE CustomerID = @CustomerID AND
					       (EndEffectiveDate IS NULL 
						 OR EndEffectiveDate > @Date)
						  
					  -- Update Carrier Account 
					  UPDATE CarrierAccount
					  SET ActivationStatus = 0,
					 	  RoutingStatus = 0,
					   	  IsDeleted = 'Y'
					  WHERE CarrierAccountID = @CustomerID
				END
		SELECT 'Success' AS strMessage
		COMMIT TRANSACTION
	END TRY
	-- CATCH
	BEGIN CATCH
		SELECT ERROR_MESSAGE() AS strMessage
		ROLLBACK TRANSACTION
	END CATCH
END