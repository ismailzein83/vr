CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierCode_GetByEffective]
       @From DateTime,
       @To DateTime
AS
BEGIN
       DECLARE @From_local DateTime = @From
       DECLARE @To_local DateTime = @To

       SELECT  sc.[ID],sc.Code,sc.ZoneID,sc.BED,sc.EED,sc.CodeGroupID,sc.SourceID
       FROM   [TOneWhS_BE].SupplierCode sc WITH(NOLOCK) 
       Where  sc.BED <= @To_local
			  AND (sc.EED is null or sc.EED > @From_local )
END