-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_CustomerCountry_GetChangedPreviews]
	@ProcessInstanceID_IN bigint,
	@CustomerIds nvarchar(max)
AS
BEGIN
DECLARE @CustomerIDsTable TABLE (CustomerID int)
INSERT INTO @CustomerIDsTable (CustomerID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CustomerIds)

DECLARE @ProcessInstanceId INT

SELECT @ProcessInstanceId  = @ProcessInstanceId_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	

	select	[ID], [CustomerID], [EED]
	from	TOneWhS_Sales.RP_CustomerCountry_ChangedPreview CC WITH(NOLOCK)
	where	ProcessInstanceID = @ProcessInstanceID
	and ((@CustomerIds is null) or (CC.CustomerID  in (select CustomerID  from @CustomerIDsTable)))
	
	SET NOCOUNT OFF
END