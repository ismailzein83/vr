-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [LCR].[sp_Code_GetUpdatedSuppliers]
	@timestamp timestamp
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT MAX(c.timestamp) LastTimestamp
	FROM Code c WITH (NOLOCK)
	
    
	SELECT distinct z.SupplierID
	FROM Code c WITH (NOLOCK)
	JOIN Zone z WITH (NOLOCK) ON c.ZoneID = z.ZoneID
	WHERE c.timestamp > @timestamp
END