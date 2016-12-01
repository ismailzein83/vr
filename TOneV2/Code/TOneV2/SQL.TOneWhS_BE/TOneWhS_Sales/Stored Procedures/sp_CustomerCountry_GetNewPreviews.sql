-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_Sales.sp_CustomerCountry_GetNewPreviews
	@ProcessInstanceId bigint
AS
BEGIN
	select [ID], [BED], [EED]
	from [TOneWhS_Sales].[RP_CustomerCountry_NewPreview]
	where ProcessInstanceID = @ProcessInstanceId
END