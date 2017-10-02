-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE bp.sp_BPInstance_GetInstanceIdsHavingChildren
	@Statuses varchar(max)
AS
BEGIN
	DECLARE @StatusesTable TABLE ([Status] int)
	INSERT INTO @StatusesTable ([Status])
	SELECT Convert(int, ParsedString) FROM bp.[ParseStringList](@Statuses)

    -- Insert statements for procedure here
	SELECT [ParentID]
	FROM bp.[BPInstance] bp WITH(NOLOCK)
	JOIN @StatusesTable statuses ON bp.ExecutionStatus = statuses.[Status]
	WHERE bp.[ParentID] IS NOT NULL
	GROUP by bp.[ParentID]
END