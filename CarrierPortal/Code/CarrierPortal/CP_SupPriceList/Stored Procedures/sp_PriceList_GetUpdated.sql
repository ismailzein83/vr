
CREATE  PROCEDURE [CP_SupPriceList].[sp_PriceList_GetUpdated]
	@TimestampAfter timestamp,
	@NbOfRows INT,
	@UserId INT
AS
BEGIN
	
	IF (@TimestampAfter IS NULL)
		SELECT @TimestampAfter = MIN([timestamp])
		FROM (SELECT TOP (@NbOfRows) [timestamp] FROM [CP_SupPriceList].[PriceList] WHERE UserID = @UserId ORDER BY ID DESC) Lastpricelist
	
	SELECT   *
		  
	INTO #temp_table
	FROM [CP_SupPriceList].[PriceList] 
	WHERE 
	 UserID = @UserId AND
	([timestamp] >= @TimestampAfter) --ONLY Updated records
	
	
	SELECT * FROM #temp_table
	ORDER BY ID DESC
	
	IF((SELECT COUNT(*) FROM #temp_table) = 0)
		SELECT @TimestampAfter AS MaxTimestamp
	ELSE
		SELECT MAX([timestamp]) MaxTimestamp FROM #temp_table
	
END