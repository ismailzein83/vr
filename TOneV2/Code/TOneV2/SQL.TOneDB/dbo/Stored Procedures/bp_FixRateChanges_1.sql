CREATE Procedure [dbo].[bp_FixRateChanges]
	@RateID bigint = NULL,
	@PriceListID bigint = NULL,
	@AllRates char(1) = 'N'
AS 
	SET NOCOUNT ON
	
	IF @RateID IS NOT NULL
		UPDATE Rate SET Change = 
			CASE 			
				WHEN ISNULL(dbo.GetPreviousRateValue(Rate.RateID), Rate) > Rate.Rate THEN -1
				WHEN ISNULL(dbo.GetPreviousRateValue(Rate.RateID), Rate) < Rate.Rate THEN 1
				ELSE 0
			END
		WHERE RateID = @RateID
	ELSE	
	BEGIN
		IF @PriceListID IS NOT NULL
		BEGIN
			DECLARE curRates CURSOR LOCAL FORWARD_ONLY
			FOR SELECT RateID FROM Rate WHERE PriceListID = @PriceListID
			
			OPEN curRates
			FETCH NEXT FROM curRates INTO @RateID
			WHILE @@FETCH_STATUS = 0
			BEGIN
				EXEC bp_FixRateChanges @RateID = @RateID
				FETCH NEXT FROM curRates INTO @RateID
			END		
			CLOSE curRates
			DEALLOCATE curRates
			
			EXEC bp_FixErroneousEffectiveRates 
		END
		ELSE IF @AllRates = 'Y'
		BEGIN
			DECLARE curRates CURSOR LOCAL FORWARD_ONLY
			FOR SELECT RateID FROM Rate ORDER BY RateID
			
			OPEN curRates
			FETCH NEXT FROM curRates INTO @RateID
			WHILE @@FETCH_STATUS = 0
			BEGIN
				EXEC bp_FixRateChanges @RateID = @RateID
				FETCH NEXT FROM curRates INTO @RateID
			END		
			CLOSE curRates
			DEALLOCATE curRates
			
			EXEC bp_FixErroneousEffectiveRates
		END
	END