﻿CREATE PROCEDURE [bp].[sp_BPInstance_GetFiltered]
	@ArrDefinitionID nvarchar(max),
	@ArrStatus nvarchar(max),
	@EntityID nvarchar(50),
	@DateFrom dateTime,
	@DateTo dateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
SET NOCOUNT ON;

BEGIN	
	SELECT	[ID],[Title],[ParentID],[DefinitionID],[WorkflowInstanceID],[InputArgument], [CompletionNotifier],[ExecutionStatus],
			[LastMessage],[CreatedTime],[StatusUpdatedTime],[InitiatorUserId],EntityID,[ViewRequiredPermissionSetId]

	FROM	bp.[BPInstance] as bps WITH(NOLOCK)
	WHERE	(@EntityID is null OR EntityID = @EntityID) 
			and (@ArrStatus is NULL or bps.ExecutionStatus in (SELECT ParsedString FROM ParseStringList(@ArrStatus) ) ) 
			and (@ArrDefinitionID is NULL or  bps.DefinitionID in (SELECT ParsedString FROM ParseStringList(@ArrDefinitionID) ) ) 
			and bps.CreatedTime >=  @DateFrom 
			and (@DateTo is NULL or bps.CreatedTime < @DateTo)
	ORDER BY CreatedTime DESC
	END
END