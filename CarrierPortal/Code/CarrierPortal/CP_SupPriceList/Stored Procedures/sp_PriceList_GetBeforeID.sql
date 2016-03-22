CREATE PROCEDURE [CP_SupPriceList].[sp_PriceList_GetBeforeID]
	@LessThanID BIGINT,
	@NbOfRows INT,
	@UserId INT
AS
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
	,[CarrierAccountName]
  FROM [CP_SupPriceList].[PriceList]
	WHERE ID < @LessThanID AND  UserID = @UserId
	ORDER BY ID DESC
END