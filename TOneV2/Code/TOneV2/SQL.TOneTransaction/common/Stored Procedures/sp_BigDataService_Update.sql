-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_BigDataService_Update]
	@BigDataServiceId bigint,
	@TotalCachedRecordsCount bigint,
	@CachedObjectIds varchar(max)
AS
BEGIN
	UPDATE common.BigDataService
	SET TotalCachedRecordsCount = @TotalCachedRecordsCount,
		CachedObjectIds = @CachedObjectIds
	WHERE ID = @BigDataServiceId
END