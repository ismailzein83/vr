-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_Sales.sp_CustomerCountry_GetChangedPreviews
	@ProcessInstanceID bigint
AS
BEGIN
	select [ID], [EED]
	from TOneWhS_Sales.RP_CustomerCountry_ChangedPreview
	where ProcessInstanceID = @ProcessInstanceID
END