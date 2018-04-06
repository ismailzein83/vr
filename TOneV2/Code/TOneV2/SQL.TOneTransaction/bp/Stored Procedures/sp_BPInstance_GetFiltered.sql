CREATE PROCEDURE [bp].[sp_BPInstance_GetFiltered]
	@ArrDefinitionID nvarchar(max),
	@ArrStatus nvarchar(max),
	@EntityID nvarchar(50),
	@DateFrom dateTime,
	@DateTo dateTime,
	@ViewRequiredPermissionSetIds varchar(max),
	@Top int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
		
SET NOCOUNT ON;

BEGIN	
	DECLARE @ViewRequiredPermissionSetTable TABLE (ViewRequiredPermissionSetId int)
	INSERT INTO @ViewRequiredPermissionSetTable (ViewRequiredPermissionSetId)
	select Convert(int, ParsedString) from [bp].[ParseStringList](@ViewRequiredPermissionSetIds)

	SELECT	TOP(@Top)[ID],[Title],[ParentID],[DefinitionID],[WorkflowInstanceID],[InputArgument], [CompletionNotifier],[ExecutionStatus], AssignmentStatus,
			[LastMessage],[CreatedTime],[StatusUpdatedTime],[InitiatorUserId],EntityID,[ViewRequiredPermissionSetId], [ServiceInstanceID], TaskId, CancellationRequestUserId

	FROM	bp.[BPInstance] as bps WITH(NOLOCK)
	WHERE	(@EntityID is null OR EntityID = @EntityID) 
			and (@ArrStatus is NULL or bps.ExecutionStatus in (SELECT ParsedString FROM ParseStringList(@ArrStatus) ) ) 
			and (@ArrDefinitionID is NULL or  bps.DefinitionID in (SELECT ParsedString FROM ParseStringList(@ArrDefinitionID) ) ) 			
			and (ViewRequiredPermissionSetId is null or ViewRequiredPermissionSetId in (select ViewRequiredPermissionSetId from @ViewRequiredPermissionSetTable))
			and bps.CreatedTime >=  @DateFrom 
			and (@DateTo is NULL or bps.CreatedTime < @DateTo)
	ORDER BY ID DESC
	END
END