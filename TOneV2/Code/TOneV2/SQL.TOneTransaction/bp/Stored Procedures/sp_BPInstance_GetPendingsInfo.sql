CREATE PROCEDURE [bp].[sp_BPInstance_GetPendingsInfo]
	@Statuses varchar(max),
	@MaxNbOfInstances int
AS
BEGIN
	DECLARE @StatusesTable TABLE ([Status] int)
	INSERT INTO @StatusesTable ([Status])
	SELECT Convert(int, ParsedString) FROM bp.[ParseStringList](@Statuses)

    SELECT top(@MaxNbOfInstances) bp.[ID]
		,ParentID
	  ,DefinitionID
	  ,ExecutionStatus
	  ,ServiceInstanceID
	FROM bp.[BPInstance] bp WITH(NOLOCK)
	JOIN @StatusesTable statuses ON bp.ExecutionStatus = statuses.[Status]
	ORDER BY ID
END