-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE bp.sp_BPInstance_SetCancellationRequestUserId
	@ID bigint,
	@AllowedStatusIds varchar(max),
	@CancellationRequestUserId int
AS
BEGIN
	
	DECLARE @AllowedStatusIdsTable TABLE (StatusId int)
	INSERT INTO @AllowedStatusIdsTable (StatusId)
	select Convert(int, ParsedString) from [bp].[ParseStringList](@AllowedStatusIds)

    Update [bp].[BPInstance]
	SET [CancellationRequestUserId] = @CancellationRequestUserId
	WHERE ID = @ID 
			AND [ExecutionStatus] IN (SELECT StatusId FROM @AllowedStatusIdsTable)
			AND CancellationRequestUserId IS NULL
		
END