-- ======================================================================
-- Author: Mohammad El-Shab
-- Description: 
-- ======================================================================
CREATE PROCEDURE [dbo].[SP_DeleteCarrierProfile]
	@ProfileID int = NULL,
    @Date DATETIME = NULL
AS
BEGIN
	-- TRY	
	BEGIN TRY
		BEGIN TRANSACTION
			DECLARE @ZoneRate TABLE(ZoneID bigint,RateID int)
			
			DECLARE @SupplierID TABLE(CarrierAccountID VARCHAR(5))
			DECLARE @SuppplierCount INT
			
			INSERT INTO @SupplierID ( CarrierAccountID )
			SELECT DISTINCT CarrierAccountID
			FROM CarrierAccount ca
			JOIN CarrierProfile cp ON ca.ProfileID = @ProfileID
			WHERE AccountType = 1 OR AccountType = 2

			SET @SuppplierCount = (SELECT COUNT(*) FROM @SupplierID)
			
			DECLARE @CustomerID TABLE(CarrierAccountID VARCHAR(5))
			DECLARE @CustomerCount INT
			
			INSERT INTO @CustomerID ( CarrierAccountID )
			SELECT DISTINCT CarrierAccountID
			FROM CarrierAccount ca
			JOIN CarrierProfile cp ON ca.ProfileID = @ProfileID
			WHERE AccountType = 0 OR AccountType = 1

			SET @CustomerCount = (SELECT COUNT(*) FROM @CustomerID)

			IF (@SuppplierCount != 0)
				BEGIN
						;WITH PriceListCTE AS 
						(
						SELECT PriceListID
						FROM PriceList WITH(NOLOCK)
						WHERE --BeginEffectiveDate <= @Date AND
							  --CustomerID = 'SYS' AND 
							    SupplierID IN (SELECT CarrierAccountID FROM @SupplierID) AND 
							   (EndEffectiveDate IS NULL 
							 OR EndEffectiveDate > @Date) --AND 
	 						    --IsEffective = 'Y')
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
							  SupplierID IN (SELECT CarrierAccountID FROM @SupplierID) AND 
							 (EndEffectiveDate IS NULL OR EndEffectiveDate > @Date) -- AND 
							  --CustomerID = @CustomerID AND 
							  --IsEffective = 'Y'
						
					    -- Update Tariff	  
					    UPDATE Tariff
   					    SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					    WHERE SupplierID IN (SELECT CarrierAccountID FROM @SupplierID) AND 
						     (EndEffectiveDate IS NULL 
						   OR EndEffectiveDate > @Date)
						  
					    -- Update TODConsideration
					    UPDATE ToDConsideration
					    SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					    WHERE SupplierID IN (SELECT CarrierAccountID FROM @SupplierID) AND 
					    	 (EndEffectiveDate IS NULL 
						   OR EndEffectiveDate > @Date)
						  
					    -- Update Commission
					    UPDATE Commission
					    SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					    WHERE SupplierID IN (SELECT CarrierAccountID FROM @SupplierID) AND 
					         (EndEffectiveDate IS NULL 
						   OR EndEffectiveDate > @Date)

					    -- Update Route Block
 					    UPDATE RouteBlock
					    SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					    WHERE SupplierID IN (SELECT CarrierAccountID FROM @SupplierID) AND 
						     (EndEffectiveDate IS NULL 
					       OR EndEffectiveDate > @Date)

					    -- Update Special Request
					    UPDATE SpecialRequest
					    SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					    WHERE SupplierID IN (SELECT CarrierAccountID FROM @SupplierID) AND 
						  	 (EndEffectiveDate IS NULL 
						   OR EndEffectiveDate > @Date)

						-- Update Route Override
					    DECLARE @Patterns VARCHAR(8000) 
						SELECT @Patterns = COALESCE(@Patterns + '|', '') + CarrierAccountID 
						FROM @SupplierID

					    UPDATE RouteOverride
					    SET RouteOptions = dbo.ErasePatterns(RouteOptions,'|',@Patterns) 

					    -- Update Bilateral_Agreement
					    UPDATE Bilateral_Agreement
					    SET EndDate = (CASE WHEN BeginDate > @Date THEN BeginDate ELSE @Date END),
					        GraceDate = (CASE WHEN BeginDate > @Date THEN BeginDate ELSE @Date END)
					    WHERE CarrierID IN (SELECT CarrierAccountID FROM @SupplierID) AND 
					         (EndDate IS NULL 
						   OR EndDate > @Date)
						  
					    -- Update BQR Criterion
					    UPDATE BQR_Threshold
					    SET EndEffectiveDate = @Date
					    WHERE CriterionID IN (SELECT ID
											  FROM BQR_Criterion WITH(NOLOCK)
											  WHERE SupplierID IN (SELECT CarrierAccountID FROM @SupplierID) AND 
												   (EndEffectiveDate IS NULL 
												 OR EndEffectiveDate > @Date))
				        UPDATE BQR_Criterion
					    SET EndEffectiveDate = @Date --(CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					    WHERE SupplierID IN (SELECT CarrierAccountID FROM @SupplierID) AND 
					         (EndEffectiveDate IS NULL 
						   OR EndEffectiveDate > @Date)
						 
						-- Update Carrier Account 
						UPDATE CarrierAccount
						SET ActivationStatus = 0,
							RoutingStatus = 0,
							IsDeleted = 'Y'
						WHERE CarrierAccountID IN (SELECT CarrierAccountID FROM @SupplierID) 
				END
			IF (@CustomerCount != 0)
				BEGIN
						;WITH PriceListCTE AS 
						(
						SELECT PriceListID
						FROM PriceList WITH(NOLOCK)
						WHERE --BeginEffectiveDate <= @Date AND
							    CustomerID IN (SELECT CarrierAccountID FROM @CustomerID)  AND 
							    --SupplierID = 'SYS' AND 
							   (EndEffectiveDate IS NULL 
							 OR EndEffectiveDate > @Date) --AND 
	 						    --IsEffective = 'Y')
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
						
						-- Update PriceList
						UPDATE PriceList
						SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
						WHERE --BeginEffectiveDate <= @Date AND
							  CustomerID IN (SELECT CarrierAccountID FROM @CustomerID) AND 
							 (EndEffectiveDate IS NULL OR EndEffectiveDate > @Date) -- AND 
							  --SupplierID = @SupplierID AND 
							  --IsEffective = 'Y'
							  
						-- Update Tariff	  
					    UPDATE Tariff
   					    SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					    WHERE CustomerID IN (SELECT CarrierAccountID FROM @CustomerID) AND 
						     (EndEffectiveDate IS NULL 
						   OR EndEffectiveDate > @Date)
						  
					   -- Update TODConsideration
					   UPDATE ToDConsideration
					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE CustomerID IN (SELECT CarrierAccountID FROM @CustomerID) AND 
					    	(EndEffectiveDate IS NULL 
						  OR EndEffectiveDate > @Date)
						  
					   -- Update Commission
					   UPDATE Commission
					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE CustomerID IN (SELECT CarrierAccountID FROM @CustomerID) AND 
					        (EndEffectiveDate IS NULL 
						  OR EndEffectiveDate > @Date)

					   -- Update Route Block
 					   UPDATE RouteBlock
					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE CustomerID IN (SELECT CarrierAccountID FROM @CustomerID) AND 
						    (EndEffectiveDate IS NULL 
					      OR EndEffectiveDate > @Date)

					   -- Update Special Request
					   UPDATE SpecialRequest
					   SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					   WHERE CustomerID IN (SELECT CarrierAccountID FROM @CustomerID) AND 
							(EndEffectiveDate IS NULL 
						  OR EndEffectiveDate > @Date)

					  -- Update Route Override
					  UPDATE RouteOverride
					  SET EndEffectiveDate = (CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					  WHERE CustomerID IN (SELECT CarrierAccountID FROM @CustomerID) AND 
					       (EndEffectiveDate IS NULL 
						 OR EndEffectiveDate > @Date)
					
					  -- Update Bilateral_Agreement
					  UPDATE Bilateral_Agreement
					  SET EndDate = (CASE WHEN BeginDate > @Date THEN BeginDate ELSE @Date END),
					      GraceDate = (CASE WHEN BeginDate > @Date THEN BeginDate ELSE @Date END)
					  WHERE CarrierID IN (SELECT CarrierAccountID FROM @CustomerID) AND 
					       (EndDate IS NULL 
						 OR EndDate > @Date)
						  
				      -- Update BQR Criterion
				      UPDATE BQR_Threshold
					  SET EndEffectiveDate = @Date
					  WHERE CriterionID IN (SELECT ID
											FROM BQR_Criterion WITH(NOLOCK)
											WHERE CustomerID IN (SELECT CarrierAccountID FROM @CustomerID) AND 
												 (EndEffectiveDate IS NULL 
											   OR EndEffectiveDate > @Date))
				      UPDATE BQR_Criterion
					  SET EndEffectiveDate = @Date --(CASE WHEN BeginEffectiveDate > @Date THEN BeginEffectiveDate ELSE @Date END)
					  WHERE CustomerID IN (SELECT CarrierAccountID FROM @CustomerID) AND 
					       (EndEffectiveDate IS NULL 
						 OR EndEffectiveDate > @Date)
						  
						-- Update Carrier Account 
						UPDATE CarrierAccount
						SET ActivationStatus = 0,
							RoutingStatus = 0,
							IsDeleted = 'Y'
						WHERE CarrierAccountID IN (SELECT CarrierAccountID FROM @CustomerID)
						
						-- Update Carrier Profile
						UPDATE CarrierProfile
						SET IsDeleted = 'Y'
						WHERE ProfileID = @ProfileID
						
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