
CREATE PROCEDURE [dbo].[bp_GetSaleCodeGaps] (@IsDebug CHAR(1) = 'N')
AS
BEGIN

	DECLARE @today datetime
	SET @today = getdate()
	DECLARE @CarrierAccountID varchar(10)

	SET NOCOUNT ON

	SELECT * INTO #CustomersCodeGaps FROM dbo.GetSaleCodeGaps('--NONE--', @today) 

	DECLARE my_cursor CURSOR LOCAL FAST_FORWARD READ_ONLY 
		FOR SELECT CarrierAccountID FROM CarrierAccount ca WHERE ca.IsDeleted = 'N' 
	OPEN my_cursor

		FETCH FROM my_cursor INTO @CarrierAccountID
		
		IF @IsDebug = 'Y' PRINT 'Started *** ' + CONVERT(VARCHAR(20), GETDATE(), 121)

		WHILE @@FETCH_STATUS = 0
		BEGIN
			
			IF @IsDebug = 'Y' PRINT ('Codes For: ' + @CarrierAccountID + ' *** ' + RIGHT(CONVERT(VARCHAR(20), GETDATE(), 121), 8))
			
			INSERT INTO #CustomersCodeGaps
				SELECT * FROM dbo.GetSaleCodeGaps(@CarrierAccountID, @today) 
			
			FETCH FROM my_cursor INTO @CarrierAccountID
			IF @IsDebug = 'Y' PRINT ('Done *** ' + RIGHT(CONVERT(VARCHAR(20), GETDATE(), 121), 8))

		END

	CLOSE my_cursor
	DEALLOCATE my_cursor 

	SELECT * FROM #CustomersCodeGaps

	DROP TABLE #CustomersCodeGaps
	
END