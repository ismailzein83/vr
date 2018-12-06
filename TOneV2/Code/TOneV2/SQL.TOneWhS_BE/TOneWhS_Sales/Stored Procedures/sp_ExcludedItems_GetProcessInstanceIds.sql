-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_ExcludedItems_GetProcessInstanceIds]
	@processInstanceIds nvarchar(max)
AS
BEGIN

DECLARE @ProcessInstanceIdsTable TABLE (ProcessInstanceId bigint)
INSERT INTO @ProcessInstanceIdsTable (ProcessInstanceId)
select Convert(bigint, ParsedString) from [TOneWhS_BE].[ParseStringList](@processInstanceIds)

	SELECT ProcessInstanceId
	FROM TOneWhS_Sales.RP_ExcludedItems
	WHERE  (@processInstanceIds  is null or ProcessInstanceId in (select ProcessInstanceId from @ProcessInstanceIdsTable))
END