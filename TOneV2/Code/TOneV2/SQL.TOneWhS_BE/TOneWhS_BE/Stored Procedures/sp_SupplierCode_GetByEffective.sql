CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierCode_GetByEffective]
       @From_FromOut DateTime,
       @To_FromOut DateTime
AS
BEGIN
       DECLARE @From DateTime
       DECLARE @To DateTime

       SELECT @From = @From_FromOut
       SELECT @To = @To_FromOut

       SELECT  sc.[ID],sc.Code,sc.ZoneID,sc.BED,sc.EED,sc.CodeGroupID,sc.SourceID
       FROM   [TOneWhS_BE].SupplierCode sc WITH(NOLOCK) 
       Where  sc.BED<= @To
			  AND (sc.EED is null or sc.EED >@From )
END