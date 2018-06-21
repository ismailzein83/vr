-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRecurringChargesType_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Name
	FROM TOneWhS_BE.SupplierRecurringChargesType WITH(NOLOCK) 
	
	SET NOCOUNT OFF;
END