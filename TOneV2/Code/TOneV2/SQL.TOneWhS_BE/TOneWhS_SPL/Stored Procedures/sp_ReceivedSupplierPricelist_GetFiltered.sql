

CREATE PROCEDURE  [TOneWhS_SPL].[sp_ReceivedSupplierPricelist_GetFiltered]
	@SupplierIds varchar(max),
	@Status varchar(max),
	@Top int
AS
BEGIN
	DECLARE @SupplierIdsTable TABLE (SupplierId int)
	INSERT INTO @SupplierIdsTable (SupplierId)
	select Convert( int, ParsedString) from [TOneWhS_BE].[ParseStringList](@SupplierIds)

	DECLARE @StatusTable TABLE ([Status]  int)
	INSERT INTO @StatusTable ([Status])
	select Convert( int, ParsedString) from [TOneWhS_BE].[ParseStringList](@Status)
          
     SELECT Top (@Top) 
		[ID]
      ,[SupplierID]
      ,[FileID]
      ,[ReceivedDate]
      ,[PricelistType]
      ,[Status]
      ,[PricelistID]
      ,[ProcessInstanceId]
      ,[StartProcessingDate]
	  ,[ErrorDetails]
	  ,[SentToSupplier]
	 FROM [TOneWhS_SPL].[ReceivedSupplierPricelist] WITH(NOLOCK) 
	 Where  (@SupplierIds  is null or SupplierID in (select SupplierId from @SupplierIdsTable)) 
	 and 	 (@Status  is null or [Status] in (select [Status] from @StatusTable))
	 ORDER BY [ReceivedDate] DESC
END