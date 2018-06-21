-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CustomerRecurringChargesType_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Name
	FROM TOneWhS_BE.CustomerRecurringChargesType WITH(NOLOCK) 
	
	SET NOCOUNT OFF;
END