
CREATE  PROCEDURE [CP_SupPriceList].[sp_PriceList_GetUpdated]
	@TimestampAfter timestamp,
	@NbOfRows INT,
	@UserId INT
AS
BEGIN
			
	IF 	@TimestampAfter IS NULL
	BEGIN
		SELECT TOP(@NbOfRows) 
	       [ID] 
		  ,[UserID]
		  ,[CarrierAccountID]
		  ,[CustomerID]
		  ,[FileID]
		  ,[PriceListType]
		  ,[Status]
		  ,[Result]
		  ,[UploadInformation]
		  ,[PriceListProgress]
		  ,[EffectiveOnDate]
		  ,[ResultRetryCount]
		  ,[UploadRetryCount]
		  ,[AlertMessage]
		  ,[AlertFileID]
		  ,[CreatedTime]
		  ,[timestamp]
		INTO #temp_table
		FROM [CP_SupPriceList].[PriceList] 
		WHERE 
		 UserID = @UserId
		ORDER BY ID DESC
		
		SELECT * FROM #temp_table
	
		SELECT MAX([timestamp]) MaxTimestamp FROM #temp_table
	END
	ELSE
	BEGIN
		SELECT TOP(@NbOfRows) 
		   [ID] 
		  ,[UserID]
		  ,[CarrierAccountID]
		  ,[CustomerID]
		  ,[FileID]
		  ,[PriceListType]
		  ,[Status]
		  ,[Result]
		  ,[UploadInformation]
		  ,[PriceListProgress]
		  ,[EffectiveOnDate]
		  ,[ResultRetryCount]
		  ,[UploadRetryCount]
		  ,[AlertMessage]
		  ,[AlertFileID]
		  ,[CreatedTime]
		  ,[timestamp]
		INTO #temp2_table
		FROM [CP_SupPriceList].[PriceList] 
		WHERE 
		 UserID = @UserId AND
		([timestamp] > @TimestampAfter) --ONLY Updated records
		ORDER BY [timestamp]
		
		SELECT * FROM #temp2_table
	
		SELECT MAX([timestamp]) MaxTimestamp FROM #temp2_table
	END	
END