

CREATE PROCEDURE  [TOneWhS_SPL].[sp_ReceivedSupplierPricelist_GetReceivedPricelistById]
	@id int
AS
BEGIN
     SELECT 
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
	 FROM [TOneWhS_SPL].[ReceivedSupplierPricelist] WITH(NOLOCK) 
	 Where [ID] = @id
END