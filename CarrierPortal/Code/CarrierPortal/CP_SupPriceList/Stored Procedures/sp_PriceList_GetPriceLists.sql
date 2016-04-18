create PROCEDURE [CP_SupPriceList].[sp_PriceList_GetPriceLists]

AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT [ID]
      ,[UserID]
      ,[FileID]
      ,[PriceListType]
      ,[Status]
      ,[Result]
      ,[UploadInformation]
      ,[PriceListProgress]
      ,[CreatedTime]
      ,[EffectiveOnDate]
      ,[timestamp]
      ,[ResultRetryCount]
      ,[UploadRetryCount]
      ,AlertMessage
      ,CustomerID
      ,AlertFileID
      ,CarrierAccountID
	,[CarrierAccountName]
  FROM [CP_SupPriceList].[PriceList]
END