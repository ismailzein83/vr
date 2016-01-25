CREATE PROCEDURE [CP_SupPriceList].[sp_PriceList_GetRequestedPriceList]
@PriceListStatusIDs varchar(max)
AS
BEGIN
	DECLARE @PriceListStatusIDsTable TABLE (PriceListStatusID int)
	INSERT INTO @PriceListStatusIDsTable (PriceListStatusID)
	select Convert(int, ParsedString) from [CP_SupPriceList].[ParseStringList](@PriceListStatusIDs)

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
  FROM [CP_SupPriceList].[PriceList]
  WHERE (@PriceListStatusIDs  is null or [CP_SupPriceList].[PriceList].[Status] in (select PriceListStatusID from @PriceListStatusIDsTable))	
END