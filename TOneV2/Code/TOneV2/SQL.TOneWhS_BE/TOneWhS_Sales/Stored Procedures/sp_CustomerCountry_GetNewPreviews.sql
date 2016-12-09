-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_CustomerCountry_GetNewPreviews]
	@ProcessInstanceID_IN bigint
AS
BEGIN
DECLARE @ProcessInstanceId INT

SELECT @ProcessInstanceId  = @ProcessInstanceId_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	

	select [ID], [BED], [EED]
	from [TOneWhS_Sales].[RP_CustomerCountry_NewPreview]
	where ProcessInstanceID = @ProcessInstanceId
	
	SET NOCOUNT OFF
END