-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CustomerZone_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, CustomerID, Details, BED
	FROM TOneWhS_BE.CustomerZone WITH(NOLOCK) 
	
	SET NOCOUNT OFF;
END